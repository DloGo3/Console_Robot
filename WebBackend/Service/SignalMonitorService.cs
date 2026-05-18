using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using WebBackend.Dao;
using WebBackend.Util;
using WebBackend.DTO;
using Microsoft.Extensions.Logging;
using S7.Net;
using System.Globalization;
using System.Net.Http;



namespace WebBackend.Service
{
    /// <summary>
    /// 用于信号监控和处理的服务类，主要是与PLC和机器人状态交互的逻辑
    /// 原来是后台服务类，先不需要了，被AutoOrManDetect代替
    /// </summary>
    public class SignalMonitorService 
    {
        private readonly SignalWatchService _signalWatchService;
        private readonly RobotStatus _robotStatus;
        private readonly RobotService _robotService;
        private readonly TraceService _traceService;
        private readonly IApplicationData _applicationData;
        private readonly PlcService _plcService;
        private readonly ILogger<SignalMonitorService> _logger;

        // 本地存放ERD和ERP文件的文件夹路径
        private readonly string _localErpErdFolder = @"data"; // 本地存放ERD/ERP文件的文件夹路径

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signalWatchService"></param>
        /// <param name="robotStatus"></param>
        /// <param name="robotService"></param>
        /// <param name="traceService"></param>
        /// <param name="applicationData"></param>
        /// <param name="plcService"></param>
        /// <param name="logger"></param>
        public SignalMonitorService(SignalWatchService signalWatchService, RobotStatus robotStatus,
            RobotService robotService, TraceService traceService, IApplicationData applicationData,
            PlcService plcService, ILogger<SignalMonitorService> logger)
        {
            _signalWatchService = signalWatchService;
            _robotStatus = robotStatus;
            _robotService = robotService;
            _traceService = traceService;
            _applicationData = applicationData;
            _plcService = plcService;
            _logger = logger;
            // 订阅信号变化事件
           // SubscribeToSignalEvents();
        }


        /// <summary>
        /// 检测PLC中的orderNumber，从而决定模式
        /// </summary>
        /// <returns></returns>
        //private  System.Threading.Tasks.Task DetermineControlMode()
        //{
        //    try
        //    {
        //        //  OrderNumber 位于 DB4 的起始字节地址 8
        //        var workOrder = _plcService.ReadWorkOrderNumber(4, 8); // 示例：DB4，偏移量0
        //        var orderNumber = workOrder.OrderNumber;

        //        if (orderNumber == 0)
        //        {
        //            _applicationData.ModeState.CurrentMode = ControlMode.Manual;
        //            _logger.LogInformation("检测到手动模式：OrderNumber == 0");
        //        }
        //        else
        //        {
        //            _applicationData.ModeState.CurrentMode = ControlMode.Automatic;
        //            _logger.LogInformation($"检测到自动模式：OrderNumber == {orderNumber}");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"无法读取 OrderNumber：{ex.Message}");
        //        throw;
        //    }
        //    return System.Threading.Tasks.Task.CompletedTask; // 明确返回一个完成的任务
        //}



        /// <summary>
        /// 异步
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        //protected override System.Threading.Tasks.Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    //读取OrderNumber并设置模式
        //    //await DetermineControlMode();
        //    // 订阅信号变化事件
        //    //SubscribeToSignalEvents();
        //    return System.Threading.Tasks.Task.CompletedTask;
        //}
        private void SubscribeToSignalEvents()
        {
            // 获取并订阅监控的信号变化事件
            //var site1InspectStart = _signalWatchService.GetSignal<Signal<bool>>("site1InspectStart");
            //var site2InspectStart = _signalWatchService.GetSignal<Signal<bool>>("site2InspectStart");

            //if (site1InspectStart != null)
            //{
            //    site1InspectStart.SignalChanged += OnSite1InspectStartChanged;
            //}
            //if (site2InspectStart != null)
            //{
            //    site2InspectStart.SignalChanged += OnSite2InspectStartChanged;
            //}
        }

        private async void OnSite1InspectStartChanged(Signal<bool> signal)
        {
           
                //if (signal.IsRisingEdge())
                //{
                //    _logger.LogInformation("site1InspectStart 信号上升沿检测，开始处理一号检测位");
                //    //await HandleSite1InspectionStart();
                //}
            
        }

        private async void OnSite2InspectStartChanged(Signal<bool> signal)
        {
            //if (signal.IsRisingEdge())
            //{
            //    _logger.LogInformation("site2InspectStart 信号上升沿检测，开始处理二号检测位");
            //    //await HandleSite2InspectionStart();
            //}
        }

        private async System.Threading.Tasks.Task HandleSite1InspectionStart()
        {
            if (_robotStatus.CurrentState == RobotStatus.Site1Ready)
            {
                _robotStatus.CurrentState = RobotStatus.DetectionAtSite1;
                _logger.LogInformation("机械臂开始在一号检测位运动");

                await System.Threading.Tasks.Task.Run(async() =>
                {
                    //var tracePaths = _traceService.GetTracePaths(_applicationData.processCardId);
                    var parser = new Parser();

                    foreach (var trace in _applicationData.Traces)
                    {
                        if (trace.Type.Equals("一号检测位"))
                        {
                            //获取本地erp，erd文件路径
                            var localErpFilePath = MapUrlToLocalFilePath(trace.ErpFilePath);
                            var localErdFilePath = MapUrlToLocalFilePath(trace.ErdFilePath);
                            //var erpCommands = parser.ParseErpFileToList(tracePath.ErpPath);
                            //var erdData = parser.ParseErdFileToDict(tracePath.ErdPath);
                            //文件不存在，下载
                            if (!File.Exists(localErpFilePath))
                            {
                                localErpFilePath = await DownloadFileFromUrlAsync(trace.ErpFilePath, _localErpErdFolder);
                            }

                            if (!File.Exists(localErdFilePath))
                            {
                                localErdFilePath = await DownloadFileFromUrlAsync(trace.ErdFilePath, _localErpErdFolder);
                            }

                            if (File.Exists(localErpFilePath) && File.Exists(localErdFilePath))
                            {
                                // 解析 ERP 和 ERD 文件
                                var erpCommands = parser.ParseErpFileToList(localErpFilePath);
                                var erdData = parser.ParseErdFileToDict(localErdFilePath);
                                _applicationData.CommandList = erpCommands;
                                _applicationData.PosDict = erdData;

                                //设置拍照文件夹
                                var createTime = DateTimeOffset.Now.ToLocalTime().ToString("yyyy-MM-dd-HH-mm-ss", CultureInfo.InvariantCulture);
                                SetPicturesDir(createTime);

                                _robotService.RunAllCommand();
                            }
                            else
                            {
                                _logger.LogError($"文件 {localErpFilePath} 或 {localErdFilePath} 不存在！");
                            }
                        }
                    }
                    //site1InspectCompleted
                    _plcService.WriteBit(DataType.DataBlock, 4, 2, 1, true);
                    _robotStatus.CurrentState = RobotStatus.Site2Ready;
                    _logger.LogInformation("一号检测位检测完成，机械臂状态设置为 Site2Ready");
                });
            }
        }

        private async System.Threading.Tasks.Task HandleSite2InspectionStart()
        {
            if (_robotStatus.CurrentState == RobotStatus.Site2Ready)
            {
                _robotStatus.CurrentState = RobotStatus.DetectionAtSite2;
                _logger.LogInformation("机械臂开始在二号检测位运动");

                await System.Threading.Tasks.Task.Run(async() =>
                {
                    //var tracePaths = _traceService.GetTracePaths(_applicationData.processCardId);
                    var parser = new Parser();

                    foreach (var trace in _applicationData.Traces)
                    {
                        if (trace.Type.Equals("二号检测位"))
                        {
                            // 获取本地erp，erd文件路径
                            var localErpFilePath = MapUrlToLocalFilePath(trace.ErpFilePath);
                            var localErdFilePath = MapUrlToLocalFilePath(trace.ErdFilePath);
                            //var erpCommands = parser.ParseErpFileToList(tracePath.ErpPath);
                            //var erdData = parser.ParseErdFileToDict(tracePath.ErdPath);

                            //如果文件不存在，下载
                            if (!File.Exists(localErpFilePath))
                            {
                                localErpFilePath = await DownloadFileFromUrlAsync(trace.ErpFilePath, _localErpErdFolder);
                            }

                            if (!File.Exists(localErdFilePath))
                            {
                                localErdFilePath = await DownloadFileFromUrlAsync(trace.ErdFilePath, _localErpErdFolder);
                            }

                            if (File.Exists(localErpFilePath) && File.Exists(localErdFilePath))
                            {
                                // 解析 ERP 和 ERD 文件
                                var erpCommands = parser.ParseErpFileToList(localErpFilePath);
                                var erdData = parser.ParseErdFileToDict(localErdFilePath);
                                _applicationData.CommandList = erpCommands;
                                _applicationData.PosDict = erdData;

                                //设置拍照文件夹
                                var createTime = DateTimeOffset.Now.ToLocalTime().ToString("yyyy-MM-dd-HH-mm-ss", CultureInfo.InvariantCulture);
                                SetPicturesDir(createTime);

                                _robotService.RunAllCommand();
                            }
                            else
                            {
                                _logger.LogError($"文件 {localErpFilePath} 或 {localErdFilePath} 不存在！");
                            }
                        }
                    }
                    // site2InspectCompleted
                    _plcService.WriteBit(DataType.DataBlock, 4, 2, 3, true);
                    _robotStatus.CurrentState = RobotStatus.Idle;
                    _logger.LogInformation("二号检测位检测完成，机械臂状态设置为 Idle");
                });
            }
        }

        // 将URL编码转换为本地文件路径
        private string MapUrlToLocalFilePath(string url)
        {
            // 解码URL编码
            var decodedFileName = Uri.UnescapeDataString(Path.GetFileName(url));

            // 构建本地文件路径
            return Path.Combine(_localErpErdFolder, decodedFileName);
        }

        public async Task<string> DownloadFileFromUrlAsync(string fileUrl, string _localErpErdFolder)
        {
            using HttpClient client = new HttpClient
            {
                // 设置超时时间为 30 秒
                Timeout = TimeSpan.FromSeconds(30)
            };

            var fileName = Path.GetFileName(fileUrl); // 从 URL 获取文件名
            var localFilePath = Path.Combine(_localErpErdFolder, fileName);

            try
            {
                // 下载文件字节内容
                var fileBytes = await client.GetByteArrayAsync(fileUrl);

                // 将字节内容保存到本地
                await File.WriteAllBytesAsync(localFilePath, fileBytes);

                return localFilePath;  // 返回文件的本地路径
            }
            catch (Exception ex)
            {
                _logger.LogError($"下载文件失败: {fileUrl}, 错误信息: {ex.Message}");
                throw;
            }
        }


        //设置拍照目录
        private async void SetPicturesDir(string name)
        {
            using var client = new HttpClient();
            string url = $"http://192.168.1.102:8080?name={name}";
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"SetPicturesDir success: {name}");
            }
            else
            {
                _logger.LogError("Failed to set pictures directory");
            }
        }

        
    }
}
