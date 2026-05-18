using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json.Serialization;
using WebBackend.DTO;
using WebBackend.Util;

namespace WebBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RobotConnectStatusController : ControllerBase
    {
        private readonly ILogger<RobotConnectStatusController> _logger;
        private readonly WebBackend.Util.Control _control;

        public RobotConnectStatusController(
            ILogger<RobotConnectStatusController> logger,
            WebBackend.Util.Control control)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _control = control ?? throw new ArgumentNullException(nameof(control));
        }

        [HttpGet]
        public IActionResult GetConnectionStatus()
        {
            try
            {
                var status = _control.GetRobotStatus();

                // 严格匹配12345为在线，其他值均不在线
                var (detail, statusCode) = status == 12345
                    ? ("机械臂在线", "connected")
                    : ("机械臂不在线", status.ToString());

                _logger.LogInformation($"连接状态码：{status} → {detail}");

                return Ok(new ApiResponse<ConnectionStatusData>(
                    code: 0,
                    data: new ConnectionStatusData
                    {
                        Status = statusCode,
                        Detail = detail
                    },
                    message: statusCode
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "状态查询失败");
                return StatusCode(500, new ApiResponse<ConnectionStatusData>(
                    code: 500,
                    data: new ConnectionStatusData { Status = "error" },
                    message: "查询异常"
                ));
            }
        }
    }

    public record ConnectionStatusData
    {
        [JsonPropertyName("status")]
        public string Status { get; init; } = "unknown";

        [JsonPropertyName("detail")]
        public string Detail { get; init; } = "未知状态";
    }
}
