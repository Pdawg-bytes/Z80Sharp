using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Z80Sharp.Registers.ProcessorRegisters;

namespace Z80Sharp.Processor
{
    // Z80 Instruction Tables
    public partial class Z80
    {
        /// <summary>
        /// The main instruction table, containing all of the regular instructions.
        /// </summary>
        private readonly Action[] instructionTable = new Action[256];

        /// <summary>
        /// Index X register instruction table.
        /// </summary>
        private readonly Action[] DDInstructionTable = new Action[256];

        /// <summary>
        /// Index Y register instruction table.
        /// </summary>
        private readonly Action[] FDInstructionTable = new Action[256];

        /// <summary>
        /// Misc. instruction table.
        /// </summary>
        private readonly Action[] EDInstructionTable = new Action[256];

        /// <summary>
        /// Bit instruction table.
        /// </summary>
        private readonly Action[] CBInstructionTable = new Action[256];


        /// <summary>
        /// Initialize instruction tables.
        /// </summary>
        private void InitializeInstructionHandlers()
        {
            instructionTable[0x00] = NOP;

            instructionTable[0x01] = () => LD_RR_NN(B);
            instructionTable[0x11] = () => LD_RR_NN(D);
            instructionTable[0x21] = () => LD_RR_NN(H);
            instructionTable[0x31] = LD_SP_NN;

            instructionTable[0x06] = () => LD_R_N(B);
            instructionTable[0x16] = () => LD_R_N(D);
            instructionTable[0x26] = () => LD_R_N(H);
            instructionTable[0x0E] = () => LD_R_N(C);
            instructionTable[0x1E] = () => LD_R_N(E);
            instructionTable[0x2E] = () => LD_R_N(L);
            instructionTable[0x3E] = () => LD_R_N(A);

            instructionTable[0x36] = LD_HLMEM_N;
            instructionTable[0x46] = LD_B_HLMEM;
        }
    }
}
