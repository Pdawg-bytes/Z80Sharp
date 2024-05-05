using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z80Sharp.Enums;
using Z80Sharp.Registers;

namespace Z80Sharp.Interfaces
{
    /// <summary>
    /// Defines a common interface that supports multiple implementations of the Z80.
    /// </summary>
    /// <remarks>This is used for the <see cref="ProcessorMode.Production"/> Z80, as well as the <see cref="ProcessorMode.Debug"/> Z80.</remarks>
    public interface IProcessor
    {
        /// <summary>
        /// All registers of the Z80.
        /// </summary>
        public IRegisterSet Registers { get; set; }

        /// <summary>
        /// Decides whether the processor is halted or not.
        /// </summary>
        public bool Halted { get; set; }

        /// <summary>
        /// Starts execution.
        /// </summary>
        public void Run();

        /// <summary>
        /// Pauses exeuction.
        /// </summary>
        public void Stop();

        /// <summary>
        /// Resets the processor to its default state.
        /// </summary>
        public void Reset();
    }
}
