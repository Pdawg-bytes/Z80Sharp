using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using static Z80Sharp.Registers.ProcessorRegisters;
using Z80Sharp.Enums;
using Z80Sharp.Helpers;

namespace Z80Sharp.Processor
{
    public partial class Z80
    {
        // Load immediate values into registers
        private void LD_RR_NN(byte operatingRegister)
        {
            Registers.RegisterSet[operatingRegister + 1] = Fetch();
            Registers.RegisterSet[operatingRegister] = Fetch();
            LogInstructionExec($"0x{_currentInstruction:X2}: LD {Registers.RegisterName(operatingRegister, true)}, 0x{Registers.RegisterSet[operatingRegister]:X2}{Registers.RegisterSet[operatingRegister + 1]:X2}");
        }
        private void LD_SP_NN()
        {
            ushort word = FetchImmediateWord();
            Registers.SP = word;
            LogInstructionExec($"0x31: LD SP, 0x{word:X4}");
        }

        // Load from/to memory with register pairs
        private void LD_RR_NNMEM(byte operatingRegister)
        {
            ushort addr = FetchImmediateWord();
            Registers.RegisterSet[operatingRegister + 1] = _memory.Read(addr++);
            Registers.RegisterSet[operatingRegister] = _memory.Read(addr);
            LogInstructionExec($"0x{_currentInstruction}: LD {Registers.RegisterName(operatingRegister, true)}, (NN:0x{addr:X4}");
        }
        private void LD_NNMEM_RR(byte operatingRegister)
        {
            ushort addr = FetchImmediateWord();
            _memory.Write(addr++, Registers.RegisterSet[operatingRegister + 1]);
            _memory.Write(addr, Registers.RegisterSet[operatingRegister]);
            LogInstructionExec($"0x{_currentInstruction:X2}: LD (0x{--addr:X4}), {Registers.RegisterName(operatingRegister, true)}");
        }

        // Load between registers
        private void LD_R_R(byte dest, byte source)
        {
            Registers.RegisterSet[dest] = Registers.RegisterSet[source];
            LogInstructionExec($"0x{_currentInstruction:X2}: LD {Registers.RegisterName(dest)}, {Registers.RegisterName(source)}:0x{Registers.RegisterSet[source]:X2}");
        }
        private void LD_R_N(byte operatingRegister)
        {
            Registers.RegisterSet[operatingRegister] = Fetch();
            LogInstructionExec($"0x{_currentInstruction:X2}: LD {Registers.RegisterName(operatingRegister)}, 0x{Registers.RegisterSet[operatingRegister]:X2}");
        }
        private void LD_RR_RR(byte dest, byte source)
        {
            Registers.RegisterSet[dest + 1] = Registers.RegisterSet[source + 1];
            Registers.RegisterSet[dest] = Registers.RegisterSet[source];
            LogInstructionExec($"0x{_currentInstruction:X2}: LD {Registers.RegisterName(dest, true)}:{Registers.GetR16FromHighIndexer(dest)}, {Registers.RegisterName(source, true)}:{Registers.GetR16FromHighIndexer(source)}");
        }

        // Load from memory to register or register to memory
        private void LD_R_RRMEM(byte dest, byte source)
        {
            byte val = _memory.Read(Registers.GetR16FromHighIndexer(source));
            Registers.RegisterSet[dest] = val;
            LogInstructionExec($"0x{_currentInstruction:X2}: LD {Registers.RegisterName(dest)}, ({Registers.RegisterName(source, true)}:0x{Registers.HL:X4}):0x{val:X2}");
        }
        private void LD_RRMEM_R(byte dest, byte source)
        {
            _memory.Write(Registers.GetR16FromHighIndexer(dest), Registers.RegisterSet[source]);
            LogInstructionExec($"0x{_currentInstruction:X2}: LD ({Registers.RegisterName(dest, true)}:0x{Registers.GetR16FromHighIndexer(dest):X2}), {Registers.RegisterName(source)}:0x{Registers.RegisterSet[source]:X2}");
        }

        // Load regs into memory
        private void LD_NNMEM_R(byte operatingRegister)
        {
            ushort word = FetchImmediateWord();
            _memory.Write(word, Registers.RegisterSet[operatingRegister]);
            LogInstructionExec($"0x32: LD (NN:0x{word:X2}), {Registers.RegisterName(operatingRegister)}:0x{Registers.RegisterSet[operatingRegister]:X2}");
        }
        private void LD_R_NNMEM(byte operatingRegister)
        {
            ushort addr = FetchImmediateWord();
            Registers.RegisterSet[operatingRegister] = _memory.Read(addr);
            LogInstructionExec($"0x{_currentInstruction}: LD {Registers.RegisterName(operatingRegister)}, (NN:0x{addr:X4})");
        }
        private void LD_HLMEM_N()
        {
            byte n = Fetch();
            _memory.Write(Registers.HL, n);
            LogInstructionExec($"0x36: LD (HL:0x{Registers.HL:X4}), 0x{n:X2}");
        }

        // Complex load operations
        private void LDI()
        {
            byte hlMem = _memory.Read(Registers.HL);
            _memory.Write(Registers.DE, hlMem);
            Registers.HL++;
            Registers.DE++;
            Registers.BC--;

            Registers.SetFlagConditionally(FlagType.PV, Registers.BC != 0); // (PV) (Set if bit parity is even)
            Registers.RegisterSet[F] = (byte)~(FlagType.N | FlagType.H);    // (N, H) (Unconditionally reset)

            byte undoc = (byte)(Registers.RegisterSet[A] + hlMem);
            Registers.SetFlagConditionally(FlagType.X, (undoc & 0x20) > 0);  // (X) (Undocumented flag)
            Registers.SetFlagConditionally(FlagType.Y, (undoc & 0x08) > 0);  // (Y) (Undocumented flag)

            LogInstructionExec("0xA0: LDI");
        }
        private void LDIR()
        {
            byte hlMem = _memory.Read(Registers.HL);
            _memory.Write(Registers.DE, hlMem);

            Registers.HL++;
            Registers.DE++;
            Registers.BC--;

            Registers.RegisterSet[F] = (byte)~(FlagType.N | FlagType.H | FlagType.PV); // (N, H, PV) (Unconditionally reset)

            byte undoc = (byte)(Registers.RegisterSet[A] + hlMem);
            Registers.SetFlagConditionally(FlagType.X, (undoc & 0x20) > 0);  // (X) (Undocumented flag)
            Registers.SetFlagConditionally(FlagType.Y, (undoc & 0x08) > 0);  // (Y) (Undocumented flag)

            if (Registers.BC != 0)
            {
                Registers.PC -= 2;
                return;
            }
            LogInstructionExec("0xB0: LDIR");
        }
        private void LDD()
        {
            byte hlMem = _memory.Read(Registers.HL);
            _memory.Write(Registers.DE, hlMem);
            Registers.HL--;
            Registers.DE--;
            Registers.BC--;

            Registers.SetFlagConditionally(FlagType.PV, Registers.BC != 0); // (PV) (Set if bit parity is even)
            Registers.RegisterSet[F] = (byte)~(FlagType.N | FlagType.H);    // (N, H) (Unconditionally reset)

            byte undoc = (byte)(Registers.RegisterSet[A] + hlMem);
            Registers.SetFlagConditionally(FlagType.X, (undoc & 0x20) > 0);  // (X) (Undocumented flag)
            Registers.SetFlagConditionally(FlagType.Y, (undoc & 0x08) > 0);  // (Y) (Undocumented flag)

            LogInstructionExec("0xA8: LDD");
        }
        private void LDDR()
        {
            byte hlMem = _memory.Read(Registers.HL);
            _memory.Write(Registers.DE, hlMem);

            Registers.HL--;
            Registers.DE--;
            Registers.BC--;

            Registers.RegisterSet[F] = (byte)~(FlagType.N | FlagType.H | FlagType.PV); // (N, H, PV) (Unconditionally reset)

            byte undoc = (byte)(Registers.RegisterSet[A] + hlMem);
            Registers.SetFlagConditionally(FlagType.X, (undoc & 0x20) > 0);  // (X) (Undocumented flag)
            Registers.SetFlagConditionally(FlagType.Y, (undoc & 0x08) > 0);  // (Y) (Undocumented flag)

            if (Registers.BC != 0)
            {
                Registers.PC -= 2;
                return;
            }
            LogInstructionExec("0xB8: LDDR");
        }
    }
}