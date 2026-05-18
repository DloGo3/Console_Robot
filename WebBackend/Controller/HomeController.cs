using Microsoft.AspNetCore.Mvc;

namespace WebBackend.Controller
{
    /// <summary>
    /// 访问测试Controller
    /// </summary>
    [ApiController]
    [Route("/")]
    public class HomeController : ControllerBase
    {
        /// <summary>
        /// 测试后端服务是否正常运行
        /// </summary>
        /// <returns>成功运行返回200HTTP状态码</returns>
        [HttpGet]
        public IActionResult Index()
        {
            return Ok();
        }
    }
}
