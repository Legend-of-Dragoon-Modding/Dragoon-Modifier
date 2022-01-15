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
        private Emulator.IEmulator _emulator;
        private readonly UI.IUIControl _uiControl;
        private static LoDDict.ILoDDictionary _LoDDict;
        private readonly string _cwd;
        private static readonly List<string> presetMods = new List<string> { "Normal", "NormalHard", "Hard", "HardHell", "Hell" };

        internal DragoonModifier(UI.IUIControl uiControl, string cwd) {
            _uiControl = uiControl;
            _cwd = cwd;
        }

        public bool Attach(string emulatorName, long previousOffset) {
            try {
                _emulator = Emulator.Factory.Create(emulatorName, previousOffset);
                Console.WriteLine($"Emulator offset:        {Convert.ToString(_emulator.EmulatorOffset, 16).ToUpper()}");
                Console.WriteLine($"Region:                 {_emulator.Region}");

                Constants.Run = true;

                if (presetMods.Contains(Settings.Mod)) {
                    ChangeLoDDirectory(GetValueFromDescription(Settings.Mod));
                } else {
                    ChangeLoDDirectory(Settings.Mod);
                }
                
                Thread t = new Thread(() => Controller.Main.Run(ref _emulator, _uiControl, ref _LoDDict));

                t.Start();
                return true;
            } catch (Emulator.EmulatorNotFoundException) {
                Console.WriteLine($"[ERROR] Failed to attach to {emulatorName}. Process not found.");
                return false;
            } catch (Emulator.EmulatorAttachException) {
                Console.WriteLine($"[ERROR] Failed to attach to {emulatorName}. Disc not recognized, make sure the game is loaded.");
                return false;
            }
            
        }

        public void ChangeLoDDirectory(string mod) {
            Settings.DualDifficulty = false;
            Settings.Mod = mod;
            if (Constants.Run) {
                _uiControl.WritePLog("Changing mod directory to " + mod);
                _LoDDict = new LoDDict.LoDDictionary(_emulator, _uiControl, _cwd, mod);
                Controller.Main.StatsChanged = false;
            }
        }

        public void ChangeLoDDirectory(Preset mod) {
            string modString = GetEnumDescription(mod);
            Settings.Difficulty = mod.ToString();

            Settings.DualDifficulty = false;
            if (mod == Preset.NormalHard || mod == Preset.HardHell) {
                Settings.DualDifficulty = true;
            } else {
                Settings.DualDifficulty = false;
            }
            Settings.Mod = modString;
            if (Constants.Run) {

                LoDDict.Scripts.IItemScript ItemScript = new LoDDict.Scripts.DummyItemScript();
                switch (mod) {
                    case Preset.Hell:
                    case Preset.HardHell:
                    case Preset.Hard:
                        ItemScript = new LoDDict.Scripts.HardMode.ItemScript();
                        break;
                }
                

                _uiControl.WritePLog("Changing mod directory to " + modString);
                _LoDDict = new LoDDict.LoDDictionary(_emulator, _uiControl, _cwd, modString, ItemScript);
                Controller.Main.StatsChanged = false;
            }
        }

        private static string GetEnumDescription(Preset mod) {
            FieldInfo fi = mod.GetType().GetField(mod.ToString());

            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

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
