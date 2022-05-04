using Dragoon_Modifier.Core;

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
        static readonly ushort[] enrageBosses = new ushort[] {
                384,
                385,
                386,
                387,
                388,
                389,
                390,
                391,
                392,
                393,
                394,
                395,
                396,
                397,
                398,
                399,
                400,
                401,
                402,
                403,
                408,
                409,
                410,
                411,
                412,
                413,
                414,
                415,
                416,
                417,
                418,
                421,
                422,
                423,
                430,
                431,
                432,
                433,
                434,
                435,
                436,
                437,
                438,
                439,
                442,
                443,
                444,
                445,
                446,
                447,
                448,
                449,
                489
            };
        static readonly byte[] damageCapScanPattern = new byte[] { 0x0F, 0x27 };

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

        public static void Setup(LoDDict.ILoDDictionary loDDictionary) {
            Console.WriteLine("Battle detected. Loading..."); 

            firstDamageCapRemoval = false;
            lastItemUsedDamageCap = 0;

            eleBombChange = false;
            eleBombTurns = 0;
            eleBombElement = 255;
            eleBombSlot = 255;
            eleBombItemUsed = 255;

            uint tableBase = Emulator.Memory.BattleBasePoint;
            while (tableBase == Emulator.Memory.CharacterPoint || tableBase == Emulator.Memory.MonsterPoint) { // Wait until both C_Point and M_Point were set
                if (Constants.Run && Emulator.Memory.GameState != GameState.Battle) {
                    return;
                }
                Thread.Sleep(Settings.WaitDelay);
            }

            Emulator.Memory.LoadBattle();

            for (int i = 0; i < Emulator.Memory.MonsterSize; i++) {
                /*if (ultimateHP[i] > 0) { //TODO Ultimate boss
                    dmgTrkHP[i] = ultimateHP[i];
                } else {*/
                dmgTrkHP[i] = Emulator.Memory.Battle.MonsterTable[i].HP;
                //}
            }
            for (int i = 0; i < 3; i++) {
                dmgTrkChr[i] = 0;
            }
            damageTrackerOnBattleEntry = true;

            Constants.UIControl.UpdateField(Emulator.Memory.BattleValue, Emulator.Memory.EncounterID, Emulator.Memory.MapID);

            if (Settings.DualDifficulty) {
                Console.WriteLine("[DEBUG] [Dual Difficulty] Changing monster stats...");
                SwitchDualDifficulty(loDDictionary);
            }

            MonsterChanges(loDDictionary);

            CharacterChanges(loDDictionary);

            Console.WriteLine("Updating UI...");
            UpdateUI();

            if (Settings.SoloMode) {
                SoloModeBattle();
            }

            if (Settings.DuoMode) {
                DuoModeBattle();
            }

            if (Settings.NoDart != 0 && Settings.NoDart != 255) {
                NoDart.Initialize(loDDictionary, Settings.NoDart);
            }

            if (Settings.NoDragoon) {
                NoDragoonMode();
            }

            if (Settings.AspectRatio) {
                ChangeAspectRatio();
            }
        }

        public static void Run(LoDDict.ILoDDictionary loDDictionary) {
            loDDictionary.ItemScript.BattleRun(loDDictionary);

            UpdateUI();

            if (Settings.RemoveDamageCaps) {
                RemoveDamageCaps();
            }

            if (Settings.ElementalBomb) {
                ElementalBomb();
            }

            if (Settings.MonsterHPAsNames) {
                MonsterHPNames();
            }

            if (Settings.NeverGuard) {
                NeverGuard();
            }

            if (Settings.DamageTracker) {
                DamageTracker();
            }

            if (Settings.EnrageMode || Settings.EnrageBossOnly) {
                for (int i = 0; i < Emulator.Memory.MonsterSize; i++) {
                    EnrageMode(i); 
                }
            }

            foreach (var hotkey in hotkeys) {
                hotkey.Run(loDDictionary);
            }
        }

        private static void UpdateUI() {
            for (int i = 0; i < Emulator.Memory.Battle.MonsterTable.Length; i++) {
                Constants.UIControl.UpdateMonster(i, new UI.MonsterUpdate(i));
            }
            for (int i = 0; i < Emulator.Memory.Battle.CharacterTable.Length; i++) {
                Constants.UIControl.UpdateCharacter(i, new UI.CharacterUpdate(i));
            }
        }

        private static void SwitchDualDifficulty(LoDDict.ILoDDictionary loDDictionary) {
            string cwd = Directory.GetCurrentDirectory();
            string mod;

            if (bosses.Contains(Emulator.Memory.Battle.EncounterID)) {
                mod = DraMod.Settings.Mod.Equals("Hell_Mode") ? "Hard_Mode" : "US_Base";
                loDDictionary.SwapMonsters(cwd, mod);
                Console.WriteLine("[DEBUG] [Dual Difficulty] Mod selected: " + mod);
                return;
            }

            loDDictionary.SwapMonsters(cwd, DraMod.Settings.Mod);
            Console.WriteLine("[DEBUG] [Dual Difficulty] Mod selected: " + DraMod.Settings.Mod);
        }

        private static void MonsterChanges(LoDDict.ILoDDictionary LoDDict) {
            if (Settings.MonsterStatChange) {
                Console.WriteLine("Changing monster stats...");
                for (int slot = 0; slot < Emulator.Memory.Battle.MonsterTable.Length; slot++) {
                    MonsterStatChange(LoDDict, slot);
                }
            }

            if (Settings.MonsterDropChange) {
                Console.WriteLine("Changing monster drops...");
                for (int slot = 0; slot < Emulator.Memory.UniqueMonsterSize; slot++) {
                    MonsterDropChanges(LoDDict, slot);
                }
            }

            if (Settings.MonsterExpGoldChange) {
                Console.WriteLine("Changing monster exp and gold Rewards...");
                for (int slot = 0; slot < Emulator.Memory.UniqueMonsterSize; slot++) {
                    MonsterExpGoldChange(LoDDict, slot);
                }
            }
        }

        private static void MonsterStatChange(LoDDict.ILoDDictionary LoDDict, int slot) {
            ushort id = Emulator.Memory.Battle.MonsterID[slot];
            double HP = Math.Round(LoDDict.Monster[id].HP * Settings.HPMulti);

            double resup = 1;
            if (HP > 65535) {
                resup = HP / 65535;
                HP = 65535;
            }
            HP = Math.Round(HP);

            Core.Memory.Battle.Monster monster = Emulator.Memory.Battle.MonsterTable[slot];

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

        private static void MonsterDropChanges(LoDDict.ILoDDictionary LoDDict, int slot) {
            ushort id = Emulator.Memory.Battle.UniqueMonsterID[slot];
            Emulator.Memory.Battle.RewardsDropChance[slot] = LoDDict.Monster[id].DropChance;
            Emulator.Memory.Battle.RewardsItemDrop[slot] = LoDDict.Monster[id].DropItem;
        }

        private static void MonsterExpGoldChange(LoDDict.ILoDDictionary LoDDict, int slot) {
            ushort id = Emulator.Memory.Battle.UniqueMonsterID[slot];
            Emulator.Memory.Battle.RewardsExp[slot] = LoDDict.Monster[id].EXP;
            Emulator.Memory.Battle.RewardsGold[slot] = LoDDict.Monster[id].Gold;
        }

        private static void CharacterChanges(LoDDict.ILoDDictionary loDDictionary) {
            Console.WriteLine("Loading Character stats...");
            for (byte character = 0; character < 9; character++) {

                if (Settings.CharacterStatChange) {
                    Character.ChangeStats(loDDictionary, character);
                }

                if (Settings.ItemStatChange) {
                    Item.BattleEquipmentChange(loDDictionary, character);
                }
            }

            if (Settings.ItemNameDescChange && Emulator.Region == Region.NTA) { // TODO Remove Region check, when other encoding tables work.
                Console.WriteLine("Changing Usable Item names and descriptions...");
                Item.BattleItemNameDescChange(loDDictionary);
            }

            if (Settings.NoDecaySoulEater) {
                NoHPDecaySoulEater(loDDictionary);
            }

            loDDictionary.ItemScript.BattleSetup(loDDictionary);

            Console.WriteLine("Changing Character stats...");
            uint characterID = 0;
            for (int slot = 0; slot < Emulator.Memory.Battle.CharacterTable.Length; slot++){
                if (slot == 0 && Settings.NoDart != 255) {
                    characterID = Settings.NoDart;
                } else {
                    characterID = Emulator.Memory.PartySlot[slot];
                }

                Emulator.Memory.Battle.CharacterTable[slot].SetStats(characterID);
            }

            if (Settings.AdditionChange) {
                Console.WriteLine("Changing Additions...");
                foreach (var character in Emulator.Memory.Battle.CharacterTable) {
                    Addition.ResetAdditionTable(character, loDDictionary);
                }
            }
        }

        private static void RemoveDamageCaps() {
            if (!firstDamageCapRemoval) {
                Emulator.Memory.Battle.DamageCap = 50000;
                DamageCapScan();
                firstDamageCapRemoval = true;
                return;
            }

            if (lastItemUsedDamageCap != Emulator.Memory.Battle.ItemUsed) {
                lastItemUsedDamageCap = Emulator.Memory.Battle.ItemUsed;

                switch (lastItemUsedDamageCap) {
                    case int a when (a >= 0xC1 && a <= 0xCA):
                    case int b when (b >= 0xCF && b <= 0xD2):
                    case 0xD6:
                    case 0xD8:
                    case 0xDC:
                    case int c when (c >= 0xF1 && c <= 0xF8):
                    case 0xFA:
                        DamageCapScan();
                        break;
                }
            }
            foreach (var character in Emulator.Memory.Battle.CharacterTable) {
                if (character.Action == 24) {
                    DamageCapScan();
                }
            }
            foreach (var monster in Emulator.Memory.Battle.MonsterTable) {
                if (monster.Action == 28) { // Most used, not all monsters use action code 28 for item spells
                    DamageCapScan();
                }
            }
        }

        private static void DamageCapScan() {
            var damageCapScan = Emulator.DirectAccess.ScanAoB(0xA8660, 0x2A865F, damageCapScanPattern);
            long lastAddress = 0;
            foreach (var address in damageCapScan) {
                long capAddress = address;
                if ((Emulator.DirectAccess.ReadUShort(capAddress) == 9999) && ((lastAddress + 0x10) == capAddress)) {
                    Emulator.DirectAccess.WriteUInt(capAddress, 50000);
                    Emulator.DirectAccess.WriteUInt(lastAddress, 50000);
                }
                lastAddress = capAddress;
            }
        }

        private static void ElementalBomb() {
            //if (ubElementalShift) TODO Ultimate Boss
            //    return;

            if (eleBombTurns == 0) {
                eleBombItemUsed = Emulator.Memory.Battle.ItemUsed;
                if ((eleBombItemUsed >= 241 && eleBombItemUsed <= 248) || eleBombItemUsed == 250) {
                    byte player1Action;
                    byte player2Action;
                    byte player3Action;
                    switch (Emulator.Memory.Battle.CharacterTable.Length) {
                        case 3:
                            Console.WriteLine("[DEBUG][Elemental Bomb] Trio party...");
                            player1Action = Emulator.Memory.Battle.CharacterTable[0].Action;
                            player2Action = Emulator.Memory.Battle.CharacterTable[1].Action;
                            player3Action = Emulator.Memory.Battle.CharacterTable[2].Action;
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
                            break;
                        case 2:
                            Console.WriteLine("[DEBUG][Elemental Bomb] Duo party...");
                            player1Action = Emulator.Memory.Battle.CharacterTable[0].Action;
                            player2Action = Emulator.Memory.Battle.CharacterTable[1].Action;
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
                            break;
                        default:
                            player1Action = Emulator.Memory.Battle.CharacterTable[0].Action;
                            Console.WriteLine("[DEBUG][Elemental Bomb] Solo party...");
                            if (player1Action == 24) {
                                eleBombSlot = 0;
                                eleBombTurns = 7;
                                eleBombChange = false;
                            }
                            break;
                    }
                }
                Console.WriteLine($"[DEBUG] Item: {eleBombItemUsed} | Slot: {eleBombSlot} | Turns: {eleBombTurns} | Change {eleBombChange}");
                return;

            }

            Console.WriteLine($"[DEBUG] Item: {eleBombItemUsed} | Slot: {eleBombSlot} | Turns: {eleBombTurns} | Change {eleBombChange} | Element {eleBombElement} | Action {Emulator.Memory.Battle.CharacterTable[eleBombSlot].Action}");
            
            if (eleBombSlot < 0) {
                return;
            }

            if ((Emulator.Memory.Battle.CharacterTable[eleBombSlot].Action == 8 || Emulator.Memory.Battle.CharacterTable[eleBombSlot].Action == 10) && !eleBombChange) {
                eleBombChange = true;
                if (eleBombTurns == 7) {
                    byte element;
                    switch (eleBombItemUsed) {
                        case 242:
                            element = 128;
                            break;
                        case 243:
                            element = 1;
                            break;
                        case 244:
                            element = 64;
                            break;
                        case 245:
                            element = 2;
                            break;
                        case 246:
                            element = 32;
                            break;
                        case 247:
                            element = 4;
                            break;
                        case 248:
                            element = 16;
                            break;
                        case 250:
                            element = 8;
                            break;
                        default:
                            element = 0;
                            break;
                    }

                    eleBombElement = element;

                    for (int i = 0; i < Emulator.Memory.MonsterSize; i++) {
                        eleBombOldElement[i] = Emulator.Memory.Battle.MonsterTable[i].Element;
                        Emulator.Memory.Battle.MonsterTable[i].Element = element;
                        Emulator.Memory.Battle.MonsterTable[i].Display_Element = element;
                    }

                    eleBombTurns -= 1;
                    Console.WriteLine($"[DEBUG][Elemental Bomb][1] Turns left: {eleBombTurns}");
                }
            }

            if (eleBombChange && (Emulator.Memory.Battle.CharacterTable[eleBombSlot].Action == 0 || Emulator.Memory.Battle.CharacterTable[eleBombSlot].Action == 2)) {
                eleBombChange = false;
                eleBombTurns -= 1;
                Console.WriteLine($"[DEBUG][Elemental Bomb][2] Turns left: {eleBombTurns}");
                if (eleBombTurns <= 0) {
                    for (int i = 0; i < Emulator.Memory.MonsterSize; i++) {
                        Emulator.Memory.Battle.MonsterTable[i].Element = (byte) eleBombOldElement[i];
                        Emulator.Memory.Battle.MonsterTable[i].Display_Element = (byte) eleBombOldElement[i];
                    }
                }
            }

            if (Emulator.Memory.Battle.CharacterTable[eleBombSlot].Action == 192) {
                for (int i = 0; i < Emulator.Memory.MonsterSize; i++) {
                    Emulator.Memory.Battle.MonsterTable[i].Element = (byte) eleBombOldElement[i];
                    Emulator.Memory.Battle.MonsterTable[i].Display_Element = (byte) eleBombOldElement[i];
                }
                eleBombChange = false;
                eleBombTurns = 0;
                eleBombElement = 255;
                eleBombSlot = 255;
                eleBombItemUsed = 255;
            }
        }

        private static void MonsterHPNames() {
            for (int i = 0; i < Emulator.Memory.MonsterSize; i++) {
                int lastX = 0;
                long hpName = Emulator.GetAddress("MONSTER_NAMES") + (i * 0x2C);
                char[] hpArray = Emulator.Memory.Battle.MonsterTable[i].HP.ToString().ToCharArray();
                /*if (ultimateHP[i] > 0) {
                    hpArray = ultimateHP[i].ToString().ToCharArray();
                }*/ //TODO: ultimateBoss
                for (int x = 0; x < hpArray.Length; x++) {
                    Emulator.DirectAccess.WriteText(hpName + (x * 2), hpArray[x].ToString());
                    lastX = x;
                }
                Emulator.DirectAccess.WriteInt(hpName + ((lastX + 1) * 2), 41215);
            }
        }

        private static void EnrageMode(int i = 0) {
            if ((Settings.EnrageBossOnly && CheckEnrageBoss()) || Settings.EnrageMode) { //TODO Ultimate Boss
                var monster = Emulator.Memory.Battle.MonsterTable[i];
                if (enragedMode[i] == 0 && (monster.HP <= (monster.MaxHP / 2))) {
                    monster.AT = (ushort) Math.Round(monster.OG_AT * 1.1);
                    monster.MAT = (ushort) Math.Round(monster.OG_MAT * 1.1);
                    monster.DF = (ushort) Math.Round(monster.OG_DF * 1.1);
                    monster.MDF = (ushort) Math.Round(monster.OG_MDF * 1.1);
                    return;
                }

                if (enragedMode[i] == 1 && (monster.HP <= (monster.MaxHP / 4))) {
                    monster.AT = (ushort) Math.Round(monster.OG_AT * 1.25);
                    monster.MAT = (ushort) Math.Round(monster.OG_MAT * 1.25);
                    monster.DF = (ushort) Math.Round(monster.OG_DF * 1.25);
                    monster.MDF = (ushort) Math.Round(monster.OG_MDF * 1.25);
                }
            }
        }

        private static bool CheckEnrageBoss() {
            if (enrageBoss) {
                if (enrageBosses.Contains(Emulator.Memory.EncounterID)) {
                    return true;
                }
            }
            return false;
        }

        private static void NoHPDecaySoulEater(LoDDict.ILoDDictionary LoDDict) {
            if (!(Emulator.Memory.CharacterTable[0].Weapon == 7)) {
                return;
            }

            if (Settings.ItemStatChange) {
                LoDDict.IEquipment soulEater = (LoDDict.IEquipment) LoDDict.Item[7];
                Emulator.Memory.SecondaryCharacterTable[0].HP_Regen -= (sbyte) soulEater.SpecialBonusAmmount;
                return;
            }

            Emulator.Memory.SecondaryCharacterTable[0].HP_Regen += 10;
        }

        private static void NeverGuard() {
            foreach (var character in Emulator.Memory.Battle.CharacterTable) {
                character.Guard = 0;
            }
        }

        private static void DamageTracker() {
            bool partyAttacking = false;
            for (int i = 0; i < Emulator.Memory.Battle.CharacterTable.Length; i++) {
                byte action = Emulator.Memory.Battle.CharacterTable[i].Action;
                if (action == 24 || action == 26 || action == 136 || action == 138) {
                    partyAttacking = true;
                    dmgTrkSlot = i;
                }
            }

            if (partyAttacking/* || ubCheckDamageCycle > 0*/) {
                for (int i = 0; i < Emulator.Memory.MonsterSize; i++) {
                    /*if (ultimateHP[i] > 0) { //TODO Ultimate Boss
                        if (ultimateHP[i] < dmgTrkHP[i]) {
                            dmgTrkChr[dmgTrkSlot] += dmgTrkHP[i] - ultimateHP[i];
                            dmgTrkHP[i] = ultimateHP[i];
                        } else if (ultimateHP[i] > dmgTrkHP[i]) {
                            dmgTrkHP[i] = ultimateHP[i];
                        }
                    } else {*/
                    if (Emulator.Memory.Battle.MonsterTable[i].HP < dmgTrkHP[i]) {
                        dmgTrkChr[dmgTrkSlot] += dmgTrkHP[i] - Emulator.Memory.Battle.MonsterTable[i].HP;
                        dmgTrkHP[i] = Emulator.Memory.Battle.MonsterTable[i].HP;
                        Console.WriteLine($"Damage Track: {dmgTrkChr[0]} / {dmgTrkChr[1]} / {dmgTrkChr[2]}");
                        Constants.UIControl.WriteGLog($"Damage Track: {dmgTrkChr[0]} / {dmgTrkChr[1]} / {dmgTrkChr[2]}");
                    } else if (Emulator.Memory.Battle.MonsterTable[i].HP > dmgTrkHP[i]) {
                        dmgTrkHP[i] = Emulator.Memory.Battle.MonsterTable[i].HP;
                    }
                    //}
                }
            }
            /*Globals.SetCustomValue("Damage Tracker1", dmgTrkChr[0]); //TODO Reader Mode
            Globals.SetCustomValue("Damage Tracker2", dmgTrkChr[1]);
            Globals.SetCustomValue("Damage Tracker3", dmgTrkChr[2]);*/
        }

        private static void NoDragoonMode() {
            foreach (var character in Emulator.Memory.Battle.CharacterTable) {
                character.Dragoon = 0;
                character.SP = 0;
            }
        }

        private static void ChangeAspectRatio() {
            Console.WriteLine($"[DEBUG][Aspect Ratio] {Settings.AspectRatioMode}");

            ushort aspectRatio;
            switch (Settings.AspectRatioMode) {
                case 1:
                    aspectRatio = 3072;
                    break;
                case 2:
                    aspectRatio = 3413;
                    break;
                case 3:
                    aspectRatio = 2340;
                    break;
                case 4:
                    aspectRatio = 2048;
                    break;
                default:
                    aspectRatio = 4096;
                    break;
            }

            Emulator.DirectAccess.WriteUShort("ASPECT_RATIO", aspectRatio);

            if (Settings.AdvancedCameraMode == 1)
                Emulator.DirectAccess.WriteUShort("ADVANCED_CAMERA", aspectRatio);
        }

        private static void SoloModeBattle() {
            byte soloLeader = Settings.SoloLeader;

            for (int i = 0; i < 3; i++) {
                if (i != soloLeader) {
                    if (Emulator.Memory.PartySlot[i] == Emulator.Memory.PartySlot[soloLeader]) {
                        soloLeader = 2;
                    }
                }
            }

            for (int i = 0; i < 3; i++) {
                if (Emulator.Memory.PartySlot[i] < 9) {
                    if (i != soloLeader) {
                        Emulator.Memory.Battle.CharacterTable[i].Action = 192;
                        Emulator.Memory.Battle.CharacterTable[i].Pos_FB = 255;
                        Emulator.Memory.Battle.CharacterTable[i].Pos_UD = 255;
                        Emulator.Memory.Battle.CharacterTable[i].Pos_RL = 255;
                    } else {
                        Emulator.Memory.Battle.CharacterTable[i].Pos_FB = 9;
                        Emulator.Memory.Battle.CharacterTable[i].Pos_UD = 0;
                        Emulator.Memory.Battle.CharacterTable[i].Pos_RL = 0;
                    }
                }
            }

            SoloDuoExit();
        }

        private static void DuoModeBattle() {
            if (Emulator.Memory.PartySlot[2] < 9) {
                Emulator.Memory.Battle.CharacterTable[2].Action = 192;
                Emulator.Memory.Battle.CharacterTable[2].Turn = 10000;
                Emulator.Memory.Battle.CharacterTable[2].Pos_FB = 255;
                Emulator.Memory.Battle.CharacterTable[2].Pos_UD = 255;
                Emulator.Memory.Battle.CharacterTable[2].Pos_RL = 255;
                Emulator.Memory.Battle.CharacterTable[0].Pos_FB = 10;
                Emulator.Memory.Battle.CharacterTable[0].Pos_UD = 0;
                Emulator.Memory.Battle.CharacterTable[0].Pos_RL = 251;
                Emulator.Memory.Battle.CharacterTable[1].Pos_FB = 10;
                Emulator.Memory.Battle.CharacterTable[1].Pos_UD = 0;
                Emulator.Memory.Battle.CharacterTable[1].Pos_RL = 4;
            }

            SoloDuoExit();
        }

        private static void SoloDuoExit() {
            if (Settings.ReduceSoloDuoEXP) {
                for (int i = 0; i < 5; i++) {
                   Emulator.DirectAccess.WriteUShort("MONSTER_REWARDS", (ushort) Math.Ceiling((double) (Emulator.DirectAccess.ReadShort(Emulator.GetAddress("MONSTER_REWARDS") + (i * 0x1A8)) * (Settings.SoloMode ? (1d / 3) : (2d / 3)))), (i * 0x1A8));
                }
            }

            if (!Settings.AlwaysAddSoloPartyMembers) {
                Settings.AddSoloPartyMembers = false;
            }
        }
    }
}