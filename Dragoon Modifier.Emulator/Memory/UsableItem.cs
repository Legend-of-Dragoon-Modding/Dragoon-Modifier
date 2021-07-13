using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Emulator.Memory {
    internal class UsableItem : Item, IUsableItem {
        public byte Target { get { return _emulator.ReadByte(_baseAddress); } set { _emulator.WriteByte(_baseAddress, value); } }
        public byte Element { get { return _emulator.ReadByte(_baseAddress + 0x1); } set { _emulator.WriteByte(_baseAddress + 0x1, value); } }
        public byte Damage { get { return _emulator.ReadByte(_baseAddress + 0x2); } set { _emulator.WriteByte(_baseAddress + 0x2, value); } }
        public byte Special1 { get { return _emulator.ReadByte(_baseAddress + 0x3); } set { _emulator.WriteByte(_baseAddress + 0x3, value); } }
        public byte Special2 { get { return _emulator.ReadByte(_baseAddress + 0x4); } set { _emulator.WriteByte(_baseAddress + 0x4, value); } }
        public byte Unknown1 { get { return _emulator.ReadByte(_baseAddress + 0x5); } set { _emulator.WriteByte(_baseAddress + 0x5, value); } }
        public byte SpecialAmmount { get { return _emulator.ReadByte(_baseAddress + 0x6); } set { _emulator.WriteByte(_baseAddress + 0x6, value); } }
        public override byte Icon { get { return _emulator.ReadByte(_baseAddress + 0x7); } set { _emulator.WriteByte(_baseAddress + 0x7, value); } }
        public byte Status { get { return _emulator.ReadByte(_baseAddress + 0x8); } set { _emulator.WriteByte(_baseAddress + 0x8, value); } }
        public byte Percentage { get { return _emulator.ReadByte(_baseAddress + 0x9); } set { _emulator.WriteByte(_baseAddress + 0x9, value); } }
        public byte Unknown2 { get { return _emulator.ReadByte(_baseAddress + 0xA); } set { _emulator.WriteByte(_baseAddress + 0xA, value); } }
        public byte BaseSwitch { get { return _emulator.ReadByte(_baseAddress + 0xB); } set { _emulator.WriteByte(_baseAddress + 0xB, value); } }

        internal UsableItem(IEmulator emulator, int baseAddress, int namePointerAddress, int descriptionPointerAddress, int sellPriceAddress, int item) : base() {
            _emulator = emulator;
            _baseAddress = baseAddress + ((item - 192) * 0xC);
            _namePointerAddress = namePointerAddress + (item * 0x4);
            _descriptionPointerAddress = descriptionPointerAddress + (item * 0x4);
            _sellPriceAddress = sellPriceAddress + (item * 0x2);
        }
    }
}
