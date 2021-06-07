using System;
using System.Windows;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Windows.Threading;
using System.Windows.Media;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.Globalization;
using System.ComponentModel;
using Mono.CSharp;
using Microsoft.Win32;
using System.Windows.Input;
using static Dragoon_Modifier.ReaderWindow;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using Dragoon_Modifier.Properties;
using Xceed.Wpf.AvalonDock.Properties;
using System.Reflection;
using ControlzEx.Standard;
using System.Text.Json;
using System.Net;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Dragoon_Modifier.Core;

namespace Dragoon_Modifier {
    public partial class MainWindow {
        #region Variables
        #region Program Variables
        public Thread globalThread, newBattleThread, fieldThread, battleThread, hotkeyThread, otherThread, ultimateThread;
        public string preset = "";
        public bool presetHotkeys = true;
        public bool dualDifficultySwitch = false;
        public static Dictionary<string, int> uiCombo = new Dictionary<string, int>();

        public TextBlock[,] monsterDisplay = new TextBlock[5, 6];
        public TextBlock[,] characterDisplay = new TextBlock[3, 9];
        public Button[] difficultyButton = new Button[3];
        public ProgressBar[] progressMATB = new ProgressBar[5];
        public ProgressBar[] progressCATB = new ProgressBar[3];
        public ReaderWindow readerWindow = new ReaderWindow();
        long oldOffset = 0;
        int currentIconState = 0;
        #endregion

        #region Script Variables
        //General
        public bool keepStats = false;
        public double[,] originalCharacterStats = new double[3, 10];
        public double[,] originalMonsterStats = new double[5, 6];
        public bool stopSave = false;
        //Shop Changes
        static bool SHOP_CHANGED = true;
        static bool SHOP_DISC_SWAP = false;
        static int[] SHOP_MAPS = new int[] { 16, 23, 83, 84, 122, 145, 175, 180, 193, 204, 211, 214, 247,
        287, 309, 329, 332, 349, 357, 384, 435, 479, 515, 530, 564, 619, 624}; // Some maps missing??
        //Icon Changes
        public bool wroteIcons = false;
        //Solo/Duo Mode
        public bool addSoloPartyMembers = false;
        public bool alwaysAddSoloPartyMembers = false;
        public bool soloModeOnBattleEntry = false;
        public bool duoModeOnBattleEntry = false;
        //Dragoon Changes
        public ushort[] currentMP = { 0, 0, 0 };
        public ushort[] previousMP = { 0, 0, 0 };
        public ushort recoveryRateSave = 0;
        public bool burnActive = false;
        public bool dragoonChangesOnBattleEntry = false;
        public bool checkFlowerStorm = false;
        public ushort checkRoseDamageSave = 0;
        public bool checkRoseDamage = false;
        public bool roseEnhanceDragoon = false;
        public byte starChildren = 0;
        public bool trackRainbowBreath = false;
        //HP Cap Break
        public double[] hpChangeCheck = { 65535, 65535, 65535 };
        public byte[] hpChangeSlot = { 255, 255, 255 };
        public ushort[] hpChangeSave = { 0, 0, 0 };
        public bool hpCapBreakOnBattleEntry = false;
        public bool maxHPTableLoaded = false;
        public ushort[,] maxHPTable = new ushort[9, 60];
        //Kill BGM
        public bool killBGMField = false;
        public bool killBGMBattle = false;
        public bool killedBGMField = false;
        public bool killedBGMBattle = false;
        public bool reKilledBGMField = false;
        public byte saveMusicSpeed = 0;
        //Elemental Bomb
        public byte eleBombTurns = 0;
        public byte eleBombItemUsed = 0;
        public byte eleBombSlot = 0;
        public byte eleBombElement = 0;
        public bool eleBombChange = false;
        public ushort[] eleBombOldElement = { 0, 0, 0, 0, 0 };
        //Addition Changes
        public bool damageIncreaseOnBattleEntry = false;
        //No HP Decay Soul Eater
        public bool noHPDecayOnBattleEntry = false;
        //Equip Changes
        public bool equipChangesOnFieldEntry = false;
        public bool equipChangesOnBattleEntry = false;
        public int[] guardStatusDF = new int[3];
        public int[] guardStatusMDF = new int[3];
        public int[] lGuardStatusDF = new int[3];
        public int[] lGuardStatusMDF = new int[3];
        public bool[] lGuardStateDF = new bool[3];
        public bool[] lGuardStateMDF = new bool[3];
        public bool[] sGuardStatusDF = new bool[3];
        public bool[] sGuardStatusMDF = new bool[3];
        //Reader Mode
        public bool readerRemoveUIOnBattleEntry = false;
        public bool partyMenu = false;
        public bool partySecondMenu = false;
        public bool readerModeOnBattleExit = false;
        //Ultimate Boss
        public int[] ultimateHP = new int[5];
        public int[] ultimateHPSave = new int[5];
        public int[] ultimateMaxHP = new int[5];
        public int ultimateBossCompleted = 0;
        public int inventorySize = 32;
        public bool ultimateBossOnBattleEntry = false;
        public bool ubHPChanged = false;
        public bool ubCheckedDamage = false;
        public bool ubUltimateHPSet = false;
        public int ubCheckDamageCycle = 0;
        public bool ubGuardBreak = false;
        public bool ubHealingPotion = false;
        public bool ubZeroSP = false;
        public bool ubMPAttack = false;
        public int[] ubMPAttackTrk = new int[3];
        public bool ubWoundDamage = false;
        public ushort[] ubWHP = new ushort[3];
        public ushort[] ubWMHP = new ushort[3];
        public bool ubHealthSteal = false;
        public bool ubHealthStealSave = false;
        public ushort ubHealthStealDamage = 0;
        public bool ubSPAttack = false;
        public bool ubMoveChange = false;
        public ushort[] ubMoveChgTrn = new ushort[5];
        public bool ubMoveChgSet = false;
        public bool ubMagicChange = false;
        public double magicChangeTurns = 0;
        public bool ubElementalShift = false;
        public bool ubArmorShell = false;
        public ushort ubHeartHPSave = 0;
        public byte ubArmorShellTurns = 0;
        public ushort ubArmorShellTP = 0;
        public bool ubSharedHP = false;
        public bool ubRemoveResistances = false;
        public bool ubTPDamage = false;
        public short tpDamage = 0;
        public bool ubTrackHPChange = false;
        public ushort[] ubTrackHP = new ushort[3];
        public int[] ubTrackEHP = new int[5];
        public ushort[] ubTrackTP = new ushort[3];
        public ushort[] ubTrackMTP = new ushort[5];
        public bool ubBodyDamage = false;
        public bool ubVirageKilledPart = false;
        public int ubDragoonBondMode = 0;
        public bool ubDragoonBond = false;
        public bool ubDragoonExtras = false;
        public int[] ubCustomStatusTurns = new int[3];
        public bool ubBlockMenuHPTrack = false;
        public bool ubBlockMenuTPTrack = false;
        public byte ubTrackDMove = 0;
        public ushort ubTrackTM = 0;
        public ushort ubElectricCharges = 0;
        public ushort ubElectricUnleash = 0;
        public bool ubTrackDragoon = false;
        public bool ubCountdown = false;
        public byte ubLivesIncreased = 0;
        public int ubTotalGold = 0;
        public bool ubUltimateEnrage = false;
        public byte enrageChangeTurns = 0;
        public byte enrageChangeIndex = 0;
        public double ubEnrageTurns = 0;
        public double ubEnrageTurnsPlus = 0;
        public bool ubInventoryRefresh = false;
        public byte inventoryRefreshSize = 0;
        public byte[] inventoryRefresh = new byte[64];
        public int inventoryRefreshStage = 0;
        public bool ubEnhancedShield = false;
        public short shieldTurnsTaken = 0;
        public short enhancedShieldTurns = 0;
        public bool ubBodyProtect = false;
        public bool ubFinalAttack = false;
        public bool ubReverseDBS = false;
        public bool ubArmorGuard = false;
        public bool ubDragoonGuard = false;
        public bool ubGrantMaxHP = false;
        public byte[] deathRes = new byte[3];
        public byte ubZiegDragoon = 82;
        public long ultimateShopLimited = 0;
        public int doubleRepeatUsed = 0;
        public bool doubleRepeatOnBattleEntry = false;
        public int magicShieldTurns = 0;
        public int magicShieldSlot = 0;
        public int materialShieldTurns = 0;
        public int materialShieldSlot = 0;
        public int sigStoneTurns = 0;
        public int sigStoneSlot = 0;
        public int pandemoniumTurns = 0;
        public int pandemoniumSlot = 0;
        public short ultimateBossMap = 0;
        public bool ultimateBossKeepMap = false;
        //Ultimate Boss Equips
        public int soasSiphonSlot = -1;
        public ushort spiritEaterSP = 0;
        public ushort spiritEaterSaveSP = 0;
        public ushort elementArrowItem = 0xC3;
        public ushort elementArrowElement = 0x80;
        public ushort elementArrowLastAction = 0;
        public ushort elementArrowTurns = 0;
        public ushort gloveLastAction = 0;
        public ushort gloveCharge = 0;
        public ushort axeLastAction = 0;
        public bool jeweledHammer = false;
        public bool checkHarpoon = false;
        public bool ubSoasWargod = false;
        public bool ubSoasDragoonBoost = false;
        //Ultimate Boss Shop
        public int[] uShopPrices = {
            70000,   //Spirit Eater
            70000,   //Harpoon
            70000,   //Element Arrow
            70000,   //Dragon Beater
            70000,   //Battery Glove
            70000,   //Jeweled Hammer
            70000,   //Giant Axe
            280000,  //Soa's Light
            30000,   //Fake Legend Casque
            120000,  //Soa's Helm
            30000,   //Fake Legend Armor
            60000,   //Divine DG Armor
            120000,  //Soa's Armor
            40000,   //Lloyd's Boots
            40000,   //Winged Shoes
            120000,  //Soa's Greveas
            20000,   //Heal Ring
            40000,   //Soa's Sash
            50000,   //Soa's Ahnk
            50000,   //Soa's Health Ring
            50000,   //Soa's Mage Ring
            50000,   //Soa's Shield Ring
            50000,   //Soa's Siphon Ring
            100000,  //Power Up
            100000,  //Power Down
            100000,  //Speed Up
            100000,  //Speed Down
            100000,  //Magic Shield
            100000,  //Material Shield
            75000,   //Magic Stone of Signet
            25000,   //Pandemonium
            1000000, //Psychedelic Bomb X
            250000,  //Empty Dragoon Crystal
            500000,  //Soa's Wargod
            500000   //Soa's Dragoon Boost
        };
        public int[] uLimited = {
            1,          //Spirit Eater
            2,          //Harpoon
            4,          //Element Arrow
            8,          //DB2
            16,         //Battery Glove
            32,         //Jeweled Hammer
            64,         //Giant Axe
            128,        //Soa's Light
            0,          //Fake Legend Casque
            256,        //Soa's Helm
            0,          //Fake Legend Armor
            0,          //Divine DG Armor
            512,        //Soa's Armor
            0,          //Lloyd's Boots
            0,          //Winged Shoes
            1024,       //Soa's Greveas
            0,          //Heal Ring
            2048,       //Soa's Sash
            4096,       //Soa's Ahnk
            8192,       //Soa's Health Ring
            16384,      //Soa's Mage Ring
            32768,      //Soa's Shield Ring
            65536,      //Soa's Siphon Ring
            131072,     //Power Up
            262144,     //Power Down
            524288,     //Speed Up
            1048576,    //Speed Down
            2097152,    //Magic Shield
            4194304,    //Material Shield
            8388608,    //Magic Stone of Signet
            16777216,   //Pandemonium
            33554432,   //Psychedelic Bomb X
            67108864,   //Empty Dragon Crystal
            134217728,  //Soa's Wargod
            268435456   //Soa's Dragoon Boost
        };
        public int[] uItemId = {
            159, //Spirit Eater
            160, //Harpoon
            161, //Element Arrow
            162, //Dragon Beater
            163, //Battery Glove
            164, //Jeweled Hammer
            165, //Giant Axe
            166, //Soa's Light
            167, //Fake Legend Casque
            168, //Soa's Helm
            169, //Fake Legend Armor
            170, //Divine DG Armor
            171, //Soa's Armor
            172, //Lloyd's Boots
            173, //Winged Shoes
            174, //Soa's Greveas
            175, //Heal Ring
            176, //Soa's Sash
            177, //Soa's Ahnk
            178, //Soa's Health Ring
            179, //Soa's Mage Ring
            180, //Soa's Shield Ring
            181, //Soa's Siphon Ring
            0,   //Power Up
            0,   //Power Down
            0,   //Speed Up
            0,   //Speed Down
            0,   //Magic Shield
            0,   //Material Shield
            0,   //Magic Stone of Signet
            0,   //Pandemonium
            250, //Psychedelic Bomb X
            0,   //Empty Dragon Crystal
            0,   //Soa's Wargod
            0    //Soa's Dragoon Boost
        };
        //Damage Tracker
        public bool damageTrackerOnBattleEntry = false;
        public int[] dmgTrkHP = new int[5];
        public int[] dmgTrkChr = new int[3];
        public int dmgTrkSlot = 0;
        //Early Additions
        public bool earlyAdditionsOnFieldEntry = false;
        //Enrage Mode
        public bool enrageBoss = false;
        public byte[] enragedMode = { 0, 0, 0, 0, 0 };
        //Battle Formation Rows
        public ComboBox[] battleRow = new ComboBox[9];
        public ComboBox[] battleRowBoost = new ComboBox[9];
        public bool battleRowsOnBattleEntry = false;
        // * Turn Battle
        public bool eatbOnBattleEntry = false;
        public bool qtbOnBattleEntry = false;
        public bool atbOnBattleEntry = false;
        public int[] extraTurnBattleM = new int[5];
        public int[] extraTurnBattleC = new int[3];
        public int timePlayed = 0;
        public int cooldowns = 0;
        public byte qtbTurns = 0;
        public bool qtbUsedDuringEnemyTurn = false;
        public bool qtbLeaderTurn = false;
        public int[] currentHP = new int[3];
        public int[] playerSpeed = new int[3];
        public bool[] eatbSound = new bool[3];
        //Hotkeys
        public int goldQuest = 0;
        public bool faustBattle = false;
        public bool saveFaust = false;
        public int faustCount = 0;
        #endregion
        #endregion

        #region Startup
        public MainWindow() {
            try {
                InitializeComponent();
                Constants.CONSOLE = txtOutput;
                Constants.GLOG = stsGame;
                Constants.PLOG = stsProgram;
                Constants.Init();
                InitUI();
                LoadKey();
                LoDDictionary.Dictionary.Init(Globals.MOD);

                if (!ModVersion.IsCurrent(Constants.VERSION, out var newVersion, out var uri)) {
                    Constants.WriteOutput($"Current version {Constants.VERSION} is outdated. You can download version {newVersion} at {uri}");
                    Constants.WriteGLog($"Newer version ({newVersion}) available.");
                }
                
                if (Constants.EMULATOR_ID != 255) {
                    SetupEmulator(true);
                } else {
                    Constants.WriteOutput("Please pick an emulator to use in the settings menu.");
                }

                SetupScripts();
                if (miOpenPreset.IsChecked) {
                    Constants.LoadPreset(preset);
                    LoadPreset();
                }
            } catch (Exception ex) {
                MessageBox.Show("Error on startup.");
                MessageBox.Show(ex.ToString());
                Application.Current.Shutdown();
            }
        }

        public void InitUI() {
            monsterDisplay[0, 0] = lblEnemy1Name;
            monsterDisplay[0, 1] = lblEnemy1HP;
            monsterDisplay[0, 2] = lblEnemy1ATK;
            monsterDisplay[0, 3] = lblEnemy1DEF;
            monsterDisplay[0, 4] = lblEnemy1SPD;
            monsterDisplay[0, 5] = lblEnemy1TRN;
            monsterDisplay[1, 0] = lblEnemy2Name;
            monsterDisplay[1, 1] = lblEnemy2HP;
            monsterDisplay[1, 2] = lblEnemy2ATK;
            monsterDisplay[1, 3] = lblEnemy2DEF;
            monsterDisplay[1, 4] = lblEnemy2SPD;
            monsterDisplay[1, 5] = lblEnemy2TRN;
            monsterDisplay[2, 0] = lblEnemy3Name;
            monsterDisplay[2, 1] = lblEnemy3HP;
            monsterDisplay[2, 2] = lblEnemy3ATK;
            monsterDisplay[2, 3] = lblEnemy3DEF;
            monsterDisplay[2, 4] = lblEnemy3SPD;
            monsterDisplay[2, 5] = lblEnemy3TRN;
            monsterDisplay[3, 0] = lblEnemy4Name;
            monsterDisplay[3, 1] = lblEnemy4HP;
            monsterDisplay[3, 2] = lblEnemy4ATK;
            monsterDisplay[3, 3] = lblEnemy4DEF;
            monsterDisplay[3, 4] = lblEnemy4SPD;
            monsterDisplay[3, 5] = lblEnemy4TRN;
            monsterDisplay[4, 0] = lblEnemy5Name;
            monsterDisplay[4, 1] = lblEnemy5HP;
            monsterDisplay[4, 2] = lblEnemy5ATK;
            monsterDisplay[4, 3] = lblEnemy5DEF;
            monsterDisplay[4, 4] = lblEnemy5SPD;
            monsterDisplay[4, 5] = lblEnemy5TRN;
            characterDisplay[0, 0] = lblCharacter1Name;
            characterDisplay[0, 1] = lblCharacter1HMP;
            characterDisplay[0, 2] = lblCharacter1ATK;
            characterDisplay[0, 3] = lblCharacter1DEF;
            characterDisplay[0, 4] = lblCharacter1VHIT;
            characterDisplay[0, 5] = lblCharacter1DATK;
            characterDisplay[0, 6] = lblCharacter1DDEF;
            characterDisplay[0, 7] = lblCharacter1SPD;
            characterDisplay[0, 8] = lblCharacter1TRN;
            characterDisplay[1, 0] = lblCharacter2Name;
            characterDisplay[1, 1] = lblCharacter2HMP;
            characterDisplay[1, 2] = lblCharacter2ATK;
            characterDisplay[1, 3] = lblCharacter2DEF;
            characterDisplay[1, 4] = lblCharacter2VHIT;
            characterDisplay[1, 5] = lblCharacter2DATK;
            characterDisplay[1, 6] = lblCharacter2DDEF;
            characterDisplay[1, 7] = lblCharacter2SPD;
            characterDisplay[1, 8] = lblCharacter2TRN;
            characterDisplay[2, 0] = lblCharacter3Name;
            characterDisplay[2, 1] = lblCharacter3HMP;
            characterDisplay[2, 2] = lblCharacter3ATK;
            characterDisplay[2, 3] = lblCharacter3DEF;
            characterDisplay[2, 4] = lblCharacter3VHIT;
            characterDisplay[2, 5] = lblCharacter3DATK;
            characterDisplay[2, 6] = lblCharacter3DDEF;
            characterDisplay[2, 7] = lblCharacter3SPD;
            characterDisplay[2, 8] = lblCharacter3TRN;

            difficultyButton[0] = btnNormal;
            difficultyButton[1] = btnHard;
            difficultyButton[2] = btnHell;

            progressCATB[0] = pgrEATBC1;
            progressCATB[1] = pgrEATBC2;
            progressCATB[2] = pgrEATBC3;

            progressMATB[0] = pgrEATBM1;
            progressMATB[1] = pgrEATBM2;
            progressMATB[2] = pgrEATBM3;
            progressMATB[3] = pgrEATBM4;
            progressMATB[4] = pgrEATBM5;

            cboSoloLeader.Items.Add("Slot 1");
            cboSoloLeader.Items.Add("Slot 2");
            cboSoloLeader.Items.Add("Slot 3");

            cboAspectRatio.Items.Add("4:3");
            cboAspectRatio.Items.Add("16:9");
            cboAspectRatio.Items.Add("16:10");
            cboAspectRatio.Items.Add("21:9");
            cboAspectRatio.Items.Add("32:9");

            cboCamera.Items.Add("Default");
            cboCamera.Items.Add("Advanced");

            cboKillBGM.Items.Add("Field");
            cboKillBGM.Items.Add("Battle");
            cboKillBGM.Items.Add("Both");

            cboSwitchChar.Items.Add("Dart");
            cboSwitchChar.Items.Add("Lavitz");
            cboSwitchChar.Items.Add("Shana");
            cboSwitchChar.Items.Add("Rose");
            cboSwitchChar.Items.Add("Haschel");
            cboSwitchChar.Items.Add("Albert");
            cboSwitchChar.Items.Add("Meru");
            cboSwitchChar.Items.Add("Kongol");
            cboSwitchChar.Items.Add("Miranda");

            cboSwitch1.Items.Add("Dart");
            cboSwitch1.Items.Add("Lavitz");
            cboSwitch1.Items.Add("Shana");
            cboSwitch1.Items.Add("Rose");
            cboSwitch1.Items.Add("Haschel");
            cboSwitch1.Items.Add("Albert");
            cboSwitch1.Items.Add("Meru");
            cboSwitch1.Items.Add("Kongol");
            cboSwitch1.Items.Add("Miranda");

            cboSwitch2.Items.Add("Dart");
            cboSwitch2.Items.Add("Lavitz");
            cboSwitch2.Items.Add("Shana");
            cboSwitch2.Items.Add("Rose");
            cboSwitch2.Items.Add("Haschel");
            cboSwitch2.Items.Add("Albert");
            cboSwitch2.Items.Add("Meru");
            cboSwitch2.Items.Add("Kongol");
            cboSwitch2.Items.Add("Miranda");

            cboElement.Items.Add("Fire");
            cboElement.Items.Add("Water");
            cboElement.Items.Add("Wind");
            cboElement.Items.Add("Earth");
            cboElement.Items.Add("Dark");
            cboElement.Items.Add("Light");
            cboElement.Items.Add("Thunder");

            cboQTB.Items.Add("Dart");
            cboQTB.Items.Add("Lavitz");
            cboQTB.Items.Add("Shana");
            cboQTB.Items.Add("Rose");
            cboQTB.Items.Add("Haschel");
            cboQTB.Items.Add("Albert");
            cboQTB.Items.Add("Meru");
            cboQTB.Items.Add("Kongol");
            cboQTB.Items.Add("Miranda");

            cboFlowerStorm.Items.Add("2 Turns (40 MP)");
            cboFlowerStorm.Items.Add("3 Turns (60 MP)");
            cboFlowerStorm.Items.Add("4 Turns (80 MP)");
            cboFlowerStorm.Items.Add("5 Turns (100 MP)");

            cboReaderUIRemoval.Items.Add("None");
            cboReaderUIRemoval.Items.Add("Remove Pictures and Stats");
            cboReaderUIRemoval.Items.Add("Remove Entire UI");

            cboReaderOnHotkey.Items.Add("None");
            cboReaderOffHotkey.Items.Add("None");
            cboReaderFieldHotkey.Items.Add("None");

            for (int i = 1; i < 17; i++) {
                cboReaderOnHotkey.Items.Add("F" + i);
                cboReaderOffHotkey.Items.Add("F" + i);
                cboReaderFieldHotkey.Items.Add("F" + i);
            }

            battleRow[0] = cboRowDart;
            battleRow[1] = cboRowLavitz;
            battleRow[2] = cboRowShana;
            battleRow[3] = cboRowRose;
            battleRow[4] = cboRowHaschel;
            battleRow[5] = cboRowAlbert;
            battleRow[6] = cboRowMeru;
            battleRow[7] = cboRowKongol;
            battleRow[8] = cboRowMiranda;

            for (int i = 0; i < 9; i++) {
                battleRow[i].Items.Add("Stay");
                battleRow[i].Items.Add("Front");
                battleRow[i].Items.Add("Back");
                battleRow[i].SelectedIndex = 0;
            }

            battleRowBoost[0] = cboRowDartBoost;
            battleRowBoost[1] = cboRowLavitzBoost;
            battleRowBoost[2] = cboRowShanaBoost;
            battleRowBoost[3] = cboRowRoseBoost;
            battleRowBoost[4] = cboRowHaschelBoost;
            battleRowBoost[5] = cboRowAlbertBoost;
            battleRowBoost[6] = cboRowMeruBoost;
            battleRowBoost[7] = cboRowKongolBoost;
            battleRowBoost[8] = cboRowMirandaBoost;

            for (int i = 0; i < 9; i++) {
                battleRowBoost[i].Items.Add("No Boost");
                battleRowBoost[i].Items.Add("Attack Boost");
                battleRowBoost[i].Items.Add("Magic Boost");
                battleRowBoost[i].SelectedIndex = 0;
            }

            cboUltimateBoss.Items.Add("Zone 1 - Commander II");
            cboUltimateBoss.Items.Add("Zone 1 - Fruegel");
            cboUltimateBoss.Items.Add("Zone 1 - Urobolus");
            cboUltimateBoss.Items.Add("Zone 2 - Sandora Elite");
            cboUltimateBoss.Items.Add("Zone 2 - Drake the Bandit");
            cboUltimateBoss.Items.Add("Zone 2 - Jiango");
            cboUltimateBoss.Items.Add("Zone 2 - Fruegel II");
            cboUltimateBoss.Items.Add("Zone 2 - Fire Bird");
            cboUltimateBoss.Items.Add("Zone 3 - Feyrbrand (Spirit)");
            cboUltimateBoss.Items.Add("Zone 3 - Mappi");
            cboUltimateBoss.Items.Add("Zone 3 - Mappi + Gehrich");
            cboUltimateBoss.Items.Add("Zone 3 - Ghost Commander");
            cboUltimateBoss.Items.Add("Zone 3 - Kamuy");
            cboUltimateBoss.Items.Add("Zone 3 - Regole (Spirit)");
            cboUltimateBoss.Items.Add("Zone 3 - Grand Jewel");
            cboUltimateBoss.Items.Add("Zone 3 - Windigo");
            cboUltimateBoss.Items.Add("Zone 3 - Polter Sword, Helm, and Armor");
            cboUltimateBoss.Items.Add("Zone 3 - Last Kraken");
            cboUltimateBoss.Items.Add("Zone 3 - Kubila, Selebus, and Vector");
            cboUltimateBoss.Items.Add("Zone 3 - Caterpiller");
            cboUltimateBoss.Items.Add("Zone 3 - Zackwell");
            cboUltimateBoss.Items.Add("Zone 3 - Divine Dragon (Spirit)");
            cboUltimateBoss.Items.Add("Zone 4 - Virage I");
            cboUltimateBoss.Items.Add("Zone 4 - Kongol II");
            cboUltimateBoss.Items.Add("Zone 4 - Lenus");
            cboUltimateBoss.Items.Add("Zone 4 - Syuviel");
            cboUltimateBoss.Items.Add("Zone 4 - Virage II");
            cboUltimateBoss.Items.Add("Zone 4 - Greham + Feybrand");
            cboUltimateBoss.Items.Add("Zone 4 - Damia");
            cboUltimateBoss.Items.Add("Zone 4 - Lenus + Regole");
            cboUltimateBoss.Items.Add("Zone 4 - Belzac");
            cboUltimateBoss.Items.Add("Zone 4 - S Virage I");
            cboUltimateBoss.Items.Add("Zone 4 - Kanzas");
            cboUltimateBoss.Items.Add("Zone 4 - Emperor Doel");
            cboUltimateBoss.Items.Add("Zone 4 - S Virage II");
            cboUltimateBoss.Items.Add("Zone 4 - Divine Dragon");
            cboUltimateBoss.Items.Add("Zone 4 - Lloyd");
            cboUltimateBoss.Items.Add("Zone 4 - Magician Faust");
            cboUltimateBoss.Items.Add("Zone 4 - Zieg");
            cboUltimateBoss.Items.Add("Zone 4 - Melbu Frahma");

            cboHelpTopic.Items.Add("General");
            cboHelpTopic.Items.Add("Battle Stats Tab");
            cboHelpTopic.Items.Add("Difficulty Tab");
            cboHelpTopic.Items.Add("Enhancements Tab I");
            cboHelpTopic.Items.Add("Enhancements Tab II");
            cboHelpTopic.Items.Add("Enhancements Tab III");
            cboHelpTopic.Items.Add("Ultimate Boss");
            cboHelpTopic.Items.Add("Shop Tab");
            cboHelpTopic.Items.Add("Reader Tab");
            cboHelpTopic.Items.Add("Settings Tab");
            cboHelpTopic.Items.Add("Hotkeys");
            cboHelpTopic.Items.Add("How To");

            lstTicketShop.Items.Add("1 Ticket / 15 G");
            lstTicketShop.Items.Add("5 Tickets / 60 G");
            lstTicketShop.Items.Add("10 Tickets / 100 G");
            lstHeroShop.Items.Add("Spirit Poition/20 Tickets");
            lstHeroShop.Items.Add("Total Vanishing/40 Tickets");
            lstHeroShop.Items.Add("Healing Rain/60 Tickets");
            lstHeroShop.Items.Add("Moon Serenade/100 Tickets");
            lstUltimateShop.Items.Add("Spirit Eater / 70,000 G");
            lstUltimateShop.Items.Add("Harpoon / 70,000 G");
            lstUltimateShop.Items.Add("Element Arrow / 70,000 G");
            lstUltimateShop.Items.Add("Dragon Beater / 70,000 G");
            lstUltimateShop.Items.Add("Battery Glove / 70,000 G");
            lstUltimateShop.Items.Add("Jeweled Hammer  / 70,000 G");
            lstUltimateShop.Items.Add("Giant Axe / 70,000 G");
            lstUltimateShop.Items.Add("Soa's Light / 280,000 G");
            lstUltimateShop.Items.Add("Fake Legend Casque / 30,000 G");
            lstUltimateShop.Items.Add("Soa's Helm / 120,000 G");
            lstUltimateShop.Items.Add("Fake Legend Armor / 30,000 G");
            lstUltimateShop.Items.Add("Divine DG Armor / 60,000 G");
            lstUltimateShop.Items.Add("Soa's Armor / 120,000 G");
            lstUltimateShop.Items.Add("Lloyd's Boots / 40,000 G");
            lstUltimateShop.Items.Add("Winged Shoes / 40,000 G");
            lstUltimateShop.Items.Add("Soa's Greaves / 120,000 G");
            lstUltimateShop.Items.Add("Heal Ring / 20,000 G");
            lstUltimateShop.Items.Add("Soa's Sash / 40,000 G");
            lstUltimateShop.Items.Add("Soa's Ahnk / 50,000 G");
            lstUltimateShop.Items.Add("Soa's Health Ring / 50,000 G");
            lstUltimateShop.Items.Add("Soa's Mage Ring / 50,000 G");
            lstUltimateShop.Items.Add("Soa's Shield / 50,000 G");
            lstUltimateShop.Items.Add("Soa's Siphon Ring / 50,000 G");
            lstUltimateShop.Items.Add("Super Power Up / 100,000 G");
            lstUltimateShop.Items.Add("Super Power Down / 100,000 G");
            lstUltimateShop.Items.Add("Super Speed Up / 100,000 G");
            lstUltimateShop.Items.Add("Super Speed Down / 100,000 G");
            lstUltimateShop.Items.Add("Super Magic Shield / 100,000 G");
            lstUltimateShop.Items.Add("Super Material Shield / 100,000 G");
            lstUltimateShop.Items.Add("Super Magic Stone of Signet / 75,000 G");
            lstUltimateShop.Items.Add("Super Pandemonium / 25,000 G");
            lstUltimateShop.Items.Add("Psychedelic Bomb X / 1,000,000 G");
            lstUltimateShop.Items.Add("Empty Dragoon Crystal / 250,000 G");
            lstUltimateShop.Items.Add("Soa's Wargod / 500,000 G");
            lstUltimateShop.Items.Add("Soa's Dragoon Boost / 500,000 G");

            cboSoloLeader.SelectedIndex = 0;
            cboAspectRatio.SelectedIndex = 0;
            cboCamera.SelectedIndex = 0;
            cboKillBGM.SelectedIndex = 1;
            cboSwitchChar.SelectedIndex = 0;
            cboElement.SelectedIndex = 0;
            cboUltimateBoss.SelectedIndex = 0;
            cboSwitch1.SelectedIndex = 0;
            cboSwitch2.SelectedIndex = 1;
            cboQTB.SelectedIndex = 0;
            cboFlowerStorm.SelectedIndex = 0;
            cboReaderUIRemoval.SelectedIndex = 0;
            cboReaderOnHotkey.SelectedIndex = 0;
            cboReaderOffHotkey.SelectedIndex = 0;
            cboReaderFieldHotkey.SelectedIndex = 0;
            cboHelpTopic.SelectedIndex = 0;

        }

        public void LoadKey() {
            if (Constants.KEY.GetValue("Save Slot") == null) {
                foreach (MenuItem mi in miSaveSlot.Items)
                    mi.IsChecked = miSaveSlot.Items.IndexOf(mi) == Constants.SAVE_SLOT ? true : false;
                Constants.KEY.SetValue("Save Slot", Constants.SAVE_SLOT);
            } else {
                int slot = (int) Constants.KEY.GetValue("Save Slot");
                Constants.SAVE_SLOT = slot;
                foreach (MenuItem mi in miSaveSlot.Items)
                    mi.IsChecked = miSaveSlot.Items.IndexOf(mi) == slot ? true : false;
            }

            if (Constants.KEY.GetValue("EmulatorType") != null) {
                int slot = (int) Constants.KEY.GetValue("EmulatorType");
                Constants.EMULATOR_ID = (byte) slot;

                foreach (MenuItem mi in miEmulator.Items)
                    mi.IsChecked = miEmulator.Items.IndexOf(mi) == slot ? true : false;
            }

            if (Constants.KEY.GetValue("Region") == null) {
                foreach (MenuItem mi in miRegion.Items)
                    mi.IsChecked = miRegion.Items.IndexOf(mi) == (byte) Constants.REGION ? true : false;
                Constants.KEY.SetValue("Region", (int) Constants.REGION);
            } else {
                Region slot = (Region) ((int) Constants.KEY.GetValue("Region"));
                Constants.REGION = slot;
                foreach (MenuItem mi in miRegion.Items)
                    mi.IsChecked = miRegion.Items.IndexOf(mi) == (byte) Constants.REGION ? true : false;
            }

            if (Constants.KEY.GetValue("LoadPreset") != null) {
                bool load = Constants.KEY.GetValue("LoadPreset").Equals("True");
                if (Constants.KEY.GetValue("Preset") != null && load) {
                    preset = (string) Constants.KEY.GetValue("Preset");
                }
                miOpenPreset.IsChecked = load;
            }

            if (Constants.KEY.GetValue("Preset Hotkeys") == null) {
                Constants.KEY.SetValue("Preset Hotkeys", true);
                miPresetHotkeys.IsChecked = true;
                presetHotkeys = true;
            } else {
                bool preset = Convert.ToBoolean(Constants.KEY.GetValue("Preset Hotkeys"));
                miPresetHotkeys.IsChecked = preset;
                presetHotkeys = miPresetHotkeys.IsChecked;
            }

            if (Constants.KEY.GetValue("Offset") == null) {
                Constants.KEY.SetValue("Offset", 0);
            } else {
                oldOffset = Int64.Parse(Constants.KEY.GetValue("Offset").ToString());
            }

            if (Constants.KEY.GetValue("Reader Hotkey On") == null) {
                Constants.KEY.SetValue("Reader Hotkey On", 0);
            } else {
                cboReaderOnHotkey.SelectedIndex = (int) Constants.KEY.GetValue("Reader Hotkey On"); ;
            }

            if (Constants.KEY.GetValue("Reader Hotkey Off") == null) {
                Constants.KEY.SetValue("Reader Hotkey Off", 0);
            } else {
                cboReaderOffHotkey.SelectedIndex = (int) Constants.KEY.GetValue("Reader Hotkey Off");
            }

            if (Constants.KEY.GetValue("Reader Hotkey Field") == null) {
                Constants.KEY.SetValue("Reader Hotkey Field", 0);
            } else {
                cboReaderFieldHotkey.SelectedIndex = (int) Constants.KEY.GetValue("Reader Hotkey Field");
            }

            if (Constants.KEY.GetValue("Zoom") == null) {
                Constants.KEY.SetValue("Zoom", 4096);
            } else {
                sldZoom.Value = Double.Parse(Constants.KEY.GetValue("Zoom").ToString());
            }

            if (Constants.KEY.GetValue("Flower Storm Turns") == null) {
                Constants.KEY.SetValue("Flower Storm Turns", 0);
            } else {
                cboFlowerStorm.SelectedIndex = (int) Constants.KEY.GetValue("Flower Storm Turns");
            }

            if (Constants.KEY.GetValue("Debug Mode") == null) {
                Constants.KEY.SetValue("Debug Mode", false);
            } else {
                Constants.DEBUG_MODE = bool.Parse(Constants.KEY.GetValue("Debug Mode").ToString());
            }

            if (Constants.KEY.GetValue("EATB Sounds") == null) {
                Constants.KEY.SetValue("EATB Sounds", false);
            } else {
                Constants.EATB_BEEP = bool.Parse(Constants.KEY.GetValue("EATB Sounds").ToString());
                miEatbSound.IsChecked = Constants.EATB_BEEP;
            }

            if (Constants.KEY.GetValue("Offset") == null) {
                Constants.KEY.SetValue("Offset", 0);
            } else {
                Constants.OFFSET = long.Parse(Constants.KEY.GetValue("Offset").ToString());
            }
        }

        public void LoadReaderKey() {
            if (Constants.KEY.GetValue("Reader Width") != null)
                readerWindow.Width = Double.Parse(Constants.KEY.GetValue("Reader Width").ToString());

            if (Constants.KEY.GetValue("Reader Height") != null)
                readerWindow.Height = Double.Parse(Constants.KEY.GetValue("Reader Height").ToString());

            if (Constants.KEY.GetValue("Reader AA") != null)
                readerWindow.SetAntiAlias(bool.Parse(Constants.KEY.GetValue("Reader AA").ToString()));

            if (Constants.KEY.GetValue("Reader Write Text") != null)
                readerWindow.WRITE_TEXT = bool.Parse(Constants.KEY.GetValue("Reader Write Text").ToString());

            if (Constants.KEY.GetValue("Reader Write Folder") != null)
                readerWindow.WRITE_LOCATION = Constants.KEY.GetValue("Reader Write Folder").ToString();

            if (Constants.KEY.GetValue("Reader Background") != null)
                readerWindow.Background = new SolidColorBrush((Color) ColorConverter.ConvertFromString(Constants.KEY.GetValue("Reader Background").ToString()));

            if (Constants.KEY.GetValue("Reader Remove UI") != null)
                cboReaderUIRemoval.SelectedIndex = (int) Constants.KEY.GetValue("Reader Remove UI");
        }

        public void LoadSubKey() {
            if (Constants.SUBKEY.GetValue("Ultimate Boss") == null) {
                Constants.SUBKEY.SetValue("Ultimate Boss", 0);
                ultimateBossCompleted = 0;
            } else {
                ultimateBossCompleted = (int) Constants.SUBKEY.GetValue("Ultimate Boss");
            }

            if (Constants.SUBKEY.GetValue("Inventory Size") == null) {
                Constants.SUBKEY.SetValue("Inventory Size", 32);
                inventorySize = 32;
            } else {
                inventorySize = (int) Constants.SUBKEY.GetValue("Inventory Size");
            }

            if (Constants.SUBKEY.GetValue("Ultimate Shop") == null) {
                Constants.SUBKEY.SetValue("Ultimate Shop", 0);
                ultimateShopLimited = 0;
            } else {
                ultimateShopLimited = Convert.ToInt64(Constants.SUBKEY.GetValue("Ultimate Shop"));
            }

            if (Constants.SUBKEY.GetValue("Faust") == null) {
                Constants.SUBKEY.SetValue("Faust", 0);
                faustCount = 0;
            } else {
                faustCount = (int) Constants.SUBKEY.GetValue("Faust");
            }
        }

        public void SaveKey() {
            Constants.KEY.SetValue("Save Slot", Constants.SAVE_SLOT);
            if (Constants.EMULATOR_ID != 255) {
                Constants.KEY.SetValue("EmulatorType", (int) Constants.EMULATOR_ID);
                if (Constants.EMULATOR_ID == 10) {
                    Constants.KEY.SetValue("Other Emulator", Constants.EMULATOR_NAME);
                }
            }
            Constants.KEY.SetValue("Offset", Constants.OFFSET);
            Constants.KEY.SetValue("Region", (int) Constants.REGION);
            Constants.KEY.SetValue("LoadPreset", miOpenPreset.IsChecked);
            if (miOpenPreset.IsChecked)
                Constants.KEY.SetValue("Preset", preset);
            Constants.KEY.SetValue("Preset Hotkeys", miPresetHotkeys.IsChecked);
            Constants.KEY.SetValue("Reader Hotkey On", cboReaderOnHotkey.SelectedIndex);
            Constants.KEY.SetValue("Reader Hotkey Off", cboReaderOffHotkey.SelectedIndex);
            Constants.KEY.SetValue("Reader Hotkey Field", cboReaderFieldHotkey.SelectedIndex);
            Constants.KEY.SetValue("Flower Storm Turns", cboFlowerStorm.SelectedIndex);
            Constants.KEY.SetValue("Zoom", sldZoom.Value);
            Constants.KEY.SetValue("EATB Sounds", miEatbSound.IsChecked);
        }

        public void SaveReaderKey() {
            Constants.KEY.SetValue("Reader Width", readerWindow.Width);
            Constants.KEY.SetValue("Reader Height", readerWindow.Height);
            Constants.KEY.SetValue("Reader AA", readerWindow.AA);
            Constants.KEY.SetValue("Reader Write Text", readerWindow.WRITE_TEXT);
            Constants.KEY.SetValue("Reader Write Folder", readerWindow.WRITE_LOCATION);
            Constants.KEY.SetValue("Reader Background", readerWindow.Background.ToString());
            Constants.KEY.SetValue("Reader Remove UI", uiCombo["cboReaderUIRemoval"]);
        }

        public void SaveSubKey() {
            if (stopSave)
                return;
            Constants.SUBKEY.SetValue("Ultimate Boss", ultimateBossCompleted);
            Constants.SUBKEY.SetValue("Inventory Size", inventorySize);
            Constants.SUBKEY.SetValue("Ultimate Shop", ultimateShopLimited);
            Constants.SUBKEY.SetValue("Faust", faustCount);
        }

        private void LoadPreset() {
            string current = "";
            try {
                foreach (KeyValuePair<string, byte> entry in Constants.PRESET_SCRIPTS) {
                    bool found = false;
                    current = entry.Key;

                    found = FindScript(lstField, entry);
                    if (found)
                        continue;

                    found = FindScript(lstBattle, entry);
                    if (found)
                        continue;

                    found = FindScript(lstHotkey, entry);
                    if (found)
                        continue;

                    found = FindScript(lstOther, entry);

                    if (!found) {
                        throw new Exception();
                    }
                }
                ChangeTitle(preset);
                Constants.WriteOutput("Preset '" + preset + "' loaded.");
                Constants.WriteOutput("Preset folder: " + Globals.MOD);
            } catch (Exception e) {
                Constants.WriteOutput("Failed to load preset. Script not found: '" + current + "'.");
                DisableScripts();
            }
        }

        public bool FindScript(ListView lst, KeyValuePair<string, byte> entry) {
            bool found = false;
            foreach (SubScript s in lst.Items) {
                if (s.GetPath().Contains(entry.Key)) {
                    s.state = (ScriptState) entry.Value;
                    found = true;
                }
            }
            return found;
        }

        public void DisableScripts() {
            DisableScripts(lstField, true);
            DisableScripts(lstBattle, true);
            DisableScripts(lstHotkey, true);
            DisableScripts(lstOther, false);
        }

        public void DisableScripts(ListView lst, bool skip) {
            int index = 0;
            foreach (SubScript s in lst.Items) {
                index++;
                if (index == 1 && skip)
                    continue;
                s.state = ScriptState.DISABLED;
            }
            ScriptDisplay(lstField);
            ScriptDisplay(lstBattle);
            ScriptDisplay(lstHotkey);
            ScriptDisplay(lstOther);
        }

        public void OpenScript(ListView lst) {
            SubScript script = (SubScript) lst.SelectedItem;
            if (script.state != ScriptState.DISABLED) {
                Constants.WriteOutput("Opening script '" + script.ToString() + "'...");
                if (script.Click() == 0) {
                    Constants.WriteOutput("Script failed.");
                }
            } else {
                Constants.WriteOutput("Script is disabled.");
            }
        }

        public void ChangeTitle(string preset) {
            this.preset = preset;
            this.Title = " Dragoon Modifier " + Constants.VERSION + " (" + preset + ")";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            Constants.SetSubKey(Constants.SAVE_SLOT);
            LoadSubKey();
        }

        public void ScriptDisplay(ListView list) {
            for (int i = 0; i < list.Items.Count; i++) {
                try {
                    ListViewItem lbl = (ListViewItem) list.ItemContainerGenerator.ContainerFromIndex(i);
                    SubScript script = (SubScript) list.Items[i];
                    if (script.state == ScriptState.DISABLED) {
                        lbl.Foreground = Brushes.Red;
                    } else {
                        lbl.Foreground = Brushes.Black;
                    }
                } catch (Exception e) { }
            }
        }
        #endregion

        #region Threads
        public void FieldController() {
            string currentScript = "";
            int run = 1;
            while (run == 1 && Constants.RUN) {
                foreach (SubScript script in lstField.Items) {
                    if (script.state == ScriptState.DISABLED)
                        continue;
                    currentScript = script.ToString();
                    this.Dispatcher.BeginInvoke(new Action(() => {
                        run = script.Run();
                    }), DispatcherPriority.ContextIdle);
                }

                //LoadMaxHPTable(false);

                if (inventorySize != 32) {
                    ExtendInventory();
                }

                try {
                    if (Globals.SHOP_CHANGE)
                        ShopChanges();
                    if (Globals.CheckDMScript("btnSaveAnywhere"))
                        SaveAnywhere();
                    if (Globals.CheckDMScript("btnSoloMode"))
                        SoloModeField();
                    if (Globals.CheckDMScript("btnDuoMode"))
                        DuoModeField();
                    if (Globals.CheckDMScript("btnHPCapBreak"))
                        HPCapBreakField();
                    if (Globals.CheckDMScript("btnKillBGM") && killBGMField)
                        KillBGMField();
                    if (Globals.CheckDMScript("btnCharmPotion"))
                        AutoCharmPotion();
                    if (Globals.CheckDMScript("btnEarlyAdditions"))
                        EarlyAdditions();
                    if (Globals.CheckDMScript("btnUltimateBoss"))
                        UltimateBossField();
                    if (Globals.CheckDMScript("btnTextSpeed"))
                        IncreaseTextSpeed();
                    if (Globals.CheckDMScript("btnAutoText"))
                        AutoText();
                } catch (Exception ex) {
                    Constants.RUN = false;
                    Constants.WriteGLog("Program stopped.");
                    Constants.WritePLogOutput("INTERNAL FIELD SCRIPT ERROR");
                    Constants.WriteOutput("Fatal Error. Closing all threads. Please see error log in Settings console.");
                    Constants.WriteError(ex.ToString());
                    EnableUI();
                }


                Thread.Sleep(500);

                if (Emulator.Memory.GameState != GameState.Battle) {
                    if ((Globals.MAP == 735 || Globals.MAP == 736) && Globals.DIFFICULTY_MODE.Contains("Hell") && ultimateBossCompleted < 34) {
                        Emulator.WriteByte("DRAGOON_SPIRITS", 127);
                    }
                }

                this.Dispatcher.BeginInvoke(new Action(() => {
                    if (Globals.CheckDMScript("btnReader") && !readerWindow.IsOpen) {
                        Globals.dmScripts["btnReader"] = false;
                        TurnOnOffButton(ref btnReader);
                    }

                    FieldUI();

                    if (Emulator.Memory.GameState == GameState.Battle && currentIconState != 3) {
                        currentIconState = 3;
                        System.Drawing.Icon newIcon = Properties.Resources.Icon_Green;
                        System.Drawing.Bitmap bitmap = newIcon.ToBitmap();
                        IntPtr hBitmap = bitmap.GetHbitmap();
                        this.Icon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                    } 

                    if ((Globals.BATTLE_VALUE > 3840 && Globals.BATTLE_VALUE <= 5120) && currentIconState != 2) {
                        currentIconState = 2;
                        System.Drawing.Icon newIcon = Properties.Resources.Icon_Red;
                        System.Drawing.Bitmap bitmap = newIcon.ToBitmap();
                        IntPtr hBitmap = bitmap.GetHbitmap();
                        this.Icon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                    } 
                    
                    if ((Globals.BATTLE_VALUE > 2560 && Globals.BATTLE_VALUE <= 3840) && currentIconState != 1) {
                        currentIconState = 1;
                        System.Drawing.Icon newIcon = Properties.Resources.Icon_Yellow;
                        System.Drawing.Bitmap bitmap = newIcon.ToBitmap();
                        IntPtr hBitmap = bitmap.GetHbitmap();
                        this.Icon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                    } 
                    
                    if (Globals.BATTLE_VALUE <= 2560 && currentIconState != 0) {
                        currentIconState = 0;
                        System.Drawing.Icon newIcon = Properties.Resources.Icon_Blue;
                        System.Drawing.Bitmap bitmap = newIcon.ToBitmap();
                        IntPtr hBitmap = bitmap.GetHbitmap();
                        this.Icon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                    }
                }), DispatcherPriority.ContextIdle);
            }
        }

        public void BattleController1() {
            string currentScript = "";
            int run = 1;
            while (run == 1 && Constants.RUN) {
                foreach (SubScript script in lstBattle.Items) {
                    if (script.state == ScriptState.DISABLED)
                        continue;
                    currentScript = script.ToString();
                    this.Dispatcher.BeginInvoke(new Action(() => {
                        run = script.Run();
                    }), DispatcherPriority.ContextIdle);
                }

                this.Dispatcher.BeginInvoke(new Action(() => {
                    if (Emulator.Memory.GameState == GameState.Battle && !Constants.BATTLE_UI) {
                        Globals.BEFORE_BATTLE_MAP = Emulator.ReadShort("MAP");
                        for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                            // monsterDisplay[i, 0].Text = Emulator.ReadName(Constants.GetAddress("MONSTERS_NAMES") + (0x2C * i)); TODO
                        }
                    }
                }), DispatcherPriority.ContextIdle);

                try {
                    if (Globals.CheckDMScript("btnSoloMode"))
                        SoloModeBattle();
                    if (Globals.CheckDMScript("btnDuoMode"))
                        DuoModeBattle();
                    if (Globals.CheckDMScript("btnHPCapBreak"))
                        HPCapBreakBattle();
                    if (Globals.CheckDMScript("btnSoulEater"))
                        NoHPDecaySoulEater();
                    if (Globals.CheckDMScript("btnHPNames"))
                        MonsterHPNames();
                    if (Globals.CheckDMScript("btnUltimateBoss") && keepStats)
                        UltimateBossBattle();
                    if (Globals.CheckDMScript("btnElementalBomb") && keepStats)
                        ElementalBomb();
                    if (Globals.CheckDMScript("btnDamageTracker"))
                        DamageTracker();
                    if (Globals.CheckDMScript("btnRows") && !Globals.DIFFICULTY_MODE.Contains("Hell"))
                        BattleFormationRows();
                    if (Globals.CheckDMScript("btnEATB") || Globals.CheckDMScript("btnATB"))
                        EATB();
                    if (Globals.CheckDMScript("btnQTB"))
                        QTB();
                    if (!Globals.DIFFICULTY_MODE.Equals("Normal"))
                        DoubleRepeat();
                    if (Globals.CheckDMScript("btnReader") && uiCombo["cboReaderUIRemoval"] > 0)
                        ReaderRemoveUI();
                    if (Globals.CheckDMScript("btnReader") && uiCombo["cboReaderOnHotkey"] > 0 && uiCombo["cboReaderOffHotkey"] > 0 && uiCombo["cboReaderFieldHotkey"] > 0)
                        ReaderAutoHotkey();
                } catch (Exception ex) {
                    Constants.RUN = false;
                    Constants.WriteGLog("Program stopped.");
                    Constants.WritePLogOutput("INTERNAL BATTLE SCRIPT ERROR");
                    Constants.WriteOutput("Fatal Error. Closing all threads. Please see error log in Settings console.");
                    Constants.WriteError(ex.ToString());
                    EnableUI();
                }

                if (Globals.MAP == 732 && Globals.ENCOUNTER_ID == 420 && Emulator.Memory.GameState == GameState.Battle && Controller.Main.StatsChanged && Emulator.Battle.MonsterTable[0].HP == 0 && !Globals.DIFFICULTY_MODE.Equals("Normal") && saveFaust) {
                    faustCount += 1;
                    saveFaust = false;
                    Constants.WriteGLogOutput("Your current Faust count is: " + faustCount);
                    SaveSubKey();
                }

                if (ultimateBossKeepMap) {
                    Emulator.WriteShort("MAP", ultimateBossMap);
                    if (Emulator.Memory.GameState != GameState.Battle) {
                        ultimateBossKeepMap = false;

                        this.Dispatcher.BeginInvoke(new Action(() => {
                            Globals.dmScripts["btnUltimateBoss"] = false;
                            TurnOnOffButton(ref btnUltimateBoss);
                        }), DispatcherPriority.ContextIdle);
                    }
                }

                if (!keepStats && Emulator.Memory.GameState == GameState.Battle && Controller.Main.StatsChanged) {
                    for (int i = 0; i < 3; i++) { //Should execute after equip changes
                        if (Globals.PARTY_SLOT[i] < 9) {
                            originalCharacterStats[i, 0] = Emulator.Battle.CharacterTable[i].Max_HP;
                            originalCharacterStats[i, 1] = Emulator.Battle.CharacterTable[i].AT;
                            originalCharacterStats[i, 2] = Emulator.Battle.CharacterTable[i].MAT;
                            originalCharacterStats[i, 3] = Emulator.Battle.CharacterTable[i].DF;
                            originalCharacterStats[i, 4] = Emulator.Battle.CharacterTable[i].MDF;
                        }
                    }

                    for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                        originalMonsterStats[i, 0] = Emulator.Battle.MonsterTable[i].Max_HP;
                        originalMonsterStats[i, 1] = Emulator.Battle.MonsterTable[i].AT;
                        originalMonsterStats[i, 2] = Emulator.Battle.MonsterTable[i].MAT;
                        originalMonsterStats[i, 3] = Emulator.Battle.MonsterTable[i].DF;
                        originalMonsterStats[i, 4] = Emulator.Battle.MonsterTable[i].MDF;
                        originalMonsterStats[i, 5] = Emulator.Battle.MonsterTable[i].SPD;
                    }

                    if (faustBattle && Globals.ENCOUNTER_ID == 420) {
                        Emulator.Battle.MonsterTable[0].HP = 25600;
                        Emulator.Battle.MonsterTable[0].Max_HP = 25600;
                        Emulator.Battle.MonsterTable[0].AT = 125;
                        Emulator.Battle.MonsterTable[0].MAT = 125;
                        Emulator.Battle.MonsterTable[0].DF = 75;
                        Emulator.Battle.MonsterTable[0].MDF = 200;
                        Emulator.Battle.MonsterTable[0].SPD = 50;

                        WipeRewards();

                        Emulator.WriteUShort("MONSTER_REWARDS", 60000, 1 * 0x1A8);
                        Emulator.WriteShort("MONSTER_REWARDS", 250, 1 * 0x1A8 + 0x2);

                        if (faustCount + 1 == 39) {
                            Emulator.WriteByte("MONSTER_REWARDS", 100, 1 * 0x1A8 + 0x4);
                            Emulator.WriteByte("MONSTER_REWARDS", 74, 1 * 0x1A8 + 0x5);
                        } else if (faustCount + 1 == 40) {
                            Emulator.WriteByte("MONSTER_REWARDS", 100, 1 * 0x1A8 + 0x4);
                            Emulator.WriteByte("MONSTER_REWARDS", 89, 1 * 0x1A8 + 0x5);
                        }

                        faustBattle = false;
                        saveFaust = true;
                    }

                    if (Emulator.Memory.GameState == GameState.Battle && ubSoasWargod) {
                        Emulator.WriteAoB("SOAS_WARGOD", "06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06");
                    }

                    if (Emulator.Memory.GameState == GameState.Battle && ubSoasDragoonBoost) {
                        Emulator.WriteShort("SOA_DRAGOON_BOOST_1", 4096);
                        Emulator.WriteShort("SOA_DRAGOON_BOOST_2", 4096);
                    }

                    this.Dispatcher.BeginInvoke(new Action(() => {
                        Emulator.WriteUShort("ZOOM", (ushort) sldZoom.Value);
                        if (!Constants.DEBUG_MODE) {
                            tabBattle.Focus();
                            tabDifficulty.IsEnabled = false;
                            tabEnhancements.IsEnabled = false;
                            tabEnhancements2.IsEnabled = false;
                            tabEnhancements3.IsEnabled = false;
                            tabShop.IsEnabled = false;
                            tabSettings.IsEnabled = false;
                        }
                    }), DispatcherPriority.ContextIdle);

                    keepStats = true;
                } else {
                    if (Emulator.Memory.GameState != GameState.Battle) {
                        keepStats = false;
                    }
                }

                Thread.Sleep(250);
                this.Dispatcher.BeginInvoke(new Action(() => {
                    BattleUI();
                }), DispatcherPriority.ContextIdle);

                if (Globals.EXITING_BATTLE > 0) {
                    Globals.EXITING_BATTLE -= 1;

                    this.Dispatcher.BeginInvoke(new Action(() => {
                        if (!Constants.DEBUG_MODE) {
                            tabDifficulty.IsEnabled = true;
                            tabEnhancements.IsEnabled = true;
                            tabEnhancements2.IsEnabled = true;
                            tabEnhancements3.IsEnabled = true;
                            tabShop.IsEnabled = true;
                            tabSettings.IsEnabled = true;
                        }
                    }), DispatcherPriority.ContextIdle);
                }
            }

            this.Dispatcher.BeginInvoke(new Action(() => {
                System.Drawing.Icon newIcon = Properties.Resources.Icon_Grey;
                System.Drawing.Bitmap bitmap = newIcon.ToBitmap();
                IntPtr hBitmap = bitmap.GetHbitmap();
                this.Icon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                EnableUI();
            }), DispatcherPriority.ContextIdle);
        }

        public void HotkeysController() {
            string currentScript = "";
            int run = 1;
            while (run == 1 && Constants.RUN) {
                Globals.CURRENT_TIME = Constants.GetTime();
                int scriptCount = 0;
                foreach (SubScript script in lstHotkey.Items) {
                    if (script.state == ScriptState.DISABLED)
                        continue;
                    if (Globals.CURRENT_TIME < (Globals.LAST_HOTKEY + 3) && scriptCount > 0)
                        continue;
                    scriptCount++;
                    currentScript = script.ToString();
                    this.Dispatcher.BeginInvoke(new Action(() => {
                        run = script.Run();
                    }), DispatcherPriority.ContextIdle);
                }
                
                if (presetHotkeys) {
                    if (Globals.CURRENT_TIME >= (Globals.LAST_HOTKEY + 3)) {
                        if (Emulator.Memory.GameState != GameState.Battle) { //Field
                            if (Globals.HOTKEY == (Hotkey.KEY_L2 + Hotkey.KEY_SQUARE)) { //Shana - GoH
                                if (Emulator.ReadShort("CHAR_TABLE", 0xA + 0x2C * 2) >= 30) {
                                    for (int i = 0; i < 9; i++) {
                                        Emulator.WriteShort("CHAR_TABLE", 9999, 0x8 + 0x2C * i);
                                    }
                                    Emulator.WriteUShort("CHAR_TABLE", (ushort) (Emulator.ReadUShort("CHAR_TABLE", 0xA + 0x2C * 2) - 30), 0xA + 0x2C * 2);
                                    Constants.WriteGLogOutput("Shana used Gates of Heaven.");
                                    Globals.LAST_HOTKEY = Constants.GetTime();
                                }
                            } else if (Globals.HOTKEY == (Hotkey.KEY_L2 + Hotkey.KEY_CROSS)) { //???
                                if (Emulator.ReadShort("CHAR_TABLE", 0xA + 0x2C * 8) >= 30) {
                                    for (int i = 0; i < 9; i++) {
                                        Emulator.WriteShort("CHAR_TABLE", 9999, 0x8 + 0x2C * i);
                                    }
                                    Emulator.WriteUShort("CHAR_TABLE", (ushort) (Emulator.ReadUShort("CHAR_TABLE", 0xA + 0x2C * 8) - 30), 0xA + 0x2C * 8);
                                    Constants.WriteGLogOutput("Miranda used Gates of Heaven.");
                                    Globals.LAST_HOTKEY = Constants.GetTime();
                                }
                            } else if (Globals.HOTKEY == (Hotkey.KEY_L2 + Hotkey.KEY_CIRCLE)) { //Meru - RB
                                if (Emulator.ReadShort("CHAR_TABLE", 0xA + 0x2C * 6) >= 30) {
                                    for (int i = 0; i < 9; i++) {
                                        Emulator.WriteShort("CHAR_TABLE", 9999, 0x8 + 0x2C * i);
                                    }
                                    Emulator.WriteUShort("CHAR_TABLE", (ushort) (Emulator.ReadShort("CHAR_TABLE", 0xA + 0x2C * 6) - 30), 0xA + 0x2C * 6);
                                    Constants.WriteGLogOutput("Meru used Rainbow Breath.");
                                    Globals.LAST_HOTKEY = Constants.GetTime();
                                }
                            } else if (Globals.HOTKEY == (Hotkey.KEY_SELECT + Hotkey.KEY_L3)) { //Add Shana
                                Emulator.WriteShort("CHAR_TABLE", 3, 0x4 + 0x2C * 2);
                                Constants.WriteGLogOutput("Added Shana.");
                                Globals.LAST_HOTKEY = Constants.GetTime();
                            } else if (Globals.HOTKEY == (Hotkey.KEY_SELECT + Hotkey.KEY_R3)) { //Add Lavitz
                                Emulator.WriteShort("CHAR_TABLE", 3, 0x4 + 0x2C * 1);
                                Constants.WriteGLogOutput("Added Lavitz.");
                                Globals.LAST_HOTKEY = Constants.GetTime();
                            } else if (Globals.HOTKEY == (Hotkey.KEY_CROSS + Hotkey.KEY_L1)) { //Add Dragoon
                                if (!Globals.DIFFICULTY_MODE.Equals("Normal")) {
                                    if (Globals.MAP == 10) {
                                        Emulator.WriteByte("DRAGOON_SPIRITS", 127);
                                        Constants.WriteGLogOutput("All Dragoons at Start.");
                                        Globals.LAST_HOTKEY = Constants.GetTime();
                                    }
                                }

                                if (Globals.MAP == 232) {
                                    byte dragoons = Emulator.ReadByte("DRAGOON_SPIRITS");
                                    dragoons |= 1 << 0;
                                    Emulator.WriteByte("DRAGOON_SPIRITS", dragoons);
                                    Constants.WriteGLogOutput("Added Dart's Dragoon.");
                                    Globals.LAST_HOTKEY = Constants.GetTime();
                                }
                                
                                if (Globals.CheckDMScript("btnSoloMode") || Globals.CheckDMScript("btnDuoMode")) {
                                    for (int i = 0; i < 9; i++) {
                                        Emulator.WriteInt("CHAR_TABLE", 0, 0x0 + 0x2C * i);  // EXP
                                        Emulator.WriteByte("CHAR_TABLE", 3, 0x4 + 0x2C * i);  // In Party
                                        Emulator.WriteByte("CHAR_TABLE", 1, 0x12 + 0x2C * i); // Level
                                        if (i > 2 && (i != 5 || i != 8)) {
                                            Emulator.WriteByte("CHAR_TABLE", 0, 0x14 + 0x2C * i); // Weapon
                                            Emulator.WriteByte("CHAR_TABLE", 76, 0x15 + 0x2C * i); // Armor
                                            Emulator.WriteByte("CHAR_TABLE", 46, 0x16 + 0x2C * i); // Helmet
                                            Emulator.WriteByte("CHAR_TABLE", 93, 0x17 + 0x2C * i); // Shoe
                                        }
                                    }
                                    Constants.WriteGLogOutput("Solo Mode all character start.");
                                    Globals.LAST_HOTKEY = Constants.GetTime();
                                }
                            } else if (Globals.HOTKEY == (Hotkey.KEY_CROSS + Hotkey.KEY_R1)) { //Gold Quest
                                if (Globals.MAP == 333) {
                                    if (goldQuest == 0) {
                                        goldQuest = Emulator.ReadInt("GOLD");
                                        Constants.WriteGLogOutput("Extra gold set, complete the quest and press this hotkey again.");
                                        Globals.LAST_HOTKEY = Constants.GetTime();
                                    } else if (goldQuest + 500 == Emulator.ReadInt("GOLD")) {
                                        Emulator.WriteInt("GOLD", goldQuest + 2500);
                                        Constants.WriteGLogOutput("5x gold.");
                                        Globals.LAST_HOTKEY = Constants.GetTime();
                                    }
                                }
                            } else if (Globals.HOTKEY == (Hotkey.KEY_SQUARE + Hotkey.KEY_TRIANGLE)) { //Divine Dragoon
                                byte dragoonSpirits = Emulator.ReadByte("DRAGOON_SPIRITS");
                                if (!Globals.DIFFICULTY_MODE.Equals("Normal") && ultimateBossCompleted >= 34) {
                                    if (Globals.MAP == 424 || Globals.MAP == 736) {
                                        if (dragoonSpirits == 127) {
                                            Emulator.WriteByte("DRAGOON_SPIRITS", 254);
                                            Constants.WriteGLogOutput("Change to Divine Dragon mode.");
                                            Globals.LAST_HOTKEY = Constants.GetTime();
                                        } else if (dragoonSpirits == 254) {
                                            Emulator.WriteByte("DRAGOON_SPIRITS", 127);
                                            Constants.WriteGLogOutput("Changed to Red-Eyed Dragon mode.");
                                            Globals.LAST_HOTKEY = Constants.GetTime();
                                        } else {
                                            Constants.WriteGLogOutput("You need all dragoons.");
                                            Globals.LAST_HOTKEY = Constants.GetTime();
                                        }
                                    }
                                } 
                            } else if (Globals.HOTKEY == (Hotkey.KEY_SQUARE + Hotkey.KEY_CROSS)) { //Faust Battle
                                if (!Globals.DIFFICULTY_MODE.Equals("Normal")) {
                                    if (Globals.MAP == 732) {
                                        Emulator.WriteShort("BATTLE_FIELD", 78);
                                        Emulator.WriteShort("ENCOUNTER_ID", 420);
                                        faustBattle = true;
                                    }
                                }
                            } else if (Globals.HOTKEY == (Hotkey.KEY_START + Hotkey.KEY_L3)) { //Moon Warp
                                if (Globals.MAP == 729 || Globals.MAP == 730 || Globals.MAP == 527)
                                    Emulator.WriteShort("MAP", 527);
                            } else if (Globals.HOTKEY == (Hotkey.KEY_START + Hotkey.KEY_R3)) { //Moon Warp 2
                                if (Globals.MAP == 597 || Globals.MAP == 521 || Globals.MAP == 524 || Globals.MAP == 526 || Globals.MAP == 527 || Globals.MAP == 729 || Globals.MAP == 9)
                                    Emulator.WriteShort("MAP", 729);
                                if (Globals.BATTLE_VALUE == 0 && Globals.CHAPTER == 4 && (Globals.BEFORE_BATTLE_MAP >= 401 && Globals.BEFORE_BATTLE_MAP <= 405))
                                    Emulator.WriteShort("MAP", 402);
                            } else if (Globals.HOTKEY == (Hotkey.KEY_CIRCLE + Hotkey.KEY_TRIANGLE)) { //Skip Dialog
                                Emulator.WriteByte("SKIP_DIALOG_1", 0);
                                Emulator.WriteByte("SKIP_DIALOG_2", 0);
                            } else if (Globals.HOTKEY == (Hotkey.KEY_L1 + Hotkey.KEY_LEFT)) { //Widescreen for World Map
                                ushort aspectRatio = 4096;

                                if (uiCombo["cboAspectRatio"] == 0)
                                    aspectRatio = 4096;
                                else if (uiCombo["cboAspectRatio"] == 1)
                                    aspectRatio = 3072;
                                else if (uiCombo["cboAspectRatio"] == 2)
                                    aspectRatio = 3413;
                                else if (uiCombo["cboAspectRatio"] == 3)
                                    aspectRatio = 2340;
                                else if (uiCombo["cboAspectRatio"] == 4)
                                    aspectRatio = 2048;

                                Emulator.WriteUShort("ASPECT_RATIO", aspectRatio);
                            } else if (Globals.HOTKEY == (Hotkey.KEY_SQUARE + Hotkey.KEY_UP)) {
                                Emulator.WriteByte("MENU", 16);
                                Emulator.WriteByte("TRANSITION", 13);
                                Globals.LAST_HOTKEY = Constants.GetTime();
                            } else if (Globals.HOTKEY == (Hotkey.KEY_SQUARE + Hotkey.KEY_DOWN)) {
                                Emulator.WriteByte("TRANSITION", 11);
                                Globals.LAST_HOTKEY = Constants.GetTime();
                            } else if (Globals.HOTKEY == (Hotkey.KEY_SQUARE + Hotkey.KEY_LEFT)) {
                                Emulator.WriteByte("MENU", 16);
                                Emulator.WriteByte("OVERWORLD", 4);
                                Globals.LAST_HOTKEY = Constants.GetTime();
                            } else if (Globals.HOTKEY == (Hotkey.KEY_SQUARE + Hotkey.KEY_RIGHT)) {
                                Emulator.WriteByte("TRANSITION", 4);
                                Globals.LAST_HOTKEY = Constants.GetTime();
                            }
                        } else { //Battle
                            if (Globals.HOTKEY == (Hotkey.KEY_SQUARE + Hotkey.KEY_UP)) { //*TB Slot 1
                                if (extraTurnBattleC[0] >= 6000) {
                                    ExtraTurnBattle(ref extraTurnBattleC[0], 0);
                                    Globals.LAST_HOTKEY = Constants.GetTime();
                                }

                                if (Globals.CheckDMScript("btnQTB") && qtbTurns > 0) {
                                    if (Globals.PARTY_SLOT[0] == uiCombo["cboQTB"]) {
                                        qtbTurns -= 1;
                                        SubQTB(0);
                                        Globals.LAST_HOTKEY = Constants.GetTime();
                                    } else {
                                        SubQTB(0);
                                        Globals.LAST_HOTKEY = Constants.GetTime();
                                    }
                                }
                            } else if (Globals.HOTKEY == (Hotkey.KEY_SQUARE + Hotkey.KEY_RIGHT)) { //*TB Slot 2
                                if (extraTurnBattleC[1] >= 6000) {
                                    ExtraTurnBattle(ref extraTurnBattleC[1], 1);
                                    Globals.LAST_HOTKEY = Constants.GetTime();
                                }

                                if (Globals.CheckDMScript("btnQTB") && qtbTurns > 0) {
                                    if (Globals.PARTY_SLOT[1] == uiCombo["cboQTB"]) {
                                        qtbTurns -= 1;
                                        SubQTB(1);
                                        Globals.LAST_HOTKEY = Constants.GetTime();
                                    } else {
                                        SubQTB(1);
                                        Globals.LAST_HOTKEY = Constants.GetTime();
                                    }
                                }
                            } else if (Globals.HOTKEY == (Hotkey.KEY_SQUARE + Hotkey.KEY_LEFT)) { //*TB Slot 3
                                if (extraTurnBattleC[2] >= 6000) {
                                    ExtraTurnBattle(ref extraTurnBattleC[2], 2);
                                    Globals.LAST_HOTKEY = Constants.GetTime();
                                }

                                if (Globals.CheckDMScript("btnQTB") && qtbTurns > 0) {
                                    if (Globals.PARTY_SLOT[2] == uiCombo["cboQTB"]) {
                                        qtbTurns -= 1;
                                        SubQTB(2);
                                        Globals.LAST_HOTKEY = Constants.GetTime();
                                    } else {
                                        SubQTB(2);
                                        Globals.LAST_HOTKEY = Constants.GetTime();
                                    }
                                }
                            } else if (Globals.HOTKEY == (Hotkey.KEY_CIRCLE + Hotkey.KEY_R2)) { //Black Room
                                if (Globals.DIFFICULTY_MODE.Equals("Hell")) {
                                    Constants.WriteGLogOutput("Killing monsters is not available in Hell Mode.");
                                } else {
                                    /*
                                    if (Globals.CheckDMScript("btnBlackRoom")) {
                                        if ((Globals.MAP >= 5 && Globals.MAP <= 7) || (Globals.MAP >= 624 && Globals.MAP <= 625)) {
                                            for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                                                Globals.MONSTER_TABLE[i].Write("HP", 0);
                                            }
                                            Constants.WriteGLogOutput("Monsters killed.");
                                        }
                                        Globals.LAST_HOTKEY = Constants.GetTime();
                                    }
                                    */
                                }
                            } else if (Globals.HOTKEY == (Hotkey.KEY_L1 + Hotkey.KEY_CIRCLE)) { //Music Speed
                                if (Emulator.ReadByte("MUSIC_SPEED_BATTLE") != 0) {
                                    saveMusicSpeed = Emulator.ReadByte("MUSIC_SPEED_BATTLE");
                                    Emulator.WriteByte("MUSIC_SPEED_BATTLE", 0);
                                    Constants.WriteGLogOutput("Music speed nulled.");
                                } else {
                                    Constants.WriteGLogOutput("Music speed returned to normal.");
                                    Emulator.WriteByte("MUSIC_SPEED_BATTLE", saveMusicSpeed);
                                }
                            } else if (Globals.HOTKEY == (Hotkey.KEY_SELECT + Hotkey.KEY_START)) {
                                if (Globals.DIFFICULTY_MODE.Equals("NormalHard") || Globals.DIFFICULTY_MODE.Equals("Hard")) {
                                    if (Globals.ENCOUNTER_ID == 411) {
                                        Emulator.Battle.MonsterTable[0].AT = 270;
                                        Emulator.Battle.MonsterTable[0].MAT = 235;
                                        Emulator.Battle.MonsterTable[1].AT = 250;
                                        Emulator.Battle.MonsterTable[1].MAT = 235;
                                        Emulator.Battle.MonsterTable[2].AT = 250;
                                        Emulator.Battle.MonsterTable[2].MAT = 235;
                                        Constants.WriteGLogOutput("Nerfed.");
                                        Globals.LAST_HOTKEY = Constants.GetTime();
                                    } else if (Globals.ENCOUNTER_ID == 442) {
                                        Emulator.Battle.MonsterTable[0].AT = 332;
                                        Emulator.Battle.MonsterTable[0].MAT = 290;
                                        Constants.WriteGLogOutput("Nerfed.");
                                        Globals.LAST_HOTKEY = Constants.GetTime();
                                    } else if (Globals.ENCOUNTER_ID == 443) {
                                        Emulator.Battle.MonsterTable[0].AT = 247;
                                        Emulator.Battle.MonsterTable[0].MAT = 220;
                                        Emulator.Battle.MonsterTable[1].Max_HP = 8000;
                                        Emulator.Battle.MonsterTable[1].AT = 247;
                                        Emulator.Battle.MonsterTable[1].MAT = 220;
                                        Emulator.Battle.MonsterTable[2].Max_HP = 8000;
                                        Emulator.Battle.MonsterTable[2].AT = 247;
                                        Emulator.Battle.MonsterTable[2].MAT = 220;
                                        Emulator.Battle.MonsterTable[3].Max_HP = 8000;
                                        Emulator.Battle.MonsterTable[3].AT = 247;
                                        Emulator.Battle.MonsterTable[3].MAT = 220;
                                        Emulator.Battle.MonsterTable[4].Max_HP = 8000;
                                        Emulator.Battle.MonsterTable[4].AT = 247;
                                        Emulator.Battle.MonsterTable[4].MAT = 220;
                                        Constants.WriteGLogOutput("Nerfed.");
                                        Globals.LAST_HOTKEY = Constants.GetTime();
                                    } else if (Globals.ENCOUNTER_ID == 390) {
                                        Emulator.Battle.MonsterTable[0].AT = 38;
                                        Emulator.Battle.MonsterTable[0].MAT = 38;
                                        Constants.WriteGLogOutput("Nerfed.");
                                        Globals.LAST_HOTKEY = Constants.GetTime();
                                    } else if (Globals.ENCOUNTER_ID == 396) {
                                        Emulator.Battle.MonsterTable[0].AT = 57;
                                        Emulator.Battle.MonsterTable[0].MAT = 72;
                                        Emulator.Battle.MonsterTable[0].DF = 140;
                                        Emulator.Battle.MonsterTable[0].MDF = 200;
                                        Constants.WriteGLogOutput("Nerfed.");
                                        Globals.LAST_HOTKEY = Constants.GetTime();
                                    } else if (Globals.ENCOUNTER_ID == 430) {
                                        Emulator.Battle.MonsterTable[0].HP = 17500;
                                        Emulator.Battle.MonsterTable[0].Max_HP = 17500;
                                        Emulator.Battle.MonsterTable[0].AT = 115;
                                        Emulator.Battle.MonsterTable[0].MAT = 110;
                                        Emulator.Battle.MonsterTable[0].DF = 180;
                                        Emulator.Battle.MonsterTable[0].MDF = 180;
                                        Emulator.Battle.MonsterTable[1].HP = 17500;
                                        Emulator.Battle.MonsterTable[1].Max_HP = 17500;
                                        Emulator.Battle.MonsterTable[1].AT = 103;
                                        Emulator.Battle.MonsterTable[1].MAT = 130;
                                        Emulator.Battle.MonsterTable[1].DF = 160;
                                        Emulator.Battle.MonsterTable[1].MDF = 220;
                                        Emulator.Battle.MonsterTable[2].HP = 20000;
                                        Emulator.Battle.MonsterTable[2].Max_HP = 20000;
                                        Emulator.Battle.MonsterTable[2].AT = 124;
                                        Emulator.Battle.MonsterTable[2].MAT = 102;
                                        Emulator.Battle.MonsterTable[2].DF = 140;
                                        Emulator.Battle.MonsterTable[2].MDF = 140;
                                        Constants.WriteGLogOutput("Nerfed.");
                                        Globals.LAST_HOTKEY = Constants.GetTime();
                                    } else {
                                        Constants.WriteGLogOutput("Nerfing not available.");
                                        Globals.LAST_HOTKEY = Constants.GetTime();
                                    }
                                }
                            } else if (Globals.HOTKEY == (Hotkey.KEY_SELECT + Hotkey.KEY_R3)) {
                                if (Globals.DIFFICULTY_MODE.Equals("NormalHard") || Globals.DIFFICULTY_MODE.Equals("Hard")) {
                                    if (Globals.ENCOUNTER_ID == 411) {
                                        Emulator.Battle.MonsterTable[0].HP = 50000;
                                        Emulator.Battle.MonsterTable[0].Max_HP = 50000;
                                        Emulator.Battle.MonsterTable[0].AT = 160;
                                        Emulator.Battle.MonsterTable[0].MAT = 125;
                                        Emulator.Battle.MonsterTable[0].DF = 160;
                                        Emulator.Battle.MonsterTable[0].MDF = 160;
                                        Emulator.Battle.MonsterTable[1].HP = 15000;
                                        Emulator.Battle.MonsterTable[1].Max_HP = 15000;
                                        Emulator.Battle.MonsterTable[1].AT = 140;
                                        Emulator.Battle.MonsterTable[1].MAT = 125;
                                        Emulator.Battle.MonsterTable[1].DF = 190;
                                        Emulator.Battle.MonsterTable[1].MDF = 170;
                                        Emulator.Battle.MonsterTable[2].HP = 50000;
                                        Emulator.Battle.MonsterTable[2].Max_HP = 50000;
                                        Emulator.Battle.MonsterTable[2].AT = 140;
                                        Emulator.Battle.MonsterTable[2].MAT = 125;
                                        Emulator.Battle.MonsterTable[2].DF = 345;
                                        Emulator.Battle.MonsterTable[2].MDF = 255;
                                        Constants.WriteGLogOutput("Ultra Nerfed.");
                                        Globals.LAST_HOTKEY = Constants.GetTime();
                                    } else if (Globals.ENCOUNTER_ID == 442) {
                                        Emulator.Battle.MonsterTable[0].HP = 60000;
                                        Emulator.Battle.MonsterTable[0].Max_HP = 60000;
                                        Emulator.Battle.MonsterTable[0].AT = 222;
                                        Emulator.Battle.MonsterTable[0].MAT = 190;
                                        Emulator.Battle.MonsterTable[0].DF = 170;
                                        Emulator.Battle.MonsterTable[0].MDF = 200;
                                        Constants.WriteGLogOutput("Ultra Nerfed.");
                                        Globals.LAST_HOTKEY = Constants.GetTime();
                                    } else if (Globals.ENCOUNTER_ID == 443) {
                                        Emulator.Battle.MonsterTable[0].HP = 52500;
                                        Emulator.Battle.MonsterTable[0].Max_HP = 52500;
                                        Emulator.Battle.MonsterTable[0].AT = 167;
                                        Emulator.Battle.MonsterTable[0].MAT = 140;
                                        Emulator.Battle.MonsterTable[0].DF = 720;
                                        Emulator.Battle.MonsterTable[0].MDF = 870;
                                        Emulator.Battle.MonsterTable[1].Max_HP = 4000;
                                        Emulator.Battle.MonsterTable[1].AT = 167;
                                        Emulator.Battle.MonsterTable[1].MAT = 140;
                                        Emulator.Battle.MonsterTable[2].Max_HP = 4000;
                                        Emulator.Battle.MonsterTable[2].AT = 167;
                                        Emulator.Battle.MonsterTable[2].MAT = 140;
                                        Emulator.Battle.MonsterTable[3].Max_HP = 4000;
                                        Emulator.Battle.MonsterTable[3].AT = 167;
                                        Emulator.Battle.MonsterTable[3].MAT = 140;
                                        Emulator.Battle.MonsterTable[4].Max_HP = 4000;
                                        Emulator.Battle.MonsterTable[4].AT = 167;
                                        Emulator.Battle.MonsterTable[4].MAT = 140;
                                        Constants.WriteGLogOutput("Ultra Nerfed.");
                                        Globals.LAST_HOTKEY = Constants.GetTime();
                                    } else {
                                        Constants.WriteGLogOutput("Ultra Nerfing not available.");
                                        Globals.LAST_HOTKEY = Constants.GetTime();
                                    }
                                }
                            } else if (Globals.HOTKEY == (Hotkey.KEY_L2 + Hotkey.KEY_LEFT)) { //Soa's Wargod
                                if ((134217728 & ultimateShopLimited) == 134217728) {
                                    ubSoasWargod = ubSoasWargod ? false : true;
                                    Emulator.WriteAoB("SOA_WARGOD", "06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06");
                                    Constants.WriteGLogOutput("Soa's Wargod has been " + (ubSoasWargod ? "activated" : "deactivated") + ".");
                                } else {
                                    Constants.WriteGLogOutput("You do not have Soa's Wargod.");
                                }
                                Globals.LAST_HOTKEY = Constants.GetTime();
                            } else if (Globals.HOTKEY == (Hotkey.KEY_L2 + Hotkey.KEY_RIGHT)) { //Soa's Dragoon Boost
                                if ((268435456 & ultimateShopLimited) == 268435456) {
                                    ubSoasDragoonBoost = ubSoasDragoonBoost ? false : true;
                                    Emulator.WriteShort("SOA_DRAGOON_BOOST_1", 4096);
                                    Emulator.WriteShort("SOA_DRAGOON_BOOST_2", 4096);
                                    Constants.WriteGLogOutput("Soa's Dragoon Boost has been " + (ubSoasDragoonBoost ? "activated" : "deactivated") + ".");
                                } else {
                                    Constants.WriteGLogOutput("You do not have Soa's Dragoon Boost.");
                                }
                                Globals.LAST_HOTKEY = Constants.GetTime();
                            } else if (Globals.HOTKEY == (Hotkey.KEY_L2 + Hotkey.KEY_UP)) { //Empty Dragoon Crystal
                                if ((ultimateShopLimited & 67108864) == 67108864) {
                                    bool pass = true;
                                    int characterSlot = 255, characterSlots = 255;
                                    for (int i = 0; i < 3; i++) {
                                        if (Globals.PARTY_SLOT[i] < 9) {
                                            if (Emulator.Battle.CharacterTable[i].SP < 100) {
                                                pass = false;
                                            }

                                            if (Emulator.Battle.CharacterTable[i].Menu == 8)
                                                characterSlot = i;

                                            characterSlots = i + 1;
                                        }
                                    }

                                    if (pass && characterSlots == 3 && characterSlot < 9 && Emulator.Battle.CharacterTable[characterSlot].Menu < 128) {
                                        Emulator.Battle.CharacterTable[characterSlot].Menu = (byte) (Emulator.Battle.CharacterTable[characterSlot].Menu + 128);
                                    } else {
                                        Constants.WriteGLogOutput("You do not meet the requirements to create a special.");
                                    }
                                } else {
                                    Constants.WriteGLogOutput("You do not have an Empty Dragoon Crystal.");
                                }

                                Globals.LAST_HOTKEY = Constants.GetTime();
                            }
                        }
                    }
                }

                Thread.Sleep(500);
            }
        }

        public void OtherController() {
            string currentScript = "";
            int run = 1;
            bool readerWrite = false;
            while (run == 1 && Constants.RUN) {
                foreach (SubScript script in lstOther.Items) {
                    if (script.state == ScriptState.DISABLED)
                        continue;
                    currentScript = script.ToString();
                    this.Dispatcher.BeginInvoke(new Action(() => {
                        run = script.Run();
                    }), DispatcherPriority.ContextIdle);
                }

                if (Globals.CheckDMScript("btnReader") && readerWindow.WRITE_TEXT && readerWrite)
                    readerWindow.WriteToText();

                readerWrite = readerWrite ? false : true;

                Thread.Sleep(1000);

                this.Dispatcher.BeginInvoke(new Action(() => {
                    if (Globals.NO_DART != null) {
                        if (btnNoDart.Background.ToString() == "#FFFFA8A8") {
                            btnNoDart.Background = new SolidColorBrush(Color.FromArgb(255, 168, 211, 255));
                        }
                    }
                }), DispatcherPriority.ContextIdle);
            }
        }

        public void UltimateController() {
            while (Emulator.Memory.GameState == GameState.Battle && Controller.Main.StatsChanged && Constants.RUN) {
                if (Globals.ENCOUNTER_ID == 442) {
                    bool finalBurst = false;
                    if (ultimateHP[0] > 360000) {
                        if (ubTrackMTP[0] > Emulator.Battle.MonsterTable[0].Turn) {
                            byte[] dragoonMagic = { 80, 81, 82 };
                            int chance = new Random().Next(1, 100);
                            if (chance > 60) {
                                ubZiegDragoon = dragoonMagic[0];
                            } else if (chance > 10) {
                                ubZiegDragoon = dragoonMagic[1];
                            } else {
                                ubZiegDragoon = dragoonMagic[2];
                                finalBurst = true;
                            }
                        }
                    } else if (ultimateHP[0] > 180000) {
                        if (ubTrackMTP[0] > Emulator.Battle.MonsterTable[0].Turn) {
                            byte[] dragoonMagic = { 80, 81, 82, 83 };
                            int chance = new Random().Next(1, 100);
                            if (chance > 65) {
                                ubZiegDragoon = dragoonMagic[2];
                                finalBurst = true;
                            } else if (chance > 35) {
                                ubZiegDragoon = dragoonMagic[1];
                            } else if (chance > 15) {
                                ubZiegDragoon = dragoonMagic[0];
                            } else {
                                ubZiegDragoon = dragoonMagic[3];
                            }
                        }
                    } else {
                        if (ubTrackMTP[0] > Emulator.Battle.MonsterTable[0].Turn) {
                            byte[] dragoonMagic = { 80, 81, 82, 83 };
                            int chance = new Random().Next(1, 100);
                            if (chance > 65) {
                                ubZiegDragoon = dragoonMagic[2];
                                finalBurst = true;
                            } else if (chance > 45) {
                                ubZiegDragoon = dragoonMagic[1];
                            } else if (chance > 25) {
                                ubZiegDragoon = dragoonMagic[0];
                            } else {
                                ubZiegDragoon = dragoonMagic[3];
                            }
                        }
                    }

                    if (Emulator.Battle.MonsterTable[0].Action == 12)
                        Emulator.WriteByte(Globals.M_POINT - 0x50, ubZiegDragoon);

                    if (Emulator.ReadByte("TARGET_1") == 254 || Emulator.ReadByte("TARGET_2") == 254 || Emulator.ReadByte(Globals.M_POINT + 0xAC4) == 254 || finalBurst) {
                        byte lowestHPSlot = 0;
                        int lowestHP = 65535;

                        for (int i = 0; i < 3; i++) {
                            if (Globals.PARTY_SLOT[i] < 9) {
                                int hp = Emulator.Battle.CharacterTable[i].HP;
                                if (hp < lowestHP && hp > 0) {
                                    lowestHPSlot = (byte) i;
                                    lowestHP = hp;
                                }
                            }
                        }

                        Emulator.WriteByte("TARGET_1", lowestHPSlot);
                        Emulator.WriteByte("TARGET_2", lowestHPSlot);
                        Emulator.WriteByte(Globals.M_POINT + 0xAC4, lowestHPSlot);
                        Emulator.WriteByte(Globals.M_POINT - 0x54, lowestHPSlot);
                        Emulator.WriteByte(Globals.M_POINT - 0x40, lowestHPSlot);
                    }

                    Emulator.WriteByte("SCREEN_FADE", 2);

                    ubTrackMTP[0] = Emulator.Battle.MonsterTable[0].Turn;
                }
                Thread.Sleep(10);
            }
        }
        #endregion

        #region Scripts

        #region Field

        #region Save Anywhere
        public void SaveAnywhere() {
            if (Emulator.Memory.GameState != GameState.Battle) {
                Emulator.WriteShort("SAVE_ANYWHERE", 1);
            }
        }
        #endregion

        #region Shop Changes
        
        public void ShopChanges() {
            if (Emulator.Memory.GameState != GameState.Battle && SHOP_MAPS.Contains((int) Globals.MAP)) {
                if (Emulator.ReadByte("SHOP_DISC_SWAP") == 0) {
                    SHOP_DISC_SWAP = true;
                }
                if (SHOP_DISC_SWAP) {
                    ShopContentChange();
                    if (SHOP_CHANGED) {
                        ShopTableChange(true);
                        SHOP_DISC_SWAP = false;
                    }
                } else {
                    ShopTableChange(false);
                }
            }
            if (Emulator.ReadByte("OVERWORLD") == 3) {
                SHOP_CHANGED = false;
            }
        }

        public void ShopContentChange() {
            if (SHOP_CHANGED == false && Emulator.ReadByte("SHOP_BUY-SELL") == 3) {
                long address = Constants.GetAddress("SHOP_CONTENT");
                byte shop_og_size = Emulator.ReadByte(address + 0x44);
                int shop = Emulator.ReadByte("SHOP_ID");
                int i = 0;
                foreach (byte item in Globals.DICTIONARY.ShopList[shop]) {
                    Emulator.WriteUShort(address + i * 4, (ushort) item);
                    Emulator.WriteUShort(address + 0x2 + i * 4, (ushort) (Globals.DICTIONARY.ItemList[item].Sell_Price * 2));
                    i += 1;
                }
                byte[] current_shop = Globals.DICTIONARY.ShopList[shop];
                byte[] filtered_shop = current_shop.Where(x => x != 255).ToArray();
                int shop_size = filtered_shop.Length;
                Emulator.WriteByte(address + 0x44, (byte) shop_size);
                if (filtered_shop[0] >= 192) {
                    Emulator.WriteByte(address + 0x45, 1);
                } else {
                    Emulator.WriteByte(address + 0x45, 0);
                }
                if (shop_og_size > shop_size) {
                    for (int z = shop_size; z < 17; z++) {
                        Emulator.WriteUShort(address + z * 4, (ushort) 255);
                        Emulator.WriteUShort(address + 2 + z * 4, (ushort) 0);
                    }
                }
                Constants.WriteDebug("Contents of Shop " + shop + " Changed");
                SHOP_CHANGED = true;
            } else {
                SHOP_CHANGED = false;
            }
        }
        public void ShopTableChange(bool bypass) {
            if (Emulator.ReadByte("OVERWORLD") == 1) {
                if (!SHOP_CHANGED || bypass) {
                    if (!bypass && Emulator.ReadByte("TRANSITION") != 12) {
                        return;
                    }
                    long address = Constants.GetAddress("SHOP_LIST");
                    int shopcount = 0;
                    foreach (byte[] shop in Globals.DICTIONARY.ShopList) {
                        int itemcount = 0;
                        foreach (byte item in shop) {
                            if (itemcount == 0) {
                                if (item > 192) {
                                    Emulator.WriteByte(address + shopcount * 0x40, 1);
                                } else {
                                    Emulator.WriteByte(address + shopcount * 0x40, 0);
                                }
                            }
                            Emulator.WriteByte(address + itemcount * 0x4 + shopcount * 0x40 + 1, item);
                            itemcount++;
                        }
                        shopcount++;
                    }
                    SHOP_CHANGED = true;
                    address = Constants.GetAddress("SHOP_PRICE");
                    int i = 0;
                    foreach (dynamic item in Globals.DICTIONARY.ItemList) {
                        Emulator.WriteUShort(address + i * 0x2, (ushort) item.Sell_Price);
                        i++;
                    }
                    SHOP_CHANGED = true;
                    Constants.WriteDebug("Shop Table and Prices Changed");
                }
            }
        }

        public static int ReadShop(int address) {
            return Emulator.ReadShort(address + ShopOffset());
        }

        public static void WriteShop(int address, ushort value) {
            Emulator.WriteUShort(address + ShopOffset(), value);
        }

        public static int ShopOffset() {
            int offset = 0x0;
            if (Constants.REGION == Region.JPN) {
                offset -= 0x4D90;
            } else if (Constants.REGION == Region.GER) {
                offset += 0x120;
            }
            return offset;
        }


        public void ShopAnywhere(byte shop) {
            if (Emulator.ReadByte("OVERWORLD") == 1) {
                ShopChanges();
                Emulator.WriteByte("SHOP_ID", shop);
                Emulator.WriteByte("MENU", 6);
                Emulator.WriteByte("TRANSITION", 13);

            }
        }

        #endregion

        #region Auto Charm Potion
        public void AutoCharmPotion() {
            if ((Emulator.ReadShort("BATTLE_VALUE") > 3850 && Emulator.ReadShort("BATTLE_VALUE") < 9999) && Emulator.ReadInt("GOLD") >= 8) {
                Emulator.WriteInt("GOLD", Emulator.ReadInt("GOLD") - 8);
                Emulator.WriteShort("BATTLE_VALUE", 0);
            }
        }
        #endregion

        #region Switch EXP
        public void SwitchEXP() {
            long char1 = Constants.GetAddress("CHAR_TABLE") + (0x2C * cboSwitch1.SelectedIndex);
            long char2 = Constants.GetAddress("CHAR_TABLE") + (0x2C * cboSwitch2.SelectedIndex);
            int maxEXP = Globals.DIFFICULTY_MODE.Contains("Hell") ? 160000 : 80000;

            if (char1 != char2) {
                if (Emulator.ReadByte(char1 + 0x4) != 0 && Emulator.ReadByte(char2 + 0x4) != 0) {
                    if (Emulator.ReadInt(char1) < maxEXP && Emulator.ReadInt(char2) < maxEXP) {
                        int tempEXP = Emulator.ReadInt(char1);
                        Emulator.WriteInt(char1, Emulator.ReadInt(char2));
                        Emulator.WriteInt(char2, tempEXP);
                        Constants.WriteGLog("Switch exp of " + cboSwitch1.Text + " and " + cboSwitch2.Text);
                    } else {
                        Constants.WriteGLog("One of the characters has " + maxEXP + " or more EXP and can't be switched.");
                    }
                } else {
                    Constants.WriteGLog("One of the characters are not in the party");
                }
            } else {
                Constants.WriteGLog("You can't switch the same character's EXP");
            }
        }
        #endregion

        #region Early Additions
        public void EarlyAdditions() {
            if (Emulator.Memory.GameState != GameState.Battle && !earlyAdditionsOnFieldEntry) {
                long address = Constants.GetAddress("MENU_ADDITION_TABLE_FLAT");
                long address2 = Constants.GetAddress("CHAR_TABLE") + 0x22;
                //Dart
                Emulator.WriteByte(address + 0xE * 3, 13); //Crush Dance
                Emulator.WriteByte(address + 0xE * 4, 18); //Madness Hero
                Emulator.WriteByte(address + 0xE * 5, 23); //Moon Strike
                Emulator.WriteByte(address + 0xE * 6, 60); //Blazying Dynamo
                if (Emulator.ReadByte(address2) >= 80 &&
                    Emulator.ReadByte(address2 + 0x1) >= 80 &&
                    Emulator.ReadByte(address2 + 0x2) >= 80 &&
                    Emulator.ReadByte(address2 + 0x3) >= 80 &&
                    Emulator.ReadByte(address2 + 0x4) >= 80 &&
                    Emulator.ReadByte(address2 + 0x5) >= 80) {
                    Emulator.WriteByte(address + 0xE * 6, 29); //Blazying Dynamo
                }
                //Lavitz
                Emulator.WriteByte(address + 0xE * 2 + 0x70, 10); //Rod Typhoon
                Emulator.WriteByte(address + 0xE * 3 + 0x70, 16); //Gust of Wind Dance
                Emulator.WriteByte(address + 0xE * 4 + 0x70, 60); //Flower Storm
                if (Emulator.ReadByte(address2 + 0x2C) >= 80 &&
                    Emulator.ReadByte(address2 + 0x2C + 0x1) >= 80 &&
                    Emulator.ReadByte(address2 + 0x2C + 0x2) >= 80 &&
                    Emulator.ReadByte(address2 + 0x2C + 0x3) >= 80) {
                    Emulator.WriteByte(address + 0xE * 4 + 0x70, 21); //Flower Storm
                }
                //Albert
                Emulator.WriteByte(address + 0xE * 2 + 0x70, 10); //Rod Typhoon
                Emulator.WriteByte(address + 0xE * 3 + 0x70, 16); //Gust of Wind Dance
                Emulator.WriteByte(address + 0xE * 4 + 0x70, 60); //Flower Storm
                if (Emulator.ReadByte(address2 + 0xDC) >= 80 &&
                    Emulator.ReadByte(address2 + 0xDC + 0x1) >= 80 &&
                    Emulator.ReadByte(address2 + 0xDC + 0x2) >= 80 &&
                    Emulator.ReadByte(address2 + 0xDC + 0x3) >= 80) {
                    Emulator.WriteByte(address + 0xE * 4 + 0x70, 21); //Flower Storm
                }
                //Rose
                Emulator.WriteByte(address + 0xE * 1 + 0xC4, 8); //More & More
                Emulator.WriteByte(address + 0xE * 2 + 0xC4, 15); //Hard Blade
                Emulator.WriteByte(address + 0xE * 3 + 0xC4, 60); //Demon's Dance
                if (Emulator.ReadByte(address2 + 0x84) >= 80 &&
                    Emulator.ReadByte(address2 + 0x84 + 0x1) >= 80 &&
                    Emulator.ReadByte(address2 + 0x84 + 0x2) >= 80) {
                    Emulator.WriteByte(address + 0xE * 3 + 0xC4, 21); //Demon's Dance
                }
                //Kongol
                Emulator.WriteByte(address + 0xE * 1 + 0x10A, 10); //Inferno
                Emulator.WriteByte(address + 0xE * 2 + 0x10A, 60); //Bone Crush
                if (Emulator.ReadByte(address2 + 0x134) >= 80 &&
                    Emulator.ReadByte(address2 + 0x134 + 0x1) >= 80) {
                    Emulator.WriteByte(address + 0xE * 2 + 0x10A, 20); //Bone Crush
                }
                //Meru
                Emulator.WriteByte(address + 0xE * 1 + 0x142, 6); //Hammer Spin
                Emulator.WriteByte(address + 0xE * 2 + 0x142, 12); //Cool Boogie
                Emulator.WriteByte(address + 0xE * 3 + 0x142, 18); //Cat's Cradle
                Emulator.WriteByte(address + 0xE * 4 + 0x142, 60); //Perky Step
                if (Emulator.ReadByte(address2 + 0x108) >= 80 &&
                    Emulator.ReadByte(address2 + 0x108 + 0x1) >= 80 &&
                    Emulator.ReadByte(address2 + 0x108 + 0x2) >= 80 &&
                    Emulator.ReadByte(address2 + 0x108 + 0x3) >= 80) {
                    Emulator.WriteByte(address + 0xE * 4 + 0x142, 22);  //Perky Step
                }
                //Haschel
                Emulator.WriteByte(address + 0xE * 1 + 0x196, 5); // Flurry of Styx
                Emulator.WriteByte(address + 0xE * 2 + 0x196, 10); //Summon 4 Gods
                Emulator.WriteByte(address + 0xE * 3 + 0x196, 16); //5 Ring Shattering
                Emulator.WriteByte(address + 0xE * 4 + 0x196, 22); //Hex Hammer
                Emulator.WriteByte(address + 0xE * 5 + 0x196, 60); //Omni-Sweep        
                if (Emulator.ReadByte(address2 + 0xB0) >= 80 &&
                    Emulator.ReadByte(address2 + 0xB0 + 0x1) >= 80 &&
                    Emulator.ReadByte(address2 + 0xB0 + 0x2) >= 80 &&
                    Emulator.ReadByte(address2 + 0xB0 + 0x3) >= 80 &&
                    Emulator.ReadByte(address2 + 0xB0 + 0x4) >= 80) {
                    Emulator.WriteByte(address + 0xE * 5 + 0x196, 25); //Omni-Sweep 
                }
                earlyAdditionsOnFieldEntry = true;
            } else {
                if (Emulator.Memory.GameState == GameState.Battle)
                    earlyAdditionsOnFieldEntry = false;
            }
        }

        public void TurnOffEarlyAdditions() {
            if (Emulator.Memory.GameState != GameState.Battle) {
                long address = Constants.GetAddress("MENU_ADDITION_TABLE_FLAT");
                //Dart
                Emulator.WriteByte(address * 0xE * 3, 15); //Crush Dance
                Emulator.WriteByte(address * 0xE * 4, 22); //Madness Hero
                Emulator.WriteByte(address * 0xE * 5, 29); //Moon Strike
                Emulator.WriteByte(address * 0xE * 6, 255); //Blazying Dynamo
                //Lavitz
                Emulator.WriteByte(address * 0xE * 2 + 0x70, 7); //Rod Typhoon
                Emulator.WriteByte(address * 0xE * 3 + 0x70, 11); //Gust of Wind Dance
                Emulator.WriteByte(address * 0xE * 4 + 0x70, 255); //Flower Storm
                //Rose
                Emulator.WriteByte(address + 0xE * 1 + 0xC4, 14); //More & More
                Emulator.WriteByte(address + 0xE * 2 + 0xC4, 19); //Hard Blade
                Emulator.WriteByte(address + 0xE * 3 + 0xC4, 255); //Demon//s Dance
                //Kongol
                Emulator.WriteByte(address + 0xE * 1 + 0x10A, 23); //Inferno
                Emulator.WriteByte(address + 0xE * 2 + 0x10A, 255); //Bone Crush
                //Meru
                Emulator.WriteByte(address * 0xE * 1 + 0x142, 21); //Hammer Spin
                Emulator.WriteByte(address * 0xE * 2 + 0x142, 26); //Cool Boogie
                Emulator.WriteByte(address * 0xE * 3 + 0x142, 30); //Cat//s Cradle
                Emulator.WriteByte(address * 0xE * 4 + 0x142, 255); //Perky Step
                //Haschel
                Emulator.WriteByte(address * 0xE * 1 + 0x196, 14); // Flurry of Styx
                Emulator.WriteByte(address * 0xE * 2 + 0x196, 18); //Summon 4 Gods
                Emulator.WriteByte(address * 0xE * 3 + 0x196, 22); //5 Ring Shattering
                Emulator.WriteByte(address * 0xE * 4 + 0x196, 26); //Hex Hammer
                Emulator.WriteByte(address * 0xE * 5 + 0x196, 255); //Omni-Sweep   
                earlyAdditionsOnFieldEntry = false;
            }
        }
        #endregion

        #region Shops
        public void HeroTicketShop() {
            if (lstTicketShop.SelectedIndex == 0)
                BuyTicket(15, 1);
            else if (lstTicketShop.SelectedIndex == 1)
                BuyTicket(60, 5);
            else if (lstTicketShop.SelectedIndex == 2)
                BuyTicket(100, 10);
        }

        public void BuyTicket(int cost, int tickets) {
            if (Emulator.ReadInt("GOLD") >= cost) {
                Emulator.WriteInt("GOLD", Emulator.ReadInt("GOLD") - cost);
                Emulator.WriteInt("HERO_TICKETS", Emulator.ReadInt("HERO_TICKETS") + tickets);
            }
            Constants.WriteGLogOutput("You have " + Emulator.ReadInt("HERO_TICKETS") + " tickets. Gold: " + Emulator.ReadInt("GOLD"));
        }

        public void HeroItemShop() {
            if (lstHeroShop.SelectedIndex == 0)
                BuyShopItem(0xD3, 0, 20);
            else if (lstHeroShop.SelectedIndex == 1)
                BuyShopItem(0xDD, 0, 40);
            else if (lstHeroShop.SelectedIndex == 2)
                BuyShopItem(0xE9, 0, 60);
            else if (lstHeroShop.SelectedIndex == 3)
                BuyShopItem(0xEA, 0, 100);
        }

        public void UltimateItemShop() {
            if (Globals.CHAPTER < 4) {
                Constants.WriteGLogOutput("You must have completed Chapter 3 to purchase from this shop.");
                return;
            }

            int gold = Emulator.ReadInt("GOLD");
            int price = uShopPrices[lstUltimateShop.SelectedIndex];
            int item = uItemId[lstUltimateShop.SelectedIndex];
            int oneLimited = uLimited[lstUltimateShop.SelectedIndex];

            if (oneLimited > 0) {
                if ((ultimateShopLimited & oneLimited) == oneLimited) {
                    Constants.WriteGLogOutput("This item can only be bought once.");
                    return;
                }
            }

            if (gold < price) {
                Constants.WriteGLogOutput("Not enough gold");
                return;
            }

            if (item > 0) {
                bool shop = BuyShopItem((byte) item, price, 0, true);
                if (shop)
                    ultimateShopLimited += oneLimited;
            } else {
                if (oneLimited > 0)
                    ultimateShopLimited += oneLimited;
                Emulator.WriteInt("GOLD", gold - price);
                Constants.WriteGLogOutput("Bought item. Gold: " + (gold - price));
            }
        }

        public bool BuyShopItem(byte item, int goldCost, int ticketCost, bool armor = false) {
            int gold = Emulator.ReadInt("GOLD");
            int tickets = Emulator.ReadInt("HERO_TICKETS");

            if (gold < goldCost) {
                Constants.WriteGLogOutput("Not enough gold.");
                return false;
            }

            if (tickets < ticketCost) {
                Constants.WriteGLogOutput("Not enough tickets.");
                return false;
            }

            long address = armor ? Constants.GetAddress("ARMOR_INVENTORY") : Constants.GetAddress("INVENTORY");
            bool slots = false;

            if (armor) {
                for (int i = 0; i < 255; i++) {
                    if (Emulator.ReadByte(address) == 255) {
                        slots = true;
                        Emulator.WriteByte(address, item);

                        if (goldCost > 0)
                            Emulator.WriteInt("GOLD", gold - goldCost);

                        if (ticketCost > 0)
                            Emulator.WriteUShort("HERO_TICKETS", (ushort) (tickets - ticketCost));

                        break;
                    }
                    address += 1;
                }
            } else {
                for (int i = 0; i < inventorySize; i++) {
                    if (Emulator.ReadByte(address) == 255) {
                        slots = true;
                        Emulator.WriteByte(address, item);

                        if (goldCost > 0)
                            Emulator.WriteInt("GOLD", gold - goldCost);

                        if (ticketCost > 0)
                            Emulator.WriteUShort("HERO_TICKETS", (ushort) (tickets - ticketCost));

                        break;
                    }
                    address += 1;
                }
            }

            if (slots)
                Constants.WriteGLogOutput("Item bought. Gold: " + (gold - goldCost) + " | Tickets: " + (tickets - ticketCost));
            else
                Constants.WriteGLogOutput("Inventory full.");

            return slots;
        }
        #endregion

        #region Increase Text Speed
        public void IncreaseTextSpeed() {
            if (Emulator.Memory.GameState != GameState.Battle) {
                Emulator.WriteShort("TEXT_SPEED", 1);
            }
        }

        public void AutoText() {
            if (Emulator.Memory.GameState != GameState.Battle) {
                Emulator.WriteShort("AUTO_TEXT", 13378);
            }
        }
        #endregion
        #endregion

        #region Both

        #region Battle Stats Display
        public void FieldUI() {
            lblEncounter.Text = "Encounter Value: " + Globals.BATTLE_VALUE;
            lblEnemyID.Text = "Enemy ID: " + Globals.ENCOUNTER_ID;
            lblMapID.Text = "Map ID: " + Globals.MAP;
        }

        public void BattleUI() {
            if (Emulator.Memory.GameState == GameState.Battle && Controller.Main.StatsChanged) {
                for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                    if (!Constants.BATTLE_UI) {
                        // monsterDisplay[i, 0].Text = Emulator.ReadName(Constants.GetAddress("MONSTERS_NAMES") + (0x2C * i)); TODO
                        Globals.MONSTER_NAME[i] = monsterDisplay[i, 0].Text;
                    }

                    if (Globals.CheckDMScript("btnUltimateBoss")) {
                        if (ultimateMaxHP[i] > 65535) {
                            monsterDisplay[i, 1].Text = " " + ultimateHP[i] + "/" + ultimateMaxHP[i];
                        } else {
                            monsterDisplay[i, 1].Text = " " + Convert.ToString(Emulator.Battle.MonsterTable[i].HP, 10) + "/" + Emulator.Battle.MonsterTable[i].Max_HP;
                        }
                    } else {
                        monsterDisplay[i, 1].Text = " " + Convert.ToString(Emulator.Battle.MonsterTable[i].HP, 10) + "/" + Emulator.Battle.MonsterTable[i].Max_HP;
                    }

                    monsterDisplay[i, 2].Text = " " + Emulator.Battle.MonsterTable[i].AT + "/" + Emulator.Battle.MonsterTable[i].MAT;
                    monsterDisplay[i, 3].Text = " " + Emulator.Battle.MonsterTable[i].DF + "/" + Emulator.Battle.MonsterTable[i].MDF;
                    monsterDisplay[i, 4].Text = " " + Emulator.Battle.MonsterTable[i].SPD;
                    monsterDisplay[i, 5].Text = " " + Emulator.Battle.MonsterTable[i].Turn;

                    EnrageMode(i);
                }
                for (int i = 0; i < 3; i++) {
                    if (Globals.PARTY_SLOT[i] < 9) {
                        characterDisplay[i, 0].Text = Constants.GetCharName(Globals.PARTY_SLOT[i]);
                        Globals.CHARACTER_NAME[i] = characterDisplay[i, 0].Text;
                        characterDisplay[i, 1].Text = " " + Emulator.Battle.CharacterTable[i].HP + "/" + Emulator.Battle.CharacterTable[i].Max_HP + "\r\n\r\n " + Emulator.Battle.CharacterTable[i].MP + "/" + Emulator.Battle.CharacterTable[i].Max_MP;
                        characterDisplay[i, 2].Text = " " + Emulator.Battle.CharacterTable[i].AT + "\r\n\r\n " + Emulator.Battle.CharacterTable[i].MAT;
                        characterDisplay[i, 3].Text = " " + Emulator.Battle.CharacterTable[i].DF + "\r\n\r\n " + Emulator.Battle.CharacterTable[i].MDF;
                        characterDisplay[i, 4].Text = " " + Emulator.Battle.CharacterTable[i].A_HIT + "/" + Emulator.Battle.CharacterTable[i].M_HIT + "\r\n\r\n " + Emulator.Battle.CharacterTable[i].A_AV + "/" + Emulator.Battle.CharacterTable[i].M_AV;
                        characterDisplay[i, 5].Text = " " + Emulator.Battle.CharacterTable[i].DAT + "\r\n\r\n " + Emulator.Battle.CharacterTable[i].DMAT;
                        characterDisplay[i, 6].Text = " " + Emulator.Battle.CharacterTable[i].DDF + "\r\n\r\n " + Emulator.Battle.CharacterTable[i].DMDF;
                        characterDisplay[i, 7].Text = " " + Emulator.Battle.CharacterTable[i].SPD + "\r\n\r\n " + Emulator.Battle.CharacterTable[i].SP;
                        characterDisplay[i, 8].Text = " " + Emulator.Battle.CharacterTable[i].Turn;
                    }
                }

                Constants.BATTLE_UI = true;
                TurnOrder();
            } else {
                if (Emulator.Memory.GameState != GameState.Battle && Constants.BATTLE_UI) {
                    Constants.BATTLE_UI = false;
                    for (int i = 0; i < 5; i++) {
                        for (int x = 0; x < 6; x++) {
                            monsterDisplay[i, x].Text = "";
                        }
                    }
                    for (int i = 0; i < 3; i++) {
                        for (int x = 0; x < 9; x++) {
                            characterDisplay[i, x].Text = "";
                        }
                    }
                    lblTurnOrder.Text = "Turn Order: ";
                }
            }
        }

        public void TurnOrder() {
            try {
                object[,] battleTurns = new object[9, 3];
                int lastNumber = 0;
                object temp1;
                object temp2;
                string turnLabel = "";
                for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                    if (Emulator.ReadShort(Globals.M_POINT - (i * 0x388)) > 0) {
                        battleTurns[lastNumber, 0] = monsterDisplay[i, 0].Text;
                        battleTurns[lastNumber, 1] = Emulator.ReadShort(Globals.M_POINT - (i * 0x388) + 0x44);
                        lastNumber += 1;
                    }
                }
                for (int i = 0; i < 3; i++) {
                    if (Globals.PARTY_SLOT[i] < 9) {
                        if (Emulator.ReadShort(Globals.C_POINT - (i * 0x388)) > 0) {
                            battleTurns[lastNumber, 0] = characterDisplay[i, 0].Text;
                            battleTurns[lastNumber, 1] = Emulator.ReadShort(Globals.C_POINT - (i * 0x388) + 0x44);
                            lastNumber += 1;
                        }
                    }
                }
                for (int i = lastNumber - 1; i >= 0; i--) {
                    for (int x = 0; x < i; x++) {
                        if (Convert.ToInt16(battleTurns[x, 1]) < Convert.ToInt16((battleTurns[x + 1, 1]))) {
                            temp1 = battleTurns[x, 0];
                            temp2 = battleTurns[x, 1];
                            battleTurns[x, 0] = battleTurns[x + 1, 0];
                            battleTurns[x, 1] = battleTurns[x + 1, 1];
                            battleTurns[x + 1, 0] = temp1;
                            battleTurns[x + 1, 1] = temp2;
                        }
                    }
                }
                for (int i = 0; i < lastNumber; i++) {
                    turnLabel += battleTurns[i, 0] + "»";
                }
                if (lastNumber >= 0 && turnLabel.Length > 1) {
                    lblTurnOrder.Text = "Turn Order: " + turnLabel.Substring(0, turnLabel.Length - 1);
                } else {
                    lblTurnOrder.Text = "Turn Order: ";
                }
            } catch (Exception e) { }
        }
        #endregion

        #region HP Cap Break
        public void HPCapBreakField() {
            if (Emulator.Memory.GameState != GameState.Battle && (Globals.BATTLE_VALUE > 0 && Globals.BATTLE_VALUE < 9999)) {
                for (int i = 0; i < 3; i++) {
                    if (Globals.PARTY_SLOT[i] != hpChangeSlot[i]) {
                        hpChangeSave[i] = 65535;
                        hpChangeSlot[i] = Globals.PARTY_SLOT[i];
                    }
                }
            }
        }

        public void HPCapBreakBattle() {
            if (Emulator.Memory.GameState == GameState.Battle && Controller.Main.StatsChanged && !hpCapBreakOnBattleEntry && maxHPTableLoaded) {
                if (!Globals.CHARACTER_STAT_CHANGE) {
                    for (int i = 0; i < 3; i++) {
                        if (Globals.PARTY_SLOT[i] < 9) {
                            if (hpChangeSave[i] != 65535 && hpChangeSave[i] > 9999) {
                                if (Emulator.Battle.CharacterTable[i].HP < hpChangeSave[i] && hpChangeSave[i] < Emulator.Battle.CharacterTable[i].Max_HP) {
                                    Emulator.Battle.CharacterTable[i].HP = hpChangeSave[i];
                                }
                            }
                        }
                    }
                }
                hpCapBreakOnBattleEntry = true;
            } else {
                if (Emulator.Memory.GameState != GameState.Battle && hpCapBreakOnBattleEntry) {
                    hpCapBreakOnBattleEntry = false;
                } else {
                    if (Emulator.ReadUShort("BATTLE_VALUE") == 41215 && Controller.Main.StatsChanged && hpCapBreakOnBattleEntry && maxHPTableLoaded) {
                        for (int i = 0; i < 3; i++) {
                            if (Globals.PARTY_SLOT[i] < 9)
                                hpChangeSave[i] = Emulator.Battle.CharacterTable[i].HP;
                        }
                    }
                }
            }
        }

        public void LoadMaxHPTable(bool forceLoad) {
            if ((!maxHPTableLoaded && Emulator.Memory.GameState != GameState.Battle && (Emulator.ReadShort("BATTLE_VALUE") > 1 && Emulator.ReadShort("BATTLE_VALUE") <= 5120)) || forceLoad) {
                byte tableSlot = 0;
                for (int i = 0; i < 9; i++) {
                    switch (i) {
                        case 0:
                            tableSlot = 1;
                            break;
                        case 1:
                        case 5:
                            tableSlot = 4;
                            break;
                        case 2:
                        case 8:
                            tableSlot = 6;
                            break;
                        case 3:
                            tableSlot = 5;
                            break;
                        case 4:
                            tableSlot = 2;
                            break;
                        case 6:
                            tableSlot = 3;
                            break;
                        case 7:
                            tableSlot = 0;
                            break;
                    }

                    for (int x = 1; x < 61; x++) {
                        maxHPTable[i, x - 1] = Emulator.ReadUShort("STAT_TABLE_HP_START", (tableSlot * 0x1E8) + (x * 0x8));
                    }
                }
                maxHPTableLoaded = true;
            }
        }
        #endregion

        #region Kill BGM
        public void SetKillBGMState() {
            if (uiCombo["cboKillBGM"] == 0) {
                killBGMField = true;
                killedBGMField = false;
                killBGMBattle = false;
                killedBGMBattle = false;
            } else if (uiCombo["cboKillBGM"] == 1) {
                killBGMField = false;
                killedBGMField = false;
                killBGMBattle = true;
                killedBGMBattle = false;
            } else if (uiCombo["cboKillBGM"] == 2) {
                killBGMField = true;
                killedBGMField = false;
                killBGMBattle = true;
                killedBGMBattle = false;
            }
        }

        public void KillBGM() {
            List<long> bgmScan = Emulator.ScanAoB(0xA8660, 0x2A865F, "53 53 73 71");
            foreach (var address in bgmScan) {
                for (int i = 0; i <= 255; i++) {
                    // Emulator.WriteByteDirect((long) address + i, (byte) 0); TODO
                    Thread.Sleep(10);
                }
            }
            Constants.WriteGLogOutput("Killed BGM.");
        }

        public void KillBGMField() {
            if (Emulator.Memory.GameState != GameState.Battle && !killedBGMField && Globals.BATTLE_VALUE < 9999) {
                KillBGM();
                killedBGMField = true;
                reKilledBGMField = false;
            } else {
                if (killedBGMField && Emulator.Memory.GameState == GameState.Battle) {
                    killedBGMField = false;
                    reKilledBGMField = false;
                } else {
                    if (!reKilledBGMField && Emulator.Memory.GameState != GameState.Battle && Globals.BATTLE_VALUE > 0) {
                        KillBGM();
                        reKilledBGMField = true;
                    }
                }
            }

        }

        #endregion

        #region Solo/Duo Mode
        public void SoloModeField() {
            if (Emulator.Memory.GameState != GameState.Battle && !addSoloPartyMembers) {
                if (Emulator.ReadByte("PARTY_SLOT", 0x4) != 255 || Emulator.ReadByte("PARTY_SLOT", 0x8) != 255) {
                    for (int i = 0; i < 8; i++) {
                        Emulator.WriteByte("PARTY_SLOT", 255, i + 0x4);
                    }
                }
            }
        }
        public void DuoModeField() {
            if (Emulator.Memory.GameState != GameState.Battle && !addSoloPartyMembers) {
                if (Emulator.ReadByte("PARTY_SLOT", 0x4) == 255) {
                    Emulator.WriteByte("PARTY_SLOT", Emulator.ReadByte("PARTY_SLOT"), 0x4);
                    Emulator.WriteByte("PARTY_SLOT", 0, 0x5);
                    Emulator.WriteByte("PARTY_SLOT", 0, 0x6);
                    Emulator.WriteByte("PARTY_SLOT", 0, 0x7);
                }

                if (Emulator.ReadByte("PARTY_SLOT", 0x8) != 255) {
                    for (int i = 0; i < 4; i++) {
                        Emulator.WriteByte("PARTY_SLOT", 255, i + 0x8);
                    }
                }
            }
        }

        public void SoloModeBattle() {
            if (Emulator.Memory.GameState == GameState.Battle && Controller.Main.StatsChanged && !soloModeOnBattleEntry) {
                for (int i = 0; i < 3; i++) {
                    if (Globals.PARTY_SLOT[i] < 9) {
                        if (i != uiCombo["cboSoloLeader"]) {
                            Emulator.Battle.CharacterTable[i].HP = 0;
                            Emulator.Battle.CharacterTable[i].Max_HP = 0;
                            Emulator.Battle.CharacterTable[i].HP_Regen = 200;
                            Emulator.Battle.CharacterTable[i].Turn = 10000;
                            //yeet
                            Emulator.Battle.CharacterTable[i].Pos_FB = 255;
                            Emulator.Battle.CharacterTable[i].Pos_UD = 255;
                            Emulator.Battle.CharacterTable[i].Pos_RL = 255;
                        } else {
                            Emulator.Battle.CharacterTable[i].Pos_FB = 9;
                            Emulator.Battle.CharacterTable[i].Pos_FB = 0;
                            Emulator.Battle.CharacterTable[i].Pos_FB = 0;
                        }
                    }
                }

                if (Globals.CheckDMScript("btnReduceSDEXP")) {
                    for (int i = 0; i < 5; i++) {
                        Emulator.WriteUShort("MONSTER_REWARDS", (ushort) Math.Ceiling((double) (Emulator.ReadShort(Constants.GetAddress("MONSTER_REWARDS") + (i * 0x1A8)) / 3)), (i * 0x1A8));
                    }
                }
                soloModeOnBattleEntry = true;
            } else {
                if (Emulator.Memory.GameState != GameState.Battle && soloModeOnBattleEntry) {
                    soloModeOnBattleEntry = false;
                    if (!alwaysAddSoloPartyMembers)
                        addSoloPartyMembers = false;
                }
            }
        }
        public void DuoModeBattle() {
            if (Emulator.Memory.GameState == GameState.Battle && Controller.Main.StatsChanged && !duoModeOnBattleEntry) {
                if (Globals.PARTY_SLOT[2] < 9) {
                    Emulator.Battle.CharacterTable[2].HP = 0;
                    Emulator.Battle.CharacterTable[2].Max_HP = 0;
                    Emulator.Battle.CharacterTable[2].HP_Regen = 200;
                    Emulator.Battle.CharacterTable[2].Turn = 10000;
                    //yeet
                    Emulator.Battle.CharacterTable[2].Pos_FB = 255;
                    Emulator.Battle.CharacterTable[2].Pos_UD = 255;
                    Emulator.Battle.CharacterTable[2].Pos_RL = 255;
                    Emulator.Battle.CharacterTable[0].Pos_FB = 10;
                    Emulator.Battle.CharacterTable[0].Pos_UD = 0;
                    Emulator.Battle.CharacterTable[0].Pos_RL = 251;
                    Emulator.Battle.CharacterTable[1].Pos_FB = 10;
                    Emulator.Battle.CharacterTable[1].Pos_UD = 0;
                    Emulator.Battle.CharacterTable[1].Pos_RL = 4;
                }

                if (Globals.CheckDMScript("btnReduceSDEXP")) {
                    for (int i = 0; i < 5; i++) {
                        Emulator.WriteUShort("MONSTER_REWARDS", (ushort) Math.Ceiling((double) (Emulator.ReadShort(Constants.GetAddress("MONSTER_REWARDS") + (i * 0x1A8)) * (2 / 3))), (i * 0x1A8));
                    }
                }
                duoModeOnBattleEntry = true;
            } else {
                if (Emulator.Memory.GameState != GameState.Battle && duoModeOnBattleEntry) {
                    duoModeOnBattleEntry = false;
                    if (!alwaysAddSoloPartyMembers)
                        addSoloPartyMembers = false;
                }
            }
        }

        public void AddSoloPartyMembers() {
            if (Globals.CheckDMScript("btnSoloMode")) {
                addSoloPartyMembers = true;
                Emulator.WriteByte("PARTY_SLOT", Globals.PARTY_SLOT[0], 0x4);
                Emulator.WriteByte("PARTY_SLOT", 0, 0x5);
                Emulator.WriteByte("PARTY_SLOT", 0, 0x6);
                Emulator.WriteByte("PARTY_SLOT", 0, 0x7);
                Emulator.WriteByte("PARTY_SLOT", Globals.PARTY_SLOT[0], 0x8);
                Emulator.WriteByte("PARTY_SLOT", 0, 0x9);
                Emulator.WriteByte("PARTY_SLOT", 0, 0xA);
                Emulator.WriteByte("PARTY_SLOT", 0, 0xB);
            } else if (Globals.CheckDMScript("btnDuoMode")) {
                addSoloPartyMembers = true;
                Emulator.WriteByte("PARTY_SLOT", Globals.PARTY_SLOT[0], 0x8);
                Emulator.WriteByte("PARTY_SLOT", 0, 0x9);
                Emulator.WriteByte("PARTY_SLOT", 0, 0xA);
                Emulator.WriteByte("PARTY_SLOT", 0, 0xB);
            } else {
                Constants.WritePLogOutput("Solo/Duo Mode must be turned on to add party members.");
            }
        }

        public void SwitchSoloCharacter() {
            if (Emulator.ReadByte("IN_PARTY", 0x2C * uiCombo["cboSwitchChar"]) != 0) {
                if (Globals.CheckDMScript("btnSoloMode") || Globals.CheckDMScript("btnDuoMode")) {
                    Emulator.WriteByte("PARTY_SLOT", (byte) uiCombo["cboSwitchChar"]);
                    Globals.NO_DART = null;
                } else {
                    if (uiCombo["cboSwitchChar"] == 0) {
                        Globals.NO_DART = null;
                        Emulator.WriteByte("PARTY_SLOT", (byte) uiCombo["cboSwitchChar"]);
                    } else {
                        Globals.DART_SWITCH = true;
                        Globals.NO_DART = uiCombo["cboSwitchChar"];
                        if (Emulator.Memory.GameState == 0) {
                            Emulator.WriteByte("PARTY_SLOT", (byte) uiCombo["cboSwitchChar"]);
                        }
                        Constants.WritePLogOutput("No Dart has been activated for Slot 1 and will swap to your select character in battle.");
                        
                    }
                }

            } else {
                Constants.WritePLogOutput("The selected character is not in the party.");
            }
        }
        #endregion

        #region Element Arrow
        public void ChangeElementArrow() {
            if (uiCombo["cboElement"] == 0) {
                elementArrowElement = 128;
                elementArrowItem = 0xC3;
                Constants.WriteGLogOutput("Element Arrow changed to Fire.");
            } else if (uiCombo["cboElement"] == 1) {
                elementArrowElement = 1;
                elementArrowItem = 0xC6;
                Constants.WriteGLogOutput("Element Arrow changed to Water.");
            } else if (uiCombo["cboElement"] == 2) {
                elementArrowElement = 64;
                elementArrowItem = 0xC7;
                Constants.WriteGLogOutput("Element Arrow changed to Wind.");
            } else if (uiCombo["cboElement"] == 3) {
                elementArrowElement = 2;
                elementArrowItem = 0xC5;
                Constants.WriteGLogOutput("Element Arrow changed to Earth.");
            } else if (uiCombo["cboElement"] == 4) {
                elementArrowElement = 4;
                elementArrowItem = 0xCA;
                Constants.WriteGLogOutput("Element Arrow changed to Dark.");
            } else if (uiCombo["cboElement"] == 5) {
                elementArrowElement = 32;
                elementArrowItem = 0xC9;
                Constants.WriteGLogOutput("Element Arrow changed to Light.");
            } else if (uiCombo["cboElement"] == 6) {
                elementArrowElement = 16;
                elementArrowItem = 0xC2;
                Constants.WriteGLogOutput("Element Arrow changed to Thunder.");
            }
        }
        #endregion

        #region Ultimate Boss

        #region Field
        public void UltimateBossField() {
            if ((Emulator.Memory.GameState == 0 && Globals.MAP >= 393 && Globals.MAP <= 405) && Globals.CHAPTER == 4 && !ultimateBossOnBattleEntry && Emulator.ReadByte("TRANSITION") == 12) {
                Thread.Sleep(200);
                if (Emulator.ReadByte("TRANSITION") != 12) {
                    return;
                }
                if (uiCombo["cboUltimateBoss"] == 0 && (Globals.MAP >= 393 && Globals.MAP <= 394)) {
                    Emulator.WriteShort("ENCOUNTER_ID", 487);
                    Emulator.WriteByte("BATTLE_FIELD", 10);
                    Emulator.WriteByte("TRANSITION", 19);
                } else if (uiCombo["cboUltimateBoss"] == 1 && (Globals.MAP >= 393 && Globals.MAP <= 394)) {
                    Emulator.WriteShort("ENCOUNTER_ID", 386);
                    Emulator.WriteByte("BATTLE_FIELD", 3);
                    Emulator.WriteByte("TRANSITION", 19);
                } else if (uiCombo["cboUltimateBoss"] == 2 && (Globals.MAP >= 393 && Globals.MAP <= 394)) {
                    Emulator.WriteShort("ENCOUNTER_ID", 414);
                    Emulator.WriteByte("BATTLE_FIELD", 8);
                    Emulator.WriteByte("TRANSITION", 19);
                } else if (uiCombo["cboUltimateBoss"] == 3 && (Globals.MAP >= 395 && Globals.MAP <= 397)) {
                    Emulator.WriteShort("ENCOUNTER_ID", 461);
                    Emulator.WriteByte("BATTLE_FIELD", 21);
                    Emulator.WriteByte("TRANSITION", 19);
                } else if (uiCombo["cboUltimateBoss"] == 4 && (Globals.MAP >= 395 && Globals.MAP <= 397)) {
                    Emulator.WriteShort("ENCOUNTER_ID", 412);
                    Emulator.WriteByte("BATTLE_FIELD", 16);
                    Emulator.WriteByte("TRANSITION", 19);
                } else if (uiCombo["cboUltimateBoss"] == 5 && (Globals.MAP >= 395 && Globals.MAP <= 397)) {
                    Emulator.WriteShort("ENCOUNTER_ID", 413);
                    Emulator.WriteByte("BATTLE_FIELD", 70);
                    Emulator.WriteByte("TRANSITION", 19);
                } else if (uiCombo["cboUltimateBoss"] == 6 && (Globals.MAP >= 395 && Globals.MAP <= 397)) {
                    Emulator.WriteShort("ENCOUNTER_ID", 387);
                    Emulator.WriteByte("BATTLE_FIELD", 5);
                    Emulator.WriteByte("TRANSITION", 19);
                } else if (uiCombo["cboUltimateBoss"] == 7 && (Globals.MAP >= 395 && Globals.MAP <= 397)) {
                    Emulator.WriteShort("ENCOUNTER_ID", 415);
                    Emulator.WriteByte("BATTLE_FIELD", 12);
                    Emulator.WriteByte("TRANSITION", 19);
                } else if (uiCombo["cboUltimateBoss"] == 8 && (Globals.MAP >= 398 && Globals.MAP <= 400)) {
                    Emulator.WriteShort("ENCOUNTER_ID", 449);
                    Emulator.WriteByte("BATTLE_FIELD", 68);
                    Emulator.WriteByte("TRANSITION", 19);
                } else if (uiCombo["cboUltimateBoss"] == 9 && (Globals.MAP >= 398 && Globals.MAP <= 400)) {
                    Emulator.WriteShort("ENCOUNTER_ID", 402);
                    Emulator.WriteByte("BATTLE_FIELD", 23);
                    Emulator.WriteByte("TRANSITION", 19);
                } else if (uiCombo["cboUltimateBoss"] == 10 && (Globals.MAP >= 398 && Globals.MAP <= 400)) {
                    Emulator.WriteShort("ENCOUNTER_ID", 403);
                    Emulator.WriteByte("BATTLE_FIELD", 29);
                    Emulator.WriteByte("TRANSITION", 19);
                } else if (uiCombo["cboUltimateBoss"] == 11 && (Globals.MAP >= 398 && Globals.MAP <= 400)) {
                    Emulator.WriteShort("ENCOUNTER_ID", 417);
                    Emulator.WriteByte("BATTLE_FIELD", 31);
                    Emulator.WriteByte("TRANSITION", 19);
                } else if (uiCombo["cboUltimateBoss"] == 12 && (Globals.MAP >= 398 && Globals.MAP <= 400)) {
                    Emulator.WriteShort("ENCOUNTER_ID", 418);
                    Emulator.WriteByte("BATTLE_FIELD", 41);
                    Emulator.WriteByte("TRANSITION", 19);
                } else if (uiCombo["cboUltimateBoss"] == 13 && (Globals.MAP >= 398 && Globals.MAP <= 400)) {
                    Emulator.WriteShort("ENCOUNTER_ID", 448);
                    Emulator.WriteByte("BATTLE_FIELD", 68);
                    Emulator.WriteByte("TRANSITION", 19);
                } else if (uiCombo["cboUltimateBoss"] == 14 && (Globals.MAP >= 398 && Globals.MAP <= 400)) {
                    Emulator.WriteShort("ENCOUNTER_ID", 416);
                    Emulator.WriteByte("BATTLE_FIELD", 38);
                    Emulator.WriteByte("TRANSITION", 19);
                } else if (uiCombo["cboUltimateBoss"] == 15 && (Globals.MAP >= 398 && Globals.MAP <= 400)) {
                    Emulator.WriteShort("ENCOUNTER_ID", 422);
                    Emulator.WriteByte("BATTLE_FIELD", 42);
                    Emulator.WriteByte("TRANSITION", 19);
                } else if (uiCombo["cboUltimateBoss"] == 16 && (Globals.MAP >= 398 && Globals.MAP <= 400)) {
                    Emulator.WriteShort("ENCOUNTER_ID", 423);
                    Emulator.WriteByte("BATTLE_FIELD", 47);
                    Emulator.WriteByte("TRANSITION", 19);
                } else if (uiCombo["cboUltimateBoss"] == 17 && (Globals.MAP >= 398 && Globals.MAP <= 400)) {
                    Emulator.WriteShort("ENCOUNTER_ID", 432);
                    Emulator.WriteByte("BATTLE_FIELD", 69);
                    Emulator.WriteByte("TRANSITION", 19);
                } else if (uiCombo["cboUltimateBoss"] == 18 && (Globals.MAP >= 398 && Globals.MAP <= 400)) {
                    Emulator.WriteShort("ENCOUNTER_ID", 430);
                    Emulator.WriteByte("BATTLE_FIELD", 67);
                    Emulator.WriteByte("TRANSITION", 19);
                } else if (uiCombo["cboUltimateBoss"] == 19 && (Globals.MAP >= 398 && Globals.MAP <= 400)) {
                    Emulator.WriteShort("ENCOUNTER_ID", 433);
                    Emulator.WriteByte("BATTLE_FIELD", 56);
                    Emulator.WriteByte("TRANSITION", 19);
                } else if (uiCombo["cboUltimateBoss"] == 20 && (Globals.MAP >= 398 && Globals.MAP <= 400)) {
                    Emulator.WriteShort("ENCOUNTER_ID", 431);
                    Emulator.WriteByte("BATTLE_FIELD", 54);
                    Emulator.WriteByte("TRANSITION", 19);
                } else if (uiCombo["cboUltimateBoss"] == 21 && (Globals.MAP >= 398 && Globals.MAP <= 400)) {
                    Emulator.WriteShort("ENCOUNTER_ID", 447);
                    Emulator.WriteByte("BATTLE_FIELD", 68);
                    Emulator.WriteByte("TRANSITION", 19);
                } else if (uiCombo["cboUltimateBoss"] == 22 && (Globals.MAP >= 401 && Globals.MAP <= 405)) {
                    Emulator.WriteShort("ENCOUNTER_ID", 408);
                    Emulator.WriteByte("BATTLE_FIELD", 12);
                    Emulator.WriteByte("TRANSITION", 19);
                } else if (uiCombo["cboUltimateBoss"] == 23 && (Globals.MAP >= 401 && Globals.MAP <= 405)) {
                    Emulator.WriteShort("ENCOUNTER_ID", 389);
                    Emulator.WriteByte("BATTLE_FIELD", 21);
                    Emulator.WriteByte("TRANSITION", 19);
                } else if (uiCombo["cboUltimateBoss"] == 24 && (Globals.MAP >= 401 && Globals.MAP <= 405)) {
                    Emulator.WriteShort("ENCOUNTER_ID", 396);
                    Emulator.WriteByte("BATTLE_FIELD", 30);
                    Emulator.WriteByte("TRANSITION", 19);
                } else if (uiCombo["cboUltimateBoss"] == 25 && (Globals.MAP >= 401 && Globals.MAP <= 405)) {
                    Emulator.WriteShort("ENCOUNTER_ID", 399);
                    Emulator.WriteByte("BATTLE_FIELD", 72);
                    Emulator.WriteByte("TRANSITION", 19);
                } else if (uiCombo["cboUltimateBoss"] == 26 && (Globals.MAP >= 401 && Globals.MAP <= 405)) {
                    Emulator.WriteShort("ENCOUNTER_ID", 409);
                    Emulator.WriteByte("BATTLE_FIELD", 27);
                    Emulator.WriteByte("TRANSITION", 19);
                } else if (uiCombo["cboUltimateBoss"] == 27 && (Globals.MAP >= 401 && Globals.MAP <= 405)) {
                    Emulator.WriteShort("ENCOUNTER_ID", 393);
                    Emulator.WriteByte("BATTLE_FIELD", 14);
                    Emulator.WriteByte("TRANSITION", 19);
                } else if (uiCombo["cboUltimateBoss"] == 28 && (Globals.MAP >= 401 && Globals.MAP <= 405)) {
                    Emulator.WriteShort("ENCOUNTER_ID", 398);
                    Emulator.WriteByte("BATTLE_FIELD", 73);
                    Emulator.WriteByte("TRANSITION", 19);
                } else if (uiCombo["cboUltimateBoss"] == 29 && (Globals.MAP >= 401 && Globals.MAP <= 405)) {
                    Emulator.WriteShort("ENCOUNTER_ID", 397);
                    Emulator.WriteByte("BATTLE_FIELD", 35);
                    Emulator.WriteByte("TRANSITION", 19);
                } else if (uiCombo["cboUltimateBoss"] == 30 && (Globals.MAP >= 401 && Globals.MAP <= 405)) {
                    Emulator.WriteShort("ENCOUNTER_ID", 400);
                    Emulator.WriteByte("BATTLE_FIELD", 76);
                    Emulator.WriteByte("TRANSITION", 19);
                } else if (uiCombo["cboUltimateBoss"] == 31 && (Globals.MAP >= 401 && Globals.MAP <= 405)) {
                    Emulator.WriteShort("ENCOUNTER_ID", 410);
                    Emulator.WriteByte("TRANSITION", 19);
                } else if (uiCombo["cboUltimateBoss"] == 32 && (Globals.MAP >= 401 && Globals.MAP <= 405)) {
                    Emulator.WriteShort("ENCOUNTER_ID", 401);
                    Emulator.WriteByte("BATTLE_FIELD", 77);
                    Emulator.WriteByte("TRANSITION", 19);
                } else if (uiCombo["cboUltimateBoss"] == 33 && (Globals.MAP >= 401 && Globals.MAP <= 405)) {
                    Emulator.WriteShort("ENCOUNTER_ID", 390);
                    Emulator.WriteByte("BATTLE_FIELD", 22);
                    Emulator.WriteByte("TRANSITION", 19);
                } else if (uiCombo["cboUltimateBoss"] == 34 && (Globals.MAP >= 401 && Globals.MAP <= 405)) {
                    Emulator.WriteShort("ENCOUNTER_ID", 411);
                    Emulator.WriteByte("BATTLE_FIELD", 24);
                    Emulator.WriteByte("TRANSITION", 19);
                } else if (uiCombo["cboUltimateBoss"] == 35 && (Globals.MAP >= 401 && Globals.MAP <= 405)) {
                    Emulator.WriteShort("ENCOUNTER_ID", 394);
                    Emulator.WriteByte("BATTLE_FIELD", 40);
                    Emulator.WriteByte("TRANSITION", 19);
                } else if (uiCombo["cboUltimateBoss"] == 36 && (Globals.MAP >= 401 && Globals.MAP <= 405)) {
                    Emulator.WriteShort("ENCOUNTER_ID", 392);
                    Emulator.WriteByte("BATTLE_FIELD", 45);
                    Emulator.WriteByte("TRANSITION", 19);
                } else if (uiCombo["cboUltimateBoss"] == 37 && (Globals.MAP >= 401 && Globals.MAP <= 405)) {
                    Emulator.WriteShort("ENCOUNTER_ID", 420);
                    Emulator.WriteByte("BATTLE_FIELD", 44);
                    Emulator.WriteByte("TRANSITION", 19);
                } else if (uiCombo["cboUltimateBoss"] == 38 && (Globals.MAP >= 401 && Globals.MAP <= 405)) {
                    Emulator.WriteShort("ENCOUNTER_ID", 442);
                    Emulator.WriteByte("BATTLE_FIELD", 71);
                    Emulator.WriteByte("TRANSITION", 19);
                } else if (uiCombo["cboUltimateBoss"] == 39 && (Globals.MAP >= 401 && Globals.MAP <= 405)) {
                    Emulator.WriteShort("ENCOUNTER_ID", 443);
                    Emulator.WriteByte("BATTLE_FIELD", 65);
                    Emulator.WriteByte("TRANSITION", 19);
                } else {
                    this.Dispatcher.BeginInvoke(new Action(() => {
                        Globals.dmScripts["btnUltimateBoss"] = false;
                        TurnOnOffButton(ref btnUltimateBoss);
                    }), DispatcherPriority.ContextIdle);
                }
            }
        }

        public void UltimateBossFieldSet() {
            if (Globals.CheckDMScript("btnUltimateBoss")) {
                if (Globals.MAP >= 393 && Globals.MAP <= 405) {
                    if (ultimateBossCompleted >= uiCombo["cboUltimateBoss"]) {
                        if ((uiCombo["cboUltimateBoss"] >= 0 && uiCombo["cboUltimateBoss"] <= 2) && Globals.MAP > 394) {
                            Globals.dmScripts["btnUltimateBoss"] = false;
                            Constants.WriteGLogOutput("You must be in Zone 1 - Maps 393-394.");
                        }

                        if ((uiCombo["cboUltimateBoss"] >= 3 && uiCombo["cboUltimateBoss"] <= 7) && (Globals.MAP < 395 || Globals.MAP > 397)) {
                            Globals.dmScripts["btnUltimateBoss"] = false;
                            Constants.WriteGLogOutput("You must be in Zone 2 - Maps 395-397.");
                        }

                        if ((uiCombo["cboUltimateBoss"] >= 8 && uiCombo["cboUltimateBoss"] <= 21) && (Globals.MAP < 398 || Globals.MAP > 400)) {
                            Globals.dmScripts["btnUltimateBoss"] = false;
                            Constants.WriteGLogOutput("You must be in Zone 3 - Maps 398-400.");
                        }

                        if (uiCombo["cboUltimateBoss"] >= 22 && Globals.MAP < 401) {
                            Globals.dmScripts["btnUltimateBoss"] = false;
                            Constants.WriteGLogOutput("You must be in Zone 4 - Maps 401-405.");
                        }
                    } else {
                        Globals.dmScripts["btnUltimateBoss"] = false;
                        Constants.WriteGLogOutput("You must defeat a previous Ultimate Boss.");
                    }
                } else {
                    Globals.dmScripts["btnUltimateBoss"] = false;
                    Constants.WriteGLogOutput("You are not in the Forbidden Land.");
                }

                this.Dispatcher.BeginInvoke(new Action(() => {
                    TurnOnOffButton(ref btnUltimateBoss);
                }), DispatcherPriority.ContextIdle);
            }
        }
        #endregion

        #region Battle
        public void UltimateBossBattle() {
            if (Emulator.Memory.GameState == GameState.Battle && Controller.Main.StatsChanged && !ultimateBossOnBattleEntry) {
                ubHPChanged = ubCheckedDamage = ubUltimateHPSet = false;
                ubCheckDamageCycle = 0;
                ubDragoonBondMode = -1;
                ubGuardBreak =
                    ubHealingPotion =
                    ubZeroSP =
                    ubMPAttack =
                    ubWoundDamage =
                    ubHealthSteal =
                    ubSPAttack =
                    ubMoveChange =
                    ubMagicChange =
                    ubElementalShift =
                    ubArmorShell =
                    ubSharedHP =
                    ubRemoveResistances =
                    ubTPDamage =
                    ubTrackHPChange =
                    ubBodyDamage =
                    ubVirageKilledPart =
                    ubDragoonBond =
                    ubDragoonExtras =
                    ubTrackDragoon =
                    ubCountdown =
                    ubUltimateEnrage =
                    ubInventoryRefresh =
                    ubEnhancedShield =
                    ubBodyProtect =
                    ubFinalAttack =
                    ubReverseDBS =
                    ubArmorGuard =
                    ubDragoonGuard =
                    ubGrantMaxHP = false;

                ultimateBossKeepMap = true;
                ultimateBossMap = Globals.MAP;
                SetUltimateStats();

                if (Globals.CheckDMScript("btnRemoveCaps")) {
                    this.Dispatcher.BeginInvoke(new Action(() => {
                        Globals.dmScripts["btnRemoveCaps"] = true;
                        TurnOnOffButton(ref btnRemoveCaps);
                    }), DispatcherPriority.ContextIdle);
                } else {
                    if (!Globals.CheckDMScript("btnRemoveCaps")) {
                        this.Dispatcher.BeginInvoke(new Action(() => {
                            Globals.dmScripts.Add("btnRemoveCaps", true);
                            TurnOnOffButton(ref btnRemoveCaps);
                        }), DispatcherPriority.ContextIdle);
                    }
                }

                if (ubZeroSP) {
                    ZeroSPStart();
                }

                if (ubMPAttack) {
                    for (int i = 0; i < 3; i++) {
                        if (Globals.PARTY_SLOT[i] < 9) {
                            ubMPAttackTrk[i] = Emulator.Battle.CharacterTable[i].HP;
                        }
                    }
                }

                if (ubWoundDamage) {
                    for (int i = 0; i < 3; i++) {
                        if (Globals.PARTY_SLOT[i] < 9) {
                            ubWHP[i] = Emulator.Battle.CharacterTable[i].HP;
                            ubWMHP[i] = Emulator.Battle.CharacterTable[i].Max_HP;
                        }
                    }
                }

                if (ubMoveChange) {
                    for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                        ubMoveChgTrn[i] = Emulator.Battle.MonsterTable[i].Turn;
                    }
                    ubMoveChgSet = false;
                }

                if (ubArmorShell) {
                    if (Globals.ENCOUNTER_ID == 422) {
                        ubHeartHPSave = Emulator.Battle.MonsterTable[3].HP;
                        ubArmorShellTurns = 0;
                    }
                }

                if (ubRemoveResistances) {
                    for (int i = 0; i < 3; i++) {
                        if (Globals.PARTY_SLOT[i] < 9) {
                            Emulator.Battle.CharacterTable[i].StatusResist = 0;
                            Emulator.Battle.CharacterTable[i].SpecialEffect = 0;
                        }
                    }
                }

                if (ubTrackHPChange) {
                    for (int i = 0; i < 3; i++) {
                        if (Globals.PARTY_SLOT[i] < 9) {
                            ubTrackHP[i] = Emulator.Battle.CharacterTable[i].HP;
                        }
                    }

                    for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                        ubTrackEHP[i] = Emulator.Battle.MonsterTable[i].HP;
                        if (ubBodyProtect)
                            ubTrackEHP[i] = ultimateHP[i];
                    }


                    for (int i = 0; i < 3; i++) {
                        if (Globals.PARTY_SLOT[i] < 9) {
                            ubTrackTP[i] = Emulator.Battle.CharacterTable[i].Turn;
                        }
                    }

                    for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                        ubTrackMTP[i] = Emulator.Battle.MonsterTable[i].Turn;
                    }

                    ubBlockMenuHPTrack = false;
                    ubBlockMenuTPTrack = false;
                    ubTrackDMove = 255;
                }

                if (ubTPDamage) {
                    tpDamage = 0;
                }

                if (ubDragoonExtras) {
                    ubCustomStatusTurns[0] = 0;
                    ubCustomStatusTurns[1] = 0;
                    ubCustomStatusTurns[2] = 0;
                }

                if (ubCountdown) {
                    ubLivesIncreased = 0;
                    ubTotalGold = 0;
                }

                if (ubInventoryRefresh) {
                    if (Globals.ENCOUNTER_ID == 390) {
                        for (int i = 0; i < inventorySize + 1; i++) {
                            if (i == inventorySize) {
                                inventoryRefreshSize = Emulator.ReadByte("INVENTORY_SIZE");
                            } else {
                                inventoryRefresh[i] = Emulator.ReadByte("INVENTORY", i);
                            }
                        }
                        ubInventoryRefresh = false;
                    }
                }

                if (ubUltimateEnrage) {
                    enrageChangeIndex = 0;
                    enrageChangeTurns = 0;
                    ubEnrageTurns = 0;
                    ubEnrageTurnsPlus = 0;
                }

                if (ubEnhancedShield) {
                    shieldTurnsTaken = 0;
                    enhancedShieldTurns = 0;
                }

                if (ubDragoonGuard) {
                    /*for (int i = 0; i < 3; i++) {
                        if (Globals.PARTY_SLOT[i] < 9) {
                            deathRes[i] = Emulator.Battle.CharacterTable[i].Read("Death_Res");
                        }
                    }*/
                }

                if (ubGrantMaxHP) {
                    double multiMax = 1.00;

                    if (Globals.ENCOUNTER_ID == 420) {
                        multiMax = 1.75;
                    }

                    for (int i = 0; i < 3; i++) {
                        if (Globals.PARTY_SLOT[i] < 9) {
                            double maxHP = Emulator.Battle.CharacterTable[i].Max_HP * multiMax;
                            if (maxHP > 32767) {
                                int character = Globals.PARTY_SLOT[i];
                                int lv = Emulator.ReadByte("CHAR_TABLE", (character * 0x2C) + 0x12);
                                ushort df = Globals.CURRENT_STATS[i].DF * 2;
                                ushort mdf = Globals.CURRENT_STATS[i].MDF * 2;
                                Emulator.Battle.CharacterTable[i].Max_HP = (ushort) (maxHP / 2);
                                Emulator.Battle.CharacterTable[i].DF = df;
                                Emulator.Battle.CharacterTable[i].OG_DF = df;
                                Emulator.Battle.CharacterTable[i].MDF = mdf;
                                Emulator.Battle.CharacterTable[i].OG_MDF = mdf;
                                Emulator.WriteUShort("SECONDARY_CHARACTER_TABLE", df, character * 0xA0 + 0x6C);
                                Emulator.WriteUShort("SECONDARY_CHARACTER_TABLE", mdf, character * 0xA0 + 0x6D);
                            } else {
                                Emulator.Battle.CharacterTable[i].Max_HP = (ushort) maxHP;
                            }
                        }
                    }
                }

                ultimateBossOnBattleEntry = true;
            } else {
                if (Emulator.Memory.GameState != GameState.Battle && ultimateBossOnBattleEntry) {
                    ultimateBossOnBattleEntry = false;
                    this.Dispatcher.BeginInvoke(new Action(() => {
                        Globals.dmScripts["btnUltimateBoss"] = false;
                        TurnOnOffButton(ref btnUltimateBoss);
                    }), DispatcherPriority.ContextIdle);
                } else {
                    if (Emulator.ReadUShort("BATTLE_VALUE") == 41215 && Controller.Main.StatsChanged) {
                        UltimateBossHP();

                        if (ubGuardBreak) {
                            if (Globals.ENCOUNTER_ID == 414) { //Urobolus
                                GuardBreak(0, 2); //Poison Spit 
                            }
                        }

                        if (ubHealingPotion) {
                            if (Globals.ENCOUNTER_ID == 412) { //Drake the Bandit
                                UltimateHealthPotion(0, 3, 3.33333); //Healing Potion
                            } else if (Globals.ENCOUNTER_ID == 416) { //Grand Jewel
                                //UltimateHealthPotion(0, 1, 3.33333); //Heal Fountain
                            }
                        }

                        if (ubMPAttack) {
                            if (Globals.ENCOUNTER_ID == 415) { //Fire Bird
                                MPAttack(0, 3, 0, 231, 2); //Summon Volcano Balls
                            }
                        }

                        if (ubWoundDamage) {
                            if (Globals.ENCOUNTER_ID == 417) { //Ghost Commander
                                WoundDamage(0, 0); //Slash
                            }
                        }

                        if (ubHealthSteal) {
                            if (Globals.ENCOUNTER_ID == 417) { //Ghost Commander
                                HealthSteal(0, 2); //Life Drain
                            }
                        }

                        if (ubSPAttack) {
                            if (Globals.ENCOUNTER_ID == 418) { //Kamuy
                                if (ultimateHP[0] > 150000) {
                                    SPAttack(0, 1, 25, 231, 2); //Wind Blade
                                } else if (ultimateHP[0] > 75000) {
                                    SPAttack(0, 1, 50, 206, 2); //Wind Blade
                                } else {
                                    SPAttack(0, 1, 125, 156, 2); //Wind Blade
                                }
                            }
                        }

                        if (ubMoveChange) {
                            if (Globals.ENCOUNTER_ID == 418) { //Kamuy
                                MoveChange(0, 2, 25); //Purple Spear
                            }
                        }

                        if (ubMagicChange) {
                            if (Globals.ENCOUNTER_ID == 416) { //Grand Jewel
                                double hpDamage = ultimateMaxHP[0] - ultimateHP[0];
                                if ((hpDamage / ultimateMaxHP[0]) * 10 >= magicChangeTurns) {
                                    MagicChange();
                                }
                            } else if (Globals.ENCOUNTER_ID == 396) { //Lenus
                                double hpDamage = ultimateMaxHP[0] - ultimateHP[0];
                                if ((hpDamage / ultimateMaxHP[0]) * 20 >= magicChangeTurns) {
                                    MagicChange();
                                }
                            } else if (Globals.ENCOUNTER_ID == 390) { //Doel
                                if (ultimateHP[1] <= 150000) {
                                    Emulator.WriteAoB(Globals.MONS_ADDRESS[1] - 0x6F8C, "C2");
                                    Emulator.WriteAoB(Globals.MONS_ADDRESS[1] - 0x6F4C, "F8");
                                } else {
                                    if (magicChangeTurns > Emulator.ReadShort(Globals.MONS_ADDRESS[1] + 0x44)) {
                                        if (Emulator.ReadByte("DRAGOON_SPECIAL_ATTACK") >= 0 && Emulator.ReadByte("DRAGOON_SPECIAL_ATTACK", 0x1) == 0) {
                                            if (Emulator.ReadByte("DRAGOON_SPECIAL_ATTACK") == 0) {
                                                Emulator.WriteAoB(Globals.MONS_ADDRESS[1] - 0x6F8C, "C3");
                                                Emulator.WriteAoB(Globals.MONS_ADDRESS[1] - 0x6F4C, "D1");
                                            } else if (Emulator.ReadByte("DRAGOON_SPECIAL_ATTACK") == 1 || Emulator.ReadByte("DRAGOON_SPECIAL_ATTACK") == 5) {
                                                Emulator.WriteAoB(Globals.MONS_ADDRESS[1] - 0x6F8C, "C7");
                                                Emulator.WriteAoB(Globals.MONS_ADDRESS[1] - 0x6F4C, "DC");
                                            } else if (Emulator.ReadByte("DRAGOON_SPECIAL_ATTACK") == 2 || Emulator.ReadByte("DRAGOON_SPECIAL_ATTACK") == 8) {
                                                Emulator.WriteAoB(Globals.MONS_ADDRESS[1] - 0x6F8C, "C9");
                                                Emulator.WriteAoB(Globals.MONS_ADDRESS[1] - 0x6F4C, "D2");
                                            } else if (Emulator.ReadByte("DRAGOON_SPECIAL_ATTACK") == 3) {
                                                Emulator.WriteAoB(Globals.MONS_ADDRESS[1] - 0x6F8C, "CA");
                                                Emulator.WriteAoB(Globals.MONS_ADDRESS[1] - 0x6F4C, "D8");
                                            } else if (Emulator.ReadByte("DRAGOON_SPECIAL_ATTACK") == 4) {
                                                Emulator.WriteAoB(Globals.MONS_ADDRESS[1] - 0x6F8C, "C2");
                                                Emulator.WriteAoB(Globals.MONS_ADDRESS[1] - 0x6F4C, "CF");
                                            } else if (Emulator.ReadByte("DRAGOON_SPECIAL_ATTACK") == 6) {
                                                Emulator.WriteAoB(Globals.MONS_ADDRESS[1] - 0x6F8C, "C6");
                                                Emulator.WriteAoB(Globals.MONS_ADDRESS[1] - 0x6F4C, "D6");
                                            } else if (Emulator.ReadByte("DRAGOON_SPECIAL_ATTACK") == 7) {
                                                Emulator.WriteAoB(Globals.MONS_ADDRESS[1] - 0x6F8C, "C5");
                                                Emulator.WriteAoB(Globals.MONS_ADDRESS[1] - 0x6F4C, "D0");
                                            }
                                        } else {
                                            Emulator.WriteAoB(Globals.MONS_ADDRESS[1] - 0x6F8C, "C2");
                                            Emulator.WriteAoB(Globals.MONS_ADDRESS[1] - 0x6F4C, "CF");
                                        }
                                    }

                                    magicChangeTurns = Emulator.ReadShort(Globals.MONS_ADDRESS[1] + 0x44);
                                }
                            } else if (Globals.ENCOUNTER_ID == 394) { //Divine Dargon
                                double hpDamage = ultimateMaxHP[0] - ultimateHP[0];
                                if ((hpDamage / ultimateMaxHP[0]) * 5 >= magicChangeTurns) {
                                    MagicChange();
                                }
                            } else if (Globals.ENCOUNTER_ID == 392) { //Lloyd
                                double hpDamage = ultimateMaxHP[0] - ultimateHP[0];
                                if ((hpDamage / ultimateMaxHP[0]) * 7 >= magicChangeTurns) {
                                    MagicChange();
                                }
                            } else if (Globals.ENCOUNTER_ID == 420) { //Magician Faust
                                double hpDamage = ultimateMaxHP[0] - ultimateHP[0];
                                if ((hpDamage / ultimateMaxHP[0]) * 100 >= magicChangeTurns) {
                                    MagicChange();
                                }
                            }
                        }

                        if (ubElementalShift)
                            ElementalShift();

                        if (ubArmorShell)
                            BreakArmor();

                        if (ubTPDamage)
                            TurnPointDamage();

                        if (ubBodyDamage)
                            BodyDamage();

                        if (ubDragoonBond && ubDragoonBondMode != 999)
                            DragoonBond();

                        if (ubDragoonExtras)
                            DragoonExtras();

                        if (ubCountdown)
                            Countdown();

                        if (ubInventoryRefresh) {
                            if (Globals.ENCOUNTER_ID == 390) {
                                if (Emulator.Battle.MonsterTable[0].HP == 0) {
                                    for (int i = 0; i < inventorySize + 1; i++) {
                                        if (i == inventorySize) {
                                            Emulator.WriteByte("INVENTORY_SIZE", inventoryRefreshSize);
                                        } else {
                                            Emulator.WriteByte("INVENTORY", inventoryRefresh[i], i);
                                        }
                                    }
                                }
                            }
                        }

                        if (ubUltimateEnrage)
                            UltimateEnrage();

                        if (ubEnhancedShield)
                            EnhancedShield();

                        if (ubBodyProtect)
                            BodyProtect();

                        if (ubReverseDBS)
                            ReverseDragonBlockStaff();

                        if (ubArmorGuard)
                            ArmorGuard();

                        if (ubDragoonGuard)
                            DragoonGuard();

                        UltimateBossDefeatCheck();
                    }
                }
            }
        }

        public void SetUltimateStats() {
            for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                ultimateHP[i] = 0;
                ultimateMaxHP[i] = 0;

                if (Globals.MONSTER_IDS[i] == 332) { //Urobolus
                    ubGuardBreak = true;
                } else if (Globals.MONSTER_IDS[i] == 325) { //Drake the Bandit
                    ubHealingPotion = true;
                } else if (Globals.MONSTER_IDS[i] == 329) { //Jiango
                    ubZeroSP = true;
                } else if (Globals.MONSTER_IDS[i] == 333) { //Fire Bird
                    ubGuardBreak = true;
                    ubZeroSP = true;
                    ubMPAttack = true;
                } else if (Globals.MONSTER_IDS[i] == 301) { //Gehrich
                    ubZeroSP = true;
                } else if (Globals.MONSTER_IDS[i] == 340) { //Ghost Commander
                    ubWoundDamage = true;
                    ubHealthSteal = true;
                } else if (Globals.MONSTER_IDS[i] == 343) { //Kamuy
                    ubSPAttack = true;
                    ubMoveChange = true;
                } else if (Globals.MONSTER_IDS[i] == 335) { //Grand Jewel
                    ubMagicChange = true;
                    ubElementalShift = true;
                    ubHealingPotion = true;
                } else if (Globals.MONSTER_IDS[i] == 346) { //Windigo
                    ubArmorShell = true;
                    ubZeroSP = true;
                } else if (Globals.MONSTER_IDS[i] == 349) { //Polter Helm
                    ubSharedHP = true;
                } else if (Globals.MONSTER_IDS[i] == 365) { //The Last Kraken
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] + 0x175, 0);
                } else if (Globals.MONSTER_IDS[i] == 308) { //Virage (head)
                    ubRemoveResistances = true;
                } else if (Globals.MONSTER_IDS[i] == 293) { //Lenus
                    ubMagicChange = true;
                } else if (Globals.MONSTER_IDS[i] == 296) { //Syuveil
                    ubTPDamage = true;
                    ubTrackHPChange = true;
                } else if (Globals.MONSTER_IDS[i] == 311) { //Virage(head)
                    ubBodyDamage = true;
                    ubTrackHPChange = true;
                    ubVirageKilledPart = false;
                } else if (Globals.MONSTER_IDS[i] == 287) { //Greham
                    ubDragoonBond = true;
                    ubRemoveResistances = true;
                    ubDragoonBondMode = -1;
                } else if (Globals.MONSTER_IDS[i] == 295) { //Damia
                    ubDragoonExtras = true;
                    ubTrackHPChange = true;
                } else if (Globals.MONSTER_IDS[i] == 294) { //Dragoon Lenus
                    ubDragoonBond = true;
                    ubDragoonBondMode = -1;
                } else if (Globals.MONSTER_IDS[i] == 297) { //Belzac
                    ubDragoonExtras = true;
                    ubTrackDragoon = true;
                    ubTrackHPChange = true;
                } else if (Globals.MONSTER_IDS[i] == 316) { //S Virage(head)
                    ubCountdown = true;
                } else if (Globals.MONSTER_IDS[i] == 298) { //Kanzas
                    ubDragoonExtras = true;
                    ubTrackDragoon = true;
                    ubTrackHPChange = true;
                } else if (Globals.MONSTER_IDS[i] == 267) { //Emperor Doel
                    ubZeroSP = true;
                    ubUltimateEnrage = true;
                } else if (Globals.MONSTER_IDS[i] == 268) { //Dragoon Doel
                    ubInventoryRefresh = true;
                    ubMagicChange = true;
                    ubEnhancedShield = true;
                    ubTrackHPChange = true;
                } else if (Globals.MONSTER_IDS[i] == 320) { //S Virage II (head)
                    ubBodyProtect = true;
                    ubFinalAttack = true;
                    ubTrackHPChange = true;
                } else if (Globals.MONSTER_IDS[i] == 283) { //Divine Dragon
                    ubReverseDBS = true;
                    ubUltimateEnrage = true;
                    ubArmorGuard = true;
                    ubTrackHPChange = true;
                    ubMagicChange = true;
                } else if (Globals.MONSTER_IDS[i] == 270) { //Lloyd
                    ubUltimateEnrage = true;
                    ubTrackHPChange = true;
                    ubRemoveResistances = true;
                    ubMagicChange = true;
                } else if (Globals.MONSTER_IDS[i] == 344) { //Magician Faust
                    ubMagicChange = true;
                    ubDragoonGuard = true;
                    ubGrantMaxHP = true;
                    ubUltimateEnrage = true;
                } else if (Globals.MONSTER_IDS[i] == 387) { //Zieg
                    ubDragoonExtras = true;
                    ubTrackDragoon = true;
                    ubTrackHPChange = true;
                    ubUltimateEnrage = true;
                    ultimateThread = new Thread(UltimateController);
                    ultimateThread.Start();
                }

                if (Globals.DICTIONARY.UltimateStatList[Globals.MONSTER_IDS[i]].HP > 65535) {
                    Emulator.Battle.MonsterTable[i].HP = 65535;
                    Emulator.Battle.MonsterTable[i].Max_HP = 65535;
                    ultimateHP[i] = ultimateMaxHP[i] = Globals.DICTIONARY.UltimateStatList[Globals.MONSTER_IDS[i]].HP;
                } else {
                    Emulator.Battle.MonsterTable[i].HP = (ushort) Globals.DICTIONARY.UltimateStatList[Globals.MONSTER_IDS[i]].HP;
                    Emulator.Battle.MonsterTable[i].Max_HP = (ushort) Globals.DICTIONARY.UltimateStatList[Globals.MONSTER_IDS[i]].HP;
                }
                Emulator.Battle.MonsterTable[i].AT = (ushort) Globals.DICTIONARY.UltimateStatList[Globals.MONSTER_IDS[i]].AT;
                Emulator.Battle.MonsterTable[i].OG_AT = (ushort) Globals.DICTIONARY.UltimateStatList[Globals.MONSTER_IDS[i]].AT;
                Emulator.Battle.MonsterTable[i].MAT = (ushort) Globals.DICTIONARY.UltimateStatList[Globals.MONSTER_IDS[i]].MAT;
                Emulator.Battle.MonsterTable[i].OG_MAT = (ushort) Globals.DICTIONARY.UltimateStatList[Globals.MONSTER_IDS[i]].MAT;
                Emulator.Battle.MonsterTable[i].DF = (ushort) Globals.DICTIONARY.UltimateStatList[Globals.MONSTER_IDS[i]].DF;
                Emulator.Battle.MonsterTable[i].OG_DF = (ushort) Globals.DICTIONARY.UltimateStatList[Globals.MONSTER_IDS[i]].DF;
                Emulator.Battle.MonsterTable[i].MDF = (ushort) Globals.DICTIONARY.UltimateStatList[Globals.MONSTER_IDS[i]].MDF;
                Emulator.Battle.MonsterTable[i].OG_MDF = (ushort) Globals.DICTIONARY.UltimateStatList[Globals.MONSTER_IDS[i]].MDF;
                Emulator.Battle.MonsterTable[i].SPD = (ushort) Globals.DICTIONARY.UltimateStatList[Globals.MONSTER_IDS[i]].SPD;
                Emulator.Battle.MonsterTable[i].OG_SPD = (ushort) Globals.DICTIONARY.UltimateStatList[Globals.MONSTER_IDS[i]].SPD;

                WipeRewards();

                if (Globals.ENCOUNTER_ID == 487) { //Commander II
                    Emulator.WriteByte("MONSTER_REWARDS_CHANCE", 100, +0x1A8);
                    Emulator.WriteByte("MONSTER_REWARDS_DROP", 158, 0x1A8); //Sabre
                }

                if (Globals.ENCOUNTER_ID == 449 || Globals.ENCOUNTER_ID == 402 || Globals.ENCOUNTER_ID == 403) {
                    if (Globals.ENCOUNTER_ID == 402 || Globals.ENCOUNTER_ID == 403) {
                        Emulator.WriteUShort("MONSTER_REWARDS", 3000, 0x1A8 + 0x2);
                    } else {
                        Emulator.WriteUShort("MONSTER_REWARDS", 3000, 0x2);
                    }
                } else if (Globals.ENCOUNTER_ID == 417 || Globals.ENCOUNTER_ID == 418 || Globals.ENCOUNTER_ID == 448) {
                    if (Globals.ENCOUNTER_ID == 418) {
                        Emulator.WriteUShort("MONSTER_REWARDS", 3000, 0x1A8 + 0x2);
                    } else {
                        Emulator.WriteUShort("MONSTER_REWARDS", 3000, 0x2);
                    }
                } else if (Globals.ENCOUNTER_ID == 416 || Globals.ENCOUNTER_ID == 422 || Globals.ENCOUNTER_ID == 423) {
                    Emulator.WriteUShort("MONSTER_REWARDS", 9000, 0x2);
                } else if (Globals.ENCOUNTER_ID == 432 || Globals.ENCOUNTER_ID == 430 || Globals.ENCOUNTER_ID == 433) {
                    Emulator.WriteUShort("MONSTER_REWARDS", 12000, 0x2);
                } else if (Globals.ENCOUNTER_ID == 431 || Globals.ENCOUNTER_ID == 408) {
                    Emulator.WriteUShort("MONSTER_REWARDS", 15000, 0x1A8 + 0x2);
                } else if (Globals.ENCOUNTER_ID == 447) {
                    Emulator.WriteUShort("MONSTER_REWARDS", 18000, 0x2);
                } else if (Globals.ENCOUNTER_ID == 389 || Globals.ENCOUNTER_ID == 396) {
                    Emulator.WriteUShort("MONSTER_REWARDS", 20000, 0x2);
                } else if (Globals.ENCOUNTER_ID == 399) {
                    Emulator.WriteUShort("MONSTER_REWARDS", 25000, 0x2);
                } else if (Globals.ENCOUNTER_ID == 409) {
                    Emulator.WriteUShort("MONSTER_REWARDS", 30000, 0x2);
                } else if (Globals.ENCOUNTER_ID == 393 || Globals.ENCOUNTER_ID == 398) {
                    Emulator.WriteUShort("MONSTER_REWARDS", 35000, 0x2);
                } else if (Globals.ENCOUNTER_ID == 397 || Globals.ENCOUNTER_ID == 400) {
                    Emulator.WriteUShort("MONSTER_REWARDS", 40000, 0x2);
                } else if (Globals.ENCOUNTER_ID == 410) {
                    Emulator.WriteUShort("MONSTER_REWARDS", 1000, 0x2);
                } else if (Globals.ENCOUNTER_ID == 401) {
                    Emulator.WriteUShort("MONSTER_REWARDS", 45000, 0x2);
                } else if (Globals.ENCOUNTER_ID == 390) {
                    Emulator.WriteInt("TOTAL_GOLD", 100000);
                } else if (Globals.ENCOUNTER_ID == 411) {
                    Emulator.WriteInt("TOTAL_GOLD", 60000);
                } else if (Globals.ENCOUNTER_ID == 394) {
                    Emulator.WriteInt("TOTAL_GOLD", 70000);
                } else if (Globals.ENCOUNTER_ID == 392) {
                    Emulator.WriteInt("TOTAL_GOLD", 80000);
                } else if (Globals.ENCOUNTER_ID == 420) {
                    Emulator.WriteInt("TOTAL_GOLD", 120000);
                } else if (Globals.ENCOUNTER_ID == 442) {
                    Emulator.WriteInt("TOTAL_GOLD", 100000);
                }
            }
        }

        public void UltimateBossDefeatCheck() {
            if (Emulator.Memory.GameState == GameState.Battle && Controller.Main.StatsChanged) {
                switch (Globals.ENCOUNTER_ID) {
                    case 386: //Fruegel I
                    case 487: //Commander II
                    case 414: //Urobolus
                        if (Emulator.Battle.MonsterTable[0].HP == 0) {
                            UltimateBossDefeated();
                        }
                        break;
                    case 461: //Sandora Elite
                    case 415: //Fire Bird
                    case 412: //Drake the Bandit
                    case 387: //Fruegel II
                    case 402: //Mappi
                    case 418: //Kamuy
                    case 422: //Windigo
                    case 432: //Last Kraken
                    case 431: //Zackwell
                    case 408: //Virage I
                    case 409: //Virage II
                    case 420: //Magician Faust
                        if (Emulator.Battle.MonsterTable[0].HP == 0 || ultimateHP[0] == 0) {
                            UltimateBossDefeated();
                        }
                        break;
                    case 449: //Feyrbrand (Spirit)
                    case 448: //Regole (Spirit)
                    case 447: //Divine Dragon (Spirit)
                        if (Emulator.Battle.MonsterTable[1].HP == 0 || ultimateHP[1] == 0) {
                            UltimateBossDefeated();
                        }
                        break;
                    case 411: //S Virage II
                        if (ultimateHP[0] == 0 || ultimateHP[1] == 0) {
                            UltimateBossDefeated();
                        }
                        break;
                    case 394: //Divine Dragon
                        if (Emulator.Battle.MonsterTable[0].HP == 0) {
                            UltimateBossDefeated();
                        }
                        break;
                    default:
                        byte defeatedEnemies = 0;
                        for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                            if (Emulator.Battle.MonsterTable[i].HP == 0 || ultimateHP[i] == 0) {
                                defeatedEnemies += 1;
                            }
                        }

                        if (defeatedEnemies == Globals.MONSTER_SIZE) {
                            UltimateBossDefeated();
                        }
                        break;
                }
            }
        }

        public void UltimateBossDefeated() {
            this.Dispatcher.BeginInvoke(new Action(() => {
                Globals.dmScripts["btnUltimateBoss"] = false;
                TurnOnOffButton(ref btnUltimateBoss);
                ultimateBossOnBattleEntry = false;
                ubHPChanged = ubCheckedDamage = ubUltimateHPSet = false;
                ubCheckDamageCycle = 0;
                ubDragoonBondMode = -1;
                ubGuardBreak =
                    ubHealingPotion =
                    ubZeroSP =
                    ubMPAttack =
                    ubWoundDamage =
                    ubHealthSteal =
                    ubSPAttack =
                    ubMoveChange =
                    ubMagicChange =
                    ubElementalShift =
                    ubArmorShell =
                    ubSharedHP =
                    ubRemoveResistances =
                    ubTPDamage =
                    ubTrackHPChange =
                    ubBodyDamage =
                    ubVirageKilledPart =
                    ubDragoonBond =
                    ubDragoonExtras =
                    ubTrackDragoon =
                    ubCountdown =
                    ubUltimateEnrage =
                    ubInventoryRefresh =
                    ubEnhancedShield =
                    ubBodyProtect =
                    ubFinalAttack =
                    ubReverseDBS =
                    ubArmorGuard =
                    ubDragoonGuard =
                    ubGrantMaxHP
                    = false;

                if (ultimateBossCompleted == uiCombo["cboUltimateBoss"]) {
                    ultimateBossCompleted += 1;
                }

                if (ultimateBossCompleted == 3) {
                    inventorySize = 36;
                    ExtendInventory();
                    Constants.WritePLog("On how to extend inventory please see the output log in the settings tab.");
                    Constants.WriteGLogOutput("Ultimate Boss Zone 1 completed! Inventory expanded to 36 slots.");
                    Constants.WriteOutput("Please note Extended Inventory must be applied at the Save Slot screen once per emulator session to avoid loss of items. To do this simply open up Dragoon Modifier right before you load your save.");
                } else if (ultimateBossCompleted == 8) {
                    inventorySize = 40;
                    ExtendInventory();
                    Constants.WritePLog("On how to extend inventory please see the output log in the settings tab.");
                    Constants.WriteGLogOutput("Ultimate Boss Zone 2 completed! Inventory expanded to 40 slots.");
                    Constants.WriteOutput("Please note Extended Inventory must be applied at the Save Slot screen once per emulator session to avoid loss of items. To do this simply open up Dragoon Modifier right before you load your save.");
                } else if (ultimateBossCompleted == 22) {
                    inventorySize = 48;
                    ExtendInventory();
                    Constants.WritePLog("On how to extend inventory please see the output log in the settings tab.");
                    Constants.WriteGLogOutput("Ultimate Boss Zone 3 completed! Inventory expanded to 48 slots.");
                    Constants.WriteOutput("Please note Extended Inventory must be applied at the Save Slot screen once per emulator session to avoid loss of items. To do this simply open up Dragoon Modifier right before you load your save.");
                } else if (ultimateBossCompleted == 34) {
                    inventorySize = 64;
                    ExtendInventory();
                    Constants.WritePLog("On how to extend inventory please see the output log in the settings tab.");
                    Constants.WriteGLogOutput("Ultimate Boss Zone 4 midboss completed! Inventory expanded to 64 slots.");
                    Constants.WriteOutput("Please note Extended Inventory must be applied at the Save Slot screen once per emulator session to avoid loss of items. To do this simply open up Dragoon Modifier right before you load your save.");
                } else {
                    Constants.WriteGLogOutput("Ultimate Boss defeated.");
                }

                SaveSubKey();
            }), DispatcherPriority.ContextIdle);
        }

        public void DoubleRepeat() {
            if (Emulator.Memory.GameState == GameState.Battle && Controller.Main.StatsChanged) {
                if ((ultimateShopLimited & 131072) == 131072) { //Power Up
                    if ((doubleRepeatUsed & 131072) != 131072) {
                        for (int i = 0; i < 3; i++) {
                            if (Globals.PARTY_SLOT[i] < 9 && Emulator.Battle.CharacterTable[i].PWR_AT == 50 && Emulator.Battle.CharacterTable[i].PWR_AT_Turn > 0) {
                                doubleRepeatUsed += 131072;
                                Emulator.Battle.CharacterTable[i].PWR_AT_Turn = (byte)(Emulator.Battle.CharacterTable[i].PWR_AT_Turn + 3);
                                Emulator.Battle.CharacterTable[i].PWR_MAT_Turn = (byte) (Emulator.Battle.CharacterTable[i].PWR_MAT_Turn + 3);
                                Emulator.Battle.CharacterTable[i].PWR_DF_Turn = (byte) (Emulator.Battle.CharacterTable[i].PWR_DF_Turn + 3);
                                Emulator.Battle.CharacterTable[i].PWR_MDF_Turn = (byte) (Emulator.Battle.CharacterTable[i].PWR_MDF_Turn + 3);
                            }
                        }
                    }
                }

                if ((ultimateShopLimited & 262144) == 262144) { //Power Down
                    if ((doubleRepeatUsed & 262144) != 262144) {
                        for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                            if (Emulator.Battle.MonsterTable[i].PWR_AT == -50 && Emulator.Battle.MonsterTable[i].PWR_AT_Turn > 0) {
                                doubleRepeatUsed += 262144;
                                Emulator.Battle.MonsterTable[i].PWR_AT_Turn += 3;
                                Emulator.Battle.MonsterTable[i].PWR_MAT_Turn += 3;
                                Emulator.Battle.MonsterTable[i].PWR_DF_Turn += 3;
                                Emulator.Battle.MonsterTable[i].PWR_MDF_Turn += 3;
                            }
                        }
                    }
                }

                if ((ultimateShopLimited & 524288) == 524288) { //Speed Up
                    if ((doubleRepeatUsed & 524288) != 524288) {
                        for (int i = 0; i < 3; i++) {
                            if (Globals.PARTY_SLOT[i] < 9 && Emulator.Battle.CharacterTable[i].Speed_Up_Turn > 0) {
                                doubleRepeatUsed += 524288;
                                if (Emulator.Battle.CharacterTable[i].Speed_Up_Turn < 6) {
                                    Emulator.Battle.CharacterTable[i].Speed_Up_Turn = (byte)(Emulator.Battle.CharacterTable[i].Speed_Up_Turn + 3);
                                } else {
                                    Emulator.Battle.CharacterTable[i].Speed_Up_Turn = 6;
                                }
                            }
                        }
                    }
                }

                if ((ultimateShopLimited & 1048576) == 1048576) { //Speed Down
                    if ((doubleRepeatUsed & 1048576) != 1048576) {
                        for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                            if (Emulator.Battle.MonsterTable[i].Speed_Down_Turn > 0) {
                                doubleRepeatUsed += 1048576;

                                if (Emulator.Battle.MonsterTable[i].Speed_Down_Turn < 6) {
                                    Emulator.Battle.MonsterTable[i].Speed_Down_Turn += 3;
                                } else {
                                    Emulator.Battle.MonsterTable[i].Speed_Down_Turn = 6;
                                }
                            }
                        }
                    }
                }

                if ((ultimateShopLimited & 2097152) == 2097152) { //Magic Shield
                    if ((doubleRepeatUsed & 2097152) != 2097152) {
                        for (int i = 0; i < 3; i++) {
                            if (Globals.PARTY_SLOT[i] < 9) {
                                if (Emulator.ReadByte("SPECIAL_EFFECT", (Globals.MONSTER_SIZE + i) * 0x20) >= 12) {
                                    doubleRepeatUsed += 2097152;
                                    magicShieldSlot = i;
                                    magicShieldTurns = 12;
                                }
                            }
                        }
                    } else {
                        DoubleShield();
                    }
                }

                if ((ultimateShopLimited & 4194304) == 4194304) { //Material Shield
                    if ((doubleRepeatUsed & 4194304) != 4194304) {
                        for (int i = 0; i < 3; i++) {
                            if (Globals.PARTY_SLOT[i] < 9) {
                                int shieldValue = Emulator.ReadByte("SPECIAL_EFFECT", (Globals.MONSTER_SIZE + i) * 0x20);
                                if (shieldValue == 3 || shieldValue == 7 || shieldValue == 11 || shieldValue == 15) {
                                    doubleRepeatUsed += 4194304;
                                    materialShieldSlot = i;
                                    materialShieldTurns = 3;
                                }
                            }
                        }
                    } else {
                        DoubleShield();
                    }
                }

                if ((ultimateShopLimited & 8388608) == 8388608) { //Magic Sig Stone
                    if ((doubleRepeatUsed & 8388608) != 8388608) {
                        for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                            if (Emulator.ReadByte("SPECIAL_EFFECT", i * 0x20) == 48) {
                                doubleRepeatUsed += 8388608;
                                sigStoneSlot = i;
                                sigStoneTurns = 48;
                            }
                        }
                    } else {
                        if (sigStoneTurns > 0) {
                            if (Emulator.ReadByte("SPECIAL_EFFECT", sigStoneSlot * 0x20) == 16) {
                                Emulator.WriteByte("SPECIAL_EFFECT", (byte) (Emulator.ReadByte("SPECIAL_EFFECT", sigStoneSlot * 0x20) + 16), sigStoneSlot * 0x20);
                                sigStoneTurns -= 16;
                            }

                            if (Emulator.Battle.MonsterTable[sigStoneSlot].HP == 0)
                                sigStoneTurns = 0;
                        }
                    }
                }

                if ((ultimateShopLimited & 16777216) == 16777216) { //Pandemonium
                    if ((doubleRepeatUsed & 16777216) != 16777216) {
                        for (int i = 0; i < 3; i++) {
                            if (Globals.PARTY_SLOT[i] < 9) {
                                if (Emulator.ReadByte("SPECIAL_EFFECT", 1 + ((Globals.MONSTER_SIZE + i) * 0x20)) == 3) {
                                    doubleRepeatUsed += 16777216;
                                    pandemoniumSlot = i;
                                    pandemoniumTurns = 3;
                                }
                            }
                        }
                    } else {
                        if (pandemoniumTurns > 0) {
                            if (pandemoniumTurns > 0) {
                                if (Emulator.ReadByte("SPECIAL_EFFECT", 1 + ((Globals.MONSTER_SIZE + pandemoniumSlot) * 0x20)) == 1) {
                                    Emulator.WriteByte(Constants.GetAddress("SPECIAL_EFFECT") + 1 + ((Globals.MONSTER_SIZE + pandemoniumSlot) * 0x20), (byte) (Emulator.ReadByte("SPECIAL_EFFECT", 1 + ((Globals.MONSTER_SIZE + pandemoniumSlot) * 0x20)) + 1));
                                    pandemoniumTurns -= 1;
                                }

                                if (Emulator.Battle.CharacterTable[pandemoniumSlot].HP == 0)
                                    pandemoniumTurns = 0;
                            }
                        }
                    }
                }

                doubleRepeatOnBattleEntry = true;
            } else {
                if (Emulator.Memory.GameState != GameState.Battle && doubleRepeatOnBattleEntry) {
                    doubleRepeatOnBattleEntry = false;
                    doubleRepeatUsed = 0;
                    magicShieldTurns = 0;
                    magicShieldSlot = 0;
                    materialShieldTurns = 0;
                    materialShieldSlot = 0;
                    sigStoneTurns = 0;
                    sigStoneSlot = 0;
                    pandemoniumTurns = 0;
                    pandemoniumSlot = 0;
                }
            }
        }

        public void DoubleShield() {
            if (materialShieldTurns == 0 && magicShieldTurns == 0)
                return;

            for (int i = 0; i < 3; i++) {
                long address = Constants.GetAddress("SPECIAL_EFFECT") + ((Globals.MONSTER_SIZE + i) * 0x20);
                if (materialShieldTurns > 0 && magicShieldTurns > 0 && materialShieldSlot == i && magicShieldSlot == i && (Emulator.ReadByte(address) == 5 || Emulator.ReadByte(address) == 8 || Emulator.ReadByte(address) == 9)) {
                    Emulator.WriteByte(address, (byte) (Emulator.ReadByte(address) + 5));
                    materialShieldTurns -= 1;
                    magicShieldTurns -= 5;
                } else {
                    if (materialShieldTurns > 0 && materialShieldSlot == i && Emulator.ReadByte(address) == 1) {
                        Emulator.WriteByte(address, (byte) (Emulator.ReadByte(address) + 1));
                        materialShieldTurns -= 1;
                    }
                    if (magicShieldTurns > 0 && magicShieldSlot == i && Emulator.ReadByte(address) == 4) {
                        Emulator.WriteByte(address, (byte) (Emulator.ReadByte(address) + 4));
                        magicShieldTurns -= 4;
                    }
                }
            }

        }
        #endregion

        #region Extras
        public void UltimateBossHP() {
            bool partyAttacking = false;
            bool guardCheck = false;
            int guardSlot = 0;
            bool dragoonSpecialCheck = false;
            int totalDamage = 0;
            for (int i = 0; i < 3; i++) {
                if (Globals.PARTY_SLOT[i] < 9) {
                    byte action = Emulator.Battle.CharacterTable[i].Action;
                    if (action == 24 || action == 26 || action == 136 || action == 138) {
                        partyAttacking = true;
                    }
                    if (action == 136) {
                        guardCheck = true;
                        guardSlot = i;
                    }
                    if (action == 24) {
                        dragoonSpecialCheck = true;
                    }
                }
            }

            if (guardCheck) {
                if (Globals.PARTY_SLOT[guardSlot] != 2 && Globals.PARTY_SLOT[guardSlot] != 8) {
                    for (int i = 0; i < 3; i++) {
                        if (Globals.PARTY_SLOT[i] < 9) {
                            byte action = Emulator.Battle.CharacterTable[i].Action;
                            if (action == 0 || action == 2) {
                                partyAttacking = false;
                            }
                        }
                    }
                }
            }

            if (dragoonSpecialCheck) {
                for (int i = 0; i < 3; i++) {
                    if (Globals.PARTY_SLOT[i] < 9) {
                        byte action = Emulator.Battle.CharacterTable[i].Action;
                        if (action == 18 || action == 19) {
                            partyAttacking = false;
                        }
                    }
                }
            }

            if (partyAttacking && !ubUltimateHPSet) {
                for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                    if (ultimateHP[i] > 0) {
                        Emulator.Battle.MonsterTable[i].HP = 65535;
                    }
                }
                ubUltimateHPSet = true;
                ubCheckedDamage = false;
            }

            for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                if (ultimateHP[i] > 0) {
                    if (partyAttacking) {
                        if (!ubCheckedDamage) {
                            ushort currentHP = Emulator.Battle.MonsterTable[i].HP;
                            if (ubSharedHP) {
                                if (currentHP < 65535) {
                                    totalDamage += 65535 - currentHP;
                                    ubHPChanged = true;
                                }
                                if ((i + 1) == Globals.MONSTER_SIZE) {
                                    for (int x = 0; x < Globals.MONSTER_SIZE; x++) {
                                        ultimateHP[x] -= totalDamage;
                                        if (ultimateHP[x] < 0) {
                                            ultimateHP[x] = 0;
                                            Emulator.Battle.MonsterTable[x].HP = 0;
                                        }
                                    }
                                }
                            } else {
                                if (currentHP < 65535) {
                                    ultimateHP[i] -= 65535 - currentHP;
                                    ubHPChanged = true;
                                }
                                if (ultimateHP[i] < 0) {
                                    ultimateHP[i] = 0;
                                    Emulator.Battle.MonsterTable[i].HP = 0;
                                }
                            }
                        }
                        ubCheckDamageCycle = ubCheckedDamage ? 0 : 2;
                    } else {
                        if (ubCheckDamageCycle > 0) {
                            ushort currentHP = Emulator.Battle.MonsterTable[i].HP;
                            if (ubSharedHP) {
                                if (currentHP < 65535) {
                                    totalDamage += 65535 - currentHP;
                                    ubHPChanged = true;
                                    ubCheckDamageCycle = 0;
                                }
                                if ((i + 1) == Globals.MONSTER_SIZE) {
                                    for (int x = 0; x < Globals.MONSTER_SIZE; x++) {
                                        ultimateHP[x] -= totalDamage;
                                        Emulator.Battle.MonsterTable[i].HP = (ushort) Math.Round(((double) ultimateHP[i] / ultimateMaxHP[i]) * 65535);
                                        if (ultimateHP[x] < 0) {
                                            ultimateHP[x] = 0;
                                            Emulator.Battle.MonsterTable[x].HP = 0;
                                        }
                                    }
                                }
                            } else {
                                if (currentHP < 65535) {
                                    ultimateHP[i] -= 65535 - currentHP;
                                    Emulator.Battle.MonsterTable[i].HP = (ushort) Math.Round(((double) ultimateHP[i] / ultimateMaxHP[i]) * 65535);
                                    ubHPChanged = true;
                                    ubCheckDamageCycle = 0;
                                }
                                if (ultimateHP[i] < 0) {
                                    ultimateHP[i] = 0;
                                    Emulator.Battle.MonsterTable[i].HP = 0;
                                }
                            }

                            if ((i + 1) == Globals.MONSTER_SIZE)
                                ubCheckDamageCycle--;
                        } else {
                            ubCheckedDamage = false;
                            ubUltimateHPSet = false;
                            ubHPChanged = false;
                            ushort hpAmt = (ushort) Math.Round(((double) ultimateHP[i] / ultimateMaxHP[i]) * 65535);
                            ushort currentHP = Emulator.Battle.MonsterTable[i].HP;
                            //Constants.WriteDebug("HP%: " + hpAmt + " / " + currentHP);
                            if (hpAmt != currentHP) {
                                Emulator.Battle.MonsterTable[i].HP = hpAmt;
                            }
                        }
                    }
                }
            }

            if (ubHPChanged) {
                ubCheckedDamage = true;
            }

            /*Constants.WriteDebug("HP[0]: " + ultimateHP[0] + "/" + Globals.MONSTER_TABLE[0].Read("HP") + " | P ATK: " + partyAttacking + "/" + ubCheckDamageCycle + " | CHK DMG: " + ubCheckedDamage + " | HP CHG: " + ubHPChanged + " | SET: " + ubUltimateHPSet + " | ACT: " + Emulator.Battle.CharacterTable[0].Read("Action") + "/" + Emulator.Battle.CharacterTable[1].Read("Action") + "/" + Emulator.Battle.CharacterTable[2].Read("Action"));

            if (!partyAttacking && ubCheckDamageCycle > 0 && ubHPChanged) {
                if (Globals.CheckDMScript("btnDamageTracker") && Globals.dmScripts["btnDamageTracker"]) {
                    DamageTracker();
                }
                ubCheckedDamage = false;
                ubHPChanged = false;
                ubCheckDamageCycle = 0;
                //Constants.WriteDebug("Failed party attack check.");
            }*/

            //Constants.WritePLog("Attack Move: " + Globals.MONSTER_TABLE[0].Read("AttackMove"));
        }

        public void GuardBreak(int monsterSlot, byte attack) {
            //Constants.WritePLog("Attack Move: " + Globals.MONSTER_TABLE[monsterSlot].Read("AttackMove") + "/" + attack);
            if (Emulator.Battle.MonsterTable[monsterSlot].AttackMove == attack) {
                //Constants.WriteDebug("Attack Move: " + Globals.MONSTER_TABLE[monsterSlot].Read("AttackMove") + "/" + attack);
                for (int i = 0; i < 3; i++) {
                    if (Globals.PARTY_SLOT[i] < 9) {
                        Emulator.Battle.CharacterTable[i].Guard = 0;
                    }
                }
                Thread.Sleep(1500); //it'll soft lock the game otherwise, game needs time to load the move
                Emulator.Battle.MonsterTable[monsterSlot].AttackMove = 255;
            }
        }

        public void UltimateHealthPotion(int monsterSlot, byte attack, double heal) {
            if (Emulator.Battle.MonsterTable[monsterSlot].AttackMove == attack) {
                ultimateHP[monsterSlot] = (int) Math.Min(ultimateMaxHP[monsterSlot], ultimateHP[monsterSlot] + Math.Round(ultimateMaxHP[monsterSlot] / heal));
                Thread.Sleep(1500);
                Emulator.Battle.MonsterTable[monsterSlot].AttackMove = 255;
            }
        }

        public void ZeroSPStart() {
            for (int i = 0; i < 3; i++) {
                if (Globals.PARTY_SLOT[i] < 9) {
                    Emulator.Battle.CharacterTable[i].SP = 0;
                }
            }
        }

        public void MPAttack(int monsterSlot, byte attack, byte mpAmount, byte mpOnHit, byte turns) {
            //Constants.WritePLog("Attack Move: " + Globals.MONSTER_TABLE[monsterSlot].Read("AttackMove") + "/" + attack);
            if (Emulator.Battle.MonsterTable[monsterSlot].AttackMove == attack) {
                for (int i = 0; i < 3; i++) {
                    if (Globals.PARTY_SLOT[i] < 9) {
                        if (mpOnHit > 0) {
                            Emulator.Battle.CharacterTable[i].MP_P_Hit_Increase = (sbyte) mpOnHit;
                            Emulator.Battle.CharacterTable[i].MP_M_Hit_Increase = (sbyte) mpOnHit;
                            Emulator.Battle.CharacterTable[i].MP_P_Hit_Increase_Turn = turns;
                            Emulator.Battle.CharacterTable[i].MP_M_Hit_Increase_Turn = turns;
                        }
                        int mp = Emulator.Battle.CharacterTable[i].MP - mpAmount;
                        Emulator.Battle.CharacterTable[i].MP = (ushort) (mpAmount > 0 ? mpAmount : 0);
                    }
                }
                Thread.Sleep(1900);
                Emulator.Battle.MonsterTable[monsterSlot].AttackMove = 255;
            }
        }

        public void WoundDamage(int monsterSlot, byte attack) {
            for (int i = 0; i < 3; i++) {
                if (Globals.PARTY_SLOT[i] < 9) {
                    ushort hp = Emulator.Battle.CharacterTable[i].HP;
                    if (hp < ubWHP[i] && Emulator.Battle.MonsterTable[i].AttackMove == attack) {
                        ushort woundDamage = (ushort) (ubWHP[i] - hp);
                        Emulator.Battle.CharacterTable[i].Max_HP = (ushort) Math.Max(0, Emulator.Battle.CharacterTable[i].Max_HP - woundDamage);
                    }
                    ubWHP[i] = hp;
                    if (Emulator.Battle.CharacterTable[i].Action == 192) {
                        Emulator.Battle.CharacterTable[i].Max_HP = ubWMHP[i];
                    }
                }
            }
        }

        public void HealthSteal(int monsterSlot, byte attack) {
            if (Emulator.Battle.MonsterTable[monsterSlot].AttackMove == attack) {
                if (ubHealthStealSave) {
                    ushort dmg = Emulator.ReadUShort("DAMAGE_SLOT1");
                    if (dmg <= 25000 && dmg != ubHealthStealDamage) {
                        ubHealthStealDamage = dmg;
                        ultimateHP[monsterSlot] += (int) Math.Round((double) dmg / 2);
                        ubHealthStealSave = false;
                    }
                } else {
                    ubHealthStealDamage = Emulator.ReadUShort("DAMAGE_SLOT1");
                    ubHealthStealSave = true;
                }
            } else {
                ubHealthStealSave = false;
            }
        }

        public void SPAttack(int monsterSlot, byte attack, byte spAmount, byte spOnHit, byte turns) {
            try {
                if (Emulator.Battle.MonsterTable[monsterSlot].AttackMove == attack) {
                    for (int i = 0; i < 3; i++) {
                        if (Globals.PARTY_SLOT[i] < 9 && Emulator.ReadByte("DRAGOON_TURNS", (i * 0x4)) == 0) {
                            if (spOnHit > 0) {
                                Emulator.Battle.CharacterTable[i].SP_P_Hit_Increase = (sbyte)spOnHit;
                                Emulator.Battle.CharacterTable[i].SP_M_Hit_Increase = (sbyte)spOnHit;
                                Emulator.Battle.CharacterTable[i].SP_P_Hit_Increase_Turn = turns;
                                Emulator.Battle.CharacterTable[i].SP_M_Hit_Increase_Turn = turns;
                            }
                            int sp = Emulator.Battle.CharacterTable[i].SP - spAmount;
                            Emulator.Battle.CharacterTable[i].SP = (ushort) (sp > 0 ? sp : 0);
                        }
                    }
                    Thread.Sleep(3500);
                    Emulator.Battle.MonsterTable[monsterSlot].AttackMove = 255;
                }
            } catch (Exception ex) {
                Constants.WriteGLogOutput("NON FATAL ERROR OCCURED.");
                Constants.WritePLogOutput("NON FATAL ERROR OCCURED.");
                Constants.WriteError(ex.ToString());
                EnableUI();
            }
        }

        public void MoveChange(int monsterSlot, byte attack, int chance) { //Some moves it won't overwrite but that's fine.
            ushort trn = Emulator.Battle.MonsterTable[monsterSlot].Turn;
            if (ubMoveChgSet) {
                bool partyAttacking = false;
                for (int i = 0; i < 3; i++) {
                    if (Globals.PARTY_SLOT[i] < 9) {
                        byte action = Emulator.Battle.CharacterTable[i].Action;
                        if (action == 8 || action == 10 || action == 24 || action == 26 || action == 136 || action == 138) {
                            partyAttacking = true;
                        }
                    }
                }

                if (!partyAttacking) {
                    Emulator.Battle.MonsterTable[monsterSlot].AttackMove = attack;
                    //Constants.WriteDebug("[MOVE] " + Globals.MONSTER_TABLE[monsterSlot].Read("AttackMove"));
                } else {
                    ubMoveChgSet = false;
                    if (Globals.ENCOUNTER_ID == 418) {
                        Emulator.Battle.MonsterTable[monsterSlot].AT = (ushort) (originalMonsterStats[monsterSlot, 1] * 4.5);
                    }
                }
            } else {
                if (trn < ubMoveChgTrn[monsterSlot]) {
                    if (new Random().Next(0, 100) < chance) {
                        ubMoveChgSet = true;
                        if (Globals.ENCOUNTER_ID == 418) {
                            Emulator.Battle.MonsterTable[monsterSlot].AT = (ushort) Math.Round(originalMonsterStats[monsterSlot, 1] * 4.5 * 1.75);
                        }
                        //Constants.WriteDebug("[MOVE CHANGE] Roll success.");
                    } else {
                        //Constants.WriteDebug("Roll failed.");
                    }
                }
            }
            ubMoveChgTrn[monsterSlot] = trn;
        }

        public void MagicChange() {
            ArrayList singleMagic = new ArrayList();
            ArrayList wideMagic = new ArrayList();
            ArrayList powerMagic = new ArrayList();
            int index = 0;

            singleMagic.Add(0xC3);
            singleMagic.Add(0xC6);
            singleMagic.Add(0xC7);
            singleMagic.Add(0xC5);
            singleMagic.Add(0xCA);
            singleMagic.Add(0xC9);
            singleMagic.Add(0xC2);
            wideMagic.Add(0xD1);
            wideMagic.Add(0xD6);
            wideMagic.Add(0xDC);
            wideMagic.Add(0xD0);
            wideMagic.Add(0xD8);
            wideMagic.Add(0xD2);
            wideMagic.Add(0xCF);
            powerMagic.Add(0xF2);
            powerMagic.Add(0xF3);
            powerMagic.Add(0xF4);
            powerMagic.Add(0xF5);
            powerMagic.Add(0xF7);
            powerMagic.Add(0xF6);
            powerMagic.Add(0xF8);

            if (Globals.ENCOUNTER_ID == 416) {
                index = new Random().Next(0, singleMagic.Count);
                Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x102C, Convert.ToByte(singleMagic[index]));
                singleMagic.RemoveAt(index);
                index = new Random().Next(0, singleMagic.Count);
                Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0xFEC, Convert.ToByte(singleMagic[index]));
                singleMagic.RemoveAt(index);
                index = new Random().Next(0, singleMagic.Count);
                Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0xFAC, Convert.ToByte(singleMagic[index]));
                singleMagic.RemoveAt(index);


                index = new Random().Next(0, wideMagic.Count);
                Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0xF6C, Convert.ToByte(wideMagic[index]));
                wideMagic.RemoveAt(index);
                index = new Random().Next(0, wideMagic.Count);
                Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0xF2C, Convert.ToByte(wideMagic[index]));
                wideMagic.RemoveAt(index);


                index = new Random().Next(0, powerMagic.Count);
                Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0xEEC, Convert.ToByte(powerMagic[index]));
                powerMagic.RemoveAt(index);
            } else if (Globals.ENCOUNTER_ID == 396) {
                index = new Random().Next(0, singleMagic.Count);
                Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0xC1C, Convert.ToByte(singleMagic[index]));
                singleMagic.RemoveAt(index);
                index = new Random().Next(0, singleMagic.Count);
                Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0xAF0, Convert.ToByte(singleMagic[index]));
                singleMagic.RemoveAt(index);

                index = new Random().Next(0, wideMagic.Count);
                Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0xBB8, Convert.ToByte(wideMagic[index]));
                wideMagic.RemoveAt(index);
                index = new Random().Next(0, wideMagic.Count);
                Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0xB54, Convert.ToByte(wideMagic[index]));
                wideMagic.RemoveAt(index);
            } else if (Globals.ENCOUNTER_ID == 394) {
                index = new Random().Next(0, singleMagic.Count);
                Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x1ABC, Convert.ToByte(singleMagic[index]));
                singleMagic.RemoveAt(index);

                index = new Random().Next(0, powerMagic.Count);
                Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x1A7C, Convert.ToByte(powerMagic[index]));
                powerMagic.RemoveAt(index);
                index = new Random().Next(0, powerMagic.Count);
                Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x1A3C, Convert.ToByte(powerMagic[index]));
                powerMagic.RemoveAt(index);
            } else if (Globals.ENCOUNTER_ID == 392) {
                index = new Random().Next(0, singleMagic.Count);
                Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0xDAC, Convert.ToByte(singleMagic[index]));
                singleMagic.RemoveAt(index);
                index = new Random().Next(0, singleMagic.Count);
                Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0xD6C, Convert.ToByte(singleMagic[index]));
                singleMagic.RemoveAt(index);

                index = new Random().Next(0, wideMagic.Count);
                Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0xD2C, Convert.ToByte(wideMagic[index]));
                wideMagic.RemoveAt(index);
                index = new Random().Next(0, wideMagic.Count);
                Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0xCEC, Convert.ToByte(wideMagic[index]));
                wideMagic.RemoveAt(index);
                index = new Random().Next(0, wideMagic.Count);
                Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0xCAC, Convert.ToByte(wideMagic[index]));
                wideMagic.RemoveAt(index);
            } else if (Globals.ENCOUNTER_ID == 420) {
                if (ultimateHP[0] >= 850000) {
                    byte[] element = { 16, 16, 16 };
                    for (int i = 0; i < 3; i++) {
                        if (Globals.PARTY_SLOT[i] < 9) {
                            element[i] = CharacterToElement(Globals.PARTY_SLOT[i]);
                        }
                    }
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x5BBC, ElementToItem(0, element[0]));
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x5B84, ElementToItem(0, element[0]));
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x5B4C, ElementToItem(0, element[1]));
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x5B14, ElementToItem(0, element[1]));
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x5ADC, ElementToItem(0, element[2]));
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x5AA4, ElementToItem(0, element[2]));
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x5A6C, ElementToItem(0, element[new Random().Next(0, 2)]));
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x5A2C, ElementToItem(1, element[0]));
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x59EC, ElementToItem(1, element[1]));
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x59AC, ElementToItem(1, element[2]));
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x596C, ElementToItem(2, element[0]));
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x592C, ElementToItem(2, element[0]));
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x58EC, ElementToItem(2, element[1]));
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x58AC, ElementToItem(2, element[1]));
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x586C, ElementToItem(2, element[2]));
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x582C, ElementToItem(2, element[2]));
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x57EC, ElementToItem(2, element[new Random().Next(0, 2)]));
                } else if (ultimateHP[0] >= 700000) {
                    index = new Random().Next(0, singleMagic.Count);
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x5BBC, Convert.ToByte(singleMagic[index]));
                    singleMagic.RemoveAt(index);
                    index = new Random().Next(0, singleMagic.Count);
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x5B84, Convert.ToByte(singleMagic[index]));
                    singleMagic.RemoveAt(index);
                    index = new Random().Next(0, singleMagic.Count);
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x5B4C, Convert.ToByte(singleMagic[index]));
                    singleMagic.RemoveAt(index);
                    index = new Random().Next(0, singleMagic.Count);
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x5B14, Convert.ToByte(singleMagic[index]));
                    singleMagic.RemoveAt(index);
                    index = new Random().Next(0, singleMagic.Count);
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x5ADC, Convert.ToByte(singleMagic[index]));
                    singleMagic.RemoveAt(index);
                    index = new Random().Next(0, singleMagic.Count);
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x5AA4, Convert.ToByte(singleMagic[index]));
                    singleMagic.RemoveAt(index);
                    index = new Random().Next(0, singleMagic.Count);
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x5A6C, Convert.ToByte(singleMagic[index]));
                    singleMagic.RemoveAt(index);

                    index = new Random().Next(0, wideMagic.Count);
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x5A2C, Convert.ToByte(wideMagic[index]));
                    wideMagic.RemoveAt(index);
                    index = new Random().Next(0, wideMagic.Count);
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x59EC, Convert.ToByte(wideMagic[index]));
                    wideMagic.RemoveAt(index);
                    index = new Random().Next(0, wideMagic.Count);
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x59AC, Convert.ToByte(wideMagic[index]));
                    wideMagic.RemoveAt(index);


                    index = new Random().Next(0, powerMagic.Count);
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x596C, Convert.ToByte(powerMagic[index]));
                    powerMagic.RemoveAt(index);
                    index = new Random().Next(0, powerMagic.Count);
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x592C, Convert.ToByte(powerMagic[index]));
                    powerMagic.RemoveAt(index);
                    index = new Random().Next(0, powerMagic.Count);
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x58EC, Convert.ToByte(powerMagic[index]));
                    powerMagic.RemoveAt(index);
                    index = new Random().Next(0, powerMagic.Count);
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x58AC, Convert.ToByte(powerMagic[index]));
                    powerMagic.RemoveAt(index);
                    index = new Random().Next(0, powerMagic.Count);
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x586C, Convert.ToByte(powerMagic[index]));
                    powerMagic.RemoveAt(index);
                    index = new Random().Next(0, powerMagic.Count);
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x582C, Convert.ToByte(powerMagic[index]));
                    powerMagic.RemoveAt(index);
                    index = new Random().Next(0, powerMagic.Count);
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x57EC, Convert.ToByte(powerMagic[index]));
                    powerMagic.RemoveAt(index);
                } else if (ultimateHP[0] >= 500000) {
                    byte[] element = { 16, 16, 16 };
                    for (int i = 0; i < 3; i++) {
                        if (Globals.PARTY_SLOT[i] < 9) {
                            element[i] = CharacterToOppositeElement(Globals.PARTY_SLOT[i]);
                        }
                    }
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x5BBC, ElementToItem(0, element[0]));
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x5B84, ElementToItem(0, element[0]));
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x5B4C, ElementToItem(0, element[1]));
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x5B14, ElementToItem(0, element[1]));
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x5ADC, ElementToItem(0, element[2]));
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x5AA4, ElementToItem(0, element[2]));
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x5A6C, ElementToItem(0, element[new Random().Next(0, 2)]));
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x5A2C, ElementToItem(1, element[0]));
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x59EC, ElementToItem(1, element[1]));
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x59AC, ElementToItem(1, element[2]));
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x596C, ElementToItem(2, element[0]));
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x592C, ElementToItem(2, element[0]));
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x58EC, ElementToItem(2, element[1]));
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x58AC, ElementToItem(2, element[1]));
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x586C, ElementToItem(2, element[2]));
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x582C, ElementToItem(2, element[2]));
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x57EC, ElementToItem(2, element[new Random().Next(0, 2)]));
                } else {
                    byte[] element = { 16, 16, 16 };
                    for (int i = 0; i < 3; i++) {
                        if (Globals.PARTY_SLOT[i] < 9) {
                            element[i] = CharacterToOppositeElement(Globals.PARTY_SLOT[i]);
                        }
                    }

                    powerMagic.Add(0xFA);

                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x5BBC, ElementToItem(0, element[0]));
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x5B84, ElementToItem(0, element[1]));
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x5B4C, ElementToItem(0, element[2]));
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x5B14, ElementToItem(0, element[new Random().Next(0, 2)]));
                    index = new Random().Next(0, singleMagic.Count);
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x5ADC, Convert.ToByte(singleMagic[index]));
                    singleMagic.RemoveAt(index);
                    index = new Random().Next(0, singleMagic.Count);
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x5AA4, Convert.ToByte(singleMagic[index]));
                    singleMagic.RemoveAt(index);
                    index = new Random().Next(0, singleMagic.Count);
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x5A6C, Convert.ToByte(singleMagic[index]));
                    singleMagic.RemoveAt(index);

                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x5A2C, ElementToItem(1, element[new Random().Next(0, 2)]));
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x59EC, ElementToItem(1, element[new Random().Next(0, 2)]));
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x59AC, 0xF1);

                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x596C, ElementToItem(2, element[0]));
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x592C, ElementToItem(2, element[1]));
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x58EC, ElementToItem(2, element[2]));
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x58AC, 0xFA);
                    index = new Random().Next(0, powerMagic.Count);
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x586C, Convert.ToByte(powerMagic[index]));
                    powerMagic.RemoveAt(index);
                    index = new Random().Next(0, powerMagic.Count);
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x582C, Convert.ToByte(powerMagic[index]));
                    powerMagic.RemoveAt(index);
                    index = new Random().Next(0, powerMagic.Count);
                    Emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x57EC, Convert.ToByte(powerMagic[index]));
                    powerMagic.RemoveAt(index);
                }
            }

            magicChangeTurns += 1;
        }

        public byte CharacterToElement(byte character) {
            byte element = 0;
            if (character == 0) {
                element = 0x80;
            } else if (character == 1 || character == 5) {
                element = 0x40;
            } else if (character == 2 || character == 8) {
                element = 0x20;
            } else if (character == 3) {
                element = 0x4;
            } else if (character == 4) {
                element = 0x10;
            } else if (character == 6) {
                element = 0x1;
            } else if (character == 7) {
                element = 0x2;
            }
            return element;
        }

        public byte CharacterToOppositeElement(byte character) {
            byte element = 0;
            if (character == 0) {
                element = 0x1;
            } else if (character == 1 || character == 5) {
                element = 0x2;
            } else if (character == 2 || character == 8) {
                element = 0x4;
            } else if (character == 3) {
                element = 0x20;
            } else if (character == 4) {
                element = 0x80;
            } else if (character == 6) {
                element = 0x80;
            } else if (character == 7) {
                element = 0x40;
            }
            return element;
        }

        public byte ElementToItem(byte type, byte element) {
            byte item = 0xC0;
            if (type == 0) {
                if (element == 0x1)  //Water
                    item = 0xC6;
                else if (element == 0x2) // Earth
                    item = 0xC5;
                else if (element == 0x4) // Dark
                    item = 0xCA;
                else if (element == 0x10) // Thunder
                    item = 0xC2;
                else if (element == 0x20) // Light
                    item = 0xC9;
                else if (element == 0x40) // Wind
                    item = 0xC7;
                else if (element == 0x80) // Fire
                    item = 0xC3;
            } else if (type == 1) {
                if (element == 0x1)  //Water
                    item = 0xD6;
                else if (element == 0x2) // Earth
                    item = 0xD0;
                else if (element == 0x4) // Dark
                    item = 0xD8;
                else if (element == 0x8) // Non Elemental
                    item = 0xC1;
                else if (element == 0x10) // Thunder
                    item = 0xCF;
                else if (element == 0x20) // Light
                    item = 0xD2;
                else if (element == 0x40) // Wind
                    item = 0xDC;
                else if (element == 0x80) // Fire
                    item = 0xD1;
            } else if (type == 2) {
                if (element == 0x1)  //Water
                    item = 0xF3;
                else if (element == 0x2) // Earth
                    item = 0xF5;
                else if (element == 0x4) // Dark
                    item = 0xF7;
                else if (element == 0x8) // Non Elemental
                    item = 0xF1;
                else if (element == 0x10) // Thunder
                    item = 0xF8;
                else if (element == 0x20) // Light
                    item = 0xF6;
                else if (element == 0x40) // Wind
                    item = 0xF4;
                else if (element == 0x80) // Fire
                    item = 0xF2;
            }
            return item;
        }

        public void ElementalShift() {
            int lastItem = Emulator.ReadByte(Globals.MONS_ADDRESS[0] + 0xABC);
            if (lastItem == 0xC0 || lastItem == 0xC3 || lastItem == 0xD1 || lastItem == 0xF2) {
                Emulator.Battle.MonsterTable[0].Element = 128;
                Emulator.Battle.MonsterTable[0].Display_Element = 128;
            } else if (lastItem == 0xC6 || lastItem == 0xD6 || lastItem == 0xF3) {
                Emulator.Battle.MonsterTable[0].Element = 1;
                Emulator.Battle.MonsterTable[0].Display_Element = 1;
            } else if (lastItem == 0xC7 || lastItem == 0xDC || lastItem == 0xF4) {
                Emulator.Battle.MonsterTable[0].Element = 64;
                Emulator.Battle.MonsterTable[0].Display_Element = 64;
            } else if (lastItem == 0xC5 || lastItem == 0xD0 || lastItem == 0xF5) {
                Emulator.Battle.MonsterTable[0].Element = 2;
                Emulator.Battle.MonsterTable[0].Display_Element = 2;
            } else if (lastItem == 0xCA || lastItem == 0xD8 || lastItem == 0xF7) {
                Emulator.Battle.MonsterTable[0].Element = 4;
                Emulator.Battle.MonsterTable[0].Display_Element = 4;
            } else if (lastItem == 0xC9 || lastItem == 0xD2 || lastItem == 0xF6) {
                Emulator.Battle.MonsterTable[0].Element = 32;
                Emulator.Battle.MonsterTable[0].Display_Element = 32;
            } else if (lastItem == 0xC2 || lastItem == 0xCF || lastItem == 0xF8) {
                Emulator.Battle.MonsterTable[0].Element = 16;
                Emulator.Battle.MonsterTable[0].Display_Element = 16;
            } else if (lastItem == 0xC1 || lastItem == 0xF1) {
                Emulator.Battle.MonsterTable[0].Element = 8;
                Emulator.Battle.MonsterTable[0].Display_Element = 8;
            }
        }

        public void BreakArmor() {
            if (Emulator.Battle.MonsterTable[3].HP != ubHeartHPSave) {
                ubHeartHPSave = Emulator.Battle.MonsterTable[3].HP;
                ubArmorShellTurns = 1;
                ubArmorShellTP = Emulator.Battle.MonsterTable[0].Turn;
            }

            if (ubArmorShellTurns >= 1) {
                Emulator.Battle.MonsterTable[0].DF = 30;
                Emulator.Battle.MonsterTable[0].MDF = 36;
                if (Emulator.Battle.MonsterTable[0].Turn >= ubArmorShellTP) {
                    ubArmorShellTP = Emulator.Battle.MonsterTable[0].Turn;
                } else {
                    ubArmorShellTurns += 1;
                    ubArmorShellTP = Emulator.Battle.MonsterTable[0].Turn;
                }
            }

            if (ubArmorShellTurns > 3) {
                Emulator.Battle.MonsterTable[0].DF = 0;
                Emulator.Battle.MonsterTable[0].MDF = 0;
                ubArmorShellTurns = 0;
            }
        }

        public void TurnPointDamage() {
            if (Globals.ENCOUNTER_ID == 399) {
                bool resetTP = false, pass = false;

                for (int i = 0; i < 3; i++) {
                    if (Globals.PARTY_SLOT[i] < 9) {
                        if (Emulator.Battle.CharacterTable[i].Action == 8 || Emulator.Battle.CharacterTable[i].Action == 10) {
                            pass = true;
                        }
                    }
                }

                for (int i = 0; i < 3; i++) {
                    if (Globals.PARTY_SLOT[i] < 9) {
                        if (ubTrackHP[i] < Emulator.Battle.CharacterTable[i].HP) {
                            if (tpDamage > 0 && pass) {
                                Emulator.Battle.CharacterTable[i].Turn = (ushort) Math.Max(0, Emulator.Battle.CharacterTable[i].Turn - tpDamage);
                                ubTrackHP[i] = Emulator.Battle.CharacterTable[i].HP;
                                resetTP = true;
                            }
                        } else if (ubTrackHP[i] >= Emulator.Battle.CharacterTable[i].HP) {
                            ubTrackHP[i] = Emulator.Battle.CharacterTable[i].HP;
                        }
                    }
                }

                if (resetTP) {
                    tpDamage = 0;
                    Emulator.WriteByte(Globals.M_POINT - 0x50, 0);
                }

                byte moveSet = Emulator.ReadByte(Globals.M_POINT - 0x50);
                if ((moveSet == 68 || moveSet == 69 || moveSet == 76) && tpDamage == 0) {
                    if (moveSet == 68) {
                        tpDamage = 120;
                    } else if (moveSet == 69) {
                        tpDamage = 32767;
                    } else if (moveSet == 76) {
                        tpDamage = 32767;
                    }
                }
            }
        }

        public void BodyDamage() {
            if (Globals.ENCOUNTER_ID == 409) {
                if (ultimateHP[1] == 0 && ubVirageKilledPart == false) {
                    ultimateHP[0] -= 540000;
                    ubVirageKilledPart = true;
                }
            }

            for (int i = 2; i < 4; i++) {
                if (Emulator.Battle.MonsterTable[i].HP < ubTrackEHP[i]) {
                    ubTrackEHP[i] = Emulator.Battle.MonsterTable[i].HP;
                } else if (Emulator.Battle.MonsterTable[i].HP > ubTrackEHP[i]) {
                    ultimateHP[0] -= 54000;
                    ubTrackEHP[i] = Emulator.Battle.MonsterTable[i].HP;
                }
            }

            if (ultimateHP[0] < 0)
                ultimateHP[0] = 0;
        }

        public void DragoonBond() {
            if (ultimateHP[0] == ultimateMaxHP[0] && ultimateHP[1] == ultimateMaxHP[1])
                ubDragoonBondMode = -1;

            if (ubDragoonBondMode == -1) {
                if (ultimateHP[0] < ultimateMaxHP[0] && ultimateHP[1] < ultimateMaxHP[1]) {
                    double[] multiplyMode = { 1, 1 };
                    ubDragoonBondMode = 2;
                    ultimateHP[0] = (int) Math.Round(ultimateHP[0] * 1.8);
                    ultimateHP[1] = (int) Math.Round(ultimateHP[1] * 1.8);
                    ultimateMaxHP[0] = (int) Math.Round(ultimateMaxHP[0] * 1.8);
                    ultimateMaxHP[1] = (int) Math.Round(ultimateMaxHP[1] * 1.8);

                    if (Globals.ENCOUNTER_ID == 393) {
                        multiplyMode[0] = 23;
                        multiplyMode[1] = 23;
                    } else if (Globals.ENCOUNTER_ID == 397) {
                        multiplyMode[0] = 6.8;
                        multiplyMode[1] = 6.8;
                    }

                    Emulator.Battle.MonsterTable[0].AT = (ushort) Math.Round(originalMonsterStats[0, 1] * multiplyMode[0]);
                    Emulator.Battle.MonsterTable[1].AT = (ushort) Math.Round(originalMonsterStats[1, 1] * multiplyMode[0]);
                    Emulator.Battle.MonsterTable[0].MAT = (ushort) Math.Round(originalMonsterStats[0, 2] * multiplyMode[1]);
                    Emulator.Battle.MonsterTable[1].MAT = (ushort) Math.Round(originalMonsterStats[1, 2] * multiplyMode[1]);
                } else {
                    if (ultimateHP[0] < ultimateMaxHP[0]) {
                        ubDragoonBondMode = 0;
                    } else {
                        if (ultimateHP[1] < ultimateMaxHP[1]) {
                            ubDragoonBondMode = 1;
                        }
                    }
                }
            } else if (ubDragoonBondMode == 0) {
                if (ultimateHP[1] < ultimateMaxHP[1]) {
                    if (ultimateHP[0] > 0) {
                        int hpLoss = ultimateMaxHP[1] - ultimateHP[1];
                        ultimateHP[0] -= hpLoss;
                        ultimateHP[1] += hpLoss;
                    }
                    if (ultimateHP[0] < 0)
                        ultimateHP[0] = 0;
                }
                DragoonBondEnhance(0);
            } else if (ubDragoonBondMode == 1) {
                if (ultimateHP[0] < ultimateMaxHP[0]) {
                    if (ultimateHP[1] > 0) {
                        int hpLoss = ultimateMaxHP[0] - ultimateHP[0];
                        ultimateHP[1] -= hpLoss;
                        ultimateHP[0] += hpLoss;
                    }

                    if (ultimateHP[1] < 0)
                        ultimateHP[1] = 0;
                }
                DragoonBondEnhance(1);
            }
        }

        public void DragoonBondEnhance(int slot) {
            if (slot == 0) {
                if (ultimateHP[0] <= 0) {
                    double[] multiplyMode = { 1, 1 };
                    ubDragoonBondMode = 999;
                    if (Globals.ENCOUNTER_ID == 393) {
                        ultimateHP[1] = (int) Math.Round(ultimateHP[1] * 1.8);
                        ultimateMaxHP[1] = (int) Math.Round(ultimateMaxHP[1] * 1.8);
                        multiplyMode[0] = 26;
                        multiplyMode[1] = 26;
                    } else if (Globals.ENCOUNTER_ID == 397) {
                        ultimateHP[1] = (int) Math.Round(ultimateHP[1] * 1.5);
                        ultimateMaxHP[1] = (int) Math.Round(ultimateMaxHP[1] * 1.5);
                        multiplyMode[0] = 7;
                        multiplyMode[1] = 7;
                    }
                    Emulator.Battle.MonsterTable[1].AT = (ushort) Math.Round(originalMonsterStats[1, 1] * multiplyMode[0]);
                    Emulator.Battle.MonsterTable[1].MAT = (ushort) Math.Round(originalMonsterStats[1, 2] * multiplyMode[1]);
                    ultimateHP[0] = 1;
                    ultimateMaxHP[0] = 1;
                    Emulator.Battle.MonsterTable[0].HP = 0;
                }
            } else {
                if (ultimateHP[1] <= 0) {
                    double[] multiplyMode = { 1, 1 };
                    ubDragoonBondMode = 999;
                    if (Globals.ENCOUNTER_ID == 393) {
                        ultimateHP[0] = (int) Math.Round(ultimateHP[0] * 1.5);
                        ultimateMaxHP[0] = (int) Math.Round(ultimateMaxHP[0] * 1.5);
                        multiplyMode[0] = 26;
                        multiplyMode[1] = 26;
                    } else if (Globals.ENCOUNTER_ID == 397) {
                        ultimateHP[1] = (int) Math.Round(ultimateHP[0] * 1.5);
                        ultimateMaxHP[1] = (int) Math.Round(ultimateMaxHP[0] * 1.5);
                        multiplyMode[0] = 7;
                        multiplyMode[1] = 7;
                    }
                    Emulator.Battle.MonsterTable[0].AT = (ushort) Math.Round(originalMonsterStats[0, 1] * multiplyMode[0]);
                    Emulator.Battle.MonsterTable[0].MAT = (ushort) Math.Round(originalMonsterStats[0, 2] * multiplyMode[1]);
                    ultimateHP[1] = 1;
                    ultimateMaxHP[1] = 1;
                    Emulator.Battle.MonsterTable[1].HP = 0;
                }
            }
        }

        public void DragoonExtras() {
            if (Globals.ENCOUNTER_ID == 398) {
                if (ubBlockMenuHPTrack) {
                    for (int i = 0; i < 3; i++) {
                        if (Globals.PARTY_SLOT[i] < 9) {
                            if (ubTrackHP[i] != Emulator.Battle.CharacterTable[i].HP) {
                                if (Emulator.Battle.CharacterTable[i].HP < ubTrackHP[i]) {
                                    if (Emulator.ReadByte(Globals.M_POINT + 0xABC) == 78) {
                                        ubCustomStatusTurns[i] += 4;
                                        ubBlockMenuHPTrack = false;
                                        ubBlockMenuTPTrack = true;
                                    } else {
                                        if (new Random().Next(10) >= 3) {
                                            ubCustomStatusTurns[i] += 2;
                                            ubBlockMenuHPTrack = false;
                                            ubBlockMenuTPTrack = true;
                                        }
                                    }
                                    ubTrackHP[i] = Emulator.Battle.CharacterTable[i].HP;
                                }
                            } else {
                                ubTrackHP[i] = Emulator.Battle.CharacterTable[i].HP;
                            }
                        }
                    }
                }

                if (ubBlockMenuHPTrack) {
                    bool allZero = true;
                    for (int i = 0; i < 3; i++) {
                        if (Globals.PARTY_SLOT[i] < 9 && ubCustomStatusTurns[i] > 0) {
                            allZero = false;
                            if (ubTrackTP[i] != Emulator.Battle.CharacterTable[i].Turn) {
                                if (Emulator.Battle.CharacterTable[i].Turn < ubTrackTP[i]) {
                                    ubCustomStatusTurns[i] -= 1;
                                }
                                ubTrackTP[i] = Emulator.Battle.CharacterTable[i].Turn;
                            }

                            if (ubCustomStatusTurns[i] > 0) {
                                Emulator.WriteByte(Constants.GetAddress("SPECIAL_EFFECT") - 0x4 + (Globals.MONSTER_SIZE + i) * 0x20, 221);
                            } else {
                                Emulator.WriteByte(Constants.GetAddress("SPECIAL_EFFECT") - 0x4 + (Globals.MONSTER_SIZE + i) * 0x20, 8);
                            }

                            if (Emulator.Battle.CharacterTable[i].HP == 0)
                                ubCustomStatusTurns[i] = 0;
                        }
                    }

                    if (allZero)
                        ubBlockMenuTPTrack = false;
                } else {
                    for (int i = 0; i < 3; i++) {
                        if (Globals.PARTY_SLOT[i] < 9 && ubCustomStatusTurns[i] > 0) {
                            Emulator.WriteByte(Constants.GetAddress("SPECIAL_EFFECT") - 0x4 + (Globals.MONSTER_SIZE + i) * 0x20, 8);
                        }
                    }
                }

                byte moveSet = Emulator.ReadByte(Globals.M_POINT + 0xABC);

                if ((moveSet == 72 || moveSet == 73 || moveSet == 78) && (ubTrackDMove != moveSet)) {
                    if (moveSet == 72 || moveSet == 78) {
                        for (int i = 0; i < 3; i++) {
                            if (Globals.PARTY_SLOT[i] < 9) {
                                ubTrackHP[i] = Emulator.Battle.CharacterTable[i].HP;
                            }
                        }
                        ubBlockMenuHPTrack = true;
                        ubBlockMenuTPTrack = true;
                    } else {
                        byte partySize = 0;
                        for (int i = 0; i < 3; i++) {
                            if (Globals.PARTY_SLOT[i] < 9) {
                                partySize++;
                            }
                        }

                        if (new Random().Next(10) >= 2) {
                            ubCustomStatusTurns[new Random().Next(partySize)] += 2;
                            ubBlockMenuTPTrack = true;
                        }
                    }

                    for (int i = 0; i < 3; i++) {
                        if (Globals.PARTY_SLOT[i] < 9) {
                            ubTrackTP[i] = Emulator.Battle.CharacterTable[i].Turn;
                        }
                    }
                }
            }

            if (ubTrackDragoon) {
                if (Globals.ENCOUNTER_ID == 400) {
                    bool trackDamage = false;
                    bool setStream = false;
                    bool setMeteor = false;
                    bool setDragon = false;

                    for (int i = 0; i < 3; i++) {
                        if (Globals.PARTY_SLOT[i] < 9 && !trackDamage) {
                            if (ubTrackHP[i] != Emulator.Battle.CharacterTable[i].HP) {
                                if (Emulator.Battle.CharacterTable[i].HP < ubTrackHP[i]) {
                                    byte moveSet = Emulator.ReadByte(Globals.M_POINT + 0xABC);

                                    if (moveSet == 74) {
                                        if (new Random().Next(10) <= 3 && !setStream)
                                            setStream = true;
                                        trackDamage = true;
                                    } else if (moveSet == 75 && !setMeteor) {
                                        if (new Random().Next(10) <= 2 && !setMeteor)
                                            setMeteor = true;
                                        trackDamage = true;
                                    } else if (moveSet == 79) {
                                        if (new Random().Next(10) <= 5)
                                            setDragon = true;
                                        trackDamage = true;
                                    }

                                    ubTrackHP[i] = Emulator.Battle.CharacterTable[i].HP;
                                } else {
                                    ubTrackHP[i] = Emulator.Battle.CharacterTable[i].HP;
                                }
                            }
                        }
                    }

                    if (setStream) {
                        Emulator.WriteByte(Globals.M_POINT + 0xB5, 1);
                        Emulator.Battle.MonsterTable[0].A_AV = 30;
                        Emulator.Battle.MonsterTable[0].M_AV = 30;
                        Constants.WriteGLogOutput("[BOSS] Grand Stream accuracy lost activated.");
                    }

                    if (setMeteor) {
                        for (int i = 0; i < 3; i++) {
                            if (Globals.PARTY_SLOT[i] < 9) {
                                Emulator.Battle.CharacterTable[i].PWR_DF = -50;
                                Emulator.Battle.CharacterTable[i].PWR_MDF = -50;
                                Emulator.Battle.CharacterTable[i].PWR_DF_Turn = 2;
                                Emulator.Battle.CharacterTable[i].PWR_MDF_Turn = 2;
                                Constants.WriteGLogOutput("[BOSS] Meteor Strike defense down activated.");
                            }
                        }
                    }

                    if (setDragon) {
                        for (int i = 0; i < 3; i++) {
                            if (Globals.PARTY_SLOT[i] < 9) {
                                Emulator.Battle.CharacterTable[i].Speed_Down_Turn = 3;
                                Constants.WriteGLogOutput("[BOSS] Golden Dragon speed down activated.");
                            }
                        }
                    }

                    if (trackDamage) {
                        for (int i = 0; i < 3; i++) {
                            if (Globals.PARTY_SLOT[i] < 9) {
                                ubTrackHP[i] = Emulator.Battle.CharacterTable[i].HP;
                            }
                        }
                    }

                    if (Emulator.Battle.MonsterTable[0].A_AV > 0 && Emulator.ReadByte(Globals.M_POINT + 0xB5) == 0) {
                        Emulator.Battle.MonsterTable[0].A_AV = 0;
                        Emulator.Battle.MonsterTable[0].M_AV = 0;
                    }
                } else if (Globals.ENCOUNTER_ID == 401) {
                    bool trackDamage = false;

                    if (ubTrackTM > Emulator.Battle.MonsterTable[0].Turn && ubElectricUnleash == 0) {
                        ubElectricCharges += 1;
                    }

                    ubTrackTM = Emulator.Battle.MonsterTable[0].Turn;

                    byte moveSet = Emulator.ReadByte(Globals.M_POINT + 0xABC);

                    for (int i = 0; i < 3; i++) {
                        if (Globals.PARTY_SLOT[i] < 9 && !trackDamage) {
                            if (ubTrackHP[i] != Emulator.Battle.CharacterTable[i].HP && (ubElectricUnleash == 0 || ubElectricUnleash == 2)) {
                                if (Emulator.Battle.CharacterTable[i].HP < ubTrackHP[i]) {
                                    if (moveSet == 59) {
                                        ubElectricCharges += 1;
                                        trackDamage = true;
                                    } else if (moveSet == 70) {
                                        ubElectricCharges += 3;
                                        trackDamage = true;

                                        Emulator.Battle.CharacterTable[i].PWR_AT = -50;
                                        Emulator.Battle.CharacterTable[i].PWR_MAT = -50;
                                        Emulator.Battle.CharacterTable[i].PWR_AT_Turn = 3;
                                        Emulator.Battle.CharacterTable[i].PWR_MAT_Turn = 3;
                                        Constants.WriteGLogOutput("[BOSS] Attack down activated.");
                                    } else if (moveSet == 71) {
                                        ubElectricCharges += 5;
                                        trackDamage = true;

                                        Emulator.Battle.CharacterTable[i].PWR_DF = -50;
                                        Emulator.Battle.CharacterTable[i].PWR_MDF = -50;
                                        Emulator.Battle.CharacterTable[i].PWR_DF_Turn = 3;
                                        Emulator.Battle.CharacterTable[i].PWR_MDF_Turn = 3;
                                        Constants.WriteGLogOutput("[BOSS] Defense down activated.");
                                    } else if (moveSet == 77) {
                                        trackDamage = true;

                                        Emulator.Battle.CharacterTable[i].PWR_AT = -50;
                                        Emulator.Battle.CharacterTable[i].PWR_MAT = -50;
                                        Emulator.Battle.CharacterTable[i].PWR_DF = -50;
                                        Emulator.Battle.CharacterTable[i].PWR_MDF = -50;
                                        Emulator.Battle.CharacterTable[i].PWR_AT_Turn = 3;
                                        Emulator.Battle.CharacterTable[i].PWR_MAT_Turn = 3;
                                        Emulator.Battle.CharacterTable[i].PWR_DF_Turn = 3;
                                        Emulator.Battle.CharacterTable[i].PWR_MDF_Turn = 3;
                                        Constants.WriteGLogOutput("[BOSS] Power down activated.");
                                    }

                                    if (ubElectricUnleash == 2)
                                        ubElectricUnleash = 3;
                                }
                            } else {
                                if (moveSet == 77 && ubElectricUnleash == 0) {
                                    ubElectricCharges += 15;
                                    ubElectricUnleash = 1;
                                    trackDamage = true;
                                    break;
                                }
                            }
                        }
                    }

                    //Constants.WriteDebug("Charges: " + ubElectricCharges + "/" + ubElectricUnleash + " | Move: " + moveSet + " | Track:" + trackDamage);

                    if (trackDamage && ubElectricUnleash == 0) {
                        if (ubElectricCharges >= 30) {
                            ubElectricCharges = 30;
                            ubElectricUnleash = 1;
                        } else if (ubElectricCharges > 20) {
                            if (new Random().Next(100) <= 50) {
                                ubElectricUnleash = 1;
                            }
                        } else if (ubElectricCharges > 10) {
                            if (new Random().Next(100) <= 15) {
                                ubElectricUnleash = 1;
                            }
                        } else {
                            if (new Random().Next(100) <= 5) {
                                ubElectricCharges = 1;
                            }
                        }
                    }

                    if (ubElectricCharges >= 30)
                        ubElectricCharges = 30;

                    for (int i = 0; i < 3; i++) {
                        if (Globals.PARTY_SLOT[i] < 9) {
                            ubTrackHP[i] = Emulator.Battle.CharacterTable[i].HP;
                            if (Emulator.Battle.CharacterTable[i].Action == 192) {
                                Emulator.Battle.CharacterTable[i].PWR_AT = 0;
                                Emulator.Battle.CharacterTable[i].PWR_MAT = 0;
                                Emulator.Battle.CharacterTable[i].PWR_DF = 0;
                                Emulator.Battle.CharacterTable[i].PWR_MDF = 0;
                                Emulator.Battle.CharacterTable[i].PWR_AT_Turn = 0;
                                Emulator.Battle.CharacterTable[i].PWR_MAT_Turn = 0;
                                Emulator.Battle.CharacterTable[i].PWR_DF_Turn = 0;
                                Emulator.Battle.CharacterTable[i].PWR_MDF_Turn = 0;
                            }
                        }
                    }

                    if (ubElectricUnleash == 1) {
                        double[] multiplyMode = { 2.4, 2.4, 1, 1 };
                        if (Globals.CheckDMScript("btnEnrage") || CheckEnrageBoss()) {
                            if (enragedMode[0] == 1) {
                                Emulator.Battle.MonsterTable[0].AT = (ushort) Math.Round(originalMonsterStats[0, 1] * multiplyMode[0] * ((ubElectricCharges / 20) + 1) * 1.1);
                                Emulator.Battle.MonsterTable[0].MAT = (ushort) Math.Round(originalMonsterStats[0, 2] * multiplyMode[1] * ((ubElectricCharges / 20) + 1) * 1.1);
                                Emulator.Battle.MonsterTable[0].DF = (ushort) Math.Round(originalMonsterStats[0, 3] * multiplyMode[2] * ((ubElectricCharges / 20) + 1) * 1.1);
                                Emulator.Battle.MonsterTable[0].MDF = (ushort) Math.Round(originalMonsterStats[0, 4] * multiplyMode[3] * ((ubElectricCharges / 20) + 1) * 1.1);
                            } else if (enragedMode[0] == 2) {
                                Emulator.Battle.MonsterTable[0].AT = (ushort) Math.Round(originalMonsterStats[0, 1] * multiplyMode[0] * ((ubElectricCharges / 20) + 1) * 1.25);
                                Emulator.Battle.MonsterTable[0].MAT = (ushort) Math.Round(originalMonsterStats[0, 2] * multiplyMode[1] * ((ubElectricCharges / 20) + 1) * 1.25);
                                Emulator.Battle.MonsterTable[0].DF = (ushort) Math.Round(originalMonsterStats[0, 3] * multiplyMode[2] * ((ubElectricCharges / 20) + 1) * 1.25);
                                Emulator.Battle.MonsterTable[0].MDF = (ushort) Math.Round(originalMonsterStats[0, 4] * multiplyMode[3] * ((ubElectricCharges / 20) + 1) * 1.25);
                            }
                        } else {
                            Emulator.Battle.MonsterTable[0].AT = (ushort) Math.Round(originalMonsterStats[0, 1] * multiplyMode[0] * ((ubElectricCharges / 20) + 1));
                            Emulator.Battle.MonsterTable[0].MAT = (ushort) Math.Round(originalMonsterStats[0, 2] * multiplyMode[1] * ((ubElectricCharges / 20) + 1));
                            Emulator.Battle.MonsterTable[0].DF = (ushort) Math.Round(originalMonsterStats[0, 3] * multiplyMode[2] * ((ubElectricCharges / 20) + 1));
                            Emulator.Battle.MonsterTable[0].MDF = (ushort) Math.Round(originalMonsterStats[0, 4] * multiplyMode[3] * ((ubElectricCharges / 20) + 1));
                        }

                        ubElectricUnleash = 2;
                    } else if (ubElectricUnleash == 3) {
                        double[] multiplyMode = { 2.4, 2.4, 1, 1 };
                        if (trackDamage) {
                            if (Globals.CheckDMScript("btnEnrage") || CheckEnrageBoss()) {
                                if (enragedMode[0] == 1) {
                                    Emulator.Battle.MonsterTable[0].AT = (ushort) Math.Round(originalMonsterStats[0, 1] * multiplyMode[0] * 1.1);
                                    Emulator.Battle.MonsterTable[0].MAT = (ushort) Math.Round(originalMonsterStats[0, 2] * multiplyMode[1] * 1.1);
                                    Emulator.Battle.MonsterTable[0].DF = (ushort) Math.Round(originalMonsterStats[0, 3] * multiplyMode[2] * 1.1);
                                    Emulator.Battle.MonsterTable[0].MDF = (ushort) Math.Round(originalMonsterStats[0, 4] * multiplyMode[3] * 1.1);
                                } else if (enragedMode[0] == 2) {
                                    Emulator.Battle.MonsterTable[0].AT = (ushort) Math.Round(originalMonsterStats[0, 1] * multiplyMode[0] * 1.25);
                                    Emulator.Battle.MonsterTable[0].MAT = (ushort) Math.Round(originalMonsterStats[0, 2] * multiplyMode[1] * 1.25);
                                    Emulator.Battle.MonsterTable[0].DF = (ushort) Math.Round(originalMonsterStats[0, 3] * multiplyMode[2] * 1.25);
                                    Emulator.Battle.MonsterTable[0].MDF = (ushort) Math.Round(originalMonsterStats[0, 4] * multiplyMode[3] * 1.25);
                                }
                            } else {
                                Emulator.Battle.MonsterTable[0].AT = (ushort) Math.Round(originalMonsterStats[0, 1] * multiplyMode[0]);
                                Emulator.Battle.MonsterTable[0].MAT = (ushort) Math.Round(originalMonsterStats[0, 2] * multiplyMode[1]);
                                Emulator.Battle.MonsterTable[0].DF = (ushort) Math.Round(originalMonsterStats[0, 3] * multiplyMode[2]);
                                Emulator.Battle.MonsterTable[0].MDF = (ushort) Math.Round(originalMonsterStats[0, 4] * multiplyMode[3]);
                            }
                        }

                        ubElectricCharges = 0;
                        ubElectricUnleash = 0;
                    }
                }
            }

            ubTrackDMove = Emulator.ReadByte(Globals.M_POINT + 0xABC);
        }

        public void Countdown() {
            double totalDamage = (ultimateMaxHP[0] - ultimateHP[0]) + (ultimateMaxHP[1] - ultimateHP[1]) + (ultimateMaxHP[2] - ultimateHP[2]);
            if (Math.Floor(totalDamage / 40000) >= (ubLivesIncreased + 1)) {
                ubLivesIncreased += 1;

                if (Emulator.ReadByte("BOSS_COUNT") > 0) {
                    Emulator.WriteByte("BOSS_COUNT", (byte) (Emulator.ReadByte("BOSS_COUNT") - 1));
                }
            }

            ubTotalGold = ubLivesIncreased * 1000;

            if (ultimateHP[0] == 0 || Emulator.Battle.MonsterTable[0].HP == 0)
                ubTotalGold += 45000;

            if (ultimateHP[1] == 0 || Emulator.Battle.MonsterTable[1].HP == 0)
                ubTotalGold += 15000;

            if (ultimateHP[2] == 0 || Emulator.Battle.MonsterTable[2].HP == 0)
                ubTotalGold += 10000;

            if (Emulator.ReadByte("BOSS_COUNT") == 11) {
                for (int i = 0; i < 3; i++) {
                    if (Globals.PARTY_SLOT[i] < 9) {
                        Emulator.Battle.CharacterTable[i].Guard = 0;
                    }
                }
            }

            Emulator.WriteInt("BATTLE_REWARDS_GOLD", ubTotalGold);

            if (ultimateHP[0] == 0 || ultimateHP[1] == 0 || Emulator.ReadByte("BOSS_COUNT") == 11)
                UltimateBossDefeated();
        }

        public void UltimateEnrage() {
            if (Globals.ENCOUNTER_ID == 390) {
                double hpDamage = ultimateMaxHP[enrageChangeIndex] - ultimateHP[enrageChangeIndex];

                if (((hpDamage / ultimateMaxHP[enrageChangeIndex]) * 100) >= enrageChangeTurns + 1) {
                    enrageChangeTurns += 1;
                    Emulator.Battle.MonsterTable[enrageChangeIndex].AT = (ushort) Math.Round(Emulator.Battle.MonsterTable[enrageChangeIndex].AT * 1.01);
                    Emulator.Battle.MonsterTable[enrageChangeIndex].MAT = (ushort) Math.Round(Emulator.Battle.MonsterTable[enrageChangeIndex].MAT * 1.01);

                    if (enrageChangeTurns == 90 && enrageChangeIndex == 1) {
                        Emulator.Battle.MonsterTable[enrageChangeIndex].DF = (ushort) Math.Round(Emulator.Battle.MonsterTable[enrageChangeIndex].DF / 0.3);
                        Emulator.Battle.MonsterTable[enrageChangeIndex].MDF = (ushort) Math.Round(Emulator.Battle.MonsterTable[enrageChangeIndex].MDF / 0.3);
                    }
                }

                if (ultimateHP[0] == 0 && enrageChangeIndex == 0) {
                    enrageChangeIndex = 1;
                    enrageChangeTurns = 0;
                }
            } else if (Globals.ENCOUNTER_ID == 394) {
                double[] multiplyMode = { 5, 5 };
                double dragoons = 0.0;
                double enrage = 0.0;
                for (int i = 0; i < 3; i++) {
                    if (Emulator.ReadByte("DRAGOON_TURNS", i * 0x4) > 0) {
                        dragoons += 0.25;
                    }
                }

                if ((Emulator.Battle.MonsterTable[0].HP <= (Emulator.Battle.MonsterTable[0].Max_HP / 2)) && enragedMode[0] == 0) {
                    enragedMode[0] = 1;
                } else if ((Emulator.Battle.MonsterTable[0].HP <= (Emulator.Battle.MonsterTable[0].Max_HP / 4)) && enragedMode[0] == 1) {
                    enragedMode[0] = 2;
                }

                if (enragedMode[0] == 2 && Emulator.ReadByte("BOSS_COUNT") == 1)
                    Emulator.WriteByte("BOSS_COUNT", 2);

                enrage = enragedMode[0] == 1 ? 1.10 : enragedMode[0] == 2 ? 1.25 : 1;

                Emulator.Battle.MonsterTable[0].AT = (ushort) Math.Round(originalMonsterStats[0, 1] * multiplyMode[0] * (1 + dragoons) * enrage);
                Emulator.Battle.MonsterTable[0].MAT = (ushort) Math.Round(originalMonsterStats[0, 2] * multiplyMode[1] * (1 + dragoons) * enrage);
            } else if (Globals.ENCOUNTER_ID == 392) {
                double multiAT = 3;
                double divDF = 1.25;

                if (ubTrackTP[0] > Emulator.Battle.MonsterTable[0].Turn)
                    ubEnrageTurns += 0.10;

                Emulator.Battle.MonsterTable[0].AT = (ushort) Math.Round(originalMonsterStats[0, 1] * multiAT * (1 + ubEnrageTurns + ubEnrageTurnsPlus));

                if (Emulator.Battle.MonsterTable[0].Action == 28 && Emulator.ReadByte(Globals.M_POINT + 0xABC) == 226)
                    multiAT *= 2;

                Emulator.Battle.MonsterTable[0].MAT = (ushort) Math.Round(originalMonsterStats[0, 2] * multiAT * (1 + ubEnrageTurns + ubEnrageTurnsPlus));

                if (ubEnrageTurns + ubEnrageTurnsPlus >= 1.75) {
                    Emulator.Battle.MonsterTable[0].DF = (ushort) Math.Round(originalMonsterStats[0, 3] / 3);
                    Emulator.Battle.MonsterTable[0].MDF = (ushort) Math.Round(originalMonsterStats[0, 4] / 3);
                } else {
                    Emulator.Battle.MonsterTable[0].DF = (ushort) Math.Round(originalMonsterStats[0, 3] / (divDF + ubEnrageTurns + ubEnrageTurnsPlus));
                    Emulator.Battle.MonsterTable[0].MDF = (ushort) Math.Round(originalMonsterStats[0, 4] / (divDF + ubEnrageTurns + ubEnrageTurnsPlus));
                }

                if (Emulator.Battle.MonsterTable[0].Action == 28 && Emulator.ReadByte(Globals.M_POINT + 0xABC) == 225) {
                    ubEnrageTurns -= 5;
                    ubEnrageTurnsPlus += 0.10;
                    Emulator.WriteByte(Globals.M_POINT + 0xABC, 0);
                    if (ubEnrageTurns < 0)
                        ubEnrageTurns = 0;
                }

                ubTrackTP[0] = Emulator.Battle.MonsterTable[0].Turn;
            } else if (Globals.ENCOUNTER_ID == 442) {
                double multiAT = 3.5;
                double multiMAT = 3.5;

                multiMAT *= enragedMode[0] == 0 ? 1 : enragedMode[0] == 1 ? 1.1 : 1.25;

                if (ubZiegDragoon == 81)
                    multiMAT *= 1.25;

                Emulator.Battle.MonsterTable[0].MAT = (ushort) Math.Round(originalMonsterStats[0, 2] * multiMAT);

                if ((Emulator.Battle.MonsterTable[0].HP <= (Emulator.Battle.MonsterTable[0].Max_HP / 2)) && enragedMode[0] == 0) {
                    Emulator.Battle.MonsterTable[0].AT = (ushort) Math.Round(originalMonsterStats[0, 1] * multiAT * 1.1);
                    Emulator.Battle.MonsterTable[0].DF = (ushort) Math.Round(originalMonsterStats[0, 3] * 1.1);
                    Emulator.Battle.MonsterTable[0].MDF = (ushort) Math.Round(originalMonsterStats[0, 4] * 1.1);
                    enragedMode[0] = 1;
                } else if ((Emulator.Battle.MonsterTable[0].HP <= (Emulator.Battle.MonsterTable[0].Max_HP / 4)) && enragedMode[0] == 1) {
                    Emulator.Battle.MonsterTable[0].AT = (ushort) Math.Round(originalMonsterStats[0, 1] * multiAT * 1.25);
                    Emulator.Battle.MonsterTable[0].DF = (ushort) Math.Round(originalMonsterStats[0, 3] * 1.25);
                    Emulator.Battle.MonsterTable[0].MDF = (ushort) Math.Round(originalMonsterStats[0, 4] * 1.25);
                    enragedMode[0] = 2;
                }
            } else if (Globals.ENCOUNTER_ID == 420) {
                double multiMAT = 2.76;
                byte magicType = 0;
                int lastItem = Emulator.ReadByte(Globals.MONS_ADDRESS[0] + 0xABC);
                ArrayList singleMagic = new ArrayList();
                ArrayList wideMagic = new ArrayList();
                ArrayList powerMagic = new ArrayList();
                
                int guardingParty = 0;
                int guardingDragoon = 0;

                singleMagic.Add(0xC3);
                singleMagic.Add(0xC6);
                singleMagic.Add(0xC7);
                singleMagic.Add(0xC5);
                singleMagic.Add(0xCA);
                singleMagic.Add(0xC9);
                singleMagic.Add(0xC2);
                wideMagic.Add(0xD1);
                wideMagic.Add(0xD6);
                wideMagic.Add(0xDC);
                wideMagic.Add(0xD0);
                wideMagic.Add(0xD8);
                wideMagic.Add(0xD2);
                wideMagic.Add(0xCF);
                powerMagic.Add(0xF2);
                powerMagic.Add(0xF3);
                powerMagic.Add(0xF4);
                powerMagic.Add(0xF5);
                powerMagic.Add(0xF7);
                powerMagic.Add(0xF6);
                powerMagic.Add(0xF8);
                powerMagic.Add(0xFA);

                for (int i = 0; i < 3; i++) {
                    if (Globals.PARTY_SLOT[i] < 9) {
                        if (Emulator.Battle.CharacterTable[i].Guard == 1)
                            guardingParty++;
                        if (Emulator.ReadByte("DRAGOON_TURNS", (0x4 * i)) > 0)
                            guardingDragoon++;
                    }
                }

                if (singleMagic.Contains(lastItem)) {
                    double multi = 1 + (0.150 * guardingParty) + (0.075 * guardingDragoon);
                    Emulator.Battle.MonsterTable[0].MAT = (ushort) Math.Round(originalMonsterStats[0, 2] * multiMAT * multi);
                } else if (wideMagic.Contains(lastItem)) {
                    double multi = 1 + (0.100 * guardingParty) + (0.050 * guardingDragoon);
                    Emulator.Battle.MonsterTable[0].MAT = (ushort) Math.Round(originalMonsterStats[0, 2] * multiMAT * multi);
                } else if (powerMagic.Contains(lastItem)) {
                    double multi = 1 + (0.180 * guardingParty) + (0.090 * guardingDragoon);
                    Emulator.Battle.MonsterTable[0].MAT = (ushort) Math.Round(originalMonsterStats[0, 2] * multiMAT * multi);
                }
            }
        }

        public void EnhancedShield() {
            if (ultimateHP[0] == 0) {
                if (shieldTurnsTaken == -1) {
                    Constants.WritePLog("[BOSS] Doel is now invincible.");
                } else {
                    Constants.WritePLog("[BOSS] Shield Turns: " + shieldTurnsTaken + " | Next Shield: " + enrageChangeTurns + "/" + enhancedShieldTurns);
                }

                if (Emulator.ReadByte("SHIELD_ACTIVE") == 1) {
                    if (shieldTurnsTaken >= 0) {
                        if (shieldTurnsTaken >= 3) {
                            Emulator.WriteByte("SHIELD_ACTION", 7);
                            Emulator.WriteByte("BOSS_COUNT", 3);
                            shieldTurnsTaken -= 1;
                        } else {
                            Emulator.Battle.MonsterTable[1].P_Half = 1;
                            Emulator.Battle.MonsterTable[1].M_Half = 1;
                        }

                        if (ubTrackTP[1] > Emulator.Battle.MonsterTable[1].Turn)
                            shieldTurnsTaken += 1;
                    }

                    if (enrageChangeTurns >= enhancedShieldTurns && enhancedShieldTurns < 90) {
                        enhancedShieldTurns += (short) new Random().Next(10, 30);
                        if (enrageChangeTurns < 90 && (enhancedShieldTurns >= 91 && enhancedShieldTurns <= 125))
                            shieldTurnsTaken = 0;
                    }
                } else {
                    shieldTurnsTaken = 0;
                    if (enrageChangeTurns >= enhancedShieldTurns && enhancedShieldTurns <= 90) {
                        Emulator.WriteByte("BOSS_COUNT", 4);
                        Emulator.WriteByte("SHIELD_ACTION", 23);
                        Emulator.Battle.MonsterTable[1].Turn += 255;
                        enhancedShieldTurns += (short) new Random().Next(10, 30);
                        if (enrageChangeTurns <= 90 && (enhancedShieldTurns >= 91 && enhancedShieldTurns <= 125)) {
                            enhancedShieldTurns = 90;
                        }
                    }

                    Emulator.Battle.MonsterTable[1].P_Half = 0;
                    Emulator.Battle.MonsterTable[1].M_Half = 0;
                }

                ubTrackTP[1] = Emulator.Battle.MonsterTable[1].Turn;
            }
        }

        public void BodyProtect() {
            double[] damage = { 0, 0, 0 };
            double[] heal = { 0, 0, 0 };
            for (int i = 0; i < 3; i++) {
                if (ultimateHP[i] < ubTrackEHP[i]) {
                    damage[i] = (ubTrackEHP[i] - (ultimateHP[i])) * 1.0;
                    heal[i] = (ubTrackEHP[i] - (ultimateHP[i])) * 0.5;
                }
            }

            for (int i = 0; i < 3; i++) {
                if (damage[i] != 0 && ultimateHP[i] != 0) {
                    if (i == 0) { //hit head
                        ultimateHP[0] -= (int) Math.Round(damage[i]); //damage head
                        ultimateHP[1] += (int) Math.Round(heal[i]); //heal arm
                    } else if (i == 1) { //hit arm
                                         //ultimateHP[1] -= (int) Math.Round(damage[i]); //damage arm
                        ultimateHP[0] += (int) Math.Round(heal[i]); //heal head
                    } else if (i == 2) { //hit body
                        ultimateHP[0] -= (int) Math.Round(damage[i]); //damage head
                        ultimateHP[2] += (int) Math.Round(heal[i]); //heal body
                    }

                    if (ultimateHP[0] < 0) {
                        ultimateHP[0] = 0;
                        Emulator.Battle.MonsterTable[0].HP = 0;
                    }
                    if (ultimateHP[1] < 0) {
                        ultimateHP[1] = 0;
                        Emulator.Battle.MonsterTable[0].HP = 0;
                    }
                    if (ultimateHP[2] < 0) {
                        ultimateHP[2] = 0;
                        Emulator.Battle.MonsterTable[0].HP = 0;
                    }

                    ubTrackEHP[0] = ultimateHP[0];
                    ubTrackEHP[1] = ultimateHP[1];
                    ubTrackEHP[2] = ultimateHP[2];
                }
            }

            if (ultimateHP[0] == 0 && ubFinalAttack) {
                ubFinalAttack = false;
                Emulator.Battle.MonsterTable[0].MAT = (ushort) Math.Round(originalMonsterStats[0, 1] * 18);
            }
        }

        public void ReverseDragonBlockStaff() {
            if (Globals.DIFFICULTY_MODE.Equals("Normal")) {
                for (int i = 0; i < 3; i++) {
                    if (Globals.PARTY_SLOT[i] < 9 && Emulator.Battle.CharacterTable[i].DAT < 1000) {
                        Emulator.Battle.CharacterTable[i].DAT = (ushort) (Emulator.Battle.CharacterTable[i].DAT * 20);
                        Emulator.Battle.CharacterTable[i].DMAT = (ushort) (Emulator.Battle.CharacterTable[i].DMAT * 20);
                        Emulator.Battle.CharacterTable[i].DDF = (ushort) (Emulator.Battle.CharacterTable[i].DDF * 20);
                        Emulator.Battle.CharacterTable[i].DMDF = (ushort) (Emulator.Battle.CharacterTable[i].DMDF * 20);
                    }
                }
            }
        }

        public void ArmorGuard() {
            for (int i = 0; i < 3; i++) {
                if (Globals.PARTY_SLOT[i] < 9) {
                    if (Emulator.ReadByte("DRAGOON_TURNS", (0x4 * i)) > 0 && Emulator.Battle.CharacterTable[i].PWR_DF != 0) {
                        Emulator.Battle.CharacterTable[i].PWR_DF = 0;
                        Emulator.Battle.CharacterTable[i].PWR_DF_Turn = 0;
                        Emulator.Battle.CharacterTable[i].PWR_MDF = 0;
                        Emulator.Battle.CharacterTable[i].PWR_MDF_Turn = 0;
                    } else {
                        if (Emulator.Battle.CharacterTable[i].Guard == 1) {
                            if (Emulator.Battle.CharacterTable[i].PWR_DF == -50 || Emulator.Battle.CharacterTable[i].PWR_DF == 0 || Emulator.Battle.CharacterTable[i].PWR_DF_Turn == 0) {
                                Emulator.Battle.CharacterTable[i].PWR_DF = 50;
                                Emulator.Battle.CharacterTable[i].PWR_DF_Turn = 20;
                                Emulator.Battle.CharacterTable[i].PWR_MDF = 50;
                                Emulator.Battle.CharacterTable[i].PWR_MDF_Turn = 20;
                            }
                        } else {
                            if (Emulator.Battle.CharacterTable[i].PWR_DF == 50 || Emulator.Battle.CharacterTable[i].PWR_DF == 0 || Emulator.Battle.CharacterTable[i].PWR_DF_Turn == 0) {
                                Emulator.Battle.CharacterTable[i].PWR_DF = -50;
                                Emulator.Battle.CharacterTable[i].PWR_DF_Turn = 20;
                                Emulator.Battle.CharacterTable[i].PWR_MDF = -50;
                                Emulator.Battle.CharacterTable[i].PWR_MDF_Turn = 20;
                            }
                        }
                    }
                }
            }

            for (int i = 1; i < 3; i++) {
                if (Emulator.Battle.MonsterTable[i].HP < 65535 && Emulator.Battle.MonsterTable[0].HP > 0) {
                    int damage = 65535 - Emulator.Battle.MonsterTable[i].HP;
                    Emulator.Battle.MonsterTable[i].HP = 65535;
                    Emulator.Battle.MonsterTable[0].HP -= (ushort) damage;
                }
            }
        }

        public void DragoonGuard() {
            for (int i = 0; i < 3; i++) {
                if (Globals.PARTY_SLOT[i] < 9) {
                    if (Emulator.Battle.CharacterTable[i].Action == 10 && (Emulator.Battle.CharacterTable[i].Menu == 96 || Emulator.Battle.CharacterTable[i].Menu == 225)) {
                        Emulator.Battle.CharacterTable[i].Menu = (byte)(Emulator.Battle.CharacterTable[i].Menu + 2);
                    }
                }
            }

            for (int i = 0; i < 3; i++) {
                if (Globals.PARTY_SLOT[i] < 9) {
                    if (Emulator.Battle.CharacterTable[i].Action == 10 && Emulator.ReadByte(Globals.M_POINT + 0xD46) == 0) {
                        //Emulator.Battle.CharacterTable[i].Write("Death_Res", 192);
                        Emulator.Battle.CharacterTable[i].Guard = 1;
                    } else {
                        if (Emulator.Battle.CharacterTable[i].Action == 10) {
                            Emulator.Battle.CharacterTable[i].Guard = 0;
                        }
                    }
                }
            }


        }
        #endregion

        #region Extend Inventory
        public void ExtendInventory() {
            if (inventorySize > 32) {
                if (Emulator.ReadShort("INVENTORY_CAP_1") != inventorySize || Emulator.ReadShort("INVENTORY_CAP_2") != inventorySize || Emulator.ReadShort("INVENTORY_CAP_3") != inventorySize || Emulator.ReadShort("INVENTORY_CAP_4") != inventorySize) {
                    Emulator.WriteUShort("INVENTORY_CAP_1", (ushort) inventorySize);
                    Emulator.WriteUShort("INVENTORY_CAP_2", (ushort) inventorySize);
                    Emulator.WriteUShort("INVENTORY_CAP_3", (ushort) inventorySize);
                    Emulator.WriteUShort("INVENTORY_CAP_4", (ushort) inventorySize);
                    Emulator.WriteShort("ITEM_CAP", 808);
                }

                if (Emulator.ReadShort("INVENTORY_CAP_MINUS_1") != inventorySize || Emulator.ReadShort("INVENTORY_CAP_MINUS_2") != inventorySize) {
                    Emulator.WriteUShort("INVENTORY_CAP_MINUS_1", (ushort) inventorySize);
                    Emulator.WriteUShort("INVENTORY_CAP_MINUS_2", (ushort) inventorySize);
                    Emulator.WriteShort("ITEM_CAP", 808);
                }

                if (Emulator.ReadShort("INVENTORY_CAP_PLUS_1") != inventorySize || Emulator.ReadShort("INVENTORY_CAP_PLUS_2") != inventorySize) {
                    Emulator.WriteUShort("INVENTORY_CAP_PLUS_1", (ushort) inventorySize);
                    Emulator.WriteUShort("INVENTORY_CAP_PLUS_2", (ushort) inventorySize);
                    Emulator.WriteShort("ITEM_CAP", 808);
                }

                if (Emulator.ReadShort("ITEM_LIMIT_1") == 32 || Emulator.ReadShort("ITEM_LIMIT_2") == 32 || Emulator.ReadShort("ITEM_LIMIT_3") == 32) {
                    Emulator.WriteUShort("ITEM_LIMIT_1", (ushort) inventorySize);
                    Emulator.WriteUShort("ITEM_LIMIT_2", (ushort) inventorySize);
                    Emulator.WriteUShort("ITEM_LIMIT_3", (ushort) inventorySize);
                    Emulator.WriteShort("ITEM_CAP", 808);
                }
            }
        }
        #endregion

        #endregion

        #endregion

        #region Battle

        #region Dragoon Changes
        #endregion

        #region Elemental Bomb
        public void ElementalBomb() {
            if (ubElementalShift)
                return;

            if (Emulator.Memory.GameState == GameState.Battle && Controller.Main.StatsChanged && eleBombTurns == 0) {
                eleBombItemUsed = Emulator.ReadByte(Globals.MONS_ADDRESS[0] + 0xABC);
                if ((eleBombItemUsed >= 241 && eleBombItemUsed <= 248) || eleBombItemUsed == 250) {
                    if (Globals.PARTY_SLOT[2] < 9) {
                        byte player1Action = Emulator.Battle.CharacterTable[0].Action;
                        byte player2Action = Emulator.Battle.CharacterTable[1].Action;
                        byte player3Action = Emulator.Battle.CharacterTable[2].Action;
                        if (player1Action == 24 && (player2Action == 16 || player2Action == 18 || player2Action == 208) && (player3Action == 16 || player3Action == 18 || player3Action == 208)) {
                            eleBombSlot = 0;
                            eleBombTurns = 5;
                            eleBombChange = false;
                        }
                        if (player2Action == 24 && (player1Action == 16 || player1Action == 18 || player1Action == 208) && (player3Action == 16 || player3Action == 18 || player3Action == 208)) {
                            eleBombSlot = 1;
                            eleBombTurns = 5;
                            eleBombChange = false;
                        }
                        if (player3Action == 24 && (player1Action == 16 || player1Action == 18 || player1Action == 208) && (player2Action == 16 || player2Action == 18 || player2Action == 208)) {
                            eleBombSlot = 2;
                            eleBombTurns = 5;
                            eleBombChange = false;
                        }
                    } else if (Globals.PARTY_SLOT[1] < 9) {
                        byte player1Action = Emulator.Battle.CharacterTable[0].Action;
                        byte player2Action = Emulator.Battle.CharacterTable[1].Action;
                        if (player1Action == 24 && (player2Action == 16 || player2Action == 18 || player2Action == 208)) {
                            eleBombSlot = 0;
                            eleBombTurns = 5;
                            eleBombChange = false;
                        }
                        if (player2Action == 24 && (player1Action == 16 || player1Action == 18 || player1Action == 208)) {
                            eleBombSlot = 1;
                            eleBombTurns = 5;
                            eleBombChange = false;
                        }
                    } else {
                        byte player1Action = Emulator.Battle.CharacterTable[0].Action;
                        if (player1Action == 24) {
                            eleBombSlot = 0;
                            eleBombTurns = 5;
                            eleBombChange = false;
                        }
                    }
                }

                //Constants.WriteDebug("Item: " + eleBombItemUsed + " | Slot: " + eleBombSlot + " | Turns: " + eleBombTurns + " | Change: " + eleBombChange);
            } else {
                //Constants.WriteDebug("Item: " + eleBombItemUsed + " | Slot: " + eleBombSlot + " | Turns: " + eleBombTurns + " | Change: " + eleBombChange + " | Element: " + eleBombElement + " | Action: " + Emulator.Battle.CharacterTable[eleBombSlot].Read("Action"));
                if (Emulator.ReadUShort("BATTLE_VALUE") == 41215 && Controller.Main.StatsChanged && eleBombSlot >= 0) {
                    if ((Emulator.Battle.CharacterTable[eleBombSlot].Action == 8 || Emulator.Battle.CharacterTable[eleBombSlot].Action == 10) && !eleBombChange) {
                        eleBombChange = true;
                        if (eleBombTurns == 5) {
                            ushort element = 0;

                            if (eleBombItemUsed == 241)
                                element = 0;
                            else if (eleBombItemUsed == 242)
                                element = 128;
                            else if (eleBombItemUsed == 243)
                                element = 1;
                            else if (eleBombItemUsed == 244)
                                element = 64;
                            else if (eleBombItemUsed == 245)
                                element = 2;
                            else if (eleBombItemUsed == 246)
                                element = 32;
                            else if (eleBombItemUsed == 247)
                                element = 4;
                            else if (eleBombItemUsed == 248)
                                element = 16;
                            else if (eleBombItemUsed == 250)
                                element = 8;

                            eleBombElement = (byte) element;

                            for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                                eleBombOldElement[i] = Emulator.Battle.MonsterTable[i].Element;
                                Emulator.Battle.MonsterTable[i].Element = (byte) element;
                                Emulator.Battle.MonsterTable[i].Display_Element = (byte) element;
                            }

                            eleBombTurns -= 1;
                        }
                    }

                    if (eleBombChange && (Emulator.Battle.CharacterTable[eleBombSlot].Action == 0 || Emulator.Battle.CharacterTable[eleBombSlot].Action == 2)) {
                        eleBombChange = false;
                        eleBombTurns -= 1;
                        if (eleBombTurns <= 0) {
                            for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                                Emulator.Battle.MonsterTable[i].Element = (byte) eleBombOldElement[i];
                                Emulator.Battle.MonsterTable[i].Display_Element = (byte) eleBombOldElement[i];
                            }
                        }
                    }

                    if (Emulator.Battle.CharacterTable[eleBombSlot].Action == 192) {
                        for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                            Emulator.Battle.MonsterTable[i].Element = (byte) eleBombOldElement[i];
                            Emulator.Battle.MonsterTable[i].Display_Element = (byte) eleBombOldElement[i];
                        }
                        eleBombChange = false;
                        eleBombTurns = 0;
                        eleBombElement = 255;
                        eleBombSlot = 255;
                        eleBombItemUsed = 255;
                    }
                } else {
                    if (Globals.EXITING_BATTLE == 1) {
                        eleBombTurns = 0;
                        eleBombElement = 255;
                        eleBombSlot = 255;
                        eleBombItemUsed = 255;
                    }
                }
            }
        }
        #endregion

        #region No HP Decay Soul Eater
        public void NoHPDecaySoulEater() {
            if (Emulator.Memory.GameState == GameState.Battle && Controller.Main.StatsChanged && !noHPDecayOnBattleEntry) {
                for (int i = 0; i < 3; i++) {
                    if (Globals.PARTY_SLOT[i] == 0) {
                        if (Emulator.Battle.CharacterTable[i].HP_Regen == 246 || Emulator.Battle.CharacterTable[i].HP_Regen == -10) { //Default
                            Emulator.Battle.CharacterTable[i].HP_Regen = 0;
                        } else if (Emulator.Battle.CharacterTable[i].HP_Regen == -3) { //Heal Ring
                            Emulator.Battle.CharacterTable[i].HP_Regen = 7;
                        } else if (Emulator.Battle.CharacterTable[i].HP_Regen == 256) {
                            Emulator.Battle.CharacterTable[i].HP_Regen = 10;
                        } else if (Emulator.Battle.CharacterTable[i].HP_Regen == 0 && Emulator.ReadByte("CHAR_TABLE", 0 * 0x2C + 0x18) == 0x7D) { //Therapy Ring
                            Emulator.Battle.CharacterTable[i].HP_Regen = 10;
                        }
                    }
                }
                noHPDecayOnBattleEntry = true;
            } else {
                if (Emulator.Memory.GameState != GameState.Battle && noHPDecayOnBattleEntry) {
                    noHPDecayOnBattleEntry = false;
                }
            }
        }
        #endregion

        #region Monster Names As HP
        public void MonsterHPNames() {
            if (Emulator.ReadUShort("BATTLE_VALUE") == 41215 && Controller.Main.StatsChanged && Constants.BATTLE_UI) {
                for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                    int lastX = 0;
                    long hpName = Constants.GetAddress("MONSTERS_NAMES") + (i * 0x2C);
                    char[] hpArray = Emulator.Battle.MonsterTable[i].HP.ToString().ToCharArray();
                    if (ultimateHP[i] > 0) {
                        hpArray = ultimateHP[i].ToString().ToCharArray();
                    }
                    for (int x = 0; x < hpArray.Length; x++) {
                        Emulator.WriteUShort(hpName + (x * 2), GetNameHP(hpArray[x]));
                        lastX = x;
                    }
                    Emulator.WriteInt(hpName + ((lastX + 1) * 2), 41215);
                }
            }
        }

        public ushort GetNameHP(char single) {
            // return (ushort) (Emulator.GetCharacterByChar(single) + GetNameHPOffset()); TODO
            return 0;
        }

        public ushort GetNameHPOffset() {
            ushort offset = 0x0;
            if (Constants.REGION == Region.JPN) {
                offset += 7;
            }
            return offset;
        }
        #endregion

        #region Damage Tracker
        public void DamageTracker() {
            if (Emulator.Memory.GameState == GameState.Battle && Controller.Main.StatsChanged && !damageTrackerOnBattleEntry) {
                for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                    if (ultimateHP[i] > 0) {
                        dmgTrkHP[i] = ultimateHP[i];
                    } else {
                        dmgTrkHP[i] = Emulator.Battle.MonsterTable[i].HP;
                    }
                }
                for (int i = 0; i < 3; i++) {
                    dmgTrkChr[i] = 0;
                }
                damageTrackerOnBattleEntry = true;
            } else {
                if (Emulator.Memory.GameState != GameState.Battle && damageTrackerOnBattleEntry) {
                    damageTrackerOnBattleEntry = false;
                } else {
                    if (Emulator.Memory.GameState == GameState.Battle && Controller.Main.StatsChanged) {
                        bool partyAttacking = false;
                        for (int i = 0; i < 3; i++) {
                            if (Globals.PARTY_SLOT[i] < 9) {
                                byte action = Emulator.Battle.CharacterTable[i].Action;
                                if (action == 24 || action == 26 || action == 136 || action == 138) {
                                    partyAttacking = true;
                                    dmgTrkSlot = i;
                                }
                            }
                        }

                        //if (partyAttacking || ubCheckDamageCycle > 0) {
                        for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                            if (ultimateHP[i] > 0) {
                                if (ultimateHP[i] < dmgTrkHP[i]) {
                                    dmgTrkChr[dmgTrkSlot] += dmgTrkHP[i] - ultimateHP[i];
                                    dmgTrkHP[i] = ultimateHP[i];
                                } else if (ultimateHP[i] > dmgTrkHP[i]) {
                                    dmgTrkHP[i] = ultimateHP[i];
                                }
                            } else {
                                if (Emulator.Battle.MonsterTable[i].HP < dmgTrkHP[i]) {
                                    dmgTrkChr[dmgTrkSlot] += dmgTrkHP[i] - Emulator.Battle.MonsterTable[i].HP;
                                    dmgTrkHP[i] = Emulator.Battle.MonsterTable[i].HP;
                                } else if (Emulator.Battle.MonsterTable[i].HP > dmgTrkHP[i]) {
                                    dmgTrkHP[i] = Emulator.Battle.MonsterTable[i].HP;
                                }
                            }
                        }
                        //}
                        Globals.SetCustomValue("Damage Tracker1", dmgTrkChr[0]);
                        Globals.SetCustomValue("Damage Tracker2", dmgTrkChr[1]);
                        Globals.SetCustomValue("Damage Tracker3", dmgTrkChr[2]);
                        Constants.WritePLog("Damage Track: " + dmgTrkChr[0] + " / " + dmgTrkChr[1] + " / " + dmgTrkChr[2]);
                    }
                }
            }
        }
        #endregion

        #region Enrage Mode
        public void EnrageMode(int i) {
            if ((Globals.CheckDMScript("btnEnrage") || CheckEnrageBoss()) && !ubUltimateEnrage) {
                if ((Emulator.Battle.MonsterTable[i].HP <= (Emulator.Battle.MonsterTable[i].Max_HP / 2)) && enragedMode[i] == 0) {
                    Emulator.Battle.MonsterTable[i].AT = (ushort) Math.Round(originalMonsterStats[i, 1] * 1.1);
                    Emulator.Battle.MonsterTable[i].MAT = (ushort) Math.Round(originalMonsterStats[i, 2] * 1.1);
                    Emulator.Battle.MonsterTable[i].DF = (ushort) Math.Round(originalMonsterStats[i, 3] * 1.1);
                    Emulator.Battle.MonsterTable[i].MDF = (ushort) Math.Round(originalMonsterStats[i, 4] * 1.1);
                    enragedMode[i] = 1;
                } else if ((Emulator.Battle.MonsterTable[i].HP <= (Emulator.Battle.MonsterTable[i].Max_HP / 4)) && enragedMode[i] == 1) {
                    Emulator.Battle.MonsterTable[i].AT = (ushort) Math.Round(originalMonsterStats[i, 1] * 1.25);
                    Emulator.Battle.MonsterTable[i].MAT = (ushort) Math.Round(originalMonsterStats[i, 2] * 1.25);
                    Emulator.Battle.MonsterTable[i].DF = (ushort) Math.Round(originalMonsterStats[i, 3] * 1.25);
                    Emulator.Battle.MonsterTable[i].MDF = (ushort) Math.Round(originalMonsterStats[i, 4] * 1.25);
                    enragedMode[i] = 2;
                }
            }
        }

        public bool CheckEnrageBoss() {
            if (enrageBoss) {
                if (Globals.ENCOUNTER_ID == 384 || Globals.ENCOUNTER_ID == 385 || Globals.ENCOUNTER_ID == 386 || Globals.ENCOUNTER_ID == 387 || Globals.ENCOUNTER_ID == 388 || Globals.ENCOUNTER_ID == 389 || Globals.ENCOUNTER_ID == 390 || Globals.ENCOUNTER_ID == 391 || Globals.ENCOUNTER_ID == 392 || Globals.ENCOUNTER_ID == 393 || Globals.ENCOUNTER_ID == 394 || Globals.ENCOUNTER_ID == 395 || Globals.ENCOUNTER_ID == 396 || Globals.ENCOUNTER_ID == 397 || Globals.ENCOUNTER_ID == 398 || Globals.ENCOUNTER_ID == 399 || Globals.ENCOUNTER_ID == 400 || Globals.ENCOUNTER_ID == 401 || Globals.ENCOUNTER_ID == 402 || Globals.ENCOUNTER_ID == 403 || Globals.ENCOUNTER_ID == 408 || Globals.ENCOUNTER_ID == 409 || Globals.ENCOUNTER_ID == 410 || Globals.ENCOUNTER_ID == 411 || Globals.ENCOUNTER_ID == 412 || Globals.ENCOUNTER_ID == 413 || Globals.ENCOUNTER_ID == 414 || Globals.ENCOUNTER_ID == 415 || Globals.ENCOUNTER_ID == 416 || Globals.ENCOUNTER_ID == 417 || Globals.ENCOUNTER_ID == 418 || Globals.ENCOUNTER_ID == 421 || Globals.ENCOUNTER_ID == 422 || Globals.ENCOUNTER_ID == 423 || Globals.ENCOUNTER_ID == 430 || Globals.ENCOUNTER_ID == 431 || Globals.ENCOUNTER_ID == 432 || Globals.ENCOUNTER_ID == 433 || Globals.ENCOUNTER_ID == 434 || Globals.ENCOUNTER_ID == 435 || Globals.ENCOUNTER_ID == 436 || Globals.ENCOUNTER_ID == 437 || Globals.ENCOUNTER_ID == 438 || Globals.ENCOUNTER_ID == 439 || Globals.ENCOUNTER_ID == 442 || Globals.ENCOUNTER_ID == 443 || Globals.ENCOUNTER_ID == 444 || Globals.ENCOUNTER_ID == 445 || Globals.ENCOUNTER_ID == 446 || Globals.ENCOUNTER_ID == 447 || Globals.ENCOUNTER_ID == 448 || Globals.ENCOUNTER_ID == 449 || Globals.ENCOUNTER_ID == 489) {
                    return true;
                } else {
                    return false;
                }
            } else {
                return false;
            }
        }
        #endregion

        #region Battle Rows
        public void BattleFormationRows() {
            if (Emulator.Memory.GameState == GameState.Battle && Controller.Main.StatsChanged && !battleRowsOnBattleEntry) {
                this.Dispatcher.BeginInvoke(new Action(() => {
                    for (int i = 0; i < 3; i++) {
                        if (Globals.PARTY_SLOT[i] < 9) {
                            int rowType = battleRow[Globals.PARTY_SLOT[i]].SelectedIndex;
                            int boostType = battleRowBoost[Globals.PARTY_SLOT[i]].SelectedIndex;
                            double attackBoost = 0, magicBoost = 0, defenseBoost = 0;

                            if (rowType == 0) { //Stay
                                if (boostType == 1) {
                                    attackBoost = 1.1;
                                    magicBoost = 1;
                                    defenseBoost = 0.75;
                                } else {
                                    attackBoost = 1;
                                    magicBoost = 1.1;
                                    defenseBoost = 0.75;
                                }
                            } else if (rowType == 1) { //Front
                                if (boostType == 0) {
                                    attackBoost = 1.25;
                                    magicBoost = 1.25;
                                    defenseBoost = 0.5;
                                } else if (boostType == 1) {
                                    attackBoost = 1.5;
                                    magicBoost = 1;
                                    defenseBoost = 0.25;
                                } else {
                                    attackBoost = 1;
                                    magicBoost = 1.5;
                                    defenseBoost = 0.25;
                                }
                            } else if (rowType == 2) { //Back
                                if (boostType == 0) {
                                    attackBoost = 0.75;
                                    magicBoost = 0.75;
                                    defenseBoost = 1.25;
                                } else if (boostType == 1) {
                                    attackBoost = 1;
                                    magicBoost = 0.5;
                                    defenseBoost = 1.1;
                                } else {
                                    attackBoost = 0.5;
                                    magicBoost = 1;
                                    defenseBoost = 1.1;
                                }
                            }

                            //if (Globals.PARTY_SLOT[i] != 2 && Globals.PARTY_SLOT[i] != 8)
                            Emulator.Battle.CharacterTable[i].AT = (ushort) Math.Round(Emulator.Battle.CharacterTable[i].AT * attackBoost);
                            Emulator.Battle.CharacterTable[i].MAT = (ushort) Math.Round(Emulator.Battle.CharacterTable[i].MAT * magicBoost);
                            Emulator.Battle.CharacterTable[i].DF = (ushort) Math.Round(Emulator.Battle.CharacterTable[i].DF * defenseBoost);
                            Emulator.Battle.CharacterTable[i].MDF = (ushort) Math.Round(Emulator.Battle.CharacterTable[i].MDF * defenseBoost);

                            if (rowType == 1)
                                Emulator.Battle.CharacterTable[i].Pos_FB = 5;
                            else if (rowType == 2)
                                Emulator.Battle.CharacterTable[i].Pos_FB = 13;
                        }
                    }
                }), DispatcherPriority.ContextIdle);

                battleRowsOnBattleEntry = true;
            } else {
                if (Emulator.Memory.GameState != GameState.Battle && battleRowsOnBattleEntry) {
                    battleRowsOnBattleEntry = false;
                }
            }
        }
        #endregion

        #region * Turn Battle
        public void EATB() {
            this.Dispatcher.BeginInvoke(new Action(() => {
                if (Emulator.Memory.GameState == GameState.Battle && Controller.Main.StatsChanged && Globals.CheckDMScript("btnEATB")) {
                    if (!eatbOnBattleEntry)
                        timePlayed = Emulator.ReadInt("TIME_PLAYED");

                    if (timePlayed + 60 < Emulator.ReadInt("TIME_PLAYED")) {
                        timePlayed = Emulator.ReadInt("TIME_PLAYED");

                        for (int i = 0; i < 3; i++) {
                            if (Globals.PARTY_SLOT[i] < 9 && Emulator.Battle.CharacterTable[i].HP > 0) {
                                if (cooldowns > 0) {
                                    cooldowns -= 1;
                                    extraTurnBattleC[i] += Emulator.Battle.CharacterTable[i].SPD / 2;
                                } else {
                                    extraTurnBattleC[i] += Emulator.Battle.CharacterTable[i].SPD;
                                }
                                if (extraTurnBattleC[i] > 6000) {
                                    extraTurnBattleC[i] = 6000;
                                    EatbBeep(i);
                                }
                                progressCATB[i].Value = extraTurnBattleC[i];
                            }
                        }

                        for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                            if (Emulator.Battle.MonsterTable[i].HP > 0) {
                                if (Globals.ENCOUNTER_ID == 390 && i == 1)
                                    break;
                                if (Globals.ENCOUNTER_ID == 433 && i == 1)
                                    if (Emulator.Battle.MonsterTable[0].HP > 0)
                                        break;
                                if (Globals.ENCOUNTER_ID == 433 && i == 2)
                                    if (Emulator.Battle.MonsterTable[1].HP > 0)
                                        break;
                                if (Globals.ENCOUNTER_ID == 433 && i >= 1)
                                    if (Emulator.Battle.MonsterTable[i - 1].HP > 0)
                                        continue;

                                extraTurnBattleM[i] += Emulator.Battle.MonsterTable[i].SPD;
                                if (extraTurnBattleM[i] > 7000 + (1000 * i)) {
                                    extraTurnBattleM[i] = 0;
                                    Emulator.WriteByte(Globals.MONS_ADDRESS[i] + 0x45, (byte) (Emulator.ReadByte(Globals.MONS_ADDRESS[i] + 0x45) + 1));
                                }
                                progressMATB[i].Value = extraTurnBattleM[i];

                                if (Globals.ENCOUNTER_ID == 385 || Globals.ENCOUNTER_ID == 415 || Globals.ENCOUNTER_ID == 412 || Globals.ENCOUNTER_ID == 417 || Globals.ENCOUNTER_ID == 394 || Globals.ENCOUNTER_ID == 422 || Globals.ENCOUNTER_ID == 432 || Globals.ENCOUNTER_ID == 443)
                                    break;
                            }
                        }
                    }

                    Globals.SetCustomValue("EATBC1", extraTurnBattleC[0]);
                    Globals.SetCustomValue("EATBC2", extraTurnBattleC[1]);
                    Globals.SetCustomValue("EATBC3", extraTurnBattleC[2]);
                    Globals.SetCustomValue("EATBM1", extraTurnBattleM[0]);
                    Globals.SetCustomValue("EATBM2", extraTurnBattleM[1]);
                    Globals.SetCustomValue("EATBM3", extraTurnBattleM[2]);
                    Globals.SetCustomValue("EATBM4", extraTurnBattleM[3]);
                    Globals.SetCustomValue("EATBM5", extraTurnBattleM[4]);
                    eatbOnBattleEntry = true;
                } else {
                    if (Emulator.Memory.GameState != GameState.Battle && eatbOnBattleEntry) {
                        eatbOnBattleEntry = false;
                        for (int i = 0; i < 3; i++) {
                            progressCATB[i].Value = 0;
                            extraTurnBattleC[i] = 0;
                        }
                        for (int i = 0; i < 5; i++) {
                            progressMATB[i].Value = 0;
                            extraTurnBattleM[i] = 0;
                        }
                    }
                }
            }), DispatcherPriority.ContextIdle);
        }

        public void ExtraTurnBattle(ref int eatbTime, int slot) {
            eatbSound[slot] = false;
            eatbTime = 0;
            if (Globals.CheckDMScript("btnATB"))
                cooldowns = 0;
            else
                cooldowns = cooldowns + 90;

            Constants.WriteDebugProgram("[EATB] Dragoon Turns: " + Emulator.ReadByte("DRAGOON_TURNS", 0x4 * slot) + "/" + Emulator.ReadShort("DRAGOON_TURNS", 0x4 * slot) + "/" + Emulator.ReadInt("DRAGOON_TURNS", 0x4 * slot));

            if (Emulator.ReadByte("DRAGOON_TURNS", 0x4 * slot) > 0 && Emulator.ReadByte("DRAGOON_TURNS", 0x4 * slot) < 6)
                Emulator.Battle.CharacterTable[slot].Action = 10;
            else
                Emulator.Battle.CharacterTable[slot].Action = 8;
        }

        public void QTB() {
            if (Emulator.Memory.GameState == GameState.Battle && Controller.Main.StatsChanged && Globals.CheckDMScript("btnQTB")) {
                if (!qtbOnBattleEntry) {
                    this.Dispatcher.BeginInvoke(new Action(() => {
                        for (int i = 0; i < 3; i++)
                            currentHP[i] = Emulator.Battle.CharacterTable[i].HP;
                        qtbTurns = 2;
                        pgrQTB.Value = qtbTurns;
                    }), DispatcherPriority.ContextIdle);
                }

                byte partyMembersAttacked = 0;
                for (int i = 0; i < 3; i++) {
                    if (Globals.PARTY_SLOT[i] < 9 && Emulator.Battle.CharacterTable[i].HP > 0) {
                        if (currentHP[i] < Emulator.Battle.CharacterTable[i].HP) {
                            partyMembersAttacked += 1;
                            currentHP[i] = Emulator.Battle.CharacterTable[i].HP;
                        } else {
                            if (uiCombo["cboQTB"] == Globals.PARTY_SLOT[i]) {
                                byte healAmount = 10;
                                if (Emulator.Battle.CharacterTable[i].Accessory == 125) {
                                    healAmount = 20;
                                }
                                if (Emulator.Battle.CharacterTable[i].HP + 1 <= (double) (currentHP[i] + Math.Round((double) Emulator.Battle.CharacterTable[i].Max_HP / healAmount) + 2)) {
                                    AddQTB();
                                    currentHP[i] = Emulator.Battle.CharacterTable[i].HP;
                                }
                            } else {
                                currentHP[i] = Emulator.Battle.CharacterTable[i].HP;
                            }
                        }

                        if (uiCombo["cboQTB"] == Globals.PARTY_SLOT[i]) {
                            if (Emulator.Battle.CharacterTable[i].Action == 8 || Emulator.Battle.CharacterTable[i].Action == 10) {
                                if (!qtbLeaderTurn)
                                    AddQTB();
                                qtbLeaderTurn = true;
                            } else {
                                qtbLeaderTurn = false;
                            }
                        }
                    }
                }

                if (partyMembersAttacked > 1) {
                    if (!qtbUsedDuringEnemyTurn) {
                        AddQTB();
                        AddQTB();
                    } else {
                        AddQTB();
                        qtbUsedDuringEnemyTurn = false;
                    }
                } else if (partyMembersAttacked == 1) {
                    if (!qtbUsedDuringEnemyTurn)
                        AddQTB();
                    else
                        qtbUsedDuringEnemyTurn = false;
                }

                qtbOnBattleEntry = true;
            } else {
                if (Emulator.Memory.GameState != GameState.Battle && qtbOnBattleEntry) {
                    qtbOnBattleEntry = false;
                    qtbTurns = 0;
                    pgrQTB.Value = qtbTurns;
                    for (int i = 0; i < 3; i++) {
                        currentHP[i] = 0;
                    }
                }
            }
        }

        public void AddQTB() {
            this.Dispatcher.BeginInvoke(new Action(() => {
                qtbTurns += 1;
                if (qtbTurns > pgrQTB.Maximum)
                    qtbTurns = (byte) pgrQTB.Maximum;
                pgrQTB.Value = qtbTurns;
                Globals.SetCustomValue("QTB", qtbTurns);
            }), DispatcherPriority.ContextIdle);
        }

        public void SubQTB(int slot) {
            this.Dispatcher.BeginInvoke(new Action(() => {
                qtbTurns -= 1;
                pgrQTB.Value = qtbTurns;

                if (Emulator.ReadByte("DRAGOON_TURNS", 0x4 * slot) > 0)
                    Emulator.Battle.CharacterTable[slot].Action = 10;
                else
                    Emulator.Battle.CharacterTable[slot].Action = 8;

                bool playerTurn = false;
                for (int i = 0; i < 3; i++) {
                    if (Globals.PARTY_SLOT[i] < 9 && Emulator.Battle.CharacterTable[i].HP > 0) {
                        if (Emulator.Battle.CharacterTable[i].Action == 24 || Emulator.Battle.CharacterTable[i].Action == 26 | Emulator.Battle.CharacterTable[i].Action == 136) {
                            playerTurn = true;
                        }
                    }
                }

                qtbUsedDuringEnemyTurn = playerTurn ? false : true;
                Globals.SetCustomValue("QTB", qtbTurns);
                AddEnemyQTB();
            }), DispatcherPriority.ContextIdle);
        }

        public void AddEnemyQTB() {
            int enemiesAlive = 0;
            int turnPoints = 0;
            for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                if (Emulator.Battle.MonsterTable[i].HP > 0)
                    enemiesAlive++;
            }
            turnPoints = enemiesAlive == 1 ? 180 : enemiesAlive == 2 ? 90 : enemiesAlive == 3 ? 70 : enemiesAlive == 4 ? 60 : 50;

            if (Globals.ENCOUNTER_ID == 390 || Globals.ENCOUNTER_ID == 433 || Globals.ENCOUNTER_ID == 385 || Globals.ENCOUNTER_ID == 415 || Globals.ENCOUNTER_ID == 413 || Globals.ENCOUNTER_ID == 417 || Globals.ENCOUNTER_ID == 394 || Globals.ENCOUNTER_ID == 422 || Globals.ENCOUNTER_ID == 432 || Globals.ENCOUNTER_ID == 443)
                turnPoints = 180;

            for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                if (Emulator.Battle.MonsterTable[i].HP > 0) {
                    if (Globals.ENCOUNTER_ID == 390)
                        if (i == 0)
                            continue;
                    if (Globals.ENCOUNTER_ID == 433 && i >= 1)
                        if (Emulator.Battle.MonsterTable[i - 1].HP > 0)
                            continue;

                    Emulator.Battle.MonsterTable[i].Turn += (ushort) turnPoints;

                    if (Globals.ENCOUNTER_ID == 385 || Globals.ENCOUNTER_ID == 415 || Globals.ENCOUNTER_ID == 413 || Globals.ENCOUNTER_ID == 417 || Globals.ENCOUNTER_ID == 394 || Globals.ENCOUNTER_ID == 422 || Globals.ENCOUNTER_ID == 432 || Globals.ENCOUNTER_ID == 443)
                        break;
                }
            }
        }

        public void ATB() {
            if (Emulator.Memory.GameState == GameState.Battle && Controller.Main.StatsChanged && Globals.CheckDMScript("btnATB")) {
                if (!atbOnBattleEntry) {
                    for (int i = 0; i < 3; i++) {
                        if (Globals.PARTY_SLOT[i] < 9) {
                            currentHP[i] = Emulator.Battle.CharacterTable[i].HP;
                            playerSpeed[i] = Emulator.Battle.CharacterTable[i].SPD;
                            Emulator.Battle.CharacterTable[i].SPD = 0;
                            Emulator.Battle.CharacterTable[i].Turn = 0;
                        }
                    }
                }

                bool[] partyMemberAttacked = new bool[3];
                int partyMembersAttacked = 0;
                bool partyTakingAction = false;

                for (int i = 0; i < 3; i++) {
                    if (Globals.PARTY_SLOT[i] < 9) {
                        byte action = Emulator.Battle.CharacterTable[i].Action;
                        if (action == 8 || action == 10 || action == 24 || action == 26 || action == 136 || action == 138)
                            partyTakingAction = true;
                    }
                }

                for (int i = 0; i < 3; i++) {
                    if (Globals.PARTY_SLOT[i] < 9 && Emulator.Battle.CharacterTable[i].HP > 0) {
                        partyMemberAttacked[i] = false;
                        Emulator.Battle.CharacterTable[i].SPD = 0;
                        Emulator.Battle.CharacterTable[i].Turn = 0;
                        if (currentHP[i] < Emulator.Battle.CharacterTable[i].HP) {
                            partyMemberAttacked[i] = true;
                            partyMembersAttacked += 1;
                        }
                        currentHP[i] = Emulator.Battle.CharacterTable[i].HP;
                    }
                }

                this.Dispatcher.BeginInvoke(new Action(() => {
                    for (int i = 0; i < 3; i++) {
                        if (Globals.PARTY_SLOT[i] < 9 && partyMemberAttacked[i]) {
                            extraTurnBattleC[i] += partyMembersAttacked == 1 ? 4000 : partyMembersAttacked == 2 ? 3250 : 2500;
                            if (extraTurnBattleC[i] > 6000) {
                                extraTurnBattleC[i] = 6000;
                                //beep
                            }
                            progressCATB[i].Value = extraTurnBattleC[i];
                        }
                    }

                    if (partyTakingAction) {
                        for (int i = 0; i < 3; i++) {
                            if (Globals.PARTY_SLOT[i] < 9 && Emulator.Battle.CharacterTable[i].HP > 0) {
                                extraTurnBattleC[i] += playerSpeed[i] * 3;
                                if (extraTurnBattleC[i] > 6000) {
                                    extraTurnBattleC[i] = 6000;
                                    //beep
                                }
                                progressCATB[i].Value = extraTurnBattleC[i];
                            }
                        }
                    }
                }), DispatcherPriority.ContextIdle);

                Globals.SetCustomValue("EATBC1", extraTurnBattleC[0]);
                Globals.SetCustomValue("EATBC2", extraTurnBattleC[1]);
                Globals.SetCustomValue("EATBC3", extraTurnBattleC[2]);
                atbOnBattleEntry = true;
            } else {
                if (Emulator.Memory.GameState != GameState.Battle && atbOnBattleEntry) {
                    atbOnBattleEntry = false;
                }
            }
        }

        public void EatbBeep(int boop) {
            if (Constants.EATB_BEEP) {
                if (boop == 0 && !eatbSound[0]) {
                    eatbSound[0] = true;
                    System.Console.Beep(500, 1000);
                } else if (boop == 1 && !eatbSound[1]) {
                    eatbSound[1] = true;
                    System.Console.Beep(750, 1000);
                } else if (boop == 2 && !eatbSound[2]) {
                    eatbSound[2] = true;
                    System.Console.Beep(1000, 1000);
                }
            }
        }
        #endregion

        #region Extras
        public void WipeRewards() {
            for (int i = 0; i < 5; i++) {
                Emulator.WriteShort("MONSTER_REWARDS", 0, i * 0x1A8);
                Emulator.WriteShort("MONSTER_REWARDS", 0, 0x2 + i * 0x1A8);
                Emulator.WriteByte("MONSTER_REWARDS", 0, 0x4 + i * 0x1A8);
                Emulator.WriteByte("MONSTER_REWARDS", 0, 0x5 + i * 0x1A8);
            }
        }
        #endregion

        #endregion

        #endregion

        #region Settings
        private void ChangeScriptState(string type, SubScript script) {
            try {
                if (script.state != ScriptState.LOCKED) {
                    script.state = script.state == ScriptState.DISABLED ? ScriptState.ENABLED : ScriptState.DISABLED;
                    Constants.WriteOutput("Script '" + script.ToString() + "' is now " + (script.state == ScriptState.DISABLED ? "disabled." : "enabled."));
                    if (script.state == ScriptState.ENABLED) {
                        script.Open();
                    } else {
                        script.Close();
                    }
                } else {
                    Constants.WriteOutput("The script '" + script.ToString() + "' is locked and can't be disabled.");
                }
            } catch (Exception ex) {
                Constants.WriteOutput("Please select a " + type + " script.");
            }
        }

        private void miAttach_Click(object sender, RoutedEventArgs e) {
            if (!Constants.RUN) {
                //bool setup = Attach.Setup(Constants.EMULATOR_NAME);
                
                //Globals.MemoryController = new MemoryController.MemoryController();

                try {
                    Emulator.Attach(Constants.EMULATOR_NAME, Constants.OFFSET);

                    Constants.RUN = true;
                    globalThread = new Thread(delegate () { GlobalController.Run(); });
                    
                    newBattleThread = new Thread(Controller.Main.Run);

                    globalThread.Start();
                    newBattleThread.Start();
                    /*
                    fieldThread = new Thread(FieldController);
                    battleThread = new Thread(BattleController1);
                    hotkeyThread = new Thread(HotkeysController);
                    otherThread = new Thread(OtherController);
                    ultimateThread = new Thread(UltimateController);
                    newBattleThread.Start();
                    globalThread.Start();
                    fieldThread.Start();
                    battleThread.Start();
                    hotkeyThread.Start();
                    otherThread.Start();
                    */

                    if (Emulator.Memory.BattleValue < 9999) {
                        Controller.Main.StatsChanged = true;
                    }

                    miAttach.Header = "Detach";

                } catch (EmulatorAttachException ex) {
                    Constants.RUN = false;
                    miAttach.Header = "Attach";

                    Constants.WriteDebug(ex);
                    Constants.WritePLogOutput(ex.Message);
                } catch (EmulatorNotFoundException ex) {
                    Constants.RUN = false;
                    miAttach.Header = "Attach";

                    Constants.WriteDebug(ex);
                    Constants.WritePLogOutput(ex.Message);
                }

            } else {
                Constants.RUN = false;
                miAttach.Header = "Attach";
                Constants.WritePLogOutput("Detached from " + Constants.EMULATOR_NAME + ". Program stopped.");
            }
        }

        private void miEmulator_Click(object sender, RoutedEventArgs e) {
            if (Emulator.Memory.GameState != GameState.Battle) {
                foreach (MenuItem mi in miEmulator.Items) {
                    mi.IsChecked = (MenuItem) sender == mi ? true : false;
                }

                Constants.EMULATOR_ID = (byte) miEmulator.Items.IndexOf((MenuItem) sender);
                SetupEmulator(false);
            } else {
                ((MenuItem) sender).IsChecked = false;
                Constants.WriteOutput("You can only change emulators outside of battle.");
            }
        }

        public void SetupEmulator(bool onOpen) {
            string oldEmulator = Constants.EMULATOR_NAME;
            switch (Constants.EMULATOR_ID) {
                case 0:
                    Constants.EMULATOR_NAME = "RetroArch";
                    break;
                case 1:
                    Constants.EMULATOR_NAME = "DuckStation";
                    break;
                case 2:
                    Constants.EMULATOR_NAME = "ePSXe";
                    break;
                case 3:
                    Constants.EMULATOR_NAME = "PCSXR-PGXP";
                    break;
                case 4:
                    Constants.EMULATOR_NAME = "PCSX2";
                    break;
                default:
                    Constants.EMULATOR_NAME = "Other";
                    break;
            }

            if (!onOpen && (Constants.EMULATOR_NAME != oldEmulator || Constants.EMULATOR_NAME.Equals("Other"))) {
                MessageBox.Show("Dragoon Modifier needs to be shut down to switch emulators.");
                if (Constants.EMULATOR_NAME.Equals("Other")) {
                    OpenFileDialog ofg = new OpenFileDialog();
                    ofg.Title = "Select Emulator";
                    ofg.Filter = "Emulator|*.exe";
                    if (ofg.ShowDialog() == true) {
                        Constants.EMULATOR_NAME = System.IO.Path.GetFileNameWithoutExtension(ofg.FileName);
                    } else {
                        Constants.EMULATOR_ID = 12;
                    }
                }
                System.Windows.Application.Current.Shutdown();
            } else {
                if (!this.IsLoaded) {
                    Constants.RUN = false;
                    Thread.Sleep(2000);
                } 
                
                miAttach_Click(null, null);
                Constants.ProgramInfo();
            }
        }

        public void SetupScripts() {
            try {
                foreach (string file in Directory.GetFiles("Scripts\\Field", "*.cs", SearchOption.AllDirectories).OrderBy(f => f))
                    lstField.Items.Add(new SubScript(file));
                foreach (string file in Directory.GetFiles("Scripts\\Battle", "*.cs", SearchOption.AllDirectories).OrderBy(f => f))
                    lstBattle.Items.Add(new SubScript(file));
                foreach (string file in Directory.GetFiles("Scripts\\Hotkeys", "*.cs", SearchOption.AllDirectories).OrderBy(f => f))
                    lstHotkey.Items.Add(new SubScript(file));
                foreach (string file in Directory.GetFiles("Scripts\\Other", "*.cs", SearchOption.AllDirectories).OrderBy(f => f))
                    lstOther.Items.Add(new SubScript(file));
            } catch (Exception ex) {
                Constants.RUN = false;
                Constants.WriteGLog("Program stopped.");
                Constants.WritePLogOutput("Error loading scripts.");
                Constants.WriteOutput("Fatal Error. Closing all threads. Please see error log in Settings console.");
                Constants.WriteError(ex.ToString());
                EnableUI();
            }
        }

        private void miRegion_Click(object sender, RoutedEventArgs e) {
            if (Emulator.Memory.GameState != GameState.Battle) {
                foreach (MenuItem mi in miRegion.Items) {
                    mi.IsChecked = (MenuItem) sender == mi ? true : false;
                }
                Constants.REGION = (Region) miRegion.Items.IndexOf((MenuItem) sender);
                Constants.ProgramInfo();

                if (Constants.REGION == Region.JPN) {
                    Hotkey.KEY_CIRCLE = 32;
                    Hotkey.KEY_CROSS = 64;
                } else {
                    Hotkey.KEY_CIRCLE = 64;
                    Hotkey.KEY_CROSS = 32;
                }
            } else {
                Constants.WriteOutput("You can change regions outside of battle only.");
            }
        }

        private void miSaveSlot_Click(object sender, RoutedEventArgs e) {
            if (Emulator.Memory.GameState != GameState.Battle) {
                SaveSubKey();

                foreach (MenuItem mi in miSaveSlot.Items) {
                    mi.IsChecked = (MenuItem) sender == mi ? true : false;
                }

                Constants.SetSubKey(miSaveSlot.Items.IndexOf((MenuItem) sender));

                foreach (SubScript s in lstField.Items) {
                    if (s.state != ScriptState.DISABLED)
                        s.Open();
                }

                foreach (SubScript s in lstBattle.Items) {
                    if (s.state != ScriptState.DISABLED)
                        s.Open();
                }

                foreach (SubScript s in lstHotkey.Items) {
                    if (s.state != ScriptState.DISABLED)
                        s.Open();
                }

                foreach (SubScript s in lstOther.Items) {
                    if (s.state != ScriptState.DISABLED)
                        s.Open();
                }

                LoadSubKey();

                Constants.ProgramInfo();
            } else {
                Constants.WriteOutput("You can change save slots outside of battle only.");
            }
        }

        private void miLog_Click(object sender, RoutedEventArgs e) {
            rdLog.Height = miLog.Header.Equals("Expand Log") ? new GridLength(9999, GridUnitType.Star) : new GridLength(2, GridUnitType.Star);
            miLog.Header = miLog.Header.Equals("Expand Log") ? "Collapse Log" : "Expand Log";
        }

        private void btnField_Click(object sender, RoutedEventArgs e) {
            ChangeScriptState("Field", (SubScript) lstField.SelectedItem);
            ScriptDisplay(lstField);
        }

        private void btnBattle_Click(object sender, RoutedEventArgs e) {
            ChangeScriptState("Battle", (SubScript) lstBattle.SelectedItem);
            ScriptDisplay(lstBattle);
        }

        private void btnHotkeys_Click(object sender, RoutedEventArgs e) {
            ChangeScriptState("Hotkeys", (SubScript) lstHotkey.SelectedItem);
            ScriptDisplay(lstHotkey);
        }

        private void btnOther_Click(object sender, RoutedEventArgs e) {
            ChangeScriptState("Other", (SubScript) lstOther.SelectedItem);
            ScriptDisplay(lstOther);
        }

        private void lstField_ScrollChanged(object sender, ScrollChangedEventArgs e) {
            ScriptDisplay(lstField);
        }

        private void lstBattle_ScrollChanged(object sender, ScrollChangedEventArgs e) {
            ScriptDisplay(lstBattle);
        }

        private void lstHotkey_ScrollChanged(object sender, ScrollChangedEventArgs e) {
            ScriptDisplay(lstHotkey);
        }

        private void lstOther_ScrollChanged(object sender, ScrollChangedEventArgs e) {
            ScriptDisplay(lstOther);
        }

        private void lstField_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            OpenScript(lstField);
        }

        private void lstBattle_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            OpenScript(lstBattle);
        }

        private void lstHotkey_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            OpenScript(lstHotkey);
        }

        private void lstOther_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            OpenScript(lstOther);
        }

        private void miNew_Click(object sender, RoutedEventArgs e) {
            DisableScripts();
            Constants.WriteOutput("All scripts have been disabled. Enable the scripts you want to save for the preset then click the Save option.");
        }

        private void miOpen_Click(object sender, RoutedEventArgs e) {
            InputWindow openPresetWindow = new InputWindow("Open Preset");
            TextBox txt = new TextBox();
            openPresetWindow.AddObject(txt);
            openPresetWindow.AddTextBlock("What preset do you want to open?");
            openPresetWindow.ShowDialog();

            if (!txt.Text.Equals("")) {
                if (Constants.LoadPreset(txt.Text)) {
                    ChangeTitle(txt.Text);
                    DisableScripts();
                    LoadPreset();
                }
            } else {
                Constants.WriteOutput("Nothing was input.");
            }
        }

        private void miSave_Click(object sender, RoutedEventArgs e) {
            InputWindow openPresetWindow = new InputWindow("Save Preset");
            TextBox txt = new TextBox();
            openPresetWindow.AddObject(txt);
            openPresetWindow.AddTextBlock("What do you want to call your preset?");
            openPresetWindow.ShowDialog();

            if (!txt.Text.Equals("")) {
                ArrayList text = new ArrayList();
                int index = 0;
                foreach (SubScript s in lstField.Items) {
                    index++;
                    if ((s.state == ScriptState.ENABLED || s.state == ScriptState.LOCKED) && index > 1) {
                        text.Add(s.GetPath().Substring(System.IO.Directory.GetCurrentDirectory().Length + 1));
                    }
                }
                index = 0;
                foreach (SubScript s in lstBattle.Items) {
                    index++;
                    if ((s.state == ScriptState.ENABLED || s.state == ScriptState.LOCKED) && index > 1) {
                        text.Add(s.GetPath().Substring(System.IO.Directory.GetCurrentDirectory().Length + 1));
                    }
                }
                index = 0;
                foreach (SubScript s in lstHotkey.Items) {
                    index++;
                    if ((s.state == ScriptState.ENABLED || s.state == ScriptState.LOCKED) && index > 1) {
                        text.Add(s.GetPath().Substring(System.IO.Directory.GetCurrentDirectory().Length + 1));
                    }
                }
                foreach (SubScript s in lstOther.Items) {
                    if (s.state == ScriptState.ENABLED || s.state == ScriptState.LOCKED) {
                        text.Add(s.GetPath().Substring(System.IO.Directory.GetCurrentDirectory().Length + 1));
                    }
                }
                if (text.Count >= 1) {
                    try {
                        using (StreamWriter presetFile = new StreamWriter("Presets\\" + txt.Text + ".csv")) {
                            foreach (string line in text)
                                presetFile.WriteLine(line.Substring(0, line.Length - 3) + ",1");

                            short config = 0;
                            if (Globals.MONSTER_STAT_CHANGE)
                                config |= 1 << 0;
                            if (Globals.MONSTER_DROP_CHANGE)
                                config |= 1 << 1;
                            if (Globals.MONSTER_EXPGOLD_CHANGE)
                                config |= 1 << 2;
                            if (Globals.CHARACTER_STAT_CHANGE)
                                config |= 1 << 3;
                            if (Globals.ADDITION_CHANGE)
                                config |= 1 << 4;
                            if (Globals.DRAGOON_STAT_CHANGE)
                                config |= 1 << 5;
                            if (Globals.DRAGOON_SPELL_CHANGE)
                                config |= 1 << 6;
                            if (Globals.DRAGOON_ADDITION_CHANGE)
                                config |= 1 << 7;
                            if (Globals.DRAGOON_DESC_CHANGE)
                                config |= 1 << 8;
                            if (Globals.ITEM_STAT_CHANGE)
                                config |= 1 << 9;
                            if (Globals.ITEM_ICON_CHANGE)
                                config |= 1 << 10;
                            if (Globals.ITEM_NAMEDESC_CHANGE)
                                config |= 1 << 11;
                            if (Globals.SHOP_CHANGE)
                                config |= 1 << 12;
                            presetFile.WriteLine("Config," + config);
                            presetFile.WriteLine(Globals.MOD + ",0");
                        }

                        Constants.WriteOutput("Saved preset '" + txt.Text + "'.");
                    } catch (Exception ex) {
                        Constants.WriteOutput("Error writing file.");
                    }
                } else {
                    Constants.WriteOutput("Nothing was enabled.");
                }
            } else {
                Constants.WriteOutput("Nothing was input.");
            }
        }

        private void miOpenPreset_Click(object sender, RoutedEventArgs e) {
            miOpenPreset.IsChecked = miOpenPreset.IsChecked ? false : true;
        }

        private void miPresetHotkeys_Click(object sender, RoutedEventArgs e) {
            miPresetHotkeys.IsChecked = miPresetHotkeys.IsChecked ? false : true;
            presetHotkeys = miPresetHotkeys.IsChecked;
        }

        private void miDeleteSave_Click(object sender, RoutedEventArgs e) {
            if (System.Windows.Forms.MessageBox.Show("Are you sure you want to delete the current save?\r\n\r\nFaust: " + faustCount + "\r\nUltimate Bosses Defeated: " + ultimateBossCompleted + "\r\nExtended Inventory: " + inventorySize, "Delete Save", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question, System.Windows.Forms.MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Yes) {
                stopSave = true;
                MessageBox.Show("Dragoon Modifier needs to be shut down to delete a save.");
                Constants.KEY.DeleteSubKey(Constants.SAVE_SLOT.ToString());
                System.Windows.Application.Current.Shutdown();
            }
        }

        private void miEatbSound_Click(object sender, RoutedEventArgs e) {
            miEatbSound.IsChecked = miEatbSound.IsChecked ? false : true;
            Constants.EATB_BEEP = miEatbSound.IsChecked;
        }

        private void miManualOffset_Click(object sender, RoutedEventArgs e) {
            InputWindow offsetEntry = new InputWindow("Manual Offset Entry");
            TextBox input = new TextBox();
            long newOffset = 0;
            offsetEntry.AddTextBlockF("Search for the below AOB and subtract 0xB070 from the result and enter it below in integer format. Once done you can try attaching the Emulator.\r\n50 53 2D 58 20 45 58 45");
            offsetEntry.AddObjectF(input);
            offsetEntry.ShowDialog();

            if (Int64.TryParse(input.Text, out newOffset)) {
                Constants.KEY.SetValue("Offset", newOffset);
                Constants.OFFSET = newOffset;
            } else {
                Constants.WriteOutput("Invalid input");
            }
        }


        private void miModOptions_Click(object sender, RoutedEventArgs e) {
            if (!Globals.DIFFICULTY_MODE.Equals("Normal")) {
                Constants.WriteOutput("You can't change Mod Options while using a preset.");
            } else {
                InputWindow openModWindow = new InputWindow("Mod Options");
                ComboBox mod = new ComboBox();
                CheckBox monsterStat = new CheckBox();
                CheckBox monsterDrop = new CheckBox();
                CheckBox monsterExpGold = new CheckBox();
                CheckBox characterStat = new CheckBox();
                CheckBox addition = new CheckBox();
                CheckBox dragoonStats = new CheckBox();
                CheckBox dragoonSpell = new CheckBox();
                CheckBox dragoonAddition = new CheckBox();
                CheckBox dragoonDescription = new CheckBox();
                CheckBox itemStat = new CheckBox();
                CheckBox itemIcon = new CheckBox();
                CheckBox itemNameDescription = new CheckBox();
                CheckBox shop = new CheckBox();

                monsterStat.Content = "Monster Stats";
                if (Globals.MONSTER_STAT_CHANGE)
                    monsterStat.IsChecked = true;

                monsterDrop.Content = "Drop";
                if (Globals.MONSTER_DROP_CHANGE)
                    monsterDrop.IsChecked = true;

                monsterExpGold.Content = "Exp + Gold";
                if (Globals.MONSTER_EXPGOLD_CHANGE)
                    monsterExpGold.IsChecked = true;

                characterStat.Content = "Character Stats";
                if (Globals.CHARACTER_STAT_CHANGE)
                    characterStat.IsChecked = true;

                addition.Content = "Addition";
                if (Globals.ADDITION_CHANGE)
                    addition.IsChecked = true;

                dragoonStats.Content = "Dragoon Stats";
                if (Globals.DRAGOON_STAT_CHANGE)
                    dragoonStats.IsChecked = true;

                dragoonSpell.Content = "Dragoon Spells";
                if (Globals.DRAGOON_SPELL_CHANGE)
                    dragoonSpell.IsChecked = true;

                dragoonAddition.Content = "Dragoon Additions";
                if (Globals.DRAGOON_ADDITION_CHANGE)
                    dragoonAddition.IsChecked = true;

                dragoonDescription.Content = "Dragoon Descriptions";
                if (Globals.DRAGOON_DESC_CHANGE)
                    dragoonDescription.IsChecked = true;

                itemStat.Content = "Item Stats";
                if (Globals.ITEM_STAT_CHANGE)
                    itemStat.IsChecked = true;

                itemIcon.Content = "Item Icons";
                if (Globals.ITEM_ICON_CHANGE)
                    itemIcon.IsChecked = true;

                itemNameDescription.Content = "Item Names + Descriptions";
                if (Globals.ITEM_NAMEDESC_CHANGE)
                    itemNameDescription.IsChecked = true;

                shop.Content = "Shop";
                if (Globals.SHOP_CHANGE)
                    shop.IsChecked = true;

                string[] dirs = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + "Mods\\");
                foreach (string dir in dirs) {
                    mod.Items.Add(new DirectoryInfo(dir).Name);
                }

                mod.SelectedValue = Globals.MOD;

                openModWindow.AddObject(mod);
                openModWindow.AddTextBlock("Database");
                openModWindow.AddObject(shop);
                openModWindow.AddObject(itemNameDescription);
                openModWindow.AddObject(itemIcon);
                openModWindow.AddObject(itemStat);
                openModWindow.AddObject(dragoonDescription);
                openModWindow.AddObject(dragoonAddition);
                openModWindow.AddObject(dragoonSpell);
                openModWindow.AddObject(dragoonStats);
                openModWindow.AddObject(addition);
                openModWindow.AddObject(characterStat);
                openModWindow.AddObject(monsterExpGold);
                openModWindow.AddObject(monsterDrop);
                openModWindow.AddObject(monsterStat);
                openModWindow.AddTextBlock("Please select the mods you want to turn on or off.");
                openModWindow.ShowDialog();

                Globals.MONSTER_STAT_CHANGE = (bool) monsterStat.IsChecked;
                Globals.MONSTER_DROP_CHANGE = (bool) monsterDrop.IsChecked;
                Globals.MONSTER_EXPGOLD_CHANGE = (bool) monsterExpGold.IsChecked;
                Globals.CHARACTER_STAT_CHANGE = (bool) characterStat.IsChecked;
                Globals.ADDITION_CHANGE = (bool) addition.IsChecked;
                Globals.DRAGOON_STAT_CHANGE = (bool) dragoonStats.IsChecked;
                Globals.DRAGOON_SPELL_CHANGE = (bool) dragoonSpell.IsChecked;
                Globals.DRAGOON_ADDITION_CHANGE = (bool) dragoonAddition.IsChecked;
                Globals.DRAGOON_DESC_CHANGE = (bool) dragoonDescription.IsChecked;
                Globals.ITEM_STAT_CHANGE = (bool) itemStat.IsChecked;
                Globals.ITEM_ICON_CHANGE = (bool) itemIcon.IsChecked;
                Globals.ITEM_NAMEDESC_CHANGE = (bool) itemNameDescription.IsChecked;
                Globals.SHOP_CHANGE = (bool) shop.IsChecked;

                if (Globals.MOD != (string) mod.SelectedValue) {
                    Globals.MOD = (string) mod.SelectedValue;
                    LoDDictionary.Dictionary.Init(Globals.MOD);
                    Constants.WriteOutput("Changing Mod");
                }

                if (Emulator.ReadShort("BATTLE_VALUE") < 9999)
                    Controller.Main.StatsChanged = true;

                Constants.WritePLogOutput("Mod directory: " + Globals.MOD);
            }
        }

        private void miAuthor_Click(object sender, RoutedEventArgs e) {
            Constants.WriteOutput("-------------");
            Constants.WriteOutput("Author: Zychronix");
            Constants.WriteOutput("https://legendofdragoonhardmode.wordpress.com/");
            Constants.WriteOutput("Author: Illeprih");
        }

        private void miCredits_Click(object sender, RoutedEventArgs e) {
            Constants.WriteOutput("-------------");
            Constants.WriteOutput("Program Base: Zychronix & Illeprih");
            Constants.WriteOutput("Memory Functions: erfg12 - memory.dll - https://github.com/erfg12/memory.dll");
            Constants.WriteOutput("Scripting Engine: CS-Script - https://github.com/oleg-shilo/cs-script/graphs/contributors");
        }

        private void miCredits1_Click(object sender, RoutedEventArgs e) {
            System.Diagnostics.Process.Start("https://legendofdragoonhardmode.wordpress.com/");
        }

        private void miCredits2_Click(object sender, RoutedEventArgs e) {
        }

        private void miCredits3_Click(object sender, RoutedEventArgs e) {
            System.Diagnostics.Process.Start("https://github.com/erfg12/memory.dll/wiki");
        }

        private void miCredits4_Click(object sender, RoutedEventArgs e) {
            System.Diagnostics.Process.Start("https://www.cs-script.net/");
        }

        private void miVersion_Click(object sender, RoutedEventArgs e) {
            Constants.WriteOutput("-------------");
            Constants.WriteOutput("Version " + Constants.VERSION);
        }
        #endregion

        #region UI
        public void SwitchButton(object sender, EventArgs e) {
            Button btn = (Button) sender;
            if (!Globals.dmScripts.ContainsKey(btn.Name)) {
                Globals.dmScripts.Add(btn.Name, true);
            } else {
                Globals.dmScripts[btn.Name] = Globals.dmScripts[btn.Name] ? false : true;
            }

            if (btn.Name.Equals("btnAddPartyMembersOn"))
                alwaysAddSoloPartyMembers = Globals.dmScripts[btn.Name] ? true : false;

            if (btn.Name.Equals("btnKillBGM"))
                SetKillBGMState();

            if (btn.Name.Equals("btnUltimateBoss") && Emulator.Memory.GameState != GameState.Battle)
                UltimateBossFieldSet();

            if (btn.Name.Equals("btnEarlyAdditions") && !Globals.dmScripts[btn.Name])
                TurnOffEarlyAdditions();

            if (btn.Name.Equals("btnReader")) {
                if (!readerWindow.IsLoaded) {
                    readerWindow = new ReaderWindow();
                    LoadReaderKey();
                    readerWindow.Show();
                } else {
                    if (readerWindow.IsOpen) {
                        readerWindow.Focus();
                        return;
                    }
                }
            }

            if (btn.Name.Equals("btnSoloMode")) {
                Globals.dmScripts["btnDuoMode"] = false;
                TurnOnOffButton(ref btnDuoMode);
            }

            if (btn.Name.Equals("btnDuoMode")) {
                Globals.dmScripts["btnSoloMode"] = false;
                TurnOnOffButton(ref btnSoloMode);
            }

            if (btn.Name.Equals("btnEATB")) {
                Globals.dmScripts["btnQTB"] = false;
                Globals.dmScripts["btnATB"] = false;
                TurnOnOffButton(ref btnQTB);
                TurnOnOffButton(ref btnATB);
            }

            if (btn.Name.Equals("btnQTB")) {
                Globals.dmScripts["btnATB"] = false;
                Globals.dmScripts["btnEATB"] = false;
                TurnOnOffButton(ref btnATB);
                TurnOnOffButton(ref btnEATB);
            }

            if (btn.Name.Equals("btnATB")) {
                Globals.dmScripts["btnQTB"] = false;
                Globals.dmScripts["btnEATB"] = false;
                TurnOnOffButton(ref btnQTB);
                TurnOnOffButton(ref btnEATB);
            }

            if (btn.Name.Equals("btnTextSpeed") && !Globals.dmScripts[btn.Name]) {
                Emulator.WriteShort("TEXT_SPEED", 223);
            }

            if (btn.Name.Equals("btnAutoText") && !Globals.dmScripts[btn.Name]) {
                Emulator.WriteShort("AUTO_TEXT", 12354);
            }

            if (btn.Name.Equals("btnDivineRed") && (Globals.DIFFICULTY_MODE.Contains("Hell") && ultimateBossCompleted < 34)) {
                Globals.dmScripts[btn.Name] = false;
                Constants.WriteGLogOutput("You have not completed Ultimate Boss Dragoon Doel.");
            }

            if (!btn.Name.Equals("btnNoDart")) {
                TurnOnOffButton(ref btn);
            } else {
                Globals.NO_DART = null;
                Emulator.WriteByte("PARTY_SLOT", 0);
                btn.Background = new SolidColorBrush(Color.FromArgb(255, 255, 168, 168));
                Constants.WritePLogOutput("No Dart turned off.");
            }
            if (btn.Name.Equals("btnAutoTransform")) {
                Globals.AUTO_TRANSFORM = !Globals.AUTO_TRANSFORM;
            }
        }

        public void TurnOnOffButton(ref Button sender) {
            if (!Globals.dmScripts[sender.Name]) {
                sender.Background = new SolidColorBrush(Color.FromArgb(255, 255, 168, 168));
            } else {
                sender.Background = new SolidColorBrush(Color.FromArgb(255, 168, 211, 255));
            }
        }

        public void GreenButton(object sender, EventArgs e) {
            Button btn = (Button) sender;
            if (btn.Name.Equals("btnAddPartyMembers")) {
                AddSoloPartyMembers();
            } else if (btn.Name.Equals("btnSwitchSoloChar")) {
                SwitchSoloCharacter();
            } else if (btn.Name.Equals("btnElementArrow")) {
                ChangeElementArrow();
            } else if (btn.Name.Equals("btnReaderConfig")) {
                ReaderModeConfig();
            } else if (btn.Name.Equals("btnReaderAdd")) {
                ReaderModeAdd();
            } else if (btn.Name.Equals("btnReaderChange")) {
                ReaderModeChange();
            } else if (btn.Name.Equals("btnReaderDelete")) {
                ReaderModeDelete();
            } else if (btn.Name.Equals("btnReaderSave")) {
                ReaderModeSave();
            } else if (btn.Name.Equals("btnReaderLoad")) {
                ReaderModeLoad();
            } else if (btn.Name.Equals("btnReaderReset")) {
                ReaderModeReset();
            } else if (btn.Name.Equals("btnSwitchEXP")) {
                SwitchEXP();
            } else if (btn.Name.Equals("btnHeroTicketShop")) {
                HeroTicketShop();
            } else if (btn.Name.Equals("btnHeroItemShop")) {
                HeroItemShop();
            } else if (btn.Name.Equals("btnUltimateShop")) {
                UltimateItemShop();
            }
        }

        public void ComboBox(object sender, EventArgs e) {
            ComboBox cbo = (ComboBox) sender;
            if (!uiCombo.ContainsKey(cbo.Name)) {
                uiCombo.Add(cbo.Name, cbo.SelectedIndex);
            } else {
                uiCombo[cbo.Name] = cbo.SelectedIndex;
            }

            if (cbo.Name.Equals("cboKillBGM")) {
                SetKillBGMState();
            }

            if (cbo.Name.Equals("cboUltimateBoss")) {
                UltimateBossFieldSet();
            }

            if (cbo.Name.Equals("cboHelpTopic")) {
                HelpTopic();
            }
        }

        public void DifficultyButton(object sender, EventArgs e) {
            Button btn = (Button) sender;
            equipChangesOnFieldEntry = false;

            if (btn == btnNormal) {
                btn.Background = new SolidColorBrush(Color.FromArgb(255, 168, 211, 255));
                btnNormalHard.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                btnHard.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                btnHardHell.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                btnHell.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                Globals.DIFFICULTY_MODE = "Normal";
                Globals.MOD = "US_Base";
                Globals.MONSTER_STAT_CHANGE = false;
                Globals.MONSTER_DROP_CHANGE = false;
                Globals.MONSTER_EXPGOLD_CHANGE = false;
                Globals.CHARACTER_STAT_CHANGE = false;
                Globals.ADDITION_CHANGE = false;
                Globals.DRAGOON_STAT_CHANGE = false;
                Globals.DRAGOON_SPELL_CHANGE = false;
                Globals.DRAGOON_DESC_CHANGE = false;
                Globals.DRAGOON_ADDITION_CHANGE = false;
                Globals.ITEM_STAT_CHANGE = false;
                Globals.ITEM_ICON_CHANGE = false;
                Globals.ITEM_NAMEDESC_CHANGE = false;
                Globals.SHOP_CHANGE = false;
            } else if (btn == btnNormalHard) {
                btn.Background = new SolidColorBrush(Color.FromArgb(255, 168, 211, 255));
                btnNormal.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));;
                btnHard.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                btnHardHell.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                btnHell.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                Globals.DIFFICULTY_MODE = "NormalHard";
                Globals.MOD = "Hard_Mode";
                Globals.MONSTER_STAT_CHANGE = true;
                Globals.MONSTER_DROP_CHANGE = true;
                Globals.MONSTER_EXPGOLD_CHANGE = true;
                Globals.CHARACTER_STAT_CHANGE = false;
                Globals.ADDITION_CHANGE = true;
                Globals.DRAGOON_STAT_CHANGE = false;
                Globals.DRAGOON_SPELL_CHANGE = false;
                Globals.DRAGOON_DESC_CHANGE = true;
                Globals.DRAGOON_ADDITION_CHANGE = false;
                Globals.ITEM_STAT_CHANGE = true;
                Globals.ITEM_ICON_CHANGE = true;
                Globals.ITEM_NAMEDESC_CHANGE = true;
                Globals.SHOP_CHANGE = true;
            } else if (btn == btnHard) {
                btn.Background = new SolidColorBrush(Color.FromArgb(255, 168, 211, 255));
                btnNormal.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                btnNormalHard.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                btnHardHell.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                btnHell.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                Globals.DIFFICULTY_MODE = "Hard";
                Globals.MOD = "Hard_Mode";
                Globals.MONSTER_STAT_CHANGE = true;
                Globals.MONSTER_DROP_CHANGE = true;
                Globals.MONSTER_EXPGOLD_CHANGE = true;
                Globals.CHARACTER_STAT_CHANGE = false;
                Globals.ADDITION_CHANGE = true;
                Globals.DRAGOON_STAT_CHANGE = false;
                Globals.DRAGOON_SPELL_CHANGE = false;
                Globals.DRAGOON_DESC_CHANGE = true;
                Globals.DRAGOON_ADDITION_CHANGE = false;
                Globals.ITEM_STAT_CHANGE = true;
                Globals.ITEM_ICON_CHANGE = true;
                Globals.ITEM_NAMEDESC_CHANGE = true;
                Globals.SHOP_CHANGE = true;
            } else if (btn == btnHardHell) {
                btn.Background = new SolidColorBrush(Color.FromArgb(255, 168, 211, 255));
                btnNormal.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                btnNormalHard.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                btnHard.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                btnHell.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                Globals.DIFFICULTY_MODE = "HardHell";
                Globals.MOD = "Hell_Mode";
                Globals.MONSTER_STAT_CHANGE = true;
                Globals.MONSTER_DROP_CHANGE = true;
                Globals.MONSTER_EXPGOLD_CHANGE = true;
                Globals.CHARACTER_STAT_CHANGE = false;
                Globals.ADDITION_CHANGE = true;
                Globals.DRAGOON_STAT_CHANGE = false;
                Globals.DRAGOON_SPELL_CHANGE = false;
                Globals.DRAGOON_DESC_CHANGE = true;
                Globals.DRAGOON_ADDITION_CHANGE = false;
                Globals.ITEM_STAT_CHANGE = true;
                Globals.ITEM_ICON_CHANGE = true;
                Globals.ITEM_NAMEDESC_CHANGE = true;
                Globals.SHOP_CHANGE = true;
            } else if (btn == btnHell) {
                btn.Background = new SolidColorBrush(Color.FromArgb(255, 168, 211, 255));
                btnNormal.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                btnNormalHard.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                btnHard.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                btnHardHell.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                Globals.DIFFICULTY_MODE = "Hell";
                Globals.MOD = "Hell_Mode";
                Globals.MONSTER_STAT_CHANGE = true;
                Globals.MONSTER_DROP_CHANGE = true;
                Globals.MONSTER_EXPGOLD_CHANGE = true;
                Globals.CHARACTER_STAT_CHANGE = false;
                Globals.ADDITION_CHANGE = true;
                Globals.DRAGOON_STAT_CHANGE = true;
                Globals.DRAGOON_SPELL_CHANGE = false;
                Globals.DRAGOON_DESC_CHANGE = true;
                Globals.DRAGOON_ADDITION_CHANGE = false;
                Globals.ITEM_STAT_CHANGE = true;
                Globals.ITEM_ICON_CHANGE = true;
                Globals.ITEM_NAMEDESC_CHANGE = true;
                Globals.SHOP_CHANGE = true;
            } else {
                enrageBoss = enrageBoss ? false : true;
                btn.Background = enrageBoss ? (new SolidColorBrush(Color.FromArgb(255, 168, 211, 255))) : (new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)));
            }

            LoDDictionary.Dictionary.Init(Globals.MOD);
            SHOP_CHANGED = false;
            if (Emulator.ReadShort("BATTLE_VALUE") < 9999) {
                Controller.Main.StatsChanged = true;
            }
            Constants.WriteOutput("LOD Dictionary updated.");
            Constants.WritePLogOutput("Mod directory switched to: " + Globals.MOD);
        }

        private void Slider_ValueChanged(object sender,
            RoutedPropertyChangedEventArgs<double> e) {
            var slider = sender as Slider;
            if (sender == sldHP) {
                Globals.HP_MULTI = slider.Value;
            } else if (sender == sldATK) {
                Globals.AT_MULTI = slider.Value;
            } else if (sender == sldDEF) {
                Globals.DF_MULTI = slider.Value;
            } else if (sender == sldMAT) {
                Globals.MAT_MULTI = slider.Value;
            } else if (sender == sldMDF) {
                Globals.MDF_MULTI = slider.Value;
            } else if (sender == sldSPD) {
                Globals.SPD_MULTI = slider.Value;
            } else if (sender == sldZoom) {
                /* TODO initializes too late
                if (Emulator.Memory.GameState == GameState.Battle)
                    Emulator.WriteUShort("ZOOM", (ushort) slider.Value);
                */
            }
        }

        public void EnableUI() {
            this.Dispatcher.BeginInvoke(new Action(() => {
                tabDifficulty.IsEnabled = true;
                tabEnhancements.IsEnabled = true;
                tabEnhancements2.IsEnabled = true;
                tabShop.IsEnabled = true;
                tabSettings.IsEnabled = true;
            }), DispatcherPriority.ContextIdle);
        }

        public void HelpTopic() {
            if (cboHelpTopic.SelectedIndex == 0) {
                txtHelp.Text = "Click on the difficulty tab to change your difficulty or use a preset.\r\n" +
                    "Click on enhancements tabs to turn on features.\r\n" +
                    "Click on Settings Tab>Settings>Emulator to change your emulator.\r\n" +
                    "Click on Settings Tab>Settings>Region to change your game region.\r\n" +
                    "If everything is setup correctly with your emulator the Encounter Value will display 41215 on the Battle Stats tab. When on the field your Encounter Value will increase when you walk. On the load screen the map id should display 675.\r\n" +
                    "Please note on emulators ePSXe 2.0 and above, and any other emulator you should follow these addition steps. Open emulator, open game, before you load your save, open Dragoon Modifier. You do not have to do this for ePSXe 1.9.25 and below.\r\n" +
                    "Please also note to NOT pause the game or speed up the emulator during battle transition to avoid crashing. Let Dragoon Modifier finish the battle setup, you can tell when it is complete when the icon turns green.\r\n" +
                    "Normal Mode sets the program to a read only state with everything turned off. If your inventory is expanded it will extend it however. To avoid this change to an empty save slot.\r\n" +
                    "Dragoon Modifier timers are built on the game running at full speed. Please make sure your game is running at full speed, not faster or slower to prevent issues.";
            } else if (cboHelpTopic.SelectedIndex == 1) {
                txtHelp.Text = "Encounter Value is how close you are to the next battle. Enemy ID will display the value that determines what monsters show up in battle. Map ID will display where you are in the game.\r\n" +
                    "The top block will display monster stats, the bottom block will display stats.\r\n" +
                    "Blue: Name\r\nRed: HP\r\nOrange: AT/MAT\r\nGreen: DF/MDF\r\nMagenta: SPD\r\nViolet: Turn Points, Characters have SP\r\nGrey: A-AV/M-AV\r\nTeal: D-AT/D-MAT\r\nBrown: D-DF/D-MDF\r\n" +
                    "Turn Order will display the current Turn Point order of who will go next. Counterattacks are not counted in this calculation.";
            } else if (cboHelpTopic.SelectedIndex == 2) {
                txtHelp.Text = "Presets Hard and Hell mode are plug and play difficulty settings which are locked in. These presets are intended to run on unmodified ISOs. Normal Mode will not do anything unless you have made changes. You can change your current mod loadout and location in Settings Tab>Settings>Mod Options. Hard and Hell Mode have a more detailed changes below.\r\n\r\n" +
                    "Hard Mode\r\nThis preset balances characters, monsters, weapons, additions, boss drops, and is very Dragoon focused. It is intended that you start off with Dragoons using hotkey (CROSS+L1) in starting Map 10. The mod starts off slightly harder than the Japanese version and gets more difficult as you progress through the discs. However you are not meant to grind for EXP in this mode. You can keep everyone at the same level by using Switch EXP in the Enhancements 1 tab up to 80,000 EXP. Dart as well has new Dragoon enhancements called Burn Stacks. Dart can have up to 6 stacks for 20% each and he gains stacks by using Dragoon magic. Select the second red attack icon or the second magic icon to activate a burn stack attack.\r\n\r\n" +
                    "Hell Mode has the same character, weapon, drop, and dragoon adjustments. However incomplete Additions are punished and you gain about 50% SP from them. To encourage the use of Elemental Bomb the drop rates for magic items are tripled, powerful items are doubled. It is intended for you to start off Hell Mode will all Dragoons with hotkey (CROSS+L1) in Map 10. It is also intended that you use Elemental Bomb, it was left optional as it made Hell Mode easier but it was designed with this turned on. This changes the element of all monsters on the field when a powerful item is used, however you do not have to use this. Monsters are much harder and you may require grinding. You can keep everyone at the same level by using Switch EXP up to 160,000 EXP. In Hell Mode you lose SP after fighting most of the major bosses, if you total SP gained falls below the level threshold you will delevel your Dragoons.\r\n\r\n" +
                    "Base + Hard (Bosses) uses Hard Mode enhancements but uses normal monster stats for normal encounters (which makes them easier) and uses Hard Mode stats for bosses only.\r\n\r\n" +
                    "Hard + Hell (Bosses) uses Hell Mode enhancements, but uses Hard Mode monster stats for normal encounters and Hell Mode stats for bosses only.\r\n\r\n" +
                    "+\r\nPlus turns on Enrage Mode for bosses only. Originally designed as a part of Hell Mode but separated for the possibility of being too hard.\r\n\r\n" +
                    "The sliders will multiply each stat at the bottom. If you choose a preset those stats will be multiplied as well. You must have Monster Changes on from Settings>Settings>Mod Options>Monster Changes to use the sliders.";
            } else if (cboHelpTopic.SelectedIndex == 3) {
                txtHelp.Text = "Save >9999 HP\r\nWhen you have characters above 9999 HP this will save their HP between battles.\r\n\r\n" +
                    "Remove Damage Caps\r\nThe game's multiple damage caps will be changed to 50,000.\r\n\r\n" +
                    "Elemental Bomb\r\nPowerful items will change the element of all monsters for 5 turns on the field even through a miss.\r\n\r\n" +
                    "Enrage Mode\r\nMonsters will increase stats by 10% when they hit Yellow HP Zone, 25% when they hit Red HP Zone.\r\n\r\n" +
                    "Never Guard\r\nYou can still heal 10% HP when using guard, however you will not guard against attacks.\r\n\r\n" +
                    "Save Anywhere\r\nYou'll be able to save anywhere. The camera may be stuck when reloading on some maps.\r\n\r\n" +
                    "Auto Charm Potion\r\nThis will auto use a charm potion when you hit the red marker, it will automatically remove 8 gold.\r\n\r\n" +
                    "Monsters HPs as Names\r\nIn battle monster names will change to its current HP.\r\n\r\n" +
                    "No HP Decay Soul Eater\r\nTurns off 10% HP damage for Dart when equipped with Soul Eater.\r\n\r\n" +
                    "Extra Turn Battle\r\nBars will appear in the Battle Stats Tab. When the bar is maxed you can use the following hotkeys to gain an extra turn. Monsters will automatically gain a turn. When you gain an extra turn your progress is slowed down for some time.\r\nSlot 1 (SQUARE + UP)\r\nSlot 2 (SQUARE + RIGHT)\r\nSlot 3 (SQUARE + LEFT)\r\n\r\n" +
                    "Action Turn Battle\r\nSame as Extra Turn Battle but without cooldowns. Same hotkeys above.\r\n\r\n" +
                    "Quick Turn Battle\r\nThe Quick Turn Battle Bar is at the top right of the Battle Stats tab. You have a maximum of five points shared by the party. You gain 1 point if a single character is attacked, 2 if 2 or more party members are attacked. You also gain 1 QTB point when the leader takes a normal turn. Monsters will gain turn points when you use this however. The hotkeys are the same as Extra Turn Battle. Select the correct leader in the drop down below the button or you will not get the extra 1 turn when the leader takes a turn.\r\n\r\n" +
                    "Solo Mode/Duo Mode\r\nSolo/Duo Mode will allow you to battle with 1 or 2 characters. For boss battles with cutscenes you need to click \"Add Party Members\". If you are unsure click the \"On\" button beside it to always have it turned on. If you have it turned on you can change which slot the leader is in for Solo Mode.\r\n\r\nWhen you do not have Dart in slot 1 please make sure to have short transformation turned on (you must have used the first time long transform even if it is on short) or you can use the Auto Transform button to automatically use short transform.\r\n\r\n" +
                    "Switch Slot 1 Character\r\nTo change Dart out use the \"Switch Slot 1 Character\" button and combobox. You can only switch characters that are in the party. Dart can be switched out now as well through the in-game replacement menu. It may take 1 battle to take effect.\r\n\r\n" +
                    "Aspect Ratio/Advanced Camera\r\nAspect Ratio will change your ratio in battle. If you are on the World Map you can use hotkey (L1 + LEFT) to change it manually. The Advanced Camera combobox beside this will allow you to change some 3D effects to your aspect ratio, however it can glitch some effects.\r\n\r\n" +
                    "Kill BGM\r\nKills the music in field, battle, or both.\r\n\r\n" +
                    "Switch EXP\r\nAllows you to switch EXP with the charters in both comboboxes. They must be in your party. For Normal/Hard Mode you can swap characters up to 80,000 EXP. In Hell Mode it is 160,000 EXP.\r\n\r\n" +
                    "New Game+\r\nUnfinished and not usable.";
            } else if (cboHelpTopic.SelectedIndex == 4) {
                txtHelp.Text = "Battle Rows\r\nCharacters will get stat changes based on their row. You cannot turn this on for Hell Mode.\r\n" +
                    "Stay - Attack - AT 110% - DF & MDF 75%\r\n" +
                    "Stay - Magic - MAT 110% - DF & MDF 75%\r\n" +
                    "Front - No Boost - AT & MAT 125% - DF & MDF 50%\r\n" +
                    "Front - Attack - AT 150% - DF & MDF 25%\r\n" +
                    "Front - Magic - MAT 150% - DF & MDF 25%\r\n" +
                    "Back - No Boost - AT & MAT 75% - DF & MDF 125%\r\n" +
                    "Back - Attack - MAT 50% - DF & MDF 110%\r\n" +
                    "Back - Magic - AT 50% - DF & MDF 110%\r\n\r\n" +
                    "No Dart will turn on automatically when detected, Dart will switch in battle not in the party menu with three characters. Refer to How To help section for more info.\r\n\r\n" +
                    "Damage Tracker\r\nTracks your damage throughout a single battle.\r\n\r\n" +
                    "No Dragoon\r\nRemoves Dragoons in battle.\r\n\r\n" +
                    "Divine Red-Eye\r\nGives Red Eyed Dragoon the same power as Divine Dragoon in Hard & Hell Mode.\r\n\r\n" +
                    "Early Additions\r\nChanges Addition unlock levels.\r\n\r\n" +
                    "Dart\r\nCrush Dance - 13\r\nMadness Hero - 18\r\nMoon Strike - 23\r\nBlazing Dynmao - 29 if all additions are max level.\r\n\r\n" +
                    "Lavitz/Albert\r\nRod Typhoon - 10\r\nGust of Wind Dance - 16\r\nFlower Storm - 21 if all additions are max level.\r\n\r\n" +
                    "Rose\r\nMore & More - 8\r\nHard Blade - 15\r\nDemon's Dance - 21 if all additions are max level.\r\n\r\n" +
                    "Kongol\r\nInferno - 10\r\nBone Crush - 20 if all additioins are max level.\r\n\r\n" +
                    "Meru\r\nHammer Spin - 6\r\nCool Boogie - 12\r\nCat's Cradle - 18\r\nPerky Step - 22 if all additions are max level.\r\n\r\n" +
                    "Haschel\r\nFlurry of Styx - 5\r\nSummon 4 Gods - 10\r\n5 Ring Shattering - 16\r\nHex Hammer - 22\r\nOmni-Sweep - 25 if all additions are max level.\r\n\r\n" +
                    "Addition Level Up\r\nLevels up additions in battle. The addition data is taken from the current Mod location in Settings Tab>Settings>Mod Options.\r\n\r\n" +
                    "Reduce Solo/Duo EXP\r\nReduces each individual monster's EXP by 33% for Solo Mode and 66% for Duo Mode to prevent over levelling. This value is always rounded up.\r\n\r\n" +
                    "Flower Storm\r\nChanges the MP usage and turn count for Flower Storm in Hell Mode only.\r\n\r\n" +
                    "Element Arrow\r\nWhen using the Element Arrow change the type of element the arrow will use.";
            } else if (cboHelpTopic.SelectedIndex == 5) {
                txtHelp.Text = "Text Speed\r\nIncreases text scrolling speed.\r\n\r\n" +
                    "Auto Advance Text\r\nAutomatically advances the first dialog box popup. You still have to press X occasionally.\r\n\r\n" +
                    "Black Room\r\nIn the forest you can quickly level up additions or practice by using the Black Room. Use hotkey (CIRCLE + R2) to kill monsters in the Black Room. You cannot kill the monsters in Hell mode.\r\n\r\n" +
                    "Zoom\r\nChanges the Zoom value in battle.";
            } else if (cboHelpTopic.SelectedIndex == 6) {
                txtHelp.Text = "Ultimate Boss\r\nFrom Chapter 4 you can go back to The Forbidden Land to fight tougher versions of bosses. You must be in the correct map per zone and each zone has a reommended level. You can gain lots of Gold from these bosses starting from Zone 3. Each boss is detailed below. The equips are for Hard and Hell mode only.\r\n\r\n" +
                    "\r\n\r\nZone 1 - Level 30\r\n\r\n" +
                    "1. Commander - 64,000 HP\r\nDrops Sabre, a +70 AT weapon for Rose.\r\n\r\n" +
                    "2. Fruegel I - 63,000 HP\r\n\r\n" +
                    "3. Urobolus - 61,600 HP\r\nDefeating this boss will increase your inventory to 36 Slots.\r\n\r\n" +
                    "\r\n\r\nZone 2 - Level 40\r\n\r\n" +
                    "4. Sandora Elite - 159,600 HP\r\n\r\n" +
                    "5. Drake the Bandit - 148,000 HP\r\n\r\n" +
                    "6. Jiango - 204,800 HP\r\nThis boss has Zero SP start.\r\n\r\n" +
                    "7. Fruegel II - 220,000 HP\r\n\r\n" +
                    "8. Fire Bird - 281,600 HP\r\nThis boss has Zero SP start. This boss has Guard Break on dive attack. This boss has MP Attack on summon. Defeating this boss will increase your inventory to 40 slots.\r\n\r\n" +
                    "\r\n\r\nZone 3 - Level 50\r\n\r\n" +
                    "9. Ghost Feyrbrand - 320,000 HP | 3,000 Gold\r\n\r\n" +
                    "10. Mappi - 128,000 HP | 3,000 Gold\r\n\r\n" +
                    "11. Gehrich - 200,000 HP | Mappi - 128,000 HP | 3,000 Gold\r\nThis boss has Zero SP start.\r\n\r\n" +
                    "12. Ghost Commander - 221,000 HP | 3,000 Gold\r\nThis boss has Wound Damage on slash attack. This boss has Health Steal on life sap attack.\r\n\r\n" +
                    "13. Kamuy - 300,000 HP | 3,000 Gold\r\nThis boss unlocks an used attack.\r\nThis boss does SP damage.\r\n\r\n" +
                    "14. Ghost Regole - 336,000 HP | 3,000 Gold\r\n\r\n" +
                    "15. Grand Jewel - 260,000 HP | 9,000 Gold\r\nThis boss has a Magic Change every 10%.\r\nThis boss has elemental shift.\r\nThis Boss has Reverse Dragon Block Staff.\r\n\r\n" +
                    "16. Windigo - 700,000 HP | 9,000 Gold\r\nThis boss has Armor Break when heart is damaged.\r\n\r\n" +
                    "17. Polter Armor - 666.666 HP | 9,000 Gold\r\nThis boss has Shared HP.\r\n\r\n" +
                    "18. The Last Kraken - 360,000 HP | 12,000 Gold\r\nThis boss is actually centered.\r\n\r\n" +
                    "19. Vector - 180,000 HP | Selebus - 135,000 HP | Kubila - 157,500 HP | 12,000 Gold\r\n\r\n" +
                    "20. Caterpillar - 120,000 HP | Pupa - 180,000 HP | Imago - 240,000 HP | 12,000 Gold\r\n\r\n" +
                    "21. Zackwell - 360,000 HP | 15,000 Gold\r\n\r\n" +
                    "22. Ghost Divine Dragon - 400,000 HP | 18,000 Gold\r\nDefeating this boss will increase your inventory to 48 Slots.\r\n\r\n" +
                    "\r\n\r\nZone 4 - Level 60\r\n\r\n" +
                    "23. Virage I | Head - 360,000 HP | Body - 360,000 HP | Arm - 60,000  HP | 15,000 Gold\r\n\r\n" +
                    "24. Kongol - 420,000 HP | 20,000 Gold\r\n\r\n" +
                    "25. Lenus - 525,000 HP | 20,000 Gold\r\nThis boss has a Magic Change every 5%.\r\n\r\n" +
                    "26. Syuveil - 500,000 HP | 25,000 Gold\r\nThis boss has Turn Point damage on all Dragoon magic attacks.\r\n\r\n" +
                    "27. Virage II | Head - 1,280,000 HP | Body - 540,000 HP | Arm - 54,000 HP | 30,000 Gold\r\nThis boss has body damage.\r\n\r\n" +
                    "28. Feyrbrand - 288,000 HP | Greham - 210,000 HP | 35,000 Gold\r\nThis boss has Dragoon Bond.\r\nThis boss will remove resistances.\r\n\r\n" +
                    "29. Damia - 360,000 HP | 35,000 Gold\r\nThis boss has a custom status effect, Menu Block on all magic attacks. Will block all menu actions, Dragoons are immune.\r\n\r\n" +
                    "30. Regole - 300,000 HP | Dragoon Lenus - 300,000 HP | 40,000 Gold\r\nThis boss has Dragoon Bond.\r\n\r\n" +
                    "31. Belzac - 608,000 HP | 40,000 Gold\r\nThis boss has custom status effects, each with a random chance of activation. 30% accuracy loss on Grand Stream. Power Down DF/MDF on Meteor Strike. Speed Down on Golden Dragoon.\r\n\r\n" +
                    "32. S Virage I | Head - 320,000 HP | Body - 320,000 HP | Arm - 160,000 HP | 1,000 - 75,000 Gold\r\nThis boss has Gold farming opportunities.\r\nThis boss has countdown changes. For every 40,000 damage, countdown increases. For each countdown increase you get 1,000 Gold each. Killing a body part grants the following gold: 45,000 Gold for the head, 15,000 Gold for the body, 10,000 Gold for the arm.\r\n\r\n" +
                    "33. Kanzas - 396,000 HP | 45,000 Gold\r\nThis boss has Electric Charges, with a maximum of 30 charges. Each charge is released all at once and can be released at any time, each charge grants 5% power on the next attack. Dragoon Addition grants 1 charge. Atomic Mind grants 3 charges and attack down for 3 turns. Thunder Kid grants 5 charges and defense down for 3 turns. Violet Dragon grants 15 charges and instantly releases all charges for this attack and grants power down for 3 turns.\r\n\r\n" +
                    "34. Emperor Doel - 250,000 HP | Dragoon Doel - 750,000 HP | 100,000 Gold\r\nThis boss has Inventory Refresh.\r\nThis boss has Ultimate Enrage Mode.\r\nThis Boss has a Magic Change. Doel can now cast any magic when he is below 75,000 HP and will use elemental weaknesses to his advantage.\r\nThis boss has Enhanced Shield. Doel's Shield when it is about to appear will grant him Damage Immunity. The Shield grants him half damage.\r\nDefeating this boss will increase your inventory to 64 Slots.\r\nIf you are on Hell Mode you will unlock Divine Red-Eyed Dragon mode.\r\n\r\n" +
                    "35. S Virage II | Head - 333,333 HP | Body - 222,222 HP | Arm 666,666 | 60,000 Gold\r\nThis boss has a modified Shared HP. Attacking the head heals the arm. Each attack to a body part will do 2x damage. Each part healed will recieve 1x HP. Attacking the arm heals the head. Attacking the head heals the body.\r\nThis boss has an enhanced Final Attack.\r\n\r\n" +
                    "36. Divine Dragon - 10,000 HP | 70,000 Gold\r\nThis boss has Armor Guard.\r\nThis Boss has Reverse Dragon Block Staff.\r\nThis boss has Ultimate Enrage Mode.\r\n\r\n" +
                    "37. Lloyd - 666,666 HP | 80,000 Gold\r\nThis boss has modified Ultimate Enrage Mode, Lloyd will increase his AT/MAT stats but lower his DF/MDF stats. Dying by his Dragoon Buster attack will lower his stats, but each time you die Lloyd's base stats increase.\r\nThis boss will remove resistances.\r\nThis boss has a Magic Change every 7%.\r\n\r\n" +
                    "38. Magician Faust - 1,000,000 HP | 120,000 Gold\r\nThis boss has Dragoon Guard.\r\nThis boss has any magic and will play to your weakness and strengths depending on the phase.\r\n\r\n" +
                    "39. Zieg - 720,000 HP | 100,000 Gold\r\nThis boss unlocks unused attacks.\r\nThis boss has enhanced damage on Explosion.\r\n\r\n" +
                    "40. Melbu Frahma - ??? HP - Unfinished.\r\n\r\n" +
                    "Zero SP - Start the battle with zero SP.\r\n" +
                    "Guard Break - Removes guard status on a certain attack.\r\n" +
                    "MP Attack - Removes MP on a certain attack.\r\n" +
                    "Wound Damage - Reduces Max HP on a certain attack. Dying will restore Max HP.\r\n" +
                    "Health Steal - Health Steals on a certain attack.\r\n" +
                    "SP Damage - Damages your SP, dragoons are immune to this damage.\r\n" +
                    "Magic Change - Changes magic based on HP intervals. All magic is applicable, Faust and Melbu are the only bosses that can cast Psyche Bomb.\r\n" +
                    "Elemental Shift - Element changes based on the item used.\r\n" +
                    "Armor Break - Defense drop drastically when a specific monster is targeted.\r\n" +
                    "Shared HP - Attacking one part deals damage to the rest.\r\n" +
                    "Turn Point Damage - Removes turn points on a cetain attack.\r\n" +
                    "Body Damage - Killing one part damages the main part for all of its HP.\r\n" +
                    "Dragoon Bond - Attacking first determines the following. Attack either dragoon or dragon first, the other monster will heal the other for all damage. When one monsters dies the other becomes more powerful. Attacking both dragon and dragoon at the same time both of them become more powerful for a lesser amount.\r\n" +
                    "Remove Resistances - All resistances are removed.\r\n" +
                    "Countdown - Changes countdown mechanics.\r\n" +
                    "Inventory Refresh - Refreshes inventory at a certain point in battle.\r\n" +
                    "Ultimate Enrage Mode - Bosses will increase their stats for every 1% of damage.\r\n" +
                    "Reverse Dragon Block Staff - Dragoons will operate at 80%.\r\n" +
                    "Armor Guard - Significant increase to defenses when Guarding, overwrites Power Up/Down DF/MDF effects.\r\n" +
                    "Dragoon Guard - Adds Guard to the menu in Dragoon.\r\n\r\n" +
                    "Equips\r\n" +
                    "Sabre - +70 AT - A weapon for Rose.\r\n" +
                    "Spirit Eater - +75 AT +50 MAT - A weapon for Dart. Removes 35 SP per turn unless full. Removes 15 SP instead in Hell Mode.\r\n" +
                    "Harpoon - +100 AT - A weapon for Lavitz/Albert. Triples Dragoon powers at the cost of 300 SP. You can only use this when you have 400 SP or more. Casts Speed Down upon Dragoon exit.\r\n" +
                    "Element Arrow - +50 AT +50 MAT - A weapon for Shana/Miranda. Changes element and uses 100G to restock that element's single target magic item every 3 turns. Change Element in Enhancements II tab.\r\n" +
                    "Dragon Beater - +130 AT - A weapon for Rose. Enhances Dragoon magic by using hotkey (CIRCLE + RIGHT) in battle. Rose will consume more MP, do more damage and heal 30% more from Dark Dragon but deal less damage. Return to normal by pressing hotkey (CIRCLE + RIGHT) again.\r\n" +
                    "Battery Glove - +80 AT +20 MAT - A weapon for Haschel. Charges for an attack that deals 250% physical damage and unleashes it automatically every 7 attacks.\r\n" +
                    "Jeweled Hammer - +40 AT +40 MAT - A weapon for Meru. Enhances Dragoon magic by using hotkey (CIRCLE + DOWN) in battle. Meru will consume more MP, do more damage and Rainbow Breath will add an additional 65% HP that can go over Max HP. Guarding will remove the additional HP. Return to normal by pressing hotkey (CIRCLE + DOWN) again.\r\n" +
                    "Giant Axe - +100 AT +10 MAT - A weapon for Kongol. Has a 20% chance to add Guard after attacking.\r\n" +
                    "Soa's Light - +200 AT +140 MAT +100 SP Regen Per Turn - Reduces other party members defenses by 30%. Removes all SP gain from additions.\r\n" +
                    "Fake Legend Casque - +30 MDF - Has a 30% chance to add +40 MDF while Guarding.\r\n" +
                    "Soa's Helm - +200 MDF +20 MP Regen Per Turn - Reduces other party members attack for 30%.\r\n" +
                    "Fake Legend Armor - +30 DF - Has a 30% chance to add +40 DF while Guarding.\r\n" +
                    "Divine DG Armor - +50 DF +50 MDF +20 SP on all hits +10 MP on all hits.\r\n" +
                    "Soa's Armor - +200 DF +20% HP Regen Per Turn - Reduces other party members MAT by 30%.\r\n" +
                    "Lloyd's Boots - +15 SPD +15 A-AV +15 M-AV\r\n" +
                    "Winged Shoes - +25 SPD\r\n" +
                    "Soa's Greaves - +40 SPD - Reduces other party members speed by 25.\r\n" +
                    "Heal Ring - +7% HP Regen Per Turn, +7 MP Regen Per Turn, +7 SP Regen Per Turn\r\n" +
                    "Soa's Sash - Double SP Gain - Reduces other party members SP gain by half.\r\n" +
                    "Soa's Ahnk - 100% Revive - Always revive but kills another party member in the process. Grants 50% revive in Solo Mode.\r\n" +
                    "Soa's Health Ring - Doubles Max HP - Reduces other party members Max HP by 25%. Max HP is capped at 32767.\r\n" +
                    "Soa's Mage Ring - Triples Max MP - Reduces other party members Max MP by 50%.\r\n" +
                    "Soa's Shield Ring - Sets DF/MDF to 1, Sets A-AV and M-AV to 90, overwrites all equips - Reduces other party members hit accuracy by 20%.\r\n" +
                    "Soa's Siphon Ring - Doubles MAT stats, reduces Dragoon Magic by 70% - Reduces other party members MAT by 20%.";
            } else if (cboHelpTopic.SelectedIndex == 7) {
                txtHelp.Text = "Hero Competition Shop\r\nAllows you to buy tickets and purchase items from the Hero Compition anywhere.\r\n\r\n" +
                    "Ultimate Boss Shop\r\n" +
                    "Allows you to buy new equips from the shop. This equips are only available for Hard & Hell Mode. You can only buy items starting at Chapter 4.";
            } else if (cboHelpTopic.SelectedIndex == 8) {
                txtHelp.Text = "Reader Mode will read stats from battle and display them to a window and or write them to a text file for an external application to use.\r\n\r\n" +
                    "When you open Dragoon Modifier opent Window Config first to load your Reader Mode config. You do not have to do this again once you've done this. Use Add/Change/Delete buttons to add UI elements to the Window, a form will open up to create the display. You can double click items to change them as well, if you can't click the item use Change. On the change windows once you have changed a value, keep it on the same textbox and press enter to update it.\r\n\r\n" +
                    "Save will save your current Reader Mode window setup and Load will load a previous setup. Reset will wipe the current setup.\r\n\r\n" +
                    "You can have Dragoon Modifier press a hotkey for you automatically when the Battle UI is open in battle.\r\n\r\n" +
                    "You have two modes to remove the UI in battle. One is to remove it complete, the other is to remove character display pictures only.\r\n\r\n" +
                    "For the change forms here are the values you can use for Value/Minimum/Maximum\r\nName, Action, Menu, LV, DLV, HP, Max_HP, MP, Max_MP, SP, Max_SP, Element, Display_Element, AT, OG_AT, MAT, OG_MAT, DF, OG_DF, MDF, OG_MDF, SPD, OG_SPD, Turn, A_HIT, M_HIT, A_AV, M_AV, P_Immune, M_Immune, P_Half, M_Half, E_Immune, E_Half, On_Hit_Status, On_Hit_Status_Chance, Stat_Res, Death_Res, SP_P_Hit, SP_M_Hit, MP_P_Hit, MP_M_Hit, HP_Regen, MP_Regen, SP_Regen, SP_Multi, Revive, Unique_Index, Image, DAT, DMAT, DDF, DMDF, Special_Effect, Guard, Dragoon, Spell_Cast, PWR_AT, PWR_AT_TRN, PWR_MAT, PWR_MAT_TRN, PWR_DF, PWR_DF_TRN, PWR_MDF, PWR_MDF_TRN, ADD_SP_Multi, ADD_DMG_Multi, Weapon, Helmet, Armor, Shoes, Accessory, POS_FB, POS_UD, POS_RL, A_HIT_INC, A_HIT_INC_TRN, M_HIT_INC, M_HIT_INC_TRN, PHYSICAL_IMMUNITY, PHYSICAL_IMMUNITY_TRN, ELEMENTAL_IMMUNITY, ELEMENTAL_IMMUNITY_TRN, SPEED_UP_TRN, SPEED_DOWN_TRN, SP_ONHIT_PHYSICAL, SP_ONHIT_PHYSICAL_TRN, MP_ONHIT_PHYSICAL, MP_ONHIT_PHYSICAL_TRN, SP_ONHIT_MAGIC, SP_ONHIT_MAGIC_TRN, MP_ONHIT_MAGIC, MP_ONHIT_MAGIC_TRN, Color_Map, Burn Stack, Damage Tracker1, Damage Tracker2, Damage Tracker3";
            } else if (cboHelpTopic.SelectedIndex == 9) {
                txtHelp.Text = "Attach/Detach - Attach or Detach Dragoon Modifier from the emulator. Use this when for example you close ePSXe and reopen it. For RetroArch (and other emulators) please reattach when you are in game or at the load save screen.\r\n" +
                    "Menu - This will wipe, create, or save your current script and mod options. External scripts are in the lists below.\r\n\r\n" +
                    "Settings - Change your Emulator, Game Region, Save Slot, or Mod Options here. Dragoon Modifier will save progress of certain features to your computer. If you want to delete your progress click Delete Current Save. Preset hotkeys are hotkeys that Dragoon Modifier provides and can be turned off.\r\n\r\n" +
                    "Mod Options - Mods will be placed in the Mods subfolder of Dragoon Modifier and can be accessed through here. You can turn on or off specfic changes.\r\nMonster StatsChanges monster stats.\r\nDrop - Changes what monsters drop.\r\nExp + Gold - Changes the exp and gold rewards for monsters.\r\nAddition - Changes addition damage / sp per hit.\r\nDragoon Stats - Changes stats of dragoons per level.\r\nDragoon Spells - Changes the damage of dragoon spells.\r\nDragoon Additions - Changes the damage of dragoon additions.\r\nItem Stats - Changes the stats of items.\r\nItem Icons - Changes the icons of items on the fields.\r\nItem Names + Descriptions - Changes the name and description of items.\r\nShop - Changes what is displayed in the shop.The item prices are controlled by items spreadsheet.\r\n\r\n\r\n" +
                    "The list of the four columns below contains external scripts for Dragoon Modifier. Dragoon Modifier requires the Field Controller, Battle Controller, and Hotkey controller to run correctly. Scripts are located in the Scripts sub folder. If something should be run when the player is on the Field should be placed in the Field folder and will display in Dragoon Modifier. Developers will release scripts that will add features not provided by Dragoon Modifier by default. You should only get your scripts from trusted sources.";
            } else if (cboHelpTopic.SelectedIndex == 10) {
                txtHelp.Text = "Field\r\n\r\n" +
                    "L2 + SQAURE       - Shana will use Gates of Heaven outside of battle.\r\n" +
                    "L2 + CROSS        - Miranda will use Gates of Heaven outside of battle.\r\n" +
                    "L2 + CIRCLE       - Meru will use Rainbow Breath outside of battle.\r\n" +
                    "SELECT + L3       - Adds Shana when she is not in the party.\r\n" +
                    "SELECT + R3       - Adds Lavitz when he is not in the party.\r\n" +
                    "CROSS + L1        - Adds all Dragoons in the opening map 10 when Hard and Hell Mode is on. Sets everyone to level 1 with base equips in the opening map 10 when Solo or Duo mode is on. After Mappi battle it will add Dart's dragoon back regardless of difficulty.\r\n" +
                    "CROSS + R1        - Before you complete the quest in Furni map 333 press this hotkey, press it again after for extra gold from the quest.\r\n" +
                    "SQUARE + TRIANGLE - When in Hard Mode (or Hell Mode when you have beaten Ultimate Boss Dragoon Doel) you may switch between Red-Eyed and Divine Dragoon at the Divine Dragon's corpse (map 424) or on the moon (map 736).\r\n" +
                    "SQUARE + CROSS    - Spawns a slightly modified Faust for EXP and Armor of Legend of Legend Casque (39/40th battle) on the moon (map 732). (Hold on screen transition)\r\n" +
                    "START + L3        - Allows you to warp off the moon to Ulra (maps 729/730/527). (Hold on screen transition)\r\n" +
                    "START + R3        - Allows you to warp back to the moon from Ulra (maps 597/521/524/526/527/729/9). (Hold on screen transition)\r\n" +
                    "CIRCLE + TRIANGLE - Skip dialogs, stop before choice dialog (will cause a softlock if used incorrectly) or use Auto Text Advanced in Enhancements III.\r\n" +
                    "L1 + LEFT         - Sets widescreen for the world map.\r\n\r\n" +
                    "Battle\r\n\r\n" +
                    "L1 + UP           - Exit Dragoon Slot 1.\r\n" +
                    "L1 + RIGHT        - Exit Dragoon Slot 2.\r\n" +
                    "L1 + LEFT         - Exit Dragoon Slot 3.\r\n" +
                    "L1 + CIRCLE       - Sets music speed to 0.\r\n" +
                    "SQUARE + UP       - Starts an extra turn on Slot 1 when a turn battle system is turned on. Try to activate on enemy turns only, may softlock otherwise.\r\n" +
                    "SQUARE + RIGHT    - Starts an extra turn on Slot 2 when a turn battle system is turned on. Try to activate on enemy turns only, may softlock otherwise.\r\n" +
                    "SQUARE + LEFT     - Starts an extra turn on Slot 3 when a turn battle system is turned on. Try to activate on enemy turns only, may softlock otherwise.\r\n" +
                    "CIRCLE + RIGHT    - Changes Rose's magic when she has the Dragon Beater equipped.\r\n" +
                    "CIRCLE + DOWN     - Changes Meru's magic when she has the Jeweled Hammer equipped.\r\n" +
                    "CIRCLE + R2       - Kills monsters when you have entered the black room. Not available on Hell Mode.\r\n" +
                    "SELECT + START    - Nerfs bosses (Doel, Lenus, Executioners) and the last three bosses of the game suitable for level 50s for Hard Mode.\r\n" +
                    "SELECT + R3       - Nerfs the last three bosses of the game suitable for level 40s for Hard Mode.\r\n" +
                    "L2 + LEFT         - Activates Soa's Wargod.\r\n" +
                    "L2 + RIGHT        - Activates Soa's Dragoon Boost.\r\n" +
                    "L2 + UP           - Activates Empty Dragoon Crystal.\r\n" +
                    "L1 + R1           - Activates Adddition Swap in battle if you have no status effects. The number of dragoon spirits the appear are equal to the number of available additions. The additions are ordered from left to right (1 - 7 max, same order as how they appear in the menu), for example Dart's third icon would be Burning Rush. Press Dragoon to automatically transform to switch additions.";
            } else if (cboHelpTopic.SelectedIndex == 11) {
                txtHelp.Text = "1. No Dart\r\n" +
                    "When you have three party members use Switch Slot 1 and No Dart should turn on. When you are switching between Solo and Duo Mode or turning them off make sure to turn off No Dart in Enhancements Tab II. To turn off No Dart click the No Dart Mode button in Enhancements Tab 3.\r\n\r\n" +
                    "2. Solo / Duo Mode - Boss Encounters with Cutscenes\r\n" +
                    "The game requires you to have three party members for cutscenes. To turn on party members for a single battle press Add Party Members green button in Enhancements Tab I. To turn it on for all battles press Add Party Members green button, and the on button beside it. Extra characters will die on entry and move off screen.\r\n\r\n" +
                    "3. Reader Mode\r\n" +
                    "Each time you open Dragoon Modifier open Windows Config first to load your Reader Mode settings.\r\n\r\n" +
                    "4. Scripts\r\n" +
                    "In the settings tab activated scripts are in black text, deactivated scripts are in red. To activate a script single click the script to select it and then press the grey button on top of the script to change it's state. Some scripts require input, in this case you would double click it. The current script loadout can be saved or loaded by clicking Menu > Save/Load.\r\n\r\n" +
                    "5. Solo Mode Start & Playthrough\r\n" +
                    "To start Solo Mode with everyone level 1, first turn on Solo Mode in the opening Seles map. Then use the hotkey Cross+L1 and everyone will be in the party at level 1. There is a separate script in the Settings tab called Party Changed. You must enable the script by selecting the script, then selecting the green button in the same column above. You can then double click it to add certain party members back into the party. You can change the character in slot 1 by using the Switch Slot 1 Character in Enhancements Tab I.";
            }
        }

        #region Reader Window
        //TODO: This can be cleaned up a lot..
        public void ReaderModeConfig() {
            if (!readerWindow.IsOpen)
                LoadReaderKey();
            InputWindow readerConfigWindow = new InputWindow("Reader Window Config");
            TextBox width = new TextBox();
            TextBox height = new TextBox();
            CheckBox write = new CheckBox();
            CheckBox antiAlias = new CheckBox();
            Button openFolder = new Button();
            Xceed.Wpf.Toolkit.ColorPicker background = new Xceed.Wpf.Toolkit.ColorPicker();

            width.Text = readerWindow.Width.ToString();
            height.Text = readerWindow.Height.ToString();
            write.IsChecked = readerWindow.GetWriteText();
            antiAlias.IsChecked = readerWindow.GetAliasMode();
            openFolder.Content = readerWindow.GetWriteLocation();
            SolidColorBrush bgBrush = ((SolidColorBrush) readerWindow.Background);
            background.SelectedColor = Color.FromArgb((byte) readerWindow.Background.Opacity, bgBrush.Color.R, bgBrush.Color.G, bgBrush.Color.B);

            openFolder.Click += new RoutedEventHandler(delegate (Object o, RoutedEventArgs b) {
                Constants.WritePLogOutput("Reader Mode writes to the disk every 2 seconds. Best to use a RAMDISK.");
                System.Windows.Forms.FolderBrowserDialog folder = new System.Windows.Forms.FolderBrowserDialog();
                System.Windows.Forms.DialogResult result = folder.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(folder.SelectedPath)) {
                    readerWindow.SetWriteLocation(folder.SelectedPath);
                    openFolder.Content = folder.SelectedPath;
                    if (!Directory.Exists(folder.SelectedPath + "/Character"))
                        Directory.CreateDirectory(folder.SelectedPath + "/Character");
                    if (!Directory.Exists(folder.SelectedPath + "/Monster"))
                        Directory.CreateDirectory(folder.SelectedPath + "/Monster");
                }
            });

            readerConfigWindow.AddObject(background);
            readerConfigWindow.AddTextBlock("Background");
            readerConfigWindow.AddObject(openFolder);
            readerConfigWindow.AddTextBlock("Write Text Folder");
            readerConfigWindow.AddObject(write);
            readerConfigWindow.AddTextBlock("Write Text?");
            readerConfigWindow.AddObject(antiAlias);
            readerConfigWindow.AddTextBlock("Anti Alias?");
            readerConfigWindow.AddObject(height);
            readerConfigWindow.AddTextBlock("Height");
            readerConfigWindow.AddObject(width);
            readerConfigWindow.AddTextBlock("Width");
            readerConfigWindow.ShowDialog();

            readerWindow.Width = Double.Parse(width.Text);
            readerWindow.Height = Double.Parse(height.Text);
            readerWindow.WriteText((bool) write.IsChecked);
            readerWindow.SetAntiAlias((bool) antiAlias.IsChecked);
            readerWindow.SetWriteLocation(openFolder.Content.ToString());
            readerWindow.Background = new SolidColorBrush((Color) background.SelectedColor);

            SaveReaderKey();
        }

        public void ReaderModeAdd() {
            InputWindow readerAddConfigWindow = new InputWindow("Reader Window Add Config");
            ComboBox readerComponents = new ComboBox();

            readerComponents.Items.Add("Label (Just Text)");
            readerComponents.Items.Add("Battle Label (Reads and updates battle values)");
            readerComponents.Items.Add("Progress Bar (Battle values with min and maxs)");
            readerComponents.Items.Add("Radial Progress Bar (Battle values with min and maxs)");
            readerComponents.SelectedIndex = 0;

            readerAddConfigWindow.AddObject(readerComponents);
            readerAddConfigWindow.AddTextBlock("Which do you want to add?");

            readerAddConfigWindow.ShowDialog();

            if (readerComponents.SelectedIndex == 0) {
                InputWindow readerAddWindow = new InputWindow("Reader Window Add Label");
                Grid grid = new Grid();
                Label lbl1 = new Label();
                Label lbl2 = new Label();
                Label lbl3 = new Label();
                Label lbl4 = new Label();
                Label lbl5 = new Label();
                Label lbl6 = new Label();
                Label lbl7 = new Label();
                Label lbl8 = new Label();
                Label lbl9 = new Label();
                Label lbl10 = new Label();
                Label lbl11 = new Label();
                TextBox txtId = new TextBox();
                TextBox txtContent = new TextBox();
                TextBox txtFontFamily = new TextBox();
                TextBox txtFontSize = new TextBox();
                TextBox txtX = new TextBox();
                TextBox txtY = new TextBox();
                TextBox txtZ = new TextBox();
                ComboBox cboAlignment = new ComboBox();
                TextBox txtWidth = new TextBox();
                Xceed.Wpf.Toolkit.ColorPicker clpForeground = new Xceed.Wpf.Toolkit.ColorPicker();
                Xceed.Wpf.Toolkit.ColorPicker clpBackground = new Xceed.Wpf.Toolkit.ColorPicker();

                lbl1.Content = "ID";
                lbl2.Content = "Content";
                lbl3.Content = "Font Family";
                lbl4.Content = "Font Size";
                lbl5.Content = "X";
                lbl6.Content = "Y";
                lbl7.Content = "Z";
                lbl8.Content = "Alignment";
                lbl9.Content = "Width";
                lbl10.Content = "Foreground";
                lbl11.Content = "Background";
                txtId.Text = "ID";
                txtContent.Text = "Label Text";
                txtFontFamily.Text = "Arial";
                txtFontSize.Text = "12";
                txtX.Text = "0";
                txtY.Text = "0";
                txtZ.Text = "0";
                cboAlignment.Items.Add("Left");
                cboAlignment.Items.Add("Center");
                cboAlignment.Items.Add("Right");
                cboAlignment.SelectedIndex = 0;
                clpForeground.SelectedColor = Color.FromArgb(255, 0, 0, 0);
                clpBackground.SelectedColor = Color.FromArgb(0, 0, 0, 0);

                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());

                SetObject(ref grid, (Control) lbl1, 0, 0);
                SetObject(ref grid, (Control) lbl2, 0, 1);
                SetObject(ref grid, (Control) lbl3, 0, 2);
                SetObject(ref grid, (Control) lbl4, 0, 3);
                SetObject(ref grid, (Control) lbl5, 0, 4);
                SetObject(ref grid, (Control) lbl6, 0, 5);
                SetObject(ref grid, (Control) lbl7, 0, 6);
                SetObject(ref grid, (Control) lbl8, 0, 7);
                SetObject(ref grid, (Control) lbl9, 0, 8);
                SetObject(ref grid, (Control) lbl10, 0, 9);
                SetObject(ref grid, (Control) lbl11, 0, 10);
                SetObject(ref grid, (Control) txtId, 1, 0);
                SetObject(ref grid, (Control) txtContent, 1, 1);
                SetObject(ref grid, (Control) txtFontFamily, 1, 2);
                SetObject(ref grid, (Control) txtFontSize, 1, 3);
                SetObject(ref grid, (Control) txtX, 1, 4);
                SetObject(ref grid, (Control) txtY, 1, 5);
                SetObject(ref grid, (Control) txtZ, 1, 6);
                SetObject(ref grid, (Control) cboAlignment, 1, 7);
                SetObject(ref grid, (Control) txtWidth, 1, 8);
                SetObject(ref grid, (Control) clpForeground, 1, 9);
                SetObject(ref grid, (Control) clpBackground, 1, 10);

                readerAddWindow.AddObject(grid);
                readerAddWindow.ShowDialog();

                if (readerWindow.IsUnique(txtId.Text)) {
                    if (!readerWindow.AddLabel(txtId.Text, txtContent.Text, txtFontFamily.Text, txtFontSize.Text, txtX.Text, txtY.Text, txtZ.Text, cboAlignment.SelectedIndex, txtWidth.Text, clpForeground, clpBackground))
                        Constants.WritePLogOutput("Something is wrong with one of the inputs.");
                } else {
                    Constants.WritePLogOutput("Input did not have a unique ID.");
                }
            } else if (readerComponents.SelectedIndex == 1) {
                InputWindow changeWindow = new InputWindow("Reader Window Add Battle Label", false);
                Grid grid = new Grid();
                Label lbl1 = new Label();
                Label lbl2 = new Label();
                Label lbl3 = new Label();
                Label lbl4 = new Label();
                Label lbl5 = new Label();
                Label lbl6 = new Label();
                Label lbl7 = new Label();
                Label lbl8 = new Label();
                Label lbl9 = new Label();
                Label lbl10 = new Label();
                Label lbl11 = new Label();
                Label lbl12 = new Label();
                Label lbl13 = new Label();
                Label lbl14 = new Label();
                TextBox txtId = new TextBox();
                TextBox txtFontFamily = new TextBox();
                TextBox txtFontSize = new TextBox();
                TextBox txtX = new TextBox();
                TextBox txtY = new TextBox();
                TextBox txtZ = new TextBox();
                CheckBox chkPlayer = new CheckBox();
                TextBox txtSlotSelect = new TextBox();
                ComboBox cboData = new ComboBox();
                TextBox txtMS = new TextBox();
                ComboBox cboAlignment = new ComboBox();
                TextBox txtWidth = new TextBox();
                Xceed.Wpf.Toolkit.ColorPicker clpForeground = new Xceed.Wpf.Toolkit.ColorPicker();
                Xceed.Wpf.Toolkit.ColorPicker clpBackground = new Xceed.Wpf.Toolkit.ColorPicker();

                lbl1.Content = "ID";
                lbl2.Content = "Font Family";
                lbl3.Content = "Font Size";
                lbl4.Content = "X";
                lbl5.Content = "Y";
                lbl6.Content = "Z";
                lbl7.Content = "Player";
                lbl8.Content = "Slot Select";
                lbl9.Content = "Data";
                lbl10.Content = "Update Time (ms)";
                lbl11.Content = "Alignment";
                lbl12.Content = "Width";
                lbl13.Content = "Foreground";
                lbl14.Content = "Background";
                txtId.Text = "ID";
                txtFontFamily.Text = "Arial";
                txtFontSize.Text = "12";
                txtX.Text = "0";
                txtY.Text = "0";
                txtZ.Text = "0";
                chkPlayer.IsChecked = true;
                txtSlotSelect.Text = "1";
                txtMS.Text = "1000";
                cboAlignment.Items.Add("Left");
                cboAlignment.Items.Add("Center");
                cboAlignment.Items.Add("Right");
                cboAlignment.SelectedIndex = 0;
                clpForeground.SelectedColor = Color.FromArgb(255, 0, 0, 0);
                clpBackground.SelectedColor = Color.FromArgb(0, 0, 0, 0);

                foreach (string s in Constants.READER_CHARACTER_LABEL)
                    cboData.Items.Add(s);

                chkPlayer.Click += new RoutedEventHandler(delegate (Object o, RoutedEventArgs r) {
                    cboData.Items.Clear();

                    if ((bool) chkPlayer.IsChecked) {
                        foreach (string s in Constants.READER_CHARACTER_LABEL)
                            cboData.Items.Add(s);
                    } else {
                        foreach (string s in Constants.READER_MONSTER_LABEL)
                            cboData.Items.Add(s);
                    }
                });

                cboData.SelectedIndex = 0;

                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());

                SetObject(ref grid, (Control) lbl1, 0, 0);
                SetObject(ref grid, (Control) lbl2, 0, 1);
                SetObject(ref grid, (Control) lbl3, 0, 2);
                SetObject(ref grid, (Control) lbl4, 0, 3);
                SetObject(ref grid, (Control) lbl5, 0, 4);
                SetObject(ref grid, (Control) lbl6, 0, 5);
                SetObject(ref grid, (Control) lbl7, 0, 6);
                SetObject(ref grid, (Control) lbl8, 0, 7);
                SetObject(ref grid, (Control) lbl9, 0, 8);
                SetObject(ref grid, (Control) lbl10, 0, 9);
                SetObject(ref grid, (Control) lbl11, 0, 10);
                SetObject(ref grid, (Control) lbl12, 0, 11);
                SetObject(ref grid, (Control) lbl13, 0, 12);
                SetObject(ref grid, (Control) lbl14, 0, 13);
                SetObject(ref grid, (Control) txtId, 1, 0);
                SetObject(ref grid, (Control) txtFontFamily, 1, 1);
                SetObject(ref grid, (Control) txtFontSize, 1, 2);
                SetObject(ref grid, (Control) txtX, 1, 3);
                SetObject(ref grid, (Control) txtY, 1, 4);
                SetObject(ref grid, (Control) txtZ, 1, 5);
                SetObject(ref grid, (Control) chkPlayer, 1, 6);
                SetObject(ref grid, (Control) txtSlotSelect, 1, 7);
                SetObject(ref grid, (Control) cboData, 1, 8);
                SetObject(ref grid, (Control) txtMS, 1, 9);
                SetObject(ref grid, (Control) cboAlignment, 1, 10);
                SetObject(ref grid, (Control) txtWidth, 1, 11);
                SetObject(ref grid, (Control) clpForeground, 1, 12);
                SetObject(ref grid, (Control) clpBackground, 1, 13);

                changeWindow.AddObject(grid);
                changeWindow.ShowDialog();

                if (readerWindow.IsUnique(txtId.Text)) {
                    if (!readerWindow.AddBattleLabel(txtId.Text, txtFontFamily.Text, txtFontSize.Text, txtX.Text, txtY.Text, txtZ.Text, (bool) chkPlayer.IsChecked, txtSlotSelect.Text, cboData.SelectedItem.ToString(), txtMS.Text, cboAlignment.SelectedIndex, txtWidth.Text, clpForeground, clpBackground))
                        Constants.WritePLogOutput("Something is wrong with one of the inputs.");
                } else {
                    Constants.WritePLogOutput("Input did not have a unique ID.");
                }
            } else if (readerComponents.SelectedIndex == 2) {
                InputWindow changeWindow = new InputWindow("Reader Window Add Progress Bar", false);
                Grid grid = new Grid();
                Label lbl1 = new Label();
                Label lbl2 = new Label();
                Label lbl3 = new Label();
                Label lbl4 = new Label();
                Label lbl5 = new Label();
                Label lbl6 = new Label();
                Label lbl7 = new Label();
                Label lbl8 = new Label();
                Label lbl9 = new Label();
                Label lbl10 = new Label();
                Label lbl11 = new Label();
                Label lbl12 = new Label();
                Label lbl13 = new Label();
                Label lbl14 = new Label();
                TextBox txtId = new TextBox();
                TextBox txtX = new TextBox();
                TextBox txtY = new TextBox();
                TextBox txtZ = new TextBox();
                CheckBox chkPlayer = new CheckBox();
                TextBox txtSlotSelect = new TextBox();
                TextBox txtData = new TextBox();
                TextBox txtMin = new TextBox();
                TextBox txtMax = new TextBox();
                TextBox txtWidth = new TextBox();
                TextBox txtHeight = new TextBox();
                TextBox txtMS = new TextBox();
                Xceed.Wpf.Toolkit.ColorPicker clpForeground = new Xceed.Wpf.Toolkit.ColorPicker();
                Xceed.Wpf.Toolkit.ColorPicker clpBackground = new Xceed.Wpf.Toolkit.ColorPicker();

                lbl1.Content = "ID";
                lbl2.Content = "X";
                lbl3.Content = "Y";
                lbl4.Content = "Z";
                lbl5.Content = "Player";
                lbl6.Content = "Slot Select";
                lbl7.Content = "Value";
                lbl8.Content = "Minimum";
                lbl9.Content = "Maximum";
                lbl10.Content = "Width";
                lbl11.Content = "Height";
                lbl12.Content = "Update Time (ms)";
                lbl13.Content = "Foreground";
                lbl14.Content = "Background";
                txtId.Text = "ID";
                txtX.Text = "0";
                txtY.Text = "0";
                txtZ.Text = "0";
                chkPlayer.IsChecked = true;
                txtSlotSelect.Text = "1";
                txtData.Text = "0";
                txtMin.Text = "0";
                txtMax.Text = "0";
                txtWidth.Text = "200";
                txtHeight.Text = "10";
                txtMS.Text = "1000";
                clpForeground.SelectedColor = Color.FromArgb(255, 0, 0, 0);
                clpBackground.SelectedColor = Color.FromArgb(0, 0, 0, 0);

                chkPlayer.Click += new RoutedEventHandler(delegate (Object o, RoutedEventArgs r) {
                    txtData.Text = "";
                    txtMin.Text = "";
                    txtMax.Text = "";
                });

                lbl7.MouseDoubleClick += new MouseButtonEventHandler(delegate (Object o, MouseButtonEventArgs m) {
                    InputWindow fieldWindow = new InputWindow("Field Select", false);
                    ComboBox cboData = new ComboBox();

                    if ((bool) chkPlayer.IsChecked) {
                        foreach (string s in Constants.READER_CHARACTER_LABEL)
                            cboData.Items.Add(s);
                    } else {
                        foreach (string s in Constants.READER_MONSTER_LABEL)
                            cboData.Items.Add(s);
                    }

                    cboData.SelectedIndex = 0;
                    fieldWindow.AddObject(cboData);
                    fieldWindow.AddTextBlock("Select Field to Read");

                    fieldWindow.ShowDialog();

                    txtData.Text = cboData.SelectedItem.ToString();
                });

                lbl8.MouseDoubleClick += new MouseButtonEventHandler(delegate (Object o, MouseButtonEventArgs m) {
                    InputWindow fieldWindow = new InputWindow("Field Select", false);
                    ComboBox cboData = new ComboBox();

                    if ((bool) chkPlayer.IsChecked) {
                        foreach (string s in Constants.READER_CHARACTER_LABEL)
                            cboData.Items.Add(s);
                    } else {
                        foreach (string s in Constants.READER_MONSTER_LABEL)
                            cboData.Items.Add(s);
                    }

                    cboData.SelectedIndex = 0;
                    fieldWindow.AddObject(cboData);
                    fieldWindow.AddTextBlock("Select Field to Read");

                    fieldWindow.ShowDialog();

                    txtMin.Text = cboData.SelectedItem.ToString();
                });


                lbl9.MouseDoubleClick += new MouseButtonEventHandler(delegate (Object o, MouseButtonEventArgs m) {
                    InputWindow fieldWindow = new InputWindow("Field Select", false);
                    ComboBox cboData = new ComboBox();

                    if ((bool) chkPlayer.IsChecked) {
                        foreach (string s in Constants.READER_CHARACTER_LABEL)
                            cboData.Items.Add(s);
                    } else {
                        foreach (string s in Constants.READER_MONSTER_LABEL)
                            cboData.Items.Add(s);
                    }

                    cboData.SelectedIndex = 0;
                    fieldWindow.AddObject(cboData);
                    fieldWindow.AddTextBlock("Select Field to Read");

                    fieldWindow.ShowDialog();

                    txtMax.Text = cboData.SelectedItem.ToString();
                });

                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());

                SetObject(ref grid, (Control) lbl1, 0, 0);
                SetObject(ref grid, (Control) lbl2, 0, 1);
                SetObject(ref grid, (Control) lbl3, 0, 2);
                SetObject(ref grid, (Control) lbl4, 0, 3);
                SetObject(ref grid, (Control) lbl5, 0, 4);
                SetObject(ref grid, (Control) lbl6, 0, 5);
                SetObject(ref grid, (Control) lbl7, 0, 6);
                SetObject(ref grid, (Control) lbl8, 0, 7);
                SetObject(ref grid, (Control) lbl9, 0, 8);
                SetObject(ref grid, (Control) lbl10, 0, 9);
                SetObject(ref grid, (Control) lbl11, 0, 10);
                SetObject(ref grid, (Control) lbl12, 0, 11);
                SetObject(ref grid, (Control) lbl13, 0, 12);
                SetObject(ref grid, (Control) lbl14, 0, 13);
                SetObject(ref grid, (Control) txtId, 1, 0);
                SetObject(ref grid, (Control) txtX, 1, 1);
                SetObject(ref grid, (Control) txtY, 1, 2);
                SetObject(ref grid, (Control) txtZ, 1, 3);
                SetObject(ref grid, (Control) chkPlayer, 1, 4);
                SetObject(ref grid, (Control) txtSlotSelect, 1, 5);
                SetObject(ref grid, (Control) txtData, 1, 6);
                SetObject(ref grid, (Control) txtMin, 1, 7);
                SetObject(ref grid, (Control) txtMax, 1, 8);
                SetObject(ref grid, (Control) txtWidth, 1, 9);
                SetObject(ref grid, (Control) txtHeight, 1, 10);
                SetObject(ref grid, (Control) txtMS, 1, 11);
                SetObject(ref grid, (Control) clpForeground, 1, 12);
                SetObject(ref grid, (Control) clpBackground, 1, 13);

                changeWindow.AddObject(grid);
                changeWindow.ShowDialog();

                if (readerWindow.IsUnique(txtId.Text)) {
                    if (!readerWindow.AddProgressBar(txtId.Text, txtX.Text, txtY.Text, txtZ.Text, (bool) chkPlayer.IsChecked, txtSlotSelect.Text, txtData.Text, txtMin.Text, txtMax.Text, txtWidth.Text, txtHeight.Text, txtMS.Text, clpForeground, clpBackground))
                        Constants.WritePLogOutput("Something is wrong with one of the inputs.");
                } else {
                    Constants.WritePLogOutput("Input did not have a unique ID.");
                }
            } else if (readerComponents.SelectedIndex == 3) {
                InputWindow changeWindow = new InputWindow("Reader Window Add Radial Bar", false);
                Grid grid = new Grid();
                Label lbl1 = new Label();
                Label lbl2 = new Label();
                Label lbl3 = new Label();
                Label lbl4 = new Label();
                Label lbl5 = new Label();
                Label lbl6 = new Label();
                Label lbl7 = new Label();
                Label lbl8 = new Label();
                Label lbl9 = new Label();
                Label lbl10 = new Label();
                Label lbl11 = new Label();
                Label lbl12 = new Label();
                Label lbl13 = new Label();
                Label lbl14 = new Label();
                Label lbl15 = new Label();
                Label lbl16 = new Label();
                Label lbl17 = new Label();
                Label lbl18 = new Label();
                TextBox txtId = new TextBox();
                TextBox txtX = new TextBox();
                TextBox txtY = new TextBox();
                TextBox txtZ = new TextBox();
                CheckBox chkPlayer = new CheckBox();
                TextBox txtSlotSelect = new TextBox();
                TextBox txtData = new TextBox();
                TextBox txtMin = new TextBox();
                TextBox txtMax = new TextBox();
                TextBox txtSize = new TextBox();
                TextBox txtStroke = new TextBox();
                TextBox txtBStroke = new TextBox();
                ComboBox cboDirection = new ComboBox();
                TextBox txtRenderDegree = new TextBox();
                TextBox txtRotationDegree = new TextBox();
                TextBox txtMS = new TextBox();
                Xceed.Wpf.Toolkit.ColorPicker clpForeground = new Xceed.Wpf.Toolkit.ColorPicker();
                Xceed.Wpf.Toolkit.ColorPicker clpBackground = new Xceed.Wpf.Toolkit.ColorPicker();

                lbl1.Content = "ID";
                lbl2.Content = "X";
                lbl3.Content = "Y";
                lbl4.Content = "Z";
                lbl5.Content = "Player";
                lbl6.Content = "Slot Select";
                lbl7.Content = "Value";
                lbl8.Content = "Minimum";
                lbl9.Content = "Maximum";
                lbl10.Content = "Size";
                lbl11.Content = "Stroke Width";
                lbl12.Content = "Background Width";
                lbl13.Content = "Direction";
                lbl14.Content = "Render Degree";
                lbl15.Content = "Rotation Degree";
                lbl16.Content = "Update Time (ms)";
                lbl17.Content = "Foreground";
                lbl18.Content = "Background";

                txtId.Text = "ID";
                txtX.Text = "0";
                txtY.Text = "0";
                txtZ.Text = "0";
                chkPlayer.IsChecked = true;
                txtSlotSelect.Text = "1";
                txtData.Text = "0";
                txtMin.Text = "0";
                txtMax.Text = "0";
                txtSize.Text = "100";
                txtStroke.Text = "10";
                txtBStroke.Text = "10";
                cboDirection.Items.Add("Counterclockwise");
                cboDirection.Items.Add("Clockwise");
                cboDirection.SelectedIndex = 0;
                txtRenderDegree.Text = "359.999";
                txtRotationDegree.Text = "0";
                txtMS.Text = "1000";
                clpForeground.SelectedColor = Color.FromArgb(255, 0, 0, 0);
                clpBackground.SelectedColor = Color.FromArgb(0, 0, 0, 0);

                chkPlayer.Click += new RoutedEventHandler(delegate (Object o, RoutedEventArgs r) {
                    txtData.Text = "";
                    txtMin.Text = "";
                    txtMax.Text = "";
                });

                lbl7.MouseDoubleClick += new MouseButtonEventHandler(delegate (Object o, MouseButtonEventArgs m) {
                    InputWindow fieldWindow = new InputWindow("Field Select", false);
                    ComboBox cboData = new ComboBox();

                    if ((bool) chkPlayer.IsChecked) {
                        foreach (string s in Constants.READER_CHARACTER_LABEL)
                            cboData.Items.Add(s);
                    } else {
                        foreach (string s in Constants.READER_MONSTER_LABEL)
                            cboData.Items.Add(s);
                    }

                    cboData.SelectedIndex = 0;
                    fieldWindow.AddObject(cboData);
                    fieldWindow.AddTextBlock("Select Field to Read");

                    fieldWindow.ShowDialog();

                    txtData.Text = cboData.SelectedItem.ToString();
                });

                lbl8.MouseDoubleClick += new MouseButtonEventHandler(delegate (Object o, MouseButtonEventArgs m) {
                    InputWindow fieldWindow = new InputWindow("Field Select", false);
                    ComboBox cboData = new ComboBox();

                    if ((bool) chkPlayer.IsChecked) {
                        foreach (string s in Constants.READER_CHARACTER_LABEL)
                            cboData.Items.Add(s);
                    } else {
                        foreach (string s in Constants.READER_MONSTER_LABEL)
                            cboData.Items.Add(s);
                    }

                    cboData.SelectedIndex = 0;
                    fieldWindow.AddObject(cboData);
                    fieldWindow.AddTextBlock("Select Field to Read");

                    fieldWindow.ShowDialog();

                    txtMin.Text = cboData.SelectedItem.ToString();
                });


                lbl9.MouseDoubleClick += new MouseButtonEventHandler(delegate (Object o, MouseButtonEventArgs m) {
                    InputWindow fieldWindow = new InputWindow("Field Select", false);
                    ComboBox cboData = new ComboBox();

                    if ((bool) chkPlayer.IsChecked) {
                        foreach (string s in Constants.READER_CHARACTER_LABEL)
                            cboData.Items.Add(s);
                    } else {
                        foreach (string s in Constants.READER_MONSTER_LABEL)
                            cboData.Items.Add(s);
                    }

                    cboData.SelectedIndex = 0;
                    fieldWindow.AddObject(cboData);
                    fieldWindow.AddTextBlock("Select Field to Read");

                    fieldWindow.ShowDialog();

                    txtMax.Text = cboData.SelectedItem.ToString();
                });

                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());

                SetObject(ref grid, (Control) lbl1, 0, 0);
                SetObject(ref grid, (Control) lbl2, 0, 1);
                SetObject(ref grid, (Control) lbl3, 0, 2);
                SetObject(ref grid, (Control) lbl4, 0, 3);
                SetObject(ref grid, (Control) lbl5, 0, 4);
                SetObject(ref grid, (Control) lbl6, 0, 5);
                SetObject(ref grid, (Control) lbl7, 0, 6);
                SetObject(ref grid, (Control) lbl8, 0, 7);
                SetObject(ref grid, (Control) lbl9, 0, 8);
                SetObject(ref grid, (Control) lbl10, 0, 9);
                SetObject(ref grid, (Control) lbl11, 0, 10);
                SetObject(ref grid, (Control) lbl12, 0, 11);
                SetObject(ref grid, (Control) lbl13, 0, 12);
                SetObject(ref grid, (Control) lbl14, 0, 13);
                SetObject(ref grid, (Control) lbl15, 0, 14);
                SetObject(ref grid, (Control) lbl16, 0, 15);
                SetObject(ref grid, (Control) lbl17, 0, 16);
                SetObject(ref grid, (Control) lbl18, 0, 17);
                SetObject(ref grid, (Control) txtId, 1, 0);
                SetObject(ref grid, (Control) txtX, 1, 1);
                SetObject(ref grid, (Control) txtY, 1, 2);
                SetObject(ref grid, (Control) txtZ, 1, 3);
                SetObject(ref grid, (Control) chkPlayer, 1, 4);
                SetObject(ref grid, (Control) txtSlotSelect, 1, 5);
                SetObject(ref grid, (Control) txtData, 1, 6);
                SetObject(ref grid, (Control) txtMin, 1, 7);
                SetObject(ref grid, (Control) txtMax, 1, 8);
                SetObject(ref grid, (Control) txtSize, 1, 9);
                SetObject(ref grid, (Control) txtStroke, 1, 10);
                SetObject(ref grid, (Control) txtBStroke, 1, 11);
                SetObject(ref grid, (Control) cboDirection, 1, 12);
                SetObject(ref grid, (Control) txtRenderDegree, 1, 13);
                SetObject(ref grid, (Control) txtRotationDegree, 1, 14);
                SetObject(ref grid, (Control) txtMS, 1, 15);
                SetObject(ref grid, (Control) clpForeground, 1, 16);
                SetObject(ref grid, (Control) clpBackground, 1, 17);

                changeWindow.AddObject(grid);
                changeWindow.ShowDialog();

                if (readerWindow.IsUnique(txtId.Text)) {
                    if (!readerWindow.AddRadialBar(txtId.Text, txtX.Text, txtY.Text, txtZ.Text, (bool) chkPlayer.IsChecked, txtSlotSelect.Text, txtData.Text, txtMin.Text, txtMax.Text, txtSize.Text, txtStroke.Text, txtBStroke.Text, cboDirection.SelectedIndex, txtRenderDegree.Text, txtRotationDegree.Text, txtMS.Text, clpForeground, clpBackground))
                        Constants.WritePLogOutput("Something is wrong with one of the inputs.");
                } else {
                    Constants.WritePLogOutput("Input did not have a unique ID.");
                }
            }
        }

        public void ReaderModeChange() {
            InputWindow readerChangeWindow = new InputWindow("Reader Window Change");
            ComboBox type = new ComboBox();
            ComboBox elements = new ComboBox();

            type.Items.Add("Label");
            type.Items.Add("Battle Label");
            type.Items.Add("Progress Bar");
            type.Items.Add("Radial Bar");

            type.SelectionChanged += new SelectionChangedEventHandler(delegate (Object o, SelectionChangedEventArgs s) {
                elements.Items.Clear();
                if (type.SelectedIndex == 0) {
                    foreach (ReaderLabel c in readerWindow.cv.Children.OfType<ReaderLabel>())
                        elements.Items.Add(c.id);
                } else if (type.SelectedIndex == 1) {
                    foreach (ReaderBattleLabel c in readerWindow.cv.Children.OfType<ReaderBattleLabel>())
                        elements.Items.Add(c.id);
                } else if (type.SelectedIndex == 2) {
                    foreach (ReaderProgressBar c in readerWindow.cv.Children.OfType<ReaderProgressBar>())
                        elements.Items.Add(c.id);
                } else if (type.SelectedIndex == 3) {
                    foreach (ReaderRadialBar c in readerWindow.cv.Children.OfType<ReaderRadialBar>())
                        elements.Items.Add(c.id);
                }
                elements.SelectedIndex = 0;
            });

            type.SelectedIndex = 0;

            readerChangeWindow.AddObject(elements);
            readerChangeWindow.AddObject(type);
            readerChangeWindow.AddTextBlock("What element do you want to change?");
            readerChangeWindow.ShowDialog();

            if (elements.Items.Count > 0) {
                if (type.SelectedIndex == 0) {
                    foreach (ReaderLabel c in readerWindow.cv.Children.OfType<ReaderLabel>()) {
                        if (c.id == elements.SelectedItem.ToString())
                            c.Click();
                    }
                } else if (type.SelectedIndex == 1) {
                    foreach (ReaderBattleLabel c in readerWindow.cv.Children.OfType<ReaderBattleLabel>()) {
                        if (c.id == elements.SelectedItem.ToString())
                            c.Click();
                    }
                } else if (type.SelectedIndex == 2) {
                    foreach (ReaderProgressBar c in readerWindow.cv.Children.OfType<ReaderProgressBar>()) {
                        if (c.id == elements.SelectedItem.ToString())
                            c.Click();
                    }
                } else if (type.SelectedIndex == 3) {
                    foreach (ReaderRadialBar c in readerWindow.cv.Children.OfType<ReaderRadialBar>()) {
                        if (c.id == elements.SelectedItem.ToString())
                            c.Click();
                    }
                }
            }
        }

        public void ReaderModeDelete() {
            InputWindow readerChangeWindow = new InputWindow("Reader Window Change");
            ComboBox type = new ComboBox();
            ComboBox elements = new ComboBox();

            type.Items.Add("Label");
            type.Items.Add("Battle Label");
            type.Items.Add("Progress Bar");
            type.Items.Add("Radial Bar");

            type.SelectionChanged += new SelectionChangedEventHandler(delegate (Object o, SelectionChangedEventArgs s) {
                elements.Items.Clear();
                if (type.SelectedIndex == 0) {
                    foreach (ReaderLabel c in readerWindow.cv.Children.OfType<ReaderLabel>())
                        elements.Items.Add(c.id);
                } else if (type.SelectedIndex == 1) {
                    foreach (ReaderBattleLabel c in readerWindow.cv.Children.OfType<ReaderBattleLabel>())
                        elements.Items.Add(c.id);
                } else if (type.SelectedIndex == 2) {
                    foreach (ReaderProgressBar c in readerWindow.cv.Children.OfType<ReaderProgressBar>())
                        elements.Items.Add(c.id);
                } else if (type.SelectedIndex == 3) {
                    foreach (ReaderRadialBar c in readerWindow.cv.Children.OfType<ReaderRadialBar>())
                        elements.Items.Add(c.id);
                }
                elements.SelectedIndex = 0;
            });

            type.SelectedIndex = 0;

            readerChangeWindow.AddObject(elements);
            readerChangeWindow.AddObject(type);
            readerChangeWindow.AddTextBlock("What element do you want to delete?");
            readerChangeWindow.ShowDialog();

            //???
            try {
                if (elements.Items.Count > 0) {
                    if (type.SelectedIndex == 0) {
                        foreach (ReaderLabel c in readerWindow.cv.Children.OfType<ReaderLabel>()) {
                            if (c.id == elements.SelectedItem.ToString())
                                readerWindow.cv.Children.Remove(c);
                        }
                    } else if (type.SelectedIndex == 1) {
                        foreach (ReaderBattleLabel c in readerWindow.cv.Children.OfType<ReaderBattleLabel>()) {
                            if (c.id == elements.SelectedItem.ToString())
                                readerWindow.cv.Children.Remove(c);
                        }
                    } else if (type.SelectedIndex == 2) {
                        foreach (ReaderProgressBar c in readerWindow.cv.Children.OfType<ReaderProgressBar>()) {
                            if (c.id == elements.SelectedItem.ToString())
                                readerWindow.cv.Children.Remove(c);
                        }
                    } else if (type.SelectedIndex == 3) {
                        foreach (ReaderRadialBar c in readerWindow.cv.Children.OfType<ReaderRadialBar>()) {
                            if (c.id == elements.SelectedItem.ToString())
                                readerWindow.cv.Children.Remove(c);
                        }
                    }
                }
            } catch (Exception e) { }
        }

        public void ReaderModeSave() {
            InputWindow openReaderSave = new InputWindow("Save Profile");
            TextBox txt = new TextBox();
            openReaderSave.AddObject(txt);
            openReaderSave.AddTextBlock("What do you want to call your reader profile?");
            openReaderSave.ShowDialog();

            if (!txt.Text.Equals(""))
                if (readerWindow.IsOpen)
                    readerWindow.Save(txt.Text);
                else
                    Constants.WriteOutput("Reader Window is not open.");
            else
                Constants.WriteOutput("Nothing was input.");
        }

        public void ReaderModeLoad() {
            InputWindow openReaderLoad = new InputWindow("Load Profile");
            TextBox txt = new TextBox();
            openReaderLoad.AddObject(txt);
            openReaderLoad.AddTextBlock("What profile do you want to load?");
            openReaderLoad.ShowDialog();

            if (!txt.Text.Equals(""))
                if (readerWindow.IsOpen)
                    readerWindow.Load(txt.Text);
                else
                    Constants.WriteOutput("Reader Window is not open.");
            else
                Constants.WriteOutput("Nothing was input.");
        }

        public void ReaderModeReset() {
            if (readerWindow.IsOpen)
                readerWindow.Reset();
            else
                Constants.WriteOutput("Reader Window is not open.");
        }

        public static void SetObject(ref Grid grid, Control obj, int x, int y) {
            Grid.SetColumn(obj, x);
            Grid.SetRow(obj, y);
            grid.Children.Add(obj);
        }

        public void ReaderRemoveUI() {
            if (Emulator.Memory.GameState == GameState.Battle && Controller.Main.StatsChanged) {
                if (uiCombo["cboReaderUIRemoval"] == 1) {
                    Emulator.WriteByte(Globals.M_POINT + 0x1775, 2);
                    Emulator.WriteByte(Globals.M_POINT + 0x18B9, 2);
                    Emulator.WriteByte(Globals.M_POINT + 0x19FD, 2);
                } else if (uiCombo["cboReaderUIRemoval"] == 2) {
                    Emulator.WriteShort("UI_Y", 270);
                }
            }
        }

        public void ReaderAutoHotkey() {
            if (Emulator.Memory.GameState == GameState.Battle && Controller.Main.StatsChanged) {
                bool update = false;
                int shanaSlot = -1;
                int[] actions = new int[3];
                for (int i = 0; i < 3; i++) {
                    if (Globals.PARTY_SLOT[i] < 9) {
                        actions[i] = Emulator.Battle.CharacterTable[i].Action;
                        if (actions[i] == 8 || actions[i] == 10 || actions[i] == 136) {
                            if (Emulator.Battle.CharacterTable[i].Menu >= 15 && Emulator.Battle.CharacterTable[i].Menu < 250) {
                                update = true;
                            }
                        }
                        if (Globals.PARTY_SLOT[i] == 2 || Globals.PARTY_SLOT[i] == 8) {
                            shanaSlot = i;
                        }
                    }
                }

                if (actions[0] == 16 || actions[1] == 16 || actions[2] == 16) {
                    update = false;
                }

                if (shanaSlot > -1) {
                    int otherSlot1 = shanaSlot == 0 ? 1 : 0;
                    int otherSlot2 = shanaSlot == 2 ? 1 : 2;

                    if (actions[shanaSlot] == 136 && actions[otherSlot1] == 0 && actions[otherSlot2] == 0) {
                        update = false;
                    }
                }

                for (int i = 0; i < 3; i++) {
                    if (Globals.PARTY_SLOT[i] < 9) {
                        if ((actions[i] == 8 || actions[i] == 10) && Emulator.Battle.CharacterTable[i].Menu == 0 && !partySecondMenu) {
                            update = true;
                            partySecondMenu = true;
                        }
                    }
                }

                if (partySecondMenu) {
                    partySecondMenu = false;
                    for (int i = 0; i < 3; i++) {
                        if (Globals.PARTY_SLOT[i] < 9) {
                            if ((actions[i] == 8 || actions[i] == 10) && Emulator.Battle.CharacterTable[i].Menu == 0 && !partySecondMenu) {
                                update = true;
                                partySecondMenu = true;
                            }
                        }
                    }
                }

                if (update != partyMenu) {
                    partyMenu = update;

                    this.Dispatcher.BeginInvoke(new Action(() => {
                        if (partyMenu) {
                            System.Windows.Forms.SendKeys.SendWait("{" + cboReaderOnHotkey.SelectedValue + "}");
                        } else {
                            System.Windows.Forms.SendKeys.SendWait("{" + cboReaderOffHotkey.SelectedValue + "}");
                        }
                    }), DispatcherPriority.ContextIdle);
                }

                readerModeOnBattleExit = true;
            } else {
                if (readerModeOnBattleExit) {
                    this.Dispatcher.BeginInvoke(new Action(() => {
                        System.Windows.Forms.SendKeys.SendWait("{" + cboReaderFieldHotkey.SelectedValue + "}");
                        readerModeOnBattleExit = false;
                    }), DispatcherPriority.ContextIdle);
                }
            }
        }
        #endregion
        #endregion

        #region On Close
        private void Window_Closing(object sender, CancelEventArgs e) {
            //readerWindow.Close();
            CloseEmulator();
        }

        private void Window_Closed(object sender, EventArgs e) {
            System.Windows.Application.Current.Shutdown();
        }

        public void CloseEmulator() {
            Constants.RUN = false;
            Thread.Sleep(2000);
            SaveKey();
            SaveSubKey();
        }
        #endregion
    }
}