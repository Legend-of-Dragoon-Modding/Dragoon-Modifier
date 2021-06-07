using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Core.Memory {
    public class AdditionTable {
        private readonly int _baseAddress;
        private readonly int _multiAddress;

        public byte Level { get { return Emulator.ReadByte(_baseAddress); } set { Emulator.WriteByte(_baseAddress, value); } }
        public byte Hits { get { return Emulator.ReadByte(_baseAddress + 0x1); } set { Emulator.WriteByte(_baseAddress + 0x1, value); } }
        public Collections.UShort SP { get; private set; }
        public ushort Damage { get { return Emulator.ReadUShort(_baseAddress + 0xC); } set { Emulator.WriteUShort(_baseAddress + 0xC, value); } }
        public Collections.Byte DamageLevelMultiplier { get; private set; }

        internal AdditionTable(int baseAddress, int multiAddress, int addition) {
            _baseAddress = baseAddress + addition * 0xE;
            _multiAddress = multiAddress + addition * 0x18;
            SP = new Collections.UShort(_baseAddress, 2, 5);
            DamageLevelMultiplier = new Collections.Byte(_multiAddress, 4, 5);
        }
    }
}
