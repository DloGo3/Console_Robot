namespace WebBackend.DTO
{
    /// <summary>
    /// 通过任务创建时间获取任务信息的请求
    /// </summary>
    public record TaskGetRequest(long CreateTime);
}
