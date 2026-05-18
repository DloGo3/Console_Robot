namespace WebBackend.Dao
{
    /// <summary>
    /// 向PLC输入的信号
    /// </summary>
    public class Signals
    {
        /// <summary>
        /// 信号由之前的0变为1，机械臂开始检测一号位
        /// </summary>
        public bool Site1ArrivalBefore { get; set; }
        /// <summary>
        /// 辊道信号变为常1告诉PLC到达一号检测位
        /// </summary>
        public bool Site1Arrival { get; set; }

        /// <summary>
        /// 信号由之前的0变为1，机械臂开始检测二号位
        /// </summary>
        public bool Site2ArrivalBefore { get; set; }
        /// <summary>
        /// 辊道信号变为常1告诉PLC到达二号检测位
        /// </summary>
        public bool Site2Arrival { get; set; }

        /// <summary>
        /// 辊道向PLC发送脉冲表示到达缺陷区域
        /// </summary>
        public bool SiteDefectArrival { get; set; }

        /// <summary>
        /// 在一号检测位未检出缺陷 机械臂控制程序向PLC发送脉冲
        /// </summary>
        public bool Site1NoDefect { get; set; }


        /// <summary>
        /// 在二号检测位未检出缺陷 机械臂控制程序向PLC发送脉冲
        /// </summary>
        public bool Site2NoDefect { get; set; }

        /// <summary>
        /// 在一号检测位检出缺陷 机械臂控制程序向PLC发送脉冲
        /// </summary>
        public bool Site1Defect { get; set; }

        /// <summary>
        /// 在二号检测位检出缺陷 机械臂控制程序向PLC发送脉冲
        /// </summary>
        public bool Site2Defect { get; set; }

        /// <summary>
        /// Glama调姿完成 工件位于底面检测位时为1，其余为0
        /// </summary>
        public bool ProductPositionAdjusted { get; set; }
        


    }
}
