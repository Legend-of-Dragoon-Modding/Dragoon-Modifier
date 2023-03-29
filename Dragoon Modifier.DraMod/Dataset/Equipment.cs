using Dragoon_Modifier.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Dataset {
    internal sealed class Equipment : Item, IEquipment {
        private static readonly Dictionary<string, byte> _types = new Dictionary<string, byte>() {
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
        private static readonly Dictionary<string, byte> _partyMembers = new Dictionary<string, byte>() {
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
        private static readonly Dictionary<string, byte> _specials1 = new Dictionary<string, byte>() {
            {"", 0 },
            {"none", 0 },
            {"mp_m_hit", 1 },
            {"sp_m_hit", 2 },
            {"mp_p_hit", 4 },
            {"sp_p_hit", 8 },
            {"sp_multi", 16 },
            {"p_half", 32 }
        };
        private static readonly Dictionary<string, byte> _specials2 = new Dictionary<string, byte>() {
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
        private static readonly Dictionary<string, byte> _specialEffects = new Dictionary<string, byte>() {
            {"", 0 },
            {"none", 0 },
            {"cannot_sell", 4 },
            {"attack_all", 8 },
            {"death_chance", 64 },
            {"death_res", 128 }
        };

        public byte WhoEquips { get; private set; } = 0;
        public byte Type { get; private set; } = 0;
        public byte WeaponElement { get; private set; } = 0;
        public byte OnHitStatus { get; private set; } = 0;
        public byte OnHitStatusChance { get; private set; } = 0;
        public ushort AT { get; private set; } = 0;
        public byte MAT { get; private set; } = 0;
        public byte DF { get; private set; } = 0;
        public byte MDF { get; private set; } = 0;
        public byte SPD { get; private set; } = 0;
        public byte A_HIT { get; private set; } = 0;
        public byte M_HIT { get; private set; } = 0;
        public byte A_AV { get; private set; } = 0;
        public byte M_AV { get; private set; } = 0;
        public byte ElementalResistance { get; private set; } = 0;
        public byte ElementalImmunity { get; private set; } = 0;
        public byte StatusResistance { get; private set; } = 0;
        public byte SpecialBonus1 { get; private set; } = 0;
        public byte SpecialBonus2 { get; private set; } = 0;
        public byte SpecialBonusAmount { get; private set; } = 0;
        public byte SpecialEffect { get; private set; } = 0;

        internal Equipment(byte index, string[] values) {
            var error = new List<string>();

            ID = index;

            if (!(values[0] == "" || values[0] == " ")) {
                Name = values[0];
                EncodedName = Emulator.TextEncoding.GetBytes(Name).Concat(new byte[] { 0xFF, 0xA0 }).ToArray();
            }

            if (_types.TryGetValue(values[1].ToLower(), out byte bkey)) {
                Type = bkey;
            } else {
                error.Add($"{values[1]} not found as equipment type.");
            }

            var errorTemp = new List<string>();
            foreach (string sub in values[2].Replace(" ", "").ToLower().Split(',')) {
                if (_partyMembers.TryGetValue(sub, out bkey)) {
                    WhoEquips |= bkey;
                } else {
                    errorTemp.Add(sub);
                }
            }
            if (errorTemp.Count != 0) {
                error.Add($"{String.Join(", ", errorTemp)} not found as character.");
            }

            if (LoDDictionary.TryEncodeIcon(values[3].ToLower(), out bkey)) {
                Icon = bkey;
            } else {
                error.Add($"{values[3]} not found as Icon.");
            }

            if (LoDDictionary.TryEncodeElement(values[4].ToLower(), out bkey)) {
                WeaponElement = bkey;
            } else {
                error.Add($"{values[4]} not found as Weapon Element");
            }

            if (LoDDictionary.TryEncodeStatus(values[5].ToLower(), out bkey)) {
                OnHitStatus = bkey;
            } else {
                error.Add($"{values[5]} not found as On Hit Status");
            }

            if (Byte.TryParse(values[6], out bkey)) {
                OnHitStatusChance = bkey;
            } else if (values[6] != "") {
                error.Add($"Couldn't parse {values[6]} as on hit status chance.");
            }

            if (UInt16.TryParse(values[7], out var uskey)) {
                AT = uskey;
            } else if (values[7] != "") {
                error.Add($"Couldn't parse {values[7]} as AT.");
            }

            if (Byte.TryParse(values[8], out bkey)) {
                MAT = bkey;
            } else if (values[8] != "") {
                error.Add($"Couldn't parse {values[8]} as MAT.");
            }

            if (Byte.TryParse(values[9], out bkey)) {
                DF = bkey;
            } else if (values[9] != "") {
                error.Add($"Couldn't parse {values[9]} as DF.");
            }

            if (Byte.TryParse(values[10], out bkey)) {
                MDF = bkey;
            } else if (values[10] != "") {
                error.Add($"Couldn't parse {values[10]} as MDF.");
            }

            if (Byte.TryParse(values[11], out bkey)) {
                SPD = bkey;
            } else if (values[11] != "") {
                error.Add($"Couldn't parse {values[11]} as SPD.");
            }

            if (Byte.TryParse(values[12], out bkey)) {
                A_HIT = bkey;
            } else if (values[12] != "") {
                error.Add($"Couldn't parse {values[12]} as A_HIT.");
            }

            if (Byte.TryParse(values[13], out bkey)) {
                M_HIT = bkey;
            } else if (values[13] != "") {
                error.Add($"Couldn't parse {values[13]} as M_HIT.");
            }

            if (Byte.TryParse(values[14], out bkey)) {
                A_AV = bkey;
            } else if (values[14] != "") {
                error.Add($"Couldn't parse {values[14]} as A_AV.");
            }

            if (Byte.TryParse(values[15], out bkey)) {
                M_AV = bkey;
            } else if (values[15] != "") {
                error.Add($"Couldn't parse {values[15]} as A_AV.");
            }

            errorTemp = new List<string>();
            foreach (string sub in values[16].Replace(" ", "").ToLower().Split(',')) {
                if (LoDDictionary.TryEncodeElement(sub, out bkey)) {
                    ElementalResistance |= bkey;
                } else {
                    errorTemp.Add(sub);
                }
            }
            if (errorTemp.Count != 0) {
                error.Add($"{String.Join(", ", errorTemp)} not found as Elemental Resistance.");
            }

            errorTemp = new List<string>();
            foreach (string sub in values[17].Replace(" ", "").ToLower().Split(',')) {
                if (LoDDictionary.TryEncodeElement(sub, out bkey)) {
                    ElementalImmunity |= bkey;
                } else {
                    errorTemp.Add(sub);
                }
            }
            if (errorTemp.Count != 0) {
                error.Add($"{String.Join(", ", errorTemp)} not found as Elemental Immunity.");
            }

            errorTemp = new List<string>();
            foreach (string sub in values[18].Replace(" ", "").ToLower().Split(',')) {
                if (LoDDictionary.TryEncodeStatus(sub, out bkey)) {
                    StatusResistance |= bkey;
                } else {
                    errorTemp.Add(sub);
                }
            }
            if (errorTemp.Count != 0) {
                error.Add($"{String.Join(", ", errorTemp)} not found as Status Resistance.");
            }

            errorTemp = new List<string>();
            foreach (string sub in values[19].Replace(" ", "").ToLower().Split(',')) {
                if (_specials1.TryGetValue(sub, out bkey)) {
                    SpecialBonus1 |= bkey;
                } else {
                    errorTemp.Add(sub);
                }
            }
            if (errorTemp.Count != 0) {
                error.Add($"{String.Join(", ", errorTemp)} not found as Special 1.");
            }

            errorTemp = new List<string>();
            foreach (string sub in values[20].Replace(" ", "").ToLower().Split(',')) {
                if (_specials2.TryGetValue(sub, out bkey)) {
                    SpecialBonus2 |= bkey;
                } else {
                    errorTemp.Add(sub);
                }
            }
            if (errorTemp.Count != 0) {
                error.Add($"{String.Join(", ", errorTemp)} not found as Special 1.");
            }

            if (Int16.TryParse(values[21], NumberStyles.AllowLeadingSign, null as IFormatProvider, out short skey)) {
                SpecialBonusAmount = (byte) skey;
            } else if (values[21] != "") {
                error.Add($"{values[21]} couldn't be parsed as special ammount");
            }

            errorTemp = new List<string>();
            foreach (string sub in values[22].Replace(" ", "").ToLower().Split(',')) {
                if (_specialEffects.TryGetValue(sub, out bkey)) {
                    SpecialEffect |= bkey;
                } else {
                    errorTemp.Add(sub);
                }
            }
            if (errorTemp.Count != 0) {
                error.Add($"{String.Join(", ", errorTemp)} not found as Special Effect.");
            }

            if (!(values[23] == "" || values[23] == " ")) {
                Description = values[23];

                EncodedDescription = Emulator.TextEncoding.GetBytes(Description).Concat(new byte[] { 0xFF, 0xA0 }).ToArray();
            }

            if (UInt16.TryParse(values[24], out uskey)) {
                SellPrice = (short) Math.Round((float) uskey / 2);
            } else if (values[24] != "") {
                error.Add($"{values[24]} couldn't be parsed as Price.");
            }

            if (error.Count != 0) {
                Console.WriteLine($"[ERROR] Item {ID} - {Name}");
                foreach (var er in error) {
                    Console.WriteLine($"\t{er}");
                }
            }
        }
    }
}
