namespace WebBackend.Dao
{
    /// <summary>
    /// .NET后端配置信息
    /// </summary>
    public class ServerConfiguration
    {
        /// <summary>
        /// IP地址
        /// </summary>
        public string? Host { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }
    }
}
