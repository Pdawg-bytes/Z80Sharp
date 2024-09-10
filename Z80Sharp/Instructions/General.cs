using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Z80Sharp.Registers.ProcessorRegisters;

namespace Z80Sharp.Processor
{
    public partial class Z80
    {
        /// <summary>
        /// No operation
        /// </summary>
        private void NOP()
        {
            LogInstructionExec("0x00: NOP");
        }

        /// <summary>
        /// Interrupt disable
        /// </summary>
        private void DI()
        {
            Registers.IFF1 = Registers.IFF2 = false;
            LogInstructionExec("0xF3: DI");
        }
        /// <summary>
        /// Interrupt enable
        /// </summary>
        private void EI()
        {
            Registers.IFF1 = Registers.IFF2 = true;
            LogInstructionExec("0xFB: EI");
        }
    }
}
