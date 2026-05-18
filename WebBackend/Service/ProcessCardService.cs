using MySql.Data.MySqlClient;
using System;

namespace WebBackend.Service
{
    /// <summary>
    /// 根据工作令号查找对应的工艺卡ID
    /// </summary>
    public class ProcessCardService
    {
        private readonly string _connectionString;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connectionString"></param>
        public ProcessCardService(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// 根据工作令号获取工艺卡ID。
        /// 优先根据工作令号（work_order_number.searchable_number）查询，
        /// 如果未找到，则尝试根据零件名称（process_cards.part_name）查询。
        /// </summary>
        /// <param name="workOrderNumber">工作令号</param>
        /// <param name="partName">零件名称 (产品类别)</param>
        /// <returns>工艺卡ID</returns>
        // ***** 注意：方法签名已更改，添加了 partName 参数 *****
        public int GetProcessCardIdByWorkOrderNumber(long workOrderNumber, int partName)
        {
            // SQL 查询语句，通过联查获取工艺卡ID
            string query = @"
                SELECT 
                    pc.process_card_id
                FROM 
                    process_cards pc
                JOIN 
                    work_order_number won ON pc.work_order_number_id = won.id
                WHERE 
                    won.searchable_number = @WorkOrderNumberId;
            ";

            // 工艺卡ID
            int processCardId = 0;

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    // 打开数据库连接
                    connection.Open();

                    // --- 第一次尝试：根据 WorkOrderNumber 查询 ---
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@WorkOrderNumberId", workOrderNumber);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // 防御性解析字段
                            processCardId = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                            Console.WriteLine($"Found ProcessCardId by WorkOrderNumber: {processCardId}");
                        }
                        else
                        {
                            Console.WriteLine($"No ProcessCardID found for WorkOrderNumber: {workOrderNumber}. Will try by part_name.");
                        }
                    } // MySqlDataReader 在这里关闭

                    // --- 第二次尝试：如果第一次未找到，根据 part_name 查询 ---
                    if (processCardId == 0)
                    {
                        // 根据 part_name 查询的 SQL 语句
                        // 假设 part_name 不是唯一的，使用 LIMIT 1 获取第一个匹配项
                        string partNameQuery = @"
                            SELECT process_card_id 
                            FROM process_cards 
                            WHERE part_name = @PartName 
                            LIMIT 1;
                        ";

                        using (MySqlCommand partNameCommand = new MySqlCommand(partNameQuery, connection))
                        {
                            // ***** 注意：这里使用的是传入的 partName 参数 *****
                            partNameCommand.Parameters.AddWithValue("@PartName", partName);

                            // 使用 ExecuteScalar，因为它更适合检索单个值
                            object result = partNameCommand.ExecuteScalar();

                            if (result != null && result != DBNull.Value)
                            {
                                processCardId = Convert.ToInt32(result);
                                Console.WriteLine($"Found ProcessCardId by part_name: {processCardId}");
                            }
                            else
                            {
                                Console.WriteLine($"No ProcessCardID found by part_name ({partName}) either.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"SQL Error: {ex.Message}");
                    throw; // 保持异常抛出，以便上层逻辑能捕获
                }
            } // MySqlConnection 在这里关闭

            return processCardId;
        }
    }
}
