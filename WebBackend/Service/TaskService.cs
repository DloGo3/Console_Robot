using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using WebBackend.Dao;
using WebBackend.DTO;

namespace WebBackend.Service
{
    /// <summary>
    /// 任务处理业务逻辑类
    /// </summary>
    public class TaskService
    {
        private readonly string _connectionString;
        private readonly ILogger<TaskService> _logger;

        /// <summary>
        /// 有参构造函数
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="logger">日志记录器</param>
        public TaskService(string connectionString, ILogger<TaskService> logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        /// <summary>
        /// 通过任务创建时间获取任务信息
        /// </summary>
        /// <param name="createTime">任务创建时间（UnixMillisecond时间戳）</param>
        /// <returns>成功查询返回对应创建时间的任务信息，查询失败或出错返回创建时间为0的任务信息</returns>
        public Dao.Task GetTaskByCreateTime(long createTime)
        {
            using var connection = new MySqlConnection(_connectionString);
            try
            {
                connection.Open();
                string sql = @"
                    SELECT task.*, trace_type.id as trace_type_id, trace_type.type, trace_type.position, trace_type.version
                    FROM task
                    JOIN trace_type ON task.trace_type_id = trace_type.id
                    WHERE task.create_time = @create_time";
                MySqlCommand cmd = new(sql, connection);
                cmd.Parameters.AddWithValue("@create_time", createTime);

                using MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    var traceType = new TraceType
                    {
                        Id = reader.GetInt32("trace_type_id"),
                        Type = reader.GetString("type"),
                        Position = reader.GetString("position"),
                        Version = reader.GetInt32("version")
                    };

                    var task = new Dao.Task
                    {
                        CreateTime = reader.GetInt64("create_time"),
                        Duration = reader.GetInt64("duration"),
                        IsCompleted = reader.GetBoolean("is_completed"),
                        TraceTypeId = traceType.Id,
                        TraceType = traceType
                    };
                    return task;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred: {Message}", ex.Message);
            }
            return new Dao.Task
            {
                CreateTime = 0,
                Duration = -1,
                IsCompleted = false,
                TraceType = new TraceType()
            };
        }

        /// <summary>
        /// 向数据库中添加新的 <see cref="Task"/> 对象。
        /// </summary>
        /// <param name="task">包含任务信息的 <see cref="Task"/> 对象。</param>
        /// <returns>如果添加成功，则返回影响的行数（理论上是1）；如果添加失败，则返回 -1。</returns>
        public int AddTask(Dao.Task task)
        {
            using var connection = new MySqlConnection(_connectionString);
            try
            {
                connection.Open();
                string sql = @"
                    INSERT INTO task (create_time, duration, is_completed, trace_type_id)
                    VALUES (@create_time, @duration, @is_completed, @trace_type_id)";
                MySqlCommand cmd = new(sql, connection);
                cmd.Parameters.AddWithValue("@create_time", task.CreateTime);
                cmd.Parameters.AddWithValue("@duration", task.Duration);
                cmd.Parameters.AddWithValue("@is_completed", task.IsCompleted);
                //cmd.Parameters.AddWithValue("@trace_type_id", task.TraceTypeId);
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred: {Message}", ex.Message);
            }
            return -1;
        }

        /// <summary>
        /// 通过任务创建时间更新任务信息
        /// </summary>
        /// <param name="task">已有任务</param>
        /// <returns>如果返回-1，表示更新失败，否则返回影响的行数（理论上是1）</returns>
        public int UpdateTaskByCreateTime(Dao.Task task)
        {
            using var connection = new MySqlConnection(_connectionString);
            try
            {
                connection.Open();
                string sql = @"
                    UPDATE task
                    SET duration = @duration,
                        is_completed = @is_completed,
                        trace_type_id = @trace_type_id
                    WHERE create_time = @create_time";
                MySqlCommand cmd = new(sql, connection);
                cmd.Parameters.AddWithValue("@create_time", task.CreateTime);
                cmd.Parameters.AddWithValue("@duration", task.Duration);
                cmd.Parameters.AddWithValue("@is_completed", task.IsCompleted);
                cmd.Parameters.AddWithValue("@trace_type_id", task.TraceTypeId);
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred: {Message}", ex.Message);
            }
            return -1;
        }

        /// <summary>
        /// 通过任务创建时间更新任务信息
        /// </summary>
        /// <param name="task">已有任务</param>
        /// <returns>如果返回-1，表示更新失败，否则返回影响的行数（理论上是1）</returns>
        public async Task<int> UpdateTaskByCreateTimeAsync(Dao.Task task)
        {
            using var connection = new MySqlConnection(_connectionString);
            try
            {
                await connection.OpenAsync();
                string sql = @"
            UPDATE task
            SET duration = @duration,
                is_completed = @is_completed,
                trace_type_id = @trace_type_id
            WHERE create_time = @create_time";
                using var cmd = new MySqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@create_time", task.CreateTime);
                cmd.Parameters.AddWithValue("@duration", task.Duration);
                cmd.Parameters.AddWithValue("@is_completed", task.IsCompleted);
                cmd.Parameters.AddWithValue("@trace_type_id", task.TraceTypeId);
                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred: {Message}", ex.Message);
            }
            return -1;
        }

        /// <summary>
        /// 通过任务创建时间删除任务
        /// </summary>
        /// <param name="createTime">任务创建时间（UnixMillisecond时间戳）</param>
        /// <returns>如果返回-1，表示删除失败，否则返回影响的行数（理论上是1）</returns>
        public int DeleteTaskByCreateTime(long createTime)
        {
            using var connection = new MySqlConnection(_connectionString);
            try
            {
                connection.Open();
                string sql = "DELETE FROM task WHERE create_time = @create_time";
                MySqlCommand cmd = new(sql, connection);
                cmd.Parameters.AddWithValue("@create_time", createTime);
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred: {Message}", ex.Message);
            }
            return -1;
        }

        /// <summary>
        /// 获取所有任务
        /// </summary>
        /// <returns>任务列表</returns>
        public List<Dao.Task> GetAllTasks()
        {
            using var connection = new MySqlConnection(_connectionString);
            List<Dao.Task> tasks = new();
            try
            {
                connection.Open();
                string sql = @"
                    SELECT task.*, trace_type.id as trace_type_id, trace_type.type, trace_type.position, trace_type.version
                    FROM task
                    JOIN trace_type ON task.trace_type_id = trace_type.id";
                MySqlCommand cmd = new(sql, connection);
                using MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var traceType = new TraceType
                    {
                        Id = reader.GetInt32("trace_type_id"),
                        Type = reader.GetString("type"),
                        Position = reader.GetString("position"),
                        Version = reader.GetInt32("version")
                    };

                    var task = new Dao.Task
                    {
                        CreateTime = reader.GetInt64("create_time"),
                        Duration = reader.GetInt64("duration"),
                        IsCompleted = reader.GetBoolean("is_completed"),
                        TraceTypeId = traceType.Id,
                        TraceType = traceType
                    };
                    tasks.Add(task);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred: {Message}", ex.Message);
            }
            return tasks;
        }

        /// <summary>
        /// 根据任务类型的详细信息获取对应的 <see cref="TraceType"/> 对象。
        /// </summary>
        /// <param name="type">任务类型名称。</param>
        /// <param name="position">任务位置。</param>
        /// <param name="version">任务版本。</param>
        /// <returns>如果找到对应的 <see cref="TraceType"/> 对象，则返回该对象；否则返回 null。</returns>
        public TraceType GetTraceTypeByDetails(string type, string position, int version)
        {
            using var connection = new MySqlConnection(_connectionString);
            try
            {
                connection.Open();
                string sql = "SELECT * FROM trace_type WHERE type = @type AND position = @position AND version = @version";
                MySqlCommand cmd = new(sql, connection);
                cmd.Parameters.AddWithValue("@type", type);
                cmd.Parameters.AddWithValue("@position", position);
                cmd.Parameters.AddWithValue("@version", version);

                using MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new TraceType
                    {
                        Id = reader.GetInt32("id"),
                        Type = reader.GetString("type"),
                        Position = reader.GetString("position"),
                        Version = reader.GetInt32("version")
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred: {Message}", ex.Message);
            }
            return null;
        }

        /// <summary>
        /// 向数据库中添加新的 <see cref="TraceType"/> 对象。
        /// </summary>
        /// <param name="traceType">包含任务类型信息的 <see cref="TraceType"/> 对象。</param>
        /// <returns>如果添加成功，则返回新插入记录的 ID；如果添加失败，则返回 -1。</returns>
        public int AddTraceType(TraceType traceType)
        {
            using var connection = new MySqlConnection(_connectionString);
            try
            {
                connection.Open();
                string sql = @"
                    INSERT INTO trace_type (type, position, version)
                    VALUES (@type, @position, @version)";
                MySqlCommand cmd = new(sql, connection);
                cmd.Parameters.AddWithValue("@type", traceType.Type);
                cmd.Parameters.AddWithValue("@position", traceType.Position);
                cmd.Parameters.AddWithValue("@version", traceType.Version);
                cmd.ExecuteNonQuery();
                return (int)cmd.LastInsertedId;
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred: {Message}", ex.Message);
            }
            return -1;
        }

        /// <summary>
        /// 设置采集服务器图片存储目录名，每次任务执行前调用
        /// </summary>
        /// <param name="name">图片存储目录名</param>
        /// <returns>设置成功返回true，否则返回false</returns>
        public async void SetPicturesDir(string name)
        {
            using HttpClient client = new();
            string url = $"http://192.168.1.102:8080?name={name}";
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("SetPicturesDir success to {Name}", name);
            }
            else
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var r = JsonConvert.DeserializeObject<R>(responseBody) ?? new R();
                string message = r.data.ToString() ?? "";
                int code = r.code;
                _logger.LogError("SetPicturesDir failed");
                _logger.LogError("Image acquisition server response: {Message}, HTTP code: {Code}", message, code);
            }
        }

        /// <summary>
        /// 设置采集服务器图片存储目录名，每次任务执行前调用
        /// </summary>
        /// <param name="name">图片存储目录名</param>
        /// <returns>设置成功返回true，否则返回false</returns>
        public async Task<bool> SetPicturesDirAsync(string name)
        {
            using HttpClient client = new();
            string url = $"http://192.168.1.102:8080?name={name}";
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("SetPicturesDir success to {Name}", name);
                return true;
            }
            else
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var r = JsonConvert.DeserializeObject<R>(responseBody) ?? new R();
                string message = r.data.ToString() ?? "";
                int code = r.code;
                _logger.LogError("SetPicturesDir failed");
                _logger.LogError("Image acquisition server response: {Message}, HTTP code: {Code}", message, code);
                return false;
            }
        }

    }
}
