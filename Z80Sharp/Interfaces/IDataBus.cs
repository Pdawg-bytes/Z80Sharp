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
        /// Maskable Interrupt raised
        /// </summary>
        bool MI { get; set; }

        /// <summary>
        /// Non-maskable Interrupt raised.
        /// </summary>
        bool NMI { get; set; }

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
