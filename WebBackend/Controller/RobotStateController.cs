using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using WebBackend.Dao;
using WebBackend.DTO;
using WebBackend.Util;

namespace WebBackend.Controllers
{
    /// <summary>
    /// 机械臂状态接口
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class RobotStateController : ControllerBase
    {
        private readonly ILogger<RobotStateController> _logger;
        private readonly RobotStatus _robotStatus;
        private readonly IApplicationData _applicationData;

        public RobotStateController(
            ILogger<RobotStateController> logger,
            RobotStatus robotStatus,
            IApplicationData applicationData)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _robotStatus = robotStatus ?? throw new ArgumentNullException(nameof(robotStatus));
            _applicationData = applicationData ?? throw new ArgumentNullException(nameof(applicationData));
        }

        /// <summary>
        /// 获取机械臂当前状态
        /// </summary>
        /// <response code="200">返回当前机械臂状态</response>
        [HttpGet]
        public IActionResult GetCurrentState()
        {
            var state = DetermineState();
            _logger.LogInformation("机械臂当前状态：{state}", state);

            return Ok(new ApiResponse<StateData>(
                code: 0,
                data: new StateData
                {
                   
                    Detail = state
                },
                message: "机械臂状态"
            ));
        }

        private string DetermineState()
        {
            if (_applicationData.CurrentTraceType == "手动")
            {
                return "空闲";
            }
            // 优先级1：检查空闲状态
            if (_robotStatus.CurrentState == RobotStatus.Idle)
            {
                return "空闲";
            }

            // 优先级2：根据检测位置判断具体状态
            return _applicationData.DetectionPosition switch
            {
                "立式检测位" => "立式检测位检测中",
                "倾斜检测位" => "倾斜检测位检测中",
                _ => "空闲" // 当检测位置未知时的默认状态
            };
        }

        /// <summary>
        /// 状态数据实体
        /// </summary>
        public class StateData
        {
            [JsonPropertyName("status")]
            public string Status { get; init; } = "connected";

            [JsonPropertyName("detail")]
            public string Detail { get; set; } = "未知状态";
        }
    }
}
