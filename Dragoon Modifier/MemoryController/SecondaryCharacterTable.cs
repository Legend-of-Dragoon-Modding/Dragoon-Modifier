using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.MemoryController {
    public class SecondaryCharacterTable {
        static readonly byte[] addCounts = new byte[] { 7, 5, 0, 4, 6, 5, 5, 3, 0 };

        int _baseAddress;
        ByteCollection _additionCount;
        ByteCollection _additionLevel;

        public uint Expirience { get { return Emulator.ReadUInt(_baseAddress); } set { Emulator.WriteUInt(_baseAddress, value); } }
        public ushort HP { get { return Emulator.ReadUShort(_baseAddress + 0x4); } set { Emulator.WriteUShort(_baseAddress + 0x4, value); } }
        public ushort MP { get { return Emulator.ReadUShort(_baseAddress + 0x6); } set { Emulator.WriteUShort(_baseAddress + 0x6, value); } }
        public ushort SP { get { return Emulator.ReadUShort(_baseAddress + 0x8); } set { Emulator.WriteUShort(_baseAddress + 0x8, value); } }
        public byte Level { get { return Emulator.ReadByte(_baseAddress + 0xE); } set { Emulator.WriteByte(_baseAddress + 0xE, value); } }
        public byte DragoonLevel { get { return Emulator.ReadByte(_baseAddress + 0xF); } set { Emulator.WriteByte(_baseAddress + 0xF, value); } }
        public ByteCollection AdditionLevel { get { return _additionLevel; } }
        public ByteCollection AdditionCount { get { return _additionCount; } }
        public byte P_Half { get { return Emulator.ReadByte(_baseAddress + 0x4A); } set { Emulator.WriteByte(_baseAddress + 0x4A, value); } }
        public short SP_Multi { get { return Emulator.ReadShort(_baseAddress + 0x4C); } set { Emulator.WriteShort(_baseAddress + 0x4C, value); } }
        public short SP_P_Hit { get { return Emulator.ReadShort(_baseAddress + 0x4E); } set { Emulator.WriteShort(_baseAddress + 0x4E, value); } }
        public short MP_P_Hit { get { return Emulator.ReadShort(_baseAddress + 0x50); } set { Emulator.WriteShort(_baseAddress + 0x50, value); } }
        public short SP_M_Hit { get { return Emulator.ReadShort(_baseAddress + 0x52); } set { Emulator.WriteShort(_baseAddress + 0x52, value); } }
        public short MP_M_Hit { get { return Emulator.ReadShort(_baseAddress + 0x54); } set { Emulator.WriteShort(_baseAddress + 0x54, value); } }
        public short HP_Regen { get { return Emulator.ReadShort(_baseAddress + 0x58); } set { Emulator.WriteShort(_baseAddress + 0x58, value); } }
        public short MP_Regen { get { return Emulator.ReadShort(_baseAddress + 0x5A); } set { Emulator.WriteShort(_baseAddress + 0x5A, value); } }
        public short SP_Regen { get { return Emulator.ReadShort(_baseAddress + 0x5C); } set { Emulator.WriteShort(_baseAddress + 0x5C, value); } }
        public byte Revive { get { return Emulator.ReadByte(_baseAddress + 0x5E); } set { Emulator.WriteByte(_baseAddress + 0x5E, value); } }
        public byte M_Half { get { return Emulator.ReadByte(_baseAddress + 0x60); } set { Emulator.WriteByte(_baseAddress + 0x60, value); } }
        public byte HP_Multi { get { return Emulator.ReadByte(_baseAddress + 0x62); } set { Emulator.WriteByte(_baseAddress + 0x62, value); } }
        public byte MP_Multi { get { return Emulator.ReadByte(_baseAddress + 0x64); } set { Emulator.WriteByte(_baseAddress + 0x64, value); } }
        public ushort Max_HP { get { return Emulator.ReadUShort(_baseAddress + 0x66); } set { Emulator.WriteUShort(_baseAddress + 0x66, value); } }
        public byte BodySPD { get { return Emulator.ReadByte(_baseAddress + 0x69); } set { Emulator.WriteByte(_baseAddress + 0x69, value); } }
        public byte BodyAT { get { return Emulator.ReadByte(_baseAddress + 0x6A); } set { Emulator.WriteByte(_baseAddress + 0x6A, value); } }
        public byte BodyMAT { get { return Emulator.ReadByte(_baseAddress + 0x6B); } set { Emulator.WriteByte(_baseAddress + 0x6B, value); } }
        public byte BodyDF { get { return Emulator.ReadByte(_baseAddress + 0x6C); } set { Emulator.WriteByte(_baseAddress + 0x6C, value); } }
        public byte BodyMDF { get { return Emulator.ReadByte(_baseAddress + 0x6D); } set { Emulator.WriteByte(_baseAddress + 0x6D, value); } }
        public ushort Max_MP { get { return Emulator.ReadUShort(_baseAddress + 0x6E); } set { Emulator.WriteUShort(_baseAddress + 0x6E, value); } }
        public byte DAT { get { return Emulator.ReadByte(_baseAddress + 0x72); } set { Emulator.WriteByte(_baseAddress + 0x72, value); } }
        public byte DMAT { get { return Emulator.ReadByte(_baseAddress + 0x73); } set { Emulator.WriteByte(_baseAddress + 0x73, value); } }
        public byte DDF { get { return Emulator.ReadByte(_baseAddress + 0x72); } set { Emulator.WriteByte(_baseAddress + 0x72, value); } }
        public byte DMDF { get { return Emulator.ReadByte(_baseAddress + 0x73); } set { Emulator.WriteByte(_baseAddress + 0x73, value); } }
        public byte Special_Effect { get { return Emulator.ReadByte(_baseAddress + 0x76); } set { Emulator.WriteByte(_baseAddress + 0x76, value); } } // Old Death_Res
        public byte WeaponElement { get { return Emulator.ReadByte(_baseAddress + 0x7A); } set { Emulator.WriteByte(_baseAddress + 0x7A, value); } }
        public byte E_Half { get { return Emulator.ReadByte(_baseAddress + 0x7C); } set { Emulator.WriteByte(_baseAddress + 0x7C, value); } }
        public byte E_Immune { get { return Emulator.ReadByte(_baseAddress + 0x7D); } set { Emulator.WriteByte(_baseAddress + 0x7D, value); } }
        public byte StatusResist { get { return Emulator.ReadByte(_baseAddress + 0x7E); } set { Emulator.WriteByte(_baseAddress + 0x7E, value); } }
        public ushort EquipSPD { get { return Emulator.ReadUShort(_baseAddress + 0x86); } set { Emulator.WriteUShort(_baseAddress + 0x86, value); } }
        public ushort EquipAT { get { return Emulator.ReadUShort(_baseAddress + 0x88); } set { Emulator.WriteUShort(_baseAddress + 0x88, value); } }
        public ushort EquipMAT { get { return Emulator.ReadUShort(_baseAddress + 0x8A); } set { Emulator.WriteUShort(_baseAddress + 0x8A, value); } }
        public ushort EquipDF { get { return Emulator.ReadUShort(_baseAddress + 0x8C); } set { Emulator.WriteUShort(_baseAddress + 0x8C, value); } }
        public ushort EquipMDF { get { return Emulator.ReadUShort(_baseAddress + 0x8E); } set { Emulator.WriteUShort(_baseAddress + 0x8E, value); } }
        public short EquipA_HIT { get { return Emulator.ReadShort(_baseAddress + 0x90); } set { Emulator.WriteShort(_baseAddress + 0x90, value); } }
        public short EquipM_HIT { get { return Emulator.ReadShort(_baseAddress + 0x92); } set { Emulator.WriteShort(_baseAddress + 0x92, value); } }
        public short EquipA_AV { get { return Emulator.ReadShort(_baseAddress + 0x94); } set { Emulator.WriteShort(_baseAddress + 0x94, value); } }
        public short EquipM_AV { get { return Emulator.ReadShort(_baseAddress + 0x96); } set { Emulator.WriteShort(_baseAddress + 0x96, value); } }
        public byte On_Hit_Status { get { return Emulator.ReadByte(_baseAddress + 0x98); } set { Emulator.WriteByte(_baseAddress + 0x98, value); } }
        public byte On_Hit_Status_Chance { get { return Emulator.ReadByte(_baseAddress + 0x9B); } set { Emulator.WriteByte(_baseAddress + 0x9B, value); } }

        public SecondaryCharacterTable(int baseAddress, int character) {
            _baseAddress = baseAddress + (character * 0xA0);
            _additionLevel = new ByteCollection(_baseAddress + 0x36, 1, addCounts[character]);
            _additionCount = new ByteCollection(_baseAddress + 0x3E, 1, addCounts[character]);
        }
    }
}
