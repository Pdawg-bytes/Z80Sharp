using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z80Sharp.Interfaces
{
    public interface IDataBus
    {
        /// <summary>
        /// NMI and MI status.
        /// </summary>
        /// <remarks>
        /// NMI and MI are raised by flipping bits 0 and 1 in this byte.
        /// </remarks>
        byte InterruptStatus { get; set; }

        /// <summary>
        /// Read data from a port.
        /// </summary>
        /// <param name="port">The address of the port.</param>
        /// <returns></returns>
        byte ReadPort(ushort port);
        /// <summary>
        /// Write data to a port.
        /// </summary>
        /// <param name="port">The address of the port.</param>
        /// <param name="data">The data to write to the port.</param>
        /// <returns></returns>
        void WritePort(ushort port, byte data);

        /// <summary>
        /// The data currently traversing the data bus.
        /// </summary>
        byte Data { get; set; }
    }
}