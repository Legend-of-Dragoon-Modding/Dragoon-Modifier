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
        Dictionary<ushort, MonsterData> monsters = new Dictionary<ushort, MonsterData>();

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
        public static readonly Dictionary<string, byte> special2num = new Dictionary<string, byte>() {
             {"", 0 },
             {"none", 0 },
             {"cannot_sell", 4 },
             {"attack_all", 8 },
             {"death_chance", 64 },
             {"death_res", 128 },
             {"monster_unknown1", 1 },
             {"monster_unknow2", 2 },
             {"monster_unknown4", 4 },
             {"monster_unknown8", 8 },
             {"monster_unknown16", 16 }
        };
        static readonly string[][] additionDict = new string[][] {
                new string[] { "Double_Slash", "Volcano", "Burning_Rush", "Crush_Dance", "Madness_Hero", "Moon_Strike", "Blazing_Dynamo"},
                new string[] { "Harpoon", "Spinning Cane", "Rod_Typhoon", "Gust_of_Wind_Dance", "Flower_Storm"},
                new string[] { },
                new string[] { "Whip_Smack", "More_and_More", "Hard_Blade", "Demons_Dance"},
                new string[] { "Double_Punch", "Flurry_of_Styx", "Summon_4_Gods", "5-Ring_Shattering", "Hex_Hammer", "Omni_Sweep"},
                new string[] { "Harpoon", "Spinning Cane", "Rod_Typhoon", "Gust_of_Wind_Dance", "Flower_Storm"},
                new string[] { "Double_Smack", "Hammer_Spin", "Cool_Boogie", "Cats_Cradle", "Perky_Step"},
                new string[] { "Pursuit", "Inferno", "Bone_Crush"},
                new string[] { }
            };
        static readonly string[] characterName = new string[] { "Dart", "Lavitz", "Shana", "Rose", "Haschel", "Albert", "Meru", "Kongol", "Miranda" };

        string nameStr;
        string descStr;
        string btlNameStr;
        string btlDescStr;

        public LoDDict2() {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            string cwd = AppDomain.CurrentDomain.BaseDirectory;
            Init(cwd);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Constants.WriteDebug($"LoDDict2 executed in {elapsedMs} ms");
        }

        private void Init(string cwd) {
            try {
                GetItems(cwd);
                SetCharacters(cwd);
                GetMonsters(cwd);
            } catch (Exception ex) {
                Constants.RUN = false;
                Constants.WriteGLog("Program stopped.");
                Constants.WritePLogOutput("LoD Dictionary fatal error.");
                Constants.WriteOutput("Fatal Error. Closing all threads.");
                Constants.WriteError(ex.ToString());
            }
        }


        private void GetEquipment(string cwd) {
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

        private void GetUsableItems(string cwd) {
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

        private string CreateItemNameString() {
            long offset = 0;
            long start = Constants.GetAddress("ITEM_NAME");
            Item[] sortedArr = itemArr.OrderByDescending(o => o.Name.Length).ToArray();
            List<string> stringList = new List<string>();
            foreach (Item item in sortedArr) {
                if (stringList.Any(l => l.Contains(item.EncodedName))) { // Item name is already a substring of a different 
                    int index = Array.IndexOf(sortedArr, Array.Find(sortedArr, x => x.EncodedName.Contains(item.EncodedName))); // Get index of matching string
                    item.NamePointer = sortedArr[index].NamePointer + (sortedArr[index].Name.Length - item.Name.Length) * 2; // Account for different string length
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

        private string CreateItemDescriptionString() {
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

        private string CreateItemBattleNameString() {
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

        private string CreateItemBattleDescriptionString() {
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

        private void GetItems(string cwd) {
            num2item = new Dictionary<byte, string>();
            item2num = new Dictionary<string, byte>();
            GetEquipment(cwd);
            GetUsableItems(cwd);
            nameStr = CreateItemNameString();
            descStr = CreateItemDescriptionString();
            btlNameStr = CreateItemBattleNameString();
            btlDescStr = CreateItemBattleDescriptionString();
        }

        private Level[][] GetLevels(string cwd) {
            var statsArr = new Level[7][];
            for (int i = 0; i < statsArr.Length; i++) {
                statsArr[i] = new Level[61];
                statsArr[i][0] = new Level(0, 0, 0, 0, 0, 0);
            }
            try {
                using (var characterData = new StreamReader(cwd + "Mods/" + Globals.MOD + "/Character_Stats.tsv")) {
                    characterData.ReadLine();
                    characterData.ReadLine(); // Skip first two lines
                    while (!characterData.EndOfStream) {
                        var line = characterData.ReadLine();
                        var values = line.Split('\t').ToArray();
                        int level = int.Parse(values[0]);
                        statsArr[0][level] = new Level(values[1], values[2], values[3], values[4], values[5], values[6]);
                        statsArr[1][level] = new Level(values[7], values[8], values[9], values[10], values[11], values[12]);
                        statsArr[2][level] = new Level(values[13], values[14], values[15], values[16], values[17], values[18]);
                        statsArr[3][level] = new Level(values[19], values[20], values[21], values[22], values[23], values[24]);
                        statsArr[4][level] = new Level(values[25], values[26], values[27], values[28], values[29], values[30]);
                        statsArr[5][level] = new Level(values[31], values[32], values[33], values[34], values[35], values[36]);
                        statsArr[6][level] = new Level(values[37], values[38], values[39], values[40], values[41], values[42]);
                    }
                }
            } catch (FileNotFoundException) {
                string file = cwd + @"Mods\" + Globals.MOD + @"\Character_Stats.tsv";
                Constants.WriteError(file + " not found. Turning off Stat Changes.");
                Globals.CHARACTER_STAT_CHANGE = false;
            }
            return statsArr;
        }

        private void GetMonsters(string cwd) {
            try {
                using (var monsterData = new StreamReader(cwd + "Mods/" + Globals.MOD + "/Monster_Data.tsv")) {
                    monsterData.ReadLine(); // Skip first line
                    while (!monsterData.EndOfStream) {
                        var line = monsterData.ReadLine();
                        var values = line.Split('\t').ToArray();
                        monsters.Add(UInt16.Parse(values[0]), new MonsterData(values, item2num, status2num, special2num, element2num));
                    }
                }
            } catch (FileNotFoundException) {
                string file = cwd + @"Mods\" + Globals.MOD + @"\Monster_Data.tsv";
                Constants.WriteError(file + " not found. Turning off Monster and Drop Changes.");
                Globals.MONSTER_STAT_CHANGE = false;
                Globals.MONSTER_DROP_CHANGE = false;
                Globals.MONSTER_EXPGOLD_CHANGE = false;
            }
        }

        private Addition[][] GetAdditions(string cwd) {
            
            var additionArr = new Addition[9][];

            var path = cwd + @"Mods\" + Globals.MOD + @"\Additions\";

            int i = 0;
            foreach (var character in additionDict) {
                additionArr[i] = new Addition[character.Length];
                var currentChar = characterName[i];
                int j = 0;
                foreach (var addition in character) {
                    try {
                        additionArr[i][j] = new Addition(path + currentChar + @"\" + addition + @".tsv", addition);
                    } catch (FileNotFoundException) {
                        Constants.WriteError($"Addition file for {currentChar} - {addition} not found!");
                    } catch (DirectoryNotFoundException) {
                        Constants.WriteError($"Addition directory for character {currentChar} not found!");
                        break;
                    }
                    j++;
                }
                i++;
            }

            return additionArr;
        }

        private void SetCharacters(string cwd) {
            var statsArr = GetLevels(cwd);
            var additionArr = GetAdditions(cwd);
            characterArr[0] = new Character(statsArr[0], additionArr[0]);
            characterArr[1] = new Character(statsArr[1], additionArr[1]);
            characterArr[2] = new Character(statsArr[2], additionArr[2]);
            characterArr[3] = new Character(statsArr[3], additionArr[3]);
            characterArr[4] = new Character(statsArr[4], additionArr[4]);
            characterArr[5] = new Character(statsArr[1], additionArr[5]);
            characterArr[6] = new Character(statsArr[5], additionArr[6]);
            characterArr[7] = new Character(statsArr[6], additionArr[7]);
            characterArr[8] = new Character(statsArr[2], additionArr[8]);
        }

    }
}
