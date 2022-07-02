using Dragoon_Modifier.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod {
    internal class DragoonModifier : IDraMod {
        private readonly string _cwd;
        private static readonly List<string> presetMods = new List<string> { "Normal", "NormalHard", "Hard", "HardHell", "Hell_Mode" };

        internal DragoonModifier(UI.IUIControl uiControl, string cwd) {
            Constants.LoadUIControl(uiControl);
            _cwd = cwd;
        }

        public bool Attach(string emulatorName, long previousOffset) {
            try {
                Emulator.Attach(emulatorName, previousOffset);
                
                Console.WriteLine($"Emulator offset:        {Convert.ToString(Emulator.EmulatorOffset, 16).ToUpper()}");
                Console.WriteLine($"Region:                 {Emulator.Region}");

                Constants.Run = true;

                if (presetMods.Contains(Settings.Mod)) {
                    ChangeLoDDirectory(GetValueFromDescription(Settings.Mod));
                } else {
                    ChangeLoDDirectory(Settings.Mod);
                }
                
                Thread t = new Thread(() => Controller.Main.Run());

                t.Start();
                return true;
            } catch (EmulatorNotFoundException) {
                Console.WriteLine($"[ERROR] Failed to attach to {emulatorName}. Process not found.");
                return false;
            } catch (EmulatorAttachException) {
                Console.WriteLine($"[ERROR] Failed to attach to {emulatorName}. Disc not recognized, make sure the game is loaded.");
                return false;
            }
            
        }

        public void ChangeLoDDirectory(string mod) {
            Settings.DualDifficulty = false;
            Settings.Mod = mod;
            if (Constants.Run) {
                Constants.UIControl.WritePLog("Changing mod directory to " + mod);
                Settings.LoadDataset(_cwd, mod);
                Controller.Main.BattleSetup = false;
                Controller.Main.AdditionsChanged = false;
                Controller.Main.ItemsChanged = false;
                Controller.Main.ShopChanged = false;
            }
        }

        public void ChangeLoDDirectory(Preset mod) {
            string modString = GetEnumDescription(mod);
            Settings.Difficulty = mod.ToString();
            Settings.Mod = modString;
            if (Constants.Run) {

                Dataset.Scripts.IScript script;
                if (mod != Preset.Normal) {
                    script = new Dataset.Scripts.HardMode.Script();
                    Settings.ItemStatChange = true;
                    Settings.ItemNameDescChange = true;
                    Settings.ItemIconChange = true;
                    Settings.DragoonStatChange = true;
                    Settings.DragoonSpellChange = true;
                    Settings.DragoonDescriptionChange = true;
                    Settings.DragoonAdditionChange = true;
                    Settings.MonsterStatChange = true;
                    Settings.MonsterExpGoldChange = true;
                    Settings.MonsterDropChange = true;
                } else {
                    script = new Dataset.Scripts.DummyScript();
                    Settings.ItemStatChange = false;
                    Settings.ItemNameDescChange = false;
                    Settings.ItemIconChange = false;
                    Settings.DragoonStatChange = false;
                    Settings.DragoonSpellChange = false;
                    Settings.DragoonDescriptionChange = false;
                    Settings.DragoonAdditionChange = false;
                    Settings.MonsterStatChange = false;
                    Settings.MonsterExpGoldChange = false;
                    Settings.MonsterDropChange = false;
                }

                bool dualMonster;
                string dualMod;
                if (mod == Preset.NormalHard || mod == Preset.HardHell) {
                    Settings.DualDifficulty = true;
                    dualMonster = true;
                    if (mod == Preset.NormalHard) {
                        dualMod = "US_Base";
                    } else {
                        dualMod = "Hard_Mode";
                    }
                } else {
                    Settings.DualDifficulty = false;
                    dualMonster = false;
                    dualMod = "";
                }

                Constants.UIControl.WritePLog("Changing mod directory to " + modString);
                Settings.LoadDataset(_cwd, modString, script, dualMonster, dualMod);

                

                Controller.Main.BattleSetup = false;
                Controller.Main.AdditionsChanged = false;
                Controller.Main.ItemsChanged = false;
                Controller.Main.ShopChanged = false;
            }
        }

        private static string GetEnumDescription(Preset mod) {
            FieldInfo fi = mod.GetType().GetField(mod.ToString());

            DescriptionAttribute[]? attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any()) {
                return attributes.First().Description;
            }

            return mod.ToString();
        }

        private static Preset GetValueFromDescription(string description) {
            foreach (var field in typeof(Preset).GetFields()) {
                if (Attribute.GetCustomAttribute(field,
                typeof(DescriptionAttribute)) is DescriptionAttribute attribute) {
                    if (attribute.Description == description)
                        return (Preset) field.GetValue(null);
                } else {
                    if (field.Name == description)
                        return (Preset) field.GetValue(null);
                }
            }

            throw new ArgumentException("Not found.", nameof(description));
        }
    }
}
