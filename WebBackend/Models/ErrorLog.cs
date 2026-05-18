namespace WebBackend.Models
{
    /// <summary>
    /// 向迪威尔数据库传的错误数据
    /// </summary>
    public class ErrorLog
    {
        /// <summary>
        /// 错误码
        /// </summary>
        public int ErrorCode { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; set; }
        /// <summary>
        /// 上传时间
        /// </summary>
        public DateTime LogTime { get; set; }
    }
}
