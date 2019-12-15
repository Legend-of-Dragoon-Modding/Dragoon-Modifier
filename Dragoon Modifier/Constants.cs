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
        public static long OFFSET = 0xA579A0;
        public static Region REGION = Region.USA;
        public static byte EMULATOR = 255;
        public static int SAVE_SLOT = 0;
        public static string EMULATOR_NAME = "None";
        public static Dictionary<string, int[]> ADDRESSES = new Dictionary<string, int[]>();
        public static Dictionary<string, byte> PRESET_SCRIPTS = new Dictionary<string, byte>();
        public static RegistryKey KEY;
        public static RegistryKey SUBKEY;

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
            SUBKEY = Registry.CurrentUser.OpenSubKey("Legend of Dragoon\\" + SAVE_SLOT, true);
            if (KEY == null) 
                KEY = Registry.CurrentUser.CreateSubKey("Legend of Dragoon", true);
            if (SUBKEY == null)
                SUBKEY = Registry.CurrentUser.CreateSubKey("Legend of Dragoon\\" + SAVE_SLOT, true);
        }

        public static bool LoadPreset(string name) {
            try {
                using (StreamReader reader = File.OpenText("Presets\\" + name + ".csv")) {
                    string line;
                    while ((line = reader.ReadLine()) != null) {
                        string[] values = line.Split(',');
                        PRESET_SCRIPTS.Add(values[0], Convert.ToByte(values[1]));
                    }
                }
                return true;
            } catch (Exception e) {
                WriteOutput("Error loading preset.");
                return false;
            }
        }

        public static void SetSubKey(int slot) {
            SAVE_SLOT = slot;
            SUBKEY = Registry.CurrentUser.OpenSubKey("Legend of Dragoon\\" + SAVE_SLOT);
            if (SUBKEY == null)
                SUBKEY = Registry.CurrentUser.CreateSubKey("Legend of Dragoon\\" + SAVE_SLOT, true);
        }

        public static void WriteOutput(string text) {
            Application.Current.Dispatcher.Invoke(() => {
                if (CONSOLE.LineCount == 1 || CONSOLE.LineCount > 2000) {
                    CONSOLE.Text = "-----LOGCUT-----\r\n" + text;
                } else {
                    CONSOLE.AppendText("\r\n" + text);
                }
                if (!CONSOLE.IsFocused) {
                    CONSOLE.ScrollToEnd();
                }
            });
        }

        public static void WriteDebug(string text) {
            Application.Current.Dispatcher.Invoke(() => {
                if (DEBUG_MODE) {
                    if (CONSOLE.LineCount == 1 || CONSOLE.LineCount > 2000) {
                        CONSOLE.Text = "-----LOGCUT-----\r\n[DEBUG] " + text;
                    } else {
                        CONSOLE.AppendText("\r\n[DEBUG] " + text);
                    }
                    if (!CONSOLE.IsFocused) {
                        CONSOLE.ScrollToEnd();
                    }
                }
            });
        }

        public static int GetAddress(string text) {
            return ADDRESSES[text][(int) REGION];
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

    }
}
