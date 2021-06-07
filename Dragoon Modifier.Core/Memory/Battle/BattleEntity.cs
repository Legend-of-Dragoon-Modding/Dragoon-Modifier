using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Core.Memory.Battle {
    public class BattleEntity {
        protected int _baseAddress;
        protected int _pShieldMShieldSigStone;
        private readonly int _statusAddr;
        private readonly int _dragoonSpellsAddr;
        private readonly int _additionSpecial;

        public uint BaseAddress { get { return Emulator.ReadUInt24(_baseAddress - 0x108) + 0x108; } }
        public byte Action { get { return Emulator.ReadByte(_baseAddress - 0xA8); } set { Emulator.WriteByte(_baseAddress - 0xA8, value); } }
        public byte Spell_Cast { get { return Emulator.ReadByte(_baseAddress + 0x46); } set { Emulator.WriteByte(_baseAddress + 0x46, value); } }
        public ushort HP { get { return Emulator.ReadUShort(_baseAddress); } set { Emulator.WriteUShort(_baseAddress, value); } }
        public ushort Max_HP { get { return Emulator.ReadUShort(_baseAddress + 0x8); } set { Emulator.WriteUShort(_baseAddress + 0x8, value); } }
        public byte Status { get { return Emulator.ReadByte(_baseAddress + 0x6); } set { Emulator.WriteByte(_baseAddress + 0x6, value); } }
        public byte StatusResist { get { return Emulator.ReadByte(_baseAddress + 0x1C); } set { Emulator.WriteByte(_baseAddress + 0x1C, value); } }
        public byte SpecialEffect { get { return Emulator.ReadByte(_baseAddress + 0xC); } set { Emulator.WriteByte(_baseAddress + 0xC, value); } } // Old Death_Res
        public byte Guard { get { return Emulator.ReadByte(_baseAddress + 0x4C); } set { Emulator.WriteByte(_baseAddress + 0x4C, value); } }
        public ushort AT { get { return Emulator.ReadUShort(_baseAddress + 0x2C); } set { Emulator.WriteUShort(_baseAddress + 0x2C, value); } }
        public ushort OG_AT { get { return Emulator.ReadUShort(_baseAddress + 0x58); } set { Emulator.WriteUShort(_baseAddress + 0x58, value); } }
        public ushort MAT { get { return Emulator.ReadUShort(_baseAddress + 0x2E); } set { Emulator.WriteUShort(_baseAddress + 0x2E, value); } }
        public ushort OG_MAT { get { return Emulator.ReadUShort(_baseAddress + 0x5A); } set { Emulator.WriteUShort(_baseAddress + 0x5A, value); } }
        public ushort DF { get { return Emulator.ReadUShort(_baseAddress + 0x30); } set { Emulator.WriteUShort(_baseAddress + 0x30, value); } }
        public ushort OG_DF { get { return Emulator.ReadUShort(_baseAddress + 0x5E); } set { Emulator.WriteUShort(_baseAddress + 0x5E, value); } }
        public ushort MDF { get { return Emulator.ReadUShort(_baseAddress + 0x32); } set { Emulator.WriteUShort(_baseAddress + 0x32, value); } }
        public ushort OG_MDF { get { return Emulator.ReadUShort(_baseAddress + 0x60); } set { Emulator.WriteUShort(_baseAddress + 0x60, value); } }
        public ushort SPD { get { return Emulator.ReadUShort(_baseAddress + 0x2A); } set { Emulator.WriteUShort(_baseAddress + 0x2A, value); } }
        public ushort OG_SPD { get { return Emulator.ReadUShort(_baseAddress + 0x5C); } set { Emulator.WriteUShort(_baseAddress + 0x5C, value); } }
        public ushort Turn { get { return Emulator.ReadUShort(_baseAddress + 0x44); } set { Emulator.WriteUShort(_baseAddress + 0x44, value); } }
        public short A_AV { get { return Emulator.ReadShort(_baseAddress + 0x38); } set { Emulator.WriteShort(_baseAddress + 0x38, value); } }
        public short M_AV { get { return Emulator.ReadShort(_baseAddress + 0x3A); } set { Emulator.WriteShort(_baseAddress + 0x3A, value); } }
        public byte P_Immune { get { return Emulator.ReadByte(_baseAddress + 0x108); } set { Emulator.WriteByte(_baseAddress + 0x108, value); } }
        public byte M_Immune { get { return Emulator.ReadByte(_baseAddress + 0x10A); } set { Emulator.WriteByte(_baseAddress + 0x10A, value); } }
        public byte P_Half { get { return Emulator.ReadByte(_baseAddress + 0x10C); } set { Emulator.WriteByte(_baseAddress + 0x10C, value); } }
        public byte M_Half { get { return Emulator.ReadByte(_baseAddress + 0x10E); } set { Emulator.WriteByte(_baseAddress + 0x10E, value); } }
        public byte ElementalImmunity { get { return Emulator.ReadByte(_baseAddress + 0x1A); } set { Emulator.WriteByte(_baseAddress + 0x1A, value); } }
        public byte ElementalResistance { get { return Emulator.ReadByte(_baseAddress + 0x18); } set { Emulator.WriteByte(_baseAddress + 0x18, value); } }
        public byte UniqueIndex { get { return Emulator.ReadByte(_baseAddress + 0x264); } set { Emulator.WriteByte(_baseAddress + 0x264, value); } }
        public sbyte PWR_AT { get { return Emulator.ReadSByte(_baseAddress + 0xAC); } set { Emulator.WriteSByte(_baseAddress + 0xAC, value); } }
        public byte PWR_AT_Turn { get { return Emulator.ReadByte(_baseAddress + 0xAD); } set { Emulator.WriteByte(_baseAddress + 0xAD, value); } }
        public sbyte PWR_MAT { get { return Emulator.ReadSByte(_baseAddress + 0xAE); } set { Emulator.WriteSByte(_baseAddress + 0xAE, value); } }
        public byte PWR_MAT_Turn { get { return Emulator.ReadByte(_baseAddress + 0xAF); } set { Emulator.WriteByte(_baseAddress + 0xAF, value); } }
        public sbyte PWR_DF { get { return Emulator.ReadSByte(_baseAddress + 0xB0); } set { Emulator.WriteSByte(_baseAddress + 0xB0, value); } }
        public byte PWR_DF_Turn { get { return Emulator.ReadByte(_baseAddress + 0xB1); } set { Emulator.WriteByte(_baseAddress + 0xB1, value); } }
        public sbyte PWR_MDF { get { return Emulator.ReadSByte(_baseAddress + 0xB2); } set { Emulator.WriteSByte(_baseAddress + 0xB2, value); } }
        public byte PWR_MDF_Turn { get { return Emulator.ReadByte(_baseAddress + 0xB3); } set { Emulator.WriteByte(_baseAddress + 0xB3, value); } }
        public byte Speed_Up_Turn { get { return Emulator.ReadByte(_baseAddress + 0xC1); } set { Emulator.WriteByte(_baseAddress + 0xC1, value); } }
        public byte Speed_Down_Turn { get { return Emulator.ReadByte(_baseAddress + 0xC3); } set { Emulator.WriteByte(_baseAddress + 0xC3, value); } }
        public byte P_Immune_Increase { get { return Emulator.ReadByte(_baseAddress + 0xBC); } set { Emulator.WriteByte(_baseAddress + 0xBC, value); } }
        public byte P_Immune_Increase_Turn { get { return Emulator.ReadByte(_baseAddress + 0xBD); } set { Emulator.WriteByte(_baseAddress + 0xBD, value); } }
        public byte M_Immune_Increase { get { return Emulator.ReadByte(_baseAddress + 0xBE); } set { Emulator.WriteByte(_baseAddress + 0xBE, value); } }
        public byte M_Immune_Increase_Turn { get { return Emulator.ReadByte(_baseAddress + 0xBF); } set { Emulator.WriteByte(_baseAddress + 0xBF, value); } }
        public int Pos_FB { get { return Emulator.ReadInt(_baseAddress + 0x16D); } set { Emulator.WriteInt(_baseAddress + 0x16D, value); } }
        public int Pos_UD { get { return Emulator.ReadInt(_baseAddress + 0x171); } set { Emulator.WriteInt(_baseAddress + 0x171, value); } }
        public int Pos_RL { get { return Emulator.ReadInt(_baseAddress + 0x175); } set { Emulator.WriteInt(_baseAddress + 0x175, value); } }
        public byte Rotation { get { return Emulator.ReadByte(_baseAddress + 0x1B7); } set { Emulator.WriteByte(_baseAddress + 0x1B7, value); } }
        public byte PhysicalShield { get { return (byte) (Emulator.ReadByte(_pShieldMShieldSigStone) & 3); } set { Emulator.WriteByte(_pShieldMShieldSigStone, (byte) (Emulator.ReadByte(_pShieldMShieldSigStone) | Math.Min(value, (byte) 3))); } }
        public byte MagicalShield { get { return (byte) ((Emulator.ReadByte(_pShieldMShieldSigStone) >> 2) & 3); } set { Emulator.WriteByte(_pShieldMShieldSigStone, (byte) (Emulator.ReadByte(_pShieldMShieldSigStone) | (Math.Min(value, (byte) 3) << 2))); } }
        public byte StatusEffect { get { return Emulator.ReadByte(_statusAddr); } set { Emulator.WriteByte(_statusAddr, value); } }
        public byte StatusTurns { get { return Emulator.ReadByte(_statusAddr + 0x1); } set { Emulator.WriteByte(_statusAddr + 0x1, value); } }
        public byte DragoonSpellID { get { return Emulator.ReadByte(_dragoonSpellsAddr); } set { Emulator.WriteByte(_dragoonSpellsAddr, value); } }
        public Collections.Byte DragoonSpell { get; private set; }
        public byte AdditionSpecial { get { return Emulator.ReadByte(_additionSpecial); } set { Emulator.WriteByte(_additionSpecial, value); } }

        internal BattleEntity(uint point, int slot, int position) {
            _baseAddress =  (int) (point - (slot * 0x388) + 0x108);
            _pShieldMShieldSigStone = 0x6E3B4 + position * 0x20; // Which one from constants is this??
            _statusAddr = 0x6E71C + position * 0x4; // Might be tied to the previous one
            _dragoonSpellsAddr = Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9; // Should this be in character???
            DragoonSpell = new Collections.Byte(_dragoonSpellsAddr + 1, 1, 8);
            _additionSpecial = Emulator.GetAddress("WARGOD") + slot * 0x4;
        }
    }
}
