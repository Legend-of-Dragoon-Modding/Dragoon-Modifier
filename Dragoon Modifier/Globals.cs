using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier {
    public class Globals {
        public static ushort HOTKEY = 0;
        public static long CURRENT_TIME = 0;
        public static long LAST_HOTKEY = 0;
        public static byte DISC = 1;
        public static byte CHAPTER = 0;
        public static int BATTLE_VALUE = 0;
        public static ushort ENCOUNTER_ID = 0;
        public static ushort MAP = 0;
        public static ushort BEFORE_BATTLE_MAP = 0;
        public static byte[] PARTY_SLOT = new byte[3];
        public static byte DRAGOON_SPIRITS = 0;
        public static bool IN_BATTLE = false;
        public static bool STATS_CHANGED = false;
        public static byte EXITING_BATTLE = 0;
        public static long M_POINT = 0;
        public static long C_POINT = 0;
        public static long[] CHAR_ADDRESS = new long[3];
        public static long[] MONS_ADDRESS = new long[5];
        public static byte MONSTER_SIZE = 0;
        public static byte UNIQUE_MONSTERS = 0;
        public static List<int> MONSTER_IDS = new List<int>();
        public static List<int> UNIQUE_MONSTER_IDS = new List<int>();
        public static List<dynamic> MONSTER_TABLE = new List<dynamic>();
        public static List<dynamic> CHARACTER_TABLE = new List<dynamic>();
        public static List<dynamic> DRAGOON_SPELLS = new List<dynamic>();
        public static dynamic[] CURRENT_STATS = new dynamic[9];
        public static dynamic DICTIONARY = new System.Dynamic.ExpandoObject();
        public static bool MONSTER_CHANGE = false;
        public static bool ULTIMATE = false;
        public static bool DROP_CHANGE = false;
        public static bool ITEM_CHANGE = false;
        public static bool CHARACTER_CHANGE = false;
        public static bool DRAGOON_CHANGE = false;
        public static bool DRAGOON_ADDITION_CHANGE = false;
        public static bool ADDITION_CHANGE = false;
        public static bool SHOP_CHANGE = false;
        public static Nullable<int> NO_DART = null;
        public static string MOD = "US_Base";
        public static string DIFFICULTY_MODE = "Normal";
        public static double HP_MULTI = 1;
        public static double AT_MULTI = 1;
        public static double MAT_MULTI = 1;
        public static double DF_MULTI = 1;
        public static double MDF_MULTI = 1;
        public static double SPD_MULTI = 1;
        public static int HASCHEL = 0;
        public static Dictionary<string, bool> dmScripts = new Dictionary<string, bool>();
        public static string[] MONSTER_NAME = new string[5];
        public static string[] CHARACTER_NAME = new string[3];

        public static void SetM_POINT(long mPoint) {
            M_POINT = mPoint;
            for (int i = 0; i < 5; i++) {
                MONS_ADDRESS[i] = mPoint - (i * 0x388);
            }
        }

        public static void SetC_POINT(long cPoint) {
            C_POINT = cPoint;
            for (int i = 0; i < 3; i++) {
                CHAR_ADDRESS[i] = cPoint - (i * 0x388);
            }

        }
        public static bool CheckDMScript(string name) {
            if (Globals.dmScripts.ContainsKey(name) && Globals.dmScripts[name])
                return true;
            else
                return false;
        }
    }
}
