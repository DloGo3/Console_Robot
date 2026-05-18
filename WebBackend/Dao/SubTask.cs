namespace WebBackend.Dao
{
    /// <summary>
    /// 子任务表
    /// </summary>
    public class SubTask
    {
        /// <summary>
        /// 子任务的Id
        /// </summary>
        public int Id { get;  set; }
        /// <summary>
        /// 对应的总任务Id
        /// </summary>
        public int TotalTaskId { get;  set; }
        /// <summary>
        /// 对应的工艺卡Id
        /// </summary>
        public int ProcessCardId { get;  set; }
        /// <summary>
        /// 这批工件开始检测的时间
        /// </summary>
        public DateTime StartTime { get;  set; }
        /// <summary>
        /// 这批工件结束检测的时间
        /// </summary>
        public DateTime EndTime { get;  set; }
    }
}
