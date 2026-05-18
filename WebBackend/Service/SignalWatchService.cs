using System;
using System.Collections.Generic;
using System.Timers;
using WebBackend.Dao;
using WebBackend.Util;

namespace WebBackend.Service
{
    /// <summary>
    /// 监视PLC信号
    /// </summary>
    public class SignalWatchService
    {
        private readonly List<ISignal> signals;
        private readonly PlcService _plcService;
        private readonly System.Timers.Timer signalRefreshTimer;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="plcService"></param>
        public SignalWatchService(PlcService plcService)
        {
            _plcService = plcService;
            signals = new List<ISignal>();

            // 初始化 Signal 实例
            //var workPieceIdSignal = new Signal<short>("workPieceId", "DB4.DBW0", typeof(short), _plcService);
            //var site1InspectStart = new Signal<bool>("site1InspectStart", "DB5.DBX0.0", typeof(bool), _plcService);
            //var site1InspectCompleted = new Signal<bool>("site1InspectCompleted", "DB5.DBX0.1", typeof(bool), _plcService);
            //var site2InspectStart = new Signal<bool>("site2InspectStart", "DB5.DBX0.2", typeof(bool), _plcService);
            //var site2InspectCompleted = new Signal<bool>("site2InspectCompleted", "DB5.DBX0.3", typeof(bool), _plcService);
            //var inspectResult = new Signal<short>("inspectResult", "DB4.DBW0", typeof(short), _plcService);
            var site1InspectStart = new Signal<bool>("site1InspectStart", "DB6.DBX32.3", typeof(bool), _plcService);
           // var site1InspectCompleted = new Signal<bool>("site1InspectCompleted", "DB6.DBX98.5", typeof(bool), _plcService);
            var site2InspectStart = new Signal<bool>("site2InspectStart", "DB6.DBX32.5", typeof(bool), _plcService);
            //var site2InspectCompleted = new Signal<bool>("site2InspectCompleted", "DB6.DBX99.4", typeof(bool), _plcService);
            //新增正在检测中的信号
            //var bRobotInside_1 = new Signal<bool>("bRobotInside_1", "DB6.DBX98.3", typeof(bool), _plcService);
            //var bRobotInside_2 = new Signal<bool>("bRobotInside_1", "DB6.DBX99.2", typeof(bool), _plcService);




            //signals.Add(workPieceIdSignal);
            signals.Add(site1InspectStart);
            //signals.Add(site1InspectCompleted);
            signals.Add(site2InspectStart);
           // signals.Add(site2InspectCompleted);
            //signals.Add(bRobotInside_1);
            //signals.Add(bRobotInside_2);
            //signals.Add(inspectResult);

            // 定时刷新信号
            signalRefreshTimer = new System.Timers.Timer(15); // 30毫秒固定速率
            signalRefreshTimer.Elapsed += OnSignalRefresh;
            signalRefreshTimer.AutoReset = true;
            signalRefreshTimer.Enabled = true;
        }

        /// <summary>
        /// 获取信号值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="signalName"></param>
        /// <returns></returns>
        public T GetSignal<T>(string signalName) where T : ISignal
        {
            foreach (var signal in signals)
            {
                if (signal.Name.Equals(signalName, StringComparison.OrdinalIgnoreCase))
                {
                    return (T)signal;
                }
            }
            return default(T);
        }
        public T GetSignals<T>(string signalName) where T : ISignal
        {
            // 确保 signals 集合中存储的是 WebBackend.Util.Signal<> 类型
            foreach (var signal in signals.OfType<T>())
            {
                if (signal.Name.Equals(signalName, StringComparison.OrdinalIgnoreCase))
                {
                    return (T)signal;
                }
            }
            return default;
        }

        // 刷新所有信号
        private void OnSignalRefresh(object sender, ElapsedEventArgs e)
        {
            foreach (var signal in signals)
            {
                signal.Flush();
            }
        }

        // 停止定时器
        public void StopRefreshing()
        {
            signalRefreshTimer.Stop();
        }

        // 开始定时器
        public void StartRefreshing()
        {
            signalRefreshTimer.Start();
        }
    }
}

