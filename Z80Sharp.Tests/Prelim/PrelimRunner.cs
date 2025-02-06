using System.Diagnostics;
using Z80Sharp.Interfaces;
using Z80Sharp.Logging;
using Z80Sharp.Data;
using Z80Sharp.Processor;

namespace Z80Sharp.Tests.Prelim
{
    internal class PrelimRunner
    {
        readonly Z80 z80;
        readonly IZ80Logger logger = new Logger(useColors: false);
        readonly Memory memory = new Memory(65536);

        internal PrelimRunner()
        {
            var dataBus = new DataBus
            {
                ReadPort = (port) => 0x00,
                WritePort = (port, data) => Console.Write((char)data)
            };

            z80 = new(memory, dataBus, logger, 0);
            z80.Reset();
        }

        internal void RunPrelim()
        {
            memory.Write(0x0000, 0x76); // halt, prelim jumps to 0x0000 after test completion.
            memory.WriteBytes(0x0005, CPM_IORoutine);

            byte[] program = File.ReadAllBytes(@"../../../Prelim/ROMs/prelim.com");
            memory.WriteBytes(0x0100, program);

            z80.Registers.PC = 0x100;

            Stopwatch testTime = Stopwatch.StartNew();
            z80.RunUntilHalt();
            testTime.Stop();
            Console.WriteLine($"\nPrelim completed in {(testTime.ElapsedMilliseconds / 1000f):n2} seconds, {z80.CyclesExecuted:n0} cycles.");
        }

        /// <summary>
        /// The print routine in CP/M
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
}