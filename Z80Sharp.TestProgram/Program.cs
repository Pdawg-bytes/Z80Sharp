using Z80Sharp.Memory;
using Z80Sharp.Logging;
using Z80Sharp.Processor;
using Z80Sharp.Constants;
using Z80Sharp.Registers;
using Z80Sharp.Interfaces;
using static Z80Sharp.Registers.ProcessorRegisters;

namespace Z80Sharp.TestProgram
{
    public static partial class Program
    {
        private static bool quitRequested = false;
        private static IZ80Logger logger = new Logger(useColors: false);
        internal static Z80 z80;
        internal static MainMemory mainMemory;

        public static void Main(string[] args)
        {
            IDataBus dataBus = new CPMBus();

            mainMemory = new MainMemory(65536);
            byte[] program = new byte[]
            {
                0x3E, 0xB0, 0x47, 0x4F, 0x57, 0x5F, 0x67, 0x6F,
                0x32, 0x00, 0x90, 0xCB, 0x17, 0x32, 0x03, 0x90,
                0xCB, 0x07, 0x32, 0x06, 0x90, 0xCB, 0x1F, 0x32,
                0x09, 0x90, 0xCB, 0x0F, 0x32, 0x0C, 0x90, 0xCB,
                0x3F, 0x32, 0x0F, 0x90, 0xCB, 0x2F, 0x32, 0x12,
                0x90, 0x00, 0xC3, 0x2A
            };

            //Array.Copy(program, 0, mainMemory._memory, 0x0, program.Length);

            z80 = new Z80(mainMemory, dataBus, logger, true);
            /*z80.Reset(new ProcessorRegisters
            {
                AF = 0x5555,
                BC = 0x5555,
                DE = 0x5555,
                HL = 0x5555,
                IX = 0x5555,
                IY = 0x5555,
                PC = 0x0000,
                SP = 0x5555
            });*/
            z80.Reset();
            Thread processorThread = new(() =>
            {
                RunCPMBinary("zexdoc.com");
                //z80.Run();
            });
            processorThread.Start();

            ReadConsoleInput();
        }

        private static ConsoleKeyInfo _key;
        private static void ReadConsoleInput()
        {
            Console.WriteLine($"{Colors.ORANGE}Welcome to the Z80Sharp interactive console! Press 'q' to quit...{Colors.ANSI_RESET}");
            while (!quitRequested)
            {
                _key = Console.ReadKey();
                switch (_key.KeyChar)
                {
                    case 'q':
                        quitRequested = true;
                        break;
                    case 'h':
                        Console.Write($"\n{Colors.CYAN}{Colors.ANSI_BOLD}==========Help=========={Colors.ANSI_RESET}\n{Colors.WHITE}{Colors.ANSI_ITALIC}Commands:{Colors.ANSI_RESET}\n{Colors.LIGHT_YELLOW}> h{Colors.ANSI_RESET} - prints this help text.\n{Colors.LIGHT_YELLOW}> d{Colors.ANSI_RESET} - dumps the processor state.\n{Colors.LIGHT_YELLOW}> q{Colors.ANSI_RESET} - quits the emulator and console.\n{Colors.LIGHT_YELLOW}> m{Colors.ANSI_RESET} - dumps the main memory.");
                        break;
                    case 'd':
                        Console.Write($"\n{Colors.ANSI_BOLD}{Colors.GREEN}=====Processor state====={Colors.ANSI_RESET}");
                        PrintProcessorState(z80.Registers);
                        break;
                    case 'm':
                        DumpMemory();
                        break;
                    default:
                        Console.Write($"\n{Colors.RED}Invalid command. Press 'h' for help.{Colors.ANSI_RESET}");
                        break;
                }
                Console.WriteLine();
            }
        }

        public static void PrintProcessorState(ProcessorRegisters registers)
        {
            z80.Halted = true;

            Console.WriteLine($"\n{Colors.LIGHT_BLUE}General-purpose registers{Colors.ANSI_RESET}");
            Console.WriteLine($"{Colors.LIGHT_YELLOW}B:{Colors.ANSI_RESET} 0x{registers.B.ToString("X").PadLeft(2, '0')}    {Colors.LIGHT_YELLOW}C:{Colors.ANSI_RESET} 0x{registers.C.ToString("X").PadLeft(2, '0')}");
            Console.WriteLine($"{Colors.LIGHT_YELLOW}D:{Colors.ANSI_RESET} 0x{registers.D.ToString("X").PadLeft(2, '0')}    {Colors.LIGHT_YELLOW}E:{Colors.ANSI_RESET} 0x{registers.E.ToString("X").PadLeft(2, '0')}");
            Console.WriteLine($"{Colors.LIGHT_YELLOW}H:{Colors.ANSI_RESET} 0x{registers.H.ToString("X").PadLeft(2, '0')}    {Colors.LIGHT_YELLOW}L:{Colors.ANSI_RESET} 0x{registers.L.ToString("X").PadLeft(2, '0')}");

            Console.WriteLine($"\n{Colors.LIGHT_BLUE}Data registers{Colors.ANSI_RESET}");
            Console.WriteLine($"{Colors.LIGHT_YELLOW}A:{Colors.ANSI_RESET} 0x{registers.A.ToString("X").PadLeft(2, '0')}\n");
            Console.WriteLine($"{Colors.LIGHT_YELLOW}F:{Colors.LIGHT_ORANGE} 0fSZYHXPNC{Colors.ANSI_RESET}");
            Console.WriteLine($"{Colors.LIGHT_YELLOW}F:{Colors.ANSI_RESET} 0b{Convert.ToString(registers.F, 2).PadLeft(8, '0')}");

            Console.WriteLine($"\n{Colors.LIGHT_BLUE}Index registers{Colors.ANSI_RESET}");
            Console.WriteLine($"{Colors.LIGHT_YELLOW}IX:{Colors.ANSI_RESET} 0x{registers.IX.ToString("X").PadLeft(4, '0')}  {Colors.LIGHT_YELLOW}IY:{Colors.ANSI_RESET} 0x{registers.IY.ToString("X").PadLeft(4, '0')}");

            Console.WriteLine($"\n{Colors.LIGHT_BLUE}Special registers{Colors.ANSI_RESET}");
            Console.WriteLine($"{Colors.LIGHT_YELLOW}IFF1:{Colors.ANSI_RESET} 0b{(registers.IFF1 ? "1" : "0")}  {Colors.LIGHT_YELLOW}IFF2:{Colors.ANSI_RESET} 0b{(registers.IFF2 ? "1" : "0")}");
            Console.WriteLine($"{Colors.LIGHT_YELLOW}I:{Colors.ANSI_RESET} 0x{registers.I.ToString("X").PadLeft(2, '0').PadRight(4)}  {Colors.LIGHT_YELLOW}R:{Colors.ANSI_RESET} 0x{registers.R.ToString("X").PadLeft(2, '0')}");

            Console.WriteLine($"\n{Colors.LIGHT_BLUE}Operating registers{Colors.ANSI_RESET}");
            Console.WriteLine($"{Colors.LIGHT_YELLOW}PC:{Colors.ANSI_RESET} 0x{registers.PC.ToString("X").PadLeft(4, '0')}");
            Console.WriteLine($"{Colors.LIGHT_YELLOW}SP:{Colors.ANSI_RESET} 0x{registers.SP.ToString("X").PadLeft(4, '0')}");
            Console.WriteLine();

            z80.Halted = false;
        }
        public static void DumpMemory()
        {
            z80.Halted = true;
            int bytesPerLine = 16;
            uint length = mainMemory.Length;
            int addrWidth = 4;

            Console.WriteLine();

            Console.WriteLine($"{Colors.LIGHT_BLUE}        00 01 02 03 04 05 06 07 08 09 0A 0B 0C 0D 0E 0F{Colors.ANSI_RESET}");

            for (int addr = 0; addr < length; addr += bytesPerLine)
            {
                Console.Write($"{Colors.LIGHT_BLUE}0x{addr.ToString($"X{addrWidth}")}: {Colors.ANSI_RESET}");

                for (int i = 0; i < bytesPerLine && addr + i < length; i++)
                {
                    byte b = mainMemory.Read((ushort)(addr + i));
                    Console.Write($"{b:X2} ");
                }

                Console.Write($" {Colors.LIGHT_PINK}|{Colors.ANSI_RESET} ");
                for (int i = 0; i < bytesPerLine && addr + i < length; i++)
                {
                    byte b = mainMemory.Read((ushort)(addr + i));
                    char c = (b >= 32 && b <= 126) ? (char)b : '.';
                    Console.Write(c);
                }

                Console.WriteLine();
            }
            Console.WriteLine($"{Colors.LIGHT_BLUE}        00 01 02 03 04 05 06 07 08 09 0A 0B 0C 0D 0E 0F{Colors.ANSI_RESET}");
            z80.Halted = false;
        }
        private class DataBus : IDataBus
        {
            public byte InterruptStatus { get; set; } = 0x00;
            public byte Data { get; set; }

            public byte ReadPort(ushort port)
            {
                //Console.Write($"IN 0x{port:X4}");
                return 0;
            }
            public void WritePort(ushort port, byte data)
            {
                //Console.Write($"OUT 0x{port:X4}, 0x{data:X2}");
            }
        }
    }
}