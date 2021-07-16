using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.LoDDict {
    internal class LoDDictionary : ILoDDictionary {
        public IItem[] Item { get; private set; } = new IItem[256];

        public string ItemNames { get; private set; } = String.Empty;
        public string ItemDescriptions { get; private set; } = String.Empty;
        public string ItemBattleNames { get; private set; } = String.Empty;
        public string ItemBattleDescriptions { get; private set; } = String.Empty;

        internal LoDDictionary(Emulator.IEmulator emulator, string cwd, string mod) {
            GetItems(emulator, cwd, mod);
        }

        private void GetItems(Emulator.IEmulator emulator, string cwd, string mod) {
            GetEquipment(emulator, cwd, mod);
            GetUsableItems(emulator, cwd, mod);
            ItemNames = CreateItemNameString(emulator);
            ItemDescriptions = CreateItemDescriptionString(emulator);
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
                        Item[i] = new Equipment(emulator, (byte) i, values); // TODO Factory
                        i++;
                    }
                }
            } catch (FileNotFoundException) {
                Console.WriteLine($"[ERROR] {file} not found.");
            } catch (DirectoryNotFoundException) {
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
                        Item[i] = new UsableItem(emulator, (byte) i, values); // TODO Factory
                        i++;
                    }
                }
            } catch (FileNotFoundException) {
                Console.WriteLine($"[ERROR] {file} not found.");
            } catch (DirectoryNotFoundException) {
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
            long end = emulator.GetAddress("ITEM_NAME_PTR");
            int len1 = result.Replace(" ", "").Length / 4;
            int len2 = (int) (end - start) / 2;
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
            long end = emulator.GetAddress("ITEM_DESC_PTR");
            int len1 = result.Replace(" ", "").Length / 4;
            int len2 = (int) (end - start) / 2;
            if (len1 >= len2) {
                Console.WriteLine($"Item description character limit exceeded! {len1} / {len2} characters. Turning off Name and Description changes.");
                Settings.ItemNameDescChange = false;
            }
            return result;
        }
    }
}
