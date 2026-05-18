namespace WebBackend.Service
{
    /// <summary>
    /// 从远程下载轨迹文件
    /// </summary>
    public class FileDownloadService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<FileDownloadService> _logger;

        public FileDownloadService(IConfiguration configuration, ILogger<FileDownloadService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public string BuildCompleteUrl(string relativePath)
        {
            var ip = _configuration["ProductCardBackend:Ip"];
            var port = _configuration["ProductCardBackend:Port"];
            var basePath = _configuration["ProductCardBackend:Base"];

            if (string.IsNullOrEmpty(ip) || string.IsNullOrEmpty(port) || string.IsNullOrEmpty(basePath))
            {
                throw new Exception("配置文件中的 ProductCardBackend 配置不完整。");
            }

            return $"http://{ip}:{port}{basePath.TrimEnd('/')}/{relativePath.TrimStart('/')}";
        }

        public string MapUrlToLocalFilePath(string url)
        {
            try
            {
                var uri = new Uri(url);
                var queryParams = System.Web.HttpUtility.ParseQueryString(uri.Query);
                var actualPath = queryParams["path"];

                if (string.IsNullOrEmpty(actualPath))
                {
                    throw new InvalidOperationException($"URL 缺少路径参数: {url}");
                }

                var fileName = Path.GetFileName(actualPath);
                var decodedFileName = Uri.UnescapeDataString(fileName);
                return Path.Combine("data", decodedFileName);
            }
            catch (Exception ex)
            {
                _logger.LogError($"URL 解析失败: {ex.Message}");
                throw;
            }
        }

        public async Task<string> DownloadFileFromUrlAsync(string fileUrl, string folder)
        {
            var localFilePath = MapUrlToLocalFilePath(fileUrl);

            if (File.Exists(localFilePath))
            {
                _logger.LogInformation($"文件已存在，直接使用: {localFilePath}");
                return localFilePath;
            }

            Directory.CreateDirectory(folder);

            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
            try
            {
                _logger.LogInformation($"开始下载文件: {fileUrl}");
                var response = await client.GetAsync(fileUrl);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"文件下载失败，状态码: {response.StatusCode}");
                }

                var fileBytes = await response.Content.ReadAsByteArrayAsync();
                await File.WriteAllBytesAsync(localFilePath, fileBytes);
                _logger.LogInformation($"文件下载成功: {localFilePath}");
                return localFilePath;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"文件下载失败: {fileUrl}, 错误: {ex.Message}");
                throw;
            }
        }
    }

}
