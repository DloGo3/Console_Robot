using Microsoft.AspNetCore.Mvc;
using WebBackend.DTO;
using WebBackend.Service;

namespace WebBackend.Controller
{

    /// <summary>
    /// 指令控制器
    /// </summary>
    /// <param name="fileService">文件服务</param>
    [ApiController]
    [Route("[controller]")]
    public class CommandController(FileService fileService) : ControllerBase
    {
        private readonly FileService _fileService = fileService;

        /// <summary>
        /// 获取当前指令数量
        /// </summary>
        /// <returns>指令数量</returns>
        [HttpGet("count")]
        public IActionResult GetCommandCount()
        {
            return Ok(new R(data: _fileService.GetCommandCount()).ToJsonString());
        }
    }
}
