using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using WebBackend.Dao;
using WebBackend.DTO;
using WebBackend.Service;

namespace WebBackend.Controller
{
    [Route("[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly TraceTypeService _traceTypeService;
        private readonly FileService _fileService;
        private readonly ILogger<TestController> _logger;
        private readonly TaskService _taskService;
        private readonly IApplicationData _applicationData;
        private readonly RobotService _robotService;
        private readonly TaskProcessingService _taskProcessingService;

        public TestController(
            TraceTypeService traceTypeService,
            FileService fileService,
            ILogger<TestController> logger,
            TaskService taskService,
            IApplicationData applicationData,
            RobotService robotService,
            TaskProcessingService taskProcessingService)
        {
            _traceTypeService = traceTypeService;
            _fileService = fileService;
            _logger = logger;
            _taskService = taskService;
            _applicationData = applicationData;
            _robotService = robotService;
            _taskProcessingService = taskProcessingService;
        }

        [HttpGet("trace-dict")]
        public IActionResult GetTraceList()
        {
            return Ok(new DTO.R(_traceTypeService.TraceTypes, 200).ToJsonString());
        }

        //[HttpPost("run-recursive")]
        //public IActionResult RunFullTraceRecursive([FromBody] TraceRequest request)
        //{
        //    List<Dao.Task> taskList = [];
        //    for (int i = 0; i < 10; i++)
        //    {
        //        try
        //        {
        //            // ================ 1. 加载ERD和ERP数据 ================
        //            bool boolRet = _fileService.LoadData(request.TraceName);
        //            if (!boolRet)
        //            {
        //                _logger.LogError("Number of points or commands is 0");
        //                return NotFound(new R("Number of points or commands is 0", 404).ToJsonString());
        //            }

        //            // ================ 2. 新建任务 ================
        //            Dao.Task task = new();
        //            int ret = _taskService.AddTask(task);
        //            if (ret < 0)
        //            {
        //                _logger.LogError("Failed to add task to database");
        //                return StatusCode(500, new R("Failed to add task to database", 500).ToJsonString());
        //            }
        //            taskList.Add(task);

        //            // 将任务添加到队列中
        //            _taskProcessingService.EnqueueTask(task);
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogError("An error occurred while processing your request: {m}", ex.Message);
        //            return StatusCode(500, new R("An error occurred while processing your request. Check service log for more details.", 500).ToJsonString());
        //        }
        //    }
        //    return Ok(new R(taskList, 200).ToJsonString());
        //}
    }
}
