using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Controller {
    internal static class Main {
        internal static bool MenuEntered = false;

        internal static bool BattleSetup = false;
        internal static bool AdditionsChanged = false;
        internal static bool ItemsChanged = false;
        internal static bool ShopChanged = false;
        private static bool ShopFix = false;

        internal static void Run(ref Emulator.IEmulator emulator, UI.IUIControl uiControl, ref LoDDict.ILoDDictionary LoDDict) {
            while (Constants.Run) {
                try {
                    switch (emulator.Memory.GameState) {
                        case Emulator.GameState.Battle:
                            if (!BattleSetup) {
                                Battle.Setup(emulator, LoDDict, uiControl);
                                BattleSetup = true;
                                AdditionsChanged = false;
                                ItemsChanged = false;
                                ShopChanged = false;
                            }
                            Battle.Run(emulator, uiControl, LoDDict);
                            break;

                        case Emulator.GameState.Field:
                            BattleSetup = false;

                            if (!ShopChanged) {
                                if (Settings.ShopChange) {
                                    Shop.TableChange(emulator, LoDDict);
                                }
                                ShopChanged = true;
                            }

                            if (MenuEntered && Settings.NoDart != 255) {
                                MenuEntered = false;
                                emulator.Memory.PartySlot[0] = 0;
                            }
                            Field.Run(emulator, uiControl);
                            break;

                        case Emulator.GameState.Overworld:
                            BattleSetup = false;
                            ShopChanged = false;
                            break;

                        case Emulator.GameState.Menu:
                            BattleSetup = false;

                            if (!ItemsChanged) {
                                Field.ItemSetup(emulator, LoDDict);
                                ItemsChanged = true;
                            }

                            if (!AdditionsChanged) {
                                Field.AdditionSetup(emulator, LoDDict);
                                AdditionsChanged = true;
                            }

                            if (!MenuEntered && Settings.NoDart != 255) {
                                emulator.Memory.PartySlot[0] = Settings.NoDart;
                                MenuEntered = true;
                            }
                            break;

                        case Emulator.GameState.BattleResult:
                            BattleSetup = false;
                            if (!ItemsChanged) {
                                Field.ItemSetup(emulator, LoDDict);
                                ItemsChanged = true;
                            }
                            break;

                        case Emulator.GameState.ChangeDisc:
                            ItemsChanged = false;
                            AdditionsChanged = false;
                            ShopChanged = false;
                            BattleSetup = false;
                            ShopFix = true;
                            break;

                        case Emulator.GameState.Shop:
                            BattleSetup = false;
                            if (!ItemsChanged) {
                                Field.ItemSetup(emulator, LoDDict);
                                ItemsChanged = true;
                            }

                            if (ShopFix) {
                                ShopChanged = false;
                                ShopFix = false;
                                Shop.ContentChange(emulator, LoDDict);
                            }

                            if (!ShopChanged) {
                                if (Settings.ShopChange) {
                                    Shop.TableChange(emulator, LoDDict);
                                }
                                ShopChanged = true;
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
