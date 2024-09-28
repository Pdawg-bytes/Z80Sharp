using static Z80Sharp.Registers.ProcessorRegisters;

namespace Z80Sharp.Processor
{
    public partial class Z80
    {
        private void ExecuteBitInstruction()
        {
            _logger.Log(Enums.LogSeverity.Decode, $"BIT decoded: 0x{_currentInstruction:X2}");
            switch (Fetch())
            {
                default:
                    _logger.Log(Enums.LogSeverity.Fatal, $"Unrecognized BIT opcode: 0x{_currentInstruction:X2}");
                    Halted = true;
                    break;
            }
        }
    }
}