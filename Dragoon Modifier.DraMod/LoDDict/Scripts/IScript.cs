using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.LoDDict.Scripts {
    public interface IScript {
        void BattleSetup(ILoDDictionary loDDictionary, UI.IUIControl uiControl);
        void BattleRun(ILoDDictionary loDDictionary, UI.IUIControl uiControl);
        void FieldSetup(ILoDDictionary loDDictionary, UI.IUIControl uiControl);
        void FieldRun(ILoDDictionary loDDictionary, UI.IUIControl uiControl);
    }
}
