using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebBackend.Dao;
using WebBackend.DTO;
using WebBackend.Service;

namespace WebBackend.Controller
{
    /// <summary>
    /// 定义 SubTasksController 类，用于处理 HTTP 请求
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SubTasksController : ControllerBase
    {
        //要使用service，就要声明
        private readonly SubTasksService _subTaskService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="subTaskService"></param>
        public SubTasksController(SubTasksService subTaskService)
        {
            _subTaskService = subTaskService;
        }

        /// <summary>
        /// 获取所有子任务的 HTTP GET 方法  
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //返回一个 Task 对象
        //任务完成后返回一个 ActionResult，包含一个 IEnumerable<SubTask> 对象。
        //ActionResult：表示一个 HTTP 响应，可以包含一个状态码和响应数据。
        //IEnumerable<SubTask>：表示一个 SubTask 对象的 集合。
        public async Task<ActionResult<IEnumerable<SubTask>>> GetAllSubTasks()
        {
            var subTasks = await _subTaskService.GetAllSubTasksAsync();
            return Ok(new R(subTasks, 200).ToJsonString());
        }

        /// <summary>
        /// 根据 ID 获取子任务的 HTTP GET 方法
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<SubTask>> GetSubTask(int id)
        {
            var subTask = await _subTaskService.GetSubTaskByIdAsync(id);
            if (subTask == null)
            {
                return NotFound();
            }
            return Ok(new R(subTask, 200).ToJsonString());
        }

        /// <summary>
        /// 添加新子任务的 HTTP POST 方法
        /// </summary>
        /// <param name="subTask"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> AddSubTask(SubTask subTask)
        {
            await _subTaskService.AddSubTaskAsync(subTask);
            //返回 201 Created 状态码，并在响应中包含新创建子任务的 URI 和子任务对象。
            // CreatedAtAction(nameof(GetSubTask), new { id = subTask.Id }, subTask);

            // 生成 Location URI
            string location = Url.Action(nameof(GetSubTask), new { id = subTask.Id });

            // 构建响应对象
            var response = new { Location = location, SubTask = subTask };

            // 使用 R 类包装响应数据
            var result = new R(response, 201);

            // 返回 JsonResult
            return new JsonResult(result) { StatusCode = 201 };
        }

        /// <summary>
        /// 更新子任务的 HTTP PUT 方法
        /// </summary>
        /// <param name="id"></param>
        /// <param name="subTask"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateSubTask(int id, SubTask subTask)
        {
            if (id != subTask.Id)
            {
                return BadRequest(new R(404).ToJsonString());
            }
            await _subTaskService.UpdateSubTaskAsync(subTask);

            return NoContent(); //返回 204 No Content 状态码，表示请求成功但不返回任何内容。
        }

        /// <summary>
        /// 删除子任务的 HTTP DELETE 方法
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSubTask(int id)
        {
            await _subTaskService.DeleteSubTaskAsync(id);
            return NoContent();
            
        }
    }
}
