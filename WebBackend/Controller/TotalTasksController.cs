using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebBackend.Dao;
using WebBackend.Service;
using WebBackend.DTO;
namespace WebBackend.Controller
{
    /// <summary>
    /// 定义 TotalTasksController 类,用于处理http请求
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    
    public class TotalTasksController : ControllerBase
    {
        private readonly TotalTasksService _totalTaskService;
        /// <summary>
        /// 构造函数2
        /// </summary>
        /// <param name="totalTaskService"></param>
        public TotalTasksController(TotalTasksService totalTaskService)
        {
            _totalTaskService = totalTaskService;
        }

        /// <summary>
        /// 获取所有任务的 HTTP GET 方法
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TotalTask>>> GetAllTotalTasks()
        {
            var totalTasks = await _totalTaskService.GetAllTotalTasksAsync();
            //return Ok(totalTasks);//返回 200 OK 状态码和任务列表。
            return Ok(new R(totalTasks, 200).ToJsonString());
        }

        /// <summary>
        /// 根据 ID 获取任务的 HTTP GET 方法
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<TotalTask>> GetTotalTask(int id)
        {
            var totalTask = await _totalTaskService.GetTotalTaskByIdAsync(id);
            if (totalTask == null)
            {
                return NotFound();
            }
            //return Ok(totalTask);
            return Ok(new R(totalTask, 200).ToJsonString());
        }

        /// <summary>
        /// 添加新任务的 HTTP POST 方法 接受一个 TotalTask 对象
        /// </summary>
        /// <param name="totalTask"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> AddTotalTask(TotalTask totalTask)
        {
            await _totalTaskService.AddTotalTaskAsync(totalTask);
            //返回 201 Created 状态码，并在响应中包含新创建任务的 URI 和任务对象
            //return CreatedAtAction(nameof(GetTotalTask), new { id = totalTask.Id }, totalTask);
            // 生成 Location URI
            string location = Url.Action(nameof(GetTotalTask), new { id = totalTask.Id });

            // 构建响应对象
            var response = new { Location = location, TotalTask = totalTask };

            // 使用 R 类包装响应数据
            var result = new R(response, 201);

            // 返回 JsonResult
            return new JsonResult(result) { StatusCode = 201 };
        }

        /// <summary>
        /// 更新任务的 HTTP PUT 方法（即数据库的update，接受任务 ID 和 TotalTask 对象）
        /// </summary>
        /// <param name="id"></param>
        /// <param name="totalTask"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateTotalTask(int id, TotalTask totalTask)
        {
            if (id != totalTask.Id)
            {
                return BadRequest(new R(404).ToJsonString());
            }
            await _totalTaskService.UpdateTotalTaskAsync(totalTask);
            return NoContent();//返回 204 No Content 状态码，表示请求成功但不返回任何内容
        }
        
        /// <summary>
        /// 删除任务的HTTP Delete方法
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTotalTask(int id)
        {
            await _totalTaskService.DeleteTotalTaskAsync(id);
            return NoContent();
        }
    }
}
