using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using WebBackend.Dao;
namespace WebBackend.Service

{

    /// <summary>
    /// 实现三表联查，最后得到erp，erd的绝对路径
    /// </summary>
    public class TraceService
    {
        private string connectionString;

        /// <summary>
        /// 构造函数，接收并设置数据库连接字符串
        /// </summary>
        /// <param name="connectionString"></param>
        public TraceService(string connectionString)
        {
            this.connectionString = connectionString;
        }

        // 从数据库中查询 trace_paths 列表
        //List<TracePath>：方法的返回类型，表示该方法返回一个包含 TracePath 对象的列表
        //GetTracePaths：方法名称，表示该方法的功能是获取轨迹路径。
        //int processCardId：方法的参数，表示要查询的 process_cards 表中的 ID。
        public List<TracePath> GetTracePaths(int processCardId)
        {
            // SQL 查询语句，通过 process_cards.id 查询相关的 trace_order, erp_path 和 erd_path
            string query = @"
            SELECT
                pct.trace_order,
                t.erp_file_path,
                t.erd_file_path,
                t.type
            FROM
                process_cards pc
            JOIN
                process_cards_traces pct ON pc.process_card_id = pct.process_card_id
            JOIN
                traces t ON pct.trace_id = t.trace_id
            WHERE
                pc.process_card_id = @ProcessCardId
            ORDER BY
                pct.trace_order;
        ";

            // 创建一个列表用于存储查询结果
            List<TracePath> tracePaths = [];

            // 使用 MySqlConnection 创建数据库连接
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                // 使用 MySqlCommand 创建 SQL 命令，并将查询语句和连接传入
                MySqlCommand command = new MySqlCommand(query, connection);
                // 添加 SQL 查询参数
                command.Parameters.AddWithValue("@ProcessCardId", processCardId);

                // 打开数据库连接
                connection.Open();
                // 执行查询并使用 MySqlDataReader 读取结果
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    // 循环读取查询结果的每一行
                    while (reader.Read())
                    {
                        // 将每一行的结果添加到 tracePaths 列表中
                        tracePaths.Add(new TracePath
                        {
                            TraceOrder = reader.GetInt32(0), // 获取 trace_order 列的值
                            ErpPath = reader.GetString(1),   // 获取 erp_path 列的值
                            ErdPath = reader.GetString(2),    // 获取 erd_path 列的值
                            Type = reader.GetString(3)       // 获取 type 列的值
                        });
                    }
                }
            }

            // 返回查询结果列表
            return tracePaths;
        }
    }

    //public class Program
    //{
    //    public static void Main(string[] args)
    //    {
    //        // 设置数据库连接字符串，替换为实际的数据库连接信息
    //        string connectionString = "server=your_server;user=your_user;database=your_database;port=3306;password=your_password;";
    //        // 创建 DatabaseHelper 实例，传入连接字符串
    //        TraceService dbHelper = new TraceService(connectionString);

    //        // 要查询的 process_card_id 值，替换为实际的值
    //        int processCardId = 1;
    //        // 调用 GetTracePaths 方法，获取查询结果
    //        List<TracePath> tracePaths = dbHelper.GetTracePaths(processCardId);

    //        // 循环输出查询结果
    //        foreach (var tracePath in tracePaths)
    //        {
    //            Console.WriteLine($"Trace Order: {tracePath.TraceOrder}, ERP Path: {tracePath.ErpPath}, ERD Path: {tracePath.ErdPath}");
    //        }
    //    }
    //}
}
