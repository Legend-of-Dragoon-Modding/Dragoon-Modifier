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
        public static string Difficulty = "Normal";

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
        public static bool RGBBattleUI = false;
        public static byte NoDart = 0;
        public static byte FlowerStorm = 0;
        public static byte AspectRatioMode = 0;
        public static byte AdvancedCameraMode = 0;

        //Field & Battle
        public static bool AddPartyMembers = false;
        public static bool AddSoloPartyMembers = false;
        public static bool AlwaysAddSoloPartyMembers = false;
        public static bool SwitchSlot1 = false;
        public static byte Slot1Select = 0;
        public static bool SoloMode = false;
        public static bool DuoMode = false;
        public static byte SwitchEXPSlot1 = 0;
        public static byte SwitchEXPSlot2 = 0;
        public static bool ReduceSoloDuoEXP = false;
        public static byte SoloLeader = 0;
        public static bool KillBGM = false;
        public static byte KillBGMMode = 0;

        //Hard/Hell Mode
        public static bool DualDifficulty = false;

        //Battle Rows

        //Turn Battle System

        //Green Buttons
        public static bool BtnAddPartyMembers = false;
        public static bool BtnSwitchExp = false;

        //Other
        public static string Mod = "US_Base";

        public static bool ItemStatChange = false;
        public static bool ItemIconChange = false;
        public static bool ItemNameDescChange = false;
        public static bool MonsterStatChange = false;
        public static bool CharacterStatChange = false;
        public static bool AdditionChange = true;
        public static bool ShopChange = true;
        public static bool MonsterDropChange = false;
        public static bool MonsterExpGoldChange = false;
        public static bool DragoonStatChange = false;
        public static bool DragoonSpellChange = false;
        public static bool DragoonAdditionChange = false;
        public static bool DragoonDescriptionChange = false;
        public static bool RemoveHPCap = true;

        public static int LoopDelay = 250;
        public static int WaitDelay = 50;

        private static Dataset.ILoDDictionary _dataset = null;

        public static Dataset.ILoDDictionary Dataset {
            get {
                if (_dataset == null) {
                    throw new Exception(); // TODO handeling for non-initialized LoDDict. Probably never happens. Can be lazy loaded?
                }
                return _dataset;
            }
        }

        public static void LoadDataset(string cwd, string mod) {
            Mod = mod;
            _dataset = new Dataset.LoDDictionary(cwd, Mod);
        }
    }
}
