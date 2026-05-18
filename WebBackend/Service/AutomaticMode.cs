//using WebBackend.Dao;
//using WebBackend.Util;
//using S7.Net;
//using System.Globalization;



//namespace WebBackend.Service
//{
//    /// <summary>
//    /// 自动检测流程
//    /// </summary>
//    public class AutomaticMode : BackgroundService
//    {
//        private readonly SignalWatchService _signalWatchService;
//        private readonly RobotStatus _robotStatus;
//        private readonly RobotService _robotService;
//        private readonly NewTraceService _newTraceService;
//        private readonly IApplicationData _applicationData;
//        private readonly PlcService _plcService;
//        private readonly ILogger<SignalMonitorService> _logger;

//        /// <summary>
//        /// 构造函数
//        /// </summary>
//        /// <param name="signalWatchService"></param>
//        /// <param name="robotStatus"></param>
//        /// <param name="robotService"></param>
//        /// <param name="newTraceService"></param>
//        /// <param name="applicationData"></param>
//        /// <param name="plcService"></param>
//        /// <param name="logger"></param>
//        public AutomaticMode(SignalWatchService signalWatchService, RobotStatus robotStatus,
//            RobotService robotService, NewTraceService newTraceService, IApplicationData applicationData,
//            PlcService plcService, ILogger<SignalMonitorService> logger)
//        {
//            _signalWatchService = signalWatchService;
//            _robotStatus = robotStatus;
//            _robotService = robotService;
//            _newTraceService = newTraceService;
//            _applicationData = applicationData;
//            _plcService = plcService;
//            _logger = logger;
//            // 订阅信号变化事件
//            // SubscribeToSignalEvents();
//        }


//        /// <summary>
//        /// 异步
//        /// </summary>
//        /// <param name="stoppingToken"></param>
//        /// <returns></returns>
//        protected override System.Threading.Tasks.Task ExecuteAsync(CancellationToken stoppingToken)
//        {

//            // 订阅信号变化事件
//            SubscribeToSignalEvents();
//            return System.Threading.Tasks.Task.CompletedTask;
//        }
//        private void SubscribeToSignalEvents()
//        {
//            // 获取并订阅监控的信号变化事件
//            var site1InspectStart = _signalWatchService.GetSignal<Signal<bool>>("site1InspectStart");
//            var site2InspectStart = _signalWatchService.GetSignal<Signal<bool>>("site2InspectStart");

//            if (site1InspectStart != null)
//            {
//                site1InspectStart.SignalChanged += OnSite1InspectStartChanged;
//            }
//            if (site2InspectStart != null)
//            {
//                site2InspectStart.SignalChanged += OnSite2InspectStartChanged;
//            }
//        }

//        private async void OnSite1InspectStartChanged(Signal<bool> signal)
//        {

//            if (signal.IsRisingEdge())
//            {
//                _logger.LogInformation("site1InspectStart 信号上升沿检测，开始处理一号检测位");
//                await HandleSite1InspectionStart();
//            }

//        }

//        private async void OnSite2InspectStartChanged(Signal<bool> signal)
//        {
//            if (signal.IsRisingEdge())
//            {
//                _logger.LogInformation("site2InspectStart 信号上升沿检测，开始处理二号检测位");
//                await HandleSite2InspectionStart();
//            }
//        }
//        private long ReadAndLogWorkOrderNumber()
//        {
//            try
//            {
//                // 从 PLC 中读取工作令号
//                _logger.LogInformation("开始从 PLC 中读取工作令号...");
//                WorkOrderNumber workOrderNumber = _plcService.ReadWorkOrderNumber(4, 0); // 示例：DB4，起始偏移地址为 0
//                long fullWorkOrderNumber = _plcService.GetFullWorkOrderNumber(workOrderNumber);
//                _logger.LogInformation($"成功从 PLC 读取工作令号：{fullWorkOrderNumber}");

//                //收到数据把idle转为Site1Ready
//                _robotStatus.CurrentState = RobotStatus.Site1Ready;
//                return fullWorkOrderNumber;
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError($"读取工作令号失败：{ex.Message}");
//                throw; // 重新抛出异常以便上层调用处理
//            }
//        }

//        private async System.Threading.Tasks.Task HandleSite1InspectionStart()
//        {

//            if (_robotStatus.CurrentState == RobotStatus.Site1Ready)
//            {
//                _robotStatus.CurrentState = RobotStatus.DetectionAtSite1;
//                _logger.LogInformation("机械臂开始在一号检测位运动");

//                await System.Threading.Tasks.Task.Run(async () =>
//                {
//                    // 读取工作令号
//                    long fullWorkOrderNumber = ReadAndLogWorkOrderNumber();

//                    // 根据完整工作令号查询工艺卡及轨迹
//                    _logger.LogInformation("根据工作令号查询工艺卡和轨迹...");
//                    var tracePaths = _newTraceService.GetTracesByWorkOrderNumber(fullWorkOrderNumber);

//                    // 初始化解析器
//                    Parser parser = new();

//                    foreach (var tracePath in tracePaths)
//                    {
//                        if (tracePath.Type.Equals("一号检测位"))
//                        {
//                            // 解析 ERP 和 ERD 文件
//                            _logger.LogInformation($"解析轨迹文件：{tracePath.ErpPath}, {tracePath.ErdPath}");
//                            var erpCommands = parser.ParseErpFileToList(tracePath.ErpPath);
//                            var erdData = parser.ParseErdFileToDict(tracePath.ErdPath);

//                            // 更新应用程序数据
//                            _applicationData.CommandList = erpCommands;
//                            _applicationData.PosDict = erdData;

//                            // 设置拍照文件夹
//                            var createTime = DateTimeOffset.Now.ToLocalTime().ToString("yyyy-MM-dd-HH-mm-ss", CultureInfo.InvariantCulture);
//                            await SetPicturesDir(createTime);

//                            // 执行所有命令
//                            _robotService.RunAllCommand();
//                        }
//                    }

//                    // 检测完成后更新状态并通知 PLC
//                    _plcService.WriteBit(DataType.DataBlock, 4, 2, 1, true); // 示例：DB4 偏移地址 2.1
//                    _robotStatus.CurrentState = RobotStatus.Site2Ready;
//                    _logger.LogInformation("一号检测位检测完成，机械臂状态设置为 Site2Ready");
//                });

//            }
//        }

//        private async System.Threading.Tasks.Task HandleSite2InspectionStart()
//        {
//            if (_robotStatus.CurrentState == RobotStatus.Site2Ready)
//            {
//                _robotStatus.CurrentState = RobotStatus.DetectionAtSite2;
//                _logger.LogInformation("机械臂开始在二号检测位运动");

//                await System.Threading.Tasks.Task.Run(async () =>
//                {
//                    // 读取工作令号
//                    long fullWorkOrderNumber = ReadAndLogWorkOrderNumber();

//                    // 根据完整工作令号查询工艺卡及轨迹
//                    _logger.LogInformation("根据工作令号查询工艺卡和轨迹...");
//                    var tracePaths = _newTraceService.GetTracesByWorkOrderNumber(fullWorkOrderNumber);

//                    // 初始化解析器
//                    Parser parser = new();

//                    foreach (var tracePath in tracePaths)
//                    {
//                        if (tracePath.Type.Equals("二号检测位"))
//                        {
//                            // 解析 ERP 和 ERD 文件
//                            _logger.LogInformation($"解析轨迹文件：{tracePath.ErpPath}, {tracePath.ErdPath}");
//                            var erpCommands = parser.ParseErpFileToList(tracePath.ErpPath);
//                            var erdData = parser.ParseErdFileToDict(tracePath.ErdPath);

//                            // 更新应用程序数据
//                            _applicationData.CommandList = erpCommands;
//                            _applicationData.PosDict = erdData;

//                            // 设置拍照文件夹
//                            var createTime = DateTimeOffset.Now.ToLocalTime().ToString("yyyy-MM-dd-HH-mm-ss", CultureInfo.InvariantCulture);
//                            await SetPicturesDir(createTime);

//                            // 执行所有命令
//                            _robotService.RunAllCommand();
//                        }
//                    }
//                    // site2InspectCompleted
//                    _plcService.WriteBit(DataType.DataBlock, 4, 2, 3, true);
//                    _robotStatus.CurrentState = RobotStatus.Idle;
//                    _logger.LogInformation("二号检测位检测完成，机械臂状态设置为 Idle");
//                });
//            }
//        }


//        //设置拍照目录
//        private async System.Threading.Tasks.Task SetPicturesDir(string name)
//        {
//            using var client = new HttpClient();
//            string url = $"http://192.168.1.102:8080?name={name}";
//            var response = await client.GetAsync(url);
//            if (response.IsSuccessStatusCode)
//            {
//                _logger.LogInformation($"SetPicturesDir success: {name}");
//            }
//            else
//            {
//                _logger.LogError("Failed to set pictures directory");
//            }
//        }

//    }
    
//}

