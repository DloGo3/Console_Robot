using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using WebBackend.DTO;
using WebBackend.Util;
namespace WebBackend.Controllers
{
    /// <summary>
    /// 机械臂错误状态接口（符合埃斯顿SDK规范5.11.3.27-28）
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")] // 强制声明响应格式
    public class ErrorController : ControllerBase
    {
        private readonly ILogger<ErrorController> _logger;
        private readonly WebBackend.Util.Control _control;
        private readonly JsonSerializerOptions _jsonOptions;

        public ErrorController(
            ILogger<ErrorController> logger,
            WebBackend.Util.Control control)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _control = control ?? throw new ArgumentNullException(nameof(control));

            // 配置JSON序列化选项
            _jsonOptions = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        /// <summary>
        /// 获取当前机械臂错误状态
        /// </summary>
        /// <response code="200">返回错误状态</response>
        /// <response code="406">不支持的请求格式</response>
        /// <response code="500">系统内部错误</response>
        [HttpGet]
        public IActionResult GetErrorStatus()
        {
            try
            {
                

                // 获取错误信息
                var (errorId, errorDetail) = GetErrorInfo();

                return Ok(new ApiResponse<ErrorResponse>(
                    code: 0,
                    data: new ErrorResponse
                    {
                        Status = errorId == 0 ? "normal" : "error",
                        Detail = errorDetail
                    },
                    message: errorId.ToString()
                ));
            }
            catch (NotSupportedException ex)
            {
                _logger.LogError(ex, "请求格式错误");
                return StatusCode(406, new ApiResponse<object>(
                    code: 406,
                    data: null,
                    message: "仅支持application/json格式请求"
                ));
            }
            catch (Exception ex) when (ex is InvalidOperationException or DllNotFoundException)
            {
                _logger.LogCritical(ex, "底层驱动通信异常");
                return StatusCode(500, new ApiResponse<object>(
                    code: 500,
                    data: new { Status = "error" },
                    message: "驱动通信失败"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "错误状态查询失败");
                return StatusCode(500, new ApiResponse<object>(
                    code: 500,
                    data: new { Status = "error" },
                    message: "状态查询异常"
                ));
            }
        }

        private (int errorId, string errorDetail) GetErrorInfo()
        {
            var errorId = _control.GetErrorId();
            var rawDetail = errorId == 0 ? string.Empty : _control.GetErrorInfo(errorId);

            return (errorId, ConvertEncoding(rawDetail, "GB2312", "UTF-8"));
        }

        private string ConvertEncoding(string input, string from, string to)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            try
            {
                var fromEncoding = Encoding.GetEncoding(from);
                var toEncoding = Encoding.GetEncoding(to);
                var bytes = fromEncoding.GetBytes(input);
                return toEncoding.GetString(bytes);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "编码转换失败");
                return input;
            }
        }



        /// <summary>
        /// 错误响应数据结构（强类型）
        /// </summary>
        public class ErrorResponse
        {
            [JsonPropertyName("status")]
            public string Status { get; set; }

            [JsonPropertyName("detail")]
            public string Detail { get; set; }
        }
    }
}
