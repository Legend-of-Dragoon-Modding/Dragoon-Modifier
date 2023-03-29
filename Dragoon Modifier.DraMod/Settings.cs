using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod {
    public sealed class Settings {

        private static Settings? _instance = null;

        public static Settings Instance {
            get {
                if (_instance == null) {
                    _instance = new();
                }
                return _instance;
            }
        }

        //Difficulty
        public double HPMulti = 1;
        public double ATMulti = 1;
        public double MATMulti = 1;
        public double DFMulti = 1;
        public double MDFMulti = 1;
        public double SPDMulti = 1;
        public string Difficulty = "Normal";

        //Field
        public bool SaveAnywhere = false;
        public bool AutoCharmPotion = false;
        public bool IncreaseTextSpeed = false;
        public bool AutoAdvanceText = false;

        //Battle
        public bool AutoTransform = true;
        public bool RemoveDamageCaps = false;
        public bool ElementalBomb = false; 
        public bool MonsterHPAsNames = false;
        public bool NeverGuard = false;
        public bool NoDecaySoulEater = false;
        public bool EnrageBossOnly = false;
        public bool EnrageMode = false;
        public bool DamageTracker = false;
        public bool NoDragoon = false;
        public bool EarlyAdditions = false;
        public bool AdditionLevel = false;
        public bool AspectRatio = false;
        public bool RGBBattleUI = false;
        public bool RGBBattleUICharacter = false;
        public bool RGBBattleUICycle = false;
        public byte[] RGBBattleUIColour = new byte[3];
        public byte NoDart = 0;
        public byte FlowerStorm = 0;
        public byte AspectRatioMode = 0;
        public byte AdvancedCameraMode = 0;

        //Field & Battle
        public bool AddPartyMembers = false;
        public bool AddSoloPartyMembers = false;
        public bool AlwaysAddSoloPartyMembers = false;
        public bool SwitchSlot1 = false;
        public byte Slot1Select = 0;
        public bool SoloMode = false;
        public bool DuoMode = false;
        public byte SwitchEXPSlot1 = 0;
        public byte SwitchEXPSlot2 = 0;
        public bool ReduceSoloDuoEXP = false;
        public byte SoloLeader = 0;
        public bool KillBGM = false;
        public byte KillBGMMode = 0;

        //Hard/Hell Mode
        public bool DualDifficulty = false;
        public bool UltimateBoss = false;
        public byte UltimateBossSelected = 0;

        //Battle Rows
        public bool BattleRows = false;
        public byte[] BattleRowsFormation = new byte[9];
        public byte[] BattleRowsBoost = new byte[9];

        //Turn Battle System
        public bool EATB = false;
        public bool ATB = false;
        public bool QTB = false;
        public int QTBLeader = 0;
        public int QTBCount = 2;
        public bool QTBUsedDuringEnemyTurn = false;

        //Green Buttons
        public bool BtnAddPartyMembers = false;
        public bool BtnSwitchExp = false;

        //Other
        public Preset Preset = Preset.Normal;
        public string CustomMod = "US_Base_Enhanced";

        public bool ItemStatChange = false;
        public bool ItemIconChange = false;
        public bool ItemNameDescChange = false;
        public bool MonsterStatChange = false;
        public bool CharacterStatChange = false;
        public bool AdditionChange = false;
        public bool ShopChange = false;
        public bool MonsterDropChange = false;
        public bool MonsterExpGoldChange = false;
        public bool DragoonStatChange = false;
        public bool DragoonSpellChange = false;
        public bool DragoonAdditionChange = false;
        public bool DragoonDescriptionChange = false;
        public bool RemoveHPCap = false;

        public int LoopDelay = 20;
        public int WaitDelay = 50;

        private Dataset.ILoDDictionary? _dataset = null;

        public Dataset.ILoDDictionary Dataset {
            get {
                if (_dataset == null) {
                    throw new Exception(); // TODO handeling for non-initialized LoDDict. Probably never happens. Can be lazy loaded?
                }
                return _dataset;
            }
        }

        public void LoadDataset(string cwd, string mod) {
            _dataset = new Dataset.LoDDictionary(cwd, mod);
        }

        public void LoadDataset(string cwd, string mod, string? dualMod, Dataset.Scripts.IScript script) {
            _dataset = new Dataset.LoDDictionary(cwd, mod, dualMod, script);
        }
    }
}
