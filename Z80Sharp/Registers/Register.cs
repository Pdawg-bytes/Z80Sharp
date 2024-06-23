using Z80Sharp.Enums;

namespace Z80Sharp.Registers
{
    public record Register
    {
        public byte Value { get; set; }
        public string Name { get; set; }
        public RegisterSize Size { get; set; }
    }
}