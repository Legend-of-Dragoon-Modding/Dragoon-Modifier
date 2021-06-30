using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Emulator.Memory {
    public class CharacterTable {
        private static readonly byte[] _additionCounts = new byte[] { 7, 5, 0, 4, 6, 5, 5, 3, 0 };

        private readonly int _baseAddress;

        private readonly IEmulator _emulator;

        public uint Expirience { get { return _emulator.ReadUInt(_baseAddress); } set { _emulator.WriteUInt(_baseAddress, value); } }
        public ushort HP { get { return _emulator.ReadUShort(_baseAddress + 0x8); } set { _emulator.WriteUShort(_baseAddress + 0x8, value); } }
        public ushort MP { get { return _emulator.ReadUShort(_baseAddress + 0xA); } set { _emulator.WriteUShort(_baseAddress + 0xA, value); } }
        public ushort SP { get { return _emulator.ReadUShort(_baseAddress + 0xC); } set { _emulator.WriteUShort(_baseAddress + 0xC, value); } }
        public ushort TotalSP { get { return _emulator.ReadUShort(_baseAddress + 0xE); } set { _emulator.WriteUShort(_baseAddress + 0xE, value); } }
        public byte Status { get { return _emulator.ReadByte(_baseAddress + 0x10); } set { _emulator.WriteByte(_baseAddress + 0x10, value); } }
        public byte Level { get { return _emulator.ReadByte(_baseAddress + 0x12); } set { _emulator.WriteByte(_baseAddress + 0x12, value); } }
        public byte DragoonLevel { get { return _emulator.ReadByte(_baseAddress + 0x13); } set { _emulator.WriteByte(_baseAddress + 0x13, value); } }
        public byte Weapon { get { return _emulator.ReadByte(_baseAddress + 0x14); } set { _emulator.WriteByte(_baseAddress + 0x14, value); } }
        public byte Armor { get { return _emulator.ReadByte(_baseAddress + 0x15); } set { _emulator.WriteByte(_baseAddress + 0x15, value); } }
        public byte Helmet { get { return _emulator.ReadByte(_baseAddress + 0x16); } set { _emulator.WriteByte(_baseAddress + 0x16, value); } }
        public byte Shoes { get { return _emulator.ReadByte(_baseAddress + 0x17); } set { _emulator.WriteByte(_baseAddress + 0x17, value); } }
        public byte Accessory { get { return _emulator.ReadByte(_baseAddress + 0x18); } set { _emulator.WriteByte(_baseAddress + 0x18, value); } }
        public byte ChosenAddition { get { return _emulator.ReadByte(_baseAddress + 0x19); } set { _emulator.WriteByte(_baseAddress + 0x19, value); } }
        public Collections.IAddress<byte> AdditionLevel { get; private set; }
        public Collections.IAddress<byte> AdditionCount { get; private set; }

        internal CharacterTable(IEmulator emulator, int baseAddress, int character) {
            _emulator = emulator;
            _baseAddress = baseAddress + (character * 0x2C);
            AdditionLevel = Factory.AddressCollection<byte>(emulator, _baseAddress + 0x1A, 1, _additionCounts[character]);
            AdditionCount = Factory.AddressCollection<byte>(emulator, _baseAddress + 0x22, 1, _additionCounts[character]);
        }
    }
}
