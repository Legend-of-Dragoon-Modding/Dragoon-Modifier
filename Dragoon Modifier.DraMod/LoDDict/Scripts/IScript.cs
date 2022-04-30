﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.LoDDict.Scripts {
    public interface IScript {
        void BattleSetup(Emulator.IEmulator emulator, ILoDDictionary loDDictionary, UI.IUIControl uiControl);
        void BattleRun(Emulator.IEmulator emulator, ILoDDictionary loDDictionary, UI.IUIControl uiControl);
        void FieldSetup(Emulator.IEmulator emulator, ILoDDictionary loDDictionary, UI.IUIControl uiControl);
        void FieldRun(Emulator.IEmulator emulator, ILoDDictionary loDDictionary, UI.IUIControl uiControl);
    }
}