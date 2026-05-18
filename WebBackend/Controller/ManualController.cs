using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebBackend.Service;
using System.Text;
using System.IO;

namespace WebBackend.Controller
{
    /// <summary>
    /// 手动存点位信息
    /// </summary>
    [Route("api/v1/robot/[controller]")]
    [ApiController]
    public class ManualController : ControllerBase
    {
        private readonly ManualService _manualService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="manualService"></param>
        public ManualController(ManualService manualService)
        {
            _manualService = manualService;
        }

        /// <summary>
        /// 保存名字为pointName的点位信息到全局数据中
        /// </summary>
        /// <param name="pointName">点位标识</param>
        /// <returns>HTTP200状态码</returns>
        [HttpGet("save-point-info")]
        public IActionResult SaveCurrentPointInfo(string pointName)
        {
            _manualService.SaveCurrentPointInfo(pointName);
            return Ok();
        }

        /// <summary>
        /// 返回当前已保存的点位信息JSON字符串
        /// </summary>
        /// <returns>当前已保存的所有点位信息</returns>
        [HttpGet("point-info")]
        public IActionResult GetPointsInfo()
        {
            return Ok(new DTO.R(data: _manualService.GetPointsInfo()).ToJsonString());
        }

        /// <summary>
        /// 清空所有已保存的点位信息
        /// </summary>
        /// <returns>HTTP200状态码</returns>
        [HttpGet("clear-point-info")]
        public IActionResult ClearPointsInfo()
        {
            _manualService.ClearPointsInfo();
            return Ok();
        }

        /// <summary>
        /// 下载当前已保存的点位信息为JSON文件
        /// </summary>
        /// <returns>保存好当前已保存的点位信息</returns>
        [HttpGet("download-point-info")]
        public IActionResult DownloadPointsInfo()
        {
            var jsonString = new DTO.R(data: _manualService.GetPointsInfo()).ToJsonString();
            var fileName = "manual-modified-points.json";
            var fileBytes = Encoding.UTF8.GetBytes(jsonString);

            return File(fileBytes, "application/json", fileName);
        }
    }
}
