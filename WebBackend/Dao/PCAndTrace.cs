namespace WebBackend.Dao
{
    /// <summary>
    /// processcard和trace类
    /// </summary>
    public class PCAndTrace
    {
        /// <summary>
        /// 工艺卡
        /// </summary>
        public class ProcessCard
        {
            public int ProcessCardId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public int StandardId { get; set; }
            public int WorkpieceId { get; set; }
            public string WorkpieceModelPath { get; set; }
            public string StcpPath { get; set; }
            public int CurrentApprovalLevel { get; set; }
            public List<Trace> Traces { get; set; }

            public ProcessCard()
            {
                Traces = new List<Trace>();
            }
        }

        /// <summary>
        /// 轨迹
        /// </summary>
        public class Trace
        {
            public int TraceId { get; set; }
            public string Name { get; set; }
            public string ErdFilePath { get; set; }
            public string ErpFilePath { get; set; }
            public string Type { get; set; }
        }
    }
}
