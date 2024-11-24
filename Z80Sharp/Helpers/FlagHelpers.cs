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
        private bool CheckParity(byte value)
        {
			return parityBit[value] == 1;
        }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void UpdateShiftRotateFlags(byte result, byte carry)
		{
            Registers.F = (byte)((byte)(FlagType.S | FlagType.Y | FlagType.X) & result);
            Registers.SetFlagConditionally(FlagType.PV, CheckParity(result));
            Registers.F |= (byte)((byte)(FlagType.C) & carry);
            Registers.SetFlagConditionally(FlagType.Z, result == 0);
        }
    }
}