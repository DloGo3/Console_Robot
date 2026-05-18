using EstunApiStruct_CLI;

namespace WebBackend.Dao
{
    /// <summary>
    /// 能判断是否需要检测的点位信息类
    /// </summary>
    public class RobotPosition : E_ROB_POS_CLI
    {
        /// <summary>
        /// 为0表示不需要检测，为1表示需要检测
        /// </summary>
        public int det;
        /// <summary>
        /// 额外解析出来的 lightTime 字段（如无则为 null）
        /// </summary>
        public double? lightTime { get; set; }


        /// <summary>
        /// 无参构造函数，默认不是检测点
        /// </summary>
        /// <remarks>posType为0表示是轴关节坐标，为1表示是空间坐标</remarks>
        public RobotPosition()
        {
            det = 0;
            lightTime = null;
        }

        /// <summary>
        /// 重写ToString方法，便于显示数据
        /// </summary>
        /// <returns>多行点位数据</returns>
        override public string ToString()
        {
            string res = "";

            // 关节坐标
            if (this.posType == 0)
            {
                res += "Type: APOS";
            }
            // 笛卡尔坐标
            else
            {
                res += "Type: CPOS";
            }

            res += "\n";

            res += "Position Config: ";
            foreach (var val in this.posCfg)
            {
                res += $"{val} ";
            }

            res += "\n";

            res += "Pos Value: ";
            foreach (var val in this.posValue)
            {
                res += $"{val} ";
            }

            res += "\n";

            res += this.det == 0 ? "toBeDetected: false" : "toBeDetected: true";
            if (this.lightTime != null)
                res += $"\nlightTime: {this.lightTime}";
            return res;
        }
    }
}
