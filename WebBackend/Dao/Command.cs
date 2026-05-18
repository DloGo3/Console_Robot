namespace WebBackend.Dao
{
    /// <summary>
    /// 定义与 .erp 文件格式对应的属性
    /// </summary>
    public class Command
    {
        /// <summary>
        /// 操作类型
        /// </summary>
        public string Type { get; private set; }
        /// <summary>
        /// 参数字典
        /// </summary>
        public Dictionary<string, string> Parameters { get; private set; }
        /// <summary>
        /// Command无参构造参数
        /// </summary>
        /// <remarks>
        /// Type为空字符串
        /// Parameters为空字典
        /// </remarks>
        public Command()
        {
            Type = "";
            Parameters = new Dictionary<string, string>();
        }
        /// <summary>
        /// Command全参构造参数
        /// </summary>
        /// <param name="type">字符串，由ERD文件决定</param>
        /// <param name="parameters">字典，键和值由ERD文件决定</param>
        public Command(string type, Dictionary<string, string> parameters)
        {
            Type = type;
            Parameters = parameters;
        }
        /// <summary>
        /// 重写 ToString 方法，以便在控制台中输出 Command 对象
        /// </summary>
        /// <returns>Command信息字符串</returns>
        public override string ToString()
        {
            // 开始构建字符串表示
            var result = new System.Text.StringBuilder();
            result.Append($"Command Type: {Type}, Parameters: {{");

            // 将每个参数键值对添加到字符串中
            bool first = true;
            foreach (var param in Parameters)
            {
                if (!first)
                    result.Append(", ");
                result.Append($"{param.Key}={param.Value}");
                first = false;
            }

            result.Append("}");
            return result.ToString();
        }
    }

}
