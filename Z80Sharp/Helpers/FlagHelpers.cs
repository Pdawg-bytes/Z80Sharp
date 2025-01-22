using System.Runtime.CompilerServices;
using Z80Sharp.Enums;
using static Z80Sharp.Registers.ProcessorRegisters;

namespace Z80Sharp.Processor
{
    public unsafe partial class Z80
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
		/// Checks the parity of a <see cref="ushort"/>.
		/// </summary>
		/// <param name="value">The value to check.</param>
		/// <returns>False if the amount of 1s is even, false otherwise.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool CheckParity(byte value) => parityBit[value] == 1;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void UpdateShiftRotateFlags(byte result, byte carry)
		{
            Registers.F = (byte)((byte)(FlagType.S | FlagType.Y | FlagType.X) & result); // (S, X, Y) (Set based on respective bits of result)
            Registers.SetFlagConditionally(FlagType.PV, CheckParity(result));			 // (P/V) (Set depending on bit parity)
            Registers.F |= (byte)((byte)FlagType.C & carry);							 // (C)   (Set if carry is true)
            Registers.SetFlagConditionally(FlagType.Z, result == 0);					 // (Z)   (Set if result is 0)
        }
    }
}