using Microsoft.AspNetCore.Mvc;
using WebBackend.DTO;
using WebBackend.Service;
using Microsoft.Extensions.Logging;
using static WebBackend.DTO.ProcessCardAndTrace;
using WebBackend.Dao;


namespace WebBackend.Controller
{
    /// <summary>
    /// 读取前端数据存放在applicationData中
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]

    public class NewProcessCardController : ControllerBase
    {
        private readonly IApplicationData _applicationData;
        private readonly ILogger<ProcessCardController> _logger;
        private readonly RobotStatus _robotStatus;
        private readonly SemiAutoService _semiAutoService;


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="applicationData"></param>
        /// <param name="logger"></param>
        public NewProcessCardController(IApplicationData applicationData, ILogger<ProcessCardController> logger, SemiAutoService semiAutoService,RobotStatus robotStatus)
        {
            _applicationData = applicationData;
            _logger = logger;
            _robotStatus = robotStatus;
            _semiAutoService = semiAutoService;
        }

        /// <summary>
        ///  接收前端传来的工艺卡数据
        /// </summary>
        /// <param name="processCards"></param>
        /// <returns></returns>

        //[HttpPost("upload")]
        //public IActionResult UploadProcessCard([FromBody] ProcessCardDTO processCard)
        //{
        //    //收到数据把idle转为Site1Ready
        //    _robotStatus.CurrentState = RobotStatus.Site1Ready;
        //    if (processCard == null)
        //    {
        //        _logger.LogWarning("Received empty or null process card data");
        //        return BadRequest("Process card data is missing or invalid.");
        //    }

        //    _logger.LogInformation($"Received process card: {processCard.Name}, ID: {processCard.ProcessCardId}");

        //    // 更新应用程序数据 (将数据存储在 IApplicationData 中)
        //    _applicationData.ProcessCardId = processCard.ProcessCardId;
        //    _applicationData.WorkpieceId = processCard.WorkpieceId;
        //    _applicationData.WorkpieceModelPath = processCard.WorkpieceModelPath;
        //    _applicationData.StcpPath = processCard.StcpPath;
        //    _applicationData.Traces = processCard.Traces.Select(t => new Trace
        //    {
        //        TraceId = t.TraceId,
        //        Name = t.Name,
        //        ErdFilePath = t.ErdFilePath,
        //        ErpFilePath = t.ErpFilePath,
        //        Type = t.Type
        //    }).ToList();

        //    _logger.LogInformation("Process card data successfully stored");

        //    return Ok("Process card data received and stored.");
        //}
        [HttpPost("upload")]
        public IActionResult UploadProcessCard([FromBody] ProcessCardDTO processCard)
        {
            if (processCard == null)
            {
                _logger.LogWarning("Received empty or null process card data");
                return BadRequest("Process card data is missing or invalid.");
            }

            _logger.LogInformation($"Received process card: {processCard.Name}, ID: {processCard.ProcessCardId}");

            // 更新应用程序数据
            UpdateApplicationData(processCard);
            // 记录半自动模式开始时间
            _applicationData.BeginTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(); // 使用 UTC 时间
            //_applicationData.BeginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            //// 将状态切换为 Site1Ready，并启动检测流程
            //_robotStatus.CurrentState = RobotStatus.Site1Ready;
            //_logger.LogInformation("状态切换为 Site1Ready，开始检测流程");
            // 启动后台任务，避免阻塞响应
            System.Threading.Tasks.Task.Run(async () =>
            {
                try
                {
                    await StartInspectionProcess();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error during inspection process: {ex.Message}");
                }
            });
            // 立即返回响应
            return Ok("Process card data received. Inspection process has started.");

            //try
            //{
            //    // 启动检测流程
            //    await StartInspectionProcess();
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError($"Error during inspection process: {ex.Message}");
            //    return StatusCode(500, "An error occurred during the inspection process.");
            //}

            //return Ok("Process card data received and inspection process completed.");
        }

        private void UpdateApplicationData(ProcessCardDTO processCard)
        {
            // 更新 _applicationData 的逻辑
            _applicationData.ProcessCardId = processCard.ProcessCardId;
            _applicationData.WorkpieceId = processCard.WorkpieceId;
            _applicationData.WorkpieceModelPath = processCard.WorkpieceModelPath;
            _applicationData.StcpPath = processCard.StcpPath;
            _applicationData.CurrentTraceType = "半自动";
            //新增半自动模式下的工作令号
            _applicationData.WorkOrderNumber = processCard.lh;



            //_applicationData.Traces = processCard.Traces.Select(t => new Trace
            //{
            //    TraceId = t.TraceId,
            //    Name = t.Name,
            //    ErdFilePath = t.ErdFilePath,
            //    ErpFilePath = t.ErpFilePath,
            //    Type = t.Type
            //}).ToList();

            // 线程安全更新
            lock (_applicationData.TracesLock)
            {
                _applicationData.Traces.Clear();  // 清空旧数据
                _applicationData.Traces.AddRange(  // 添加新数据
                    processCard.Traces.Select(t => new Trace
                    {
                        TraceId = t.TraceId,
                        Name = t.Name,
                        ErdFilePath = t.ErdFilePath,
                        ErpFilePath = t.ErpFilePath,
                        Type = t.Type
                    }).ToList()
                );
            }

            _logger.LogInformation("Process card data successfully stored");
        }

        private async System.Threading.Tasks.Task StartInspectionProcess()
        {
            // 工艺卡要执行的总轨迹数
            _semiAutoService.CalculateTotalTracesInProcessCard();
            try
            {
                //_logger.LogInformation("Starting VerticalInspection...");
                await _semiAutoService.VerticalInspection(); // 直接调用立式检测

                //_logger.LogInformation("Starting ObliqueInspection...");
                await _semiAutoService.ObliqueInspection(); // 直接调用斜式检测
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during inspection process: {ex.Message}");
                throw; // 将异常抛出供上层处理
            }
            finally
            {
                // 无论成功或失败，最终重置时间为 "0"
                _applicationData.BeginTime = 0;
                _logger.LogInformation("半自动模式检测流程已全部完成，BeginTime 已重置");
            }

            //// 检测完成后设置状态为 Idle
            //_robotStatus.CurrentState = RobotStatus.Idle;
            //_logger.LogInformation("检测流程完成，状态设置为 Idle");
        }

    }
}
