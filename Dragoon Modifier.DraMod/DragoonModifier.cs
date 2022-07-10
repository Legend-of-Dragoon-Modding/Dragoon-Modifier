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

                if (Settings.Instance.Preset == Preset.Custom) {
                    ChangeLoDDirectory(Settings.Instance.CustomMod);
                } else {
                    ChangeLoDDirectory(Settings.Instance.Preset);
                }
                
                Thread t = new(() => Controller.Main.Run());

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
            Settings.Instance.DualDifficulty = false;
            Settings.Instance.Preset = Preset.Custom;
            Settings.Instance.CustomMod = mod;
            if (Constants.Run) {
                Constants.UIControl.WritePLog("Changing mod directory to " + mod);
                Settings.Instance.LoadDataset(_cwd, mod);
                Controller.Main.BattleSetup = false;
                Controller.Main.AdditionsChanged = false;
                Controller.Main.ItemsChanged = false;
                Controller.Main.ShopChanged = false;
            }
        }

        public void ChangeLoDDirectory(Preset mod) {
            Settings.Instance.Preset = mod;
            if (Constants.Run) {
                Dataset.Scripts.IScript script;
                if (mod != Preset.Normal) {
                    script = new Dataset.Scripts.HardMode.Script();
                    Settings.Instance.ItemStatChange = true;
                    Settings.Instance.ItemNameDescChange = true;
                    Settings.Instance.ItemIconChange = true;
                    Settings.Instance.DragoonStatChange = true;
                    Settings.Instance.DragoonSpellChange = true;
                    Settings.Instance.DragoonDescriptionChange = true;
                    Settings.Instance.DragoonAdditionChange = true;
                    Settings.Instance.MonsterStatChange = true;
                    Settings.Instance.MonsterExpGoldChange = true;
                    Settings.Instance.MonsterDropChange = true;
                } else {
                    script = new Dataset.Scripts.DummyScript();
                    Settings.Instance.ItemStatChange = false;
                    Settings.Instance.ItemNameDescChange = false;
                    Settings.Instance.ItemIconChange = false;
                    Settings.Instance.DragoonStatChange = false;
                    Settings.Instance.DragoonSpellChange = false;
                    Settings.Instance.DragoonDescriptionChange = false;
                    Settings.Instance.DragoonAdditionChange = false;
                    Settings.Instance.MonsterStatChange = false;
                    Settings.Instance.MonsterExpGoldChange = false;
                    Settings.Instance.MonsterDropChange = false;
                }

                string? dualMod;
                if (mod == Preset.NormalHard || mod == Preset.HardHell) {
                    Settings.Instance.DualDifficulty = true;
                    if (mod == Preset.NormalHard) {
                        dualMod = "US_Base";
                    } else {
                        dualMod = "Hard_Mode";
                    }
                } else {
                    Settings.Instance.DualDifficulty = false;
                    dualMod = null;
                }

                Constants.UIControl.WritePLog("Changing mod directory to " + mod.PresetToModFolder());
                Settings.Instance.LoadDataset(_cwd, mod.PresetToModFolder(), dualMod, script);

                Controller.Main.BattleSetup = false;
                Controller.Main.AdditionsChanged = false;
                Controller.Main.ItemsChanged = false;
                Controller.Main.ShopChanged = false;
            }
        }
    }
}
