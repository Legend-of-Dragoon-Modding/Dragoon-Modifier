using Dragoon_Modifier.Core;

using Microsoft.Win32;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod {
    public static class Constants {
        public const string Version = "4.0"; // Lower version for testing
        public static bool Run = false;
        public static string EmulatorName = "ePSXe";
        public static long PreviousOffset = 0xA579A0;
        public static bool Debug = true;
        public static RegistryKey KEY;
        public static RegistryKey SUBKEY;
        public static int SaveSlot = 0;
        public static byte EmulatorID = 255;
        public static Region Region = Region.NTA;
        public static int UltimateBossCompleted = 0;
        public static int InventorySize = 32;
        public static long UltimateShopLimited = 0;
        public static int FaustCount = 0;

        private static UI.IUIControl? _uiControl = null;
        public static UI.IUIControl UIControl {
            get {
                if (_uiControl == null) {
                    throw new NotImplementedException(); // This should never ever be called.
                }
                return _uiControl;
            }
        }

        public static void LoadUIControl(UI.IUIControl uiControl) {
            _uiControl = uiControl;
        }

        public static void LoadRegistry() {
            KEY = Registry.CurrentUser.OpenSubKey("Legend of Dragoon", true);
            if (KEY == null)
                KEY = Registry.CurrentUser.CreateSubKey("Legend of Dragoon", true);


            SUBKEY = Registry.CurrentUser.OpenSubKey("Legend of Dragoon\\" + SaveSlot, true);
            if (SUBKEY == null)
                SUBKEY = Registry.CurrentUser.CreateSubKey("Legend of Dragoon\\" + SaveSlot, true);


            if (SUBKEY.GetValue("Ultimate Boss") == null) {
                SUBKEY.SetValue("Ultimate Boss", 0);
            } else {
                UltimateBossCompleted = (int) SUBKEY.GetValue("Ultimate Boss");
            }

            if (SUBKEY.GetValue("Inventory Size") == null) {
                SUBKEY.SetValue("Inventory Size", 0);
            } else {
                InventorySize = (int) SUBKEY.GetValue("Inventory Size");
            }

            if (SUBKEY.GetValue("Ultimate Shop") == null) {
                SUBKEY.SetValue("Ultimate Shop", 0);
            } else {
                UltimateShopLimited = Convert.ToInt64(SUBKEY.GetValue("Ultimate Shop"));
            }

            if (SUBKEY.GetValue("Faust") == null) {
                SUBKEY.SetValue("Faust", 0);
            } else {
                FaustCount = (int) SUBKEY.GetValue("Faust");
            }
        }

        public static void SaveRegistry() {
            KEY.SetValue("Save Slot", SaveSlot);
            KEY.SetValue("Region", (int) Region);

            if (EmulatorID != 255) {
                KEY.SetValue("EmulatorType", (int) EmulatorID);
                if (EmulatorID == 10) {
                    KEY.SetValue("Other Emulator", EmulatorName);
                }
            }

            SUBKEY.SetValue("Ultimate Boss", UltimateBossCompleted);
            SUBKEY.SetValue("Inventory Size", InventorySize);
            SUBKEY.SetValue("Ultimate Shop", UltimateShopLimited);
            SUBKEY.SetValue("Faust", FaustCount);
        }

        public static StreamReader GetMod(string directory) {
            if (Settings.Instance.Preset == DraMod.Preset.Normal) {
                return new StreamReader(directory);
            } else {
                string embeddedResource = "";
                string[] split = new string[1];
                try {
                    split = directory.Split("\\");
                    string embeddedBuilder = "";

                    for (int i = split.Length - 1; i > -1; i--) {
                        if (split[i].Equals("Mods")) {
                            break;
                        } else {
                            embeddedBuilder += split[i] + ".|";
                        }
                    }

                    split = embeddedBuilder.Split("|");
                    Array.Reverse(split);
                    embeddedResource = "Dragoon_Modifier.DraMod.Resources." + String.Concat(split);
                    embeddedResource = embeddedResource.Substring(0, embeddedResource.Length - 1);
                    embeddedResource = embeddedResource.Replace("Lavitz Albert", "Lavitz_Albert");
                    embeddedResource = embeddedResource.Replace("Shana Miranda", "Shana_Miranda");

                    StreamReader resource = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedResource));

                    if (resource == null)
                        throw new Exception();

                    return new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedResource));
                } catch (Exception ex) {
                    Constants.Run = false;
                    Constants.UIControl.WriteGLog("Program stopped.");
                    Constants.UIControl.WritePLog("INTERNAL FILE ERROR");
                    Console.WriteLine("INTERNAL FILE ERROR\nFatal Error. Closing all threads.");
                    Console.WriteLine(String.Concat(split));
                }
            }
            return null;
        } 

        public static void WriteLine(object text) {
            Console.WriteLine(text);
        }
    }
}
