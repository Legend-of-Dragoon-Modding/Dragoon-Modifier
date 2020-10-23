using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Dragoon_Modifier {
    public class LoDDict {
        IEnumerable<string> EnumerateLines(TextReader reader) {
            string line;

            while ((line = reader.ReadLine()) != null) {
                yield return line;
            }
        }

        string[] ReadAllResourceLines(byte[] resourceData) {
            using (Stream stream = new MemoryStream(resourceData))
            using (StreamReader reader = new StreamReader(stream)) {
                return EnumerateLines(reader).ToArray();
            }
        }

        List<dynamic> itemList = new List<dynamic>();
        List<dynamic> originalItemList = new List<dynamic>();
        List<string> descriptionList = new List<string>();
        List<string> battleDescriptionList = new List<string>();
        List<string> nameList = new List<string>();
        List<string> battleNameList = new List<string>();
        IDictionary<int, dynamic> statList = new Dictionary<int, dynamic>();
        IDictionary<int, dynamic> ultimateStatList = new Dictionary<int, dynamic>();
        byte[][] shopList = new byte[44][];
        dynamic[][] characterStats = new dynamic[9][];
        dynamic[][] originalCharacterStats = new dynamic[9][];
        dynamic[,,] additionData = new dynamic[9, 8, 8];
        byte[] additionLevels = new byte[30];
        List<int> monsterScript = new List<int>();
        dynamic[][] dragoonStats = new dynamic[9][];
        dynamic[] dragoonAddition = new dynamic[9];
        IDictionary<byte, string> num2item = new Dictionary<byte, string>();
        IDictionary<string, byte> item2num = new Dictionary<string, byte>();
        IDictionary<byte, string> num2element = new Dictionary<byte, string>() {
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
        IDictionary<string, byte> element2num = new Dictionary<string, byte>() {
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
        IDictionary<string, byte> status2num = new Dictionary<string, byte>() {
                {"none", 0 },
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
                {"po", 128 }
            };

        public List<dynamic> ItemList { get { return itemList; } }
        public List<dynamic> OriginalItemList { get { return originalItemList; } }
        public List<string> DescriptionList { get { return descriptionList; } }
        public List<string> BattleDescriptionList { get { return battleDescriptionList; } }
        public List<string> NameList { get { return nameList; } }
        public List<string> BattleNameList { get { return battleNameList; } }
        public IDictionary<int, dynamic> StatList { get { return statList; } }
        public IDictionary<int, dynamic> UltimateStatList { get { return ultimateStatList; } }
        public byte[][] ShopList { get { return shopList; } }
        public dynamic[][] CharacterStats { get { return characterStats; } }
        public dynamic[][] OriginalCharacterStats { get { return originalCharacterStats; } }
        public dynamic[,,] AdditionData { get { return additionData; } }
        public byte[] AdditionLevels { get { return additionLevels; } }
        public dynamic[] DragoonAddition { get { return dragoonAddition; } }
        public List<int> MonsterScript { get { return monsterScript; } }
        public IDictionary<byte, string> Num2Item { get { return num2item; } }
        public IDictionary<string, byte> Item2Num { get { return item2num; } }
        public IDictionary<byte, string> Num2Element { get { return num2element; } }
        public IDictionary<string, byte> Element2Num { get { return element2num; } }
        public IDictionary<string, byte> Status2Num { get { return status2num; } }
        public dynamic[][] DragoonStats { get { return dragoonStats; } }

        public LoDDict() {
            string cwd = AppDomain.CurrentDomain.BaseDirectory;
            int origI = 0;
            try {
                try {
                    using (var itemData = new StreamReader(cwd + "Mods/" + Globals.MOD + "/Equipment.tsv")) {
                        itemList = new List<dynamic>();
                        bool firstline = true;
                        byte i = 0;
                        while (!itemData.EndOfStream) {
                            var line = itemData.ReadLine();
                            if (firstline == false) {
                                var values = line.Split('\t').ToArray();
                                itemList.Add(new Equipment(i, values));
                                if (values[0] != "") {
                                    item2num.Add(values[0].ToLower(), i);
                                    num2item.Add(i, values[0]);
                                }
                                i++;
                            } else {
                                firstline = false;
                            }
                        }
                    }
                    using (var itemData = new StreamReader(cwd + "Mods/" + Globals.MOD + "/Items.tsv")) {
                        bool firstline = true;
                        byte i = 192;
                        while (!itemData.EndOfStream) {
                            var line = itemData.ReadLine();
                            if (firstline == false) {
                                var values = line.Split('\t').ToArray();
                                itemList.Add(new Item(i, values));
                                if (values[0] != "") {
                                    item2num.Add(values[0].ToLower(), i);
                                    num2item.Add(i, values[0]);
                                }
                                i++;
                            } else {
                                firstline = false;
                            }
                        }
                    }
                    long offset = 0x0;
                    long start = Constants.GetAddress("ITEM_DESC");
                    List<dynamic> sortedList = itemList.OrderByDescending(o => o.Description.Length).ToList();
                    descriptionList = new List<string>();
                    foreach (dynamic item in sortedList) {
                        if (descriptionList.Any(l => l.Contains(item.EncodedDescription)) == true) {
                            int index = sortedList.IndexOf(sortedList.Find(x => x.EncodedDescription.Contains(item.EncodedDescription)));
                            item.DescriptionPointer = sortedList[index].DescriptionPointer + (sortedList[index].Description.Length - item.Description.Length) * 2;
                        } else {
                            descriptionList.Add(item.EncodedDescription);
                            item.DescriptionPointer = start + offset;
                            offset += (item.EncodedDescription.Replace(" ", "").Length / 2);

                        }
                    }

                    offset = 0x0;
                    start = Constants.GetAddress("ITEM_BTL_DESC");
                    var tempList = itemList.GetRange(193, 63);
                    sortedList = tempList.OrderByDescending(o => o.BattleDescription.Length).ToList();
                    battleDescriptionList = new List<string>();
                    foreach (dynamic item in sortedList) {
                        if (battleDescriptionList.Any(l => l.Contains(item.EncodedBattleDescription)) == true) {
                            int index = sortedList.IndexOf(sortedList.Find(x => x.EncodedBattleDescription.Contains(item.EncodedBattleDescription)));
                            item.BattleDescriptionPointer = sortedList[index].BattleDescriptionPointer + (sortedList[index].BattleDescription.Length - item.BattleDescription.Length) * 2;
                        } else {
                            battleDescriptionList.Add(item.EncodedBattleDescription);
                            item.BattleDescriptionPointer = start + offset;
                            offset += (item.EncodedBattleDescription.Replace(" ", "").Length / 2);

                        }
                    }


                    offset = 0;
                    start = Constants.GetAddress("ITEM_NAME");
                    sortedList = itemList.OrderByDescending(o => o.Name.Length).ToList();
                    nameList = new List<string>();
                    foreach (dynamic item in sortedList) {
                        if (nameList.Any(l => l.Contains(item.EncodedName)) == true) {
                            int index = sortedList.IndexOf(sortedList.Find(x => x.EncodedName.Contains(item.EncodedName)));
                            item.NamePointer = sortedList[index].NamePointer + (sortedList[index].Name.Length - item.Name.Length) * 2;
                        } else {
                            nameList.Add(item.EncodedName);
                            item.NamePointer = start + (int) offset;
                            offset += (item.EncodedName.Replace(" ", "").Length / 2);
                        }
                    }

                    offset = 0;
                    start = Constants.GetAddress("ITEM_BTL_NAME");
                    sortedList = itemList.GetRange(193, 63).OrderByDescending(o => o.Name.Length).ToList();
                    battleNameList = new List<string>();
                    foreach (dynamic item in sortedList) {
                        if (battleNameList.Any(l => l.Contains(item.EncodedName)) == true) {
                            int index = sortedList.IndexOf(sortedList.Find(x => x.EncodedName.Contains(item.EncodedName)));
                            item.BattleNamePointer = sortedList[index].BattleNamePointer + (sortedList[index].Name.Length - item.Name.Length) * 2;
                        } else {
                            battleNameList.Add(item.EncodedName);
                            item.BattleNamePointer = start + (int) offset;
                            offset += (item.EncodedName.Replace(" ", "").Length / 2);
                        }
                    }

                    foreach (var line in ReadAllResourceLines(Properties.Resources.Items)) {
                        if (origI > 0) {
                            var values = line.Split('\t').ToArray();
                            originalItemList.Add(new Equipment(origI, values));
                        }
                        origI++;
                    }

                    for (int i = 0; i < characterStats.Length; i++) {
                        characterStats[i] = new dynamic[61];
                        originalCharacterStats[i] = new dynamic[61];
                    }
                    try {
                        using (var characterData = new StreamReader(cwd + "Mods/" + Globals.MOD + "/Character_Stats.tsv")) {
                            var i = 0;
                            while (!characterData.EndOfStream) {
                                var line = characterData.ReadLine();
                                if (i > 1) {
                                    var values = line.Split('\t').ToArray();
                                    int level = int.Parse(values[0]);
                                    characterStats[0][level] = new CharacterStats(values[1], values[2], values[3], values[4], values[5], values[6]);
                                    characterStats[1][level] = new CharacterStats(values[7], values[8], values[9], values[10], values[11], values[12]);
                                    characterStats[2][level] = new CharacterStats(values[13], values[14], values[15], values[16], values[17], values[18]);
                                    characterStats[3][level] = new CharacterStats(values[19], values[20], values[21], values[22], values[23], values[24]);
                                    characterStats[4][level] = new CharacterStats(values[25], values[26], values[27], values[28], values[29], values[30]);
                                    characterStats[5][level] = new CharacterStats(values[7], values[8], values[9], values[10], values[11], values[12]);
                                    characterStats[6][level] = new CharacterStats(values[31], values[32], values[33], values[34], values[35], values[36]);
                                    characterStats[7][level] = new CharacterStats(values[37], values[38], values[39], values[40], values[41], values[42]);
                                    characterStats[8][level] = new CharacterStats(values[13], values[14], values[15], values[16], values[17], values[18]);
                                }
                                i++;
                            }
                        }
                    } catch (FileNotFoundException) {
                        string file = cwd + @"Mods\" + Globals.MOD + @"\Character_Stats.tsv";
                        Constants.WriteError(file + " not found. Turning off Stat and Equip Changes.");
                    }

                    origI = 0;
                    foreach (var line in ReadAllResourceLines(Properties.Resources.Character_Stats)) {
                        if (origI > 1) {
                            var values = line.Split('\t').ToArray();
                            int level = int.Parse(values[0]);
                            originalCharacterStats[0][level] = new CharacterStats(values[1], values[2], values[3], values[4], values[5], values[6]);
                            originalCharacterStats[1][level] = new CharacterStats(values[7], values[8], values[9], values[10], values[11], values[12]);
                            originalCharacterStats[2][level] = new CharacterStats(values[13], values[14], values[15], values[16], values[17], values[18]);
                            originalCharacterStats[3][level] = new CharacterStats(values[19], values[20], values[21], values[22], values[23], values[24]);
                            originalCharacterStats[4][level] = new CharacterStats(values[25], values[26], values[27], values[28], values[29], values[30]);
                            originalCharacterStats[5][level] = new CharacterStats(values[7], values[8], values[9], values[10], values[11], values[12]);
                            originalCharacterStats[6][level] = new CharacterStats(values[31], values[32], values[33], values[34], values[35], values[36]);
                            originalCharacterStats[7][level] = new CharacterStats(values[37], values[38], values[39], values[40], values[41], values[42]);
                            originalCharacterStats[8][level] = new CharacterStats(values[13], values[14], values[15], values[16], values[17], values[18]);
                        }
                        origI++;
                    }

                    try {
                        using (var monsterData = new StreamReader(cwd + "Mods/" + Globals.MOD + "/Monster_Data.tsv")) {
                            bool firstline = true;
                            while (!monsterData.EndOfStream) {
                                var line = monsterData.ReadLine();
                                if (firstline == false) {
                                    var values = line.Split('\t').ToArray();
                                    statList.Add(Int32.Parse(values[0]), new StatList(values, element2num, item2num));
                                } else {
                                    firstline = false;
                                }
                            }
                        }
                    } catch (FileNotFoundException) {
                        string file = cwd + @"Mods\" + Globals.MOD + @"\Monster_Data.tsv";
                        Constants.WriteError(file + " not found. Turning off Monster and Drop Changes.");
                        Globals.MONSTER_STAT_CHANGE = false;
                        Globals.MONSTER_DROP_CHANGE = false;
                        Globals.MONSTER_EXPGOLD_CHANGE = false;
                    }

                    origI = 0;
                    foreach (var line in ReadAllResourceLines(Properties.Resources.Ultimate_Data)) {
                        if (origI > 0) {
                            var values = line.Split('\t').ToArray();
                            ultimateStatList.Add(Int32.Parse(values[0]), new StatList(values, element2num, item2num));
                        }
                        origI++;
                    }
                } catch (FileNotFoundException) {
                    string file = cwd + @"Mods\" + Globals.MOD + @"\Items.tsv";
                    Constants.WriteError(file + " not found. Turning off Monster, Drop, and Item Changes.");
                    Globals.MONSTER_STAT_CHANGE = false;
                    Globals.MONSTER_DROP_CHANGE = false;
                    Globals.MONSTER_EXPGOLD_CHANGE = false;
                    Globals.ITEM_ICON_CHANGE = false;
                    Globals.ITEM_STAT_CHANGE = false;
                    Globals.ITEM_NAMEDESC_CHANGE = false;
                }
                try {
                    string[] lines = File.ReadAllLines(cwd + @"Mods\" + Globals.MOD + @"\Monster_Script.txt");
                    foreach (string row in lines) {
                        if (row != "") {
                            monsterScript.Add(Int32.Parse(row));
                        }
                    }
                } catch (FileNotFoundException) {
                    string file = cwd + @"Mods\" + Globals.MOD + @"\Monster_Script.txt";
                    Constants.WriteDebug(file + " not found.");
                }
                try {
                    using (var dragoon = new StreamReader(cwd + "Mods/" + Globals.MOD + "/Dragoon_Stats.tsv")) {
                        bool firstline = true;
                        var i = 0;
                        while (!dragoon.EndOfStream) {
                            var line = dragoon.ReadLine();
                            if (firstline == false) {
                                var values = line.Split('\t').ToArray();
                                dragoonStats[i] = new dynamic[] {
                                    new DragoonStats("0", "0", "0", "0", "0"),
                                    new DragoonStats(values[1], values[2], values[3], values[4], values[5]),
                                    new DragoonStats(values[6], values[7], values[8], values[9], values[10]),
                                    new DragoonStats(values[11], values[12], values[13], values[14], values[15]),
                                    new DragoonStats(values[16], values[17], values[18], values[19], values[20]),
                                    new DragoonStats(values[21], values[22], values[23], values[24], values[25])
                                };
                                i++;
                            } else {
                                firstline = false;
                            }
                        }
                    }
                } catch (FileNotFoundException) {
                    string file = cwd + @"Mods\" + Globals.MOD + @"\Dragoon_Stats.tsv";
                    Constants.WriteError(file + " not found. Turning off Dragoon Changes.");
                    Globals.DRAGOON_STAT_CHANGE = false;
                }

                for (int shop = 0; shop < 44; shop++) {
                    shopList[shop] = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
                }
                try {
                    byte key = 0;
                    using (var shop = new StreamReader(cwd + "Mods/" + Globals.MOD + "/Shops.tsv")) {
                        var row = 0;
                        while (!shop.EndOfStream) {
                            var line = shop.ReadLine();
                            if (row > 0 && row < 17) {
                                var values = line.Split('\t').ToArray();
                                for (int column = 0; column < 44; column++) {
                                    if (item2num.TryGetValue(values[column].ToLower(), out key)) {
                                        shopList[column][row - 1] = key;
                                    } else {
                                        if (values[column] != "") {
                                            Constants.WriteDebug(values[column] + " not found in as item in Shops.tsv");
                                        }
                                        shopList[column][row - 1] = 0xFF;
                                    }
                                }
                            }
                            row++;
                        }
                    }
                } catch (FileNotFoundException) {
                    string file = cwd + @"Mods\" + Globals.MOD + @"\Shops.tsv";
                    Constants.WriteError(file + " not found. Turning off Shop Changes.");
                    Globals.SHOP_CHANGE = false;
                }
                try {
                    Globals.DRAGOON_SPELLS = new List<dynamic>();
                    using (var spell = new StreamReader(cwd + "Mods/" + Globals.MOD + "/Dragoon_Spells.tsv")) {
                        var i = 0;
                        while (!spell.EndOfStream) {
                            var line = spell.ReadLine();
                            if (i > 0) {
                                var values = line.Split('\t').ToArray();
                                Globals.DRAGOON_SPELLS.Add(new DragoonSpells(values, i - 1, element2num));
                            }
                            i++;
                        }
                    }
                    long offset = 0x0;
                    long start = Constants.GetAddress("DRAGOON_DESC");
                    foreach (dynamic spell in Globals.DRAGOON_SPELLS) {
                        spell.Description_Pointer = start + offset;
                        offset += (spell.Encoded_Description.Replace(" ", "").Length / 2);

                    }
                } catch (FileNotFoundException) {
                    string file = cwd + @"Mods\" + Globals.MOD + @"\Dragoon_Spells.tsv";
                    Constants.WriteError(file + " not found. Turning off Dragoon Changes.");
                    Globals.DRAGOON_SPELL_CHANGE = false;
                }
                try {
                    using (var addition = new StreamReader(cwd + "Mods/" + Globals.MOD + "/Additions.tsv")) {
                        var i = 0;
                        bool firstline = true;
                        while (!addition.EndOfStream) {
                            var line = addition.ReadLine();
                            if (firstline == false) {
                                var values = line.Split('\t').ToArray();
                                additionData[0, i / 8, i % 8] = new AdditionData(values.Skip(1).Take(26).ToArray());
                                additionData[1, i / 8, i % 8] = new AdditionData(values.Skip(28).Take(53).ToArray());
                                additionData[2, i / 8, i % 8] = new AdditionData(values.Skip(55).Take(80).ToArray());
                                additionData[3, i / 8, i % 8] = new AdditionData(values.Skip(82).Take(107).ToArray());
                                additionData[4, i / 8, i % 8] = new AdditionData(values.Skip(109).Take(134).ToArray());
                                additionData[5, i / 8, i % 8] = new AdditionData(values.Skip(136).Take(161).ToArray());
                                additionData[6, i / 8, i % 8] = new AdditionData(values.Skip(163).Take(188).ToArray());
                                additionData[7, i / 8, i % 8] = new AdditionData(values.Skip(190).Take(215).ToArray());
                                additionData[8, i / 8, i % 8] = new AdditionData(values.Skip(217).Take(242).ToArray());
                                i++;
                            } else {
                                firstline = false;
                            }
                        }
                    }
                } catch (FileNotFoundException) {
                    string file = cwd + @"Mods\" + Globals.MOD + @"\Additions.tsv";
                    Constants.WriteError(file + " not found. Turning off Addition Changes.");
                    Globals.ADDITION_CHANGE = false;
                }
                try {
                    using (var dAddition = new StreamReader(cwd + "Mods/" + Globals.MOD + "/Dragoon_Additions.tsv")) {
                        bool firstline = true;
                        var i = 0;
                        while (!dAddition.EndOfStream) {
                            var line = dAddition.ReadLine();
                            if (firstline == false) {
                                var values = line.Split('\t').ToArray();
                                dragoonAddition[i] = new DragoonAdditionStats(values[1], values[2], values[3], values[4], values[5]);
                                i++;
                            } else {
                                firstline = false;
                            }
                        }
                    }
                } catch (FileNotFoundException) {
                    string file = cwd + @"Mods\" + Globals.MOD + @"\Dragoon_Additions.tsv";
                    Constants.WriteError(file + " not found. Turning off Dragoon Addition Changes.");
                    Globals.DRAGOON_ADDITION_CHANGE = false;
                }
                try {
                    using (var additionLevelData = new StreamReader(cwd + "Mods/" + Globals.MOD + "/Addition_Levels.tsv")) {
                        bool firstline = true;
                        int i = 0;
                        while (!additionLevelData.EndOfStream) {
                            var line = additionLevelData.ReadLine();
                            if (!firstline) {
                                var values = line.Split('\t').ToArray();
                                if (values[1].ToLower() == "final") {
                                    additionLevels[i] = 255;
                                } else if (Byte.TryParse(values[1], NumberStyles.AllowLeadingSign, null as IFormatProvider, out byte key)) {
                                    additionLevels[i] = key;
                                } else {
                                    Constants.WriteDebug($"{values[1]} not found as Level for Addition: {values[0]}.");
                                }
                                i++;
                            } else {
                                firstline = false;
                            }
                        }
                    }

                } catch (FileNotFoundException) {
                    string file = cwd + @"Mods\" + Globals.MOD + @"\Addition_Levels.tsv";
                    Constants.WriteError(file + " not found. Turning off Addition Level Changes.");
                    Globals.ADDITION_LEVEL_CHANGE = false;
                }
            } catch (DirectoryNotFoundException ex) {
                if (!Globals.MOD.Equals("US_Base")) {
                    Constants.WriteOutput("Trying to reinitalize with US_Base...");
                    Globals.MOD = "US_Base";
                    Globals.DICTIONARY = new LoDDict();
                } else {
                    Constants.RUN = false;
                    Constants.WriteGLog("Program stopped.");
                    Constants.WritePLogOutput("LOD Dictionary fatal error, US_BASE not found.");
                    Constants.WriteOutput("Fatal Error. Closing all threads.");
                    Constants.WriteError(ex.ToString());
                }
            } catch (Exception ex) {
                Constants.RUN = false;
                Constants.WriteGLog("Program stopped.");
                Constants.WritePLogOutput("LOD Dictionary fatal error.");
                Constants.WriteOutput("Fatal Error. Closing all threads.");
                Constants.WriteError(ex.ToString());
            }
        }

        public void SwapMonsterStats(string mod) {
            string cwd = AppDomain.CurrentDomain.BaseDirectory;
            try {
                using (var monsterData = new StreamReader(cwd + "Mods/" + mod + "/Monster_Data.tsv")) {
                    Globals.DICTIONARY.StatList.Clear();
                    bool firstline = true;
                    while (!monsterData.EndOfStream) {
                        var line = monsterData.ReadLine();
                        if (firstline == false) {
                            var values = line.Split('\t').ToArray();
                            Globals.DICTIONARY.StatList.Add(Int32.Parse(values[0]), new StatList(values, element2num, item2num));
                        } else {
                            firstline = false;
                        }
                    }
                }
                Constants.WritePLogOutput("Mod stats swapped to: " + mod);
            } catch (FileNotFoundException) {
                string file = cwd + @"Mods\" + Globals.MOD + @"\Monster_Data.tsv";
                Constants.WriteError(file + " not found. Turning off Monster and Drop Changes.");
                Globals.MONSTER_STAT_CHANGE = false;
                Globals.MONSTER_DROP_CHANGE = false;
                Globals.MONSTER_EXPGOLD_CHANGE = false;
            }
        }

        public static string StringEncode(string text) {
            IDictionary<string, string> codeDict = new Dictionary<string, string>() {
                {"<END>", "FF A0" },
                {"<LINE>", "FF A1" },
                {"<GOLD>", "00 A8" },
                {"<WHITE>", "00 A7" },
                {"<RED>", "05 A7" },
                {"<YELLOW>", "08 A7" }
            };
            IDictionary<char, string> symbolDict = new Dictionary<char, string>() {
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
            List<string> encoded = new List<string>();
            string[] parts = Regex.Split(text, @"(<[\s\S]+?>)").Where(l => l != string.Empty).ToArray();
            foreach (string segment in parts) {
                if (segment.StartsWith("<")) {
                    encoded.Add(codeDict[segment]);
                } else {
                    List<string> temp = new List<string>();
                    foreach (char letter in segment.ToCharArray()) {
                        temp.Add(symbolDict[letter]);
                    }
                    encoded.Add(String.Join(" ", temp.ToArray()));
                }
            }
            return String.Join(" ", encoded.ToArray());
        }
    }

    public class Equipment {
        int id = 0;
        string name = "<END>";
        string description = "<END>";
        string encodedName = "FF A0 FF A0";
        string encodedDescription = "FF A0 FF A0";
        long descriptionPointer = 0;
        long namePointer = 0;
        byte icon = 0;
        byte equips = 0;
        byte type = 0;
        byte element = 0;
        byte on_hit_status = 0;
        byte status_chance = 0;
        byte at = 0;
        byte mat = 0;
        byte df = 0;
        byte mdf = 0;
        byte spd = 0;
        byte a_hit = 0;
        byte m_hit = 0;
        byte a_av = 0;
        byte m_av = 0;
        byte e_half = 0;
        byte e_immune = 0;
        byte stat_res = 0;
        byte special1 = 0;
        byte special2 = 0;
        short special_ammount = 0;
        byte death_res = 0;
        short sell_price = 0;

        #region Dictionaries

        Dictionary<string, byte> iconDict = new Dictionary<string, byte>() {
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
                { "scarf", 31 },
                { "horn", 32 },
                {"none", 64 },
                {"", 64 }
            };
        Dictionary<string, byte> charDict = new Dictionary<string, byte>() {
                {"meru", 1 },
                {"shana", 2 },
                {"miranda", 2 },
                {"???", 2 },
                {"rose", 4 },
                {"haschel", 16 },
                {"kongol", 32 },
                {"lavitz", 64 },
                {"albert", 64 },
                {"dart", 128 },
                {"female", 7 },
                {"male", 240 },
                {"all", 247 },
                {"none", 0 },
                {"", 0 }
            };
        Dictionary<string, byte> typeDict = new Dictionary<string, byte>() {
                {"weapon", 128 },
                {"armor", 32 },
                {"helm", 64 },
                {"boots", 16 },
                {"accessory", 8 },
                {"none", 0},
                {"", 0 }
            };
        Dictionary<string, byte> element2num = new Dictionary<string, byte>() {
                {"", 0 },
                {"none", 0 },
                {"water", 1 },
                {"earth", 2 },
                {"dark", 4 },
                {"non-elemental", 8 },
                {"thunder", 16 },
                {"light", 32 },
                {"wind", 64 },
                {"fire", 128 }
            };
        Dictionary<string, byte> status2num = new Dictionary<string, byte>() {
                {"", 0 },
                {"none", 0 },
                {"death", 0 },
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
        Dictionary<string, byte> special12num = new Dictionary<string, byte>() {
                {"", 0 },
                {"none", 0 },
                {"mp_m_hit", 1 },
                {"sp_m_hit", 2 },
                {"mp_p_hit", 4 },
                {"sp_p_hit", 8 },
                {"sp_multi", 16 },
                {"p_half", 32 }
            };
        Dictionary<string, byte> special22num = new Dictionary<string, byte>() {
                {"", 0 },
                {"none", 0 },
                {"mp_multi", 1 },
                {"hp_multi", 2 },
                {"m_half", 4 },
                {"revive", 8 },
                {"sp_regen", 16 },
                {"mp_regen", 32 },
                {"hp_regen", 64 }
            };
        Dictionary<string, byte> death2num = new Dictionary<string, byte>() {
                {"", 0 },
                {"none", 0 },
                {"dragon_buster", 4 },
                {"attack_all", 8 },
                {"death_chance", 64 },
                {"death_res", 128 }
            };

        #endregion

        public int ID { get { return id; } }
        public string Name { get { return name; } }
        public string Description { get { return description; } }
        public string EncodedName { get { return encodedName; } }
        public string EncodedDescription { get { return encodedDescription; } }
        public long DescriptionPointer { get; set; }
        public long NamePointer { get; set; }
        public byte Icon { get { return icon; } }
        public byte Equips { get { return equips; } }
        public byte Type { get { return type; } }
        public byte Element { get { return element; } }
        public byte On_Hit_Status { get { return on_hit_status; } }
        public byte Status_Chance { get { return status_chance; } }
        public byte AT { get { return at; } }
        public byte MAT { get { return mat; } }
        public byte DF { get { return df; } }
        public byte MDF { get { return mdf; } }
        public byte SPD { get { return spd; } }
        public byte A_Hit { get { return a_hit; } }
        public byte M_Hit { get { return m_hit; } }
        public byte A_AV { get { return a_av; } }
        public byte M_AV { get { return m_av; } }
        public byte E_Half { get { return e_half; } }
        public byte E_Immune { get { return e_immune; } }
        public byte Stat_Res { get { return stat_res; } }
        public byte Special1 { get { return special1; } }
        public byte Special2 { get { return special2; } }
        public short Special_Ammount { get { return special_ammount; } }
        public byte Death_Res { get { return death_res; } }
        public short Sell_Price { get { return sell_price; } }

        public Equipment(int index, string[] values) {
            byte key = 0;
            short key2 = 0;
            id = index;
            name = values[0];
            if (!(name == "" || name == " ")) {
                encodedName = LoDDict.StringEncode(name) + " FF A0";
            }
            if (typeDict.TryGetValue(values[1].ToLower(), out key)) {
                type = key;
            } else {
                Constants.WriteDebug(values[1] + " not found as equipment type for item: " + name);
            }
            foreach (string substring in values[2].Replace(" ", "").Split(',')) {
                if (charDict.TryGetValue(substring.ToLower(), out key)) {
                    equips |= key;
                } else {
                    Constants.WriteDebug(substring + " not found as character for item: " + name);
                }
            }
            if (iconDict.TryGetValue(values[3].ToLower(), out key)) {
                icon = key;
            } else {
                Constants.WriteDebug(values[3] + " not found as icon for item: " + name);
            }
            if (element2num.TryGetValue(values[4].ToLower(), out key)) {
                element = key;
            } else {
                Constants.WriteDebug(values[4] + " not found as element for item: " + name);
            }
            if (status2num.TryGetValue(values[5].ToLower(), out key)) {
                on_hit_status = key;
            } else {
                Constants.WriteDebug(values[5] + " not found as Status for item: " + name);
            }
            if (Byte.TryParse(values[6], NumberStyles.AllowLeadingSign, null as IFormatProvider, out key)) {
                status_chance = key;
            } else if (values[6] != "") {
                Constants.WriteDebug(values[6] + " not found as Status Chance for item: " + name);
            }
            if (Byte.TryParse(values[7], NumberStyles.AllowLeadingSign, null as IFormatProvider, out key)) {
                at = key;
            } else if (values[7] != "") {
                Constants.WriteDebug(values[7] + " not found as AT for item: " + name);
            }
            if (Byte.TryParse(values[8], NumberStyles.AllowLeadingSign, null as IFormatProvider, out key)) {
                mat = key;
            } else if (values[8] != "") {
                Constants.WriteDebug(values[8] + " not found as MAT for item: " + name);
            }
            if (Byte.TryParse(values[9], NumberStyles.AllowLeadingSign, null as IFormatProvider, out key)) {
                df = key;
            } else if (values[9] != "") {
                Constants.WriteDebug(values[9] + " not found as DF for item: " + name);
            }
            if (Byte.TryParse(values[10], NumberStyles.AllowLeadingSign, null as IFormatProvider, out key)) {
                mdf = key;
            } else if (values[10] != "") {
                Constants.WriteDebug(values[10] + " not found as MDF for item: " + name);
            }
            if (Byte.TryParse(values[11], NumberStyles.AllowLeadingSign, null as IFormatProvider, out key)) {
                spd = key;
            } else if (values[11] != "") {
                Constants.WriteDebug(values[11] + " not found as SPD for item: " + name);
            }
            if (Byte.TryParse(values[12], NumberStyles.AllowLeadingSign, null as IFormatProvider, out key)) {
                a_hit = key;
            } else if (values[12] != "") {
                Constants.WriteDebug(values[12] + " not found as A_Hit for item: " + name);
            }
            if (Byte.TryParse(values[13], NumberStyles.AllowLeadingSign, null as IFormatProvider, out key)) {
                m_hit = key;
            } else if (values[13] != "") {
                Constants.WriteDebug(values[13] + " not found as M_Hit for item: " + name);
            }
            if (Byte.TryParse(values[14], NumberStyles.AllowLeadingSign, null as IFormatProvider, out key)) {
                a_av = key;
            } else if (values[14] != "") {
                Constants.WriteDebug(values[14] + " not found as A_AV for item: " + name);
            }
            if (Byte.TryParse(values[15], NumberStyles.AllowLeadingSign, null as IFormatProvider, out key)) {
                m_av = key;
            } else if (values[15] != "") {
                Constants.WriteDebug(values[15] + " not found as M_AV for item: " + name);
            }
            foreach (string substring in values[16].Replace(" ", "").Split(',')) {
                if (element2num.TryGetValue(substring.ToLower(), out key)) {
                    e_half |= key;
                } else {
                    Constants.WriteDebug(substring + " not found as E_Half for item:" + name);
                }
            }
            foreach (string substring in values[17].Replace(" ", "").Split(',')) {
                if (element2num.TryGetValue(substring.ToLower(), out key)) {
                    e_immune |= key;
                } else {
                    Constants.WriteDebug(substring + " not found as E_Immune for item:" + name);
                }
            }
            foreach (string substring in values[18].Replace(" ", "").Split(',')) {
                if (status2num.TryGetValue(substring.ToLower(), out key)) {
                    stat_res |= key;
                } else {
                    Constants.WriteDebug(substring + " not found as Status_Resist for item:" + name);
                }
            }
            foreach (string substring in values[19].Replace(" ", "").Split(',')) {
                if (special12num.TryGetValue(substring.ToLower(), out key)) {
                    special1 |= key;
                } else {
                    Constants.WriteDebug(substring + " not found as Special1 for item:" + name);
                }
            }
            foreach (string substring in values[20].Replace(" ", "").Split(',')) {
                if (special22num.TryGetValue(substring.ToLower(), out key)) {
                    special2 |= key;
                } else {
                    Constants.WriteDebug(substring + " not found as Special2 for item:" + name);
                }
            }
            if (Int16.TryParse(values[21], NumberStyles.AllowLeadingSign, null as IFormatProvider, out key2)) {
                special_ammount = key2;
            } else if (values[21] != "") {
                Constants.WriteDebug(values[21] + " not found as Special_Ammount for item: " + name);
            }
            foreach (string substring in values[22].Replace(" ", "").Split(',')) {
                if (death2num.TryGetValue(substring.ToLower(), out key)) {
                    death_res |= key;
                } else {
                    Constants.WriteDebug(substring + " not found as Death_Res for item:" + name);
                }
            }
            description = values[23];
            if (!(description == "" || description == " ")) {
                encodedDescription = LoDDict.StringEncode(description) + " FF A0";
            }
            if (UInt16.TryParse(values[24], NumberStyles.AllowLeadingSign, null as IFormatProvider, out ushort key3)) {
                float temp = (float) key3 / 2;
                sell_price = (short) Math.Round(temp);
            } else if (values[24] != "") {
                Constants.WriteDebug(values[24] + " not found as Price for item: " + name);
            }
        }
    }

    public class Item {
        byte id = 0;
        string name = "<END>";
        string description = "<END>";
        string encodedName = "FF A0 FF A0";
        string encodedDescription = "FF A0 FF A0";
        string battleDescription = "<END>";
        string encondedBattleDescription = "FF A0 FF A0";
        long descriptionPointer = 0;
        long namePointer = 0;
        long battleDescriptionPointer = 0;
        long battleNamePointer = 0;
        byte target = 0;
        byte element = 0;
        byte damage = 0;
        byte special1 = 0;
        byte special2 = 0;
        byte uu1 = 0;
        byte special_ammount = 0;
        byte icon = 0x26;
        byte status = 0;
        byte percentage = 0;
        byte uu2 = 0;
        byte baseSwitch = 0;
        short sell_price = 0;


        #region Dictionaries

        Dictionary<string, byte> iconDict = new Dictionary<string, byte>() {
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
                { "jewelery", 26 },
                { "ankh", 27 },
                { "bell", 28 },
                { "bag", 29 },
                { "cloak", 30 },
                { "scarf", 31 },
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
                {"none", 64 },
                {"", 64 }
            };
        Dictionary<string, byte> element2num = new Dictionary<string, byte>() {
                {"", 0 },
                {"none", 0 },
                {"water", 1 },
                {"earth", 2 },
                {"dark", 4 },
                {"non-elemental", 8 },
                {"thunder", 16 },
                {"light", 32 },
                {"wind", 64 },
                {"fire", 128 }
            };
        Dictionary<string, byte> status2num = new Dictionary<string, byte>() {
                {"", 0 },
                {"none", 0 },
                {"death", 0 },
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
        Dictionary<string, byte> special12num = new Dictionary<string, byte>() {
                {"", 0 },
                {"none", 0 },
                {"power1", 0x10 },
                {"power2", 0x20 },
                {"power3", 0x40 },
                {"power4", 0x80 }
            };
        Dictionary<string, byte> special22num = new Dictionary<string, byte>() {
                {"", 0 },
                {"none", 0 },
                {"speed_down", 0x10 },
                {"speed_up", 0x20 },
                {"magic_res", 0x40 },
                {"physical_res", 0x80 }
            };
        Dictionary<string, byte> base_table = new Dictionary<string, byte>() {

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
        Dictionary<string, byte> base_dict = new Dictionary<string, byte>() {
            {"", 0x0 },
            {"none", 0x0 },
            {"status_cause", 0x4 },
            {"status_cure", 0x8 },
            {"revive", 0x10 },
            {"sp", 0x20 },
            {"mp", 0x40 },
            {"hp", 0x80 }
        };

        #endregion

        public byte ID { get { return id; } }
        public string Name { get { return name; } }
        public string Description { get { return description; } }
        public string BattleDescription { get { return battleDescription; } }
        public string EncodedName { get { return encodedName; } }
        public string EncodedDescription { get { return encodedDescription; } }
        public string EncodedBattleDescription { get { return encondedBattleDescription; } }
        public long DescriptionPointer { get; set; }
        public long NamePointer { get; set; }
        public long BattleDescriptionPointer { get; set; }
        public long BattleNamePointer { get; set; }
        public byte Target { get { return target; } }
        public byte Element { get { return element; } }
        public byte Damage { get { return damage; } }
        public byte Special1 { get { return special1; } }
        public byte Special2 { get { return special2; } }
        public byte UU1 { get { return uu1; } }
        public byte Special_Ammount { get { return special_ammount; } }
        public byte Icon { get { return icon; } }
        public byte Status { get { return status; } }
        public byte Percentage { get { return percentage; } }
        public byte UU2 { get { return uu2; } }
        public byte BaseSwitch { get { return baseSwitch; } }
        public short Sell_Price { get { return sell_price; } }

        public Item(byte index, string[] values) {
            byte key = 0;
            short key2 = 0;
            id = index;
            name = values[0];
            if (!(name == "" || name == " ")) {
                encodedName = LoDDict.StringEncode(name) + " FF A0";
            }
            if (Byte.TryParse(values[1], NumberStyles.AllowLeadingSign, null as IFormatProvider, out key)) {
                target = key;
            } else if (values[1] != "") {
                Constants.WriteDebug(values[1] + " not found as Target for item: " + name);
            }
            if (element2num.TryGetValue(values[2].ToLower(), out key)) {
                element = key;
            } else {
                Constants.WriteDebug(values[2] + " not found as Element for item: " + name);
            }
            if (base_table.TryGetValue(values[3], out key)) {
                damage = key;
            } else if (values[3] != "") {
                Constants.WriteDebug(values[3] + " not found as Damage for item: " + name);
            }
            foreach (string substring in values[4].Replace(" ", "").Split(',')) {
                if (special12num.TryGetValue(substring.ToLower(), out key)) {
                    special1 |= key;
                } else {
                    Constants.WriteDebug(substring + " not found as Special1 for item:" + name);
                }
            }
            foreach (string substring in values[5].Replace(" ", "").Split(',')) {
                if (special22num.TryGetValue(substring.ToLower(), out key)) {
                    special2 |= key;
                } else {
                    Constants.WriteDebug(substring + " not found as Special2 for item:" + name);
                }
            }
            if (Byte.TryParse(values[6], NumberStyles.AllowLeadingSign, null as IFormatProvider, out key)) {
                uu1 = key;
            } else if (values[6] != "") {
                Constants.WriteDebug(values[6] + " not found as UU1 for item: " + name);
            }
            if (Int16.TryParse(values[7], NumberStyles.AllowLeadingSign, null as IFormatProvider, out key2)) {
                special_ammount = (byte )key2;
            } else if (values[7] != "") {
                Constants.WriteDebug(values[7] + " not found as Special_Ammount for item: " + name);
            }
            if (iconDict.TryGetValue(values[8].ToLower(), out key)) {
                icon = key;
            } else {
                Constants.WriteDebug(values[8] + " not found as icon for item: " + name);
            }
            foreach (string substring in values[9].Replace(" ", "").Split(',')) {
                if (status2num.TryGetValue(substring.ToLower(), out key)) {
                    status |= key;
                } else {
                    Constants.WriteDebug(substring + " not found as Status for item:" + name);
                }
            }
            if (Byte.TryParse(values[10], NumberStyles.AllowLeadingSign, null as IFormatProvider, out key)) {
                percentage = key;
            } else if (values[10] != "") {
                Constants.WriteDebug(values[10] + " not found as Percentage for item: " + name);
            }
            if (Byte.TryParse(values[11], NumberStyles.AllowLeadingSign, null as IFormatProvider, out key)) {
                uu2 = key;
            } else if (values[11] != "") {
                Constants.WriteDebug(values[11] + " not found as UU2 for item: " + name);
            }
            foreach (string substring in values[12].Replace(" ", "").Split(',')) {
                if (base_dict.TryGetValue(substring.ToLower(), out key)) {
                    baseSwitch |= key;
                } else {
                    Constants.WriteDebug(substring + " not found as Base_Switch for item:" + name);
                }
            }
            description = values[13];
            if (!(description == "" || description == " ")) {
                encodedDescription = LoDDict.StringEncode(description) + " FF A0";
            }
            if (UInt16.TryParse(values[14], NumberStyles.AllowLeadingSign, null as IFormatProvider, out ushort key3)) {
                float temp = (float)key3 / 2;
                sell_price = (short) Math.Round(temp);
            } else if (values[14] != "") {
                Constants.WriteDebug(values[14] + " not found as Price for item: " + name);
            }
            battleDescription = values[15];
            if (!(battleDescription == "" || battleDescription == " ")) {
                encondedBattleDescription = LoDDict.StringEncode(battleDescription) + " FF A0";
            }
        }
    }

    public class StatList {
        string name = "Monster";
        byte element = 128;
        int hp = 1;
        ushort at = 1;
        ushort mat = 1;
        ushort df = 1;
        ushort mdf = 1;
        ushort spd = 1;
        byte a_av = 0;
        byte m_av = 0;
        byte p_immune = 0;
        byte m_immune = 0;
        byte p_half = 0;
        byte m_half = 0;
        byte e_immune = 0;
        byte e_half = 0;
        byte stat_res = 0;
        byte death_res = 0;
        ushort exp = 0;
        ushort gold = 0;
        byte drop_item = 255;
        byte drop_chance = 0;

        public string Name { get { return name; } }
        public byte Element { get { return element; } }
        public int HP { get { return hp; } }
        public ushort AT { get { return at; } }
        public ushort MAT { get { return mat; } }
        public ushort DF { get { return df; } }
        public ushort MDF { get { return mdf; } }
        public ushort SPD { get { return spd; } }
        public byte A_AV { get { return a_av; } }
        public byte M_AV { get { return m_av; } }
        public byte P_Immune { get { return p_immune; } }
        public byte M_Immune { get { return m_immune; } }
        public byte P_Half { get { return p_half; } }
        public byte M_Half { get { return m_half; } }
        public byte E_Immune { get { return e_immune; } }
        public byte E_Half { get { return e_half; } }
        public byte Stat_Res { get { return stat_res; } }
        public byte Death_Res { get { return death_res; } }
        public ushort EXP { get { return exp; } }
        public ushort Gold { get { return gold; } }
        public byte Drop_Item { get { return drop_item; } }
        public byte Drop_Chance { get { return drop_chance; } }

        public StatList(string[] monster, IDictionary<string, byte> element2num, IDictionary<string, byte> item2num) {
            name = monster[1];
            byte key = 0;
            ushort key2 = 0;
            int key3 = 0;
            if (element2num.TryGetValue(monster[2].ToLower(), out key)) {
                element = key;
            } else {
                Constants.WriteDebug(monster[2] + " not found as element for " + monster[1] + " (ID " + monster[0] + ")");
            }
            if (Int32.TryParse(monster[3], out key3)) {
                hp = key3;
            } else {
                Constants.WriteDebug($"{monster[3]} not found as HP for {monster[1]} (ID {monster[0]})");
            }
            if (UInt16.TryParse(monster[4], out key2)) {
                at = key2;
            } else {
                Constants.WriteDebug($"{monster[4]} not found as AT for {monster[1]} (ID {monster[0]})");
            }
            if (UInt16.TryParse(monster[5], out key2)) {
                mat = key2;
            } else {
                Constants.WriteDebug($"{monster[5]} not found as MAT for {monster[1]} (ID {monster[0]})");
            }
            if (UInt16.TryParse(monster[6], out key2)) {
                df = key2;
            } else {
                Constants.WriteDebug($"{monster[6]} not found as DF for {monster[1]} (ID {monster[0]})");
            }
            if (UInt16.TryParse(monster[7], out key2)) {
                mdf = key2;
            } else {
                Constants.WriteDebug($"{monster[7]} not found as MDF for {monster[1]} (ID {monster[0]})");
            }
            if (UInt16.TryParse(monster[8], out key2)) {
                spd = key2;
            } else {
                Constants.WriteDebug($"{monster[8]} not found as SPD for {monster[1]} (ID {monster[0]})");
            }
            if (Byte.TryParse(monster[9], out key)) {
                a_av = key;
            } else {
                Constants.WriteDebug($"{monster[9]} not found as A_AV for {monster[1]} (ID {monster[0]})");
            }
            if (Byte.TryParse(monster[10], out key)) {
                m_av = key;
            } else {
                Constants.WriteDebug($"{monster[10]} not found as M_AV for {monster[1]} (ID {monster[0]})");
            }
            if (Byte.TryParse(monster[11], out key)) {
                p_immune = key;
            } else {
                Constants.WriteDebug($"{monster[11]} not found as P_Immune for {monster[1]} (ID {monster[0]})");
            }
            if (Byte.TryParse(monster[12], out key)) {
                m_immune = key;
            } else {
                Constants.WriteDebug($"{monster[12]} not found as M_Immune for {monster[1]} (ID {monster[0]})");
            }
            if (Byte.TryParse(monster[13], out key)) {
                p_half = key;
            } else {
                Constants.WriteDebug($"{monster[13]} not found as P_Half for {monster[1]} (ID {monster[0]})");
            }
            if (Byte.TryParse(monster[14], out key)) {
                a_av = key;
            } else {
                Constants.WriteDebug($"{monster[14]} not found as M_Half for {monster[1]} (ID {monster[0]})");
            }
            foreach (string substring in monster[15].Replace(" ", "").Split(',')) {
                if (element2num.TryGetValue(substring.ToLower(), out key)) {
                    e_immune |= key;
                } else {
                    Constants.WriteDebug(substring + " not found as e_immune for " + monster[1] + " (ID " + monster[0] + ")");
                }
            }
            foreach (string substring in monster[16].Replace(" ", "").Split(',')) {
                if (element2num.TryGetValue(substring.ToLower(), out key)) {
                    e_half |= key;
                } else {
                    Constants.WriteDebug(substring + " not found as e_half for " + monster[1] + " (ID " + monster[0] + ")");
                }
            }
            if (Byte.TryParse(monster[17], out key)) {
                stat_res = key;
            } else {
                Constants.WriteDebug($"{monster[17]} not found as Status Resist for {monster[1]} (ID {monster[0]})");
            }
            if (Byte.TryParse(monster[18], out key)) {
                death_res = key;
            } else {
                Constants.WriteDebug($"{monster[18]} not found as Death Resist for {monster[1]} (ID {monster[0]})");
            }
            if (UInt16.TryParse(monster[19], out key2)) {
                exp = key2;
            } else {
                Constants.WriteDebug($"{monster[19]} not found as EXP for {monster[1]} (ID {monster[0]})");
            }
            if (UInt16.TryParse(monster[20], out key2)) {
                gold = key2;
            } else {
                Constants.WriteDebug($"{monster[20]} not found as Gold for {monster[1]} (ID {monster[0]})");
            }
            if (item2num.TryGetValue(monster[21].ToLower(), out key)) {
                drop_item = key;
            } else {
                Constants.WriteDebug(monster[21] + " not found in Item List as drop for " + monster[1] + " (ID " + monster[0] + ")");
            }
            if (Byte.TryParse(monster[22], out key)) {
                drop_chance = key;
            } else {
                Constants.WriteDebug($"{monster[22]} not found as Drop Chance for {monster[1]} (ID {monster[0]})");
            }
        }
    }

    public class DragoonStats {
        byte dat = 0;
        byte dmat = 0;
        byte ddf = 0;
        byte dmdf = 0;
        ushort mp = 0;

        public byte DAT { get { return dat; } }
        public byte DMAT { get { return dmat; } }
        public byte DDF { get { return ddf; } }
        public byte DMDF { get { return dmdf; } }
        public ushort MP { get { return mp; } }

        public DragoonStats(string ndat, string nddf, string ndmat, string ndmdf, string nmp) {
            byte key = 0;
            if (Byte.TryParse(ndat, NumberStyles.AllowLeadingSign, null as IFormatProvider, out key)) {
                dat = key;
            } else if (ndat != "") {
                Constants.WriteDebug(ndat + " not found as D-AT");
            }
            if (Byte.TryParse(ndmat, NumberStyles.AllowLeadingSign, null as IFormatProvider, out key)) {
                dmat = key;
            } else if (ndmat != "") {
                Constants.WriteDebug(ndmat + " not found as D-MAT");
            }
            if (Byte.TryParse(nddf, NumberStyles.AllowLeadingSign, null as IFormatProvider, out key)) {
                ddf = key;
            } else if (nddf != "") {
                Constants.WriteDebug(nddf + " not found as D-DF");
            }
            if (Byte.TryParse(ndmdf, NumberStyles.AllowLeadingSign, null as IFormatProvider, out key)) {
                dmdf = key;
            } else if (ndmdf != "") {
                Constants.WriteDebug(ndmdf + " not found as D-MDF");
            }
            if (UInt16.TryParse(nmp, NumberStyles.AllowLeadingSign, null as IFormatProvider, out ushort shortkey)) {
                mp = shortkey;
            } else if (nmp != "") {
                Constants.WriteDebug(ndmdf + " not found as MP");
            }
        }
    }

    public class DragoonSpells {
        bool percentage = false;
        double damage = 0;
        byte accuracy = 100;
        byte mp = 10;
        byte element = 128;
        string description = "<END>";
        string encoded_description = "FF A0 FF A0";
        long description_pointer = 0x0;
        IDictionary<string, bool> perc = new Dictionary<string, bool> {
                { "yes", true},
                { "no", false },
                { "true", true },
                { "false", false },
                { "1", true },
                { "0", false },
                { "", false}
            };

        public bool Percentage { get { return percentage; } }
        public double Damage { get { return damage; } }
        public byte Accuracy { get { return accuracy; } }
        public byte MP { get { return mp; } }
        public byte Element { get { return element; } }
        public string Description { get { return description; } }
        public string Encoded_Description { get { return encoded_description; } }
        public long Description_Pointer { get; set; }

        public DragoonSpells(string[] values, int spell, IDictionary<string, byte> Element2Num) {
            bool key = new bool();
            double dkey = new double();
            byte bkey = new byte();
            string name = values[0];
            if (perc.TryGetValue(values[1].ToLower(), out key)) {
                percentage = key;
            } else {
                Constants.WriteDebug("Incorrect percentage swith " + values[1] + " for spell " + values[0]);
            }
            if (Double.TryParse(values[2], out dkey)) {
                damage = dkey;
            } else {
                Constants.WriteDebug($"{values[2]} couldn't be parsed as damage for spell {values[0]}");
            }
            if (Byte.TryParse(values[3], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                accuracy = bkey;
            } else {
                Constants.WriteDebug($"{values[3]} couldn't be parsed as accuracy for spell {values[0]}");
            }
            if (Byte.TryParse(values[4], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                mp = bkey;
            } else {
                Constants.WriteDebug($"{values[4]} couldn't be parsed as MP for spell {values[0]}");
            }
            if (Element2Num.TryGetValue(values[5].ToLower(), out bkey)) {
                element = bkey;
            } else {
                Constants.WriteDebug($"{values[5]} not found as element for spell {values[0]}");
            }
            description = values[6];
            if (description != "") {
                encoded_description = LoDDict.StringEncode(description) + " FF A0";
            }
        }
    }

    public class DragoonAdditionStats {
        ushort hit1 = 0;
        ushort hit2 = 0;
        ushort hit3 = 0;
        ushort hit4 = 0;
        ushort hit5 = 0;

        public ushort HIT1 { get { return hit1; } }
        public ushort HIT2 { get { return hit2; } }
        public ushort HIT3 { get { return hit3; } }
        public ushort HIT4 { get { return hit4; } }
        public ushort HIT5 { get { return hit5; } }

        public DragoonAdditionStats(string nhit1, string nhit2, string nhit3, string nhit4, string nhit5) {
            ushort key = 0;
            if (ushort.TryParse(nhit1, NumberStyles.AllowLeadingSign, null as IFormatProvider, out key)) {
                hit1 = key;
            } else if (nhit1 != "") {
                Constants.WriteDebug(nhit1 + " not found as HIT1");
            }
            if (ushort.TryParse(nhit2, NumberStyles.AllowLeadingSign, null as IFormatProvider, out key)) {
                hit2 = key;
            } else if (nhit2 != "") {
                Constants.WriteDebug(nhit2 + " not found as HIT2");
            }
            if (ushort.TryParse(nhit3, NumberStyles.AllowLeadingSign, null as IFormatProvider, out key)) {
                hit3 = key;
            } else if (nhit3 != "") {
                Constants.WriteDebug(nhit3 + " not found as HIT3");
            }
            if (ushort.TryParse(nhit4, NumberStyles.AllowLeadingSign, null as IFormatProvider, out key)) {
                hit4 = key;
            } else if (nhit4 != "") {
                Constants.WriteDebug(nhit4 + " not found as HIT4");
            }
            if (ushort.TryParse(nhit5, NumberStyles.AllowLeadingSign, null as IFormatProvider, out key)) {
                hit5 = key;
            } else if (nhit5 != "") {
                Constants.WriteDebug(nhit5 + " not found as HIT5");
            }
        }
    }

    public class CharacterStats {
        ushort max_hp = 1;
        byte spd = 1;
        byte at = 1;
        byte mat = 1;
        byte df = 1;
        byte mdf = 1;
        byte key = 0;
        ushort key2 = 0;

        public ushort Max_HP { get { return max_hp; } }
        public byte SPD { get { return spd; } }
        public byte AT { get { return at; } }
        public byte MAT { get { return mat; } }
        public byte DF { get { return df; } }
        public byte MDF { get { return mdf; } }

        public CharacterStats(string nmax_hp, string nspd, string nat, string nmat, string ndf, string nmdf) {
            if (UInt16.TryParse(nmax_hp, out key2)) {
                max_hp = key2;
            }
            if (Byte.TryParse(nspd, out key)) {
                spd = key;
            }
            if (Byte.TryParse(nat, out key)) {
                at = key;
            }
            if (Byte.TryParse(nmat, out key)) {
                mat = key;
            }
            if (Byte.TryParse(ndf, out key)) {
                df = key;
            }
            if (Byte.TryParse(nmdf, out key)) {
                mdf = key;
            }
        }
    }

    public class AdditionData {
        short uu1 = 0;
        short next_hit = 0;
        short blue_time = 0;
        short gray_time = 0;
        short dmg = 0;
        short sp = 0;
        short id = 0;
        byte final_hit = 0;
        byte uu2 = 0;
        byte uu3 = 0;
        byte uu4 = 0;
        byte uu5 = 0;
        byte uu6 = 0;
        byte uu7 = 0;
        byte uu8 = 0;
        byte uu9 = 0;
        byte uu10 = 0;
        short vertical_distance = 0;
        byte uu11 = 0;
        byte uu12 = 0;
        byte uu13 = 0;
        byte uu14 = 0;
        byte start_time = 0;
        byte uu15 = 0;
        short add_dmg_multi = 0;
        short add_sp_multi = 0;

        public short UU1 { get { return uu1; } }
        public short Next_Hit { get { return next_hit; } }
        public short Blue_Time { get { return blue_time; } }
        public short Gray_Time { get { return gray_time; } }
        public short DMG { get { return dmg; } }
        public short SP { get { return sp; } }
        public short ID { get { return id; } }
        public byte Final_Hit { get { return final_hit; } }
        public byte UU2 { get { return uu2; } }
        public byte UU3 { get { return uu3; } }
        public byte UU4 { get { return uu4; } }
        public byte UU5 { get { return uu5; } }
        public byte UU6 { get { return uu6; } }
        public byte UU7 { get { return uu7; } }
        public byte UU8 { get { return uu8; } }
        public byte UU9 { get { return uu9; } }
        public byte UU10 { get { return uu10; } }
        public short Vertical_Distance { get { return vertical_distance; } }
        public byte UU11 { get { return uu11; } }
        public byte UU12 { get { return uu12; } }
        public byte UU13 { get { return uu13; } }
        public byte UU14 { get { return uu14; } }
        public byte Start_Time { get { return start_time; } }
        public byte UU15 { get { return uu15; } }
        public short ADD_DMG_Multi { get { return add_dmg_multi; } }
        public short ADD_SP_Multi { get { return add_sp_multi; } }

        public AdditionData(string[] values) {
            short skey = 0;
            byte bkey = 0;
            if (Int16.TryParse(values[0], NumberStyles.AllowLeadingSign, null as IFormatProvider, out skey)) {
                uu1 = skey;
            } else {
                Constants.WriteDebug(values[0] + " not found as UU1");
            }
            if (Int16.TryParse(values[1], NumberStyles.AllowLeadingSign, null as IFormatProvider, out skey)) {
                next_hit = skey;
            } else {
                Constants.WriteDebug(values[1] + " not found as Next Hit");
            }
            if (Int16.TryParse(values[2], NumberStyles.AllowLeadingSign, null as IFormatProvider, out skey)) {
                blue_time = skey;
            } else {
                Constants.WriteDebug(values[2] + " not found as Blue Time");
            }
            if (Int16.TryParse(values[3], NumberStyles.AllowLeadingSign, null as IFormatProvider, out skey)) {
                gray_time = skey;
            } else {
                Constants.WriteDebug(values[3] + " not found as Gray Time");
            }
            if (Int16.TryParse(values[4], NumberStyles.AllowLeadingSign, null as IFormatProvider, out skey)) {
                dmg = skey;
            } else {
                Constants.WriteDebug(values[4] + " not found as DMG");
            }
            if (Int16.TryParse(values[5], NumberStyles.AllowLeadingSign, null as IFormatProvider, out skey)) {
                sp = skey;
            } else {
                Constants.WriteDebug(values[5] + " not found as DMG");
            }
            if (Int16.TryParse(values[6], NumberStyles.AllowLeadingSign, null as IFormatProvider, out skey)) {
                id = skey;
            } else {
                Constants.WriteDebug(values[6] + " not found as ID");
            }
            if (Byte.TryParse(values[7], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                final_hit = bkey;
            } else {
                Constants.WriteDebug(values[7] + " not found as Final Hit");
            }
            if (Byte.TryParse(values[8], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                uu2 = bkey;
            } else {
                Constants.WriteDebug(values[8] + " not found as UU2");
            }
            if (Byte.TryParse(values[9], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                uu3 = bkey;
            } else {
                Constants.WriteDebug(values[9] + " not found as UU3");
            }
            if (Byte.TryParse(values[10], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                uu4 = bkey;
            } else {
                Constants.WriteDebug(values[10] + " not found as UU4");
            }
            if (Byte.TryParse(values[11], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                uu5 = bkey;
            } else {
                Constants.WriteDebug(values[11] + " not found as UU5");
            }
            if (Byte.TryParse(values[12], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                uu6 = bkey;
            } else {
                Constants.WriteDebug(values[12] + " not found as UU6");
            }
            if (Byte.TryParse(values[13], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                uu7 = bkey;
            } else {
                Constants.WriteDebug(values[13] + " not found as UU7");
            }
            if (Byte.TryParse(values[14], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                uu8 = bkey;
            } else {
                Constants.WriteDebug(values[14] + " not found as UU8");
            }
            if (Byte.TryParse(values[15], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                uu9 = bkey;
            } else {
                Constants.WriteDebug(values[15] + " not found as UU9");
            }
            if (Byte.TryParse(values[16], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                uu10 = bkey;
            } else {
                Constants.WriteDebug(values[16] + " not found as UU10");
            }
            if (Int16.TryParse(values[17], NumberStyles.AllowLeadingSign, null as IFormatProvider, out skey)) {
                vertical_distance = skey;
            } else {
                Constants.WriteDebug(values[17] + " not found as Vertical Distance");
            }
            if (Byte.TryParse(values[18], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                uu11 = bkey;
            } else {
                Constants.WriteDebug(values[18] + " not found as UU11");
            }
            if (Byte.TryParse(values[19], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                uu12 = bkey;
            } else {
                Constants.WriteDebug(values[19] + " not found as UU12");
            }
            if (Byte.TryParse(values[20], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                uu13 = bkey;
            } else {
                Constants.WriteDebug(values[20] + " not found as UU13");
            }
            if (Byte.TryParse(values[21], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                uu14 = bkey;
            } else {
                Constants.WriteDebug(values[21] + " not found as UU14");
            }
            if (Byte.TryParse(values[22], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                start_time = bkey;
            } else {
                Constants.WriteDebug(values[22] + " not found as Start Time");
            }
            if (Byte.TryParse(values[23], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                uu15 = bkey;
            } else {
                Constants.WriteDebug(values[23] + " not found as UU15");
            }
            if (Int16.TryParse(values[24], NumberStyles.AllowLeadingSign, null as IFormatProvider, out skey)) {
                add_dmg_multi = skey;
            } else {
                Constants.WriteDebug(values[24] + " not found as ADD_DMG_Multi");
            }
            if (Int16.TryParse(values[25], NumberStyles.AllowLeadingSign, null as IFormatProvider, out skey)) {
                add_sp_multi = skey;
            } else {
                Constants.WriteDebug(values[25] + " not found as ADD_SP_Multi");
            }
        }
    }
}