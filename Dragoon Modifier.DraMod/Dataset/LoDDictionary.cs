using Dragoon_Modifier.Core;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSScriptLib;

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

        private readonly string _modDirectory;
        private readonly string? _secondaryModDirectory = null;
        private Dictionary<ushort, IMonster> _monsters = new();
        private Dictionary<ushort, IMonster> _secondaryMonsters = new();

        public IItem[] Item { get; } = new IItem[256];
        public Dictionary<ushort, IMonster> Monster {
            get {
                if (Settings.Instance.DualDifficulty && (_secondaryModDirectory != null || _secondaryMonsters.Count > 0)) {
                    return _secondaryMonsters;
                }
                return _monsters;
            }
        }

        public ICharacter[] Character { get; } = new ICharacter[9];

        public byte[] ItemNames { get; private set; } = Array.Empty<byte>();
        public byte[] ItemDescriptions { get; private set; } = Array.Empty<byte>();
        public byte[] ItemBattleNames { get; private set; } = Array.Empty<byte>();
        public byte[] ItemBattleDescriptions { get; private set; } = Array.Empty<byte>();
        public Dictionary<int, IDragoonAddition> DragoonAddition { get; private set; } = new();
        public Dictionary<int, IDragoonSpells> DragoonSpell { get; private set; } = new();
        public dynamic[][] DragoonStats { get; private set; } = new dynamic[9][];

        public List<byte>[] Shop { get; } = new List<byte>[45];

        public Scripts.IScript Script { get; private set; } = new Scripts.DummyScript();

        internal delegate bool MyFunc<T1, T2>(T1 a, out T2 b);
        private MyFunc<string, byte> _tryEncodeItemDelegate;

        internal LoDDictionary(string cwd, string mod) {
            _modDirectory = $"{cwd}\\Mods\\{mod}";

            ParseScript();

            Load();
        }

        internal LoDDictionary(string cwd, string mod, string? dualMod, Scripts.IScript script) {
            _modDirectory = $"{cwd}\\Mods\\{mod}";
            if (dualMod != null) {
                _secondaryModDirectory = $"{cwd}\\Mods\\{dualMod}";
            }

            Script = script;

            Load();
        }

        private void ParseScript() {
            foreach (var file in Directory.GetFiles(_modDirectory, "*.cs")) {
                if (file.Equals($"{_modDirectory}\\Script.cs")) {
                    try {
                        Script = new Scripts.CustomScript(file);
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
            _tryEncodeItemDelegate = TryEncodeItem;

            GetItems();
            _monsters = GetMonsters(_modDirectory);

            if (_secondaryModDirectory != null) {
                _secondaryMonsters = GetMonsters(_secondaryModDirectory);
            }

            GetCharacters();
            DragoonAddition = GetDragoonAdditions();
            DragoonSpell = GetDragoonSpells();
            GetDragoonAdditions();
            GetDragoonStats();
            GetShops();
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

        private void GetItems() {
            GetEquipment();
            GetUsableItems();

            ItemNames = GetEncodedNames();
            ItemDescriptions = GetEncodedDescriptions();
            ItemBattleNames = GetEncodedBattleNames();
            ItemBattleDescriptions = GetEncodedBattleDescriptions();
        }

        private void GetEquipment() {
            string file = $"{_modDirectory}\\Equipment.tsv";
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

        private void GetUsableItems() {
            string file = $"{_modDirectory}\\Items.tsv";
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
                Settings.Instance.ItemNameDescChange = false;
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
                Settings.Instance.ItemNameDescChange = false;
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
                Settings.Instance.ItemNameDescChange = false;
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
                Settings.Instance.ItemNameDescChange = false;
            }

            return result;
        }

        private Dictionary<ushort, IMonster> GetMonsters(string modDirectory) {
            Dictionary<ushort, IMonster> monsterDict = new();
            string file = $"{modDirectory}\\Monster_Data.tsv";
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
                        i++;
                    }
                }
            } catch (IOException) {
                Console.WriteLine($"[ERROR] {file} not found.");
            } catch (IndexOutOfRangeException) {
                Console.WriteLine($"[ERROR] Incorrect fromat of {file} at line {i + 1}");
            }
            return monsterDict;
        }

        private void GetCharacters() {
            for (byte i = 0; i < 9; i++) {
                Character[i] = new Character(i, _modDirectory);
            }
        }

        private Dictionary<int, IDragoonAddition> GetDragoonAdditions() {
            Dictionary<int, IDragoonAddition> dragoonAdditionDict = new();
            string file = $"{_modDirectory}\\Dragoon_Additions.tsv";
            int i = 0;
            try {
                using (var dragoonAdditionData = new StreamReader(file)) {
                    dragoonAdditionData.ReadLine(); //Skip
                    while (!dragoonAdditionData.EndOfStream) {
                        var line = dragoonAdditionData.ReadLine();
                        var values = line.Split('\t').ToArray();
                        dragoonAdditionDict.Add(i, new DragoonAddition(values[1], values[2], values[3], values[4], values[5]));
                        i++;
                    }
                }
            } catch (IOException) {
                Console.WriteLine($"[ERROR] {file} not found.");
            } catch (Exception) {
                Console.WriteLine($"[ERROR] Incorrect fromat of {file} at line {i + 1}");
            }
            return dragoonAdditionDict;
        }

        private void GetDragoonStats() {
            DragoonStats = new dynamic[9][];
            string file = $"{_modDirectory}\\Dragoon_Stats.tsv";
            int i = 0;

            try {
                using (var dragoonStatsData = new StreamReader(file)) {
                    dragoonStatsData.ReadLine(); //Skip
                    while (!dragoonStatsData.EndOfStream) {
                        var line = dragoonStatsData.ReadLine();
                        var values = line.Split('\t').ToArray();
                        DragoonStats[i] = new dynamic[] {
                                    new DragoonStats("0", "0", "0", "0", "0"),
                                    new DragoonStats(values[1], values[2], values[3], values[4], values[5]),
                                    new DragoonStats(values[6], values[7], values[8], values[9], values[10]),
                                    new DragoonStats(values[11], values[12], values[13], values[14], values[15]),
                                    new DragoonStats(values[16], values[17], values[18], values[19], values[20]),
                                    new DragoonStats(values[21], values[22], values[23], values[24], values[25])
                                };
                        i++;
                    }
                }
            } catch (IOException) {
                Console.WriteLine($"[ERROR] {file} not found.");
            } catch (Exception) {
                Console.WriteLine($"[ERROR] Incorrect fromat of {file} at line {i + 1}");
            }
        }

        private Dictionary<int, IDragoonSpells> GetDragoonSpells() {
            Dictionary<int, IDragoonSpells> dragoonSpellsDict = new();
            string file = $"{_modDirectory}\\Dragoon_Spells.tsv";
            int i = 0;
            try {
                using (var dragoonSpellsData = new StreamReader(file)) {
                    dragoonSpellsData.ReadLine(); //Skip
                    while (!dragoonSpellsData.EndOfStream) {
                        var line = dragoonSpellsData.ReadLine();
                        var values = line.Split('\t').ToArray();
                        dragoonSpellsDict.Add(i, new DragoonSpells(values, i, _element2Num));
                        i++;
                    }
                }

                long offset = 0x0;
                long start = Emulator.GetAddress("DRAGOON_DESC");
                for (int x = 0; x < dragoonSpellsDict.Count; x++) {
                    dragoonSpellsDict[x].Description_Pointer = start + offset;
                    offset += dragoonSpellsDict[x].Encoded_Description.Length;
                }
            } catch (IOException) {
                Console.WriteLine($"[ERROR] {file} not found.");
            } catch (Exception) {
                Console.WriteLine($"[ERROR] Incorrect fromat of {file} at line {i + 1}");
            }
            return dragoonSpellsDict;
        }

        private void GetShops() {
            string file = $"{_modDirectory}\\Shops.tsv";
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
