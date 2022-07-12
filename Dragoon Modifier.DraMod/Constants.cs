using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod {
    public static class Constants {
        public const string Version = "4.0"; // Lower version for testing
        public static bool Run = false;
        public static string EmulatorName = "ePSXe";
        public static long PreviousOffset = 0xA579A0;
        public static bool Debug = true;

        private static UI.IUIControl? _uiControl = null;
        public static UI.IUIControl UIControl {
            get {
                if (_uiControl == null) {
                    throw new NotImplementedException(); // This should never ever be called.
                }
                return _uiControl;
            }
        }

        public static void LoadUIControl(UI.IUIControl uiControl) {
            _uiControl = uiControl;
        }

        public static void WriteLine(object text) {
            Console.WriteLine(text);
        }
    }
}
