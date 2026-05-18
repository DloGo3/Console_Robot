using Microsoft.AspNetCore.Mvc;
using WebBackend.Dao;
using WebBackend.Service;
using WebBackend.DTO;
using Task = System.Threading.Tasks.Task;
using Microsoft.Extensions.Logging;
using System;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Threading.Tasks;

namespace WebBackend.Controller 
{
    /// <summary>
    /// 机器人控制、状态获取相关的Controller
    /// </summary>
    /// <param name="robotService">机器人服务（单例）</param>
    /// <param name="logger">日志记录器</param>
    /// <param name="taskService">任务业务逻辑类</param>
    /// <param name="taskProcessingService">任务处理服务</param>
    [ApiController]  
    [Route("[controller]")] 

    public class RobotController(RobotService robotService, ILogger<RobotController> logger, TaskService taskService, TaskProcessingService taskProcessingService, TraceService traceService ) : ControllerBase
    {   //定义了一个名为RobotController的公共类，它继承自ControllerBase。在ASP.NET Core的MVC或Web API项目中，控制器类通常会继承ControllerBase
        //控制器通过构造函数接受四个参数：RobotService, ILogger<RobotController>, TaskService, 和 TaskProcessingService。
        //这些参数通常是通过依赖注入（Dependency Injection, DI）提供的。构造函数内部，这些参数被赋值给类的私有字段，以便在类的其他方法中使用。
        //四个私有字段（_robotService, _logger, _taskService, _taskProcessingService）用于存储通过构造函数注入的服务。
        //RobotService _robotService 私有只读字段，用于存储RobotService实例:robotService
        private readonly RobotService _robotService = robotService;  //  将传入的robotService实例赋值给私有字段
        private readonly ILogger<RobotController> _logger = logger;
        private readonly TaskService _taskService = taskService;
        private readonly TaskProcessingService _taskProcessingService = taskProcessingService;
        private readonly TraceService _traceService = traceService;
        private readonly string connectionString = "server=your_server;user=your_user;database=your_database;port=3306;password=your_password;";



        // 在这里可以添加你的Action方法，例如：GetRobotStatus, ControlRobot等  
        // [HttpGet("status")]  
        // public IActionResult GetRobotStatus()  
        // {  
        //     // 实现获取机器人状态的逻辑  
        // }  

        // 其他Action方法...
        // 如下

        /// <summary>
        /// 启动机器人（包括配置用户坐标系、工具坐标系、全局速度等）
        /// </summary>
        /// <returns>
        /// <para>code=200: 成功启动机器人</para>
        /// <para>code=400: 启动机器人失败，机器人方面的问题</para>
        /// <para>code=500: 启动机器人失败，程序内部问题</para>
        /// </returns>
        [HttpPost("startup")] // 这表示该方法将响应HTTP POST请求，并且其路由路径为 /robot/startup（假设控制器名为RobotController）。
        public IActionResult RobotStartup()
        {//使用 try-catch 块来捕获并处理可能发生的异常
            try
            {
                int res = _robotService.Startup();
                if (res < 0)
                {
                    return BadRequest(new R(res, 400).ToJsonString());
                }
                return Ok(new R(res, 200).ToJsonString());
            }
            catch
            {
                _logger.LogError("An error occurred while processing your request! Check service log for more details.");
                return StatusCode(500, new { message = "An error occurred while processing your request. Check service log for more details.", code = 500 });
            }
        }

        [HttpPost("connect")]
        public IActionResult RobotConnect()
        {
            try
            {
                var r = new R(data: _robotService.Connect());
                return Ok(r.ToJsonString());
            }
            catch
            {
                _logger.LogError("An error occurred while processing your request! Check service log for more details.");
                return StatusCode(500, new { Data = "An error occurred while processing your request. Check service log for more details.", Code = 500 });
            }
        }

        /// <summary>
        /// 向机器人发送开始运动信号
        /// </summary>
        /// <returns>
        /// <para>code=200: 发送开始运动信号成功</para>
        /// <para>code=400: 发送开始运动信号失败</para>
        /// </returns>
        [HttpPost("start")]
        public IActionResult RobotMotionStart()
        {
            try
            {
                bool res = _robotService.Start(400);
                if (res)
                {
                    return Ok(new R().ToJsonString());
                }
                else
                {
                    return BadRequest(new R($"Robot motion start failed!", 400).ToJsonString());
                }
            }
            catch
            {
                _logger.LogError("An error occurred while processing your request! Check service log for more details.");
                return StatusCode(500, new { message = "An error occurred while processing your request. Check service log for more details.", code = 500 });
            }
        }

        /// <summary>
        /// 向机器人发送暂停运动信号
        /// </summary>
        /// <returns>
        /// <para>code=200: 发送暂停运动信号成功</para>
        /// <para>code=400: 发送暂停运动信号失败，机器人原因</para>
        /// <para>code=500: 发送暂停运动信号失败，程序内部原因</para>
        /// </returns>
        [HttpPost("pause")]
        public IActionResult RobotMotionPause()
        {
            try
            {
                bool res = _robotService.Pause(10);

                if (res)
                {
                    return Ok(new R().ToJsonString());
                }
                else
                {
                    return BadRequest(new R($"Robot motion pause failed!", 400).ToJsonString());
                }
            }
            catch
            {
                _logger.LogError("An error occurred while processing your request! Check service log for more details.");
                return StatusCode(500, new { message = "An error occurred while processing your request. Check service log for more details.", code = 500 });
            }
        }

        /// <summary>
        /// 向机器人发送停止运动信号
        /// </summary>
        /// <returns>
        /// <para>code=200: 发送停止运动信号成功</para>
        /// <para>code=400: 发送停止运动信号失败，机器人原因</para>
        /// <para>code=500: 发送停止运动信号失败，程序内部原因</para>
        /// </returns>
        [HttpPost("stop")]
        public IActionResult RobotMotionStop()
        {
            try
            {
                bool res = _robotService.Stop(0);
                if (res)
                {
                    return Ok(new R().ToJsonString());
                }
                else
                {
                    return BadRequest(new R($"Robot motion stop failed!", 400).ToJsonString());
                }
            }
            catch
            {
                _logger.LogError("An error occurred while processing your request! Check service log for more details.");
                return StatusCode(500, new { message = "An error occurred while processing your request. Check service log for more details.", code = 500 });
            }
        }

        /// <summary>
        /// 断开机器人连接
        /// </summary>
        /// <returns>
        /// <para>code=200: 机器人成功断开连接</para>
        /// <para>code=500: 程序异常</para>
        /// </returns>
        [HttpPost("disconnect")]
        public IActionResult RobotDisconnect()
        {
            try
            {
                _robotService.Disconnect();
                return Ok(new R(code: 200).ToJsonString());
            }
            catch
            {
                _logger.LogError("An error occurred while processing your request! Check service log for more details.");
                return StatusCode(500, new { message = "An error occurred while processing your request. Check service log for more details.", code = 500 });
            }
        }

        /// <summary>
        /// 获取舵机状态
        /// </summary>
        /// <returns>上使能data为true，掉使能data为false</returns>
        [HttpGet("servo")] //舵机
        public IActionResult ServoOn()
        {
            return Ok(new R(_robotService.GetServoState(), 200).ToJsonString());
        }

        /// <summary>
        /// 设置全局速度
        /// </summary>
        /// <param name="globalSpeedRequest">全局速度DTO，成员GlobalSpeed为整数值0~100</param>
        /// <returns>设置成功状态码200，否则状态码400</returns>
        [HttpPost("global-speed")]
        public IActionResult SetGlobalSpeed([FromBody] GlobalSpeedRequest globalSpeedRequest)
        {
            bool ret = _robotService.SetGlobalSpeed(globalSpeedRequest.GlobalSpeed);
            if (ret)
                return Ok(new R().ToJsonString());
            return BadRequest(new R("Set global speed failed!", 400));
        }

        /// <summary>
        /// 获取全局速度
        /// </summary>
        /// <returns>整数值 0~100 表示百分比</returns>
        [HttpGet("global-speed")]
        public IActionResult GetGlobalSpeed()
        {
            return Ok(new R(_robotService.GetGlobalSpeed(), 200).ToJsonString());
        }

        //[HttpGet("{processCardId}")]
        //public ActionResult<List<TracePath>> GetTracePaths(int processCardId)
        //{

        //    TraceService dbHelper = new TraceService(connectionString);
        //    List<TracePath> tracePaths = dbHelper.GetTracePaths(processCardId);

        //    if (tracePaths == null || tracePaths.Count == 0)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(tracePaths);
        //}

       /// <summary>
       /// 
       /// </summary>
       /// <param name="processCardId"></param>
       /// <returns></returns>
        [HttpPost("run")] // 用于根据给定的轨迹名称（TraceName）来执行整个轨迹
        public IActionResult RunFullTrace([FromBody] ProcessCardId processCardId) 
        {// TODO: 删除tracetype及其相关
        
            TraceService traceService = new TraceService(connectionString);
            List<TracePath> tracePaths = traceService.GetTracePaths(processCardId.id);

            try
            {
                List<Dao.Task> tasks = new(); //任务列表 主要用于返回给前端看
                foreach (var trace in tracePaths)
                {
                    Dao.Task task = new();
                    //数据库中得到的erperd赋值给task
                    task.ErpAbsolutePath = trace.ErpPath;
                    task.ErdAbsolutePath = trace.ErdPath;
                    //每次得到一对erperd，再增加一个任务
                    tasks.Add(task);

                    int ret = _taskService.AddTask(task);
                    if (ret < 0)
                    {
                        _logger.LogError("Failed to add task to database");
                        return StatusCode(500, new R("Failed to add task to database", 500).ToJsonString());
                    }

                    // 将任务添加到队列中
                    _taskProcessingService.EnqueueTask(task);//实际后端的task
                    
                }
                return Ok(new R(tasks, 200).ToJsonString());//返回给前端看



            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while processing your request: {Message}", ex.Message);
                return StatusCode(500, new R("An error occurred while processing your request. Check service log for more details.", 500).ToJsonString());
            }
        }

        /// <summary>
        /// 模拟急停，不清空运动队列，停止之后可以通过"/continue"接口来继续后续的动作执行
        /// </summary>
        /// <returns>200或者500的状态码</returns>
        [HttpGet("emergency-stop")]
        public IActionResult EmergecyStop()
        {
            _taskProcessingService.EmergencyStop();
            return Ok(new R().ToJsonString());
        }

        /// <summary>
        /// 继续机器人运动（通过TaskProcessingService实现）
        /// </summary>
        /// <returns></returns>
        [HttpGet("continue")]
        public IActionResult Continue()
        {
            _taskProcessingService.TaskContinue();
            return Ok(new R().ToJsonString());
        }

        /// <summary>
        /// 释放机器人控制权限
        /// </summary>
        /// <returns></returns>
        [HttpGet("release-control")]
        public IActionResult ReleaseControl()
        {
            _robotService.ReleasePermit();
            return Ok(new R().ToJsonString());
        }
        //加个接口
    }
}
