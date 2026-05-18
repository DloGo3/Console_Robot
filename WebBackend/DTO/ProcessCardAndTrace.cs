using System.Text.Json.Serialization;

namespace WebBackend.DTO
{
    /// <summary>
    /// 工艺卡和轨迹的数据结构（容器类）与宏杰前端传来的对应，8.21新添加了工作令号字段
    /// 封装 和 传输 工艺卡和轨迹的数据，通常用于与前端或外部系统进行交互（如 API 请求和响应的传递）。
    /// 例如，前端请求获取工艺卡信息时，后端可以返回一个包含工艺卡信息和轨迹的 ProcessCardAndTrace 对象。
    /// </summary>
   
    public class ProcessCardAndTrace
    {
        /// <summary>
        /// 工艺卡的数据结构
        /// </summary>
        public class ProcessCardDTO
        {
            [JsonPropertyName("process_card_id")]
            public int ProcessCardId { get; set; }

            // 新增字段 半自动模式下的工作令号
            [JsonPropertyName("lh")]
            public long lh { get; set; }

            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("description")]
            public string Description { get; set; }

            [JsonPropertyName("standard_id")]
            public int StandardId { get; set; }

            [JsonPropertyName("workpiece_id")]
            public int WorkpieceId { get; set; }

            [JsonPropertyName("workpiece_model_path")]
            public string WorkpieceModelPath { get; set; }

            [JsonPropertyName("stcp_path")]
            public string StcpPath { get; set; }

            [JsonPropertyName("current_approval_level")]
            public int CurrentApprovalLevel { get; set; }

            /// <summary>
            /// 要加载不止一条轨迹，所以用list
            /// </summary>
            [JsonPropertyName("traces")]
            public List<TraceDTO> Traces { get; set; }
        }
        /// <summary>
        ///单个轨迹数据的类
        /// </summary>
        public class TraceDTO
        {
            [JsonPropertyName("trace_id")]
            public int TraceId { get; set; }

            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("erd_file_path")]
            public string ErdFilePath { get; set; }

            [JsonPropertyName("erp_file_path")]
            public string ErpFilePath { get; set; }

            [JsonPropertyName("type")]
            public string Type { get; set; }
        }
    }
}
