using System.Runtime.CompilerServices;

namespace Z80Sharp.Processor
{
    public unsafe partial class Z80
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ADD_HL_RR(ref ushort operatingRegister) => Registers.HL = Add16(Registers.HL, operatingRegister);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ADD_IR_RR(ref ushort mode, ref ushort operatingRegister) => mode = Add16(mode, operatingRegister);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ADC_HL_RR(ref ushort operatingRegister) => AddSub16CarryHL(operatingRegister, false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SBC_HL_RR(ref ushort operatingRegister) => AddSub16CarryHL(operatingRegister, true);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void INC_RR(ref ushort operatingRegister) => operatingRegister++;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DEC_RR(ref ushort operatingRegister) => operatingRegister = (ushort)(operatingRegister - 1);
    }
}