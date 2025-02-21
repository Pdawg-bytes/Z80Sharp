using System.Runtime.CompilerServices;
using Z80Sharp.Enums;

namespace Z80Sharp.Processor
{
    public partial class Z80
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
            Registers.IFF1 = false;
            Registers.IncrementRefresh();

            _clock.Add(11);
            PUSH_PC();
            Registers.PC = 0x0066;
        }

        private void HandleMI()
        {
            UnhaltIfHalted();
            if (!Registers.IFF1) { return; }
            
            Registers.IFF1 = false;
            Registers.IncrementRefresh();

            switch (Registers.InterruptMode)
            {
                case InterruptMode.IM0:
                    _clock.Add(11);
                    _lastInstruction = _pendingInstruction;
                    _currentInstruction = _dataBus.Data;
                    _pendingInstruction.Opcode1 = _currentInstruction;
                    switch (_currentInstruction)
                    {
                        case 0xDD: ExecuteIndexRInstruction(ref Registers.IX, ref Registers.IXhi, ref Registers.IXlo); break;
                        case 0xFD: ExecuteIndexRInstruction(ref Registers.IY, ref Registers.IYhi, ref Registers.IYlo); break;
                        case 0xED: ExecuteMiscInstruction(); break;
                        case 0xCB: ExecuteBitInstruction(); break;
                        default: ExecuteMainInstruction(); break;
                    }
                    break;
                case InterruptMode.IM1:
                    _clock.Add(13);
                    PUSH_PC();
                    Registers.MEMPTR = Registers.PC = 0x0038;
                    break;
                case InterruptMode.IM2:
                    PUSH_PC();
                    byte interruptVector = _dataBus.Data;
                    ushort vectorAddress = (ushort)((Registers.I << 8) | interruptVector);
                    _clock.Add(19);
                    Registers.MEMPTR = Registers.PC = _memory.ReadWord(vectorAddress);
                    break;
            }
        }
    }
}