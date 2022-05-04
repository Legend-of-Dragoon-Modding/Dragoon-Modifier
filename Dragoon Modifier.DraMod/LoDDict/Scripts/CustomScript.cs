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

        internal CustomScript(string file, ILoDDictionary loDDictionary) {
            this.file = Path.GetFullPath(file);
            script = CSScript.LoadFile(file, null, true);
            battleRun = script.GetStaticMethod("*.BattleRun", loDDictionary);
            battleSetup = script.GetStaticMethod("*.BattleSetup", loDDictionary);
            fieldRun = script.GetStaticMethod("*.FieldRun", loDDictionary);
            fieldSetup = script.GetStaticMethod("*.FieldSetup", loDDictionary);
        }

        public void BattleRun(ILoDDictionary loDDictionary) {
            try {
                battleRun(loDDictionary);
            } catch (Exception ex) {
                Constants.Run = false;
                Constants.UIControl.WriteGLog("Program stopped.");
                Constants.UIControl.WritePLog("CUSTOM ITEM SCRIPT ERROR");
                Console.WriteLine("CUSTOM ITEM SCRIPT ERROR\nFatal Error. Closing all threads.");
                Console.WriteLine(ex.ToString());
            }
        }

        public void BattleSetup(ILoDDictionary loDDictionary) {
            try {
                battleSetup(loDDictionary);
            } catch (Exception ex) {
                Constants.Run = false;
                Constants.UIControl.WriteGLog("Program stopped.");
                Constants.UIControl.WritePLog("CUSTOM ITEM SCRIPT ERROR");
                Console.WriteLine("CUSTOM ITEM SCRIPT ERROR\nFatal Error. Closing all threads.");
                Console.WriteLine(ex.ToString());
            }
        }

        public void FieldRun(ILoDDictionary loDDictionary) {
            try {
                fieldRun(loDDictionary);
            } catch (Exception ex) {
                Constants.Run = false;
                Constants.UIControl.WriteGLog("Program stopped.");
                Constants.UIControl.WritePLog("CUSTOM ITEM SCRIPT ERROR");
                Console.WriteLine("CUSTOM ITEM SCRIPT ERROR\nFatal Error. Closing all threads.");
                Console.WriteLine(ex.ToString());
            }
        }

        public void FieldSetup(ILoDDictionary loDDictionary) {
            try {
                fieldSetup(loDDictionary);
            } catch (Exception ex) {
                Constants.Run = false;
                Constants.UIControl.WriteGLog("Program stopped.");
                Constants.UIControl.WritePLog("CUSTOM ITEM SCRIPT ERROR");
                Console.WriteLine("CUSTOM ITEM SCRIPT ERROR\nFatal Error. Closing all threads.");
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
