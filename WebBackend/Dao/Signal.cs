using System;
using S7.Net;  // 使用 S7.Net 库
using WebBackend.Service;
using WebBackend.Util;

namespace WebBackend.Dao
{
    public class Signal<SignalDataType> : ISignal
    {
        public string Name { get; set; }
        public SignalDataType PreviousData { get; private set; }
        public SignalDataType CurrentData { get; private set; }
        public string Address { get; private set; }
        public Type Type { get; private set; }
        private readonly PlcService _plcService;
        //添加信号变化的事件
        public event Action<Signal<SignalDataType>> SignalChanged;

        public Signal(string name, string address, Type type, PlcService plcService)
        {
            Name = name;
            Address = address;
            Type = type;
            _plcService = plcService;


            // 初始化 CurrentData
            if (type == typeof(short))
            {
                CurrentData = (SignalDataType)(object)(short)0;
            }
            else if (type == typeof(float))
            {
                CurrentData = (SignalDataType)(object)0.0f;
            }
            else if (type == typeof(bool))
            {
                CurrentData = (SignalDataType)(object)false;
            }
            else
            {
                throw new InvalidOperationException("Unsupported data type");
            }
        }

        // 实现 ISignal 接口
        public string ReadAsString()
        {
            return CurrentData?.ToString();
        }

        public void Flush()
        {
            //当前值存储到PreviousData
            PreviousData = CurrentData;
            //获取新值
            var tmp = _plcService.Plc.Read(Address);

            //Value值从tmp中获取
            if (typeof(SignalDataType) == typeof(short) && tmp is ushort ushortValue)
            {
                CurrentData = (SignalDataType)(object)(short)ushortValue;
            }
            else if (typeof(SignalDataType) == typeof(ushort) && tmp is ushort ushortValue2)
            {
                CurrentData = (SignalDataType)(object)ushortValue2;
            }
            else if (typeof(SignalDataType) == typeof(float) && tmp is float floatValue)
            {
                CurrentData = (SignalDataType)(object)floatValue;
            }
            else if (typeof(SignalDataType) == typeof(bool) && tmp is bool boolValue)
            {
                CurrentData = (SignalDataType)(object)boolValue;
            }
            else
            {
                throw new InvalidOperationException($"Unsupported data type or conversion: SignalDataType={typeof(SignalDataType)}, ReadType={tmp.GetType()}");
            }
            //如果信号值发生变化，触发SignalChanged事件
            if (!CurrentData.Equals(PreviousData))
            {
                SignalChanged?.Invoke(this);
            }
        }

        public SignalDataType Read()
        {
            return CurrentData;
        }

        /// <summary>
        /// 上升沿检测
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public bool IsRisingEdge()
        {
            //检查SignalDataType是否为bool类型
            if (typeof(SignalDataType) == typeof(bool))
            {
                var prev = (bool)(object)PreviousData;
                var curr = (bool)(object)CurrentData;
                Console.WriteLine($"IsRisingEdge: PreviousData={prev}, CurrentData={curr}");
                return !prev && curr;
            }
            throw new InvalidOperationException("Rising edge detection only applies to boolean signals.");
        }

        /// <summary>
        /// 下降沿检测
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public bool IsFallingEdge()
        {
            if (typeof(SignalDataType) == typeof(bool))
            {
                return (bool)(object)PreviousData && !(bool)(object)CurrentData;
            }
            throw new InvalidOperationException("Falling edge detection only applies to boolean signals.");
        }
    }
}
