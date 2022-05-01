using Dragoon_Modifier.DraMod.UI;
using Dragoon_Modifier.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.LoDDict.Scripts {
    internal class DummyScript : IScript {
        public void BattleRun(ILoDDictionary loDDictionary, IUIControl uiControl) {}

        public void BattleSetup(ILoDDictionary loDDictionary, IUIControl uiControl) {}

        public void FieldRun(ILoDDictionary loDDictionary, IUIControl uiControl) {}

        public void FieldSetup(ILoDDictionary loDDictionary, IUIControl uiControl) {}
    }
}
