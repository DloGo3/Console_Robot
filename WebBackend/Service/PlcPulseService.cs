using Microsoft.Extensions.Logging;
using S7.Net;

namespace WebBackend.Service
{
    /// <summary>
    /// 服务类，以便调用 SendPulse 方法。
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="plcService"></param>
    /// <param name="logger"></param>
    public class PlcPulseService(PlcService plcService, ILogger<PlcPulseService> logger)
    {
        private readonly PlcService _plcService = plcService;
        private readonly ILogger<PlcPulseService> _logger = logger;

        /// <summary>
        /// 发送脉冲
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="dbNumber"></param>
        /// <param name="startByte"></param>
        /// <param name="bitAdr"></param>
        public void SendPulse(DataType dataType, int dbNumber, int startByte, int bitAdr)
        {
            try
            {
                // 设定位为1（高电平）
                _plcService.WriteBit(dataType, dbNumber, startByte, bitAdr, true);
                System.Threading.Thread.Sleep(100); // 等待100毫秒（脉冲持续时间）

                // 设定位为0（低电平）
                _plcService.WriteBit(dataType, dbNumber, startByte, bitAdr, false);
                _logger.LogInformation("Pulse sent to {dataType}, DB{dbNumber}.{startByte}.{bitAdr}", dataType, dbNumber, startByte, bitAdr);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error sending pulse to {dataType}, DB{dbNumber}.{startByte}.{bitAdr}: {ErrorMessage}", dataType, dbNumber, startByte, bitAdr, ex.Message);
            }
        }
    }
}
