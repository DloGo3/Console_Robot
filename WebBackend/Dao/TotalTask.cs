using System;
using System.Collections.Generic;
namespace WebBackend.Dao
{
    /// <summary>
    /// 总任务表
    /// </summary>
    public class TotalTask
    {
        /// <summary>
        /// 总任务Id
        /// </summary>
        public int Id { get;  set; }
        /// <summary>
        /// 对应的工艺卡Id
        /// </summary>
        public int ProcessCardId { get;  set; }
        /// <summary>
        /// 批次号
        /// </summary>
        public string BatchNumber { get;  set; }
        /// <summary>
        /// 工件数量
        /// </summary>
        public int WorkpieceCount { get;  set; }
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
