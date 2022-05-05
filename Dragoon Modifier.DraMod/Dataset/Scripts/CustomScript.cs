using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSScriptLibrary;
using System.Reflection;

namespace Dragoon_Modifier.DraMod.Dataset.Scripts {
    internal class CustomScript : IScript {
        private readonly string _file;
        private readonly Assembly _script;
        private readonly MethodDelegate _battleRun;
        private readonly MethodDelegate _battleSetup;
        private readonly MethodDelegate _fieldRun;
        private readonly MethodDelegate _fieldSetup;

        internal CustomScript(string file) {
            _file = Path.GetFullPath(file);
            _script = CSScript.LoadFile(_file, null, true);
            _battleRun = _script.GetStaticMethod("*.BattleRun");
            _battleSetup = _script.GetStaticMethod("*.BattleSetup");
            _fieldRun = _script.GetStaticMethod("*.FieldRun");
            _fieldSetup = _script.GetStaticMethod("*.FieldSetup");
        }

        public void BattleRun() {
            try {
                _battleRun();
            } catch (Exception ex) {
                WriteError(ex);
            }
        }

        public void BattleSetup() {
            try {
                _battleSetup();
            } catch (Exception ex) {
                WriteError(ex);
            }
        }

        public void FieldRun() {
            try {
                _fieldRun();
            } catch (Exception ex) {
                WriteError(ex);
            }
        }

        public void FieldSetup() {
            try {
                _fieldSetup();
            } catch (Exception ex) {
                WriteError(ex);
            }
        }


        private static void WriteError(Exception ex) {
            Constants.UIControl.WriteGLog("Program stopped.");
            Constants.UIControl.WritePLog("CUSTOM ITEM SCRIPT ERROR");
            Console.WriteLine("CUSTOM ITEM SCRIPT ERROR\nFatal Error. Closing all threads.");
            Console.WriteLine(ex.ToString());
        }
    }
}
