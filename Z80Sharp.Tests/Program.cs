using Z80Sharp.Interfaces;
using Z80Sharp.Logging;
using Z80Sharp.Memory;
using Z80Sharp.Processor;
using Z80Sharp.Tests.FUSE;

namespace Z80Sharp.Tests
{
    internal class Program
    {
        static Z80 z80;
        static IZ80Logger logger = new Logger(useColors: false);
        static MainMemory memory = new MainMemory(65536);

        static void Main(string[] args)
        {
            z80 = new(memory, new DefaultBus(), logger, 2.5);
            Runner runner = new(z80, memory, false);
        }
    }

    internal struct DefaultBus : IDataBus
    {
        public DefaultBus() { }

        public byte InterruptStatus { get; set; } = 0x00;

        public byte Data { get; set; }

        public byte ReadPort(ushort port)
        {
            return 0;
        }
        public void WritePort(ushort port, byte data)
        {
            //Console.Write((char)data);
        }
    }
}
