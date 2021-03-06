using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.MemoryController {
    public class CharacterAddress : BattleEntityAddress {
        long _menu;
        long _lv;
        long _dlv;
        long _dragoon;
        long _mp;
        long _maxMP;
        long _sp;
        long _aHit;
        long _mHit;
        long _onHitStatus;
        long _onHitStatusChance;
        long _dat;
        long _dmat;
        long _ddf;
        long _dmdf;
        long _weapon;
        long _helmet;
        long _armor;
        long _shoes;
        long _accessory;
        long _addDmgMulti;
        long _addSpMulti;
        long _spMulti;
        long _spPHit;
        long _mpPHit;
        long _spMHit;
        long _mpMHit;
        long _hpRegen;
        long _mpRegen;
        long _spRegen;
        long _revive;
        long _weaponElement;
        long _image;
        long _detransform1;
        long _detransform2;
        long _aHitIncrease;
        long _aHitIncreaseTurn;
        long _mHitIncrease;
        long _mHitIncreaseTurn;
        long _spPHitIncrease;
        long _spPHitIncreaseTurn;
        long _mpPHitIncrease;
        long _mpPHitIncreaseTurn;
        long _spMHitIncrease;
        long _spMHitIncreaseTurn;
        long _mpMHitIncrease;
        long _mpMHitIncreaseTurn;

        public byte Menu { get { return Emulator.ReadByte(_menu); } set { Emulator.WriteByte(_menu, value); } }
        public byte LV { get { return Emulator.ReadByte(_lv); } set { Emulator.WriteByte(_lv, value); } }
        public byte DLV { get { return Emulator.ReadByte(_dlv); } set { Emulator.WriteByte(_dlv, value); } }
        public byte Dragoon { get { return Emulator.ReadByte(_dragoon); } set { Emulator.WriteByte(_dragoon, value); } }
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
        public byte On_Hit_Status_Chance { get { return Emulator.ReadByte(_onHitStatusChance); } set { Emulator.WriteByte(_onHitStatusChance, value); } }
        public ushort Add_DMG_Multi { get { return Emulator.ReadUShort(_addDmgMulti); } set { Emulator.WriteUShort(_addDmgMulti, value); } }
        public ushort Add_SP_Multi { get { return Emulator.ReadUShort(_addSpMulti); } set { Emulator.WriteUShort(_addSpMulti, value); } }
        public short SP_Multi { get { return Emulator.ReadShort(_spMulti); } set { Emulator.WriteShort(_spMulti, value); } }
        public byte SP_P_Hit { get { return Emulator.ReadByte(_spPHit); } set { Emulator.WriteByte(_spPHit, value); } }
        public byte MP_P_Hit { get { return Emulator.ReadByte(_mpPHit); } set { Emulator.WriteByte(_mpPHit, value); } }
        public byte SP_M_Hit { get { return Emulator.ReadByte(_spMHit); } set { Emulator.WriteByte(_spMHit, value); } } 
        public byte MP_M_Hit { get { return Emulator.ReadByte(_mpMHit); } set { Emulator.WriteByte(_mpMHit, value); } }
        public short HP_Regen { get { return Emulator.ReadShort(_hpRegen); } set { Emulator.WriteShort(_hpRegen, value); } }
        public short MP_Regen { get { return Emulator.ReadShort(_mpRegen); } set { Emulator.WriteShort(_mpRegen, value); } }
        public short SP_Regen { get { return Emulator.ReadShort(_spRegen); } set { Emulator.WriteShort(_spRegen, value); } }
        public byte Revive { get { return Emulator.ReadByte(_revive); } set { Emulator.WriteByte(_revive, value); } }
        public byte Weapon_Element { get { return Emulator.ReadByte(_weaponElement); } set { Emulator.WriteByte(_weaponElement, value); } }
        public ushort DAT { get { return Emulator.ReadUShort(_dat); } set { Emulator.WriteUShort(_dat, value); } }
        public ushort DMAT { get { return Emulator.ReadUShort(_dmat); }set { Emulator.WriteUShort(_dmat, value); } }
        public ushort DDF { get { return Emulator.ReadUShort(_ddf); } set { Emulator.WriteUShort(_ddf, value); } }
        public ushort DMDF { get { return Emulator.ReadUShort(_dmdf); } set { Emulator.WriteUShort(_dmdf, value); } }
        public byte Image { get { return Emulator.ReadByte(_image); } set { Emulator.WriteByte(_image, value); } }
        public ushort Detransform1 { get { return Emulator.ReadUShort(_detransform1); } set { Emulator.WriteUShort(_detransform1, value); } }
        public byte Detransform2 { get { return Emulator.ReadByte(_detransform2); } set { Emulator.WriteByte(_detransform2, value); } }
        public sbyte A_HIT_Increase { get { return Emulator.ReadSByte(_aHitIncrease); } set { Emulator.WriteByte(_aHitIncrease, value); } }
        public byte A_HIT_Increase_Turn { get { return Emulator.ReadByte(_aHitIncreaseTurn); } set { Emulator.WriteByte(_aHitIncreaseTurn, value); } }
        public sbyte M_HIT_Increase { get { return Emulator.ReadSByte(_mHitIncrease); } set { Emulator.WriteByte(_mHitIncrease, value); } }
        public byte M_HIT_Increase_Turn { get { return Emulator.ReadByte(_mHitIncreaseTurn); } set { Emulator.WriteByte(_mHitIncreaseTurn, value); } }
        public sbyte SP_P_Hit_Increase { get { return Emulator.ReadSByte(_spPHitIncrease); } set { Emulator.WriteByte(_spPHitIncrease, value); } }
        public byte SP_P_Hit_Increase_Turn { get { return Emulator.ReadByte(_spPHitIncreaseTurn); } set { Emulator.WriteByte(_spPHitIncreaseTurn, value); } }
        public sbyte MP_P_Hit_Increase { get { return Emulator.ReadSByte(_mpPHitIncrease); } set { Emulator.WriteByte(_mpPHitIncrease, value); } }
        public byte MP_P_Hit_Increase_Turn { get { return Emulator.ReadByte(_mpPHitIncreaseTurn); } set { Emulator.WriteByte(_mpPHitIncreaseTurn, value); } }
        public sbyte SP_M_Hit_Increase { get { return Emulator.ReadSByte(_spMHitIncrease); } set { Emulator.WriteByte(_spMHitIncrease, value); } }
        public byte SP_M_Hit_Increase_Turn { get { return Emulator.ReadByte(_spMHitIncreaseTurn); } set { Emulator.WriteByte(_spMHitIncreaseTurn, value); } }
        public sbyte MP_M_Hit_Increase { get { return Emulator.ReadSByte(_mpMHitIncrease); } set { Emulator.WriteByte(_mpMHitIncrease, value); } }
        public byte MP_M_Hit_Increase_Turn { get { return Emulator.ReadByte(_mpMHitIncreaseTurn); } set { Emulator.WriteByte(_mpMHitIncreaseTurn, value); } }


        public CharacterAddress(long c_point, int slot, int position) : base(c_point, slot, position) {
            long curr_point = c_point - slot * 0x388;
            _menu = curr_point - 0xA4;
            _lv = curr_point - 0x4 ;
            _dlv = curr_point - 0x2;
            _dragoon = curr_point + 0x7;
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
            _addDmgMulti = curr_point + 0x114;
            _addSpMulti = curr_point + 0x112;
            _spMulti = curr_point + 0x120;
            _spPHit = curr_point + 0x122;
            _mpPHit = curr_point + 0x124;
            _spMHit = curr_point + 0x126;
            _mpMHit = curr_point + 0x128;
            _hpRegen = curr_point + 0x12C;
            _mpRegen = curr_point + 0x12E;
            _spRegen = curr_point + 0x130;
            _revive = curr_point + 0x132;
            _weaponElement = curr_point + 0x14;
            _dat = curr_point + 0xA4;
            _dmat = curr_point + 0xA6;
            _ddf = curr_point + 0xA8;
            _dmdf = curr_point + 0xAA;
            _image = curr_point + 0x26A;
            _detransform1 = curr_point - 0xF0;
            _detransform2 = curr_point - 0xEE;
            _aHitIncrease = curr_point + 0xB4;
            _aHitIncreaseTurn = curr_point + 0xB5;
            _mHitIncrease = curr_point + 0xB6;
            _mHitIncreaseTurn = curr_point + 0xB7;
            _spPHitIncrease = curr_point + 0xC4;
            _spPHitIncreaseTurn = curr_point + 0xC5;
            _mpPHitIncrease = curr_point + 0xC6;
            _mpPHitIncreaseTurn = curr_point + 0xC7;
            _spMHitIncrease = curr_point + 0xC8;
            _spMHitIncreaseTurn = curr_point + 0xC9;
            _mpMHitIncrease = curr_point + 0xCA;
            _mpMHitIncreaseTurn = curr_point + 0xCB;
        }
    }
}
