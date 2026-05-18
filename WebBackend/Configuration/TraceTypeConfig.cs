namespace WebBackend.Configuration
{
    /// <summary>
    /// 轨迹类型配置类
    /// </summary>
    public class TraceTypeConfig
    {
        /// <summary>
        /// 轨迹类型字典
        /// </summary>
        public Dictionary<string, string> TraceTypeDict { get; set; }

        /// <summary>
        /// 无参构造函数，初始化TraceTypeDict为一个空字典
        /// </summary>
        public TraceTypeConfig()
        {
            TraceTypeDict = new();
        }
        
    }
}
