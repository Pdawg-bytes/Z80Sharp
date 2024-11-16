using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z80Sharp.Interfaces;
using Z80Sharp.Memory;
using Z80Sharp.Processor;

namespace Z80Sharp.TestProgram
{
    public static partial class Program
    {
        /// <summary>
        /// The I/O routine in CPM that Zexdoc/Zexall makes use of.
        /// </summary>
        private static readonly byte[] CPM_IORoutine = new byte[]
        {
            0xF5, // PUSH AF
            0xD5, // PUSH DE
            0x79, // LD A, C
            0xFE, 0x02, // CP 2
            0x28, 0x07, // JR Z, 7
            0xFE, 0x09, // CP 9
            0x28, 0x08, // JR Z, 8
            0xD1, // POP DE
            0xF1, // POP AF
            0xC9, // RET
            0x7B, // LD A, E
            0xD3, 0x00, // OUT (0), A
            0x18, 0xF8, // JR -8
            0x1A, // LD A, (DE)
            0xFE, 0x24, // CP 36
            0x28, 0xF3, // JR Z, -13
            0xD3, 0x00, // OUT (0), A
            0x13, // INC DE
            0x18, 0xF6  // JR -10
        };

        /// <summary>
        /// Runs a simple CP/M binary that depends on the standard I/O routine.
        /// </summary>
        /// <param name="filename"></param>
        internal static void RunCPMBinary(string filename)
        {
            // Copy I/O routine & program into RAM at expected locations
            Array.Copy(CPM_IORoutine, 0, mainMemory._memory, 0x5, CPM_IORoutine.Length);

            byte[] program = File.ReadAllBytes(filename);
            Array.Copy(program, 0, mainMemory._memory, 0x100, program.Length);

            z80.Registers.PC = 0x100;
            z80.Run();
        }
    }

    internal class CPMBus : IDataBus
    {
        public bool MI { get; set; } = false;
        public bool NMI { get; set; } = false;
        public byte Data { get; set; }

        public byte ReadPort(ushort port)
        {
            //Console.Write($"IN 0x{port:X4}");
            return 0;
        }
        public void WritePort(ushort port, byte data)
        {
            Console.Write((char)data);
        }
    }
}