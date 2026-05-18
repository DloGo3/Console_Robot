using System.Collections.Concurrent;
using WebBackend.Dao;
using WebBackend.Util;

namespace WebBackend.Service
{
    /// <summary>
    /// 文件服务，用于解析ERD和ERP文件
    /// </summary>
    public class FileService
    {
        /// <summary>
        /// 全局数据
        /// </summary>
        private readonly IApplicationData _appData;
        /// <summary>
        /// 解析器
        /// </summary>
        private readonly Parser _parser;
        /// <summary>
        /// 轨迹文件绝对路径（在config.yaml中设置）
        /// </summary>
        private readonly string _dataDirectory;
        /// <summary>
        /// 日志记录器
        /// </summary>
        private readonly ILogger<FileService> _logger;

        /// <summary>
        /// FileService全参构造函数
        /// </summary>
        /// <param name="appData">全局数据（单例）</param>
        /// <param name="parser">文件解析器（单例）</param>
        /// <param name="config">配置类（单例）</param>
        /// <param name="logger">日志记录器</param>
        /// <exception cref="Exception">若config.yaml中没有设置DataDirectory则抛出此异常</exception>
        public FileService(IApplicationData appData, Parser parser, IConfiguration config, ILogger<FileService> logger)
        {
            this._appData = appData;
            this._parser = parser;
            this._dataDirectory = config.GetValue<string>("DataDirectory") ?? "";
            this._logger = logger;
            if (_dataDirectory == null)
            {
                _logger.LogError("Data directory load error, please check if there are relevant settings set in config.yaml.");
                throw new Exception();
            }
        }

        /// <summary>
        /// 读取并加载ERD文件到内存中
        /// </summary>
        /// <param name="filename">可以是本地文件绝对路径，也可以是共享文件的路径</param>
        /// <returns>全点位数量（包含过渡点和不需检测的点）</returns>
        /// <exception cref="Exception">若解析ERD文件失败则抛出此异常</exception>
        public int ReadErdFile(string filename)
        {
            //TODO 直接改为绝对路径
            try
            {
                var posDict = _parser.ParseErdFileToDict(filename);
                _appData.PosDict = posDict;//更新点位信息
                return posDict.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError("Parse .erd file failed: {ex.Message}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// 读取并加载ERP文件到内存中
        /// </summary>
        /// <param name="filename">可以是本地文件绝对路径，也可以是共享文件的路径</param>
        /// <returns>指令数量</returns>
        /// <exception cref="Exception">若解析ERP文件失败则抛出此异常</exception>
        public int ReadErpFile(string filename)
        {
            //TODO 直接改为绝对路径
            try
            {
                var commandList = _parser.ParseErpFileToList(filename);
                _appData.CommandList = commandList;
                return commandList.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError("Parse .erd file failed: {ex.Message}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// 获取当前轨迹数据文件夹绝对路径
        /// </summary>
        /// <returns>轨迹数据文件夹绝对路径</returns>
        public string GetDataDirectory()
        {
            return _dataDirectory;
        }

        /// <summary>
        /// 获取当前数据文件夹下可导入轨迹的名称列表
        /// </summary>
        /// <returns>当前轨迹名称列表</returns>
        /// <exception cref="DirectoryNotFoundException">如果轨迹数据文件夹不存在，抛出此异常</exception>
        public List<string> GetTraceList()
        {
            if (!Directory.Exists(_dataDirectory))
            {
                _logger.LogError("Data directory not exists! Please check if it's set correctly in the config.yaml file");
                throw new DirectoryNotFoundException("Data directory not exists! Please check if it's set correctly in the config.yaml file");
            }

            DirectoryInfo directoryInfo = new(_dataDirectory);
            var fileList = directoryInfo.GetFiles();
            try
            {
                // 使用 LINQ 进行筛选
                var traceList = fileList
                    // 去除文件后缀
                    .Select(file => Path.GetFileNameWithoutExtension(file.Name))
                    // 去除文件名中包含 "system"和"global" 的项
                    .Where(fileName => !fileName.Contains("system") && !fileName.Contains("global"))
                    // 去除重复的文件名
                    .Distinct()
                    // 保证erd和erp成对存在
                    .Where(fileName => File.Exists(Path.Combine(_dataDirectory, $"{fileName}.erd")) && File.Exists(Path.Combine(_dataDirectory, $"{fileName}.erp")))
                    // 转换为列表
                    .ToList();
                return traceList;
            }
            catch (Exception e)
            {
                _logger.LogError("Error occured while filtering the trace name by file name: {Message}", e.Message);
                throw;
            }
        }

        /// <summary>
        /// 根据轨迹名称（不包含后缀）加载全部点位信息、命令信息和待测点信息
        /// </summary>
        /// <param name="filename">轨迹名称</param>
        /// <returns>加载成功返回true，若ERD或者ERP文件解析出0个条目返回false</returns>
        /// <exception cref="Exception">若ERD或者ERP解析失败则抛出该异常</exception>
        public bool LoadData(string ErdAbsolutePath, string ErpAbsolutePath)
            //TODO
        {
            try
            {
                // 加载ERD文件
                int pointCount = ReadErdFile(ErdAbsolutePath);
                // 加载ERP文件
                int commandCount = ReadErpFile(ErpAbsolutePath);
                // 如果有任意一个为空，返回false
                if (pointCount == 0 || commandCount == 0)
                {
                    return false;
                }
                // 加载待测点字典
                _appData.PointsToBeDetected = _parser.ParsePointToBeDetected(_appData.CommandList, _appData.PosDict);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("{Message}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// 获取当前任务的指令数量
        /// </summary>
        /// <returns>指令数量</returns>
        public int GetCommandCount()
        {
            return _appData.CommandList.Count;
        }

        /// <summary>
        /// 获取当前任务的点位数量
        /// </summary>
        /// <returns>点位数量</returns>
        public int GetPointCount()
        {
            return _appData.PosDict.Count;
        }
    }
}
