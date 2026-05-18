using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using S7.Net;
using WebBackend.Dao;
using WebBackend.DTO;
using WebBackend.Util;

namespace WebBackend.Service
{
    /// <summary>
    /// 全自动模式检测服务（轮询模式）
    /// </summary>
    public class AutoOrManDetectService : BackgroundService
    {
        private readonly RobotStatus _robotStatus;
        private readonly RobotService _robotService;
        private readonly NewTraceService _newTraceService;
        private readonly IApplicationData _applicationData;
        private readonly PlcService _plcService;
        private readonly ProcessCardService _processCardService;
        private readonly ILogger<AutoOrManDetectService> _logger;
        private readonly IConfiguration _configuration;
        private readonly WorkOrderNumberDao _workOrderNumberDao;
        private readonly string _localErpErdFolder = @"data";
        private volatile bool _inspectionStarted;
        private readonly object _robotStatusLock = new();

        public AutoOrManDetectService(
            SignalWatchService signalWatchService,
            RobotStatus robotStatus,
            RobotService robotService,
            NewTraceService newTraceService,
            IApplicationData applicationData,
            PlcService plcService,
            ProcessCardService processCardService,
            ILogger<AutoOrManDetectService> logger,
            WorkOrderNumberDao workOrderNumberDao,
            IConfiguration configuration)
        {
            _robotStatus = robotStatus;
            _robotService = robotService;
            _newTraceService = newTraceService;
            _applicationData = applicationData;
            _plcService = plcService;
            _processCardService = processCardService;
            _logger = logger;
            _configuration = configuration;
            _workOrderNumberDao = workOrderNumberDao;
        }

        /// <summary>
        /// 自动模式主循环，使用轮询方式检测信号
        /// </summary>
        protected override async System.Threading.Tasks.Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("AutoDetectService启动（轮询模式）");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (_applicationData.ModeState.CurrentMode == ControlMode.Automatic && !_inspectionStarted)
                    {
                        // ========== 1. 周期开始 & 初始握手 ==========
                        InitializeAndResetSignals();

                        if (!await CheckRobotReady(stoppingToken)) continue;

                        if (!await WaitForWorkpieceArrival(stoppingToken)) continue;

                        if (!await SyncWorkpieceData(stoppingToken)) continue;

                        CurrentRobotStatus = RobotStatus.Site1Ready;

                        // 向辊道发送一号检测位检测就绪信号

                        _plcService.WriteBit(DataType.DataBlock, 6, 98, 2, true); // bReadyforchecking.1
                        Console.WriteLine("已向辊道发送一号检测位检测就绪信号 (bReadyForChecking_1)！");

                        

                        // ========== 2. 等待1号位检测就绪 ==========
                        Console.WriteLine("等待一号检测位就绪信号 (32.3)...");
                        while (!_plcService.ReadBit(DataType.DataBlock, 6, 32, 3)) // 轮询 bPosition1ReadyForChecking
                        {
                            await System.Threading.Tasks.Task.Delay(100, stoppingToken);
                        }
                        // =========================================================================
                        // [新增代码开始] 安全互锁：检查 Galama 机器人是否在 1# 位置 (DB6.DBX33.0)
                        // =========================================================================
                        if (_plcService.ReadBit(DataType.DataBlock, 6, 33, 0))
                        {
                            Console.WriteLine("警告：检测到 Galama 机器人在 1# 位置 (6.33.0)，暂停等待其离开...");

                            // 如果信号为 True，一直循环等待，直到信号变为 False
                            while (_plcService.ReadBit(DataType.DataBlock, 6, 33, 0))
                            {
                                await System.Threading.Tasks.Task.Delay(100, stoppingToken); // 每100ms轮询一次
                            }

                            Console.WriteLine("Galama 机器人已离开 1# 位置，安全互锁解除，继续流程。");
                        }
                        // =========================================================================
                        // [新增代码结束]
                        // =========================================================================
                        _inspectionStarted = true; // 标记本轮检测已正式开始
                        Console.WriteLine("检测到一号检测位就绪信号，开始执行一号位检测流程...");
                        await HandleSite1InspectionStartAuto();

                        // ========== 3. 根据1号位结果，决定是否进入2号位 ==========
                        if (CurrentRobotStatus == RobotStatus.Site2Ready)
                        {
                            Console.WriteLine("一号位检测合格，流程转向二号位。");
                            Console.WriteLine("等待二号检测位就绪信号 (32.5)...");
                            while (!_plcService.ReadBit(DataType.DataBlock, 6, 32, 5)) // 轮询 bPosition2ReadyForChecking
                            {
                                await System.Threading.Tasks.Task.Delay(100, stoppingToken);
                            }
                            Console.WriteLine("检测到二号检测位就绪信号，开始执行二号位检测流程...");
                            await HandleSite2InspectionStartAuto(); // 此方法内部会调用FinishInspection来结束周期
                            await System.Threading.Tasks.Task.Delay(2000, stoppingToken);
                        }
                        else
                        {
                            // 如果1号位检测结果为不合格或有瑕疵，HandleSite1InspectionStartAuto 内部已调用 FinishInspection
                            // 这会重置 _inspectionStarted，使主循环可以开始全新的一轮
                            Console.WriteLine("一号位检测未通过或流程分支结束，准备开始新的检测周期。");
                        }
                    }
                    else
                    {
                        // 自动模式关闭或正在检测中，短暂等待
                        await System.Threading.Tasks.Task.Delay(1000, stoppingToken);
                    }
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("服务已停止。");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"服务主循环发生严重错误: {ex}");
                    FinishInspection(); // 发生未知异常时，尝试重置状态
                    await System.Threading.Tasks.Task.Delay(5000, stoppingToken); // 发生错误后等待一段时间
                }
            }
        }

        #region Helper Methods for ExecuteAsync

        private void InitializeAndResetSignals()
        {
            _plcService.WriteBit(DataType.DataBlock, 6, 98, 1, false); // bDataOK
            _plcService.WriteBit(DataType.DataBlock, 6, 98, 2, false); // bReadyForChecking_1
            _plcService.WriteBit(DataType.DataBlock, 6, 98, 3, false); // bRobotinside_1
            _plcService.WriteBit(DataType.DataBlock, 6, 98, 5, false); // bFinish_1
            _plcService.WriteBit(DataType.DataBlock, 6, 98, 6, false); // bQualityPassed_1
            _plcService.WriteBit(DataType.DataBlock, 6, 98, 7, false); // bQualityRecheck_1
            _plcService.WriteBit(DataType.DataBlock, 6, 99, 0, false); // bQualityFailed_1
            _plcService.WriteBit(DataType.DataBlock, 6, 99, 1, false); // bReadyForChecking_2
            _plcService.WriteBit(DataType.DataBlock, 6, 99, 2, false); // bRobotinside_2
            _plcService.WriteBit(DataType.DataBlock, 6, 99, 4, false); // bFinish_2
            _plcService.WriteBit(DataType.DataBlock, 6, 99, 5, false); // bQualityPassed_2
            _plcService.WriteBit(DataType.DataBlock, 6, 99, 6, false); // bQualityRecheck_2
            _plcService.WriteBit(DataType.DataBlock, 6, 99, 7, false); // bQualityFailed_2
            _plcService.WriteBit(DataType.DataBlock, 6, 98, 4, true);  // bRobotoutside_1
            _plcService.WriteBit(DataType.DataBlock, 6, 99, 3, true);  // bRobotoutside_2
            _plcService.WriteBit(DataType.DataBlock, 6, 100, 0, false); //复位旋转请求
            Console.WriteLine("新周期开始，信号已全部复位。");
        }

        private async Task<bool> CheckRobotReady(CancellationToken stoppingToken)
        {
            if (!_robotService.IsReady())
            {
                Console.WriteLine("机器人未启动或未就绪，自动流程暂停。请在前端点击“启动机器人”。");
                await System.Threading.Tasks.Task.Delay(5000, stoppingToken);
                return false;
            }
            return true;
        }

        private async Task<bool> WaitForWorkpieceArrival(CancellationToken stoppingToken)
        {
            Console.WriteLine("机器人已就绪，等待辊道发bPosition1FreePasing信号...");
            while (!_plcService.ReadBit(DataType.DataBlock, 6, 32, 2))
            {
                await System.Threading.Tasks.Task.Delay(100, stoppingToken);
            }
            Console.WriteLine("读到辊道发bPosition1FreePasing信号。");
            return true;
        }

        private async Task<bool> SyncWorkpieceData(CancellationToken stoppingToken)
        {
            const int maxRetryCount = 20;
            int retryCount = 0;
            while (retryCount < maxRetryCount)
            {
                if (SyncWorkOrderFromRollerToMe())
                {
                    _plcService.WriteBit(DataType.DataBlock, 6, 98, 1, true); // bDataOK
                    Console.WriteLine("工件号同步成功，等待辊道反馈bDataOk信号...");
                    while (!_plcService.ReadBit(DataType.DataBlock, 6, 32, 1))
                    {
                        await System.Threading.Tasks.Task.Delay(100, stoppingToken);
                    }
                    Console.WriteLine("收到辊道bDataOk信号，握手完成。");
                    return true;
                }
                retryCount++;
                Console.WriteLine($"工件号同步失败，第{retryCount}次重试，1秒后再试...");
                await System.Threading.Tasks.Task.Delay(1000, stoppingToken);
            }
            Console.WriteLine("多次同步工件号失败，跳到下一轮循环。");
            return false;
        }

        #endregion


        public int CurrentRobotStatus
        {
            get
            {
                lock (_robotStatusLock)
                {
                    return _robotStatus.CurrentState;// 直接返回 int 类型状态
                }
            }
            set
            {
                lock (_robotStatusLock)
                {
                    _robotStatus.CurrentState = value;// 设置 int 类型状态
                }
            }
        }

        /// <summary>
        /// 一轮检测结束之后的标志
        /// </summary>
        private void FinishInspection()
        {
            _inspectionStarted = false;
            Console.WriteLine("已重置_inspectionStarted，准备接收下一批工件！");
            // 在此处调用清理方法，确保每次流程结束时都会执行
            CleanupAfterCycle();
        }
        /// <summary>
        /// 清零BeginTime
        /// </summary>
        private void CleanupAfterCycle()
        {
            _applicationData.BeginTime = 0;
            Console.WriteLine("全自动模式检测流程已全部完成，BeginTime 已重置");
        }
        /// <summary>
        /// 从辊道中读取工作令号，判读是否为全0，若不是，且与数据库的工作令号一致，则写入我的plc中
        /// 并读取并写入辊道发来的工件详情
        /// </summary>
        /// <returns></returns>
        public bool SyncWorkOrderFromRollerToMe()
        {
            // === 第1步: 读取和验证工作令号 ===
            var rollerWorkOrder = _plcService.ReadWorkOrderNumber(6, 0); // 从辊道PLC读取
                                                                         //从plc中读取partname
            //var patrName = _plcService.ReadPartName(6, 0); // 从辊道PLC读取


            Console.WriteLine("读取辊道给的工作令号成功");
            //必须所有字段全不为0
            bool isAllZero = rollerWorkOrder.PartName == 0
            && rollerWorkOrder.ProductUnit == 0
            && rollerWorkOrder.OrderDate == 0
            && rollerWorkOrder.CustomerCode == 0
            && rollerWorkOrder.OrderNumber == 0
            && rollerWorkOrder.OrderStartNumber == 0;
            //&& rollerWorkOrder.PartsNumber == 0;
            if (isAllZero)
            {
                Console.WriteLine("写入工作令号失败，工作令号为全0！");
                return false;
            }
            // 生成searchable_number 方便与数据库中的工作令号对比
            //TODO：这里生成的searchableNumber不需要包括partsnumber
            // === 第2步: 数据库验证（新逻辑） ===

            // 2.1 生成searchable_number 方便与数据库中的工作令号对比
            long searchableNumber = _plcService.GetFullWorkOrderNumber(rollerWorkOrder);
            // 2.2 查库，优先检查工作令号
            bool workOrderExists = _workOrderNumberDao.ExistsWorkOrderInDb(searchableNumber);

            if (!workOrderExists)
            {
                // 2.3 如果工作令号不存在，则检查 part_name 是否存在
                Console.WriteLine($"数据库不存在该工件令号：{searchableNumber}！正在检查 PartName...");

                int partNameFromPlc = rollerWorkOrder.PartName; // 从已读取的对象中获取PartName
                bool partNameExists = _workOrderNumberDao.ExistsPartNameInProcessCards(partNameFromPlc);

                if (!partNameExists)
                {
                    // 2.4 如果工作令号 和 part_name 都不存在，则同步失败
                    Console.WriteLine($"数据库也不存在该 PartName：{partNameFromPlc}！同步失败。");
                    return false;
                }

                // 2.5 如果 part_name 存在，则允许继续
                Console.WriteLine($"PartName: {partNameFromPlc} 存在于 process_cards。允许继续同步...");
            }
            // else 
            // {
            //    // 工作令号存在，直接继续，不需要额外判断 part_name
            // }
            // === 第3步: 如果验证通过（工作令号存在 或 PartName存在），则读取工件详情 ===
            // 详情数据从地址 12 开始
            var rollerPartDetails = _plcService.ReadPartDetails(6, 12); // 从辊道PLC读取 (DB6, 地址12)
            Console.WriteLine("读取辊道给的工件详情成功");

            // === 第4步: 将所有数据写入到我的PLC ===
            // 写入工作令号到我的PLC (DB6，地址66)
            _plcService.WriteWorkOrderNumber(6, 66, rollerWorkOrder);
            Console.WriteLine("写入工作令号到本地PLC成功！");
            // 写入工件详情到我的PLC (DB6，地址78)
            _plcService.WritePartDetails(6, 78, rollerPartDetails);
            Console.WriteLine("写入工件详情到本地PLC成功！");
            Console.WriteLine("全部工作令号数据同步成功！");
            return true;


        }
        public enum DetectionSite
        {
            Site1,
            Site2
        }

        /// <summary>
        /// 检测结果
        /// </summary>
        public enum WorkpieceQuality
        {
            /// <summary>
            /// 合格
            /// </summary>
            Passed = 0,
            /// <summary>
            /// 有瑕疵
            /// </summary>
            Recheck = 1,
            /// <summary>
            /// 不合格
            /// </summary>
            Failed = 2
        }
        private static readonly Random _random = new Random();
        /// <summary>
        /// 检测结果接口 
        /// </summary>
        /// <param name="workOrderNumber"></param>
        /// <param name="productIndex"></param>
        /// <param name="detectionPosition"></param>
        /// <returns></returns>
        private async Task<WorkpieceQuality> GetWorkpieceQualityAsync(string workOrderNumber, int productIndex, string detectionPosition)
        {
            // 检测位后缀使用随机 0、1、2
            int randomResult = _random.Next(0, 3);
            string randomDetectionPosition = detectionPosition.Contains("_")
                ? detectionPosition.Substring(0, detectionPosition.IndexOf('_')) + "_" + randomResult
                : detectionPosition + "_" + randomResult;
            var httpClient = new HttpClient();
            var requestUrl = "http://192.168.1.100:8000/view/workpieceQualityQuery";
            var req = new
            {
                work_order_number = workOrderNumber,
                product_index = productIndex,
                detection_position = detectionPosition
            };
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(req), System.Text.Encoding.UTF8, "application/json");
            var resp = await httpClient.PostAsync(requestUrl, content);
            resp.EnsureSuccessStatusCode();
            var respStr = await resp.Content.ReadAsStringAsync();
            using var doc = System.Text.Json.JsonDocument.Parse(respStr);
            int quality = doc.RootElement.GetProperty("workpiece_quality").GetInt32();
            return (WorkpieceQuality)quality;
        }
        // 自动模式：一号检测位
        // 9.2正反面版本
        private async System.Threading.Tasks.Task HandleSite1InspectionStartAuto()
        {
            try
            {

                // ---------- 检测开始前：清理相关信号 ----------
                // 清空一号检测位所有状态相关信号（保证流程绝对干净）
                await System.Threading.Tasks.Task.Delay(2000);
                _plcService.WriteBit(DataType.DataBlock, 6, 98, 1, false); // bDataOK
                _plcService.WriteBit(DataType.DataBlock, 6, 98, 2, false); // bReadyForChecking_1
                _plcService.WriteBit(DataType.DataBlock, 6, 98, 3, false); // bRobotinside_1
                _plcService.WriteBit(DataType.DataBlock, 6, 98, 4, false); // bRobotoutside_1
                _plcService.WriteBit(DataType.DataBlock, 6, 98, 5, false); // bFinish_1
                _plcService.WriteBit(DataType.DataBlock, 6, 98, 6, false); // bQualityPassed_1
                _plcService.WriteBit(DataType.DataBlock, 6, 98, 7, false); // bQualityRecheck_1
                _plcService.WriteBit(DataType.DataBlock, 6, 99, 0, false); // bQualityFailed_1
                _plcService.WriteBit(DataType.DataBlock, 6, 99, 4, false); // bFinish_2
                _plcService.WriteBit(DataType.DataBlock, 6, 99, 5, false); // bQualityPassed_2
                _plcService.WriteBit(DataType.DataBlock, 6, 99, 6, false); // bQualityRecheck_2
                _plcService.WriteBit(DataType.DataBlock, 6, 99, 7, false); // bQualityFailed_2

                Console.WriteLine("一号位全部信号已经复位");

                // =======================================================
                // Step 1: 工件正面检测
                // =======================================================

                _plcService.WriteBit(DataType.DataBlock, 6, 98, 3, true); // bRobotinside_1（正在检测）
                _plcService.WriteBit(DataType.DataBlock, 6, 98, 4, false);// bRobotoutside_1（不在检测位外）


                // 设置轨迹类型为全自动检测
                _applicationData.CurrentTraceType = "全自动";
                //读取工作令号，并转换格式
                long fullWorkOrderNumber = ReadAndLogFullWorkOrderNumber();
                Console.WriteLine($"向采集传输的工作令号: {fullWorkOrderNumber}");
                // 保存工作令号到全局变量
                _applicationData.WorkOrderNumber = fullWorkOrderNumber;
                //从plc中读取partname
                var patrName = _plcService.ReadPartName(6, 0); // 从辊道PLC读取
                //从数据库中联查轨迹
                var tracePaths = _newTraceService.GetTraces(fullWorkOrderNumber,patrName);
                //调用ProcessCardService方法，得到工艺卡ID
                var processCardId = _processCardService.GetProcessCardIdByWorkOrderNumber(fullWorkOrderNumber, patrName);
                // 将工艺卡 ID 存入全局变量
                _applicationData.ProcessCardId = processCardId;
                Console.WriteLine($"工艺卡ID：{processCardId}已存入全局变量");
                //执行轨迹
                Console.WriteLine("开始在立式检测位检测工件正面...");
                //更新
                _applicationData.DetectionPosition = "立式检测位_正面";

                //第一次用到ProcessAutoModeTrace，即开始执行轨迹
                await ProcessAutoModeTrace(tracePaths, "立式检测位");
                //写入一号检测位完成信号
                //_plcService.WriteBit(DataType.DataBlock, 5, 0, 1, true);


                // ---------- 检测结束：置位流程 ----------
                _plcService.WriteBit(DataType.DataBlock, 6, 98, 3, false); // bRobotinside_1（检测完成）
                _plcService.WriteBit(DataType.DataBlock, 6, 98, 4, true);  // bRobotoutside_1（检测位外）
                _plcService.WriteBit(DataType.DataBlock, 6, 99, 3, true);  // bRobotoutside_2（检测位外）

                // =======================================================
                // Step 2: 请求旋转
                // =======================================================

                _plcService.WriteBit(DataType.DataBlock, 6, 100, 0, true);  // bRotatePart_P1 (新增)

                Console.WriteLine("已向辊道发送了请求旋转和在一号检测位外的信号，等待 bPartRotated_P1 信号...");
                while (!_plcService.ReadBit(DataType.DataBlock, 6, 32, 7)) // bPartRotated_P1
                {
                    await System.Threading.Tasks.Task.Delay(100);
                }
                while (!_plcService.ReadBit(DataType.DataBlock, 6, 33, 1))//Galama机器人已经离开1#位
                {
                    await System.Threading.Tasks.Task.Delay(100); // 轮询等待
                }
                Console.WriteLine("收到 bPartRotated_P1 信号、Galama机器人已经离开1#位，准备检测工件背面");

                // 复位旋转请求
                _plcService.WriteBit(DataType.DataBlock, 6, 100, 0, false);
                Console.WriteLine("已复位请求旋转信号");
                // =======================================================
                // Step 3: 工件反面检测
                // =======================================================
                _plcService.WriteBit(DataType.DataBlock, 6, 98, 3, true);// bRobotinside_1（正在检测）
                _plcService.WriteBit(DataType.DataBlock, 6, 98, 4, false);// bRobotoutside_1（正在检测）

                Console.WriteLine("开始在立式检测位检测工件反面...");
                //更新
                _applicationData.DetectionPosition = "立式检测位_反面";
                //第二次用到ProcessAutoModeTrace，即开始执行轨迹
                await ProcessAutoModeTrace(tracePaths, "立式检测位");

                // 回原位
                _plcService.WriteBit(DataType.DataBlock, 6, 98, 3, false);
                _plcService.WriteBit(DataType.DataBlock, 6, 98, 4, true);

                Console.WriteLine("工件在立式检测位正反面检测完毕，调用质量接口...");
                // =======================================================
                // Step 4: 调用质量接口（一次）
                // =======================================================

                //先调用检测结果的接口，得到结果后告诉辊道
                string workOrderNumber = _applicationData.WorkOrderNumber.ToString();
                int productIndex = _applicationData.CurrentProductIndex;
                string detectionPosition = "立式检测位_0";
                var quality = await GetWorkpieceQualityAsync(workOrderNumber, productIndex, detectionPosition);
                // =======================================================
                // Step 5: 按质量结果分支（与之前一致）
                // =======================================================

                //根据结果从而判断是要一号检测还是二号检测
                switch (quality)
                {
                    case WorkpieceQuality.Passed: // 合格
                        _plcService.WriteBit(DataType.DataBlock, 6, 98, 6, true); // bQualityPassed_1
                        Console.WriteLine("bQualityPassed_1已写入");
                        //await System.Threading.Tasks.Task.Delay(2000);
                        //_plcService.WriteBit(DataType.DataBlock, 6, 98, 6, false);
                        Console.WriteLine("检测合格信号已发给辊道，准备进入2号检测位。");
                        //给一号位检测完成信号
                        _plcService.WriteBit(DataType.DataBlock, 6, 98, 5, true);// bFinish_1
                        Console.WriteLine("一号检测结束信号已发给辊道，准备进入2号检测位。");
                        // =========== 二号检测位握手流程 ==============
                        // 1. 等待bPosition2FreePasing
                        Console.WriteLine("等待bPosition2FreePasing信号...");
                        while (!_plcService.ReadBit(DataType.DataBlock, 6, 32, 4))
                        {
                            await System.Threading.Tasks.Task.Delay(100);
                        }
                        Console.WriteLine("收到bPosition2FreePasing信号");


                        // 2. 读取并同步工件号（自动重试）
                        const int maxRetryCount = 20; // 最大重试次数
                        int retryCount = 0;
                        bool success2 = false;
                        while (retryCount < maxRetryCount)
                        {
                            success2 = SyncWorkOrderFromRollerToMe();
                            if (success2)
                            {
                                Console.WriteLine("二号检测位工件号同步成功！");
                                break;
                            }
                            retryCount++;
                            Console.WriteLine($"二号检测位工件号同步失败，第{retryCount}次重试，2秒后再试...");
                            await System.Threading.Tasks.Task.Delay(2000); // 等待1秒
                        }
                        if (!success2)
                        {
                            Console.WriteLine("多次同步工件号失败，退出等待。");
                            break;
                        }

                        _plcService.WriteBit(DataType.DataBlock, 6, 98, 1, true); // bDataOK
                        Console.WriteLine("二号检测位工件号同步成功，等待bDataOk信号");

                        // 3. 等待bDataOk
                        while (!_plcService.ReadBit(DataType.DataBlock, 6, 32, 1))
                        {
                            await System.Threading.Tasks.Task.Delay(100);
                        }
                        Console.WriteLine("收到bDataOk信号");

                        //给辊道发送二号检测位检测就绪信号
                        _plcService.WriteBit(DataType.DataBlock, 6, 99, 1, true); // bReadyForChecking_2
                        Console.WriteLine("bReadyForChecking_2已写入,等待辊道bPosition2Readyforchecking信号");

                        CurrentRobotStatus = RobotStatus.Site2Ready;


                        //await System.Threading.Tasks.Task.Delay(2000); // 通信稳定等一下
                        //_plcService.WriteBit(DataType.DataBlock, 6, 99, 1, false);

                        break;

                    case WorkpieceQuality.Recheck: // 有瑕疵
                        _plcService.WriteBit(DataType.DataBlock, 6, 98, 7, true); // bQualityRecheck_1
                        Console.WriteLine("bQualityRecheck_1已写入");
                        //await System.Threading.Tasks.Task.Delay(2000);
                        //_plcService.WriteBit(DataType.DataBlock, 6, 98, 7, false);
                        Console.WriteLine("有瑕疵信号已发给辊道，准备检测下一个新工件。");
                        // 跳过2号位，检测下一个新工件
                        CurrentRobotStatus = RobotStatus.Site1Ready;
                        //向辊道发送一号检测位准备就绪信号
                        _plcService.WriteBit(DataType.DataBlock, 6, 98, 2, true); // bReadyForChecking_1
                                                                                  //await System.Threading.Tasks.Task.Delay(2000);
                                                                                  //_plcService.WriteBit(DataType.DataBlock, 6, 98, 2, false);
                        FinishInspection();
                        break;

                    case WorkpieceQuality.Failed: // 不合格
                        _plcService.WriteBit(DataType.DataBlock, 6, 99, 0, true); // bQualityFailed_1
                        Console.WriteLine("bQualityFailed_1已写入");
                        //await System.Threading.Tasks.Task.Delay(2000);
                        //_plcService.WriteBit(DataType.DataBlock, 6, 99, 0, false);
                        Console.WriteLine("不合格信号已发给辊道，准备检测下一个新工件。");
                        // 跳过2号位，检测下一个新工件
                        CurrentRobotStatus = RobotStatus.Site1Ready;
                        //向辊道发送一号检测位准备就绪信号
                        _plcService.WriteBit(DataType.DataBlock, 6, 98, 2, true); // bReadyForChecking_1
                                                                                  //await System.Threading.Tasks.Task.Delay(2000);
                                                                                  //_plcService.WriteBit(DataType.DataBlock, 6, 98, 2, false);
                        FinishInspection();
                        break;
                }
                //再向辊道写入一号完成信号
                _plcService.WriteBit(DataType.DataBlock, 6, 98, 5, true);
                Console.WriteLine("立式检测位检测完成。");

            }
            catch (Exception ex)
            {
                _logger.LogError("一号检测位异常：" + ex.Message);
                FinishInspection();
            }


        }

        // 自动模式：二号检测位
        private async System.Threading.Tasks.Task HandleSite2InspectionStartAuto()
        {
            try
            {
                Console.WriteLine("Site2检测流程启动");
                // ---------- 检测前：清理相关信号 ----------
                await System.Threading.Tasks.Task.Delay(2000);
                _plcService.WriteBit(DataType.DataBlock, 6, 99, 2, false); // bRobotinside_2
                _plcService.WriteBit(DataType.DataBlock, 6, 99, 3, false); // bRobotoutside_2
                _plcService.WriteBit(DataType.DataBlock, 6, 99, 4, false); // bFinish_2
                _plcService.WriteBit(DataType.DataBlock, 6, 99, 5, false); // bQualityPassed_2
                _plcService.WriteBit(DataType.DataBlock, 6, 99, 6, false); // bQualityRecheck_2
                _plcService.WriteBit(DataType.DataBlock, 6, 99, 7, false); // bQualityFailed_2
                _plcService.WriteBit(DataType.DataBlock, 6, 98, 6, false); // bQualityPassed_1
                Console.WriteLine("二号位全部信号已经复位");
                // ---------- 检测开始 ----------
                _plcService.WriteBit(DataType.DataBlock, 6, 99, 2, true);  // bRobotinside_2（检测中）
                _plcService.WriteBit(DataType.DataBlock, 6, 99, 3, false); // bRobotoutside_2


                // 设置轨迹类型为全自动检测
                _applicationData.CurrentTraceType = "全自动";
                long fullWorkOrderNumber = ReadAndLogWorkOrderNumber();
                //从plc中读取partname
                var patrName = _plcService.ReadPartName(6, 0); // 从辊道PLC读取
                //从数据库中联查轨迹
                var tracePaths = _newTraceService.GetTraces(fullWorkOrderNumber, patrName);

                //更新
                _applicationData.DetectionPosition = "倾斜检测位";
                // 第三次用到ProcessAutoModeTrace，即开始执行轨迹
                await ProcessAutoModeTrace(tracePaths, "倾斜检测位");
                //_plcService.WriteBit(DataType.DataBlock, 5, 0, 3, true);
                // ---------- 检测结束 ----------
                _plcService.WriteBit(DataType.DataBlock, 6, 99, 2, false); // bRobotinside_2
                _plcService.WriteBit(DataType.DataBlock, 6, 99, 3, true);  // bRobotoutside_2（检测完成到位外）


                //先调用检测结果接口，得到结果后告诉辊道
                string workOrderNumber = _applicationData.WorkOrderNumber.ToString();
                int productIndex = _applicationData.CurrentProductIndex;
                string detectionPosition = "倾斜检测位_0";
                var quality = await GetWorkpieceQualityAsync(workOrderNumber, productIndex, detectionPosition);

                switch (quality)
                {
                    case WorkpieceQuality.Passed: // 合格
                        _plcService.WriteBit(DataType.DataBlock, 6, 99, 5, true); // bQualityPassed_2
                                                                                  //await System.Threading.Tasks.Task.Delay(2000);
                                                                                  //_plcService.WriteBit(DataType.DataBlock, 6, 99, 5, false);
                        Console.WriteLine("检测合格信号已发给辊道并延迟，准备检测下一个新工件。");
                        await System.Threading.Tasks.Task.Delay(2000);
                        // 下一步业务流转
                        CurrentRobotStatus = RobotStatus.Site1Ready;
                        break;
                    case WorkpieceQuality.Recheck: // 有瑕疵
                        _plcService.WriteBit(DataType.DataBlock, 6, 99, 6, true); // bQualityRecheck_2
                                                                                  //await System.Threading.Tasks.Task.Delay(2000);
                                                                                  //_plcService.WriteBit(DataType.DataBlock, 6, 99, 6, false);
                        Console.WriteLine("有瑕疵信号已发给辊道并延迟，准备检测下一个新工件。");
                        await System.Threading.Tasks.Task.Delay(2000);
                        // 跳过2号位，检测下一个新工件
                        CurrentRobotStatus = RobotStatus.Site1Ready;
                        break;
                    case WorkpieceQuality.Failed: // 不合格
                        _plcService.WriteBit(DataType.DataBlock, 6, 99, 7, true); // bQualityFailed_2
                                                                                  //await System.Threading.Tasks.Task.Delay(2000);
                                                                                  //_plcService.WriteBit(DataType.DataBlock, 6, 99, 7, false);
                        Console.WriteLine("不合格信号已发给辊道并延迟，准备检测下一个新工件。");
                        await System.Threading.Tasks.Task.Delay(2000);
                        // 跳过2号位，检测下一个新工件
                        CurrentRobotStatus = RobotStatus.Site1Ready;
                        break;
                }
                //向辊道写入二号完成信号
                _plcService.WriteBit(DataType.DataBlock, 6, 99, 4, true); // bFinish_2

                Console.WriteLine("二号检测位检测结束信号已发给辊道并延迟，准备检测下一个新工件。");

                //Console.WriteLine("二号检测位检测流程结束。");
                await System.Threading.Tasks.Task.Delay(2000);
                // _applicationData.CurrentTraceType = "手动";

            }
            catch (Exception ex)
            {
                Console.WriteLine("HandleSite2InspectionStartAuto异常: " + ex);
                throw;
            }
            finally
            {
                FinishInspection(); // 无论什么结果都结束流程
            }
        }
        //构建完整的下载URL
        private string BuildCompleteUrl(string relativePath)
        {
            // 从配置文件中读取 IP、Port 和 Base
            var ip = _configuration["ProductCardBackend:Ip"];
            var port = _configuration["ProductCardBackend:Port"];
            var basePath = _configuration["ProductCardBackend:Base"];

            if (string.IsNullOrEmpty(ip) || string.IsNullOrEmpty(port) || string.IsNullOrEmpty(basePath))
            {
                throw new Exception("配置文件中的 ProductCardBackend 节点配置不完整。");
            }
            // 确保拼接路径的正确性
            string url = $"http://{ip}:{port}{basePath.TrimEnd('/')}/{relativePath.TrimStart('/')}";
            Console.WriteLine($"拼接后的完整 URL: {url}");
            return url;

        }

        //执行轨迹

        private async System.Threading.Tasks.Task ProcessAutoModeTrace(IEnumerable<TracePath> tracePaths, string detectionType)
        {
            if (tracePaths == null || !tracePaths.Any())
            {
                _logger.LogError($"未找到{detectionType}的轨迹文件");
                throw new InvalidOperationException($"未找到{detectionType}轨迹文件");
            }

            // 当前检测位置一共多少条轨迹
            int currentPositionTotalTraces = tracePaths.Count(t => t.Type.Equals(detectionType));
            _applicationData.TotalTracesInCurrentPosition = currentPositionTotalTraces;

            // 初始化当前轨迹索引
            int currentTraceIndex = 0;
            Parser parser = new();
            foreach (var tracePath in tracePaths.Where(t => t.Type.Equals(detectionType)))
            {
                try
                {
                    // 更新当前检测位置（立式/倾斜）
                    // 9.4修改：在执行ProcessAutoModeTrace之前更新
                    //_applicationData.DetectionPosition = detectionType;
                    // 更新当前轨迹索引
                    currentTraceIndex++;
                    //正在执行当前位置的第几条轨迹
                    _applicationData.CurrentTraceIndex = currentTraceIndex;
                    // 更新当前轨迹名称
                    _applicationData.CurrentTraceName = tracePath.Name;
                    Console.WriteLine($"开始处理{detectionType}轨迹文件 - 第 {currentTraceIndex}/{currentPositionTotalTraces} 条");

                    //_logger.LogInformation($"开始处理{detectionType}轨迹文件");
                    // 构建完整URL
                    var completeErdUrl = BuildCompleteUrl(tracePath.ErdPath);
                    var completeErpUrl = BuildCompleteUrl(tracePath.ErpPath);
                    Console.WriteLine($"构建URL完成: ERD={completeErdUrl}, ERP={completeErpUrl}");

                    // 获取/下载文件
                    Console.WriteLine("开始下载文件...");
                    var localErpFilePath = await DownloadFileFromUrlAsync(completeErpUrl, _localErpErdFolder);
                    var localErdFilePath = await DownloadFileFromUrlAsync(completeErdUrl, _localErpErdFolder);
                    Console.WriteLine($"文件下载完成: ERD={localErdFilePath}, ERP={localErpFilePath}");

                    // 验证文件存在
                    if (!File.Exists(localErpFilePath) || !File.Exists(localErdFilePath))
                    {
                        throw new FileNotFoundException($"轨迹文件不存在: ERP={localErpFilePath}, ERD={localErdFilePath}");
                    }

                    // 解析文件
                    Console.WriteLine("开始解析轨迹文件");
                    var erpCommands = parser.ParseErpFileToList(localErpFilePath);
                    Console.WriteLine("erp文件解析成功");
                    var erdData = parser.ParseErdFileToDict(localErdFilePath);
                    Console.WriteLine("erd文件解析成功");

                    // 统计当前轨迹一共多少个检测点位
                    // 对每个 RobotPosition 元素 pos 判断其 det 值是否等于 1
                    int detectionPointsCount = erdData.Values.Count(pos => pos.det == 1);
                    _applicationData.CurrentTraceDetectionPoints = detectionPointsCount;

                    // 验证解析结果
                    if (erpCommands == null || !erpCommands.Any() || erdData == null || !erdData.Any())
                    {
                        throw new InvalidOperationException("轨迹文件解析结果为空");
                    }

                    // 更新应用数据
                    _applicationData.CommandList = erpCommands;
                    _applicationData.PosDict = erdData;

                    // 设置拍照目录
                    var timeStamp = DateTimeOffset.Now.ToString("yyyy-MM-dd-HH-mm-ss", CultureInfo.InvariantCulture);
                    await SetPicturesDir(timeStamp);

                    // 执行命令
                    Console.WriteLine("开始执行机械臂命令");
                    //RunCommand里面会触发拍照，之后采集程序会调用到pointcontroller
                    _robotService.RunAllCommand();
                    Console.WriteLine("机械臂命令执行完成");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"处理轨迹文件时发生错误: {ex}");
                    throw;
                }

            }
        }
        /// <summary>
        /// 从PLC中获取工作令号
        /// </summary>
        /// <returns></returns>
        public long ReadAndLogWorkOrderNumber()
        {
            //读取工作令号
            var workOrderNumber = _plcService.ReadWorkOrderNumber(6, 66);
            //去除两个属性：PartName，OrderStartNumber
            //再转化成long型，跟数据库的工作令号一致
            return _plcService.GetFullWorkOrderNumber(workOrderNumber);
        }
        /// <summary>
        /// 从PLC中获取工作令号(13位完整）
        /// </summary>
        /// <returns></returns>
        public long ReadAndLogFullWorkOrderNumber()
        {
            //读取工作令号
            var workOrderNumber = _plcService.ReadWorkOrderNumber(6, 66);
            //去除两个属性：PartName，OrderStartNumber
            //再转化成long型，跟数据库的工作令号一致
            return _plcService.GetFullWorkOrderNumber1(workOrderNumber);
        }
        //"http://127.0.0.1:6062/files/download?path=/upload/erd/test_left_face.erd"
        //流程：
        //1.提取文件名：Path.GetFileName返回test_left_face.erd
        //2.解码文件名：UnescapeDataString防止文件名中可能存在的 URL 编码（如空格被编码为 %20）
        //这里返回还是test_left_face.erd
        //3.生成本地文件路径：Path.Combine(_localErpErdFolder, "test_left_face.erd")，即"data/test_left_face.erd"
        //4.检查本地文件是否存在：if (!File.Exists("data/test_left_face.erd"))
        //private string MapUrlToLocalFilePath(string url) =>
        //    Path.Combine(_localErpErdFolder, Uri.UnescapeDataString(Path.GetFileName(url)));
        private string MapUrlToLocalFilePath(string url)
        {
            try
            {
                // 从 URL 中提取实际的文件路径部分
                var uri = new Uri(url);
                var queryParams = System.Web.HttpUtility.ParseQueryString(uri.Query);
                var actualPath = queryParams["path"];

                if (string.IsNullOrEmpty(actualPath))
                {
                    _logger.LogWarning($"URL中未找到path参数: {url}");
                    return Path.Combine(_localErpErdFolder, Uri.UnescapeDataString(Path.GetFileName(url)));
                }

                // 解码实际文件名（处理中文）
                var fileName = Path.GetFileName(actualPath);
                var decodedFileName = Uri.UnescapeDataString(fileName);
                var localFilePath = Path.Combine(_localErpErdFolder, decodedFileName);
                Console.WriteLine($"URL映射到本地路径: {localFilePath}");
                return localFilePath;
            }
            catch (Exception ex)
            {
                _logger.LogError($"URL解析失败: {ex.Message}");
                // 降级处理：使用原始方法
                return Path.Combine(_localErpErdFolder, Uri.UnescapeDataString(Path.GetFileName(url)));
            }
        }

        //5.不存在，下载远程文件：DownloadFileFromUrlAsync
        private async Task<string> DownloadFileFromUrlAsync(string fileUrl, string folder)
        {


            //下载逻辑：
            //1.生成本地保存路径：
            //var localFilePath = Path.Combine("data", "test_left_face.erd");
            //结果："data/test_left_face.erd"
            // 解码文件名并构建本地保存路径
            //var decodedFileName = Uri.UnescapeDataString(Path.GetFileName(fileUrl));
            var localFilePath = MapUrlToLocalFilePath(fileUrl);
            // 检查文件是否已存在
            if (File.Exists(localFilePath))
            {
                Console.WriteLine($"文件已存在，直接使用: {localFilePath}");
                return localFilePath;
            }
            // 确保目录存在
            Directory.CreateDirectory(folder);

            //2.使用 HttpClient 下载文件：
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
            //var fileBytes = await client.GetByteArrayAsync("http://127.0.0.1:6062/files/download?path=/upload/erd/test_left_face.erd");
            try
            {
                Console.WriteLine($"开始下载文件: {fileUrl}");
                var fileBytes = await client.GetByteArrayAsync(fileUrl);
                //3.将下载的字节内容保存为本地文件：
                await File.WriteAllBytesAsync(localFilePath, fileBytes);
                Console.WriteLine($"文件下载成功: {localFilePath}");
                return localFilePath;

            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"文件下载失败: {fileUrl}, 错误: {ex.Message}");
                throw;
            }
        }

        private async System.Threading.Tasks.Task SetPicturesDir(string folderName)
        {
            using HttpClient client = new();
            var response = await client.GetAsync($"http://192.168.1.102:8080?name={folderName}");
        }

    }
}
