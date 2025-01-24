using System.Runtime.CompilerServices;
using Z80Sharp.Enums;

namespace Z80Sharp.Processor
{
    public unsafe partial class Z80
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void HandleInterrupts()
        {
            byte status = _dataBus.InterruptStatus;
            if (status == 0) return;

            if ((status & 0x2) != 0) HandleNMI();
            if ((status & 0x1) != 0) HandleMI();
        }

        private void HandleNMI()
        {
            UnhaltIfHalted();

            PUSH_PC();
            Registers.IFF1 = Registers.IFF2 = false;
            Registers.PC = 0x0066;
            //LogInterrupt("NMI");
        }

        private void HandleMI()
        {
            UnhaltIfHalted();
            if (!Registers.IFF1) { return; }
            
            Registers.IFF1 = Registers.IFF2 = false;
            switch (Registers.InterruptMode)
            {
                case InterruptMode.IM0:
                    _currentInstruction = _dataBus.Data;
                    switch (_currentInstruction)
                    {
                        case 0xDD: ExecuteIndexRInstruction(ref Registers.IX, ref Registers.IXhi, ref Registers.IXlo); break;
                        case 0xFD: ExecuteIndexRInstruction(ref Registers.IY, ref Registers.IYhi, ref Registers.IYlo); break;
                        case 0xED: ExecuteMiscInstruction(); break;
                        case 0xCB: ExecuteBitInstruction(); break;
                        default: ExecuteMainInstruction(); break;
                    }
                    //LogInterrupt("MI Mode 0");
                    break;
                case InterruptMode.IM1:
                    PUSH_PC();
                    Registers.PC = 0x0038;
                    //LogInterrupt("MI Mode 1");
                    break;
                case InterruptMode.IM2:
                    PUSH_PC();
                    byte interruptVector = _dataBus.Data;
                    ushort vectorAddress = (ushort)((Registers.I << 8) | interruptVector);
                    Registers.PC = _memory.ReadWord(vectorAddress);
                    //LogInterrupt("MI Mode 2");
                    break;
            }
        }
    }
}