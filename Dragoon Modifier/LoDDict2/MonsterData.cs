using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.LoDDict2 {
    public class MonsterData {
        string _name = "Monster";
        byte _element = 128;
        uint _hp = 1;
        ushort _at = 1;
        ushort _mat = 1;
        ushort _df = 1;
        ushort _mdf = 1;
        ushort _spd = 1;
        short _a_av = 0;
        short _m_av = 0;
        byte _physicalImmunity = 0;
        byte _magicalImmunity = 0;
        byte _p_half = 0;
        byte _m_half = 0;
        byte _elementalImmunity = 0;
        byte _e_half = 0;
        byte _statusResist = 0;
        byte _specialEffect = 0;
        ushort _exp = 0;
        ushort _gold = 0;
        byte _dropItem = 255;
        byte _dropChance = 0;

        public MonsterData(string[] data, Dictionary<string, byte> item2num, Dictionary<string, byte> status2num, Dictionary<string, byte> special2num, Dictionary<string, byte> element2num) {
            _name = data[1];
            if (element2num.TryGetValue(data[2].ToLower(), out byte bkey)) {
                _element = bkey;
            } else {
                Constants.WriteError($"{data[2]} not found as Element for {data[1]} (ID: {data[0]})");
            }
            if (UInt32.TryParse(data[3], out uint uikey)) {
                _hp = uikey;
            } else {
                Constants.WriteError($"{data[3]} not found as HP for {data[1]} (ID: {data[0]})");
            }
            if (UInt16.TryParse(data[4], out ushort uskey)) {
                _at = uskey;
            } else {
                Constants.WriteError($"{data[4]} not found as AT for {data[1]} (ID: {data[0]})");
            }
            if (UInt16.TryParse(data[5], out uskey)) {
                _mat = uskey;
            } else {
                Constants.WriteError($"{data[5]} not found as MAT for {data[1]} (ID: {data[0]})");
            }
            if (UInt16.TryParse(data[6], out uskey)) {
                _df = uskey;
            } else {
                Constants.WriteError($"{data[6]} not found as DF for {data[1]} (ID: {data[0]})");
            }
            if (UInt16.TryParse(data[7], out uskey)) {
                _mdf = uskey;
            } else {
                Constants.WriteError($"{data[7]} not found as MDF for {data[1]} (ID: {data[0]})");
            }
            if (UInt16.TryParse(data[8], out uskey)) {
                _spd = uskey;
            } else {
                Constants.WriteError($"{data[8]} not found as SPD for {data[1]} (ID: {data[0]})");
            }
            if (Int16.TryParse(data[9], out short skey)) {
                _a_av = skey;
            } else {
                Constants.WriteError($"{data[9]} not found as A_AV for {data[1]} (ID: {data[0]})");
            }
            if (Int16.TryParse(data[10], out skey)) {
                _m_av = skey;
            } else {
                Constants.WriteError($"{data[10]} not found as M_AV for {data[1]} (ID: {data[0]})");
            }
            if (Byte.TryParse(data[11], out bkey)) {
                _physicalImmunity = bkey;
            } else {
                Constants.WriteError($"{data[11]} not found as Physical Immunity for {data[1]} (ID: {data[0]})");
            }
            if (Byte.TryParse(data[12], out bkey)) {
                _magicalImmunity = bkey;
            } else {
                Constants.WriteError($"{data[12]} not found as Magical Immunity for {data[1]} (ID: {data[0]})");
            }
            if (Byte.TryParse(data[13], out bkey)) {
                _p_half = bkey;
            } else {
                Constants.WriteError($"{data[13]} not found as P_Half for {data[1]} (ID: {data[0]})");
            }
            if (Byte.TryParse(data[14], out bkey)) {
                _m_half = bkey;
            } else {
                Constants.WriteError($"{data[13]} not found as M_Half for {data[1]} (ID: {data[0]})");
            }
            foreach (string substring in data[15].Replace(" ", "").Split(',')) {
                if (element2num.TryGetValue(substring.ToLower(), out bkey)) {
                    _elementalImmunity |= bkey;
                } else {
                    Constants.WriteError($"{substring} not found as Elemental Immunity for {data[1]} (ID: {data[0]})");
                }
            }
            foreach (string substring in data[16].Replace(" ", "").Split(',')) {
                if (element2num.TryGetValue(substring.ToLower(), out bkey)) {
                    _e_half |= bkey;
                } else {
                    Constants.WriteError($"{substring} not found as E_Half for {data[1]} (ID: {data[0]})");
                }
            }
            foreach (string substring in data[17].Replace(" ", "").Split(',')) {
                if (status2num.TryGetValue(substring.ToLower(), out bkey)) {
                    _statusResist |= bkey;
                } else {
                    Constants.WriteError($"{substring} not found as Status Resist for {data[1]} (ID: {data[0]})");
                }
            }
            foreach (string substring in data[18].Replace(" ", "").Split(',')) {
                if (special2num.TryGetValue(substring.ToLower(), out bkey)) {
                    _specialEffect |= bkey;
                } else {
                    Constants.WriteError($"{substring} not found as Special Effect for {data[1]} (ID: {data[0]})");
                }
            }
            if (UInt16.TryParse(data[19], out uskey)) {
                _exp = uskey;
            } else {
                Constants.WriteError($"{data[19]} not found as EXP for {data[1]} (ID: {data[0]})");
            }
            if (UInt16.TryParse(data[20], out uskey)) {
                _gold = uskey;
            } else {
                Constants.WriteError($"{data[20]} not found as Gold for {data[1]} (ID: {data[0]})");
            }
            if (item2num.TryGetValue(data[21].ToLower(), out bkey)) {
                _dropItem = bkey;
            } else {
                Constants.WriteError($"{data[21]} not found in Item List for {data[1]} (ID: {data[0]})");
            }
            if (Byte.TryParse(data[22], out bkey)) {
                _dropChance = bkey;
            } else {
                Constants.WriteError($"{data[22]} not found as Drop Chance for {data[1]} (ID: {data[0]})");
            }
        }
    }
}
