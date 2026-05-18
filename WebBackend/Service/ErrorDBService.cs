//using Dapper;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;
//using MySqlConnector;
//using System.Threading;
//namespace WebBackend.Service
//{
//    /// <summary>
//    /// 后台服务 把错误信息写到迪威尔数据库
//    /// </summary>
//    public class ErrorDBService : BackgroundService
//    {
//        private readonly IConfiguration _config;
//        private readonly ErrorService _errorService;
//        private readonly ILogger<ErrorDBService> _logger;
//        private readonly PeriodicTimer _timer;

//        public ErrorDBService(
//            IConfiguration config,
//            ErrorService errorService,
//            ILogger<ErrorDBService> logger)
//        {
//            _config = config;
//            _errorService = errorService;
//            _logger = logger;
//            _timer = new PeriodicTimer(TimeSpan.FromSeconds(200));
//        }

//        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//        {
//            while (await _timer.WaitForNextTickAsync(stoppingToken))
//            {
//                try
//                {
//                    var errorInfo = _errorService.GetErrorInfo();

//                    using var conn = new MySqlConnection(
//                        _config.GetConnectionString("ErrorLogs"));

//                    await conn.ExecuteAsync(
//                        @"INSERT INTO ErrorLogs (ErrorCode, ErrorMessage) 
//                        VALUES (@ErrorCode, @ErrorMessage)",
//                        new { errorInfo.ErrorCode, errorInfo.ErrorMessage });

//                    _logger.LogInformation("成功写入错误日志：{Code}", errorInfo.ErrorCode);
//                }
//                catch (Exception ex)
//                {
//                    _logger.LogError(ex, "数据库写入失败");
//                }
//            }
//        }

//        public override void Dispose()
//        {
//            _timer.Dispose();
//            base.Dispose();
//        }
//    }
//}
