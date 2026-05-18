using WebBackend.Dao;
namespace WebBackend.Service
{
    /// <summary>
    /// 机械臂状态机
    /// </summary>
    public class ArmStateMachine
    {
        private readonly Signals _signals;
        private readonly RobotStatus _robotStatus;
        private readonly RobotService _robotService;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="signals"></param>
        /// <param name="robotStatus"></param>
        /// <param name="robotService"></param>
        public ArmStateMachine(Signals signals, RobotStatus robotStatus, RobotService robotService)
        {
            _signals = signals;
            _robotStatus = robotStatus;
            _robotService = robotService;
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void UpdateState()
        {
            switch (_robotStatus.CurrentState)
            {
                case RobotStatus.Idle:
                    if (_signals.Site1Arrival)
                    {
                        _robotStatus.CurrentState = RobotStatus.Site1Ready;
                        
                    }
                    break;
                case RobotStatus.Site1Ready:
                    if (_signals.Site1ArrivalBefore == false && _signals.Site1Arrival == true)
                    {
                        _robotStatus.CurrentState = RobotStatus.DetectionAtSite1;
                        // 应在此处加入开始检测一号位的工件的机械臂运动的代码
                    }
                    break;
                case RobotStatus.DetectionAtSite1:
                    if (_signals.Site1NoDefect)
                    {
                        _robotStatus.CurrentState = RobotStatus.Site2Ready;
                        // 检测完无缺陷，准备检测二号位
                    }
                    else if (_signals.Site1Defect)
                    {
                        _robotStatus.CurrentState = RobotStatus.Idle;
                        // 检测有缺陷，机械臂回到Idle状态
                    }
                    break;
                case RobotStatus.Site2Ready:
                    if (_signals.Site2ArrivalBefore == false && _signals.Site2Arrival == true)
                    {
                        _robotStatus.CurrentState = RobotStatus.DetectionAtSite2;
                        // 应在此处加入开始检测二号位的工件的机械臂运动的代码
                    }
                    break;
                case RobotStatus.DetectionAtSite2:
                    // 结束检测二号位的工件，回到Idle状态
                    _robotStatus.CurrentState = RobotStatus.Idle;
                    break;
                case RobotStatus.Stop:
                    // 处理紧急停止情况
                    _robotService.Stop(500);

                    _robotStatus.CurrentState = RobotStatus.Idle;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// 获取当前机械臂状态
        /// </summary>
        public RobotStatus GetCurrentState()
        {
            return _robotStatus;
        }
        /// <summary>
        /// 急停
        /// </summary>
        public void Stop()
        {
            _robotStatus.CurrentState = RobotStatus.Stop;
        }
    }
}
