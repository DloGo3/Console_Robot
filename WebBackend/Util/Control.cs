using EstunApiForPublic;
using EstunApiStruct_CLI;
using System;
using System.Collections.Generic;

namespace WebBackend.Util
{
    /// <summary>
    /// 机器人控制类，封装了埃斯顿机械臂的API接口
    /// </summary>
    public class Control
    {
        private readonly EstunApi_CLI _api = new();  // API 接口

        /// <summary>
        /// 连接到服务器
        /// </summary>
        /// <param name="ip">服务器IP地址</param>
        /// <param name="autoReconnect">是否自动重连</param>
        /// <returns>成功连接返回true，否则返回false</returns>
        public bool Connect(string ip, bool autoReconnect)
        {
            return _api.connectToServer(ip, autoReconnect);
        }

        /// <summary>
        /// 获取操作许可
        /// </summary>
        /// <returns>返回操作许可</returns>
        public E_ROB_PERMIT_CLI AcquirePermit()
        {
            return _api.E_AcquirePermit();
        }

        /// <summary>
        /// 查询当前机器人运动操作权限
        /// </summary>
        /// <returns>返回权限详细</returns>
        public E_ROB_PERMIT_CLI CurrentPermit()
        {
            return _api.E_CurrentPermit();
        }

        /// <summary>
        /// 设置伺服电机状态
        /// </summary>
        /// <param name="on">是否开启</param>
        /// <returns>成功返回true，失败返回false</returns>
        public bool SetServoState(bool on)
        {
            return _api.E_SetServoOn(on);
        }

        /// <summary>
        /// 获取伺服电机状态
        /// </summary>
        /// <returns>三种状态，详见E_ServoStatusType_CLI枚举类</returns>
        public E_ServoStatusType_CLI GetServoOn()
        {
            return _api.E_GetServoOn();
        }

        /// <summary>
        /// 设置全局速度
        /// </summary>
        /// <param name="speed">0~100 表示百分比</param>
        /// <returns>成功返回true，失败返回false</returns>
        public bool SetGlobalSpeed(int speed)
        {
            return _api.E_SetGlobalSpeed(speed);
        }

        /// <summary>
        /// 获取全局速度
        /// </summary>
        /// <returns>0~100 表示百分比</returns>
        public int GetGlobalSpeed()
        {
            return _api.E_GetGlobalSpeed();
        }

        /// <summary>
        /// 获取错误号
        /// </summary>
        /// <returns>Error ID，为0表示没有错误</returns>
        public int GetErrorId()
        {
            int errorId = _api.E_GetErrorId();
            return errorId;
        }

        /// <summary>
        /// 获取错误详情
        /// </summary>
        /// <param name="errorId">错误号，由GetErrorId方法获取</param>
        /// <returns>错误信息</returns>
        public string GetErrorInfo(int errorId)
        {
            

            return _api.E_GetErrorString(errorId);
        }

        /// <summary>
        /// 清除错误信息
        /// </summary>
        /// <returns>成功返回true，失败返回false</returns>
        public bool ClearError()
        {
            return _api.E_ClearError();
        }

        /// <summary>
        /// 释放操作许可
        /// </summary>
        /// <param name="permit">操作许可</param>
        public void ReleasePermit(E_ROB_PERMIT_CLI permit)
        {
            _api.E_ReleasePermit(permit);
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect()
        {
            _api.disConnect();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            _api.Dispose();
        }

        /// <summary>
        /// 停止运动并断开链接
        /// </summary>
        /// <param name="permit">通过E_</param>
        public void CleanUpAndDispose(E_ROB_PERMIT_CLI permit)
        {
            _api.E_MotionStop();
            _api.E_ReleasePermit(permit);
            _api.disConnect();
            _api.Dispose();
        }

        /// <summary>
        /// 停止机器人运行并清空队列
        /// </summary>
        ///
        /// <param name="milliseconds">等待信号发送和接受的时间（以毫秒为单位）</param>
        /// <returns>成功返回true，失败返回false</returns>
        public bool MotionStop(Int32 milliseconds = 0)
        {
            bool ret = _api.E_MotionStop();
            System.Threading.Thread.Sleep(milliseconds);
            return ret;
        }

        /// <summary>
        /// 暂停机器人运动，但不清空队列
        /// </summary>
        /// <param name="milliseconds">等待信号发送和接受的时间（以毫秒为单位）</param>
        /// <returns>成功返回true，失败返回false</returns>
        public bool MotionPause(Int32 milliseconds = 0)
        {
            bool ret = _api.E_MotionPause();
            System.Threading.Thread.Sleep(milliseconds);
            return ret;
        }

        /// <summary>
        /// 开始机器人运动
        /// </summary>
        /// <param name="milliseconds">等待信号发送和接受的时间（以毫秒为单位）</param>
        /// <returns>成功返回true，失败返回false</returns>
        public bool MotionStart(Int32 milliseconds = 0)
        {
            bool ret = _api.E_MotionStart();
            System.Threading.Thread.Sleep(milliseconds);
            return ret;
        }

        /// <summary>
        /// 继续机器人运动（Pause后使用）
        /// </summary>
        /// <param name="milliseconds">等待信号发送和接受的时间（以毫秒为单位）</param>
        /// <returns>成功返回true，失败返回false</returns>
        public bool MotionContinue(Int32 milliseconds = 0)
        {
            bool ret = _api.E_MotionContinue();
            System.Threading.Thread.Sleep(milliseconds);
            return ret;
        }

        /// <summary>
        /// 加载用户坐标系
        /// </summary>
        /// <param name="userLocationid">用户坐标系ID</param>
        /// <returns>加载成功返回true，失败返回false</returns>
        public bool LoadUserCoord(int userLocationid)
        {
            return _api.E_LoadUserCoord(userLocationid);
        }

        /// <summary>
        /// 加载工具坐标系
        /// </summary>
        /// <param name="toolId">工具坐标系ID</param>
        /// <returns>加载成功返回true，失败返回false</returns>
        public bool LoadTool(int toolId)
        {
            return _api.E_LoadTool(toolId);
        }

        /// <summary>
        /// 设置系统运行模式
        /// </summary>
        /// <param name="mode">E_SysModeType_CLI枚举类，manualMode-手动模式，autoMode-自动模式，API-API模式（其他模式略）</param>
        /// <returns>成功返回true，失败返回false</returns>
        public bool SetSystemMode(E_SysModeType_CLI mode)
        {
            return _api.E_SetSysMode(mode);
        }

        /// <summary>
        /// 获取系统运行模式
        /// </summary>
        /// <returns>E_SysModeType_CLI枚举类，manualMode-手动模式，autoMode-自动模式，API-API模式（其他模式略）</returns>
        public E_SysModeType_CLI GetSystemMode()
        {
            return _api.E_GetSysMode();
        }

        /// <summary>
        /// 设置点位
        /// </summary>
        /// <param name="varName">点名称</param>
        /// <param name="varScope">点作用域，0-系统，1-全局，2-工程，3-本地</param>
        /// <param name="pos">待设置的点位</param>
        /// <returns>成功返回true，失败返回false</returns>
        public bool SetPoint(string varName, int varScope, E_ROB_POS_CLI pos)
        {
            return _api.E_WritePos(varName, varScope, pos);
        }

        /// <summary>
        /// 获取点位信息
        /// </summary>
        /// <param name="varName">点名称</param>
        /// <param name="varScope">点作用域，0-系统，1-全局，2-工程，3-本地</param>
        /// <param name="posBack">存储获取到的点信息</param>
        /// <returns>成功返回true，失败返回false</returns>
        public bool GetPoint(string varName, int varScope, ref E_ROB_POS_CLI posBack)
        {
            return _api.E_ReadPos(varName, varScope, ref posBack);
        }

        /// <summary>
        /// 批量设置点位
        /// </summary>
        /// <param name="points">点位列表</param>
        /// <param name="varScope">点作用域，0-系统，1-全局，2-工程，3-本地</param>
        /// <returns>全部设置成功返回true，否则返回false</returns>
        public bool SetPoints(List<Tuple<string, E_ROB_POS_CLI>> points, int varScope)
        {
            bool result = true;
            foreach (var point in points)
            {
                result &= SetPoint(point.Item1, varScope, point.Item2);
            }
            return result;
        }

        /// <summary>
        /// 加载工程（仅当系统在自动模式下有效）
        /// </summary>
        /// <param name="prjname">工程名称</param>
        /// <returns>成功返回true，失败返回false</returns>
        public bool LoadProject(string prjname)
        {
            return _api.E_LoadProject(prjname);
        }

        /// <summary>
        /// 加载程序（仅当系统在自动模式下有效）
        /// </summary>
        /// <param name="prjname">工程名称</param>
        /// <param name="prgname">工程下面的程序名称</param>
        /// <returns>成功返回true，失败返回false</returns>
        public bool LoadProgram(string prjname, string prgname)
        {
            return _api.E_LoadProgame(prjname, prgname);
        }

        /// <summary>
        /// 启动程序（仅当系统在自动模式下有效）
        /// </summary>
        /// <returns>成功返回true，失败返回false</returns>
        public bool RunProgram()
        {
            return _api.E_ProgRun();
        }

        /// <summary>
        /// 暂停程序，不会清空动作队列（仅当系统在自动模式下有效）
        /// </summary>
        /// <returns>成功返回true，失败返回false</returns>
        public bool PauseProgram()
        {
            return _api.E_ProgPause();
        }

        /// <summary>
        /// 停止程序并清空动作队列（仅当系统在自动模式下有效）
        /// </summary>
        /// <returns>成功返回true，失败返回false</returns>
        public bool StopProgram()
        {
            return _api.E_ProgStop();
        }

        /// <summary>
        /// api.E_MovJ2（省略了非必要字段）
        /// </summary>
        /// <param name="dstPoint">目标点信息</param>
        /// <param name="velo">速度信息</param>
        /// <param name="zone">过渡参数信息</param>
        /// <param name="isWaitFinished">是否等待动作完成再返回值</param>
        /// <returns>jobId，包含ID字段和时间戳，如果jobId.m_jobID小于0，说明执行命令失败</returns>
        public E_ROB_JOBID_CLI MovJ2(E_ROB_POS_CLI dstPoint, E_ROB_SPEED_CLI velo, E_ROB_ZONE_CLI zone, bool isWaitFinished)
        {
            var jobId = _api.E_MovJ2(dstPoint, velo: velo, zone: zone, IsWaitFinish: isWaitFinished);
            return jobId;
        }

        /// <summary>
        /// api.E_MovL2（省略了非必要字段）
        /// </summary>
        /// <param name="dstPoint">目标点信息</param>
        /// <param name="velo">速度信息</param>
        /// <param name="zone">过渡参数信息</param>
        /// <param name="isWaitFinished">是否等待动作完成再返回值</param>
        public E_ROB_JOBID_CLI MovL2(E_ROB_POS_CLI dstPoint, E_ROB_SPEED_CLI velo, E_ROB_ZONE_CLI zone, bool isWaitFinished)
        {
            var jobId = _api.E_MovL2(dstPoint, velo: velo, zone: zone, IsWaitFinish: isWaitFinished);
            return jobId;
        }

        /// <summary>
        /// api.E_MovC2（省略了非必要字段）
        /// </summary>
        /// <param name="auxP">过渡点信息</param>
        /// <param name="dstPoint">目标点信息</param>
        /// <param name="velo">速度信息</param>
        /// <param name="zone">过渡参数信息</param>
        /// <param name="isWaitFinished">是否等待动作完成再返回值</param>
        /// <returns></returns>
        public E_ROB_JOBID_CLI MovC2(E_ROB_POS_CLI auxP, E_ROB_POS_CLI dstPoint, E_ROB_SPEED_CLI velo, E_ROB_ZONE_CLI zone, bool isWaitFinished)
        {
            var jobId = _api.E_MovC2(auxP: auxP, dstPoint: dstPoint, velo: velo, zone: zone, IsWaitFinish: isWaitFinished);
            return jobId;
        }

        /// <summary>
        /// 获取当前机器人的关节坐标
        /// </summary>
        /// <returns>机器人的关节坐标（六个轴转动了多少度）</returns>
        public E_ROB_POS_CLI GetCurrentJPos()
        {
            return _api.E_GetCurJPos();
        }

        /// <summary>
        /// 获取当前机器人的世界坐标
        /// </summary>
        /// <returns>机器人第六轴中心的笛卡尔坐标（x, y, z, a, b, c）</returns>
        public E_ROB_POS_CLI GetCurrentWPos()
        {
            return _api.E_GetCurWPos();
        }

        /// <summary>
        /// 获取工作状态
        /// </summary>
        /// <returns>
        /// <para>-1  失败</para>
        /// <para>0   正常</para>
        /// <para>1   机器人错误</para>
        /// <para>2   机器人处于停止状态（需要调用MotionStart）</para>
        /// </returns>
        public int GetAPIStatus()
        {
            return _api.E_GetAPIStatus();
        }

        /// <summary>
        /// 获取运动模式
        /// </summary>
        /// <returns>E_RunModeType_CLI枚举类，1为步进，2为连续</returns>
        public E_RunModeType_CLI GetRunMode()
        {
            return _api.E_GetCurRunMode();
        }

        /// <summary>
        /// 设置运动模式
        /// </summary>
        /// <param name="mode">E_RunModeType_CLI枚举类，1为步进，2为连续</param>
        /// <returns>设置成功返回true，失败返回false</returns>
        public bool SetRunMode(E_RunModeType_CLI mode)
        {
            return _api.E_SetRunMode(mode);
        }

        /// <summary>
        /// 获取所有工具坐标系ID
        /// </summary>
        /// <returns>包含所有工具坐标系ID的值</returns>
        public List<int> GetToolIdList()
        {
            int toolNum = 0;
            List<int> toolIdList = [];
            _api.E_GetToolsID(ref toolNum, ref toolIdList);
            return toolIdList;
        }
        /// <summary>
        /// 获取机器人状态.return 12345表示机器人在线，其余为机器人不在线
        /// </summary>
        /// <returns></returns>
        public int GetRobotStatus()
        {
            return _api.E_GetRobotStatus();
        }

    }
}
