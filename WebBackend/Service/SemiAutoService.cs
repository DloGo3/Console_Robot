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
using MySqlX.XDevAPI.Common;

namespace WebBackend.Service
{
    /// <summary>
    /// 半自动检测模式，收到前端的数据，直接执行，不需要监听信号的变化
    /// </summary>
    public class SemiAutoService
    {
        private readonly RobotStatus _robotStatus;
        private readonly RobotService _robotService;
        private readonly IApplicationData _appData;
        private readonly PlcService _plcService;
        private readonly ILogger<SignalMonitorService> _logger;
        private readonly FileDownloadService _fileDownloadService;
        private readonly Parser _parser;
        // 本地存放ERD和ERP文件的文件夹路径
        private readonly string _localErpErdFolder = @"data";
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="robotStatus"></param>
        /// <param name="robotService"></param>
        /// <param name="traceService"></param>
        /// <param name="applicationData"></param>
        /// <param name="plcService"></param>
        /// <param name="logger"></param>
        /// <param name="fileDownloadService"></param>
        public SemiAutoService(RobotStatus robotStatus, RobotService robotService, IApplicationData appData,
            PlcService plcService, ILogger<SignalMonitorService> logger, Parser parser, FileDownloadService fileDownloadService)
        {
            _robotStatus = robotStatus;
            _robotService = robotService;
            _appData = appData;
            _plcService = plcService;
            _logger = logger;
            _fileDownloadService = fileDownloadService;
            _parser = parser;

        }
        // 添加执行状态属性

        public async System.Threading.Tasks.Task VerticalInspection()
        {
            //// 添加状态检查
            //if (_robotService.IsRunning)
            //{
            //    _logger.LogWarning("机械臂正在执行其他命令，拒绝新请求");
            //    return;
            //}
            //if (_robotStatus.CurrentState == RobotStatus.Site1Ready)
            //{
            //    _robotStatus.CurrentState = RobotStatus.DetectionAtSite1;
            // 设置轨迹类型为半自动检测
            _appData.CurrentTraceType = "半自动";
            _logger.LogInformation("机械臂开始在立式检测位运动");
            // 计算当前检测位置的轨迹数
            CalculateTracesInCurrentPosition("立式检测位");

            await System.Threading.Tasks.Task.Run(async () =>
                {
                    //var parser = new Parser();
                    int currentTraceIndex = 0; // 初始化轨迹索引

                    foreach (var trace in _appData.Traces)
                    {
                        if (trace.Type.Equals("立式检测位"))
                        {
                            currentTraceIndex++;
                            _appData.CurrentTraceIndex = currentTraceIndex; // 更新全局变量
                            //正在执行的轨迹名称
                            _appData.CurrentTraceName = trace.Name;
                            _appData.DetectionPosition = "立式检测位";

                            // 添加日志确认更新
                            _logger.LogInformation($"更新当前轨迹名称: {_appData.CurrentTraceName}");
                            _logger.LogInformation($"正在执行第 {currentTraceIndex} 条轨迹: {trace.Name}");
                            // 构建完整 URL 并下载文件
                            var completeErdUrl = _fileDownloadService.BuildCompleteUrl(trace.ErdFilePath);
                            var completeErpUrl = _fileDownloadService.BuildCompleteUrl(trace.ErpFilePath);

                            var localErpPath = await _fileDownloadService.DownloadFileFromUrlAsync(completeErpUrl, "data");
                            var localErdPath = await _fileDownloadService.DownloadFileFromUrlAsync(completeErdUrl, "data");
                            // 解析文件
                            var commands = _parser.ParseErpFileToList(localErpPath);
                            var positions = _parser.ParseErdFileToDict(localErdPath);

                            _logger.LogInformation($"Parsed {commands.Count} commands and {positions.Count} positions from ERD and ERP files.");

                            // 统计当前轨迹一共多少个检测点位
                            // 对每个 RobotPosition 元素 pos 判断其 det 值是否等于 1
                            int detectionPointsCount = positions.Values.Count(pos => pos.det == 1);
                            _appData.CurrentTraceDetectionPoints = detectionPointsCount;

                            // 更新全局数据
                            _appData.CommandList = commands; // 加载指令列表
                            _appData.PosDict = positions; // 加载点位信息
                            // 设置拍照目录
                            var timeStamp = DateTimeOffset.Now.ToString("yyyy-MM-dd-HH-mm-ss", CultureInfo.InvariantCulture);
                            await SetPicturesDir(timeStamp);
                            //_robotService.RunAllCommand();
                            await _robotService.RunAllCommandAsync();
                            _logger.LogInformation($"轨迹 {trace.Name} 执行完成");


                        }
                    }
                    ////写入立式检测位完成信号
                    //_plcService.WriteBit(DataType.DataBlock, 5, 0, 1, true);
                    //_robotStatus.CurrentState = RobotStatus.Site2Ready;
                    _logger.LogInformation("立式检测位检测完成");
                    _appData.CurrentTraceType = "手动";
                });
            
        }

        public async System.Threading.Tasks.Task ObliqueInspection()
        {
            //// 添加状态检查
            //if (_robotService.IsRunning)
            //{
            //    _logger.LogWarning("机械臂正在执行其他命令，拒绝新请求");
            //    return;
            //}
            //if (_robotStatus.CurrentState == RobotStatus.Site2Ready)
            //{
            //    _robotStatus.CurrentState = RobotStatus.DetectionAtSite2;
            // 设置轨迹类型为半自动检测
            _appData.CurrentTraceType = "半自动";
            _logger.LogInformation("机械臂开始在倾斜检测位运动");
            // 更新当前检测位置
            // _appData.DetectionPosition = detectionType;

            // 计算当前检测位置的轨迹数
            CalculateTracesInCurrentPosition("倾斜检测位");

            await System.Threading.Tasks.Task.Run(async () =>
                {
                    int currentTraceIndex = 0; // 初始化轨迹索引
                    //var parser = new Parser();

                    foreach (var trace in _appData.Traces)
                    {
                        if (trace.Type.Equals("倾斜检测位"))
                        {
                            currentTraceIndex++;
                            _appData.CurrentTraceIndex = currentTraceIndex; // 更新全局变量
                            //正在执行的轨迹名称
                            _appData.CurrentTraceName = trace.Name;
                            _appData.DetectionPosition = "倾斜检测位";

                            _logger.LogInformation($"正在执行第 {currentTraceIndex} 条轨迹: {trace.Name}");
                            // 构建完整 URL 并下载文件
                            var completeErdUrl = _fileDownloadService.BuildCompleteUrl(trace.ErdFilePath);
                            var completeErpUrl = _fileDownloadService.BuildCompleteUrl(trace.ErpFilePath);

                            var localErdPath = await _fileDownloadService.DownloadFileFromUrlAsync(completeErdUrl, "data");
                            var localErpPath = await _fileDownloadService.DownloadFileFromUrlAsync(completeErpUrl, "data");
                            // 解析文件
                            var commands = _parser.ParseErpFileToList(localErpPath);
                            var positions = _parser.ParseErdFileToDict(localErdPath);

                            _logger.LogInformation($"Parsed {commands.Count} commands and {positions.Count} positions from ERD and ERP files.");
                            // 更新全局数据
                            _appData.CommandList = commands; // 加载指令列表
                            _appData.PosDict = positions; // 加载点位信息
                            // 设置拍照目录
                            var timeStamp = DateTimeOffset.Now.ToString("yyyy-MM-dd-HH-mm-ss", CultureInfo.InvariantCulture);
                            await SetPicturesDir(timeStamp);
                            //_robotService.RunAllCommand();
                            await _robotService.RunAllCommandAsync();
                            _logger.LogInformation($"轨迹 {trace.Name} 执行完成");
                        }
                    }
                    ////写入斜式检测位完成信号
                    //_plcService.WriteBit(DataType.DataBlock, 5, 0, 3, true);
                    //_robotStatus.CurrentState = RobotStatus.Idle;
                    _logger.LogInformation("斜式检测位检测完成");
                    _appData.CurrentTraceType = "手动";
                });
            
        }

        /// <summary>
        /// 工艺卡需要执行多少条轨迹
        /// </summary>
        public void CalculateTotalTracesInProcessCard()
        {
            if (_appData.Traces != null)
            {
                int totalTraces = _appData.Traces.Count;
                _appData.TotalTracesInProcessCard = totalTraces; // 将总轨迹数存入全局变量
                _logger.LogInformation($"工艺卡需要执行的总轨迹数: {totalTraces}");
            }
            else
            {
                _logger.LogWarning("无法计算总轨迹数，因为 _appData.Traces 为空");
            }
        }
        /// <summary>
        /// 当前检测位置一共多少条轨迹
        /// </summary>
        /// <param name="detectionType"></param>
        public void CalculateTracesInCurrentPosition(string detectionType)
        {
            if (_appData.Traces != null)
            {
                int count = _appData.Traces.Count(trace => trace.Type.Equals(detectionType));
                _appData.TotalTracesInCurrentPosition = count;
                _logger.LogInformation($"当前检测位置 '{detectionType}' 的总轨迹数: {count}");
            }
            else
            {
                _logger.LogWarning("无法计算当前检测位置的轨迹数，因为 _appData.Traces 为空");
            }
        }


        private async System.Threading.Tasks.Task SetPicturesDir(string folderName)
        {
            using HttpClient client = new();
            var response = await client.GetAsync($"http://192.168.1.102:8080?name={folderName}");
        }
    }
}
