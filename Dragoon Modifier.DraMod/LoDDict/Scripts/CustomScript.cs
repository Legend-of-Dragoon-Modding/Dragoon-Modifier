using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSScriptLibrary;
using System.Reflection;

namespace Dragoon_Modifier.DraMod.LoDDict.Scripts {
    internal class CustomScript : IScript {
        private readonly string file;
        private readonly Assembly script;
        private readonly MethodDelegate battleRun;
        private readonly MethodDelegate battleSetup;
        private readonly MethodDelegate fieldRun;
        private readonly MethodDelegate fieldSetup;

        internal CustomScript(string file, ILoDDictionary loDDictionary, UI.IUIControl uiControl) {
            this.file = Path.GetFullPath(file);
            script = CSScript.LoadFile(file, null, true);
            battleRun = script.GetStaticMethod("*.BattleRun", loDDictionary, uiControl);
            battleSetup = script.GetStaticMethod("*.BattleSetup", loDDictionary, uiControl);
            fieldRun = script.GetStaticMethod("*.FieldRun", loDDictionary, uiControl);
            fieldSetup = script.GetStaticMethod("*.FieldSetup", loDDictionary, uiControl);
        }

        public void BattleRun(ILoDDictionary loDDictionary, UI.IUIControl uiControl) {
            try {
                battleRun(loDDictionary, uiControl);
            } catch (Exception ex) {
                Constants.Run = false;
                uiControl.WriteGLog("Program stopped.");
                uiControl.WritePLog("CUSTOM ITEM SCRIPT ERROR");
                Console.WriteLine("CUSTOM ITEM SCRIPT ERROR\nFatal Error. Closing all threads.");
                Console.WriteLine(ex.ToString());
            }
        }

        public void BattleSetup(ILoDDictionary loDDictionary, UI.IUIControl uiControl) {
            try {
                battleSetup(loDDictionary, uiControl);
            } catch (Exception ex) {
                Constants.Run = false;
                uiControl.WriteGLog("Program stopped.");
                uiControl.WritePLog("CUSTOM ITEM SCRIPT ERROR");
                Console.WriteLine("CUSTOM ITEM SCRIPT ERROR\nFatal Error. Closing all threads.");
                Console.WriteLine(ex.ToString());
            }
        }

        public void FieldRun(ILoDDictionary loDDictionary, UI.IUIControl uiControl) {
            try {
                fieldRun(loDDictionary, uiControl);
            } catch (Exception ex) {
                Constants.Run = false;
                uiControl.WriteGLog("Program stopped.");
                uiControl.WritePLog("CUSTOM ITEM SCRIPT ERROR");
                Console.WriteLine("CUSTOM ITEM SCRIPT ERROR\nFatal Error. Closing all threads.");
                Console.WriteLine(ex.ToString());
            }
        }

        public void FieldSetup(ILoDDictionary loDDictionary, UI.IUIControl uiControl) {
            try {
                fieldSetup(loDDictionary, uiControl);
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
