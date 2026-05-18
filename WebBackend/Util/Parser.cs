using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using WebBackend.Dao;
using WebBackend.Service;

namespace WebBackend.Util
{
    /// <summary>
    /// 实现解析erperd文件
    /// </summary>
    public class Parser
    {
        /// <summary>
        /// 文件读取器
        /// </summary>
        private static readonly FileReader _fileReader = new ();
       

        /// <summary>
        /// 解析 .erd 文件为一个Dictionary
        /// </summary>
        /// <param name="filename">ERD文件的绝对路径</param>
        /// <returns>一个字典，键为`P/J+数字`格式的轨迹点变量名，值为变量的值</returns>
        /// <exception cref="Exception">读取文件出现异常时抛出</exception>
        public ConcurrentDictionary<string, RobotPosition> ParseErdFileToDict(string filename)
        {
            // 轨迹点列表
            var posDict = new ConcurrentDictionary<string, RobotPosition>();
            try
            {
                // 检查文件是否存在
                if (!File.Exists(filename))
                {
                   
                    throw new FileNotFoundException("ERD 文件不存在", filename);
                }

                Console.WriteLine($"开始读取ERD文件: {filename}");
                // 文件的每一行信息
                string[] lines = _fileReader.ReadAllLines(filename);
                Console.WriteLine($"ERD文件读取完成，共有 {lines.Length} 行");

                foreach (var line in lines)
                {
                    try
                    {
                        // 忽略空行和注释行
                        if (string.IsNullOrWhiteSpace(line) || line.StartsWith("--"))

                            continue;

                        var position = ParsePosition(line);
                        // 只有解析出有效的位置信息才添加到字典中
                        if (position.Item1.Length > 0 && position.Item2.posValue.Count > 0)
                        {
                            posDict.TryAdd(position.Item1, position.Item2);
                        }
                        else
                        {
                            Console.WriteLine($"无效的位置信息行: {line}");
                        }
                    }
                    catch (Exception parseEx)
                    {
                        Console.WriteLine($"解析行失败: {line}, 错误: {parseEx.Message}");
                    }
                }


                // 设置最后一个点位的det字段为2以保证复位
                if (posDict.IsEmpty)
                {
                    var lastPos = posDict.Values.Last();
                    lastPos.det = 2;
                }
                else
                {
                   // Console.WriteLine("ERD文件解析后未找到有效轨迹点信息");
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"文件未找到: {ex.Message}");
            }
            catch (IOException ioEx)
            {
                Console.WriteLine($"读取文件时发生IO异常: {ioEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"解析ERD文件时发生未知错误: {ex.Message}");
            }
            return posDict;
        }

        /// <summary>
        /// 解析代表系统变量的 .erd 文件为两个Dictionary，一个表示ZONE，一个表示SPEED
        /// </summary>
        /// <param name="filename">system.erd文件的绝对路径</param>
        /// <param name="zoneDict">一个字典，键是`C+数字`格式的字符串，值是E_ROB_ZONE_CLI类型的过渡系数</param>
        /// <param name="speedDict">一个字典，键是`V+数字`格式的字符串，值是E_ROB_SPEED_CLI类型的速度系数</param>
        /// <returns>如果两个字典均不为空返回true，否则返回false</returns>
        /// <exception cref="Exception">读取文件出现异常时抛出</exception>
        public bool ParseSystemErdFileToDict
            (string filename,
            ConcurrentDictionary<string, EstunApiStruct_CLI.E_ROB_ZONE_CLI> zoneDict,
            ConcurrentDictionary<string, EstunApiStruct_CLI.E_ROB_SPEED_CLI> speedDict
            )
        {
            String[] lines = _fileReader.ReadAllLines(filename);

            foreach (var line in lines)
            {
                // 忽略空行和注释行
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("--"))
                {
                    continue;
                }
                // 解析zone
                if (line.StartsWith('C'))
                {
                    var zone = ParseZone(line);
                    zoneDict.TryAdd(zone.Item1, zone.Item2);
                }
                // 解析Speed
                if (line.StartsWith('V'))
                {
                    var speed = ParseSpeed(line);
                    speedDict.TryAdd(speed.Item1, speed.Item2);
                }
            }
            return !zoneDict.IsEmpty && !speedDict.IsEmpty;
        }

        /// <summary>
        /// 解析 .erp 文件为一个List
        /// </summary>
        /// <param name="filename">ERD文件的绝对路径</param>
        /// <returns>一个列表，每个元素代表一个命令</returns>
        /// <exception cref="Exception">读取文件出现异常时抛出</exception>
        public List<Command> ParseErpFileToList(string filename)
        {
            var commands = new List<Command>();
            try
            {
                Console.WriteLine($"开始读取ERP文件: {filename}");
                String[] lines = _fileReader.ReadAllLines(filename);
                Console.WriteLine($"ERP文件读取完成，共有 {lines.Length} 行");

                foreach (var line in lines)
                {
                    // 忽略空行和注释行
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("--"))
                    {
                        continue;
                    }
                    var command = ParseCommand(line);
                    // 只有解析出有效的命令才添加到列表中
                    if (command != null && command.Type.Length > 0)
                    {
                        commands.Add(command);
                    }
                    else
                    {
                        Console.WriteLine($"解析无效命令: {line}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"读取ERP文件失败: {ex.Message}");
                throw;
            }
            return commands;
        }

        /// <summary>
        /// 根据一行数据解析出 E_ROB_POS_CLI 对象
        /// </summary>
        /// <param name="line">erd文件的一行（保证不是空行且不是注释）</param>
        /// <returns>
        /// <para>如果解析的是以P或者J开头的行，则返回一个Tuple：第一个元素代表格式是`P/J+数字`的轨迹点变量名，第二个元素代表该轨迹点对应的RobotPosition类的坐标；</para>
        /// <para>否则返回的Tuple中第一个元素为空字符串</para>
        /// </returns>
        /// <exception cref="Exception">某个部分解析格式出错抛出对应异常</exception>
        public Tuple<string, RobotPosition> ParsePosition(string line)
        {
            // 解析正负整数
            string intPattern = @"((\-|\+)?\d+)";
            // 解析整数或者小数
            string numberPattern = @"((\-|\+)?\d+(\.\d+)?)";
            // 提取变量名
            string namePattern = @"([P|J][\d]+)={";
            // 存储变量名
            string posName;
            // 存储正则表达式
            string pattern;
            // 根据det判断是否为检测点和结束点
            int det = 0;

            // CPOS
            if (line.StartsWith('P'))
            {
                // =========================== 0. 定义构造函数字段 ===========================
                List<int> cfg_Array;
                List<double> cpos_Array;

                // =========================== 1. 解析点位名称 ===========================
                pattern = namePattern;
                Regex regex = new(pattern);
                Match match = regex.Match(line);

                if (match.Success)
                {
                    posName = match.Groups[1].Value;
                }
                else
                {
                    throw new Exception("Invalid CPOS format in point name.");
                }

                // =========================== 2. 解析PosCfg ===========================
                pattern = $@"mode={intPattern},cf1={intPattern},cf2={intPattern},cf3={intPattern},cf4={intPattern},cf5={intPattern},cf6={intPattern}";
                regex = new Regex(pattern);
                match = regex.Match(line);

                if (match.Success)
                {
                    // 通过索引提取匹配的分组值
                    int mode = int.Parse(match.Groups[1].Value);
                    int cf1 = int.Parse(match.Groups[3].Value);
                    int cf2 = int.Parse(match.Groups[5].Value);
                    int cf3 = int.Parse(match.Groups[7].Value);
                    int cf4 = int.Parse(match.Groups[9].Value);
                    int cf5 = int.Parse(match.Groups[11].Value);
                    int cf6 = int.Parse(match.Groups[13].Value);
                    cfg_Array = [mode, cf1, cf2, cf3, cf4, cf5, cf6];
                }
                else
                {
                    throw new Exception("Invalid CPOS format in confdata");
                }

                // =========================== 2. 解析笛卡尔坐标系 ===========================
                pattern = $@"x={numberPattern},y={numberPattern},z={numberPattern},a={numberPattern},b={numberPattern},c={numberPattern}";
                // 新增LightTime参数
                //pattern = $@"x={numberPattern},y={numberPattern},z={numberPattern},a={numberPattern},b={numberPattern},c={numberPattern},lightTime={numberPattern}";
                match = Regex.Match(line, pattern);

                if (match.Success)
                {
                    // 通过索引提取匹配的分组值
                    double x = double.Parse(match.Groups[1].Value);
                    double y = double.Parse(match.Groups[4].Value);
                    double z = double.Parse(match.Groups[7].Value);
                    double a = double.Parse(match.Groups[10].Value);
                    double b = double.Parse(match.Groups[13].Value);
                    double c = double.Parse(match.Groups[16].Value);
                    //新增字段
                    //double lightTime = double.Parse(match.Groups[19].Value); 
                    cpos_Array = [x, y, z, a, b, c];
                }
                else
                {
                    throw new Exception("Invalid CPOS format in x,y,z,a,b,c.");
                }

                // =========================== 3. 解析额外的轴 ===========================
                pattern = $@"a7={numberPattern},a8={numberPattern},a9={numberPattern},a10={numberPattern},a11={numberPattern},a12={numberPattern},a13={numberPattern},a14={numberPattern},a15={numberPattern},a16={numberPattern}";
                match = Regex.Match(line, pattern);

                if (match.Success)
                {
                    // 通过索引提取匹配的分组值
                    double a7 = double.Parse(match.Groups[1].Value);
                    double a8 = double.Parse(match.Groups[4].Value);
                    double a9 = double.Parse(match.Groups[7].Value);
                    double a10 = double.Parse(match.Groups[10].Value);
                    double a11 = double.Parse(match.Groups[13].Value);
                    double a12 = double.Parse(match.Groups[16].Value);
                    double a13 = double.Parse(match.Groups[19].Value);
                    double a14 = double.Parse(match.Groups[22].Value);
                    double a15 = double.Parse(match.Groups[25].Value);
                    double a16 = double.Parse(match.Groups[28].Value);
                    cpos_Array.AddRange([a7, a8, a9, a10, a11, a12, a13, a14, a15, a16]);
                }
                else
                {
                    throw new Exception("Invalid CPOS format in extra axes position.");
                }

                // =========================== 4. 解析是否为检测点 ===========================
                pattern = $@"det=(\d)";
                match = Regex.Match(line, pattern);

                if (match.Success)
                {
                    det = int.Parse(match.Groups[1].Value);
                    var pos = new RobotPosition
                    {
                        posType = 1,
                        posCfg = cfg_Array,
                        posValue = cpos_Array,
                        det = det
                    };
                    // ===== 新增 lightTime 字段解析 =====
                    pattern = @"lightTime=([+-]?\d+(\.\d+)?)";
                    match = Regex.Match(line, pattern);
                    if (match.Success)
                    {
                        pos.lightTime = double.Parse(match.Groups[1].Value);
                    }
                    else
                    {
                        pos.lightTime = null;
                    }
                    return Tuple.Create(posName, pos);
                }
                else
                {
                    throw new Exception("Invalid CPOS format in det.");
                }
            }
            // APOS
            if (line.StartsWith('J'))
            {
                // =========================== 0. 定义构造函数字段 ===========================
                List<double> apos_Array;

                // =========================== 1. 解析点位名称 ===========================
                pattern = namePattern;
                Regex regex = new (pattern);
                Match match = regex.Match(line);

                if (match.Success)
                {
                    posName = match.Groups[1].Value;
                }
                else
                {
                    throw new Exception("Invalid CPOS format in joint position.");
                }

                // =========================== 2. 解析关节坐标系 ===========================
                pattern = $@"a1={numberPattern},a2={numberPattern},a3={numberPattern},a4={numberPattern},a5={numberPattern},a6={numberPattern},a7={numberPattern},a8={numberPattern},a9={numberPattern},a10={numberPattern},a11={numberPattern},a12={numberPattern},a13={numberPattern},a14={numberPattern},a15={numberPattern},a16={numberPattern}";
                regex = new Regex(pattern);
                match = regex.Match(line);

                if (match.Success)
                {
                    // 通过索引提取匹配的分组值
                    double a1 = double.Parse(match.Groups[1].Value);
                    double a2 = double.Parse(match.Groups[4].Value);
                    double a3 = double.Parse(match.Groups[7].Value);
                    double a4 = double.Parse(match.Groups[10].Value);
                    double a5 = double.Parse(match.Groups[13].Value);
                    double a6 = double.Parse(match.Groups[16].Value);
                    double a7 = double.Parse(match.Groups[19].Value);
                    double a8 = double.Parse(match.Groups[22].Value);
                    double a9 = double.Parse(match.Groups[25].Value);
                    double a10 = double.Parse(match.Groups[28].Value);
                    double a11 = double.Parse(match.Groups[31].Value);
                    double a12 = double.Parse(match.Groups[34].Value);
                    double a13 = double.Parse(match.Groups[37].Value);
                    double a14 = double.Parse(match.Groups[40].Value);
                    double a15 = double.Parse(match.Groups[43].Value);
                    double a16 = double.Parse(match.Groups[46].Value);
                    apos_Array = [a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15, a16];
                }
                else
                {
                    throw new Exception("Invalid APOS format in joint position.");
                }

                // =========================== 3. 解析是否为检测点 ===========================
                pattern = $@"det=(\d)";
                regex = new Regex(pattern);
                match = regex.Match(line);

                if (match.Success)
                {
                    det = int.Parse(match.Groups[1].Value);
                    var pos = new RobotPosition
                    {
                        posType = 0,
                        posValue = apos_Array,
                        det = det
                    };
                    // ===== 新增 lightTime 字段解析 =====
                    pattern = @"lightTime=([+-]?\d+(\.\d+)?)";
                    match = Regex.Match(line, pattern);
                    if (match.Success)
                    {
                        pos.lightTime = double.Parse(match.Groups[1].Value);
                    }
                    else
                    {
                        pos.lightTime = null;
                    }
                    return Tuple.Create(posName, pos);
                }
                else
                {
                    throw new Exception("Invalid CPOS format in det.");
                }
            }

            return new Tuple<string, RobotPosition>("", new RobotPosition());
        }

        /// <summary>
        /// 根据一行数据解析出 ZONE 对象
        /// </summary>
        /// <param name="line">erd文件的一行（保证不是空行且不是注释，且以C开头）</param>
        /// <returns>
        /// 第一个元素代表格式是`C+数字`的过渡变量名，第二个元素代表该变量对应的过渡系数
        /// </returns>
        /// <exception cref="Exception">某个部分解析格式出错抛出对应异常</exception>
        public Tuple<string, EstunApiStruct_CLI.E_ROB_ZONE_CLI> ParseZone(string line)
        {
            // 解析整数或者小数
            string numberPattern = @"((\-|\+)?\d+(\.\d+)?)";
            // 提取变量名
            string namePattern = @"(C[\d]+)={";
            //
            string zoneName = Regex.Match(line, namePattern).Groups[1].Value;
            string pattern = $@"per={numberPattern},dis={numberPattern},vConst={numberPattern}";
            Regex regex = new (pattern);
            Match match = regex.Match(line);
            if (match.Success)
            {
                Int32 m_ZoneType = 2; // 0-无过渡，1-绝对过渡，2-相对过渡
                Double m_Per = Double.Parse(match.Groups[1].Value);
                Double m_dis = Double.Parse(match.Groups[4].Value);
                Int32 m_vConst = Int32.Parse(match.Groups[7].Value);
                EstunApiStruct_CLI.E_ROB_ZONE_CLI ZONE_CLI = new EstunApiStruct_CLI.E_ROB_ZONE_CLI();
                ZONE_CLI.m_ZoneType = m_ZoneType;
                ZONE_CLI.m_Per = m_Per;
                ZONE_CLI.m_dis = m_dis;
                ZONE_CLI.m_vConst = m_vConst;
                return new Tuple<string, EstunApiStruct_CLI.E_ROB_ZONE_CLI>(zoneName, ZONE_CLI);
            }
            else
            {
                throw new Exception("Invalid zone format.");
            }
        }

        /// <summary>
        /// 根据一行数据解析出 SPEED 对象
        /// </summary>
        /// <param name="line">erd文件的一行（保证不是空行且不是注释，且以V开头）</param>
        /// <returns></returns>
        /// <exception cref="Exception">某个部分解析格式出错抛出对应异常</exception>
        public Tuple<string, EstunApiStruct_CLI.E_ROB_SPEED_CLI> ParseSpeed(string line)
        {
            string numberPattern = @"((\-|\+)?\d+(\.\d+)?)";
            string namePattern = @"(V[\d]+)={";

            string speedName = Regex.Match(line, namePattern).Groups[1].Value;
            string pattern = $@"per={numberPattern},tcp={numberPattern},ori={numberPattern},exj_l={numberPattern},exj_r={numberPattern}";
            Regex regex = new (pattern);
            Match match = regex.Match(line);
            if (match.Success)
            {
                double m_Per = double.Parse(match.Groups[1].Value);
                double m_Tcp = double.Parse(match.Groups[4].Value);
                double m_ori = double.Parse(match.Groups[7].Value);
                double m_exj_l = double.Parse(match.Groups[10].Value);
                double m_exj_r = double.Parse(match.Groups[13].Value);

                EstunApiStruct_CLI.E_ROB_SPEED_CLI SPEED_CLI = new()
                {
                    m_Per = m_Per,
                    m_Tcp = m_Tcp,
                    m_ori = m_ori,
                    m_exj_l = m_exj_l,
                    m_exj_r = m_exj_r
                };

                return new Tuple<string, EstunApiStruct_CLI.E_ROB_SPEED_CLI>(speedName, SPEED_CLI);
            }
            else
            {
                throw new Exception("Invalid speed format.");
            }
        }

        /// <summary>
        /// 解析指令
        /// </summary>
        /// <param name="line">erp文件中的一行（保证不是空行和注释行）</param>
        /// <returns>Command对象</returns>
        /// <exception cref="Exception">某个部分解析格式出错抛出对应异常</exception>
        public Command ParseCommand(string line)
        {
            // 用于匹配命令和参数的正则表达式
            var commandMatch = Regex.Match(line, @"(\w+)\{(.+)\}");
            if (!commandMatch.Success)
            {
                return new Command();
            }

            string commandType = commandMatch.Groups[1].Value;

            // 解析参数
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            // 使用正则表达式匹配参数和值
            MatchCollection matches = Regex.Matches(line, @"(\w+)=(\w+(?:\.\w+)?|""[^""]*"")");
            if (matches.Count == 0)
                throw new Exception("Invalid command parameters format.");

            // 将匹配结果存储到字典中
            foreach (Match match in matches.Cast<Match>())
            {
                string parameter = match.Groups[1].Value;
                string value = match.Groups[2].Value.Trim('"'); // 去除引号

                parameters[parameter] = value;
            }

            return new Command(commandType, parameters);
        }


        /// <summary>
        /// 获取待检测点列表（有序）
        /// </summary>
        /// <param name="commandDict">待解析的指令列表</param>
        /// <param name="posDict">所有点位信息字典</param>
        /// <returns>待检测点列表（有序）</returns>
        public List<RobotPosition> ParsePointToBeDetected(List<Command> commandDict,ConcurrentDictionary<string, RobotPosition> posDict)
        {
            List<RobotPosition> points = new();
            foreach (var command in commandDict)
            {
                foreach (var param in command.Parameters)
                {
                    // 只有当参数的键为P时，才表示待检测点
                    if (param.Key.Equals("P"))
                    {
                        var pointName = param.Value.Split('.')[1];
                        var point = posDict[pointName];
                        if(point.det == 1)
                        {
                            points.Add(point);
                        }
                    }
                }
            }
            return points;
        }
    }
}
