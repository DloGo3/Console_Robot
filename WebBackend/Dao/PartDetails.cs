namespace WebBackend.Dao
{
    /// <summary>
    /// 存储从 usiMaterial 到 uiDiameterMM 的工件详细信息
    /// </summary>
    public class PartDetails
    {
        public byte Material { get; set; }           // USInt at offset 12 (Source) / 78 (Dest)
                                                     // 注意：根据PLC的内存对齐规则，UDInt前面通常会有一个字节的填充
        public uint IngotSmeltSerial { get; set; }   // UDInt at offset 14 (Source) / 80 (Dest)
        public ushort IngotBodySerial { get; set; }  // UInt at offset 18 (Source) / 84 (Dest)
        public byte BilletSerial { get; set; }       // USInt at offset 20 (Source) / 86 (Dest)
        public byte Group { get; set; }              // USInt at offset 21 (Source) / 87 (Dest)
        public byte DieNumber { get; set; }          // USInt at offset 22 (Source) / 88 (Dest)
        public byte PressSerial { get; set; }        // USInt at offset 23 (Source) / 89 (Dest)
        public byte Route100_400 { get; set; }       // USInt at offset 24 (Source) / 90 (Dest)
        public byte Route600 { get; set; }           // USInt at offset 25 (Source) / 91 (Dest)
        public byte Route700 { get; set; }           // USInt at offset 26 (Source) / 92 (Dest)
        public byte Route800 { get; set; }           // USInt at offset 27 (Source) / 93 (Dest)
        public ushort LengthMM { get; set; }         // UInt at offset 28 (Source) / 94 (Dest)
        public ushort DiameterMM { get; set; }       // UInt at offset 30 (Source) / 96 (Dest)
    }
}
