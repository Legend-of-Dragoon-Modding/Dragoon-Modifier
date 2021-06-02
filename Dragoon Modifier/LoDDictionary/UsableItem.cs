using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.LoDDictionary {
    public class UsableItem : Item {
        static readonly Dictionary<string, byte> _special12Num = new Dictionary<string, byte>() {
            {"", 0 },
            {"none", 0 },
            {"power1", 0x10 },
            {"power2", 0x20 },
            {"power3", 0x40 },
            {"power4", 0x80 }
        };
        static readonly Dictionary<string, byte> _special22Num = new Dictionary<string, byte>() {
            {"", 0 },
            {"none", 0 },
            {"speed_down", 0x10 },
            {"speed_up", 0x20 },
            {"magic_res", 0x40 },
            {"physical_res", 0x80 }
        };
        static readonly Dictionary<string, byte> _bases = new Dictionary<string, byte>() {
            {"", 0x0 },
            {"none", 0x0 },
            {"status_cause", 0x4 },
            {"status_cure", 0x8 },
            {"revive", 0x10 },
            {"sp", 0x20 },
            {"mp", 0x40 },
            {"hp", 0x80 }
        };

        string _battleDescription = " ";
        string _encodedBattleDescription = "00 00 FF A0";
        byte _target = 0xC0;
        byte _element = 0;
        byte _damage = 0;
        byte _special1 = 0;
        byte _special2 = 0;
        byte _unknown1 = 0;
        byte _specialAmmount = 0;
        byte _status = 0;
        byte _percentage = 0;
        byte _unknown2 = 0;
        byte _baseSwitch = 0;

        public string BattleDescription { get { return _battleDescription; } private set { _battleDescription = value; } }
        public string EncodedBattleDescription { get { return _encodedBattleDescription; } private set { _encodedBattleDescription = value; } }
        public int BattleDescriptionPointer { get; set; }
        public int BattleNamePointer { get; set; }
        public byte Target { get { return _target; } private set { _target = value; } }
        public byte Element { get { return _element; } private set { _element = value; } }
        public byte Damage { get { return _damage; } private set { _damage = value; } }
        public byte Special1 { get { return _special1; } private set { _special1 = value; } }
        public byte Special2 { get { return _special2; } private set { _special2 = value; } }
        public byte Unknown1 { get { return _unknown1; } private set { _unknown1 = value; } }
        public byte SpecialAmmount { get { return _specialAmmount; } private set { _specialAmmount = value; } }
        public byte Status { get { return _status; } private set { _status = value; } }
        public byte Percentage { get { return _percentage; } private set { _percentage = value; } }
        public byte Unknown2 { get { return _unknown2; } private set { _unknown2 = value; } }
        public byte BaseSwitch { get { return _baseSwitch; } private set { _baseSwitch = value; } }

        public UsableItem(byte index, string[] values) {
            var error = new List<string>();
            Id = index;
            if (!(values[0] == "" || values[0] == " ")) {
                Name = values[0];
                EncodedName = Dictionary.EncodeText(Name) + " FF A0";
            }
            if (Byte.TryParse(values[1], NumberStyles.AllowLeadingSign, null as IFormatProvider, out byte bkey)) {
                _target = bkey;
            } else if (values[1] != "") {
                error.Add($"{values[1]} not found as target.");
            }
            if (Dictionary.Element2Num.TryGetValue(values[2].ToLower(), out bkey)) {
                _element = bkey;
            } else {
                error.Add($"{values[2]} not found as element.");
            }
            if (Dictionary.DamageBase2Num.TryGetValue(values[3], out bkey)) {
                _damage = bkey;
            } else if (values[3] != "") {
                error.Add($"{values[3]} not found as damage base.");
            }
            var errorTemp = new List<string>();
            foreach (string substring in values[4].Replace(" ", "").Split(',')) {
                if (_special12Num.TryGetValue(substring.ToLower(), out bkey)) {
                    _special1 |= bkey;
                } else {
                    errorTemp.Add(substring);
                }
            }
            if (errorTemp.Count > 0) {
                error.Add($"{String.Join(", ", errorTemp)} not found as special 1.");
            }
            errorTemp = new List<string>();
            foreach (string substring in values[5].Replace(" ", "").Split(',')) {
                if (_special22Num.TryGetValue(substring.ToLower(), out bkey)) {
                    _special2 |= bkey;
                } else {
                    errorTemp.Add(substring);
                }
            }
            if (errorTemp.Count > 0) {
                error.Add($"{String.Join(", ", errorTemp)} not found as special 2.");
            }
            if (Byte.TryParse(values[6], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                _unknown1 = bkey;
            } else if (values[6] != "") {
                error.Add($"{values[6]} not found as unknown 1.");
            }
            if (Int16.TryParse(values[7], NumberStyles.AllowLeadingSign, null as IFormatProvider, out short skey)) {
                _specialAmmount = (byte) skey;
            } else if (values[7] != "") {
                error.Add($"{values[7]} not found as special ammount.");
            }
            if (icons.TryGetValue(values[8].ToLower(), out bkey)) {
                Icon = bkey;
            } else {
                error.Add($"{values[8]} not found as icon.");
            }
            errorTemp = new List<string>();
            foreach (string substring in values[9].Replace(" ", "").Split(',')) {
                if (Dictionary.Status2Num.TryGetValue(substring.ToLower(), out bkey)) {
                    _status |= bkey;
                } else {
                    errorTemp.Add(substring);
                }
            }
            if (errorTemp.Count > 0) {
                error.Add($"{String.Join(", ", errorTemp)} not found as status.");
            }
            if (Byte.TryParse(values[10], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                _percentage = bkey;
            } else if (values[10] != "") {
                error.Add($"{values[10]} not found as percentage.");
            }
            if (Byte.TryParse(values[11], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                _unknown2 = bkey;
            } else if (values[11] != "") {
                error.Add($"{values[11]} not found as unknown 2.");
            }
            errorTemp = new List<string>();
            foreach (string substring in values[12].Replace(" ", "").Split(',')) {
                if (_bases.TryGetValue(substring.ToLower(), out bkey)) {
                    _baseSwitch |= bkey;
                } else {
                    errorTemp.Add(substring);
                }
            }
            if (errorTemp.Count > 0) {
                error.Add($"{String.Join(", ", errorTemp)} not found as base switch.");
            }
            if (!(values[13] == "" || values[13] == " ")) {
                Description = values[13];
                EncodedDescription = Dictionary.EncodeText(Description) + " FF A0";
            }
            if (UInt16.TryParse(values[14], NumberStyles.AllowLeadingSign, null as IFormatProvider, out ushort uskey)) {
                float temp = (float) uskey / 2;
                SellPrice = (short) Math.Round(temp);
            } else if (values[14] != "") {
                error.Add($"{values[14]} not found as price.");
            }
            if (!(values[15] == "" || values[15] == " ")) {
                _battleDescription = values[15];
                _encodedBattleDescription = LoDDict.StringEncode(_battleDescription) + " FF A0";
            }

            if (error.Count > 0) {
                Constants.WriteError($"Item: {Name} - ID: {Id}");
                foreach (var e in error) {
                    Constants.WriteError($"\t{e}");
                }
            }
        }
    }
}
