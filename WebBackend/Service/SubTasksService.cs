using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using WebBackend.Dao;

namespace WebBackend.Service
{
    /// <summary>
    /// 提供与 SubTask 相关的数据库操作。
    /// </summary>
    public class SubTasksService
    {
        //用于存储数据库连接字符串。
        private readonly string _connectionString;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configuration"></param>
        public SubTasksService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MySqlConnection");
        }

        private IDbConnection Connection => new MySqlConnection(_connectionString);
        /// <summary>
        /// 获取所有子任务
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<SubTask>> GetAllSubTasksAsync()
        {
            using (var dbConnection = Connection)
            {
                string query = "SELECT * FROM sub_tasks";
                return await dbConnection.QueryAsync<SubTask>(query);
            }
        }
        /// <summary>
        /// 根据 ID 获取子任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<SubTask> GetSubTaskByIdAsync(int id)
        {
            using (var dbConnection = Connection)
            {
                string query = "SELECT * FROM sub_tasks WHERE id = @Id";
                return await dbConnection.QueryFirstOrDefaultAsync<SubTask>(query, new { Id = id });
            }
        }
        /// <summary>
        /// 添加子任务
        /// </summary>
        /// <param name="subTask"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task AddSubTaskAsync(SubTask subTask)

        {
            using (var dbConnection = Connection)
            {
                string query = @"
                    INSERT INTO sub_tasks (total_task_id, process_card_id, start_time, end_time) 
                    VALUES (@TotalTaskId, @ProcessCardId, @StartTime, @EndTime)";
                await dbConnection.ExecuteAsync(query, subTask);
            }
        }
        /// <summary>
        /// 更新子任务
        /// </summary>
        /// <param name="subTask"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task UpdateSubTaskAsync(SubTask subTask)
        {
            using (var dbConnection = Connection)
            {
                string query = @"
                    UPDATE sub_tasks SET 
                        total_task_id = @TotalTaskId, 
                        process_card_id = @ProcessCardId, 
                        start_time = @StartTime, 
                        end_time = @EndTime 
                    WHERE id = @Id";
                await dbConnection.ExecuteAsync(query, subTask);
            }
        }
        /// <summary>
        /// 删除子任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task DeleteSubTaskAsync(int id)
        {
            using (var dbConnection = Connection)
            {
                string query = "DELETE FROM sub_tasks WHERE id = @Id";
                await dbConnection.ExecuteAsync(query, new { Id = id });
            }
        }
    }
}
