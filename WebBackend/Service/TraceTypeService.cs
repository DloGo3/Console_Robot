namespace WebBackend.Service
{
    /// <summary>
    /// 用于管理和重新加载 YAML 配置数据的服务。
    /// </summary>
    public class TraceTypeService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TraceTypeService> _logger;

        /// <summary>
        /// 初始化 <see cref="TraceTypeService"/> 类的新实例。
        /// </summary>
        /// <param name="configuration">应用程序的配置。</param>
        /// <param name="logger">日志记录器。</param>
        public TraceTypeService(IConfiguration configuration, ILogger<TraceTypeService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            TraceTypes = [];
            try
            {
                LoadData();
            }
            catch (Exception ex){
                _logger.LogError("Error loading TraceTypeService: {Message}", ex.Message);
            }
            // 注册配置更改时回调以重新加载数据
            _configuration.GetReloadToken().RegisterChangeCallback(_ => LoadData(), null);
        }

        /// <summary>
        /// 从配置中加载数据。
        /// </summary>
        private void LoadData()
        {
            // 将配置部分 "TraceTypeDict" 解析为字典
            TraceTypes = _configuration.GetSection("TraceTypeDict").Get<Dictionary<string, string>>() ?? throw new Exception("TraceTypeDict configuration is not correctly set in appsettings.json");
        }

        /// <summary>
        /// 获取或设置轨迹类型的字典。
        /// </summary>
        public Dictionary<string, string> TraceTypes { get; private set; }

    }

}
