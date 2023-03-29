using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Core.Memory {
    public sealed class SecondaryCharacterTable {
        private static readonly byte[] _additionCounts = new byte[] { 7, 5, 0, 4, 6, 5, 5, 3, 0 };

        private readonly int _baseAddress;
        public uint Expirience { get { return Emulator.DirectAccess.ReadUInt(_baseAddress); } set { Emulator.DirectAccess.WriteUInt(_baseAddress, value); } }
        public ushort HP { get { return Emulator.DirectAccess.ReadUShort(_baseAddress + 0x4); } set { Emulator.DirectAccess.WriteUShort(_baseAddress + 0x4, value); } }
        public ushort MP { get { return Emulator.DirectAccess.ReadUShort(_baseAddress + 0x6); } set { Emulator.DirectAccess.WriteUShort(_baseAddress + 0x6, value); } }
        public ushort SP { get { return Emulator.DirectAccess.ReadUShort(_baseAddress + 0x8); } set { Emulator.DirectAccess.WriteUShort(_baseAddress + 0x8, value); } }
        public byte Level { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0xE); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0xE, value); } }
        public byte DragoonLevel { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0xF); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0xF, value); } }
        public Collections.IAddress<byte> AdditionLevel { get; private set; }
        public Collections.IAddress<byte> AdditionCount { get; private set; }
        public byte P_Half { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x4A); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x4A, value); } }
        public ushort SP_Multi { get { return Emulator.DirectAccess.ReadUShort(_baseAddress + 0x4C); } set { Emulator.DirectAccess.WriteUShort(_baseAddress + 0x4C, value); } }
        public byte SP_P_Hit { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x4E); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x4E, value); } }
        public byte MP_P_Hit { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x50); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x50, value); } }
        public byte SP_M_Hit { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x52); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x52, value); } }
        public byte MP_M_Hit { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x54); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x54, value); } }
        public short HP_Regen { get { return Emulator.DirectAccess.ReadShort(_baseAddress + 0x58); } set { Emulator.DirectAccess.WriteShort(_baseAddress + 0x58, value); } }
        public short MP_Regen { get { return Emulator.DirectAccess.ReadShort(_baseAddress + 0x5A); } set { Emulator.DirectAccess.WriteShort(_baseAddress + 0x5A, value); } }
        public short SP_Regen { get { return Emulator.DirectAccess.ReadShort(_baseAddress + 0x5C); } set { Emulator.DirectAccess.WriteShort(_baseAddress + 0x5C, value); } }
        public byte Revive { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x5E); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x5E, value); } }
        public byte M_Half { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x60); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x60, value); } }
        public byte HP_Multi { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x62); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x62, value); } }
        public byte MP_Multi { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x64); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x64, value); } }
        public ushort MaxHP { get { return Emulator.DirectAccess.ReadUShort(_baseAddress + 0x66); } set { Emulator.DirectAccess.WriteUShort(_baseAddress + 0x66, value); } }
        public byte BodySPD { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x69); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x69, value); } }
        public byte BodyAT { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x6A); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x6A, value); } }
        public byte BodyMAT { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x6B); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x6B, value); } }
        public byte BodyDF { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x6C); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x6C, value); } }
        public byte BodyMDF { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x6D); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x6D, value); } }
        public ushort MaxMP { get { return Emulator.DirectAccess.ReadUShort(_baseAddress + 0x6E); } set { Emulator.DirectAccess.WriteUShort(_baseAddress + 0x6E, value); } }
        public byte DAT { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x72); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x72, value); } }
        public byte DMAT { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x73); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x73, value); } }
        public byte DDF { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x72); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x72, value); } }
        public byte DMDF { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x73); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x73, value); } }
        public byte SpecialEffect { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x76); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x76, value); } } // Old Death_Res
        public byte WeaponElement { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x7A); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x7A, value); } }
        public byte E_Half { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x7C); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x7C, value); } }
        public byte E_Immune { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x7D); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x7D, value); } }
        public byte StatusResist { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x7E); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x7E, value); } }
        public ushort EquipSPD { get { return Emulator.DirectAccess.ReadUShort(_baseAddress + 0x86); } set { Emulator.DirectAccess.WriteUShort(_baseAddress + 0x86, value); } }
        public ushort EquipAT { get { return Emulator.DirectAccess.ReadUShort(_baseAddress + 0x88); } set { Emulator.DirectAccess.WriteUShort(_baseAddress + 0x88, value); } }
        public ushort EquipMAT { get { return Emulator.DirectAccess.ReadUShort(_baseAddress + 0x8A); } set { Emulator.DirectAccess.WriteUShort(_baseAddress + 0x8A, value); } }
        public ushort EquipDF { get { return Emulator.DirectAccess.ReadUShort(_baseAddress + 0x8C); } set { Emulator.DirectAccess.WriteUShort(_baseAddress + 0x8C, value); } }
        public ushort EquipMDF { get { return Emulator.DirectAccess.ReadUShort(_baseAddress + 0x8E); } set { Emulator.DirectAccess.WriteUShort(_baseAddress + 0x8E, value); } }
        public short EquipA_HIT { get { return Emulator.DirectAccess.ReadShort(_baseAddress + 0x90); } set { Emulator.DirectAccess.WriteShort(_baseAddress + 0x90, value); } }
        public short EquipM_HIT { get { return Emulator.DirectAccess.ReadShort(_baseAddress + 0x92); } set { Emulator.DirectAccess.WriteShort(_baseAddress + 0x92, value); } }
        public short EquipA_AV { get { return Emulator.DirectAccess.ReadShort(_baseAddress + 0x94); } set { Emulator.DirectAccess.WriteShort(_baseAddress + 0x94, value); } }
        public short EquipM_AV { get { return Emulator.DirectAccess.ReadShort(_baseAddress + 0x96); } set { Emulator.DirectAccess.WriteShort(_baseAddress + 0x96, value); } }
        public byte OnHitStatus { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x98); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x98, value); } }
        public byte OnHitStatusChance { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x9B); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x9B, value); } }

        internal SecondaryCharacterTable(int baseAddress, int character) {
            _baseAddress = baseAddress + (character * 0xA0);
            AdditionLevel = Collections.Factory.Create<byte>(_baseAddress + 0x36, 1, _additionCounts[character]);
            AdditionCount = Collections.Factory.Create<byte>(_baseAddress + 0x3E, 1, _additionCounts[character]);
        }
    }
}
