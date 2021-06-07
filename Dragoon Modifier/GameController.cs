using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dragoon_Modifier.Core;

namespace Dragoon_Modifier {
    public static class GameController {
        public static byte InventorySize = 32;
        public static bool StatsChanged = false;
        public static void Run() {
            while (Constants.RUN) {
                try { 
                    switch (Emulator.Memory.GameState) {
                        case GameState.Battle:
                            if (StatsChanged) {
                                BattleController2.Run();
                            } else {
                                BattleController2.Setup();
                            }
                            break;
                        case GameState.Field:
                            FieldController.Field();
                            break;
                        case GameState.Overworld:
                            FieldController.Overworld();
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
