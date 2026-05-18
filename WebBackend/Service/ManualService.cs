using EstunApiStruct_CLI;
using WebBackend.Dao;

namespace WebBackend.Service
{
    /// <summary>
    /// 手动存点位信息
    /// </summary>
    public class ManualService
    {
        /// <summary>
        /// 全局数据
        /// </summary>
        private readonly IApplicationData _appData;
        /// <summary>
        /// 机器人控制器
        /// </summary>
        private readonly WebBackend.Util.Control _controller;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appData"></param>
        /// <param name="controller"></param>
        public ManualService(IApplicationData appData, WebBackend.Util.Control controller)
        {
            _appData = appData;
            _controller = controller;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pointName"></param>
        public void SaveCurrentPointInfo(string pointName)
        {
            var pos = _controller.GetCurrentWPos();
            _appData.ManuallySavedPoints.Add(new Tuple<string, E_ROB_POS_CLI>(pointName, pos));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Tuple<string, E_ROB_POS_CLI>> GetPointsInfo()
        {
            return _appData.ManuallySavedPoints;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearPointsInfo()
        {
            _appData.ManuallySavedPoints.Clear();
        }
    }
}
