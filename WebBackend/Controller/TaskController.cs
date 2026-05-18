using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using WebBackend.DTO;
using WebBackend.Service;

namespace WebBackend.Controller
{
    /// <summary>
    /// 任务控制器
    /// </summary>
    /// <param name="robotService">机器人服务</param>
    /// <param name="taskService">任务服务</param>
    /// <param name="taskProcessingService">任务处理服务</param>
    /// <param name="logger">日志记录器</param>
    [ApiController]
    [Route("[controller]")]
    public class TaskController(RobotService robotService, TaskService taskService, TaskProcessingService taskProcessingService, ILogger<TaskController> logger) : ControllerBase
    {
        private readonly RobotService _robotService = robotService;
        private readonly ILogger<TaskController> _logger = logger;
        private readonly TaskService _taskService = taskService;
        private readonly TaskProcessingService _taskProcessingService = taskProcessingService;


        /// <summary>
        /// 获取当前任务信息
        /// </summary>
        /// <returns>状态码为200时，若当前不存在任务则返回的task的Duration（持续时间）为-1，出现异常状态码返回500</returns>
        [HttpGet]
        public IActionResult GetCurrentTask()
        {
            try
            {
                return Ok(new R(_robotService.GetCurrentTask()).ToJsonString());
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured when getting current task: {Message}", ex.Message);
                return StatusCode(500, new R("An error occurred while processing your request. Check service log for more details.", 500).ToJsonString());
            }
        }

        /// <summary>
        /// 通过创建时间获取任务信息
        /// </summary>
        /// <param name="createTime">任务创建时间</param>
        /// <returns>
        /// <para>code=200: 返回任务信息</para>
        /// <para>code=500: 服务器内部异常</para>
        /// </returns>
        [HttpGet("create-time")]
        public IActionResult GetTaskInfoByCreateTime([FromQuery] long createTime)
        {
            try
            {
                var task = _taskService.GetTaskByCreateTime(createTime);
                if (_taskService.GetTaskByCreateTime(createTime) == null)
                {
                    return NotFound(new R("Task not found", 404).ToJsonString());
                }
                return Ok(new R(data: task).ToJsonString());
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured when getting task by create time: {Message}", ex.Message);
                return StatusCode(500, new R("An error occurred while processing your request. Check service log for more details.", 500).ToJsonString());
            }
        }

        /// <summary>
        /// 获取当前任务队列数量
        /// </summary>
        /// <returns>当前任务队列数量</returns>
        [HttpGet("count")]
        public IActionResult GetTaskCount()
        {
            return Ok(new R(_taskProcessingService.TaskCount, 200).ToJsonString());
        }

        /// <summary>
        /// 清空任务队列
        /// </summary>
        /// <returns>成功状态码为200</returns>
        [HttpPost("clear")]
        public IActionResult ClearTaskQueue()
        {
            _taskProcessingService.ClearTaskQueue();
            return Ok(new R(code: 200).ToJsonString());
        }

        /// <summary>
        /// 判断任务是否正在执行
        /// </summary>
        /// <returns>true为正在执行，false为未执行</returns>
        [HttpGet("is-executing")]
        public IActionResult IfTaskIsExecuting()
        {
            bool ret = _taskProcessingService.IsExecuting();
            return Ok(new R(data: ret).ToJsonString());
        }
    }
}
