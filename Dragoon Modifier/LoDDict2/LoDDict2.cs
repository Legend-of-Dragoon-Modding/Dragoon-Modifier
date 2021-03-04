using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.LoDDict2 {
    public class LoDDict2 {
        Item[] itemArr = new Item[256];
        Dictionary<string, byte> item2num = new Dictionary<string, byte>();
        Dictionary<byte, string> num2item = new Dictionary<byte, string>();
        Character[] characterArr = new Character[9];

        public static readonly Dictionary<byte, string> num2element = new Dictionary<byte, string>() {
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
        public static readonly Dictionary<string, byte> element2num = new Dictionary<string, byte>() {
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
        public static readonly Dictionary<string, byte> status2num = new Dictionary<string, byte>() {
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

        string nameStr;
        string descStr;
        string btlNameStr;
        string btlDescStr;

        public LoDDict2() {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            string cwd = AppDomain.CurrentDomain.BaseDirectory;
            GetItems(cwd);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Constants.WriteDebug($"LoDDict2 executed in {elapsedMs} ms");
        }



        public void GetEquipment(string cwd) {
            byte i = 0;
            try {
                using (var itemData = new StreamReader(cwd + "Mods/" + Globals.MOD + "/Equipment.tsv")) {
                    itemData.ReadLine(); // Skip the first line
                    while (!itemData.EndOfStream) {
                        var line = itemData.ReadLine();
                        var values = line.Split('\t').ToArray();
                        itemArr[i] = new Equipment(i, values);
                        if (values[0] != "") {
                            item2num.Add(values[0].ToLower(), i);
                            num2item.Add(i, values[0]);
                        }
                        i++;
                    }
                }
            } catch (FileNotFoundException) {
                string file = cwd + @"Mods\" + Globals.MOD + @"\Equipment.tsv";
                Constants.WriteError($"{file} not found. Turning off Item Stat changes and Monster Drop changes.");
                Globals.ITEM_STAT_CHANGE = false;
                Globals.MONSTER_DROP_CHANGE = false;
            } catch (IndexOutOfRangeException) {
                string file = cwd + @"Mods\" + Globals.MOD + @"\Equipment.tsv";
                Constants.WriteError($"Incorrect format of {file} on line {i + 1}. Turning off Item Stat changes and Monster Drop changes.");
                Globals.ITEM_STAT_CHANGE = false;
                Globals.MONSTER_DROP_CHANGE = false;
            }
        }

        public void GetUsableItems(string cwd) {
            byte i = 192;
            try {
                using (var itemData = new StreamReader(cwd + "Mods/" + Globals.MOD + "/Items.tsv")) {
                    itemData.ReadLine(); // Skip the first line
                    while (!itemData.EndOfStream) {
                        var line = itemData.ReadLine();
                        var values = line.Split('\t').ToArray();
                        itemArr[i] = new UsableItem(i, values);
                        if (values[0] != "") {
                            item2num.Add(values[0].ToLower(), i);
                            num2item.Add(i, values[0]);
                        }
                        i++;
                    }
                }
            } catch (FileNotFoundException) {
                string file = cwd + @"Mods\" + Globals.MOD + @"\Items.tsv";
                Constants.WriteError($"{file} not found. Turning off Usable Items changes and Monster Drop changes.");
                Globals.THROWN_ITEM_CHANGE = false;
                Globals.MONSTER_DROP_CHANGE = false;
            } catch (IndexOutOfRangeException) {
                string file = cwd + @"Mods\" + Globals.MOD + @"\Items.tsv";
                Constants.WriteError($"Incorrect format of {file} on line {i - 191}. Turning off Usable Items changes and Monster Drop changes.");
                Globals.THROWN_ITEM_CHANGE = false;
                Globals.MONSTER_DROP_CHANGE = false;
            }
        }

        public string CreateItemNameString() {
            long offset = 0;
            long start = Constants.GetAddress("ITEM_NAME");
            Item[] sortedArr = itemArr.OrderByDescending(o => o.Name.Length).ToArray();
            List<string> stringList = new List<string>();
            foreach (Item item in sortedArr) {
                if (stringList.Any(l => l.Contains(item.EncodedName))) {
                    int index = Array.IndexOf(sortedArr, Array.Find(sortedArr, x => x.EncodedName.Contains(item.EncodedName)));
                    item.NamePointer = sortedArr[index].NamePointer + (sortedArr[index].Name.Length - item.Name.Length) * 2;
                } else {
                    stringList.Add(item.EncodedName);
                    item.NamePointer = start + offset;
                    offset += (item.EncodedName.Replace(" ", "").Length / 2);
                }
            }
            string result = String.Join(" ", stringList);
            long end = Constants.GetAddress("ITEM_NAME_PTR");
            int len1 = result.Replace(" ", "").Length / 4;
            int len2 = (int) (end - start) / 2;
            if (len1 >= len2) {
                Constants.WriteError($"Item name character limit exceeded! {len1} / {len2} characters. Turning off Name and Description changes.");
                Globals.ITEM_NAMEDESC_CHANGE = false;
            }
            return result;

        }

        public string CreateItemDescriptionString() {
            long offset = 0;
            long start = Constants.GetAddress("ITEM_DESC");
            Item[] sortedArr = itemArr.OrderByDescending(o => o.Description.Length).ToArray();
            List<string> stringList = new List<string>();
            foreach (Item item in sortedArr) {
                if (stringList.Any(l => l.Contains(item.EncodedDescription))) {
                    int index = Array.IndexOf(sortedArr, Array.Find(sortedArr, x => x.EncodedDescription.Contains(item.EncodedDescription)));
                    item.DescriptionPointer = sortedArr[index].DescriptionPointer + (sortedArr[index].Description.Length - item.Description.Length) * 2;
                } else {
                    stringList.Add(item.EncodedDescription);
                    item.DescriptionPointer = start + offset;
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

        public string CreateItemBattleNameString() {
            long offset = 0;
            long start = Constants.GetAddress("ITEM_BTL_NAME");
            Item[] sortedArr = (Item[]) itemArr.Skip(193).Take(63).OrderBy(o => o.Description.Length).ToArray();
            List<string> stringList = new List<string>();
            foreach (UsableItem item in sortedArr) {
                if (stringList.Any(l => l.Contains(item.EncodedName))) {
                    int index = Array.IndexOf(sortedArr, Array.Find(sortedArr, x => x.EncodedName.Contains(item.EncodedName)));
                    item.BattleNamePointer = Convert.ToUInt32(sortedArr[index].GetType().GetProperty("BattleNamePointer").GetValue(sortedArr[index])) + (sortedArr[index].Name.Length - item.Name.Length) * 2;
                } else {
                    stringList.Add(item.EncodedName);
                    item.BattleNamePointer = start + offset;
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

        public string CreateItemBattleDescriptionString() {
            long offset = 0;
            long start = Constants.GetAddress("ITEM_BTL_DESC");
            Item[] sortedArr = itemArr.Skip(193).Take(63).OrderBy(o => o.Description.Length).ToArray();
            List<string> stringList = new List<string>();
            foreach (UsableItem item in sortedArr) {
                if (stringList.Any(l => l.Contains(item.EncodedBattleDescription))) {
                    int index = Array.IndexOf(sortedArr, Array.Find(sortedArr, x => x.GetType().GetProperty("EncodedBattleDescription").GetValue(x).ToString().Contains(item.EncodedBattleDescription)));
                    item.BattleDescriptionPointer = Convert.ToUInt32(sortedArr[index].GetType().GetProperty("BattleDescriptionPointer").GetValue(sortedArr[index])) + (sortedArr[index].GetType().GetProperty("BattleDescription").GetValue(sortedArr[index]).ToString().Length - item.BattleDescription.Length) * 2;
                } else {
                    stringList.Add(item.EncodedBattleDescription);
                    item.BattleDescriptionPointer = start + offset;
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

        public void GetItems(string cwd) {
            try {
                num2item = new Dictionary<byte, string>();
                item2num = new Dictionary<string, byte>();
                GetEquipment(cwd);
                GetUsableItems(cwd);
                nameStr = CreateItemNameString();
                descStr = CreateItemDescriptionString();
                btlNameStr = CreateItemBattleNameString();
                btlDescStr = CreateItemBattleDescriptionString();
            } catch (Exception ex) {
                Constants.RUN = false;
                Constants.WriteGLog("Program stopped.");
                Constants.WritePLogOutput("LoD Dictionary fatal error.");
                Constants.WriteOutput("Fatal Error. Closing all threads.");
                Constants.WriteError(ex.ToString());
            }

        }

    }
}
