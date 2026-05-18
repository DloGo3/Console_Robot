namespace WebBackend.Dao
{
    /// <summary>
    /// 机器人配置信息（接口）
    /// </summary>
    public interface IRobotConfiguration
    {
        /// <summary>
        /// 机器人IP地址（示教器上设置）
        /// </summary>
        string Ip { get; set; }
        /// <summary>
        /// 断线是否重连标识，为true表示断线重连
        /// </summary>
        bool AutoReconnect { get; set; }
        /// <summary>
        /// 全局速度，int值，0~100
        /// </summary>
        int GlobalSpeed { get; set; }
        /// <summary>
        /// 用户坐标系ID
        /// </summary>
        int UserId { get; set; }
        /// <summary>
        /// 工具坐标系ID
        /// </summary>
        int ToolId { get; set; }
    }

    /// <summary>
    /// 机器人配置信息
    /// </summary>
    public class RobotConfiguration : IRobotConfiguration
    {
        /// <summary>
        /// 机器人IP地址（示教器上设置）
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        /// 断线是否重连标识，为true表示断线重连
        /// </summary>
        public bool AutoReconnect { get; set; }
        /// <summary>
        /// 全局速度，int值，0~100
        /// </summary>
        public int GlobalSpeed { get; set; }
        /// <summary>
        /// 用户坐标系ID
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 工具坐标系ID
        /// </summary>
        public int ToolId { get; set; }

        /// <summary>
        /// 无参构造函数，IP为空，全局速度为0，UserId和ToolId为false
        /// </summary>
        public RobotConfiguration()
        {
            Ip = "";
            AutoReconnect = false;
            GlobalSpeed = 0;
            UserId = -1;
            ToolId = -1;
        }
    }
}
