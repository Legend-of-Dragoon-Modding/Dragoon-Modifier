using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.MemoryController {
    public class CharacterTable {
        long _baseAddress;

        public ulong Expirience { get { return Emulator.ReadULong(_baseAddress); } set { Emulator.WriteULong(_baseAddress, value); } }
        public ushort HP { get { return Emulator.ReadUShort(_baseAddress + 0x8); } set { Emulator.WriteUShort(_baseAddress + 0x8, value); } }
        public ushort MP { get { return Emulator.ReadUShort(_baseAddress + 0xA); } set { Emulator.WriteUShort(_baseAddress + 0xA, value); } }
        public ushort SP { get { return Emulator.ReadUShort(_baseAddress + 0xC); } set { Emulator.WriteUShort(_baseAddress + 0xC, value); } }
        public ushort TotalSP { get { return Emulator.ReadUShort(_baseAddress + 0xE); } set { Emulator.WriteUShort(_baseAddress + 0xE, value); } }
        public byte Status { get { return Emulator.ReadByte(_baseAddress + 0x10); } set { Emulator.WriteByte(_baseAddress + 0x10, value); } }
        public byte Level { get { return Emulator.ReadByte(_baseAddress + 0x12); } set { Emulator.WriteByte(_baseAddress + 0x12, value); } }
        public byte DragoonLevel { get { return Emulator.ReadByte(_baseAddress + 0x13); } set { Emulator.WriteByte(_baseAddress + 0x13, value); } }
        public byte Weapon { get { return Emulator.ReadByte(_baseAddress + 0x14); } set { Emulator.WriteByte(_baseAddress + 0x14, value); } }
        public byte Armor { get { return Emulator.ReadByte(_baseAddress + 0x15); } set { Emulator.WriteByte(_baseAddress + 0x15, value); } }
        public byte Helmet { get { return Emulator.ReadByte(_baseAddress + 0x16); } set { Emulator.WriteByte(_baseAddress + 0x16, value); } }
        public byte Shoes { get { return Emulator.ReadByte(_baseAddress + 0x17); } set { Emulator.WriteByte(_baseAddress + 0x17, value); } }
        public byte Accessory { get { return Emulator.ReadByte(_baseAddress + 0x18); } set { Emulator.WriteByte(_baseAddress + 0x18, value); } }
        public byte ChosenAddition { get { return Emulator.ReadByte(_baseAddress + 0x19); } set { Emulator.WriteByte(_baseAddress + 0x19, value); } }
        public ushort Test { get { return Emulator.ReadUShort(_baseAddress); } set { Emulator.WriteUShort(_baseAddress, value); } }


        public CharacterTable(long baseAddress) {
            _baseAddress = baseAddress;
        }

    }
}
