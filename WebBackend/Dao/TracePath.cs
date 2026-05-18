namespace WebBackend.Dao
{
    /// <summary>
    /// 定义 TracePath 类，用于存储查询结果中的 trace_order, erp_path 和 erd_path
    /// </summary>
    public class TracePath
    {
    
        //实现三表联查 最后返回包含erderp绝对路径的列表
        public int TraceOrder { get; set; }
        public string ErpPath { get; set; }
        public string ErdPath { get; set; }

        public string Type { get; set; } // 新增 Type 属性（立式/斜式）
        public string Name { get; set; } // 新增 name 属性(轨迹名称）

    }
}
