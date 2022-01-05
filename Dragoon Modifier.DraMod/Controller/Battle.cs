using Dragoon_Modifier.Emulator;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Controller {
    internal static class Battle {
        static readonly uint[] sharanda = new uint[] { 0x2, 0x8 };
        static readonly ushort[] slot1FinalBlow = new ushort[] { 414, 408, 409, 392, 431 };      // Urobolus, Wounded Virage, Complete Virage, Lloyd, Zackwell
        static readonly ushort[] slot2FinalBlow = new ushort[] { 387, 403 };                     // Fruegel II, Gehrich
        static double[,] originalCharacterStats = new double[3, 10];
        static double[,] originalMonsterStats = new double[5, 6];

        static readonly ushort[] bosses = new ushort[] {
            384, //Commander
            386, //Fruegel I
            414, //Urobolus
            385, //Sandora Elite
            388, //Kongol I
            408, //Virage I
            415, //Fire Bird
            393, //Greham + Feyrbrand
            412, //Drake the Bandit
            413, //Jiango
            387, //Fruegel II
            461, //Sandora Elite II
            389, //Kongol II
            390, //Emperor Doel
            402, //Mappi
            409, //Virage II
            403, //Gehrich + Mappi
            396, //Lenus
            417, //Ghost Commander
            397, //Lenus + Regole
            418, //Kamuy
            410, //S Virage
            416, //Grand Jewel
            394, //Divine Dragon
            422, //Windigo
            392, //Lloyd
            423, //Polter Set
            398, //Damia
            399, //Syuveil
            400, //Belzac
            401, //Kanzas
            420, //Magician Faust
            432, //Last Kraken
            430, //Executioners
            449, //Spirit (Feyrbrand)
            448, //Spirit (Regole)
            447, //Spirit (Divine Dragon)
            431, //Zackwell
            433, //Imago
            411, //S Virage II
            442, //Zieg
            443 //Melbu Fraahma
            };

        //Damage Cap Scan
        static bool firstDamageCapRemoval = false;
        static int lastItemUsedDamageCap = 0;
        //Elemental Bomb
        static byte eleBombTurns = 0;
        static byte eleBombItemUsed = 0;
        static byte eleBombSlot = 0;
        static byte eleBombElement = 0;
        static bool eleBombChange = false;
        static ushort[] eleBombOldElement = { 0, 0, 0, 0, 0 };
        //Enrage
        static byte[] enragedMode = { 0, 0, 0, 0, 0 };
        static bool enrageBoss = false;
        //Soul Eater
        static bool noHPDecayOnBattleEntry = false;
        //Damage Tracker
        static bool damageTrackerOnBattleEntry = false;
        static int[] dmgTrkHP = new int[6];
        static int[] dmgTrkChr = new int[3];
        static int dmgTrkSlot = 0;
        //Solo/Duo Mode
        static bool addSoloPartyMembers = false;
        static bool alwaysAddSoloPartyMembers = false;
        static bool soloModeOnBattleEntry = false;
        static bool duoModeOnBattleEntry = false;

        static readonly List<Hotkey> hotkeys = BattleHotkeys.Load();

        public static void Setup(Emulator.IEmulator emulator, LoDDict.ILoDDictionary LoDDict, UI.IUIControl uiControl) {
            Console.WriteLine("Battle detected. Loading..."); 

            firstDamageCapRemoval = false;
            lastItemUsedDamageCap = 0;

            eleBombChange = false;
            eleBombTurns = 0;
            eleBombElement = 255;
            eleBombSlot = 255;
            eleBombItemUsed = 255;

            noHPDecayOnBattleEntry = false;

            uint tableBase = emulator.Memory.BattleBasePoint;
            while (tableBase == emulator.Memory.CharacterPoint || tableBase == emulator.Memory.MonsterPoint) { // Wait until both C_Point and M_Point were set
                if (Constants.Run && emulator.Memory.GameState != Emulator.GameState.Battle) {
                    return;
                }
                Thread.Sleep(50);
            }

            emulator.LoadBattle();

            for (int i = 0; i < emulator.Memory.MonsterSize; i++) {
                /*if (ultimateHP[i] > 0) { //TODO Ultimate boss
                    dmgTrkHP[i] = ultimateHP[i];
                } else {*/
                dmgTrkHP[i] = emulator.Battle.MonsterTable[i].HP;
                //}
            }
            for (int i = 0; i < 3; i++) {
                dmgTrkChr[i] = 0;
            }
            damageTrackerOnBattleEntry = true;

            uiControl.UpdateField(emulator.Memory.BattleValue, emulator.Memory.EncounterID, emulator.Memory.MapID);

            if (Settings.DualDifficulty) {
                Console.WriteLine("[DEBUG] [Dual Difficulty] Changing monster stats...");
                SwitchDualDifficulty(emulator, LoDDict);
            }

            MonsterChanges(emulator, LoDDict);

            UpdateUI(emulator, uiControl);

            if (Settings.SoloMode) {
                SoloModeBattle(emulator, uiControl);
            }

            if (Settings.DuoMode) {
                DuoModeBattle(emulator, uiControl);
            }

            if (Settings.NoDart != 0 && Settings.NoDart != 255) {
                NoDart.Initialize(emulator, Settings.NoDart);
            }

            if (Settings.NoDragoon) {
                NoDragoonMode(emulator);
            }

            if (Settings.AspectRatio) {
                ChangeAspectRatio(emulator, uiControl);
            }

            for (int i = 0; i < 3; i++) { //Should execute last
                if (emulator.Memory.PartySlot[i] < 9) {
                    originalCharacterStats[i, 0] = emulator.Battle.CharacterTable[i].MaxHP;
                    originalCharacterStats[i, 1] = emulator.Battle.CharacterTable[i].AT;
                    originalCharacterStats[i, 2] = emulator.Battle.CharacterTable[i].MAT;
                    originalCharacterStats[i, 3] = emulator.Battle.CharacterTable[i].DF;
                    originalCharacterStats[i, 4] = emulator.Battle.CharacterTable[i].MDF;
                }
            }

            for (int i = 0; i < emulator.Memory.MonsterSize; i++) {
                originalMonsterStats[i, 0] = emulator.Battle.MonsterTable[i].MaxHP;
                originalMonsterStats[i, 1] = emulator.Battle.MonsterTable[i].AT;
                originalMonsterStats[i, 2] = emulator.Battle.MonsterTable[i].MAT;
                originalMonsterStats[i, 3] = emulator.Battle.MonsterTable[i].DF;
                originalMonsterStats[i, 4] = emulator.Battle.MonsterTable[i].MDF;
                originalMonsterStats[i, 5] = emulator.Battle.MonsterTable[i].SPD;
                enragedMode[i] = 0;
            }
        }

        public static void Run(Emulator.IEmulator emulator, UI.IUIControl uiControl) {
            UpdateUI(emulator, uiControl);

            if (Settings.RemoveDamageCaps) {
                RemoveDamageCaps(emulator);
            }

            if (Settings.ElementalBomb) {
                ElementalBomb(emulator);
            }

            if (Settings.MonsterHPAsNames) {
                MonsterHPNames(emulator);
            }

            if (Settings.NoDecaySoulEater && !noHPDecayOnBattleEntry) {
                NoHPDecaySoulEater(emulator);
            }

            if (Settings.NeverGuard) {
                NeverGuard(emulator);
            }

            if (Settings.DamageTracker) {
                DamageTracker(emulator, uiControl);
            }

            if (Settings.EnrageMode || Settings.EnrageBossOnly) {
                for (int i = 0; i < emulator.Memory.MonsterSize; i++) {
                    EnrageMode(emulator, i); 
                }
            }

            foreach (var hotkey in hotkeys) {
                hotkey.Run(emulator);
            }
        }

        private static void UpdateUI(Emulator.IEmulator emulator, UI.IUIControl uiControl) {
            for (int i = 0; i < emulator.Battle.MonsterTable.Length; i++) {
                uiControl.UpdateMonster(i, new UI.MonsterUpdate(emulator, i));
            }
            for (int i = 0; i < emulator.Battle.CharacterTable.Length; i++) {
                uiControl.UpdateCharacter(i, new UI.CharacterUpdate(emulator, i));
            }
        }

        private static void SwitchDualDifficulty(Emulator.IEmulator emulator, LoDDict.ILoDDictionary LoDDict) {
            string cwd = Directory.GetCurrentDirectory();
            string mod;

            if (bosses.Contains(emulator.Battle.EncounterID)) {
                mod = DraMod.Settings.Mod.Equals("Hell_Mode") ? "Hard_Mode" : "US_Base";
                LoDDict.SwapMonsters(cwd, mod);
                Console.WriteLine("[DEBUG] [Dual Difficulty] Mod selected: " + mod);
                return;
            }

            LoDDict.SwapMonsters(cwd, DraMod.Settings.Mod);
            Console.WriteLine("[DEBUG] [Dual Difficulty] Mod selected: " + DraMod.Settings.Mod);
        }

        private static void MonsterChanges(Emulator.IEmulator emulator, LoDDict.ILoDDictionary LoDDict) {
            if (Settings.MonsterStatChange) {
                Console.WriteLine("Changing monster stats...");
                for (int slot = 0; slot < emulator.Battle.MonsterTable.Length; slot++) {
                    MonsterStatChange(emulator, LoDDict, slot);
                }
            }
        }

        private static void MonsterStatChange(Emulator.IEmulator emulator, LoDDict.ILoDDictionary LoDDict, int slot) {
            ushort id = emulator.Battle.MonsterID[slot];
            double HP = Math.Round(LoDDict.Monster[id].HP * Settings.HPMulti);

            double resup = 1;
            if (HP > 65535) {
                resup = HP / 65535;
                HP = 65535;
            }
            HP = Math.Round(HP);

            Emulator.Memory.Battle.Monster monster = emulator.Battle.MonsterTable[slot];

            monster.HP = (ushort) HP;
            monster.MaxHP = (ushort) HP;
            monster.AT = (ushort) Math.Round(LoDDict.Monster[id].AT * Settings.ATMulti);
            monster.OG_AT = (ushort) Math.Round(LoDDict.Monster[id].AT * Settings.ATMulti);
            monster.MAT = (ushort) Math.Round(LoDDict.Monster[id].MAT * Settings.MATMulti);
            monster.OG_MAT = (ushort) Math.Round(LoDDict.Monster[id].MAT * Settings.MATMulti);
            monster.DF = (ushort) Math.Round(LoDDict.Monster[id].DF * Settings.DFMulti * resup);
            monster.OG_DF = (ushort) Math.Round(LoDDict.Monster[id].DF * Settings.DFMulti * resup);
            monster.MDF = (ushort) Math.Round(LoDDict.Monster[id].MDF * Settings.MDFMulti * resup);
            monster.OG_MDF = (ushort) Math.Round(LoDDict.Monster[id].MDF * Settings.MDFMulti * resup);
            monster.SPD = (ushort) Math.Round(LoDDict.Monster[id].SPD * Settings.SPDMulti);
            monster.OG_SPD = (ushort) Math.Round(LoDDict.Monster[id].SPD * Settings.SPDMulti);
            monster.A_AV = LoDDict.Monster[id].A_AV;
            monster.M_AV = LoDDict.Monster[id].M_AV;
            monster.P_Immune = LoDDict.Monster[id].PhysicalImmunity;
            monster.M_Immune = LoDDict.Monster[id].MagicalImmunity;
            monster.P_Half = LoDDict.Monster[id].PhysicalResistance;
            monster.M_Half = LoDDict.Monster[id].MagicalResistance;
            monster.Element = LoDDict.Monster[id].Element;
            monster.ElementalImmunity = LoDDict.Monster[id].ElementalImmunity;
            monster.ElementalResistance = LoDDict.Monster[id].ElementalResistance;
            monster.StatusResist = LoDDict.Monster[id].StatusResist;
            monster.SpecialEffect = LoDDict.Monster[id].SpecialEffect;
        }

        private static void CharacterChanges(Emulator.IEmulator emulator, LoDDict.ILoDDictionary LoDDict, UI.IUIControl uiControl) {
            for (byte character = 0; character < 9; character++) {

                if (Settings.CharacterStatChange) {
                    CharacterStatChange(emulator, LoDDict, character);
                }

                if (Settings.ItemStatChange) {
                    ItemStatChange(emulator, LoDDict, character);
                }
            }

            LoDDict.ItemScript.BattleSetup(emulator, uiControl);

            foreach (var slot in emulator.Battle.CharacterTable) {
                // Reset Stats
            }

        }

        private static void CharacterStatChange(Emulator.IEmulator emulator, LoDDict.ILoDDictionary LoDDict, int character) {
            byte level = emulator.Memory.CharacterTable[character].Level;

            Emulator.Memory.SecondaryCharacterTable secondaryTable = emulator.Memory.SecondaryCharacterTable[character];

            secondaryTable.BodyAT = LoDDict.Character[character].BaseStats.AT[level];
            secondaryTable.BodyMAT = LoDDict.Character[character].BaseStats.MAT[level];
            secondaryTable.BodyDF = LoDDict.Character[character].BaseStats.DF[level];
            secondaryTable.BodyMDF = LoDDict.Character[character].BaseStats.MDF[level];
            secondaryTable.BodySPD = LoDDict.Character[character].BaseStats.SPD[level];
        }

        private static void ItemStatChange(Emulator.IEmulator emulator, LoDDict.ILoDDictionary LoDDict, int character) {
            LoDDict.IEquipment weapon = (LoDDict.IEquipment) LoDDict.Item[emulator.Memory.CharacterTable[character].Weapon];
            LoDDict.IEquipment helmet = (LoDDict.IEquipment) LoDDict.Item[emulator.Memory.CharacterTable[character].Helmet];
            LoDDict.IEquipment armor = (LoDDict.IEquipment) LoDDict.Item[emulator.Memory.CharacterTable[character].Armor];
            LoDDict.IEquipment shoes = (LoDDict.IEquipment) LoDDict.Item[emulator.Memory.CharacterTable[character].Shoes];
            LoDDict.IEquipment accessory = (LoDDict.IEquipment) LoDDict.Item[emulator.Memory.CharacterTable[character].Accessory];
            LoDDict.IEquipment[] equipment = new LoDDict.IEquipment[5] { weapon, helmet, armor, shoes, accessory };

            Emulator.Memory.SecondaryCharacterTable secondaryTable = emulator.Memory.SecondaryCharacterTable[character];

            secondaryTable.EquipAT = (ushort) equipment.Sum(item => item.AT);
            secondaryTable.EquipMAT = (ushort) equipment.Sum(item => item.MAT);
            secondaryTable.EquipDF = (ushort) equipment.Sum(item => item.DF);
            secondaryTable.EquipMDF = (ushort) equipment.Sum(item => item.MDF);
            secondaryTable.EquipSPD = (ushort) equipment.Sum(item => item.SPD);

            secondaryTable.StatusResist = (byte) (weapon.StatusResistance | helmet.StatusResistance | armor.StatusResistance | shoes.StatusResistance | accessory.StatusResistance);
            secondaryTable.E_Half = (byte) (weapon.ElementalResistance | helmet.ElementalResistance | armor.ElementalResistance | shoes.ElementalResistance | accessory.ElementalResistance);
            secondaryTable.E_Immune = (byte) (weapon.ElementalImmunity | helmet.ElementalImmunity | armor.ElementalImmunity | shoes.ElementalImmunity | accessory.ElementalImmunity);
            secondaryTable.EquipA_AV = (short) equipment.Sum(item => item.A_AV);
            secondaryTable.EquipM_AV = (short) equipment.Sum(item => item.M_AV);
            secondaryTable.EquipA_HIT = (short) equipment.Sum(item => item.A_HIT);
            secondaryTable.EquipM_HIT = (short) equipment.Sum(item => item.M_HIT);
            secondaryTable.P_Half = (byte) (((weapon.SpecialBonus1 | helmet.SpecialBonus1 | armor.SpecialBonus1 | shoes.SpecialBonus1 | accessory.SpecialBonus1) >> 5) & 0x1);
            secondaryTable.M_Half = (byte) (((weapon.SpecialBonus2 | helmet.SpecialBonus2 | armor.SpecialBonus2 | shoes.SpecialBonus2 | accessory.SpecialBonus2) >> 4) & 0x1);
            secondaryTable.OnHitStatus = weapon.OnHitStatus;
            secondaryTable.OnHitStatusChance = weapon.OnHitStatusChance;

            secondaryTable.MP_M_Hit = (short) equipment.Sum(item => (item.SpecialBonus1 & 0x1) * item.SpecialBonusAmmount);
            secondaryTable.SP_M_Hit = (short) equipment.Sum(item => ((item.SpecialBonus1 >> 1) & 0x1) * item.SpecialBonusAmmount);
            secondaryTable.MP_P_Hit = (short) equipment.Sum(item => ((item.SpecialBonus1 >> 2) & 0x1) * item.SpecialBonusAmmount);
            secondaryTable.SP_P_Hit = (short) equipment.Sum(item => ((item.SpecialBonus1 >> 3) & 0x1) * item.SpecialBonusAmmount);
            secondaryTable.SP_Multi = (short) equipment.Sum(item => ((item.SpecialBonus1 >> 4) & 0x1) * item.SpecialBonusAmmount);

            secondaryTable.MP_Multi = (byte) equipment.Sum(item => (item.SpecialBonus2 & 0x1) * item.SpecialBonusAmmount);
            secondaryTable.HP_Multi = (byte) equipment.Sum(item => ((item.SpecialBonus2 >> 2) & 0x1) * item.SpecialBonusAmmount);
            secondaryTable.Revive = (byte) equipment.Sum(item => ((item.SpecialBonus2 >> 3) & 0x1) * item.SpecialBonusAmmount);
            secondaryTable.SP_Regen = (short) equipment.Sum(item => ((item.SpecialBonus2 >> 4) & 0x1) * item.SpecialBonusAmmount);
            secondaryTable.MP_Regen = (short) equipment.Sum(item => ((item.SpecialBonus2 >> 5) & 0x1) * item.SpecialBonusAmmount);
            secondaryTable.HP_Regen = (short) equipment.Sum(item => ((item.SpecialBonus2 >> 6) & 0x1) * item.SpecialBonusAmmount);
           
            secondaryTable.SpecialEffect = (byte) (weapon.SpecialEffect | helmet.SpecialEffect | armor.SpecialEffect | shoes.SpecialEffect | accessory.SpecialEffect);
            secondaryTable.WeaponElement = weapon.WeaponElement;
        }

        private static void RemoveDamageCaps(Emulator.IEmulator emulator) {
            if (!firstDamageCapRemoval) {
                emulator.WriteInt("DAMAGE_CAP", 50000);
                emulator.WriteInt("DAMAGE_CAP", 50000, 0x8);
                emulator.WriteInt("DAMAGE_CAP", 50000, 0x14);
                DamageCapScan(emulator);
                firstDamageCapRemoval = true;
            } else {
                if (lastItemUsedDamageCap != emulator.Battle.ItemUsed) {
                    lastItemUsedDamageCap = emulator.Battle.ItemUsed;
                    if ((lastItemUsedDamageCap >= 0xC1 && lastItemUsedDamageCap <= 0xCA) || (lastItemUsedDamageCap >= 0xCF && lastItemUsedDamageCap <= 0xD2) || lastItemUsedDamageCap == 0xD6 || lastItemUsedDamageCap == 0xD8 || lastItemUsedDamageCap == 0xDC || (lastItemUsedDamageCap >= 0xF1 && lastItemUsedDamageCap <= 0xF8) || lastItemUsedDamageCap == 0xFA) {
                        DamageCapScan(emulator);
                    }
                }
                for (int i = 0; i < 3; i++) {
                    if (emulator.Memory.PartySlot[i] < 9) {
                        if (emulator.Battle.CharacterTable[i].Action == 24) {
                            DamageCapScan(emulator);
                        }
                    }
                }
                for (int i = 0; i < emulator.Memory.MonsterSize; i++) {
                    if (emulator.Battle.MonsterTable[i].Action == 28) { // Most used, not all monsters use action code 28 for item spells
                        DamageCapScan(emulator);
                    }
                }
            }
        }

        private static void DamageCapScan(Emulator.IEmulator emulator) {
            var damageCapScan = emulator.ScanAoB(0xA8660, 0x2A865F, "0F 27");
            long lastAddress = 0;
            foreach (var address in damageCapScan) {
                long capAddress = (long) address;
                if (emulator.ReadUShort(capAddress) == 9999 && (lastAddress + 0x10) == capAddress) {
                    emulator.WriteUInt(capAddress, 50000);
                    emulator.WriteUInt(lastAddress, 50000);
                }
                lastAddress = capAddress;
            }
        }

        private static void ElementalBomb(Emulator.IEmulator emulator) {
            //if (ubElementalShift) TODO Ultimate Boss
            //    return;

            if (eleBombTurns == 0) {
                eleBombItemUsed = emulator.Battle.ItemUsed;
                if ((eleBombItemUsed >= 241 && eleBombItemUsed <= 248) || eleBombItemUsed == 250) {
                    if (emulator.Memory.PartySlot[2] < 9) {
                        Console.WriteLine("[DEBUG][Elemental Bomb] Trio party...");
                        byte player1Action = emulator.Battle.CharacterTable[0].Action;
                        byte player2Action = emulator.Battle.CharacterTable[1].Action;
                        byte player3Action = emulator.Battle.CharacterTable[2].Action;
                        if (player1Action == 24 && (player2Action == 16 || player2Action == 18 || player2Action == 208) && (player3Action == 16 || player3Action == 18 || player3Action == 208)) {
                            eleBombSlot = 0;
                            eleBombTurns = 7;
                            eleBombChange = false;
                            Console.WriteLine("[DEBUG][Elemental Bomb] Bomb slot 1.");
                        }
                        if (player2Action == 24 && (player1Action == 16 || player1Action == 18 || player1Action == 208) && (player3Action == 16 || player3Action == 18 || player3Action == 208)) {
                            eleBombSlot = 1;
                            eleBombTurns = 7;
                            eleBombChange = false;
                            Console.WriteLine("[DEBUG][Elemental Bomb] Bomb slot 2.");
                        }
                        if (player3Action == 24 && (player1Action == 16 || player1Action == 18 || player1Action == 208) && (player2Action == 16 || player2Action == 18 || player2Action == 208)) {
                            eleBombSlot = 2;
                            eleBombTurns = 7;
                            eleBombChange = false;
                            Console.WriteLine("[DEBUG][Elemental Bomb] Bomb slot 3.");
                        }
                    } else if (emulator.Memory.PartySlot[1] < 9) {
                        Console.WriteLine("[DEBUG][Elemental Bomb] Duo party...");
                        byte player1Action = emulator.Battle.CharacterTable[0].Action;
                        byte player2Action = emulator.Battle.CharacterTable[1].Action;
                        if (player1Action == 24 && (player2Action == 16 || player2Action == 18 || player2Action == 208)) {
                            eleBombSlot = 0;
                            eleBombTurns = 7;
                            eleBombChange = false;
                            Console.WriteLine("[DEBUG][Elemental Bomb] Bomb slot 1.");
                        }
                        if (player2Action == 24 && (player1Action == 16 || player1Action == 18 || player1Action == 208)) {
                            eleBombSlot = 1;
                            eleBombTurns = 7;
                            eleBombChange = false;
                            Console.WriteLine("[DEBUG][Elemental Bomb] Bomb slot 2.");
                        }
                    } else {
                        byte player1Action = emulator.Battle.CharacterTable[0].Action;
                        Console.WriteLine("[DEBUG][Elemental Bomb] Solo party...");
                        if (player1Action == 24) {
                            eleBombSlot = 0;
                            eleBombTurns = 7;
                            eleBombChange = false;
                        }
                    }
                }

                //Constants.WriteDebug("Item: " + eleBombItemUsed + " | Slot: " + eleBombSlot + " | Turns: " + eleBombTurns + " | Change: " + eleBombChange);
            } else {
                //Constants.WriteDebug("Item: " + eleBombItemUsed + " | Slot: " + eleBombSlot + " | Turns: " + eleBombTurns + " | Change: " + eleBombChange + " | Element: " + eleBombElement + " | Action: " + Core.Emulator.Battle.CharacterTable[eleBombSlot].Read("Action"));
                if (eleBombSlot >= 0) {
                    if ((emulator.Battle.CharacterTable[eleBombSlot].Action == 8 || emulator.Battle.CharacterTable[eleBombSlot].Action == 10) && !eleBombChange) {
                        eleBombChange = true;
                        if (eleBombTurns == 7) {
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

                            for (int i = 0; i < emulator.Memory.MonsterSize; i++) {
                                eleBombOldElement[i] = emulator.Battle.MonsterTable[i].Element;
                                emulator.Battle.MonsterTable[i].Element = (byte) element;
                                emulator.Battle.MonsterTable[i].Display_Element = (byte) element;
                            }

                            eleBombTurns -= 1;
                            Console.WriteLine("[DEBUG][Elemental Bomb][1] Turns left: " + eleBombTurns);
                        }
                    }

                    if (eleBombChange && (emulator.Battle.CharacterTable[eleBombSlot].Action == 0 || emulator.Battle.CharacterTable[eleBombSlot].Action == 2)) {
                        eleBombChange = false;
                        eleBombTurns -= 1;
                        Console.WriteLine("[DEBUG][Elemental Bomb][2] Turns left: " + eleBombTurns);
                        if (eleBombTurns <= 0) {
                            for (int i = 0; i < emulator.Memory.MonsterSize; i++) {
                                emulator.Battle.MonsterTable[i].Element = (byte) eleBombOldElement[i];
                                emulator.Battle.MonsterTable[i].Display_Element = (byte) eleBombOldElement[i];
                            }
                        }
                    }

                    if (emulator.Battle.CharacterTable[eleBombSlot].Action == 192) {
                        for (int i = 0; i < emulator.Memory.MonsterSize; i++) {
                            emulator.Battle.MonsterTable[i].Element = (byte) eleBombOldElement[i];
                            emulator.Battle.MonsterTable[i].Display_Element = (byte) eleBombOldElement[i];
                        }
                        eleBombChange = false;
                        eleBombTurns = 0;
                        eleBombElement = 255;
                        eleBombSlot = 255;
                        eleBombItemUsed = 255;
                    }
                }
            }
        }

        private static void MonsterHPNames(Emulator.IEmulator emulator) {
            for (int i = 0; i < emulator.Memory.MonsterSize; i++) {
                int lastX = 0;
                long hpName = emulator.GetAddress("MONSTERS_NAMES") + (i * 0x2C);
                char[] hpArray = emulator.Battle.MonsterTable[i].HP.ToString().ToCharArray();
                /*if (ultimateHP[i] > 0) {
                    hpArray = ultimateHP[i].ToString().ToCharArray();
                }*/ //TODO: ultimateBoss
                for (int x = 0; x < hpArray.Length; x++) {
                    emulator.WriteText(hpName + (x * 2), hpArray[x].ToString());
                    lastX = x;
                }
                emulator.WriteInt(hpName + ((lastX + 1) * 2), 41215);
            }
        }

        private static void EnrageMode(Emulator.IEmulator emulator, int i = 0) {
            if ((Settings.EnrageBossOnly && CheckEnrageBoss(emulator)) || Settings.EnrageMode) { //TODO Ultimate Boss
                if ((emulator.Battle.MonsterTable[i].HP <= (emulator.Battle.MonsterTable[i].MaxHP / 2)) && enragedMode[i] == 0) {
                    emulator.Battle.MonsterTable[i].AT = (ushort) Math.Round(originalMonsterStats[i, 1] * 1.1);
                    emulator.Battle.MonsterTable[i].MAT = (ushort) Math.Round(originalMonsterStats[i, 2] * 1.1);
                    emulator.Battle.MonsterTable[i].DF = (ushort) Math.Round(originalMonsterStats[i, 3] * 1.1);
                    emulator.Battle.MonsterTable[i].MDF = (ushort) Math.Round(originalMonsterStats[i, 4] * 1.1);
                    enragedMode[i] = 1;
                } else if ((emulator.Battle.MonsterTable[i].HP <= (emulator.Battle.MonsterTable[i].MaxHP / 4)) && enragedMode[i] == 1) {
                    emulator.Battle.MonsterTable[i].AT = (ushort) Math.Round(originalMonsterStats[i, 1] * 1.25);
                    emulator.Battle.MonsterTable[i].MAT = (ushort) Math.Round(originalMonsterStats[i, 2] * 1.25);
                    emulator.Battle.MonsterTable[i].DF = (ushort) Math.Round(originalMonsterStats[i, 3] * 1.25);
                    emulator.Battle.MonsterTable[i].MDF = (ushort) Math.Round(originalMonsterStats[i, 4] * 1.25);
                    enragedMode[i] = 2;
                }
            }
        }

        private static bool CheckEnrageBoss(Emulator.IEmulator emulator) {
            if (enrageBoss) {
                if (emulator.Memory.EncounterID == 384 || emulator.Memory.EncounterID == 385 || emulator.Memory.EncounterID == 386 || emulator.Memory.EncounterID == 387 || emulator.Memory.EncounterID == 388 || emulator.Memory.EncounterID == 389 || emulator.Memory.EncounterID == 390 || emulator.Memory.EncounterID == 391 || emulator.Memory.EncounterID == 392 || emulator.Memory.EncounterID == 393 || emulator.Memory.EncounterID == 394 || emulator.Memory.EncounterID == 395 || emulator.Memory.EncounterID == 396 || emulator.Memory.EncounterID == 397 || emulator.Memory.EncounterID == 398 || emulator.Memory.EncounterID == 399 || emulator.Memory.EncounterID == 400 || emulator.Memory.EncounterID == 401 || emulator.Memory.EncounterID == 402 || emulator.Memory.EncounterID == 403 || emulator.Memory.EncounterID == 408 || emulator.Memory.EncounterID == 409 || emulator.Memory.EncounterID == 410 || emulator.Memory.EncounterID == 411 || emulator.Memory.EncounterID == 412 || emulator.Memory.EncounterID == 413 || emulator.Memory.EncounterID == 414 || emulator.Memory.EncounterID == 415 || emulator.Memory.EncounterID == 416 || emulator.Memory.EncounterID == 417 || emulator.Memory.EncounterID == 418 || emulator.Memory.EncounterID == 421 || emulator.Memory.EncounterID == 422 || emulator.Memory.EncounterID == 423 || emulator.Memory.EncounterID == 430 || emulator.Memory.EncounterID == 431 || emulator.Memory.EncounterID == 432 || emulator.Memory.EncounterID == 433 || emulator.Memory.EncounterID == 434 || emulator.Memory.EncounterID == 435 || emulator.Memory.EncounterID == 436 || emulator.Memory.EncounterID == 437 || emulator.Memory.EncounterID == 438 || emulator.Memory.EncounterID == 439 || emulator.Memory.EncounterID == 442 || emulator.Memory.EncounterID == 443 || emulator.Memory.EncounterID == 444 || emulator.Memory.EncounterID == 445 || emulator.Memory.EncounterID == 446 || emulator.Memory.EncounterID == 447 || emulator.Memory.EncounterID == 448 || emulator.Memory.EncounterID == 449 || emulator.Memory.EncounterID == 489) {
                    return true;
                } else {
                    return false;
                }
            } else {
                return false;
            }
        }

        private static void NoHPDecaySoulEater(Emulator.IEmulator emulator) {
            if (!noHPDecayOnBattleEntry) {
                for (int i = 0; i < 3; i++) {
                    if (emulator.Memory.PartySlot[i] == 0) {
                        if (emulator.Battle.CharacterTable[i].HP_Regen == 246 || emulator.Battle.CharacterTable[i].HP_Regen == -10) { //Default
                            emulator.Battle.CharacterTable[i].HP_Regen = 0;
                        } else if (emulator.Battle.CharacterTable[i].HP_Regen == -3) { //Heal Ring
                            emulator.Battle.CharacterTable[i].HP_Regen = 7;
                        } else if (emulator.Battle.CharacterTable[i].HP_Regen == 256) {
                            emulator.Battle.CharacterTable[i].HP_Regen = 10;
                        } else if (emulator.Battle.CharacterTable[i].HP_Regen == 0 && emulator.ReadByte("CHAR_TABLE", 0 * 0x2C + 0x18) == 0x7D) { //Therapy Ring
                            emulator.Battle.CharacterTable[i].HP_Regen = 10;
                        }
                    }
                }
                noHPDecayOnBattleEntry = true;
            }
        }

        private static void NeverGuard(Emulator.IEmulator emulator) {
            for (int i = 0; i < 3; i++) {
                if (emulator.Memory.PartySlot[i] > 8) {
                    break;
                }
                emulator.Battle.CharacterTable[i].Guard = 0;
            }
        }

        private static void DamageTracker(Emulator.IEmulator emulator, UI.IUIControl uiControl) {
            bool partyAttacking = false;
            for (int i = 0; i < 3; i++) {
                if (emulator.Memory.PartySlot[i] < 9) {
                    byte action = emulator.Battle.CharacterTable[i].Action;
                    if (action == 24 || action == 26 || action == 136 || action == 138) {
                        partyAttacking = true;
                        dmgTrkSlot = i;
                    }
                }
            }

            if (partyAttacking/* || ubCheckDamageCycle > 0*/) {
                for (int i = 0; i < emulator.Memory.MonsterSize; i++) {
                    /*if (ultimateHP[i] > 0) { //TODO Ultimate Boss
                        if (ultimateHP[i] < dmgTrkHP[i]) {
                            dmgTrkChr[dmgTrkSlot] += dmgTrkHP[i] - ultimateHP[i];
                            dmgTrkHP[i] = ultimateHP[i];
                        } else if (ultimateHP[i] > dmgTrkHP[i]) {
                            dmgTrkHP[i] = ultimateHP[i];
                        }
                    } else {*/
                    if (emulator.Battle.MonsterTable[i].HP < dmgTrkHP[i]) {
                        dmgTrkChr[dmgTrkSlot] += dmgTrkHP[i] - emulator.Battle.MonsterTable[i].HP;
                        dmgTrkHP[i] = emulator.Battle.MonsterTable[i].HP;
                        Console.WriteLine("Damage Track: " + dmgTrkChr[0] + " / " + dmgTrkChr[1] + " / " + dmgTrkChr[2]);
                        uiControl.WriteGLog("Damage Track: " + dmgTrkChr[0] + " / " + dmgTrkChr[1] + " / " + dmgTrkChr[2]);
                    } else if (emulator.Battle.MonsterTable[i].HP > dmgTrkHP[i]) {
                        dmgTrkHP[i] = emulator.Battle.MonsterTable[i].HP;
                    }
                    //}
                }
            }
            /*Globals.SetCustomValue("Damage Tracker1", dmgTrkChr[0]); //TODO Reader Mode
            Globals.SetCustomValue("Damage Tracker2", dmgTrkChr[1]);
            Globals.SetCustomValue("Damage Tracker3", dmgTrkChr[2]);*/
        }

        private static void NoDragoonMode(Emulator.IEmulator emulator) {
            for (int slot = 0; slot < 3; slot++) {
                if (emulator.Memory.PartySlot[slot] > 8) {
                    break;
                }
                emulator.Battle.CharacterTable[slot].Dragoon = 0;
                emulator.Battle.CharacterTable[slot].SP = 0;
            }
        }

        private static void ChangeAspectRatio(Emulator.IEmulator emulator, UI.IUIControl uiControl) {
            ushort aspectRatio = 4096;
            int aspectRatioOption = Settings.AspectRatioMode;

            Console.WriteLine("[DEBUG][Aspect Ratio] " + aspectRatioOption);

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

            emulator.WriteUShort("ASPECT_RATIO", aspectRatio);

            if (Settings.AdvancedCameraMode == 1)
                emulator.WriteUShort("ADVANCED_CAMERA", aspectRatio);
        }

        private static void SoloModeBattle(Emulator.IEmulator emulator, UI.IUIControl uiControl) {
            byte soloLeader = Settings.SoloLeader;

            for (int i = 0; i < 3; i++) {
                if (i != soloLeader) {
                    if (emulator.Memory.PartySlot[i] == emulator.Memory.PartySlot[soloLeader]) {
                        soloLeader = 2;
                    }
                }
            }

            for (int i = 0; i < 3; i++) {
                if (emulator.Memory.PartySlot[i] < 9) {
                    if (i != soloLeader) {
                        emulator.Battle.CharacterTable[i].Action = 192;
                        emulator.Battle.CharacterTable[i].Pos_FB = 255;
                        emulator.Battle.CharacterTable[i].Pos_UD = 255;
                        emulator.Battle.CharacterTable[i].Pos_RL = 255;
                    } else {
                        emulator.Battle.CharacterTable[i].Pos_FB = 9;
                        emulator.Battle.CharacterTable[i].Pos_UD = 0;
                        emulator.Battle.CharacterTable[i].Pos_RL = 0;
                    }
                }
            }

            SoloDuoExit(emulator);
        }

        private static void DuoModeBattle(Emulator.IEmulator emulator, UI.IUIControl uiControl) {
            if (emulator.Memory.PartySlot[2] < 9) {
                emulator.Battle.CharacterTable[2].Action = 192;
                emulator.Battle.CharacterTable[2].Turn = 10000;
                emulator.Battle.CharacterTable[2].Pos_FB = 255;
                emulator.Battle.CharacterTable[2].Pos_UD = 255;
                emulator.Battle.CharacterTable[2].Pos_RL = 255;
                emulator.Battle.CharacterTable[0].Pos_FB = 10;
                emulator.Battle.CharacterTable[0].Pos_UD = 0;
                emulator.Battle.CharacterTable[0].Pos_RL = 251;
                emulator.Battle.CharacterTable[1].Pos_FB = 10;
                emulator.Battle.CharacterTable[1].Pos_UD = 0;
                emulator.Battle.CharacterTable[1].Pos_RL = 4;
            }

            SoloDuoExit(emulator);
        }

        private static void SoloDuoExit(Emulator.IEmulator emulator) {
            if (Settings.ReduceSoloDuoEXP) {
                for (int i = 0; i < 5; i++) {
                    emulator.WriteUShort("MONSTER_REWARDS", (ushort) Math.Ceiling((double) (emulator.ReadShort(emulator.GetAddress("MONSTER_REWARDS") + (i * 0x1A8)) * (Settings.SoloMode ? (1d / 3) : (2d / 3)))), (i * 0x1A8));
                }
            }

            if (!Settings.AlwaysAddSoloPartyMembers) {
                Settings.AddSoloPartyMembers = false;
            }
        }
    }
}