using RabbitMQ.Client;
using S7.Net;
using Serilog;
using WebBackend.Dao;
using WebBackend.Service;
using Microsoft.OpenApi.Models;
using WebBackend.Configuration;
using Serilog.Filters;  // 关键命名空间
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.Extensions.Hosting;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

// 添加Swagger服务
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Robot Control RESTful API", Version = "v1" });
});

// 添加配置项（热加载）
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
    .AddYamlFile("config.yaml", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// 注册 DatabaseAccess 服务
builder.Services.AddSingleton<DatabaseAccess>(sp =>
    {
        var logger = sp.GetRequiredService<ILogger<DatabaseAccess>>();
        try
        {
            var databaseAccess = new DatabaseAccess(builder.Configuration.GetConnectionString("MySqlConnection") ?? throw new Exception("Error occured when connecting to MySQL Database"), sp.GetRequiredService<ILogger<DatabaseAccess>>());
            return databaseAccess;
        }
        catch (Exception ex)
        {
            logger.LogError("{Message}", ex.Message);
            // 出错返回默认写死的连接
            return new DatabaseAccess("server=192.168.1.100;port=3306;database=robot_control;uid=root;pwd=123456;", logger);
        }
    });

// 注册 DatabaseInitializer 服务
builder.Services.AddHostedService<DatabaseInitializer>();

// 绑定配置到 SeqDatabaseSettings 类
var seqConfig = new SeqDatabaseSettings();
builder.Configuration.GetSection("SeqDatabaseSettings").Bind(seqConfig);
// 配置 Serilog 日志记录
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Error)
    // 只包含对应级别日志
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Debug)
        .WriteTo.File("logs/debug/debug.log", outputTemplate: "{Timestamp:yyyyMMdd_HHmm} {Level:u3} {Message:lj}{NewLine}{Exception}")
    )
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Information)
        .WriteTo.File("logs/info/info.log", outputTemplate: "{Timestamp:yyyyMMdd_HHmm} {Level:u3} {Message:lj}{NewLine}{Exception}")
    )
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Error)
        .WriteTo.File("logs/error/error.log", outputTemplate: "{Timestamp:yyyyMMdd_HHmm} {Level:u3} {Message:lj}{NewLine}{Exception}")
    )
    // 你的 Seq 配置保留
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(e => e.Properties.ContainsKey("SourceContext") && e.Properties["SourceContext"].ToString().Contains("Controller"))
        .Enrich.WithProperty("ApiKey", $"{seqConfig.ControllerLogApiKey}")
        .WriteTo.Seq($"http://{seqConfig.Host}:{seqConfig.Port}", apiKey: $"{seqConfig.ControllerLogApiKey}")
    )
    .WriteTo.Logger(lc => lc
        .Filter.ByExcluding(e => e.Properties.ContainsKey("SourceContext") && e.Properties["SourceContext"].ToString().Contains("Controller"))
        .Enrich.WithProperty("ApiKey", $"{seqConfig.NonControllerLogApiKey}")
        .WriteTo.Seq($"http://{seqConfig.Host}:{seqConfig.Port}", apiKey: $"{seqConfig.NonControllerLogApiKey}")
    )
    .CreateLogger();


// 注册 TraceTypeService 作为单例服务
builder.Services.AddSingleton<TraceTypeService>();

//注册 SubtasksService 作为单例服务
builder.Services.AddSingleton<SubTasksService>();

//注册 TotaltasksService 作为单例服务
builder.Services.AddSingleton<TotalTasksService>();

//注册 Signals 作为单例服务
builder.Services.AddSingleton<Signals>();

//注册 RobotStatus 作为单例服务
builder.Services.AddSingleton<RobotStatus>();

//注册 ArmStateMachine 作为单例服务
builder.Services.AddSingleton<ArmStateMachine>();

builder.Services.AddSingleton<SignalWatchService>();
// 注册单例服务（硬件连接全局唯一）
builder.Services.AddSingleton<HardwareService>();
//注册 AutoOrManDetectService 作为单例服务
builder.Services.AddSingleton<AutoOrManDetectService>();
//注册 AutoOrManDetectService 为后台服务
builder.Services.AddHostedService<AutoOrManDetectService>();
//注册FileDownloadService 为后台服务
builder.Services.AddSingleton<FileDownloadService>();
// 错误码上传至迪威尔服务器
builder.Services.AddSingleton<ErrorService>();
//builder.Services.AddHostedService<ErrorDBService>();
builder.Services.AddSingleton<LightSourceService>();
builder.Services.AddSingleton<WorkOrderNumberDao>();



builder.Services.AddSingleton<ProcessCardService>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("MySqlConnection");
    return new ProcessCardService(connectionString);
});

builder.Services.AddSingleton<ProcessCardDao>(provider =>
{
    // 获取 IConfiguration 服务
    var configuration = provider.GetRequiredService<IConfiguration>();
    // 从配置文件中获取连接字符串
    string connectionString = configuration.GetConnectionString("MySqlConnection");
    // 返回 ProcessCardDao 实例并注入连接字符串
    return new ProcessCardDao(connectionString);
});





builder.Services.AddSingleton<NewTraceService>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("MySqlConnection");
    var applicationData = sp.GetRequiredService<IApplicationData>();
    return new NewTraceService(connectionString, applicationData);
});



builder.Services.AddControllers();

// 注册 PLC Simulator Controller
builder.Services.AddControllers(); // 确保添加控制器服务

builder.Services.AddSingleton<TraceService>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("MySqlConnection");
    return new TraceService(connectionString);
});
// 注册 SignalMonitorService 作为后台服务
//builder.Services.AddHostedService<SignalMonitorService>();

// 注册PlcPulseService更新依赖注入
builder.Services.AddSingleton<PlcPulseService>();
builder.Services.AddSingleton<SemiAutoService>();


// 注册 TaskService 作为单例服务
builder.Services.AddSingleton(sp => {
    var logger = sp.GetRequiredService<ILogger<TaskService>>();
    var connectionString = builder.Configuration.GetConnectionString("MySqlConnection")?? "server=192.168.1.100;port=3306;database=robot_control;uid=root;pwd=123456;";
    if(connectionString.Length == 0)
    {
        logger.LogWarning("No connection string found for TaskService, using default connection string");
    }
    return new TaskService(connectionString, logger);
});

// 绑定配置到 RobotConfiguration 类
var robotConfig = new RobotConfiguration();
builder.Configuration.GetSection("RobotConfiguration").Bind(robotConfig);
// 注册 RobotConfiguration 实例
builder.Services.AddSingleton(robotConfig);

// 注册 RobotConfigurationService 作为单例服务
builder.Services.AddSingleton<RobotConfigurationService>();    

// 依赖注入->创建新的消息队列连接
builder.Services.AddSingleton(sp =>
{
    var factory = new ConnectionFactory() { HostName = "localhost" };
    return factory.CreateConnection();
});
// 依赖注入->创建新的消息队列通道
builder.Services.AddScoped<IModel>(sp =>
{
    var connection = sp.GetRequiredService<IConnection>();
    return connection.CreateModel();
});

// 依赖注入->存储保存全局数据
builder.Services.AddSingleton<IApplicationData, ApplicationData>();

// 依赖注入->文件解析器
builder.Services.AddSingleton<WebBackend.Util.Parser>();

// 依赖注入->注入服务单例
builder.Services.AddSingleton<FileService>();

// RobotService相关依赖注入
builder.Services.AddSingleton<WebBackend.Util.Control>();
builder.Services.AddSingleton<RobotService>();


// PointService相关依赖注入
builder.Services.AddSingleton<PointService>();

// ManualService注入
builder.Services.AddSingleton<ManualService>();

// PlcService相关注入
builder.Services.AddSingleton(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var logger = sp.GetRequiredService<ILogger<PlcService>>();
    try
    {
        // 加载PLC配置类
        var plcConfig = config.GetSection("PlcSettings").Get<PlcConfiguration>() ?? throw new Exception("PLC configuration is not correctly set in config.yaml");
        // 解析枚举值
        CpuType cpuType = Enum.Parse<CpuType>(plcConfig.CpuType, true);
        return new PlcService(cpuType, plcConfig.IpAddress, logger);
    }
    catch (Exception ex)
    {
        logger.LogError("Error configuring PlcService: {Message}", ex.Message);
        throw;
    }
});

// 任务流处理服务注入
// 注册 TaskProcessingService 作为单例服务
builder.Services.AddSingleton<TaskProcessingService>();

// 注册 TaskProcessingService 作为后台服务
builder.Services.AddHostedService(provider => provider.GetRequiredService<TaskProcessingService>());

// 注册 ThreadMonitoringService 作为后台服务
builder.Services.AddHostedService<ThreadMonitoringService>();

// 把Controllers作为服务
builder.Services.AddControllers();

// 配置CORS跨域
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("http://127.0.0.1:8080", "http://127.0.0.1:8081")
                          .AllowAnyMethod()
                          .AllowAnyHeader());
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
            );
});

var app = builder.Build();


// 读取后端配置项信息
var config = new ServerConfiguration();
builder.Configuration.GetSection("ServerSettings").Bind(config);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Robot Control RESTful API");
    });
}

// CORS 使用 AllowSpecificOrigin 选项
app.UseCors("AllowAll");

// Map controller routes
app.MapControllers();

// 设置线程池最小线程数
ThreadPool.SetMinThreads(workerThreads: 100, completionPortThreads: 100);

app.Run();

