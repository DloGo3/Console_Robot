using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using WebBackend.Configuration;

namespace WebBackend.Service
{
    /// <summary>
    /// 数据库初始化类
    /// </summary>
    /// <param name="databaseAccess">数据库连接控制类（单例）</param>
    public class DatabaseInitializer(DatabaseAccess databaseAccess) : IHostedService
    {
        private readonly DatabaseAccess _databaseAccess = databaseAccess;

        /// <summary>
        /// 异步启动线程连接数据库
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>已完成的线程</returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _databaseAccess.Connect();
            return Task.CompletedTask;
        }

        /// <summary>
        /// 异步启动线程关闭数据库
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _databaseAccess.Disconnect();
            return Task.CompletedTask;
        }
    }
}
