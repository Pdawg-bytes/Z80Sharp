using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z80Sharp.Interfaces;
using Z80Sharp.Logging;
using Z80Sharp.Memory;
using Z80Sharp.Processor;

namespace Z80Sharp.Tests.Zex
{
    internal class ZexRunner
    {
        readonly Z80 z80;
        readonly IZ80Logger logger = new Logger(useColors: false);
        readonly MainMemory memory = new MainMemory(65536);

        internal ZexRunner() 
        {

        }

        internal void RunZex(ZexType type)
        {
            Array.Copy(CPM_IORoutine, 0, memory._memory, 0x5, CPM_IORoutine.Length);

            string fileName = type switch
            {
                ZexType.Zexall => "zexall.com",
                ZexType.Zexdoc => "zexdoc.com"
            };

            byte[] program = File.ReadAllBytes(fileName);
            Array.Copy(program, 0, memory._memory, 0x100, program.Length);

            z80.Registers.PC = 0x100;
            z80.RunUntil(46_734_978_649L);
        }

        /// <summary>
        /// The I/O routine in CPM that Zexdoc/Zexall makes use of.
        /// </summary>
        private static readonly byte[] CPM_IORoutine =
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
    }

    internal struct CPMBus : IDataBus
    {
        public CPMBus() { }

        public byte InterruptStatus { get; set; } = 0x00;
        public byte Data { get; set; }

        public byte ReadPort(ushort port) => 0;

        public void WritePort(ushort port, byte data)
        {
            Console.Write((char)data);
        }
    }

    internal enum ZexType
    {
        Zexdoc,
        Zexall
    }
}
