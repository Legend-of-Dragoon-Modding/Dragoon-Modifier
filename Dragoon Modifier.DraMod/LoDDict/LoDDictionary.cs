using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.LoDDict {
    internal class LoDDictionary : ILoDDictionary {
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

        public IItem[] Item { get; private set; } = new IItem[256];
        public Dictionary<ushort, Monster> Monster { get; private set; } = new Dictionary<ushort, Monster>();
        public Character[] Character { get; } = new Character[9];

        public byte[] ItemNames { get; private set; } = new byte[0];
        public byte[] ItemDescriptions { get; private set; } = new byte[0];
        public byte[] ItemBattleNames { get; private set; } = new byte[0];
        public byte[] ItemBattleDescriptions { get; private set; } = new byte[0];

        public List<byte>[] Shop { get; private set; } = new List<byte>[45];

        public Scripts.IItemScript ItemScript { get; private set; } = new Scripts.DummyItemScript();

        internal LoDDictionary(Emulator.IEmulator emulator, ILoDDictionary loDDictionary, UI.IUIControl uiControl, string cwd, string mod) {
            Load(emulator, uiControl, cwd, mod);

            ParseScripts($"{cwd}\\Mods\\{mod}", emulator, loDDictionary, uiControl);
        }

        internal LoDDictionary(Emulator.IEmulator emulator, UI.IUIControl uiControl, string cwd, string mod, Scripts.IItemScript itemScript) {
            Load(emulator, uiControl, cwd, mod);

            ItemScript = itemScript;
        }

        private void Load(Emulator.IEmulator emulator, UI.IUIControl uiControl, string cwd, string mod) {
            string modPath = $"{cwd}\\Mods\\{mod}";

            GetItems(emulator, modPath);
            GetMonsters(modPath);
            GetCharacters(modPath);
            GetShops(modPath);
        }

        private void ParseScripts(string path, Emulator.IEmulator emulator, ILoDDictionary loDDictionary, UI.IUIControl uiControl) {
            foreach (var file in Directory.GetFiles(path, "*.cs")) {
                if (file.EndsWith("ItemScript.cs")) {
                    try {
                        ItemScript = new Scripts.CustomItemScript(file, emulator, loDDictionary, uiControl);
                        Console.WriteLine("Custom item script inserted.");
                    } catch (ApplicationException ex) {
                        Console.WriteLine($"[ERROR] Item script not compatible.");
                        Console.WriteLine($"[ERROR] {ex}");
                    }
                    continue;
                }
            }
        }

        public bool TryItem2Num(string name, out byte id) {
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

        private void GetItems(Emulator.IEmulator emulator, string modPath) {
            GetEquipment(emulator, modPath);
            GetUsableItems(emulator, modPath);

            ItemNames = GetEncodedNames(emulator);
            ItemDescriptions = GetEncodedDescriptions(emulator);
            ItemBattleNames = GetEncodedBattleNames(emulator);
            ItemBattleDescriptions = GetEncodedBattleDescriptions(emulator);
        }

        private void GetEquipment(Emulator.IEmulator emulator, string modPath) {
            string file = $"{modPath}\\Equipment.tsv";
            int i = 0;
            try {
                using (var itemData = new StreamReader(file)) {
                    itemData.ReadLine(); // Skip first line
                    while (!itemData.EndOfStream) {
                        var line = itemData.ReadLine();
                        var values = line.Split('\t').ToArray();
                        Item[i] = new Equipment(emulator, (byte) i, values, Element2Num, Status2Num); // TODO Factory
                        i++;
                    }
                }
            } catch (IOException) {
                Console.WriteLine($"[ERROR] {file} not found.");
            } catch (IndexOutOfRangeException) {
                Console.WriteLine($"[ERROR] Incorrect fromat of {file} at line {i + 1}");
            }
        }

        private void GetUsableItems(Emulator.IEmulator emulator, string modPath) {
            string file = $"{modPath}\\Items.tsv";
            int i = 192;
            try {
                using (var itemData = new StreamReader(file)) {
                    itemData.ReadLine(); // Skip first line
                    while (!itemData.EndOfStream) {
                        var line = itemData.ReadLine();
                        var values = line.Split('\t').ToArray();
                        Item[i] = new UsableItem(emulator, (byte) i, values, Element2Num, Status2Num); // TODO Factory
                        i++;
                    }
                }
            } catch (IOException) {
                Console.WriteLine($"[ERROR] {file} not found.");
            } catch (IndexOutOfRangeException) {
                Console.WriteLine($"[ERROR] Incorrect fromat of {file} at line {i + 1}");
            }
        }

        private byte[] GetEncodedNames(Emulator.IEmulator emulator) {
            int start = emulator.GetAddress("ITEM_NAME");

            var sorted = Item.OrderByDescending(item => item.Name.Length);
            byte[] result = new byte[0];

            foreach (var item in sorted) {
                var search = Emulator.KMP.UnmaskedSearch(item.EncodedName, result);
                if (search.Count != 0) {
                    item.NamePointer = start + (int) search[0];
                } else {
                    item.NamePointer = start + result.Length;
                    result = result.Concat(item.EncodedName).ToArray();
                }
            }

            var end = emulator.GetAddress("ITEM_NAME_PTR");
            if (result.Length > end - start) {
                Console.WriteLine($"Item name character limit exceeded! {result.Length} / {end - start} characters. Turning off Name and Description changes.");
                Settings.ItemNameDescChange = false;
            }

            return result;
        }

        private byte[] GetEncodedDescriptions(Emulator.IEmulator emulator) {
            int start = emulator.GetAddress("ITEM_DESC"); ;

            var sorted = Item.OrderByDescending(item => item.Description.Length);
            byte[] result = new byte[0];

            foreach (var item in sorted) {
                var search = Emulator.KMP.UnmaskedSearch(item.EncodedDescription, result);
                if (search.Count != 0) {
                    item.DescriptionPointer = start + (int) search[0];
                } else {
                    item.DescriptionPointer = start + result.Length;
                    result = result.Concat(item.EncodedDescription).ToArray();
                }
            }

            var end = emulator.GetAddress("ITEM_DESC_PTR");
            if (result.Length > end - start) {
                Console.WriteLine($"Item description character limit exceeded! {result.Length} / {end - start} characters. Turning off Name and Description changes.");
                Settings.ItemNameDescChange = false;
            }

            return result;
        }

        private byte[] GetEncodedBattleNames(Emulator.IEmulator emulator) {
            int start = emulator.GetAddress("ITEM_BTL_NAME");

            var sorted = Item.Where(item => item is IUsableItem).Cast<IUsableItem>().OrderByDescending(item => item.Name.Length);
            byte[] result = new byte[0];

            foreach (var item in sorted) {
                var search = Emulator.KMP.UnmaskedSearch(item.EncodedName, result);
                if (search.Count != 0) {
                    item.BattleNamePointer = start + (int) search[0];
                } else {
                    item.BattleNamePointer = start + result.Length;
                    result = result.Concat(item.EncodedName).ToArray();
                }
            }

            var end = emulator.GetAddress("ITEM_BTL_NAME_PTR");
            if (result.Length > end - start) {
                Console.WriteLine($"Item battle name character limit exceeded! {result.Length} / {end - start} characters. Turning off Name and Description changes.");
                Settings.ItemNameDescChange = false;
            }

            return result;
        }

        private byte[] GetEncodedBattleDescriptions(Emulator.IEmulator emulator) {
            int start = emulator.GetAddress("ITEM_BTL_DESC");

            var sorted = Item.Where(item => item is IUsableItem).Cast<IUsableItem>().OrderByDescending(item => item.BattleDescription.Length);
            byte[] result = new byte[0];

            foreach (var item in sorted) {
                var search = Emulator.KMP.UnmaskedSearch(item.EncodedBattleDescription, result);
                if (search.Count != 0) {
                    item.BattleDescriptionPointer = start + (int) search[0];
                } else {
                    item.BattleDescriptionPointer = start + result.Length;
                    result = result.Concat(item.EncodedBattleDescription).ToArray();
                }
            }

            var end = emulator.GetAddress("ITEM_BTL_DESC_PTR");
            if (result.Length > end - start) {
                Console.WriteLine($"Item battle description character limit exceeded! {result.Length} / {end - start} characters. Turning off Name and Description changes.");
                Settings.ItemNameDescChange = false;
            }

            return result;
        }

        private void GetMonsters(string modPath) {
            string file = $"{modPath}\\Monster_Data.tsv";
            int i = 0;
            try {
                using (var monsterData = new StreamReader(file)) {
                    monsterData.ReadLine(); // Skip first line
                    while (!monsterData.EndOfStream) {
                        var line = monsterData.ReadLine();
                        var values = line.Split('\t').ToArray();
                        if (UInt16.TryParse(values[0], out var uskey)) {
                            Monster.Add(uskey, new Monster(values, this, Status2Num, Element2Num));
                        }
                    }
                }
            } catch (IOException) {
                Console.WriteLine($"[ERROR] {file} not found.");
            } catch (IndexOutOfRangeException) {
                Console.WriteLine($"[ERROR] Incorrect fromat of {file} at line {i + 1}");
            }
        }

        public void SwapMonsters(string cwd, string mod) {
            string file = $"{cwd}\\Mods\\{mod}\\Monster_Data.tsv";
            int i = 0;
            Monster.Clear();
            try {
                using (var monsterData = new StreamReader(file)) {
                    monsterData.ReadLine(); // Skip first line
                    while (!monsterData.EndOfStream) {
                        var line = monsterData.ReadLine();
                        var values = line.Split('\t').ToArray();
                        if (UInt16.TryParse(values[0], out var uskey)) {
                            Monster.Add(uskey, new Monster(values, this, Status2Num, Element2Num));
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
                            if (TryItem2Num(values[item], out var itemID)) {
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
