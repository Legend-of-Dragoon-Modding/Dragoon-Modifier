using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.MemoryController {
    public class BattleEntityAddress {
        long _action;
        long _spellCast;
        long _hp;
        long _maxHP;
        long _status;
        long _statusRes;
        long _specialEffect; // Old Death_Res
        long _guard;
        long _at;
        long _ogAT;
        long _mat;
        long _ogMAT;
        long _df;
        long _ogDF;
        long _mdf;
        long _ogMDF;
        long _spd;
        long _ogSPD;
        long _turn;
        long _aAV;
        long _mAV;
        long _pImmune;
        long _mImmune;
        long _pHalf;
        long _mHalf;
        long _eImmune;
        long _eHalf;
        long _uniqueIndex;
        long _pwrAT;
        long _pwrATTurn;
        long _pwrMAT;
        long _pwrMATTurn;
        long _pwrDF;
        long _pwrDFTurn;
        long _pwrMDF;
        long _pwrMDFTurn;
        long _spdUpTurn;
        long _spdDownTurn;
        long _posFB;
        long _posUD;
        long _posRL;
        long _pImmuneIncrease;
        long _pImmuneIcreaseTurn;
        long _mImmuneIncrease;
        long _mImmuneIncreaseTurn;
        protected long _pShieldMshieldSigStone;

        public byte Action { get { return Emulator.ReadByte(_action); } set { Emulator.WriteByte(_action, value); } }
        public byte Spell_Cast { get { return Emulator.ReadByte(_spellCast); } set { Emulator.WriteByte(_spellCast, value); } }
        public ushort HP { get { return Emulator.ReadUShort(_hp); } set { Emulator.WriteUShort(_hp, value); } }
        public ushort Max_HP { get { return Emulator.ReadUShort(_maxHP); } set { Emulator.WriteUShort(_maxHP, value); } }
        public byte Status { get { return Emulator.ReadByte(_status); } set { Emulator.WriteByte(_status, value); } }
        public byte Special_Efect { get { return Emulator.ReadByte(_specialEffect); } set { Emulator.WriteByte(_specialEffect, value); } }
        public byte Guard { get { return Emulator.ReadByte(_guard); } set { Emulator.WriteByte(_guard, value); } }
        public ushort AT { get { return Emulator.ReadUShort(_at); } set { Emulator.WriteUShort(_at, value); } }
        public ushort OG_AT { get { return Emulator.ReadUShort(_ogAT); } set { Emulator.WriteUShort(_ogAT, value); } }
        public ushort MAT { get { return Emulator.ReadUShort(_mat); } set { Emulator.WriteUShort(_mat, value); } }
        public ushort OG_MAT { get { return Emulator.ReadUShort(_ogMAT); } set { Emulator.WriteUShort(_ogMAT, value); } }
        public ushort DF { get { return Emulator.ReadUShort(_df); } set { Emulator.WriteUShort(_df, value); } }
        public ushort OG_DF { get { return Emulator.ReadUShort(_ogDF); } set { Emulator.WriteUShort(_ogDF, value); } }
        public ushort MDF { get { return Emulator.ReadUShort(_mdf); } set { Emulator.WriteUShort(_mdf, value); } }
        public ushort OG_MDF { get { return Emulator.ReadUShort(_ogMDF); } set { Emulator.WriteUShort(_ogMDF, value); } }
        public ushort SPD { get { return Emulator.ReadUShort(_spd); } set { Emulator.WriteUShort(_spd, value); } }
        public ushort OG_SPD { get { return Emulator.ReadUShort(_ogSPD); } set { Emulator.WriteUShort(_ogSPD, value); } }
        public ushort Turn { get { return Emulator.ReadUShort(_turn); } set { Emulator.WriteUShort(_turn, value); } }
        public byte A_AV { get { return Emulator.ReadByte(_aAV); } set { Emulator.WriteByte(_aAV, value); } }
        public byte M_AV { get { return Emulator.ReadByte(_mAV); } set { Emulator.WriteByte(_mAV, value); } }
        public byte P_Immune { get { return Emulator.ReadByte(_pImmune); } set { Emulator.WriteByte(_pImmune, value); } }
        public byte M_Immune { get { return Emulator.ReadByte(_mImmune); } set { Emulator.WriteByte(_mImmune, value); } }
        public byte P_Half { get { return Emulator.ReadByte(_pHalf); } set { Emulator.WriteByte(_pHalf, value); } }
        public byte M_Half { get { return Emulator.ReadByte(_mHalf); } set { Emulator.WriteByte(_mHalf, value); } }
        public byte E_Immune { get { return Emulator.ReadByte(_eImmune); } set { Emulator.WriteByte(_eImmune, value); } }
        public byte E_Half { get { return Emulator.ReadByte(_eHalf); } set { Emulator.WriteByte(_eHalf, value); } }
        public byte Status_Res { get { return Emulator.ReadByte(_statusRes); } set { Emulator.WriteByte(_statusRes, value); } }
        public byte Unique_Index { get { return Emulator.ReadByte(_uniqueIndex); } set { Emulator.WriteByte(_uniqueIndex, value); } }
        public sbyte PWR_AT { get { return Emulator.ReadSByte(_pwrAT); } set { Emulator.WriteByte(_pwrAT, value); } }
        public byte PWR_AT_Turn { get { return Emulator.ReadByte(_pwrATTurn); } set { Emulator.WriteByte(_pwrAT, value); } }
        public sbyte PWR_MAT { get { return Emulator.ReadSByte(_pwrMAT); } set { Emulator.WriteByte(_pwrMAT, value); } }
        public byte PWR_MAT_Turn { get { return Emulator.ReadByte(_pwrMATTurn); } set { Emulator.WriteByte(_pwrMATTurn, value); } }
        public sbyte PWR_DF { get { return Emulator.ReadSByte(_pwrDF); } set { Emulator.WriteByte(_pwrDF, value); } }
        public byte PWR_DF_Turn { get { return Emulator.ReadByte(_pwrDFTurn); } set { Emulator.WriteByte(_pwrDFTurn, value); } }
        public sbyte PWR_MDF { get { return Emulator.ReadSByte(_pwrMDF); } set { Emulator.WriteByte(_pwrMDF, value); } }
        public byte PWR_MDF_Turn { get { return Emulator.ReadByte(_pwrMDFTurn); } set { Emulator.WriteByte(_pwrMDFTurn, value); } }
        public byte Speed_Up_Turn { get { return Emulator.ReadByte(_spdUpTurn); } set { Emulator.WriteByte(_spdUpTurn, value); } }
        public byte Speed_Down_Turn { get { return Emulator.ReadByte(_spdDownTurn); } set { Emulator.WriteByte(_spdDownTurn, value); } }
        public byte P_Immune_Increase { get { return Emulator.ReadByte(_pImmuneIncrease); } set { Emulator.WriteByte(_pImmuneIncrease, value); } }
        public byte P_Immune_Increase_Turn { get { return Emulator.ReadByte(_pImmuneIcreaseTurn); } set { Emulator.WriteByte(_pImmuneIcreaseTurn, value); } }
        public byte M_Immune_Increase { get { return Emulator.ReadByte(_mImmuneIncrease); } set { Emulator.WriteByte(_mImmuneIncrease, value); } }
        public byte M_Immune_Increase_Turn { get { return Emulator.ReadByte(_mImmuneIncreaseTurn); } set { Emulator.WriteByte(_mImmuneIncreaseTurn, value); } }
        public long Pos_FB { get { return Emulator.ReadLong(_posFB); } set { Emulator.WriteLong(_posFB, value); } }
        public long Pos_UD { get { return Emulator.ReadLong(_posUD); } set { Emulator.WriteLong(_posUD, value); } }
        public long Pos_RL { get { return Emulator.ReadLong(_posRL); } set { Emulator.WriteLong(_posRL, value); } }
        public byte PhysicalShield { get { return (byte)(Emulator.ReadByte(_pShieldMshieldSigStone) & 3); } set { Emulator.WriteByte(_pShieldMshieldSigStone, Emulator.ReadByte(_pShieldMshieldSigStone) | Math.Min(value, (byte)3)); } }
        public byte MagicalShield { get { return (byte)((Emulator.ReadByte(_pShieldMshieldSigStone) >> 2) & 3); } set { Emulator.WriteByte(_pShieldMshieldSigStone, Emulator.ReadByte(_pShieldMshieldSigStone) | (Math.Min(value, (byte)3) << 2)); } }
        

        public BattleEntityAddress(long point, int slot, int position) {
            long curr_point = point - slot * 0x388;
            _action = curr_point - 0xA8;
            _spellCast = curr_point + 0x46;
            _hp = curr_point;
            _maxHP = curr_point + 0x8;
            _status = curr_point + 0x6;
            _statusRes = curr_point + 0x1C;
            _specialEffect = curr_point + 0xC;
            _guard = curr_point + 0x4C;
            _at = curr_point + 0x2C;
            _ogAT = curr_point + 0x58;
            _mat = curr_point + 0x2E;
            _ogMAT = curr_point + 0x5A;
            _df = curr_point + 0x30;
            _ogDF = curr_point + 0x5E;
            _mdf = curr_point + 0x32;
            _ogMDF = curr_point + 0x60;
            _spd = curr_point + 0x2A;
            _ogSPD = curr_point + 0x5C;
            _turn = curr_point + 0x44;
            _aAV = curr_point + 0x38;
            _mAV = curr_point + 0x3A;
            _pImmune = curr_point + 0x108;
            _mImmune = curr_point + 0x10A;
            _pHalf = curr_point + 0x10C;
            _mHalf = curr_point + 0x10E;
            _eImmune = curr_point + 0x1A;
            _eHalf = curr_point + 0x18;
            _uniqueIndex = curr_point + 0x264;
            _pwrAT = curr_point + 0xAC;
            _pwrATTurn = curr_point + 0xAD;
            _pwrMAT = curr_point + 0xAE;
            _pwrMATTurn = curr_point + 0xAF;
            _pwrDF = curr_point + 0xB0;
            _pwrDFTurn = curr_point + 0xB1;
            _pwrMDF = curr_point + 0xB2;
            _pwrMDFTurn = curr_point + 0xB3;
            _spdUpTurn = curr_point + 0xC1;
            _spdDownTurn = curr_point + 0xC3;
            _pImmuneIncrease = curr_point + 0xBC;
            _pImmuneIcreaseTurn = curr_point + 0xBD;
            _mImmuneIncrease = curr_point + 0xBE;
            _mImmuneIncreaseTurn = curr_point + 0xBF;
            _posFB = curr_point + 0x16D;
            _posUD = curr_point + 0x171;
            _posRL = curr_point + 0x175;

            _pShieldMshieldSigStone = 0x6E3B4 + position * 0x20; // Which one from constants is this??
        }
    }
}
