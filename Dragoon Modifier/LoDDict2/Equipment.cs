using System;
using System.Collections.Generic;
using System.Globalization;

namespace Dragoon_Modifier.LoDDict2 {
    public class Equipment : Item {
        byte equips = 0;
        byte type = 0;
        byte element = 0;
        byte on_hit_status = 0;
        byte status_chance = 0;
        byte at = 0;
        byte mat = 0;
        byte df = 0;
        byte mdf = 0;
        byte spd = 0;
        byte a_hit = 0;
        byte m_hit = 0;
        byte a_av = 0;
        byte m_av = 0;
        byte e_half = 0;
        byte e_immune = 0;
        byte stat_res = 0;
        byte special1 = 0;
        byte special2 = 0;
        short special_ammount = 0;
        byte death_res = 0;


        #region Dictionaries

        Dictionary<string, byte> typeDict = new Dictionary<string, byte>() {
                {"weapon", 128 },
                {"armor", 32 },
                {"helm", 64 },
                {"helmet", 64 },
                {"boots", 16 },
                {"shoes", 16 },
                {"accessory", 8 },
                {"none", 0},
                {"", 0 }
            };
        Dictionary<string, byte> charDict = new Dictionary<string, byte>() {
                {"meru", 1 },
                {"shana", 2 },
                {"miranda", 2 },
                {"sharanda", 2 },
                {"???", 2 },
                {"rose", 4 },
                {"haschel", 16 },
                {"kongol", 32 },
                {"lavitz", 64 },
                {"albert", 64 },
                {"dart", 128 },
                {"female", 7 },
                {"male", 240 },
                {"all", 247 },
                {"none", 0 },
                {"", 0 }
            };
        Dictionary<string, byte> special12num = new Dictionary<string, byte>() {
                {"", 0 },
                {"none", 0 },
                {"mp_m_hit", 1 },
                {"sp_m_hit", 2 },
                {"mp_p_hit", 4 },
                {"sp_p_hit", 8 },
                {"sp_multi", 16 },
                {"p_half", 32 }
            };
        Dictionary<string, byte> special22num = new Dictionary<string, byte>() {
                {"", 0 },
                {"none", 0 },
                {"mp_multi", 1 },
                {"hp_multi", 2 },
                {"m_half", 4 },
                {"revive", 8 },
                {"sp_regen", 16 },
                {"mp_regen", 32 },
                {"hp_regen", 64 }
            };
        Dictionary<string, byte> death2num = new Dictionary<string, byte>() {
                {"", 0 },
                {"none", 0 },
                {"dragon_buster", 4 },
                {"attack_all", 8 },
                {"death_chance", 64 },
                {"death_res", 128 }
            };

        #endregion

        public byte Equips { get { return equips; } }
        public byte Type { get { return type; } }
        public byte Element { get { return element; } }
        public byte On_Hit_Status { get { return on_hit_status; } }
        public byte Status_Chance { get { return status_chance; } }
        public byte AT { get { return at; } }
        public byte MAT { get { return mat; } }
        public byte DF { get { return df; } }
        public byte MDF { get { return mdf; } }
        public byte SPD { get { return spd; } }
        public byte A_Hit { get { return a_hit; } }
        public byte M_Hit { get { return m_hit; } }
        public byte A_AV { get { return a_av; } }
        public byte M_AV { get { return m_av; } }
        public byte E_Half { get { return e_half; } }
        public byte E_Immune { get { return e_immune; } }
        public byte Stat_Res { get { return stat_res; } }
        public byte Special1 { get { return special1; } }
        public byte Special2 { get { return special2; } }
        public short Special_Ammount { get { return special_ammount; } }
        public byte Death_Res { get { return death_res; } }

        public Equipment(byte index, string[] values) {
            id = index;
            name = values[0];
            if (!(name == "" || name == " ")) {
                encodedName = LoDDict.StringEncode(Name) + " FF A0";
            }
            if (typeDict.TryGetValue(values[1].ToLower(), out byte bkey)) {
                type = bkey;
            } else {
                Constants.WriteError(values[1] + " not found as equipment type for item: " + Name);
            }
            foreach (string substring in values[2].Replace(" ", "").Split(',')) {
                if (charDict.TryGetValue(substring.ToLower(), out bkey)) {
                    equips |= bkey;
                } else {
                    Constants.WriteError(substring + " not found as character for item: " + Name);
                }
            }
            if (IconDict.TryGetValue(values[3].ToLower(), out bkey)) {
                icon = bkey;
            } else {
                Constants.WriteError(values[3] + " not found as icon for item: " + Name);
            }
            if (LoDDict2.element2num.TryGetValue(values[4].ToLower(), out bkey)) {
                element = bkey;
            } else {
                Constants.WriteError(values[4] + " not found as element for item: " + Name);
            }
            if (LoDDict2.status2num.TryGetValue(values[5].ToLower(), out bkey)) {
                on_hit_status = bkey;
            } else {
                Constants.WriteError(values[5] + " not found as Status for item: " + Name);
            }
            if (Byte.TryParse(values[6], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                status_chance = bkey;
            } else if (values[6] != "") {
                Constants.WriteError(values[6] + " not found as Status Chance for item: " + Name);
            }
            if (Byte.TryParse(values[7], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                at = bkey;
            } else if (values[7] != "") {
                Constants.WriteError(values[7] + " not found as AT for item: " + Name);
            }
            if (Byte.TryParse(values[8], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                mat = bkey;
            } else if (values[8] != "") {
                Constants.WriteError(values[8] + " not found as MAT for item: " + Name);
            }
            if (Byte.TryParse(values[9], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                df = bkey;
            } else if (values[9] != "") {
                Constants.WriteError(values[9] + " not found as DF for item: " + Name);
            }
            if (Byte.TryParse(values[10], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                mdf = bkey;
            } else if (values[10] != "") {
                Constants.WriteError(values[10] + " not found as MDF for item: " + Name);
            }
            if (Byte.TryParse(values[11], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                spd = bkey;
            } else if (values[11] != "") {
                Constants.WriteError(values[11] + " not found as SPD for item: " + Name);
            }
            if (Byte.TryParse(values[12], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                a_hit = bkey;
            } else if (values[12] != "") {
                Constants.WriteError(values[12] + " not found as A_Hit for item: " + Name);
            }
            if (Byte.TryParse(values[13], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                m_hit = bkey;
            } else if (values[13] != "") {
                Constants.WriteError(values[13] + " not found as M_Hit for item: " + Name);
            }
            if (Byte.TryParse(values[14], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                a_av = bkey;
            } else if (values[14] != "") {
                Constants.WriteError(values[14] + " not found as A_AV for item: " + Name);
            }
            if (Byte.TryParse(values[15], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                m_av = bkey;
            } else if (values[15] != "") {
                Constants.WriteError(values[15] + " not found as M_AV for item: " + Name);
            }
            foreach (string substring in values[16].Replace(" ", "").Split(',')) {
                if (LoDDict2.element2num.TryGetValue(substring.ToLower(), out bkey)) {
                    e_half |= bkey;
                } else {
                    Constants.WriteError(substring + " not found as E_Half for item:" + Name);
                }
            }
            foreach (string substring in values[17].Replace(" ", "").Split(',')) {
                if (LoDDict2.element2num.TryGetValue(substring.ToLower(), out bkey)) {
                    e_immune |= bkey;
                } else {
                    Constants.WriteError(substring + " not found as E_Immune for item:" + Name);
                }
            }
            foreach (string substring in values[18].Replace(" ", "").Split(',')) {
                if (LoDDict2.status2num.TryGetValue(substring.ToLower(), out bkey)) {
                    stat_res |= bkey;
                } else {
                    Constants.WriteError(substring + " not found as Status_Resist for item:" + Name);
                }
            }
            foreach (string substring in values[19].Replace(" ", "").Split(',')) {
                if (special12num.TryGetValue(substring.ToLower(), out bkey)) {
                    special1 |= bkey;
                } else {
                    Constants.WriteError(substring + " not found as Special1 for item:" + Name);
                }
            }
            foreach (string substring in values[20].Replace(" ", "").Split(',')) {
                if (special22num.TryGetValue(substring.ToLower(), out bkey)) {
                    special2 |= bkey;
                } else {
                    Constants.WriteError(substring + " not found as Special2 for item:" + Name);
                }
            }
            if (Int16.TryParse(values[21], NumberStyles.AllowLeadingSign, null as IFormatProvider, out short skey)) {
                special_ammount = skey;
            } else if (values[21] != "") {
                Constants.WriteError(values[21] + " not found as Special_Ammount for item: " + Name);
            }
            foreach (string substring in values[22].Replace(" ", "").Split(',')) {
                if (death2num.TryGetValue(substring.ToLower(), out bkey)) {
                    death_res |= bkey;
                } else {
                    Constants.WriteError(substring + " not found as Death_Res for item:" + Name);
                }
            }
            description = values[23];
            if (!(description == "" || description == " ")) {
                encodedDescription = LoDDict.StringEncode(Description) + " FF A0";
            }
            if (UInt16.TryParse(values[24], NumberStyles.AllowLeadingSign, null as IFormatProvider, out ushort key3)) {
                float temp = (float) key3 / 2;
                sell_price = (short) Math.Round(temp);
            } else if (values[24] != "") {
                Constants.WriteError(values[24] + " not found as Price for item: " + Name);
            }
        }
    }
}
