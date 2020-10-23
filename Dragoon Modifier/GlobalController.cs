using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Dragoon_Modifier {
    class GlobalController {
        public static void Run(Emulator emulator) {
            while (Constants.RUN) {
                ushort battleValue = emulator.ReadShort("BATTLE_VALUE");
                Globals.BATTLE_VALUE = battleValue;
                byte menu = emulator.ReadByte("MENU");
                if (menu == 4) {                // Menu
                    Globals.GAME_STATE = 2;
                } else if (menu == 9) {         // Shop
                    Globals.GAME_STATE = 3;
                } else if (menu == 14) {        // Loading screen
                    Globals.GAME_STATE = 4;
                } else if (menu == 19) {        // End of disc
                    Globals.GAME_STATE = 5;
                } else if (menu == 24) {        // Replace prompt
                    Globals.GAME_STATE = 6;
                } else if (menu == 29) {
                    Globals.GAME_STATE = 7;     // Battle result screen
                } else if (battleValue == 41215) {
                    Globals.GAME_STATE = 1;     // Battle
                } else if (menu == 0) {
                    Globals.GAME_STATE = 0;     // Field
                }
                Globals.DISC = Emulator2.ReadByte("DISC");
                Globals.CHAPTER = (byte)(Emulator2.ReadByte("CHAPTER") + 1);
                Globals.ENCOUNTER_ID = emulator.ReadShort("ENCOUNTER_ID");
                Globals.MAP = Emulator2.ReadUShort("MAP");
                Globals.PARTY_SLOT[0] = Emulator2.ReadByte("PARTY_SLOT");
                Globals.PARTY_SLOT[1] = Emulator2.ReadByte("PARTY_SLOT", 4);
                Globals.PARTY_SLOT[2] = Emulator2.ReadByte("PARTY_SLOT", 8);
                Globals.DRAGOON_SPIRITS = Emulator2.ReadByte("DRAGOON_SPIRITS");
                Globals.HOTKEY = Emulator2.ReadUShort("HOTKEY");
                Thread.Sleep(100);
            }
        }

    }
}
