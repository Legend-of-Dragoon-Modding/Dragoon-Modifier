using CSScriptLibrary;
using System;
using System.IO;

namespace Dragoon_Modifier {
    class SubScript {
        string file;
        public ScriptState state = ScriptState.DISABLED;

        public SubScript(string file) {
            this.file = Path.GetFullPath(file);
        }

        public SubScript(string file, ScriptState state) {
            this.file = Path.GetFullPath(file);
            this.state = state;
        }

        public int Run(Emulator emulator) {
            try {
                var script = CSScript.LoadFile(file, null, true).GetStaticMethod("*.Run", emulator);
                script(emulator);
                return 1;
            } catch (Exception ex) {
                Constants.RUN = false;
                Constants.WriteGLog("Program stopped.");
                Constants.WritePLog("SCRIPT ERROR: " + ToString());
                Constants.WriteOutput("SCRIPT ERROR: " + ToString());
                Constants.WriteOutput("Fatal Error. Closing all threads.");
                Constants.WriteDebug(ex.ToString());
                return 0;
            }
        }

        public int Open(Emulator emulator) {
            try {
                var script = CSScript.LoadFile(file, null, true).GetStaticMethod("*.Open", emulator);
                script(emulator);
                return 1;
            } catch (Exception ex) {
                Constants.RUN = false;
                Constants.WriteGLog("Program stopped.");
                Constants.WritePLog("SCRIPT ERROR: " + ToString());
                Constants.WriteOutput("SCRIPT ERROR: " + ToString());
                Constants.WriteOutput("Fatal Error. Closing all threads.");
                Constants.WriteDebug(ex.ToString());
                return 0;
            }
        }

        public int Close(Emulator emulator) {
            try {
                var script = CSScript.LoadFile(file, null, true).GetStaticMethod("*.Close", emulator);
                script(emulator);
                return 1;
            } catch (Exception ex) {
                Constants.RUN = false;
                Constants.WriteGLog("Program stopped.");
                Constants.WritePLog("SCRIPT ERROR: " + ToString());
                Constants.WriteOutput("SCRIPT ERROR: " + ToString());
                Constants.WriteOutput("Fatal Error. Closing all threads.");
                Constants.WriteDebug(ex.ToString());
                return 0;
            }
        }

        public int Click(Emulator emulator) {
            try {
                var script = CSScript.LoadFile(file, null, true).GetStaticMethod("*.Click", emulator);
                script(emulator);
                return 1;
            } catch (Exception ex) {
                Constants.RUN = false;
                Constants.WriteGLog("Program stopped.");
                Constants.WritePLog("SCRIPT ERROR: " + ToString());
                Constants.WriteOutput("SCRIPT ERROR: " + ToString());
                Constants.WriteOutput("Fatal Error. Closing all threads.");
                Constants.WriteDebug(ex.ToString());
                return 0;
            }
        }

        override public string ToString() {
            return Path.GetFileNameWithoutExtension(file);
        }

        public string GetPath() {
            return file;
        }
    }
}