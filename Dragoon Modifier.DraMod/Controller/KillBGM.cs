using Dragoon_Modifier.Core;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Controller {
    internal static class KillBGM {

        static Core.GameState previousState = Core.GameState.None;
        static int musicKillCount = 0;
        static ArrayList musicSSsq = new ArrayList(); 

        public static void Run(UI.IUIControl uiControl) {
            if (previousState != Emulator.Memory.GameState) {
                switch (Emulator.Memory.GameState) {
                    case GameState.Field:
                        if (Settings.KillBGMMode == 0 || Settings.KillBGMMode == 2) {
                            KillMusic();
                            musicKillCount = 10;
                            musicSSsq = new ArrayList();
                        }
                        break;
                    case GameState.Menu:
                        if (Settings.KillBGMMode == 0 || Settings.KillBGMMode == 2) {
                            KillMusic();
                            musicKillCount = 8;
                            musicSSsq = new ArrayList();
                        }
                        break;
                    case GameState.Battle:
                        if (Settings.KillBGMMode == 1 || Settings.KillBGMMode == 2) {
                            KillMusic();
                            musicKillCount = 20;
                            musicSSsq = new ArrayList();
                        }
                        break;
                }
            } else {
                if (Emulator.Memory.GameState == GameState.Field && musicKillCount > 0) {
                    KillMusic();
                    musicKillCount--;
                }
            }

            previousState = Emulator.Memory.GameState;
        }

        public static void KillMusic() {
            List<long> bgmScan = Emulator.DirectAccess.ScanAoB(0xA8660, 0x2A865F, "53 53 73 71");
            foreach (var address in bgmScan) {
                musicSSsq.Add(address);
                for (int i = 0; i <= 255; i++) {
                    Emulator.DirectAccess.WriteByte((long) address + i, (byte) 0);
                    //Thread.Sleep(10);
                }
            }

            foreach (var address in musicSSsq) {
                for (int i = 0; i <= 255; i++) {
                    Emulator.DirectAccess.WriteByte((long) address + i, (byte) 0);
                    //Thread.Sleep(10);
                }
            }
            Console.WriteLine("Killed music.");
        }
    }
}
