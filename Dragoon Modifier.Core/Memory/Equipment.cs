﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Core.Memory {
    internal sealed class Equipment : Item, IEquipment {
        public byte SpecialEffect { get { return Emulator.DirectAccess.ReadByte(_baseAddress); } set { Emulator.DirectAccess.WriteByte(_baseAddress, value); } }
        public byte ItemType { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x1); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x1, value); } }
        public byte WhoEquips { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x3); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x3, value); } }
        public byte WeaponElement { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x4); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x4, value); } }
        public byte E_Half { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x6); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x6, value); } }
        public byte E_Immune { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x7); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x7, value); } }
        public byte StatusResist { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x8); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x8, value); } }
        public byte AT { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0xA); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0xA, value); } }
        public byte Special1 { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0xB); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0xB, value); } }
        public byte Special2 { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0xC); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0xC, value); } }
        public byte SpecialAmmount { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0xD); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0xD, value); } }
        public override byte Icon { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0xE); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0xE, value); } }
        public byte SPD { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0xF); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0xF, value); } }
        public byte AT2 { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x10); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x10, value); } }
        public byte MAT { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x11); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x11, value); } }
        public byte DF { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x12); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x12, value); } }
        public byte MDF { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x13); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x13, value); } }
        public byte A_HIT { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x14); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x14, value); } }
        public byte M_HIT { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x15); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x15, value); } }
        public byte A_AV { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x16); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x16, value); } }
        public byte M_AV { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x17); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x17, value); } }
        public byte StatusChance { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x18); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x18, value); } }
        public byte Status { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x1B); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x1B, value); } }

        internal Equipment(int baseAddress, int namePointerAddress, int descriptionPointerAddress, int sellPriceAddress, int item) : base() {
            _baseAddress = baseAddress + (item * 0x1C);
            _namePointerAddress = namePointerAddress + (item * 0x4);
            _descriptionPointerAddress = descriptionPointerAddress + (item * 0x4);
            _sellPriceAddress = sellPriceAddress + (item * 0x2);
        }
    }
}
