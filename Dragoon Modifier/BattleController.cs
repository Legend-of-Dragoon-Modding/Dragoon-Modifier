using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using CSScriptLibrary;
using System.Windows.Forms;

namespace Dragoon_Modifier {
    class BattleController {
        static byte[] sharanda = new byte[] { 0x2, 0x8 };
        static short[] slot1FinalBlow = new short[] { 414, 408, 409, 392, 431 };      // Urobolus, Wounded Virage, Complete Virage, Lloyd, Zackwell
        static short[] slot2FinalBlow = new short[] { 387, 403 };                     // Fruegel II, Gehrich

        static bool firstDamageCapRemoval = false;
        static int lastItemUsedDamageCap = 0;

        static bool dartSwitcheroo = false;

        static string difficulty = "Normal";
        static int aspectRatioOption = 0;
        static int cameraOption = 0;
        static int killBGM = 0;

        static int bossSPLoss = 0;
        static int ultimateBossStage = 0;

        //Chapter 3 Elemental Buffs
        static int sparkleArrowBuff = 8;
        static int heatBladeBuff = 7;
        static int shadowCutterBuff = 9;
        static int morningStarBuff = -20;

        // Equipment variables
        static byte spiritEaterSlot = 0;
        static bool spiritEaterCheck = false;
        static byte harpoonSlot = 0;
        static bool harpoonCheck = false;
        static byte elementArrowSlot = 0;
        static byte elementArrowElement = 0x80;
        static byte elementArrowItem = 0xC3;
        static byte elementArrowLastAction = 255;
        static byte elementArrowTurns = 0;
        static byte dragonBeaterSlot = 0;
        static byte batteryGloveSlot = 0;
        static byte batteryGloveLastAction = 0;
        static byte batteryGloveCharge = 0;
        static byte jeweledHammerSlot = 0;
        static byte giantAxeSlot = 0;
        static byte giantAxeLastAction = 0;
        static byte fakeLegendCasqueSlot = 0;
        static bool[] fakeLegendCasqueCheck = { false, false, false };
        static byte fakeLegendArmorSlot = 0;
        static bool[] fakeLegendArmorCheck = { false, false, false };
        static byte legendCasqueSlot = 0;
        static byte[] legendCasqueCount = { 0, 0, 0 };
        static bool[] legendCasqueCheckMDF = { false, false, false };
        static bool[] legendCasqueCheckShield = { false, false, false };
        static byte armorOfLegendSlot = 0;
        static byte[] armorOfLegendCount = { 0, 0, 0 };
        static bool[] armorOfLegendCheckDF = { false, false, false };
        static bool[] armorOfLegendCheckShield = { false, false, false };
        static byte soasSiphonRingSlot = 0;
        static byte soasAnkhSlot = 0;

        // Dragoon Changes variables
        static ushort[] previousMP = { 0, 0, 0 };
        static ushort[] currentMP = { 0, 0, 0 };
        static ushort[] trackMonsterHP = { 0, 0, 0, 0, 0 };
        static byte flowerStorm = 2;
        static bool checkWingBlaster = false;
        static bool checkGaspless = false;
        static bool checkJadeDragon = false;
        static bool checkFlowerStorm = false;
        static byte starChildren = 0;
        static short recoveryRateSave = 0;
        static bool checkRoseDamage = false;
        static ushort checkRoseDamageSave = 0;
        static bool roseEnhanceDragoon = false;
        static bool meruEnhanceDragoon = false;
        static bool trackRainbowBreath = false;

        #region Dragoon Stats

        #region Burn Stacks
        static int dartBurnStacks = 0;
        static int dartPreviousBurnStacks = 0;
        static int burnStackFlameshot = 1;
        static int burnStackExplosion = 2;
        static int burnStackFinalBurst = 3;
        static int burnStackRedEye = 4;
        static int dartMaxBurnStacks = 12;
        static int[] dartPreviousAction = { 0, 0, 0 };
        static int[] dartBurnMP = { 0, 0, 0 };
        static double damagePerBurn = 0.1;
        static double burnStackAdditionMulti = 1.4;
        static double burnStackFlameshotMulti = 1.5;
        static double burnStackExplosionMulti = 1.5;
        static double burnStackFinalBurstMulti = 1.33;
        static double burnStackRedEyeMulti = 0.75;
        static double burnStackMaxAdditionMulti = 1.6;
        static double burnStackMaxFlameshotMulti = 2.0;
        static double burnStackMaxExplosionMulti = 2.25;
        static double burnStackMaxFinalBurstMulti = 1.33;
        static double burnStackMaxRedEyeMulti = 0.75;
        static bool burnStackSelected = false;
        static bool resetBurnStack = false;
        static bool dartBurnMPHeal = false;
        #endregion

        static ushort dartDAT = 281;
        static ushort dartSpecialDAT = 422;
        static ushort dartDivineDAT = 204;
        static ushort dartDivineSpecialDAT = 306;
        static ushort dartDREDAT = 408;
        static ushort dartDRESpecialDAT = 612;

        static ushort flameshotDMAT = 255;
        static ushort explosionDMAT = 340;
        static ushort finalBurstDMAT = 255;
        static ushort redEyeDMAT = 340;
        static ushort divineDMAT = 255;
        static ushort explosionDREDMAT = 1020;
        static ushort finalBurstDREMAT = 510;

        static ushort lavitzDAT = 400;
        static ushort lavitzSpecialDAT = 600;

        static ushort wingBlasterDMAT = 440;
        static byte blossomStormTurnMP = 20;
        static ushort gasplessDMAT = 330;
        static ushort jadeDragonDMAT = 440;
        static byte wingBlasterTPDamage = 30;
        static byte gasplessTPDamage = 90;
        static byte jadeDragonTPDamage = 60;

        static ushort shanaDAT = 365;
        static ushort shanaSpecialDAT = 510;
        static ushort shanaSaveAT = 0;

        static byte moonLightMP = 20;
        static ushort starChildrenDMAT = 332;
        static byte gatesOfHeavenMP = 40;
        static ushort wSilverDragonDMAT = 289;
        static int gatesOfHeavenHeal = 50;
        static int gatesOfHeavenHealBase = 50;
        static int gatesOfHeavenHellModePenalty = 15;
        static int gatesOfHeavenHardModePenalty = 8;

        static ushort roseDAT = 330;
        static ushort roseSpecialDAT = 495;

        static ushort astralDrainDMAT = 295;
        static byte astralDrainMP = 10;
        static ushort deathDimensionDMAT = 395;
        static byte deathDimensionMP = 20;
        static ushort darkDragonDMAT = 420;
        static byte darkDragonMP = 80;
        static ushort enhancedAstralDrainDMAT = 410;
        static byte enhancedAstralDrainMP = 25;
        static ushort enhancedDeathDimensionDMAT = 790;
        static byte enhancedDeathDimensionMP = 50;
        static ushort enhancedDarkDragonDMAT = 290;
        static byte enhancedDarkDragonMP = 100;

        static ushort haschelDAT = 281;
        static ushort haschelSpecialDAT = 422;

        static ushort atomicMindDMAT = 330;
        static ushort thunderKidDMAT = 330;
        static ushort thunderGodDMAT = 330;
        static ushort violetDragonDMAT = 374;

        static ushort meruDAT = 330;
        static ushort meruSpecialDAT = 495;

        static ushort freezingRingDMAT = 255;
        static byte freezingRingMP = 10;
        static byte rainbowBreathMP = 30;
        static ushort diamondDustDMAT = 264;
        static byte diamonDustMP = 30;
        static ushort blueSeaDragonDMAT = 350;
        static byte blueSeaDragonMP = 80;
        static ushort enhancedFreezingRingDMAT = 400;
        static byte enhancedFreezingRingMP = 50;
        static byte enhancedRainbowBreathMP = 100;
        static ushort enhancedDiamondDustDMAT = 440;
        static byte enhancedDiamondDustMP = 100;
        static ushort enhancedBlueSeaDragonDMAT = 525;
        static byte enhancedBlueSeaDragonMP = 150;

        static ushort kongolDAT = 500;
        static ushort kongolSpecialDAT = 600;

        static ushort grandStreamDMAT = 450;
        static ushort meteorStrike = 560;
        static ushort goldDragon = 740;

        #endregion

        public static void Run(Dictionary<string, int> uiCombo, byte eleBombTurns, byte eleBombElement, bool reverseDBS, int inventorySize) {
            while (Constants.RUN) {
                try {
                    switch (Globals.GAME_STATE) {
                        case Globals.GameStateEnum.Battle:
                            if (!Globals.STATS_CHANGED) {
                                ultimateBossStage = uiCombo["cboUltimateBoss"];
                                Setup(uiCombo);
                                break;
                            }
                            if (Globals.PARTY_SLOT[0] == 4 && Emulator.ReadByte("HASCHEL_FIX" + Globals.DISC) != 0x80) {
                                MemoryController.Battle.NoDart.HaschelFix(Globals.DISC);
                            }
                            if (slot1FinalBlow.Contains(Globals.ENCOUNTER_ID) && sharanda.Contains(Globals.PARTY_SLOT[0])) {
                                ShanaFix(0);
                            }
                            if (slot2FinalBlow.Contains(Globals.ENCOUNTER_ID) && sharanda.Contains(Globals.PARTY_SLOT[1])) {
                                ShanaFix(1);
                            }
                            if (Globals.ADDITION_SWAP) {
                                AdditionSwapInit();
                            }
                            if (Globals.CheckDMScript("btnAdditionLevel")) {
                                AdditionLevelUp();
                            }
                            if (Globals.CheckDMScript("btnNeverGuard")) {
                                NeverGuard();
                            }
                            if (Globals.CheckDMScript("btnRemoveCaps")) {
                                RemoveDamageCap();
                            }
                            if (difficulty != "Normal") {
                                HardMode.EquipChangesRun(inventorySize);
                                if (!Globals.CheckDMScript("btnDivineRed")) {
                                    DartBurnStackHandler();
                                }
                                //MagicInventoryHandler();
                                HardMode.BattleDragoonRun((ushort)Globals.ENCOUNTER_ID, reverseDBS, eleBombTurns, eleBombElement);
                            }
                            BattleHotkeys();
                            break;
                        case Globals.GameStateEnum.BattleResult:
                            if (Globals.STATS_CHANGED) {
                                Globals.EXITING_BATTLE = 2;
                                ReduceSP();
                                ItemFieldChanges();
                                CharacterFieldChanges();
                                if (Globals.DIFFICULTY_MODE != "Normal") {
                                    HardMode.PostBattleChapter3Buffs();
                                }
                                Globals.STATS_CHANGED = false;
                            }
                            break;
                        case Globals.GameStateEnum.Field:
                            if (Globals.STATS_CHANGED) {
                                Globals.EXITING_BATTLE = 2;
                                ItemFieldChanges();
                                CharacterFieldChanges();
                                if (Globals.DIFFICULTY_MODE != "Normal") {
                                    HardMode.PostBattleChapter3Buffs();
                                }
                                Globals.STATS_CHANGED = false;
                            }
                            if (Globals.PARTY_SLOT[2] < 9 && Globals.PARTY_SLOT[0] != 0) {
                                Globals.NO_DART = Globals.PARTY_SLOT[0];
                                Emulator.WriteByte("PARTY_SLOT", 0);
                                dartSwitcheroo = true;
                            }
                            break;
                        case Globals.GameStateEnum.Menu:
                            if (Globals.STATS_CHANGED) {
                                Globals.EXITING_BATTLE = 2;
                                ItemFieldChanges();
                                CharacterFieldChanges();
                                if (Globals.DIFFICULTY_MODE != "Normal") {
                                    HardMode.PostBattleChapter3Buffs();
                                }
                                Globals.STATS_CHANGED = false;
                            }
                            if (Globals.NO_DART != null) {
                                Emulator.WriteByte("MENU_UNLOCK", 1);
                                if (dartSwitcheroo) {
                                    Emulator.WriteByte("PARTY_SLOT", (byte) Globals.NO_DART);
                                    dartSwitcheroo = false;
                                    Thread.Sleep(200);
                                }
                                Globals.NO_DART = Globals.PARTY_SLOT[0];
                            }
                            break;
                    }
                    Thread.Sleep(250);
                } catch (Exception ex) {
                    Constants.RUN = false;
                    Constants.WriteGLog("Program stopped.");
                    Constants.WritePLogOutput("INTERNAL BATTLE CONTROLLER SCRIPT ERROR");
                    Constants.WriteOutput("Fatal Error. Closing all threads.");
                    Constants.WriteError(ex.ToString());
                }
            }
        }

        public static void Setup(Dictionary<string, int> uiCombo) {
            Constants.WriteOutput("Battle detected. Loading...");

            
            difficulty = Globals.DIFFICULTY_MODE;
            aspectRatioOption = uiCombo["cboAspectRatio"];
            cameraOption = uiCombo["cboCamera"];
            killBGM = uiCombo["cboKillBGM"];
            if (Globals.CheckDMScript("btnAspectRatio")) {
                ChangeAspectRatio();
            }
            if (Globals.CheckDMScript("btnKillBGM") && killBGM != 0) {
                KillBGM();
            }
            if (difficulty == "NormalHard" || difficulty == "HardHell") {
                SwitchDualDifficulty();
            }
            if (Globals.DIFFICULTY_MODE.Contains("Hell")) {
                SetSPLossAmmount();
            } else {
                bossSPLoss = 0;
            }

            long cmtable = Emulator.ReadUInt24("C_POINT", -0x18); // Base address in the Battle Pointer Table
            while (cmtable == Globals.MemoryController.CharacterPoint || cmtable == Globals.MemoryController.MonsterPoint) { // Wait until both C_Point and M_Point were set
                if (Globals.GAME_STATE != Globals.GameStateEnum.Battle) { // No longer in battle
                    return;
                }
                Thread.Sleep(50);
            }

            Globals.SetM_POINT((int)Globals.MemoryController.MonsterPoint + 0x108);
            Globals.SetC_POINT((int) Globals.MemoryController.CharacterPoint + 0x108);

            Globals.BattleController = new Battle.Battle();

            Globals.MONSTER_SIZE = Emulator.ReadByte("MONSTER_SIZE");
            Globals.UNIQUE_MONSTER_SIZE = Emulator.ReadByte("UNIQUE_MONSTER_SIZE");
            Globals.UNIQUE_MONSTER_IDS = new List<int>();
            Globals.MONSTER_IDS = new List<int>();
            Globals.MONSTER_TABLE = new List<dynamic>();
            Globals.CHARACTER_TABLE = new List<MemoryController.CharacterAddress>();

            for (int monster = 0; monster < Globals.UNIQUE_MONSTER_SIZE; monster++) {
                Globals.UNIQUE_MONSTER_IDS.Add(Emulator.ReadUShort("UNIQUE_SLOT", (monster * 0x1A8)));
            }
            for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                Globals.MONSTER_IDS.Add(Emulator.ReadUShort("MONSTER_ID", Globals.BattleController.BattleOffset + (i * 0x8)));
            }
            for (int monster = 0; monster < Globals.MONSTER_SIZE; monster++) {
                Globals.MONSTER_TABLE.Add(new MonsterAddress(Globals.M_POINT, monster, Globals.MONSTER_IDS[monster], Globals.UNIQUE_MONSTER_IDS));
            }
            for (int character = 0; character < 3; character++) {
                if (Globals.PARTY_SLOT[character] > 9) {
                    break;
                }
                Globals.CHARACTER_TABLE.Add(new MemoryController.CharacterAddress(Globals.MemoryController.CharacterPoint, character, Globals.MONSTER_SIZE + character));
            }

            firstDamageCapRemoval = false;
            lastItemUsedDamageCap = 0;

            Constants.WriteDebug("Monster Size:        " + Globals.MONSTER_SIZE);
            Constants.WriteDebug("Unique Monsters:     " + Globals.UNIQUE_MONSTER_SIZE);
            Constants.WriteDebug("Monster Point:       " + Convert.ToString(Globals.M_POINT + Constants.OFFSET, 16).ToUpper());
            Constants.WriteDebug("Character Point:     " + Convert.ToString(Globals.C_POINT + Constants.OFFSET, 16).ToUpper());
            Constants.WriteDebug("Monster IDs:         " + String.Join(", ", Globals.MONSTER_IDS.ToArray()));
            Constants.WriteDebug("Unique Monster IDs:  " + String.Join(", ", Globals.UNIQUE_MONSTER_IDS.ToArray()));
            // in battle model pointers
            Constants.WriteDebug("Slot1 Address:       " + Convert.ToString(Constants.OFFSET + Globals.BattleController.BattleOffset + 0x1D95F4, 16).ToUpper());
            Constants.WriteDebug("Slot2 Address:       " + Convert.ToString(Constants.OFFSET + Globals.BattleController.BattleOffset + 0x1DA88C, 16).ToUpper());
            Constants.WriteDebug("Slot3 Address:       " + Convert.ToString(Constants.OFFSET + Globals.BattleController.BattleOffset + 0x1DBB24, 16).ToUpper());

            Constants.WriteDebug("Addition Table:      " + Convert.ToString(Constants.GetAddress("ADDITION") + Globals.BattleController.BattleOffset + Constants.OFFSET, 16).ToUpper());

            MonsterChanges();
            CharacterBattleChanges();
            if (Globals.NO_DART > 0) {
                Constants.WriteOutput("Finished loading. Waiting for No Dart to complete...");
                MemoryController.Battle.NoDart.Initialize((byte)Globals.NO_DART);
            }
            if (Globals.CheckDMScript("btnBlackRoom")) {
                BlackRoomBattle();
            }
            if (Globals.CheckDMScript("btnNoDragoon")) {
                NoDragoonMode();
            }
            if (difficulty != "Normal") {
                HardMode.Setup(uiCombo, difficulty);
            }

            for (int i = 0; i < 3; i++) {
                if (Globals.PARTY_SLOT[i] == 0) {
                    int dlv = Globals.CHARACTER_TABLE[i].DLV;
                    dartMaxBurnStacks = dlv == 1 ? 3 : dlv == 2 ? 6 : dlv == 3 ? 9 : 12;
                }
                dartPreviousAction[i] = 0;
            }

            Constants.WriteOutput("Finished loading.");
            Globals.STATS_CHANGED = true;
        }

        #region Battle Changes

        #region Monster Changes
        public static void MonsterChanges() {
            if (Globals.MONSTER_STAT_CHANGE && !Globals.CheckDMScript("btnUltimateBoss")) {
                Constants.WriteOutput("Changing Monster Stats...");
                for (int slot = 0; slot < Globals.MONSTER_SIZE; slot++) {
                    MonsterStatChange(slot);
                }
            }

            if (Globals.MONSTER_DROP_CHANGE && !Globals.CheckDMScript("btnUltimateBoss")) {
                Constants.WriteOutput("Changing Monster Drops...");
                for (int slot = 0; slot < Globals.UNIQUE_MONSTER_SIZE; slot++) {
                    MonsterDropChange(slot);
                }
            }

            if (Globals.MONSTER_EXPGOLD_CHANGE && !Globals.CheckDMScript("btnUltimateBoss")) {
                Constants.WriteOutput("Changing Monster Exp and Gold Rewards...");
                for (int slot = 0; slot < Globals.UNIQUE_MONSTER_SIZE; slot++) {
                    MonsterExpGoldChange(slot);
                }
            }
        }

        public static void MonsterStatChange(int slot) {
            int ID = Globals.MONSTER_IDS[slot];
            double HP = Globals.DICTIONARY.StatList[ID].HP * Globals.HP_MULTI;
            double resup = 1;
            if (HP > 65535) {
                resup = HP / 65535;
                HP = 65535;
            }
            Globals.MONSTER_TABLE[slot].Write("HP", (ushort) Math.Round(HP));
            Globals.MONSTER_TABLE[slot].Write("Max_HP", (ushort) Math.Round(HP));
            Globals.MONSTER_TABLE[slot].Write("AT", (short) Math.Round(Globals.DICTIONARY.StatList[ID].AT * Globals.AT_MULTI));
            Globals.MONSTER_TABLE[slot].Write("OG_AT", (short) Math.Round(Globals.DICTIONARY.StatList[ID].AT * Globals.AT_MULTI));
            Globals.MONSTER_TABLE[slot].Write("MAT", (short) Math.Round(Globals.DICTIONARY.StatList[ID].MAT * Globals.MAT_MULTI));
            Globals.MONSTER_TABLE[slot].Write("OG_MAT", (short) Math.Round(Globals.DICTIONARY.StatList[ID].MAT * Globals.MAT_MULTI));
            Globals.MONSTER_TABLE[slot].Write("DF", (short) Math.Round(Globals.DICTIONARY.StatList[ID].DF * Globals.DF_MULTI * resup));
            Globals.MONSTER_TABLE[slot].Write("OG_DF", (short) Math.Round(Globals.DICTIONARY.StatList[ID].DF * Globals.DF_MULTI * resup));
            Globals.MONSTER_TABLE[slot].Write("MDF", (short) Math.Round(Globals.DICTIONARY.StatList[ID].MDF * Globals.MDF_MULTI * resup));
            Globals.MONSTER_TABLE[slot].Write("OG_MDF", (short) Math.Round(Globals.DICTIONARY.StatList[ID].MDF * Globals.MDF_MULTI * resup));
            Globals.MONSTER_TABLE[slot].Write("SPD", (short) Math.Round(Globals.DICTIONARY.StatList[ID].SPD * Globals.SPD_MULTI));
            Globals.MONSTER_TABLE[slot].Write("OG_SPD", (short) Math.Round(Globals.DICTIONARY.StatList[ID].SPD * Globals.SPD_MULTI));
            Globals.MONSTER_TABLE[slot].Write("A_AV", Globals.DICTIONARY.StatList[ID].A_AV);
            Globals.MONSTER_TABLE[slot].Write("M_AV", Globals.DICTIONARY.StatList[ID].M_AV);
            Globals.MONSTER_TABLE[slot].Write("P_Immune", Globals.DICTIONARY.StatList[ID].P_Immune);
            Globals.MONSTER_TABLE[slot].Write("M_Immune", Globals.DICTIONARY.StatList[ID].M_Immune);
            Globals.MONSTER_TABLE[slot].Write("P_Half", Globals.DICTIONARY.StatList[ID].P_Half);
            Globals.MONSTER_TABLE[slot].Write("M_Half", Globals.DICTIONARY.StatList[ID].M_Half);
            Globals.MONSTER_TABLE[slot].Write("Element", Globals.DICTIONARY.StatList[ID].Element);
            Globals.MONSTER_TABLE[slot].Write("E_Immune", Globals.DICTIONARY.StatList[ID].E_Immune);
            Globals.MONSTER_TABLE[slot].Write("E_Half", Globals.DICTIONARY.StatList[ID].E_Half);
            Globals.MONSTER_TABLE[slot].Write("Stat_Res", Globals.DICTIONARY.StatList[ID].Stat_Res);
            Globals.MONSTER_TABLE[slot].Write("Death_Res", Globals.DICTIONARY.StatList[ID].Death_Res);
        }

        public static void MonsterDropChange(int slot) {
            int ID = Globals.UNIQUE_MONSTER_IDS[slot];
            Emulator.WriteByte("MONSTER_REWARDS", (byte) Globals.DICTIONARY.StatList[ID].Drop_Chance, 0x4 + slot * 0x1A8);
            Emulator.WriteByte("MONSTER_REWARDS", (byte) Globals.DICTIONARY.StatList[ID].Drop_Item, 0x5 + slot * 0x1A8);
        }

        public static void MonsterExpGoldChange(int slot) {
            int ID = Globals.UNIQUE_MONSTER_IDS[slot];
            Emulator.WriteUShort("MONSTER_REWARDS", (ushort) Globals.DICTIONARY.StatList[ID].EXP, slot * 0x1A8);
            Emulator.WriteUShort("MONSTER_REWARDS", (ushort) Globals.DICTIONARY.StatList[ID].Gold, 0x2 + slot * 0x1A8);
        }

        #endregion

        #region Character Changes
        public static void CharacterBattleChanges() {
            if (Globals.PARTY_SLOT[1] == Globals.NO_DART || Globals.PARTY_SLOT[2] == Globals.NO_DART) { // Do not swap, if it were to duplicate a character
                Globals.NO_DART = 0;
            }
            if (Globals.PARTY_SLOT[2] > 8) { // Do not swap, if there are not 3 characters
                Globals.NO_DART = 0;
            }

            for (int slot = 0; slot < 3; slot++) { // Current Stats are only used for Max HP. It should probably we reworked and removed
                int character = Globals.PARTY_SLOT[slot];
                if (slot == 0 && Globals.NO_DART != null) {
                    character = (int) Globals.NO_DART;
                }
                if (character > 8) {
                    break;
                }
                Globals.CURRENT_STATS[slot] = new CurrentStats(character, slot, ultimateBossStage);
            }

            if (Globals.ADDITION_CHANGE) {
                Constants.WriteOutput("Changing Additions...");
                for (int slot = 0; slot < 3; slot++) {
                    int character = Globals.PARTY_SLOT[slot];
                    if (slot == 0 && Globals.NO_DART != null) {
                        character = (int) Globals.NO_DART;
                    }
                    if (character > 8) {
                        break;
                    }
                    AdditionsBattleChanges(slot, character);
                }
            }

            if (Globals.ITEM_STAT_CHANGE || Globals.CHARACTER_STAT_CHANGE || Globals.CheckDMScript("btnUltimateBoss")) {
                Constants.WriteOutput("Changing Character Stats...");
                for (int slot = 0; slot < 3; slot++) {
                    int character = Globals.PARTY_SLOT[slot];
                    if (slot == 0 && Globals.NO_DART != null) {
                        character = (int) Globals.NO_DART;
                    }
                    if (character > 8) {
                        break;
                    }
                    SetCharacterStats(slot, character);
                }

                if (Globals.ITEM_STAT_CHANGE) {
                    ItemBattleNameDescChange();
                }
            }

            if (Globals.DRAGOON_STAT_CHANGE) {
                Constants.WriteOutput("Changing Dragoon Stats...");
                for (int slot = 0; slot < 3; slot++) {
                    int character = Globals.PARTY_SLOT[slot];
                    if (slot == 0 && Globals.NO_DART != null) {
                        character = (int) Globals.NO_DART;
                    }
                    if (character > 8) {
                        break;
                    }
                    DragoonStatChanges(slot, character);
                }
            }

            if (Globals.DRAGOON_ADDITION_CHANGE) {
                Constants.WriteOutput("Changing Dragoon Additions...");
                for (int slot = 0; slot < 3; slot++) {
                    int character = Globals.PARTY_SLOT[slot];
                    if (slot == 0 && Globals.NO_DART != null) {
                        character = (int) Globals.NO_DART;
                    }
                    if (character > 8) {
                        break;
                    }
                    DragoonAdditionsBattleChanges(slot, character);
                }
            }

            if (Globals.DRAGOON_SPELL_CHANGE) {
                Constants.WriteOutput("Changing Dragoon Spells...");
                for (int slot = 0; slot < 3; slot++) {
                    int character = Globals.PARTY_SLOT[slot];
                    if (slot == 0 && Globals.NO_DART != null) {
                        character = (int) Globals.NO_DART;
                    }
                    if (character > 8) {
                        break;
                    }
                    DragoonSpellChange(slot, character);
                }
            }

            if (Globals.DRAGOON_DESC_CHANGE) {
                Constants.WriteOutput("Changing Dragoon Spell Descriptions...");
                DragoonSpellDescriptionChange();
            }


            
        }

        public static void AdditionsBattleChanges(int slot, int character) {
            long address = Constants.GetAddress("ADDITION") + Globals.BattleController.BattleOffset;
            Dictionary<int, int> additionnum = new Dictionary<int, int> {
            {0, 0},{1, 1},{2, 2},{3, 3},{4, 4},{5, 5},{6, 6},//Dart
            {8, 0},{9, 1},{10, 2},{11, 3},{12, 4},           //Lavitz
            {14, 0},{15, 1},{16, 2},{17, 3},                 //Rose
            {29, 0},{30, 1},{31, 2},{32, 3},{33, 4},{34, 5}, //Haschel
            {23, 0},{24, 1},{25, 2},{26, 3},{27, 4},         //Meru
            {19, 0},{20, 1},{21, 2},                         //Kongol
            {255, 0}
        };
            if (character == 2 || character == 8) {
                Emulator.WriteByte(Globals.M_POINT + 0x148AC, Globals.DICTIONARY.AdditionData[character, 0, 0].SP);
                Emulator.WriteByte(Globals.M_POINT + 0x148AC + 0x4, Globals.DICTIONARY.AdditionData[character, 0, 1].SP);
                Emulator.WriteByte(Globals.M_POINT + 0x148AC + 0x8, Globals.DICTIONARY.AdditionData[character, 0, 2].SP);
                Emulator.WriteByte(Globals.M_POINT + 0x148AC + 0xC, Globals.DICTIONARY.AdditionData[character, 0, 3].SP);
                Emulator.WriteByte(Globals.M_POINT + 0x148AC + 0x10, Globals.DICTIONARY.AdditionData[character, 0, 4].SP);
            } else {
                int addition = additionnum[Emulator.ReadByte("CHAR_TABLE", (character * 0x2C) + 0x19)];
                for (int hit = 0; hit < 8; hit++) {
                    Emulator.WriteUShort(address + (slot * 0x100) + (hit * 0x20), (ushort) Globals.DICTIONARY.AdditionData[character, addition, hit].UU1);
                    Emulator.WriteUShort(address + (slot * 0x100) + (hit * 0x20) + 0x2, (ushort) Globals.DICTIONARY.AdditionData[character, addition, hit].Next_Hit);
                    Emulator.WriteUShort(address + (slot * 0x100) + (hit * 0x20) + 0x4, (ushort) Globals.DICTIONARY.AdditionData[character, addition, hit].Blue_Time);
                    Emulator.WriteUShort(address + (slot * 0x100) + (hit * 0x20) + 0x6, (ushort) Globals.DICTIONARY.AdditionData[character, addition, hit].Gray_Time);
                    Emulator.WriteUShort(address + (slot * 0x100) + (hit * 0x20) + 0x8, (ushort) Globals.DICTIONARY.AdditionData[character, addition, hit].DMG);
                    Emulator.WriteUShort(address + (slot * 0x100) + (hit * 0x20) + 0xA, (ushort) Globals.DICTIONARY.AdditionData[character, addition, hit].SP);
                    Emulator.WriteUShort(address + (slot * 0x100) + (hit * 0x20) + 0xC, (ushort) Globals.DICTIONARY.AdditionData[character, addition, hit].ID);
                    Emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0xE, Globals.DICTIONARY.AdditionData[character, addition, hit].Final_Hit);
                    Emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0xF, Globals.DICTIONARY.AdditionData[character, addition, hit].UU2);
                    Emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x10, Globals.DICTIONARY.AdditionData[character, addition, hit].UU3);
                    Emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x11, Globals.DICTIONARY.AdditionData[character, addition, hit].UU4);
                    Emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x12, Globals.DICTIONARY.AdditionData[character, addition, hit].UU5);
                    Emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x13, Globals.DICTIONARY.AdditionData[character, addition, hit].UU6);
                    Emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x14, Globals.DICTIONARY.AdditionData[character, addition, hit].UU7);
                    Emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x15, Globals.DICTIONARY.AdditionData[character, addition, hit].UU8);
                    Emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x16, Globals.DICTIONARY.AdditionData[character, addition, hit].UU9);
                    Emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x17, Globals.DICTIONARY.AdditionData[character, addition, hit].UU10);
                    Emulator.WriteUShort(address + (slot * 0x100) + (hit * 0x20) + 0x18, (ushort) Globals.DICTIONARY.AdditionData[character, addition, hit].Vertical_Distance);
                    Emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x1A, Globals.DICTIONARY.AdditionData[character, addition, hit].UU11);
                    Emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x1B, Globals.DICTIONARY.AdditionData[character, addition, hit].UU12);
                    Emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x1C, Globals.DICTIONARY.AdditionData[character, addition, hit].UU13);
                    Emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x1D, Globals.DICTIONARY.AdditionData[character, addition, hit].UU14);
                    Emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x1E, Globals.DICTIONARY.AdditionData[character, addition, hit].Start_Time);
                    Emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x1F, Globals.DICTIONARY.AdditionData[character, addition, hit].UU15);
                }
                int addition_level = Emulator.ReadByte("CHAR_TABLE", (character * 0x2C) + 0x1A + addition);
                Globals.CHARACTER_TABLE[slot].Add_DMG_Multi = (ushort) Globals.DICTIONARY.AdditionData[character, addition, addition_level].ADD_DMG_Multi;
                Globals.CHARACTER_TABLE[slot].Add_SP_Multi = (ushort) Globals.DICTIONARY.AdditionData[character, addition, addition_level].ADD_SP_Multi;
            }
        }

        public static void DragoonStatChanges(int slot, int character) {
            if (slot == 0 && Globals.NO_DART != null) {
                character = (byte) Globals.NO_DART;
            }
            
            long address = Constants.GetAddress("SECONDARY_CHARACTER_TABLE") + (character * 0xA0);

            int dlv = Emulator.ReadByte("CHAR_TABLE", (character * 0x2C) + 0x13);
            Globals.CHARACTER_TABLE[slot].DAT = Globals.DICTIONARY.DragoonStats[character][dlv].DAT;
            Globals.CHARACTER_TABLE[slot].DMAT = Globals.DICTIONARY.DragoonStats[character][dlv].DMAT;
            Globals.CHARACTER_TABLE[slot].DDF = Globals.DICTIONARY.DragoonStats[character][dlv].DDF;
            Globals.CHARACTER_TABLE[slot].DMDF =  Globals.DICTIONARY.DragoonStats[character][dlv].DMDF;
            double MP_base = Globals.DICTIONARY.DragoonStats[character][dlv].MP;
            double MP_multi = 1 + (double)Emulator.ReadByte(address + 0x64) / 100;
            ushort MP_Max = (ushort) (MP_base * MP_multi);
            ushort MP_Curr = Math.Min(Emulator.ReadUShort("CHAR_TABLE", (character * 0x2C) + 0xA), MP_Max);
            Globals.CHARACTER_TABLE[slot].MP = MP_Curr;
            Emulator.WriteUShort(address + 0x6, MP_Curr); // HAS TO BE CHECKED
            Globals.CHARACTER_TABLE[slot].Max_MP = MP_Max;
            Emulator.WriteUShort(address + 0x6E, MP_Max);
            
        }

        public static void SetCharacterStats(int slot, int character) {
            byte lv = Emulator.ReadByte("CHAR_TABLE", (character * 0x2C) + 0x12);
            long address = Constants.GetAddress("SECONDARY_CHARACTER_TABLE") + character * 0xA0;
            ushort base_HP = 0;
            byte hp_multi = 0;

            if (Globals.CheckDMScript("btnUltimateBoss")) {
                if (ultimateBossStage < 22) {
                    if (ultimateBossStage < 3) {
                        if (lv > 30) lv = 30;
                    } else if (ultimateBossStage < 8) {
                        if (lv > 40) lv = 40;
                    } else if (ultimateBossStage < 22) {
                        if (lv > 50) lv = 50;
                    }
                    Globals.CHARACTER_TABLE[slot].LV = lv;
                }
            }

            if (Globals.CHARACTER_STAT_CHANGE || Globals.CheckDMScript("btnUltimateBoss")) {
                Emulator.WriteByte("SECONDARY_CHARACTER_TABLE", Globals.DICTIONARY.CharacterStats[character][lv].SPD, character * 0xA0 + 0x69);
                Emulator.WriteByte("SECONDARY_CHARACTER_TABLE", Globals.DICTIONARY.CharacterStats[character][lv].AT, character * 0xA0 + 0x6A);
                Emulator.WriteByte("SECONDARY_CHARACTER_TABLE", Globals.DICTIONARY.CharacterStats[character][lv].MAT, character * 0xA0 + 0x6B);
                Emulator.WriteByte("SECONDARY_CHARACTER_TABLE", Globals.DICTIONARY.CharacterStats[character][lv].DF, character * 0xA0 + 0x6C);
                Emulator.WriteByte("SECONDARY_CHARACTER_TABLE", Globals.DICTIONARY.CharacterStats[character][lv].MDF, character * 0xA0 + 0x6D);
                base_HP = Globals.DICTIONARY.CharacterStats[character][lv].Max_HP;
            } else {
                double calc = Emulator.ReadUShort("SECONDARY_CHARACTER_TABLE", character * 0xA0 + 0x66);
                double divide = 1 + Emulator.ReadByte("SECONDARY_CHARACTER_TABLE", character * 0xA0 + 0x62) / 100;
                base_HP = (ushort) Math.Round(calc / divide);
            }
            if (Globals.ITEM_STAT_CHANGE) {
                dynamic weapon = Globals.DICTIONARY.ItemList[Emulator.ReadByte("CHAR_TABLE", character * 0x2C + 0x14)];
                dynamic armor = Globals.DICTIONARY.ItemList[Emulator.ReadByte("CHAR_TABLE", character * 0x2C + 0x15)];
                dynamic helm = Globals.DICTIONARY.ItemList[Emulator.ReadByte("CHAR_TABLE", character * 0x2C + 0x16)];
                dynamic boots = Globals.DICTIONARY.ItemList[Emulator.ReadByte("CHAR_TABLE", character * 0x2C + 0x17)];
                dynamic accessory = Globals.DICTIONARY.ItemList[Emulator.ReadByte("CHAR_TABLE", character * 0x2C + 0x18)];

                Emulator.WriteUShort(address + character * 0xA0 + 0x86, (ushort) (weapon.SPD + armor.SPD + helm.SPD + boots.SPD + accessory.SPD));
                Emulator.WriteUShort(address + character * 0xA0 + 0x88, (ushort) (weapon.AT + armor.AT + helm.AT + boots.AT + accessory.AT));
                Emulator.WriteUShort(address + character * 0xA0 + 0x8A, (ushort) (weapon.MAT + armor.MAT + helm.AT + boots.MAT + accessory.MAT));
                Emulator.WriteUShort(address + character * 0xA0 + 0x8C, (ushort) (weapon.DF + armor.DF + helm.DF + boots.DF + accessory.DF));
                Emulator.WriteUShort(address + character * 0xA0 + 0x8E, (ushort) (weapon.MDF + armor.MDF + helm.MDF + boots.MDF + accessory.MDF));

                byte stat_res = (byte) (weapon.Stat_Res | armor.Stat_Res | helm.Stat_Res | boots.Stat_Res | accessory.Stat_Res);
                Emulator.WriteByte(address + 0x7E, stat_res);
                Globals.CHARACTER_TABLE[slot].StatusResist = stat_res;
                byte e_half = (byte) (weapon.E_Half | armor.E_Half | helm.E_Half | boots.E_Half | accessory.E_Half);
                Emulator.WriteByte(address + 0x7C, e_half);
                Globals.CHARACTER_TABLE[slot].E_Half = e_half;
                byte e_immune = (byte) (weapon.E_Immune | armor.E_Immune | helm.E_Immune | boots.E_Immune | accessory.E_Immune);
                Emulator.WriteByte(address + 0x7D, e_immune);
                Globals.CHARACTER_TABLE[slot].E_Immune = e_immune;
                byte a_av = (byte) (weapon.A_AV + armor.A_AV + helm.A_AV + boots.A_AV + accessory.A_AV);
                Emulator.WriteByte(address + 0x94, a_av);
                Globals.CHARACTER_TABLE[slot].A_AV = a_av;
                byte m_av = (byte) (weapon.M_AV + armor.M_AV + helm.M_AV + boots.M_AV + accessory.M_AV);
                Emulator.WriteByte(address+ 0x96, m_av);
                Globals.CHARACTER_TABLE[slot].M_AV = m_av;
                byte a_hit = (byte) (weapon.A_Hit + armor.A_Hit + helm.A_Hit + boots.A_Hit + accessory.A_Hit);
                Emulator.WriteByte(address + 0x90, a_hit);
                Globals.CHARACTER_TABLE[slot].A_HIT = a_hit;
                byte m_hit = (byte) (weapon.M_Hit + armor.M_Hit + helm.M_Hit + boots.M_Hit + accessory.M_Hit);
                Emulator.WriteByte(address + 0x92, m_hit);
                Globals.CHARACTER_TABLE[slot].M_HIT = m_hit;
                byte p_half = (byte) (((weapon.Special1 & 0x20) | (armor.Special1 & 0x20) | (helm.Special1 & 0x20) | (boots.Special1 & 0x20) | (accessory.Special1 & 0x20)) >> 5);
                Emulator.WriteByte(address + 0x4A, p_half);
                Globals.CHARACTER_TABLE[slot].P_Half = p_half;
                byte m_half = (byte) (((weapon.Special2 & 0x4) | (armor.Special2 & 0x4) | (helm.Special2 & 0x4) | (boots.Special2 & 0x4) | (accessory.Special2 & 0x4)) >> 2);
                Emulator.WriteByte(address + 0x60, m_half);
                Globals.CHARACTER_TABLE[slot].M_Half = m_half;
                byte on_hit_status = weapon.On_Hit_Status;
                Emulator.WriteByte(address + 0x9B, on_hit_status);
                Globals.CHARACTER_TABLE[slot].On_Hit_Status = on_hit_status;
                byte status_chance = weapon.Status_Chance;
                Emulator.WriteByte(address + 0x98, status_chance);
                Globals.CHARACTER_TABLE[slot].On_Hit_Status_Chance = status_chance;
                byte revive = (byte) (((weapon.Special2 & 0x8) >> 3) * weapon.Special_Ammount + ((armor.Special2 & 0x8) >> 3) * armor.Special_Ammount + ((helm.Special2 & 0x8) >> 3) * helm.Special_Ammount
                + ((boots.Special2 & 0x8) >> 3) * boots.Special_Ammount + ((accessory.Special2 & 0x8) >> 3) * accessory.Special_Ammount);
                Emulator.WriteByte(address + 0x5E, revive);
                Globals.CHARACTER_TABLE[slot].Revive = revive;
                short sp_regen = (short) (((weapon.Special2 & 0x10) >> 4) * weapon.Special_Ammount + ((armor.Special2 & 0x10) >> 4) * armor.Special_Ammount + ((helm.Special2 & 0x10) >> 4) * helm.Special_Ammount
                    + ((boots.Special2 & 0x10) >> 4) * boots.Special_Ammount + ((accessory.Special2 & 0x10) >> 4) * accessory.Special_Ammount);
                Emulator.WriteShort(address + 0x5C, sp_regen);
                Globals.CHARACTER_TABLE[slot].SP_Regen = sp_regen;
                short mp_regen = (short) (((weapon.Special2 & 0x20) >> 5) * weapon.Special_Ammount + ((armor.Special2 & 0x20) >> 5) * armor.Special_Ammount + ((helm.Special2 & 0x20) >> 5) * helm.Special_Ammount
                    + ((boots.Special2 & 0x20) >> 5) * boots.Special_Ammount + ((accessory.Special2 & 0x20) >> 5) * accessory.Special_Ammount);
                Emulator.WriteShort(address + 0x5A, mp_regen);
                Globals.CHARACTER_TABLE[slot].MP_Regen = mp_regen;
                short hp_regen = (short) (((weapon.Special2 & 0x40) >> 6) * weapon.Special_Ammount + ((armor.Special2 & 0x40) >> 6) * armor.Special_Ammount + ((helm.Special2 & 0x40) >> 6) * helm.Special_Ammount
                    + ((boots.Special2 & 0x40) >> 6) * boots.Special_Ammount + ((accessory.Special2 & 0x40) >> 6) * accessory.Special_Ammount);
                Emulator.WriteShort(address + 0x58, hp_regen);
                Globals.CHARACTER_TABLE[slot].HP_Regen = hp_regen;
                byte mp_m_hit = (byte) ((weapon.Special1 & 0x1) * weapon.Special_Ammount + (armor.Special1 & 0x1) * armor.Special_Ammount + (helm.Special1 & 0x1) * helm.Special_Ammount
                + (boots.Special1 & 0x1) * boots.Special_Ammount + (accessory.Special1 & 0x1) * accessory.Special_Ammount);
                Emulator.WriteByte(address + 0x54, mp_m_hit);
                Globals.CHARACTER_TABLE[slot].MP_M_Hit = mp_m_hit;
                byte sp_m_hit = (byte) (((weapon.Special1 & 0x2) >> 1) * weapon.Special_Ammount + ((armor.Special1 & 0x2) >> 1) * armor.Special_Ammount + ((helm.Special1 & 0x2) >> 1) * helm.Special_Ammount
                    + ((boots.Special1 & 0x2) >> 1) * boots.Special_Ammount + ((accessory.Special1 & 0x2) >> 1) * accessory.Special_Ammount);
                Emulator.WriteByte(address + 0x52, sp_m_hit);
                Globals.CHARACTER_TABLE[slot].SP_M_Hit = sp_m_hit;
                byte mp_p_hit = (byte) (((weapon.Special1 & 0x4) >> 2) * weapon.Special_Ammount + ((armor.Special1 & 0x4) >> 2) * armor.Special_Ammount + ((helm.Special1 & 0x4) >> 2) * helm.Special_Ammount
                    + ((boots.Special1 & 0x4) >> 2) * boots.Special_Ammount + ((accessory.Special1 & 0x4) >> 2) * accessory.Special_Ammount);
                Emulator.WriteByte(address + 0x50, mp_p_hit);
                Globals.CHARACTER_TABLE[slot].MP_P_Hit = mp_p_hit;
                byte sp_p_hit = (byte) (((weapon.Special1 & 0x8) >> 3) * weapon.Special_Ammount + ((armor.Special1 & 0x8) >> 3) * armor.Special_Ammount + ((helm.Special1 & 0x8) >> 3) * helm.Special_Ammount
                    + ((boots.Special1 & 0x8) >> 3) * boots.Special_Ammount + ((accessory.Special1 & 0x8) >> 3) * accessory.Special_Ammount);
                Emulator.WriteByte(address + 0x4E, sp_p_hit);
                Globals.CHARACTER_TABLE[slot].SP_P_Hit = sp_p_hit;
                byte sp_multi = (byte) (((weapon.Special1 & 0x10) >> 4) * weapon.Special_Ammount + ((armor.Special1 & 0x10) >> 4) * armor.Special_Ammount + ((helm.Special1 & 0x10) >> 4) * helm.Special_Ammount
                    + ((boots.Special1 & 0x4) >> 4) * boots.Special_Ammount + ((accessory.Special1 & 0x10) >> 4) * accessory.Special_Ammount);
                Emulator.WriteByte(address + 0x4C, sp_multi);
                Globals.CHARACTER_TABLE[slot].SP_Multi = sp_multi;
                byte death_res = (byte) (weapon.Death_Res | armor.Death_Res | helm.Death_Res | boots.Death_Res | accessory.Death_Res);
                Emulator.WriteByte(address + 0x76, death_res);
                Globals.CHARACTER_TABLE[slot].Special_Efect = death_res;
                byte weapon_element = (byte) weapon.Element;
                Emulator.WriteByte(address + 0x7A, weapon_element);
                Globals.CHARACTER_TABLE[slot].Weapon_Element = weapon_element;

                hp_multi = (byte) (weapon.Special_Ammount * ((weapon.Special2 & 2) >> 1) + armor.Special_Ammount * ((armor.Special2 & 2) >> 1) + helm.Special_Ammount * ((helm.Special2 & 2) >> 1)
                    + boots.Special_Ammount * ((boots.Special2 & 2) >> 1) + accessory.Special_Ammount * ((accessory.Special2 & 2) >> 1));
                Emulator.WriteByte(address + 0x62, hp_multi);

                byte mp_multi = (byte) (weapon.Special_Ammount * (weapon.Special2 & 1) + armor.Special_Ammount * (armor.Special2 & 1) + helm.Special_Ammount * (helm.Special2 & 1)
                    + boots.Special_Ammount * (boots.Special2 & 1) + accessory.Special_Ammount * (accessory.Special2 & 1));

                Emulator.WriteByte((address + 0x64), mp_multi);
                double mp_multi2 = 1 + ((double) mp_multi) / (double)100;
                byte dlv = Emulator.ReadByte("CHAR_TABLE", (character * 0x2C) + 0x13);
                ushort mp_max = (ushort)(dlv * 20 * mp_multi2);
                ushort MP_Curr = Math.Min(Emulator.ReadUShort("CHAR_TABLE", (character * 0x2C) + 0xA), mp_max);
                Globals.CHARACTER_TABLE[slot].MP = MP_Curr;
                Emulator.WriteUShort(address + 0x6, MP_Curr); // HAS TO BE CHECKED
                Globals.CHARACTER_TABLE[slot].Max_MP = mp_max;
                Emulator.WriteUShort(address + 0x6E, mp_max);
            }
            
            ushort spd = (ushort) (Emulator.ReadByte(address + 0x69) + Emulator.ReadUShort(address + 0x86));
            Globals.CHARACTER_TABLE[slot].SPD = spd;
            Globals.CHARACTER_TABLE[slot].OG_SPD = spd;
            ushort at = (ushort) (Emulator.ReadByte(address + 0x6A) + Emulator.ReadUShort(address + 0x88));
            Globals.CHARACTER_TABLE[slot].AT = at;
            Globals.CHARACTER_TABLE[slot].OG_AT = at;
            ushort mat = (ushort) (Emulator.ReadByte(address + 0x6B) + Emulator.ReadUShort(address + 0x8A));
            Globals.CHARACTER_TABLE[slot].MAT = mat;
            Globals.CHARACTER_TABLE[slot].OG_MAT = mat;
            ushort df = (ushort) (Emulator.ReadByte(address + 0x6C) + Emulator.ReadUShort(address + 0x8C));
            Globals.CHARACTER_TABLE[slot].DF = df;
            Globals.CHARACTER_TABLE[slot].OG_DF = df;
            ushort mdf = (ushort) (Emulator.ReadByte(address + 0x6D) + Emulator.ReadUShort(address + 0x8E));
            Globals.CHARACTER_TABLE[slot].MDF = mdf;
            Globals.CHARACTER_TABLE[slot].OG_MDF = mdf;

            ushort hp_max = (ushort) (base_HP * (1 + (double) hp_multi / 100));
            //Globals.CHARACTER_TABLE[slot].Write("Max_HP", (ushort) (base_HP * (1 + hp_multi / 100)));
            //Globals.CHARACTER_TABLE[slot].Write("HP", Math.Min(Emulator2.ReadUShort("CHAR_TABLE", character * 0x2C + 0x8), hp_max));
            Globals.CHARACTER_TABLE[slot].HP = Math.Min(Globals.CURRENT_STATS[slot].HP, Globals.CURRENT_STATS[slot].Max_HP);
            Globals.CHARACTER_TABLE[slot].Max_HP = Globals.CURRENT_STATS[slot].Max_HP;
        }


        public static void DragoonAdditionsBattleChanges(int slot, int character) {
            long address = Constants.GetAddress("ADDITION") + Globals.BattleController.BattleOffset + 0x300 + slot * 0x100;
            Emulator.WriteUShort(address + 0x8, (ushort) Globals.DICTIONARY.DragoonAddition[character].HIT1);
            Emulator.WriteUShort(address + 0x20 + 0x8, (ushort) Globals.DICTIONARY.DragoonAddition[character].HIT2);
            Emulator.WriteUShort(address + 0x40 + 0x8, (ushort) Globals.DICTIONARY.DragoonAddition[character].HIT3);
            Emulator.WriteUShort(address + 0x60 + 0x8, (ushort) Globals.DICTIONARY.DragoonAddition[character].HIT4);
            Emulator.WriteUShort(address + 0x80 + 0x8, (ushort) Globals.DICTIONARY.DragoonAddition[character].HIT5);
        }

        public static void DragoonSpellChange(int slot, int character) {
            long address = Constants.GetAddress("SPELL_TABLE");
            double[] bases = new double[] { 800, 600, 500, 400, 300, 200, 150, 100, 50 };
            byte[] base_table = new byte[] { 0x1, 0x2, 0x4, 0x8, 0x10, 0x20, 0x40, 0, 0x80 };
            byte[][] Spells = new byte[9][] {
            new byte[] {0, 1, 2, 3, 4, 9 },
            new byte[]{14, 17, 26, 8 },
            new byte[]{10, 11, 12, 13 },
            new byte[]{15, 16, 18, 19 },
            new byte[]{20, 21, 22, 23 },
            new byte[]{5, 6, 7, 8 },
            new byte[]{24, 25, 27, 28 },
            new byte[]{29, 30, 31 },
            new byte[]{10, 11, 12, 13 }
        };
            foreach (byte id in Spells[character]) {
                byte dmg_base = 0;
                byte multi = 0;

                dynamic Spell = Globals.DRAGOON_SPELLS[id];
                int intValue = (int) Emulator.ReadByte("SPELL_TABLE", (id * 0xC) + 0x2);
                if (Spell.Percentage == true) {
                    intValue |= 1 << 2;
                    dmg_base = 0;
                    multi = (byte) Math.Round(Spell.Damage);
                } else {
                    intValue &= ~(1 << 2);
                    double stat = (double) Globals.CURRENT_STATS[slot].MAT;
                    double[] nearest_list = new double[9];
                    byte[] multi_list = new byte[9];
                    for (int i = 0; i < 9; i++) {
                        if (Spell.Damage < bases[i]) {
                            multi_list[i] = 0;
                        } else if (Spell.Damage > ((stat + 255) * bases[i]) / stat) {
                            multi_list[i] = 255;
                        } else {
                            multi_list[i] = (byte) Math.Round((Spell.Damage * stat - bases[i] * stat) / bases[i]);
                        }
                        nearest_list[i] = Math.Abs(Spell.Damage - (stat + multi_list[i]) * bases[i] / stat);
                    }
                    int index = Array.IndexOf(nearest_list, nearest_list.Min());
                    dmg_base = base_table[index];
                    multi = multi_list[index];
                }
                Emulator.WriteByte(address + (id * 0xC) + 0x2, (byte) intValue);
                Emulator.WriteByte(address + (id * 0xC) + 0x4, dmg_base);
                Emulator.WriteByte(address + (id * 0xC) + 0x5, multi);
                Emulator.WriteByte(address + (id * 0xC) + 0x6, Spell.Accuracy);
                Emulator.WriteByte(address + (id * 0xC) + 0x7, Spell.MP);
                Emulator.WriteByte(address + (id * 0xC) + 0x9, Spell.Element);
            }
        }

        public static void DragoonSpellDescriptionChange() {
            if (Globals.DRAGOON_DESC_CHANGE && Constants.REGION == Region.NTA) {
                int i = 0;
                string descr = String.Empty;
                long address = Constants.GetAddress("DRAGOON_DESC_PTR");
                foreach (dynamic Spell in Globals.DRAGOON_SPELLS) {
                    descr += (string) Spell.Encoded_Description + " ";
                    Emulator.WriteUInt(address + i * 0x4, (uint) Spell.Description_Pointer);
                    Emulator.WriteByte(address + i * 0x4 + 0x3, 0x80);
                    i++;
                }
                descr = descr.Remove(descr.Length - 1);
                Emulator.WriteAoB(Constants.GetAddress("DRAGOON_DESC"), descr);
            }
        }

        #endregion

        #region Item Changes

        public static void ItemBattleNameDescChange() {
            if (Globals.ITEM_NAMEDESC_CHANGE && Constants.REGION == Region.NTA) {
                Constants.WriteOutput("Changing Item Names and Descriptions...");
                long address = Constants.GetAddress("ITEM_BTL_NAME");
                long address2 = Constants.GetAddress("ITEM_BTL_NAME_PTR");
                int len1 = String.Join("", Globals.DICTIONARY.BattleNameList).Replace(" ", "").Length / 4;
                int len2 = (int) (address2 - address) / 2;
                if (len1 < len2) {
                    Emulator.WriteAoB(address, String.Join(" ", Globals.DICTIONARY.BattleNameList));
                    int i = 0;
                    foreach (dynamic item in Globals.DICTIONARY.ItemList) {
                        if (i < 192) {
                            i++;
                            continue;
                        }
                        if (i == 255) {
                            break;
                        }
                        Emulator.WriteUInt(address2 + (i - 192) * 0x4, (uint) item.BattleNamePointer);
                        Emulator.WriteByte(address2 + (i - 192) * 0x4 + 0x3, 0x80);
                        i++;
                    }
                } else {
                    string s = String.Format("Item name character limit exceeded! {0} / {1} characters.", len1, len2);
                    Constants.WriteDebug(s);
                }
                address = Constants.GetAddress("ITEM_BTL_DESC");
                address2 = Constants.GetAddress("ITEM_BTL_DESC_PTR");
                len1 = String.Join("", Globals.DICTIONARY.BattleDescriptionList).Replace(" ", "").Length / 4;
                len2 = (int) (address2 - address) / 2;
                if (len1 < len2) {
                    Emulator.WriteAoB(address, String.Join(" ", Globals.DICTIONARY.BattleDescriptionList));
                    int i = 0;
                    foreach (dynamic item in Globals.DICTIONARY.ItemList) {
                        if (i < 192) {
                            i++;
                            continue;
                        }
                        if (i == 255) {
                            break;
                        }
                        Emulator.WriteUInt(address2 + (i - 192) * 0x4, (uint) item.BattleDescriptionPointer);
                        Emulator.WriteByte(address2 + (i - 192) * 0x4 + 0x3, 0x80);
                        i++;
                    }
                } else {
                    string s = String.Format("Item description character limit exceeded! {0} / {1} characters.", len1, len2);
                    Constants.WriteDebug(s);
                }
            }
        }

        #endregion

        #region No Dart

        public static void ShanaFix(byte slot) {
            byte HP = 0;
            if (Globals.ENCOUNTER_ID == 408 || Globals.ENCOUNTER_ID == 409 || Globals.ENCOUNTER_ID == 387) {
                if (Globals.MONSTER_TABLE[0].Read("HP") != 0) {
                    HP = 1;
                }
            } else {
                foreach (dynamic monster in Globals.MONSTER_TABLE) {
                    if (monster.Read("HP") != 0) {
                        HP |= 1;
                    }
                }
            }
            if (HP == 0) {
                Emulator.WriteByte("PARTY_SLOT", 0, slot * 0x4);
                Globals.CHARACTER_TABLE[slot].Action = 2;
                while (Globals.CHARACTER_TABLE[slot].Action != 0) {
                    Thread.Sleep(250);
                }
                try {
                    Thread.Sleep(10000);
                    Emulator.WriteByte("PARTY_SLOT", (byte) Globals.NO_DART, slot * 0x4);
                } catch {
                    Constants.WriteDebug("No Dart not set");
                }
                Globals.SHANA_FIX = true;
            }
        }

        #endregion

        #endregion

        #region Field Changes

        #region Item Changes
        public static void ItemFieldChanges() {
            if (Globals.ITEM_STAT_CHANGE) {
                EquipStatChange();
            }
            if (Globals.THROWN_ITEM_CHANGE) {
                ItemChange();
            }
            if (Globals.ITEM_ICON_CHANGE) {
                ItemIconChange();
            }
            if (Globals.ITEM_NAMEDESC_CHANGE) {
                ItemNameDescChange();
            }
        }

        public static void EquipStatChange() {
            Constants.WriteOutput("Changing Equipment Stats...");
            long address = Constants.GetAddress("ITEM_TABLE");
            int i = 0;
            foreach (dynamic item in Globals.DICTIONARY.ItemList) {
                if (i > 185) //hopefully safe ammount
                    break;
                Emulator.WriteByte(address + i * 0x1C, item.Type);
                Emulator.WriteByte(address + i * 0x1C + 0x2, item.Equips);
                Emulator.WriteByte(address + i * 0x1C + 0x3, item.Element);
                Emulator.WriteByte(address + i * 0x1C + 0x1A, item.On_Hit_Status);
                Emulator.WriteByte(address + i * 0x1C + 0x17, item.Status_Chance);
                if (item.AT > 255) {
                    Emulator.WriteByte(address + i * 0x1C + 0x9, 255);
                    Emulator.WriteByte(address + i * 0x1C + 0xF, item.AT - 255);
                } else {
                    Emulator.WriteByte(address + i * 0x1C + 0x9, item.AT);
                    Emulator.WriteByte(address + i * 0x1C + 0xF, 0);
                }
                Emulator.WriteByte(address + i * 0x1C + 0x10, item.MAT);
                Emulator.WriteByte(address + i * 0x1C + 0x11, item.DF);
                Emulator.WriteByte(address + i * 0x1C + 0x12, item.MDF);
                Emulator.WriteByte(address + i * 0x1C + 0xE, item.SPD);
                Emulator.WriteByte(address + i * 0x1C + 0x13, item.A_Hit);
                Emulator.WriteByte(address + i * 0x1C + 0x14, item.M_Hit);
                Emulator.WriteByte(address + i * 0x1C + 0x15, item.A_AV);
                Emulator.WriteByte(address + i * 0x1C + 0x16, item.M_AV);
                Emulator.WriteByte(address + i * 0x1C + 0x5, item.E_Half);
                Emulator.WriteByte(address + i * 0x1C + 0x6, item.E_Immune);
                Emulator.WriteByte(address + i * 0x1C + 0x7, item.Stat_Res);
                Emulator.WriteByte(address + i * 0x1C + 0xA, item.Special1);
                Emulator.WriteByte(address + i * 0x1C + 0xB, item.Special2);
                Emulator.WriteByte(address + i * 0x1C + 0xC, (byte) item.Special_Ammount);
                i++;
            }
        }

        public static void ItemChange() {
            Constants.WriteOutput("Changing Thrown Items...");
            long address = Constants.GetAddress("THROWN_ITEM_TABLE");
            for (int i = 193; i < 255; i++) {
                Emulator.WriteByte(address + (i - 192) * 0xC, Globals.DICTIONARY.ItemList[i].Target);
                Emulator.WriteByte(address + (i - 192) * 0xC + 0x1, Globals.DICTIONARY.ItemList[i].Element);
                Emulator.WriteByte(address + (i - 192) * 0xC + 0x2, Globals.DICTIONARY.ItemList[i].Damage);
                Emulator.WriteByte(address + (i - 192) * 0xC + 0x3, Globals.DICTIONARY.ItemList[i].Special1);
                Emulator.WriteByte(address + (i - 192) * 0xC + 0x4, Globals.DICTIONARY.ItemList[i].Special2);
                Emulator.WriteByte(address + (i - 192) * 0xC + 0x5, Globals.DICTIONARY.ItemList[i].UU1);
                Emulator.WriteByte(address + (i - 192) * 0xC + 0x6, Globals.DICTIONARY.ItemList[i].Special_Ammount);
                Emulator.WriteByte(address + (i - 192) * 0xC + 0x8, Globals.DICTIONARY.ItemList[i].Status);
                Emulator.WriteByte(address + (i - 192) * 0xC + 0x9, Globals.DICTIONARY.ItemList[i].Percentage);
                Emulator.WriteByte(address + (i - 192) * 0xC + 0xA, Globals.DICTIONARY.ItemList[i].UU2);
                Emulator.WriteByte(address + (i - 192) * 0xC + 0xB, Globals.DICTIONARY.ItemList[i].BaseSwitch);
            }
        }

        public static void ItemIconChange() {
            Constants.WriteOutput("Changing Item Icons...");
            long address = Constants.GetAddress("ITEM_TABLE");
            for (int i = 0; i < 186; i++) {
                Emulator.WriteByte(address + i * 0x1C + 0xD, Globals.DICTIONARY.ItemList[i].Icon);
            }
            address = Constants.GetAddress("THROWN_ITEM_TABLE");
            for (int i = 192; i < 255; i++) {
                Emulator.WriteByte(address + (i - 192) * 0xC + 0x7, Globals.DICTIONARY.ItemList[i].Icon);
            }
        }

        public static void ItemNameDescChange() {
            if (Globals.ITEM_NAMEDESC_CHANGE && Constants.REGION == Region.NTA) {
                Constants.WriteOutput("Changing Item Names and Descriptions...");
                long address = Constants.GetAddress("ITEM_NAME");
                long address2 = Constants.GetAddress("ITEM_NAME_PTR");
                int len1 = String.Join("", Globals.DICTIONARY.NameList).Replace(" ", "").Length / 4;
                int len2 = (int) (address2 - address) / 2;
                if (len1 < len2) {
                    Emulator.WriteAoB(address, String.Join(" ", Globals.DICTIONARY.NameList));
                    int i = 0;
                    foreach (dynamic item in Globals.DICTIONARY.ItemList) {
                        Emulator.WriteUInt(address2 + i * 0x4, (uint) item.NamePointer);
                        Emulator.WriteByte(address2 + i * 0x4 + 0x3, 0x80);
                        i++;
                    }
                } else {
                    string s = String.Format("Item name character limit exceeded! {0} / {1} characters.", len1, len2);
                    Constants.WriteDebug(s);
                }
                address = Constants.GetAddress("ITEM_DESC");
                address2 = Constants.GetAddress("ITEM_DESC_PTR");
                len1 = String.Join("", Globals.DICTIONARY.DescriptionList).Replace(" ", "").Length / 4;
                len2 = (int) (address2 - address) / 2;
                if (len1 < len2) {
                    Emulator.WriteAoB(address, String.Join(" ", Globals.DICTIONARY.DescriptionList));
                    int i = 0;
                    foreach (dynamic item in Globals.DICTIONARY.ItemList) {
                        Emulator.WriteUInt(address2 + i * 0x4, (uint) item.DescriptionPointer);
                        Emulator.WriteByte(address2 + i * 0x4 + 0x3, 0x80);
                        i++;
                    }
                } else {
                    string s = String.Format("Item description character limit exceeded! {0} / {1} characters.", len1, len2);
                    Constants.WriteDebug(s);
                }
            }
        }

        #endregion

        #region Character Changes
        public static void CharacterFieldChanges() {
            if (Globals.DRAGOON_STAT_CHANGE) {
                DragoonTableChange();
            }
            if (Globals.CHARACTER_STAT_CHANGE) {
                CharacterTableChange();
            }
            if (Globals.ADDITION_CHANGE) {
                AdditionTableChange();
            }
            if (Globals.ADDITION_LEVEL_CHANGE) {
                AdditionLevelChange();
            }
        }

        public static void DragoonTableChange() {
            Constants.WriteOutput("Changing Dragoon Stat table...");
            long address = Constants.GetAddress("DRAGOON_TABLE");
            int[] charReorder = new int[] { 5, 7, 0, 4, 6, 8, 1, 3, 2 };
            for (int character = 0; character < 8; character++) {
                int reorderedChar = charReorder[character];
                for (int level = 1; level < 6; level++) {
                    Emulator.WriteUShort(address + character * 0x30 + level * 0x8, Globals.DICTIONARY.DragoonStats[reorderedChar][level].MP);
                    Emulator.WriteByte(address + character * 0x30 + level * 0x8 + 0x4, Globals.DICTIONARY.DragoonStats[reorderedChar][level].DAT);
                    Emulator.WriteByte(address + character * 0x30 + level * 0x8 + 0x5, Globals.DICTIONARY.DragoonStats[reorderedChar][level].DMAT);
                    Emulator.WriteByte(address + character * 0x30 + level * 0x8 + 0x6, Globals.DICTIONARY.DragoonStats[reorderedChar][level].DDF);
                    Emulator.WriteByte(address + character * 0x30 + level * 0x8 + 0x7, Globals.DICTIONARY.DragoonStats[reorderedChar][level].DMDF);
                }
            }
        }

        public static void CharacterTableChange() {
            Constants.WriteOutput("Changing Character Stat table...");
            long address = Constants.GetAddress("CHAR_STAT_TABLE");
            int[] charReorder = new int[] { 7, 0, 4, 6, 1, 3, 2 };
            for (int character = 0; character < 7; character++) {
                int reorderedChar = charReorder[character];
                for (int level = 0; level < 61; level++) {
                    if (level > 0) {
                        Emulator.WriteUShort(address + level * 8 + character * 0x1E8, (ushort) (Globals.DICTIONARY.CharacterStats[reorderedChar][level].Max_HP));
                        Emulator.WriteByte(address + level * 8 + character * 0x1E8 + 0x3, Globals.DICTIONARY.CharacterStats[reorderedChar][level].SPD);
                        Emulator.WriteByte(address + level * 8 + character * 0x1E8 + 0x4, Globals.DICTIONARY.CharacterStats[reorderedChar][level].AT);
                        Emulator.WriteByte(address + level * 8 + character * 0x1E8 + 0x5, Globals.DICTIONARY.CharacterStats[reorderedChar][level].MAT);
                        Emulator.WriteByte(address + level * 8 + character * 0x1E8 + 0x6, Globals.DICTIONARY.CharacterStats[reorderedChar][level].DF);
                        Emulator.WriteByte(address + level * 8 + character * 0x1E8 + 0x7, Globals.DICTIONARY.CharacterStats[reorderedChar][level].MDF);
                    }
                }
            }
        }

        public static void AdditionTableChange() {
            Constants.WriteOutput("Changing Addition table...");
            int reorderedaddition = 0;
            int character = 0;
            long address = Constants.GetAddress("MENU_ADDITION_TABLE_FLAT");
            long address2 = Constants.GetAddress("MENU_ADDITION_TABLE_MULTI");
            for (int addition = 0; addition < 35; addition++) {
                if (new int[] { 7, 13, 18, 22, 28 }.Contains(addition)) {
                    continue;
                }
                if (addition == 8) {
                    character = 1;
                    reorderedaddition = 0;
                } else if (addition == 14) {
                    character = 3;
                    reorderedaddition = 0;
                } else if (addition == 19) {
                    character = 7;
                    reorderedaddition = 0;
                } else if (addition == 23) {
                    character = 6;
                    reorderedaddition = 0;
                } else if (addition == 29) {
                    character = 4;
                    reorderedaddition = 0;
                } else {
                    if (addition != 0) {
                        reorderedaddition += 1;
                    }
                }
                ushort damage = 0;
                ushort sp1 = 0;
                ushort sp2 = 0;
                ushort sp3 = 0;
                ushort sp4 = 0;
                ushort sp5 = 0;
                for (int hit = 0; hit < 8; hit++) {
                    damage += (ushort) Globals.DICTIONARY.AdditionData[character, reorderedaddition, hit].DMG;
                    sp1 += (ushort) (Globals.DICTIONARY.AdditionData[character, reorderedaddition, hit].SP * (1 + (double) Globals.DICTIONARY.AdditionData[character, reorderedaddition, 1].ADD_SP_Multi / 100));
                    sp2 += (ushort) (Globals.DICTIONARY.AdditionData[character, reorderedaddition, hit].SP * (1 + (double) Globals.DICTIONARY.AdditionData[character, reorderedaddition, 2].ADD_SP_Multi / 100));
                    sp3 += (ushort) (Globals.DICTIONARY.AdditionData[character, reorderedaddition, hit].SP * (1 + (double) Globals.DICTIONARY.AdditionData[character, reorderedaddition, 3].ADD_SP_Multi / 100));
                    sp4 += (ushort) (Globals.DICTIONARY.AdditionData[character, reorderedaddition, hit].SP * (1 + (double) Globals.DICTIONARY.AdditionData[character, reorderedaddition, 4].ADD_SP_Multi / 100));
                    sp5 += (ushort) (Globals.DICTIONARY.AdditionData[character, reorderedaddition, hit].SP * (1 + (double) Globals.DICTIONARY.AdditionData[character, reorderedaddition, 5].ADD_SP_Multi / 100));
                }
                Emulator.WriteUShort(address + addition * 0xE + 0x2, sp1);
                Emulator.WriteUShort(address + addition * 0xE + 0x4, sp2);
                Emulator.WriteUShort(address + addition * 0xE + 0x6, sp3);
                Emulator.WriteUShort(address + addition * 0xE + 0x8, sp4);
                Emulator.WriteUShort(address + addition * 0xE + 0xA, sp5);
                Emulator.WriteUShort(address + addition * 0xE + 0xC, damage);

                Emulator.WriteByte(address2 + addition * 0x18, (byte) Globals.DICTIONARY.AdditionData[character, reorderedaddition, 1].ADD_DMG_Multi);
                Emulator.WriteByte(address2 + 0x4 + addition * 0x18, (byte) Globals.DICTIONARY.AdditionData[character, reorderedaddition, 2].ADD_DMG_Multi);
                Emulator.WriteByte(address2 + 0x8 + addition * 0x18, (byte) Globals.DICTIONARY.AdditionData[character, reorderedaddition, 3].ADD_DMG_Multi);
                Emulator.WriteByte(address2 + 0xC + addition * 0x18, (byte) Globals.DICTIONARY.AdditionData[character, reorderedaddition, 4].ADD_DMG_Multi);
                Emulator.WriteByte(address2 + 0x10 + addition * 0x18, (byte) Globals.DICTIONARY.AdditionData[character, reorderedaddition, 5].ADD_DMG_Multi);
            }
        }

        public static void AdditionLevelChange() {
            Constants.WriteDebug("Changing Addition Levels...");
            long address = Constants.GetAddress("MENU_ADDITION_TABLE_FLAT");
            int addition = 0;
            for (int i = 0; i < 35; i++) {
                if (new int[] { 7, 13, 18, 22, 28 }.Contains(i)) {
                    continue;
                }
                Emulator.WriteByte(address + addition * 0xE, Globals.DICTIONARY.AdditionLevels[addition]);
                addition++;
            }
        }

        #endregion

        #endregion

        #region Scripts

        #region Addition Swap
        public static void AdditionSwapInit() {
            Nullable<int> character = null;
            for (byte slot = 0; slot < 3; slot++) {
                if (Globals.CHARACTER_TABLE[slot].Action == 8) {
                    character = (int) slot;
                }
            }

            if (character != null) {
                int slot = (int) character;
                long additionUnlock = Constants.GetAddress("MENU_ADDITION_TABLE_FLAT");
                long additionXP = Constants.GetAddress("CHAR_TABLE") + 0x22;
                byte level = Emulator.ReadByte((long) (Constants.GetAddress("CHAR_TABLE") + 0x12 + 0x2C * character));
                byte swappableAdditions = 1;

                if (Globals.PARTY_SLOT[slot] == (byte) 0) {
                    if (Emulator.ReadByte(additionXP) >= 80 &&
                        Emulator.ReadByte(additionXP + 0x1) >= 80 &&
                        Emulator.ReadByte(additionXP + 0x2) >= 80 &&
                        Emulator.ReadByte(additionXP + 0x3) >= 80 &&
                        Emulator.ReadByte(additionXP + 0x4) >= 80 &&
                        Emulator.ReadByte(additionXP + 0x5) >= 80) {
                        if (Emulator.ReadByte(additionUnlock + 0xE * 6) < 255) {
                            if (level >= Emulator.ReadByte(additionUnlock + 0xE * 6)) {
                                swappableAdditions = 7;
                            } else {
                                swappableAdditions = 6;
                            }
                        } else {
                            swappableAdditions = 7;
                        }
                    } else {
                        if (level >= Emulator.ReadByte(additionUnlock + 0xE * 5)) {
                            swappableAdditions = 6;
                        } else if (level >= Emulator.ReadByte(additionUnlock + 0xE * 4)) {
                            swappableAdditions = 5;
                        } else if (level >= Emulator.ReadByte(additionUnlock + 0xE * 3)) {
                            swappableAdditions = 4;
                        } else if (level >= Emulator.ReadByte(additionUnlock + 0xE * 2)) {
                            swappableAdditions = 3;
                        } else if (level >= Emulator.ReadByte(additionUnlock + 0xE * 1)) {
                            swappableAdditions = 2;
                        } else {
                            swappableAdditions = 1;
                        }
                    }
                } else if (Globals.PARTY_SLOT[slot] == (byte) 1) {
                    if (Emulator.ReadByte(additionXP + 0x2C) >= 80 &&
                        Emulator.ReadByte(additionXP + 0x2C + 0x1) >= 80 &&
                        Emulator.ReadByte(additionXP + 0x2C + 0x2) >= 80 &&
                        Emulator.ReadByte(additionXP + 0x2C + 0x3) >= 80) {
                        if (Emulator.ReadByte(additionUnlock + 0xE * 4 + 0x70) < 255) {
                            if (level >= Emulator.ReadByte(additionUnlock + 0xE * 4 + 0x70)) {
                                swappableAdditions = 5;
                            } else {
                                swappableAdditions = 4;
                            }
                        } else {
                            swappableAdditions = 5;
                        }
                    } else {
                        if (level >= Emulator.ReadByte(additionUnlock + 0xE * 4 + 0x70)) {
                            swappableAdditions = 4;
                        } else if (level >= Emulator.ReadByte(additionUnlock + 0xE * 3 + 0x70)) {
                            swappableAdditions = 3;
                        } else if (level >= Emulator.ReadByte(additionUnlock + 0xE * 2 + 0x70)) {
                            swappableAdditions = 2;
                        } else {
                            swappableAdditions = 1;
                        }
                    }
                } else if (Globals.PARTY_SLOT[slot] == (byte) 5) {
                    if (Emulator.ReadByte(additionXP + 0xDC) >= 80 &&
                        Emulator.ReadByte(additionXP + 0xDC + 0x1) >= 80 &&
                        Emulator.ReadByte(additionXP + 0xDC + 0x2) >= 80 &&
                        Emulator.ReadByte(additionXP + 0xDC + 0x3) >= 80) {
                        if (Emulator.ReadByte(additionUnlock + 0xE * 4 + 0x70) < 255) {
                            if (level >= Emulator.ReadByte(additionUnlock + 0xE * 4 + 0x70)) {
                                swappableAdditions = 5;
                            } else {
                                swappableAdditions = 4;
                            }
                        } else {
                            swappableAdditions = 5;
                        }
                    } else {
                        if (level >= Emulator.ReadByte(additionUnlock + 0xE * 4 + 0x70)) {
                            swappableAdditions = 4;
                        } else if (level >= Emulator.ReadByte(additionUnlock + 0xE * 3 + 0x70)) {
                            swappableAdditions = 3;
                        } else if (level >= Emulator.ReadByte(additionUnlock + 0xE * 2 + 0x70)) {
                            swappableAdditions = 2;
                        } else {
                            swappableAdditions = 1;
                        }
                    }
                } else if (Globals.PARTY_SLOT[slot] == (byte) 3) {
                    if (Emulator.ReadByte(additionXP + 0x84) >= 80 &&
                        Emulator.ReadByte(additionXP + 0x84 + 0x1) >= 80 &&
                        Emulator.ReadByte(additionXP + 0x84 + 0x2) >= 80) {
                        if (Emulator.ReadByte(additionUnlock + 0xE * 3 + 0xC4) < 255) {
                            if (level >= Emulator.ReadByte(additionUnlock + 0xE * 3 + 0xC4)) {
                                swappableAdditions = 4;
                            } else {
                                swappableAdditions = 3;
                            }
                        } else {
                            swappableAdditions = 4;
                        }
                    } else {
                        if (level >= Emulator.ReadByte(additionUnlock + 0xE * 3 + 0xC4)) {
                            swappableAdditions = 3;
                        } else if (level >= Emulator.ReadByte(additionUnlock + 0xE * 2 + 0xC4)) {
                            swappableAdditions = 2;
                        } else {
                            swappableAdditions = 1;
                        }
                    }
                } else if (Globals.PARTY_SLOT[slot] == (byte) 7) {
                    if (Emulator.ReadByte(additionXP + 0x134) >= 80 &&
                        Emulator.ReadByte(additionXP + 0x134 + 0x1) >= 80) {
                        if (Emulator.ReadByte(additionUnlock + 0xE * 2 + 0x10A) < 255) {
                            if (level >= Emulator.ReadByte(additionUnlock + 0xE * 2 + 0x10A)) {
                                swappableAdditions = 3;
                            } else {
                                swappableAdditions = 2;
                            }
                        } else {
                            swappableAdditions = 3;
                        }
                    } else {
                        if (level >= Emulator.ReadByte(additionUnlock + 0xE * 2 + 0x10A)) {
                            swappableAdditions = 2;
                        } else {
                            swappableAdditions = 1;
                        }
                    }
                } else if (Globals.PARTY_SLOT[slot] == (byte) 6) {
                    if (Emulator.ReadByte(additionXP + 0x108) >= 80 &&
                        Emulator.ReadByte(additionXP + 0x108 + 0x1) >= 80 &&
                        Emulator.ReadByte(additionXP + 0x108 + 0x2) >= 80 &&
                        Emulator.ReadByte(additionXP + 0x108 + 0x3) >= 80) {
                        if (Emulator.ReadByte(additionUnlock + 0xE * 4 + 0x142) < 255) {
                            if (level >= Emulator.ReadByte(additionUnlock + 0xE * 4 + 0x142)) {
                                swappableAdditions = 5;
                            } else {
                                swappableAdditions = 4;
                            }
                        } else {
                            swappableAdditions = 5;
                        }
                    } else {
                        if (level >= Emulator.ReadByte(additionUnlock + 0xE * 4 + 0x142)) {
                            swappableAdditions = 4;
                        } else if (level >= Emulator.ReadByte(additionUnlock + 0xE * 3 + 0x142)) {
                            swappableAdditions = 3;
                        } else if (level >= Emulator.ReadByte(additionUnlock + 0xE * 2 + 0x142)) {
                            swappableAdditions = 2;
                        } else {
                            swappableAdditions = 1;
                        }
                    }
                } else if (Globals.PARTY_SLOT[slot] == (byte) 4) {
                    if (Emulator.ReadByte(additionXP + 0xB0) >= 80 &&
                        Emulator.ReadByte(additionXP + 0xB0 + 0x1) >= 80 &&
                        Emulator.ReadByte(additionXP + 0xB0 + 0x2) >= 80 &&
                        Emulator.ReadByte(additionXP + 0xB0 + 0x3) >= 80 &&
                        Emulator.ReadByte(additionXP + 0xB0 + 0x4) >= 80) {
                        if (Emulator.ReadByte(additionUnlock + 0xE * 5 + 0x196) < 255) {
                            if (level >= Emulator.ReadByte(additionUnlock + 0xE * 5 + 0x196)) {
                                swappableAdditions = 6;
                            } else {
                                swappableAdditions = 5;
                            }
                        } else {
                            swappableAdditions = 6;
                        }
                    } else {
                        if (level >= Emulator.ReadByte(additionUnlock + 0xE * 5 + 0x196)) {
                            swappableAdditions = 5;
                        } else if (level >= Emulator.ReadByte(additionUnlock + 0xE * 4 + 0x196)) {
                            swappableAdditions = 4;
                        } else if (level >= Emulator.ReadByte(additionUnlock + 0xE * 3 + 0x196)) {
                            swappableAdditions = 3;
                        } else if (level >= Emulator.ReadByte(additionUnlock + 0xE * 2 + 0x196)) {
                            swappableAdditions = 2;
                        } else {
                            swappableAdditions = 1;
                        }
                    }
                }

                AdditionSwap((byte) character, swappableAdditions);
            }
        }

        public static void AdditionSwap(byte slot, byte additionCount) {
            byte[][] reorderAddition = new byte[9][] {
                new byte[] { 0, 1, 2, 3, 4, 5, 6 },
                new byte[]{ 8, 9, 10, 11, 12 },
                new byte[]{ 255 },
                new byte[]{ 14, 15, 16, 17 },
                new byte[]{ 29, 30, 31, 32, 33, 34 },
                new byte[]{ 8, 9, 10, 11, 12 },
                new byte[]{ 23, 24, 25, 26, 27 },
                new byte[]{ 19, 20, 21 },
                new byte[]{ 255 }
            };
            byte character = Globals.PARTY_SLOT[slot];
            if (additionCount > 1) {
                ushort SP = Globals.CHARACTER_TABLE[slot].SP;
                Globals.CHARACTER_TABLE[slot].SP = 100;
                Globals.CHARACTER_TABLE[slot].Action = 10;
                Emulator.WriteByte(Globals.C_POINT - 0x388 * slot - 0x48, 1);
                Emulator.WriteByte("DRAGOON_TURNS", 1, slot * 0x4);
                byte menu = 0;
                byte addition = 0;
                for (int i = 0; i < additionCount; i++) {
                    menu +=  (byte) Math.Pow(2, i);
                }
                Globals.CHARACTER_TABLE[slot].Menu = menu;
                Thread.Sleep(50);
                Emulator.WriteByte(Globals.M_POINT + 0xD32, additionCount);
                for (byte i = 0; i < additionCount; i++) {
                    Emulator.WriteByte(Globals.M_POINT + 0xD34 + i * 0x2, 2);
                }

                while (true) {
                    if (Emulator.ReadUShort("BATTLE_VALUE") < 5130) {
                        return;
                    }
                    if (Globals.CHARACTER_TABLE[slot].Action != 9) {
                        addition = Emulator.ReadByte(Globals.M_POINT + 0xD46);
                        Thread.Sleep(50);
                    } else {
                        break;
                    }
                }

                byte add = reorderAddition[character][addition];
                Emulator.WriteByte(Constants.GetAddress("CHAR_TABLE") + (character * 0x2C) + 0x19, add);
                Thread.Sleep(200);
                long address = Constants.GetAddress("ADDITION") + Globals.BattleController.BattleOffset;
                for (int hit = 0; hit < 8; hit++) {
                    Emulator.WriteUShort(address + (slot * 0x100) + (hit * 0x20), (ushort) Globals.DICTIONARY.AdditionData[character, addition, hit].UU1);
                    Emulator.WriteUShort(address + (slot * 0x100) + (hit * 0x20) + 0x2, (ushort) Globals.DICTIONARY.AdditionData[character, addition, hit].Next_Hit);
                    Emulator.WriteUShort(address + (slot * 0x100) + (hit * 0x20) + 0x4, (ushort) Globals.DICTIONARY.AdditionData[character, addition, hit].Blue_Time);
                    Emulator.WriteUShort(address + (slot * 0x100) + (hit * 0x20) + 0x6, (ushort) Globals.DICTIONARY.AdditionData[character, addition, hit].Gray_Time);
                    Emulator.WriteUShort(address + (slot * 0x100) + (hit * 0x20) + 0x8, (ushort) Globals.DICTIONARY.AdditionData[character, addition, hit].DMG);
                    Emulator.WriteUShort(address + (slot * 0x100) + (hit * 0x20) + 0xA, (ushort) Globals.DICTIONARY.AdditionData[character, addition, hit].SP);
                    Emulator.WriteUShort(address + (slot * 0x100) + (hit * 0x20) + 0xC, (ushort) Globals.DICTIONARY.AdditionData[character, addition, hit].ID);
                    Emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0xE, Globals.DICTIONARY.AdditionData[character, addition, hit].Final_Hit);
                    Emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0xF, Globals.DICTIONARY.AdditionData[character, addition, hit].UU2);
                    Emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x10, Globals.DICTIONARY.AdditionData[character, addition, hit].UU3);
                    Emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x11, Globals.DICTIONARY.AdditionData[character, addition, hit].UU4);
                    Emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x12, Globals.DICTIONARY.AdditionData[character, addition, hit].UU5);
                    Emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x13, Globals.DICTIONARY.AdditionData[character, addition, hit].UU6);
                    Emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x14, Globals.DICTIONARY.AdditionData[character, addition, hit].UU7);
                    Emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x15, Globals.DICTIONARY.AdditionData[character, addition, hit].UU8);
                    Emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x16, Globals.DICTIONARY.AdditionData[character, addition, hit].UU9);
                    Emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x17, Globals.DICTIONARY.AdditionData[character, addition, hit].UU10);
                    Emulator.WriteUShort(address + (slot * 0x100) + (hit * 0x20) + 0x18, (ushort) Globals.DICTIONARY.AdditionData[character, addition, hit].Vertical_Distance);
                    Emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x1A, Globals.DICTIONARY.AdditionData[character, addition, hit].UU11);
                    Emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x1B, Globals.DICTIONARY.AdditionData[character, addition, hit].UU12);
                    Emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x1C, Globals.DICTIONARY.AdditionData[character, addition, hit].UU13);
                    Emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x1D, Globals.DICTIONARY.AdditionData[character, addition, hit].UU14);
                    Emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x1E, Globals.DICTIONARY.AdditionData[character, addition, hit].Start_Time);
                    Emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x1F, Globals.DICTIONARY.AdditionData[character, addition, hit].UU15);
                }
                int addition_level = Emulator.ReadByte("CHAR_TABLE", (character * 0x2C) + 0x1A + addition);
                Globals.CHARACTER_TABLE[slot].Add_DMG_Multi = (ushort) Globals.DICTIONARY.AdditionData[character, addition, addition_level].ADD_DMG_Multi;
                Globals.CHARACTER_TABLE[slot].Add_SP_Multi = (ushort) Globals.DICTIONARY.AdditionData[character, addition, addition_level].ADD_SP_Multi;
                /*
                while ((Emulator2.ReadUShort("BATTLE_VALUE") > 9999) && (Globals.CHARACTER_TABLE[slot].Read("Menu") != 96)) {
                    Thread.Sleep(50);
                }
                if (slot == 0 && Globals.NO_DART == 4) {
                    HaschelFix(emulator);
                }
                Emulator2.WriteByte("DRAGOON_TURNS", 1, slot * 0x4);
                */
                ushort turn = Globals.CHARACTER_TABLE[slot].Turn;
                Globals.CHARACTER_TABLE[slot].Turn = 800;
                ushort MP = Globals.CHARACTER_TABLE[slot].MP;
                Globals.CHARACTER_TABLE[slot].MP = 0;
                short HP_reg = Globals.CHARACTER_TABLE[slot].HP_Regen;
                Globals.CHARACTER_TABLE[slot].HP_Regen = 0;
                short MP_reg = Globals.CHARACTER_TABLE[slot].MP_Regen;
                Globals.CHARACTER_TABLE[slot].MP_Regen = 1;
                short SP_reg = Globals.CHARACTER_TABLE[slot].SP_Regen;
                Globals.CHARACTER_TABLE[slot].SP_Regen = 0;
                /*
                ushort val = Emulator2.ReadUShort(Globals.C_POINT - slot * 0x388 - 0xF0);
                Emulator2.WriteUShort(Globals.C_POINT - slot * 0x388 - 0xF0, (ushort)(val + 0x4478));
                Emulator2.WriteByte(Globals.C_POINT - slot * 0x388 - 0xEE, 27);
                */

                while (true) {
                    if (Emulator.ReadUShort("BATTLE_VALUE") < 5130) {
                        return;
                    }
                    if (Globals.CHARACTER_TABLE[slot].MP != 1) {
                        Thread.Sleep(50);
                    } else {
                        break;
                    }
                }

                Globals.CHARACTER_TABLE[slot].SP = SP;
                Globals.CHARACTER_TABLE[slot].MP = MP;
                Globals.CHARACTER_TABLE[slot].Turn = turn;
                Globals.CHARACTER_TABLE[slot].HP_Regen = HP_reg;
                Globals.CHARACTER_TABLE[slot].MP_Regen = MP_reg;
                Globals.CHARACTER_TABLE[slot].SP_Regen = SP_reg;
                Globals.ADDITION_SWAP = false;
                Constants.WriteGLogOutput("Addition swap complete.");
            }
        }

        #endregion

        #region Addition Level Up in Battle
        public static void AdditionLevelUp() {
            Dictionary<int, int> additionnum = new Dictionary<int, int> {
                {0, 0},{1, 1},{2, 2},{3, 3},{4, 4},{5, 5},{6, 6},//Dart
			    {8, 0},{9, 1},{10, 2},{11, 3},{12, 4},           //Lavitz
			    {14, 0},{15, 1},{16, 2},{17, 3},                 //Rose
			    {29, 0},{30, 1},{31, 2},{32, 3},{33, 4},{34, 5}, //Haschel
			    {23, 0},{24, 1},{25, 2},{26, 3},{27, 4},         //Meru
			    {19, 0},{20, 1},{21, 2},                         //Kongol
			    {255, 0}
            };

            for (int slot = 0; slot < 3; slot++) {
                int character = Globals.PARTY_SLOT[slot];
                if (Globals.PARTY_SLOT[slot] < 9) {
                    int addition = additionnum[Emulator.ReadByte("CHAR_TABLE", (character * 0x2C) + 0x19)];
                    int level = Emulator.ReadByte("CHAR_TABLE", (character * 0x2C) + 0x1A + addition);
                    int newlevel = 1 + Emulator.ReadByte("CHAR_TABLE", (character * 0x2C) + 0x22 + addition) / 20;
                    if (newlevel > level) {
                        Constants.WriteDebug(newlevel);
                        Emulator.WriteByte(Constants.GetAddress("CHAR_TABLE") + (character * 0x2C) + 0x1A + addition, (byte) newlevel);
                        Globals.CHARACTER_TABLE[slot].Add_DMG_Multi = Globals.DICTIONARY.AdditionData[character, addition, newlevel].ADD_DMG_Multi;
                        Globals.CHARACTER_TABLE[slot].Add_SP_Multi = Globals.DICTIONARY.AdditionData[character, addition, newlevel].ADD_SP_Multi;
                    }
                }
            }
        }
        #endregion

        #region Black Room

        public static void BlackRoomField() {
            if ((Globals.MAP >= 5 && Globals.MAP <= 7) || (Globals.MAP >= 624 && Globals.MAP <= 625)) {
                Emulator.WriteByte("BATTLE_FIELD", 96);
            }
        }

        public static void BlackRoomBattle() {
            if ((Globals.MAP >= 5 && Globals.MAP <= 7) || (Globals.MAP >= 624 && Globals.MAP <= 625)) {
                WipeRewards();
                for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                    Globals.MONSTER_TABLE[i].Write("HP", 65535);
                    Globals.MONSTER_TABLE[i].Write("Max_HP", 65535);
                    Globals.MONSTER_TABLE[i].Write("SPD", 0);
                    Globals.MONSTER_TABLE[i].Write("AT", 0);
                    Globals.MONSTER_TABLE[i].Write("MAT", 0);
                    Globals.MONSTER_TABLE[i].Write("DF", 65535);
                    Globals.MONSTER_TABLE[i].Write("MDF", 65535);
                    Globals.MONSTER_TABLE[i].Write("P_Immune", 1);
                    Globals.MONSTER_TABLE[i].Write("M_Immune", 1);
                    Globals.MONSTER_TABLE[i].Write("Turn", 0);
                }
            }
        }

        #endregion

        #region Never Guard
        public static void NeverGuard() {
            for (int i = 0; i < 3; i++) {
                if (Globals.PARTY_SLOT[i] > 8) {
                    break;
                }
                Globals.CHARACTER_TABLE[i].Guard = 0;
            }
        }

        #endregion

        #region Damage Cap Removal
        public static void RemoveDamageCap() {
            if (!firstDamageCapRemoval) {
                Emulator.WriteUInt("DAMAGE_CAP", 50000);
                Emulator.WriteUInt("DAMAGE_CAP", 50000, 0x8);
                Emulator.WriteUInt("DAMAGE_CAP", 50000, 0x14);
                DamageCapScan();
                firstDamageCapRemoval = true;
            } else {
                ushort currentItem = Emulator.ReadUShort(Globals.M_POINT + 0xABC);
                if (lastItemUsedDamageCap != currentItem) {
                    lastItemUsedDamageCap = currentItem;
                    if ((lastItemUsedDamageCap >= 0xC1 && lastItemUsedDamageCap <= 0xCA) || (lastItemUsedDamageCap >= 0xCF && lastItemUsedDamageCap <= 0xD2) || lastItemUsedDamageCap == 0xD6 || lastItemUsedDamageCap == 0xD8 || lastItemUsedDamageCap == 0xDC || (lastItemUsedDamageCap >= 0xF1 && lastItemUsedDamageCap <= 0xF8) || lastItemUsedDamageCap == 0xFA) {
                        DamageCapScan();
                    }
                }
                for (int i = 0; i < 3; i++) {
                    if (Globals.PARTY_SLOT[i] < 9) {
                        if (Globals.CHARACTER_TABLE[i].Action == 24) {
                            DamageCapScan();
                        }
                    }
                }
                for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                    if (Globals.MONSTER_TABLE[i].Read("Action") == 28) { //Most used, not all monsters use action code 28 for item spells
                        DamageCapScan();
                    }
                }
            }
        }

        public static void DamageCapScan() {
            var damageCapScan = Emulator.ScanAoB(0xA8660, 0x2A865F, "0F 27");
            long lastAddress = 0;
            foreach (var address in damageCapScan) {
                long capAddress = (long) address;
                if (Emulator.ReadUShort(capAddress) == 9999 && (lastAddress + 0x10) == capAddress) {
                    Emulator.WriteUInt(capAddress, 50000);
                    Emulator.WriteUInt(lastAddress, 50000);
                }
                lastAddress = capAddress;
            }
        }

        #endregion

        #region Change Aspect Ratio

        public static void ChangeAspectRatio() {
            ushort aspectRatio = 4096;

            if (aspectRatioOption == 0)
                aspectRatio = 4096;
            else if (aspectRatioOption == 1)
                aspectRatio = 3072;
            else if (aspectRatioOption == 2)
                aspectRatio = 3413;
            else if (aspectRatioOption == 3)
                aspectRatio = 2340;
            else if (aspectRatioOption == 4)
                aspectRatio = 2048;

            Emulator.WriteUShort("ASPECT_RATIO", aspectRatio);

            if (cameraOption == 1)
                Emulator.WriteUShort("ADVANCED_CAMERA", aspectRatio);
        }

        #endregion

        #region Kill BGM

        public static void KillBGM() {
            var bgmScan = Emulator.ScanAoB(0xA8660, 0x2A865F, "53 53 73 71");
            foreach (var address in bgmScan) {
                for (int i = 0; i <= 255; i++) {
                    Emulator.WriteByte((long) address + i, 0);
                    Thread.Sleep(10);
                }
            }
            Constants.WriteGLogOutput("Killed BGM.");
        }

        #endregion

        #region No Dragoon
        public static void NoDragoonMode() {
            for (int slot = 0; slot < 3; slot++) {
                if (Globals.PARTY_SLOT[slot] > 8) {
                    break;
                }
                Globals.CHARACTER_TABLE[slot].Dragoon = 0;
            }
        }
        #endregion

        #endregion

        #region Hard/Hell Mode specific


        #region Hard / Hell Mode Dragoon Changes

        public static void AddBurnStacks(int stacks, int slot) {
            dartPreviousBurnStacks = dartBurnStacks;
            dartBurnStacks = Math.Min(dartMaxBurnStacks, dartBurnStacks + stacks);
            if (dartBurnStacks >= 4 && dartPreviousBurnStacks < 4) {
                dartBurnMPHeal = true;
            } else if (dartBurnStacks >= 8 && dartPreviousBurnStacks < 8) {
                dartBurnMPHeal = true;
            } else if (dartBurnStacks >= 12 && dartPreviousBurnStacks < 12) {
                dartBurnMPHeal = true;
            }
            Constants.WriteGLogOutput("Dart Burn Stacks: " + dartBurnStacks + " / " + dartMaxBurnStacks);
        }

        public static void DartBurnStackHandler() {
            for (int i = 0; i < 3; i++) {
                if (Globals.PARTY_SLOT[i] == 0) {
                    if (resetBurnStack) {
                        if (Globals.CHARACTER_TABLE[i].Action == 8 || Globals.CHARACTER_TABLE[i].Action == 10) {
                            dartBurnStacks = 0;
                            dartPreviousBurnStacks = 0;
                            Emulator.WriteText(Globals.DRAGOON_SPELLS[0].Description_Pointer + 0x1E, "1.00");
                            Emulator.WriteText(Globals.DRAGOON_SPELLS[1].Description_Pointer + 0x1E, "1.00");
                            Emulator.WriteText(Globals.DRAGOON_SPELLS[2].Description_Pointer + 0x1E, "1.00");
                            Emulator.WriteText(Globals.DRAGOON_SPELLS[3].Description_Pointer + 0x20, "1.00");
                            Globals.CHARACTER_TABLE[i].MP = 0;
                            Globals.CHARACTER_TABLE[i].Spell_Cast = 255;
                            burnStackSelected = false;
                            resetBurnStack = false;
                            Constants.WriteGLogOutput("Dart Burn Stacks: " + dartBurnStacks + " / " + dartMaxBurnStacks);
                        }
                    }

                    if (Globals.CHARACTER_TABLE[i].Action == 10 && dartPreviousAction[i] != 10) {
                        dartBurnMP[i] = Globals.CHARACTER_TABLE[i].MP;
                    }

                    if (dartPreviousAction[i] == 26 && dartBurnMPHeal) {
                        if (dartBurnStacks >= 4 && dartPreviousBurnStacks < 4) {
                            Globals.CHARACTER_TABLE[i].MP = (ushort) Math.Min(Globals.CHARACTER_TABLE[i].MP + 10, Globals.CHARACTER_TABLE[i].Max_MP);
                        } else if (dartBurnStacks >= 8 && dartPreviousBurnStacks < 8) {
                            Globals.CHARACTER_TABLE[i].MP = (ushort) Math.Min(Globals.CHARACTER_TABLE[i].MP + 20, Globals.CHARACTER_TABLE[i].Max_MP);
                        } else if (dartBurnStacks >= 12 && dartPreviousBurnStacks < 12) {
                            Globals.CHARACTER_TABLE[i].MP = (ushort) Math.Min(Globals.CHARACTER_TABLE[i].MP + 30, Globals.CHARACTER_TABLE[i].Max_MP);
                        }
                        dartBurnMPHeal = false;
                    }

                    if (dartBurnStacks > 0) {
                        if (Globals.CHARACTER_TABLE[i].Action == 10) {
                            byte menu = Globals.CHARACTER_TABLE[i].Menu;

                            if (menu == 96 || menu == 98) {
                                byte[] icons = { 1, 4, 9, 3, 3 };
                                Globals.CHARACTER_TABLE[i].Menu = (byte) (225 - (96 - Globals.CHARACTER_TABLE[i].Menu));
                                Thread.Sleep(60);
                                for (int x = menu == 96 ? 1 : 0; x < icons.Length; x++) {
                                    Emulator.WriteByte(Globals.M_POINT + 0xD34 + (x - (menu == 96 ? 1 : 0)) * 0x2, icons[x]);
                                }
                            }

                            int iconCount = Emulator.ReadByte(Globals.M_POINT + 0xD32);
                            int iconSelected = Emulator.ReadByte(Globals.M_POINT + 0xD46);

                            if (burnStackSelected) {
                                if ((iconCount == 4 && (iconSelected == 0 || iconSelected == 2)) || (iconCount == 5 && (iconSelected == 1 || iconSelected == 3))) {
                                    burnStackSelected = false;
                                    double burnAmount = 1 + (dartBurnStacks * damagePerBurn);
                                    Emulator.WriteText(Globals.DRAGOON_SPELLS[0].Description_Pointer + 0x1E, "1.00");
                                    Emulator.WriteText(Globals.DRAGOON_SPELLS[1].Description_Pointer + 0x1E, "1.00");
                                    Emulator.WriteText(Globals.DRAGOON_SPELLS[2].Description_Pointer + 0x1E, "1.00");
                                    Emulator.WriteText(Globals.DRAGOON_SPELLS[3].Description_Pointer + 0x20, "1.00");

                                    if (dartBurnStacks == dartMaxBurnStacks) {
                                        Emulator.WriteByte("SPELL_TABLE", 10, 0x7 + (0 * 0xC));
                                        Emulator.WriteByte("SPELL_TABLE", 20, 0x7 + (1 * 0xC));
                                        Emulator.WriteByte("SPELL_TABLE", 30, 0x7 + (2 * 0xC));
                                        Emulator.WriteByte("SPELL_TABLE", 80, 0x7 + (3 * 0xC));
                                    }
                                }
                            } else {
                                if ((iconCount == 4 && (iconSelected == 1 || iconSelected == 3)) || (iconCount == 5 && (iconSelected == 2 || iconSelected == 4))) {
                                    burnStackSelected = true;
                                    double burnAmount = 1 + (dartBurnStacks * damagePerBurn);
                                    bool max = dartBurnStacks == dartMaxBurnStacks;
                                    string flameshotDesc = Convert.ToString(max ? (burnAmount * burnStackMaxFlameshotMulti) : (burnAmount * burnStackFlameshotMulti));
                                    string explosionDesc = Convert.ToString(max ? (burnAmount * burnStackMaxExplosionMulti) : (burnAmount * burnStackExplosionMulti));
                                    string finalBurstDesc = Convert.ToString(max ? (burnAmount * burnStackMaxFinalBurstMulti) : (burnAmount * burnStackFinalBurstMulti));
                                    string redEyeDesc = Convert.ToString(max ? (burnAmount * burnStackMaxRedEyeMulti) : (burnAmount * burnStackRedEyeMulti));
                                    Emulator.WriteText(Globals.DRAGOON_SPELLS[0].Description_Pointer + 0x1E, flameshotDesc.Substring(0, Math.Min(4, flameshotDesc.Length)));
                                    Emulator.WriteText(Globals.DRAGOON_SPELLS[1].Description_Pointer + 0x1E, explosionDesc.Substring(0, Math.Min(4, explosionDesc.Length)));
                                    Emulator.WriteText(Globals.DRAGOON_SPELLS[2].Description_Pointer + 0x1E, finalBurstDesc.Substring(0, Math.Min(4, finalBurstDesc.Length)));
                                    Emulator.WriteText(Globals.DRAGOON_SPELLS[3].Description_Pointer + 0x20, redEyeDesc.Substring(0, Math.Min(4, redEyeDesc.Length)));

                                    if (dartBurnStacks == dartMaxBurnStacks) {
                                        Emulator.WriteByte("SPELL_TABLE", 1, 0x7 + (0 * 0xC));
                                        Emulator.WriteByte("SPELL_TABLE", 1, 0x7 + (1 * 0xC));
                                        Emulator.WriteByte("SPELL_TABLE", 1, 0x7 + (2 * 0xC));
                                        Emulator.WriteByte("SPELL_TABLE", 1, 0x7 + (3 * 0xC));
                                    }
                                }
                            }
                        } else if (Globals.CHARACTER_TABLE[i].Action == 26) {
                            if (dartPreviousAction[i] == 10 && dartBurnMP[i] == Globals.CHARACTER_TABLE[i].MP) {
                                byte dlv = Globals.CHARACTER_TABLE[i].DLV;
                                AddBurnStacks(dlv == 5 ? 2 : 1, i);
                            }

                            if (burnStackSelected) {
                                resetBurnStack = true;
                            }
                        }
                    } else {
                        if (Globals.CHARACTER_TABLE[i].Action == 26) {
                            if (dartPreviousAction[i] == 10 && dartBurnMP[i] == Globals.CHARACTER_TABLE[i].MP) {
                                byte dlv = Globals.CHARACTER_TABLE[i].DLV;
                                AddBurnStacks(dlv == 5 ? 2 : 1, i);
                            }
                        }
                    }

                    dartPreviousAction[i] = Globals.CHARACTER_TABLE[i].Action;
                }
            }
        }

        public static void MagicInventoryHandler() {
            for (int i = 0; i < 3; i++) {
                if (Globals.PARTY_SLOT[i] == 2 || Globals.PARTY_SLOT[i] == 8) {
                    if (Globals.CHARACTER_TABLE[i].Action == 10) {

                    }

                        /*if (dartBurnStacks > 0) {
                        if (Globals.CHARACTER_TABLE[i].Read("Action") == 10) {
                            int menu = Globals.CHARACTER_TABLE[i].Read("Menu");

                            if (menu == 96 || menu == 98) {
                                byte[] icons = { 1, 4, 9, 3, 3 };
                                Globals.CHARACTER_TABLE[i].Write("Menu", 225 - (96 - Globals.CHARACTER_TABLE[i].Read("Menu")));
                                Thread.Sleep(60);
                                for (int x = menu == 96 ? 1 : 0; x < icons.Length; x++) {
                                    Emulator.WriteByte(Globals.M_POINT + 0xD34 + (x - (menu == 96 ? 1 : 0)) * 0x2, icons[x]);
                                }
                            }

                            int iconCount = Emulator.ReadByte(Globals.M_POINT + 0xD32);
                            int iconSelected = Emulator.ReadByte(Globals.M_POINT + 0xD46);

                            if (burnStackSelected) {
                                if ((iconCount == 4 && (iconSelected == 0 || iconSelected == 2)) || (iconCount == 5 && (iconSelected == 1 || iconSelected == 3))) {
                                    burnStackSelected = false;
                                    double burnAmount = 1 + (dartBurnStacks * damagePerBurn);
                                    Emulator.WriteText(Globals.DRAGOON_SPELLS[0].Description_Pointer + 0x1E, "1.00");
                                    Emulator.WriteText(Globals.DRAGOON_SPELLS[1].Description_Pointer + 0x1E, "1.00");
                                    Emulator.WriteText(Globals.DRAGOON_SPELLS[2].Description_Pointer + 0x1E, "1.00");
                                    Emulator.WriteText(Globals.DRAGOON_SPELLS[3].Description_Pointer + 0x20, "1.00");

                                    if (dartBurnStacks == dartMaxBurnStacks) {
                                        Emulator.WriteByte("SPELL_TABLE", 10, 0x7 + (0 * 0xC));
                                        Emulator.WriteByte("SPELL_TABLE", 20, 0x7 + (1 * 0xC));
                                        Emulator.WriteByte("SPELL_TABLE", 30, 0x7 + (2 * 0xC));
                                        Emulator.WriteByte("SPELL_TABLE", 80, 0x7 + (3 * 0xC));
                                    }
                                }
                            } else {
                                if ((iconCount == 4 && (iconSelected == 1 || iconSelected == 3)) || (iconCount == 5 && (iconSelected == 2 || iconSelected == 4))) {
                                    burnStackSelected = true;
                                    double burnAmount = 1 + (dartBurnStacks * damagePerBurn);
                                    bool max = dartBurnStacks == dartMaxBurnStacks;
                                    string flameshotDesc = Convert.ToString(max ? (burnAmount * burnStackMaxFlameshotMulti) : (burnAmount * burnStackFlameshotMulti));
                                    string explosionDesc = Convert.ToString(max ? (burnAmount * burnStackMaxExplosionMulti) : (burnAmount * burnStackExplosionMulti));
                                    string finalBurstDesc = Convert.ToString(max ? (burnAmount * burnStackMaxFinalBurstMulti) : (burnAmount * burnStackFinalBurstMulti));
                                    string redEyeDesc = Convert.ToString(max ? (burnAmount * burnStackMaxRedEyeMulti) : (burnAmount * burnStackRedEyeMulti));
                                    Emulator.WriteText(Globals.DRAGOON_SPELLS[0].Description_Pointer + 0x1E, flameshotDesc.Substring(0, Math.Min(4, flameshotDesc.Length)));
                                    Emulator.WriteText(Globals.DRAGOON_SPELLS[1].Description_Pointer + 0x1E, explosionDesc.Substring(0, Math.Min(4, explosionDesc.Length)));
                                    Emulator.WriteText(Globals.DRAGOON_SPELLS[2].Description_Pointer + 0x1E, finalBurstDesc.Substring(0, Math.Min(4, finalBurstDesc.Length)));
                                    Emulator.WriteText(Globals.DRAGOON_SPELLS[3].Description_Pointer + 0x20, redEyeDesc.Substring(0, Math.Min(4, redEyeDesc.Length)));

                                    if (dartBurnStacks == dartMaxBurnStacks) {
                                        Emulator.WriteByte("SPELL_TABLE", 1, 0x7 + (0 * 0xC));
                                        Emulator.WriteByte("SPELL_TABLE", 1, 0x7 + (1 * 0xC));
                                        Emulator.WriteByte("SPELL_TABLE", 1, 0x7 + (2 * 0xC));
                                        Emulator.WriteByte("SPELL_TABLE", 1, 0x7 + (3 * 0xC));
                                    }
                                }
                            }
                        }
                    }*/
                }
            }
        }
        #endregion

        #region Dual Difficulty

        public static void SwitchDualDifficulty() {
            bool boss = false;
            string cwd = AppDomain.CurrentDomain.BaseDirectory;
            string monsterBoss = difficulty == "HardHell" ? "Hell_Mode" : "Hard_Mode";
            string monsterDefault = monsterBoss == "Hell_Mode" ? "Hard_Mode" : "US_Base";
            string mod;

            if (Globals.ENCOUNTER_ID == 384 || //Commander
                Globals.ENCOUNTER_ID == 386 || //Fruegel I
                Globals.ENCOUNTER_ID == 414 || //Urobolus
                Globals.ENCOUNTER_ID == 385 || //Sandora Elite
                Globals.ENCOUNTER_ID == 388 || //Kongol I
                Globals.ENCOUNTER_ID == 408 || //Virage I
                Globals.ENCOUNTER_ID == 415 || //Fire Bird
                Globals.ENCOUNTER_ID == 393 || //Greham + Feyrbrand
                Globals.ENCOUNTER_ID == 412 || //Drake the Bandit
                Globals.ENCOUNTER_ID == 413 || //Jiango
                Globals.ENCOUNTER_ID == 387 || //Fruegel II
                Globals.ENCOUNTER_ID == 461 || //Sandora Elite II
                Globals.ENCOUNTER_ID == 389 || //Kongol II
                Globals.ENCOUNTER_ID == 390 || //Emperor Doel
                Globals.ENCOUNTER_ID == 402 || //Mappi
                Globals.ENCOUNTER_ID == 409 || //Virage II
                Globals.ENCOUNTER_ID == 403 || //Gehrich + Mappi
                Globals.ENCOUNTER_ID == 396 || //Lenus
                Globals.ENCOUNTER_ID == 417 || //Ghost Commander
                Globals.ENCOUNTER_ID == 397 || //Lenus + Regole
                Globals.ENCOUNTER_ID == 418 || //Kamuy
                Globals.ENCOUNTER_ID == 410 || //S Virage
                Globals.ENCOUNTER_ID == 416 || //Grand Jewel
                Globals.ENCOUNTER_ID == 394 || //Divine Dragon
                Globals.ENCOUNTER_ID == 422 || //Windigo
                Globals.ENCOUNTER_ID == 392 || //Lloyd
                Globals.ENCOUNTER_ID == 423 || //Polter Set
                Globals.ENCOUNTER_ID == 398 || //Damia
                Globals.ENCOUNTER_ID == 399 || //Syuveil
                Globals.ENCOUNTER_ID == 400 || //Belzac
                Globals.ENCOUNTER_ID == 401 || //Kanzas
                Globals.ENCOUNTER_ID == 420 || //Magician Faust
                Globals.ENCOUNTER_ID == 432 || //Last Kraken
                Globals.ENCOUNTER_ID == 430 || //Executioners
                Globals.ENCOUNTER_ID == 449 || //Spirit (Feyrbrand)
                Globals.ENCOUNTER_ID == 448 || //Spirit (Regole)
                Globals.ENCOUNTER_ID == 447 || //Spirit (Divine Dragon)
                Globals.ENCOUNTER_ID == 431 || //Zackwell
                Globals.ENCOUNTER_ID == 433 || //Imago
                Globals.ENCOUNTER_ID == 411 || //S Virage II
                Globals.ENCOUNTER_ID == 442 || //Zieg
                Globals.ENCOUNTER_ID == 443) { //Melbu Fraahma
                boss = true;
            }

            mod = boss ? monsterBoss : monsterDefault;
            Globals.DICTIONARY.SwapMonsterStats(mod);
        }

        #endregion

        #region Boss SP Loss

        public static void SetSPLossAmmount() {
            if (Globals.ENCOUNTER_ID == 384) //Marsh Commander
                bossSPLoss = 500;
            else if (Globals.ENCOUNTER_ID == 386) //Fruegel I
                bossSPLoss = 250;
            else if (Globals.ENCOUNTER_ID == 414) //Urobolus
                bossSPLoss = 750;
            else if (Globals.ENCOUNTER_ID == 388) //Kongol I
                bossSPLoss = 2000;
            else if (Globals.ENCOUNTER_ID == 408) //Virage I
                bossSPLoss = 250;
            else if (Globals.ENCOUNTER_ID == 415) //Fire Bird
                bossSPLoss = 1000;
            else if (Globals.ENCOUNTER_ID == 393) //Greham + Ferybrand
                bossSPLoss = 1500;
            else if (Globals.ENCOUNTER_ID == 412) //Drake the Bandit
                bossSPLoss = -3;
            else if (Globals.ENCOUNTER_ID == 413) //Jiango
                bossSPLoss = 500;
            else if (Globals.ENCOUNTER_ID == 387) //Fruegel II
                bossSPLoss = 1500;
            else if (Globals.ENCOUNTER_ID == 390) //Dragoon Doel
                bossSPLoss = -4;
            else if (Globals.ENCOUNTER_ID == 402) //Mappi + Craft Theif
                bossSPLoss = 1000;
            else if (Globals.ENCOUNTER_ID == 409) //Virage II
                bossSPLoss = 1000;
            else if (Globals.ENCOUNTER_ID == 403) //Gehrich + Mappi
                bossSPLoss = 2000;
            else if (Globals.ENCOUNTER_ID == 396) //Lenus
                bossSPLoss = -2;
            else if (Globals.ENCOUNTER_ID == 417) //Ghost Commander
                bossSPLoss = 1000;
            else if (Globals.ENCOUNTER_ID == 397) //Lenus II
                bossSPLoss = -4;
            else if (Globals.ENCOUNTER_ID == 410) //S Virage I
                bossSPLoss = 2000;
            else if (Globals.ENCOUNTER_ID == 416) //Grand Jewel
                bossSPLoss = 1000;
            else if (Globals.ENCOUNTER_ID == 394) //Divine Dragon
                bossSPLoss = -2;
            else if (Globals.ENCOUNTER_ID == 392) //Lloyd
                bossSPLoss = -5;
            else if (Globals.ENCOUNTER_ID == 423) //Polter Set
                bossSPLoss = 500;
            else if (Globals.ENCOUNTER_ID == 432) //Last Kraken
                bossSPLoss = -1;
            else if (Globals.ENCOUNTER_ID == 430) //Executioners
                bossSPLoss = 1000;
            else if (Globals.ENCOUNTER_ID == 431) //Zackwell
                bossSPLoss = 1000;
            else if (Globals.ENCOUNTER_ID == 433) //Imago
                bossSPLoss = 1000;
            else
                bossSPLoss = 0;

            if (Globals.CheckDMScript("btnUltimateBoss"))
                bossSPLoss = 0;
        }

        public static void ReduceSP() {
            if (bossSPLoss != 0) {
                for (int character = 0; character < 9; character++) {
                    ushort currentTotalSP = Emulator.ReadUShort("CHAR_TABLE", (character * 0x2C) + 0xE);
                    int newSP = 0;
                    if (bossSPLoss > 0)
                        newSP = Math.Max(currentTotalSP - bossSPLoss, 0);
                    else if (bossSPLoss == -1)
                        newSP = (int) Math.Max(Math.Round(currentTotalSP / 1.2), 0);
                    else if (bossSPLoss == -2)
                        newSP = Math.Max(currentTotalSP / 2, 0);
                    else if (bossSPLoss == -3)
                        newSP = Math.Max(currentTotalSP / 2 - 500, 0);
                    else if (bossSPLoss == -4)
                        newSP = Math.Max(currentTotalSP / 4, 0);
                    else if (bossSPLoss == -5)
                        newSP = Math.Max(currentTotalSP / 4 - 500, 0);

                    Emulator.WriteUShort("CHAR_TABLE", (ushort) newSP, (character * 0x2C) + 0xE);

                    byte dragoonLevel = Emulator.ReadByte("CHAR_TABLE", (character * 0x2C) + 0x13);

                    if (character == 0 || character == 3) {
                        if (newSP < 20000 && dragoonLevel >= 5)
                            Emulator.WriteByte("CHAR_TABLE", 4, (character * 0x2C) + 0x13);
                        if (newSP < 12000 && dragoonLevel >= 4)
                            Emulator.WriteByte("CHAR_TABLE", 3, (character * 0x2C) + 0x13);
                        if (newSP < 6000 && dragoonLevel >= 3)
                            Emulator.WriteByte("CHAR_TABLE", 2, (character * 0x2C) + 0x13);
                        if (newSP < 1200 && dragoonLevel >= 2)
                            Emulator.WriteByte("CHAR_TABLE", 1, (character * 0x2C) + 0x13);
                    } else if (character == 6 || character == 7) {
                        if (newSP < 20000 && dragoonLevel >= 5)
                            Emulator.WriteByte("CHAR_TABLE", 4, (character * 0x2C) + 0x13);
                        if (newSP < 12000 && dragoonLevel >= 4)
                            Emulator.WriteByte("CHAR_TABLE", 3, (character * 0x2C) + 0x13);
                        if (newSP < 2000 && dragoonLevel >= 3)
                            Emulator.WriteByte("CHAR_TABLE", 2, (character * 0x2C) + 0x13);
                        if (newSP < 1200 && dragoonLevel >= 2)
                            Emulator.WriteByte("CHAR_TABLE", 1, (character * 0x2C) + 0x13);
                    } else {
                        if (newSP < 20000 && dragoonLevel >= 5)
                            Emulator.WriteByte("CHAR_TABLE", 4, (character * 0x2C) + 0x13);
                        if (newSP < 12000 && dragoonLevel >= 4)
                            Emulator.WriteByte("CHAR_TABLE", 3, (character * 0x2C) + 0x13);
                        if (newSP < 6000 && dragoonLevel >= 3)
                            Emulator.WriteByte("CHAR_TABLE", 2, (character * 0x2C) + 0x13);
                        if (newSP < 1000 && dragoonLevel >= 2)
                            Emulator.WriteByte("CHAR_TABLE", 1, (character * 0x2C) + 0x13);
                    }
                }
                bossSPLoss = 0;
            }
        }

        #endregion

        #endregion

        #region Extras
        public static void WipeRewards() {
            for (int i = 0; i < Globals.UNIQUE_MONSTER_SIZE; i++) {
                Emulator.WriteUShort("MONSTER_REWARDS", 0, i * 0x1A8);
                Emulator.WriteUShort("MONSTER_REWARDS", 0, 0x2 + i * 0x1A8);
                Emulator.WriteByte("MONSTER_REWARDS", 0, 0x4 + i * 0x1A8);
                Emulator.WriteByte("MONSTER_REWARDS", 0, 0x5 + i * 0x1A8);
            }
        }
        #endregion

        #region Hotkeys

        public static void BattleHotkeys() {
            if (Globals.HOTKEY == (Hotkey.KEY_L1 + Hotkey.KEY_UP)) { //Exit Dragoon Slot 1
                if (Emulator.ReadByte("DRAGOON_TURNS") > 1) {
                    Emulator.WriteByte("DRAGOON_TURNS", 1);
                    Constants.WriteGLogOutput("Slot 1 will exit Dragoon after next action.");
                    Globals.LAST_HOTKEY = Constants.GetTime();
                }
            } else if (Globals.HOTKEY == (Hotkey.KEY_L1 + Hotkey.KEY_RIGHT)) { //Exit Dragoon Slot 2
                if (Emulator.ReadByte("DRAGOON_TURNS", 0x4) > 1) {
                    Emulator.WriteByte("DRAGOON_TURNS", 1, 0x4);
                    Constants.WriteGLogOutput("Slot 2 will exit Dragoon after next action.");
                    Globals.LAST_HOTKEY = Constants.GetTime();
                }
            } else if (Globals.HOTKEY == (Hotkey.KEY_L1 + Hotkey.KEY_LEFT)) { //Exit Dragoon Slot 3
                if (Emulator.ReadByte("DRAGOON_TURNS", 0x8) > 1) {
                    Emulator.WriteByte("DRAGOON_TURNS", 1, 0x8);
                    Constants.WriteGLogOutput("Slot 3 will exit Dragoon after next action.");
                    Globals.LAST_HOTKEY = Constants.GetTime();
                }
            } else if (Globals.HOTKEY == (Hotkey.KEY_CIRCLE + Hotkey.KEY_RIGHT)) { //Dragon Beater
                if (dragonBeaterSlot != 0) {
                    if (!Globals.DIFFICULTY_MODE.Equals("Normal") && !checkRoseDamage) {
                        if (roseEnhanceDragoon) {
                            if (Constants.REGION == Region.NTA) {
                                Emulator.WriteAoB(Globals.DRAGOON_SPELLS[15].Description_Pointer, "22 00 39 00 4A 00 43 00 00 00 31 00 32 00 30 00 00 00 1A 00 1E 00 15 00 0F 00 00 00 10 00 00 00 26 00 2E 00 FF A0");
                                Emulator.WriteAoB(Globals.DRAGOON_SPELLS[16].Description_Pointer, "22 00 39 00 4A 00 43 00 00 00 31 00 32 00 30 00 00 00 18 00 1E 00 1A 00 0F 00 FF A0");
                                Emulator.WriteAoB(Globals.DRAGOON_SPELLS[19].Description_Pointer, "22 00 39 00 4A 00 43 00 00 00 31 00 32 00 30 00 00 00 16 00 1B 00 1D 00 15 00 0F 00 00 00 10 00 00 00 26 00 2E 00 FF A0");
                            }
                            Emulator.WriteByte("SPELL_TABLE", astralDrainMP, 0x7 + (15 * 0xC)); //Astral Drain MP
                            Emulator.WriteByte("SPELL_TABLE", deathDimensionMP, 0x7 + (16 * 0xC)); //Death Dimension MP
                            Emulator.WriteByte("SPELL_TABLE", darkDragonMP, 0x7 + (19 * 0xC)); //Dark Dragon MP
                            roseEnhanceDragoon = false;
                            Constants.WriteGLogOutput("Rose's dragoon magic has returned to normal.");
                        } else {
                            if (Constants.REGION == Region.NTA) {
                                Emulator.WriteAoB(Globals.DRAGOON_SPELLS[15].Description_Pointer, "22 00 39 00 4A 00 43 00 00 00 31 00 32 00 30 00 00 00 1D 00 17 00 1A 00 0F 00 00 00 10 00 00 00 26 00 2E 00 FF A0");
                                Emulator.WriteAoB(Globals.DRAGOON_SPELLS[16].Description_Pointer, "22 00 39 00 4A 00 43 00 00 00 31 00 32 00 30 00 00 00 1C 00 1E 00 1A 00 0F 00 FF A0");
                                Emulator.WriteAoB(Globals.DRAGOON_SPELLS[19].Description_Pointer, "22 00 39 00 4A 00 43 00 00 00 31 00 32 00 30 00 00 00 16 00 16 00 1A 00 15 00 0F 00 00 00 10 00 00 00 26 00 2E 00 FF A0");
                            }
                            Emulator.WriteByte("SPELL_TABLE", enhancedAstralDrainMP, 0x7 + (15 * 0xC)); //Astral Drain MP
                            Emulator.WriteByte("SPELL_TABLE", enhancedDeathDimensionMP, 0x7 + (16 * 0xC)); //Death Dimension MP
                            Emulator.WriteByte("SPELL_TABLE", enhancedDarkDragonMP, 0x7 + (19 * 0xC)); //Dark Dragon MP
                            roseEnhanceDragoon = true;
                            Constants.WriteGLogOutput("Rose will now consume more MP for bonus effects.");
                        }
                    } else {
                        Constants.WriteGLogOutput("You can't swap MP modes right now.");
                    }
                } else {
                    Constants.WriteGLogOutput("Dragon Beater not equipped.");
                }
                Globals.LAST_HOTKEY = Constants.GetTime();
            } else if (Globals.HOTKEY == (Hotkey.KEY_CIRCLE + Hotkey.KEY_DOWN)) {
                if (jeweledHammerSlot != 0) {
                    if (!Globals.DIFFICULTY_MODE.Equals("Normal")) {
                        if (meruEnhanceDragoon) {
                            if (Constants.REGION == Region.NTA) {
                                Emulator.WriteAoB(Globals.DRAGOON_SPELLS[24].Description_Pointer, "35 00 39 00 4C 00 3D 00 4A 00 00 00 31 00 32 00 30 00 00 00 1A 00 16 00 15 00 0F 00 FF A0");
                                Emulator.WriteAoB(Globals.DRAGOON_SPELLS[27].Description_Pointer, "35 00 39 00 4C 00 3D 00 4A 00 00 00 31 00 32 00 30 00 00 00 1A 00 18 00 15 00 0F 00 FF A0");
                                Emulator.WriteAoB(Globals.DRAGOON_SPELLS[28].Description_Pointer, "35 00 39 00 4C 00 3D 00 4A 00 00 00 31 00 32 00 30 00 00 00 16 00 19 00 15 00 15 00 0F 00 FF A0");
                            }
                            Emulator.WriteByte("SPELL_TABLE", freezingRingMP, 0x7 + (24 * 0xC)); //Freezing Ring MP
                            Emulator.WriteByte("SPELL_TABLE", rainbowBreathMP, 0x7 + (25 * 0xC)); //Rainbow Breath MP
                            Emulator.WriteByte("SPELL_TABLE", diamonDustMP, 0x7 + (27 * 0xC)); //Diamond Dust MP
                            Emulator.WriteByte("SPELL_TABLE", blueSeaDragonMP, 0x7 + (28 * 0xC)); //Blue Sea Dragon MP
                            meruEnhanceDragoon = false;
                            Constants.WriteGLogOutput("Meru's dragoon magic has returned to normal.");
                        } else {
                            if (Constants.REGION == Region.NTA) {
                                Emulator.WriteAoB(Globals.DRAGOON_SPELLS[24].Description_Pointer, "35 00 39 00 4C 00 3D 00 4A 00 00 00 31 00 32 00 30 00 00 00 1D 00 15 00 15 00 0F 00 FF A0");
                                Emulator.WriteAoB(Globals.DRAGOON_SPELLS[27].Description_Pointer, "35 00 39 00 4C 00 3D 00 4A 00 00 00 31 00 32 00 30 00 00 00 1D 00 1D 00 15 00 0F 00 FF A0");
                                Emulator.WriteAoB(Globals.DRAGOON_SPELLS[28].Description_Pointer, "35 00 39 00 4C 00 3D 00 4A 00 00 00 31 00 32 00 30 00 00 00 17 00 16 00 15 00 15 00 0F 00 FF A0");
                            }
                            Emulator.WriteByte("SPELL_TABLE", enhancedFreezingRingMP, 0x7 + (24 * 0xC)); //Freezing Ring MP
                            Emulator.WriteByte("SPELL_TABLE", enhancedRainbowBreathMP, 0x7 + (25 * 0xC)); //Rainbow Breath MP
                            Emulator.WriteByte("SPELL_TABLE", enhancedDiamondDustMP, 0x7 + (27 * 0xC)); //Diamond Dust MP
                            Emulator.WriteByte("SPELL_TABLE", enhancedBlueSeaDragonMP, 0x7 + (28 * 0xC)); //Blue Sea Dragon MP
                            meruEnhanceDragoon = true;
                            Constants.WriteGLogOutput("Meru will now consume more MP for bonus effects.");
                        }
                    } else {
                        Constants.WriteGLogOutput("You can't swap MP modes right now.");
                    }
                } else {
                    Constants.WriteGLogOutput("Jeweled Hammer not equipped.");
                }
                Globals.LAST_HOTKEY = Constants.GetTime();
            }
        }

        #endregion
    }

    #region Objects
    public class MonsterAddress {
        long[] action = { 0, 1 };
        long[] hp = { 0, 2 };
        long[] max_hp = { 0, 2 };
        long[] element = { 0, 2 };
        long[] display_element = { 0, 2 };
        long[] guard = { 0, 1 };
        long[] at = { 0, 2 };
        long[] og_at = { 0, 2 };
        long[] mat = { 0, 2 };
        long[] og_mat = { 0, 2 };
        long[] df = { 0, 2 };
        long[] og_df = { 0, 2 };
        long[] mdf = { 0, 2 };
        long[] og_mdf = { 0, 2 };
        long[] spd = { 0, 2 };
        long[] og_spd = { 0, 2 };
        long[] turn = { 0, 2 };
        long[] a_av = { 0, 1 };
        long[] m_av = { 0, 1 };
        long[] pwr_at = { 0, 1 };
        long[] pwr_at_trn = { 0, 1 };
        long[] pwr_mat = { 0, 1 };
        long[] pwr_mat_trn = { 0, 1 };
        long[] pwr_df = { 0, 1 };
        long[] pwr_df_trn = { 0, 1 };
        long[] pwr_mdf = { 0, 1 };
        long[] pwr_mdf_trn = { 0, 1 };
        long[] speed_up_trn = { 0, 1 };
        long[] speed_down_trn = { 0, 1 };
        long[] p_immune = { 0, 1 };
        long[] m_immune = { 0, 1 };
        long[] p_half = { 0, 1 };
        long[] m_half = { 0, 1 };
        long[] e_immune = { 0, 1 };
        long[] e_half = { 0, 1 };
        long[] stat_res = { 0, 1 };
        long[] death_res = { 0, 1 };
        long[] unique_index = { 0, 1 };
        long[] exp = { 0, 2 };
        long[] gold = { 0, 2 };
        long[] drop_chance = { 0, 1 };
        long[] drop_item = { 0, 1 };
        long[] special_effect = { 0, 1 };
        long[] attack_move = { 0, 1 };

        public long[] Action { get { return action; } }
        public long[] HP { get { return hp; } }
        public long[] Max_HP { get { return max_hp; } }
        public long[] Element { get { return element; } }
        public long[] Display_Element { get { return display_element; } }
        public long[] Guard { get { return guard; } }
        public long[] AT { get { return at; } }
        public long[] OG_AT { get { return og_at; } }
        public long[] MAT { get { return mat; } }
        public long[] OG_MAT { get { return og_mat; } }
        public long[] DF { get { return df; } }
        public long[] OG_DF { get { return og_df; } }
        public long[] MDF { get { return mdf; } }
        public long[] OG_MDF { get { return og_mdf; } }
        public long[] SPD { get { return spd; } }
        public long[] OG_SPD { get { return og_spd; } }
        public long[] Turn { get { return turn; } }
        public long[] A_AV { get { return a_av; } }
        public long[] M_AV { get { return m_av; } }
        public long[] PWR_AT { get { return pwr_at; } }
        public long[] PWR_AT_TRN { get { return pwr_at_trn; } }
        public long[] PWR_MAT { get { return pwr_mat; } }
        public long[] PWR_MAT_TRN { get { return pwr_mat_trn; } }
        public long[] PWR_DF { get { return pwr_df; } }
        public long[] PWR_DF_TRN { get { return pwr_df_trn; } }
        public long[] PWR_MDF { get { return pwr_mdf; } }
        public long[] PWR_MDF_TRN { get { return pwr_mdf_trn; } }
        public long[] SPEED_UP_TRN { get { return speed_up_trn; } }
        public long[] SPEED_DOWN_TRN { get { return speed_down_trn; } }
        public long[] P_Immune { get { return p_immune; } }
        public long[] M_Immune { get { return m_immune; } }
        public long[] P_Half { get { return p_half; } }
        public long[] M_Half { get { return m_half; } }
        public long[] E_Immune { get { return e_immune; } }
        public long[] E_Half { get { return e_half; } }
        public long[] Stat_Res { get { return stat_res; } }
        public long[] Death_Res { get { return death_res; } }
        public long[] Unique_Index { get { return unique_index; } }
        public long[] EXP { get { return exp; } }
        public long[] Gold { get { return gold; } }
        public long[] Drop_Chance { get { return drop_chance; } }
        public long[] Drop_Item { get { return drop_item; } }
        public long[] Special_Effect { get { return special_effect; } }
        public long[] Attack_Move { get { return attack_move; } }

        public MonsterAddress(long m_point, int monster, int ID, List<int> monsterUniqueIdList) {
            action[0] = m_point - 0xA8 - monster * 0x388;
            hp[0] = m_point - monster * 0x388;
            max_hp[0] = m_point + 0x8 - monster * 0x388;
            element[0] = m_point + 0x14 - monster * 0x388;
            display_element[0] = m_point + 0x6A - monster * 0x388;
            guard[0] = m_point + 0x4C - monster * 0x388;
            at[0] = m_point + 0x2C - monster * 0x388;
            og_at[0] = m_point + 0x58 - monster * 0x388;
            mat[0] = m_point + 0x2E - monster * 0x388;
            og_mat[0] = m_point + 0x5A - monster * 0x388;
            df[0] = m_point + 0x30 - monster * 0x388;
            og_df[0] = m_point + 0x5E - monster * 0x388;
            mdf[0] = m_point + 0x32 - monster * 0x388;
            og_mdf[0] = m_point + 0x60 - monster * 0x388;
            spd[0] = m_point + 0x2A - monster * 0x388;
            og_spd[0] = m_point + 0x5C - monster * 0x388;
            turn[0] = m_point + 0x44 - monster * 0x388;
            a_av[0] = m_point + 0x38 - monster * 0x388;
            m_av[0] = m_point + 0x3A - monster * 0x388;
            pwr_at[0] = m_point + 0xAC - monster * 0x388;
            pwr_at_trn[0] = m_point + 0xAD - monster * 0x388;
            pwr_mat[0] = m_point + 0xAE - monster * 0x388;
            pwr_mat_trn[0] = m_point + 0xAF - monster * 0x388;
            pwr_df[0] = m_point + 0xB0 - monster * 0x388;
            pwr_df_trn[0] = m_point + 0xB1 - monster * 0x388;
            pwr_mdf[0] = m_point + 0xB2 - monster * 0x388;
            pwr_mdf_trn[0] = m_point + 0xB3 - monster * 0x388;
            speed_up_trn[0] = m_point + 0xC1 - monster * 0x388;
            speed_down_trn[0] = m_point + 0xC3 - monster * 0x388;
            p_immune[0] = m_point + 0x108 - monster * 0x388;
            m_immune[0] = m_point + 0x10A - monster * 0x388;
            p_half[0] = m_point + 0x10C - monster * 0x388;
            m_half[0] = m_point + 0x10E - monster * 0x388;
            e_immune[0] = m_point + 0x1A - monster * 0x388;
            e_half[0] = m_point + 0x18 - monster * 0x388;
            stat_res[0] = m_point + 0x1C - monster * 0x388;
            death_res[0] = m_point + 0xC - monster * 0x388;
            attack_move[0] = m_point + 0xACC - monster * 0x388;
            unique_index[0] = m_point + 0x264 - monster * 0x388;
            exp[0] = Constants.GetAddress("MONSTER_REWARDS") + (int) Constants.OFFSET + Globals.UNIQUE_MONSTER_IDS.IndexOf(ID) * 0x1A8;
            gold[0] = Constants.GetAddress("MONSTER_REWARDS") + (int) Constants.OFFSET + 0x2 + Globals.UNIQUE_MONSTER_IDS.IndexOf(ID) * 0x1A8;
            drop_chance[0] = Constants.GetAddress("MONSTER_REWARDS") + (int) Constants.OFFSET + 0x4 + Globals.UNIQUE_MONSTER_IDS.IndexOf(ID) * 0x1A8;
            drop_item[0] = Constants.GetAddress("MONSTER_REWARDS") + (int) Constants.OFFSET + 0x5 + Globals.UNIQUE_MONSTER_IDS.IndexOf(ID) * 0x1A8;
            special_effect[0] = Constants.GetAddress("UNIQUE_MONSTER_SIZE") + monster * 0x20;
        }

        public object Read(string attribute) {
            try {
                PropertyInfo property = GetType().GetProperty(attribute);
                var address = (long[]) property.GetValue(this, null);
                if (address[1] == 2) {
                    return Emulator.ReadUShort(address[0]);
                } else if (address[1] == 4) {
                    return Emulator.ReadUInt(address[0]);
                } else {
                    return Emulator.ReadByte(address[0]);
                }
            } catch (Exception e) {
                Constants.WriteError("Monster Read Error - A: " + attribute);
                Console.WriteLine("Monster Read Error - A: " + attribute);
                return 0;
            }
        }

        public void Write(string attribute, object value) {
            try {
                PropertyInfo property = GetType().GetProperty(attribute);
                var address = (long[]) property.GetValue(this, null);
                if (address[1] == 2) {
                    Emulator.WriteUShort(address[0], Convert.ToUInt16(value));
                } else if (address[1] == 4) {
                    Emulator.WriteUInt(address[0], Convert.ToUInt32(value));
                } else {;
                    Emulator.WriteByte(address[0], Convert.ToByte(value));
                }
            } catch (Exception e) {
                Constants.WriteError(e);
                Constants.WriteError("Monster Write Error - A: " + attribute + " V: " + value);
                Console.WriteLine("Monster Write Error - A: " + attribute + " V: " + value);
            }
        }
    }

    public class CurrentStats {
        byte lv = 1;
        byte dlv = 0;
        dynamic weapon = new System.Dynamic.ExpandoObject();
        dynamic armor = new System.Dynamic.ExpandoObject();
        dynamic helm = new System.Dynamic.ExpandoObject();
        dynamic boots = new System.Dynamic.ExpandoObject();
        dynamic accessory = new System.Dynamic.ExpandoObject();
        ushort hp = 1;
        ushort max_hp = 1;
        ushort mp = 0;
        ushort max_mp = 0;
        ushort sp = 0;
        ushort at = 1;
        ushort mat = 1;
        ushort df = 1;
        ushort mdf = 1;
        ushort spd = 1;
        ushort equip_spd = 0;
        ushort body_spd = 1;
        byte a_av = 0;
        byte m_av = 0;
        byte a_hit = 0;
        byte m_hit = 0;
        byte stat_res = 0;
        byte e_half = 0;
        byte e_immune = 0;
        byte p_half = 0;
        byte m_half = 0;
        byte on_hit_status = 0;
        byte status_chance = 0;
        byte revive = 0;
        ushort sp_regen = 0;
        ushort mp_regen = 0;
        ushort hp_regen = 0;
        byte mp_m_hit = 0;
        byte sp_m_hit = 0;
        byte mp_p_hit = 0;
        byte sp_p_hit = 0;
        byte sp_multi = 0;
        byte element = 0;
        byte death_res = 0;
        byte[] dragoon_spirits = new byte[] { 1, 4, 32, 64, 16, 4, 2, 8, 32, 128 };

        public byte LV { get { return lv; } }
        public byte DLV { get { return dlv; } }
        public ushort HP { get { return hp; } }
        public ushort Max_HP { get { return max_hp; } }
        public ushort MP { get { return mp; } }
        public ushort Max_MP { get { return max_mp; } }
        public ushort SP { get { return sp; } }
        public ushort AT { get { return at; } }
        public ushort MAT { get { return mat; } }
        public ushort DF { get { return df; } }
        public ushort MDF { get { return mdf; } }
        public ushort SPD { get { return spd; } }
        public ushort Body_SPD { get { return body_spd; } }
        public ushort Equip_SPD { get { return equip_spd; } }
        public byte A_AV { get { return a_av; } }
        public byte M_AV { get { return m_av; } }
        public byte A_Hit { get { return a_hit; } }
        public byte M_Hit { get { return m_hit; } }
        public byte Stat_Res { get { return stat_res; } }
        public byte E_Half { get { return e_half; } }
        public byte E_Immune { get { return e_immune; } }
        public byte P_Half { get { return p_half; } }
        public byte M_Half { get { return m_half; } }
        public byte On_Hit_Status { get { return on_hit_status; } }
        public byte Status_Chance { get { return status_chance; } }
        public byte Revive { get { return revive; } }
        public ushort SP_Regen { get { return sp_regen; } }
        public ushort MP_Regen { get { return mp_regen; } }
        public ushort HP_Regen { get { return hp_regen; } }
        public byte SP_M_Hit { get { return sp_m_hit; } }
        public byte MP_M_Hit { get { return mp_m_hit; } }
        public byte SP_P_Hit { get { return sp_p_hit; } }
        public byte MP_P_Hit { get { return mp_p_hit; } }
        public byte SP_Multi { get { return sp_multi; } }
        public byte Element { get { return element; } }
        public byte Death_Res { get { return death_res; } }
        public dynamic Weapon { get { return weapon; } }
        public dynamic Armor { get { return armor; } }
        public dynamic Helm { get { return helm; } }
        public dynamic Boots { get { return boots; } }
        public dynamic Accessory { get { return accessory; } }

        public CurrentStats(int character, int slot, int ultimateBossStage) {
            lv = Emulator.ReadByte("CHAR_TABLE", (character * 0x2C) + 0x12);

            if (Globals.CheckDMScript("btnUltimateBoss")) {
                if (ultimateBossStage < 22) {
                    if (ultimateBossStage < 3) {
                        if (lv > 30) lv = 30;
                    } else if (ultimateBossStage < 8) {
                        if (lv > 40) lv = 40;
                    } else if (ultimateBossStage < 22) {
                        if (lv > 50) lv = 50;
                    }
                    Globals.CHARACTER_TABLE[slot].LV = lv;
                }
            }

            if ((dragoon_spirits[character] & Emulator.ReadByte("DRAGOON_SPIRITS")) > 0) {
                dlv = Emulator.ReadByte("CHAR_TABLE", (character * 0x2C) + 0x13);
            }

            if (character == 0 && Emulator.ReadByte("DRAGOON_SPIRITS") >= 254) {
                dlv = Emulator.ReadByte("CHAR_TABLE", (character * 0x2C) + 0x13);
            }

            if (!Globals.ITEM_STAT_CHANGE) {
                weapon = Globals.DICTIONARY.OriginalItemList[Emulator.ReadByte("CHAR_TABLE", character * 0x2C + 0x14)];
                armor = Globals.DICTIONARY.OriginalItemList[Emulator.ReadByte("CHAR_TABLE", character * 0x2C + 0x15)];
                helm = Globals.DICTIONARY.OriginalItemList[Emulator.ReadByte("CHAR_TABLE", character * 0x2C + 0x16)];
                boots = Globals.DICTIONARY.OriginalItemList[Emulator.ReadByte("CHAR_TABLE", character * 0x2C + 0x17)];
                accessory = Globals.DICTIONARY.OriginalItemList[Emulator.ReadByte("CHAR_TABLE", character * 0x2C + 0x18)];
            } else {
                weapon = Globals.DICTIONARY.ItemList[Emulator.ReadByte("CHAR_TABLE", character * 0x2C + 0x14)];
                armor = Globals.DICTIONARY.ItemList[Emulator.ReadByte("CHAR_TABLE", character * 0x2C + 0x15)];
                helm = Globals.DICTIONARY.ItemList[Emulator.ReadByte("CHAR_TABLE", character * 0x2C + 0x16)];
                boots = Globals.DICTIONARY.ItemList[Emulator.ReadByte("CHAR_TABLE", character * 0x2C + 0x17)];
                accessory = Globals.DICTIONARY.ItemList[Emulator.ReadByte("CHAR_TABLE", character * 0x2C + 0x18)];
            }

            hp = Emulator.ReadUShort("CHAR_TABLE", character * 0x2C + 0x8);
            mp = Emulator.ReadUShort("CHAR_TABLE", character * 0x2C + 0xA);
            sp = Emulator.ReadUShort("CHAR_TABLE", character * 0x2C + 0xC);


            if (!Globals.CHARACTER_STAT_CHANGE) {
                max_hp = (ushort) (Globals.DICTIONARY.OriginalCharacterStats[character][lv].Max_HP * (1 + ((weapon.Special2 & 2) >> 1) * (float) (weapon.Special_Ammount) / 100 + ((armor.Special2 & 2) >> 1) * (float) (armor.Special_Ammount) / 100
                       + ((helm.Special2 & 2) >> 1) * (float) (helm.Special_Ammount) / 100 + ((boots.Special2 & 2) >> 1) * (float) (boots.Special_Ammount) / 100 + ((accessory.Special2 & 2) >> 1) * (float) (accessory.Special_Ammount) / 100));

                at = (ushort) (Globals.DICTIONARY.OriginalCharacterStats[character][lv].AT + weapon.AT + armor.AT + helm.AT + boots.AT + accessory.AT);
                mat = (ushort) (Globals.DICTIONARY.OriginalCharacterStats[character][lv].MAT + weapon.MAT + armor.MAT + helm.MAT + boots.MAT + accessory.MAT);
                df = (ushort) (Globals.DICTIONARY.OriginalCharacterStats[character][lv].DF + weapon.DF + armor.DF + helm.DF + boots.DF + accessory.DF);
                mdf = (ushort) (Globals.DICTIONARY.OriginalCharacterStats[character][lv].MDF + weapon.MDF + armor.MDF + helm.MDF + boots.MDF + accessory.MDF);
                body_spd = (ushort) (Globals.DICTIONARY.OriginalCharacterStats[character][lv].SPD);
                equip_spd = (ushort) (weapon.SPD + armor.SPD + helm.SPD + boots.SPD + accessory.SPD);
                spd = (ushort) (body_spd + equip_spd);
            } else {
                max_hp = (ushort) (Globals.DICTIONARY.CharacterStats[character][lv].Max_HP * (1 + ((weapon.Special2 & 2) >> 1) * (float) (weapon.Special_Ammount) / 100 + ((armor.Special2 & 2) >> 1) * (float) (armor.Special_Ammount) / 100
                       + ((helm.Special2 & 2) >> 1) * (float) (helm.Special_Ammount) / 100 + ((boots.Special2 & 2) >> 1) * (float) (boots.Special_Ammount) / 100 + ((accessory.Special2 & 2) >> 1) * (float) (accessory.Special_Ammount) / 100));

                at = (ushort) (Globals.DICTIONARY.CharacterStats[character][lv].AT + weapon.AT + armor.AT + helm.AT + boots.AT + accessory.AT);
                mat = (ushort) (Globals.DICTIONARY.CharacterStats[character][lv].MAT + weapon.MAT + armor.MAT + helm.MAT + boots.MAT + accessory.MAT);
                df = (ushort) (Globals.DICTIONARY.CharacterStats[character][lv].DF + weapon.DF + armor.DF + helm.DF + boots.DF + accessory.DF);
                mdf = (ushort) (Globals.DICTIONARY.CharacterStats[character][lv].MDF + weapon.MDF + armor.MDF + helm.MDF + boots.MDF + accessory.MDF);
                body_spd = (ushort) Globals.DICTIONARY.CharacterStats[character][lv].SPD;
                equip_spd = (ushort) (weapon.SPD + armor.SPD + helm.SPD + boots.SPD + accessory.SPD);
                spd = (ushort) (body_spd + equip_spd);
            }

            if (Globals.DRAGOON_STAT_CHANGE) {
                max_mp = (ushort) (Globals.DICTIONARY.DragoonStats[character][dlv].MP * (1 + (weapon.Special2 & 1) * (float) (weapon.Special_Ammount) / 100 + (armor.Special2 & 1) * (float) (armor.Special_Ammount) / 100
                     + (helm.Special2 & 1) * (float) (helm.Special_Ammount) / 100 + (boots.Special2 & 1) * (float) (boots.Special_Ammount) / 100 + (accessory.Special2 & 1) * (float) (accessory.Special_Ammount) / 100));
            } else {
                max_mp = (ushort) (dlv * 20 * (1 + (weapon.Special2 & 1) * (float) (weapon.Special_Ammount) / 100 + (armor.Special2 & 1) * (float) (armor.Special_Ammount) / 100
                     + (helm.Special2 & 1) * (float) (helm.Special_Ammount) / 100 + (boots.Special2 & 1) * (float) (boots.Special_Ammount) / 100 + (accessory.Special2 & 1) * (float) (accessory.Special_Ammount) / 100));
            }

            stat_res |= weapon.Stat_Res | armor.Stat_Res | helm.Stat_Res | boots.Stat_Res | accessory.Stat_Res;
            e_half |= weapon.E_Half | armor.E_Half | helm.E_Half | boots.E_Half | accessory.E_Half;
            e_immune |= weapon.E_Immune | armor.E_Immune | helm.E_Immune | boots.E_Immune | accessory.E_Immune;
            a_av = (byte) (weapon.A_AV + armor.A_AV + helm.A_AV + boots.A_AV + accessory.A_AV);
            m_av = (byte) (weapon.M_AV + armor.M_AV + helm.M_AV + boots.M_AV + accessory.M_AV);
            a_hit = (byte) (weapon.A_Hit + armor.A_Hit + helm.A_Hit + boots.A_Hit + accessory.A_Hit);
            m_hit = (byte) (weapon.M_Hit + armor.M_Hit + helm.M_Hit + boots.M_Hit + accessory.M_Hit);
            p_half |= ((weapon.Special1 & 0x20) | (armor.Special1 & 0x20) | (helm.Special1 & 0x20) | (boots.Special1 & 0x20) | (accessory.Special1 & 0x20)) >> 5;
            m_half |= ((weapon.Special2 & 0x4) | (armor.Special2 & 0x4) | (helm.Special2 & 0x4) | (boots.Special2 & 0x4) | (accessory.Special2 & 0x4)) >> 2;
            on_hit_status = weapon.On_Hit_Status;
            status_chance = weapon.Status_Chance;
            element = weapon.Element;
            revive = (byte) (((weapon.Special2 & 0x8) >> 3) * weapon.Special_Ammount + ((armor.Special2 & 0x8) >> 3) * armor.Special_Ammount + ((helm.Special2 & 0x8) >> 3) * helm.Special_Ammount
                + ((boots.Special2 & 0x8) >> 3) * boots.Special_Ammount + ((accessory.Special2 & 0x8) >> 3) * accessory.Special_Ammount);
            sp_regen = (ushort) (((weapon.Special2 & 0x10) >> 4) * weapon.Special_Ammount + ((armor.Special2 & 0x10) >> 4) * armor.Special_Ammount + ((helm.Special2 & 0x10) >> 4) * helm.Special_Ammount
                + ((boots.Special2 & 0x10) >> 4) * boots.Special_Ammount + ((accessory.Special2 & 0x10) >> 4) * accessory.Special_Ammount);
            mp_regen = (ushort) (((weapon.Special2 & 0x20) >> 5) * weapon.Special_Ammount + ((armor.Special2 & 0x20) >> 5) * armor.Special_Ammount + ((helm.Special2 & 0x20) >> 5) * helm.Special_Ammount
                + ((boots.Special2 & 0x20) >> 5) * boots.Special_Ammount + ((accessory.Special2 & 0x20) >> 5) * accessory.Special_Ammount);
            hp_regen = (ushort) (((weapon.Special2 & 0x40) >> 6) * weapon.Special_Ammount + ((armor.Special2 & 0x40) >> 6) * armor.Special_Ammount + ((helm.Special2 & 0x40) >> 6) * helm.Special_Ammount
                + ((boots.Special2 & 0x40) >> 6) * boots.Special_Ammount + ((accessory.Special2 & 0x40) >> 6) * accessory.Special_Ammount);
            mp_m_hit = (byte) ((weapon.Special1 & 0x1) * weapon.Special_Ammount + (armor.Special1 & 0x1) * armor.Special_Ammount + (helm.Special1 & 0x1) * helm.Special_Ammount
                + (boots.Special1 & 0x1) * boots.Special_Ammount + (accessory.Special1 & 0x1) * accessory.Special_Ammount);
            sp_m_hit = (byte) (((weapon.Special1 & 0x2) >> 1) * weapon.Special_Ammount + ((armor.Special1 & 0x2) >> 1) * armor.Special_Ammount + ((helm.Special1 & 0x2) >> 1) * helm.Special_Ammount
                + ((boots.Special1 & 0x2) >> 1) * boots.Special_Ammount + ((accessory.Special1 & 0x2) >> 1) * accessory.Special_Ammount);
            mp_p_hit = (byte) (((weapon.Special1 & 0x4) >> 2) * weapon.Special_Ammount + ((armor.Special1 & 0x4) >> 2) * armor.Special_Ammount + ((helm.Special1 & 0x4) >> 2) * helm.Special_Ammount
                + ((boots.Special1 & 0x4) >> 2) * boots.Special_Ammount + ((accessory.Special1 & 0x4) >> 2) * accessory.Special_Ammount);
            sp_p_hit = (byte) (((weapon.Special1 & 0x8) >> 3) * weapon.Special_Ammount + ((armor.Special1 & 0x8) >> 3) * armor.Special_Ammount + ((helm.Special1 & 0x8) >> 3) * helm.Special_Ammount
                + ((boots.Special1 & 0x8) >> 3) * boots.Special_Ammount + ((accessory.Special1 & 0x8) >> 3) * accessory.Special_Ammount);
            sp_multi = (byte) (((weapon.Special1 & 0x10) >> 4) * weapon.Special_Ammount + ((armor.Special1 & 0x10) >> 4) * armor.Special_Ammount + ((helm.Special1 & 0x10) >> 4) * helm.Special_Ammount
                + ((boots.Special1 & 0x4) >> 4) * boots.Special_Ammount + ((accessory.Special1 & 0x10) >> 4) * accessory.Special_Ammount);
            death_res |= weapon.Death_Res | armor.Death_Res | helm.Death_Res | boots.Death_Res | accessory.Death_Res;

            if (hp > max_hp) hp = max_hp;
            if (mp > max_mp) mp = max_mp;
        }
    }
    #endregion
}
