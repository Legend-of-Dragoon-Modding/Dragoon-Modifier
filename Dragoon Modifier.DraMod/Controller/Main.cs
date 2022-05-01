using Dragoon_Modifier.Core;
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
        private static bool HPCapSet = false;

        internal static void Run(UI.IUIControl uiControl, ref LoDDict.ILoDDictionary LoDDict) {
            while (Constants.Run) {
                try {
                    switch (Emulator.Memory.GameState) {
                        case GameState.Battle:
                            if (!BattleSetup) {
                                if (Settings.NoDart > 0) {
                                    Emulator.Memory.PartySlot[0] = 0;
                                }
                                Battle.Setup(LoDDict, uiControl);
                                BattleSetup = true;
                                AdditionsChanged = false;
                                ItemsChanged = false;
                                ShopChanged = false;
                                HPCapSet = false;
                            }
                            Battle.Run(uiControl, LoDDict);
                            break;

                        case GameState.Field:
                            BattleSetup = false;

                            if (!ShopChanged) {
                                uiControl.ResetBattle();
                                if (Settings.ShopChange) {
                                    Shop.TableChange(LoDDict);
                                }
                                ShopChanged = true;
                            }

                            if (Settings.NoDart != 255 && Emulator.Memory.PartySlot[0] != 0) {
                                Settings.NoDart = (byte) Emulator.Memory.PartySlot[0];
                                Emulator.Memory.PartySlot[0] = 0;
                            }
                            

                            if (!HPCapSet) {
                                HPCapSet = true;
                                if (Settings.RemoveHPCap) {
                                    Emulator.Memory.FieldHPCap = 30000;
                                }
                            }

                            Field.Run(uiControl);
                            break;

                        case GameState.Overworld:
                            BattleSetup = false;
                            ShopChanged = false;

                            if (!HPCapSet) {
                                HPCapSet = true;
                                if (Settings.RemoveHPCap) {
                                    Emulator.Memory.FieldHPCap = 30000;
                                }
                            }
                            break;

                        case GameState.Menu:
                            BattleSetup = false;

                            if (!ItemsChanged) {
                                Field.ItemSetup(LoDDict);
                                ItemsChanged = true;
                            }

                            if (!AdditionsChanged) {
                                Field.AdditionSetup(LoDDict);
                                AdditionsChanged = true;
                            }

                            if (Settings.NoDart != 255) {
                                if (Emulator.Memory.MenuSubTypes == MenuSubTypes.Replace) {
                                    Settings.NoDart = (byte) Emulator.Memory.PartySlot[0];
                                } else if (Emulator.Memory.MenuSubTypes == MenuSubTypes.FirstMenu) {
                                    Emulator.Memory.MenuUnlock = 1;
                                    Emulator.Memory.PartySlot[0] = Settings.NoDart;
                                }
                            }

                            break;

                        case GameState.BattleResult:
                            BattleSetup = false;
                            if (!ItemsChanged) {
                                Field.ItemSetup(LoDDict);
                                ItemsChanged = true;
                            }
                            break;

                        case GameState.ChangeDisc:
                            ItemsChanged = false;
                            AdditionsChanged = false;
                            ShopChanged = false;
                            BattleSetup = false;
                            ShopFix = true;
                            HPCapSet = false;
                            break;

                        case GameState.Shop:
                            BattleSetup = false;
                            if (!ItemsChanged) {
                                Field.ItemSetup(LoDDict);
                                ItemsChanged = true;
                            }

                            if (ShopFix) {
                                ShopChanged = false;
                                ShopFix = false;
                                Shop.ContentChange(LoDDict);
                            }

                            if (!ShopChanged) {
                                if (Settings.ShopChange) {
                                    Shop.TableChange(LoDDict);
                                }
                                ShopChanged = true;
                            }
                            break;
                    }

                    GreenButton.Run(uiControl);

                    if (Settings.KillBGM) {
                        KillBGM.Run(uiControl);
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
