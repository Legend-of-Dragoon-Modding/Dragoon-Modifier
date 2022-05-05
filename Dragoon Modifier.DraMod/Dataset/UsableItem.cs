using Dragoon_Modifier.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Dataset {
    internal class UsableItem : Item, IUsableItem {
        private static readonly Dictionary<string, byte> _damageBases = new Dictionary<string, byte>() {
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
        private static readonly Dictionary<string, byte> _specials1 = new Dictionary<string, byte>() {
            {"", 0 },
            {"none", 0 },
            {"power1", 0x10 },
            {"power2", 0x20 },
            {"power3", 0x40 },
            {"power4", 0x80 }
        };
        private static readonly Dictionary<string, byte> _specials2 = new Dictionary<string, byte>() {
            {"", 0 },
            {"none", 0 },
            {"speed_down", 0x10 },
            {"speed_up", 0x20 },
            {"magic_res", 0x40 },
            {"physical_res", 0x80 }
        };
        private static readonly Dictionary<string, byte> _baseSwitches = new Dictionary<string, byte>() {
            {"", 0x0 },
            {"none", 0x0 },
            {"status_cause", 0x4 },
            {"status_cure", 0x8 },
            {"revive", 0x10 },
            {"sp", 0x20 },
            {"mp", 0x40 },
            {"hp", 0x80 }
        };

        public string BattleDescription { get; private set; } = "<END>";
        public byte[] EncodedBattleDescription { get; private set; } = new byte[] { 0xFF, 0xA0, 0xFF, 0xA0 };
        public int BattleDescriptionPointer { get; set; } = 0;
        public int BattleNamePointer { get; set; } = 0;
        public byte Target { get; private set; } = 0;
        public byte Element { get; private set; } = 0;
        public byte Damage { get; private set; } = 0;
        public byte Special1 { get; private set; } = 0;
        public byte Special2 { get; private set; } = 0;
        public byte Unknown1 { get; private set; } = 0;
        public byte SpecialAmmount { get; private set; } = 0;
        public byte Status { get; private set; } = 0;
        public byte Percentage { get; private set; } = 0;
        public byte Unknown2 { get; private set; } = 0;
        public byte BaseSwitch { get; private set; } = 0;

        internal UsableItem(byte index, string[] values) {
            List<string> error = new List<string>();

            ID = index;

            if (!(values[0] == "" || values[0] == " ")) {
                Name = values[0];
                EncodedName = Emulator.TextEncoding.GetBytes(Name).Concat(new byte[] { 0xFF, 0xA0 }).ToArray();
            }

            if (Byte.TryParse(values[1], out var bkey)) {
                Target = bkey;
            } else if (values[1] != "") {
                error.Add($"{values[1]} couldn't be parsed as Target.");
            }

            if (LoDDictionary.TryEncodeElement(values[2], out bkey)) {
                Element = bkey;
            } else if (values[2] != "") {
                error.Add($"{values[2]} not found as Element.");
            }

            if (_damageBases.TryGetValue(values[3], out bkey)) {
                Damage = bkey;
            } else if (values[3] != "") {
                error.Add($"{values[3]} not found as Damage Base.");
            }

            var errorTemp = new List<string>();
            foreach (string sub in values[4].Replace(" ", "").ToLower().Split(',')) {
                if (_specials1.TryGetValue(sub, out bkey)) {
                    Special1 |= bkey;
                } else {
                    errorTemp.Add(sub);
                }
            }
            if (errorTemp.Count != 0) {
                error.Add($"{String.Join(", ", errorTemp)} not found as Special 1.");
            }

            errorTemp = new List<string>();
            foreach (string sub in values[5].Replace(" ", "").ToLower().Split(',')) {
                if (_specials2.TryGetValue(sub, out bkey)) {
                    Special2 |= bkey;
                } else {
                    errorTemp.Add(sub);
                }
            }
            if (errorTemp.Count != 0) {
                error.Add($"{String.Join(", ", errorTemp)} not found as Special 1.");
            }

            if (Byte.TryParse(values[6], out bkey)) {
                Unknown1 = bkey;
            } else if (values[6] != "") {
                error.Add($"{values[6]} couldn't be parsed as Unknown 1.");
            }

            if (Int16.TryParse(values[7], NumberStyles.AllowLeadingSign, null as IFormatProvider, out short skey)) {
                SpecialAmmount = (byte) skey;
            } else if (values[7] != "") {
                error.Add($"{values[7]} couldn't be parsed as Special Ammount");
            }

            if (LoDDictionary.TryEncodeIcon(values[8].ToLower(), out bkey)) {
                Icon = bkey;
            } else {
                error.Add($"{values[8]} not found as Icon.");
            }

            if (LoDDictionary.TryEncodeStatus(values[9], out bkey)) {
                Status = bkey;
            } else if (values[9] != "") {
                error.Add($"{values[9]} not found as Status.");
            }

            if (Byte.TryParse(values[10], out bkey)) {
                Percentage = bkey;
            } else if (values[10] != "") {
                error.Add($"{values[10]} couldn't be parsed as Percentage.");
            }

            if (Byte.TryParse(values[11], out bkey)) {
                Unknown2 = bkey;
            } else if (values[11] != "") {
                error.Add($"{values[11]} couldn't be parsed as Unknown 2.");
            }

            errorTemp = new List<string>();
            foreach (string sub in values[12].Replace(" ", "").ToLower().Split(',')) {
                if (_baseSwitches.TryGetValue(sub, out bkey)) {
                    BaseSwitch |= bkey;
                } else {
                    errorTemp.Add(sub);
                }
            }
            if (errorTemp.Count != 0) {
                error.Add($"{String.Join(", ", errorTemp)} not found as Base Switch.");
            }

            if (!(values[13] == "" || values[13] == " ")) {
                Description = values[13];
                EncodedDescription = Emulator.TextEncoding.GetBytes(Description).Concat(new byte[] { 0xFF, 0xA0 }).ToArray();
            }

            if (UInt16.TryParse(values[14], out var uskey)) {
                SellPrice = (short) Math.Round((float) uskey / 2);
            } else if (values[14] != "") {
                error.Add($"{values[14]} couldn't be parsed as Price.");
            }

            if (!(values[15] == "" || values[15] == " ")) {
                BattleDescription = values[15];
                EncodedBattleDescription = Emulator.TextEncoding.GetBytes(BattleDescription).Concat(new byte[] { 0xFF, 0xA0 }).ToArray();
            }
        }
    }
}
