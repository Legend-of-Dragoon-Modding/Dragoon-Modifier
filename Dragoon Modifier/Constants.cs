using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Dragoon_Modifier {
    public class Constants {
        public static string VERSION = "3.0";
        public static bool RUN = true;
        public static bool DEBUG_MODE = true;
        public static bool BATTLE_UI = false;
        public static TextBox CONSOLE;
        public static TextBlock GLOG;
        public static TextBlock PLOG;
        public static long OFFSET = 0x0;
        public static Region REGION = Region.USA;
        public static byte EMULATOR = 255;
        public static int SAVE_SLOT = 0;
        public static string EMULATOR_NAME = "None";
        public static Dictionary<string, int[]> ADDRESSES = new Dictionary<string, int[]>();
        public static Dictionary<string, byte> PRESET_SCRIPTS = new Dictionary<string, byte>();
        public static RegistryKey KEY;
        public static RegistryKey SUBKEY;
        public static string[] READER_CHARACTER_LABEL = { "Name","Action","Menu","LV","DLV","HP","Max_HP","MP","Max_MP","SP","Max_SP","Element","Display_Element","AT","OG_AT","MAT","OG_MAT","DF","OG_DF","MDF","OG_MDF","SPD","OG_SPD","Turn","A_HIT","M_HIT","A_AV","M_AV","P_Immune","M_Immune","P_Half","M_Half","E_Immune","E_Half","On_Hit_Status","On_Hit_Status_Chance","Stat_Res","Death_Res","SP_P_Hit","SP_M_Hit","MP_P_Hit","MP_M_Hit","HP_Regen","MP_Regen","SP_Regen","SP_Multi","Revive","Unique_Index","Image","DAT","DMAT","DDF","DMDF","Special_Effect","Guard","Dragoon","Spell_Cast","PWR_AT","PWR_AT_TRN","PWR_MAT","PWR_MAT_TRN","PWR_DF","PWR_DF_TRN","PWR_MDF","PWR_MDF_TRN","ADD_SP_Multi","ADD_DMG_Multi","Weapon","Helmet","Armor","Shoes","Accessory","POS_FB","POS_UD","POS_RL","A_HIT_INC","A_HIT_INC_TRN","M_HIT_INC","M_HIT_INC_TRN","PHYSICAL_IMMUNITY","PHYSICAL_IMMUNITY_TRN","ELEMENTAL_IMMUNITY","ELEMENTAL_IMMUNITY_TRN","SPEED_UP_TRN","SPEED_DOWN_TRN","SP_ONHIT_PHYSICAL","SP_ONHIT_PHYSICAL_TRN","MP_ONHIT_PHYSICAL","MP_ONHIT_PHYSICAL_TRN","SP_ONHIT_MAGIC","SP_ONHIT_MAGIC_TRN","MP_ONHIT_MAGIC","MP_ONHIT_MAGIC_TRN","Color_Map" };
        public static string[] READER_MONSTER_LABEL = { "Name","Action","HP","Max_HP","Element","Display_Element","AT","OG_AT","MAT","OG_MAT","DF","OG_DF","MDF","OG_MDF","SPD","OG_SPD","Turn","A_AV","M_AV","P_Immune","M_Immune","P_Half","M_Half","E_Immune","E_Half","Stat_Res","Death_Res","Unique_Index","EXP","Gold","Drop_Chance","Drop_Item","Special_Effect","Attack_Move"  };
        public static void Init() {
            using (StreamReader reader = File.OpenText("Scripts\\Addresses.csv")) {
                string line;
                while ((line = reader.ReadLine()) != null) {
                    string[] values = line.Split(',');
                    ADDRESSES.Add(values[0], new int[] { Convert.ToInt32(values[1], 16), Convert.ToInt32(values[2], 16), Convert.ToInt32(values[3], 16) });
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
                            ADDRESSES.Add(values[0], new int[] { Convert.ToInt32(values[1], 16), Convert.ToInt32(values[2], 16), Convert.ToInt32(values[3], 16) });
                        }
                    }
                }
            }

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
                            byte value = Convert.ToByte(values[1]);
                            if ((value & (1 << 0)) != 0)
                                Globals.MONSTER_CHANGE = true;
                            if ((value & (1 << 1)) != 0)
                                Globals.DROP_CHANGE = true;
                            if ((value & (1 << 2)) != 0)
                                Globals.ITEM_CHANGE = true;
                            if ((value & (1 << 3)) != 0)
                                Globals.CHARACTER_CHANGE = true;
                            if ((value & (1 << 4)) != 0)
                                Globals.ADDITION_CHANGE = true;
                            if ((value & (1 << 5)) != 0)
                                Globals.DRAGOON_CHANGE = true;
                            if ((value & (1 << 6)) != 0)
                                Globals.DRAGOON_ADDITION_CHANGE = true;
                            if ((value & (1 << 7)) != 0)
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
            try {
                return ADDRESSES[text][(int) REGION];
            } catch (Exception e) {
                Constants.WriteOutput("Missing key: " + text);
                return 0;
            }
        }

        public static void ProgramInfo() {
            WriteOutput("Region:    " + Constants.REGION);
            WriteOutput("Emulator:  " + Constants.EMULATOR_NAME);
            WriteOutput("Save Slot: " + (Constants.SAVE_SLOT + 1));
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
    }
}
