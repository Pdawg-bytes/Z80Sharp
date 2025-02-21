namespace Z80Sharp.Data
{
    /// <summary>
    /// Defines the Z80's databus.
    /// </summary>
    /// <remarks>
    /// This is implemented as a class for performance reasons.
    /// The extra layer of indirection created by virtual lookups on interfaces are problematic!
    /// </remarks>
    public class DataBus
    {
        /// <summary>
        /// NMI and MI status.
        /// </summary>
        /// <remarks>
        /// NMI and MI are raised by flipping bits 1 and 0, respectively.
        /// </remarks>
        public byte InterruptStatus;

        public const byte MaskMI = 1 << 0;
        public const byte MaskNMI = 1 << 1;


        /// <summary>
        /// The data currently traversing the data bus.
        /// </summary>
        public byte Data;

        /// <summary>
        /// Read data from a port.
        /// </summary>
        /// <param name="port">The address of the port.</param>
        /// <returns></returns>
        public Func<ushort, byte> ReadPort { get; set; }

        /// <summary>
        /// Write data to a port.
        /// </summary>
        /// <param name="port">The address of the port.</param>
        /// <param name="data">The data to write to the port.</param>
        /// <returns></returns>
        public Action<ushort, byte> WritePort { get; set; }

        public DataBus()
        {
            ReadPort = DefaultReadPort;
            WritePort = DefaultWritePort;
        }

        private byte DefaultReadPort(ushort port) => 0x00;
        private void DefaultWritePort(ushort port, byte data) { }
    }
}