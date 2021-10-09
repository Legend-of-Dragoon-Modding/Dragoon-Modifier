using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Controller {
    internal static class KillBGM {

        static Emulator.GameState previousState = Emulator.GameState.None;
        static int musicKillCount = 0;

        public static void Run(Emulator.IEmulator emulator, UI.IUIControl uiControl) {
            Console.WriteLine("Current: " + emulator.Memory.GameState + " | Before: " + previousState);
            if (previousState != emulator.Memory.GameState) {
                switch (emulator.Memory.GameState) {
                    case Emulator.GameState.Field:
                        if (Settings.KillBGMMode == 0 || Settings.KillBGMMode == 2) {
                            KillMusic(emulator);
                        }
                        break;
                    case Emulator.GameState.Menu:
                        if (Settings.KillBGMMode == 0 || Settings.KillBGMMode == 2) {
                            KillMusic(emulator);
                            musicKillCount = 8;
                        }
                        break;
                    case Emulator.GameState.Battle:
                        if (Settings.KillBGMMode == 1 || Settings.KillBGMMode == 2) {
                            KillMusic(emulator);
                            musicKillCount = 20;
                        }
                        break;
                }
            } else {
                if (emulator.Memory.GameState == Emulator.GameState.Field && musicKillCount > 0) {
                    KillMusic(emulator);
                    musicKillCount--;
                }
            }

            previousState = emulator.Memory.GameState;
        }

        public static void KillMusic(Emulator.IEmulator emulator) {
            List<long> bgmScan = emulator.ScanAoB(0xA8660, 0x2A865F, "53 53 73 71");
            foreach (var address in bgmScan) {
                for (int i = 0; i <= 255; i++) {
                    emulator.WriteByte((long) address + i, (byte) 0);
                    //Thread.Sleep(10);
                }
            }
            Console.WriteLine("Killed music.");
        }
    }
}
