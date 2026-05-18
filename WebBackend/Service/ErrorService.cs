using System.Text;
using WebBackend.Models;
using WebBackend.Util;
using Microsoft.Extensions.Logging;

namespace WebBackend.Service
{
    public class ErrorService
    {
        private readonly WebBackend.Util.Control _control;
        private readonly ILogger<ErrorService> _logger;

        public ErrorService(WebBackend.Util.Control control, ILogger<ErrorService> logger)
        {
            _control = control;
            _logger = logger;
        }

        public ErrorLog GetErrorInfo()
        {
            var errorId = _control.GetErrorId();
            var rawDetail = errorId == 0 ? string.Empty : _control.GetErrorInfo(errorId);

            return new ErrorLog
            {
                ErrorCode = errorId,
                ErrorMessage = ConvertEncoding(rawDetail, "GB2312", "UTF-8"),
                LogTime = DateTime.Now
            };
        }

        private string ConvertEncoding(string input, string from, string to)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            try
            {
                var fromEncoding = Encoding.GetEncoding(from);
                var toEncoding = Encoding.GetEncoding(to);
                var bytes = fromEncoding.GetBytes(input);
                return toEncoding.GetString(bytes);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "编码转换失败");
                return input;
            }
        }
    }
}

