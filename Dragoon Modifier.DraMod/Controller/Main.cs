using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Controller {
    internal static class Main {
        internal static bool StatsChanged = false;
        internal static bool MenuEntered = false;
        internal static void Run(ref Emulator.IEmulator emulator, UI.IUIControl uiControl, ref LoDDict.ILoDDictionary LoDDict) {
            while (Constants.Run) {
                try {
                    switch (emulator.Memory.GameState) {
                        case Emulator.GameState.Battle:
                            if (!StatsChanged) {
                                Battle.Setup(emulator, LoDDict, uiControl);
                                StatsChanged = true;
                            }
                            Battle.Run(emulator, uiControl, LoDDict);
                            break;
                        case Emulator.GameState.Field:
                            if (StatsChanged) {
                                Field.Setup(emulator, LoDDict, uiControl);
                                StatsChanged = false;
                            }
                            /*if (MenuEntered) {
                                Menu.Exit(emulator, uiControl);
                                MenuEntered = false;
                            }*/
                            Field.Run(emulator, uiControl);
                            //MenuEntered = false;
                            break;

                        case Emulator.GameState.Menu:
                            /*if (!MenuEntered) {
                                Menu.Setup(emulator, LoDDict, uiControl);
                                MenuEntered = true;
                            }*/
                            Field.Run(emulator, uiControl);
                            break;
                        case Emulator.GameState.BattleResult:
                            if (StatsChanged) {
                                BattleResult.Setup(emulator, LoDDict, uiControl);
                                StatsChanged = false;
                            }
                            break;
                    }

                    GreenButton.Run(emulator, uiControl);

                    if (Settings.KillBGM) {
                        KillBGM.Run(emulator, uiControl);
                    }

                    Thread.Sleep(Settings.LoopDelay);
                } catch (Exception ex) {
                    Constants.Run = false;
                    uiControl.WriteGLog("Program stopped.");
                    uiControl.WritePLog("INTERNAL SCRIPT ERROR");
                    Console.WriteLine("INTERNAL SCRIPT ERROR\nFatal Error. Closing all threads.");
                    Console.WriteLine(ex.ToString());
                }

            }
        }
    }
}
