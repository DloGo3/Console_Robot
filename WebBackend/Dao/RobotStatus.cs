using static Mysqlx.Crud.Order.Types;

namespace WebBackend.Dao
{
    /// <summary>
    /// 机械臂六状态
    /// </summary>
    public class RobotStatus
    {
        /// <summary>
        /// 机械臂空闲状态（无工艺卡下发任务）
        /// </summary>
        public const int Idle = 0;

        /// <summary>
        /// 机械臂就绪状态（一号位）（接收到工艺卡下发的任务）
        /// </summary>
        public const int Site1Ready = 1;

        /// <summary>
        ///工件到一号检测位了 0→1 检测一号位工件状态
        /// </summary>
        public const int DetectionAtSite1 = 2;

        //分为两种情况 1.无缺陷，机械臂进入就绪状态（二号位）
        //             2.有缺陷，机械臂直接回到idle状态（机械臂每执行完一条轨迹就会回到原位变为idle状态）
        //             等待下一轮的开始

        /// <summary>
        /// 机械臂就绪状态（二号位）
        /// </summary>
        public const int Site2Ready = 3;

        /// <summary>
        /// 检测二号位工件状态
        /// </summary>
        public const int DetectionAtSite2 = 4;

        /// <summary>
        /// 急停，模拟突发事件，之后变为idle状态
        /// </summary>
        public const int Stop = 5;
        /// <summary>
        /// 当前的机械臂状态,初始为Idle
        /// </summary>

        public int CurrentState { get; set; } = Idle;

        /// <summary>
        /// 将 RobotStatus 实例转换为 int 类型
        /// </summary>
        public static int ToInt(RobotStatus status)
        {
            return status.CurrentState;
        }

        /// <summary>
        /// 创建一个 RobotStatus 实例
        /// </summary>
        public static RobotStatus FromInt(int state)
        {
            return new RobotStatus { CurrentState = state };
        }
    }


}

