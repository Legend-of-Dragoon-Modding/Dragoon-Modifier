using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Emulator.Memory.Battle {
    public class Character : Entity {
        private readonly int _menuBlock;
        private readonly int _dragoonTurns;
        private readonly int _slot;

        public byte Menu { get { return _emulator.ReadByte(_baseAddress - 0xA4); } set { _emulator.WriteByte(_baseAddress - 0xA4, value); } }
        public byte LV { get { return _emulator.ReadByte(_baseAddress - 0x4); } set { _emulator.WriteByte(_baseAddress - 0x4, value); } }
        public byte DLV { get { return _emulator.ReadByte(_baseAddress - 0x2); } set { _emulator.WriteByte(_baseAddress - 0x2, value); } }
        public byte Dragoon { get { return _emulator.ReadByte(_baseAddress + 0x7); } set { _emulator.WriteByte(_baseAddress + 0x7, value); } }
        public ushort MP { get { return _emulator.ReadUShort(_baseAddress + 0x4); } set { _emulator.WriteUShort(_baseAddress + 0x4, value); } }
        public ushort MaxMP { get { return _emulator.ReadUShort(_baseAddress + 0xA); } set { _emulator.WriteUShort(_baseAddress + 0xA, value); } }
        public ushort SP { get { return _emulator.ReadUShort(_baseAddress + 0x2); } set { _emulator.WriteUShort(_baseAddress + 0x2, value); } }
        public byte Weapon { get { return _emulator.ReadByte(_baseAddress + 0x116); } set { _emulator.WriteByte(_baseAddress + 0x116, value); } }
        public byte Helmet { get { return _emulator.ReadByte(_baseAddress + 0x118); } set { _emulator.WriteByte(_baseAddress + 0x118, value); } }
        public byte Armor { get { return _emulator.ReadByte(_baseAddress + 0x11A); } set { _emulator.WriteByte(_baseAddress + 0x11A, value); } }
        public byte Shoes { get { return _emulator.ReadByte(_baseAddress + 0x11C); } set { _emulator.WriteByte(_baseAddress + 0x11C, value); } }
        public byte Accessory { get { return _emulator.ReadByte(_baseAddress + 0x11E); } set { _emulator.WriteByte(_baseAddress + 0x11E, value); } }
        public short A_HIT { get { return _emulator.ReadShort(_baseAddress + 0x34); } set { _emulator.WriteShort(_baseAddress + 0x34, value); } }
        public short M_HIT { get { return _emulator.ReadShort(_baseAddress + 0x36); } set { _emulator.WriteShort(_baseAddress + 0x36, value); } }
        public byte On_Hit_Status { get { return _emulator.ReadByte(_baseAddress + 0x42); } set { _emulator.WriteByte(_baseAddress + 0x42, value); } }
        public byte On_Hit_Status_Chance { get { return _emulator.ReadByte(_baseAddress + 0x3C); } set { _emulator.WriteByte(_baseAddress + 0x3C, value); } }
        public ushort Add_DMG_Multi { get { return _emulator.ReadUShort(_baseAddress + 0x114); } set { _emulator.WriteUShort(_baseAddress + 0x114, value); } }
        public ushort Add_SP_Multi { get { return _emulator.ReadUShort(_baseAddress + 0x112); } set { _emulator.WriteUShort(_baseAddress + 0x112, value); } }
        public short SP_Multi { get { return _emulator.ReadShort(_baseAddress + 0x120); } set { _emulator.WriteShort(_baseAddress + 0x120, value); } }
        public short SP_P_Hit { get { return _emulator.ReadShort(_baseAddress + 0x122); } set { _emulator.WriteShort(_baseAddress + 0x122, value); } }
        public short MP_P_Hit { get { return _emulator.ReadShort(_baseAddress + 0x124); } set { _emulator.WriteShort(_baseAddress + 0x124, value); } }
        public short SP_M_Hit { get { return _emulator.ReadShort(_baseAddress + 0x126); } set { _emulator.WriteShort(_baseAddress + 0x126, value); } }
        public short MP_M_Hit { get { return _emulator.ReadShort(_baseAddress + 0x128); } set { _emulator.WriteShort(_baseAddress + 0x128, value); } }
        public short HP_Regen { get { return _emulator.ReadShort(_baseAddress + 0x12C); } set { _emulator.WriteShort(_baseAddress + 0x12C, value); } }
        public short MP_Regen { get { return _emulator.ReadShort(_baseAddress + 0x12E); } set { _emulator.WriteShort(_baseAddress + 0x12E, value); } }
        public short SP_Regen { get { return _emulator.ReadShort(_baseAddress + 0x130); } set { _emulator.WriteShort(_baseAddress + 0x130, value); } }
        public byte Revive { get { return _emulator.ReadByte(_baseAddress + 0x132); } set { _emulator.WriteByte(_baseAddress + 0x132, value); } }
        public byte Weapon_Element { get { return _emulator.ReadByte(_baseAddress + 0x14); } set { _emulator.WriteByte(_baseAddress + 0x14, value); } }
        public ushort DAT { get { return _emulator.ReadUShort(_baseAddress + 0xA4); } set { _emulator.WriteUShort(_baseAddress + 0xA4, value); } }
        public ushort DMAT { get { return _emulator.ReadUShort(_baseAddress + 0xA6); } set { _emulator.WriteUShort(_baseAddress + 0xA6, value); } }
        public ushort DDF { get { return _emulator.ReadUShort(_baseAddress + 0xA8); } set { _emulator.WriteUShort(_baseAddress + 0xA8, value); } }
        public ushort DMDF { get { return _emulator.ReadUShort(_baseAddress + 0xAA); } set { _emulator.WriteUShort(_baseAddress + 0xAA, value); } }
        public byte Image { get { return _emulator.ReadByte(_baseAddress + 0x26A); } set { _emulator.WriteByte(_baseAddress + 0x26A, value); } }
        public ushort Detransform1 { get { return _emulator.ReadUShort(_baseAddress - 0xF0); } set { _emulator.WriteUShort(_baseAddress - 0xF0, value); } }
        public byte Detransform2 { get { return _emulator.ReadByte(_baseAddress - 0xEE); } set { _emulator.WriteByte(_baseAddress - 0xEE, value); } }
        public sbyte A_HIT_Increase { get { return _emulator.ReadSByte(_baseAddress + 0xB4); } set { _emulator.WriteSByte(_baseAddress + 0xB4, value); } }
        public byte A_HIT_Increase_Turn { get { return _emulator.ReadByte(_baseAddress + 0xB5); } set { _emulator.WriteByte(_baseAddress + 0xB5, value); } }
        public sbyte M_HIT_Increase { get { return _emulator.ReadSByte(_baseAddress + 0xB6); } set { _emulator.WriteSByte(_baseAddress + 0xB6, value); } }
        public byte M_HIT_Increase_Turn { get { return _emulator.ReadByte(_baseAddress + 0xB7); } set { _emulator.WriteByte(_baseAddress + 0xB7, value); } }
        public sbyte SP_P_Hit_Increase { get { return _emulator.ReadSByte(_baseAddress + 0xC4); } set { _emulator.WriteSByte(_baseAddress + 0xC4, value); } }
        public byte SP_P_Hit_Increase_Turn { get { return _emulator.ReadByte(_baseAddress + 0xC5); } set { _emulator.WriteByte(_baseAddress + 0xC5, value); } }
        public sbyte MP_P_Hit_Increase { get { return _emulator.ReadSByte(_baseAddress + 0xC6); } set { _emulator.WriteSByte(_baseAddress + 0xC6, value); } }
        public byte MP_P_Hit_Increase_Turn { get { return _emulator.ReadByte(_baseAddress + 0xC7); } set { _emulator.WriteByte(_baseAddress + 0xC7, value); } }
        public sbyte SP_M_Hit_Increase { get { return _emulator.ReadSByte(_baseAddress + 0xC8); } set { _emulator.WriteSByte(_baseAddress + 0xC8, value); } }
        public byte SP_M_Hit_Increase_Turn { get { return _emulator.ReadByte(_baseAddress + 0xC9); } set { _emulator.WriteByte(_baseAddress + 0xC9, value); } }
        public sbyte MP_M_Hit_Increase { get { return _emulator.ReadSByte(_baseAddress + 0xCA); } set { _emulator.WriteSByte(_baseAddress + 0xCA, value); } }
        public byte MP_M_Hit_Increase_Turn { get { return _emulator.ReadByte(_baseAddress + 0xCB); } set { _emulator.WriteByte(_baseAddress + 0xCB, value); } }
        public byte ColorMap { get { return _emulator.ReadByte(_baseAddress + 0x1DD); } set { _emulator.WriteByte(_baseAddress + 0x1DD, value); } }
        public byte AdditionSlotIndex { get { return _emulator.ReadByte(_baseAddress + 0x26E); } set { _emulator.WriteByte(_baseAddress + 0x26E, value); } }
        public byte Pandemonium { get { return (byte) (_emulator.ReadByte(_pShieldMShieldSigStone + 0x1) & 3); } set { _emulator.WriteByte(_pShieldMShieldSigStone + 0x1, (byte) (_emulator.ReadByte(_pShieldMShieldSigStone + 0x1) | Math.Min(value, (byte) 3))); } }
        public byte MenuBlock { get { return _emulator.ReadByte(_menuBlock); } set { _emulator.WriteByte(_menuBlock, value); } }
        public byte DragoonTurns { get { return _emulator.ReadByte(_dragoonTurns); } set { _emulator.WriteByte(_dragoonTurns, value); } }
        public byte IsDragoon { get { return _emulator.ReadByte(_baseAddress - 0x48); } set { _emulator.WriteByte(_baseAddress - 0x48, value); } }
        public AdditionHit[] Addition = new AdditionHit[8];
        public uint ID { get { return _emulator.Memory.PartySlot[_slot]; } set { _emulator.Memory.PartySlot[_slot] = value; } }

        internal Character(IEmulator emulator, uint c_point, int slot, int position, int battleOffset) : base(emulator, c_point, slot, position) {
            _menuBlock = 0x6E3B0 + slot * 0x20; // TODO This has to get an address
            _dragoonTurns = _emulator.GetAddress("DRAGOON_TURNS") + slot * 0x4;
            _slot = slot;

            var additionAddress = _emulator.GetAddress("ADDITION") + battleOffset;
            for (int i = 0; i < Addition.Length; i++) {
                Addition[i] = new AdditionHit(_emulator, additionAddress + (i * 0x20) + _slot * (0x100));
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
            var primaryTable = _emulator.Memory.CharacterTable[character];
            var secondaryTable = _emulator.Memory.SecondaryCharacterTable[character];

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
