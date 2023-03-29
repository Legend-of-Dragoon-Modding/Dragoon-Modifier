using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using CSScriptLib;

namespace Dragoon_Modifier.DraMod.Dataset.Scripts {
    internal class CustomScript : IScript {
        private readonly string _file;
        private readonly dynamic _script;

        internal CustomScript(string file) {
            _file = Path.GetFullPath(file);
            _script = CSScript.Evaluator.LoadFile(file);
        }

        public void BattleRun() {
            try {
                _script.BattleRun();
            } catch (Exception ex) {
                WriteError(ex);
            }
        }

        public void BattleSetup() {
            try {
                _script.BattleSetup();
            } catch (Exception ex) {
                WriteError(ex);
            }
        }

        public void FieldRun() {
            try {
                _script.FieldRun();
            } catch (Exception ex) {
                WriteError(ex);
            }
        }

        public void FieldSetup() {
            try {
                _script.FieldSetup();
            } catch (Exception ex) {
                WriteError(ex);
            }
        }


        private static void WriteError(Exception ex) {
            Constants.UIControl.WriteGLog("Program stopped.");
            Constants.UIControl.WritePLog("CUSTOM SCRIPT ERROR");
            Console.WriteLine("CUSTOM SCRIPT ERROR\nFatal Error. Closing all threads.");
            Console.WriteLine(ex.ToString());
        }
    }
}
