using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.LoDDict.Scripts {
    public interface IItemScript {
        void BattleSetup(Emulator.IEmulator emulator, UI.IUIControl uiControl);
        void BattleRun(Emulator.IEmulator emulator, UI.IUIControl uiControl);
        void FieldSetup(Emulator.IEmulator emulator, UI.IUIControl uiControl);
        void FieldRun(Emulator.IEmulator emulator, UI.IUIControl uiControl);
    }
}
