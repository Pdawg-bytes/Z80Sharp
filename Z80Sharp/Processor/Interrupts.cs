﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z80Sharp.Enums;
using static Z80Sharp.Registers.ProcessorRegisters;

namespace Z80Sharp.Processor
{
    public partial class Z80
    {
        private void HandleInterrupts()
        {
            if (_dataBus.NMI) HandleNMI();
            if (_dataBus.MI) HandleMI();
        }

        private void HandleNMI()
        {
            UnhaltIfHalted();

            PUSH_PC();
            Registers.IFF1 = Registers.IFF2 = false;
            Registers.PC = 0x0066;
            LogInterrupt("NMI");
        }

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
                    ushort vectorAddress = (ushort)((Registers.RegisterSet[I] << 8) + interruptVector);
                    Registers.PC = _memory.Read(vectorAddress);
                    LogInterrupt("MI Mode 2");
                    break;
            }
        }
    }
}