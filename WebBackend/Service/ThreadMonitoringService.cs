using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WebBackend.Service
{
    public class ThreadMonitoringService : BackgroundService
    {
        private readonly ILogger<ThreadMonitoringService> _logger;
        private const int MaxThreadCount = 2000;
        private const int LoggingIntervalMs = 5000;  // 调整为5秒检测一次
        private const int MaxThreadsToLog = 50;      // 最多记录50个线程详情

        public ThreadMonitoringService(ILogger<ThreadMonitoringService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Thread monitoring service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var threadCount = Process.GetCurrentProcess().Threads.Count;

                    if (threadCount > MaxThreadCount)
                    {
                        LogThreadDetails(threadCount);
                        await HandleThreadOverflow();  // 添加流控处理
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error monitoring threads");
                }

                await Task.Delay(LoggingIntervalMs, stoppingToken);
            }
        }

        private void LogThreadDetails(int currentCount)
        {
            try
            {
                var sb = new System.Text.StringBuilder();
                sb.AppendLine($"Threads exceeded! Current: {currentCount}/Max: {MaxThreadCount}");

                int loggedThreads = 0;
                foreach (ProcessThread thread in Process.GetCurrentProcess().Threads)
                {
                    if (loggedThreads++ >= MaxThreadsToLog) break;

                    try
                    {
                        var waitReason = thread.ThreadState == System.Diagnostics.ThreadState.Wait
                            ? thread.WaitReason.ToString()
                            : "N/A";

                        sb.AppendLine($"Thread {thread.Id}: {thread.ThreadState} [{waitReason}]");
                    }
                    catch (InvalidOperationException)
                    {
                        // 线程可能已退出
                        sb.AppendLine($"Thread {thread.Id}: [Unavailable]");
                    }
                }

                _logger.LogWarning(sb.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log thread details");
            }
        }

        private async Task HandleThreadOverflow()
        {
            // 示例：自动尝试释放资源
            _logger.LogWarning("Initiating thread cleanup...");

            // 这里可以添加具体的清理逻辑，例如：
            // 1. 终止非关键后台任务
            // 2. 释放空闲资源
            // 3. 触发GC回收
            await Task.Delay(100); // 模拟清理操作
        }
    }
}
