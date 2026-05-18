using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using WebBackend.Controller;
using WebBackend.Dao;
using WebBackend.Util;

namespace WebBackend.Service
{
    /// <summary>
    /// 用于管理和重新加载机器人配置数据的服务。
    /// </summary>
    public class RobotConfigurationService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<RobotConfigurationService> _logger;
        private readonly Util.Control _control;

        /// <summary>
        /// 初始化 <see cref="RobotConfigurationService"/> 类的新实例。
        /// </summary>
        /// <param name="configuration">应用程序的配置。</param>
        /// <param name="logger">用于记录日志的记录器。</param>
        /// <param name="control">机器人控制器。</param>
        public RobotConfigurationService(IConfiguration configuration, ILogger<RobotConfigurationService> logger, WebBackend.Util.Control control)
        {
            _configuration = configuration;
            _logger = logger;
            _control = control;
            RobotConfig = new RobotConfiguration();
            try
            {
                LoadData();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error loading RobotConfiguration: {Message}", ex.Message);
            }
            // 注册配置更改时回调以重新加载数据
            _configuration.GetReloadToken().RegisterChangeCallback(_ => LoadData(), null);
        }

        /// <summary>
        /// 从配置中加载数据。
        /// </summary>
        private void LoadData()
        {
            var configSection = _configuration.GetSection("RobotConfiguration");
            if (configSection.Exists())
            {
                RobotConfig = configSection.Get<RobotConfiguration>() ?? new RobotConfiguration();
                if (RobotConfig.Ip.Length == 0)
                {
                    _logger.LogWarning("RobotConfiguration section is missing in the configuration file.");
                }
                else
                {
                    bool ret = _control.SetGlobalSpeed(RobotConfig.GlobalSpeed);
                    ret = _control.LoadTool(RobotConfig.ToolId);
                    ret = _control.LoadUserCoord(RobotConfig.UserId);
                    if (ret)
                    {
                        _logger.LogInformation("Successfully loaded RobotConfiguration.");
                    }
                    else
                    {
                        _logger.LogWarning("Failed to load RobotConfiguration.");
                    }
                }
            }
            else
            {
                _logger.LogWarning("RobotConfiguration section is missing in the configuration file.");
            }
        }

        /// <summary>
        /// 获取或设置机器人配置。
        /// </summary>
        public IRobotConfiguration RobotConfig { get; private set; }
    }
}
