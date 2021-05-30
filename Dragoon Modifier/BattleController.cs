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

        static ushort[] previousMP = { 0, 0, 0 };
        static ushort[] currentMP = { 0, 0, 0 };
        static ushort[] trackMonsterHP = { 0, 0, 0, 0, 0 };

        static Hotkey[] hotkeys = new Hotkey[] {
            new ExitDragoonSlot(Hotkey.KEY_L1 + Hotkey.KEY_UP, 0),
            new ExitDragoonSlot(Hotkey.KEY_L1 + Hotkey.KEY_RIGHT, 1),
            new ExitDragoonSlot(Hotkey.KEY_L1 + Hotkey.KEY_LEFT, 2),
            new AdditionSwap(Hotkey.KEY_L1 + Hotkey.KEY_R1)
        };

        public static void Run(byte eleBombTurns, byte eleBombElement, bool reverseDBS, int inventorySize) {
            while (Constants.RUN) {
                try {
                    switch (Globals.GAME_STATE) {
                        case Globals.GameStateEnum.Battle:
                            if (!Globals.STATS_CHANGED) {
                                // ultimateBossStage = uiCombo["cboUltimateBoss"]; TODO
                                Setup();
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
                                    HardMode.DartBurnStackHandler();
                                }
                                //HardMode.MagicInventoryHandler();
                                HardMode.BattleDragoonRun((ushort)Globals.ENCOUNTER_ID, reverseDBS, eleBombTurns, eleBombElement);
                            }
                            foreach (var hotkey in hotkeys) {
                                if (hotkey.ButtonPress == Globals.HOTKEY) {
                                    hotkey.Init();
                                }
                            }
                            if (difficulty != "Normal") {
                                foreach (var hotkey in HardMode.Hotkeys) {
                                    if (hotkey.ButtonPress == Globals.HOTKEY) {
                                        hotkey.Init();
                                    }
                                }
                            }
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

        public static void Setup() {
            Constants.WriteOutput("Battle detected. Loading...");

            
            difficulty = Globals.DIFFICULTY_MODE;
            // aspectRatioOption = uiCombo["cboAspectRatio"]; TODO UI
            // cameraOption = uiCombo["cboCamera"]; TODO UI
            // killBGM = uiCombo["cboKillBGM"]; TODO UI
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

            for (int monster = 0; monster < Globals.UNIQUE_MONSTER_SIZE; monster++) {
                Globals.UNIQUE_MONSTER_IDS.Add(Emulator.ReadUShort("UNIQUE_SLOT", (monster * 0x1A8)));
            }
            for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                Globals.MONSTER_IDS.Add(Emulator.ReadUShort("MONSTER_ID", Globals.BattleController.BattleOffset + (i * 0x8)));
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
                HardMode.Setup(difficulty);
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
            Globals.BattleController.MonsterTable[slot].HP = (ushort) Math.Round(HP);
            Globals.BattleController.MonsterTable[slot].Max_HP = (ushort) Math.Round(HP);
            Globals.BattleController.MonsterTable[slot].AT = (ushort) Math.Round(Globals.DICTIONARY.StatList[ID].AT * Globals.AT_MULTI);
            Globals.BattleController.MonsterTable[slot].OG_AT = (ushort) Math.Round(Globals.DICTIONARY.StatList[ID].AT * Globals.AT_MULTI);
            Globals.BattleController.MonsterTable[slot].MAT = (ushort) Math.Round(Globals.DICTIONARY.StatList[ID].MAT * Globals.MAT_MULTI);
            Globals.BattleController.MonsterTable[slot].OG_MAT = (ushort) Math.Round(Globals.DICTIONARY.StatList[ID].MAT * Globals.MAT_MULTI);
            Globals.BattleController.MonsterTable[slot].DF = (ushort) Math.Round(Globals.DICTIONARY.StatList[ID].DF * Globals.DF_MULTI * resup);
            Globals.BattleController.MonsterTable[slot].OG_DF = (ushort) Math.Round(Globals.DICTIONARY.StatList[ID].DF * Globals.DF_MULTI * resup);
            Globals.BattleController.MonsterTable[slot].MDF = (ushort) Math.Round(Globals.DICTIONARY.StatList[ID].MDF * Globals.MDF_MULTI * resup);
            Globals.BattleController.MonsterTable[slot].OG_MDF = (ushort) Math.Round(Globals.DICTIONARY.StatList[ID].MDF * Globals.MDF_MULTI * resup);
            Globals.BattleController.MonsterTable[slot].SPD = (ushort) Math.Round(Globals.DICTIONARY.StatList[ID].SPD * Globals.SPD_MULTI);
            Globals.BattleController.MonsterTable[slot].OG_SPD = (ushort) Math.Round(Globals.DICTIONARY.StatList[ID].SPD * Globals.SPD_MULTI);
            Globals.BattleController.MonsterTable[slot].A_AV = Globals.DICTIONARY.StatList[ID].A_AV;
            Globals.BattleController.MonsterTable[slot].M_AV = Globals.DICTIONARY.StatList[ID].M_AV;
            Globals.BattleController.MonsterTable[slot].P_Immune = Globals.DICTIONARY.StatList[ID].P_Immune;
            Globals.BattleController.MonsterTable[slot].M_Immune = Globals.DICTIONARY.StatList[ID].M_Immune;
            Globals.BattleController.MonsterTable[slot].P_Half = Globals.DICTIONARY.StatList[ID].P_Half;
            Globals.BattleController.MonsterTable[slot].M_Half = Globals.DICTIONARY.StatList[ID].M_Half;
            Globals.BattleController.MonsterTable[slot].Element = Globals.DICTIONARY.StatList[ID].Element;
            Globals.BattleController.MonsterTable[slot].E_Immune = Globals.DICTIONARY.StatList[ID].E_Immune;
            Globals.BattleController.MonsterTable[slot].E_Half = Globals.DICTIONARY.StatList[ID].E_Half;
            Globals.BattleController.MonsterTable[slot].StatusResist = Globals.DICTIONARY.StatList[ID].Stat_Res;
            Globals.BattleController.MonsterTable[slot].Special_Effect = Globals.DICTIONARY.StatList[ID].Death_Res;
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
                Globals.BattleController.CharacterTable[slot].Add_DMG_Multi = (ushort) Globals.DICTIONARY.AdditionData[character, addition, addition_level].ADD_DMG_Multi;
                Globals.BattleController.CharacterTable[slot].Add_SP_Multi = (ushort) Globals.DICTIONARY.AdditionData[character, addition, addition_level].ADD_SP_Multi;
            }
        }

        public static void DragoonStatChanges(int slot, int character) {
            if (slot == 0 && Globals.NO_DART != null) {
                character = (byte) Globals.NO_DART;
            }
            
            long address = Constants.GetAddress("SECONDARY_CHARACTER_TABLE") + (character * 0xA0);

            int dlv = Emulator.ReadByte("CHAR_TABLE", (character * 0x2C) + 0x13);
            Globals.BattleController.CharacterTable[slot].DAT = Globals.DICTIONARY.DragoonStats[character][dlv].DAT;
            Globals.BattleController.CharacterTable[slot].DMAT = Globals.DICTIONARY.DragoonStats[character][dlv].DMAT;
            Globals.BattleController.CharacterTable[slot].DDF = Globals.DICTIONARY.DragoonStats[character][dlv].DDF;
            Globals.BattleController.CharacterTable[slot].DMDF =  Globals.DICTIONARY.DragoonStats[character][dlv].DMDF;
            double MP_base = Globals.DICTIONARY.DragoonStats[character][dlv].MP;
            double MP_multi = 1 + (double)Emulator.ReadByte(address + 0x64) / 100;
            ushort MP_Max = (ushort) (MP_base * MP_multi);
            ushort MP_Curr = Math.Min(Emulator.ReadUShort("CHAR_TABLE", (character * 0x2C) + 0xA), MP_Max);
            Globals.BattleController.CharacterTable[slot].MP = MP_Curr;
            Emulator.WriteUShort(address + 0x6, MP_Curr); // HAS TO BE CHECKED
            Globals.BattleController.CharacterTable[slot].Max_MP = MP_Max;
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
                    Globals.BattleController.CharacterTable[slot].LV = lv;
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
                Globals.BattleController.CharacterTable[slot].StatusResist = stat_res;
                byte e_half = (byte) (weapon.E_Half | armor.E_Half | helm.E_Half | boots.E_Half | accessory.E_Half);
                Emulator.WriteByte(address + 0x7C, e_half);
                Globals.BattleController.CharacterTable[slot].E_Half = e_half;
                byte e_immune = (byte) (weapon.E_Immune | armor.E_Immune | helm.E_Immune | boots.E_Immune | accessory.E_Immune);
                Emulator.WriteByte(address + 0x7D, e_immune);
                Globals.BattleController.CharacterTable[slot].E_Immune = e_immune;
                byte a_av = (byte) (weapon.A_AV + armor.A_AV + helm.A_AV + boots.A_AV + accessory.A_AV);
                Emulator.WriteByte(address + 0x94, a_av);
                Globals.BattleController.CharacterTable[slot].A_AV = a_av;
                byte m_av = (byte) (weapon.M_AV + armor.M_AV + helm.M_AV + boots.M_AV + accessory.M_AV);
                Emulator.WriteByte(address+ 0x96, m_av);
                Globals.BattleController.CharacterTable[slot].M_AV = m_av;
                byte a_hit = (byte) (weapon.A_Hit + armor.A_Hit + helm.A_Hit + boots.A_Hit + accessory.A_Hit);
                Emulator.WriteByte(address + 0x90, a_hit);
                Globals.BattleController.CharacterTable[slot].A_HIT = a_hit;
                byte m_hit = (byte) (weapon.M_Hit + armor.M_Hit + helm.M_Hit + boots.M_Hit + accessory.M_Hit);
                Emulator.WriteByte(address + 0x92, m_hit);
                Globals.BattleController.CharacterTable[slot].M_HIT = m_hit;
                byte p_half = (byte) (((weapon.Special1 & 0x20) | (armor.Special1 & 0x20) | (helm.Special1 & 0x20) | (boots.Special1 & 0x20) | (accessory.Special1 & 0x20)) >> 5);
                Emulator.WriteByte(address + 0x4A, p_half);
                Globals.BattleController.CharacterTable[slot].P_Half = p_half;
                byte m_half = (byte) (((weapon.Special2 & 0x4) | (armor.Special2 & 0x4) | (helm.Special2 & 0x4) | (boots.Special2 & 0x4) | (accessory.Special2 & 0x4)) >> 2);
                Emulator.WriteByte(address + 0x60, m_half);
                Globals.BattleController.CharacterTable[slot].M_Half = m_half;
                byte on_hit_status = weapon.On_Hit_Status;
                Emulator.WriteByte(address + 0x9B, on_hit_status);
                Globals.BattleController.CharacterTable[slot].On_Hit_Status = on_hit_status;
                byte status_chance = weapon.Status_Chance;
                Emulator.WriteByte(address + 0x98, status_chance);
                Globals.BattleController.CharacterTable[slot].On_Hit_Status_Chance = status_chance;
                byte revive = (byte) (((weapon.Special2 & 0x8) >> 3) * weapon.Special_Ammount + ((armor.Special2 & 0x8) >> 3) * armor.Special_Ammount + ((helm.Special2 & 0x8) >> 3) * helm.Special_Ammount
                + ((boots.Special2 & 0x8) >> 3) * boots.Special_Ammount + ((accessory.Special2 & 0x8) >> 3) * accessory.Special_Ammount);
                Emulator.WriteByte(address + 0x5E, revive);
                Globals.BattleController.CharacterTable[slot].Revive = revive;
                short sp_regen = (short) (((weapon.Special2 & 0x10) >> 4) * weapon.Special_Ammount + ((armor.Special2 & 0x10) >> 4) * armor.Special_Ammount + ((helm.Special2 & 0x10) >> 4) * helm.Special_Ammount
                    + ((boots.Special2 & 0x10) >> 4) * boots.Special_Ammount + ((accessory.Special2 & 0x10) >> 4) * accessory.Special_Ammount);
                Emulator.WriteShort(address + 0x5C, sp_regen);
                Globals.BattleController.CharacterTable[slot].SP_Regen = sp_regen;
                short mp_regen = (short) (((weapon.Special2 & 0x20) >> 5) * weapon.Special_Ammount + ((armor.Special2 & 0x20) >> 5) * armor.Special_Ammount + ((helm.Special2 & 0x20) >> 5) * helm.Special_Ammount
                    + ((boots.Special2 & 0x20) >> 5) * boots.Special_Ammount + ((accessory.Special2 & 0x20) >> 5) * accessory.Special_Ammount);
                Emulator.WriteShort(address + 0x5A, mp_regen);
                Globals.BattleController.CharacterTable[slot].MP_Regen = mp_regen;
                short hp_regen = (short) (((weapon.Special2 & 0x40) >> 6) * weapon.Special_Ammount + ((armor.Special2 & 0x40) >> 6) * armor.Special_Ammount + ((helm.Special2 & 0x40) >> 6) * helm.Special_Ammount
                    + ((boots.Special2 & 0x40) >> 6) * boots.Special_Ammount + ((accessory.Special2 & 0x40) >> 6) * accessory.Special_Ammount);
                Emulator.WriteShort(address + 0x58, hp_regen);
                Globals.BattleController.CharacterTable[slot].HP_Regen = hp_regen;
                byte mp_m_hit = (byte) ((weapon.Special1 & 0x1) * weapon.Special_Ammount + (armor.Special1 & 0x1) * armor.Special_Ammount + (helm.Special1 & 0x1) * helm.Special_Ammount
                + (boots.Special1 & 0x1) * boots.Special_Ammount + (accessory.Special1 & 0x1) * accessory.Special_Ammount);
                Emulator.WriteByte(address + 0x54, mp_m_hit);
                Globals.BattleController.CharacterTable[slot].MP_M_Hit = mp_m_hit;
                byte sp_m_hit = (byte) (((weapon.Special1 & 0x2) >> 1) * weapon.Special_Ammount + ((armor.Special1 & 0x2) >> 1) * armor.Special_Ammount + ((helm.Special1 & 0x2) >> 1) * helm.Special_Ammount
                    + ((boots.Special1 & 0x2) >> 1) * boots.Special_Ammount + ((accessory.Special1 & 0x2) >> 1) * accessory.Special_Ammount);
                Emulator.WriteByte(address + 0x52, sp_m_hit);
                Globals.BattleController.CharacterTable[slot].SP_M_Hit = sp_m_hit;
                byte mp_p_hit = (byte) (((weapon.Special1 & 0x4) >> 2) * weapon.Special_Ammount + ((armor.Special1 & 0x4) >> 2) * armor.Special_Ammount + ((helm.Special1 & 0x4) >> 2) * helm.Special_Ammount
                    + ((boots.Special1 & 0x4) >> 2) * boots.Special_Ammount + ((accessory.Special1 & 0x4) >> 2) * accessory.Special_Ammount);
                Emulator.WriteByte(address + 0x50, mp_p_hit);
                Globals.BattleController.CharacterTable[slot].MP_P_Hit = mp_p_hit;
                byte sp_p_hit = (byte) (((weapon.Special1 & 0x8) >> 3) * weapon.Special_Ammount + ((armor.Special1 & 0x8) >> 3) * armor.Special_Ammount + ((helm.Special1 & 0x8) >> 3) * helm.Special_Ammount
                    + ((boots.Special1 & 0x8) >> 3) * boots.Special_Ammount + ((accessory.Special1 & 0x8) >> 3) * accessory.Special_Ammount);
                Emulator.WriteByte(address + 0x4E, sp_p_hit);
                Globals.BattleController.CharacterTable[slot].SP_P_Hit = sp_p_hit;
                byte sp_multi = (byte) (((weapon.Special1 & 0x10) >> 4) * weapon.Special_Ammount + ((armor.Special1 & 0x10) >> 4) * armor.Special_Ammount + ((helm.Special1 & 0x10) >> 4) * helm.Special_Ammount
                    + ((boots.Special1 & 0x4) >> 4) * boots.Special_Ammount + ((accessory.Special1 & 0x10) >> 4) * accessory.Special_Ammount);
                Emulator.WriteByte(address + 0x4C, sp_multi);
                Globals.BattleController.CharacterTable[slot].SP_Multi = sp_multi;
                byte death_res = (byte) (weapon.Death_Res | armor.Death_Res | helm.Death_Res | boots.Death_Res | accessory.Death_Res);
                Emulator.WriteByte(address + 0x76, death_res);
                Globals.BattleController.CharacterTable[slot].Special_Effect = death_res;
                byte weapon_element = (byte) weapon.Element;
                Emulator.WriteByte(address + 0x7A, weapon_element);
                Globals.BattleController.CharacterTable[slot].Weapon_Element = weapon_element;

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
                Globals.BattleController.CharacterTable[slot].MP = MP_Curr;
                Emulator.WriteUShort(address + 0x6, MP_Curr); // HAS TO BE CHECKED
                Globals.BattleController.CharacterTable[slot].Max_MP = mp_max;
                Emulator.WriteUShort(address + 0x6E, mp_max);
            }
            
            ushort spd = (ushort) (Emulator.ReadByte(address + 0x69) + Emulator.ReadUShort(address + 0x86));
            Globals.BattleController.CharacterTable[slot].SPD = spd;
            Globals.BattleController.CharacterTable[slot].OG_SPD = spd;
            ushort at = (ushort) (Emulator.ReadByte(address + 0x6A) + Emulator.ReadUShort(address + 0x88));
            Globals.BattleController.CharacterTable[slot].AT = at;
            Globals.BattleController.CharacterTable[slot].OG_AT = at;
            ushort mat = (ushort) (Emulator.ReadByte(address + 0x6B) + Emulator.ReadUShort(address + 0x8A));
            Globals.BattleController.CharacterTable[slot].MAT = mat;
            Globals.BattleController.CharacterTable[slot].OG_MAT = mat;
            ushort df = (ushort) (Emulator.ReadByte(address + 0x6C) + Emulator.ReadUShort(address + 0x8C));
            Globals.BattleController.CharacterTable[slot].DF = df;
            Globals.BattleController.CharacterTable[slot].OG_DF = df;
            ushort mdf = (ushort) (Emulator.ReadByte(address + 0x6D) + Emulator.ReadUShort(address + 0x8E));
            Globals.BattleController.CharacterTable[slot].MDF = mdf;
            Globals.BattleController.CharacterTable[slot].OG_MDF = mdf;

            ushort hp_max = (ushort) (base_HP * (1 + (double) hp_multi / 100));
            //Globals.BattleController.CharacterTable[slot].Write("Max_HP", (ushort) (base_HP * (1 + hp_multi / 100)));
            //Globals.BattleController.CharacterTable[slot].Write("HP", Math.Min(Emulator2.ReadUShort("CHAR_TABLE", character * 0x2C + 0x8), hp_max));
            Globals.BattleController.CharacterTable[slot].HP = Math.Min(Globals.CURRENT_STATS[slot].HP, Globals.CURRENT_STATS[slot].Max_HP);
            Globals.BattleController.CharacterTable[slot].Max_HP = Globals.CURRENT_STATS[slot].Max_HP;
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
                if (Globals.BattleController.MonsterTable[0].HP != 0) {
                    HP = 1;
                }
            } else {
                foreach (dynamic monster in Globals.BattleController.MonsterTable) {
                    if (monster.HP != 0) {
                        HP |= 1;
                    }
                }
            }
            if (HP == 0) {
                Emulator.WriteByte("PARTY_SLOT", 0, slot * 0x4);
                Globals.BattleController.CharacterTable[slot].Action = 2;
                while (Globals.BattleController.CharacterTable[slot].Action != 0) {
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
                        Globals.BattleController.CharacterTable[slot].Add_DMG_Multi = Globals.DICTIONARY.AdditionData[character, addition, newlevel].ADD_DMG_Multi;
                        Globals.BattleController.CharacterTable[slot].Add_SP_Multi = Globals.DICTIONARY.AdditionData[character, addition, newlevel].ADD_SP_Multi;
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
                    Globals.BattleController.MonsterTable[i].HP = 65535;
                    Globals.BattleController.MonsterTable[i].Max_HP = 65535;
                    Globals.BattleController.MonsterTable[i].SPD = 0;
                    Globals.BattleController.MonsterTable[i].AT = 0;
                    Globals.BattleController.MonsterTable[i].MAT = 0;
                    Globals.BattleController.MonsterTable[i].DF = 65535;
                    Globals.BattleController.MonsterTable[i].MDF = 65535;
                    Globals.BattleController.MonsterTable[i].P_Immune = 1;
                    Globals.BattleController.MonsterTable[i].M_Immune = 1;
                    Globals.BattleController.MonsterTable[i].Turn = 0;
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
                Globals.BattleController.CharacterTable[i].Guard = 0;
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
                        if (Globals.BattleController.CharacterTable[i].Action == 24) {
                            DamageCapScan();
                        }
                    }
                }
                for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                    if (Globals.BattleController.MonsterTable[i].Action == 28) { // Most used, not all monsters use action code 28 for item spells
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
                Globals.BattleController.CharacterTable[slot].Dragoon = 0;
            }
        }
        #endregion

        #endregion

        #region Hard/Hell Mode specific

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

        class ExitDragoonSlot : Hotkey {
            byte _slot;

            public ExitDragoonSlot(int buttonPress, byte slot) : base(buttonPress) {
                _slot = slot;
            }

            public override void Init() {
                if (Globals.BattleController.CharacterTable[_slot].DragoonTurns > 1) {
                    Globals.BattleController.CharacterTable[_slot].DragoonTurns = 1;
                    Constants.WriteGLogOutput("Slot 1 will exit Dragoon after next action.");
                }
                Globals.LAST_HOTKEY = Constants.GetTime();
                return;
            }

        }

        class AdditionSwap : Hotkey {
            public AdditionSwap(int buttonPress) : base(buttonPress) {

            }

            public override void Init() {
                MemoryController.Battle.AdditionSwap.Init();
                Globals.LAST_HOTKEY = Constants.GetTime();
                return;
            }
        }

        #endregion
    }

    #region Objects

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
                    Globals.BattleController.CharacterTable[slot].LV = lv;
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
