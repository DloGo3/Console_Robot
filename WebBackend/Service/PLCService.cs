using Microsoft.Extensions.Logging;
using S7.Net;//导入用于与西门子PLC进行通信的S7.Net库
using System;
using WebBackend.Dao;

namespace WebBackend.Service
{
    /// <summary>
    /// PLC业务逻辑类，实际PLC设备通信的服务类
    /// </summary>
    public class PlcService
    {
        /// <summary>
        /// Plc对象，用于控制Plc
        /// </summary>
        public Plc Plc { get; private set; }
        private readonly ILogger<PlcService> _logger;

        /// <summary>   
        /// 初始化PLC服务，创建Plc对象并连接PLC
        /// </summary>
        /// <remarks>
        /// CPU类型和IP地址在config.yaml中设置
        /// </remarks>
        /// <param name="cpuType">PLC的CPU类型，见S7.Net.CpuType</param>
        /// <param name="ipAddress">PLC的IP地址，可在博图软件上进行配置</param>
        /// <param name="rack">
        /// （机架）编号。对于大部分紧凑型PLC（如S7-1200或S7-1500），机架号通常是0。
        /// 这个参数是用来指定PLC系统中的物理或逻辑机架号，对于较大的系统，可能会有多个机架。
        /// </param>
        /// <param name="slot">PLC中CPU模块所在的位置</param>
        /// <param name="logger">日志记录器</param>
        public PlcService(CpuType cpuType, string ipAddress, ILogger<PlcService> logger, short rack = 0, short slot = 0)
        {
            //构造函数
            Plc = new Plc(cpuType, ipAddress, rack, slot);
            _logger = logger;
            try
            {
                Plc.Open();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured when connecting to PLC: {Message}", ex.Message);
            }
        }

        public static class S7PlcConverter
        {
            /// <summary>
            /// 从字节数组的大端字节序转换为 UInt32 (UDInt)
            /// </summary>
            /// <param name="bytes">源字节数组</param>
            /// <param name="startIndex">起始索引</param>
            /// <returns>转换后的 uint 值</returns>
            public static uint ToUInt32(byte[] bytes, int startIndex)
            {
                // 大端字节序: 高位字节在前
                // bytes[startIndex]   是最高位字节
                // bytes[startIndex+3] 是最低位字节
                return (uint)(bytes[startIndex] << 24 |
                               bytes[startIndex + 1] << 16 |
                               bytes[startIndex + 2] << 8 |
                               bytes[startIndex + 3]);
            }

            /// <summary>
            /// 将 UInt32 (UDInt) 转换为大端字节序并写入字节数组
            /// </summary>
            /// <param name="buffer">目标字节数组</param>
            /// <param name="startIndex">起始索引</param>
            /// <param name="value">要写入的 uint 值</param>
            public static void WriteUInt32(byte[] buffer, int startIndex, uint value)
            {
                // 大端字节序: 高位字节在前
                buffer[startIndex] = (byte)(value >> 24);
                buffer[startIndex + 1] = (byte)(value >> 16);
                buffer[startIndex + 2] = (byte)(value >> 8);
                buffer[startIndex + 3] = (byte)value;
            }
        }


        /// <summary>
        /// 从PLC中读取工件详细信息 (从usiMaterial到uiDiameterMM)
        /// </summary>
        /// <param name="dbNumber">DB块号</param>
        /// <param name="startByteAddress">起始字节地址 (对于辊道PLC，这里是 12)</param>
        /// <returns>PartDetails 对象</returns>
        public PartDetails ReadPartDetails(int dbNumber, int startByteAddress)
        {
            try
            {
                if (!Plc.IsConnected)
                {
                    Plc.Open();
                }

                // 从 usiMaterial (offset 12) 到 uiDiameterMM (offset 30, 占2字节)
                // 总长度为 (30 - 12) + 2 = 20 字节
                byte[] rawData = Plc.ReadBytes(DataType.DataBlock, dbNumber, startByteAddress, 20);

                if (rawData == null || rawData.Length < 20)
                {
                    throw new Exception("Failed to read PartDetails data from PLC.");
                }

                // 根据相对偏移量解析字节数组
                return new PartDetails
                {
                    // 源地址 12, 相对地址 0
                    Material = rawData[0],
                    // 源地址 14, 相对地址 2 (UDInt = 4字节)
                    IngotSmeltSerial = S7PlcConverter.ToUInt32(rawData, 2),
                    // 源地址 18, 相对地址 6
                    IngotBodySerial = S7ByteConverter.ToUInt16(rawData, 6),
                    // 源地址 20, 相对地址 8
                    BilletSerial = rawData[8],
                    // 源地址 21, 相对地址 9
                    Group = rawData[9],
                    // 源地址 22, 相对地址 10
                    DieNumber = rawData[10],
                    // 源地址 23, 相对地址 11
                    PressSerial = rawData[11],
                    // 源地址 24, 相对地址 12
                    Route100_400 = rawData[12],
                    // 源地址 25, 相对地址 13
                    Route600 = rawData[13],
                    // 源地址 26, 相对地址 14
                    Route700 = rawData[14],
                    // 源地址 27, 相对地址 15
                    Route800 = rawData[15],
                    // 源地址 28, 相对地址 16
                    LengthMM = S7ByteConverter.ToUInt16(rawData, 16),
                    // 源地址 30, 相对地址 18
                    DiameterMM = S7ByteConverter.ToUInt16(rawData, 18)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("ReadPartDetails error: {Message}", ex.Message);
                throw new Exception($"Read PartDetails failed: {ex.Message}");
            }
        }

        /// <summary>
        /// 往PLC里面写工件详细信息
        /// </summary>
        /// <param name="dbNumber">DB块号</param>
        /// <param name="startByteAddress">起始字节地址 (对于你的PLC，这里是 78)</param>
        /// <param name="details">要写入的PartDetails对象</param>
        public void WritePartDetails(int dbNumber, int startByteAddress, PartDetails details)
        {
            try
            {
                if (!Plc.IsConnected)
                {
                    Plc.Open();
                }

                // 创建一个20字节的数组来填充数据
                byte[] rawData = new byte[20];

                // 根据目标地址的相对偏移量填充数据
                // 目标地址 78, 相对地址 0
                rawData[0] = details.Material;
                // 目标地址 80, 相对地址 2
                S7PlcConverter.WriteUInt32(rawData, 2, details.IngotSmeltSerial);
                // 目标地址 84, 相对地址 6
                S7ByteConverter.WriteUInt16(rawData, 6, details.IngotBodySerial);
                // 目标地址 86, 相对地址 8
                rawData[8] = details.BilletSerial;
                rawData[9] = details.Group;
                rawData[10] = details.DieNumber;
                rawData[11] = details.PressSerial;
                rawData[12] = details.Route100_400;
                rawData[13] = details.Route600;
                rawData[14] = details.Route700;
                rawData[15] = details.Route800;
                // 目标地址 94, 相对地址 16
                S7ByteConverter.WriteUInt16(rawData, 16, details.LengthMM);
                // 目标地址 96, 相对地址 18
                S7ByteConverter.WriteUInt16(rawData, 18, details.DiameterMM);

                Plc.WriteBytes(DataType.DataBlock, dbNumber, startByteAddress, rawData);
            }
            catch (Exception ex)
            {
                _logger.LogError("WritePartDetails error: {Message}", ex.Message);
                throw new Exception($"Write PartDetails failed: {ex.Message}");
            }
        }
        /// <summary>
        /// 在PLC指定位置写入一个位的信息
        /// </summary>
        /// <param name="dataType">S7.Net.DataType类型，具体见官方文档</param>
        /// <param name="db">db块索引号</param>
        /// <param name="startByteAdr">起始字节索引地址</param>
        /// <param name="bitAdr">位地址</param>
        /// <param name="value">布尔值，true写入1，false写入0</param>
        /// <returns>写入成功返回true，否则抛出异常</returns>
        /// <exception cref="Exception">异常，记录了详细的异常信息</exception>
        public bool WriteBit(DataType dataType, int db, int startByteAdr, int bitAdr, bool value)
        {
            try
            {
                // 检查连接是否成功 若连接 直接写入位值
                if (Plc.IsConnected)
                {
                    Plc.WriteBit(dataType, db, startByteAdr, bitAdr, value);
                    return true;
                }
                else
                {
                    try
                    {
                        Plc.Open();
                        Plc.WriteBit(dataType, db, startByteAdr, bitAdr, value);
                        return true;
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError("Can't connect to PLC!");
                        throw new Exception("Can't connect to PLC!");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Write Error: {Message}", ex.Message);
                throw new Exception("写错误：" + ex.Message);
            }
        }

        /// <summary>
        /// 读取PLC指定位置一个位的信息
        /// </summary>
        /// <param name="dataType">S7.Net.DataType类型，具体见官方文档</param>
        /// <param name="db">db块索引号</param>
        /// <param name="startByteAdr">起始字节索引地址</param>
        /// <param name="bitAdr">位地址</param>
        /// <returns>读取成功返回该位的值</returns>
        /// <exception cref="Exception">异常，记录了详细的异常信息</exception>
        public bool ReadBit(DataType dataType, int db, int startByteAdr, int bitAdr)
        {
            try
            {
                // 检查连接是否成功
                if (Plc.IsConnected)
                {
                    // 读取包含指定位的整个字节
                    byte[] buffer = Plc.ReadBytes(dataType, db, startByteAdr, 1);
                    if (buffer != null && buffer.Length > 0)
                    {
                        // 提取特定位的值
                        bool bitValue = (buffer[0] & (1 << bitAdr)) != 0;
                        return bitValue;
                    }
                    else
                    {
                        _logger.LogError("Fail to read or return null value.");
                        throw new Exception("Fail to read or return null value.");
                    }
                }
                else
                {
                    _logger.LogError("Can't connect to PLC!");
                    throw new Exception("Can't connect to PLC!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Read error: {Message}", ex.Message);
                throw new Exception($"PLC read error: {ex.Message}");
            }
        }

        /// <summary>
        /// 读取格式的转换
        /// </summary>
        public static class S7ByteConverter
        {
            // 读取功能：将字节数组转换为UInt16数组
            public static ushort[] ToUInt16Array(byte[] bytes)
            {
                if (bytes == null)
                    throw new ArgumentNullException(nameof(bytes));

                if (bytes.Length % 2 != 0)
                    throw new ArgumentException("字节数组长度必须为偶数");

                ushort[] result = new ushort[bytes.Length / 2];

                for (int i = 0; i < bytes.Length; i += 2)
                {
                    // 由于S7使用大端序,需要将高字节和低字节交换位置
                    result[i / 2] = BitConverter.ToUInt16(new byte[] { bytes[i + 1], bytes[i] }, 0);
                }

                return result;
            }

            // 读取功能：转换单个UInt16值
            public static ushort ToUInt16(byte[] bytes, int startIndex = 0)
            {
                if (bytes == null)
                    throw new ArgumentNullException(nameof(bytes));

                if (bytes.Length < startIndex + 2)
                    throw new ArgumentException("字节数组长度不足");

                return BitConverter.ToUInt16(new byte[] { bytes[startIndex + 1], bytes[startIndex] }, 0);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="values"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException"></exception>
            // 写入功能：将UInt16数组转换为字节数组
            public static byte[] FromUInt16Array(ushort[] values)
            {
                if (values == null)
                    throw new ArgumentNullException(nameof(values));

                byte[] result = new byte[values.Length * 2];

                for (int i = 0; i < values.Length; i++)
                {
                    byte[] bytes = FromUInt16(values[i]);
                    result[i * 2] = bytes[0];
                    result[i * 2 + 1] = bytes[1];
                }

                return result;
            }

            // 写入功能：将单个UInt16值转换为字节数组
            public static byte[] FromUInt16(ushort value)
            {
                byte[] bytes = BitConverter.GetBytes(value);
                // 转换为大端序
                return new byte[] { bytes[1], bytes[0] };
            }

            // 写入功能：将UInt16值写入现有字节数组的指定位置
            public static void WriteUInt16(byte[] buffer, int startIndex, ushort value)
            {
                if (buffer == null)
                    throw new ArgumentNullException(nameof(buffer));

                if (buffer.Length < startIndex + 2)
                    throw new ArgumentException("缓冲区长度不足");

                byte[] bytes = FromUInt16(value);
                buffer[startIndex] = bytes[0];
                buffer[startIndex + 1] = bytes[1];
            }
        }
        /// <summary>
        /// 从plc中读工作令号，返回工作令号
        /// </summary>
        /// <param name="dbNumber"></param>
        /// <param name="startByteAddress"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public WorkOrderNumber ReadWorkOrderNumber(int dbNumber, int startByteAddress)
        {
            try
            {
                if (!Plc.IsConnected)
                {
                    Plc.Open();
                }

                // 读取整个 WorkOrderNumber 数据块（按照偏移计算字节总长度为 12 字节）
                byte[] rawData = Plc.ReadBytes(DataType.DataBlock, dbNumber, startByteAddress, 12);

                if (rawData == null || rawData.Length < 12)
                {
                    throw new Exception("Failed to read WorkOrder data from PLC.");
                }

                // 根据偏移量解析字节数组为 WorkOrderNumber
                return new WorkOrderNumber
                {
                    PartName = rawData[0],         // 偏移量 0，1 字节
                    ProductUnit = rawData[1],     // 偏移量 1，1 字节
                    OrderDate = S7ByteConverter.ToUInt16(rawData, 2),       // 偏移量 2，2 字节
                    CustomerCode = S7ByteConverter.ToUInt16(rawData, 4) ,    // 偏移量 4，2 字节
                    OrderNumber = S7ByteConverter.ToUInt16(rawData, 6), // 偏移量 6，2 字节
                    OrderStartNumber = S7ByteConverter.ToUInt16(rawData, 8), // 偏移量 8，2 字节
                    PartsNumber = S7ByteConverter.ToUInt16(rawData, 10)  // 偏移量 10，2 字节
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("ReadWorkOrder error: {Message}", ex.Message);
                throw new Exception($"Read WorkOrder failed: {ex.Message}");
            }
        }
        /// <summary>
        /// 从plc中只读取 PartName
        /// </summary>
        /// <param name="dbNumber"></param>
        /// <param name="startByteAddress"></param>
        /// <returns>PartName 字段的字节值</returns>
        /// <exception cref="Exception"></exception>
        public byte ReadPartName(int dbNumber, int startByteAddress)
        {
            try
            {
                if (!Plc.IsConnected)
                {
                    Plc.Open();
                }

                // 只读取 PartName (偏移量 0，1 字节)
                byte[] rawData = Plc.ReadBytes(DataType.DataBlock, dbNumber, startByteAddress, 1);

                if (rawData == null || rawData.Length < 1)
                {
                    throw new Exception("Failed to read PartName data from PLC.");
                }

                // 返回 PartName
                return rawData[0];
            }
            catch (Exception ex)
            {
                _logger.LogError("ReadPartName error: {Message}", ex.Message);
                throw new Exception($"Read PartName failed: {ex.Message}");
            }
        }
        /// <summary>
        /// 只读取OrderStartNumber
        /// </summary>
        /// <param name="dbNumber"></param>
        /// <param name="startByteAddress"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int ReadOrderStartNumber(int dbNumber, int startByteAddress)
        {
            try
            {
                if (!Plc.IsConnected)
                {
                    Plc.Open();
                }

                // 读取 OrderStartNumber 的数据块（2 字节）
                byte[] rawData = Plc.ReadBytes(DataType.DataBlock, dbNumber, startByteAddress + 8, 2);

                if (rawData == null || rawData.Length < 2)
                {
                    throw new Exception("Failed to read OrderStartNumber data from PLC.");
                }

                // 将字节数组解析为 16 位无符号整数
                int orderStartNumber = S7ByteConverter.ToUInt16(rawData, 0);
                _logger.LogInformation($"读取到 OrderStartNumber: {orderStartNumber}");
                return orderStartNumber;
            }
            catch (Exception ex)
            {
                _logger.LogError("ReadOrderStartNumber error: {Message}", ex.Message);
                throw new Exception($"Read OrderStartNumber failed: {ex.Message}");
            }
        }

        /// <summary>
        /// 从plc中读到的 WorkOrderNumber 对象中截取
        /// 生成完整的工作令号 32309199002 101（BIGINT）（后三位不要了）为了从数据库中联查
        /// </summary>
        /// <param name="workOrderNumber">WorkOrderNumber 对象</param>
        /// <returns>完整的工作令号（long）</returns>
        public long GetFullWorkOrderNumber(WorkOrderNumber workOrderNumber)
        {
            // 按照格式拼接工作令号
            string fullNumber = $"{workOrderNumber.ProductUnit}" +    // ProductUnit (1 字节)
                                $"{workOrderNumber.OrderDate:D4}" +   // OrderDate (2 字节, 补齐4位)
                                $"{workOrderNumber.CustomerCode:D3}" +// CustomerCode (2 字节, 补齐3位)
                                $"{workOrderNumber.OrderNumber:D3}"; // OrderNumber (2 字节, 补齐3位)
                                //$"{workOrderNumber.PartsNumber:D3}";  // PartsNumber (2 字节, 补齐3位)

            // 将字符串转换为 long
            return long.Parse(fullNumber);
        }
        /// <summary>
        /// 从plc中读到的 WorkOrderNumber 对象中截取
        /// 生成完整的工作令号 32309199002 101（BIGINT）（保留后三位）
        /// </summary>
        /// <param name="workOrderNumber">WorkOrderNumber 对象</param>
        /// <returns>完整的工作令号（long）</returns>
        public long GetFullWorkOrderNumber1(WorkOrderNumber workOrderNumber)
        {
            // 按照格式拼接工作令号
            string fullNumber = $"{workOrderNumber.ProductUnit}" +    // ProductUnit (1 字节)
                                $"{workOrderNumber.OrderDate:D4}" +   // OrderDate (2 字节, 补齐4位)
                                $"{workOrderNumber.CustomerCode:D3}" +// CustomerCode (2 字节, 补齐3位)
                                $"{workOrderNumber.OrderNumber:D3}"+// OrderNumber (2 字节, 补齐3位)
                                $"{workOrderNumber.PartsNumber:D3}";  // PartsNumber (2 字节, 补齐3位)

            // 将字符串转换为 long
            return long.Parse(fullNumber);
        }


        /// <summary>
        /// 往PLC里面写工作令号
        /// </summary>
        /// <param name="dbNumber"></param>
        /// <param name="startByteAddress"></param>
        /// <param name="workOrderNumber"></param>
        /// <exception cref="Exception"></exception>

        public void WriteWorkOrderNumber(int dbNumber, int startByteAddress, WorkOrderNumber workOrderNumber)
        {
            try
            {
                if (!Plc.IsConnected)
                {
                    Plc.Open();
                }

                // 将 WorkOrderNumber 转换为12字节数组,按照偏移量填充数据
                byte[] rawData = new byte[12];
                rawData[0] = workOrderNumber.PartName;  // 偏移量 0，1 字节
                rawData[1] = workOrderNumber.ProductUnit;  // 偏移量 1，1 字节
                S7ByteConverter.WriteUInt16(rawData, 2, workOrderNumber.OrderDate); // 偏移量 2，2 字节
                S7ByteConverter.WriteUInt16(rawData, 4, workOrderNumber.CustomerCode); // 偏移量 4，2 字节
                S7ByteConverter.WriteUInt16(rawData, 6, workOrderNumber.OrderNumber); // 偏移量 6，2 字节
                S7ByteConverter.WriteUInt16(rawData, 8, workOrderNumber.OrderStartNumber); // 偏移量 8，2 字节
                S7ByteConverter.WriteUInt16(rawData, 10, workOrderNumber.PartsNumber); // 偏移量 10，2 字节


                // 写入整个 WorkOrder 数据块到 PLC
                Plc.WriteBytes(DataType.DataBlock, dbNumber, startByteAddress, rawData);
            }
            catch (Exception ex)
            {
                _logger.LogError("WriteWorkOrder error: {Message}", ex.Message);
                throw new Exception($"Write WorkOrder failed: {ex.Message}");
            }
        }



        /// <summary>
        /// 通过向PLC指定位置写1，向PLC发送拍照信号 触发光源
        /// </summary>
        /// <param name="milliseconds">高电平持续多少毫秒</param>
        public void TakePhoto(int milliseconds)
        {
            //如果为0，则默认设置为500毫秒
            if (milliseconds == 0)
            {
                milliseconds = 500;
            }
            try
            {
                //在PLC的指定位置发出一个短暂的控制信号
                //将指定位置的位写入0。
                //将同一位置的位写入1，表示发出信号。
                //等待指定的毫秒数。
                //再次将该位置的位写入0，重置信号
                WriteBit(DataType.DataBlock, 1, 2, 1, false);
                WriteBit(DataType.DataBlock, 1, 2, 1, true);
                System.Threading.Thread.Sleep(milliseconds);
                WriteBit(DataType.DataBlock, 1, 2, 1, false);
            }
            catch (Exception e)
            {
                _logger.LogError("Error occured when taking photo: {Message}", e.Message);
                throw;
            }
        }
    }
}
