using Dragoon_Modifier.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.UI {
    public class UltimateBoss {

        private static ushort[] bossEncounter = { 487, 386, 414, 461, 412, 413, 387, 415, 449, 402, 403, 417, 418, 448, 416, 422, 423, 432, 430, 433, 431, 447, 408, 389, 396, 399, 409, 393, 398, 397, 400, 410, 401, 390, 411, 394, 392, 420, 442, 443 };
        private static byte[] bossField = { 10, 3, 8, 21, 16, 70, 5, 12, 68, 23, 29, 31, 41, 68, 38, 42, 47, 69, 67, 56, 54, 68, 12, 21, 30, 72, 27, 14, 73, 35, 76, 37, 77, 22, 24, 40, 45, 44, 71, 65 };

        public static void Setup(int boss) {
            if (Constants.UltimateBossCompleted >= boss && Settings.Instance.MonsterStatChange) {
                bool pass = false;
                if (Emulator.Memory.Chapter == 4 && Emulator.Memory.Transition == 12) {
                    if ((boss >= 0 && boss <= 2) && (Emulator.Memory.MapID >= 393 && Emulator.Memory.MapID <= 394)) {
                        pass = true;
                    } else { 
                        if (boss >= 0 && boss <= 2)
                            Constants.UIControl.WriteGLog("You must be in Zone 1 of Forbidden Land (393 - 394)");
                    }

                    if ((boss >= 3 && boss <= 7) && (Emulator.Memory.MapID >= 395 && Emulator.Memory.MapID <= 397)) {
                        pass = true;
                    } else {
                        if (boss >= 3 && boss <= 7)
                            Constants.UIControl.WriteGLog("You must be in Zone 2 of Forbidden Land (395 - 397)");
                    }

                    if ((boss >= 8 && boss <= 21) && (Emulator.Memory.MapID >= 398 && Emulator.Memory.MapID <= 400)) {
                        pass = true;
                    } else {
                        if (boss >= 8 && boss <= 21)
                            Constants.UIControl.WriteGLog("You must be in Zone 3 of Forbidden Land (398 - 400)");
                    }

                    if (boss >= 22 && (Emulator.Memory.MapID >= 401 && Emulator.Memory.MapID <= 405)) {
                        pass = true;
                    } else {
                        if (boss >= 22)
                            Constants.UIControl.WriteGLog("You must be in Zone 4 of Forbidden Land (401 - 405)");
                    }
                } else {
                    Constants.UIControl.WriteGLog("You must have completed Chapter 3 and on field.");
                }

                if (pass) {
                    Emulator.Memory.Transition = 19;
                    Emulator.Memory.EncounterID = bossEncounter[boss];
                    Emulator.Memory.BattleField = bossField[boss];
                    Settings.Instance.UltimateBoss = true;
                    Constants.UIControl.WriteGLog("Starting Ultimate Boss...");
                }
            } else {
                Constants.UIControl.WriteGLog($"You need to complete a previous Ultimate Boss. Bosses completed {Constants.UltimateBossCompleted}.");
                if (!Settings.Instance.MonsterStatChange)
                    Constants.UIControl.WritePLog("Monster Stat Change is not turned on for Ultimate Boss.");
            }
        }
    }
}
