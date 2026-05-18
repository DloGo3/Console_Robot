namespace WebBackend.DTO
{
    /// <summary>
    /// 
    /// </summary>
    public record SubTasksRequest(int ProcessCardId, int TotalTaskId, 
            DateTime StartTime, DateTime EndTime);
}
