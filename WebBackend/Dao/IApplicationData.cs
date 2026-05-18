using WebBackend.Util;
using EstunApiStruct_CLI;
using System.Collections.Concurrent;
using System.Collections.Generic;
using WebBackend.DTO;
using static WebBackend.Dao.IApplicationData;
using static WebBackend.Service.RobotService;

namespace WebBackend.Dao
{
    /// <summary>
    /// 前端传过来的轨迹
    /// </summary>
    public class Trace
    {
        public int TraceId { get; set; }
        public string Name { get; set; }
        public string ErdFilePath { get; set; }
        public string ErpFilePath { get; set; }
        public string Type { get; set; }
    }
    
    /// <summary>
    /// 控制手动/自动枚举类
    /// </summary>
    public enum ControlMode
    {
        /// <summary>
        /// 手动模式
        /// </summary>
        Manual,
        /// <summary>
        /// 自动模式
        /// </summary>
        Automatic
    }

    /// <summary>
    /// 状态类
    /// </summary>
    public class ModeState
    {
        /// <summary>
        /// 初始化为自动
        /// </summary>
        public ControlMode CurrentMode { get; set; } = ControlMode.Automatic;
    }
    /// <summary>
    /// 接口：存储全局信息，是一种抽象的数据模型，不包含任何具体的实现
    /// </summary>
    public interface IApplicationData
    {
        /// <summary>
        /// 半自动下的机械臂状态
        /// </summary>
        string SemiCurrentState { get; set; }
        /// <summary>
        /// 轨迹列表的专用线程锁
        /// </summary>
        object TracesLock { get; }

        /// <summary>
        /// 手动、半自动、全自动、标定模型下各自的轨迹开始执行时间
        /// 存一个时间戳，每次执行任务(手动、半自动、全自动、标定)之前更新该字段，执行完后设置为0;
        /// </summary>
        long BeginTime { get; set; }

        /// <summary>
        /// 标定粗糙度的值
        /// </summary>
        double RoughnessValue { get; set; }
        /// <summary>
        /// 当前轨迹一共多少个检测点位
        /// </summary>
        int CurrentTraceDetectionPoints { get; set; }

        /// <summary>
        /// 当前检测的是第几个产品
        /// </summary>
        int CurrentProductIndex { get; set; }
        /// <summary>
        /// 当前轨迹正在执行第几个检测点位
        /// </summary>
        int CurrentDetectionPointIndex { get; set; }
        /// <summary>
        /// 工艺卡需要执行的一共多少条轨迹
        /// </summary>
        int TotalTracesInProcessCard { get; set; }
        /// <summary>
        /// 当前检测位置一共多少条轨迹（立式/倾斜）
        /// </summary>
        int TotalTracesInCurrentPosition { get; set; }
        /// <summary>
        /// 正在执行当前位置的第几条轨迹
        /// </summary>
        int CurrentTraceIndex { get; set; }
        /// <summary>
        /// 正在执行的轨迹名称
        /// </summary>
        string CurrentTraceName { get; set; } 
        /// <summary>
        /// 检测位置：立式/倾斜
        /// </summary>
        string DetectionPosition { get; set; } 
        /// <summary>
        /// 轨迹类型"半自动" “全自动”“标定”
        /// </summary>
        string CurrentTraceType { get; set; } 
        /// <summary>
        /// 保存前端传递的标定数据
        /// </summary>
        CalibrationRequest CalibrationData { get; set; } 
        /// <summary>
        /// 当前拍摄的次数索引
        /// </summary>
        int CurrentPhotoIndex { get; set; }  

        /// <summary>
        /// 当前控制模式状态
        /// </summary>
        ModeState ModeState { get; set; }


        /// <summary>
        /// Zone数据
        /// </summary>
        ConcurrentDictionary<string, E_ROB_ZONE_CLI> ZoneDict { get; set; }
        /// <summary>
        /// Speed数据
        /// </summary>
        ConcurrentDictionary<string, E_ROB_SPEED_CLI> SpeedDict { get; set; }
        /// <summary>
        /// 保存点位信息
        /// </summary>
        ConcurrentDictionary<string, RobotPosition> PosDict { get; set; }
        /// <summary>
        /// 指令列表
        /// </summary>
        List<Command> CommandList { get; set; }
        /// <summary>
        /// 任务列表
        /// </summary>
        ConcurrentQueue<Task> TaskQueue { get; set; }
        /// <summary>
        /// 当前正在执行的指令
        /// </summary>
        Task CurrentTask { get; set; }
        /// <summary>
        ///  待检测点
        /// </summary>
        List<RobotPosition> PointsToBeDetected { get; set; }
        /// <summary>
        /// 当前正在执行的指令索引
        /// </summary>
        int CurrentCommandIndex { get; set; }
        /// <summary>
        /// 全局计时器
        /// </summary>
        System.Diagnostics.Stopwatch StopWatch { get; set; }
        /// <summary>
        /// 当前已检测点数量
        /// </summary>
        int CurrentDetectedPointsNum { get; set; }
        /// <summary>
        /// 当前图片文件夹名称
        /// </summary>
        string CurrentPictureFolderName { get; set; }
        /// <summary>
        /// 手动保存的点位信息
        /// </summary>
        List<Tuple<string, E_ROB_POS_CLI>> ManuallySavedPoints { get; set; }
      
        //int processCardId { get; set; }

        /// <summary>
        /// 存储当前机械臂所在点位名称
        /// </summary>
        // 工艺卡ID
        int ProcessCardId { get; set; }

        // 工件ID
        long WorkpieceId { get; set; }

        // 工件模型的下载路径
        string WorkpieceModelPath { get; set; }

        // STCP 文件的下载路径
        string StcpPath { get; set; }

        // 保存轨迹信息
        List<Trace> Traces { get;  }

        string CurrentPointName { get; set; }
        /// <summary>
        /// 工作令号
        /// </summary>
        long WorkOrderNumber { get; set; }



    }

    /// <summary>
    /// 实现类：存储全局信息
    /// </summary>
    public class ApplicationData : IApplicationData
    {
        /// <summary>
        /// 半自动模型下的机械臂状态
        /// </summary>
        public string SemiCurrentState { get; set; } = "空闲";
        /// <summary>
        /// 轨迹列表的专用线程锁
        /// </summary>
        public object TracesLock { get; } = new object();

        public long BeginTime { get; set; } = 0;
        public double RoughnessValue { get; set; } = 0;

        public int CurrentTraceDetectionPoints { get; set; } = 0;
        public int CurrentProductIndex { get; set; } = 0;
        public int CurrentDetectionPointIndex { get; set; } = 0;
        public int TotalTracesInProcessCard { get; set; } = 0;
        public int TotalTracesInCurrentPosition { get; set; } = 0;
        public int CurrentTraceIndex { get; set; } = 0;
        public string CurrentTraceName { get; set; } = string.Empty;
        /// <summary>
        /// 检测位置默认值为空字符串
        /// </summary>
        public string DetectionPosition { get; set; } = string.Empty; 

        /// <summary>
        /// 轨迹类型默认设置为空字符串
        /// </summary>
        public string CurrentTraceType { get; set; } = string.Empty;
        /// <summary>
        /// 工作令号
        /// </summary>
        public long WorkOrderNumber { get; set; }
        /// <summary>
        /// 标定数据
        /// </summary>
        public CalibrationRequest CalibrationData { get; set; } 
        /// <summary>
        /// 当前拍摄的次数索引
        /// </summary>
        public int CurrentPhotoIndex { get; set; } 
        // <summary>
        /// 当前控制模式状态
        /// </summary>
        public ModeState ModeState { get; set; } = new ModeState();

        // 构造函数中可以省略初始化 ModeState，因为已经有默认值

        /// <summary>
        /// 现在的模式
        /// </summary>
        public ControlMode CurrentMode { get; set; } = ControlMode.Automatic;
        /// <summary>
        /// 轨迹数据文件夹根目录（在config.yaml中设置）
        /// </summary>
        private readonly string _dataDirectory;
        /// <summary>
        /// 文件解析器
        /// </summary>
        private readonly Parser _parser;

        /// <summary>
        /// Zone数据
        /// </summary>
        public ConcurrentDictionary<string, E_ROB_ZONE_CLI> ZoneDict { get; set; }
        /// <summary>
        /// Speed数据
        /// </summary>
        public ConcurrentDictionary<string, E_ROB_SPEED_CLI> SpeedDict { get; set; }
        /// <summary>
        /// 点位信息
        /// </summary>
        public ConcurrentDictionary<string, RobotPosition> PosDict { get; set; }
        /// <summary>
        /// 指令列表
        /// </summary>
        public List<Command> CommandList { get; set; }
        /// <summary>
        /// 任务队列
        /// </summary>
        public ConcurrentQueue<Task> TaskQueue { get; set; }
        /// <summary>
        /// 当前正在执行的任务
        /// </summary>
        public Task CurrentTask { get; set; }
        /// <summary>
        ///  待检测点
        /// </summary>
        public List<RobotPosition> PointsToBeDetected { get; set; }
        /// <summary>
        /// 当前正在执行的指令
        /// </summary>
        public int CurrentCommandIndex { get; set; }
        /// <summary>
        /// 全局计时器
        /// </summary>
        public System.Diagnostics.Stopwatch StopWatch { get; set; }
        /// <summary>
        /// 当前已检测点数量
        /// </summary>
        public int CurrentDetectedPointsNum { get; set; }
        /// <summary>
        /// 当前图片文件夹名称
        /// </summary>
        public string CurrentPictureFolderName { get; set; }
        /// <summary>
        /// 手动保存的点位信息
        /// </summary>
        public List<Tuple<string, E_ROB_POS_CLI>> ManuallySavedPoints { get; set; }

        private ILogger<ApplicationData> _logger;
        /// <summary>
        /// 设置获取工艺卡ID
        /// </summary>
        //public int processCardId { get; set; }
        /// <summary>
        /// 存储当前机械臂所在点位名称
        /// </summary>
        public int ProcessCardId { get; set; }
        public long WorkpieceId { get; set; }
        public string WorkpieceModelPath { get; set; }
        public string StcpPath { get; set; }
        // 显式初始化 Traces
        public List<Trace> Traces { get; } = new List<Trace>();

        public string CurrentPointName { get; set; }


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="config">从config.yaml来的配置信息</param>
        /// <param name="parser">注入文件解析器</param>
        /// <param name="logger">日志记录器</param>
        /// <exception cref="Exception">解析Zone或者Speed变量出错时抛出该异常</exception>
        public ApplicationData(IConfiguration config, Parser parser, ILogger<ApplicationData> logger)
        {
            _dataDirectory = config.GetValue<string>("DataDirectory") ?? "";
            string systemErdFilePath = Path.Combine(_dataDirectory, "system.erd");
            _parser = parser;
            _logger = logger;

            PosDict = new ConcurrentDictionary<string, RobotPosition>();
            CommandList = [];
            ZoneDict = new ConcurrentDictionary<string, EstunApiStruct_CLI.E_ROB_ZONE_CLI>();
            SpeedDict = new ConcurrentDictionary<string, EstunApiStruct_CLI.E_ROB_SPEED_CLI>();
            TaskQueue = new ConcurrentQueue<Task>();
            CurrentTask = new Task();
            CurrentTask.CreateTime = -1;
            PointsToBeDetected = [];
            ManuallySavedPoints = [];
            CurrentCommandIndex = -1;
            CurrentDetectedPointsNum = -1;
            CurrentPictureFolderName = "";
            CurrentPointName = "";

            StopWatch = new System.Diagnostics.Stopwatch();
            StopWatch.Restart();

            // 将Zone变量和Speed变量添加到内存中
            try
            {
                _parser.ParseSystemErdFileToDict(systemErdFilePath, ZoneDict, SpeedDict);
                _logger.LogInformation("Load Zone and Speed data successfully.");
            }
            catch (Exception e)
            {
                throw new Exception($"Error occured when parsing system.erd file: {e.Message}");
            }
        }

    }
}
