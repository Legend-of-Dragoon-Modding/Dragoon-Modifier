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

        public SubScript(string file) {
            this.file = Path.GetFullPath(file);
            script = CSScript.LoadFile(file, null, true);
            run = script.GetStaticMethod("*.Run");
            open = script.GetStaticMethod("*.Open");
            close = script.GetStaticMethod("*.Close");
            click = script.GetStaticMethod("*.Click");
        }

        public SubScript(string file, ScriptState state) {
            this.file = Path.GetFullPath(file);
            this.state = state;
            script = CSScript.LoadFile(file, null, true);
            run = script.GetStaticMethod("*.Run");
            open = script.GetStaticMethod("*.Open");
            close = script.GetStaticMethod("*.Close");
            click = script.GetStaticMethod("*.Click");
        }

        public int Run() {
            try {
                run();
                return 1;
            } catch (Exception ex) {
                Constants.RUN = false;
                Constants.WriteGLog("Program stopped.");
                Constants.WritePLogOutput("SCRIPT ERROR: " + ToString());
                Constants.WriteOutput("Fatal Error. Closing all threads.");
                Constants.WriteError(ex.ToString());
                return 0;
            }
        }

        public int Open() {
            try {
                open();
                return 1;
            } catch (Exception ex) {
                Constants.RUN = false;
                Constants.WriteGLog("Program stopped.");
                Constants.WritePLogOutput("SCRIPT ERROR: " + ToString());
                Constants.WriteOutput("Fatal Error. Closing all threads.");
                Constants.WriteError(ex.ToString());
                return 0;
            }
        }

        public int Close() {
            try {
                close();
                return 1;
            } catch (Exception ex) {
                Constants.RUN = false;
                Constants.WriteGLog("Program stopped.");
                Constants.WritePLogOutput("SCRIPT ERROR: " + ToString());
                Constants.WriteOutput("Fatal Error. Closing all threads.");
                Constants.WriteError(ex.ToString());
                return 0;
            }
        }

        public int Click() {
            try {
                click();
                return 1;
            } catch (Exception ex) {
                Constants.RUN = false;
                Constants.WriteGLog("Program stopped.");
                Constants.WritePLogOutput("SCRIPT ERROR: " + ToString());
                Constants.WriteOutput("Fatal Error. Closing all threads.");
                Constants.WriteError(ex.ToString());
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