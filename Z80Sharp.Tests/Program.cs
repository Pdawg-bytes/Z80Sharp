﻿using Z80Sharp.Tests.Zex;
using Z80Sharp.Tests.FUSE;
using Z80Sharp.Tests.Prelim;
using Z80Sharp.Tests.Z80Test;
using Z80Sharp.Processor;
using Z80Sharp.Logging;

namespace Z80Sharp.Tests
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ZexRunner zexRunner = new();
            PrelimRunner prelimRunner = new();
            FUSERunner fuseRunner = new();
            Z80TestRunner z80TestRunner = new();

            Console.WriteLine("Enter the test # you'd like to execute:" +
                "\n    1. Zexdoc\n    2. Zexall\n    3. Prelim\n    4. FUSE" +
                "\n    5. Z80Test - documented\n    6. Z80Test - full\n    " +
                "7. Z80Test - MEMPTR\n    8. Z80Test - CCF" +
                "\n    9. Custom file");

            int test = Console.ReadKey().KeyChar - '0';
            Console.WriteLine();

            switch (test)
            {
                case 1:
                    zexRunner.RunZex(ZexType.Zexdoc);
                    break;
                case 2:
                    zexRunner.RunZex(ZexType.Zexall);
                    break;
                case 3:
                    prelimRunner.RunPrelim();
                    break;
                case 4:
                    fuseRunner.RunFUSETests();
                    break;
                case 5:
                    z80TestRunner.RunZ80Test(Z80TestType.Documented);
                    break;
                case 6:
                    z80TestRunner.RunZ80Test(Z80TestType.Full);
                    break;
                case 7:
                    z80TestRunner.RunZ80Test(Z80TestType.MEMPTR);
                    break;
                case 8:
                    z80TestRunner.RunZ80Test(Z80TestType.CCF);
                    break;

                case 9:
                    Console.Write("Enter a filename: ");
                    string filename = Console.ReadLine();
                    Data.Memory ram = new(65536);
                    byte[] program = File.ReadAllBytes(filename);
                    if (program.Length > 65536) { Console.WriteLine("\nFile too big!"); break; }
                    ram.WriteBytes(0, program);
                    Z80 z80 = new(ram, new Data.DataBus(), new Logger(false), 0);
                    z80.RunUntilHalt();
                    break;

                default:
                    Console.WriteLine("Invalid test code, please restart.");
                    return;
            }
        }
    }
}
