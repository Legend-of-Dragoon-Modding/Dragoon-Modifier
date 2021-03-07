using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.MemoryController {
    public class ItemTableEntry {
        long _baseAddress;

        public byte Target { get { return Emulator.ReadByte(_baseAddress); } set { Emulator.WriteByte(_baseAddress, value); } }
        public byte Element { get { return Emulator.ReadByte(_baseAddress + 0x1); } set { Emulator.WriteByte(_baseAddress + 0x1, value); } }
        public byte Damage { get { return Emulator.ReadByte(_baseAddress + 0x2); } set { Emulator.WriteByte(_baseAddress + 0x2, value); } }
        public byte Special1 { get { return Emulator.ReadByte(_baseAddress + 0x3); } set { Emulator.WriteByte(_baseAddress + 0x3, value); } }
        public byte Special2 { get { return Emulator.ReadByte(_baseAddress + 0x4); } set { Emulator.WriteByte(_baseAddress + 0x4, value); } }
        public byte SpecialAmmount { get { return Emulator.ReadByte(_baseAddress + 0x6); } set { Emulator.WriteByte(_baseAddress + 0x6, value); } }
        public byte Icon { get { return Emulator.ReadByte(_baseAddress + 0x7); } set { Emulator.WriteByte(_baseAddress + 0x7, value); } }
        public byte Status { get { return Emulator.ReadByte(_baseAddress + 0x8); } set { Emulator.WriteByte(_baseAddress + 0x8, value); } }
        public byte Percentage { get { return Emulator.ReadByte(_baseAddress + 0x9); } set { Emulator.WriteByte(_baseAddress + 0x9, value); } }
        public byte BaseSwitch { get { return Emulator.ReadByte(_baseAddress + 0xB); } set { Emulator.WriteByte(_baseAddress + 0xB, value); } }


        public ItemTableEntry(long baseAddress, int item) {
            _baseAddress = baseAddress + item * 0xC;
        }
    }
}
