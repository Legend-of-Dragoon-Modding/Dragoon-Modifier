using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier {
    public class Globals {
        public static ushort HOTKEY = 0;
        public static byte DISC = 1;
        public static byte CHAPTER = 0;
        public static int BATTLE_VALUE = 0;
        public static ushort ENCOUNTER_ID = 0;
        public static ushort MAP = 0;
        public static byte[] PARTY_SLOT = new byte[3];
        public static byte DRAGOON_SPIRITS = 0;
        public static bool IN_BATTLE = false;
        public static bool STATS_CHANGED = false;
        public static int M_POINT = 0;
        public static int C_POINT = 0;
        public static byte MONSTER_SIZE = 0;
        public static byte UNIQUE_MONSTERS = 0;
        public static ushort[] MONSTER_IDS = new ushort[5];
    }
}
