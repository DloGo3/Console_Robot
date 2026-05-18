using Microsoft.AspNetCore.Mvc;
using WebBackend.Service;
using WebBackend.DTO;
using WebBackend.Dao;
using static WebBackend.Service.RobotService;

namespace WebBackend.Controller
{
    /// <summary>
    /// 点位 控制器
    /// </summary>
    /// <param name="pointService">点位服务</param>
    /// <param name="robotService">机器人服务</param>
    /// <param name="applicationData">应用数据</param>
    /// <param name="fileService">文件服务</param>
    /// <param name="logger">日志记录器</param>
    [ApiController]
    [Route("[controller]")]
    public class PointController(AutoOrManDetectService autoOrManDetectService,PointService pointService, RobotService robotService, IApplicationData applicationData, FileService fileService, ProcessCardDao processCardDao, PlcService plcService ,ILogger<PointController> logger) : ControllerBase
    {
        private readonly AutoOrManDetectService _autoOrManDetectService = autoOrManDetectService;
        private readonly PointService _pointService = pointService;
        private readonly RobotService _robotService = robotService;
        private readonly FileService _fileService = fileService;
        private readonly ProcessCardDao _processCardDao = processCardDao;
        private readonly PlcService _plcService = plcService;
        private readonly IApplicationData _applicationData = applicationData;
        private readonly ILogger<PointController> _logger = logger;

        /// <summary>
        /// 获取用于3D实时位置信息的数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("3d")]
        public IActionResult GetRealTimePositionInformation()
        {
            R r = new(
                data: new realTimePositionInformation(
                               this._pointService.GetCurrentSeventhAxisPosition(),
                               this._pointService.GetToolPositionInUser(),
                               this._pointService.GetUserPositionInWorld(),
                               this._pointService.GetUserRotationInWorld(),
                               this._pointService.GetWorkpiecePosition()
                )
            );
            return Ok(r.ToJsonString());
        }

        /// <summary>
        /// 获取当前机械臂的世界坐标系（实际对应示教器上的用户坐标系）
        /// </summary>
        /// <returns>
        /// <para>code=200: data存放六轴转动角度（JSON字符串）</para>
        /// <para>code=500: 程序异常</para>
        /// </returns>
        [HttpGet("wpos")]
        public IActionResult GetCurrentWPOS()
        {
            try
            {
                var pointCoord = _robotService.GetCurrentWPos();
                Dictionary<string, string> data = [];
                data.Add("x", pointCoord.posValue[0].ToString());
                data.Add("y", pointCoord.posValue[1].ToString());
                data.Add("z", pointCoord.posValue[2].ToString());
                data.Add("a", pointCoord.posValue[3].ToString());
                data.Add("b", pointCoord.posValue[4].ToString());
                data.Add("c", pointCoord.posValue[5].ToString());
                data.Add("w", pointCoord.posValue[6].ToString());
                data.Add("point_name", _applicationData.CurrentPointName);//p0, p1..
                data.Add("process_card_id", _applicationData.ProcessCardId.ToString());
                //轨迹类型（检测/标定）改为（全自动/半自动/标定）
                data.Add("trace_type", _applicationData.CurrentTraceType);
                //检测位置（立式/倾斜）
                data.Add("detection_position", _applicationData.DetectionPosition);

                //工作令号
                data.Add("work_order_number", _applicationData.WorkOrderNumber.ToString());
                //启动时间
                data.Add("begin_time",_applicationData.BeginTime.ToString());
                
                // 产品名称和产品数量
                int processCardId = _applicationData.ProcessCardId;
                string name = "";
                int productQuantity = 0;
                // 如果工艺卡ID不等于0，则为正常检测，不是标定
                if (processCardId != 0)
                {
                    (name, productQuantity) = _processCardDao.GetProductDetailsByProcessCardId(processCardId);
                }
               
                // 产品名称和产品数量
                data.Add("product_name", name);
                data.Add("product_quantity", productQuantity.ToString());

                //工艺卡需要执行多少条轨迹

                data.Add("total_traces_in_process_card", _applicationData.TotalTracesInProcessCard.ToString());

                //当前检测位置一共多少条轨迹
                data.Add("total_traces_in_current_position", _applicationData.TotalTracesInCurrentPosition.ToString());

                //正在执行当前位置的第几条轨迹
                data.Add("current_trace_index", _applicationData.CurrentTraceIndex.ToString());

                //正在执行的轨迹名称
                data.Add("current_trace_name", _applicationData.CurrentTraceName);

                // 统计当前轨迹中需要检测的点位数量
                data.Add("detection_point_count", _applicationData.CurrentTraceDetectionPoints.ToString());

                // 当前检测的是第几个产品

                //int orderStartNumber = _plcService.ReadOrderStartNumber(4, 0); // 假设 DB4 起始地址为 0
                int PartNumber = _plcService.ReadPartName(6, 0); // 假设 DB4 起始地址为 0
                _applicationData.CurrentProductIndex = PartNumber; // 更新全局变量
                data.Add("current_product_index", PartNumber.ToString());

                //data.Add("current_product_index", _applicationData.CurrentProductIndex.ToString());

                // 当前轨迹正在执行第几个检测点位
                data.Add("current_detection_point_index", _applicationData.CurrentDetectedPointsNum.ToString());
                // 添加标定相关数据
                if (_applicationData.CalibrationData != null)
                {
                    // 一共拍几次
                    data.Add("total_photos", _applicationData.CalibrationData.Number.ToString());

                    // 当前拍的第几次
                    data.Add("current_photo_index", _applicationData.CurrentPhotoIndex.ToString());

                    //粗糙度值
                    data.Add("roughness_value", _applicationData.RoughnessValue.ToString());
                  
                    // 粗糙度的标定值列表
                    //通过 LINQ 方法链，提取所有 Value，转换为 List<double>。
                    var roughnessCalibrationValues = _applicationData.CalibrationData.Data?.RoughnessCalibration?
                        .Select(rc => rc.Value)
                        .ToList() ?? new List<double>();
                    data.Add("roughness_values_List", System.Text.Json.JsonSerializer.Serialize(roughnessCalibrationValues));

                    // 长度标定值列表
                    var lengthCalibrationValues = _applicationData.CalibrationData.Data?.LengthCalibration?
                        .Select(lc => lc.Value)
                        .ToList() ?? new List<double>();
                    data.Add("length_values", System.Text.Json.JsonSerializer.Serialize(lengthCalibrationValues));
                }
                else
                //无标定数据（正常检测模式）
                {
                    data.Add("total_photos", "0");
                    data.Add("current_photo_index", "0");
                    data.Add("roughness_values", "0");
                    data.Add("length_values", "0");
                }


                return Ok(new R(data, 200).ToJsonString());
            }
            catch
            {
                _logger.LogError("An error occurred while processing your request! Check service log for more details.");
                return StatusCode(500, new { message = "An error occurred while processing your request. Check service log for more details.", code = 500 });
            }
        }

        /// <summary>
        /// 获取当前机械臂的关节坐标系
        /// </summary>
        /// <returns>
        /// <para>code=200: data存放六轴转动角度（JSON字符串）</para>
        /// <para>code=500: 程序异常</para>
        /// </returns>
        [HttpGet("jpos")]
        public IActionResult GetCurrentJPOS()
        {
            try
            {
                return Ok(new R(_robotService.GetCurrentJPos(), 200).ToJsonString());
            }
            catch
            {
                _logger.LogError("An error occurred while processing your request! Check service log for more details.");
                return StatusCode(500, new R("An error occurred while processing your request. Check service log for more details.", 500).ToJsonString());
            }
        }

        /// <summary>
        /// 返回当前已检测点数量
        /// </summary>
        /// <returns>已检测点数量</returns>
        [HttpGet("already-detected")]
        public IActionResult GetCurrentDetectedPoints()
        {
            return Ok(new R(data: _applicationData.CurrentDetectedPointsNum).ToJsonString());
        }

        /// <summary>
        /// 获取当前点位数量
        /// </summary>
        /// <returns>点位数量</returns>
        [HttpGet("count")]
        public IActionResult GetPointCount()
        {
            return Ok(new R(data: _fileService.GetPointCount()).ToJsonString());
        }

        /// <summary>
        /// 获取所有需要检测的点位信息
        /// </summary>
        /// <returns>一个按顺序排列好的字典</returns>
        [HttpGet("all-points-to-be-detected")]
        public IActionResult GetPointsToBeDetected()
        {
            var list = _applicationData.PointsToBeDetected;
            string folderName = _applicationData.CurrentPictureFolderName;
            var data = new PointsToBeDetectedInfo(folderName, list);
            return Ok(new R(data: data).ToJsonString());
        }

        /// <summary>
        /// 设置工具坐标系
        /// </summary>
        /// <param name="request">包含ToolId字段的请求数据</param>
        /// <returns>成功状态码为200，失败状态码为400</returns>
        [HttpPost("tool-id")]
        public IActionResult SetToolId([FromBody] ToolIdRequest request)
        {
            bool ret = _robotService.SetToolId(request.ToolId);
            if (ret)
            {
                return Ok(new R().ToJsonString());
            }
            else
            {
                return BadRequest(new R(code: 400).ToJsonString());
            }
        }
    }
}
