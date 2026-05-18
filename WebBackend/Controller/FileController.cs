using Microsoft.AspNetCore.Mvc;
using WebBackend.Dao;
using WebBackend.Service;
using WebBackend.DTO;
using Newtonsoft.Json;

namespace WebBackend.Controller
{
    /// <summary>
    /// 文件相关的Controller
    /// </summary>
    /// <param name="fileService">文件服务（单例）</param>
    /// <param name="applicationData">应用数据</param>
    /// <param name="logger">日志记录器</param>
    [ApiController]
    [Route("[controller]")]
    public class FileController(FileService fileService, IApplicationData applicationData, ILogger<FileController> logger) : ControllerBase
    {
        private readonly FileService _fileService = fileService;
        private readonly ILogger<FileController> _logger = logger;
        private readonly IApplicationData _applicationData = applicationData;

        /// <summary>
        /// 返回轨迹名称列表
        /// </summary>
        /// <returns>成功获取返回轨迹名称列表，code为200，数据为空返回code404，出现异常返回500</returns>
        [HttpGet("list")]
        public IActionResult GetWorkpieceTrajectoryList()
        {
            try
            {
                var dataList = _fileService.GetTraceList();
                if (dataList == null || dataList.Count == 0) // 没有数据也算失败
                {
                    _logger.LogWarning("No trace data found!");
                    return NotFound(new R("No data found.", 404).ToJsonString());
                }

                return Ok(new R(dataList,  200).ToJsonString());
            }
            catch
            {
                _logger.LogError("An error occurred while processing your request! Check service log for more details.");
                return StatusCode(500, new R("An error occurred while processing your request. Check service log for more details.", 500).ToJsonString());
            }
        }

        /// <summary>
        /// 加载轨迹文件
        /// </summary>
        /// <param name="request">包含轨迹名的DTO对象</param>
        /// <returns></returns>
        //[HttpPost("trace")]
        //public IActionResult LoadData([FromBody] TraceRequest request)
        //{
        //    try
        //    {
        //        bool boolRet = _fileService.LoadData(request.TraceName);
        //        if (!boolRet)
        //        {
        //            _logger.LogError("Trace data load failed! One or more file contains no data.");
        //            return NotFound(new R("Trace data load failed! One or more file contains no data.", 404).ToJsonString());
        //        }
        //        else
        //        {
        //            _logger.LogInformation("Trace data loaded successfully. All points num: {AllPointsNum}, command num: {CommandNum}, point-to-be-detected num: {Num}",
        //                _applicationData.PosDict.Count, _applicationData.CommandList.Count, _applicationData.PointsToBeDetected.Count);
        //            return Ok(new R(data: "Trace data loaded successfully.").ToJsonString());
        //        }
        //    }
        //    catch (FileNotFoundException e)
        //    {
        //        _logger.LogError("Trace name is not valid, check again!");
        //        _logger.LogError("{m}", e.Message);
        //        return BadRequest(new R("Trace name is not valid, check again!", 400).ToJsonString());
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("{m}", ex.Message);
        //        return StatusCode(500, new R("Server error!", 500).ToJsonString());
        //    }
        //}
    }
}
