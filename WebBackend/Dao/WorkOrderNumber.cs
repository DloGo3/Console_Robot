namespace WebBackend.Dao
{
    /// <summary>
    /// 工作令号 3-2309-199-002-101或32309199002101
    /// PLC中USInt一个字节，UInt两个字节
    /// </summary>
    public class WorkOrderNumber
    {
        /// <summary>
        /// 订单种产品的名称（用于指示5种典型工件类型）
        /// 例Label Show：4
        /// </summary>
        public byte PartName { get; set; }
        /// <summary>
        /// 本订单选用的生产单元（通常为 3，锤为1、压机为2，挤压件为3）
        /// </summary>
        public byte ProductUnit { get; set; }
        /// <summary>
        /// 订单下发的日期（包含年份和月份，如2309表示2023年9月）
        /// </summary>
        public ushort OrderDate { get; set; }
        /// <summary>
        /// 公司规定的客户代码（通常为3位数），199
        /// </summary>
        public ushort CustomerCode { get; set; }
        /// <summary>
        /// 这个月客户下订单的序号（通常为3位数）002
        /// </summary>
        public ushort OrderNumber { get; set; }
        /// <summary>
        /// 订单的开始序号 无表示
        /// </summary>
        public ushort OrderStartNumber { get; set; }
        /// <summary>
        /// 订单工件的序号 101
        /// </summary>
        public ushort PartsNumber { get; set; }
    }
}
