﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Controller {
    internal static class Main {
        internal static bool StatsChanged = false;
        internal static void Run(Emulator.IEmulator emulator, UI.IUIControl uiControl) {
            while (true) { // TODO Constants.RUN
                try {
                    switch (emulator.Memory.GameState) {
                        case Emulator.GameState.Battle:
                            if (!StatsChanged) {
                                Battle.Setup(emulator, uiControl);
                                StatsChanged = true;
                            }
                            Battle.Run(emulator, uiControl);
                            break;
                    }
                    Thread.Sleep(250);
                } catch (Exception ex) {
                    // Constants.RUN = false;
                    uiControl.WriteGLog("Program stopped.");
                    uiControl.WritePLog("INTERNAL SCRIPT ERROR");
                    Console.WriteLine("INTERNAL SCRIPT ERROR\nFatal Error. Closing all threads.");
                    Console.WriteLine(ex.ToString());
                }

            }
        }
    }
}
