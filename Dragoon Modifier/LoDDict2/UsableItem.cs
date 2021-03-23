using System;
using System.Collections.Generic;
using System.Globalization;

namespace Dragoon_Modifier.LoDDict2 {
    public class UsableItem : Item {
        string battleDescription = "<END>";
        string encondedBattleDescription = "FF A0 FF A0";
        long battleDescriptionPointer = 0;
        long battleNamePointer = 0;
        byte target = 0;
        byte element = 0;
        byte damage = 0;
        byte special1 = 0;
        byte special2 = 0;
        byte uu1 = 0;
        byte special_ammount = 0;
        byte status = 0;
        byte percentage = 0;
        byte uu2 = 0;
        byte baseSwitch = 0;

        #region Dictionaries

        Dictionary<string, byte> base_table = new Dictionary<string, byte>() {

            {"", 0x0 },
            {"none", 0x0 },
            {"100", 0x0 },
            {"800", 0x1 },
            {"600", 0x2 },
            {"500", 0x4 },
            {"400", 0x8 },
            {"300", 0x10 },
            {"200", 0x20 },
            {"150", 0x40 },
            {"50", 0x80 }
            };
        Dictionary<string, byte> special12num = new Dictionary<string, byte>() {
                {"", 0 },
                {"none", 0 },
                {"power1", 0x10 },
                {"power2", 0x20 },
                {"power3", 0x40 },
                {"power4", 0x80 }
            };
        Dictionary<string, byte> special22num = new Dictionary<string, byte>() {
                {"", 0 },
                {"none", 0 },
                {"speed_down", 0x10 },
                {"speed_up", 0x20 },
                {"magic_res", 0x40 },
                {"physical_res", 0x80 }
            };
        Dictionary<string, byte> base_dict = new Dictionary<string, byte>() {
            {"", 0x0 },
            {"none", 0x0 },
            {"status_cause", 0x4 },
            {"status_cure", 0x8 },
            {"revive", 0x10 },
            {"sp", 0x20 },
            {"mp", 0x40 },
            {"hp", 0x80 }
        };

        #endregion

        public string BattleDescription { get { return battleDescription; } }
        public string EncodedBattleDescription { get { return encondedBattleDescription; } }
        public long BattleDescriptionPointer { get { return battleDescriptionPointer; } set { battleDescriptionPointer = value; } }
        public long BattleNamePointer { get { return battleNamePointer; } set { battleNamePointer = value; } }

        public UsableItem(byte index, string[] values) {
            id = index;
            if (!(values[0] == "" || values[0] == " ")) {
                name = values[0];
                encodedName = LoDDict.StringEncode(Name) + " FF A0";
            }
            if (Byte.TryParse(values[1], NumberStyles.AllowLeadingSign, null as IFormatProvider, out byte bkey)) {
                target = bkey;
            } else if (values[1] != "") {
                Constants.WriteError(values[1] + " not found as Target for item: " + Name);
            }
            if (LoDDict2.element2num.TryGetValue(values[2].ToLower(), out bkey)) {
                element = bkey;
            } else {
                Constants.WriteError(values[2] + " not found as Element for item: " + Name);
            }
            if (base_table.TryGetValue(values[3], out bkey)) {
                damage = bkey;
            } else if (values[3] != "") {
                Constants.WriteError(values[3] + " not found as Damage for item: " + Name);
            }
            foreach (string substring in values[4].Replace(" ", "").Split(',')) {
                if (special12num.TryGetValue(substring.ToLower(), out bkey)) {
                    special1 |= bkey;
                } else {
                    Constants.WriteError(substring + " not found as Special1 for item:" + Name);
                }
            }
            foreach (string substring in values[5].Replace(" ", "").Split(',')) {
                if (special22num.TryGetValue(substring.ToLower(), out bkey)) {
                    special2 |= bkey;
                } else {
                    Constants.WriteError(substring + " not found as Special2 for item:" + Name);
                }
            }
            if (Byte.TryParse(values[6], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                uu1 = bkey;
            } else if (values[6] != "") {
                Constants.WriteError(values[6] + " not found as UU1 for item: " + Name);
            }
            if (Int16.TryParse(values[7], NumberStyles.AllowLeadingSign, null as IFormatProvider, out short skey)) {
                special_ammount = (byte) skey;
            } else if (values[7] != "") {
                Constants.WriteError(values[7] + " not found as Special_Ammount for item: " + Name);
            }
            if (IconDict.TryGetValue(values[8].ToLower(), out bkey)) {
                icon = bkey;
            } else {
                Constants.WriteError(values[8] + " not found as Icon for item: " + Name);
            }
            foreach (string substring in values[9].Replace(" ", "").Split(',')) {
                if (LoDDict2.status2num.TryGetValue(substring.ToLower(), out bkey)) {
                    status |= bkey;
                } else {
                    Constants.WriteError(substring + " not found as Status for item:" + Name);
                }
            }
            if (Byte.TryParse(values[10], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                percentage = bkey;
            } else if (values[10] != "") {
                Constants.WriteError(values[10] + " not found as Percentage for item: " + Name);
            }
            if (Byte.TryParse(values[11], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                uu2 = bkey;
            } else if (values[11] != "") {
                Constants.WriteError(values[11] + " not found as UU2 for item: " + Name);
            }
            foreach (string substring in values[12].Replace(" ", "").Split(',')) {
                if (base_dict.TryGetValue(substring.ToLower(), out bkey)) {
                    baseSwitch |= bkey;
                } else {
                    Constants.WriteError(substring + " not found as Base_Switch for item:" + Name);
                }
            }
            description = values[13];
            if (!(description == "" || description == " ")) {
                encodedDescription = LoDDict.StringEncode(Description) + " FF A0";
            }
            if (UInt16.TryParse(values[14], NumberStyles.AllowLeadingSign, null as IFormatProvider, out ushort uskey)) {
                float temp = (float) uskey / 2;
                sell_price = (short) Math.Round(temp);
            } else if (values[14] != "") {
                Constants.WriteError(values[14] + " not found as Price for item: " + Name);
            }
            battleDescription = values[15];
            if (!(battleDescription == "" || battleDescription == " ")) {
                encondedBattleDescription = LoDDict.StringEncode(battleDescription) + " FF A0";
            }
        }
    }
}
