using Microsoft.AspNetCore.Mvc;
using WebBackend.Dao;
using WebBackend.DTO;
using WebBackend.Service;

namespace WebBackend.Controller
{
    /// <summary>
    /// 展示当前机械臂工作模式（标定、手动（默认）、半自动、自动）
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ModeController : ControllerBase
    {
        private readonly IApplicationData _applicationData;
        private readonly ILogger<ModeController> _logger;
        private readonly PlcService _plcService;
        private readonly AutoOrManDetectService _autoOrManDetectService;


        /// <summary>
        /// 构造函数，注入全局应用数据和日志
        /// </summary>
        public ModeController(IApplicationData applicationData, ILogger<ModeController> logger, PlcService plcService, AutoOrManDetectService autoOrManDetectService)
        {
            _applicationData = applicationData;
            _logger = logger;
            _plcService = plcService;
            _autoOrManDetectService = autoOrManDetectService;
        }

        /// <summary>
        /// 获取当前机械臂工作模式
        /// </summary>
        /// <response code="200">返回标准格式的工作模式信息</response>
        /// <response code="500">服务器内部错误</response>
        [HttpGet]
        public IActionResult GetCurrentMode()
        {
            try
            {
                var mode = GetFormattedMode();
                _logger.LogInformation("当前工作模式查询成功: {Mode}", mode);

                return Ok(new ApiResponse<RobotModeData>(
                    code: 0,
                    data: new RobotModeData
                    {
                        Status ="connected",
                        Detail = mode
                    },
                    message: "当前工作模式"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "工作模式查询失败");
                return StatusCode(500, new ApiResponse<RobotModeData>(
                    code: 500,
                    data: new RobotModeData { Status = "error" },
                    message: "服务器内部错误"
                ));
            }
        }
        /// <summary>
        /// 获取格式化后的工作模式
        /// </summary>
        private string GetFormattedMode()
        {
            // 处理空值和空格
            var traceType = _applicationData.CurrentTraceType?.Trim() ?? "";

            return traceType switch
            {
                "标定" => "标定",
                "半自动" => "半自动",
                "全自动" => "自动",
                "手动" => "手动",// 内部类型转换为对外暴露类型
                _ => "手动"   // 默认值
            };
        }

    }
}   
        
