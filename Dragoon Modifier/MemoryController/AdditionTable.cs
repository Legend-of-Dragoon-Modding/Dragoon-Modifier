using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.MemoryController {
    public class AdditionTable {
        int _baseAddress;
        int _multiAddress;
        UShortCollection _sp;
        ByteCollection _multi;

        public byte Level { get { return Emulator.ReadByte(_baseAddress); } set { Emulator.WriteByte(_baseAddress, value); } }
        public byte Hits { get { return Emulator.ReadByte(_baseAddress + 0x1); } set { Emulator.WriteByte(_baseAddress + 0x1, value); } }
        public UShortCollection SP { get { return _sp; } }
        public ushort Damage { get { return Emulator.ReadUShort(_baseAddress + 0xC); } set { Emulator.WriteUShort(_baseAddress + 0xC, value); } }
        public ByteCollection DamageLevelMultiplier { get { return _multi; } }

        public AdditionTable(int baseAddress, int multiAddress, int addition) {
            _baseAddress = baseAddress + addition * 0xE;
            _multiAddress = multiAddress + addition * 0x18;
            _sp = new UShortCollection(_baseAddress, 2, 5);
            _multi = new ByteCollection(_multiAddress, 4, 5);
        }
    }
}
