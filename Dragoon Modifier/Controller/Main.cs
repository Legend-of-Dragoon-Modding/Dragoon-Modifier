using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Controller {
    public static class Main {
        public static byte InventorySize = 32;
        public static bool StatsChanged = false;

        public static void Run(Emulator.IEmulator emulator) {
            while (Constants.RUN) {
                try {
                    switch (emulator.Memory.GameState) {
                        case Emulator.GameState.Battle:
                            if (!StatsChanged) {
                                Battle.Setup(emulator);
                                StatsChanged = true;
                            }
                            Battle.Run(emulator);
                            break;
                        case Emulator.GameState.Field:
                            if (StatsChanged) {
                                Field.Setup(emulator);
                                StatsChanged = false;
                            }
                            Field.Run(emulator);
                            break;
                    }
                    Thread.Sleep(250);
                } catch (Exception ex) {
                    Constants.RUN = false;
                    Constants.WriteGLog("Program stopped.");
                    Constants.WritePLogOutput("INTERNAL SCRIPT ERROR");
                    Constants.WriteOutput("Fatal Error. Closing all threads.");
                    Constants.WriteError(ex.ToString());
                }
            }
        }
    }
}
