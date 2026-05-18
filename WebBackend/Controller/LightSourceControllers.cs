using Microsoft.AspNetCore.Mvc;
using BX_struct_space;
using WebBackend.Service;

namespace WebBackend.Controller
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class LightSourceController : ControllerBase
    {
        private readonly LightSourceService _lightSourceService;
        public LightSourceController(LightSourceService lightSourceService)
        {
            _lightSourceService = lightSourceService;
        }

        public class StuGenerSoftChDto
        {
            public int nCh { get; set; }
            public int nChTrig { get; set; }
            public string eMode { get; set; }
            public double nCurrentMax { get; set; }
            public int nBright { get; set; }
            public double nSynChDelay { get; set; }
            public double nLightDelay { get; set; }
            public double nLightTime { get; set; }
        }

        [HttpGet]
        public async Task<IActionResult> GetChannelParam(int channel)
        {
            var param = await _lightSourceService.ReadChannelParametersAsync(channel);
            if (param == null)
                return StatusCode(504, "读取超时或失败");
            var dto = new StuGenerSoftChDto
            {
                nCh = param.Value.nCh,
                nChTrig = param.Value.nChTrig,
                eMode = param.Value.eMode.ToString(),
                nCurrentMax = param.Value.nCurrentMax,
                nBright = param.Value.nBright,
                nSynChDelay = param.Value.nSynChDelay,
                nLightDelay = param.Value.nLightDelay,
                nLightTime = param.Value.nLightTime,
            };
            return Ok(dto);
        }

        /// <summary>
        /// 设置当前通道的最大电流（需先调用 GetChannelParam 触发一次读取）
        /// </summary>
        [HttpPost]
        public IActionResult SetCurrentMax([FromQuery] double currentMax)
        {
            bool result = _lightSourceService.ChangeCurrentMaxAndSet(currentMax);
            return result ? Ok("已下发") : BadRequest("还未读取过通道参数，无法设置");
        }
    }
}
