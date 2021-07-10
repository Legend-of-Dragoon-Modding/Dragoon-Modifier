using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Dragoon_Modifier.UI {
    public static class Factory {
        public static DraMod.UI.IUIControl UIControl(TextBlock[,] monsterDisplay, TextBlock[,] characterDisplay, TextBlock glog, TextBlock plog) {
            return new UIControl(monsterDisplay, characterDisplay, glog, plog);
        }
    }
}
