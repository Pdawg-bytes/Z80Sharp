using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z80Sharp.Interfaces
{
    public interface IMemory
    {
        byte Read(ushort address);
        void Write(ushort address, byte value);

        ushort ReadWord(ushort address);
        void WriteWord(ushort address, ushort value);

        ushort Length { get; }
    }
}
