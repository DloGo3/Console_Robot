using WebBackend.Dao;
using WebBackend.Service;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using WebBackend.Util;

namespace WebBackend.Service
{
    /// <summary>
    /// 后台服务类，用于定期监测读取PLC的信号状态，并根据这些状态更新机械臂的状态
    /// </summary>
    public class SignalMonitorServiceTest : BackgroundService
    {
        private readonly PlcService _plcService;
        private readonly Signals _signals;
        private readonly ArmStateMachine _armStateMachine;
        private readonly RobotStatus _robotStatus;
        private readonly PlcPulseService _plcPulseService;
        private readonly TraceService _traceService;
        private readonly RobotService _robotService;
        private readonly IApplicationData _applicationData ;
        private readonly ILogger<SignalMonitorService> _logger;

        public SignalMonitorServiceTest(
            PlcService plcService,
            Signals signals,
            ArmStateMachine armStateMachine,
            RobotStatus robotStatus,
            IApplicationData applicationData,
            RobotService robotService,
            PlcPulseService plcPulseService,
            TraceService traceService,
            ILogger<SignalMonitorService> logger)
        {
            _plcService = plcService;
            _signals = signals;
            _armStateMachine = armStateMachine;
            _plcPulseService = plcPulseService;
            _logger = logger;
        }

        /// <summary>
        /// 启动一个新的任务，以异步方式处理机械臂的运动。
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async System.Threading.Tasks.Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // 定期检查和更新PLC信号
                CheckAndUpdateSignals();

                // 根据信号状态触发机械臂状态更新
                WatchAndUpdateState();

                // 等待50毫秒，避免频繁调用
                await System.Threading.Tasks.Task.Delay(50, stoppingToken);
            }
        }

        /// <summary>
        /// 检查并更新PLC信号
        /// </summary>
        private void CheckAndUpdateSignals()
        {
            // 示例：模拟从PLC读取状态信号并更新到_signals对象中
            // _signals.Site1Arrival = _plcService.ReadBit(DataType.Input, 0, 2, 0);
            // _signals.Site2Arrival = _plcService.ReadBit(DataType.Input, 0, 3, 0);
            // _signals.Site1Defect = _plcService.ReadBit(DataType.Input, 0, 4, 0);
            // _signals.Site1NoDefect = _plcService.ReadBit(DataType.Input, 0, 5, 0);
            // _signals.Site2Defect = _plcService.ReadBit(DataType.Input, 0, 6, 0);
            // _signals.Site2NoDefect = _plcService.ReadBit(DataType.Input, 0, 7, 0);
        }

        /// <summary>
        /// 根据信号和状态进行监视并更新状态
        /// </summary>
        private void WatchAndUpdateState()
        {
            // 更新状态机
            _armStateMachine.UpdateState();

            // 根据当前状态决定动作
            var currentState = _armStateMachine.GetCurrentState().CurrentState;

            switch (currentState)
            {
                case RobotStatus.Site1Ready:
                    if (_signals.Site1ArrivalBefore == false && _signals.Site1Arrival == true)
                    {
                        _logger.LogInformation("机械臂正在检测一号位...");
                        HandleSite1Detection();
                    }
                    break;

                case RobotStatus.Site2Ready:
                    if (_signals.Site2ArrivalBefore == false && _signals.Site2Arrival == true)
                    {
                        _logger.LogInformation("机械臂正在检测二号位...");
                        HandleSite2Detection();
                    }
                    break;
            }

            // 更新前一帧的信号状态
            _signals.Site1ArrivalBefore = _signals.Site1Arrival;
            _signals.Site2ArrivalBefore = _signals.Site2Arrival;
        }


        /// <summary>
        /// 处理一号位检测逻辑
        /// </summary>
        private void HandleSite1Detection()
        {
            var tracePaths = _traceService.GetTracePaths(_applicationData.ProcessCardId);
            Parser parser = new();

            foreach (var tracePath in tracePaths)
            {
                if (tracePath.Type.Equals("一号检测位"))
                {
                    var erpCommands = parser.ParseErpFileToList(tracePath.ErpPath);
                    var erdData = parser.ParseErdFileToDict(tracePath.ErdPath);

                    _applicationData.CommandList = erpCommands;
                    _applicationData.PosDict = erdData;

                    // 执行所有命令
                    _robotService.RunAllCommand();
                }
            }
            // 发送脉冲信号给PLC，表示检测完成
            if (_signals.Site1NoDefect)
            {
                //_plcPulseService.SendPulse(DataType.Output, 0, 5, 0);
                _armStateMachine.UpdateState(); // 进入Site2Ready状态
            }
            else if (_signals.Site1Defect)
            {
                //_plcPulseService.SendPulse(DataType.Output, 0, 4, 0);
                _armStateMachine.UpdateState(); // 回到Idle状态
            }
        }

        /// <summary>
        /// 处理二号位检测逻辑
        /// </summary>
        private void HandleSite2Detection()
        {
            var tracePaths = _traceService.GetTracePaths(_applicationData.ProcessCardId);
            Parser parser = new();

            foreach (var tracePath in tracePaths)
            {
                if (tracePath.Type.Equals("二号检测位"))
                {
                    var erpCommands = parser.ParseErpFileToList(tracePath.ErpPath);
                    var erdData = parser.ParseErdFileToDict(tracePath.ErdPath);

                    _applicationData.CommandList = erpCommands;
                    _applicationData.PosDict = erdData;

                    // 执行所有命令
                    _robotService.RunAllCommand();
                }
            }
            // 发送脉冲信号给PLC，表示检测完成
            if (_signals.Site2NoDefect)
            {
                //_plcPulseService.SendPulse(DataType.Output, 0, 6, 0);
                _armStateMachine.UpdateState(); // 进入Idle状态
            }
            else if (_signals.Site2Defect)
            {
                //_plcPulseService.SendPulse(DataType.Output, 0, 7, 0);
                _armStateMachine.UpdateState(); // 回到Idle状态
            }
        }
    }
}

