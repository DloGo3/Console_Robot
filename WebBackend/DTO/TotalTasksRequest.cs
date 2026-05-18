namespace WebBackend.DTO
{
    /// <summary>
    /// 总任务DTO
    /// </summary>
    /// <param name="ProcessCardId"></param>
    /// <param name="BatchNumber"></param>
    /// <param name="WorkpieceCount"></param>
    /// <param name="StartTime"></param>
    /// <param name="EndTime"></param>
        public record TotalTasksRequest(int ProcessCardId, string BatchNumber, int WorkpieceCount,
            DateTime StartTime, DateTime EndTime);


}
