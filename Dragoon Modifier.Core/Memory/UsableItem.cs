namespace Dragoon_Modifier.Core.Memory {
    public class UsableItem : Item {
        public byte Target { get { return Emulator.ReadByte(_baseAddress); } set { Emulator.WriteByte(_baseAddress, value); } }
        public byte Element { get { return Emulator.ReadByte(_baseAddress + 0x1); } set { Emulator.WriteByte(_baseAddress + 0x1, value); } }
        public byte Damage { get { return Emulator.ReadByte(_baseAddress + 0x2); } set { Emulator.WriteByte(_baseAddress + 0x2, value); } }
        public byte Special1 { get { return Emulator.ReadByte(_baseAddress + 0x3); } set { Emulator.WriteByte(_baseAddress + 0x3, value); } }
        public byte Special2 { get { return Emulator.ReadByte(_baseAddress + 0x4); } set { Emulator.WriteByte(_baseAddress + 0x4, value); } }
        public byte Unknown1 { get { return Emulator.ReadByte(_baseAddress + 0x5); } set { Emulator.WriteByte(_baseAddress + 0x5, value); } }
        public byte SpecialAmmount { get { return Emulator.ReadByte(_baseAddress + 0x6); } set { Emulator.WriteByte(_baseAddress + 0x6, value); } }
        public override byte Icon { get { return Emulator.ReadByte(_baseAddress + 0x7); } set { Emulator.WriteByte(_baseAddress + 0x7, value); } }
        public byte Status { get { return Emulator.ReadByte(_baseAddress + 0x8); } set { Emulator.WriteByte(_baseAddress + 0x8, value); } }
        public byte Percentage { get { return Emulator.ReadByte(_baseAddress + 0x9); } set { Emulator.WriteByte(_baseAddress + 0x9, value); } }
        public byte Unknown2 { get { return Emulator.ReadByte(_baseAddress + 0xA); } set { Emulator.WriteByte(_baseAddress + 0xA, value); } }
        public byte BaseSwitch { get { return Emulator.ReadByte(_baseAddress + 0xB); } set { Emulator.WriteByte(_baseAddress + 0xB, value); } }

        internal UsableItem(int baseAddress, int namePointerAddress, int descriptionPointerAddress, int sellPriceAddress, int item) : base() {
            _baseAddress = baseAddress + ((item - 192) * 0xC);
            _namePointerAddress = namePointerAddress + (item * 0x4);
            _descriptionPointerAddress = descriptionPointerAddress + (item * 0x4);
            _sellPriceAddress = sellPriceAddress + (item * 0x2);
        }
    }
}
