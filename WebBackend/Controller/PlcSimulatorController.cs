using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using S7.Net;
using WebBackend.Dao;
using WebBackend.Service;
using WebBackend.DTO;


namespace WebBackend.Controller
{
    /// <summary>
    /// 模拟与 PLC 通信的过程
    /// 通过一些 HTTP 接口与前端或其他系统进行交互，更新和获取 PLC 信号，并控制机械臂状态机。
    /// </summary>
    /// <param name="signals"></param>
    /// <param name="armStateMachine"></param>
    /// <param name="robotStatus"></param>
    /// <param name="plcService"></param>
    /// <param name="plcPulseService"></param>
    /// <param name="logger"></param>

    [ApiController]
    [Route("[controller]")]
    public class PlcSimulatorController(Signals signals, ArmStateMachine armStateMachine, RobotStatus robotStatus, PlcService plcService,PlcPulseService plcPulseService,ILogger<PlcSimulatorController> logger) : ControllerBase
    {
        private readonly Signals _signals = signals;//保存 PLC 信号状态的实例。
        private readonly ArmStateMachine _armStateMachine = armStateMachine;//管理机械臂状态的状态机实例。
        private readonly RobotStatus _robotStatus = robotStatus;//存储当前机械臂状态的实例。
        private readonly PlcService _plcService = plcService;
        private readonly PlcPulseService _plcPulseService = plcPulseService;//使用其中的发送脉冲函数
        private readonly ILogger<PlcSimulatorController> _logger = logger;



        /// <summary>
        /// 通用方法：设置信号状态
        /// </summary>
        /// <param name="signalName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpGet("Set/{signalName}/{value}")]
        public IActionResult SetSignal(string signalName, bool value)
        {
            //使用反射来获取 Signals 类中的某个属性（property）信息。
            //具体来说，这行代码用于在运行时根据属性名称（signalName）动态地获取 Signals 类中的相应属性
            //例如，如果 signalName 的值是 "Site1Arrival"，
            //则 GetProperty("Site1Arrival") 会尝试查找 Signals 类中名为 Site1Arrival 的属性。
            var property = typeof(Signals).GetProperty(signalName);
            if (property == null)
            {
                _logger.LogError("Signal {signalName} not found.",signalName);
                return NotFound(new DTO.R($"Signal {signalName} not found", 404).ToJsonString());
            }

            property.SetValue(_signals, value);
            _logger.LogInformation("Signal {signalName} set to {value}.",signalName,value);

            // 更新状态机状态
            // _armStateMachine.UpdateState();

            return Ok(new DTO.R($"Signal {signalName} set to {value}", 200).ToJsonString());
        }
        /// <summary>
        /// 通用方法：获取信号状态
        /// </summary>
        /// <param name="signalName"></param>
        /// <returns></returns>
        [HttpGet("Get/{signalName}")]
        public IActionResult GetSignal(string signalName)
        {
            var property = typeof(Signals).GetProperty(signalName);
            if (property == null)
            {
                _logger.LogError("Signal {signalName} not found.",signalName);
                return NotFound(new DTO.R($"Signal {signalName} not found", 404).ToJsonString());
            }

            var value = property.GetValue(_signals);
            _logger.LogInformation("Signal {signalName} status requested.",signalName);

            return Ok(new DTO.R(value, 200).ToJsonString());
        }

        /// <summary>
        /// 获取当前机械臂状态
        /// </summary>
        /// <returns></returns>
        [HttpGet("RobotStatus")]
        public IActionResult GetRobotStatus()
        {
            return Ok(new DTO.R(_robotStatus.CurrentState, 200).ToJsonString());
        }


        /// <summary>
        /// 紧急停止机械臂
        /// </summary>
        /// <returns></returns>
        [HttpPost("Control/Stop")]
        public IActionResult Stop()
        {
            _armStateMachine.Stop();
            _logger.LogInformation("Emergency stop triggered. Arm set to Idle.");
            return Ok(new DTO.R("Arm stopped and set to Idle", 200).ToJsonString());
        }

        /// <summary>
        /// 示例：特定信号的方法，如Site1Arrival的设置信号
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpGet("Site1Arrival/{value}")]
        public IActionResult SetSite1Arrival(bool value)
        {
            return SetSignal(nameof(_signals.Site1Arrival), value);
        }

        /// <summary>
        /// 特定信号的方法，如获取Site1Arrival信号
        /// </summary>
        /// <returns></returns>
        [HttpGet("Site1Arrival")]
        public IActionResult GetSite1Arrival()
        {
            return GetSignal(nameof(_signals.Site1Arrival));
        }

        // 可以为其他信号继续添加专门的方法



        /// <summary>
        /// 处理 Site1NoDefect 信号，模拟在一号检测位无缺陷的情况，并发送脉冲信号给PLC，设置状态为Site2Ready
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpGet("Site1NoDefect/{value}")]
        public IActionResult SetSite1NoDefect(bool value)
        {
            if (value)
            {
                //无缺陷
                _signals.Site1NoDefect = true;
                // 发送脉冲信号
                _plcPulseService.SendPulse(DataType.Input, 0, 4, 0); // 示例地址
                //更新状态为 Site2Ready
                _armStateMachine.UpdateState();//因为watch函数已经更新了机械臂的状态为DetectionAtSite1，再进入状态机，即可转变状态
                Console.WriteLine($"机械臂目前状态(应为Site2Ready 3）: {_robotStatus.CurrentState}");
                return Ok(new R($"Site1NoDefect set to {value}, status set to Site2Ready", 200).ToJsonString());
            }
            else
            {
                return BadRequest(new R("Invalid value for Site1NoDefect", 400).ToJsonString());
            }

        }
        /// <summary>
        /// 处理 Site1Defect 信号，模拟在一号检测位有缺陷的情况，并发送脉冲信号给PLC，设置状态为Idle
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpGet("Site1Defect/{value}")]
        public IActionResult SetSite1Defect(bool value)
        {
            if (value)
            {
                // 设置 Site1Defect 信号为 true
                _signals.Site1Defect = true;
                // 发送脉冲信号
                _plcPulseService.SendPulse(DataType.Input, 0, 4, 1); // 示例地址
                // 更新状态为 Idle
                _armStateMachine.UpdateState(); // 更新状态
                Console.WriteLine($"机械臂目前状态(应为idle 0 ）: {_robotStatus.CurrentState}");
                return Ok(new R($"Site1Defect set to {value}, status set to Idle", 200).ToJsonString());
            }
            else
            {
                return BadRequest(new R("Invalid value for Site1Defect", 400).ToJsonString());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpGet("Site2NoDefect/{value}")]
        public IActionResult SetSite2NoDefect(bool value)
        {
            if (value)
            {
                //无缺陷
                _signals.Site2NoDefect = true;
                // 发送脉冲信号
                _plcPulseService.SendPulse(DataType.Input, 0, 4, 0); // 示例地址
                //更新状态为 idle
                _armStateMachine.UpdateState();//因为watch函数已经更新了机械臂的状态为DetectionAtSite2，再进入状态机，即可转变状态
                return Ok(new R($"Site2NoDefect set to {value}, status set to idle", 200).ToJsonString());
            }
            else
            {
                return BadRequest(new R("Invalid value for Site2NoDefect", 400).ToJsonString());
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpGet("Site2Defect/{value}")]
        public IActionResult SetSite2Defect(bool value)
        {
            if (value)
            {
                //有缺陷
                _signals.Site2Defect = true;
                // 发送脉冲信号
                _plcPulseService.SendPulse(DataType.Input, 0, 4, 1); // 示例地址
                //更新状态为 idle
                _armStateMachine.UpdateState();//因为watch函数已经更新了机械臂的状态为DetectionAtSite2，再进入状态机，即可转变状态
                return Ok(new R($"Site2Defect set to {value}, status set to idle", 200).ToJsonString());
            }
            else
            {
                return BadRequest(new R("Invalid value for Site2Defect", 400).ToJsonString());
            }

        }

    }
}
