namespace WebBackend.Dao
{
    /// <summary>
    /// 
    /// </summary>

    public class TaskAndSub
    {
        public class Task
        {
            public int TaskId { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime? EndTime { get; set; }
            public int ProcessCardId { get; set; }
            public bool IsCompleted { get; set; }
            public string DetectionResult { get; set; }
            public List<Subtask> Subtasks { get; set; }

            public Task()
            {
                Subtasks = new List<Subtask>();
            }
        }

        public class Subtask
        {
            public int SubtaskId { get; set; }
            public int TaskId { get; set; }
            public int TraceId { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime? EndTime { get; set; }
            public bool IsCompleted { get; set; }
        }
    }
}
