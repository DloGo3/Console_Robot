using Microsoft.AspNetCore.Mvc;
using WebBackend.Dao;
using WebBackend.DTO;
using WebBackend.Service;

namespace WebBackend.Controller
{
    /// <summary>
    /// 处理由PLC控制和转发的信号的Controller
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class PlcController : ControllerBase
    {
        private readonly PlcService _plcService;
        private readonly ILogger<PlcController> _logger;
        private readonly IApplicationData _appData;

        /// <summary>
        /// 全参构造函数，注入两个服务
        /// </summary>
        /// <param name="plcService">PLC业务逻辑类</param>
        /// <param name="logger">日志记录器</param>
        public PlcController(PlcService plcService, ILogger<PlcController> logger,IApplicationData applicationData)
        {
            this._plcService = plcService;
            this._logger = logger;
            this._appData = applicationData;
        }

        /// <summary>
        /// 测试是否连上PLC
        /// </summary>
        /// <returns>
        /// <para>200: PLC 已连接</para>
        /// <para>400: PLC 未连接</para>
        /// <para>500: 服务器异常</para>
        /// </returns>
        [HttpGet("test-connection")]
        public IActionResult TestConnection()
        {
            try
            {
                if (_plcService.Plc.IsConnected)
                {
                    return Ok(new R("PLC is connected!", 200).ToJsonString());
                }
                else
                {
                    return BadRequest(new R("PLC is not connected!", 400).ToJsonString());
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new R($"Connection test failed: {ex.Message}", 500).ToJsonString());
            }
        }


        /// <summary>
        /// 向PLC发送拍照信号
        /// </summary>
        /// <param name="milliseconds">拍照信号高电平持续时间（不要太短就行）</param>
        /// <returns>
        /// <para>code:</para>
        /// <para>200: 成功写入PLC</para>
        /// <para>500: 服务器异常</para>
        /// </returns>
        [HttpGet("take-photo")]
        public IActionResult TakePhoto(int milliseconds)
        {

            try
            {
                _plcService.Plc.Open();
                _plcService.TakePhoto(milliseconds);
                _logger.LogInformation("Photo taken successfully!");
                return Ok(new R(code: 200).ToJsonString());
            }
            catch
            {
                return StatusCode(500, new R($"PLC write failed!", 500).ToJsonString());
            }
        }

        [HttpGet("take-photo-manual")]
        public IActionResult TakePhotoManual(int milliseconds)
        {
            _appData.CurrentTraceType = "手动";
            _appData.BeginTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            try
            {
                _plcService.Plc.Open();
                _plcService.TakePhoto(milliseconds);
                _logger.LogInformation("Photo taken successfully!");
                return Ok(new R(code: 200).ToJsonString());
            }
            catch
            {
                return StatusCode(500, new R($"PLC write failed!", 500).ToJsonString());
            }
            finally
            {
                // 无论成功或失败，最终重置时间为 "0"
                _appData.BeginTime = 0;
                _logger.LogInformation("手动拍照完成，BeginTime 已重置");
            }
        }
        /// <summary>
        /// 从 PLC 读取 WorkOrderNumber 数据
        /// </summary>
        /// <param name="dbNumber">数据块编号</param>
        /// <param name="startByteAddress">起始字节地址</param>
        /// <returns>
        /// <para>200: 返回读取的工作令号数据</para>
        /// <para>500: 服务器异常</para>
        /// </returns>
        [HttpGet("read-workordernum/{dbNumber}/{startByteAddress}")]
        public IActionResult ReadWorkOrderNumber(int dbNumber, int startByteAddress)
        {
            try
            {
                var workOrderNumber = _plcService.ReadWorkOrderNumber(dbNumber, startByteAddress);
                return Ok(workOrderNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error reading WorkOrderNumber: {Message}", ex.Message);
                return StatusCode(500, new R($"Error reading WorkOrderNumber: {ex.Message}", 500).ToJsonString());
            }
        }

        /// <summary>
        /// 向 PLC 写入 WorkOrderNumber 数据
        /// </summary>
        /// <param name="dbNumber">数据块编号</param>
        /// <param name="startByteAddress">起始字节地址</param>
        /// <param name="workOrderNumber">要写入的 WorkOrderNumber 对象</param>
        /// <returns>
        /// <para>200: 写入成功</para>
        /// <para>500: 写入失败</para>
        /// </returns>
        [HttpPost("write-workordernum/{dbNumber}/{startByteAddress}")]
        public IActionResult WriteWorkOrderNumber(int dbNumber, int startByteAddress, [FromBody] WorkOrderNumber workOrderNumber)
        {
            try
            {
                _plcService.WriteWorkOrderNumber(dbNumber, startByteAddress, workOrderNumber);
                return Ok(new R("WorkOrderNumber written successfully!", 200).ToJsonString());
            }
            catch (Exception ex)
            {
                _logger.LogError("Error writing WorkOrderNumber: {Message}", ex.Message);
                return StatusCode(500, new R($"Error writing WorkOrderNumber: {ex.Message}", 500).ToJsonString());
            }
        }



        /// <summary>
        /// 打开PLC连接,用于主动发起与 PLC 的连接
        /// </summary>
        /// <returns>
        /// <para>code:</para>
        /// <para>200: 成功连接</para>
        /// <para>400: 未连接</para>
        /// <para>500: 服务器异常</para>
        /// </returns>
        [HttpGet("connect")]
        public IActionResult Connect2PLC()
        {
            try
            {
                _plcService.Plc.Open();
                if (_plcService.Plc.IsConnected)
                {
                    return Ok(new R(code: 200).ToJsonString());
                }
                else
                {
                    return BadRequest(new R("PLC is not connected!", 400).ToJsonString());
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new R($"PLC connect failed: {ex.Message}", 500).ToJsonString());
            }
        }
    }
}
