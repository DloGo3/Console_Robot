using Microsoft.AspNetCore.Mvc;
using WebBackend.Dao;
using WebBackend.Service;

namespace WebBackend.Controller
{
    [ApiController]
    [Route("[controller]")]
    public class InspectionController : ControllerBase
    {
        private readonly AutoOrManDetectService _detectService;
        private readonly ILogger<InspectionController> _logger;
        private readonly IApplicationData _applicationData;

        public InspectionController(AutoOrManDetectService detectService, ILogger<InspectionController> logger, IApplicationData applicationData)
        {
            _detectService = detectService;
            _logger = logger;
            _applicationData = applicationData;

        }

        /// <summary>
        /// 启动检测
        /// </summary>
        [HttpPost("start")]
        public async Task<IActionResult> StartInspection()
        {
            try
            {
                if (_applicationData.ModeState.CurrentMode != ControlMode.Manual)
                {
                    return BadRequest(new { error = "只能在手动模式下启动检测" });
                }

                //await _detectService.StartInspection();
                return Ok(new { message = "检测已启动" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning($"启动检测失败: {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"启动检测时发生错误: {ex}");
                return StatusCode(500, new { error = "启动检测失败" });
            }
        }

        /// <summary>
        /// 检查自动模式下是否可以开始检测
        /// </summary>
        //[HttpGet("can-start")]
        //public IActionResult CanStartAutomaticInspection()
        //{
        //    try
        //    {
        //        if (_detectService.IsFirstWorkpiece())
        //        {
        //            return Ok(new { message = "可以启动检测。" });
        //        }

        //        return BadRequest(new { message = "当前工作令号不符合要求，无法启动检测。" });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"检查检测启动条件失败: {ex.Message}");
        //        return StatusCode(500, new { message = $"检查检测启动条件失败: {ex.Message}" });
        //    }
        //}
    }
}
