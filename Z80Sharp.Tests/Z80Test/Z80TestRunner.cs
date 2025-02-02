using Z80Sharp.Data;
using Z80Sharp.Logging;
using Z80Sharp.Processor;
using System.Diagnostics;
using Z80Sharp.Interfaces;

namespace Z80Sharp.Tests.Z80Test
{
    internal class Z80TestRunner
    {
        readonly Z80 z80;
        readonly IZ80Logger logger = new Logger(useColors: false);
        readonly MainMemory memory = new MainMemory(65536);

        internal Z80TestRunner()
        {
            var dataBus = new DataBus()
            {
                ReadPort = (port) => 0xBF
            };

            z80 = new(memory, dataBus, logger, 0);
            z80.Reset();
        }

        internal void RunZ80Test(Z80TestType type)
        {
            string fileName = type switch
            {
                Z80TestType.Full => "../../../Z80Test/ROMs/z80full.tap",
                Z80TestType.MEMPTR => "../../../Z80Test/ROMs/z80memptr.tap",
                Z80TestType.Documented => "../../../Z80Test/ROMs/z80doc.tap",
                Z80TestType.CCF => "../../../Z80Test/ROMs/z80ccf.tap",
                _ => ""
            };

            byte[] testProgram = File.ReadAllBytes(fileName);

            int skip = 0x5B;
            int loadAddr = 0x8000;

            for (int i = skip; i < testProgram.Length; i++)
                memory.Write((ushort)(loadAddr + (i - skip)), testProgram[i]);

            // Patch to RET
            memory.Write(0x10, 0xC9);
            memory.Write(0x1601, 0xC9);

            z80.Registers.PC = (ushort)loadAddr;

            string msg = "";
            Stopwatch testTime = Stopwatch.StartNew();
            while (true)
            {
                z80.Step();
                if (z80.Halted) break;

                if (z80.Registers.PC == 0x0000) break;

                if (z80.Registers.PC == 0x0010)
                {
                    char ch = (char)z80.Registers.A;
                    if (ch == '\r') ch = '\n';
                    if (ch == (char)23 || ch == (char)26) ch = ' ';

                    Console.Write(ch);
                    msg += ch;
                }
            }
            testTime.Stop();

            if (msg.Contains("CPU TESTS OK") || msg.Contains("all tests passed"))
                Console.WriteLine($"\nZ80Test completed in {(testTime.ElapsedMilliseconds / 1000f):n2} seconds, {z80.CyclesExecuted:n0} cycles.");
            else
                Console.WriteLine($"\nZ80Test failed! {z80.CyclesExecuted:n0} cycles executed.");
        }
    }

    internal enum Z80TestType
    {
        Full,
        MEMPTR,
        Documented,
        CCF
    }
}