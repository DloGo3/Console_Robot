using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BX_struct_space;
using BXSoftDll_Space;
using MySqlX.XDevAPI.Common;
namespace WebBackend.Service
{
    /// <summary>
    /// 光源的服务类
    /// </summary>
    public class LightSourceService
    {
        private BXSoft_Dll Dll = new BXSoft_Dll();
        private delegateRecvInfo1 callback;
        private TaskCompletionSource<_stuGenerSoftCh> tcsChannelParams;
        private _stuGenerSoftCh? lastChannelParams = null;

        public LightSourceService()
        {
            callback = new delegateRecvInfo1(RecvDataCallback);
            Dll.RegistEvent1(ref callback);

            // 你可以改为读配置
            bool isConnected = Dll.connectTCPIP("192.168.1.10", 10000);
            if (!isConnected)
                throw new Exception("光源控制器连接失败");
        }

        /// <summary>
        /// 读取指定通道参数（异步，适合Web调用）
        /// </summary>
        public async Task<_stuGenerSoftCh?> ReadChannelParametersAsync(int channel)
        {
            tcsChannelParams = new TaskCompletionSource<_stuGenerSoftCh>();
            string readCommand = Dll.ReadLEDTest(channel);
            Console.WriteLine($"[光源] 准备读取通道 {channel} 参数, 发送命令: {readCommand}");
            Dll.SendData(readCommand);
            var timeoutTask = Task.Delay(3000);
            var completedTask = await Task.WhenAny(tcsChannelParams.Task, timeoutTask);
            if (completedTask == tcsChannelParams.Task)
            {
                var result = await tcsChannelParams.Task;
                Console.WriteLine($"[光源] 通道 {channel} 参数读取成功，当前电流值: {result.nCurrentMax}");
                return result;
            }
            else
            {
                Console.WriteLine($"[光源] 通道 {channel} 参数读取超时！");
                return null;
            }
        }

        /// <summary>
        /// 只更改电流并下发
        /// </summary>
        public bool ChangeCurrentMaxAndSet(double newCurrentMax)
        {
            if (lastChannelParams == null)
            {
                Console.WriteLine("[光源] ChangeCurrentMaxAndSet 失败：lastChannelParams 为空，需先调用ReadChannelParametersAsync！");
                return false;
            }
            var newParams = lastChannelParams.Value;
            double oldCurrent = newParams.nCurrentMax;
            newParams.nCurrentMax = newCurrentMax;
            string cmd = Dll.SetLEDTest(newParams);

            Console.WriteLine($"[光源] 准备将通道{newParams.nCh}电流从 {oldCurrent} 更改为 {newCurrentMax}，下发命令: {cmd}");
            Dll.SendData(cmd);

            Console.WriteLine($"[光源] ChangeCurrentMaxAndSet 通道{newParams.nCh} 已下发参数！");
            return true;
        }


        private void RecvDataCallback(object rawData)
        {
            string receivedData = (string)rawData;
            receivedData = receivedData.Trim();
            _stuReturnRes2 result = new _stuReturnRes2(1);
            bool parseSuccess = Dll.getreadstatus(ref receivedData, ref result);

            if (parseSuccess && result.bOK)
            {
                if (result.strComm == "B" && (eReadG)result.eRead == eReadG.LEDParm)
                {
                    _stuGenerSoftCh channelParams = (_stuGenerSoftCh)result.list_oContent[0];
                    lastChannelParams = channelParams;
                    tcsChannelParams?.TrySetResult(channelParams);
                }
            }
        }
    }
}
