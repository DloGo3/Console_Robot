using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
namespace WebBackend.Service
{
    /// <summary>
    /// 连接数据库并得到工作令号
    /// </summary>
    public class WorkOrderNumberDao
    {
        private readonly string _connectionString;

        public WorkOrderNumberDao(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MySqlConnection");
        }

        // 判断searchable_number是否存在
        public bool ExistsWorkOrderInDb(long searchableNumber)
        {
            const string sql = "SELECT COUNT(1) FROM work_order_number WHERE searchable_number = @searchableNumber";
            using (var conn = new MySql.Data.MySqlClient.MySqlConnection(_connectionString))
            {
                return conn.ExecuteScalar<int>(sql, new { searchableNumber }) > 0;
            }
        }

        // 判断part_name是否存在
        public bool ExistsPartNameInProcessCards(int partName)
        {
            const string sql = "SELECT COUNT(1) FROM process_cards WHERE part_name = @partName";
            using (var conn = new MySql.Data.MySqlClient.MySqlConnection(_connectionString))
            {
                return conn.ExecuteScalar<int>(sql, new { partName }) > 0;
            }
        }
    }
}
