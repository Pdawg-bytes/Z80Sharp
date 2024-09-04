using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            instructionTable[0x01] = LD_BC_NN;
            instructionTable[0x11] = LD_DE_NN;
            instructionTable[0x21] = LD_HL_NN;
            instructionTable[0x31] = LD_SP_NN;

            instructionTable[0x06] = LD_B_N;
            instructionTable[0x16] = LD_D_N;
            instructionTable[0x26] = LD_H_N;
            instructionTable[0x0E] = LD_C_N;
            instructionTable[0x1E] = LD_E_N;
            instructionTable[0x2E] = LD_L_N;
            instructionTable[0x3E] = LD_A_N;

            instructionTable[0x36] = LD_HLMEM_N;
            instructionTable[0x46] = LD_B_HLMEM;
        }
    }
}
