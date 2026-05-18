using EstunApiStruct_CLI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using WebBackend.DTO;
using WebBackend.Util;

namespace WebBackend.Controllers
{
    /// <summary>
    /// 机械臂权限控制接口
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PermissionController : ControllerBase
    {
        private readonly ILogger<PermissionController> _logger;
        private readonly WebBackend.Util.Control _control;

        /// <summary>
        /// 查询当前机器人运动操作权限
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="control"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public PermissionController(
            ILogger<PermissionController> logger,
            WebBackend.Util.Control control)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _control = control ?? throw new ArgumentNullException(nameof(control));
        }

        /// <summary>
        /// 查询当前机器人运动操作权限
        /// </summary>
        [HttpGet]
        public IActionResult GetControlPermission()
        {
            try
            {
                // 调用Control中的CurrentPermit函数
                var permit = _control.CurrentPermit();

                // 根据逻辑生成响应
                return Ok(new
                {
                    code = 0,
                    message = "机械臂控制权限",
                    data = new
                    {
                        status = CheckPermission(permit) ? "connected" : "disconnected",
                        detail = CheckPermission(permit)
                            ? "已获得机械臂运动操作权限"
                            : "未获得机械臂运动操作权限"
                    },
                    timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                });
            }
            catch (Exception ex) when (ex is DllNotFoundException || ex is InvalidOperationException)
            {
                _logger.LogCritical(ex, "驱动通信异常（图片1中的头文件缺失）");
                return StatusCode(500, new
                {
                    code = 500,
                    message = "驱动通信失败",
                    data = new
                    {
                        status = "error",
                        detail = "无法连接权限服务"
                    },
                    timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "权限获取异常（图片3中的函数调用失败）");
                return StatusCode(500, new
                {
                    code = 500,
                    message = "权限服务异常",
                    data = new
                    {
                        status = "error",
                        detail = ex.Message
                    },
                    timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                });
            }
        }

        /// <summary>
        /// 权限验证逻辑
        /// </summary>
        private bool CheckPermission(E_ROB_PERMIT_CLI permit)
        {
            // 判断条件：m_mainctrlcode>1 且 m_timestamp>0 则有权限
            return permit.m_mainctrlcode > 1 && permit.m_timestamp > 0;
        }
    }
}
