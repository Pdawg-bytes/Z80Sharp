using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Z80Sharp.Enums;
using Z80Sharp.Interfaces;
using static Z80Sharp.Registers.ProcessorRegisters;

namespace Z80Sharp.Processor
{
    public unsafe partial class Z80
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void HandleInterrupts()
        {
            byte status = _dataBus.InterruptStatus;
            if ((status & 0x2) > 0) HandleNMI();
            if ((status & 0x1) > 0) HandleMI();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void HandleNMI()
        {
            UnhaltIfHalted();

            PUSH_PC();
            Registers.IFF1 = Registers.IFF2 = false;
            Registers.PC = 0x0066;
            LogInterrupt("NMI");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void HandleMI()
        {
            UnhaltIfHalted();
            if (!Registers.IFF1) { return; }
            
            Registers.IFF1 = Registers.IFF2 = false;
            switch (Registers.InterruptMode)
            {
                case InterruptMode.IM0:
                    {
                        RST_HH_SILENT((byte)(_dataBus.Data & 0x38));
                        LogInterrupt("MI Mode 0");
                        break;
                    }
                case InterruptMode.IM1:
                    PUSH_PC();
                    Registers.PC = 0x0038;
                    LogInterrupt("MI Mode 1");
                    break;
                case InterruptMode.IM2:
                    PUSH_PC();
                    byte interruptVector = _dataBus.Data;
                    ushort vectorAddress = (ushort)((Registers.I << 8) + interruptVector);
                    Registers.PC = _memory.ReadWord(vectorAddress);
                    LogInterrupt("MI Mode 2");
                    break;
            }
        }
    }
}