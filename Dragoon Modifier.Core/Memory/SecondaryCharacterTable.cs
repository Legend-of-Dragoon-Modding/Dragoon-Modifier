using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Core.Memory {
    public class SecondaryCharacterTable {
        private static readonly byte[] _additionCounts = new byte[] { 7, 5, 0, 4, 6, 5, 5, 3, 0 };

        private readonly int _baseAddress;

        public uint Expirience { get { return Emulator.ReadUInt(_baseAddress); } set { Emulator.WriteUInt(_baseAddress, value); } }
        public ushort HP { get { return Emulator.ReadUShort(_baseAddress + 0x4); } set { Emulator.WriteUShort(_baseAddress + 0x4, value); } }
        public ushort MP { get { return Emulator.ReadUShort(_baseAddress + 0x6); } set { Emulator.WriteUShort(_baseAddress + 0x6, value); } }
        public ushort SP { get { return Emulator.ReadUShort(_baseAddress + 0x8); } set { Emulator.WriteUShort(_baseAddress + 0x8, value); } }
        public byte Level { get { return Emulator.ReadByte(_baseAddress + 0xE); } set { Emulator.WriteByte(_baseAddress + 0xE, value); } }
        public byte DragoonLevel { get { return Emulator.ReadByte(_baseAddress + 0xF); } set { Emulator.WriteByte(_baseAddress + 0xF, value); } }
        public Collections.Byte AdditionLevel { get; private set; }
        public Collections.Byte AdditionCount { get; private set; }

        internal SecondaryCharacterTable(int baseAddress, int character) {
            _baseAddress = baseAddress + (character * 0xA0);
            AdditionLevel = new Collections.Byte(_baseAddress + 0x36, 1, _additionCounts[character]);
            AdditionCount = new Collections.Byte(_baseAddress + 0x3E, 1, _additionCounts[character]);
        }
    }
}
