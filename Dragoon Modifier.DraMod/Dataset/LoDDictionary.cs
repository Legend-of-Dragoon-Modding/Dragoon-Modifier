using Dragoon_Modifier.Core;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Dataset {
    internal sealed class LoDDictionary : ILoDDictionary {
        private static readonly Dictionary<string, byte> _status2Num = new() {
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
        private static readonly Dictionary<byte, string> _num2Status = new() {
            {0, "None" },
            {1, "Petrification" },
            {2, "Bewitchment" },
            {4, "Confusion" },
            {8, "Fear" },
            {16, "Stun" },
            {32, "Armblocking" },
            {64, "Dispirit" },
            {128, "Poison" }
        };
        private static readonly Dictionary<string, byte> _element2Num = new() {
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
        private static readonly Dictionary<byte, string> _num2Element = new() {
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
        private static readonly Dictionary<string, byte> _icon2Num = new() {
            { "sword", 0 },
            { "axe", 1 },
            { "hammer", 2 },
            { "spear", 3 },
            { "bow", 4 },
            { "mace", 5 },
            { "knuckle", 6 },
            { "boxing glove", 7 },
            { "clothes", 8 },
            { "robe", 9 },
            { "armor", 10 },
            { "breastplate", 11 },
            { "red dress", 12 },
            { "loincloth", 13 },
            { "warrior dress", 14 },
            { "crown", 15 },
            { "hairband", 16 },
            { "bandana", 16 },
            { "hat", 17 },
            { "helm", 18 },
            { "shoes", 19 },
            { "kneepiece", 20 },
            { "boots", 21 },
            { "bracelet", 22 },
            { "ring", 23 },
            { "amulet", 24 },
            { "stone", 25 },
            { "jewellery", 26 },
            { "ankh", 27 },
            { "bell", 28 },
            { "bag", 29 },
            { "cloak", 30 },
            { "scarf", 30 },
            { "glove", 31 },
            { "horn", 32 },
            { "blue potion", 33 },
            { "yellow potion", 34 },
            { "red potion", 35 },
            { "angel's prayer", 36 },
            { "green potion", 37 },
            { "magic", 38 },
            { "skull", 39 },
            { "up", 40 },
            { "down", 41 },
            { "shield", 42 },
            { "smoke ball", 43 },
            { "sig stone", 44 },
            { "charm", 45 },
            { "sack", 46 },
            { "invalid", 57 },
            { "waring", 58 },
            {"none", 64 },
            {"", 64 }
        };

        private readonly string _cwd;
        private readonly string _mod;
        private readonly string? _secondaryMod;
        private bool _dualMonster = false;
        private Dictionary<ushort, IMonster> _monsters = new();
        private readonly Dictionary<ushort, IMonster> _secondaryMonsters = new();

        public IItem[] Item { get; } = new IItem[256];
        public Dictionary<ushort, IMonster> Monster {
            get {
                if (!_dualMonster || _secondaryMonsters.Count == 0) {
                    return _monsters;
                }
                return _secondaryMonsters;
            }
        }

        public ICharacter[] Character { get; } = new ICharacter[9];

        public byte[] ItemNames { get; private set; } = new byte[0];
        public byte[] ItemDescriptions { get; private set; } = new byte[0];
        public byte[] ItemBattleNames { get; private set; } = new byte[0];
        public byte[] ItemBattleDescriptions { get; private set; } = new byte[0];

        public List<byte>[] Shop { get; } = new List<byte>[45];

        public Scripts.IScript Script { get; private set; } = new Scripts.DummyScript();

        internal delegate bool MyFunc<T1, T2>(T1 a, out T2 b);
        private MyFunc<string, byte> _tryEncodeItemDelegate;

        internal LoDDictionary(string cwd, string mod) {
            _cwd = cwd;
            _mod = mod;
            _dualMonster = false;
            _secondaryMod = null;
            _secondaryMonsters = new();

            ParseScript();

            Load();
        }

        internal LoDDictionary(string cwd, string mod, Scripts.IScript script, bool dualMonsters, string dualMod) {
            _cwd = cwd;
            _mod = mod;
            _secondaryMod = dualMod;
            _dualMonster = dualMonsters;

            Script = script;

            Load();

            if (dualMonsters) {
                _secondaryMonsters = new();
                string modPath = $"{_cwd}\\Mods\\{_secondaryMod}";
                GetMonsters(modPath, ref _secondaryMonsters);
            } else {
                _secondaryMonsters = new();
            }

        }

        private void ParseScript() {
            foreach (var file in Directory.GetFiles($"{_cwd}\\Mods\\{_mod}", "*.cs")) {
                if (file.Equals("Script.cs")) {
                    try {
                        // Script = new Scripts.CustomScript(file, this);
                        Console.WriteLine("Custom script inserted.");
                        return;
                    } catch (ApplicationException ex) {
                        Console.WriteLine($"[ERROR] Script {file} not compatible.");
                        Console.WriteLine($"[ERROR] {ex}");
                    }
                }
            }
        }

        private void Load() {
            string modPath = $"{_cwd}\\Mods\\{_mod}";
            _tryEncodeItemDelegate = TryEncodeItem;

            GetItems(modPath);
            GetMonsters(modPath, ref _monsters);
            GetCharacters(modPath);
            GetShops(modPath);
        }

        public static string GetStatus(byte status) {
            if (_num2Status.TryGetValue(status, out var result)) {
                return result;
            }
            Console.WriteLine($"[ERROR] {status} not found as Status.");
            return String.Empty;
        }

        public static byte EncodeStatus(string status) {
            if (_status2Num.TryGetValue(status.ToLower(), out var result)) {
                return result;
            }
            Console.WriteLine($"[ERROR] {status} couldn't be encoded as Status.");
            return 0;
        }

        public static bool TryEncodeStatus(string status, out byte result) {
            if (_status2Num.TryGetValue(status.ToLower(), out result)) {
                return true;
            }
            result = 0;
            return false;
        }

        public static string GetElement(byte element) {
            if (_num2Element.TryGetValue(element, out var result)) {
                return result;
            }
            Console.WriteLine($"[ERROR] {element} not found as Element.");
            return String.Empty;
        }

        public static byte EncodeElement(string element) {
            if (_element2Num.TryGetValue(element.ToLower(), out var result)) {
                return result;
            }
            Console.WriteLine($"[ERROR] {element} couldn't be encoded as Element.");
            return 0;
        }

        public static bool TryEncodeElement(string element, out byte result) {
            if (_element2Num.TryGetValue(element.ToLower(), out result)) {
                return true;
            }
            result = 0;
            return false;
        }

        public static byte EncodeIcon(string icon) {
            if (_icon2Num.TryGetValue(icon.ToLower(), out var result)) {
                return result;
            }
            Console.WriteLine($"[ERROR] {icon} couldn't be encoded as Icon.");
            return 0;
        }

        public static bool TryEncodeIcon(string icon, out byte result) {
            if (_icon2Num.TryGetValue(icon.ToLower(), out result)) {
                return true;
            }
            result = 0;
            return false;
        }

        public bool TryEncodeItem(string name, out byte id) {
            if (name == "" || name == " ") {
                id = 255;
                return true;
            }
            var item = Item.FirstOrDefault(x => x.Name.ToLower() == name.ToLower());
            if (item != null) {
                id = item.ID;
                return true;
            }
            id = 255;
            return false;
        }

        private void GetItems(string modPath) {
            GetEquipment(modPath);
            GetUsableItems(modPath);

            ItemNames = GetEncodedNames();
            ItemDescriptions = GetEncodedDescriptions();
            ItemBattleNames = GetEncodedBattleNames();
            ItemBattleDescriptions = GetEncodedBattleDescriptions();
        }

        private void GetEquipment(string modPath) {
            string file = $"{modPath}\\Equipment.tsv";
            int i = 0;
            try {
                using (var itemData = new StreamReader(file)) {
                    itemData.ReadLine(); // Skip first line
                    while (!itemData.EndOfStream) {
                        var line = itemData.ReadLine();
                        var values = line.Split('\t').ToArray();
                        Item[i] = new Equipment((byte) i, values); // TODO Factory
                        i++;
                    }
                }
            } catch (IOException) {
                Console.WriteLine($"[ERROR] {file} not found.");
            } catch (IndexOutOfRangeException) {
                Console.WriteLine($"[ERROR] Incorrect fromat of {file} at line {i + 1}");
            }
        }

        private void GetUsableItems(string modPath) {
            string file = $"{modPath}\\Items.tsv";
            int i = 192;
            try {
                using (var itemData = new StreamReader(file)) {
                    itemData.ReadLine(); // Skip first line
                    while (!itemData.EndOfStream) {
                        var line = itemData.ReadLine();
                        var values = line.Split('\t').ToArray();
                        Item[i] = new UsableItem((byte) i, values); // TODO Factory
                        i++;
                    }
                }
            } catch (IOException) {
                Console.WriteLine($"[ERROR] {file} not found.");
            } catch (IndexOutOfRangeException) {
                Console.WriteLine($"[ERROR] Incorrect fromat of {file} at line {i + 1}");
            }
        }

        private byte[] GetEncodedNames() {
            int start = Emulator.GetAddress("ITEM_NAME");

            var sorted = Item.OrderByDescending(item => item.Name.Length);
            byte[] result = new byte[0];

            foreach (var item in sorted) {
                var search = KMP.UnmaskedSearch(item.EncodedName, result);
                if (search.Count != 0) {
                    item.NamePointer = start + (int) search[0];
                } else {
                    item.NamePointer = start + result.Length;
                    result = result.Concat(item.EncodedName).ToArray();
                }
            }

            var end = Emulator.GetAddress("ITEM_NAME_PTR");
            if (result.Length > end - start) {
                Console.WriteLine($"Item name character limit exceeded! {result.Length} / {end - start} characters. Turning off Name and Description changes.");
                Settings.ItemNameDescChange = false;
            }

            return result;
        }

        private byte[] GetEncodedDescriptions() {
            int start = Emulator.GetAddress("ITEM_DESC"); ;

            var sorted = Item.OrderByDescending(item => item.Description.Length);
            byte[] result = new byte[0];

            foreach (var item in sorted) {
                var search = KMP.UnmaskedSearch(item.EncodedDescription, result);
                if (search.Count != 0) {
                    item.DescriptionPointer = start + (int) search[0];
                } else {
                    item.DescriptionPointer = start + result.Length;
                    result = result.Concat(item.EncodedDescription).ToArray();
                }
            }

            var end = Emulator.GetAddress("ITEM_DESC_PTR");
            if (result.Length > end - start) {
                Console.WriteLine($"Item description character limit exceeded! {result.Length} / {end - start} characters. Turning off Name and Description changes.");
                Settings.ItemNameDescChange = false;
            }

            return result;
        }

        private byte[] GetEncodedBattleNames() {
            int start = Emulator.GetAddress("ITEM_BTL_NAME");

            var sorted = Item.Where(item => item is IUsableItem).Cast<IUsableItem>().OrderByDescending(item => item.Name.Length);
            byte[] result = new byte[0];

            foreach (var item in sorted) {
                var search = KMP.UnmaskedSearch(item.EncodedName, result);
                if (search.Count != 0) {
                    item.BattleNamePointer = start + (int) search[0];
                } else {
                    item.BattleNamePointer = start + result.Length;
                    result = result.Concat(item.EncodedName).ToArray();
                }
            }

            var end = Emulator.GetAddress("ITEM_BTL_NAME_PTR");
            if (result.Length > end - start) {
                Console.WriteLine($"Item battle name character limit exceeded! {result.Length} / {end - start} characters. Turning off Name and Description changes.");
                Settings.ItemNameDescChange = false;
            }

            return result;
        }

        private byte[] GetEncodedBattleDescriptions() {
            int start = Emulator.GetAddress("ITEM_BTL_DESC");

            var sorted = Item.Where(item => item is IUsableItem).Cast<IUsableItem>().OrderByDescending(item => item.BattleDescription.Length);
            byte[] result = new byte[0];

            foreach (var item in sorted) {
                var search = KMP.UnmaskedSearch(item.EncodedBattleDescription, result);
                if (search.Count != 0) {
                    item.BattleDescriptionPointer = start + (int) search[0];
                } else {
                    item.BattleDescriptionPointer = start + result.Length;
                    result = result.Concat(item.EncodedBattleDescription).ToArray();
                }
            }

            var end = Emulator.GetAddress("ITEM_BTL_DESC_PTR");
            if (result.Length > end - start) {
                Console.WriteLine($"Item battle description character limit exceeded! {result.Length} / {end - start} characters. Turning off Name and Description changes.");
                Settings.ItemNameDescChange = false;
            }

            return result;
        }

        private void GetMonsters(string modPath, ref Dictionary<ushort, IMonster> monsterDict) {
            string file = $"{modPath}\\Monster_Data.tsv";
            int i = 0;
            try {
                using (var monsterData = new StreamReader(file)) {
                    monsterData.ReadLine(); // Skip first line
                    while (!monsterData.EndOfStream) {
                        var line = monsterData.ReadLine();
                        var values = line.Split('\t').ToArray();
                        if (UInt16.TryParse(values[0], out var uskey)) {
                            monsterDict.Add(uskey, new Monster(values, _tryEncodeItemDelegate));
                        }
                    }
                }
            } catch (IOException) {
                Console.WriteLine($"[ERROR] {file} not found.");
            } catch (IndexOutOfRangeException) {
                Console.WriteLine($"[ERROR] Incorrect fromat of {file} at line {i + 1}");
            }
        }

        private void GetCharacters(string modPath) {
            for (byte i = 0; i < 9; i++) {
                Character[i] = new Character(i, modPath);
            }
        }

        private void GetShops(string modPath) {
            string file = $"{modPath}\\Shops.tsv";
            for (int i = 0; i < Shop.Length; i++) {
                Shop[i] = new List<byte>();
            }

            try {
                using (var shopData = new StreamReader(file)) {
                    shopData.ReadLine(); // Skip first line
                    for (int shop = 0; shop < 45; shop++) {
                        var line = shopData.ReadLine();
                        var values = line.Split('\t').ToArray();
                        for (int item = 1; item < 17; item++) {
                            if (TryEncodeItem(values[item], out var itemID)) {
                                if (itemID == 255) {
                                    break;
                                }
                                Shop[shop].Add(itemID);
                            }
                        }
                    }
                }
            } catch (IOException) {
                Console.WriteLine($"[ERROR] {file} not found.");
            }
        }
    }
}
