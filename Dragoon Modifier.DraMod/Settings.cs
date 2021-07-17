using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod {
    public static class Settings {
        public static bool KillBGM = false;
        public static bool AutoTransform = true;
        public static bool SaveAnywhere = false;
        public static bool AutoCharmPotion = false;
        public static bool RemoveDamageCaps = false;
        public static bool MonsterHPAsNames = false;
        public static bool NeverGuard = false;
        public static bool NoDecaySoulEater = false;

        public static byte NoDart = 255;
        public static byte FlowerStorm = 0;
        public static byte KillBGMMode = 0;
        public static byte AspectRatioMode = 0;

        public static string Mod = "US_Base";

        public static bool ItemStatChange = true;
        public static bool ItemIconChange = true;
        public static bool ItemNameDescChange = true;
        public static bool MonsterStatChange = true;

        public static double HPMulti = 1;
        public static double ATMulti = 1;
        public static double MATMulti = 1;
        public static double DFMulti = 1;
        public static double MDFMulti = 1;
        public static double SPDMulti = 1;
    }
}
