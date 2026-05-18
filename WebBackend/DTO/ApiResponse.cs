using System.Text.Json.Serialization;

namespace WebBackend.DTO
{
    /// <summary>
    /// 通用API响应格式
    /// </summary>
    /// <typeparam name="T">数据载荷类型</typeparam>
    public record ApiResponse<T>
    {
        /// <summary>
        /// 状态代码（0=成功）
        /// </summary>
        [JsonPropertyOrder(0)]
        public int Code { get; init; }

        /// <summary>
        /// 状态描述信息
        /// </summary>
        [JsonPropertyOrder(1)]
        public string Message { get; init; } = "当前工作模式";

        /// <summary>
        /// 数据载荷（泛型）
        /// </summary>
        [JsonPropertyOrder(2)]
        public T Data { get; init; }

        /// <summary>
        /// 响应时间戳（UTC毫秒）
        /// </summary>
        [JsonPropertyOrder(3)]
        public long Timestamp { get; init; }

        public ApiResponse(int code, T data, string message = "")
        {
            Code = code;
            Data = data;
            Message = message;
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
    }

    /// <summary>
    /// 机械臂工作模式数据载荷
    /// </summary>
    public record RobotModeData
    {
        /// <summary>
        /// 预留状态字段（可用于未来扩展）
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; init; } = "";

        /// <summary>
        /// 实际工作模式详情
        /// </summary>
        [JsonPropertyName("detail")]
        public string Detail { get; init; } = "手动";
    }
}
