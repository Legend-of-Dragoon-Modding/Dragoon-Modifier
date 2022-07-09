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

        internal static void Run() {
            while (Constants.Run) {
                try {
                    switch (Emulator.Memory.GameState) {
                        case GameState.Battle:
                            if (!BattleSetup) {
                                if (Settings.Instance.NoDart > 0) {
                                    Emulator.Memory.PartySlot[0] = 0;
                                }
                                Battle.Setup();
                                BattleSetup = true;
                                AdditionsChanged = false;
                                ItemsChanged = false;
                                ShopChanged = false;
                                HPCapSet = false;
                            }
                            Battle.Run();
                            break;

                        case GameState.Field:
                            BattleSetup = false;

                            if (!ShopChanged) {
                                Constants.UIControl.ResetBattle();
                                if (Settings.Instance.ShopChange) {
                                    Shop.TableChange();
                                }
                                ShopChanged = true;
                            }

                            if (Settings.Instance.NoDart != 255 && Emulator.Memory.PartySlot[0] != 0) {
                                Settings.Instance.NoDart = (byte) Emulator.Memory.PartySlot[0];
                                Emulator.Memory.PartySlot[0] = 0;
                            }
                            

                            if (!HPCapSet) {
                                HPCapSet = true;
                                if (Settings.Instance.RemoveHPCap) {
                                    Emulator.Memory.FieldHPCap = 30000;
                                }
                            }

                            Field.Run();
                            break;

                        case GameState.Overworld:
                            BattleSetup = false;
                            ShopChanged = false;

                            if (!HPCapSet) {
                                HPCapSet = true;
                                if (Settings.Instance.RemoveHPCap) {
                                    Emulator.Memory.FieldHPCap = 30000;
                                }
                            }
                            break;

                        case GameState.Menu:
                            BattleSetup = false;

                            if (!ItemsChanged) {
                                Field.ItemSetup();
                                ItemsChanged = true;
                            }

                            if (!AdditionsChanged) {
                                Field.AdditionSetup();
                                AdditionsChanged = true;
                            }

                            if (Settings.Instance.NoDart != 255) {
                                if (Emulator.Memory.MenuSubTypes == MenuSubTypes.Replace) {
                                    Settings.Instance.NoDart = (byte) Emulator.Memory.PartySlot[0];
                                } else if (Emulator.Memory.MenuSubTypes == MenuSubTypes.FirstMenu) {
                                    Emulator.Memory.MenuUnlock = 1;
                                    Emulator.Memory.PartySlot[0] = Settings.Instance.NoDart;
                                }
                            }

                            break;

                        case GameState.BattleResult:
                            BattleSetup = false;
                            if (!ItemsChanged) {
                                Field.ItemSetup();
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
                                Field.ItemSetup();
                                ItemsChanged = true;
                            }

                            if (ShopFix) {
                                ShopChanged = false;
                                ShopFix = false;
                                Shop.ContentChange();
                            }

                            if (!ShopChanged) {
                                if (Settings.Instance.ShopChange) {
                                    Shop.TableChange();
                                }
                                ShopChanged = true;
                            }
                            break;
                    }

                    GreenButton.Run();

                    if (Settings.Instance.KillBGM) {
                        KillBGM.Run();
                    }

                    Thread.Sleep(Settings.Instance.LoopDelay);
                } catch (Exception ex) {
                    Constants.Run = false;
                    Constants.UIControl.WriteGLog("Program stopped.");
                    Constants.UIControl.WritePLog("INTERNAL SCRIPT ERROR");
                    Console.WriteLine("INTERNAL SCRIPT ERROR\nFatal Error. Closing all threads.");
                    Console.WriteLine(ex.ToString());
                }

            }
        }
    }
}
