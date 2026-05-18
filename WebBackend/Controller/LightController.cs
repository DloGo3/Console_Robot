using BX_struct_space;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using WebBackend.Service;

namespace WebBackend.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class LightController : ControllerBase
    {
        private readonly HardwareService _hardwareService;

        public LightController(HardwareService hardwareService)
        {
            _hardwareService = hardwareService;
        }

        // 读取通道基本参数
        [HttpGet("parameters/{channel}")]
        public async Task<IActionResult> ReadParameters(int channel)
        {
            try
            {
                // 发送读取命令
                string command = _hardwareService.GetLEDTestCommand(channel);
                var response = await _hardwareService.SendCommandAsync(command);

                // 解析响应数据
                if (response.bOK && response.strComm == "B")
                {
                    var basicParams = (_stuGenerSoftCh)response.list_oContent[0];
                    return Ok(new
                    {
                        Mode = basicParams.eMode.ToString(),
                        CurrentMax = basicParams.nCurrentMax,
                        Brightness = basicParams.nBright
                    });
                }
                return BadRequest("Failed to read parameters.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        // 连接光源（精准匹配图片中的JSON格式）
        [HttpPost("connect")]
        public async Task<IActionResult> Connect([FromBody] ConnectRequest request)
        {
            try
            {
                // 直接映射图片中的JSON字段名（IP全大写）
                await _hardwareService.ConnectAsync(request.IP, request.Port);
                return Ok("Connected successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 请求模型类：严格匹配图片中的大写的"IP"和"Port"
        public class ConnectRequest
        {
            // 关键修改：JsonPropertyName值必须与图片中的JSON完全一致（区分大小写！）
            [JsonPropertyName("IP")]
            public string IP { get; set; }

            [JsonPropertyName("port")] // 根据实际输入决定是否改为Port
            public int Port { get; set; }
        }
    }
}
