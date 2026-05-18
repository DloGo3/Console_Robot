using WebBackend.Service;
using Microsoft.AspNetCore.Mvc;

namespace WebBackend.Controller
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("api/signal")]
    public class SignalController : ControllerBase
    {
        private readonly SignalWatchService signalWatchService;

        public SignalController(SignalWatchService signalWatchService)
        {
            this.signalWatchService = signalWatchService;
        }

        //[HttpGet("value")]
        //public IActionResult GetSignalValue([FromQuery] string name)
        //{
        //    var value = signalWatchService.GetSignalValue(name);
        //    if (value == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(value);
        //}
    }
}
