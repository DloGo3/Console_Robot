using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using WebBackend.Dao;

namespace WebBackend.Service

{
    /// <summary>
    /// 根据工作令号，实现四表联查，最后返回轨迹的绝对路径
    /// </summary>
    public class NewTraceService
    {
        private readonly string _connectionString;
        private readonly IApplicationData _applicationData;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connectionString"></param>
        public NewTraceService(string connectionString, IApplicationData applicationData)
        {
            _connectionString = connectionString;
            _applicationData = applicationData;

        }

        /// <summary>
        /// 优先根据工作令号联查获取轨迹。
        /// 如果根据工作令号查不到 (searchable_number 不匹配, 导致 tracePaths.Count == 0)，
        /// 则尝试根据产品类别 (part_name) 查。
        /// 当根据产品类别查时，如果有多张工艺卡，则自动选用 process_card_id 最大的那一T张。
        /// </summary>
        /// <param name="workOrderNumber">工作令号 (对应 won.searchable_number)</param>
        /// <param name="partName">产品类别 (对应 pc.part_name)，作为备选查询条件</param>
        /// <returns>轨迹路径列表</returns>
        public List<TracePath> GetTraces(long workOrderNumber, int partName)
        {
            // SQL 查询语句 1：优先使用工作令号
            // (通过 JOIN 和 WHERE won.searchable_number 实现您的“对应上”)
            string queryByWorkOrder = @"
        SELECT
            pct.trace_order,
            t.erd_file_path,
            t.erp_file_path,
            t.type,
            t.name
        FROM 
            process_cards pc
        JOIN 
            process_cards_traces pct ON pc.process_card_id = pct.process_card_id
        JOIN 
            traces t ON pct.trace_id = t.trace_id
        JOIN
            work_order_number won ON pc.work_order_number_id = won.id
        WHERE 
            won.searchable_number = @WorkOrderNumber
            AND pct.enabled = 1 
        ORDER BY 
            pct.trace_order;
    ";

            // SQL 查询语句 2：备选方案，使用产品类别 (part_name)
            string queryByPartName = @"
        SELECT
            pct.trace_order,
            t.erd_file_path,
            t.erp_file_path,
            t.type,
            t.name
        FROM 
            (
                SELECT process_card_id
                FROM process_cards
                WHERE part_name = @PartName
                ORDER BY process_card_id DESC
                LIMIT 1
            ) AS pc 
        JOIN 
            process_cards_traces pct ON pc.process_card_id = pct.process_card_id
        JOIN 
            traces t ON pct.trace_id = t.trace_id
        WHERE 
            pct.enabled = 1 
        ORDER BY 
            pct.trace_order;
    ";

            List<TracePath> tracePaths = new List<TracePath>();

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();

                    // --- 1. 尝试使用工作令号查询 (优先) ---
                    MySqlCommand command = new MySqlCommand(queryByWorkOrder, connection);
                    // !! 请注意这里的参数名
                    command.Parameters.AddWithValue("@WorkOrderNumber", workOrderNumber);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tracePaths.Add(ParseTracePathFromReader(reader));
                        }
                    }

                    // --- 2. 检查结果，如果为空 ("没有对应上")，则尝试使用产品类别查询 (备选) ---
                    if (tracePaths.Count == 0 && partName > 0)
                    {
                        Console.WriteLine($"No traces found for WorkOrderNumber: {workOrderNumber}. Trying PartName: {partName}");

                        command = new MySqlCommand(queryByPartName, connection);
                        command.Parameters.AddWithValue("@PartName", partName);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tracePaths.Add(ParseTracePathFromReader(reader));
                                Console.WriteLine("利用的是part_name查到的轨迹");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"SQL Error: {ex.Message}");
                    throw;
                }

                _applicationData.TotalTracesInProcessCard = tracePaths.Count;
            }

            return tracePaths;
        }

        /// <summary>
        /// 辅助方法：从 MySqlDataReader 解析 TracePath 对象
        /// </summary>
        private TracePath ParseTracePathFromReader(MySqlDataReader reader)
        {
            var traceOrder = reader.IsDBNull(0) ? 0 : Convert.ToInt32(reader[0]);
            var erdPath = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
            var erpPath = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
            var type = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
            var name = reader.IsDBNull(4) ? string.Empty : reader.GetString(4);

            Console.WriteLine($"TracePath Found: TraceOrder={traceOrder}, ErdPath={erdPath}, ErpPath={erpPath}, Type={type}, Name={name}");

            return new TracePath
            {
                TraceOrder = traceOrder,
                ErdPath = erdPath,
                ErpPath = erpPath,
                Type = type,
                Name = name
            };
        }
    }
}

