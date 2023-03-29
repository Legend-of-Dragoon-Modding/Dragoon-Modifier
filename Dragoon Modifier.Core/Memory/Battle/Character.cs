using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Core.Memory.Battle {
    public sealed class Character : Entity {
        private readonly int _menuBlock;
        private readonly int _dragoonTurns;
        private readonly int _slot;

        public uint BattleScriptPtr1 { get { return Emulator.DirectAccess.ReadUInt(_baseAddress - 0x108); } set { Emulator.DirectAccess.WriteUInt(_baseAddress - 0x108, value); } }
        public uint BattleScriptPtr2 { get { return Emulator.DirectAccess.ReadUInt(_baseAddress - 0x104); } set { Emulator.DirectAccess.WriteUInt(_baseAddress - 0x104, value); } }
        public uint BattleScriptPtr3 { get { return Emulator.DirectAccess.ReadUInt(_baseAddress - 0x100); } set { Emulator.DirectAccess.WriteUInt(_baseAddress - 0x100, value); } }
        public uint BattleScriptPtr4 { get { return Emulator.DirectAccess.ReadUInt(_baseAddress - 0xFC); } set { Emulator.DirectAccess.WriteUInt(_baseAddress - 0xFC, value); } }
        public uint BattleScriptPtr5 { get { return Emulator.DirectAccess.ReadUInt(_baseAddress - 0xF8); } set { Emulator.DirectAccess.WriteUInt(_baseAddress - 0xF8, value); } }
        public uint BattleScriptPtr6 { get { return Emulator.DirectAccess.ReadUInt(_baseAddress - 0xF4); } set { Emulator.DirectAccess.WriteUInt(_baseAddress - 0xF4, value); } }
        public uint BattleScriptPtr7 { get { return Emulator.DirectAccess.ReadUInt(_baseAddress - 0xF0); } set { Emulator.DirectAccess.WriteUInt(_baseAddress - 0xF0, value); } }
        public uint BattleScriptPtr8 { get { return Emulator.DirectAccess.ReadUInt(_baseAddress - 0xEC); } set { Emulator.DirectAccess.WriteUInt(_baseAddress - 0xEC, value); } }
        public uint BattleScriptPtr9 { get { return Emulator.DirectAccess.ReadUInt(_baseAddress - 0xE8); } set { Emulator.DirectAccess.WriteUInt(_baseAddress - 0xE8, value); } }
        public uint BattleScriptPtr10 { get { return Emulator.DirectAccess.ReadUInt(_baseAddress - 0xE4); } set { Emulator.DirectAccess.WriteUInt(_baseAddress - 0xE4, value); } } //not sure if these are used or if they are even pointers
        public uint BattleScriptPtr11 { get { return Emulator.DirectAccess.ReadUInt(_baseAddress - 0xE0); } set { Emulator.DirectAccess.WriteUInt(_baseAddress - 0xE0, value); } }
        public uint BattleScriptPtr12 { get { return Emulator.DirectAccess.ReadUInt(_baseAddress - 0xCC); } set { Emulator.DirectAccess.WriteUInt(_baseAddress - 0xCC, value); } }
        public byte Menu { get { return Emulator.DirectAccess.ReadByte(_baseAddress - 0xA4); } set { Emulator.DirectAccess.WriteByte(_baseAddress - 0xA4, value); } }
        public byte SpecialAnimationFrameCounter { get { return Emulator.DirectAccess.ReadByte(_baseAddress - 0x68); } set { Emulator.DirectAccess.WriteByte(_baseAddress - 0x68, value); } }
        public byte SpellTarget { get { return Emulator.DirectAccess.ReadByte(_baseAddress - 0x54); } set { Emulator.DirectAccess.WriteByte(_baseAddress - 0x54, value); } }
        public byte CastingSpell { get { return Emulator.DirectAccess.ReadByte(_baseAddress - 0x50); } set { Emulator.DirectAccess.WriteByte(_baseAddress - 0x50, value); } }
        public byte UnknownShanaBowValue { get { return Emulator.DirectAccess.ReadByte(_baseAddress - 0x4C); } set { Emulator.DirectAccess.WriteByte(_baseAddress - 0x4C, value); } } //don't know what it is used for but it changes from 8->7 when Shana uses her Dragoon Attack
        public byte LV { get { return Emulator.DirectAccess.ReadByte(_baseAddress - 0x4); } set { Emulator.DirectAccess.WriteByte(_baseAddress - 0x4, value); } }
        public byte DLV { get { return Emulator.DirectAccess.ReadByte(_baseAddress - 0x2); } set { Emulator.DirectAccess.WriteByte(_baseAddress - 0x2, value); } }
        public byte Dragoon { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x7); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x7, value); } }
        public ushort MP { get { return Emulator.DirectAccess.ReadUShort(_baseAddress + 0x4); } set { Emulator.DirectAccess.WriteUShort(_baseAddress + 0x4, value); } }
        public ushort MaxMP { get { return Emulator.DirectAccess.ReadUShort(_baseAddress + 0xA); } set { Emulator.DirectAccess.WriteUShort(_baseAddress + 0xA, value); } }
        public ushort SP { get { return Emulator.DirectAccess.ReadUShort(_baseAddress + 0x2); } set { Emulator.DirectAccess.WriteUShort(_baseAddress + 0x2, value); } }
        public byte Weapon { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x116); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x116, value); } }
        public byte Helmet { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x118); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x118, value); } }
        public byte Armor { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x11A); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x11A, value); } }
        public byte Shoes { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x11C); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x11C, value); } }
        public byte Accessory { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x11E); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x11E, value); } }
        public short A_HIT { get { return Emulator.DirectAccess.ReadShort(_baseAddress + 0x34); } set { Emulator.DirectAccess.WriteShort(_baseAddress + 0x34, value); } }
        public short M_HIT { get { return Emulator.DirectAccess.ReadShort(_baseAddress + 0x36); } set { Emulator.DirectAccess.WriteShort(_baseAddress + 0x36, value); } }
        public byte On_Hit_Status { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x42); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x42, value); } }
        public byte On_Hit_Status_Chance { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x3C); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x3C, value); } }
        public ushort Add_DMG_Multi { get { return Emulator.DirectAccess.ReadUShort(_baseAddress + 0x114); } set { Emulator.DirectAccess.WriteUShort(_baseAddress + 0x114, value); } }
        public ushort Add_SP_Multi { get { return Emulator.DirectAccess.ReadUShort(_baseAddress + 0x112); } set { Emulator.DirectAccess.WriteUShort(_baseAddress + 0x112, value); } }
        public ushort SP_Multi { get { return Emulator.DirectAccess.ReadUShort(_baseAddress + 0x120); } set { Emulator.DirectAccess.WriteUShort(_baseAddress + 0x120, value); } }
        public byte SP_P_Hit { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x122); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x122, value); } }
        public byte SP_P_Hit_Turns { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x123); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x123, value); } }
        public byte MP_P_Hit { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x124); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x124, value); } }
        public byte MP_P_Hit_Turns { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x125); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x125, value); } }
        public byte SP_M_Hit { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x126); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x126, value); } }
        public byte SP_M_Hit_Turns { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x127); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x127, value); } }
        public byte MP_M_Hit { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x128); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x128, value); } }
        public byte MP_M_Hit_Turns { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x129); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x129, value); } }
        public short HP_Regen { get { return Emulator.DirectAccess.ReadShort(_baseAddress + 0x12C); } set { Emulator.DirectAccess.WriteShort(_baseAddress + 0x12C, value); } }
        public short MP_Regen { get { return Emulator.DirectAccess.ReadShort(_baseAddress + 0x12E); } set { Emulator.DirectAccess.WriteShort(_baseAddress + 0x12E, value); } }
        public short SP_Regen { get { return Emulator.DirectAccess.ReadShort(_baseAddress + 0x130); } set { Emulator.DirectAccess.WriteShort(_baseAddress + 0x130, value); } }
        public byte Revive { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x132); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x132, value); } }
        public byte Weapon_Element { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x14); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x14, value); } }
        public ushort DAT { get { return Emulator.DirectAccess.ReadUShort(_baseAddress + 0xA4); } set { Emulator.DirectAccess.WriteUShort(_baseAddress + 0xA4, value); } }
        public ushort DMAT { get { return Emulator.DirectAccess.ReadUShort(_baseAddress + 0xA6); } set { Emulator.DirectAccess.WriteUShort(_baseAddress + 0xA6, value); } }
        public ushort DDF { get { return Emulator.DirectAccess.ReadUShort(_baseAddress + 0xA8); } set { Emulator.DirectAccess.WriteUShort(_baseAddress + 0xA8, value); } }
        public ushort DMDF { get { return Emulator.DirectAccess.ReadUShort(_baseAddress + 0xAA); } set { Emulator.DirectAccess.WriteUShort(_baseAddress + 0xAA, value); } }
        public byte Image { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x26A); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x26A, value); } }
        public ushort Detransform1 { get { return Emulator.DirectAccess.ReadUShort(_baseAddress - 0xF0); } set { Emulator.DirectAccess.WriteUShort(_baseAddress - 0xF0, value); } }
        public byte Detransform2 { get { return Emulator.DirectAccess.ReadByte(_baseAddress - 0xEE); } set { Emulator.DirectAccess.WriteByte(_baseAddress - 0xEE, value); } }
        public byte A_HIT_Increase { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0xB4); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0xB4, value); } }
        public byte A_HIT_Increase_Turn { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0xB5); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0xB5, value); } }
        public byte M_HIT_Increase { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0xB6); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0xB6, value); } }
        public byte M_HIT_Increase_Turn { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0xB7); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0xB7, value); } }
        public byte SelectedItem1 { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0xB8); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0xB8, value); } }
        public byte SP_P_Hit_Increase { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0xC4); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0xC4, value); } }
        public byte SP_P_Hit_Increase_Turn { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0xC5); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0xC5, value); } }
        public byte MP_P_Hit_Increase { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0xC6); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0xC6, value); } }
        public byte MP_P_Hit_Increase_Turn { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0xC7); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0xC7, value); } }
        public byte SP_M_Hit_Increase { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0xC8); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0xC8, value); } }
        public byte SP_M_Hit_Increase_Turn { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0xC9); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0xC9, value); } }
        public byte MP_M_Hit_Increase { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0xCA); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0xCA, value); } }
        public byte MP_M_Hit_Increase_Turn { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0xCB); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0xCB, value); } }
        public byte SelectedItem2 { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x14E); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x14E, value); } }
        public byte ColorMap { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x1DD); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x1DD, value); } }
        public byte AdditionSlotIndex { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x26E); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x26E, value); } }
        public byte Pandemonium { get { return (byte) (Emulator.DirectAccess.ReadByte(_pShieldMShieldSigStone + 0x1) & 3); } set { Emulator.DirectAccess.WriteByte(_pShieldMShieldSigStone + 0x1, (byte) (Emulator.DirectAccess.ReadByte(_pShieldMShieldSigStone + 0x1) | Math.Min(value, (byte) 3))); } }
        public byte MenuBlock { get { return Emulator.DirectAccess.ReadByte(_menuBlock); } set { Emulator.DirectAccess.WriteByte(_menuBlock, value); } }
        public byte DragoonTurns { get { return Emulator.DirectAccess.ReadByte(_dragoonTurns); } set { Emulator.DirectAccess.WriteByte(_dragoonTurns, value); } }
        public byte IsDragoon { get { return Emulator.DirectAccess.ReadByte(_baseAddress - 0x48); } set { Emulator.DirectAccess.WriteByte(_baseAddress - 0x48, value); } }
        public AdditionHit[] Addition = new AdditionHit[8];
        public uint ID { get { return Emulator.Memory.PartySlot[_slot]; } set { Emulator.Memory.PartySlot[_slot] = value; } }

        internal Character(uint c_point, int slot, int position, int battleOffset) : base(c_point, slot, position) {
            _menuBlock = 0x6E3B0 + slot * 0x20; // TODO This has to get an address
            _dragoonTurns = Emulator.GetAddress("DRAGOON_TURNS") + slot * 0x4;
            _slot = slot;

            var additionAddress = Emulator.GetAddress("ADDITION") + battleOffset;
            for (int i = 0; i < Addition.Length; i++) {
                Addition[i] = new AdditionHit(additionAddress + (i * 0x20) + _slot * (0x100));
            }
        }

        public void Detransform() {
            DragoonTurns = 1;
            IsDragoon = 1;
            Detransform1 += 0x4478;
            Detransform2 = 27;
        }

        public void SetStats(uint character) {
            Console.WriteLine(character);
            var primaryTable = Emulator.Memory.CharacterTable[character];
            var secondaryTable = Emulator.Memory.SecondaryCharacterTable[character];

            HP = secondaryTable.HP;
            MaxHP = secondaryTable.MaxHP;
            MP = secondaryTable.MP;
            MaxMP = secondaryTable.MaxMP;
            SP = secondaryTable.SP;

            ushort at = (ushort) (secondaryTable.BodyAT + secondaryTable.EquipAT);
            AT = at;
            OG_AT = at;

            ushort mat = (ushort) (secondaryTable.BodyMAT + secondaryTable.EquipMAT);
            MAT = mat;
            OG_MAT = mat;

            ushort df = (ushort) (secondaryTable.BodyDF + secondaryTable.EquipDF);
            DF = df;
            OG_DF = df;

            ushort mdf = (ushort) (secondaryTable.BodyMDF + secondaryTable.EquipMDF);
            MDF = mdf;
            OG_MDF = mdf;

            ushort spd = (ushort) (secondaryTable.BodySPD + secondaryTable.EquipSPD);
            SPD = spd;
            OG_SPD = spd;

            StatusResist = secondaryTable.StatusResist;
            ElementalResistance = secondaryTable.E_Half;
            ElementalImmunity = secondaryTable.E_Immune;
            A_AV = secondaryTable.EquipA_AV;
            M_AV = secondaryTable.EquipM_AV;
            A_HIT = secondaryTable.EquipA_HIT;
            M_HIT = secondaryTable.EquipM_HIT;
            P_Half = secondaryTable.P_Half;
            M_Half = secondaryTable.M_Half;

            MP_M_Hit = secondaryTable.MP_M_Hit;
            SP_M_Hit = secondaryTable.SP_M_Hit;
            MP_P_Hit = secondaryTable.MP_P_Hit;
            SP_P_Hit = secondaryTable.SP_P_Hit;

            SP_Multi = secondaryTable.SP_Multi;

            Revive = secondaryTable.Revive;
            SpecialEffect = secondaryTable.SpecialEffect;

            Weapon_Element = secondaryTable.WeaponElement;
            On_Hit_Status = secondaryTable.OnHitStatus;
            On_Hit_Status_Chance = secondaryTable.OnHitStatusChance;
        }
    }
}
