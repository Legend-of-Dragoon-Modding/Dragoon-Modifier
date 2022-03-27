using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSScriptLibrary;
using System.Reflection;

namespace Dragoon_Modifier.DraMod.LoDDict.Scripts {
    internal class CustomItemScript : IItemScript {
        private readonly string file;
        private readonly Assembly script;
        private readonly MethodDelegate battleRun;
        private readonly MethodDelegate battleSetup;
        private readonly MethodDelegate fieldRun;
        private readonly MethodDelegate fieldSetup;

        internal CustomItemScript(string file, Emulator.IEmulator emulator, ILoDDictionary loDDictionary, UI.IUIControl uiControl) {
            this.file = Path.GetFullPath(file);
            script = CSScript.LoadFile(file, null, true);
            battleRun = script.GetStaticMethod("*.BattleRun", emulator, uiControl);
            battleSetup = script.GetStaticMethod("*.BattleSetup", emulator, uiControl);
            fieldRun = script.GetStaticMethod("*.FieldRun", emulator, uiControl);
            fieldSetup = script.GetStaticMethod("*.FieldSetup", emulator, uiControl);
        }

        public void BattleRun(Emulator.IEmulator emulator, ILoDDictionary loDDictionary, UI.IUIControl uiControl) {
            try {
                battleRun(emulator, uiControl);
            } catch (Exception ex) {
                Constants.Run = false;
                uiControl.WriteGLog("Program stopped.");
                uiControl.WritePLog("CUSTOM ITEM SCRIPT ERROR");
                Console.WriteLine("CUSTOM ITEM SCRIPT ERROR\nFatal Error. Closing all threads.");
                Console.WriteLine(ex.ToString());
            }
        }

        public void BattleSetup(Emulator.IEmulator emulator, ILoDDictionary loDDictionary, UI.IUIControl uiControl) {
            try {
                battleSetup(emulator, uiControl);
            } catch (Exception ex) {
                Constants.Run = false;
                uiControl.WriteGLog("Program stopped.");
                uiControl.WritePLog("CUSTOM ITEM SCRIPT ERROR");
                Console.WriteLine("CUSTOM ITEM SCRIPT ERROR\nFatal Error. Closing all threads.");
                Console.WriteLine(ex.ToString());
            }
        }

        public void FieldRun(Emulator.IEmulator emulator, ILoDDictionary loDDictionary, UI.IUIControl uiControl) {
            try {
                fieldRun(emulator, uiControl);
            } catch (Exception ex) {
                Constants.Run = false;
                uiControl.WriteGLog("Program stopped.");
                uiControl.WritePLog("CUSTOM ITEM SCRIPT ERROR");
                Console.WriteLine("CUSTOM ITEM SCRIPT ERROR\nFatal Error. Closing all threads.");
                Console.WriteLine(ex.ToString());
            }
        }

        public void FieldSetup(Emulator.IEmulator emulator, ILoDDictionary loDDictionary, UI.IUIControl uiControl) {
            try {
                fieldSetup(emulator, uiControl);
            } catch (Exception ex) {
                Constants.Run = false;
                uiControl.WriteGLog("Program stopped.");
                uiControl.WritePLog("CUSTOM ITEM SCRIPT ERROR");
                Console.WriteLine("CUSTOM ITEM SCRIPT ERROR\nFatal Error. Closing all threads.");
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
