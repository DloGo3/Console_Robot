using Microsoft.AspNetCore.Mvc;
using static WebBackend.Service.RobotService;
using WebBackend.Service;

namespace WebBackend.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class CalibrationController : ControllerBase
    {
        private readonly RobotService _robotService;
        private readonly ILogger<CalibrationController> _logger;

        public CalibrationController(RobotService robotService, ILogger<CalibrationController> logger)
        {
            _robotService = robotService;
            _logger = logger;
        }

        [HttpPost("calibrate")]
        public IActionResult Calibrate([FromBody] CalibrationRequest request)
        {
            try
            {
                // 验证请求
                if (request == null)
                {
                    return BadRequest("Invalid request body.");
                }

                // 启动后台任务处理标定逻辑
                Task.Run(async () =>
                {
                    try
                    {
                        _logger.LogInformation("开始处理标定任务...");
                        int result = await _robotService.PerformCalibration(request);
                        if (result == 0)
                        {
                            _logger.LogInformation("标定完成：Calibration completed successfully.");
                        }
                        else
                        {
                            _logger.LogError($"标定失败，错误代码: {result}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"标定过程中发生异常: {ex.Message}");
                    }
                });

                // 立即返回响应
                return Ok("Calibration request received. Processing in the background.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"接收标定请求时发生错误: {ex.Message}");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
