using BX_struct_space;
using BXSoftDll_Space;
using System.Threading.Tasks;

namespace WebBackend.Service
{
    /// <summary>
    /// 连接光源
    /// </summary>
    public class HardwareService : IDisposable
    {
        private readonly BXSoft_Dll _dll;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1); // 线程安全锁
        private TaskCompletionSource<_stuReturnRes2> _tcs; // 异步结果包装器

        public HardwareService()
        {
            _dll = new BXSoft_Dll();
            // 注册回调函数
            delegateRecvInfo1 recvHandler = HandleReceivedData;
            _dll.RegistEvent1(ref recvHandler);
        }

        // 封装ReadLEDTest方法
        public string GetLEDTestCommand(int channel)
        {
            return _dll.ReadLEDTest(channel);
        }

        // 连接光源（网口）
        public async Task ConnectAsync(string ip, int port)
        {
            await _semaphore.WaitAsync();
            try
            {
                bool success = _dll.connectTCPIP(ip, port);
                if (!success)
                    throw new Exception("Failed to connect to the light source.");
            }
            finally
            {
                _semaphore.Release();
            }
        }

        // 发送命令并等待响应（通用方法）
        public async Task<_stuReturnRes2> SendCommandAsync(string command)
        {
            await _semaphore.WaitAsync();
            try
            {
                _tcs = new TaskCompletionSource<_stuReturnRes2>();
                _dll.SendData(command);
                return await _tcs.Task.WaitAsync(TimeSpan.FromSeconds(5)); // 超时5秒
            }
            finally
            {
                _semaphore.Release();
            }
        }

        // 回调处理（解析硬件响应）
        private void HandleReceivedData(object data)
        {
            string rawData = (string)data;
            _stuReturnRes2 result = new _stuReturnRes2(0);
            _dll.getreadstatus(ref rawData, ref result);
            _tcs?.TrySetResult(result); // 设置异步结果
        }

        public void Dispose() => _dll.UnconnectTCP();
    }
}
