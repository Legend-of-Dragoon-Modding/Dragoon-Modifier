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

namespace Dragoon_Modifier {
    class BattleController {
        static byte[] sharanda = new byte[] { 0x2, 0x8 };
        static ushort[] slot1FinalBlow = new ushort[] { 414, 408, 409, 392, 431 };      // Urobolus, Wounded Virage, Complete Virage, Lloyd, Zackwell
        static ushort[] slot2FinalBlow = new ushort[] { 387, 403 };                     // Fruegel II, Gehrich

        static bool firstDamageCapRemoval = false;
        static int lastItemUsedDamageCap = 0;

        static bool dartSwitcheroo = false;

        static byte flowerStorm = 3;
        static string difficulty = "Normal";
        static int aspectRatioOption = 0;
        static int cameraOption = 0;
        static int killBGM = 0;

        static int bossSPLoss = 0;

        //Chapter 3 Elemental Buffs
        static int sparkleArrowBuff = 8;
        static int heatBladeBuff = 7;
        static int shadowCutterBuff = 9;
        static int morningStarBuff = -20;

        public static void Run(Emulator emulator, Dictionary<string, int> uiCombo) {
            while (Constants.RUN) {
                try {
                    if (Globals.GAME_STATE == 1) {          // Battle
                        if (!Globals.STATS_CHANGED) { 
                            Setup(emulator, uiCombo);
                        } else {
                            if (Globals.PARTY_SLOT[0] == 4 && emulator.ReadByte("HASCHEL_FIX" + Globals.DISC) != 0x80) {
                                HaschelFix(emulator);
                            }
                            if (slot1FinalBlow.Contains(Globals.ENCOUNTER_ID) && sharanda.Contains(Globals.PARTY_SLOT[0])) {
                                ShanaFix(emulator, 0);
                            }
                            if (slot2FinalBlow.Contains(Globals.ENCOUNTER_ID) && sharanda.Contains(Globals.PARTY_SLOT[1])) {
                                ShanaFix(emulator, 1);
                            }
                            if (Globals.ADDITION_SWAP) {
                                AdditionSwapInit(emulator);
                            }
                            if (Globals.CheckDMScript("btnAdditionLevel")) {
                                AdditionLevelUp(emulator);
                            }
                            if (Globals.CheckDMScript("btnNeverGuard")) {
                                NeverGuard(emulator);
                            }
                            if (Globals.CheckDMScript("btnRemoveCaps")) {
                                RemoveDamageCap(emulator);
                            }
                        }
                    } else if (Globals.GAME_STATE == 7) {   // Battle result screen
                        if (Globals.STATS_CHANGED) {
                            ReduceSP(emulator);
                            ItemFieldChanges(emulator);
                            CharacterFieldChanges(emulator);
                            Globals.STATS_CHANGED = false;
                        }
                    } else if (Globals.GAME_STATE == 0) {   // Field
                        if (Globals.STATS_CHANGED) {
                            ItemFieldChanges(emulator);
                            CharacterFieldChanges(emulator);
                            Globals.STATS_CHANGED = false;
                        }
                        if (Globals.PARTY_SLOT[2] < 9 && Globals.PARTY_SLOT[0] != 0) {

                            Globals.NO_DART = Globals.PARTY_SLOT[0];
                            emulator.WriteByte("PARTY_SLOT", 0);
                            dartSwitcheroo = true;
                        }
                    } else if (Globals.GAME_STATE == 2) {
                        if (Globals.NO_DART != null) {
                            emulator.WriteByte("MENU_UNLOCK", 1);
                            if (dartSwitcheroo) {
                                emulator.WriteByte("PARTY_SLOT", (byte) Globals.NO_DART);
                                dartSwitcheroo = false;
                                Thread.Sleep(200);
                            }
                            Globals.NO_DART = Globals.PARTY_SLOT[0];
                        }
                    }
                    Thread.Sleep(250);
                } catch (Exception ex) {
                    Constants.RUN = false;
                    Constants.WriteGLog("Program stopped.");
                    Constants.WritePLogOutput("INTERNAL BATTLE SCRIPT ERROR");
                    Constants.WriteOutput("Fatal Error. Closing all threads.");
                    Constants.WriteError(ex.ToString());
                }
            
            }
        }

        public static void Setup(Emulator emulator, Dictionary<string, int> uiCombo) {
            Constants.WriteOutput("Battle detected. Loading...");

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            flowerStorm = (byte) (uiCombo["cboFlowerStorm"] + 1);
            difficulty = Globals.DIFFICULTY_MODE;
            aspectRatioOption = uiCombo["cboAspectRatio"];
            cameraOption = uiCombo["cboCamera"];
            killBGM = uiCombo["cboKillBGM"];
            if (Globals.CheckDMScript("btnAspectRatio")) {
                ChangeAspectRatio(emulator);
            }
            if (Globals.CheckDMScript("btnKillBGM") && killBGM != 0) {
                KillBGM(emulator);
            }
            if (difficulty == "NormalHard" || difficulty == "HardHell") {
                SwitchDualDifficulty(emulator);
            }
            if (Globals.DIFFICULTY_MODE.Contains("Hell")) {
                SetSPLossAmmount();
            } else {
                bossSPLoss = 0;
            }
            while (stopWatch.ElapsedMilliseconds < 3500) {
                if (Globals.GAME_STATE != 1) {
                    stopWatch.Stop();
                    return;
                }
                Thread.Sleep(50);
            }
            stopWatch.Stop();

            Globals.SetM_POINT((long) (emulator.ReadInteger("C_POINT", 0x14) - 0x7FFFFEF8));
            Globals.SetC_POINT((long) (emulator.ReadInteger("C_POINT") - 0x7FFFFEF8));
            Globals.MONSTER_SIZE = emulator.ReadByte("MONSTER_SIZE");
            Globals.UNIQUE_MONSTER_SIZE = emulator.ReadByte("UNIQUE_MONSTER_SIZE");
            Globals.UNIQUE_MONSTER_IDS = new List<int>();
            Globals.MONSTER_IDS = new List<int>();
            Globals.MONSTER_TABLE = new List<dynamic>();
            Globals.CHARACTER_TABLE = new List<dynamic>();

            for (int monster = 0; monster < Globals.UNIQUE_MONSTER_SIZE; monster++) {
                Globals.UNIQUE_MONSTER_IDS.Add(emulator.ReadShort("UNIQUE_SLOT", (monster * 0x1A8)));
            }
            for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                Globals.MONSTER_IDS.Add(emulator.ReadShort("MONSTER_ID", GetOffset(emulator) + (i * 0x8)));
            }
            for (int monster = 0; monster < Globals.MONSTER_SIZE; monster++) {
                Globals.MONSTER_TABLE.Add(new MonsterAddress(Globals.M_POINT, monster, Globals.MONSTER_IDS[monster], Globals.UNIQUE_MONSTER_IDS, emulator));
            }
            for (int character = 0; character < 3; character++) {
                if (Globals.PARTY_SLOT[character] < 9) {
                    Globals.CHARACTER_TABLE.Add(new CharAddress(Globals.C_POINT, character, emulator));
                }
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
            Constants.WriteDebug("Slot1 Address:       " + Convert.ToString(Constants.OFFSET + GetOffset(emulator) + 0x1D95F4, 16).ToUpper());
            Constants.WriteDebug("Slot2 Address:       " + Convert.ToString(Constants.OFFSET + GetOffset(emulator) + 0x1DA88C, 16).ToUpper());
            Constants.WriteDebug("Slot3 Address:       " + Convert.ToString(Constants.OFFSET + GetOffset(emulator) + 0x1DBB24, 16).ToUpper());

            Constants.WriteDebug("Addition Table:      " + Convert.ToString(Constants.GetAddress("ADDITION") + GetOffset(emulator) + Constants.OFFSET, 16).ToUpper());

            MonsterChanges(emulator);
            CharacterBattleChanges(emulator);
            if (Globals.NO_DART > 0) {
                Constants.WriteOutput("Finished loading. Waiting for No Dart to complete...");
                NoDart(emulator);
            }
            if (Globals.CheckDMScript("btnBlackRoom")) {
                BlackRoomBattle(emulator);
            }
            if (Globals.CheckDMScript("btnNoDragoon")) {
                NoDragoonMode();
            }
            if (difficulty != "Normal") {
                HardHellModeSetup(emulator);
            }
            Constants.WriteOutput("Finished loading.");
            Globals.STATS_CHANGED = true;
        }

        public static int GetOffset(Emulator emulator) {
            if (Constants.REGION == Region.NTA || Constants.REGION == Region.ENG) {
                return emulator.ReadShort("BATTLE_OFFSET") - 0x8F44;
            } else {
                int[] discOffset = { 0xD80, 0x0, 0x1458, 0x1B0 };
                int[] charOffset = { 0x0, 0x180, -0x180, 0x420, 0x540, 0x180, 0x350, 0x2F0, -0x180 };
                int[] duoCharOffset = { 0x0, -0x180, 0x180, -0x420, -0x180, -0x540, -0x350, -0x350, -0x2F0 };
                int[] duoDartOffset = { 0x0, 0x470, 0x170, 0x710, 0x830, 0x470, 0x640, 0x640, 0x170 };
                int partyOffset = 0;

                if (Globals.PARTY_SLOT[2] == 255 && Globals.PARTY_SLOT[1] < 9) {
                    if (Globals.PARTY_SLOT[0] == 0) {
                        partyOffset = duoDartOffset[Globals.PARTY_SLOT[1]];
                    } else if (Globals.PARTY_SLOT[1] == 0) {
                        partyOffset = duoDartOffset[Globals.PARTY_SLOT[0]];
                    } else {
                        partyOffset = charOffset[Globals.PARTY_SLOT[0]] + charOffset[Globals.PARTY_SLOT[1]];
                    }
                } else {
                    if (Globals.PARTY_SLOT[0] < 9 && Globals.PARTY_SLOT[1] < 9 && Globals.PARTY_SLOT[2] < 9) {
                        partyOffset = charOffset[Globals.PARTY_SLOT[1]] + charOffset[Globals.PARTY_SLOT[2]];
                    }
                }

                return discOffset[Globals.DISC - 1] - partyOffset;
            }
        }

        #region Battle Changes

        #region Monster Changes
        public static void MonsterChanges(Emulator emulator) {
            if (Globals.MONSTER_STAT_CHANGE && !Globals.CheckDMScript("btnUltimateBoss")) {
                Constants.WriteOutput("Changing Monster Stats...");
                for (int slot = 0; slot < Globals.MONSTER_SIZE; slot++) {
                    MonsterStatChange(emulator, slot);
                }
            }

            if (Globals.MONSTER_DROP_CHANGE && !Globals.CheckDMScript("btnUltimateBoss")) {
                Constants.WriteOutput("Changing Monster Drops...");
                for (int slot = 0; slot < Globals.UNIQUE_MONSTER_SIZE; slot++) {
                    MonsterDropChange(emulator, slot);
                }
            }

            if (Globals.MONSTER_EXPGOLD_CHANGE && !Globals.CheckDMScript("btnUltimateBoss")) {
                Constants.WriteOutput("Changing Monster Exp and Gold Rewards...");
                for (int slot = 0; slot < Globals.UNIQUE_MONSTER_SIZE; slot++) {
                    MonsterExpGoldChange(emulator, slot);
                }
            }
        }

        public static void MonsterStatChange(Emulator emulator, int slot) {
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
            Globals.MONSTER_TABLE[slot].Write("E_Immune", Globals.DICTIONARY.StatList[ID].E_Immune);
            Globals.MONSTER_TABLE[slot].Write("E_Half", Globals.DICTIONARY.StatList[ID].E_Half);
            Globals.MONSTER_TABLE[slot].Write("Stat_Res", Globals.DICTIONARY.StatList[ID].Stat_Res);
            Globals.MONSTER_TABLE[slot].Write("Death_Res", Globals.DICTIONARY.StatList[ID].Death_Res);
        }

        public static void MonsterDropChange(Emulator emulator, int slot) {
            int ID = Globals.UNIQUE_MONSTER_IDS[slot];
            emulator.WriteByte("MONSTER_REWARDS", (byte) Globals.DICTIONARY.StatList[ID].Drop_Chance, 0x4 + slot * 0x1A8);
            emulator.WriteByte("MONSTER_REWARDS", (byte) Globals.DICTIONARY.StatList[ID].Drop_Item, 0x5 + slot * 0x1A8);
        }

        public static void MonsterExpGoldChange(Emulator emulator, int slot) {
            int ID = Globals.UNIQUE_MONSTER_IDS[slot];
            emulator.WriteShort("MONSTER_REWARDS", (ushort) Globals.DICTIONARY.StatList[ID].EXP, slot * 0x1A8);
            emulator.WriteShort("MONSTER_REWARDS", (ushort) Globals.DICTIONARY.StatList[ID].Gold, 0x2 + slot * 0x1A8);
        }

        #endregion

        #region Character Changes
        public static void CharacterBattleChanges(Emulator emulator) {
            if (Globals.PARTY_SLOT[1] == Globals.NO_DART || Globals.PARTY_SLOT[2] == Globals.NO_DART) {
                Globals.NO_DART = null;
            }

            for (int slot = 0; slot < 3; slot++) {
                int character = Globals.PARTY_SLOT[slot];
                if (slot == 0 && Globals.NO_DART != null) {
                    character = (int) Globals.NO_DART;
                }
                if (character > 8) {
                    break;
                }
                Globals.CURRENT_STATS[slot] = new CurrentStats(character, slot, emulator);
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
                    AdditionsBattleChanges(emulator, slot, character);
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
                    DragoonStatChanges(emulator, slot, character);
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
                    DragoonAdditionsBattleChanges(emulator, slot, character);
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
                    DragoonSpellChange(emulator, slot, character);
                }
            }

            if (Globals.DRAGOON_DESC_CHANGE && Constants.REGION == Region.NTA) {
                Constants.WriteOutput("Changing Dragoon Spell Descriptions...");
                DragoonSpellDescriptionChange(emulator);
            }

            if (Globals.ITEM_STAT_CHANGE) {
                Constants.WriteOutput("Changing Character Stats...");
                for (int slot = 0; slot < 3; slot++) {
                    int character = Globals.PARTY_SLOT[slot];
                    if (slot == 0 && Globals.NO_DART != null) {
                        character = (int) Globals.NO_DART;
                    }
                    if (character > 8) {
                        break;
                    }
                    ItemBattleChanges(emulator, slot);
                }
            } else if (Globals.CHARACTER_STAT_CHANGE) {
                Constants.WriteOutput("Changing Character Stats...");
                for (int slot = 0; slot < 3; slot++) {
                    int character = Globals.PARTY_SLOT[slot];
                    if (slot == 0 && Globals.NO_DART != null) {
                        character = (int) Globals.NO_DART;
                    }
                    if (character > 8) {
                        break;
                    }
                    CharacterBattleChanges(emulator, slot);
                }
            }
        }

        public static void AdditionsBattleChanges(Emulator emulator, int slot, int character) {
            long address = Constants.GetAddress("ADDITION") + GetOffset(emulator);
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
                emulator.WriteByte(Globals.M_POINT + 0x148AC, Globals.DICTIONARY.AdditionData[character, 0, 0].SP);
                emulator.WriteByte(Globals.M_POINT + 0x148AC + 0x4, Globals.DICTIONARY.AdditionData[character, 0, 1].SP);
                emulator.WriteByte(Globals.M_POINT + 0x148AC + 0x8, Globals.DICTIONARY.AdditionData[character, 0, 2].SP);
                emulator.WriteByte(Globals.M_POINT + 0x148AC + 0xC, Globals.DICTIONARY.AdditionData[character, 0, 3].SP);
                emulator.WriteByte(Globals.M_POINT + 0x148AC + 0x10, Globals.DICTIONARY.AdditionData[character, 0, 4].SP);
            } else {
                int addition = additionnum[emulator.ReadByte("CHAR_TABLE", (character * 0x2C) + 0x19)];
                for (int hit = 0; hit < 8; hit++) {
                    emulator.WriteShort(address + (slot * 0x100) + (hit * 0x20), (ushort) Globals.DICTIONARY.AdditionData[character, addition, hit].UU1);
                    emulator.WriteShort(address + (slot * 0x100) + (hit * 0x20) + 0x2, (ushort) Globals.DICTIONARY.AdditionData[character, addition, hit].Next_Hit);
                    emulator.WriteShort(address + (slot * 0x100) + (hit * 0x20) + 0x4, (ushort) Globals.DICTIONARY.AdditionData[character, addition, hit].Blue_Time);
                    emulator.WriteShort(address + (slot * 0x100) + (hit * 0x20) + 0x6, (ushort) Globals.DICTIONARY.AdditionData[character, addition, hit].Gray_Time);
                    emulator.WriteShort(address + (slot * 0x100) + (hit * 0x20) + 0x8, (ushort) Globals.DICTIONARY.AdditionData[character, addition, hit].DMG);
                    emulator.WriteShort(address + (slot * 0x100) + (hit * 0x20) + 0xA, (ushort) Globals.DICTIONARY.AdditionData[character, addition, hit].SP);
                    emulator.WriteShort(address + (slot * 0x100) + (hit * 0x20) + 0xC, (ushort) Globals.DICTIONARY.AdditionData[character, addition, hit].ID);
                    emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0xE, Globals.DICTIONARY.AdditionData[character, addition, hit].Final_Hit);
                    emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0xF, Globals.DICTIONARY.AdditionData[character, addition, hit].UU2);
                    emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x10, Globals.DICTIONARY.AdditionData[character, addition, hit].UU3);
                    emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x11, Globals.DICTIONARY.AdditionData[character, addition, hit].UU4);
                    emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x12, Globals.DICTIONARY.AdditionData[character, addition, hit].UU5);
                    emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x13, Globals.DICTIONARY.AdditionData[character, addition, hit].UU6);
                    emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x14, Globals.DICTIONARY.AdditionData[character, addition, hit].UU7);
                    emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x15, Globals.DICTIONARY.AdditionData[character, addition, hit].UU8);
                    emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x16, Globals.DICTIONARY.AdditionData[character, addition, hit].UU9);
                    emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x17, Globals.DICTIONARY.AdditionData[character, addition, hit].UU10);
                    emulator.WriteShort(address + (slot * 0x100) + (hit * 0x20) + 0x18, (ushort) Globals.DICTIONARY.AdditionData[character, addition, hit].Vertical_Distance);
                    emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x1A, Globals.DICTIONARY.AdditionData[character, addition, hit].UU11);
                    emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x1B, Globals.DICTIONARY.AdditionData[character, addition, hit].UU12);
                    emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x1C, Globals.DICTIONARY.AdditionData[character, addition, hit].UU13);
                    emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x1D, Globals.DICTIONARY.AdditionData[character, addition, hit].UU14);
                    emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x1E, Globals.DICTIONARY.AdditionData[character, addition, hit].Start_Time);
                    emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x1F, Globals.DICTIONARY.AdditionData[character, addition, hit].UU15);
                }
                int addition_level = emulator.ReadByte("CHAR_TABLE", (character * 0x2C) + 0x1A + addition);
                Globals.CHARACTER_TABLE[slot].Write("ADD_DMG_Multi", Globals.DICTIONARY.AdditionData[character, addition, addition_level].ADD_DMG_Multi);
                Globals.CHARACTER_TABLE[slot].Write("ADD_SP_Multi", Globals.DICTIONARY.AdditionData[character, addition, addition_level].ADD_SP_Multi);
            }
        }

        public static void DragoonStatChanges(Emulator emulator, int slot, int character) {
            int dlv = Globals.CURRENT_STATS[slot].DLV;
            Globals.CHARACTER_TABLE[slot].Write("DAT", Globals.DICTIONARY.DragoonStats[character][dlv].DAT);
            Globals.CHARACTER_TABLE[slot].Write("DMAT", Globals.DICTIONARY.DragoonStats[character][dlv].DMAT);
            Globals.CHARACTER_TABLE[slot].Write("DDF", Globals.DICTIONARY.DragoonStats[character][dlv].DDF);
            Globals.CHARACTER_TABLE[slot].Write("DMDF", Globals.DICTIONARY.DragoonStats[character][dlv].DMDF);
            if (!Globals.ITEM_STAT_CHANGE) {
                Globals.CHARACTER_TABLE[slot].Write("MP", Math.Min(Globals.CURRENT_STATS[slot].MP, Globals.CURRENT_STATS[slot].Max_MP));
                Globals.CHARACTER_TABLE[slot].Write("Max_MP", Globals.CURRENT_STATS[slot].Max_MP);
            }
        }

        public static void ItemBattleChanges(Emulator emulator, int slot) {
            Globals.CHARACTER_TABLE[slot].Write("MP", Math.Min(Globals.CURRENT_STATS[slot].MP, Globals.CURRENT_STATS[slot].Max_MP));
            Globals.CHARACTER_TABLE[slot].Write("Max_MP", Globals.CURRENT_STATS[slot].Max_MP);
            Globals.CHARACTER_TABLE[slot].Write("Stat_Res", Globals.CURRENT_STATS[slot].Stat_Res);
            Globals.CHARACTER_TABLE[slot].Write("E_Half", Globals.CURRENT_STATS[slot].E_Half);
            Globals.CHARACTER_TABLE[slot].Write("E_Immune", Globals.CURRENT_STATS[slot].E_Immune);
            Globals.CHARACTER_TABLE[slot].Write("A_AV", Globals.CURRENT_STATS[slot].A_AV);
            Globals.CHARACTER_TABLE[slot].Write("M_AV", Globals.CURRENT_STATS[slot].M_AV);
            Globals.CHARACTER_TABLE[slot].Write("A_HIT", Globals.CURRENT_STATS[slot].A_Hit);
            Globals.CHARACTER_TABLE[slot].Write("M_HIT", Globals.CURRENT_STATS[slot].M_Hit);
            Globals.CHARACTER_TABLE[slot].Write("P_Half", Globals.CURRENT_STATS[slot].P_Half);
            Globals.CHARACTER_TABLE[slot].Write("M_Half", Globals.CURRENT_STATS[slot].M_Half);
            Globals.CHARACTER_TABLE[slot].Write("On_Hit_Status", Globals.CURRENT_STATS[slot].On_Hit_Status);
            Globals.CHARACTER_TABLE[slot].Write("On_Hit_Status_Chance", Globals.CURRENT_STATS[slot].Status_Chance);
            Globals.CHARACTER_TABLE[slot].Write("Revive", Globals.CURRENT_STATS[slot].Revive);
            Globals.CHARACTER_TABLE[slot].Write("SP_Regen", Globals.CURRENT_STATS[slot].SP_Regen);
            Globals.CHARACTER_TABLE[slot].Write("MP_Regen", Globals.CURRENT_STATS[slot].MP_Regen);
            Globals.CHARACTER_TABLE[slot].Write("HP_Regen", Globals.CURRENT_STATS[slot].HP_Regen);
            Globals.CHARACTER_TABLE[slot].Write("Element", Globals.CURRENT_STATS[slot].Element);
            Globals.CHARACTER_TABLE[slot].Write("Display_Element", Globals.CURRENT_STATS[slot].Element);
            Globals.CHARACTER_TABLE[slot].Write("MP_M_Hit", Globals.CURRENT_STATS[slot].MP_M_Hit);
            Globals.CHARACTER_TABLE[slot].Write("SP_M_Hit", Globals.CURRENT_STATS[slot].SP_M_Hit);
            Globals.CHARACTER_TABLE[slot].Write("MP_P_Hit", Globals.CURRENT_STATS[slot].MP_P_Hit);
            Globals.CHARACTER_TABLE[slot].Write("SP_P_Hit", Globals.CURRENT_STATS[slot].SP_P_Hit);
            Globals.CHARACTER_TABLE[slot].Write("SP_Multi", Globals.CURRENT_STATS[slot].SP_Multi);
            Globals.CHARACTER_TABLE[slot].Write("Death_Res", Globals.CURRENT_STATS[slot].Death_Res);
            Globals.CHARACTER_TABLE[slot].Write("HP", Math.Min(Globals.CURRENT_STATS[slot].HP, Globals.CURRENT_STATS[slot].Max_HP));
            Globals.CHARACTER_TABLE[slot].Write("Max_HP", Globals.CURRENT_STATS[slot].Max_HP);
            Globals.CHARACTER_TABLE[slot].Write("AT", Globals.CURRENT_STATS[slot].AT);
            Globals.CHARACTER_TABLE[slot].Write("OG_AT", Globals.CURRENT_STATS[slot].AT);
            Globals.CHARACTER_TABLE[slot].Write("MAT", Globals.CURRENT_STATS[slot].MAT);
            Globals.CHARACTER_TABLE[slot].Write("OG_MAT", Globals.CURRENT_STATS[slot].MAT);
            Globals.CHARACTER_TABLE[slot].Write("DF", Globals.CURRENT_STATS[slot].DF);
            Globals.CHARACTER_TABLE[slot].Write("DF", Globals.CURRENT_STATS[slot].DF);
            Globals.CHARACTER_TABLE[slot].Write("OG_DF", Globals.CURRENT_STATS[slot].DF);
            Globals.CHARACTER_TABLE[slot].Write("MDF", Globals.CURRENT_STATS[slot].MDF);
            Globals.CHARACTER_TABLE[slot].Write("OG_MDF", Globals.CURRENT_STATS[slot].MDF);
            Globals.CHARACTER_TABLE[slot].Write("SPD", Globals.CURRENT_STATS[slot].SPD);
            Globals.CHARACTER_TABLE[slot].Write("OG_SPD", Globals.CURRENT_STATS[slot].SPD);
        }

        public static void CharacterBattleChanges(Emulator emulator, int slot) {
            Globals.CHARACTER_TABLE[slot].Write("HP", Math.Min(Globals.CURRENT_STATS[slot].HP, Globals.CURRENT_STATS[slot].Max_HP));
            Globals.CHARACTER_TABLE[slot].Write("Max_HP", Globals.CURRENT_STATS[slot].Max_HP);
            Globals.CHARACTER_TABLE[slot].Write("AT", Globals.CURRENT_STATS[slot].AT);
            Globals.CHARACTER_TABLE[slot].Write("OG_AT", Globals.CURRENT_STATS[slot].AT);
            Globals.CHARACTER_TABLE[slot].Write("MAT", Globals.CURRENT_STATS[slot].MAT);
            Globals.CHARACTER_TABLE[slot].Write("OG_MAT", Globals.CURRENT_STATS[slot].MAT);
            Globals.CHARACTER_TABLE[slot].Write("DF", Globals.CURRENT_STATS[slot].DF);
            Globals.CHARACTER_TABLE[slot].Write("DF", Globals.CURRENT_STATS[slot].DF);
            Globals.CHARACTER_TABLE[slot].Write("OG_DF", Globals.CURRENT_STATS[slot].DF);
            Globals.CHARACTER_TABLE[slot].Write("MDF", Globals.CURRENT_STATS[slot].MDF);
            Globals.CHARACTER_TABLE[slot].Write("OG_MDF", Globals.CURRENT_STATS[slot].MDF);
            Globals.CHARACTER_TABLE[slot].Write("SPD", Globals.CURRENT_STATS[slot].SPD);
            Globals.CHARACTER_TABLE[slot].Write("OG_SPD", Globals.CURRENT_STATS[slot].SPD);
        }

        public static void DragoonAdditionsBattleChanges(Emulator emulator, int slot, int character) {
            long address = Constants.GetAddress("ADDITION") + GetOffset(emulator) + 0x300;
            emulator.WriteShort(address + (slot * 0x100) + 0x8, (ushort) Globals.DICTIONARY.DragoonAddition[character].HIT1);
            emulator.WriteShort(address + (slot * 0x100) + 0x20 + 0x8, (ushort) Globals.DICTIONARY.DragoonAddition[character].HIT2);
            emulator.WriteShort(address + (slot * 0x100) + 0x40 + 0x8, (ushort) Globals.DICTIONARY.DragoonAddition[character].HIT3);
            emulator.WriteShort(address + (slot * 0x100) + 0x60 + 0x8, (ushort) Globals.DICTIONARY.DragoonAddition[character].HIT4);
            emulator.WriteShort(address + (slot * 0x100) + 0x80 + 0x8, (ushort) Globals.DICTIONARY.DragoonAddition[character].HIT5);
        }

        public static void DragoonSpellChange(Emulator emulator, int slot, int character) {
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
                int intValue = (int) emulator.ReadByte("SPELL_TABLE", (id * 0xC) + 0x2);
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
                emulator.WriteByte(address + (id * 0xC) + 0x2, (byte) intValue);
                emulator.WriteByte(address + (id * 0xC) + 0x4, dmg_base);
                emulator.WriteByte(address + (id * 0xC) + 0x5, multi);
                emulator.WriteByte(address + (id * 0xC) + 0x6, Spell.Accuracy);
                emulator.WriteByte(address + (id * 0xC) + 0x7, Spell.MP);
                emulator.WriteByte(address + (id * 0xC) + 0x9, Spell.Element);
            }
        }

        public static void DragoonSpellDescriptionChange(Emulator emulator) {
            int i = 0;
            string descr = String.Empty;
            long address = Constants.GetAddress("DRAGOON_DESC_PTR");
            foreach (dynamic Spell in Globals.DRAGOON_SPELLS) {
                descr += (string) Spell.Encoded_Description + " ";
                emulator.WriteInteger(address + i * 0x4, (int) Spell.Description_Pointer);
                emulator.WriteByte(address + i * 0x4 + 0x3, 0x80);
                i++;
            }
            descr = descr.Remove(descr.Length - 1);
            emulator.WriteAOB(Constants.GetAddress("DRAGOON_DESC"), descr);
        }

        #endregion

        #region No Dart
        public static void HaschelFix(Emulator emulator) {
            Constants.WriteDebug("Haschel Fix - " + Globals.DISC);

            if (Constants.REGION == Region.NTA) {
                emulator.WriteAOB(Constants.GetAddress("HASCHEL_FIX" + Globals.DISC), "0x80 0x80 0x80 0x00");
                emulator.WriteAOB(Constants.GetAddress("HASCHEL_FIX" + Globals.DISC) + 0x4, Globals.DISC == 1 ? "0x90 0xA0" : Globals.DISC == 2 ? "0x10 0x93" : Globals.DISC == 3 ? "0x68 0xA7" : "0xC0 0x94");
                emulator.WriteAOB(Constants.GetAddress("HASCHEL_FIX" + Globals.DISC) + 0x6, ("0x1E 0x80 0x74 0x12 0x00 0x00 0x02 0x00 0x8C 0x8C 0x4D 0x52 0x47 0x1A 0x04 0x00 0x00 0x00 0x28 0x00" +
                        " 0x00 0x00 0x02 0x00 0x00 0x00 0x2C 0x00 0x00 0x00 0x68 0x00 0x00 0x00 0x94 0x00 0x00 0x00 0xD4 0x11 0x00 0x00 0x68 0x12 0x00 0x00 0xB0 0x05 0x02 0x00 0x00 0x00 0x8C 0x8C 0x00 0x00" +
                        " 0x00 0x01 0x00 0x02 0x00 0x03 0x00 0x04 0x00 0x05 0x00 0x06 0x00 0x07 0x00 0x08 0x00 0x09 0x00 0x0A 0x00 0x0B 0x00 0x0C 0x00 0x0D 0x00 0x0E 0x00 0x0F 0x00 0x10 0x00 0x11 0x00 0x12" +
                        " 0x00 0x13 0x00 0x14 0x00 0x15 0x00 0x16 0x00 0x17 0x00 0x18 0x00 0x19 0x00 0x1A 0x00 0x1B"));
            } else if (Constants.REGION == Region.JPN) {
                emulator.WriteAOB(Constants.GetAddress("HASCHEL_FIX" + Globals.DISC), "80 80 80 00");
                emulator.WriteAOB(Constants.GetAddress("HASCHEL_FIX" + Globals.DISC) + 0x4, Globals.DISC == 1 ? "98 90" : Globals.DISC == 2 ? "18 83" : Globals.DISC == 3 ? "70 97" : "C8 84");
                emulator.WriteAOB(Constants.GetAddress("HASCHEL_FIX" + Globals.DISC) + 0x6, ("1E 80 74 12 00 00 02 00 00 00 4D 52 47 1A 04 00 00 00 28 00 00 00 02 00 00 00" +
                        " 2C 00 00 00 68 00 00 00 94 00 00 00 D4 11 00 00 68 12 00 00 B0 05 02 00 00 00 8C 8C 00 00 00 01 00 02 00 03 00 04 00 05 00 06 00 07 00 08 00 09 00 0A 00 0B 00 0C 00 0D 00 0E 00 0F 00 10 00 11" +
                        " 00 12 00 13 00 14 00 15 00 16 00 17 00 18 00 19 00 1A 00 1B 00 1C 00 1D 00 1E 00 1F 00 20 00 21 00 22 00 23 00 24 00 25 00 26 00 27 00 28 00 29 00 2A 00 2B 00 2C 00 2D 00 2E 00 2F 00 30 00 31" +
                        " 00 32 00 33 D4 11 00 00 B0 05 02 00 00 00 00 00 53 53 68 64 FF FF FF FF 80 00 00 00 0C 11 00 00 02 01 00 00 B6 07 00 00"));
            }

        }

        public static void ShanaFix(Emulator emulator, byte slot) {
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
                emulator.WriteByte("PARTY_SLOT", 0, slot * 0x4);
                Globals.CHARACTER_TABLE[slot].Write("Action", 2);
                while (Globals.CHARACTER_TABLE[slot].Read("Action") != 0) {
                    Thread.Sleep(250);
                }
                try {
                    Thread.Sleep(10000);
                    emulator.WriteByte("PARTY_SLOT", (byte) Globals.NO_DART, slot * 0x4);
                } catch {
                    Constants.WriteDebug("No Dart not set");
                }
                Globals.SHANA_FIX = true;
            }
        }

        public static void NoDart(Emulator emulator) {
            byte character = (byte) Globals.NO_DART;
            Globals.CHARACTER_TABLE[0].Write("Status", emulator.ReadByte("CHAR_TABLE", character * 0x2C + 0x10));
            if (Globals.ENCOUNTER_ID == 413) {
                Globals.MONSTER_TABLE[0].Write("Action", 12);
                Thread.Sleep(1500);
            }
            Globals.CHARACTER_TABLE[0].Write("Action", 10);

            while (true) {
                if (Globals.GAME_STATE != 1) {
                    return;
                }
                if (Globals.CHARACTER_TABLE[0].Read("Menu") != 96) {
                    Thread.Sleep(50);
                } else {
                    break;
                }
            }

            emulator.WriteByte(Constants.GetAddress("DRAGOON_TURNS"), 1);

            Globals.CHARACTER_TABLE[0].Write("Dragoon", 0x20);
            emulator.WriteByte("PARTY_SLOT", character);
            emulator.WriteByte("PARTY_SLOT", character, 0x234E); // Secondary ID
            Globals.CHARACTER_TABLE[0].Write("Image", character);
            Globals.CHARACTER_TABLE[0].Write("Weapon", emulator.ReadByte("CHAR_TABLE", 0x14 + (character * 0x2C)));
            Globals.CHARACTER_TABLE[0].Write("Helmet", emulator.ReadByte("CHAR_TABLE", 0x15 + (character * 0x2C)));
            Globals.CHARACTER_TABLE[0].Write("Armor", emulator.ReadByte("CHAR_TABLE", 0x16 + (character * 0x2C)));
            Globals.CHARACTER_TABLE[0].Write("Shoes", emulator.ReadByte("CHAR_TABLE", 0x17 + (character * 0x2C)));
            Globals.CHARACTER_TABLE[0].Write("Accessory", emulator.ReadByte("CHAR_TABLE", 0x18 + (character * 0x2C)));
            Dictionary<int, byte> charelement = new Dictionary<int, byte> {
                {0, 128},{1, 64},{2, 32},{3, 4},{4, 16},{5, 64},{6, 1},{7, 2},{8, 32}
            };
            Globals.CHARACTER_TABLE[0].Write("LV", Globals.CURRENT_STATS[0].LV);
            Globals.CHARACTER_TABLE[0].Write("DLV", 1);
            Globals.CHARACTER_TABLE[0].Write("SP", 100);

            int dlv = Globals.CURRENT_STATS[0].DLV;

            #region Dragoon Magic
            emulator.WriteByte("DRAGOON_SPELL_SLOT", character); // Magic
            Dictionary<byte, byte> dmagic5 = new Dictionary<byte, byte> {
                {0, 3},{1, 8},{2, 13},{3, 19},{4, 23},{5, 8},{6, 28},{7, 31},{8, 13}
            };
            Dictionary<byte, byte> dmagic3 = new Dictionary<byte, byte> {
                {0, 2},{1, 6},{2, 12},{3, 18},{4, 22},{5, 17},{6, 27},{7, 255},{8, 67}
            };
            Dictionary<byte, byte> dmagic2 = new Dictionary<byte, byte> {
                {0, 1},{1, 7},{2, 10},{3, 16},{4, 21},{5, 26},{6, 25},{7, 30},{8, 65}
            };
            Dictionary<byte, byte> dmagic1 = new Dictionary<byte, byte> {
                {0, 0},{1, 5},{2, 11},{3, 15},{4, 20},{5, 14},{6, 24},{7, 29},{8, 66}
            };

            if (dlv == 5) {
                if (Globals.NO_DART != 7) {
                    emulator.WriteByte("DRAGOON_SPELL_SLOT", dmagic5[character], 4);
                    emulator.WriteByte("DRAGOON_SPELL_SLOT", dmagic3[character], 3);
                } else {
                    emulator.WriteByte("DRAGOON_SPELL_SLOT", 0xFF, 4);
                    emulator.WriteByte("DRAGOON_SPELL_SLOT", dmagic5[character], 3);
                }
                emulator.WriteByte("DRAGOON_SPELL_SLOT", dmagic2[character], 2);
                emulator.WriteByte("DRAGOON_SPELL_SLOT", dmagic1[character], 1);
            } else if (dlv > 2) {
                emulator.WriteByte("DRAGOON_SPELL_SLOT", 0xFF, 4);
                emulator.WriteByte("DRAGOON_SPELL_SLOT", dmagic3[character], 3);
                emulator.WriteByte("DRAGOON_SPELL_SLOT", dmagic2[character], 2);
                emulator.WriteByte("DRAGOON_SPELL_SLOT", dmagic1[character], 1);
            } else if (dlv > 1) {
                emulator.WriteByte("DRAGOON_SPELL_SLOT", 0xFF, 4);
                emulator.WriteByte("DRAGOON_SPELL_SLOT", 0xFF, 3);
                emulator.WriteByte("DRAGOON_SPELL_SLOT", dmagic2[character], 2);
                emulator.WriteByte("DRAGOON_SPELL_SLOT", dmagic1[character], 1);
            } else if (dlv > 0) {
                emulator.WriteByte("DRAGOON_SPELL_SLOT", 0xFF, 4);
                emulator.WriteByte("DRAGOON_SPELL_SLOT", 0xFF, 3);
                emulator.WriteByte("DRAGOON_SPELL_SLOT", 0xFF, 2);
                emulator.WriteByte("DRAGOON_SPELL_SLOT", dmagic1[character], 1);
            } else {
                emulator.WriteByte("DRAGOON_SPELL_SLOT", 0xFF, 4);
                emulator.WriteByte("DRAGOON_SPELL_SLOT", 0xFF, 3);
                emulator.WriteByte("DRAGOON_SPELL_SLOT", 0xFF, 2);
                emulator.WriteByte("DRAGOON_SPELL_SLOT", 0xFF, 1);
            }
            #endregion

            #region Wargod/Destroyer Mace fix
            byte special_effect = 0;
            if (Globals.CURRENT_STATS[0].Weapon.ID == 45) {
                special_effect |= 1;
            }

            if (Globals.CURRENT_STATS[0].Accessory.ID == 157) {
                special_effect |= 2;
            }

            if (Globals.CURRENT_STATS[0].Accessory.ID == 158) {
                special_effect |= 6;
            }

            emulator.WriteByte("WARGOD", special_effect);

            #endregion

            /*
            while (true) {
                if (emulator.ReadShort("BATTLE_VALUE") < 5130) {
                    return;
                }
                if (!(Globals.CHARACTER_TABLE[0].Read("Menu") > 14)) {
                    Thread.Sleep(50);
                } else {
                    break;
                }
            }
            Thread.Sleep(350);
            if (Globals.AUTO_TRANSFORM) {
                ushort val = emulator.ReadShort(Globals.C_POINT - 0xF0);
                int temp = emulator.ReadByte(Globals.C_POINT - 0xEC) + 20;
                emulator.WriteByte(Globals.C_POINT - 0xE5, 128);
                emulator.WriteByte(Globals.C_POINT - 0xE6, 28);
                emulator.WriteByte(Globals.C_POINT - 0xE7, emulator.ReadByte(Globals.C_POINT - 0xEB));
                emulator.WriteByte(Globals.C_POINT - 0xE8, emulator.ReadByte(Globals.C_POINT - 0xEC));
                if (temp > 255) {
                    emulator.WriteByte(Globals.C_POINT - 0xEB, emulator.ReadByte(Globals.C_POINT - 0xEF) + 1);
                    emulator.WriteByte(Globals.C_POINT - 0xEC, (byte)((emulator.ReadByte(Globals.C_POINT - 0xEC) + 20) & 255));
                } else {
                    emulator.WriteByte(Globals.C_POINT - 0xEB, emulator.ReadByte(Globals.C_POINT - 0xEF));
                    emulator.WriteByte(Globals.C_POINT - 0xEC, (byte)(emulator.ReadByte(Globals.C_POINT - 0xEC) + 20));
                }
                emulator.WriteShort(Globals.C_POINT - 0xF0, (ushort)(val + 0xE84));
                emulator.WriteByte(Constants.GetAddress("DRAGOON_TURNS"), 1);
            } else {
                Globals.CHARACTER_TABLE[0].Write("Menu", 16);
            }
            */

            if (!Globals.ADDITION_CHANGE) {
                AdditionsBattleChanges(emulator, 0, character);
            }

            if (!Globals.DRAGOON_STAT_CHANGE) {
                DragoonStatChanges(emulator, 0, character);
            }

            if (!Globals.ITEM_STAT_CHANGE) {
                ItemBattleChanges(emulator, 0);
            }

            if (Globals.AUTO_TRANSFORM) {
                ushort val = emulator.ReadShort(Globals.C_POINT - 0xF0);
                emulator.WriteShort(Globals.C_POINT - 0xF0, (ushort) (val + 0x4478));
                emulator.WriteByte(Globals.C_POINT - 0xEE, 27);
            } else {
                Globals.CHARACTER_TABLE[0].Write("Menu", 16);
            }
            while (true) {
                if (Globals.GAME_STATE != 1) {
                    return;
                }
                if (Globals.CHARACTER_TABLE[0].Read("Action") != 9) {
                    Thread.Sleep(50);
                } else {
                    break;
                }
            }
            Globals.CHARACTER_TABLE[0].Write("DLV", Globals.CURRENT_STATS[0].DLV);
            Globals.CHARACTER_TABLE[0].Write("SP", Globals.CURRENT_STATS[0].SP);
            if (dlv == 0) {
                Globals.CHARACTER_TABLE[0].Write("Dragoon", 0);
            }
            Constants.WriteOutput("No Dart complete.");
        }

        #endregion

        #endregion

        #region Field Changes

        #region Item Changes
        public static void ItemFieldChanges(Emulator emulator) {
            if (Globals.ITEM_STAT_CHANGE) {
                EquipStatChange(emulator);
            }
            if (Globals.THROWN_ITEM_CHANGE) {
                ItemChange(emulator);
            }
            if (Globals.ITEM_ICON_CHANGE) {
                ItemIconChange(emulator);
            }
            if (Globals.ITEM_NAMEDESC_CHANGE && Constants.REGION == Region.NTA) {
                ItemNameDescChange(emulator);
            }
        }

        public static void EquipStatChange(Emulator emulator) {
            Constants.WriteOutput("Changing Equipment Stats...");
            long address = Constants.GetAddress("ITEM_TABLE");
            int i = 0;
            foreach (dynamic item in Globals.DICTIONARY.ItemList) {
                if (i > 185) //hopefully safe ammount
                    break;
                emulator.WriteByte(address + i * 0x1C, item.Type);
                emulator.WriteByte(address + i * 0x1C + 0x2, item.Equips);
                emulator.WriteByte(address + i * 0x1C + 0x3, item.Element);
                emulator.WriteByte(address + i * 0x1C + 0x1A, item.On_Hit_Status);
                emulator.WriteByte(address + i * 0x1C + 0x17, item.Status_Chance);
                if (item.AT > 255) {
                    emulator.WriteByte(address + i * 0x1C + 0x9, 255);
                    emulator.WriteByte(address + i * 0x1C + 0xF, item.AT - 255);
                } else {
                    emulator.WriteByte(address + i * 0x1C + 0x9, item.AT);
                    emulator.WriteByte(address + i * 0x1C + 0xF, 0);
                }
                emulator.WriteByte(address + i * 0x1C + 0x10, item.MAT);
                emulator.WriteByte(address + i * 0x1C + 0x11, item.DF);
                emulator.WriteByte(address + i * 0x1C + 0x12, item.MDF);
                emulator.WriteByte(address + i * 0x1C + 0xE, item.SPD);
                emulator.WriteByte(address + i * 0x1C + 0x13, item.A_Hit);
                emulator.WriteByte(address + i * 0x1C + 0x14, item.M_Hit);
                emulator.WriteByte(address + i * 0x1C + 0x15, item.A_AV);
                emulator.WriteByte(address + i * 0x1C + 0x16, item.M_AV);
                emulator.WriteByte(address + i * 0x1C + 0x5, item.E_Half);
                emulator.WriteByte(address + i * 0x1C + 0x6, item.E_Immune);
                emulator.WriteByte(address + i * 0x1C + 0x7, item.Stat_Res);
                emulator.WriteByte(address + i * 0x1C + 0xA, item.Special1);
                emulator.WriteByte(address + i * 0x1C + 0xB, item.Special2);
                emulator.WriteByte(address + i * 0x1C + 0xC, (byte) item.Special_Ammount);
                i++;
            }
        }

        public static void ItemChange(Emulator emulator) {
            Constants.WriteOutput("Changing Thrown Items...");
            long address = Constants.GetAddress("THROWN_ITEM_TABLE");
            for (int i = 193; i < 255; i++) {
                emulator.WriteByte(address + (i - 192) * 0xC, Globals.DICTIONARY.ItemList[i].Target);
                emulator.WriteByte(address + (i - 192) * 0xC + 0x1, Globals.DICTIONARY.ItemList[i].Element);
                emulator.WriteByte(address + (i - 192) * 0xC + 0x2, Globals.DICTIONARY.ItemList[i].Damage);
                emulator.WriteByte(address + (i - 192) * 0xC + 0x3, Globals.DICTIONARY.ItemList[i].Special1);
                emulator.WriteByte(address + (i - 192) * 0xC + 0x4, Globals.DICTIONARY.ItemList[i].Special2);
                emulator.WriteByte(address + (i - 192) * 0xC + 0x5, Globals.DICTIONARY.ItemList[i].UU1);
                emulator.WriteByte(address + (i - 192) * 0xC + 0x6, Globals.DICTIONARY.ItemList[i].Special_Ammount);
                emulator.WriteByte(address + (i - 192) * 0xC + 0x8, Globals.DICTIONARY.ItemList[i].Status);
                emulator.WriteByte(address + (i - 192) * 0xC + 0x9, Globals.DICTIONARY.ItemList[i].Percentage);
                emulator.WriteByte(address + (i - 192) * 0xC + 0xA, Globals.DICTIONARY.ItemList[i].UU2);
                emulator.WriteByte(address + (i - 192) * 0xC + 0xB, Globals.DICTIONARY.ItemList[i].BaseSwitch);
            }
        }

        public static void ItemIconChange(Emulator emulator) {
            Constants.WriteOutput("Changing Item Icons...");
            long address = Constants.GetAddress("ITEM_TABLE");
            for (int i = 0; i < 186; i++) {
                emulator.WriteByte(address + i * 0x1C + 0xD, Globals.DICTIONARY.ItemList[i].Icon);
            }
            address = Constants.GetAddress("THROWN_ITEM_TABLE");
            for (int i = 192; i < 255; i++) {
                emulator.WriteByte(address + (i - 192) * 0xC + 0x7, Globals.DICTIONARY.ItemList[i].Icon);
            }
        }

        public static void ItemNameDescChange(Emulator emulator) {
            Constants.WriteOutput("Changing Item Names and Descriptions...");
            long address = Constants.GetAddress("ITEM_NAME");
            long address2 = Constants.GetAddress("ITEM_NAME_PTR");
            int len1 = String.Join("", Globals.DICTIONARY.NameList).Replace(" ", "").Length / 4;
            int len2 = (int) (address2 - address) / 2;
            if (len1 < len2) {
                emulator.WriteAOB(address, String.Join(" ", Globals.DICTIONARY.NameList));
                int i = 0;
                foreach (dynamic item in Globals.DICTIONARY.ItemList) {
                    emulator.WriteInteger(address2 + i * 0x4, (int) item.NamePointer);
                    emulator.WriteByte(address2 + i * 0x4 + 0x3, 0x80);
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
                emulator.WriteAOB(address, String.Join(" ", Globals.DICTIONARY.DescriptionList));
                int i = 0;
                foreach (dynamic item in Globals.DICTIONARY.ItemList) {
                    emulator.WriteInteger(address2 + i * 0x4, (int) item.DescriptionPointer);
                    emulator.WriteByte(address2 + i * 0x4 + 0x3, 0x80);
                    i++;
                }
            } else {
                string s = String.Format("Item description character limit exceeded! {0} / {1} characters.", len1, len2);
                Constants.WriteDebug(s);
            }
        }

        #endregion

        #region Character Changes
        public static void CharacterFieldChanges(Emulator emulator) {
            if (Globals.DRAGOON_STAT_CHANGE) {
                DragoonTableChange(emulator);
            }
            if (Globals.CHARACTER_STAT_CHANGE) {
                CharacterTableChange(emulator);
            }
            if (Globals.ADDITION_CHANGE) {
                AdditionTableChange(emulator);
            }
        }

        public static void DragoonTableChange(Emulator emulator) {
            Constants.WriteOutput("Changing Dragoon Stat table...");
            long address = Constants.GetAddress("DRAGOON_TABLE");
            int[] charReorder = new int[] { 5, 7, 0, 4, 6, 8, 1, 3, 2 };
            for (int character = 0; character < 8; character++) {
                int reorderedChar = charReorder[character];
                for (int level = 1; level < 6; level++) {
                    emulator.WriteShort(address + character * 0x30 + level * 0x8, Globals.DICTIONARY.DragoonStats[reorderedChar][level].MP);
                    emulator.WriteByte(address + character * 0x30 + level * 0x8 + 0x4, Globals.DICTIONARY.DragoonStats[reorderedChar][level].DAT);
                    emulator.WriteByte(address + character * 0x30 + level * 0x8 + 0x5, Globals.DICTIONARY.DragoonStats[reorderedChar][level].DMAT);
                    emulator.WriteByte(address + character * 0x30 + level * 0x8 + 0x6, Globals.DICTIONARY.DragoonStats[reorderedChar][level].DDF);
                    emulator.WriteByte(address + character * 0x30 + level * 0x8 + 0x7, Globals.DICTIONARY.DragoonStats[reorderedChar][level].DMDF);
                }
            }
        }

        public static void CharacterTableChange(Emulator emulator) {
            Constants.WriteOutput("Changing Character Stat table...");
            long address = Constants.GetAddress("CHAR_STAT_TABLE");
            int[] charReorder = new int[] { 7, 0, 4, 6, 1, 3, 2 };
            for (int character = 0; character < 7; character++) {
                int reorderedChar = charReorder[character];
                for (int level = 0; level < 61; level++) {
                    if (level > 0) {
                        emulator.WriteShort(address + level * 8 + character * 0x1E8, (ushort) (Globals.DICTIONARY.CharacterStats[reorderedChar][level].Max_HP));
                        emulator.WriteByte(address + level * 8 + character * 0x1E8 + 0x3, Globals.DICTIONARY.CharacterStats[reorderedChar][level].SPD);
                        emulator.WriteByte(address + level * 8 + character * 0x1E8 + 0x4, Globals.DICTIONARY.CharacterStats[reorderedChar][level].AT);
                        emulator.WriteByte(address + level * 8 + character * 0x1E8 + 0x5, Globals.DICTIONARY.CharacterStats[reorderedChar][level].MAT);
                        emulator.WriteByte(address + level * 8 + character * 0x1E8 + 0x6, Globals.DICTIONARY.CharacterStats[reorderedChar][level].DF);
                        emulator.WriteByte(address + level * 8 + character * 0x1E8 + 0x7, Globals.DICTIONARY.CharacterStats[reorderedChar][level].MDF);
                    }
                }
            }
        }

        public static void AdditionTableChange(Emulator emulator) {
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
                emulator.WriteShort(address + addition * 0xE + 0x2, sp1);
                emulator.WriteShort(address + addition * 0xE + 0x4, sp2);
                emulator.WriteShort(address + addition * 0xE + 0x6, sp3);
                emulator.WriteShort(address + addition * 0xE + 0x8, sp4);
                emulator.WriteShort(address + addition * 0xE + 0xA, sp5);
                emulator.WriteShort(address + addition * 0xE + 0xC, damage);

                emulator.WriteByte(address2 + addition * 0x18, (byte) Globals.DICTIONARY.AdditionData[character, reorderedaddition, 1].ADD_DMG_Multi);
                emulator.WriteByte(address2 + 0x4 + addition * 0x18, (byte) Globals.DICTIONARY.AdditionData[character, reorderedaddition, 2].ADD_DMG_Multi);
                emulator.WriteByte(address2 + 0x8 + addition * 0x18, (byte) Globals.DICTIONARY.AdditionData[character, reorderedaddition, 3].ADD_DMG_Multi);
                emulator.WriteByte(address2 + 0xC + addition * 0x18, (byte) Globals.DICTIONARY.AdditionData[character, reorderedaddition, 4].ADD_DMG_Multi);
                emulator.WriteByte(address2 + 0x10 + addition * 0x18, (byte) Globals.DICTIONARY.AdditionData[character, reorderedaddition, 5].ADD_DMG_Multi);
            }
        }

        #endregion

        #endregion

        #region Scripts

        #region Addition Swap
        public static void AdditionSwapInit(Emulator emulator) {
            Nullable<int> character = null;
            for (byte slot = 0; slot < 3; slot++) {
                if (Globals.CHARACTER_TABLE[slot].Read("Action") == 8) {
                    character = (int) slot;
                }
            }

            if (character != null) {
                int slot = (int) character;
                long additionUnlock = Constants.GetAddress("MENU_ADDITION_TABLE_FLAT");
                long additionXP = Constants.GetAddress("CHAR_TABLE") + 0x22;
                byte level = emulator.ReadByte((long) (Constants.GetAddress("CHAR_TABLE") + 0x12 + 0x2C * character));
                byte swappableAdditions = 1;

                if (Globals.PARTY_SLOT[slot] == (byte) 0) {
                    if (emulator.ReadByte(additionXP) >= 80 &&
                        emulator.ReadByte(additionXP + 0x1) >= 80 &&
                        emulator.ReadByte(additionXP + 0x2) >= 80 &&
                        emulator.ReadByte(additionXP + 0x3) >= 80 &&
                        emulator.ReadByte(additionXP + 0x4) >= 80 &&
                        emulator.ReadByte(additionXP + 0x5) >= 80) {
                        if (emulator.ReadByte(additionUnlock + 0xE * 6) < 255) {
                            if (level >= emulator.ReadByte(additionUnlock + 0xE * 6)) {
                                swappableAdditions = 7;
                            } else {
                                swappableAdditions = 6;
                            }
                        } else {
                            swappableAdditions = 7;
                        }
                    } else {
                        if (level >= emulator.ReadByte(additionUnlock + 0xE * 5)) {
                            swappableAdditions = 6;
                        } else if (level >= emulator.ReadByte(additionUnlock + 0xE * 4)) {
                            swappableAdditions = 5;
                        } else if (level >= emulator.ReadByte(additionUnlock + 0xE * 3)) {
                            swappableAdditions = 4;
                        } else if (level >= emulator.ReadByte(additionUnlock + 0xE * 2)) {
                            swappableAdditions = 3;
                        } else if (level >= emulator.ReadByte(additionUnlock + 0xE * 1)) {
                            swappableAdditions = 2;
                        } else {
                            swappableAdditions = 1;
                        }
                    }
                } else if (Globals.PARTY_SLOT[slot] == (byte) 1) {
                    if (emulator.ReadByte(additionXP + 0x2C) >= 80 &&
                        emulator.ReadByte(additionXP + 0x2C + 0x1) >= 80 &&
                        emulator.ReadByte(additionXP + 0x2C + 0x2) >= 80 &&
                        emulator.ReadByte(additionXP + 0x2C + 0x3) >= 80) {
                        if (emulator.ReadByte(additionUnlock + 0xE * 4 + 0x70) < 255) {
                            if (level >= emulator.ReadByte(additionUnlock + 0xE * 4 + 0x70)) {
                                swappableAdditions = 5;
                            } else {
                                swappableAdditions = 4;
                            }
                        } else {
                            swappableAdditions = 5;
                        }
                    } else {
                        if (level >= emulator.ReadByte(additionUnlock + 0xE * 4 + 0x70)) {
                            swappableAdditions = 4;
                        } else if (level >= emulator.ReadByte(additionUnlock + 0xE * 3 + 0x70)) {
                            swappableAdditions = 3;
                        } else if (level >= emulator.ReadByte(additionUnlock + 0xE * 2 + 0x70)) {
                            swappableAdditions = 2;
                        } else {
                            swappableAdditions = 1;
                        }
                    }
                } else if (Globals.PARTY_SLOT[slot] == (byte) 5) {
                    if (emulator.ReadByte(additionXP + 0xDC) >= 80 &&
                        emulator.ReadByte(additionXP + 0xDC + 0x1) >= 80 &&
                        emulator.ReadByte(additionXP + 0xDC + 0x2) >= 80 &&
                        emulator.ReadByte(additionXP + 0xDC + 0x3) >= 80) {
                        if (emulator.ReadByte(additionUnlock + 0xE * 4 + 0x70) < 255) {
                            if (level >= emulator.ReadByte(additionUnlock + 0xE * 4 + 0x70)) {
                                swappableAdditions = 5;
                            } else {
                                swappableAdditions = 4;
                            }
                        } else {
                            swappableAdditions = 5;
                        }
                    } else {
                        if (level >= emulator.ReadByte(additionUnlock + 0xE * 4 + 0x70)) {
                            swappableAdditions = 4;
                        } else if (level >= emulator.ReadByte(additionUnlock + 0xE * 3 + 0x70)) {
                            swappableAdditions = 3;
                        } else if (level >= emulator.ReadByte(additionUnlock + 0xE * 2 + 0x70)) {
                            swappableAdditions = 2;
                        } else {
                            swappableAdditions = 1;
                        }
                    }
                } else if (Globals.PARTY_SLOT[slot] == (byte) 3) {
                    if (emulator.ReadByte(additionXP + 0x84) >= 80 &&
                        emulator.ReadByte(additionXP + 0x84 + 0x1) >= 80 &&
                        emulator.ReadByte(additionXP + 0x84 + 0x2) >= 80) {
                        if (emulator.ReadByte(additionUnlock + 0xE * 3 + 0xC4) < 255) {
                            if (level >= emulator.ReadByte(additionUnlock + 0xE * 3 + 0xC4)) {
                                swappableAdditions = 4;
                            } else {
                                swappableAdditions = 3;
                            }
                        } else {
                            swappableAdditions = 4;
                        }
                    } else {
                        if (level >= emulator.ReadByte(additionUnlock + 0xE * 3 + 0xC4)) {
                            swappableAdditions = 3;
                        } else if (level >= emulator.ReadByte(additionUnlock + 0xE * 2 + 0xC4)) {
                            swappableAdditions = 2;
                        } else {
                            swappableAdditions = 1;
                        }
                    }
                } else if (Globals.PARTY_SLOT[slot] == (byte) 7) {
                    if (emulator.ReadByte(additionXP + 0x134) >= 80 &&
                        emulator.ReadByte(additionXP + 0x134 + 0x1) >= 80) {
                        if (emulator.ReadByte(additionUnlock + 0xE * 2 + 0x10A) < 255) {
                            if (level >= emulator.ReadByte(additionUnlock + 0xE * 2 + 0x10A)) {
                                swappableAdditions = 3;
                            } else {
                                swappableAdditions = 2;
                            }
                        } else {
                            swappableAdditions = 3;
                        }
                    } else {
                        if (level >= emulator.ReadByte(additionUnlock + 0xE * 2 + 0x10A)) {
                            swappableAdditions = 2;
                        } else {
                            swappableAdditions = 1;
                        }
                    }
                } else if (Globals.PARTY_SLOT[slot] == (byte) 6) {
                    if (emulator.ReadByte(additionXP + 0x108) >= 80 &&
                        emulator.ReadByte(additionXP + 0x108 + 0x1) >= 80 &&
                        emulator.ReadByte(additionXP + 0x108 + 0x2) >= 80 &&
                        emulator.ReadByte(additionXP + 0x108 + 0x3) >= 80) {
                        if (emulator.ReadByte(additionUnlock + 0xE * 4 + 0x142) < 255) {
                            if (level >= emulator.ReadByte(additionUnlock + 0xE * 4 + 0x142)) {
                                swappableAdditions = 5;
                            } else {
                                swappableAdditions = 4;
                            }
                        } else {
                            swappableAdditions = 5;
                        }
                    } else {
                        if (level >= emulator.ReadByte(additionUnlock + 0xE * 4 + 0x142)) {
                            swappableAdditions = 4;
                        } else if (level >= emulator.ReadByte(additionUnlock + 0xE * 3 + 0x142)) {
                            swappableAdditions = 3;
                        } else if (level >= emulator.ReadByte(additionUnlock + 0xE * 2 + 0x142)) {
                            swappableAdditions = 2;
                        } else {
                            swappableAdditions = 1;
                        }
                    }
                } else if (Globals.PARTY_SLOT[slot] == (byte) 4) {
                    if (emulator.ReadByte(additionXP + 0xB0) >= 80 &&
                        emulator.ReadByte(additionXP + 0xB0 + 0x1) >= 80 &&
                        emulator.ReadByte(additionXP + 0xB0 + 0x2) >= 80 &&
                        emulator.ReadByte(additionXP + 0xB0 + 0x3) >= 80 &&
                        emulator.ReadByte(additionXP + 0xB0 + 0x4) >= 80) {
                        if (emulator.ReadByte(additionUnlock + 0xE * 5 + 0x196) < 255) {
                            if (level >= emulator.ReadByte(additionUnlock + 0xE * 5 + 0x196)) {
                                swappableAdditions = 6;
                            } else {
                                swappableAdditions = 5;
                            }
                        } else {
                            swappableAdditions = 6;
                        }
                    } else {
                        if (level >= emulator.ReadByte(additionUnlock + 0xE * 5 + 0x196)) {
                            swappableAdditions = 5;
                        } else if (level >= emulator.ReadByte(additionUnlock + 0xE * 4 + 0x196)) {
                            swappableAdditions = 4;
                        } else if (level >= emulator.ReadByte(additionUnlock + 0xE * 3 + 0x196)) {
                            swappableAdditions = 3;
                        } else if (level >= emulator.ReadByte(additionUnlock + 0xE * 2 + 0x196)) {
                            swappableAdditions = 2;
                        } else {
                            swappableAdditions = 1;
                        }
                    }
                }

                AdditionSwap(emulator, (byte) character, swappableAdditions);
            }
        }

        public static void AdditionSwap(Emulator emulator, byte slot, byte additionCount) {
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
                ushort SP = Globals.CHARACTER_TABLE[slot].Read("SP");
                Globals.CHARACTER_TABLE[slot].Write("SP", 100);
                Globals.CHARACTER_TABLE[slot].Write("Action", 10);
                emulator.WriteByte(Globals.C_POINT - 0x388 * slot - 0x48, 1);
                emulator.WriteByte("DRAGOON_TURNS", 1, slot * 0x4);
                int menu = 0;
                byte addition = 0;
                for (int i = 0; i < additionCount; i++) {
                    menu += (int) Math.Pow(2, i);
                }
                Globals.CHARACTER_TABLE[slot].Write("Menu", menu);
                Thread.Sleep(50);
                emulator.WriteByte(Globals.M_POINT + 0xD32, additionCount);
                for (byte i = 0; i < additionCount; i++) {
                    emulator.WriteByte(Globals.M_POINT + 0xD34 + i * 0x2, 2);
                }

                while (true) {
                    if (emulator.ReadShort("BATTLE_VALUE") < 5130) {
                        return;
                    }
                    if (Globals.CHARACTER_TABLE[slot].Read("Action") != 9) {
                        addition = emulator.ReadByte(Globals.M_POINT + 0xD46);
                        Thread.Sleep(50);
                    } else {
                        break;
                    }
                }

                byte add = reorderAddition[character][addition];
                emulator.WriteByte(Constants.GetAddress("CHAR_TABLE") + (character * 0x2C) + 0x19, add);
                Thread.Sleep(200);
                long address = Constants.GetAddress("ADDITION") + GetOffset(emulator);
                for (int hit = 0; hit < 8; hit++) {
                    emulator.WriteShort(address + (slot * 0x100) + (hit * 0x20), (ushort) Globals.DICTIONARY.AdditionData[character, addition, hit].UU1);
                    emulator.WriteShort(address + (slot * 0x100) + (hit * 0x20) + 0x2, (ushort) Globals.DICTIONARY.AdditionData[character, addition, hit].Next_Hit);
                    emulator.WriteShort(address + (slot * 0x100) + (hit * 0x20) + 0x4, (ushort) Globals.DICTIONARY.AdditionData[character, addition, hit].Blue_Time);
                    emulator.WriteShort(address + (slot * 0x100) + (hit * 0x20) + 0x6, (ushort) Globals.DICTIONARY.AdditionData[character, addition, hit].Gray_Time);
                    emulator.WriteShort(address + (slot * 0x100) + (hit * 0x20) + 0x8, (ushort) Globals.DICTIONARY.AdditionData[character, addition, hit].DMG);
                    emulator.WriteShort(address + (slot * 0x100) + (hit * 0x20) + 0xA, (ushort) Globals.DICTIONARY.AdditionData[character, addition, hit].SP);
                    emulator.WriteShort(address + (slot * 0x100) + (hit * 0x20) + 0xC, (ushort) Globals.DICTIONARY.AdditionData[character, addition, hit].ID);
                    emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0xE, Globals.DICTIONARY.AdditionData[character, addition, hit].Final_Hit);
                    emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0xF, Globals.DICTIONARY.AdditionData[character, addition, hit].UU2);
                    emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x10, Globals.DICTIONARY.AdditionData[character, addition, hit].UU3);
                    emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x11, Globals.DICTIONARY.AdditionData[character, addition, hit].UU4);
                    emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x12, Globals.DICTIONARY.AdditionData[character, addition, hit].UU5);
                    emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x13, Globals.DICTIONARY.AdditionData[character, addition, hit].UU6);
                    emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x14, Globals.DICTIONARY.AdditionData[character, addition, hit].UU7);
                    emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x15, Globals.DICTIONARY.AdditionData[character, addition, hit].UU8);
                    emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x16, Globals.DICTIONARY.AdditionData[character, addition, hit].UU9);
                    emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x17, Globals.DICTIONARY.AdditionData[character, addition, hit].UU10);
                    emulator.WriteShort(address + (slot * 0x100) + (hit * 0x20) + 0x18, (ushort) Globals.DICTIONARY.AdditionData[character, addition, hit].Vertical_Distance);
                    emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x1A, Globals.DICTIONARY.AdditionData[character, addition, hit].UU11);
                    emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x1B, Globals.DICTIONARY.AdditionData[character, addition, hit].UU12);
                    emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x1C, Globals.DICTIONARY.AdditionData[character, addition, hit].UU13);
                    emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x1D, Globals.DICTIONARY.AdditionData[character, addition, hit].UU14);
                    emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x1E, Globals.DICTIONARY.AdditionData[character, addition, hit].Start_Time);
                    emulator.WriteByte(address + (slot * 0x100) + (hit * 0x20) + 0x1F, Globals.DICTIONARY.AdditionData[character, addition, hit].UU15);
                }
                int addition_level = emulator.ReadByte("CHAR_TABLE", (character * 0x2C) + 0x1A + addition);
                Globals.CHARACTER_TABLE[slot].Write("ADD_DMG_Multi", Globals.DICTIONARY.AdditionData[character, addition, addition_level].ADD_DMG_Multi);
                Globals.CHARACTER_TABLE[slot].Write("ADD_SP_Multi", Globals.DICTIONARY.AdditionData[character, addition, addition_level].ADD_SP_Multi);
                /*
                while ((emulator.ReadShort("BATTLE_VALUE") > 9999) && (Globals.CHARACTER_TABLE[slot].Read("Menu") != 96)) {
                    Thread.Sleep(50);
                }
                if (slot == 0 && Globals.NO_DART == 4) {
                    HaschelFix(emulator);
                }
                emulator.WriteByte("DRAGOON_TURNS", 1, slot * 0x4);
                */
                ushort turn = Globals.CHARACTER_TABLE[slot].Read("Turn");
                Globals.CHARACTER_TABLE[slot].Write("Turn", 800);
                ushort MP = Globals.CHARACTER_TABLE[slot].Read("MP");
                Globals.CHARACTER_TABLE[slot].Write("MP", 0);
                ushort HP_reg = Globals.CHARACTER_TABLE[slot].Read("HP_Regen");
                Globals.CHARACTER_TABLE[slot].Write("HP_Regen", 0);
                ushort MP_reg = Globals.CHARACTER_TABLE[slot].Read("MP_Regen");
                Globals.CHARACTER_TABLE[slot].Write("MP_Regen", 1);
                ushort SP_reg = Globals.CHARACTER_TABLE[slot].Read("SP_Regen");
                Globals.CHARACTER_TABLE[slot].Write("SP_Regen", 0);
                /*
                ushort val = emulator.ReadShort(Globals.C_POINT - slot * 0x388 - 0xF0);
                emulator.WriteShort(Globals.C_POINT - slot * 0x388 - 0xF0, (ushort)(val + 0x4478));
                emulator.WriteByte(Globals.C_POINT - slot * 0x388 - 0xEE, 27);
                */

                while (true) {
                    if (emulator.ReadShort("BATTLE_VALUE") < 5130) {
                        return;
                    }
                    if (Globals.CHARACTER_TABLE[slot].Read("MP") != 1) {
                        Thread.Sleep(50);
                    } else {
                        break;
                    }
                }

                Globals.CHARACTER_TABLE[slot].Write("SP", SP);
                Globals.CHARACTER_TABLE[slot].Write("MP", MP);
                Globals.CHARACTER_TABLE[slot].Write("Turn", turn);
                Globals.CHARACTER_TABLE[slot].Write("HP_Regen", HP_reg);
                Globals.CHARACTER_TABLE[slot].Write("MP_Regen", MP_reg);
                Globals.CHARACTER_TABLE[slot].Write("SP_Regen", SP_reg);
                Globals.ADDITION_SWAP = false;
                Constants.WriteGLogOutput("Addition swap complete.");
            }
        }

        #endregion

        #region Addition Level Up in Battle
        public static void AdditionLevelUp(Emulator emulator) {
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
                    int addition = additionnum[emulator.ReadByte("CHAR_TABLE", (character * 0x2C) + 0x19)];
                    int level = emulator.ReadByte("CHAR_TABLE", (character * 0x2C) + 0x1A + addition);
                    int newlevel = 1 + emulator.ReadByte("CHAR_TABLE", (character * 0x2C) + 0x22 + addition) / 20;
                    if (newlevel > level) {
                        Constants.WriteDebug(newlevel);
                        emulator.WriteByte(Constants.GetAddress("CHAR_TABLE") + (character * 0x2C) + 0x1A + addition, (byte) newlevel);
                        Globals.CHARACTER_TABLE[slot].Write("ADD_DMG_Multi", Globals.DICTIONARY.AdditionData[character, addition, newlevel].ADD_DMG_Multi);
                        Globals.CHARACTER_TABLE[slot].Write("ADD_SP_Multi", Globals.DICTIONARY.AdditionData[character, addition, newlevel].ADD_SP_Multi);
                    }
                }
            }
        }
        #endregion

        #region Black Room

        public static void BlackRoomField(Emulator emulator) {
            if ((Globals.MAP >= 5 && Globals.MAP <= 7) || (Globals.MAP >= 624 && Globals.MAP <= 625)) {
                emulator.WriteByte("BATTLE_FIELD", 96);
            }
        }

        public static void BlackRoomBattle(Emulator emulator) {
            if ((Globals.MAP >= 5 && Globals.MAP <= 7) || (Globals.MAP >= 624 && Globals.MAP <= 625)) {
                WipeRewards(emulator);
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
        public static void NeverGuard(Emulator emulator) {
            for (int i = 0; i < 3; i++) {
                if (Globals.PARTY_SLOT[i] > 8) {
                    break;
                }
                Globals.CHARACTER_TABLE[i].Write("Guard", 0);
            }
        }

        #endregion

        #region Damage Cap Removal
        public static void RemoveDamageCap(Emulator emulator) {
            if (!firstDamageCapRemoval) {
                emulator.WriteInteger("DAMAGE_CAP", 50000);
                emulator.WriteInteger("DAMAGE_CAP", 50000, 0x8);
                emulator.WriteInteger("DAMAGE_CAP", 50000, 0x14);
                DamageCapScan(emulator);
                firstDamageCapRemoval = true;
            } else {
                ushort currentItem = emulator.ReadShort(Globals.M_POINT + 0xABC);
                if (lastItemUsedDamageCap != currentItem) {
                    lastItemUsedDamageCap = currentItem;
                    if ((lastItemUsedDamageCap >= 0xC1 && lastItemUsedDamageCap <= 0xCA) || (lastItemUsedDamageCap >= 0xCF && lastItemUsedDamageCap <= 0xD2) || lastItemUsedDamageCap == 0xD6 || lastItemUsedDamageCap == 0xD8 || lastItemUsedDamageCap == 0xDC || (lastItemUsedDamageCap >= 0xF1 && lastItemUsedDamageCap <= 0xF8) || lastItemUsedDamageCap == 0xFA) {
                        DamageCapScan(emulator);
                    }
                }
                for (int i = 0; i < 3; i++) {
                    if (Globals.PARTY_SLOT[i] < 9) {
                        if (Globals.CHARACTER_TABLE[i].Read("Action") == 24) {
                            DamageCapScan(emulator);
                        }
                    }
                }
                for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                    if (Globals.MONSTER_TABLE[i].Read("Action") == 28) { //Most used, not all monsters use action code 28 for item spells
                        DamageCapScan(emulator);
                    }
                }
            }
        }

        public static void DamageCapScan(Emulator emulator) {
            ArrayList damageCapScan = emulator.ScanAllAOB("0F 27", 0xA8660, 0x2A865F);
            long lastAddress = 0;
            foreach (var address in damageCapScan) {
                long capAddress = (long) address;
                if (emulator.ReadShortU(capAddress) == 9999 && (lastAddress + 0x10) == capAddress) {
                    emulator.WriteIntegerU(capAddress, 50000);
                    emulator.WriteIntegerU(lastAddress, 50000);
                }
                lastAddress = capAddress;
            }
        }

        #endregion

        #region Change Aspect Ratio

        public static void ChangeAspectRatio(Emulator emulator) {
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

            emulator.WriteShort("ASPECT_RATIO", aspectRatio);

            if (cameraOption == 1)
                emulator.WriteShort("ADVANCED_CAMERA", aspectRatio);
        }

        #endregion

        #region Kill BGM

        public static void KillBGM(Emulator emulator) {
            ArrayList bgmScan = emulator.ScanAllAOB("53 53 73 71", 0xA8660, 0x2A865F);
            foreach (var address in bgmScan) {
                for (int i = 0; i <= 255; i++) {
                    emulator.WriteByteU((long) address + i, 0);
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
                Globals.CHARACTER_TABLE[slot].Write("Dragoon", 0);
            }
        }
        #endregion

        #endregion

        #region Hard/Hell Mode specific

        public static void HardHellModeSetup(Emulator emulator) {
            if (difficulty.Contains("Hell")) {
                HellDragoonChanges(emulator);
            }
            if (difficulty == "Hell") {
                ApplyNoEscape(emulator);
            }
        }

        #region Apply No Escape
        public static void ApplyNoEscape(Emulator emulator) {
            if (Globals.CheckDMScript("btnBlackRoom")) {
                if (!((Globals.MAP >= 5 && Globals.MAP <= 7) || (Globals.MAP >= 624 && Globals.MAP <= 625))) {
                    for (int i = 0; i < 3; i++) {
                        emulator.WriteByte("NO_ESCAPE", 8, (Globals.MONSTER_SIZE + i) * 0x20);
                    }
                }
            } else {
                for (int i = 0; i < 3; i++) {
                    emulator.WriteByte("NO_ESCAPE", 8, (Globals.MONSTER_SIZE + i) * 0x20);
                }
            }
        }

        #endregion

        #region Hell Mode Dragoon Changes

        public static void HellDragoonChanges(Emulator emulator) {
            emulator.WriteByte("SPELL_TABLE", (flowerStorm) * 20, 0x7 + (7 * 0xC)); //Lavitz's Blossom Storm MP
            emulator.WriteByte("SPELL_TABLE", (flowerStorm) * 20, 0x7 + (26 * 0xC)); //Albert's Rose storm MP

            if (Constants.REGION == Region.NTA) {
                emulator.WriteShort(Globals.DRAGOON_SPELLS[7].Description_Pointer + 44, (ushort)(0x15 + flowerStorm));
                emulator.WriteShort(Globals.DRAGOON_SPELLS[26].Description_Pointer + 44, (ushort)(0x15 + flowerStorm));
            }

            emulator.WriteByte("SPELL_TABLE", 20, 0x7 + (11 * 0xC)); //Shana's Moon Light MP
            emulator.WriteByte("SPELL_TABLE", 20, 0x7 + (66 * 0xC)); //???'s Moon Light MP
            emulator.WriteByte("SPELL_TABLE", 30, 0x7 + (25 * 0xC)); //Rainbow Breath MP
            emulator.WriteByte("SPELL_TABLE", 40, 0x7 + (12 * 0xC)); //Shana's Gates of Heaven MP
            emulator.WriteByte("SPELL_TABLE", 40, 0x7 + (67 * 0xC)); //???'s Gates of Heaven MP
        }

        #endregion

        #region Dual Difficulty

        public static void SwitchDualDifficulty(Emulator emulator) {
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

        public static void ReduceSP(Emulator emulator) {
            if (bossSPLoss != 0) {
                for (int character = 0; character < 9; character++) {
                    ushort currentTotalSP = emulator.ReadShort("CHAR_TABLE", (character * 0x2C) + 0xE);
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

                    emulator.WriteShort("CHAR_TABLE", (ushort) newSP, (character * 0x2C) + 0xE);

                    byte dragoonLevel = emulator.ReadByte("CHAR_TABLE", (character * 0x2C) + 0x13);

                    if (character == 0 || character == 3) {
                        if (newSP < 20000 && dragoonLevel >= 5)
                            emulator.WriteByte("CHAR_TABLE", 4, (character * 0x2C) + 0x13);
                        if (newSP < 12000 && dragoonLevel >= 4)
                            emulator.WriteByte("CHAR_TABLE", 3, (character * 0x2C) + 0x13);
                        if (newSP < 6000 && dragoonLevel >= 3)
                            emulator.WriteByte("CHAR_TABLE", 2, (character * 0x2C) + 0x13);
                        if (newSP < 1200 && dragoonLevel >= 2)
                            emulator.WriteByte("CHAR_TABLE", 1, (character * 0x2C) + 0x13);
                    } else if (character == 6 || character == 7) {
                        if (newSP < 20000 && dragoonLevel >= 5)
                            emulator.WriteByte("CHAR_TABLE", 4, (character * 0x2C) + 0x13);
                        if (newSP < 12000 && dragoonLevel >= 4)
                            emulator.WriteByte("CHAR_TABLE", 3, (character * 0x2C) + 0x13);
                        if (newSP < 2000 && dragoonLevel >= 3)
                            emulator.WriteByte("CHAR_TABLE", 2, (character * 0x2C) + 0x13);
                        if (newSP < 1200 && dragoonLevel >= 2)
                            emulator.WriteByte("CHAR_TABLE", 1, (character * 0x2C) + 0x13);
                    } else {
                        if (newSP < 20000 && dragoonLevel >= 5)
                            emulator.WriteByte("CHAR_TABLE", 4, (character * 0x2C) + 0x13);
                        if (newSP < 12000 && dragoonLevel >= 4)
                            emulator.WriteByte("CHAR_TABLE", 3, (character * 0x2C) + 0x13);
                        if (newSP < 6000 && dragoonLevel >= 3)
                            emulator.WriteByte("CHAR_TABLE", 2, (character * 0x2C) + 0x13);
                        if (newSP < 1000 && dragoonLevel >= 2)
                            emulator.WriteByte("CHAR_TABLE", 1, (character * 0x2C) + 0x13);
                    }
                }
                bossSPLoss = 0;
            }
        }

        #endregion

        #region Equipment Changes

        public static void EquipChangesSetup(Emulator emulator) {
            Chapter3buffs();
            PercBuffs(emulator);
            SpecialEquipSetup(emulator);
        }

        public static void Chapter3buffs() {
            for (int slot = 0; slot < 3; slot++) {
                if (Globals.PARTY_SLOT[slot] > 8) {
                    break;
                }
                if (Globals.CHAPTER >= 3) {
                    if (Globals.CHARACTER_TABLE[slot].Read("Weapon") == 28) { //Sparkle Arrow
                        Globals.CHARACTER_TABLE[slot].Write("AT", (Globals.CHARACTER_TABLE[slot].Read("AT") + 8));
                    }

                    if (Globals.CHARACTER_TABLE[slot].Read("Weapon") == 2) { //Heat Blade
                        Globals.CHARACTER_TABLE[slot].Write("AT", (Globals.CHARACTER_TABLE[slot].Read("AT") + 7));
                    }

                    if (Globals.CHARACTER_TABLE[slot].Read("Weapon") == 14) { //Shadow Cutter
                        Globals.CHARACTER_TABLE[slot].Write("AT", (Globals.CHARACTER_TABLE[slot].Read("AT") + 9));
                    }

                    if (Globals.CHARACTER_TABLE[slot].Read("Weapon") == 35) { //Morning Star
                        Globals.CHARACTER_TABLE[slot].Write("AT", (Globals.CHARACTER_TABLE[slot].Read("AT") - 20));
                        Globals.CHARACTER_TABLE[slot].Write("Element", 1);
                    }
                }
            }
        }

        public static void PercBuffs(Emulator emulator) {
            for (int slot = 0; slot < 3; slot++) {
                if (Globals.PARTY_SLOT[slot] > 8) {
                    break;
                }

                if ((Globals.PARTY_SLOT[slot] == 2 || Globals.PARTY_SLOT[slot] == 8) && emulator.ReadByte("CHAR_TABLE", 0x6A) >= 30) {
                    Globals.CHARACTER_TABLE[slot].Write("DF", Math.Round(Globals.CHARACTER_TABLE[slot].Read("DF") * 1.12));
                    byte level = emulator.ReadByte("CHAR_TABLE", 0x12 + (0x2C * slot));
                    double boost = 1;
                    if (Globals.CHARACTER_TABLE[slot].Read("Weapon") == 32) {
                        boost = 1.4;
                    } else if (level >= 28) {
                        boost = 2.15;
                    } else if (level >= 20) {
                        boost = 1.9;
                    } else if (level >= 10) {
                        boost = 1.6;
                    }
                    Globals.CHARACTER_TABLE[slot].Write("AT", Math.Round(Globals.CHARACTER_TABLE[slot].Read("AT") * boost));
                }

                if (Globals.PARTY_SLOT[slot] == 8 && emulator.ReadByte("CHAR_TABLE", 0x172) >= 30) {
                    Globals.CHARACTER_TABLE[slot].Write("DF", Math.Round(Globals.CHARACTER_TABLE[slot].Read("DF") * 1.12));
                }

                if (Globals.PARTY_SLOT[slot] == 3 && emulator.ReadByte("CHAR_TABLE", 0x96) >= 30) {
                    Globals.CHARACTER_TABLE[slot].Write("DF", Math.Round(Globals.CHARACTER_TABLE[slot].Read("DF") * 1.1));
                }

                if (Globals.PARTY_SLOT[slot] == 6 && emulator.ReadByte("CHAR_TABLE", 0x11A) >= 30) {
                    Globals.CHARACTER_TABLE[slot].Write("DF", Math.Round(Globals.CHARACTER_TABLE[slot].Read("DF") * 1.26));
                }
            }
        }

        public static void SpecialEquipSetup(Emulator emulator) {
            soasSiphonSlot = -1;
            for (int i = 0; i < 3; i++) {
                if (Globals.PARTY_SLOT[i] < 9) {
                    long p = Globals.CHAR_ADDRESS[i];
                    int s = 0;
                    if (Globals.CHARACTER_TABLE[i].Read("Accessory") == 149) { //Phantom Shield
                        s = Globals.CHARACTER_TABLE[i].Read("DF");
                        if (Globals.CHARACTER_TABLE[i].Read("Armor") == 74) {
                            Globals.CHARACTER_TABLE[i].Write("DF", (Math.Ceiling(s * 1.1)));
                        } else {
                            Globals.CHARACTER_TABLE[i].Write("DF", (Math.Ceiling(s * 0.7)));
                        }
                        s = Globals.CHARACTER_TABLE[i].Read("MDF");
                        if (Globals.CHARACTER_TABLE[i].Read("Helmet") == 89) {
                            Globals.CHARACTER_TABLE[i].Write("MDF", (Math.Ceiling(s * 1.1)));
                        } else {
                            Globals.CHARACTER_TABLE[i].Write("MDF", (Math.Ceiling(s * 0.7)));
                        }
                    }

                    if (Globals.CHARACTER_TABLE[i].Read("Accessory") == 150) { //Dragon Shield
                        s = Globals.CHARACTER_TABLE[i].Read("DF");
                        if (Globals.CHARACTER_TABLE[i].Read("Armor") == 74) {
                            Globals.CHARACTER_TABLE[i].Write("DF", (Math.Ceiling(s * 1.2)));
                        } else {
                            Globals.CHARACTER_TABLE[i].Write("DF", (Math.Ceiling(s * 0.7)));
                        }
                    }

                    if (Globals.CHARACTER_TABLE[i].Read("Accessory") == 151) { //Angel Scarf
                        s = Globals.CHARACTER_TABLE[i].Read("MDF");
                        if (Globals.CHARACTER_TABLE[i].Read("Helmet") == 89) {
                            Globals.CHARACTER_TABLE[i].Write("MDF", (Math.Ceiling(s * 1.2)));
                        } else {
                            Globals.CHARACTER_TABLE[i].Write("MDF", (Math.Ceiling(s * 0.7)));
                        }
                    }

                    if (Globals.CHARACTER_TABLE[i].Read("Weapon") == 159 && Globals.PARTY_SLOT[i] == 0) { //Spirit Eater
                        spiritEaterSP = 35;
                        if (Globals.DIFFICULTY_MODE.Contains("Hell")) {
                            spiritEaterSP = 15;
                        }
                        spiritEaterSaveSP = Globals.CHARACTER_TABLE[i].Read("SP_Regen");
                        if (spiritEaterSaveSP < spiritEaterSP) {
                            Globals.CHARACTER_TABLE[i].Write("SP_Regen", (65536 - (spiritEaterSP - spiritEaterSaveSP)));
                        } else {
                            Globals.CHARACTER_TABLE[i].Write("SP_Regen", (spiritEaterSaveSP - spiritEaterSP));
                        }
                    }

                    if (Globals.CHARACTER_TABLE[i].Read("Weapon") == 161 && (Globals.PARTY_SLOT[i] == 2 || Globals.PARTY_SLOT[i] == 8)) { //Element Arrow
                        Globals.CHARACTER_TABLE[i].Write("Element", elementArrowElement);
                        elementArrowLastAction = 255;
                        elementArrowTurns = 0;
                    }

                    if (Globals.CHARACTER_TABLE[i].Read("Weapon") == 163 && Globals.PARTY_SLOT[i] == 4) { //Battery Glove
                        gloveLastAction = 0;
                        gloveCharge = 0;
                    }

                    if (Globals.CHARACTER_TABLE[i].Read("Weapon") == 165 && Globals.PARTY_SLOT[i] == 7) { //Giant Axe
                        axeLastAction = 0;
                    }

                    if (Globals.CHARACTER_TABLE[i].Read("Weapon") == 166) { //Soa's Light
                        Globals.CHARACTER_TABLE[i].Write("SP_Multi", 65436);
                        Globals.CHARACTER_TABLE[i].Write("SP_Regen", 100);
                        for (int x = 0; x < 3; x++) {
                            if (x != i && Globals.PARTY_SLOT[x] < 9) {
                                Globals.CHARACTER_TABLE[x].Write("DF", Math.Round(Globals.CHARACTER_TABLE[i].Read("DF") * 0.7));
                                Globals.CHARACTER_TABLE[x].Write("MDF", Math.Round(Globals.CHARACTER_TABLE[i].Read("MDF") * 0.7));
                            }
                        }
                    }

                    if (Globals.CHARACTER_TABLE[i].Read("Helmet") == 168) { //Soa's Helm
                        for (int x = 0; x < 3; x++) {
                            if (x != i && Globals.PARTY_SLOT[x] < 9) {
                                Globals.CHARACTER_TABLE[x].Write("AT", Math.Round(Globals.CHARACTER_TABLE[x].Read("AT") * 0.7));
                            }
                        }
                    }

                    if (Globals.CHARACTER_TABLE[i].Read("Armor") == 170) { //Divine DG Armor
                        Globals.CHARACTER_TABLE[i].Write("SP_P_Hit", (Globals.CHARACTER_TABLE[i].Read("SP_P_Hit") + 20));
                        Globals.CHARACTER_TABLE[i].Write("MP_P_Hit", (Globals.CHARACTER_TABLE[i].Read("MP_P_Hit") + 10));
                        Globals.CHARACTER_TABLE[i].Write("SP_M_Hit", (Globals.CHARACTER_TABLE[i].Read("SP_M_Hit") + 20));
                        Globals.CHARACTER_TABLE[i].Write("MP_M_Hit", (Globals.CHARACTER_TABLE[i].Read("MP_M_Hit") + 10));
                    }

                    if (Globals.CHARACTER_TABLE[i].Read("Armor") == 171) { //Soa's Armor
                        for (int x = 0; x < 3; x++) {
                            if (x != i && Globals.PARTY_SLOT[x] < 9) {
                                Globals.CHARACTER_TABLE[x].Write("MAT", Math.Round(Globals.CHARACTER_TABLE[x].Read("MAT") * 0.7));
                            }
                        }
                    }

                    if (Globals.CHARACTER_TABLE[i].Read("Shoes") == 174) { //Soa's Greaves
                        for (int x = 0; x < 3; x++) {
                            if (x != i && Globals.PARTY_SLOT[x] < 9) {
                                Globals.CHARACTER_TABLE[x].Write("SPD", (Globals.CHARACTER_TABLE[x].Read("SPD") - 25));
                            }
                        }
                    }

                    if (Globals.CHARACTER_TABLE[i].Read("Accessory") == 175) { //Heal Ring
                        if (Globals.PARTY_SLOT[i] == 2 || Globals.PARTY_SLOT[i] == 8) {
                            recoveryRateSave = Globals.CHARACTER_TABLE[i].Read("HP_Regen");
                        }
                    }

                    if (Globals.CHARACTER_TABLE[i].Read("Accessory") == 176) { //Soa's Sash
                        for (int x = 0; x < 3; x++) {
                            if (x != i && Globals.PARTY_SLOT[x] < 9) {
                                ushort spMulti = Globals.CHARACTER_TABLE[x].Read("SP_Multi");
                                if (spMulti == 0) {
                                    Globals.CHARACTER_TABLE[x].Write("SP_Multi", 65486);
                                } else {
                                    Globals.CHARACTER_TABLE[x].Write("SP_Multi", (Globals.CHARACTER_TABLE[x].Read("SP_Multi") - 50));
                                }
                            }
                        }
                    }

                    if (Globals.CHARACTER_TABLE[i].Read("Accessory") == 177) { //Soa's Ahnk
                        if (Globals.CheckDMScript("btnSoloMode")) {
                            Globals.CHARACTER_TABLE[i].Write("Revive", (Globals.CHARACTER_TABLE[i].Read("Revive") - 50));
                        }
                    }

                    if (Globals.CHARACTER_TABLE[i].Read("Accessory") == 178) { //Soa's Health Ring
                        for (int x = 0; x < 3; x++) {
                            if (x != i && Globals.PARTY_SLOT[x] < 9) {
                                Globals.CHARACTER_TABLE[x].Write("Max_HP", Math.Round(Globals.CHARACTER_TABLE[x].Read("Max_HP") * 0.75));

                                if (Globals.CHARACTER_TABLE[x].Read("HP") > Globals.CHARACTER_TABLE[x].Read("Max_HP")) {
                                    Globals.CHARACTER_TABLE[x].Write("HP", Globals.CHARACTER_TABLE[x].Read("Max_HP"));
                                }
                            } else {
                                if (Globals.PARTY_SLOT[x] < 9) {
                                    ushort maxhp = Globals.CHARACTER_TABLE[x].Read("Max_HP") * 2;
                                    Globals.CHARACTER_TABLE[x].Write("HP", Globals.CHARACTER_TABLE[x].Read("HP") * 2);
                                    Globals.CHARACTER_TABLE[x].Write("Max_HP", Math.Min(short.MaxValue, maxhp));
                                    if (Globals.CHARACTER_TABLE[x].Read("HP") > Globals.CHARACTER_TABLE[x].Read("Max_HP")) {
                                        Globals.CHARACTER_TABLE[x].Write("HP", Globals.CHARACTER_TABLE[x].Read("Max_HP"));
                                    }
                                }
                            }
                        }
                    }

                    if (Globals.CHARACTER_TABLE[i].Read("Accessory") == 179) { //Soa's Mage Ring
                        Globals.CHARACTER_TABLE[i].Write("MP", (Globals.CHARACTER_TABLE[i].Read("MP") * 3));
                        Globals.CHARACTER_TABLE[i].Write("Max_MP", (Globals.CHARACTER_TABLE[i].Read("Max_MP") * 3));
                        for (int x = 0; x < 3; x++) {
                            if (x != i && Globals.PARTY_SLOT[x] < 9) {
                                Globals.CHARACTER_TABLE[x].Write("MP", (ushort) Math.Round(Globals.CHARACTER_TABLE[x].Read("Max_MP") * 0.5));

                                if (Globals.CHARACTER_TABLE[x].Read("MP") > Globals.CHARACTER_TABLE[x].Read("Max_MP")) {
                                    Globals.CHARACTER_TABLE[x].Write("MP", Globals.CHARACTER_TABLE[x].Read("Max_MP"));
                                }
                            }
                        }
                    }

                    if (Globals.CHARACTER_TABLE[i].Read("Accessory") == 181) { //Soa's Siphon Ring
                        soasSiphonSlot = i;
                        Globals.CHARACTER_TABLE[i].Write("MAT", (Globals.CHARACTER_TABLE[i].Read("MAT") * 2));
                        Globals.CHARACTER_TABLE[i].Write("DMAT", Math.Round(Globals.CHARACTER_TABLE[i].Read("DMAT") * 0.3));
                        for (int x = 0; x < 3; x++) {
                            if (x != i && Globals.PARTY_SLOT[x] < 9) {
                                Globals.CHARACTER_TABLE[x].Write("MAT", Math.Round(Globals.CHARACTER_TABLE[x].Read("MAT") * 0.8));
                            }
                        }
                    }

                    if (Globals.CHARACTER_TABLE[i].Read("Armor") == 74) { //Armor of Legend
                        if (Globals.PARTY_SLOT[i] == 0) {
                            Globals.CHARACTER_TABLE[i].Write("DF", (Globals.CHARACTER_TABLE[i].Read("DF") + 41 - 127));
                            Globals.CHARACTER_TABLE[i].Write("MDF", (Globals.CHARACTER_TABLE[i].Read("MDF") + 40));
                        } else if (Globals.PARTY_SLOT[i] == 1 || Globals.PARTY_SLOT[i] == 5) {
                            Globals.CHARACTER_TABLE[i].Write("DF", (Globals.CHARACTER_TABLE[i].Read("DF") + 54 - 127));
                            Globals.CHARACTER_TABLE[i].Write("MDF", (Globals.CHARACTER_TABLE[i].Read("MDF") + 27));
                        } else if (Globals.PARTY_SLOT[i] == 2 || Globals.PARTY_SLOT[i] == 8) {
                            Globals.CHARACTER_TABLE[i].Write("DF", (Globals.CHARACTER_TABLE[i].Read("DF") + 27 - 127));
                            Globals.CHARACTER_TABLE[i].Write("MDF", (Globals.CHARACTER_TABLE[i].Read("MDF") + 80));
                        } else if (Globals.PARTY_SLOT[i] == 3) {
                            Globals.CHARACTER_TABLE[i].Write("DF", (Globals.CHARACTER_TABLE[i].Read("DF") + 41 - 127));
                            Globals.CHARACTER_TABLE[i].Write("MDF", (Globals.CHARACTER_TABLE[i].Read("MDF") + 42));
                        } else if (Globals.PARTY_SLOT[i] == 4) {
                            Globals.CHARACTER_TABLE[i].Write("DF", (Globals.CHARACTER_TABLE[i].Read("DF") + 45 - 127));
                            Globals.CHARACTER_TABLE[i].Write("MDF", (Globals.CHARACTER_TABLE[i].Read("MDF") + 40));
                        } else if (Globals.PARTY_SLOT[i] == 6) {
                            Globals.CHARACTER_TABLE[i].Write("DF", (Globals.CHARACTER_TABLE[i].Read("DF") + 30 - 127));
                            Globals.CHARACTER_TABLE[i].Write("MDF", (Globals.CHARACTER_TABLE[i].Read("MDF") + 54));
                        } else if (Globals.PARTY_SLOT[i] == 7) {
                            Globals.CHARACTER_TABLE[i].Write("DF", (Globals.CHARACTER_TABLE[i].Read("DF") + 88 - 127));
                            Globals.CHARACTER_TABLE[i].Write("MDF", (Globals.CHARACTER_TABLE[i].Read("MDF") + 23));
                        }
                    }

                    if (Globals.CHARACTER_TABLE[i].Read("Accessory") == 180) { //Soa's Shield Ring
                        Globals.CHARACTER_TABLE[i].Write("HP", 1);
                        Globals.CHARACTER_TABLE[i].Write("Max_HP", 1);
                        Globals.CHARACTER_TABLE[i].Write("DF", 10);
                        Globals.CHARACTER_TABLE[i].Write("MDF", 10);
                        Globals.CHARACTER_TABLE[i].Write("A_AV", 90);
                        Globals.CHARACTER_TABLE[i].Write("M_AV", 90);
                        for (int x = 0; x < 3; x++) {
                            if (x != i && Globals.PARTY_SLOT[x] < 9) {
                                Globals.CHARACTER_TABLE[x].Write("A_HIT", Math.Round(Globals.CHARACTER_TABLE[x].Read("A_HIT") * 0.8));
                                Globals.CHARACTER_TABLE[x].Write("M_HIT", Math.Round(Globals.CHARACTER_TABLE[x].Read("M_HIT") * 0.8));
                            }
                        }
                    }

                    if (Globals.PARTY_SLOT[i] == 7) {
                        if (Globals.CHARACTER_TABLE[i].Read("SPD") >= 40) {
                            Globals.CHARACTER_TABLE[i].Write("SPD", 30 + Math.Round((double) (Globals.CHARACTER_TABLE[i].Read("SPD") - 30) / 2));
                        }
                    }

                    guardStatusDF[i] = 0;
                    guardStatusMDF[i] = 0;
                    lGuardStatusDF[i] = 0;
                    lGuardStatusMDF[i] = 0;
                    lGuardStateDF[i] = false;
                    lGuardStateMDF[i] = false;
                    sGuardStatusDF[i] = false;
                    sGuardStatusMDF[i] = false;
                }
            }
        }

        public static void EquipRun(Emulator emulator) {
            if (emulator.ReadShort("BATTLE_VALUE") == 41215 && Globals.STATS_CHANGED /*&& equipChangesLoop*/) { //Battle Loop
                for (int i = 0; i < 3; i++) {
                    if (Globals.PARTY_SLOT[i] < 9) {
                        long p = Globals.CHAR_ADDRESS[i];
                        if (Globals.CHARACTER_TABLE[i].Read("Weapon") == 159 && Globals.PARTY_SLOT[i] == 0) { //Spirit Eater
                            if (Globals.CHARACTER_TABLE[i].Read("SP") == (emulator.ReadByte("CHAR_TABLE", 0x13) * 100)) {
                                Globals.CHARACTER_TABLE[i].Write("SP_Regen", spiritEaterSaveSP);
                            } else {
                                if (spiritEaterSaveSP < spiritEaterSP) {
                                    Globals.CHARACTER_TABLE[i].Write("SP_Regen", (65536 - (spiritEaterSP - spiritEaterSaveSP)));
                                } else {
                                    Globals.CHARACTER_TABLE[i].Write("SP_Regen", (spiritEaterSaveSP - spiritEaterSP));
                                }
                            }
                        }

                        if (Globals.CHARACTER_TABLE[i].Read("Weapon") == 160 && (Globals.PARTY_SLOT[i] == 1 || Globals.PARTY_SLOT[i] == 5)) { //Harpoon
                            if (Globals.CHARACTER_TABLE[i].Read("Action") == 10 && Globals.CHARACTER_TABLE[i].Read("SP") >= 400) {
                                checkHarpoon = true;
                                if (Globals.CHARACTER_TABLE[i].Read("SP") == 500) {
                                    emulator.WriteAOB(p + 0xC0, "00 00 00 04");
                                    Globals.CHARACTER_TABLE[i].Write("SP", 200);
                                    emulator.WriteByte("DRAGOON_TURNS", 2, i * 4);
                                } else {
                                    emulator.WriteAOB(p + 0xC0, "00 00 00 03");
                                    Globals.CHARACTER_TABLE[i].Write("SP", 100);
                                    emulator.WriteByte("DRAGOON_TURNS", 1, i * 4);
                                }
                            }

                            if (emulator.ReadByte("DRAGOON_TURNS", i * 4) == 0) {
                                checkHarpoon = false;
                            }
                        }

                        if (Globals.CHARACTER_TABLE[i].Read("Weapon") == 161 && (Globals.PARTY_SLOT[i] == 2 || Globals.PARTY_SLOT[i] == 8)) { //Element Arrow
                            if (elementArrowLastAction != Globals.CHARACTER_TABLE[i].Read("Action")) {
                                if (elementArrowLastAction == 8 && Globals.CHARACTER_TABLE[i].Read("Action") == 136) {
                                    //old method ...
                                } else {
                                    elementArrowLastAction = Globals.CHARACTER_TABLE[i].Read("Action");
                                    if (elementArrowLastAction == 8) {
                                        Globals.CHARACTER_TABLE[i].Write("Element", elementArrowElement);
                                        elementArrowTurns += 1;
                                    } else {
                                        if (elementArrowLastAction == 10) {
                                            Globals.CHARACTER_TABLE[i].Write("Element", 0);
                                        }
                                        if (elementArrowTurns == 4) {
                                            elementArrowTurns = 0;
                                            if (emulator.ReadInteger("GOLD") >= 100) {
                                                for (int x = 0; x < inventorySize; x++) {
                                                    if (emulator.ReadByte("INVENTORY", x) == 255) {
                                                        emulator.WriteByte("INVENTORY", elementArrowItem, x);
                                                        emulator.WriteByte("INVENTORY_SIZE", emulator.ReadByte("INVENTORY_SIZE") + 1);
                                                        emulator.WriteInteger("GOLD", emulator.ReadInteger("GOLD") - 100);
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (Globals.CHARACTER_TABLE[i].Read("Weapon") == 162 && Globals.PARTY_SLOT[i] == 3) { //Dragon Beater
                            if (Globals.CHARACTER_TABLE[i].Read("Action") == 136) {
                                if (emulator.ReadShort("DAMAGE_SLOT1") != 0) {
                                    emulator.WriteShort(p, (ushort) Math.Min(Globals.CHARACTER_TABLE[i].Read("HP") + Math.Round(emulator.ReadShort("DAMAGE_SLOT1") * 0.02) + 2, Globals.CHARACTER_TABLE[i].Read("Max_HP")));
                                    emulator.WriteShort("DAMAGE_SLOT1", 0);
                                    /*} else {
                                        emulator.WriteShort("DAMAGE_SLOT1", 0);*/
                                }
                            }
                        }

                        if (Globals.CHARACTER_TABLE[i].Read("Weapon") == 163 && Globals.PARTY_SLOT[i] == 4) { //Battery Glove
                            if ((Globals.CHARACTER_TABLE[i].Read("Action") == 136 || Globals.CHARACTER_TABLE[i].Read("Action") == 26) &&
                            (gloveLastAction != 136 && gloveLastAction != 26)) {
                                gloveCharge += 1;
                                if (gloveCharge == 7) {
                                    Globals.CHARACTER_TABLE[i].Write("AT", Math.Round(originalCharacterStats[i, 1] * 2.5));
                                } else {
                                    if (gloveCharge > 7) {
                                        gloveCharge = 1;
                                        Globals.CHARACTER_TABLE[i].Write("AT", originalCharacterStats[i, 1]);
                                    }
                                }
                            }

                            gloveLastAction = Globals.CHARACTER_TABLE[i].Read("Action");
                        }

                        if (Globals.CHARACTER_TABLE[i].Read("Weapon") == 165 && Globals.PARTY_SLOT[i] == 7) { //Giant Axe
                            if (Globals.CHARACTER_TABLE[i].Read("Action") == 136 && axeLastAction != Globals.CHARACTER_TABLE[i].Read("Action")) {
                                axeLastAction = Globals.CHARACTER_TABLE[i].Read("Action");
                                if (new Random().Next(0, 9) < 2) {
                                    Globals.CHARACTER_TABLE[i].Write("Guard", 1);
                                }
                            } else {
                                axeLastAction = Globals.CHARACTER_TABLE[i].Read("Action");
                            }
                        }

                        if (Globals.CHARACTER_TABLE[i].Read("Helmet") == 167) { //Fake Legend Casque
                            if (Globals.CHARACTER_TABLE[i].Read("Guard") == 1 && guardStatusMDF[i] == 0) {
                                if (new Random().Next(0, 9) < 3) {
                                    Globals.CHARACTER_TABLE[i].Write("MDF", (Globals.CHARACTER_TABLE[i].Read("MDF") + 40));
                                    guardStatusMDF[i] = 1;
                                }
                            }
                            if ((Globals.CHARACTER_TABLE[i].Read("Action") == 8 || Globals.CHARACTER_TABLE[i].Read("Action") == 10) && guardStatusMDF[i] == 1) {
                                Globals.CHARACTER_TABLE[i].Write("MDF", originalCharacterStats[i, 4]);
                                guardStatusMDF[i] = 0;
                            }
                        }

                        if (Globals.CHARACTER_TABLE[i].Read("Armor") == 169) { //Fake Legend Armor
                            if (Globals.CHARACTER_TABLE[i].Read("Guard") == 1 && guardStatusDF[i] == 0) {
                                if (new Random().Next(0, 9) < 3) {
                                    Globals.CHARACTER_TABLE[i].Write("DF", (Globals.CHARACTER_TABLE[i].Read("DF") + 40));
                                    guardStatusDF[i] = 1;
                                }
                            }
                            if ((Globals.CHARACTER_TABLE[i].Read("Action") == 8 || Globals.CHARACTER_TABLE[i].Read("Action") == 10) && guardStatusDF[i] == 1) {
                                Globals.CHARACTER_TABLE[i].Write("DF", originalCharacterStats[i, 3]);
                                guardStatusDF[i] = 0;
                            }
                        }

                        if (Globals.CHARACTER_TABLE[i].Read("Armor") == 170) { //Divine DG Armor
                            Globals.CHARACTER_TABLE[i].Write("SP_P_Hit", originalCharacterStats[i, 6]);
                            Globals.CHARACTER_TABLE[i].Write("MP_P_Hit", originalCharacterStats[i, 7]);
                            Globals.CHARACTER_TABLE[i].Write("SP_M_Hit", originalCharacterStats[i, 8]);
                            Globals.CHARACTER_TABLE[i].Write("MP_M_Hit", originalCharacterStats[i, 9]);
                        }

                        if (Globals.CHARACTER_TABLE[i].Read("Shoes") == 172 || Globals.CHARACTER_TABLE[i].Read("Shoes") == 173 || Globals.CHARACTER_TABLE[i].Read("Shoes") == 174 || //Lloyd's Boots/ Winged Shoes / Soa's Greaves
                            Globals.CHARACTER_TABLE[i].Read("Shoes") == 99 || Globals.CHARACTER_TABLE[i].Read("Shoes") == 100 || Globals.CHARACTER_TABLE[i].Read("Shoes") == 101 || Globals.CHARACTER_TABLE[i].Read("Accessory") == 131 || Globals.CHARACTER_TABLE[i].Read("Accessory") == 133) {  //Magical Greaves/Dancer's Shoes/Bandit's Shoes/Dancer's Ring/Bandit's Ring
                            if (emulator.ReadByte(p + 0xC1) > 0) {
                                Globals.CHARACTER_TABLE[i].Write("SPD", (originalCharacterStats[i, 5] * 2));
                            } else {
                                if (emulator.ReadByte(p + 0xC3) > 0) {
                                    Globals.CHARACTER_TABLE[i].Write("SPD", (originalCharacterStats[i, 5] / 2));
                                } else {
                                    Globals.CHARACTER_TABLE[i].Write("SPD", (originalCharacterStats[i, 5]));
                                }
                            }

                            if (Globals.CHARACTER_TABLE[i].Read("Shoes") == 174) {
                                for (int x = 0; x < 3; x++) {
                                    if (x != i && Globals.PARTY_SLOT[i] < 9 && Globals.CHARACTER_TABLE[x].Read("HP") > 0) {
                                        if (Globals.CHARACTER_TABLE[x].Read("HP") > 0) {
                                            if (emulator.ReadByte(Globals.CHAR_ADDRESS[x] + 0xC1) > 0) {
                                                Globals.CHARACTER_TABLE[x].Write("SPD", (ushort) (originalCharacterStats[x, 5] * 2));
                                            } else {
                                                if (emulator.ReadByte(Globals.CHAR_ADDRESS[x] + 0xC3) > 0) {
                                                    Globals.CHARACTER_TABLE[x].Write("SPD", (ushort) (originalCharacterStats[x, 5] / 2));
                                                } else {
                                                    Globals.CHARACTER_TABLE[x].Write("SPD", (ushort) (originalCharacterStats[x, 5]));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (Globals.CHARACTER_TABLE[i].Read("Accessory") == 177) { //Soa's Ahnk
                            bool alive = false;
                            int kill = -1;
                            int lastPartyID = -1;
                            if (Globals.CHARACTER_TABLE[i].Read("HP") == 0) {
                                for (int x = 0; x < 3; x++) {
                                    if (x != i && Globals.PARTY_SLOT[i] < 9 && Globals.CHARACTER_TABLE[x].Read("HP") > 0) {
                                        alive = true;
                                    }
                                }

                                if (alive) {
                                    for (int x = 0; x < 3; x++) {
                                        if (kill == -1 && new Random().Next(0, 9) < 5 && Globals.CHARACTER_TABLE[x].Read("HP") > 0) {
                                            kill = x;
                                        } else {
                                            lastPartyID = x;
                                        }
                                    }
                                }

                                if (kill != -1) {
                                    Globals.CHARACTER_TABLE[kill].Write("HP", 0);
                                    Globals.CHARACTER_TABLE[kill].Write("Action", 192);
                                } else {
                                    Globals.CHARACTER_TABLE[lastPartyID].Write("HP", 0);
                                    Globals.CHARACTER_TABLE[lastPartyID].Write("Action", 192);
                                }
                                Globals.CHARACTER_TABLE[i].Write("HP", 1);
                            } else {
                                Globals.CHARACTER_TABLE[i].Write("MAX_HP", 0);
                                Globals.CHARACTER_TABLE[i].Write("Revive", 0);
                                Globals.CHARACTER_TABLE[i].Write("Action", 192);
                            }
                        }

                        if (Globals.CHARACTER_TABLE[i].Read("Armor") == 74) { //Armor of Legend
                            if (lGuardStateDF[i] == false) {
                                if (Globals.CHARACTER_TABLE[i].Read("Guard") == 1 && guardStatusDF[i] == 0) {
                                    guardStatusDF[i] = 1;
                                    lGuardStatusDF[i] += 1;
                                }
                                if ((Globals.CHARACTER_TABLE[i].Read("Action") == 8 || Globals.CHARACTER_TABLE[i].Read("Action") == 10) && guardStatusDF[i] == 1 && Globals.CHARACTER_TABLE[i].Read("Guard") == 0) {
                                    guardStatusDF[i] = 0;
                                }
                                if (lGuardStatusDF[i] >= 3) {
                                    lGuardStatusDF[i] = 0;
                                    guardStatusDF[i] = 1;
                                    lGuardStateDF[i] = true;
                                    emulator.WriteByte(p + 0xB5, 4);
                                    Globals.CHARACTER_TABLE[i].Write("DF", Math.Round(originalCharacterStats[i, 3] * 1.2));
                                }
                            } else {
                                if (emulator.ReadByte(p + 0xB5) == 0) {
                                    Globals.CHARACTER_TABLE[i].Write("DF", originalCharacterStats[i, 3]);
                                    lGuardStateDF[i] = false;
                                }
                            }
                            if (!sGuardStatusDF[i]) {
                                if (Globals.CHARACTER_TABLE[i].Read("Action") == 8 || Globals.CHARACTER_TABLE[i].Read("Action") == 10) {
                                    if (new Random().Next(0, 100) <= 10) {
                                        emulator.WriteByte(Constants.GetAddress("SPECIAL_EFFECT") + ((Globals.MONSTER_SIZE + i) * 0x20), emulator.ReadByte("SPECIAL_EFFECT", ((Globals.MONSTER_SIZE + i) * 0x20)) + 1);
                                    }
                                    sGuardStatusDF[i] = true;
                                }
                            } else {
                                if (Globals.CHARACTER_TABLE[i].Read("Action") != 8 && Globals.CHARACTER_TABLE[i].Read("Action") != 10) {
                                    sGuardStatusDF[i] = false;
                                }
                            }
                        }

                        if (Globals.CHARACTER_TABLE[i].Read("Helmet") == 89) { //Legend Casque
                            if (!lGuardStateMDF[i]) {
                                if (Globals.CHARACTER_TABLE[i].Read("Guard") == 1 && guardStatusMDF[i] == 0) {
                                    guardStatusMDF[i] = 1;
                                    lGuardStatusMDF[i] += 1;
                                }
                                if ((Globals.CHARACTER_TABLE[i].Read("Action") == 8 || Globals.CHARACTER_TABLE[i].Read("Action") == 10) && guardStatusMDF[i] == 1 && Globals.CHARACTER_TABLE[i].Read("Guard") == 0) {
                                    guardStatusMDF[i] = 0;
                                }
                                if (lGuardStatusMDF[i] >= 3) {
                                    lGuardStatusMDF[i] = 0;
                                    guardStatusMDF[i] = 1;
                                    lGuardStateMDF[i] = true;
                                    emulator.WriteByte(p + 0xB7, 4);
                                    Globals.CHARACTER_TABLE[i].Write("MDF", Math.Round(originalCharacterStats[i, 4] * 1.2));
                                }
                            } else {
                                if (emulator.ReadByte(p + 0xB7) == 0) {
                                    Globals.CHARACTER_TABLE[i].Write("MDF", originalCharacterStats[i, 4]);
                                    lGuardStateMDF[i] = false;
                                }
                            }
                            if (!sGuardStatusMDF[i]) {
                                if (Globals.CHARACTER_TABLE[i].Read("Action") == 8 || Globals.CHARACTER_TABLE[i].Read("Action") == 10) {
                                    if (new Random().Next(0, 100) <= 10) {
                                        emulator.WriteByte(Constants.GetAddress("SPECIAL_EFFECT") + ((Globals.MONSTER_SIZE + i) * 0x20), emulator.ReadByte("SPECIAL_EFFECT", ((Globals.MONSTER_SIZE + i) * 0x20)) + 4);
                                    }
                                    sGuardStatusMDF[i] = true;
                                }
                            } else {
                                if (Globals.CHARACTER_TABLE[i].Read("Action") != 8 && Globals.CHARACTER_TABLE[i].Read("Action") != 10) {
                                    sGuardStatusMDF[i] = false;
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #endregion

        #region Extras
        public static void WipeRewards(Emulator emulator) {
            for (int i = 0; i < Globals.UNIQUE_MONSTER_SIZE; i++) {
                emulator.WriteShort("MONSTER_REWARDS", 0, i * 0x1A8);
                emulator.WriteShort("MONSTER_REWARDS", 0, 0x2 + i * 0x1A8);
                emulator.WriteByte("MONSTER_REWARDS", 0, 0x4 + i * 0x1A8);
                emulator.WriteByte("MONSTER_REWARDS", 0, 0x5 + i * 0x1A8);
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
        public Emulator emulator = null;

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

        public MonsterAddress(long m_point, int monster, int ID, List<int> monsterUniqueIdList, Emulator emu) {
            emulator = emu;
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
                    return this.emulator.ReadShort(address[0]);
                } else {
                    return this.emulator.ReadByte(address[0]);
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
                    this.emulator.WriteShort(address[0], (ushort) Convert.ToInt32(value));
                } else {
                    this.emulator.WriteByte(address[0], Convert.ToByte(value));
                }
            } catch (Exception e) {
                Constants.WriteError(e);
                Constants.WriteError("Monster Write Error - A: " + attribute + " V: " + value);
                Console.WriteLine("Monster Write Error - A: " + attribute + " V: " + value);
            }
        }
    }

    public class CharAddress {
        long[] action = { 0, 1 };
        long[] menu = { 0, 1 };
        long[] lv = { 0, 1 };
        long[] dlv = { 0, 1 };
        long[] hp = { 0, 2 };
        long[] max_hp = { 0, 2 };
        long[] mp = { 0, 2 };
        long[] max_mp = { 0, 2 };
        long[] sp = { 0, 2 };
        long[] status = { 0, 1 };
        long[] element = { 0, 2 };
        long[] display_element = { 0, 2 };
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
        long[] dragoon_move = { 0, 1 };
        long[] a_hit = { 0, 1 };
        long[] m_hit = { 0, 1 };
        long[] a_av = { 0, 1 };
        long[] m_av = { 0, 1 };
        long[] p_immune = { 0, 1 };
        long[] m_immune = { 0, 1 };
        long[] p_half = { 0, 1 };
        long[] m_half = { 0, 1 };
        long[] e_immune = { 0, 1 };
        long[] e_half = { 0, 1 };
        long[] on_hit_status = { 0, 1 };
        long[] on_hit_status_chance = { 0, 1 };
        long[] stat_res = { 0, 1 };
        long[] death_res = { 0, 1 };
        long[] sp_p_hit = { 0, 1 };
        long[] sp_m_hit = { 0, 1 };
        long[] mp_p_hit = { 0, 1 };
        long[] mp_m_hit = { 0, 1 };
        long[] hp_regen = { 0, 2 };
        long[] mp_regen = { 0, 2 };
        long[] sp_regen = { 0, 2 };
        long[] sp_multi = { 0, 2 };
        long[] revive = { 0, 1 };
        long[] dat = { 0, 2 };
        long[] dmat = { 0, 2 };
        long[] ddf = { 0, 2 };
        long[] dmdf = { 0, 2 };
        long[] unique_index = { 0, 1 };
        long[] image = { 0, 1 };
        long[] special_effect = { 0, 1 };
        long[] guard = { 0, 1 };
        long[] dragoon = { 0, 1 };
        long[] spell_cast = { 0, 1 };
        long[] pwr_at = { 0, 1 };
        long[] pwr_at_trn = { 0, 1 };
        long[] pwr_mat = { 0, 1 };
        long[] pwr_mat_trn = { 0, 1 };
        long[] pwr_df = { 0, 1 };
        long[] pwr_df_trn = { 0, 1 };
        long[] pwr_mdf = { 0, 1 };
        long[] pwr_mdf_trn = { 0, 1 };
        long[] add_sp_multi = { 0, 2 };
        long[] add_dmg_multi = { 0, 2 };
        long[] weapon = { 0, 1 };
        long[] helmet = { 0, 1 };
        long[] armor = { 0, 1 };
        long[] shoes = { 0, 1 };
        long[] accessory = { 0, 1 };
        long[] pos_fb = { 0, 4 };
        long[] pos_ud = { 0, 4 };
        long[] pos_rl = { 0, 4 };
        long[] a_hit_inc = { 0, 1 };
        long[] a_hit_inc_trn = { 0, 1 };
        long[] m_hit_inc = { 0, 1 };
        long[] m_hit_inc_trn = { 0, 1 };
        long[] physical_immunity = { 0, 1 };
        long[] physical_immunity_trn = { 0, 1 };
        long[] elemental_immunity = { 0, 1 };
        long[] elemental_immunity_trn = { 0, 1 };
        long[] speed_up_trn = { 0, 1 };
        long[] speed_down_trn = { 0, 1 };
        long[] sp_onhit_physical = { 0, 1 };
        long[] sp_onhit_physical_trn = { 0, 1 };
        long[] mp_onhit_physical = { 0, 1 };
        long[] mp_onhit_physical_trn = { 0, 1 };
        long[] sp_onhit_magic = { 0, 1 };
        long[] sp_onhit_magic_trn = { 0, 1 };
        long[] mp_onhit_magic = { 0, 1 };
        long[] mp_onhit_magic_trn = { 0, 1 };
        long[] color_map = { 0, 1 };
        public Emulator emulator = null;

        public long[] Action { get { return action; } }
        public long[] Menu { get { return menu; } }
        public long[] LV { get { return lv; } }
        public long[] DLV { get { return dlv; } }
        public long[] HP { get { return hp; } }
        public long[] Max_HP { get { return max_hp; } }
        public long[] MP { get { return mp; } }
        public long[] Max_MP { get { return max_mp; } }
        public long[] SP { get { return sp; } }
        public long[] Status { get { return status; } }
        public long[] Element { get { return element; } }
        public long[] Display_Element { get { return display_element; } }
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
        public long[] Dragoon_Move { get { return dragoon_move; } }
        public long[] A_HIT { get { return a_hit; } }
        public long[] M_HIT { get { return m_hit; } }
        public long[] A_AV { get { return a_av; } }
        public long[] M_AV { get { return m_av; } }
        public long[] P_Immune { get { return p_immune; } }
        public long[] M_Immune { get { return m_immune; } }
        public long[] P_Half { get { return p_half; } }
        public long[] M_Half { get { return m_half; } }
        public long[] E_Immune { get { return e_immune; } }
        public long[] E_Half { get { return e_half; } }
        public long[] On_Hit_Status { get { return on_hit_status; } }
        public long[] On_Hit_Status_Chance { get { return on_hit_status_chance; } }
        public long[] Stat_Res { get { return stat_res; } }
        public long[] Death_Res { get { return death_res; } }
        public long[] SP_P_Hit { get { return sp_p_hit; } }
        public long[] SP_M_Hit { get { return sp_m_hit; } }
        public long[] MP_P_Hit { get { return mp_p_hit; } }
        public long[] MP_M_Hit { get { return mp_m_hit; } }
        public long[] HP_Regen { get { return hp_regen; } }
        public long[] MP_Regen { get { return mp_regen; } }
        public long[] SP_Regen { get { return sp_regen; } }
        public long[] SP_Multi { get { return sp_multi; } }
        public long[] Revive { get { return revive; } }
        public long[] Unique_Index { get { return unique_index; } }
        public long[] Image { get { return image; } }
        public long[] DAT { get { return dat; } }
        public long[] DMAT { get { return dmat; } }
        public long[] DDF { get { return ddf; } }
        public long[] DMDF { get { return dmdf; } }
        public long[] Special_Effect { get { return special_effect; } }
        public long[] Guard { get { return guard; } }
        public long[] Dragoon { get { return dragoon; } }
        public long[] Spell_Cast { get { return spell_cast; } }
        public long[] PWR_AT { get { return pwr_at; } }
        public long[] PWR_AT_TRN { get { return pwr_at_trn; } }
        public long[] PWR_MAT { get { return pwr_mat; } }
        public long[] PWR_MAT_TRN { get { return pwr_mat_trn; } }
        public long[] PWR_DF { get { return pwr_df; } }
        public long[] PWR_DF_TRN { get { return pwr_df_trn; } }
        public long[] PWR_MDF { get { return pwr_mdf; } }
        public long[] PWR_MDF_TRN { get { return pwr_mdf_trn; } }
        public long[] ADD_SP_Multi { get { return add_sp_multi; } }
        public long[] ADD_DMG_Multi { get { return add_dmg_multi; } }
        public long[] Weapon { get { return weapon; } }
        public long[] Helmet { get { return helmet; } }
        public long[] Armor { get { return armor; } }
        public long[] Shoes { get { return shoes; } }
        public long[] Accessory { get { return accessory; } }
        public long[] POS_FB { get { return pos_fb; } }
        public long[] POS_UD { get { return pos_ud; } }
        public long[] POS_RL { get { return pos_rl; } }
        public long[] A_HIT_INC { get { return a_hit_inc; } }
        public long[] A_HIT_INC_TRN { get { return a_hit_inc_trn; } }
        public long[] M_HIT_INC { get { return m_hit_inc; } }
        public long[] M_HIT_INC_TRN { get { return m_hit_inc_trn; } }
        public long[] PHYSICAL_IMMUNITY { get { return physical_immunity; } }
        public long[] PHYSICAL_IMMUNITY_TRN { get { return physical_immunity_trn; } }
        public long[] ELEMENTAL_IMMUNITY { get { return elemental_immunity; } }
        public long[] ELEMENTAL_IMMUNITY_TRN { get { return elemental_immunity_trn; } }
        public long[] SPEED_UP_TRN { get { return speed_up_trn; } }
        public long[] SPEED_DOWN_TRN { get { return speed_down_trn; } }
        public long[] SP_ONHIT_PHYSICAL { get { return sp_onhit_physical; } }
        public long[] SP_ONHIT_PHYSICAL_TRN { get { return sp_onhit_physical_trn; } }
        public long[] MP_ONHIT_PHYSICAL { get { return mp_onhit_physical; } }
        public long[] MP_ONHIT_PHYSICAL_TRN { get { return mp_onhit_physical_trn; } }
        public long[] SP_ONHIT_MAGIC { get { return sp_onhit_magic; } }
        public long[] SP_ONHIT_MAGIC_TRN { get { return sp_onhit_magic_trn; } }
        public long[] MP_ONHIT_MAGIC { get { return mp_onhit_magic; } }
        public long[] MP_ONHIT_MAGIC_TRN { get { return mp_onhit_magic_trn; } }
        public long[] Color_Map { get { return color_map; } }

        public CharAddress(long c_point, int character, Emulator emu) {
            emulator = emu;
            special_effect[0] = Constants.GetAddress("UNIQUE_MONSTER_SIZE") + (character + Globals.MONSTER_SIZE) * 0x20;
            action[0] = c_point - 0xA8 - character * 0x388;
            menu[0] = c_point - 0xA4 - character * 0x388;
            lv[0] = c_point - 0x04 - character * 0x388;
            dlv[0] = c_point - 0x02 - character * 0x388;
            hp[0] = c_point - character * 0x388;
            max_hp[0] = c_point + 0x8 - character * 0x388;
            mp[0] = c_point + 0x4 - character * 0x388;
            dragoon[0] = c_point + 0x7 - character * 0x388;
            max_mp[0] = c_point + 0xA - character * 0x388;
            sp[0] = c_point + 0x2 - character * 0x388;
            status[0] = c_point + 0x6 - character * 0x388;
            element[0] = c_point + 0x14 - character * 0x388;
            at[0] = c_point + 0x2C - character * 0x388;
            og_at[0] = c_point + 0x58 - character * 0x388;
            mat[0] = c_point + 0x2E - character * 0x388;
            og_mat[0] = c_point + 0x5A - character * 0x388;
            df[0] = c_point + 0x30 - character * 0x388;
            og_df[0] = c_point + 0x5E - character * 0x388;
            mdf[0] = c_point + 0x32 - character * 0x388;
            og_mdf[0] = c_point + 0x60 - character * 0x388;
            spd[0] = c_point + 0x2A - character * 0x388;
            og_spd[0] = c_point + 0x5C - character * 0x388;
            turn[0] = c_point + 0x44 - character * 0x388;
            dragoon_move[0] = c_point + 0x46 - character * 0x388;
            a_hit[0] = c_point + 0x34 - character * 0x388;
            m_hit[0] = c_point + 0x36 - character * 0x388;
            a_av[0] = c_point + 0x38 - character * 0x388;
            m_av[0] = c_point + 0x3A - character * 0x388;
            spell_cast[0] = c_point + 0x46 - character * 0x388;
            guard[0] = c_point + 0x4C - character * 0x388;
            display_element[0] = c_point + 0x6A - character * 0x388;
            dat[0] = c_point + 0xA4 - character * 0x388;
            dmat[0] = c_point + 0xA6 - character * 0x388;
            ddf[0] = c_point + 0xA8 - character * 0x388;
            dmdf[0] = c_point + 0xAA - character * 0x388;
            pwr_at[0] = c_point + 0xAC - character * 0x388;
            pwr_at_trn[0] = c_point + 0xAD - character * 0x388;
            pwr_mat[0] = c_point + 0xAE - character * 0x388;
            pwr_mat_trn[0] = c_point + 0xAF - character * 0x388;
            pwr_df[0] = c_point + 0xB0 - character * 0x388;
            pwr_df_trn[0] = c_point + 0xB1 - character * 0x388;
            pwr_mdf[0] = c_point + 0xB2 - character * 0x388;
            pwr_mdf_trn[0] = c_point + 0xB3 - character * 0x388;
            a_hit_inc[0] = c_point + 0xB4 - character * 0x388;
            a_hit_inc_trn[0] = c_point + 0xB5 - character * 0x388;
            m_hit_inc[0] = c_point + 0xB6 - character * 0x388;
            m_hit_inc_trn[0] = c_point + 0xB7 - character * 0x388;
            physical_immunity[0] = c_point + 0xBC - character * 0x388;
            physical_immunity_trn[0] = c_point + 0xBD - character * 0x388;
            elemental_immunity[0] = c_point + 0xBE - character * 0x388;
            elemental_immunity_trn[0] = c_point + 0xBF - character * 0x388;
            speed_up_trn[0] = c_point + 0xC1 - character * 0x388;
            speed_down_trn[0] = c_point + 0xC3 - character * 0x388;
            sp_onhit_physical[0] = c_point + 0xC4 - character * 0x388;
            sp_onhit_physical_trn[0] = c_point + 0xC5 - character * 0x388;
            mp_onhit_physical[0] = c_point + 0xC6 - character * 0x388;
            mp_onhit_physical_trn[0] = c_point + 0xC7 - character * 0x388;
            sp_onhit_magic[0] = c_point + 0xC8 - character * 0x388;
            sp_onhit_magic_trn[0] = c_point + 0xC9 - character * 0x388;
            mp_onhit_magic[0] = c_point + 0xCA - character * 0x388;
            mp_onhit_magic_trn[0] = c_point + 0xCB - character * 0x388;
            p_immune[0] = c_point + 0x108 - character * 0x388;
            m_immune[0] = c_point + 0x10A - character * 0x388;
            p_half[0] = c_point + 0x10C - character * 0x388;
            m_half[0] = c_point + 0x10E - character * 0x388;
            e_immune[0] = c_point + 0x1A - character * 0x388;
            e_half[0] = c_point + 0x18 - character * 0x388;
            on_hit_status[0] = c_point + 0x42 - character * 0x388;
            on_hit_status_chance[0] = c_point + 0x3C - character * 0x388;
            stat_res[0] = c_point + 0x1C - character * 0x388;
            death_res[0] = c_point + 0xC - character * 0x388;
            add_sp_multi[0] = c_point + 0x112 - character * 0x388;
            add_dmg_multi[0] = c_point + 0x114 - character * 0x388;
            weapon[0] = c_point + 0x116 - character * 0x388;
            helmet[0] = c_point + 0x118 - character * 0x388;
            armor[0] = c_point + 0x11A - character * 0x388;
            shoes[0] = c_point + 0x11C - character * 0x388;
            accessory[0] = c_point + 0x11E - character * 0x388;
            sp_multi[0] = c_point + 0x120 - character * 0x388;
            sp_p_hit[0] = c_point + 0x122 - character * 0x388;
            mp_p_hit[0] = c_point + 0x124 - character * 0x388;
            sp_m_hit[0] = c_point + 0x126 - character * 0x388;
            mp_m_hit[0] = c_point + 0x128 - character * 0x388;
            hp_regen[0] = c_point + 0x12C - character * 0x388;
            mp_regen[0] = c_point + 0x12E - character * 0x388;
            sp_regen[0] = c_point + 0x130 - character * 0x388;
            revive[0] = c_point + 0x132 - character * 0x388;
            pos_fb[0] = c_point + 0x16D - character * 0x388;
            pos_ud[0] = c_point + 0x171 - character * 0x388;
            pos_rl[0] = c_point + 0x175 - character * 0x388;
            unique_index[0] = c_point + 0x264 - character * 0x388;
            image[0] = c_point + 0x26A - character * 0x388;
            color_map[0] = c_point + 0x1DD - character * 0x388;
        }

        public object Read(string attribute) {
            try {
                PropertyInfo property = GetType().GetProperty(attribute);
                var address = (long[]) property.GetValue(this, null);
                if (address[1] == 4) {
                    return this.emulator.ReadInteger(address[0]);
                } else if (address[1] == 2) {
                    return this.emulator.ReadShort(address[0]);
                } else {
                    return this.emulator.ReadByte(address[0]);
                }
            } catch (Exception e) {
                Constants.WriteError("Character Read Error - A: " + attribute);
                Console.WriteLine("Character Read Error - A: " + attribute);
                return 0;
            }
        }

        public void Write(string attribute, object value) {
            try {
                PropertyInfo property = GetType().GetProperty(attribute);
                var address = (long[]) property.GetValue(this, null);
                if (address[1] == 4) {
                    this.emulator.WriteInteger(address[0], Convert.ToInt32(value));
                } else if (address[1] == 2) {
                    this.emulator.WriteShort(address[0], (ushort) Convert.ToInt32(value));
                } else {
                    this.emulator.WriteByte(address[0], Convert.ToByte(value));
                }
            } catch (Exception e) {
                Constants.WriteError("Character Write Error - A: " + attribute + " V: " + value);
                Console.WriteLine("Character Write Error - A: " + attribute + " V: " + value);
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

        public CurrentStats(int character, int slot, Emulator emulator) {
            lv = emulator.ReadByte("CHAR_TABLE", (character * 0x2C) + 0x12);
            if ((dragoon_spirits[character] & emulator.ReadByte("DRAGOON_SPIRITS")) > 0) {
                dlv = emulator.ReadByte("CHAR_TABLE", (character * 0x2C) + 0x13);
            }

            if (character == 0 && emulator.ReadByte("DRAGOON_SPIRITS") >= 254) {
                dlv = emulator.ReadByte("CHAR_TABLE", (character * 0x2C) + 0x13);
            }

            if (!Globals.ITEM_STAT_CHANGE) {
                weapon = Globals.DICTIONARY.OriginalItemList[emulator.ReadByte("CHAR_TABLE", character * 0x2C + 0x14)];
                armor = Globals.DICTIONARY.OriginalItemList[emulator.ReadByte("CHAR_TABLE", character * 0x2C + 0x15)];
                helm = Globals.DICTIONARY.OriginalItemList[emulator.ReadByte("CHAR_TABLE", character * 0x2C + 0x16)];
                boots = Globals.DICTIONARY.OriginalItemList[emulator.ReadByte("CHAR_TABLE", character * 0x2C + 0x17)];
                accessory = Globals.DICTIONARY.OriginalItemList[emulator.ReadByte("CHAR_TABLE", character * 0x2C + 0x18)];
            } else {
                weapon = Globals.DICTIONARY.ItemList[emulator.ReadByte("CHAR_TABLE", character * 0x2C + 0x14)];
                armor = Globals.DICTIONARY.ItemList[emulator.ReadByte("CHAR_TABLE", character * 0x2C + 0x15)];
                helm = Globals.DICTIONARY.ItemList[emulator.ReadByte("CHAR_TABLE", character * 0x2C + 0x16)];
                boots = Globals.DICTIONARY.ItemList[emulator.ReadByte("CHAR_TABLE", character * 0x2C + 0x17)];
                accessory = Globals.DICTIONARY.ItemList[emulator.ReadByte("CHAR_TABLE", character * 0x2C + 0x18)];
            }

            hp = emulator.ReadShort("CHAR_TABLE", character * 0x2C + 0x8);
            mp = emulator.ReadShort("CHAR_TABLE", character * 0x2C + 0xA);
            sp = emulator.ReadShort("CHAR_TABLE", character * 0x2C + 0xC);


            if (!Globals.CHARACTER_STAT_CHANGE) {
                max_hp = (ushort) (Globals.DICTIONARY.OriginalCharacterStats[character][lv].Max_HP * (1 + ((weapon.Special2 & 2) >> 1) * (float) (weapon.Special_Ammount) / 100 + ((armor.Special2 & 2) >> 1) * (float) (armor.Special_Ammount) / 100
                       + ((helm.Special2 & 2) >> 1) * (float) (helm.Special_Ammount) / 100 + ((boots.Special2 & 2) >> 1) * (float) (boots.Special_Ammount) / 100 + ((accessory.Special2 & 2) >> 1) * (float) (accessory.Special_Ammount) / 100));

                at = (ushort) (Globals.DICTIONARY.OriginalCharacterStats[character][lv].AT + weapon.AT + armor.AT + helm.AT + boots.AT + accessory.AT);
                mat = (ushort) (Globals.DICTIONARY.OriginalCharacterStats[character][lv].MAT + weapon.MAT + armor.MAT + helm.MAT + boots.MAT + accessory.MAT);
                df = (ushort) (Globals.DICTIONARY.OriginalCharacterStats[character][lv].DF + weapon.DF + armor.DF + helm.DF + boots.DF + accessory.DF);
                mdf = (ushort) (Globals.DICTIONARY.OriginalCharacterStats[character][lv].MDF + weapon.MDF + armor.MDF + helm.MDF + boots.MDF + accessory.MDF);
                spd = (ushort) (Globals.DICTIONARY.OriginalCharacterStats[character][lv].SPD + weapon.SPD + armor.SPD + helm.SPD + boots.SPD + accessory.SPD);
            } else {
                max_hp = (ushort) (Globals.DICTIONARY.CharacterStats[character][lv].Max_HP * (1 + ((weapon.Special2 & 2) >> 1) * (float) (weapon.Special_Ammount) / 100 + ((armor.Special2 & 2) >> 1) * (float) (armor.Special_Ammount) / 100
                       + ((helm.Special2 & 2) >> 1) * (float) (helm.Special_Ammount) / 100 + ((boots.Special2 & 2) >> 1) * (float) (boots.Special_Ammount) / 100 + ((accessory.Special2 & 2) >> 1) * (float) (accessory.Special_Ammount) / 100));

                at = (ushort) (Globals.DICTIONARY.CharacterStats[character][lv].AT + weapon.AT + armor.AT + helm.AT + boots.AT + accessory.AT);
                mat = (ushort) (Globals.DICTIONARY.CharacterStats[character][lv].MAT + weapon.MAT + armor.MAT + helm.MAT + boots.MAT + accessory.MAT);
                df = (ushort) (Globals.DICTIONARY.CharacterStats[character][lv].DF + weapon.DF + armor.DF + helm.DF + boots.DF + accessory.DF);
                mdf = (ushort) (Globals.DICTIONARY.CharacterStats[character][lv].MDF + weapon.MDF + armor.MDF + helm.MDF + boots.MDF + accessory.MDF);
                spd = (ushort) (Globals.DICTIONARY.CharacterStats[character][lv].SPD + weapon.SPD + armor.SPD + helm.SPD + boots.SPD + accessory.SPD);
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
        }
    }
    #endregion
}
