using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Core.Memory {
    public sealed class CharacterTable {
        private static readonly byte[] _additionCounts = new byte[] { 7, 5, 0, 4, 6, 5, 5, 3, 0 };

        private readonly int _baseAddress;

        public uint Expirience { get { return Emulator.DirectAccess.ReadUInt(_baseAddress); } set { Emulator.DirectAccess.WriteUInt(_baseAddress, value); } }
        public ushort HP { get { return Emulator.DirectAccess.ReadUShort(_baseAddress + 0x8); } set { Emulator.DirectAccess.WriteUShort(_baseAddress + 0x8, value); } }
        public ushort MP { get { return Emulator.DirectAccess.ReadUShort(_baseAddress + 0xA); } set { Emulator.DirectAccess.WriteUShort(_baseAddress + 0xA, value); } }
        public ushort SP { get { return Emulator.DirectAccess.ReadUShort(_baseAddress + 0xC); } set { Emulator.DirectAccess.WriteUShort(_baseAddress + 0xC, value); } }
        public ushort TotalSP { get { return Emulator.DirectAccess.ReadUShort(_baseAddress + 0xE); } set { Emulator.DirectAccess.WriteUShort(_baseAddress + 0xE, value); } }
        public byte Status { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x10); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x10, value); } }
        public byte Level { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x12); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x12, value); } }
        public byte DragoonLevel { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x13); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x13, value); } }
        public byte Weapon { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x14); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x14, value); } }
        public byte Armor { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x15); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x15, value); } }
        public byte Helmet { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x16); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x16, value); } }
        public byte Shoes { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x17); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x17, value); } }
        public byte Accessory { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x18); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x18, value); } }
        public byte ChosenAddition { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x19); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x19, value); } }
        public Collections.IAddress<byte> AdditionLevel { get; private set; }
        public Collections.IAddress<byte> AdditionCount { get; private set; }

        internal CharacterTable(int baseAddress, int character) {
            _baseAddress = baseAddress + (character * 0x2C);
            AdditionLevel = Collections.Factory.Create<byte>(_baseAddress + 0x1A, 1, _additionCounts[character]);
            AdditionCount = Collections.Factory.Create<byte>(_baseAddress + 0x22, 1, _additionCounts[character]);
        }
    }
}
