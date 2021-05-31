using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Dragoon_Modifier {
    class GlobalController {
        public static void Run() {
            while (Constants.RUN) {
                ushort battleValue = Emulator.ReadUShort("BATTLE_VALUE");
                Globals.BATTLE_VALUE = battleValue;
                Globals.DISC = Emulator.ReadByte("DISC");
                Globals.CHAPTER = (byte) (Emulator.ReadByte("CHAPTER") + 1);
                Globals.ENCOUNTER_ID = Emulator.ReadShort("ENCOUNTER_ID");
                Globals.MAP = Emulator.ReadShort("MAP");
                Globals.PARTY_SLOT[0] = Emulator.ReadByte("PARTY_SLOT");
                Globals.PARTY_SLOT[1] = Emulator.ReadByte("PARTY_SLOT", 4);
                Globals.PARTY_SLOT[2] = Emulator.ReadByte("PARTY_SLOT", 8);
                Globals.DRAGOON_SPIRITS = Emulator.ReadByte("DRAGOON_SPIRITS");
                Globals.HOTKEY = Emulator.ReadUShort("HOTKEY");
                Thread.Sleep(100);
            }
        }
    }
}
