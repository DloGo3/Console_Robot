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
    /// 提供与 TotalTask 相关的数据库操作(增删改查)
    /// </summary>
    public class TotalTasksService
    {
        private readonly string _connectionString;
        /// <summary>
        /// 构造函数，接受一个IConfiguration对象
        /// </summary>
        /// <param name="configuration"></param>
        public TotalTasksService(IConfiguration configuration)
        {
            //从 appsettings.json 文件中读取连接字符串存储在_connectionString中
            _connectionString = configuration.GetConnectionString("MySqlConnection");
        }

        //一个每次访问Connection属性时都会创建并返回一个新的MySqlConnection对象
        //IDbConnection是接口类型，表示数据库连接，具体的数据库连接实现（MySqlConnection）需要实现这个接口
        //Connection为属性名，表示这个属性用于获取数据库连接，且他的值由=>右侧给出
        //创建一个新的 MySqlConnection 对象，是 IDbConnection 接口的具体实现，用于连接 MySQL 数据库。
        //_connectionString是一个私有字段，包含用于连接数据库的连接字符串。
        private IDbConnection Connection => new MySqlConnection(_connectionString);
        /// <summary>
        /// 定义一个异步方法，获取所有任务，返回一个包含
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<TotalTask>> GetAllTotalTasksAsync()
        {
            //每次需要与数据库交互时，使用 Connection 属性来获取新的数据库连接
            //Connection 属性获取一个新的 MySqlConnection 对象
            //可以确保每次数据库操作使用一个新的连接，从而避免连接共享带来的潜在问题，
            //同时确保连接在使用后被正确释放
            using (var dbConnection = Connection)
            {
                string query = "SELECT * FROM total_tasks";
                //使用 Dapper 的 QueryAsync 方法执行 SQL 查询，
                //并将结果映射到 TotalTask 对象的集合中。
                return await dbConnection.QueryAsync<TotalTask>(query);
            }
        }
        /// <summary>
        /// 根据ID获取任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<TotalTask> GetTotalTaskByIdAsync(int id)
        {
            using (var dbConnection = Connection)
            {
                string query = "SELECT * FROM total_tasks WHERE id = @Id";
                //使用 Dapper 的 QueryFirstOrDefaultAsync 方法执行 SQL 查询，并将结果映射到 TotalTask 对象。
                //如果找不到记录，返回 null。
                return await dbConnection.QueryFirstOrDefaultAsync<TotalTask>(query, new { Id = id });
            }
        }
        /// <summary>
        /// 在总任务表里面增加任务（添加 TotalTask 对象到total_tasks表中）
        /// </summary>
        /// <param name="totalTask"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task AddTotalTaskAsync(TotalTask totalTask)
        {
            //using可以确保在 using 语句块结束时，
            //dbConnection 对象会被正确地释放和关闭，防止数据库连接泄漏。
            //创建数据库连接
            using (var dbConnection = Connection)
            {
                //@ 符号：允许在字符串中使用多行文本
                string query = @"
                    INSERT INTO total_tasks (process_card_id, batch_number, workpiece_count, start_time, end_time) 
                    VALUES (@ProcessCardId, @BatchNumber, @WorkpieceCount, @StartTime, @EndTime)";
                //await：异步等待 ExecuteAsync 方法完成。它不会阻塞当前线程，
                //而是释放线程去处理其他任务。
                //totalTask：包含查询参数的对象。Dapper 会将 totalTask 对象的属性值映射到 SQL 查询中的参数。
                //例如，totalTask.ProcessCardId 的值会被映射到 @ProcessCardId 参数。

                await dbConnection.ExecuteAsync(query, totalTask);
            }
        }
        /// <summary>
        /// 更新数据库中的 total_tasks 表中的一条记录，Task相当于void的异步版本
        /// </summary>
        /// <param name="totalTask"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task UpdateTotalTaskAsync(TotalTask totalTask)
        {
            using (var dbConnection = Connection)
            {
                string query = @"
                    UPDATE total_tasks SET 
                        process_card_id = @ProcessCardId, 
                        batch_number = @BatchNumber, 
                        workpiece_count = @WorkpieceCount, 
                        start_time = @StartTime, 
                        end_time = @EndTime 
                    WHERE id = @Id";
                await dbConnection.ExecuteAsync(query, totalTask);
            }
        }
        /// <summary>
        /// 删除某个任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task DeleteTotalTaskAsync(int id)
        {
            using (var dbConnection = Connection)
            {
                string query = "DELETE FROM total_tasks WHERE id = @Id";
                //new { Id = id }：匿名对象，包含查询参数。
                //Dapper 会将 id 参数的值映射到 SQL 查询中的 @Id 参数。
                await dbConnection.ExecuteAsync(query, new { Id = id });
            }
        }
    }
}
