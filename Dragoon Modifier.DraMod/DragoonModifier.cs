using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod {
    internal class DragoonModifier : IDraMod {
        private Emulator.IEmulator _emulator;
        private readonly UI.IUIControl _uiControl;
        private static LoDDict.ILoDDictionary _LoDDict;
        private string _cwd;

        internal DragoonModifier(UI.IUIControl uiControl, string cwd) {
            _uiControl = uiControl;
            _cwd = cwd;
        }

        public bool Attach(string emulatorName, long previousOffset) {
            try {
                _emulator = Emulator.Factory.Create(emulatorName, previousOffset);
                Console.WriteLine($"Emulator offset:        {Convert.ToString(_emulator.EmulatorOffset, 16).ToUpper()}");
                Console.WriteLine($"Region:                 {_emulator.Region}");

                _LoDDict = Factory.LoDDictionary(_emulator, _cwd, Settings.Mod);

                Constants.Run = true;
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
            Settings.Mod = mod;
            if (Constants.Run) {
                _uiControl.WritePLog("Changing mod directory to " + mod);
                _LoDDict = Factory.LoDDictionary(_emulator, _cwd, mod);
                Controller.Main.StatsChanged = false;
            }
        }
    }
}
