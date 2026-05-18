namespace WebBackend.Dao
{
    /// <summary>
    /// 日志数据库配置类
    /// </summary>
    public class SeqDatabaseSettings
    {
        /// <summary>
        /// Seq数据库运行Host
        /// </summary>
        public string Host {  get; set; }
        /// <summary>
        /// Seq数据库运行端口
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// Controller层的Seq数据库的Api Key
        /// </summary>
        public string ControllerLogApiKey { get; set; }
        /// <summary>
        /// 非Controller层的Seq数据库的Api Key
        /// </summary>
        public string NonControllerLogApiKey { get; set; }
        /// <summary>
        /// 无参构造，默认Port为5341，其他字段为空字符串
        /// </summary>
        public SeqDatabaseSettings()
        {
            Host = "";
            Port = 5341;
            ControllerLogApiKey = "";
            NonControllerLogApiKey = "";
        }
    }
}
