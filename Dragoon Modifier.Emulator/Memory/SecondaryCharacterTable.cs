using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Emulator.Memory {
    public class SecondaryCharacterTable {
        private static readonly byte[] _additionCounts = new byte[] { 7, 5, 0, 4, 6, 5, 5, 3, 0 };

        private readonly int _baseAddress;

        private readonly IEmulator _emulator;

        public uint Expirience { get { return _emulator.ReadUInt(_baseAddress); } set { _emulator.WriteUInt(_baseAddress, value); } }
        public ushort HP { get { return _emulator.ReadUShort(_baseAddress + 0x4); } set { _emulator.WriteUShort(_baseAddress + 0x4, value); } }
        public ushort MP { get { return _emulator.ReadUShort(_baseAddress + 0x6); } set { _emulator.WriteUShort(_baseAddress + 0x6, value); } }
        public ushort SP { get { return _emulator.ReadUShort(_baseAddress + 0x8); } set { _emulator.WriteUShort(_baseAddress + 0x8, value); } }
        public byte Level { get { return _emulator.ReadByte(_baseAddress + 0xE); } set { _emulator.WriteByte(_baseAddress + 0xE, value); } }
        public byte DragoonLevel { get { return _emulator.ReadByte(_baseAddress + 0xF); } set { _emulator.WriteByte(_baseAddress + 0xF, value); } }
        public Collections.IAddress<byte> AdditionLevel { get; private set; }
        public Collections.IAddress<byte> AdditionCount { get; private set; }
        public byte P_Half { get { return _emulator.ReadByte(_baseAddress + 0x4A); } set { _emulator.WriteByte(_baseAddress + 0x4A, value); } }
        public short SP_Multi { get { return _emulator.ReadShort(_baseAddress + 0x4C); } set { _emulator.WriteShort(_baseAddress + 0x4C, value); } }
        public short SP_P_Hit { get { return _emulator.ReadShort(_baseAddress + 0x4E); } set { _emulator.WriteShort(_baseAddress + 0x4E, value); } }
        public short MP_P_Hit { get { return _emulator.ReadShort(_baseAddress + 0x50); } set { _emulator.WriteShort(_baseAddress + 0x50, value); } }
        public short SP_M_Hit { get { return _emulator.ReadShort(_baseAddress + 0x52); } set { _emulator.WriteShort(_baseAddress + 0x52, value); } }
        public short MP_M_Hit { get { return _emulator.ReadShort(_baseAddress + 0x54); } set { _emulator.WriteShort(_baseAddress + 0x54, value); } }
        public short HP_Regen { get { return _emulator.ReadShort(_baseAddress + 0x58); } set { _emulator.WriteShort(_baseAddress + 0x58, value); } }
        public short MP_Regen { get { return _emulator.ReadShort(_baseAddress + 0x5A); } set { _emulator.WriteShort(_baseAddress + 0x5A, value); } }
        public short SP_Regen { get { return _emulator.ReadShort(_baseAddress + 0x5C); } set { _emulator.WriteShort(_baseAddress + 0x5C, value); } }
        public byte Revive { get { return _emulator.ReadByte(_baseAddress + 0x5E); } set { _emulator.WriteByte(_baseAddress + 0x5E, value); } }
        public byte M_Half { get { return _emulator.ReadByte(_baseAddress + 0x60); } set { _emulator.WriteByte(_baseAddress + 0x60, value); } }
        public byte HP_Multi { get { return _emulator.ReadByte(_baseAddress + 0x62); } set { _emulator.WriteByte(_baseAddress + 0x62, value); } }
        public byte MP_Multi { get { return _emulator.ReadByte(_baseAddress + 0x64); } set { _emulator.WriteByte(_baseAddress + 0x64, value); } }
        public ushort MaxHP { get { return _emulator.ReadUShort(_baseAddress + 0x66); } set { _emulator.WriteUShort(_baseAddress + 0x66, value); } }
        public byte BodySPD { get { return _emulator.ReadByte(_baseAddress + 0x69); } set { _emulator.WriteByte(_baseAddress + 0x69, value); } }
        public byte BodyAT { get { return _emulator.ReadByte(_baseAddress + 0x6A); } set { _emulator.WriteByte(_baseAddress + 0x6A, value); } }
        public byte BodyMAT { get { return _emulator.ReadByte(_baseAddress + 0x6B); } set { _emulator.WriteByte(_baseAddress + 0x6B, value); } }
        public byte BodyDF { get { return _emulator.ReadByte(_baseAddress + 0x6C); } set { _emulator.WriteByte(_baseAddress + 0x6C, value); } }
        public byte BodyMDF { get { return _emulator.ReadByte(_baseAddress + 0x6D); } set { _emulator.WriteByte(_baseAddress + 0x6D, value); } }
        public ushort MaxMP { get { return _emulator.ReadUShort(_baseAddress + 0x6E); } set { _emulator.WriteUShort(_baseAddress + 0x6E, value); } }
        public byte DAT { get { return _emulator.ReadByte(_baseAddress + 0x72); } set { _emulator.WriteByte(_baseAddress + 0x72, value); } }
        public byte DMAT { get { return _emulator.ReadByte(_baseAddress + 0x73); } set { _emulator.WriteByte(_baseAddress + 0x73, value); } }
        public byte DDF { get { return _emulator.ReadByte(_baseAddress + 0x72); } set { _emulator.WriteByte(_baseAddress + 0x72, value); } }
        public byte DMDF { get { return _emulator.ReadByte(_baseAddress + 0x73); } set { _emulator.WriteByte(_baseAddress + 0x73, value); } }
        public byte SpecialEffect { get { return _emulator.ReadByte(_baseAddress + 0x76); } set { _emulator.WriteByte(_baseAddress + 0x76, value); } } // Old Death_Res
        public byte WeaponElement { get { return _emulator.ReadByte(_baseAddress + 0x7A); } set { _emulator.WriteByte(_baseAddress + 0x7A, value); } }
        public byte E_Half { get { return _emulator.ReadByte(_baseAddress + 0x7C); } set { _emulator.WriteByte(_baseAddress + 0x7C, value); } }
        public byte E_Immune { get { return _emulator.ReadByte(_baseAddress + 0x7D); } set { _emulator.WriteByte(_baseAddress + 0x7D, value); } }
        public byte StatusResist { get { return _emulator.ReadByte(_baseAddress + 0x7E); } set { _emulator.WriteByte(_baseAddress + 0x7E, value); } }
        public ushort EquipSPD { get { return _emulator.ReadUShort(_baseAddress + 0x86); } set { _emulator.WriteUShort(_baseAddress + 0x86, value); } }
        public ushort EquipAT { get { return _emulator.ReadUShort(_baseAddress + 0x88); } set { _emulator.WriteUShort(_baseAddress + 0x88, value); } }
        public ushort EquipMAT { get { return _emulator.ReadUShort(_baseAddress + 0x8A); } set { _emulator.WriteUShort(_baseAddress + 0x8A, value); } }
        public ushort EquipDF { get { return _emulator.ReadUShort(_baseAddress + 0x8C); } set { _emulator.WriteUShort(_baseAddress + 0x8C, value); } }
        public ushort EquipMDF { get { return _emulator.ReadUShort(_baseAddress + 0x8E); } set { _emulator.WriteUShort(_baseAddress + 0x8E, value); } }
        public short EquipA_HIT { get { return _emulator.ReadShort(_baseAddress + 0x90); } set { _emulator.WriteShort(_baseAddress + 0x90, value); } }
        public short EquipM_HIT { get { return _emulator.ReadShort(_baseAddress + 0x92); } set { _emulator.WriteShort(_baseAddress + 0x92, value); } }
        public short EquipA_AV { get { return _emulator.ReadShort(_baseAddress + 0x94); } set { _emulator.WriteShort(_baseAddress + 0x94, value); } }
        public short EquipM_AV { get { return _emulator.ReadShort(_baseAddress + 0x96); } set { _emulator.WriteShort(_baseAddress + 0x96, value); } }
        public byte OnHitStatus { get { return _emulator.ReadByte(_baseAddress + 0x98); } set { _emulator.WriteByte(_baseAddress + 0x98, value); } }
        public byte OnHitStatusChance { get { return _emulator.ReadByte(_baseAddress + 0x9B); } set { _emulator.WriteByte(_baseAddress + 0x9B, value); } }

        internal SecondaryCharacterTable(IEmulator emulator, int baseAddress, int character) {
            _emulator = emulator;
            _baseAddress = baseAddress + (character * 0xA0);
            AdditionLevel = Factory.AddressCollection<byte>(emulator, _baseAddress + 0x36, 1, _additionCounts[character]);
            AdditionCount = Factory.AddressCollection<byte>(emulator, _baseAddress + 0x3E, 1, _additionCounts[character]);
        }
    }
}
