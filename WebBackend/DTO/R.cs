using Newtonsoft.Json;
using System;

namespace WebBackend.DTO
{
    /// <summary>
    /// 与前端统一数据格式的类
    /// </summary>
    public record R(object data = null, int code = 200)
    {
        /// <summary>
        /// 无参构造函数，data为空字符串，code为200
        /// </summary>
        public R() : this(null, 200) { }

        /// <summary>
        /// 不带数据的构造函数，code为需要返回的HTTP状态码
        /// </summary>
        /// <param name="code"></param>
        public R(int code) : this(null, code) { }

        /// <summary>
        /// HTTP状态码为200的构造函数，有数据
        /// </summary>
        /// <param name="data">返回数据</param>
        public R(object data) : this(data, 200) { }

        /// <summary>
        /// 序列化对象为JSON字符串（可嵌套）
        /// </summary>
        /// <returns>JSON字符串</returns>
        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
