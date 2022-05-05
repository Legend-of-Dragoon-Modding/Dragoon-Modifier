using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Dataset {
    internal class Monster : IMonster {
        private static readonly Dictionary<string, byte> _specialEffects = new Dictionary<string, byte>() {
            { "", 0 },
            { "none", 0 },
            { "0", 0 },
            { "unknown1", 1 },
            { "unknown2", 2 },
            { "unknown3", 4 },
            { "unknown4", 8 },
            { "unknown5", 16 },
            { "unknown6", 32 },
            { "unknown7", 64 },
            { "death_resist", 128 },
        };
        public string Name { get; private set; } = "Monster";
        public byte Element { get; private set; } = 128;
        public uint HP { get; private set; } = 1;
        public ushort AT { get; private set; } = 1;
        public ushort MAT { get; private set; } = 1;
        public ushort DF { get; private set; } = 1;
        public ushort MDF { get; private set; } = 1;
        public ushort SPD { get; private set; } = 1;
        public short A_AV { get; private set; } = 0;
        public short M_AV { get; private set; } = 0;
        public byte PhysicalImmunity { get; private set; } = 0;
        public byte PhysicalResistance { get; private set; } = 0;
        public byte MagicalImmunity { get; private set; } = 0;
        public byte MagicalResistance { get; private set; } = 0;
        public byte ElementalImmunity { get; private set; } = 0;
        public byte ElementalResistance { get; private set; } = 0;
        public byte StatusResist { get; private set; } = 99;
        public byte SpecialEffect { get; private set; } = 0;
        public ushort EXP { get; private set; } = 0;
        public ushort Gold { get; private set; } = 0;
        public byte DropItem { get; private set; } = 255;
        public byte DropChance { get; private set; } = 0;

        internal Monster(string[] values, LoDDictionary.MyFunc<string, byte> tryEncodeItem) {
            var error = new List<string>();

            Name = values[1];

            if (LoDDictionary.TryEncodeElement(values[2], out var bkey)) {
                Element = bkey;
            } else {
                error.Add($"{values[2]} not found as Element.");
            }

            if (UInt32.TryParse(values[3], out var uikey)) {
                HP = uikey;
            } else {
                error.Add($"{values[3]} couldn't be parsed as HP.");
            }

            if (UInt16.TryParse(values[4], out var uskey)) {
                AT = uskey;
            } else {
                error.Add($"{values[4]} couldn't be parsed as AT.");
            }

            if (UInt16.TryParse(values[5], out uskey)) {
                MAT = uskey;
            } else {
                error.Add($"{values[5]} couldn't be parsed as MAT.");
            }

            if (UInt16.TryParse(values[6], out uskey)) {
                DF = uskey;
            } else {
                error.Add($"{values[6]} couldn't be parsed as DF.");
            }

            if (UInt16.TryParse(values[7], out uskey)) {
                MDF = uskey;
            } else {
                error.Add($"{values[7]} couldn't be parsed as MDF.");
            }

            if (UInt16.TryParse(values[8], out uskey)) {
                SPD = uskey;
            } else {
                error.Add($"{values[8]} couldn't be parsed as SPD.");
            }

            if (Int16.TryParse(values[9], NumberStyles.AllowLeadingSign, null as IFormatProvider, out var skey)) {
                A_AV = skey;
            } else {
                error.Add($"{values[9]} couldn't be parsed as A_AV.");
            }

            if (Int16.TryParse(values[10], NumberStyles.AllowLeadingSign, null as IFormatProvider, out skey)) {
                M_AV = skey;
            } else {
                error.Add($"{values[10]} couldn't be parsed as M_AV.");
            }

            if (Byte.TryParse(values[11], out bkey)) {
                PhysicalImmunity = bkey;
            } else {
                error.Add($"{values[11]} couldn't be parsed as Physical Immunity.");
            }

            if (Byte.TryParse(values[12], out bkey)) {
                MagicalImmunity = bkey;
            } else {
                error.Add($"{values[12]} couldn't be parsed as Magical Immunity.");
            }

            if (Byte.TryParse(values[13], out bkey)) {
                PhysicalResistance = bkey;
            } else {
                error.Add($"{values[13]} couldn't be parsed as Physical Resistance.");
            }

            if (Byte.TryParse(values[14], out bkey)) {
                MagicalResistance = bkey;
            } else {
                error.Add($"{values[14]} couldn't be parsed as Magical Resistance.");
            }

            var errorTemp = new List<string>();
            foreach (string sub in values[15].Replace(" ", "").ToLower().Split(',')) {
                if (LoDDictionary.TryEncodeElement(sub, out bkey)) {
                    ElementalImmunity |= bkey;
                } else if (Byte.TryParse(sub, out bkey)) {
                    ElementalImmunity |= bkey;
                } else {
                    errorTemp.Add(sub);
                }
            }
            if (errorTemp.Count != 0) {
                error.Add($"{String.Join(", ", errorTemp)} not found as Elemental Immunity.");
            }

            errorTemp = new List<string>();
            foreach (string sub in values[16].Replace(" ", "").ToLower().Split(',')) {
                if (LoDDictionary.TryEncodeElement(sub, out bkey)) {
                    ElementalResistance |= bkey;
                } else if (Byte.TryParse(sub, out bkey)) {
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
                if (LoDDictionary.TryEncodeStatus(sub, out bkey)) {
                    StatusResist |= bkey;
                } else if (Byte.TryParse(sub, out bkey)) {
                    StatusResist |= bkey;
                } else {
                    errorTemp.Add(sub);
                }
            }
            if (errorTemp.Count != 0) {
                error.Add($"{String.Join(", ", errorTemp)} not found as Status Resist.");
            }

            errorTemp = new List<string>();
            foreach (string sub in values[18].Replace(" ", "").ToLower().Split(',')) {
                if (_specialEffects.TryGetValue(sub, out bkey)) {
                    SpecialEffect |= bkey;
                } else if (Byte.TryParse(sub, out bkey)) {
                    SpecialEffect |= bkey;
                } else {
                    errorTemp.Add(sub);
                }
            }
            if (errorTemp.Count != 0) {
                error.Add($"{String.Join(", ", errorTemp)} not found as Special Effect.");
            }

            if (UInt16.TryParse(values[19], out uskey)) {
                EXP = uskey;
            } else {
                error.Add($"{values[19]} couldn't be parsed as EXP.");
            }

            if (UInt16.TryParse(values[20], out uskey)) {
                Gold = uskey;
            } else {
                error.Add($"{values[20]} couldn't be parsed as Gold.");
            }

            if (tryEncodeItem(values[21], out bkey)) {
                DropItem = bkey;
            } else {
                error.Add($"{values[21]} not found as Item");
            }

            if (Byte.TryParse(values[22], out bkey)) {
                DropChance = bkey;
            } else {
                error.Add($"{values[22]} couldn't be parsed as Drop Chance.");
            }

            if (error.Count != 0) {
                Console.WriteLine($"[ERROR] Monster {values[0]} - {Name}");
                foreach (var er in error) {
                    Console.WriteLine($"\t{er}");
                }
            }
        }
    }
}
