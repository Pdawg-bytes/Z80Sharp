﻿using Z80Sharp.Enums;
using System.Runtime.CompilerServices;

using static Z80Sharp.Constants.QTables;

namespace Z80Sharp.Processor
{
    public partial class Z80
    {
        private ReadOnlySpan<byte> parityBit => [
	        1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1,
	        0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0, 
	        0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0, 
	        1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1, 
	        0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0, 
	        1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1, 
	        1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1, 
	        0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0, 
	        0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0, 
	        1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1, 
	        1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1, 
	        0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0, 
	        1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1, 
	        0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0, 
	        0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0, 
	        1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1 ];

        /// <summary>
        /// Determines whether a given <see cref="byte"/> has even parity.
        /// </summary>
        /// <param name="value">The byte value to check.</param>
        /// <returns><c>true</c> if the number of 1 bits in <paramref name="value"/> is even; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool CheckParity(byte value) => parityBit[value] == 1;


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void UpdateShiftRotateFlags(byte result, byte carry)
		{
            Registers.F = (byte)(0xA8 & result);                              // (S, Y, X) (Set based on respective bits of result)
            Registers.SetFlagConditionally(FlagType.PV, CheckParity(result)); // (P/V) (Set depending on bit parity)
            Registers.F |= (byte)((byte)FlagType.C & carry);				  // (C)   (Set if carry is true)
            Registers.SetFlagConditionally(FlagType.Z, result == 0);          // (Z)   (Set if result is 0)
            Registers.SetFlagConditionally(FlagType.X, (result & 0x08) != 0); // (X)   (copy of bit 3)
            Registers.SetFlagConditionally(FlagType.Y, (result & 0x20) != 0); // (Y)   (copy of bit 5)
        }


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateCommonInxrOtxrFlags(byte regB, byte hcf, byte p, byte nf)
        {
            Registers.F = 0;
            Registers.SetFlagConditionally(FlagType.S, (regB & 0x80) != 0);
            Registers.SetFlagConditionally(FlagType.Y, (Registers.PC & 0x2000) != 0);
            Registers.SetFlagConditionally(FlagType.X, (Registers.PC & 0x0800) != 0);
            Registers.SetFlagBits(nf);

            if (hcf != 0)
            {
                Registers.SetFlag(FlagType.C);
                bool isNFSet = nf != 0;
                Registers.SetFlagConditionally(FlagType.H, (regB & 0xF) == (isNFSet ? 0 : 0xF));
                Registers.SetFlagConditionally(FlagType.PV, CheckParity((byte)(p ^ ((regB + (isNFSet ? -1 : 1)) & 0x7))));
            }
            else
            {
                Registers.SetFlagConditionally(FlagType.PV, CheckParity((byte)(p ^ (regB & 0x7))));
            }
        }


        private bool GetQForLastInstruction()
        {
            byte prevOp = _lastInstruction.Opcode1;
            byte prevNext = _lastInstruction.Opcode2;

            if (prevOp == 0xDD || prevOp == 0xFD)
            {
                if (prevNext != 0xCB) return IDXRQTable[prevNext];
                else return IXRBQTable[_lastInstruction.Opcode4];
            }

            return prevOp switch
            {
                0xED => MiscQTable[prevNext],
                0xCB => BTOPQTable[prevNext],
                _ => MainQTable[prevOp]
            };
        }
    }
}