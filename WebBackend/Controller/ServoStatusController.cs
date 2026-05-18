using EstunApiStruct_CLI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json.Serialization;
using WebBackend.DTO;
using WebBackend.Util;

namespace WebBackend.Controllers
{
    /// <summary>
    /// 机械臂使能状态控制器（基于埃斯顿API实现）
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ServoStatusController : ControllerBase
    {
        private readonly ILogger<ServoStatusController> _logger;
        private readonly WebBackend.Util.Control _control;

        public ServoStatusController(
            ILogger<ServoStatusController> logger,
            WebBackend.Util.Control control)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _control = control ?? throw new ArgumentNullException(nameof(control));
        }

        /// <summary>
        /// 获取机械臂使能状态
        /// </summary>
        /// <response code="200">返回当前使能状态</response>
        /// <response code="500">伺服状态查询异常</response>
        [HttpGet]
        public IActionResult GetServoStatus()
        {
            try
            {
                // 调用底层API获取状态
                var status = _control.GetServoOn();

                // 转换为接口需要的状态描述
                var (statusCode, detail, originalValue) = status switch
                {
                    E_ServoStatusType_CLI.ServoOff => ("unable", "机械臂未使能", 0),
                    E_ServoStatusType_CLI.ServoOn => ("connected", "机械臂已使能", 1),
                    E_ServoStatusType_CLI.errStatus => ("error", "机械臂使能错误", 3),
                    _ => ("unknown", $"未知状态（原始值:{(int)status}）", (int)status)
                };

                _logger.LogInformation("伺服状态查询成功：{detail} (原始值:{originalValue})", detail, originalValue);

                return Ok(new ApiResponse<ServoStatusData>(
                    code: 0,
                    data: new ServoStatusData
                    {
                        Status = statusCode,
                        Detail = detail
                    },
                    message: statusCode
                ));
            }
            catch (Exception ex) when (ex is InvalidOperationException || ex is DllNotFoundException)
            {
                _logger.LogCritical(ex, "底层驱动异常");
                return StatusCode(500, new ApiResponse<ServoStatusData>(
                    code: 500,
                    data: new ServoStatusData { Status = "error" },
                    message: "驱动通信失败"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "伺服状态查询失败");
                return StatusCode(500, new ApiResponse<ServoStatusData>(
                    code: 500,
                    data: new ServoStatusData { Status = "error" },
                    message: "状态查询异常"
                ));
            }
        }
    }

    /// <summary>
    /// 伺服状态数据实体（与Control类强耦合）
    /// </summary>
    public record ServoStatusData
    {
        /// <summary>
        /// 状态码（与E_ServoStatusType_CLI枚举对应）
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; init; } = "-1";

        /// <summary>
        /// 状态描述（中文）
        /// </summary>
        [JsonPropertyName("detail")]
        public string Detail { get; init; } = "未知状态";
    }
}
