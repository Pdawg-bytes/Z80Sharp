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
using System.Diagnostics.CodeAnalysis;

namespace Z80Sharp.Processor
{
    public unsafe partial class Z80
    {
        // Load immediate values into registers
        private void LD_RR_NN(ref ushort operatingRegister)
        {
            operatingRegister = FetchImmediateWord();
            //LogInstructionExec($"0x{_currentInstruction:X2}: LD RR, NN");
        }

        // Load from/to memory with register pairs
        private void LD_RR_NNMEM(ref ushort operatingRegister)
        {
            operatingRegister = _memory.ReadWord(FetchImmediateWord());
            //LogInstructionExec($"0x{_currentInstruction:X2}: LD RR, (NN)");
        }
        private void LD_NNMEM_RR(ref ushort operatingRegister)
        {
            _memory.WriteWord(FetchImmediateWord(), operatingRegister);
            //LogInstructionExec($"0x{_currentInstruction:X2}: LD (NN), RR");
        }

        // Load between registers
        private void LD_R_R(ref byte dest, ref byte source)
        {
            dest = source;
            //LogInstructionExec($"0x{_currentInstruction:X2}: LD R, R");
        }
        private void LD_R_N(ref byte operatingRegister)
        {
            operatingRegister = Fetch();
            //LogInstructionExec($"0x{_currentInstruction:X2}: LD R, N");
        }
        private void LD_RR_RR(ref ushort dest, ref ushort source)
        {
            dest = source;
            //LogInstructionExec($"0x{_currentInstruction:X2}: LD RR, RR");
        }

        // Load from memory to register or register to memory
        private void LD_R_RRMEM(ref byte dest, ref ushort source)
        {
            dest = _memory.Read(source);
            //LogInstructionExec($"0x{_currentInstruction:X2}: LD R, (RR)");
        }
        private void LD_R_IRDMEM(ref byte dest, ref ushort indexAddressingMode)
        {
            dest = _memory.Read((ushort)(indexAddressingMode + (sbyte)Fetch()));
            //LogInstructionExec($"0x{_currentInstruction:X2}: LD R, (IR + d)");
        }
        private void LD_RRMEM_R(ref ushort dest, ref byte source)
        {
            _memory.Write(dest, source);
            //LogInstructionExec($"0x{_currentInstruction:X2}: LD (RR), R");
        }
        private void LD_IRDMEM_R(ref ushort indexAddressingMode, ref byte source)
        {
            _memory.Write((ushort)(indexAddressingMode + (sbyte)Fetch()), source);
            //LogInstructionExec($"0x{_currentInstruction:X2}: LD (IR + d), R");
        }

        // Load regs into memory
        private void LD_NNMEM_R(ref byte operatingRegister)
        {
            _memory.Write(FetchImmediateWord(), operatingRegister);
            //LogInstructionExec($"0x32: LD (NN), R");
        }
        private void LD_R_NNMEM(ref byte operatingRegister)
        {
            operatingRegister = _memory.Read(FetchImmediateWord());
            //LogInstructionExec($"0x{_currentInstruction:X2}: LD R, (NN)");
        }
        private void LD_HLMEM_N()
        {
            _memory.Write(Registers.HL, Fetch());
            //LogInstructionExec($"0x36: LD (HL:0x{Registers.HL:X4}), N");
        }
        private void LD_IRDMEM_N(ref ushort indexAddressingMode)
        {
            _memory.Write((ushort)(indexAddressingMode + (sbyte)Fetch()), Fetch());
            //LogInstructionExec($"0x{_currentInstruction:X2}: LD (IR + d), N");
        }

        // Complex load operations
        private void LDI()
        {
            byte hlMem = _memory.Read(Registers.HL);
            _memory.Write(Registers.DE, hlMem);
            Registers.HL++;
            Registers.DE++;
            Registers.BC--;

            Registers.SetFlagConditionally(FlagType.PV, Registers.BC != 0); // (PV) (Reset in case of overflow; BC = 0)
            Registers.F &= (byte)~(FlagType.N | FlagType.H);    // (N, H) (Unconditionally reset)

            byte undoc = (byte)(Registers.A + hlMem);
            Registers.SetFlagConditionally(FlagType.X, (undoc & 0x20) > 0); // (X) (Undocumented flag)
            Registers.SetFlagConditionally(FlagType.Y, (undoc & 0x08) > 0); // (Y) (Undocumented flag)

            //LogInstructionExec("0xA0: LDI");
        }
        private void LDIR()
        {
            byte hlMem = _memory.Read(Registers.HL);
            _memory.Write(Registers.DE, hlMem);

            Registers.HL++;
            Registers.DE++;
            Registers.BC--;

            Registers.F &= (byte)~(FlagType.N | FlagType.H); // (N, H) (Unconditionally reset)
            Registers.SetFlagConditionally(FlagType.PV, Registers.BC != 0);

            byte undoc = (byte)(Registers.A + hlMem);
            Registers.SetFlagConditionally(FlagType.X, (undoc & 0x02) > 0);  // (X) (Undocumented flag)
            Registers.SetFlagConditionally(FlagType.Y, (undoc & 0x08) > 0);  // (Y) (Undocumented flag)

            if (Registers.BC != 0)
            {
                Registers.PC -= 2;
                //LogInstructionExec("0xB0: LDIR (no continue)");
                return;
            }
            //LogInstructionExec("0xB0: LDIR");
        }
        private void LDD()
        {
            byte hlMem = _memory.Read(Registers.HL);
            _memory.Write(Registers.DE, hlMem);
            Registers.HL--;
            Registers.DE--;
            Registers.BC--;

            Registers.SetFlagConditionally(FlagType.PV, Registers.BC != 0); // (PV) (Set if bit parity is even)
            Registers.F &= (byte)~(FlagType.N | FlagType.H);    // (N, H) (Unconditionally reset)

            byte undoc = (byte)(Registers.A + hlMem);
            Registers.SetFlagConditionally(FlagType.X, (undoc & 0x20) > 0);  // (X) (Undocumented flag)
            Registers.SetFlagConditionally(FlagType.Y, (undoc & 0x08) > 0);  // (Y) (Undocumented flag)

            //LogInstructionExec("0xA8: LDD");
        }
        private void LDDR()
        {
            byte hlMem = _memory.Read(Registers.HL);
            _memory.Write(Registers.DE, hlMem);

            Registers.DE--;
            Registers.HL--;
            Registers.BC--;

            Registers.F &= (byte)~(FlagType.N | FlagType.H | FlagType.PV); // (N, H, PV) (Unconditionally reset)

            byte undoc = (byte)(Registers.A + hlMem);
            Registers.SetFlagConditionally(FlagType.X, (undoc & 0x20) > 0);  // (X) (Undocumented flag)
            Registers.SetFlagConditionally(FlagType.Y, (undoc & 0x08) > 0);  // (Y) (Undocumented flag)

            if (Registers.BC != 0)
            {
                Registers.PC -= 2;
                //LogInstructionExec("0xB8: LDDR (no continue)");
                return;
            }
            //LogInstructionExec("0xB8: LDDR");
        }
    }
}