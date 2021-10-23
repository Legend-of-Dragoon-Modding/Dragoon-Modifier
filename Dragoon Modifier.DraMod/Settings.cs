using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod {
    public static class Settings {
        //Difficulty
        public static double HPMulti = 1;
        public static double ATMulti = 1;
        public static double MATMulti = 1;
        public static double DFMulti = 1;
        public static double MDFMulti = 1;
        public static double SPDMulti = 1;

        //Field
        public static bool SaveAnywhere = false;
        public static bool AutoCharmPotion = false;
        public static bool IncreaseTextSpeed = false;
        public static bool AutoAdvanceText = false;

        //Battle
        public static bool AutoTransform = true;
        public static bool RemoveDamageCaps = false;
        public static bool ElementalBomb = false; 
        public static bool MonsterHPAsNames = false;
        public static bool NeverGuard = false;
        public static bool NoDecaySoulEater = false;
        public static bool EnrageBossOnly = false;
        public static bool EnrageMode = false;
        public static bool DamageTracker = false;
        public static bool NoDragoon = false;
        public static bool EarlyAdditions = false;
        public static bool AdditionLevel = false;
        public static bool SaveHP = false;
        public static bool AspectRatio = false;
        public static byte NoDart = 255;
        public static byte FlowerStorm = 0;
        public static byte AspectRatioMode = 0;
        public static byte AdvancedCameraMode = 0; 

        //Field & Battle
        public static bool KillBGM = false;
        public static byte KillBGMMode = 0;

        //Hard/Hell Mode
        public static bool DualDifficulty = false;

        //Battle Rows

        //Turn Battle System

        //Other
        public static string Mod = "US_Base";

        public static bool ItemStatChange = true;
        public static bool ItemIconChange = true;
        public static bool ItemNameDescChange = true;
        public static bool MonsterStatChange = true;
        public static bool CharacterStatChange = true;
    }
}
