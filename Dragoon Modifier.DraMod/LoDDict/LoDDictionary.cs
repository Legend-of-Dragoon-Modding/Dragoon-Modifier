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
        public Character[] Character = new Character[9];

        public string ItemNames { get; private set; } = String.Empty;
        public string ItemDescriptions { get; private set; } = String.Empty;
        public string ItemBattleNames { get; private set; } = String.Empty;
        public string ItemBattleDescriptions { get; private set; } = String.Empty;

        internal LoDDictionary(Emulator.IEmulator emulator, string cwd, string mod) {
            GetItems(emulator, cwd, mod);
            GetMonsters(cwd, mod);
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

        private void GetItems(Emulator.IEmulator emulator, string cwd, string mod) {
            GetEquipment(emulator, cwd, mod);
            GetUsableItems(emulator, cwd, mod);
            ItemNames = CreateItemNameString(emulator);
            ItemDescriptions = CreateItemDescriptionString(emulator);
            ItemBattleNames = CreateItemBattleNameString(emulator);
            ItemBattleDescriptions = CreateItemBattleDescriptionString(emulator);
        }

        private void GetEquipment(Emulator.IEmulator emulator, string cwd, string mod) {
            string file = $"{cwd}\\Mods\\{mod}\\Equipment.tsv";
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

        private void GetUsableItems(Emulator.IEmulator emulator, string cwd, string mod) {
            string file = $"{cwd}\\Mods\\{mod}\\Items.tsv";
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

        private string CreateItemNameString(Emulator.IEmulator emulator) {
            int offset = 0;
            int start = emulator.GetAddress("ITEM_NAME");
            IItem[] sorted = Item.OrderByDescending(o => o.Name.Length).ToArray();
            List<string> names = new List<string>();
            foreach (var item in sorted) {
                if (names.Any(l => l.Contains(item.EncodedName))) {
                    int index = Array.IndexOf(sorted, Array.Find(sorted, x => x.EncodedName.Contains(item.EncodedName)));
                    item.NamePointer = sorted[index].NamePointer + (sorted[index].Name.Length - item.Name.Length) * 2;
                } else {
                    names.Add(item.EncodedName);
                    item.NamePointer = start + offset;
                    offset += (item.EncodedName.Replace(" ", "").Length / 2);
                }
            }
            string result = String.Join(" ", names);
            int end = emulator.GetAddress("ITEM_NAME_PTR");
            int len1 = result.Replace(" ", "").Length / 4;
            int len2 = (end - start) / 2;
            if (len1 >= len2) {
                Console.WriteLine($"Item name character limit exceeded! {len1} / {len2} characters. Turning off Name and Description changes.");
                Settings.ItemNameDescChange = false;
            }
            return result;
        }

        private string CreateItemDescriptionString(Emulator.IEmulator emulator) {
            int offset = 0;
            int start = emulator.GetAddress("ITEM_DESC");
            IItem[] sorted = Item.OrderByDescending(o => o.Description.Length).ToArray();
            List<string> descriptions = new List<string>();

            foreach (var item in sorted) {
                if (descriptions.Any(l => l.Contains(item.EncodedDescription))) {
                    int index = Array.IndexOf(sorted, Array.Find(sorted, x => x.EncodedDescription.Contains(item.EncodedDescription)));
                    item.DescriptionPointer = sorted[index].DescriptionPointer + (sorted[index].Description.Length - item.Description.Length) * 2;
                } else {
                    descriptions.Add(item.EncodedDescription);
                    item.DescriptionPointer = start + offset;
                    offset += (item.EncodedDescription.Replace(" ", "").Length / 2);
                }
            }
            string result = String.Join(" ", descriptions);
            int end = emulator.GetAddress("ITEM_DESC_PTR");
            int len1 = result.Replace(" ", "").Length / 4;
            int len2 = (end - start) / 2;
            if (len1 >= len2) {
                Console.WriteLine($"Item description character limit exceeded! {len1} / {len2} characters. Turning off Name and Description changes.");
                Settings.ItemNameDescChange = false;
            }
            return result;
        }

        private string CreateItemBattleNameString(Emulator.IEmulator emulator) {
            int offset = 0;
            int start = emulator.GetAddress("ITEM_BTL_NAME");
            var temp = new List<IUsableItem>();
            foreach (var item in Item) {
                if (item is IUsableItem usableItem) {
                    temp.Add(usableItem);
                }
            }
            IUsableItem[] sorted = temp.OrderBy(o => o.BattleDescription.Length).ToArray();
            List<string> names = new List<string>();
            foreach (IUsableItem item in sorted) {
                if (names.Any(l => l.Contains(item.EncodedName))) {
                    int index = Array.IndexOf(sorted, Array.Find(sorted, x => x.EncodedName.Contains(item.EncodedName)));
                    item.BattleNamePointer = sorted[index].BattleNamePointer + (sorted[index].Name.Length - item.Name.Length) * 2;
                } else {
                    names.Add(item.EncodedName);
                    item.BattleNamePointer = start + offset;
                    offset += (item.EncodedName.Replace(" ", "").Length / 2);
                }
            }
            string result = String.Join(" ", names);
            int end = emulator.GetAddress("ITEM_BTL_NAME_PTR");
            int len1 = result.Replace(" ", "").Length / 4;
            int len2 = (end - start) / 2;
            if (len1 >= len2) {
                Console.WriteLine($"Item battle name character limit exceeded! {len1} / {len2} characters. Turning off Name and Description changes.");
                Settings.ItemNameDescChange = false;
            }
            return result;
        }

        private string CreateItemBattleDescriptionString(Emulator.IEmulator emulator) {
            int offset = 0;
            int start = emulator.GetAddress("ITEM_BTL_DESC");
            var temp = new List<IUsableItem>();
            foreach (var item in Item) {
                if (item is IUsableItem usableItem) {
                    temp.Add(usableItem);
                }
            }
            IUsableItem[] sorted = temp.OrderBy(o => o.BattleDescription.Length).ToArray();
            List<string> battleDescriptions = new List<string>();
            foreach (IUsableItem item in sorted) {
                if (battleDescriptions.Any(l => l.Contains(item.EncodedBattleDescription))) {
                    int index = Array.IndexOf(sorted, Array.Find(sorted, x => x.EncodedBattleDescription.Contains(item.EncodedBattleDescription)));
                    item.BattleDescriptionPointer = sorted[index].BattleDescriptionPointer + (sorted[index].BattleDescription.Length - item.BattleDescription.Length) * 2;
                } else {
                    battleDescriptions.Add(item.EncodedBattleDescription);
                    item.BattleDescriptionPointer = start + offset;
                    offset += (item.EncodedBattleDescription.Replace(" ", "").Length / 2);
                }
            }
            string result = String.Join(" ", battleDescriptions);
            int end = emulator.GetAddress("ITEM_BTL_DESC_PTR");
            int len1 = result.Replace(" ", "").Length / 4;
            int len2 = (end - start) / 2;
            if (len1 >= len2) {
                Console.WriteLine($"Item battle description character limit exceeded! {len1} / {len2} characters. Turning off Name and Description changes.");
                Settings.ItemNameDescChange = false;
            }
            return result;
        }

        private void GetMonsters(string cwd, string mod) {
            string file = $"{cwd}\\Mods\\{mod}\\Monster_Data.tsv";
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
    }
}
