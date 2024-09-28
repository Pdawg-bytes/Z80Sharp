using static Z80Sharp.Registers.ProcessorRegisters;

namespace Z80Sharp.Processor
{
    public partial class Z80
    {
        private void ExecuteIndexXInstruction()
        {
            _logger.Log(Enums.LogSeverity.Decode, $"INXR decoded: 0x{_currentInstruction:X2}");
            switch (Fetch())
            {
                default:
                    _logger.Log(Enums.LogSeverity.Fatal, $"Unrecognized INXR opcode: 0x{_currentInstruction:X2}");
                    Halted = true;
                    break;
            }
        }

        private void ExecuteIndexYInstruction()
        {
            _logger.Log(Enums.LogSeverity.Decode, $"INYR decoded: 0x{_currentInstruction:X2}");
            switch (Fetch())
            {
                default:
                    _logger.Log(Enums.LogSeverity.Fatal, $"Unrecognized INYR opcode: 0x{_currentInstruction:X2}");
                    Halted = true;
                    break;
            }
        }
    }
}
