using System;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace WebBackend.Dao
{
    /// <summary>
    /// 每个检测流程对应一个任务
    /// </summary>
    public class Task
    {
        /// <summary>
        /// 任务创建时间
        /// </summary>
        public long CreateTime { get; set; }
        /// <summary>
        /// 任务运行时长
        /// </summary>
        public long Duration { get; set; }
        /// <summary>
        /// 任务是否完成
        /// </summary>
        public bool IsCompleted { get; set; }
        /// <summary>
        /// 任务类型ID
        /// </summary>
        public int TraceTypeId { get; set; }
        /// <summary>
        /// Erd文件的绝对路径
        /// </summary>
        public string ErdAbsolutePath { get; set; }
        /// <summary>
        /// Erp文件的绝对路径
        /// </summary>
        public string ErpAbsolutePath { get; set; }

        /// <summary>
        /// 任务类型
        /// </summary>
        public Dao.TraceType TraceType { get; set; }

        /// <summary>
        /// 无参构造函数，CreateTime为当前时间戳，IsCompleted为false，Duration为-1，TraceType为默认值（Type为空，Position为空，Version为0）
        /// </summary>
        public Task()
        {
            this.CreateTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            this.IsCompleted = false;
            this.Duration = -1;
            this.TraceTypeId = 0; // 默认值为0，表示没有关联的TraceType
            this.TraceType = new Dao.TraceType();
            this.ErdAbsolutePath = "";
            this.ErpAbsolutePath = "";
        }
    }

    /// <summary>
    /// 表示任务类型的类
    /// </summary>
    public class TraceType
    {
        /// <summary>
        /// 任务类型ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 任务类型名称
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 任务位置
        /// </summary>
        public string Position { get; set; }
        /// <summary>
        /// 任务版本
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// 无参构造函数，Type为空，Position为空，Version为0
        /// </summary>
        public TraceType()
        {
            this.Type = string.Empty;
            this.Position = string.Empty;
            this.Version = 0;
        }

        /// <summary>
        /// 有参构造函数
        /// </summary>
        /// <param name="type">任务类型名称</param>
        /// <param name="position">任务位置</param>
        /// <param name="version">任务版本</param>
        public TraceType(string type, string position, int version)
        {
            this.Type = type;
            this.Position = position;
            this.Version = version;
        }

        /// <summary>
        /// 有参构造函数，基于轨迹文件原始名称
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="ArgumentException"></exception>
        public TraceType(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("名称不能为空", nameof(name));
            }

            // 正则表达式模式
            string pattern = @"^([a-zA-Z]+)-([a-zA-Z]+)-(\d+)$";

            // 尝试匹配名称
            var match = Regex.Match(name, pattern);
            if (!match.Success)
            {
                throw new ArgumentException("名称格式不正确，应为 {字母}-{字母}-{数字}", nameof(name));
            }

            Type = match.Groups[1].Value;
            Position = match.Groups[2].Value;
            // 因为导出的轨迹名称最后会默认加一个1，所以这里要减去1再除以10得到正确的版本号
            Version = (int.Parse(match.Groups[3].Value) - 1) / 10;
        }

        /// <summary>
        /// 根据TraceType还原轨迹文件原始名称
        /// </summary>
        /// <returns>ZA-ALL-01这样的字符串，对应data文件夹中的轨迹文件名称</returns>
        public String ToTraceName()
        {
            String res = "";
            res += Type + "-" + Position + "-" + Version + "1";
            return res;
        }
    }
}
