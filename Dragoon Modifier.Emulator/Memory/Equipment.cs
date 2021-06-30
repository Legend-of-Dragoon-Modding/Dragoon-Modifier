using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Emulator.Memory {
    public class Equipment : Item {
        public byte SpecialEffect { get { return _emulator.ReadByte(_baseAddress); } set { _emulator.WriteByte(_baseAddress, value); } }
        public byte ItemType { get { return _emulator.ReadByte(_baseAddress + 0x1); } set { _emulator.WriteByte(_baseAddress + 0x1, value); } }
        public byte WhoEquips { get { return _emulator.ReadByte(_baseAddress + 0x3); } set { _emulator.WriteByte(_baseAddress + 0x3, value); } }
        public byte WeaponElement { get { return _emulator.ReadByte(_baseAddress + 0x4); } set { _emulator.WriteByte(_baseAddress + 0x4, value); } }
        public byte E_Half { get { return _emulator.ReadByte(_baseAddress + 0x6); } set { _emulator.WriteByte(_baseAddress + 0x6, value); } }
        public byte E_Immune { get { return _emulator.ReadByte(_baseAddress + 0x7); } set { _emulator.WriteByte(_baseAddress + 0x7, value); } }
        public byte StatusResist { get { return _emulator.ReadByte(_baseAddress + 0x8); } set { _emulator.WriteByte(_baseAddress + 0x8, value); } }
        public byte AT { get { return _emulator.ReadByte(_baseAddress + 0xA); } set { _emulator.WriteByte(_baseAddress + 0xA, value); } }
        public byte Special1 { get { return _emulator.ReadByte(_baseAddress + 0xB); } set { _emulator.WriteByte(_baseAddress + 0xB, value); } }
        public byte Special2 { get { return _emulator.ReadByte(_baseAddress + 0xC); } set { _emulator.WriteByte(_baseAddress + 0xC, value); } }
        public byte SpecialAmmount { get { return _emulator.ReadByte(_baseAddress + 0xD); } set { _emulator.WriteByte(_baseAddress + 0xD, value); } }
        public override byte Icon { get { return _emulator.ReadByte(_baseAddress + 0xE); } set { _emulator.WriteByte(_baseAddress + 0xE, value); } }
        public byte SPD { get { return _emulator.ReadByte(_baseAddress + 0xF); } set { _emulator.WriteByte(_baseAddress + 0xF, value); } }
        public byte AT2 { get { return _emulator.ReadByte(_baseAddress + 0x10); } set { _emulator.WriteByte(_baseAddress + 0x10, value); } }
        public byte MAT { get { return _emulator.ReadByte(_baseAddress + 0x11); } set { _emulator.WriteByte(_baseAddress + 0x11, value); } }
        public byte DF { get { return _emulator.ReadByte(_baseAddress + 0x12); } set { _emulator.WriteByte(_baseAddress + 0x12, value); } }
        public byte MDF { get { return _emulator.ReadByte(_baseAddress + 0x13); } set { _emulator.WriteByte(_baseAddress + 0x13, value); } }
        public byte A_HIT { get { return _emulator.ReadByte(_baseAddress + 0x14); } set { _emulator.WriteByte(_baseAddress + 0x14, value); } }
        public byte M_HIT { get { return _emulator.ReadByte(_baseAddress + 0x15); } set { _emulator.WriteByte(_baseAddress + 0x15, value); } }
        public byte A_AV { get { return _emulator.ReadByte(_baseAddress + 0x16); } set { _emulator.WriteByte(_baseAddress + 0x16, value); } }
        public byte M_AV { get { return _emulator.ReadByte(_baseAddress + 0x17); } set { _emulator.WriteByte(_baseAddress + 0x17, value); } }
        public byte StatusChance { get { return _emulator.ReadByte(_baseAddress + 0x18); } set { _emulator.WriteByte(_baseAddress + 0x18, value); } }
        public byte Status { get { return _emulator.ReadByte(_baseAddress + 0x1B); } set { _emulator.WriteByte(_baseAddress + 0x1B, value); } }

        internal Equipment(IEmulator emulator, int baseAddress, int namePointerAddress, int descriptionPointerAddress, int sellPriceAddress, int item) : base() {
            _emulator = emulator;
            _baseAddress = baseAddress + (item * 0x1C);
            _namePointerAddress = namePointerAddress + (item * 0x4);
            _descriptionPointerAddress = descriptionPointerAddress + (item * 0x4);
            _sellPriceAddress = sellPriceAddress + (item * 0x2);
        }
    }
}
