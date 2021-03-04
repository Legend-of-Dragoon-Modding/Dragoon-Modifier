using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.MemoryController {
    class BattleAddress {
        long _action;
        long _hp;
        long _maxHP;
        //long _element;
        //long _displayElement;
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

        public byte Action { get { return Emulator.ReadByte(_action); } set { Emulator.WriteByte(_action, value); } }
        public ushort HP { get { return Emulator.ReadUShort(_hp); } set { Emulator.WriteUShort(_hp, value); } }
        public ushort Max_HP { get { return Emulator.ReadUShort(_maxHP); } set { Emulator.WriteUShort(_maxHP, value); } }
        // Element
        // DisplayElement
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

        public BattleAddress(long point, int position) {
            long curr_point = point - position * 0x388;
            _action = curr_point - 0xA0;
            _hp = curr_point;
            _maxHP = curr_point + 0x8;
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

        }
    }
}
