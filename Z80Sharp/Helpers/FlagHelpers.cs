using System.Runtime.CompilerServices;
using Z80Sharp.Enums;
using static Z80Sharp.Registers.ProcessorRegisters;

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
		/// Checks the parity of a <see cref="byte"/>.
		/// </summary>
		/// <param name="value">The value to check.</param>
		/// <returns>False if the amount of 1s is even, false otherwise.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool CheckParity(byte value) => parityBit[value] == 1;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void UpdateShiftRotateFlags(byte result, byte carry)
		{
            Registers.F = (byte)((byte)(FlagType.S | FlagType.Y | FlagType.X) & result); // (S, Y, X) (Set based on respective bits of result)
            Registers.SetFlagConditionally(FlagType.PV, CheckParity(result));			 // (P/V) (Set depending on bit parity)
            Registers.F |= (byte)((byte)FlagType.C & carry);							 // (C)   (Set if carry is true)
            Registers.SetFlagConditionally(FlagType.Z, result == 0);                     // (Z)   (Set if result is 0)
            Registers.SetFlagConditionally(FlagType.X, (result & 0x08) != 0);			 // (X)   (copy of bit 3)
            Registers.SetFlagConditionally(FlagType.Y, (result & 0x20) != 0);			 // (Y)   (copy of bit 5)
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
    }
}