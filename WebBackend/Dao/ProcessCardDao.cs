using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace WebBackend.Dao
{
    public class ProcessCardDao
    {
        private readonly string _connectionString;

        public ProcessCardDao(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// 根据工艺卡 ID 查询产品名称和产品数量
        /// </summary>
        /// <param name="processCardId">工艺卡 ID</param>
        /// <returns>包含产品名称和产品数量的字典</returns>
        public (string Name, int ProductQuantity) GetProductDetailsByProcessCardId(int processCardId)
        {
            // SQL 查询语句
            string query = @"
            SELECT 
                name, 
                product_quantity 
            FROM 
                process_cards 
            WHERE 
                process_card_id = @ProcessCardId
        ";

            // 打开数据库连接并执行查询
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ProcessCardId", processCardId);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // 获取产品名称和产品数量
                                string name = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                                int productQuantity = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);

                                return (name, productQuantity);
                            }
                            else
                            {
                                throw new KeyNotFoundException($"No process card found with ID {processCardId}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"SQL Error: {ex.Message}");
                    throw;
                }
            }
        }
    }
}
