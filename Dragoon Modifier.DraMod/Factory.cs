using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod {
    public static class Factory {
        public static IDraMod DraMod(UI.IUIControl uiControl, string cwd) {
            return new DragoonModifier(uiControl, cwd);
        }

        public static LoDDict.ILoDDictionary LoDDictionary(Emulator.IEmulator emulator, UI.IUIControl uiControl, string cwd, string mod) {
            return new LoDDict.LoDDictionary(emulator, uiControl, cwd, mod);
        }
    }
}
