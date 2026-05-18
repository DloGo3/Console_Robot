using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;

namespace WebBackend.Configuration
{
    /// <summary>
    /// MySQL数据库连接控制类
    /// </summary>
    /// <param name="connectionString">在appsettings.json中配置的MySQL数据库连接信息</param>
    /// <param name="logger">日志记录器</param>
    public class DatabaseAccess(string connectionString, ILogger<DatabaseAccess> logger)
    {
        private readonly string _connectionString = connectionString;
        private readonly ILogger<DatabaseAccess> _logger = logger;

        /// <summary>
        /// 连接数据库
        /// </summary>
        public void Connect()
        {
            using var connection = new MySqlConnection(_connectionString);
            try
            {
                connection.Open();
                _logger.LogInformation("Successfully connected to MySQL database");
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred connecting to MySQL: {Message}", ex.Message);
            }
        }

        /// <summary>
        /// 断开数据库连接
        /// </summary>
        public void Disconnect()
        {
            using var connection = new MySqlConnection(_connectionString);
            try
            {
                connection.Close();
                _logger.LogInformation("Successfully close connecting to MySQL database");
            }
            catch(Exception ex)
            {
                _logger.LogError("An error occurred closing MySQL: {Message}", ex.Message);
            }
        }
    }
}
