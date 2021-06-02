using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dragoon_Modifier.LoDDictionary {
    public static class Dictionary {
        private static readonly Dictionary<string, string> _textCodes = new Dictionary<string, string>() {
                {"<END>", "FF A0" },
                {"<LINE>", "FF A1" },
                {"<GOLD>", "00 A8" },
                {"<WHITE>", "00 A7" },
                {"<RED>", "05 A7" },
                {"<YELLOW>", "08 A7" }
            };
        private static Dictionary<char, string> _symbols = new Dictionary<char, string> {
                {' ', "00 00" },
                {',', "01 00" },
                {'.', "02 00" },
                {'·', "03 00" },
                {':', "04 00" },
                {'?', "05 00" },
                {'!', "06 00" },
                {'_', "07 00" },
                {'/', "08 00" },
                {'\'', "09 00" },
                {'"', "0A 00" },
                {'(', "0B 00" },
                {')', "0C 00" },
                {'-', "0D 00" },
                {'`', "0E 00" },
                {'%', "0F 00" },
                {'&', "10 00" },
                {'*', "11 00" },
                {'@', "12 00" },
                {'+', "13 00" },
                {'~', "14 00" },
                {'0', "15 00" },
                {'1', "16 00" },
                {'2', "17 00" },
                {'3', "18 00" },
                {'4', "19 00" },
                {'5', "1A 00" },
                {'6', "1B 00" },
                {'7', "1C 00" },
                {'8', "1D 00" },
                {'9', "1E 00" },
                {'A', "1F 00" },
                {'B', "20 00" },
                {'C', "21 00" },
                {'D', "22 00" },
                {'E', "23 00" },
                {'F', "24 00" },
                {'G', "25 00" },
                {'H', "26 00" },
                {'I', "27 00" },
                {'J', "28 00" },
                {'K', "29 00" },
                {'L', "2A 00" },
                {'M', "2B 00" },
                {'N', "2C 00" },
                {'O', "2D 00" },
                {'P', "2E 00" },
                {'Q', "2F 00" },
                {'R', "30 00" },
                {'S', "31 00" },
                {'T', "32 00" },
                {'U', "33 00" },
                {'V', "34 00" },
                {'W', "35 00" },
                {'X', "36 00" },
                {'Y', "37 00" },
                {'Z', "38 00" },
                {'a', "39 00" },
                {'b', "3A 00" },
                {'c', "3B 00" },
                {'d', "3C 00" },
                {'e', "3D 00" },
                {'f', "3E 00" },
                {'g', "3F 00" },
                {'h', "40 00" },
                {'i', "41 00" },
                {'j', "42 00" },
                {'k', "43 00" },
                {'l', "44 00" },
                {'m', "45 00" },
                {'n', "46 00" },
                {'o', "47 00" },
                {'p', "48 00" },
                {'q', "49 00" },
                {'r', "4A 00" },
                {'s', "4B 00" },
                {'t', "4C 00" },
                {'u', "4D 00" },
                {'v', "4E 00" },
                {'w', "4F 00" },
                {'x', "50 00" },
                {'y', "51 00" },
                {'z', "52 00" },
                {'[', "53 00" },
                {']', "54 00" }
            };

        static Item[] _items = new Item[256];

        public static readonly Dictionary<byte, string> Num2Element = new Dictionary<byte, string>() {
                {0, "None" },
                {1, "Water" },
                {2, "Earth" },
                {4, "Dark" },
                {8, "Non-Elemental" },
                {16, "Thunder" },
                {32, "Light" },
                {64, "Wind" },
                {128, "Fire" }
            };
        public static readonly Dictionary<string, byte> Element2Num = new Dictionary<string, byte>() {
                {"", 0 },
                {"none", 0 },
                {"null", 0 },
                {"water", 1 },
                {"earth", 2 },
                {"dark", 4 },
                {"non-elemental", 8 },
                {"thunder", 16 },
                {"light", 32 },
                {"wind", 64 },
                {"fire", 128 }
            };
        public static readonly Dictionary<string, byte> Status2Num = new Dictionary<string, byte>() {
                {"none", 0 },
                {"", 0 },
                {"petrification", 1 },
                {"pe", 1 },
                {"bewitchment", 2 },
                {"be", 2 },
                {"confusion", 4 },
                {"cn", 4 },
                {"fear", 8 },
                {"fe", 8 },
                {"stun", 16 },
                {"st", 16 },
                {"armblocking", 32 },
                {"ab", 32 },
                {"dispirit", 64 },
                {"ds", 64 },
                {"poison", 128 },
                {"po", 128 },
                {"all", 255 }
            };
        public static readonly Dictionary<string, byte> DamageBase2Num = new Dictionary<string, byte>() {
            {"", 0x0 },
            {"none", 0x0 },
            {"0", 0x0},
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

        public static Item[] Items { get { return _items; } private set { _items = value; } }
        public static string EncodedNames { get; private set; }
        public static string EncodedDescriptions { get; private set; }
        public static string EncodedBattleNames { get; private set; }
        public static string EncodedBattleDescriptions { get; private set; }

        public static void Init(string mod) {
            string cwd = AppDomain.CurrentDomain.BaseDirectory;
            GetItems(cwd, mod);
        }

        public static bool Item2Num(string name, out byte id) {
            if (name == "" || name == " ") {
                id = 255;
                return true;
            }
            var item = Items.FirstOrDefault(x => x.Name.ToLower() == name.ToLower());
            if (item != null) {
                id = item.Id;
                return true;
            }
            id = 255;
            return false;
        }

        public static string Num2Item(byte id) {
            return Items[id].Name;
        }

        private static void GetEquipment(string cwd, string mod) {
            byte index = 0;
            using (var itemData = new StreamReader(cwd + "Mods/" + mod + "/Equipment.tsv")) {
                itemData.ReadLine(); // Skip the first line
                while (!itemData.EndOfStream && index < 192) {
                    var line = itemData.ReadLine();
                    var values = line.Split('\t').ToArray();
                    Items[index] = new Equipment(index, values);
                    index++;
                }
            }
        }

        private static void GetUsableItems(string cwd, string mod) {
            byte index = 192;
            using (var itemData = new StreamReader(cwd + "Mods/" + mod + "/Items.tsv")) {
                itemData.ReadLine(); // Skip the first line
                while (!itemData.EndOfStream && index != 0) {
                    var line = itemData.ReadLine();
                    var values = line.Split('\t').ToArray();
                    Items[index] = new UsableItem(index, values);
                    index++;
                }
            }
        }

        private static string CreateItemNameString() {
            int offset = 0;
            int start = Emulator.GetAddress("ITEM_NAME");
            LoDDictionary.Item[] sortedArr = Items.OrderByDescending(o => o.Name.Length).ToArray();
            List<string> stringList = new List<string>();
            foreach (LoDDictionary.Item item in sortedArr) {
                if (stringList.Any(l => l.Contains(item.EncodedName))) { // Item name is already a substring of a different 
                    int index = Array.IndexOf(sortedArr, Array.Find(sortedArr, x => x.EncodedName.Contains(item.EncodedName))); // Get index of matching string
                    item.NamePointer = (uint) (sortedArr[index].NamePointer + (sortedArr[index].Name.Length - item.Name.Length) * 2); // Account for different string length
                } else {
                    stringList.Add(item.EncodedName);
                    item.NamePointer = (uint) (start + offset);
                    offset += (item.EncodedName.Replace(" ", "").Length / 2);
                }
            }
            string result = String.Join(" ", stringList);
            long end = Constants.GetAddress("ITEM_NAME_PTR");
            int len1 = result.Replace(" ", "").Length / 4;
            int len2 = (int) (end - start) / 2;
            if (len1 >= len2) {
                Constants.WriteError($"Item name character limit exceeded! {len1} / {len2} characters. Turning off Name and Description changes.");
            }
            return result;

        }

        private static string CreateItemDescriptionString() {
            int offset = 0;
            int start = Emulator.GetAddress("ITEM_DESC");
            Item[] sortedArr = Items.OrderByDescending(o => o.Description.Length).ToArray();
            List<string> stringList = new List<string>();
            foreach (Item item in sortedArr) {
                if (stringList.Any(l => l.Contains(item.EncodedDescription))) {
                    int index = Array.IndexOf(sortedArr, Array.Find(sortedArr, x => x.EncodedDescription.Contains(item.EncodedDescription)));
                    item.DescriptionPointer = (uint) (sortedArr[index].DescriptionPointer + (sortedArr[index].Description.Length - item.Description.Length) * 2);
                } else {
                    stringList.Add(item.EncodedDescription);
                    item.DescriptionPointer = (uint) (start + offset);
                    offset += (item.EncodedDescription.Replace(" ", "").Length / 2);
                }
            }
            string result = String.Join(" ", stringList);
            long end = Constants.GetAddress("ITEM_DESC_PTR");
            int len1 = result.Replace(" ", "").Length / 4;
            int len2 = (int) (end - start) / 2;
            if (len1 >= len2) {
                Constants.WriteError($"Item description character limit exceeded! {len1} / {len2} characters. Turning off Name and Description changes.");
                Globals.ITEM_NAMEDESC_CHANGE = false;
            }
            return result;
        }

        private static string CreateItemBattleNameString() {
            int offset = 0;
            int start = Constants.GetAddress("ITEM_BTL_NAME");
            Item[] sortedArr = Items.Skip(193).Take(63).OrderBy(o => o.Description.Length).ToArray();
            List<string> stringList = new List<string>();
            foreach (UsableItem item in sortedArr) {
                if (stringList.Any(l => l.Contains(item.EncodedName))) {
                    int index = Array.IndexOf(sortedArr, Array.Find(sortedArr, x => x.EncodedName.Contains(item.EncodedName)));
                    item.BattleNamePointer = (int) (Convert.ToInt32(sortedArr[index].GetType().GetProperty("BattleNamePointer").GetValue(sortedArr[index])) + (sortedArr[index].Name.Length - item.Name.Length) * 2);
                } else {
                    stringList.Add(item.EncodedName);
                    item.BattleNamePointer = (int) (start + offset);
                    offset += (item.EncodedName.Replace(" ", "").Length / 2);
                }
            }
            string result = String.Join(" ", stringList);
            long end = Constants.GetAddress("ITEM_BTL_NAME_PTR");
            int len1 = result.Replace(" ", "").Length / 4;
            int len2 = (int) (end - start) / 2;
            if (len1 >= len2) {
                Constants.WriteError($"Item battle name character limit exceeded! {len1} / {len2} characters. Turning off Name and Description changes.");
                Globals.ITEM_NAMEDESC_CHANGE = false;
            }
            return result;
        }

        private static string CreateItemBattleDescriptionString() {
            long offset = 0;
            long start = Constants.GetAddress("ITEM_BTL_DESC");
            Item[] sortedArr = Items.Skip(193).Take(63).OrderBy(o => o.Description.Length).ToArray();
            List<string> stringList = new List<string>();
            foreach (UsableItem item in sortedArr) {
                if (stringList.Any(l => l.Contains(item.EncodedBattleDescription))) {
                    int index = Array.IndexOf(sortedArr, Array.Find(sortedArr, x => x.GetType().GetProperty("EncodedBattleDescription").GetValue(x).ToString().Contains(item.EncodedBattleDescription)));
                    item.BattleDescriptionPointer = (int) (Convert.ToInt32(sortedArr[index].GetType().GetProperty("BattleDescriptionPointer").GetValue(sortedArr[index])) + (sortedArr[index].GetType().GetProperty("BattleDescription").GetValue(sortedArr[index]).ToString().Length - item.BattleDescription.Length) * 2);
                } else {
                    stringList.Add(item.EncodedBattleDescription);
                    item.BattleDescriptionPointer = (int) (start + offset);
                    offset += (item.EncodedBattleDescription.Replace(" ", "").Length / 2);
                }
            }
            string result = String.Join(" ", stringList);
            long end = Constants.GetAddress("ITEM_BTL_DESC_PTR");
            int len1 = result.Replace(" ", "").Length / 4;
            int len2 = (int) (end - start) / 2;
            if (len1 >= len2) {
                Constants.WriteError($"Item battle description character limit exceeded! {len1} / {len2} characters. Turning off Name and Description changes.");
                Globals.ITEM_NAMEDESC_CHANGE = false;
            }
            return result;
        }

        private static void GetItems(string cwd, string mod) {
            try {
                GetEquipment(cwd, mod);
            } catch (Exception ex) {
                Constants.WriteError(ex);
            }

            try {
                GetUsableItems(cwd, mod);
            } catch (Exception ex) {
                Constants.WriteError(ex);
            }

            try {
                EncodedNames = CreateItemNameString();
                EncodedDescriptions = CreateItemDescriptionString();
                EncodedBattleNames = CreateItemBattleNameString();
                EncodedBattleDescriptions = CreateItemBattleDescriptionString();
            } catch (Exception ex) {
                Constants.WriteError(ex);
            }
        }

        public static string EncodeText(string text) {
            List<string> encoded = new List<string>();
            string[] parts = Regex.Split(text, @"(<[\s\S]+?>)").Where(l => l != string.Empty).ToArray();
            foreach (string segment in parts) {
                if (segment.StartsWith("<")) {
                    if (_textCodes.TryGetValue(segment, out var key)) {
                        encoded.Add(key);
                    } else {
                        encoded.Add("00 00");
                        Constants.WriteError($"Encoding table doesn't include {segment} code.");
                    }
                } else {
                    List<string> temp = new List<string>();
                    foreach (char letter in segment) {
                        if (_symbols.TryGetValue(letter, out var key)) {
                            temp.Add(key);
                        } else {
                            temp.Add("00 00");
                            Constants.WriteError($"Encoding table doesn't include {letter} symbol.");
                        }
                    }
                }
            }
            return String.Join(" ", encoded);
        }

    }
}
