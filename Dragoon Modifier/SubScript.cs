using CSScriptLibrary;
using System;
using System.IO;
using System.Reflection;

namespace Dragoon_Modifier {
    class SubScript {
        string file;
        public ScriptState state = ScriptState.DISABLED;
        Assembly script;
        MethodDelegate run;
        MethodDelegate open;
        MethodDelegate close;
        MethodDelegate click;

        public SubScript(string file, Emulator emulator) {
            this.file = Path.GetFullPath(file);
            script = CSScript.LoadFile(file, null, true);
            run = script.GetStaticMethod("*.Run", emulator);
            open = script.GetStaticMethod("*.Open", emulator);
            close = script.GetStaticMethod("*.Close", emulator);
            click = script.GetStaticMethod("*.Click", emulator);
        }

        public SubScript(string file, ScriptState state, Emulator emulator) {
            this.file = Path.GetFullPath(file);
            this.state = state;
            script = CSScript.LoadFile(file, null, true);
            run = script.GetStaticMethod("*.Run", emulator);
            open = script.GetStaticMethod("*.Open", emulator);
            close = script.GetStaticMethod("*.Close", emulator);
            click = script.GetStaticMethod("*.Click", emulator);
        }

        public int Run(Emulator emulator) {
            try {
                run(emulator);
                return 1;
            } catch (Exception ex) {
                Constants.RUN = false;
                Constants.WriteGLog("Program stopped.");
                Constants.WritePLogOutput("SCRIPT ERROR: " + ToString());
                Constants.WriteOutput("Fatal Error. Closing all threads.");
                Constants.WriteOutput(ex.ToString());
                return 0;
            }
        }

        public int Open(Emulator emulator) {
            try {
                open(emulator);
                return 1;
            } catch (Exception ex) {
                Constants.RUN = false;
                Constants.WriteGLog("Program stopped.");
                Constants.WritePLogOutput("SCRIPT ERROR: " + ToString());
                Constants.WriteOutput("Fatal Error. Closing all threads.");
                Constants.WriteOutput(ex.ToString());
                return 0;
            }
        }

        public int Close(Emulator emulator) {
            try {
                close(emulator);
                return 1;
            } catch (Exception ex) {
                Constants.RUN = false;
                Constants.WriteGLog("Program stopped.");
                Constants.WritePLogOutput("SCRIPT ERROR: " + ToString());
                Constants.WriteOutput("Fatal Error. Closing all threads.");
                Constants.WriteOutput(ex.ToString());
                return 0;
            }
        }

        public int Click(Emulator emulator) {
            try {
                click(emulator);
                return 1;
            } catch (Exception ex) {
                Constants.RUN = false;
                Constants.WriteGLog("Program stopped.");
                Constants.WritePLogOutput("SCRIPT ERROR: " + ToString());
                Constants.WriteOutput("Fatal Error. Closing all threads.");
                Constants.WriteOutput(ex.ToString());
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