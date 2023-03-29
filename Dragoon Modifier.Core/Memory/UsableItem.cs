using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Core.Memory {
    internal sealed class UsableItem : Item, IUsableItem {
        private readonly int _battleNamePointerAddress;
        private readonly int _battleDescriptionPointerAddress;

        public byte Target { get { return Emulator.DirectAccess.ReadByte(_baseAddress); } set { Emulator.DirectAccess.WriteByte(_baseAddress, value); } }
        public byte Element { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x1); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x1, value); } }
        public byte Damage { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x2); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x2, value); } }
        public byte Special1 { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x3); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x3, value); } }
        public byte Special2 { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x4); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x4, value); } }
        public byte Unknown1 { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x5); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x5, value); } }
        public byte SpecialAmmount { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x6); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x6, value); } }
        public override byte Icon { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x7); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x7, value); } }
        public byte Status { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x8); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x8, value); } }
        public byte Percentage { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x9); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x9, value); } }
        public byte Unknown2 { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0xA); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0xA, value); } }
        public byte BaseSwitch { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0xB); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0xB, value); } }
        public string BattleName { get { return Emulator.DirectAccess.ReadText(BattleNamePointer); } }
        public uint BattleNamePointer { get { return Emulator.DirectAccess.ReadUInt24(_battleNamePointerAddress); } set { Emulator.DirectAccess.WriteUInt24(_battleNamePointerAddress, value); } }
        public string BattleDescription { get { return Emulator.DirectAccess.ReadText(BattleDescriptionPointer); } }
        public uint BattleDescriptionPointer { get { return Emulator.DirectAccess.ReadUInt24(_battleDescriptionPointerAddress); } set { Emulator.DirectAccess.WriteUInt24(_battleDescriptionPointerAddress, value); } }


        internal UsableItem(int baseAddress, int namePointerAddress, int descriptionPointerAddress, int battleNamePointerAddress, int battleDescriptionPointerAddress, int sellPriceAddress, int item) : base() {
            _baseAddress = baseAddress + ((item - 192) * 0xC);
            _namePointerAddress = namePointerAddress + (item * 0x4);
            _descriptionPointerAddress = descriptionPointerAddress + (item * 0x4);
            _sellPriceAddress = sellPriceAddress + (item * 0x2);
            _battleNamePointerAddress = battleNamePointerAddress + ((item - 192) * 0x4);
            _battleDescriptionPointerAddress = battleDescriptionPointerAddress + ((item - 192) * 0x4);
        }
    }
}
