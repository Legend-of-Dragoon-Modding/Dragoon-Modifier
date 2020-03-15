using Dragoon_Modifier;
using System;
using System.Threading;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.CSharp;
using System.Globalization;
using System.Reflection;

public class BattleController {
    public static void Run(Emulator emulator) {
        int encounterValue = emulator.ReadShort(Constants.GetAddress("BATTLE_VALUE"));
        if (Globals.IN_BATTLE && !Globals.STATS_CHANGED && encounterValue == 41215) {
            Constants.WriteOutput("Battle detected. Loading...");
            Globals.UNIQUE_MONSTER_IDS = new List<int>();
            Globals.MONSTER_TABLE = new List<dynamic>();
            Globals.MONSTER_IDS = new List<int>();
            Thread.Sleep(2000);
            Globals.MONSTER_SIZE = emulator.ReadByte(Constants.GetAddress("MONSTER_SIZE"));
            Globals.UNIQUE_MONSTERS = emulator.ReadByte(Constants.GetAddress("UNIQUE_MONSTERS"));

            if (Constants.REGION == Region.USA) {
                Globals.SetM_POINT(0x1A439C + emulator.ReadShort(Constants.GetAddress("M_POINT")));
            } else {
                Globals.SetM_POINT(0x1A43B4 + emulator.ReadShort(Constants.GetAddress("M_POINT")));
            }
            Globals.SetC_POINT((int) (emulator.ReadInteger(Constants.GetAddress("C_POINT")) - 0x7F5A8558 - (uint) Constants.OFFSET));

            LoDDictInIt(emulator);
            Globals.STATS_CHANGED = true;

            Constants.WriteDebug("Monster Size:        " + Globals.MONSTER_SIZE);
            Constants.WriteDebug("Unique Monsters:     " + Globals.UNIQUE_MONSTERS);
            Constants.WriteDebug("Monster Point:       " + Convert.ToString(Globals.M_POINT + Constants.OFFSET, 16).ToUpper());
            Constants.WriteDebug("Character Point:     " + Convert.ToString(Globals.C_POINT + Constants.OFFSET, 16).ToUpper());
            Constants.WriteDebug("Monster IDs:         " + String.Join(", ", Globals.MONSTER_IDS.ToArray()));
            Constants.WriteDebug("Unique Monster IDs:  " + String.Join(", ", Globals.UNIQUE_MONSTER_IDS.ToArray()));
            Constants.WriteOutput("Finished loading.");
        } else {
            if (Globals.STATS_CHANGED && encounterValue < 9999) {
                Globals.STATS_CHANGED = false;
                Globals.IN_BATTLE = false;
                Globals.EXITING_BATTLE = 2;
                Constants.WriteOutput("Exiting out of battle.");
                if (Globals.ITEM_CHANGE == true) {
                    Constants.WriteOutput("Changing Item table...");
                    if (String.Join("", Globals.DICTIONARY.NameList).Replace(" ", "").Length / 2 < 6423) {
                        emulator.WriteAOB(Constants.GetAddress("ITEM_NAME"), String.Join(" ", Globals.DICTIONARY.NameList));
                    } else {
                        Constants.WriteDebug("Item name character limit exceded! " + Convert.ToString(String.Join("", Globals.DICTIONARY.NameList).Replace(" ", "").Length / 4, 10) + " / 3211 characters.");
                    }
                    if (String.Join("", Globals.DICTIONARY.DescriptionList).Replace(" ", "").Length / 2 < 13465) {
                        emulator.WriteAOB(Constants.GetAddress("ITEM_DESC"), String.Join(" ", Globals.DICTIONARY.DescriptionList));
                    } else {
                        Constants.WriteDebug("Item description character limit exceded! " + Convert.ToString(String.Join("", Globals.DICTIONARY.DescriptionList).Replace(" ", "").Length / 4, 10) + " / 6732 characters.");
                    }
                    int i = 0;
                    foreach (dynamic item in Globals.DICTIONARY.ItemList) {
                        if (i < 194) {
                            emulator.WriteByteU((long)(Constants.GetAddress("ITEM_TABLE") + i * 0x1C) + Constants.OFFSET, item.Type);
                            emulator.WriteByteU((long)(Constants.GetAddress("ITEM_TABLE") + i * 0x1C + 0x2) + Constants.OFFSET, item.Equips);
                            emulator.WriteByteU((long)(Constants.GetAddress("ITEM_TABLE") + i * 0x1C + 0x3) + Constants.OFFSET, item.Element);
                            emulator.WriteByteU((long)(Constants.GetAddress("ITEM_TABLE") + i * 0x1C + 0x1A) + Constants.OFFSET, item.Status);
                            emulator.WriteByteU((long)(Constants.GetAddress("ITEM_TABLE") + i * 0x1C + 0x17) + Constants.OFFSET, item.Status_Chance);
                            emulator.WriteByteU((long)(Constants.GetAddress("ITEM_TABLE") + i * 0x1C + 0x9) + Constants.OFFSET, item.AT);
                            emulator.WriteByteU((long)(Constants.GetAddress("ITEM_TABLE") + i * 0x1C + 0x10) + Constants.OFFSET, item.MAT);
                            emulator.WriteByteU((long)(Constants.GetAddress("ITEM_TABLE") + i * 0x1C + 0x11) + Constants.OFFSET, item.DF);
                            emulator.WriteByteU((long)(Constants.GetAddress("ITEM_TABLE") + i * 0x1C + 0x12) + Constants.OFFSET, item.MDF);
                            emulator.WriteByteU((long)(Constants.GetAddress("ITEM_TABLE") + i * 0x1C + 0xE) + Constants.OFFSET, item.SPD);
                            emulator.WriteByteU((long)(Constants.GetAddress("ITEM_TABLE") + i * 0x1C + 0x13) + Constants.OFFSET, item.A_Hit);
                            emulator.WriteByteU((long)(Constants.GetAddress("ITEM_TABLE") + i * 0x1C + 0x14) + Constants.OFFSET, item.M_Hit);
                            emulator.WriteByteU((long)(Constants.GetAddress("ITEM_TABLE") + i * 0x1C + 0x15) + Constants.OFFSET, item.A_AV);
                            emulator.WriteByteU((long)(Constants.GetAddress("ITEM_TABLE") + i * 0x1C + 0x16) + Constants.OFFSET, item.M_AV);
                            emulator.WriteByteU((long)(Constants.GetAddress("ITEM_TABLE") + i * 0x1C + 0x5) + Constants.OFFSET, item.E_Half);
                            emulator.WriteByteU((long)(Constants.GetAddress("ITEM_TABLE") + i * 0x1C + 0x6) + Constants.OFFSET, item.E_Immune);
                            emulator.WriteByteU((long)(Constants.GetAddress("ITEM_TABLE") + i * 0x1C + 0x7) + Constants.OFFSET, item.Stat_Res);
                            emulator.WriteByteU((long)(Constants.GetAddress("ITEM_TABLE") + i * 0x1C + 0xD) + Constants.OFFSET, item.Icon);
                        }
                        emulator.WriteInteger(Constants.GetAddress("ITEM_DESC_PTR") + i * 0x4, (int)item.DescriptionPointer);
                        emulator.WriteInteger(Constants.GetAddress("ITEM_NAME_PTR") + i * 0x4, (int)item.NamePointer);
                        i++;
                    }
                }
                if (Globals.DRAGOON_CHANGE == true) {
                    Constants.WriteOutput("Changing Dragoon Stat table...");
                    int[] charReorder = new int[] { 5, 7, 0, 4, 6, 8, 1, 3, 2 };
                    for (int character = 0; character < 8; character++) {
                        int reorderedChar = charReorder[character];
                        for (int level = 1; level < 6; level++) {
                            emulator.WriteShort((long)(Constants.GetAddress("DRAGOON_TABLE") + character * 0x30 + level * 0x8), Globals.DICTIONARY.DragoonStats[reorderedChar][level].MP);
                            emulator.WriteByte((long)(Constants.GetAddress("DRAGOON_TABLE") + character * 0x30 + level * 0x8 + 0x4), Globals.DICTIONARY.DragoonStats[reorderedChar][level].DAT);
                            emulator.WriteByte((long)(Constants.GetAddress("DRAGOON_TABLE") + character * 0x30 + level * 0x8 + 0x5), Globals.DICTIONARY.DragoonStats[reorderedChar][level].DMAT);
                            emulator.WriteByte((long)(Constants.GetAddress("DRAGOON_TABLE") + character * 0x30 + level * 0x8 + 0x6), Globals.DICTIONARY.DragoonStats[reorderedChar][level].DDF);
                            emulator.WriteByte((long)(Constants.GetAddress("DRAGOON_TABLE") + character * 0x30 + level * 0x8 + 0x7), Globals.DICTIONARY.DragoonStats[reorderedChar][level].DMDF);
                        }
                    }
                }
                if (Globals.CHARACTER_CHANGE == true) {
                    Constants.WriteOutput("Changing Character Stat table...");
                    int[] charReorder = new int[] {7, 0, 4, 6, 1, 3, 2 };
                    for (int character = 0; character < 7; character++) {
                        int reorderedChar = charReorder[character];
                        for (int level = 0; level < 61; level++) {
                            if (level > 0) {
                                emulator.WriteShortU(Constants.GetAddress("CHAR_STAT_TABLE") + Constants.OFFSET + level * 8 + character * 0x1E8, (ushort)(Globals.DICTIONARY.CharacterStats[reorderedChar][level].Max_HP));
                                emulator.WriteByte(Constants.GetAddress("CHAR_STAT_TABLE") + level * 8 + character * 0x1E8 + 0x3, Globals.DICTIONARY.CharacterStats[reorderedChar][level].SPD);
                                emulator.WriteByte(Constants.GetAddress("CHAR_STAT_TABLE") + level * 8 + character * 0x1E8 + 0x4, Globals.DICTIONARY.CharacterStats[reorderedChar][level].AT);
                                emulator.WriteByte(Constants.GetAddress("CHAR_STAT_TABLE") + level * 8 + character * 0x1E8 + 0x5, Globals.DICTIONARY.CharacterStats[reorderedChar][level].MAT);
                                emulator.WriteByte(Constants.GetAddress("CHAR_STAT_TABLE") + level * 8 + character * 0x1E8 + 0x6, Globals.DICTIONARY.CharacterStats[reorderedChar][level].DF);
                                emulator.WriteByte(Constants.GetAddress("CHAR_STAT_TABLE") + level * 8 + character * 0x1E8 + 0x7, Globals.DICTIONARY.CharacterStats[reorderedChar][level].MDF);
                            }
                        }
                    }
                }
                if (Globals.PARTY_SLOT[0] != 0) {
                    emulator.WriteByteU(Constants.GetAddress("PARTY_SLOT") + Constants.OFFSET, 0);
                }
            }
        }
    }
    public static int GetOffset() {
        int[] discOffset = { 0xD80, 0x0, 0x1458, 0x1B0 };
        int[] charOffset = { 0x0, 0x180, -0x180, 0x420, 0x540, 0x180, 0x350, 0x2F0, -0x180 };
        int partyOffset = 0;
        if (Globals.PARTY_SLOT[0] < 9 && Globals.PARTY_SLOT[1] < 9 && Globals.PARTY_SLOT[2] < 9) {
            partyOffset = charOffset[Globals.PARTY_SLOT[1]] + charOffset[Globals.PARTY_SLOT[2]];
        }
        return discOffset[Globals.DISC - 1] - partyOffset;
    }

    public static void LoDDictInIt(Emulator emulator) {
        Globals.UNIQUE_MONSTER_IDS = new List<int>();
        Globals.MONSTER_IDS = new List<int>();
        Globals.MONSTER_TABLE = new List<dynamic>();
        Globals.CHARACTER_TABLE = new List<dynamic>();

        for (int monster = 0; monster < Globals.UNIQUE_MONSTERS; monster++) {
            Globals.UNIQUE_MONSTER_IDS.Add(emulator.ReadShortU(Constants.GetAddress("UNIQUE_SLOT") + (int) Constants.OFFSET + (monster * 0x1A8)));
        }
        for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
            Globals.MONSTER_IDS.Add(emulator.ReadShort(Constants.GetAddress("MONSTER_ID") + GetOffset() + (i * 0x8)));
        }
        for (int monster = 0; monster < Globals.MONSTER_SIZE; monster++) {
            Globals.MONSTER_TABLE.Add(new MonsterAddress(Globals.M_POINT + Constants.OFFSET, monster, Globals.MONSTER_IDS[monster], Globals.UNIQUE_MONSTER_IDS, emulator));
        }
        for (int character = 0; character < 3; character++) {
            if (Globals.PARTY_SLOT[character] < 9) {
                Globals.CHARACTER_TABLE.Add(new CharAddress(Globals.C_POINT + Constants.OFFSET, character, emulator));
            }
        }
        if ((Globals.ITEM_CHANGE | Globals.CHARACTER_CHANGE == true) || (Globals.NO_DART != null)) {
            Constants.WriteOutput("Changing Character/Item stats...");
            for (int character = 0; character < 9; character++) {
                Globals.CURRENT_STATS[character] = new CurrentStats(character, emulator);
            }
            for (int character = 0; character < 3; character++) {
                int charid = 0;
                if (Globals.NO_DART != null & character == 0) {
                    charid = (int)Globals.NO_DART;
                } else {
                    charid = Globals.PARTY_SLOT[character];
                }
                if (charid < 9) {
                    Globals.CHARACTER_TABLE[character].Write("HP", Globals.CURRENT_STATS[charid].HP);
                    Globals.CHARACTER_TABLE[character].Write("Max_HP", Globals.CURRENT_STATS[charid].Max_HP);
                    Globals.CHARACTER_TABLE[character].Write("AT", Globals.CURRENT_STATS[charid].AT);
                    Globals.CHARACTER_TABLE[character].Write("OG_AT", Globals.CURRENT_STATS[charid].AT);
                    Globals.CHARACTER_TABLE[character].Write("MAT", Globals.CURRENT_STATS[charid].MAT);
                    Globals.CHARACTER_TABLE[character].Write("OG_MAT", Globals.CURRENT_STATS[charid].MAT);
                    Globals.CHARACTER_TABLE[character].Write("DF", Globals.CURRENT_STATS[charid].DF);
                    Globals.CHARACTER_TABLE[character].Write("OG_DF", Globals.CURRENT_STATS[charid].DF);
                    Globals.CHARACTER_TABLE[character].Write("MDF", Globals.CURRENT_STATS[charid].MDF);
                    Globals.CHARACTER_TABLE[character].Write("OG_MDF", Globals.CURRENT_STATS[charid].MDF);
                    Globals.CHARACTER_TABLE[character].Write("SPD", Globals.CURRENT_STATS[charid].SPD);
                    Globals.CHARACTER_TABLE[character].Write("OG_SPD", Globals.CURRENT_STATS[charid].SPD);
                    if ((Globals.ITEM_CHANGE == true) || (Globals.NO_DART != null)) {
                        Globals.CHARACTER_TABLE[character].Write("MP", Globals.CURRENT_STATS[charid].MP);
                        Globals.CHARACTER_TABLE[character].Write("Max_MP", Globals.CURRENT_STATS[charid].Max_MP);
                        Globals.CHARACTER_TABLE[character].Write("Stat_Res", Globals.CURRENT_STATS[charid].Stat_Res);
                        Globals.CHARACTER_TABLE[character].Write("E_Half", Globals.CURRENT_STATS[charid].E_Half);
                        Globals.CHARACTER_TABLE[character].Write("E_Immune", Globals.CURRENT_STATS[charid].E_Immune);
                        Globals.CHARACTER_TABLE[character].Write("A_AV", Globals.CURRENT_STATS[charid].A_AV);
                        Globals.CHARACTER_TABLE[character].Write("M_AV", Globals.CURRENT_STATS[charid].M_AV);
                        Globals.CHARACTER_TABLE[character].Write("A_HIT", Globals.CURRENT_STATS[charid].A_Hit);
                        Globals.CHARACTER_TABLE[character].Write("M_HIT", Globals.CURRENT_STATS[charid].M_Hit);
                        Globals.CHARACTER_TABLE[character].Write("P_Half", Globals.CURRENT_STATS[charid].P_Half);
                        Globals.CHARACTER_TABLE[character].Write("M_Half", Globals.CURRENT_STATS[charid].M_Half);
                        Globals.CHARACTER_TABLE[character].Write("On_Hit_Status", Globals.CURRENT_STATS[charid].Status);
                        Globals.CHARACTER_TABLE[character].Write("On_Hit_Status_Chance", Globals.CURRENT_STATS[charid].Status_Chance);
                        Globals.CHARACTER_TABLE[character].Write("Revive", Globals.CURRENT_STATS[charid].Revive);
                        Globals.CHARACTER_TABLE[character].Write("SP_Regen", Globals.CURRENT_STATS[charid].SP_Regen);
                        Globals.CHARACTER_TABLE[character].Write("MP_Regen", Globals.CURRENT_STATS[charid].MP_Regen);
                        Globals.CHARACTER_TABLE[character].Write("HP_Regen", Globals.CURRENT_STATS[charid].HP_Regen);
                        Globals.CHARACTER_TABLE[character].Write("Display_Element", Globals.CURRENT_STATS[charid].Element);
                        Globals.CHARACTER_TABLE[character].Write("MP_M_Hit", Globals.CURRENT_STATS[charid].MP_M_Hit);
                        Globals.CHARACTER_TABLE[character].Write("SP_M_Hit", Globals.CURRENT_STATS[charid].SP_M_Hit);
                        Globals.CHARACTER_TABLE[character].Write("MP_P_Hit", Globals.CURRENT_STATS[charid].MP_P_Hit);
                        Globals.CHARACTER_TABLE[character].Write("SP_P_Hit", Globals.CURRENT_STATS[charid].SP_P_Hit);
                        Globals.CHARACTER_TABLE[character].Write("SP_Multi", Globals.CURRENT_STATS[charid].SP_Multi);
                    }
                }
            }
        }
        if (Globals.MONSTER_CHANGE == true) {
            Constants.WriteOutput("Changing Monster stats...");
            if (Globals.ULTIMATE == false) {
                for (int monster = 0; monster < Globals.MONSTER_SIZE; monster++) {
                    int ID = Globals.MONSTER_IDS[monster];
                    Globals.MONSTER_TABLE[monster].Write("HP", Globals.DICTIONARY.StatList[ID].HP);
                    Globals.MONSTER_TABLE[monster].Write("Max_HP", Globals.DICTIONARY.StatList[ID].HP);
                    Globals.MONSTER_TABLE[monster].Write("AT", Globals.DICTIONARY.StatList[ID].AT);
                    Globals.MONSTER_TABLE[monster].Write("OG_AT", Globals.DICTIONARY.StatList[ID].AT);
                    Globals.MONSTER_TABLE[monster].Write("MAT", Globals.DICTIONARY.StatList[ID].MAT);
                    Globals.MONSTER_TABLE[monster].Write("OG_MAT", Globals.DICTIONARY.StatList[ID].MAT);
                    Globals.MONSTER_TABLE[monster].Write("DF", Globals.DICTIONARY.StatList[ID].DF);
                    Globals.MONSTER_TABLE[monster].Write("OG_DF", Globals.DICTIONARY.StatList[ID].DF);
                    Globals.MONSTER_TABLE[monster].Write("MDF", Globals.DICTIONARY.StatList[ID].MDF);
                    Globals.MONSTER_TABLE[monster].Write("OG_MDF", Globals.DICTIONARY.StatList[ID].MDF);
                    Globals.MONSTER_TABLE[monster].Write("SPD", Globals.DICTIONARY.StatList[ID].SPD);
                    Globals.MONSTER_TABLE[monster].Write("OG_SPD", Globals.DICTIONARY.StatList[ID].SPD);
                    Globals.MONSTER_TABLE[monster].Write("A_AV", Globals.DICTIONARY.StatList[ID].A_AV);
                    Globals.MONSTER_TABLE[monster].Write("M_AV", Globals.DICTIONARY.StatList[ID].M_AV);
                    Globals.MONSTER_TABLE[monster].Write("P_Immune", Globals.DICTIONARY.StatList[ID].P_Immune);
                    Globals.MONSTER_TABLE[monster].Write("M_Immune", Globals.DICTIONARY.StatList[ID].M_Immune);
                    Globals.MONSTER_TABLE[monster].Write("P_Half", Globals.DICTIONARY.StatList[ID].P_Half);
                    Globals.MONSTER_TABLE[monster].Write("M_Half", Globals.DICTIONARY.StatList[ID].M_Half);
                    Globals.MONSTER_TABLE[monster].Write("E_Immune", Globals.DICTIONARY.StatList[ID].E_Immune);
                    Globals.MONSTER_TABLE[monster].Write("E_Half", Globals.DICTIONARY.StatList[ID].E_Half);
                    Globals.MONSTER_TABLE[monster].Write("Stat_Res", Globals.DICTIONARY.StatList[ID].Stat_Res);
                    Globals.MONSTER_TABLE[monster].Write("Death_Res", Globals.DICTIONARY.StatList[ID].Death_Res);
                }
            } else {
                for (int monster = 0; monster < Globals.MONSTER_SIZE; monster++) {
                    int ID = Globals.MONSTER_IDS[monster];
                    Globals.MONSTER_TABLE[monster].Write("HP", Globals.DICTIONARY.UltimateStatList[ID].HP);
                    Globals.MONSTER_TABLE[monster].Write("Max_HP", Globals.DICTIONARY.UltimateStatList[ID].HP);
                    Globals.MONSTER_TABLE[monster].Write("ATK", Globals.DICTIONARY.UltimateStatList[ID].ATK);
                    Globals.MONSTER_TABLE[monster].Write("OG_ATK", Globals.DICTIONARY.UltimateStatList[ID].ATK);
                    Globals.MONSTER_TABLE[monster].Write("MAT", Globals.DICTIONARY.UltimateStatList[ID].MAT);
                    Globals.MONSTER_TABLE[monster].Write("OG_MAT", Globals.DICTIONARY.UltimateStatList[ID].MAT);
                    Globals.MONSTER_TABLE[monster].Write("DEF", Globals.DICTIONARY.UltimateStatList[ID].DEF);
                    Globals.MONSTER_TABLE[monster].Write("OG_DEF", Globals.DICTIONARY.UltimateStatList[ID].DEF);
                    Globals.MONSTER_TABLE[monster].Write("MDEF", Globals.DICTIONARY.UltimateStatList[ID].MDEF);
                    Globals.MONSTER_TABLE[monster].Write("OG_MDEF", Globals.DICTIONARY.UltimateStatList[ID].MDEF);
                    Globals.MONSTER_TABLE[monster].Write("SPD", Globals.DICTIONARY.UltimateStatList[ID].SPD);
                    Globals.MONSTER_TABLE[monster].Write("OG_SPD", Globals.DICTIONARY.UltimateStatList[ID].SPD);
                    Globals.MONSTER_TABLE[monster].Write("A_AV", Globals.DICTIONARY.UltimateStatList[ID].A_AV);
                    Globals.MONSTER_TABLE[monster].Write("M_AV", Globals.DICTIONARY.UltimateStatList[ID].M_AV);
                    Globals.MONSTER_TABLE[monster].Write("P_Immune", Globals.DICTIONARY.UltimateStatList[ID].P_Immune);
                    Globals.MONSTER_TABLE[monster].Write("M_Immune", Globals.DICTIONARY.UltimateStatList[ID].M_Immune);
                    Globals.MONSTER_TABLE[monster].Write("P_Half", Globals.DICTIONARY.UltimateStatList[ID].P_Half);
                    Globals.MONSTER_TABLE[monster].Write("M_Half", Globals.DICTIONARY.UltimateStatList[ID].M_Half);
                    Globals.MONSTER_TABLE[monster].Write("E_Immune", Globals.DICTIONARY.UltimateStatList[ID].E_Immune);
                    Globals.MONSTER_TABLE[monster].Write("E_Half", Globals.DICTIONARY.UltimateStatList[ID].E_Half);
                    Globals.MONSTER_TABLE[monster].Write("Stat_Res", Globals.DICTIONARY.UltimateStatList[ID].Stat_Res);
                    Globals.MONSTER_TABLE[monster].Write("Death_Res", Globals.DICTIONARY.UltimateStatList[ID].Death_Res);
                }
            }
        }
        if (Globals.DROP_CHANGE == true) {
            Constants.WriteOutput("Changing drops...");
            if (Globals.ULTIMATE == false) {
                for (int monster = 0; monster < Globals.UNIQUE_MONSTERS; monster++) {
                    int ID = Globals.UNIQUE_MONSTER_IDS[monster];
                    emulator.WriteShortU(Constants.GetAddress("MONSTER_REWARDS") + Constants.OFFSET + monster * 0x1A8, (ushort)Globals.DICTIONARY.StatList[ID].EXP);
                    emulator.WriteShortU(Constants.GetAddress("MONSTER_REWARDS") + Constants.OFFSET + 0x2 + monster * 0x1A8, (ushort)Globals.DICTIONARY.StatList[ID].Gold);
                    emulator.WriteByteU(Constants.GetAddress("MONSTER_REWARDS") + Constants.OFFSET + 0x4 + monster * 0x1A8, (byte)Globals.DICTIONARY.StatList[ID].Drop_Chance);
                    emulator.WriteByteU(Constants.GetAddress("MONSTER_REWARDS") + Constants.OFFSET + 0x5 + monster * 0x1A8, (byte)Globals.DICTIONARY.StatList[ID].Drop_Item);
                }
            } else {
                for (int monster = 0; monster < Globals.UNIQUE_MONSTERS; monster++) {
                    int ID = Globals.UNIQUE_MONSTER_IDS[monster];
                    emulator.WriteShortU(Constants.GetAddress("MONSTER_REWARDS") + Constants.OFFSET + monster * 0x1A8, (ushort)Globals.DICTIONARY.UltimateStatList[ID].EXP);
                    emulator.WriteShortU(Constants.GetAddress("MONSTER_REWARDS") + Constants.OFFSET + 0x2 + monster * 0x1A8, (ushort)Globals.DICTIONARY.UltimateStatList[ID].Gold);
                    emulator.WriteByteU(Constants.GetAddress("MONSTER_REWARDS") + Constants.OFFSET + 0x4 + monster * 0x1A8, (byte)Globals.DICTIONARY.UltimateStatList[ID].Drop_Chance);
                    emulator.WriteByteU(Constants.GetAddress("MONSTER_REWARDS") + Constants.OFFSET + 0x5 + monster * 0x1A8, (byte)Globals.DICTIONARY.UltimateStatList[ID].Drop_Item);
                }
            }
        }
        if (Globals.DRAGOON_CHANGE == true) {
            Constants.WriteOutput("Changing dragoon stats and magic...");
            for (int character = 0; character < 3; character++) {
                int slot = Globals.PARTY_SLOT[character];
                if (Globals.NO_DART != null) {
                    slot = (int)Globals.NO_DART;
                }
                if (slot < 9) {
                    int dlv = Globals.CHARACTER_TABLE[character].Read("DLV");
                    if (dlv != 0) {
                        Globals.CHARACTER_TABLE[character].Write("DAT", Globals.DICTIONARY.DragoonStats[Globals.PARTY_SLOT[character]][dlv].DAT);
                        Globals.CHARACTER_TABLE[character].Write("DMAT", Globals.DICTIONARY.DragoonStats[Globals.PARTY_SLOT[character]][dlv].DMAT);
                        Globals.CHARACTER_TABLE[character].Write("DDF", Globals.DICTIONARY.DragoonStats[Globals.PARTY_SLOT[character]][dlv].DDF);
                        Globals.CHARACTER_TABLE[character].Write("DMDF", Globals.DICTIONARY.DragoonStats[Globals.PARTY_SLOT[character]][dlv].DMDF);
                        if (Globals.ITEM_CHANGE == false) {
                            Globals.CHARACTER_TABLE[character].Write("MP", emulator.ReadShortU(Constants.GetAddress("CHAR_TABLE") + Constants.OFFSET + slot * 0x2C + 0xA));
                            float tableMP = (float)Globals.DICTIONARY.DragoonStats[slot][dlv].MP;
                            Globals.CHARACTER_TABLE[character].Write("Max_MP", (ushort)(tableMP * ((float)(Globals.CHARACTER_TABLE[character].Read("Max_MP")) / tableMP)));
                        }
                    }     
                }
            }
            int i = 0;
            foreach (dynamic Spell in Globals.DRAGOON_SPELLS) {
                int intValue = (int)emulator.ReadByteU(Constants.GetAddress("SPELL_TABLE") + 0x2 + (int)Constants.OFFSET + i * 0xC);
                if (Spell.Percentage == true) {
                    intValue |= 1 << 2; 
                } else {
                    intValue &= ~(1 << 2);
                }
                emulator.WriteByteU(Constants.GetAddress("SPELL_TABLE") + 0x2 + Constants.OFFSET + i * 0xC, (byte)intValue);
                emulator.WriteByteU(Constants.GetAddress("SPELL_TABLE") + 0x4 + Constants.OFFSET + i * 0xC, Spell.DMG_Base);
                emulator.WriteByteU(Constants.GetAddress("SPELL_TABLE") + 0x5 + Constants.OFFSET + i * 0xC, Spell.Multi);
                emulator.WriteByteU(Constants.GetAddress("SPELL_TABLE") + 0x6 + Constants.OFFSET + i * 0xC, Spell.Accuracy);
                emulator.WriteByteU(Constants.GetAddress("SPELL_TABLE") + 0x7 + Constants.OFFSET + i * 0xC, Spell.MP);
                emulator.WriteByteU(Constants.GetAddress("SPELL_TABLE") + 0x9 + Constants.OFFSET + i * 0xC, Spell.Element);
                i++;
            }
            for (int z = 0; z < 3; z++) { // Miranda Hotfix
                int intValue = (int)emulator.ReadByteU(Constants.GetAddress("SPELL_TABLE") + 0x2 + (int)Constants.OFFSET + (z + 65) * 0xC);
                if (Globals.DRAGOON_SPELLS[z + 10].Percentage == true) {
                    intValue |= 1 << 2;
                } else {
                    intValue &= ~(1 << 2);
                }
                emulator.WriteByteU(Constants.GetAddress("SPELL_TABLE") + 0x2 + Constants.OFFSET + (z + 65) * 0xC, (byte)intValue);
                emulator.WriteByteU(Constants.GetAddress("SPELL_TABLE") + 0x4 + Constants.OFFSET + (z + 65) * 0xC, Globals.DRAGOON_SPELLS[z + 10].DMG_Base);
                emulator.WriteByteU(Constants.GetAddress("SPELL_TABLE") + 0x5 + Constants.OFFSET + (z + 65) * 0xC, Globals.DRAGOON_SPELLS[z + 10].Multi);
                emulator.WriteByteU(Constants.GetAddress("SPELL_TABLE") + 0x6 + Constants.OFFSET + (z + 65) * 0xC, Globals.DRAGOON_SPELLS[z + 10].Accuracy);
                emulator.WriteByteU(Constants.GetAddress("SPELL_TABLE") + 0x7 + Constants.OFFSET + (z + 65) * 0xC, Globals.DRAGOON_SPELLS[z + 10].MP);
                emulator.WriteByteU(Constants.GetAddress("SPELL_TABLE") + 0x9 + Constants.OFFSET + (z + 65) * 0xC, Globals.DRAGOON_SPELLS[z + 10].Element);
            }
        }
        if (Globals.ADDITION_CHANGE == true) {
            Dictionary<int, int> additionnum = new Dictionary<int, int> {
                {0, 0 },
                {1, 2 },
                {2, 2 },
                {3, 3 },
                {4, 4 },
                {5, 5 },
                {6, 6 },
                {8, 0 },
                {9, 1 },
                {10, 2 },
                {11, 3 },
                {12, 4 },
                {14, 0 },
                {15, 1 },
                {16, 2 },
                {17, 3 },
                {29, 0 },
                {30, 1 },
                {31, 2 },
                {32, 3 },
                {33, 4 },
                {34, 5 },
                {23, 0 },
                {24, 1 },
                {25, 2 },
                {26, 3 },
                {27, 4 },
                {19, 0 },
                {20, 1 },
                {21, 2 },
                {255, 0 }
            };
            for (int character = 0; character < 3; character++) {
                int slot = Globals.PARTY_SLOT[character];
                if (Globals.NO_DART != null) {
                    slot = (int)Globals.NO_DART;
                }
                if (slot < 9) {
                    int addition = emulator.ReadByteU(Constants.GetAddress("CHAR_TABLE") + Constants.OFFSET + (slot * 0x2C) + 0x19);
                    for (int hit = 0; hit < 8; hit++) {
                        emulator.WriteShort(Constants.GetAddress("ADDITION") + GetOffset() + character * 0x100 + hit * 0x20, (ushort)Globals.DICTIONARY.AdditionData[slot, additionnum[addition], hit].UU1);
                        emulator.WriteShort(Constants.GetAddress("ADDITION") + GetOffset() + character * 0x100 + hit * 0x20 + 0x2, (ushort)Globals.DICTIONARY.AdditionData[slot, additionnum[addition], hit].Next_Hit);
                        emulator.WriteShort(Constants.GetAddress("ADDITION") + GetOffset() + character * 0x100 + hit * 0x20 + 0x4, (ushort)Globals.DICTIONARY.AdditionData[slot, additionnum[addition], hit].Blue_Time);
                        emulator.WriteShort(Constants.GetAddress("ADDITION") + GetOffset() + character * 0x100 + hit * 0x20 + 0x6, (ushort)Globals.DICTIONARY.AdditionData[slot, additionnum[addition], hit].Gray_Time);
                        emulator.WriteShort(Constants.GetAddress("ADDITION") + GetOffset() + character * 0x100 + hit * 0x20 + 0x8, (ushort)Globals.DICTIONARY.AdditionData[slot, additionnum[addition], hit].DMG);
                        emulator.WriteShort(Constants.GetAddress("ADDITION") + GetOffset() + character * 0x100 + hit * 0x20 + 0xA, (ushort)Globals.DICTIONARY.AdditionData[slot, additionnum[addition], hit].SP);
                        emulator.WriteShort(Constants.GetAddress("ADDITION") + GetOffset() + character * 0x100 + hit * 0x20 + 0xC, (ushort)Globals.DICTIONARY.AdditionData[slot, additionnum[addition], hit].ID);
                        emulator.WriteByte(Constants.GetAddress("ADDITION") + GetOffset() + character * 0x100 + hit * 0x20 + 0xE, Globals.DICTIONARY.AdditionData[slot, additionnum[addition], hit].Final_Hit);
                        emulator.WriteByte(Constants.GetAddress("ADDITION") + GetOffset() + character * 0x100 + hit * 0x20 + 0xF, Globals.DICTIONARY.AdditionData[slot, additionnum[addition], hit].UU2);
                        emulator.WriteByte(Constants.GetAddress("ADDITION") + GetOffset() + character * 0x100 + hit * 0x20 + 0x10, Globals.DICTIONARY.AdditionData[slot, additionnum[addition], hit].UU3);
                        emulator.WriteByte(Constants.GetAddress("ADDITION") + GetOffset() + character * 0x100 + hit * 0x20 + 0x11, Globals.DICTIONARY.AdditionData[slot, additionnum[addition], hit].UU4);
                        emulator.WriteByte(Constants.GetAddress("ADDITION") + GetOffset() + character * 0x100 + hit * 0x20 + 0x12, Globals.DICTIONARY.AdditionData[slot, additionnum[addition], hit].UU5);
                        emulator.WriteByte(Constants.GetAddress("ADDITION") + GetOffset() + character * 0x100 + hit * 0x20 + 0x13, Globals.DICTIONARY.AdditionData[slot, additionnum[addition], hit].UU6);
                        emulator.WriteByte(Constants.GetAddress("ADDITION") + GetOffset() + character * 0x100 + hit * 0x20 + 0x14, Globals.DICTIONARY.AdditionData[slot, additionnum[addition], hit].UU7);
                        emulator.WriteByte(Constants.GetAddress("ADDITION") + GetOffset() + character * 0x100 + hit * 0x20 + 0x15, Globals.DICTIONARY.AdditionData[slot, additionnum[addition], hit].UU8);
                        emulator.WriteByte(Constants.GetAddress("ADDITION") + GetOffset() + character * 0x100 + hit * 0x20 + 0x16, Globals.DICTIONARY.AdditionData[slot, additionnum[addition], hit].UU9);
                        emulator.WriteByte(Constants.GetAddress("ADDITION") + GetOffset() + character * 0x100 + hit * 0x20 + 0x17, Globals.DICTIONARY.AdditionData[slot, additionnum[addition], hit].UU10);
                        emulator.WriteShort(Constants.GetAddress("ADDITION") + GetOffset() + character * 0x100 + hit * 0x20 + 0x18, (ushort)Globals.DICTIONARY.AdditionData[slot, additionnum[addition], hit].Vertical_Distance);
                        emulator.WriteByte(Constants.GetAddress("ADDITION") + GetOffset() + character * 0x100 + hit * 0x20 + 0x1A, Globals.DICTIONARY.AdditionData[slot, additionnum[addition], hit].UU11);
                        emulator.WriteByte(Constants.GetAddress("ADDITION") + GetOffset() + character * 0x100 + hit * 0x20 + 0x1B, Globals.DICTIONARY.AdditionData[slot, additionnum[addition], hit].UU12);
                        emulator.WriteByte(Constants.GetAddress("ADDITION") + GetOffset() + character * 0x100 + hit * 0x20 + 0x1C, Globals.DICTIONARY.AdditionData[slot, additionnum[addition], hit].UU13);
                        emulator.WriteByte(Constants.GetAddress("ADDITION") + GetOffset() + character * 0x100 + hit * 0x20 + 0x1D, Globals.DICTIONARY.AdditionData[slot, additionnum[addition], hit].UU14);
                        emulator.WriteByte(Constants.GetAddress("ADDITION") + GetOffset() + character * 0x100 + hit * 0x20 + 0x1E, Globals.DICTIONARY.AdditionData[slot, additionnum[addition], hit].Start_Time);
                        emulator.WriteByte(Constants.GetAddress("ADDITION") + GetOffset() + character * 0x100 + hit * 0x20 + 0x1F, Globals.DICTIONARY.AdditionData[slot, additionnum[addition], hit].UU15);
                    }
                    int addition_level = emulator.ReadByteU(Constants.GetAddress("CHAR_TABLE") + Constants.OFFSET + (slot * 0x2C) + 0x1A + additionnum[addition]);
                    Globals.CHARACTER_TABLE[character].Write("ADD_DMG_Multi", Globals.DICTIONARY.AdditionData[slot, additionnum[addition], addition_level].ADD_DMG_Multi);
                    Globals.CHARACTER_TABLE[character].Write("ADD_SP_Multi", Globals.DICTIONARY.AdditionData[slot, additionnum[addition], addition_level].ADD_SP_Multi);
                }
            }
        }
        if (Globals.NO_DART != null) {
            Dictionary<int, byte> charelement = new Dictionary<int, byte> {
                {0, 128 },
                {1, 64 },
                {2, 32 },
                {3, 4 },
                {4, 16 },
                {5, 64 },
                {6, 1 },
                {7, 2 },
                {8, 32 }
            };
            while ((emulator.ReadShort(Constants.GetAddress("BATTLE_VALUE")) > 9999) && (Globals.CHARACTER_TABLE[0].Read("Turn") == 0)) {
                Thread.Sleep(50);
            }
            int current_turn = Globals.CHARACTER_TABLE[0].Read("Turn");
            int character = (int)Globals.NO_DART;
            int dlv = Globals.CURRENT_STATS[character].DLV;
            Globals.CHARACTER_TABLE[0].Write("Turn", 800);
            Globals.CHARACTER_TABLE[0].Write("SP", 100);
            emulator.WriteByteU(Constants.GetAddress("PARTY_SLOT") + Constants.OFFSET, (byte)character);
            emulator.WriteByteU(Constants.GetAddress("PARTY_SLOT") + Constants.OFFSET + 0x234E, (byte)character); // Secondary ID
            Globals.CHARACTER_TABLE[0].Write("Image", (byte)Globals.NO_DART);
            Globals.CHARACTER_TABLE[0].Write("A_HIT", Globals.CURRENT_STATS[character].A_Hit);
            Globals.CHARACTER_TABLE[0].Write("LV", Globals.CURRENT_STATS[character].LV);
            Globals.CHARACTER_TABLE[0].Write("DLV", Globals.CURRENT_STATS[character].DLV);
            Globals.CHARACTER_TABLE[0].Write("HP_Regen", 0);
            Globals.CHARACTER_TABLE[0].Write("SP_Regen", 0);
            Globals.CHARACTER_TABLE[0].Write("MP_Regen", 0);
            Globals.CHARACTER_TABLE[0].Write("Element", charelement[character]);
            if (Globals.DRAGOON_CHANGE == false) {
                Globals.CHARACTER_TABLE[0].Write("DAT", Globals.DICTIONARY.DragoonStats[character][dlv].DAT);
                Globals.CHARACTER_TABLE[0].Write("DMAT", Globals.DICTIONARY.DragoonStats[character][dlv].DMAT);
                Globals.CHARACTER_TABLE[0].Write("DDF", Globals.DICTIONARY.DragoonStats[character][dlv].DDF);
                Globals.CHARACTER_TABLE[0].Write("DMDF", Globals.DICTIONARY.DragoonStats[character][dlv].DMDF);
                if (Globals.ITEM_CHANGE == false) {
                    Globals.CHARACTER_TABLE[0].Write("MP", Globals.CURRENT_STATS[character].MP);
                    Globals.CHARACTER_TABLE[0].Write("Max_MP", Globals.CURRENT_STATS[character].Max_MP);
                }
            }
            if (Globals.ADDITION_CHANGE == false) {
                Dictionary<int, int> additionnum = new Dictionary<int, int> {
                    {0, 0 },
                    {1, 2 },
                    {2, 2 },
                    {3, 3 },
                    {4, 4 },
                    {5, 5 },
                    {6, 6 },
                    {8, 0 },
                    {9, 1 },
                    {10, 2 },
                    {11, 3 },
                    {12, 4 },
                    {14, 0 },
                    {15, 1 },
                    {16, 2 },
                    {17, 3 },
                    {29, 0 },
                    {30, 1 },
                    {31, 2 },
                    {32, 3 },
                    {33, 4 },
                    {34, 5 },
                    {23, 0 },
                    {24, 1 },
                    {25, 2 },
                    {26, 3 },
                    {27, 4 },
                    {19, 0 },
                    {20, 1 },
                    {21, 2 },
                    {255, 0 }
                };
                int addition = emulator.ReadByteU(Constants.GetAddress("CHAR_TABLE") + Constants.OFFSET + (character * 0x2C) + 0x19);
                for (int hit = 0; hit < 8; hit++) {
                    emulator.WriteShort(Constants.GetAddress("ADDITION") + GetOffset() + hit * 0x20, (ushort)Globals.DICTIONARY.AdditionData[character, additionnum[addition], hit].UU1);
                    emulator.WriteShort(Constants.GetAddress("ADDITION") + GetOffset() + hit * 0x20 + 0x2, (ushort)Globals.DICTIONARY.AdditionData[character, additionnum[addition], hit].Next_Hit);
                    emulator.WriteShort(Constants.GetAddress("ADDITION") + GetOffset() + hit * 0x20 + 0x4, (ushort)Globals.DICTIONARY.AdditionData[character, additionnum[addition], hit].Blue_Time);
                    emulator.WriteShort(Constants.GetAddress("ADDITION") + GetOffset() + hit * 0x20 + 0x6, (ushort)Globals.DICTIONARY.AdditionData[character, additionnum[addition], hit].Gray_Time);
                    emulator.WriteShort(Constants.GetAddress("ADDITION") + GetOffset() + hit * 0x20 + 0x8, (ushort)Globals.DICTIONARY.AdditionData[character, additionnum[addition], hit].DMG);
                    emulator.WriteShort(Constants.GetAddress("ADDITION") + GetOffset() + hit * 0x20 + 0xA, (ushort)Globals.DICTIONARY.AdditionData[character, additionnum[addition], hit].SP);
                    emulator.WriteShort(Constants.GetAddress("ADDITION") + GetOffset() + hit * 0x20 + 0xC, (ushort)Globals.DICTIONARY.AdditionData[character, additionnum[addition], hit].ID);
                    emulator.WriteByte(Constants.GetAddress("ADDITION") + GetOffset() + hit * 0x20 + 0xE, Globals.DICTIONARY.AdditionData[character, additionnum[addition], hit].Final_Hit);
                    emulator.WriteByte(Constants.GetAddress("ADDITION") + GetOffset() + hit * 0x20 + 0xF, Globals.DICTIONARY.AdditionData[character, additionnum[addition], hit].UU2);
                    emulator.WriteByte(Constants.GetAddress("ADDITION") + GetOffset() + hit * 0x20 + 0x10, Globals.DICTIONARY.AdditionData[character, additionnum[addition], hit].UU3);
                    emulator.WriteByte(Constants.GetAddress("ADDITION") + GetOffset() + hit * 0x20 + 0x11, Globals.DICTIONARY.AdditionData[character, additionnum[addition], hit].UU4);
                    emulator.WriteByte(Constants.GetAddress("ADDITION") + GetOffset() + hit * 0x20 + 0x12, Globals.DICTIONARY.AdditionData[character, additionnum[addition], hit].UU5);
                    emulator.WriteByte(Constants.GetAddress("ADDITION") + GetOffset() + hit * 0x20 + 0x13, Globals.DICTIONARY.AdditionData[character, additionnum[addition], hit].UU6);
                    emulator.WriteByte(Constants.GetAddress("ADDITION") + GetOffset() + hit * 0x20 + 0x14, Globals.DICTIONARY.AdditionData[character, additionnum[addition], hit].UU7);
                    emulator.WriteByte(Constants.GetAddress("ADDITION") + GetOffset() + hit * 0x20 + 0x15, Globals.DICTIONARY.AdditionData[character, additionnum[addition], hit].UU8);
                    emulator.WriteByte(Constants.GetAddress("ADDITION") + GetOffset() + hit * 0x20 + 0x16, Globals.DICTIONARY.AdditionData[character, additionnum[addition], hit].UU9);
                    emulator.WriteByte(Constants.GetAddress("ADDITION") + GetOffset() + hit * 0x20 + 0x17, Globals.DICTIONARY.AdditionData[character, additionnum[addition], hit].UU10);
                    emulator.WriteShort(Constants.GetAddress("ADDITION") + GetOffset() + hit * 0x20 + 0x18, (ushort)Globals.DICTIONARY.AdditionData[character, additionnum[addition], hit].Vertical_Distance);
                    emulator.WriteByte(Constants.GetAddress("ADDITION") + GetOffset() + hit * 0x20 + 0x1A, Globals.DICTIONARY.AdditionData[character, additionnum[addition], hit].UU11);
                    emulator.WriteByte(Constants.GetAddress("ADDITION") + GetOffset() + hit * 0x20 + 0x1B, Globals.DICTIONARY.AdditionData[character, additionnum[addition], hit].UU12);
                    emulator.WriteByte(Constants.GetAddress("ADDITION") + GetOffset() + hit * 0x20 + 0x1C, Globals.DICTIONARY.AdditionData[character, additionnum[addition], hit].UU13);
                    emulator.WriteByte(Constants.GetAddress("ADDITION") + GetOffset() + hit * 0x20 + 0x1D, Globals.DICTIONARY.AdditionData[character, additionnum[addition], hit].UU14);
                    emulator.WriteByte(Constants.GetAddress("ADDITION") + GetOffset() + hit * 0x20 + 0x1E, Globals.DICTIONARY.AdditionData[character, additionnum[addition], hit].Start_Time);
                    emulator.WriteByte(Constants.GetAddress("ADDITION") + GetOffset() + hit * 0x20 + 0x1F, Globals.DICTIONARY.AdditionData[character, additionnum[addition], hit].UU15);
                }
                int addition_level = emulator.ReadByteU(Constants.GetAddress("CHAR_TABLE") + Constants.OFFSET + (character * 0x2C) + 0x1A + additionnum[addition]);
                Globals.CHARACTER_TABLE[0].Write("ADD_DMG_Multi", Globals.DICTIONARY.AdditionData[character, additionnum[addition], addition_level].ADD_DMG_Multi);
                Globals.CHARACTER_TABLE[0].Write("ADD_SP_Multi", Globals.DICTIONARY.AdditionData[character, additionnum[addition], addition_level].ADD_SP_Multi);
            }
            emulator.WriteByteU(Constants.GetAddress("DRAGOON_MAGIC") + Constants.OFFSET, (byte)character); // Magic
            Dictionary<int, byte> dmagic5 = new Dictionary<int, byte> {
                    {0, 3 },
                    {1, 8 },
                    {2, 13 },
                    {3, 19 },
                    {4, 23 },
                    {5, 8 },
                    {6, 28 },
                    {7, 32 },
                    {8, 13 }
                };
            Dictionary<int, byte> dmagic3 = new Dictionary<int, byte> {
                    {0, 2 },
                    {1, 6 },
                    {2, 12 },
                    {3, 18 },
                    {4, 22 },
                    {5, 17 },
                    {6, 27 },
                    {7, 255 },
                    {8, 67 }
                };
            Dictionary<int, byte> dmagic2 = new Dictionary<int, byte> {
                    {0, 1 },
                    {1, 7 },
                    {2, 10 },
                    {3, 16 },
                    {4, 21 },
                    {5, 26 },
                    {6, 25 },
                    {7, 31 },
                    {8, 65 }
                };
            Dictionary<int, byte> dmagic1 = new Dictionary<int, byte> {
                    {0, 0 },
                    {1, 5 },
                    {2, 11 },
                    {3, 15 },
                    {4, 20 },
                    {5, 14 },
                    {6, 24 },
                    {7, 30 },
                    {8, 66 }
                };
            if (dlv == 5) {
                emulator.WriteByteU(Constants.GetAddress("DRAGOON_MAGIC") + Constants.OFFSET + 4, dmagic5[character]);
                emulator.WriteByteU(Constants.GetAddress("DRAGOON_MAGIC") + Constants.OFFSET + 3, dmagic3[character]);
                emulator.WriteByteU(Constants.GetAddress("DRAGOON_MAGIC") + Constants.OFFSET + 2, dmagic2[character]);
                emulator.WriteByteU(Constants.GetAddress("DRAGOON_MAGIC") + Constants.OFFSET + 1, dmagic1[character]);
            } else if (dlv > 2) {
                emulator.WriteByteU(Constants.GetAddress("DRAGOON_MAGIC") + Constants.OFFSET + 4, 0xFF);
                emulator.WriteByteU(Constants.GetAddress("DRAGOON_MAGIC") + Constants.OFFSET + 3, dmagic3[character]);
                emulator.WriteByteU(Constants.GetAddress("DRAGOON_MAGIC") + Constants.OFFSET + 2, dmagic2[character]);
                emulator.WriteByteU(Constants.GetAddress("DRAGOON_MAGIC") + Constants.OFFSET + 1, dmagic1[character]);
            } else if (dlv > 1) { 
                emulator.WriteByteU(Constants.GetAddress("DRAGOON_MAGIC") + Constants.OFFSET + 4, 0xFF);
                emulator.WriteByteU(Constants.GetAddress("DRAGOON_MAGIC") + Constants.OFFSET + 3, 0xFF);
                emulator.WriteByteU(Constants.GetAddress("DRAGOON_MAGIC") + Constants.OFFSET + 2, dmagic2[character]);
                emulator.WriteByteU(Constants.GetAddress("DRAGOON_MAGIC") + Constants.OFFSET + 1, dmagic1[character]);
            } else if (dlv > 0) {
                emulator.WriteByteU(Constants.GetAddress("DRAGOON_MAGIC") + Constants.OFFSET + 4, 0xFF);
                emulator.WriteByteU(Constants.GetAddress("DRAGOON_MAGIC") + Constants.OFFSET + 3, 0xFF);
                emulator.WriteByteU(Constants.GetAddress("DRAGOON_MAGIC") + Constants.OFFSET + 2, 0xFF);
                emulator.WriteByteU(Constants.GetAddress("DRAGOON_MAGIC") + Constants.OFFSET + 1, dmagic1[character]);
            } else {
                emulator.WriteByteU(Constants.GetAddress("DRAGOON_MAGIC") + Constants.OFFSET + 4, 0xFF);
                emulator.WriteByteU(Constants.GetAddress("DRAGOON_MAGIC") + Constants.OFFSET + 3, 0xFF);
                emulator.WriteByteU(Constants.GetAddress("DRAGOON_MAGIC") + Constants.OFFSET + 2, 0xFF);
                emulator.WriteByteU(Constants.GetAddress("DRAGOON_MAGIC") + Constants.OFFSET + 1, 0xFF);
            }
            while ((emulator.ReadShort(Constants.GetAddress("BATTLE_VALUE")) > 9999) && (Globals.CHARACTER_TABLE[0].Read("Action") != 8)) {
                Thread.Sleep(50);
            }
            Constants.WriteDebug("TURN");
            Globals.CHARACTER_TABLE[0].Write("Turn", current_turn);
            Thread.Sleep(250);
            Globals.CHARACTER_TABLE[0].Write("Menu", 16);
            while ((emulator.ReadShort(Constants.GetAddress("BATTLE_VALUE")) > 9999) && (Globals.CHARACTER_TABLE[0].Read("Menu") != 96)) {
                Thread.Sleep(50);
            }
            Globals.CHARACTER_TABLE[0].Write("Menu", 16);
            while ((emulator.ReadShort(Constants.GetAddress("BATTLE_VALUE")) > 9999) && (Globals.CHARACTER_TABLE[0].Read("Action") != 0)) {
                Thread.Sleep(50);
            }
            Globals.CHARACTER_TABLE[0].Write("SP", Globals.CURRENT_STATS[character].SP);
            Globals.CHARACTER_TABLE[0].Write("HP_Regen", Globals.CURRENT_STATS[character].HP_Regen);
            Globals.CHARACTER_TABLE[0].Write("MP_Regen", Globals.CURRENT_STATS[character].MP_Regen);
            Globals.CHARACTER_TABLE[0].Write("SP_Regen", Globals.CURRENT_STATS[character].SP_Regen);
        }
    }

    public class MonsterAddress {
        long[] action = { 0, 1 };
        long[] hp = { 0, 2 };
        long[] max_hp = { 0, 2 };
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
        long[] a_av = { 0, 1 };
        long[] m_av = { 0, 1 };
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
            at[0] = m_point + 0x2c - monster * 0x388;
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
            special_effect[0] = Constants.GetAddress("UNIQUE_MONSTERS") + monster * 0x20;
        }

        public object Read(string attribute) {
            PropertyInfo property = GetType().GetProperty(attribute);
            var address = (long[]) property.GetValue(this, null);
            if (address[1] == 2) {
                return this.emulator.ReadShortU(address[0]);
            } else {
                return this.emulator.ReadByteU(address[0]);
            }
        }

        public void Write(string attribute, object value) {
            PropertyInfo property = GetType().GetProperty(attribute);
            var address = (long[]) property.GetValue(this, null);
            if (address[1] == 2) {
                this.emulator.WriteShortU(address[0], Convert.ToUInt16(value));
            } else {
                this.emulator.WriteByteU(address[0], Convert.ToByte(value));
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
        long[] pos_x = { 0, 4 };
        long[] pos_y = { 0, 4 };
        long[] pos_z = { 0, 4 };
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
        public long[] POS_X { get { return pos_x; } }
        public long[] POS_Y { get { return pos_y; } }
        public long[] POS_Z { get { return pos_z; } }
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

        public CharAddress(long c_point, int character, Emulator emu) {
            emulator = emu;
            special_effect[0] = Constants.GetAddress("UNIQUE_MONSTERS") + (character + Globals.MONSTER_SIZE) * 0x20;
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
            pos_x[0] = c_point + 0x16D - character * 0x388;
            pos_y[0] = c_point + 0x171 - character * 0x388;
            pos_z[0] = c_point + 0x175 - character * 0x388;
            unique_index[0] = c_point + 0x264 - character * 0x388;
            image[0] = c_point + 0x26A - character * 0x388;
        }

        public object Read(string attribute) {
            PropertyInfo property = GetType().GetProperty(attribute);
            var address = (long[]) property.GetValue(this, null);
            if (address[1] == 4) {
                return this.emulator.ReadIntegerU(address[0]);
            } else if (address[1] == 2) {
                return this.emulator.ReadShortU(address[0]);
            } else {
                return this.emulator.ReadByteU(address[0]);
            }
        }

        public void Write(string attribute, object value) {
            PropertyInfo property = GetType().GetProperty(attribute);
            var address = (long[]) property.GetValue(this, null);
            if (address[1] == 4) {
                this.emulator.WriteIntegerU(address[0], Convert.ToInt32(value));
            } else if (address[1] == 2) {
                this.emulator.WriteShortU(address[0], Convert.ToUInt16(value));
            } else {
                this.emulator.WriteByteU(address[0], Convert.ToByte(value));
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
        byte a_hit = 100;
        byte m_hit = 100;
        byte stat_res = 0;
        byte e_half = 0;
        byte e_immune = 0;
        byte p_half = 0;
        byte m_half = 0;
        byte status = 0;
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
        public byte Status { get { return status; } }
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

        public CurrentStats(int character, Emulator emulator) {
            lv = emulator.ReadByteU(Constants.GetAddress("CHAR_TABLE") + Constants.OFFSET + (character * 0x2C) + 0x12);
            dlv = emulator.ReadByteU(Constants.GetAddress("CHAR_TABLE") + Constants.OFFSET + (character * 0x2C) + 0x13);
            weapon = Globals.DICTIONARY.ItemList[emulator.ReadByteU(Constants.GetAddress("CHAR_TABLE") + Constants.OFFSET + character * 0x2C + 0x14)];
            armor = Globals.DICTIONARY.ItemList[emulator.ReadByteU(Constants.GetAddress("CHAR_TABLE") + Constants.OFFSET + character * 0x2C + 0x15)];
            helm = Globals.DICTIONARY.ItemList[emulator.ReadByteU(Constants.GetAddress("CHAR_TABLE") + Constants.OFFSET + character * 0x2C + 0x16)];
            boots = Globals.DICTIONARY.ItemList[emulator.ReadByteU(Constants.GetAddress("CHAR_TABLE") + Constants.OFFSET + character * 0x2C + 0x17)];
            accessory = Globals.DICTIONARY.ItemList[emulator.ReadByteU(Constants.GetAddress("CHAR_TABLE") + Constants.OFFSET + character * 0x2C + 0x18)];
            hp = emulator.ReadShortU(Constants.GetAddress("CHAR_TABLE") + Constants.OFFSET + character * 0x2C + 0x8);
            max_hp = (ushort)(Globals.DICTIONARY.CharacterStats[character][lv].Max_HP * (1 + ((weapon.Special2 & 2) >> 1) * (float)(weapon.Special_Ammount) / 100 + ((armor.Special2 & 2) >> 1) * (float)(armor.Special_Ammount) / 100
                + ((helm.Special2 & 2) >> 1) * (float)(helm.Special_Ammount) / 100 + ((boots.Special2 & 2) >> 1) * (float)(boots.Special_Ammount) / 100 + ((accessory.Special2 & 2) >> 1) * (float)(accessory.Special_Ammount) / 100));
            mp = emulator.ReadShortU(Constants.GetAddress("CHAR_TABLE") + Constants.OFFSET + character * 0x2C + 0xA);
            if (Globals.DRAGOON_CHANGE == true) {
                max_mp = (ushort)(Globals.DICTIONARY.DragoonStats[character][dlv].MP * (1 + (weapon.Special2 & 1) * (float)(weapon.Special_Ammount) / 100 + (armor.Special2 & 1) * (float)(armor.Special_Ammount) / 100
                     + (helm.Special2 & 1) * (float)(helm.Special_Ammount) / 100 + (boots.Special2 & 1) * (float)(boots.Special_Ammount) / 100 + (accessory.Special2 & 1) * (float)(accessory.Special_Ammount) / 100));
            } else {
                max_mp = (ushort)(dlv * 20 * (1 + (weapon.Special2 & 1) * (float)(weapon.Special_Ammount) / 100 + (armor.Special2 & 1) * (float)(armor.Special_Ammount) / 100
                     + (helm.Special2 & 1) * (float)(helm.Special_Ammount) / 100 + (boots.Special2 & 1) * (float)(boots.Special_Ammount) / 100 + (accessory.Special2 & 1) * (float)(accessory.Special_Ammount) / 100));
            }
            sp = emulator.ReadShortU(Constants.GetAddress("CHAR_TABLE") + Constants.OFFSET + character * 0x2C + 0xC);
            at = (ushort)(Globals.DICTIONARY.CharacterStats[character][lv].AT + weapon.AT + armor.AT + helm.AT + boots.AT + accessory.AT);
            mat = (ushort)(Globals.DICTIONARY.CharacterStats[character][lv].MAT + weapon.MAT + armor.MAT + helm.MAT + boots.MAT + accessory.MAT);
            df = (ushort)(Globals.DICTIONARY.CharacterStats[character][lv].DF + weapon.DF + armor.DF + helm.DF + boots.DF + accessory.DF);
            mdf = (ushort)(Globals.DICTIONARY.CharacterStats[character][lv].MDF + weapon.MDF + armor.MDF + helm.MDF + boots.MDF + accessory.MDF);
            spd = (ushort)(Globals.DICTIONARY.CharacterStats[character][lv].SPD + weapon.SPD + armor.SPD + helm.SPD + boots.SPD + accessory.SPD);
            stat_res |= weapon.Stat_Res | armor.Stat_Res | helm.Stat_Res | boots.Stat_Res | accessory.Stat_Res;
            e_half |= weapon.E_Half | armor.E_Half | helm.E_Half | boots.E_Half | accessory.E_Half;
            e_immune |= weapon.E_Immune | armor.E_Immune | helm.E_Immune | boots.E_Immune | accessory.E_Immune;
            a_av = (byte)(weapon.A_AV + armor.A_AV + helm.A_AV + boots.A_AV + accessory.A_AV);
            m_av = (byte)(weapon.M_AV + armor.M_AV + helm.M_AV + boots.M_AV + accessory.M_AV);
            a_hit += (byte)(weapon.A_Hit + armor.A_Hit + helm.A_Hit + boots.A_Hit + accessory.A_Hit);
            m_hit += (byte)(weapon.M_Hit + armor.M_Hit + helm.M_Hit + boots.M_Hit + accessory.M_Hit);
            p_half |= ((weapon.Special1 & 0x20) | (armor.Special1 & 0x20) | (helm.Special1 & 0x20) | (boots.Special1 & 0x20) | (accessory.Special1 & 0x20)) >> 5;
            m_half |= ((weapon.Special2 & 0x4) | (armor.Special2 & 0x4) | (helm.Special2 & 0x4) | (boots.Special2 & 0x4) | (accessory.Special2 & 0x4)) >> 2;
            status = weapon.Status;
            status_chance = weapon.Status_Chance;
            element = weapon.Element;
            revive = (byte)(((weapon.Special2 & 0x8) >> 3) * weapon.Special_Ammount + ((armor.Special2 & 0x8) >> 3) * armor.Special_Ammount + ((helm.Special2 & 0x8) >> 3) * helm.Special_Ammount
                + ((boots.Special2 & 0x8) >> 3) * boots.Special_Ammount + ((accessory.Special2 & 0x8) >> 3) * accessory.Special_Ammount);
            sp_regen = (ushort)(((weapon.Special2 & 0x10) >> 4) * weapon.Special_Ammount + ((armor.Special2 & 0x10) >> 4) * armor.Special_Ammount + ((helm.Special2 & 0x10) >> 4) * helm.Special_Ammount
                + ((boots.Special2 & 0x10) >> 4) * boots.Special_Ammount + ((accessory.Special2 & 0x10) >> 4) * accessory.Special_Ammount);
            mp_regen = (ushort)(((weapon.Special2 & 0x20) >> 5) * weapon.Special_Ammount + ((armor.Special2 & 0x20) >> 5) * armor.Special_Ammount + ((helm.Special2 & 0x20) >> 5) * helm.Special_Ammount
                + ((boots.Special2 & 0x20) >> 5) * boots.Special_Ammount + ((accessory.Special2 & 0x20) >> 5) * accessory.Special_Ammount);
            hp_regen = (ushort)(((weapon.Special2 & 0x40) >> 6) * weapon.Special_Ammount + ((armor.Special2 & 0x40) >> 6) * armor.Special_Ammount + ((helm.Special2 & 0x40) >> 6) * helm.Special_Ammount
                + ((boots.Special2 & 0x40) >> 6) * boots.Special_Ammount + ((accessory.Special2 & 0x40) >> 6) * accessory.Special_Ammount);
            mp_m_hit = (byte)((weapon.Special1 & 0x1) * weapon.Special_Ammount + (armor.Special1 & 0x1) * armor.Special_Ammount + (helm.Special1 & 0x1) * helm.Special_Ammount
                + (boots.Special1 & 0x1) * boots.Special_Ammount + (accessory.Special1 & 0x1) * accessory.Special_Ammount);
            sp_m_hit = (byte)(((weapon.Special1 & 0x2) >> 1) * weapon.Special_Ammount + ((armor.Special1 & 0x2) >> 1) * armor.Special_Ammount + ((helm.Special1 & 0x2) >> 1) * helm.Special_Ammount
                + ((boots.Special1 & 0x2) >> 1) * boots.Special_Ammount + ((accessory.Special1 & 0x2) >> 1) * accessory.Special_Ammount);
            mp_p_hit = (byte)(((weapon.Special1 & 0x4) >> 2) * weapon.Special_Ammount + ((armor.Special1 & 0x4) >> 2) * armor.Special_Ammount + ((helm.Special1 & 0x4) >> 2) * helm.Special_Ammount
                + ((boots.Special1 & 0x4) >> 2) * boots.Special_Ammount + ((accessory.Special1 & 0x4) >> 2) * accessory.Special_Ammount);
            sp_p_hit = (byte)(((weapon.Special1 & 0x8) >> 3) * weapon.Special_Ammount + ((armor.Special1 & 0x8) >> 3) * armor.Special_Ammount + ((helm.Special1 & 0x8) >> 3) * helm.Special_Ammount
                + ((boots.Special1 & 0x8) >> 3) * boots.Special_Ammount + ((accessory.Special1 & 0x8) >> 3) * accessory.Special_Ammount);
            sp_multi = (byte)(((weapon.Special1 & 0x10) >> 4) * weapon.Special_Ammount + ((armor.Special1 & 0x10) >> 4) * armor.Special_Ammount + ((helm.Special1 & 0x10) >> 4) * helm.Special_Ammount
                + ((boots.Special1 & 0x4) >> 4) * boots.Special_Ammount + ((accessory.Special1 & 0x10) >> 4) * accessory.Special_Ammount);
            death_res |= weapon.Death_Res | armor.Death_Res | helm.Death_Res | boots.Death_Res | accessory.Death_Res;
        }
    }

    public static void Open(Emulator emulator) { }
    public static void Close(Emulator emulator) { }
    public static void Click(Emulator emulator) { }
}