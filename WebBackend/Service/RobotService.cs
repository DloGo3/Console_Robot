using EstunApiStruct_CLI;
using Microsoft.Extensions.Logging;
using S7.Net.Types;
using System;
using System.Collections.Concurrent;
using WebBackend.Dao;
using WebBackend.Util;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;
using System.Diagnostics;
using WebBackend.Controller;
using S7.Net;





namespace WebBackend.Service
{
    /// <summary>
    /// 提供给RobotController机器人相关功能
    /// </summary>
    /// <remarks>
    /// 构造函数，用于依赖注入全局数据以及机器人控制器
    /// </remarks>
    /// <param name="appData">全局数据</param>
    /// <param name="controller">机器人控制器</param>
    /// <param name="plcService">PLC业务逻辑类</param>
    /// <param name="logger">日志记录器</param>
    /// <param name="configuration">机器人配置信息</param>
    public class RobotService(IApplicationData appData, WebBackend.Util.Control controller, PlcService plcService, ILogger<RobotService> logger, RobotConfiguration configuration, Parser parser, FileDownloadService fileDownloadService, LightSourceService lightSourceService)
    {
        /// <summary>
        /// 存储全局数据
        /// </summary>
        private readonly IApplicationData _appData = appData;

        /// <summary>
        /// 机器人控制器
        /// </summary>
        private readonly WebBackend.Util.Control _controller = controller;

        /// <summary>
        /// 机器人控制权限
        /// </summary>
        private E_ROB_PERMIT_CLI _robot_permit = new();

        /// <summary>
        /// PLC控制服务
        /// </summary>
        private readonly PlcService _plcService = plcService;

        /// <summary>
        /// 机器人配置信息
        /// </summary>
        private readonly RobotConfiguration _configuration = configuration;

        /// <summary>
        /// 日志记录器
        /// </summary>
        private readonly ILogger<RobotService> _logger = logger;

        private readonly Parser _parser = parser;

        private readonly FileDownloadService _fileDownloadService = fileDownloadService;

        private readonly LightSourceService _lightSourceService = lightSourceService;


        /// <summary>
        /// 启动机器人并让机器人准备运动
        /// </summary>
        /// <returns>
        /// <para>0       启动成功</para>
        /// <para>-1      机械臂连接失败</para>
        /// <para>-2      尝试清除机械臂错误信息但还是存在错误，需要上示教器查看</para>
        /// <para>-3      申请运动权限失败</para>
        /// <para>-4      舵机启动失败，还处于关机状态</para>
        /// <para>-5      舵机启动失败，处于错误状态</para>
        /// <para>-6      设置全局速度失败</para>
        /// <para>-7      加载用户坐标系失败</para>
        /// <para>-8      加载工具坐标系失败</para>
        /// <para>-9      停止机械臂失败（清空运动队列失败）</para>
        /// <para>-10     设置系统模式为API模式失败</para>
        /// </returns>
        public int Startup()
        {
            // ========== 清空所有PLC信号 ==========
            _plcService.WriteBit(DataType.DataBlock, 6, 98, 2, false); // bReadyForChecking_1
            _plcService.WriteBit(DataType.DataBlock, 6, 98, 3, false); // bRobotinside_1
            //_plcService.WriteBit(DataType.DataBlock, 6, 98, 4, false); // bRobotoutside_1
            _plcService.WriteBit(DataType.DataBlock, 6, 98, 5, false); // bFinish_1
            _plcService.WriteBit(DataType.DataBlock, 6, 98, 6, false); // bQualityPassed_1
            _plcService.WriteBit(DataType.DataBlock, 6, 98, 7, false); // bQualityRecheck_1
            _plcService.WriteBit(DataType.DataBlock, 6, 99, 0, false); // bQualityFailed_1
            _plcService.WriteBit(DataType.DataBlock, 6, 99, 1, false); // bReadyForChecking_2
            _plcService.WriteBit(DataType.DataBlock, 6, 99, 2, false);  // bRobotinside_2（检测中）
            //_plcService.WriteBit(DataType.DataBlock, 6, 99, 3, false);  // bRobotoutside_2（检测位外）
            _plcService.WriteBit(DataType.DataBlock, 6, 99, 4, false); // bFinish_2
            _plcService.WriteBit(DataType.DataBlock, 6, 99, 5, false); // bQualityPassed_2
            _plcService.WriteBit(DataType.DataBlock, 6, 99, 6, false); // bQualityRecheck_2
            _plcService.WriteBit(DataType.DataBlock, 6, 99, 7, false); // bQualityFailed_2

            // ========== 清空所有轨迹相关变量和索引 ==========
            _appData.CommandList?.Clear();
            _appData.PosDict?.Clear();
            _appData.CurrentCommandIndex = -1;
            _appData.CurrentTraceIndex = 0;
            _appData.CurrentDetectedPointsNum = 0;
            _appData.CurrentTraceName = string.Empty;
            _appData.DetectionPosition = string.Empty;
            _appData.CurrentTraceType = string.Empty;
            _appData.CurrentPointName = string.Empty;
            _appData.TotalTracesInCurrentPosition = 0;
            _appData.CurrentTraceDetectionPoints = 0;
            _appData.CurrentPhotoIndex = 0;
            _logger.LogInformation("Robot configuration:\nIP: {IP}\nAutoReconnect: {AutoReconnect}\nGlobal speed: {GlobalSpeed}\nUser ID: {UserId}\nTool ID: {ToolId}",
                _configuration.Ip,
                _configuration.AutoReconnect,
                _configuration.GlobalSpeed,
                _configuration.UserId,
                _configuration.ToolId
                );

            try
            {
                // 布尔类型返回值，返回true表示成功，返回false表示失败
                bool boolRet;
                // 整型返回值
                int intRet;
                // ============== 关键步骤：清空轨迹列表（线程安全） ==============


                lock (_appData.TracesLock)
                {
                    // 即使 Traces 不可能为 null，仍建议防御性检查
                    if (_appData.Traces is { Count: > 0 })
                    {
                        _appData.Traces.Clear();
                        _logger.LogInformation("已清空 {Count} 条历史轨迹", _appData.Traces.Count);
                    }
                }
                // ================== 1. 连接控制器 ==================

                boolRet = _controller.Connect(_configuration.Ip, _configuration.AutoReconnect);
                Sleep(100);
                if (!boolRet)
                {
                    _logger.LogError("Robot connection can not be established.");
                    _controller.Disconnect();
                    return -1;
                }
                _logger.LogInformation("Controller connected successfully!");

                // ================== 2. 读取错误信息 ==================

                intRet = _controller.GetErrorId();
                if (intRet != 0)
                {
                    string errorInfo = _controller.GetErrorInfo(intRet);
                    _logger.LogWarning("Controller error exists: {info}", errorInfo);
                    _ = _controller.ClearError();
                    intRet = _controller.GetErrorId();
                    if (intRet != 0)
                    {
                        _logger.LogError("Cannot clear the error info! Check Estun Controller for more details.");
                        _controller.Disconnect();
                        return -2;
                    }
                }
                _logger.LogInformation("Error cleared successfully!");

                // ================== 3. 申请运动权限 ==================

                _robot_permit = _controller.AcquirePermit();
                if (_robot_permit.m_mainctrlcode <= 1)
                {
                    _logger.LogError("Acquire control permission failed!");
                    _controller.Disconnect();
                    return -3;
                }
                _logger.LogInformation("Control permission acquired successfully!");

                // ================== 4. 机器人使能 ==================

                boolRet = _controller.SetServoState(true);
                // 有必要的延时
                Sleep(400);
                E_ServoStatusType_CLI servoState = _controller.GetServoOn();
                // 舵机关闭
                if (servoState == E_ServoStatusType_CLI.ServoOff)
                {
                    // 尝试启动启动舵机
                    _controller.SetServoState(true);
                    Sleep(400);
                    // 启动失败
                    if (servoState != E_ServoStatusType_CLI.ServoOn)
                    {
                        _logger.LogError("Starting servo failed!");
                        _controller.ReleasePermit(_robot_permit);
                        _controller.Disconnect();
                        return -4;
                    }
                }
                if (servoState == E_ServoStatusType_CLI.errStatus)
                {
                    // 尝试启动启动舵机
                    _controller.SetServoState(true);
                    Sleep(400);
                    // 启动失败
                    if (servoState != E_ServoStatusType_CLI.ServoOn)
                    {
                        _logger.LogError("Starting servo failed!");
                        _controller.ReleasePermit(_robot_permit);
                        _controller.Disconnect();
                        return -5;
                    }
                }
                _logger.LogInformation("Servo started successfully!");

                // ================== 5. 设置全局速度 ==================

                boolRet = _controller.SetGlobalSpeed(_configuration.GlobalSpeed);
                Sleep(100);
                if (!boolRet)
                {
                    _logger.LogError("Global speed set failed!");
                    _controller.SetServoState(false);
                    _controller.ReleasePermit(_robot_permit);
                    _controller.Disconnect();
                    return -6;
                }
                _logger.LogInformation("Global speed set successfully!");

                // ================== 6. 加载用户坐标和工具坐标 ==================

                boolRet = _controller.LoadUserCoord(_configuration.UserId);
                Sleep(100);
                if (!boolRet)
                {
                    _logger.LogError("User coord set failed!");
                    _controller.SetServoState(false);
                    _controller.ReleasePermit(_robot_permit);
                    _controller.Disconnect();
                    return -7;
                }
                boolRet = _controller.LoadTool(_configuration.ToolId);
                Sleep(100);
                if (!boolRet)
                {
                    _logger.LogError("Tool coord set failed!");
                    _controller.SetServoState(false);
                    _controller.ReleasePermit(_robot_permit);
                    _controller.Disconnect();
                    return -8;
                }
                _logger.LogInformation("User coord and tool coord set successfully!");

                // ================== 7. 清空运动队列 ==================

                boolRet = _controller.MotionStop(100);
                if (!boolRet)
                {
                    // 这里经常容易返回false，暂时不做处理
                    _logger.LogError("Robot motion stop and move queue cleaned failed!");
                    //_controller.SetServoState(false);
                    //_controller.ReleasePermit(_robot_permit);
                    //_controller.Disconnect();
                    //return -9;
                }

                // ================== 8. 设置系统模式 ==================

                boolRet = _controller.SetSystemMode(E_SysModeType_CLI.API);
                Sleep(100);
                if (!boolRet)
                {
                    _logger.LogError("System mode set to API failed!");
                    _controller.SetServoState(false);
                    _controller.ReleasePermit(_robot_permit);
                    _controller.Disconnect();
                    return -10;
                }
                _logger.LogInformation("System mode set to API successfully!");

                // 返回0说明机械臂正常启动，可以开始执行指令
                return 0;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while starting robot: {ErrorMessage}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// 开始机器人运动
        /// </summary>
        /// <param name="milliseconds">等待信号发送和接受的时间（以毫秒为单位）</param>
        /// <returns>成功返回true，失败返回false</returns>
        public bool Start(Int32 milliseconds)
        {
            return _controller.MotionStart(milliseconds);
        }

        /// <summary>
        /// 暂停机器人运动，但不清空队列
        /// </summary>
        /// <param name="milliseconds">等待信号发送和接受的时间（以毫秒为单位）</param>
        /// <returns>成功返回true，失败返回false</returns>
        public bool Pause(Int32 milliseconds)
        {
            return _controller.MotionPause(milliseconds);
        }

        /// <summary>
        /// 停止机器人运行并清空队列
        /// </summary>
        ///
        /// <param name="milliseconds">等待信号发送和接受的时间（以毫秒为单位）</param>
        /// <returns>成功返回true，失败返回false</returns>
        public bool Stop(Int32 milliseconds)
        {
            return _controller.MotionStop(milliseconds);
        }

        /// <summary>
        /// 连接机械臂
        /// </summary>
        /// <returns>连接成功返回true，失败返回false</returns>
        public bool Connect()
        {
            bool boolRet = _controller.Connect(_configuration.Ip, _configuration.AutoReconnect);
            Sleep(100);
            if (!boolRet)
            {
                _logger.LogError("Robot connection can not be established.");
                _controller.Disconnect();
                return false;
            }
            _logger.LogInformation("Controller connected successfully!");
            return true;
        }

        /// <summary>
        /// 掉使能舵机、释放控制权限并断开连接
        /// </summary>
        /// <returns>释放成功返回true，失败抛出异常</returns>
        /// <exception cref="Exception">异常，记录了详细的异常信息</exception>
        public bool Disconnect()
        {
            try
            {
                _controller.SetServoState(false);
                _controller.ReleasePermit(_robot_permit);
                _controller.Disconnect();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Release permit failed: {ex.Message}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// 睡眠
        /// </summary>
        /// <param name="milliseconds">以毫秒为单位</param>
        public void Sleep(int milliseconds)
        {
            System.Threading.Thread.Sleep(milliseconds);
        }

        /// <summary>
        /// 设置全局速度
        /// </summary>
        /// <param name="speed">0~100 表示百分比</param>
        /// <returns>成功返回true，失败返回false</returns>
        public bool SetGlobalSpeed(Int32 speed)
        {
            return _controller.SetGlobalSpeed(speed);
        }

        /// <summary>
        /// 获取全局速度
        /// </summary>
        /// <returns>整型值0~100 表示百分比</returns>
        public int GetGlobalSpeed()
        {
            return _controller.GetGlobalSpeed();
        }

        /// <summary>
        /// 加载用户坐标系
        /// </summary>
        /// <param name="userId">用户坐标系ID</param>
        /// <returns>加载成功返回true，失败返回false</returns>
        public bool LoadUserCoord(Int32 userId)
        {
            return _controller.LoadUserCoord(userId);
        }

        /// <summary>
        /// 加载工具坐标系
        /// </summary>
        /// <param name="toolId">工具坐标系ID</param>
        /// <returns>加载成功返回true，失败返回false</returns>
        public bool LoadTool(Int32 toolId)
        {
            return _controller.LoadTool(toolId);
        }
        /// <summary>
        /// 检测机械臂是否准备好
        /// </summary>
        /// <returns></returns>
        public bool IsReady()
        {
            Console.WriteLine($"机械臂状态为：{_controller.GetAPIStatus()}");

            // 0 代表机器人正常工作状态
            return _controller.GetAPIStatus() != -1;
        }
        /// <summary>
        /// 运行一条指令
        /// </summary>
        /// <param name="command">待运行指令参数</param>
        /// <returns>整数状态值：
        /// <para>0   运行成功</para>
        /// <para>-1  机械臂启动失败</para>
        /// <para>-2  非运动指令</para>
        /// <para>-3  指令执行失败</para>
        /// </returns>
        public int RunCommand(Command command)
        {
            // =============== 1. 判断机械臂是否启动 ===============

            int intRet = _controller.GetAPIStatus();
            Console.WriteLine($"Robot status {intRet}");
            // 机器人不处于工作状态，尝试重新启动机器人
            if (intRet != 0)
            {
                bool boolRet = _controller.MotionStart(500);
                if (!boolRet)
                {
                    return -1;
                }
            }

            Console.WriteLine(command);

            // =============== 2. 执行命令 ===============
            if (command.Type == "MovJ" || command.Type == "MovC" || command.Type == "MovL")
            {
                // 轨迹点变量名
                string P_Name = command.Parameters["P"].Split('.')[1];
                // 速度变量名
                string V_Name = command.Parameters["V"].Split('.')[1];
                // 转弯区变量名
                string C_Name = command.Parameters["C"].Split('.')[1];
                // 中间轨迹点变量名
                string A_Name = "";
                // 定义JobID
                E_ROB_JOBID_CLI jobId = new();
                // 判断是否连续运动（det == 2用来判断是否是最后一个点位，以完成机器人复位行为）
                bool isWaitFinished = _appData.PosDict[P_Name].det == 1 || _appData.PosDict[P_Name].det == 2;
                // 判断是否拍照
                bool isDetectedPoint = _appData.PosDict[P_Name].det == 1;

                // 防抖
                Sleep(100);

                if (command.Type == "MovC")
                {
                    A_Name = command.Parameters["A"].Split('.')[1];
                }
                if (command.Type == "MovJ")
                {
                    jobId = _controller.MovJ2(_appData.PosDict[P_Name], _appData.SpeedDict[V_Name], _appData.ZoneDict[C_Name], isWaitFinished: isWaitFinished);
                }
                if (command.Type == "MovC")
                {
                    jobId = _controller.MovC2(_appData.PosDict[A_Name], _appData.PosDict[P_Name], _appData.SpeedDict[V_Name], _appData.ZoneDict[C_Name], isWaitFinished: isWaitFinished);
                }
                if (command.Type == "MovL")
                {
                    jobId = _controller.MovL2(_appData.PosDict[P_Name], _appData.SpeedDict[V_Name], _appData.ZoneDict[C_Name], isWaitFinished: isWaitFinished);
                }

                // API手册中有写：jobId.m_jobID > 0表示指令下发成功
                if (jobId.m_jobID > 0)
                {
                    if (isDetectedPoint)
                    {
                        _appData.CurrentPointName = P_Name;
                        // 检测到点位，拍照，更新已检测点数量
                        _appData.CurrentDetectedPointsNum++;
                        System.Threading.Tasks.Task.Run(() =>
                        {
                            _plcService.TakePhoto(500);
                        });
                        //减少时间
                        Sleep(500);
                    }
                    return 0;
                }
                else
                {
                    return -3;
                }
            }
            // 非运动指令返回-2
            else
            {
                return -2;
            }
        }

        //异步版本
        private async Task<int> RunCommandAsync(Command command)
        {
            // 原有逻辑...

            // =============== 1. 判断机械臂是否启动 ===============

            int intRet = _controller.GetAPIStatus();
            Console.WriteLine($"Robot status {intRet}");
            // 机器人不处于工作状态，尝试重新启动机器人
            if (intRet != 0)
            {
                bool boolRet = _controller.MotionStart(500);
                if (!boolRet)
                {
                    return -1;
                }
            }

            Console.WriteLine(command);

            // =============== 2. 执行命令 ===============
            if (command.Type == "MovJ" || command.Type == "MovC" || command.Type == "MovL")
            {
                // 轨迹点变量名
                string P_Name = command.Parameters["P"].Split('.')[1];
                // 获取当前点位对象
                var currentPos = _appData.PosDict[P_Name];

                // 默认电流（比如50）
                double defaultCurrent = 2;
                // 取lightTime，如果有就用，没有就用默认
                double currentValue = currentPos.lightTime ?? defaultCurrent;
                for (int channel = 0; channel < 8; channel++)
                {
                    Console.WriteLine($"[光源控制] 正在读取通道{channel}参数 ...");
                    var param = await _lightSourceService.ReadChannelParametersAsync(channel);
                    if (param == null)
                    {
                        Console.WriteLine($"[光源控制] 通道{channel}参数读取失败，跳过本次电流设置！");
                        continue;
                    }
                    bool ok = _lightSourceService.ChangeCurrentMaxAndSet(currentValue);
                    if (ok)
                    {
                        Console.WriteLine($"[光源控制] 通道{channel}已设置电流为 {currentValue}，等待下次确认...");
                        var check = await _lightSourceService.ReadChannelParametersAsync(channel);
                        if (check != null)
                            Console.WriteLine($"[光源控制] 通道{channel}参数更改后，电流值: {check.Value.nCurrentMax}");
                    }
                    else
                    {
                        Console.WriteLine($"[光源控制] 通道{channel}设置失败，lastChannelParams未获取到！");
                    }
                }

                string V_Name = command.Parameters["V"].Split('.')[1];
                // 转弯区变量名
                string C_Name = command.Parameters["C"].Split('.')[1];
                // 中间轨迹点变量名
                string A_Name = "";
                // 定义JobID
                E_ROB_JOBID_CLI jobId = new();
                // 判断是否连续运动（det == 2用来判断是否是最后一个点位，以完成机器人复位行为）
                bool isWaitFinished = _appData.PosDict[P_Name].det == 1 || _appData.PosDict[P_Name].det == 2;
                // 判断是否拍照
                bool isDetectedPoint = _appData.PosDict[P_Name].det == 1;

                // 防抖
                Sleep(100);

                if (command.Type == "MovC")
                {
                    A_Name = command.Parameters["A"].Split('.')[1];
                }
                if (command.Type == "MovJ")
                {
                    jobId = _controller.MovJ2(_appData.PosDict[P_Name], _appData.SpeedDict[V_Name], _appData.ZoneDict[C_Name], isWaitFinished: isWaitFinished);
                }
                if (command.Type == "MovC")
                {
                    jobId = _controller.MovC2(_appData.PosDict[A_Name], _appData.PosDict[P_Name], _appData.SpeedDict[V_Name], _appData.ZoneDict[C_Name], isWaitFinished: isWaitFinished);
                }
                if (command.Type == "MovL")
                {
                    jobId = _controller.MovL2(_appData.PosDict[P_Name], _appData.SpeedDict[V_Name], _appData.ZoneDict[C_Name], isWaitFinished: isWaitFinished);
                }

                // API手册中有写：jobId.m_jobID > 0表示指令下发成功
                if (jobId.m_jobID > 0)
                {
                    // 添加异步等待
                    //await WaitForMotionCompletion();
                    if (isDetectedPoint)
                    {
                        _appData.CurrentPointName = P_Name;
                        // 检测到点位，拍照，更新已检测点数量
                        _appData.CurrentDetectedPointsNum++;
                        await System.Threading.Tasks.Task.Run(() => _plcService.TakePhoto(500));
                        //减少时间
                        await System.Threading.Tasks.Task.Delay(500); // 替换原Sleep
                    }
                    return 0;
                }
                else
                {
                    return -3;
                }
            }
            // 非运动指令返回-2
            else
            {
                return -2;
            }

        }
        public bool IsRunning => _commandLock.CurrentCount == 0;
        //添加并发控制锁
        private readonly SemaphoreSlim _commandLock = new(1, 1);
        /// <summary>
        /// 运行所有指令，运行结束后才返回值
        /// </summary>
        /// <returns>整数状态值：
        /// <para>0   所有指令运行成功</para>
        /// <para>-1  机械臂启动失败</para>
        /// <para>-2  非运动指令</para>
        /// <para>-3  指令执行失败</para>
        /// <para>-4  指令为空，未导入指令</para>
        /// <para>-5  点位为空，未导入点位</para>
        /// <para>-99 任务执行失败，服务器内部错误</para>
        /// </returns>
        public int RunAllCommand() //对应erp文件的一行指令
        {
            _commandLock.Wait();
            _appData.CurrentCommandIndex = -1;
            try
            {

                int ret = 0;
                if (_appData.CommandList.Count == 0)
                    return -4;
                if (_appData.PosDict.IsEmpty)
                    return -5;

                // 清空指令队列
                _controller.MotionStop(500);
                // 启动机器人
                _controller.MotionStart(1000);
                // 重置当前已检测点数量
                _appData.CurrentDetectedPointsNum = 0;

                //从appdata里面一条一条指令的执行runcommand
                for (int i = 0; i < _appData.CommandList.Count; i++)
                {
                    _appData.CurrentCommandIndex = i;
                    ret = RunCommand(_appData.CommandList[i]);
                    if (ret == -1 || ret == -3)
                    {
                        return ret;
                    }
                }

                // 停止机器人并清空指令队列
                _controller.MotionStop(50);

                // TODO：新建线程将Task写入数据库或者发请求给Java后端

                return ret;
            }

            catch (Exception ex)
            {
                _logger.LogError("Running all commands failed: {Message}", ex.Message);
                return -99; // -99表示内部异常
            }
            finally { _commandLock.Release(); }
        }

        /// <summary>
        /// 新增异步版本
        /// </summary>
        /// <returns></returns>
        public async Task<int> RunAllCommandAsync()
        {
            await _commandLock.WaitAsync();
            try
            {
                _appData.CurrentCommandIndex = -1;
                if (_appData.CommandList.Count == 0) return -4;
                if (_appData.PosDict.IsEmpty) return -5;

                await System.Threading.Tasks.Task.Run(() => _controller.MotionStop(500));
                await System.Threading.Tasks.Task.Run(() => _controller.MotionStart(1000));

                _appData.CurrentDetectedPointsNum = 0;

                foreach (var cmd in _appData.CommandList)
                {
                    var ret = await RunCommandAsync(cmd);
                    if (ret == -1 || ret == -3)
                    {
                        return ret;
                    }
                }

                await System.Threading.Tasks.Task.Run(() => _controller.MotionStop(50));
                return 0;
            }
            finally
            {
                _commandLock.Release();
            }
        }
        // 添加异步等待
        private async System.Threading.Tasks.Task WaitForMotionCompletion(int timeoutMs = 30000)
        {
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < timeoutMs)
            {
                //await System.Threading.Tasks.Task.Delay(100);
                await System.Threading.Tasks.Task.Delay(100).ConfigureAwait(false); // 添加ConfigureAwait
                if (_controller.GetAPIStatus() == 0) break;
            }
            throw new TimeoutException("机械臂运动超时"); // 必须抛出异常
        }
        /// <summary>
        /// 继续任务运行
        /// </summary>
        /// <returns>整数状态值：
        /// <para>0   所有指令运行成功</para>
        /// <para>-1  机械臂启动失败</para>
        /// <para>-2  非运动指令</para>
        /// <para>-3  指令执行失败</para>
        /// <para>-4  指令为空，未导入指令</para>
        /// <para>-5  点位为空，未导入点位</para>
        /// <para>-99 任务执行失败，服务器内部错误</para>
        /// </returns>
        public int ContinueMove()
        {
            try
            {
                int ret = 0;
                if (_appData.CommandList.Count == 0)
                    return -4;
                if (_appData.PosDict.IsEmpty)
                    return -5;

                // 启动机器人
                Console.WriteLine("机器人重新开始运动");
                bool boolRet = _controller.MotionContinue(100);
                Console.WriteLine(boolRet);
                for (int i = _appData.CurrentCommandIndex; i < _appData.CommandList.Count; i++)
                {
                    _appData.CurrentCommandIndex = i;
                    Console.WriteLine($"Executing ${i} command...");
                    ret = RunCommand(_appData.CommandList[i]);
                    Console.WriteLine($"Command {i} executed, return value: {ret}");
                    if (ret == -1 || ret == -3)
                    {
                        return ret;
                    }
                }

                // 停止机器人并清空指令队列
                _controller.MotionStop(50);

                return ret;
            }
            catch (Exception ex)
            {
                _logger.LogError("Continue running all commands failed: {ex.Message}", ex.Message);
                return -99; // -99表示内部异常
            }
        }

        /// <summary>
        /// 获取当前机械臂的世界坐标系（实际对应示教器上的用户坐标系）
        /// </summary>
        /// <returns>笛卡尔坐标系</returns>
        public E_ROB_POS_CLI GetCurrentJPos()
        {
            return _controller.GetCurrentJPos();
        }

        /// <summary>
        /// 获取当前机械臂的关节坐标系
        /// </summary>
        /// <returns>六轴转动角度</returns>
        public E_ROB_POS_CLI GetCurrentWPos()
        {
            return _controller.GetCurrentWPos();
        }

        /// <summary>
        /// 获得舵机状态
        /// </summary>
        /// <returns>三种状态，详见E_ServoStatusType_CLI枚举类</returns>
        public E_ServoStatusType_CLI GetServoState()
        {
            return _controller.GetServoOn();
        }

        /// <summary>
        /// 获取控制器状态
        /// </summary>
        /// <returns>-1 表示失败，0 表示正常，1 表示机器人错误，2 表示机器人处于停止状态，需要调用 start 接口启动</returns>
        public int GetAPIStatus()
        {
            return _controller.GetAPIStatus();
        }

        /// <summary>
        /// 获取系统模式
        /// </summary>
        /// <returns>E_SysModeType_CLI枚举类，manualMode-手动模式，autoMode-自动模式，API-API模式（其他模式略）</returns>
        public E_SysModeType_CLI GetSysMode()
        {
            return _controller.GetSystemMode();
        }

        /// <summary>
        /// 设置系统运行模式
        /// </summary>
        /// <param name="mode">E_SysModeType_CLI枚举类，manualMode-手动模式，autoMode-自动模式，API-API模式（其他模式略）</param>
        /// <returns>成功返回true，失败返回false</returns>
        public bool SetSysMode(E_SysModeType_CLI mode)
        {
            return _controller.SetSystemMode(mode);
        }

        /// <summary>
        /// 获取运动模式
        /// </summary>
        /// <returns>E_RunModeType_CLI枚举类，1为步进，2为连续</returns>
        public E_RunModeType_CLI GetRunMode()
        {
            return _controller.GetRunMode();
        }

        /// <summary>
        /// 设置运动模式
        /// </summary>
        /// <param name="mode">E_RunModeType_CLI枚举类，1为步进，2为连续</param>
        /// <returns>设置成功返回true，失败返回false</returns>
        public bool SetRunMode(E_RunModeType_CLI mode)
        {
            return _controller.SetRunMode(mode);
        }

        /// <summary>
        /// 获取任务队列
        /// </summary>
        /// <returns>任务队列</returns>
        public ConcurrentQueue<Dao.Task> GetTaskQueue()
        {
            return _appData.TaskQueue;
        }

        /// <summary>
        /// 获取当前执行的任务
        /// </summary>
        /// <returns>如果队列非空，返回当前任务，否则返回一个持续时间（Duration）为-1的Task</returns>
        public Dao.Task GetCurrentTask()
        {
            if (_appData.TaskQueue.IsEmpty)
            {
                return new Dao.Task();
            }
            return _appData.CurrentTask;
        }

        /// <summary>
        /// 设置工具坐标系
        /// </summary>
        /// <param name="toolId">工具坐标系ID</param>
        /// <returns>设置成功返回true，失败返回false</returns>
        public bool SetToolId(int toolId)
        {
            bool ret = _controller.LoadTool(toolId);
            if (ret)
            {
                return ret;
            }
            else
            {
                _logger.LogWarning("Tool Coord set failed!");
                return ret;
            }

        }

        /// <summary>
        /// 释放控制权限
        /// </summary>
        public void ReleasePermit()
        {
            _controller.ReleasePermit(_robot_permit);
        }
        /// <summary>
        /// 根据工艺卡ID获取工艺卡相关信息
        /// </summary>
        public class ProcessCardInfo
        {
            public int Id { get; set; }
            public string Name { get; set; }
            // 其他属性...  
        }
        //-----------------------------------------------------------------------------------
        /// <summary>
        /// 接收 POST 请求后处理标定任务
        /// </summary>
        //http://127.0.0.1:6060/api/v1/vt-process-card-software/files/download?path=%2Fupload%2Ferd%2F%E6%A0%87%E5%AE%9A-6.3%CE%BCm.erd
        public async Task<int> PerformCalibration(CalibrationRequest requestData)
        {
            // 设置轨迹类型为标定
            _appData.CurrentTraceType = "标定";
            // 标定启动时间
            //_appData.BeginTime = System.DateTime.Now("yyyy-MM-dd HH:mm:ss");
            _appData.BeginTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(); // 使用 UTC 时间

            try
            {
                // 验证请求数据
                if (requestData == null || requestData.Data == null)
                {
                    _logger.LogError("Invalid calibration request data.");
                    return -1; // 数据无效
                }

                // 验证 RoughnessCalibration 数据是否有效
                if (requestData.Data.RoughnessCalibration == null || requestData.Data.RoughnessCalibration.Count == 0)
                {
                    _logger.LogError("Roughness calibration data is missing or invalid.");
                    return -1; // 数据无效
                }

                // 保存标定数据到全局应用数据
                _appData.CalibrationData = requestData;
                _appData.CurrentPhotoIndex = 0; // 初始化当前拍摄次数索引

                //标定无工艺卡
                _appData.ProcessCardId = 0;

                int numberOfPhotos = requestData.Number; // 连续拍照次数

                //var roughnessCalibration = requestData.Data.RoughnessCalibration[0];
                foreach (var roughnessCalibration in requestData.Data.RoughnessCalibration)
                {
                    // 重置当前拍摄次数为0，每个标定值开始时重新计数
                    _appData.CurrentPhotoIndex = 0;

                    // 保存粗糙度的值到全局应用数据
                    _appData.RoughnessValue = roughnessCalibration.Value;
                    _logger.LogInformation($"Processing roughness calibration for value: {roughnessCalibration.Value}");
                    if (string.IsNullOrEmpty(roughnessCalibration.ErdFilePath) || string.IsNullOrEmpty(roughnessCalibration.ErpFilePath))
                    {
                        _logger.LogError($"ERD or ERP file path is missing for roughness value: {roughnessCalibration.Value}");

                        return -1;
                    }
                    _logger.LogInformation($"Performing calibration with ERD path: {roughnessCalibration.ErdFilePath} and ERP path: {roughnessCalibration.ErpFilePath}");


                    // 构建完整 URL 并下载文件
                    var completeErdUrl = _fileDownloadService.BuildCompleteUrl(roughnessCalibration.ErdFilePath);
                    var completeErpUrl = _fileDownloadService.BuildCompleteUrl(roughnessCalibration.ErpFilePath);

                    var localErdPath = await _fileDownloadService.DownloadFileFromUrlAsync(completeErdUrl, "data");
                    var localErpPath = await _fileDownloadService.DownloadFileFromUrlAsync(completeErpUrl, "data");

                    // 解析文件
                    var commands = _parser.ParseErpFileToList(localErpPath);
                    var positions = _parser.ParseErdFileToDict(localErdPath);

                    if (commands == null || commands.Count == 0)
                    {
                        _logger.LogError($"Failed to parse ERP file or no commands found for roughness value: {roughnessCalibration.Value}");
                        return -2; // ERP文件解析失败
                    }

                    if (positions == null || positions.IsEmpty)
                    {
                        _logger.LogError($"Failed to parse ERD file or no commands found for roughness value: {roughnessCalibration.Value}");
                        return -2; // ERD文件解析失败
                    }

                    _logger.LogInformation($"Parsed {commands.Count} commands and {positions.Count} positions from ERD and ERP files.");

                    // 更新全局数据
                    _appData.CommandList = commands; // 加载指令列表
                    _appData.PosDict = positions; // 加载点位信息

                    // 设置拍照目录
                    var timeStamp = DateTimeOffset.Now.ToString("yyyy-MM-dd-HH-mm-ss", CultureInfo.InvariantCulture);
                    await SetPicturesDir(timeStamp);

                    // 执行指令并拍照
                    int result = RunAllCommandWithPhotos(numberOfPhotos);

                    // 检查结果
                    if (result != 0)
                    {
                        _logger.LogError($"Calibration failed for roughness value {roughnessCalibration.Value} with error code: {result}");
                        return result;
                    }

                    _logger.LogInformation($"Calibration completed successfully for roughness value: {roughnessCalibration.Value}");
                }

                return 0; // 所有标定都成功完成
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in PerformCalibration: {ex.Message}");
                return -99;
            }
            finally
            {
                // 无论成功或失败，最终重置时间为 "0"
                _appData.BeginTime = 0;
                _appData.CurrentTraceType = "手动";
            }
        }


        /// <summary>
        /// 执行所有指令并拍照
        /// </summary>
        private int RunAllCommandWithPhotos(int numberOfPhotos)
        {
            _appData.CurrentCommandIndex = -1;

            try
            {
                int ret = 0;
                if (_appData.CommandList.Count == 0) return -4; // 无指令
                if (_appData.PosDict.IsEmpty) return -5; // 无点位


                // 清空运动队列并启动机器人
                _controller.MotionStop(500);
                _controller.MotionStart(1000);
                _appData.CurrentDetectedPointsNum = 0;
                //从appdata里面一条一条指令的执行runcommand
                for (int i = 0; i < _appData.CommandList.Count; i++)
                {
                    _appData.CurrentCommandIndex = i;
                    ret = RunCommandWithPhotos(_appData.CommandList[i], numberOfPhotos);
                    if (ret == -1 || ret == -3)
                    {
                        return ret;
                    }
                }
                //foreach (var command in _appData.CommandList)
                //{
                //    int result = RunCommandWithPhotos(command, numberOfPhotos);
                //    if (result < 0)
                //    {
                //        _logger.LogError($"Command execution failed with return value: {result}");
                //        return result; // 执行失败
                //    }
                //}

                _controller.MotionStop(50); // 停止机器人
                return ret;
                //return 0; // 所有指令执行成功
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in RunAllCommandWithPhotos: {ex.Message}");
                return -99; // 内部异常
            }
        }
        /// <summary>
        /// 执行单条指令并在拍照点位拍照
        /// </summary>
        private int RunCommandWithPhotos(Command command, int numberOfPhotos)
        {
            // =============== 1. 判断机械臂是否启动 ===============

            int intRet = _controller.GetAPIStatus();
            Console.WriteLine($"Robot status {intRet}");
            // 机器人不处于工作状态，尝试重新启动机器人
            if (intRet != 0)
            {
                bool boolRet = _controller.MotionStart(500);
                if (!boolRet)
                {
                    return -1;
                }
            }

            Console.WriteLine(command);

            // =============== 2. 执行命令 ===============
            if (command.Type == "MovJ" || command.Type == "MovC" || command.Type == "MovL")
            {
                // 轨迹点变量名
                string P_Name = command.Parameters["P"].Split('.')[1];
                // 速度变量名
                string V_Name = command.Parameters["V"].Split('.')[1];
                // 转弯区变量名
                string C_Name = command.Parameters["C"].Split('.')[1];
                // 中间轨迹点变量名
                string A_Name = "";
                // 定义JobID
                E_ROB_JOBID_CLI jobId = new();
                // 判断是否连续运动（det == 2用来判断是否是最后一个点位，以完成机器人复位行为）
                bool isWaitFinished = _appData.PosDict[P_Name].det == 1 || _appData.PosDict[P_Name].det == 2;
                // 判断是否拍照
                bool isDetectedPoint = _appData.PosDict[P_Name].det == 1;



                if (command.Type == "MovC")
                {
                    A_Name = command.Parameters["A"].Split('.')[1];
                }
                if (command.Type == "MovJ")
                {
                    jobId = _controller.MovJ2(_appData.PosDict[P_Name], _appData.SpeedDict[V_Name], _appData.ZoneDict[C_Name], isWaitFinished: isWaitFinished);
                }
                if (command.Type == "MovC")
                {
                    jobId = _controller.MovC2(_appData.PosDict[A_Name], _appData.PosDict[P_Name], _appData.SpeedDict[V_Name], _appData.ZoneDict[C_Name], isWaitFinished: isWaitFinished);
                }
                if (command.Type == "MovL")
                {
                    jobId = _controller.MovL2(_appData.PosDict[P_Name], _appData.SpeedDict[V_Name], _appData.ZoneDict[C_Name], isWaitFinished: isWaitFinished);
                }

                // 防抖
                Sleep(100);

                //Console.WriteLine("标定拍照次数:"+ numberOfPhotos);
                // API手册中有写：jobId.m_jobID > 0表示指令下发成功
                if (jobId.m_jobID > 0)
                {
                    if (isDetectedPoint)
                    {
                        _appData.CurrentPointName = P_Name;
                        // 检测到点位，拍照，更新已检测点数量
                        _appData.CurrentDetectedPointsNum++;
                        for (int i = 0; i < numberOfPhotos; i++)
                        {
                            _appData.CurrentPhotoIndex++; // 更新当前拍摄次数
                            System.Threading.Tasks.Task.Run(() =>
                            {
                                _plcService.TakePhoto(500); // 每次拍 5 张
                            });
                            Thread.Sleep(2000); // 间隔 2 秒
                        }
                        Sleep(1000);
                    }
                    return 0;
                }
                else
                {
                    return -3;
                }
            }
            // 非运动指令返回-2
            else
            {
                return -2;
            }
        }
        // 数据模型
        public class CalibrationRequest
        {
            public int Number { get; set; } // 连续拍照次数
            public CalibrationData Data { get; set; } // 标定数据
        }

        public class CalibrationData
        {
            public int Id { get; set; }
            [JsonPropertyName("length_calibration")]
            public List<LengthCalibration> LengthCalibration { get; set; }
            [JsonPropertyName("roughness_calibration")]
            public List<RoughnessCalibration> RoughnessCalibration { get; set; }
        }

        public class LengthCalibration
        {
            public int Id { get; set; }
            public double Value { get; set; }
        }

        public class RoughnessCalibration
        {
            public int Id { get; set; }
            public double Value { get; set; }
            [JsonPropertyName("erd_file_path")]
            public string ErdFilePath { get; set; } // ERD 文件路径
            [JsonPropertyName("erp_file_path")]
            public string ErpFilePath { get; set; } // ERP 文件路径
        }

        private async System.Threading.Tasks.Task SetPicturesDir(string folderName)
        {
            using HttpClient client = new();
            var response = await client.GetAsync($"http://192.168.1.102:8080?name={folderName}");
        }
    }
}
