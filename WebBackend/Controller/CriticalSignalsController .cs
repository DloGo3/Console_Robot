using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using WebBackend.Dao;
using WebBackend.Service;

namespace WebBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CriticalSignalsController : ControllerBase
    {
        private readonly ILogger<CriticalSignalsController> _logger;
        private readonly PlcService _plcService;

        // 关键信号定义
        private static readonly List<SignalConfig> _signalConfigs = new()
        {
            new SignalConfig("site1InspectStart", 0.0m, "立式检测位开始检测"),
            new SignalConfig("site1InspectCompleted", 0.1m, "立式检测位检测完毕"),
            new SignalConfig("site2InspectStart", 0.2m, "倾斜检测位开始检测"),
            new SignalConfig("site2InspectCompleted", 0.3m, "倾斜检测位检测完毕")
        };

        public CriticalSignalsController(
            ILogger<CriticalSignalsController> logger,
            PlcService plcService)
        {
            _logger = logger;
            _plcService = plcService;
        }

        /// <summary>
        /// 获取所有关键信号状态
        /// </summary>
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var result = new List<SignalStatusData>();

                foreach (var config in _signalConfigs)
                {
                    var signal = CreatePlcSignal(config);
                    signal.Flush();  // 立即刷新信号值

                    result.Add(new SignalStatusData
                    {
                        Status = signal.Read(),// 获取Bool类型状态
                        Detail = config.Description,// 中文描述
                  
                    });
                }

                return Ok(new ApiResponse<List<SignalStatusData>>(
                    code: 0,
                    data: result,
                    message: "关键信号状态"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PLC信号读取失败");
                return StatusCode(500, new ApiResponse<object>(
                    code: 500,
                    data: null,
                    message: $"信号监控异常: {ex.Message}"
                ));
            }
        }

        /// <summary>
        /// 创建PLC信号实例（根据图片偏移量配置）
        /// </summary>
        private Signal<bool> CreatePlcSignal(SignalConfig config)
        {
            // 根据图片中的偏移量生成地址格式（DB5.DBX0.x）
            var address = $"DB5.DBX0.{config.Offset.ToString().Split('.')[1]}";

            return new Signal<bool>(
                name: config.Name,
                address: address,
                type: typeof(bool),
                plcService: _plcService
            );
        }

        #region Helper Classes

        /// <summary>
        /// 信号配置（对应图片中的表格列）
        /// </summary>
        private record SignalConfig(
            string Name,         // 对应"名称"列（如site1InspectStart）
            decimal Offset,      // 对应"偏移量"列（0.0-0.3）
            string Description  // 对应"起始值"列的中文描述
        );

        /// <summary>
        /// 信号状态数据实体
        /// </summary>
        public class SignalStatusData
        {
            [JsonPropertyName("status")]
            public bool Status { get; set; }  // 对应Bool类型信号值

            [JsonPropertyName("detail")]
            public string Detail { get; set; }  // 对应中文描述

        
        }

        /// <summary>
        /// 标准化API响应格式
        /// </summary>
        public class ApiResponse<T>
        {
            public int Code { get; }
            public string Message { get; }
            public T Data { get; }

            public ApiResponse(int code, T data, string message)
            {
                Code = code;
                Data = data;
                Message = message;
            }
        }

        #endregion
    }
}
