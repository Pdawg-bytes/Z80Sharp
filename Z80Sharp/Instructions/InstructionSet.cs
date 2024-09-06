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

            instructionTable[0x01] = () => LD_RR_NN(B); // LD BC, NN
            instructionTable[0x11] = () => LD_RR_NN(D); // LD DE, NN
            instructionTable[0x21] = () => LD_RR_NN(H); // LD HL, NN
            instructionTable[0x31] = LD_SP_NN; // LD SP, NN

            instructionTable[0x06] = () => LD_R_N(B); // LD B, N
            instructionTable[0x16] = () => LD_R_N(D); // LD D, N
            instructionTable[0x26] = () => LD_R_N(H); // LD H, N
            instructionTable[0x0E] = () => LD_R_N(C); // LD C, N
            instructionTable[0x1E] = () => LD_R_N(E); // LD E, N
            instructionTable[0x2E] = () => LD_R_N(L); // LD L, N
            instructionTable[0x3E] = () => LD_R_N(A); // LD A, N

            instructionTable[0x36] = LD_HLMEM_N; // LD (HL), N
            instructionTable[0x46] = LD_B_HLMEM; // LD B, (HL)

            instructionTable[0x02] = () => LD_RRMEM_A(B); // LD (BC), A
            instructionTable[0x12] = () => LD_RRMEM_A(D); // LD (DE), A
            instructionTable[0x77] = () => LD_RRMEM_A(H); // LD (HL), A

            instructionTable[0x32] = LD_NNMEM_A; // LD (NN), A
        }
    }
}
