﻿using Dragoon_Modifier.DraMod.UI;
using Dragoon_Modifier.Emulator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.LoDDict.Scripts {
    internal class DummyScript : IScript {
        public void BattleRun(IEmulator emulator, ILoDDictionary loDDictionary, IUIControl uiControl) {}

        public void BattleSetup(IEmulator emulator, ILoDDictionary loDDictionary, IUIControl uiControl) {}

        public void FieldRun(IEmulator emulator, ILoDDictionary loDDictionary, IUIControl uiControl) {}

        public void FieldSetup(IEmulator emulator, ILoDDictionary loDDictionary, IUIControl uiControl) {}
    }
}