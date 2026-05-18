using Microsoft.Extensions.Configuration;

namespace WebBackend.Dao
{
    /// <summary>
    /// PLC配置接口
    /// </summary>
    public interface IPlcConfiguration
    {
        /// <summary>
        /// CPU类型，如S71200，S71500等
        /// </summary>
        string CpuType { get; set; }
        /// <summary>
        /// IP地址，如192.168.1.201
        /// </summary>
        string IpAddress { get; set; }
        /// <summary>
        /// PLC运行端口
        /// </summary>
        string Port { get; set; }
    }

    /// <summary>
    /// PLC配置类
    /// </summary>
    public class PlcConfiguration : IPlcConfiguration
    {
        /// <summary>
        /// CPU类型，如S71200，S71500等
        /// </summary>
        public string CpuType { get; set; }
        /// <summary>
        /// IP地址，如192.168.1.201
        /// </summary>
        public string IpAddress { get; set; }
        /// <summary>
        /// PLC运行端口
        /// </summary>
        public string Port { get; set; }
        /// <summary>
        /// 无参构造函数，默认每个值都为空字符串
        /// </summary>
        public PlcConfiguration()
        {
            CpuType = "";
            IpAddress = "";
            Port = "";
        }
    }
}
