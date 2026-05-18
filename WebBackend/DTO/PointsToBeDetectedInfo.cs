using WebBackend.Dao;

namespace WebBackend.DTO
{
    /// <summary>
    /// 待检测点的点位信息
    /// </summary>
    /// <param name="FolderName">存储图片的文件夹名称</param>
    /// <param name="Positions">点位信息</param>
    public record PointsToBeDetectedInfo(string FolderName, List<RobotPosition> Positions);
}
