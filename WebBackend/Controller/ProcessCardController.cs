using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebBackend.Service;
using WebBackend.Dao;
using WebBackend.DTO;

namespace WebBackend.Controller
{
    /// <summary>
    /// 设置获取工艺卡ID
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProcessCardController : ControllerBase
    {
        private readonly IApplicationData _applicationData;
        private readonly RobotStatus _robotStatus;


        public ProcessCardController(IApplicationData applicationData, RobotStatus robotStatus)
        {
            _applicationData = applicationData;
            _robotStatus = robotStatus;
        }

        // GET: api/ProcessCard/{id}
        // 获取特定的工艺卡ID
        [HttpGet]
        public IActionResult GetProcessCardId()
        {
            return Ok(new { ProcessCardId = _applicationData.ProcessCardId });
        }




        // POST: api/ProcessCard
        // 设置新的工艺卡ID
        [HttpPost]
        public IActionResult SetProcessCardId([FromBody] int processCardId)
        {
            _applicationData.ProcessCardId = processCardId;
            _robotStatus.CurrentState = RobotStatus.Site1Ready;
            return Ok(new { Message = "Process Card ID updated successfully.", ProcessCardId = processCardId });
        }

    }
}
