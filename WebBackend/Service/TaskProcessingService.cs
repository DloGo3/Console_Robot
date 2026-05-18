using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Crypto.Modes.Gcm;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using WebBackend.Dao;
using WebBackend.DTO;

namespace WebBackend.Service
{
    /// <summary>
    /// 后台服务类，负责处理任务队列中的任务。
    /// </summary>
    public class TaskProcessingService : BackgroundService
    {
        private readonly ILogger<TaskProcessingService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentQueue<Dao.Task> _taskQueue = new();

        /// <summary>
        /// 初始化 <see cref="TaskProcessingService"/> 类的新实例。
        /// </summary>
        /// <param name="logger">用于记录日志的日志记录器实例。</param>
        /// <param name="serviceProvider">用于创建作用域和解析服务的服务提供者。</param>
        public TaskProcessingService(ILogger<TaskProcessingService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 将任务加入队列中等待处理。
        /// </summary>
        /// <param name="task">要加入队列的任务实例。</param>
        public void EnqueueTask(Dao.Task task)
        {
            _taskQueue.Enqueue(task);
            _logger.LogInformation("Task enqueued: {task.ErpAbsolutePath}, {task.ErdAbsolutePath}", task.ErpAbsolutePath, task.ErdAbsolutePath);
        }

        /// <summary>
        /// 清空所有队列中的任务。
        /// </summary>
        public void ClearTaskQueue()
        {
            _taskQueue.Clear();
        }

        /// <summary>
        /// 获取当前任务队列中的任务数量。
        /// </summary>
        /// <returns>队列中的任务数量。</returns>
        public int TaskCount => _taskQueue.Count;

        /// <summary>
        /// 立即停止所有任务的执行。
        /// </summary>
        public void EmergencyStop()
        {
            using var scope = _serviceProvider.CreateScope();
            var robotService = scope.ServiceProvider.GetRequiredService<RobotService>();
            var applicationData = scope.ServiceProvider.GetRequiredService<IApplicationData>();

            var stopwatch = applicationData.StopWatch;

            stopwatch.Stop();
            robotService.Pause(500);
            _logger.LogWarning("Emergency stop activated. Task {a} at {b} has been halted.", applicationData.CurrentTask.TraceType.Type, applicationData.CurrentTask.TraceType.Position);
        }

        /// <summary>
        /// 继续执行当前任务。
        /// </summary>
        public void TaskContinue()
        {
            System.Threading.Tasks.Task.Run(() =>
            {
                using var scope = _serviceProvider.CreateScope();
                var robotService = scope.ServiceProvider.GetRequiredService<RobotService>();
                var applicationData = scope.ServiceProvider.GetRequiredService<IApplicationData>();
                var taskService = scope.ServiceProvider.GetRequiredService<TaskService>();
                try
                {
                    var stopwatch = applicationData.StopWatch;
                    stopwatch.Start();
                    robotService.ContinueMove();
                    stopwatch.Stop();
                    applicationData.CurrentTask.Duration = stopwatch.ElapsedMilliseconds;
                    applicationData.CurrentTask.IsCompleted = true;
                    int ret = taskService.UpdateTaskByCreateTime(applicationData.CurrentTask);
                    if (ret < 0)
                    {
                        _logger.LogError("Failed to update task to database");
                        throw new Exception("Failed to update task to database");
                    }
                    // 让当前任务的创建时间为-1，表示当前没有任务
                    applicationData.CurrentTask.CreateTime = -1;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "处理任务时发生错误。");
                    // 让当前任务的创建时间为-1，表示当前没有任务
                    applicationData.CurrentTask.CreateTime = -1;
                }
            });
        }

        /// <summary>
        /// 执行后台服务，从队列中持续处理任务。每一秒自动运行
        /// </summary>
        /// <param name="stoppingToken">用于停止任务执行的取消令牌。</param>
        /// <returns>表示后台执行的异步操作。</returns>
        protected override async System.Threading.Tasks.Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var applicationData = scope.ServiceProvider.GetRequiredService<IApplicationData>();
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_taskQueue.TryDequeue(out var task)) /*task不是none的话，即任务队列中还有任务，即true，若没有即false*/
                {
                    ProcessTask(task, stoppingToken);
                }
                else
                {
                    await System.Threading.Tasks.Task.Delay(1000, stoppingToken); //false的话等待1s，再次运行这个函数
                }
            }
        }

        /// <summary>
        /// 处理单个任务。
        /// </summary>
        /// <param name="task">待处理的任务。</param>
        /// <param name="stoppingToken">取消令牌，用于停止处理。</param>
        private void ProcessTask(Dao.Task task, CancellationToken stoppingToken)
        {
            // 创建一个范围，以便在处理任务时解析服务
            using var scope = _serviceProvider.CreateScope();
            var taskService = scope.ServiceProvider.GetRequiredService<TaskService>();
            var robotService = scope.ServiceProvider.GetRequiredService<RobotService>();
            var fileService = scope.ServiceProvider.GetRequiredService<FileService>();
            var applicationData = scope.ServiceProvider.GetRequiredService<IApplicationData>();

            // 判断当前是否有正在执行的任务，如果没有正在执行的任务则开始执行新任务
            if (applicationData.CurrentTask.CreateTime < 0)
            {
                try
                {
                    //TODO loadData改为传入一对erderp的绝对路径 ture则加载成功
                    bool boolRet = fileService.LoadData(task.ErpAbsolutePath,task.ErdAbsolutePath);
                    if (!boolRet)
                    {
                        _logger.LogError("Number of points or commands is 0.");
                        throw new Exception("Number of points or commands is 0.");
                    }

                    _logger.LogInformation("开始处理任务。");
                    _logger.LogInformation("任务创建时间：{m}", DateTimeOffset.FromUnixTimeMilliseconds(task.CreateTime).ToLocalTime().ToString("yyyy-MM-dd-HH-mm-ss", CultureInfo.InvariantCulture));
                    _logger.LogInformation("任务持续时间: {m}", task.Duration);
                    _logger.LogInformation("轨迹类型ID：{m}", task.TraceTypeId);
                    _logger.LogInformation("任务类型：{m}", task.TraceType.Type);
                    _logger.LogInformation("任务位置：{m}", task.TraceType.Position);
                    _logger.LogInformation("任务版本：{m}", task.TraceType.Version);

                    // 设置当前任务
                    applicationData.CurrentTask = task;
                    // 格式化文件夹名称
                    var createTime = DateTimeOffset.FromUnixTimeMilliseconds(task.CreateTime).ToLocalTime().ToString("yyyy-MM-dd-HH-mm-ss", CultureInfo.InvariantCulture);
                    var traceType = task.TraceType;
                    var picturesDir = $"{traceType.Type}-{traceType.Position}-{traceType.Version}-{createTime}";
                    // 向采集服务器发送http请求，设置当前任务类型-开始时间为采集图片存放的文件夹
                    taskService.SetPicturesDir(picturesDir);
                    // 更新当前图片文件夹名称
                    applicationData.CurrentPictureFolderName = picturesDir;

                    var stopwatch = applicationData.StopWatch;

                    // 开始计时
                    stopwatch.Restart();

                    // 执行所有命令 核心
                    int res = robotService.RunAllCommand();

                    stopwatch.Stop();

                    task.Duration = stopwatch.ElapsedMilliseconds;
                    task.IsCompleted = true;
                    int ret = taskService.UpdateTaskByCreateTime(task);
                    if (ret < 0)
                    {
                        _logger.LogError("Failed to update task to database");
                        throw new Exception("Failed to update task to database");
                    }

                    // 让当前任务的创建时间为-1，表示当前没有任务
                    applicationData.CurrentTask.CreateTime = -1;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "处理任务时发生错误。");
                    // 让当前任务的创建时间为-1，表示当前没有任务
                    applicationData.CurrentTask.CreateTime = -1;
                }
            }

        }

        internal bool IsExecuting()
        {
            var scope = _serviceProvider.CreateScope();
            var applicationData = scope.ServiceProvider.GetRequiredService<IApplicationData>();
            return applicationData.CurrentTask.CreateTime >= 0;
        }


    }
}
