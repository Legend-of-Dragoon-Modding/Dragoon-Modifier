using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.LoDDictionary {
    public class Equipment : Item {

        static readonly Dictionary<string, byte> types = new Dictionary<string, byte>() {
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
        static readonly Dictionary<string, byte> characters = new Dictionary<string, byte>() {
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
        static readonly Dictionary<string, byte> specialBonuses1 = new Dictionary<string, byte>() {
            {"", 0 },
            {"none", 0 },
            {"mp_m_hit", 1 },
            {"sp_m_hit", 2 },
            {"mp_p_hit", 4 },
            {"sp_p_hit", 8 },
            {"sp_multi", 16 },
            {"p_half", 32 }
        };
        static readonly Dictionary<string, byte> specialBonuses2 = new Dictionary<string, byte>() {
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
        static readonly Dictionary<string, byte> specialEffects = new Dictionary<string, byte>() {
            {"", 0 },
            {"none", 0 },
            {"cannot_sell", 4 },
            {"attack_all", 8 },
            {"death_chance", 64 },
            {"death_res", 128 }
        };

        byte _whoEquips = 0;
        byte _type = 0;
        byte _element = 0;
        byte _onHitStatus = 0;
        byte _onHitStatusChance = 0;
        ushort _at = 0;
        byte _mat = 0;
        byte _df = 0;
        byte _mdf = 0;
        byte _spd = 0;
        byte _aHit = 0;
        byte _mHit = 0;
        byte _aAV = 0;
        byte _mAV = 0;
        byte _elementalResistance = 0;
        byte _elementalImmunity = 0;
        byte _statusResistance = 0;
        byte _specialBonus1 = 0;
        byte _specialBonus2 = 0;
        short _specialBonusAmmount = 0;
        byte _specialEffect = 0;

        public byte WhoEquips { get { return _whoEquips; } private set { _whoEquips = value; } }
        public byte Type { get { return _type; } private set { _type = value; } }
        public byte Element { get { return _element; } private set { _element = value; } }
        public byte OnHitStatus { get { return _onHitStatus; } private set { _onHitStatus = value; } }
        public byte OnHitStatusChance { get { return _onHitStatusChance; } private set { _onHitStatusChance = value; } }
        public ushort AT { get { return _at; } private set { _at = value; } }
        public byte MAT { get { return _mat; } private set { _mat = value; } }
        public byte DF { get { return _df; } private set { _df = value; } }
        public byte MDF { get { return _mdf; } private set { _mdf = value; } }
        public byte SPD { get { return _spd; } private set { _spd = value; } }
        public byte A_HIT { get { return _aHit; } private set { _aHit = value; } }
        public byte M_HIT { get { return _mHit; } private set { _mHit = value; } }
        public byte A_AV { get { return _aAV; } private set { _aAV = value; } }
        public byte M_AV { get { return _mAV; } private set { _mAV = value; } }
        public byte ElementalResistance { get { return _elementalResistance; } private set { _elementalResistance = value; } }
        public byte ElementalImmunity { get { return _elementalImmunity; } private set { _elementalImmunity = value; } }
        public byte StatusResistance { get { return _statusResistance; } private set { _statusResistance = value; } }
        public byte SpecialBonus1 { get { return _specialBonus1; } private set { _specialBonus1 = value; } }
        public byte SpecialBonus2 { get { return _specialBonus2; } private set { _specialBonus2 = value; } }
        public short SpecialBonusAmmount { get { return _specialBonusAmmount; } private set { _specialBonusAmmount = value; } }
        public byte SpecialEffect { get { return _specialEffect; } private set { _specialEffect = value; } }

        public Equipment(byte index, string[] values) {
            var error = new List<string>();
            Id = index;
            if (!(values[0] == "" || values[0] == " ")) {
                Name = values[0];
                EncodedName = Dictionary.EncodeText(values[0]);
                Constants.WriteDebug(EncodedName);
            }

            if (types.TryGetValue(values[1].ToLower(), out byte bkey)) {
                _type = bkey;
            } else {
                error.Add($"{values[1]} not found as equipment type.");
            }

            var errorTemp = new List<string>();
            foreach (string substring in values[2].Replace(" ", "").Split(',')) {
                if (characters.TryGetValue(substring.ToLower(), out bkey)) {
                    _whoEquips |= bkey;
                } else {
                    errorTemp.Add(substring);
                }
            }
            if (errorTemp.Count > 0) {
                error.Add($"{String.Join(", ", errorTemp)} not found as character.");
            }

            if (icons.TryGetValue(values[3].ToLower(), out bkey)) {
                Icon = bkey;
            } else {
                error.Add($"{values[3]} not found as icon.");
            }

            if (Dictionary.Element2Num.TryGetValue(values[4].ToLower(), out bkey)) {
                _element = bkey;
            } else {
                error.Add($"{values[4]} not found as element.");
            }

            if (Dictionary.Status2Num.TryGetValue(values[5].ToLower(), out bkey)) {
                _onHitStatus = bkey;
            } else {
                error.Add($"{values[5]} not found as status.");
            }

            if (Byte.TryParse(values[6], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                _onHitStatusChance = bkey;
            } else if (values[6] != "") {
                error.Add($"{values[6]} not found as status chance.");
            }

            if (UInt16.TryParse(values[7], NumberStyles.AllowLeadingSign, null as IFormatProvider, out var uskey)) {
                _at = uskey;
            } else if (values[7] != "") {
                error.Add($"{values[7]} not found as AT.");
            }

            if (Byte.TryParse(values[8], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                _mat = bkey;
            } else if (values[8] != "") {
                error.Add($"{values[8]} not found as MAT.");
            }

            if (Byte.TryParse(values[9], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                _df = bkey;
            } else if (values[9] != "") {
                error.Add($"{values[9]} not found as DF.");
            }

            if (Byte.TryParse(values[10], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                _mdf = bkey;
            } else if (values[10] != "") {
                error.Add($"{values[10]} not found as MDF.");
            }

            if (Byte.TryParse(values[11], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                _spd = bkey;
            } else if (values[11] != "") {
                error.Add($"{values[11]} not found as SPD.");
            }

            if (Byte.TryParse(values[12], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                _aHit = bkey;
            } else if (values[12] != "") {
                error.Add($"{values[12]} not found as A_HIT.");
            }

            if (Byte.TryParse(values[13], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                _mHit = bkey;
            } else if (values[13] != "") {
                error.Add($"{values[13]} not found as M_HIT.");
            }

            if (Byte.TryParse(values[14], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                _aAV = bkey;
            } else if (values[14] != "") {
                error.Add($"{values[14]} not found as A_AV.");
            }

            if (Byte.TryParse(values[15], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                _mAV = bkey;
            } else if (values[15] != "") {
                error.Add($"{values[15]} not found as M_AV.");
            }

            errorTemp = new List<string>();
            foreach (string substring in values[16].Replace(" ", "").Split(',')) {
                if (Dictionary.Element2Num.TryGetValue(substring.ToLower(), out bkey)) {
                    _elementalResistance |= bkey;
                } else {
                    errorTemp.Add(substring);
                }
            }
            if (errorTemp.Count > 0) {
                error.Add($"{String.Join(", ", errorTemp)} not found as elemental resistance.");
            }

            errorTemp = new List<string>();
            foreach (string substring in values[17].Replace(" ", "").Split(',')) {
                if (Dictionary.Element2Num.TryGetValue(substring.ToLower(), out bkey)) {
                    _elementalImmunity |= bkey;
                } else {
                    errorTemp.Add(substring);
                }
            }
            if (errorTemp.Count > 0) {
                error.Add($"{String.Join(", ", errorTemp)} not found as elemental immunity.");
            }

            errorTemp = new List<string>();
            foreach (string substring in values[18].Replace(" ", "").Split(',')) {
                if (Dictionary.Status2Num.TryGetValue(substring.ToLower(), out bkey)) {
                    _statusResistance |= bkey;
                } else {
                    errorTemp.Add(substring);
                }
            }
            if (errorTemp.Count > 0) {
                error.Add($"{String.Join(", ", errorTemp)} not found as status resistance.");
            }

            errorTemp = new List<string>();
            foreach (string substring in values[19].Replace(" ", "").Split(',')) {
                if (specialBonuses1.TryGetValue(substring.ToLower(), out bkey)) {
                    _specialBonus1 |= bkey;
                } else {
                    errorTemp.Add(substring);
                }
            }
            if (errorTemp.Count > 0) {
                error.Add($"{String.Join(", ", errorTemp)} not found as special bonus 1.");
            }

            errorTemp = new List<string>();
            foreach (string substring in values[20].Replace(" ", "").Split(',')) {
                if (specialBonuses2.TryGetValue(substring.ToLower(), out bkey)) {
                    _specialBonus2 |= bkey;
                } else {
                    errorTemp.Add(substring);
                }
            }
            if (errorTemp.Count > 0) {
                error.Add($"{String.Join(", ", errorTemp)} not found as special bonus 2.");
            }

            if (Int16.TryParse(values[21], NumberStyles.AllowLeadingSign, null as IFormatProvider, out short skey)) {
                _specialBonusAmmount = skey;
            } else if (values[21] != "") {
                error.Add($"{values[15]} not found as special ammount.");
            }

            errorTemp = new List<string>();
            foreach (string substring in values[22].Replace(" ", "").Split(',')) {
                if (specialEffects.TryGetValue(substring.ToLower(), out bkey)) {
                    _specialEffect |= bkey;
                } else {
                    errorTemp.Add(substring);
                }
            }
            if (errorTemp.Count > 0) {
                error.Add($"{String.Join(", ", errorTemp)} not found as special effect.");
            }

            if (!(values[23] == "" || values[23] == " ")) {
                Description = values[23];
                EncodedDescription = Dictionary.EncodeText(Description) + " FF A0";
            }

            if (UInt16.TryParse(values[24], NumberStyles.AllowLeadingSign, null as IFormatProvider, out uskey)) {
                float temp = (float) uskey / 2;
                SellPrice = (short) Math.Round(temp);
            } else if (values[24] != "") {
               error.Add($"{values[24]} not found as price.");
            }

            if (error.Count > 0) {
                Constants.WriteError($"Item: {Name} - ID: {Id}");
                foreach(var e in error) {
                    Constants.WriteError($"\t{e}");
                }
            }
        }
    }
}
