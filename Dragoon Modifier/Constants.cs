using CSScriptLibrary;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Linq;

namespace Dragoon_Modifier {
    public class Constants {
        public static string VERSION = "3.5.4";
        public static bool RUN = true;
        public static bool DEBUG_MODE = false;
        public static bool BATTLE_UI = false;
        public static bool EATB_BEEP = false;
        public static TextBox CONSOLE;
        public static TextBlock GLOG;
        public static TextBlock PLOG;
        public static long OFFSET = 0x0;
        public static Region REGION = Region.NTA;
        public static byte EMULATOR_ID = 255;
        public static int SAVE_SLOT = 0;
        public static string EMULATOR_NAME = "None";
        public static Dictionary<string, int[]> ADDRESSES = new Dictionary<string, int[]>();
        public static Dictionary<string, byte> PRESET_SCRIPTS = new Dictionary<string, byte>();
        public static RegistryKey KEY;
        public static RegistryKey SUBKEY;
        public static string[] READER_CHARACTER_LABEL = { "Name","Action","Menu","LV","DLV","HP","Max_HP","MP","Max_MP","SP","Max_SP","Element","Display_Element","AT","OG_AT","MAT","OG_MAT","DF","OG_DF","MDF","OG_MDF","SPD","OG_SPD","Turn","A_Hit","M_Hit","A_AV","M_AV","P_Immune","M_Immune","P_Half","M_Half","E_Immune","E_Half","On_Hit_Status","On_Hit_Status_Chance","Stat_Res","Death_Res","SP_P_Hit","SP_M_Hit","MP_P_Hit","MP_M_Hit","HP_Regen","MP_Regen","SP_Regen","SP_Multi","Revive","Unique_Index","Image","DAT","DMAT","DDF","DMDF","Special_Effect","Guard","Dragoon","Spell_Cast","PWR_AT","PWR_AT_TRN","PWR_MAT","PWR_MAT_TRN","PWR_DF","PWR_DF_TRN","PWR_MDF","PWR_MDF_TRN","ADD_SP_Multi","ADD_DMG_Multi","Weapon","Helmet","Armor","Shoes","Accessory","POS_FB","POS_UD","POS_RL","A_HIT_INC","A_HIT_INC_TRN","M_HIT_INC","M_HIT_INC_TRN","PHYSICAL_IMMUNITY","PHYSICAL_IMMUNITY_TRN","ELEMENTAL_IMMUNITY","ELEMENTAL_IMMUNITY_TRN","SPEED_UP_TRN","SPEED_DOWN_TRN","SP_ONHIT_PHYSICAL","SP_ONHIT_PHYSICAL_TRN","MP_ONHIT_PHYSICAL","MP_ONHIT_PHYSICAL_TRN","SP_ONHIT_MAGIC","SP_ONHIT_MAGIC_TRN","MP_ONHIT_MAGIC","MP_ONHIT_MAGIC_TRN","Color_Map","Burn Stack","Damage Tracker1","Damage Tracker2","Damage Tracker3","EATBC1","EATBC2","EATBC3","QTB" };
        public static string[] READER_MONSTER_LABEL = { "Name","Action","HP","Max_HP","Element","Display_Element","AT","OG_AT","MAT","OG_MAT","DF","OG_DF","MDF","OG_MDF","SPD","OG_SPD","Turn","A_AV","M_AV","P_Immune","M_Immune","P_Half","M_Half","E_Immune","E_Half","Stat_Res","Death_Res","Unique_Index","EXP","Gold","Drop_Chance","Drop_Item","Special_Effect","Attack_Move","EATBM1","EATBM2","EATBM3","EATBM4","EATBM5"  };
        public static int[] DISC_OFFSET = { 0xD80, 0x0, 0x1458, 0x1B0 };

        public static void Init() {
            using (StreamReader reader = File.OpenText("Scripts\\Addresses.csv")) {
                string line;
                while ((line = reader.ReadLine()) != null) {
                    string[] values = line.Split(',');
                    ADDRESSES.Add(values[0], new int[] { Convert.ToInt32(values[1], 16), Convert.ToInt32(values[2], 16), Convert.ToInt32(values[3], 16), Convert.ToInt32(values[4], 16), Convert.ToInt32(values[5], 16), Convert.ToInt32(values[6], 16), Convert.ToInt32(values[7], 16) });
                }
            }

            foreach (string file in Directory.GetFiles("Scripts\\Addresses", "*.csv", SearchOption.AllDirectories)) {
                using (StreamReader reader = File.OpenText(file)) {
                    string line;
                    while ((line = reader.ReadLine()) != null) {
                        string[] values = line.Split(',');
                        if (ADDRESSES.ContainsKey(values[0])) {
                            Constants.WriteDebug("Same key warning: " + values[0]);
                        } else {
                            ADDRESSES.Add(values[0], new int[] { Convert.ToInt32(values[1], 16), Convert.ToInt32(values[2], 16), Convert.ToInt32(values[3], 16), Convert.ToInt32(values[4], 16), Convert.ToInt32(values[5], 16), Convert.ToInt32(values[6], 16), Convert.ToInt32(values[7], 16) });
                        }
                    }
                }
            }

            ADDRESSES.Add("HASCHEL_FIX" + 1, new int[] { ADDRESSES["HASCHEL_FIX"][0] + DISC_OFFSET[0], ADDRESSES["HASCHEL_FIX"][1] + DISC_OFFSET[0], ADDRESSES["HASCHEL_FIX"][2] + DISC_OFFSET[0], ADDRESSES["HASCHEL_FIX"][3] + DISC_OFFSET[0], ADDRESSES["HASCHEL_FIX"][4] + DISC_OFFSET[0], ADDRESSES["HASCHEL_FIX"][5] + DISC_OFFSET[0], ADDRESSES["HASCHEL_FIX"][6] + DISC_OFFSET[0] });
            ADDRESSES.Add("HASCHEL_FIX" + 2, new int[] { ADDRESSES["HASCHEL_FIX"][0] + DISC_OFFSET[1], ADDRESSES["HASCHEL_FIX"][1] + DISC_OFFSET[1], ADDRESSES["HASCHEL_FIX"][2] + DISC_OFFSET[1], ADDRESSES["HASCHEL_FIX"][3] + DISC_OFFSET[1], ADDRESSES["HASCHEL_FIX"][4] + DISC_OFFSET[1], ADDRESSES["HASCHEL_FIX"][5] + DISC_OFFSET[1], ADDRESSES["HASCHEL_FIX"][6] + DISC_OFFSET[1] });
            ADDRESSES.Add("HASCHEL_FIX" + 3, new int[] { ADDRESSES["HASCHEL_FIX"][0] + DISC_OFFSET[2], ADDRESSES["HASCHEL_FIX"][1] + DISC_OFFSET[2], ADDRESSES["HASCHEL_FIX"][2] + DISC_OFFSET[2], ADDRESSES["HASCHEL_FIX"][3] + DISC_OFFSET[2], ADDRESSES["HASCHEL_FIX"][4] + DISC_OFFSET[2], ADDRESSES["HASCHEL_FIX"][5] + DISC_OFFSET[2], ADDRESSES["HASCHEL_FIX"][6] + DISC_OFFSET[2] });
            ADDRESSES.Add("HASCHEL_FIX" + 4, new int[] { ADDRESSES["HASCHEL_FIX"][0] + DISC_OFFSET[3], ADDRESSES["HASCHEL_FIX"][1] + DISC_OFFSET[3], ADDRESSES["HASCHEL_FIX"][2] + DISC_OFFSET[3], ADDRESSES["HASCHEL_FIX"][3] + DISC_OFFSET[3], ADDRESSES["HASCHEL_FIX"][4] + DISC_OFFSET[3], ADDRESSES["HASCHEL_FIX"][5] + DISC_OFFSET[3], ADDRESSES["HASCHEL_FIX"][6] + DISC_OFFSET[3] });

            KEY = Registry.CurrentUser.OpenSubKey("Legend of Dragoon", true);
            if (KEY == null) 
                KEY = Registry.CurrentUser.CreateSubKey("Legend of Dragoon", true);
        }

        public static bool LoadPreset(string name) {
            try {
                bool config = false;
                using (StreamReader reader = File.OpenText("Presets\\" + name + ".csv")) {
                    string line;
                    while ((line = reader.ReadLine()) != null) {
                        string[] values = line.Split(',');
                        if (values[0].Equals("Config")) {
                            config = true;
                            short value = short.Parse(values[1]);
                            if ((value & (1 << 0)) != 0)
                                Globals.MONSTER_STAT_CHANGE = true;
                            if ((value & (1 << 1)) != 0)
                                Globals.MONSTER_DROP_CHANGE = true;
                            if ((value & (1 << 2)) != 0)
                                Globals.MONSTER_EXPGOLD_CHANGE = true;
                            if ((value & (1 << 3)) != 0)
                                Globals.CHARACTER_STAT_CHANGE = true;
                            if ((value & (1 << 4)) != 0)
                                Globals.ADDITION_CHANGE = true;
                            if ((value & (1 << 5)) != 0)
                                Globals.DRAGOON_STAT_CHANGE = true;
                            if ((value & (1 << 6)) != 0)
                                Globals.DRAGOON_SPELL_CHANGE = true;
                            if ((value & (1 << 7)) != 0)
                                Globals.DRAGOON_ADDITION_CHANGE = true;
                            if ((value & (1 << 8)) != 0)
                                Globals.DRAGOON_DESC_CHANGE = true;
                            if ((value & (1 << 9)) != 0)
                                Globals.ITEM_STAT_CHANGE = true;
                            if ((value & (1 << 10)) != 0)
                                Globals.ITEM_ICON_CHANGE = true;
                            if ((value & (1 << 11)) != 0)
                                Globals.ITEM_NAMEDESC_CHANGE = true;
                            if ((value & (1 << 12)) != 0)
                                Globals.SHOP_CHANGE = true;
                        } else {
                            if (config) {
                                Globals.MOD = values[0];
                            } else {
                                PRESET_SCRIPTS.Add(values[0], Convert.ToByte(values[1]));
                            }
                        }
                    }
                }
                return true;
            } catch (Exception e) {
                WriteOutput("Error loading preset.");
                WriteOutput(e.ToString());
                return false;
            }
        }

        public static void SetSubKey(int slot) {
            SAVE_SLOT = slot;
            SUBKEY = Registry.CurrentUser.OpenSubKey("Legend of Dragoon\\" + SAVE_SLOT, true);
            if (SUBKEY == null)
                SUBKEY = Registry.CurrentUser.CreateSubKey("Legend of Dragoon\\" + SAVE_SLOT, true);
        }

        public static void WriteOutput(object text) {
            Application.Current.Dispatcher.Invoke(() => {
                if (CONSOLE.LineCount == 1 || CONSOLE.LineCount > 2000) {
                    CONSOLE.Text = "-----LOGCUT-----\r\n" + text.ToString();
                } else {
                    CONSOLE.AppendText("\r\n" + text.ToString());
                }
                if (!CONSOLE.IsFocused) {
                    CONSOLE.ScrollToEnd();
                }
            });
        }

        public static void WriteDebug(object text) {
            Application.Current.Dispatcher.Invoke(() => {
                if (DEBUG_MODE) {
                    if (CONSOLE.LineCount == 1 || CONSOLE.LineCount > 2000) {
                        CONSOLE.Text = "-----LOGCUT-----\r\n[DEBUG] " + text.ToString();
                    } else {
                        CONSOLE.AppendText("\r\n[DEBUG] " + text.ToString());
                    }
                    if (!CONSOLE.IsFocused) {
                        CONSOLE.ScrollToEnd();
                    }
                }
            });
        }

        public static void WriteDebugProgram(object text) {
            Application.Current.Dispatcher.Invoke(() => {
                if (CONSOLE.LineCount == 1 || CONSOLE.LineCount > 2000) {
                    CONSOLE.Text = "-----LOGCUT-----\r\n[DEBUG] " + text.ToString();
                } else {
                    CONSOLE.AppendText("\r\n[DEBUG] " + text.ToString());
                }
                if (!CONSOLE.IsFocused) {
                    CONSOLE.ScrollToEnd();
                }
                PLOG.Text = "[DEBUG] " + text.ToString();
            });
        }

        public static void WriteError(object text) {
            Application.Current.Dispatcher.Invoke(() => {
                if (CONSOLE.LineCount == 1 || CONSOLE.LineCount > 2000) {
                    CONSOLE.Text = "-----LOGCUT-----\r\n[ERROR] " + text.ToString();
                } else {
                    CONSOLE.AppendText("\r\n[ERROR] " + text.ToString());
                }
                if (!CONSOLE.IsFocused) {
                    CONSOLE.ScrollToEnd();
                }
            });
        }

        public static void WriteGLog(object text) {
            Application.Current.Dispatcher.Invoke(() => {
                GLOG.Text = text.ToString();
            });
        }

        public static void WritePLog(object text) {
            Application.Current.Dispatcher.Invoke(() => {
                PLOG.Text = text.ToString();
            });
        }

        public static void WriteGLogOutput(object text) {
            Application.Current.Dispatcher.Invoke(() => {
                WriteOutput(text);
                GLOG.Text = text.ToString();
            });
        }

        public static void WritePLogOutput(object text) {
            Application.Current.Dispatcher.Invoke(() => {
                WriteOutput(text);
                PLOG.Text = text.ToString();
            });
        }

        public static int GetAddress(string text) {
            if (text[0] != '$') {
                try {
                    return ADDRESSES[text][(int) REGION];
                } catch (Exception e) {
                    Constants.WriteOutput("Missing key: " + text);
                    return 0;
                }
            } else {
                Dictionary<string, Dictionary<string, int>> master = new Dictionary<string, Dictionary<string, int>>() {
                    {"CHAR_TABLE",  new Dictionary<string, int>() {
                            {"DART", 0x0 },
                            {"LAVITZ", 0x2C },
                            {"SHANA", 0x58 },
                            {"ROSE", 0x84 },
                            {"HASCHEL", 0xB0 },
                            {"ALBERT", 0xDC },
                            {"MERU", 0x108 },
                            {"KONGOL", 0x134 },
                            {"MIRANDA", 0x160 },
                            {"HP", 0x8 },
                            {"MP", 0xA },
                            {"SP",  0xC},
                            {"TOTAL_SP", 0xE },
                            {"STATUS", 0x10 },
                            {"LV", 0x12 },
                            {"DLV", 0x13 },
                            {"WEAPON", 0x14 },
                            {"HELM", 0x15 },
                            {"ARMOR", 0x16 },
                            {"BOOTS", 0x17 },
                            {"RING", 0x18 },
                            {"CHOSEN_ADDITION", 0x19 },
                            {"ADDITION_LEVEL", 0x1A },
                            {"ADDITION_COUNT", 0x22 },


                        }
                    }
                };
                text = text.Substring(1);
                var sub = text.Split('-').ToArray();
                try {
                    int address = ADDRESSES[sub[0]][(int) REGION];
                    var array = master[sub[0]];
                    foreach (var category in sub.Skip(1).ToArray()) {
                        address += array[category];
                    }
                    return address;
                } catch (Exception e) {
                    Constants.WriteOutput("Missing key: " + text);
                    return 0;
                }
            }
            
        }

        public static void ProgramInfo() {
            WriteOutput("Region:    " + Constants.REGION);
            WriteOutput("Emulator:  " + Constants.EMULATOR_NAME);
            WriteOutput("Save Slot: " + (Constants.SAVE_SLOT + 1));
            WriteDebug("Offset:    " + Convert.ToString(Constants.OFFSET, 16).ToUpper());
        }

        public static string GetCharName(byte partySlot) {
            if (partySlot == 0) {
                return "Dart";
            } else if (partySlot == 1) {
                return "Lavitz";
            } else if (partySlot == 2) {
                return "Shana";
            } else if (partySlot == 3) {
                return "Rose";
            } else if (partySlot == 4) {
                return "Haschel";
            } else if (partySlot == 5) {
                return "Albert";
            } else if (partySlot == 6) {
                return "Meru";
            } else if (partySlot == 7) {
                return "Kongol";
            } else if (partySlot == 8) {
                return "Miranda";
            }
            return "Null";
        }

        public static long GetTime() {
            return (long) (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static void UltimateBossRewards() { //temp?
            for (int i = 0; i < 5; i++) {
                Emulator.WriteShort("MONSTER_REWARDS", 0, i * 0x1A8);
                Emulator.WriteShort("MONSTER_REWARDS", 0, 0x2 + i * 0x1A8);
                Emulator.WriteByte("MONSTER_REWARDS", 0, 0x4 + i * 0x1A8);
                Emulator.WriteByte("MONSTER_REWARDS", 0, 0x5 + i * 0x1A8);
            }

            if (Globals.ENCOUNTER_ID == 487) { //Commander II
                Emulator.WriteByte("MONSTER_REWARDS", 100, 0x4 + 0x1A8);
                Emulator.WriteByte("MONSTER_REWARDS", 158, 0x5 + 0x1A8); //Sabre
            }

            if (Globals.ENCOUNTER_ID == 449 || Globals.ENCOUNTER_ID == 402 || Globals.ENCOUNTER_ID == 403) {
                if (Globals.ENCOUNTER_ID == 402 || Globals.ENCOUNTER_ID == 403) {
                    Emulator.WriteShort("MONSTER_REWARDS", 3000, 0x1A8 + 0x2);
                } else {
                    Emulator.WriteShort("MONSTER_REWARDS", 3000, 0x2);
                }
            } else if (Globals.ENCOUNTER_ID == 417 || Globals.ENCOUNTER_ID == 418 || Globals.ENCOUNTER_ID == 448) {
                if (Globals.ENCOUNTER_ID == 418) {
                    Emulator.WriteShort("MONSTER_REWARDS", 3000, 0x1A8 + 0x2);
                } else {
                    Emulator.WriteShort("MONSTER_REWARDS", 3000, 0x2);
                }
            } else if (Globals.ENCOUNTER_ID == 416 || Globals.ENCOUNTER_ID == 422 || Globals.ENCOUNTER_ID == 423) {
                Emulator.WriteShort("MONSTER_REWARDS", 9000, 0x2);
            } else if (Globals.ENCOUNTER_ID == 432 || Globals.ENCOUNTER_ID == 430 || Globals.ENCOUNTER_ID == 433) {
                Emulator.WriteShort("MONSTER_REWARDS", 12000, 0x2);
            } else if (Globals.ENCOUNTER_ID == 431 || Globals.ENCOUNTER_ID == 408) {
                Emulator.WriteShort("MONSTER_REWARDS", 15000, 0x1A8 + 0x2);
            } else if (Globals.ENCOUNTER_ID == 447) {
                Emulator.WriteShort("MONSTER_REWARDS", 18000, 0x2);
            } else if (Globals.ENCOUNTER_ID == 389 || Globals.ENCOUNTER_ID == 396) {
                Emulator.WriteShort("MONSTER_REWARDS", 20000, 0x2);
            } else if (Globals.ENCOUNTER_ID == 399) {
                Emulator.WriteShort("MONSTER_REWARDS", 25000, 0x2);
            } else if (Globals.ENCOUNTER_ID == 409) {
                Emulator.WriteShort("MONSTER_REWARDS", 30000, 0x2);
            } else if (Globals.ENCOUNTER_ID == 393 || Globals.ENCOUNTER_ID == 398) {
                Emulator.WriteUShort("MONSTER_REWARDS", 35000, 0x2);
            } else if (Globals.ENCOUNTER_ID == 397 || Globals.ENCOUNTER_ID == 400) {
                Emulator.WriteUShort("MONSTER_REWARDS", 40000, 0x2);
            } else if (Globals.ENCOUNTER_ID == 410) {
                Emulator.WriteShort("MONSTER_REWARDS", 1000, 0x2);
            } else if (Globals.ENCOUNTER_ID == 401) {
                Emulator.WriteUShort("MONSTER_REWARDS", 45000, 0x2);
            } else if (Globals.ENCOUNTER_ID == 390) {
                Emulator.WriteInt("TOTAL_GOLD", 100000);
            } else if (Globals.ENCOUNTER_ID == 411) {
                Emulator.WriteInt("TOTAL_GOLD", 60000);
            } else if (Globals.ENCOUNTER_ID == 394) {
                Emulator.WriteInt("TOTAL_GOLD", 70000);
            } else if (Globals.ENCOUNTER_ID == 392) {
                Emulator.WriteInt("TOTAL_GOLD", 80000);
            } else if (Globals.ENCOUNTER_ID == 420) {
                Emulator.WriteInt("TOTAL_GOLD", 120000);
            } else if (Globals.ENCOUNTER_ID == 442) {
                Emulator.WriteInt("TOTAL_GOLD", 100000);
            }
        }
    }
}
