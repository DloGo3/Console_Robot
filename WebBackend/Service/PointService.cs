using Microsoft.AspNetCore.Mvc;

namespace WebBackend.Service
{
    /// <summary>
    /// 点位服务
    /// </summary>
    /// <param name="control">机器人控制工具类</param>
    public class PointService(WebBackend.Util.Control control)
    {
        /// <summary>
        /// 机器人控制器
        /// </summary>
        private readonly WebBackend.Util.Control _control = control;

        /// <summary>
        /// 获取当前机器人第七轴的位置
        /// </summary>
        /// <returns>机器人第七轴的位置（相对于用户坐标系）</returns>
        public double GetCurrentSeventhAxisPosition()
        {
            return this._control.GetCurrentWPos().posValue[6];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<double> GetToolPositionInUser()
        {
            return _control.GetCurrentWPos().posValue.GetRange(0, 3);
        }

        public List<double> GetUserPositionInWorld()
        {
            // TODO: 目前先写死
            return [947, 0, -60];
        }

        public List<double> GetUserRotationInWorld()
        {
            // TODO: 目前先写死
            return [90, 0, 0];
        }

        public List<double> GetWorkpiecePosition()
        {
            // TODO: 目前先写死
            return [0, 0, 0];
        }
    }
}
