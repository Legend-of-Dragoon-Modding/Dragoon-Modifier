using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.MemoryController {
    class CharacterAddress : BattleAddress {
        long _lv;
        long _dlv;
        long _mp;
        long _maxMP;
        long _sp;
        long _aHit;
        long _mHit;
        long _onHitStatus;
        long _onHitStatusChance;
        long _weapon;
        long _helmet;
        long _armor;
        long _shoes;
        long _accessory;
        long _addDmgMulti;
        long _addSpMulti;

        public byte LV { get { return Emulator.ReadByte(_lv); } set { Emulator.WriteByte(_lv, value); } }
        public byte DLV { get { return Emulator.ReadByte(_dlv); } set { Emulator.WriteByte(_dlv, value); } }
        public ushort MP { get { return Emulator.ReadUShort(_mp); } set { Emulator.WriteUShort(_mp, value); } }
        public ushort Max_MP { get { return Emulator.ReadUShort(_maxMP); } set { Emulator.WriteUShort(_maxMP, value); } }
        public ushort SP { get { return Emulator.ReadUShort(_sp); } set { Emulator.WriteUShort(_sp, value); } }
        public byte Weapon { get { return Emulator.ReadByte(_weapon); } set { Emulator.WriteByte(_weapon, value); } }
        public byte Helmet { get { return Emulator.ReadByte(_helmet); } set { Emulator.WriteByte(_helmet, value); } }
        public byte Armor { get { return Emulator.ReadByte(_armor);  } set { Emulator.WriteByte(_armor, value); } }
        public byte Shoes { get { return Emulator.ReadByte(_shoes); } set { Emulator.WriteByte(_shoes, value); } }
        public byte Accessory { get { return Emulator.ReadByte(_accessory); } set { Emulator.WriteByte(_accessory, value); } }
        public byte A_HIT { get { return Emulator.ReadByte(_aHit); } set { Emulator.WriteByte(_aHit, value); } }
        public byte M_HIT { get { return Emulator.ReadByte(_mHit); } set { Emulator.WriteByte(_mHit, value); } }
        public byte On_Hit_Status { get { return Emulator.ReadByte(_onHitStatus); } set { Emulator.WriteByte(_onHitStatus, value); } }
        public byte On_Hit_Status_Chance { get { return Emulator.ReadByte(_onHitStatusChance); }set { Emulator.WriteByte(_onHitStatusChance, value); } }
        public ushort Add_DMG_Multi { get { return Emulator.ReadUShort(_addDmgMulti); } set { Emulator.WriteUShort(_addDmgMulti, value); } }
        public ushort Add_SP_Multi { get { return Emulator.ReadUShort(_addSpMulti); } set { Emulator.WriteUShort(_addSpMulti, value); } }


        CharacterAddress(long c_point, int position) : base(c_point, position) {
            long curr_point = c_point - position * 0x388;
            _lv = curr_point - 0x4 ;
            _dlv = curr_point - 0x2;
            _mp = curr_point + 0x4;
            _maxMP = curr_point + 0xA;
            _sp = curr_point + 0x2;
            _aHit = curr_point + 0x34;
            _mHit = curr_point + 0x36;
            _onHitStatus = curr_point + 0x42;
            _onHitStatusChance = curr_point + 0x3C;
            _weapon = curr_point + 0x116;
            _helmet = curr_point + 0x118;
            _armor = curr_point + 0x11A;
            _shoes = curr_point + 0x11C;
            _accessory = curr_point + 0x11E;
        }

    }
}
