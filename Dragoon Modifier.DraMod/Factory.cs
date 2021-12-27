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
    }
}