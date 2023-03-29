using Dragoon_Modifier.Core;
using Dragoon_Modifier.DraMod.Dataset;
using Dragoon_Modifier.DraMod.UI;

using System;
using System.Collections;
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
        static ushort[] eleBombOldElement = { 0, 0, 0, 0, 0, 0 };
        //Enrage
        static byte[] enragedMode = { 0, 0, 0, 0, 0, 0 };
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
        //TB
        static int timePlayed = 0;
        static int[] extraTurnBattleC = new int[3];
        static int[] extraTurnBattleM = new int[6];
        static int[] cooldowns = new int[3];
        static ushort[] currentHP = new ushort[3];
        static int qtbCount = 2;
        static bool qtbLeaderTurn = false;
        //Ultimate Boss
        static int[] ultimateHP = new int[6];
        static int[] ultimateHPSave = new int[6];
        static int[] ultimateMaxHP = new int[6];
        static bool ubUltimateHPSet = false;
        static bool ubPartyAttacking = false;
        static ushort[] ubWHP = new ushort[3];
        static ushort[] ubWMHP = new ushort[3];
        static bool ubHealthSteal = false;
        static ushort ubHealthStealDamage = 0;
        static bool ubMoveChange = false;
        static bool ubMoveChangeSet = false;
        static ushort[] ubMoveChangeTurn = new ushort[6];
        static bool ubUltimateEnrage = false;
        static bool ubElementalShift = false;
        static double ubMagicChangeTurns = 0;
        static byte ubArmorShellTurns = 0;
        static ushort ubHeartHPSave = 0;
        static ushort ubArmorShellTP = 0;
        static bool ubSharedHP = false;

        static readonly List<Hotkey> hotkeys = BattleHotkeys.Load();

        public static void Setup() {
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
                if (Settings.Instance.RemoveHPCap) {
                    Emulator.DirectAccess.WriteShort(Emulator.GetAddress("FIELD_HP_CAP_4"), 30000);
                }
                Thread.Sleep(Settings.Instance.WaitDelay);
            }

            Emulator.Memory.LoadBattle();
            Debug.WriteLine("---------------------");
            Debug.WriteLine("Monster Point:       " + Convert.ToString(Emulator.Memory.MonsterPoint + Emulator.EmulatorOffset, 16).ToUpper());
            Debug.WriteLine("Character Point:     " + Convert.ToString(Emulator.Memory.CharacterPoint + Emulator.EmulatorOffset, 16).ToUpper());
            Debug.WriteLine("Monster Size:        " + Emulator.Memory.MonsterSize);
            Debug.WriteLine("Unique Monsters:     " + Emulator.Memory.UniqueMonsterSize);
            Debug.WriteLine("Monster IDs:         " + String.Join(", ", Emulator.Memory.Battle.MonsterID));

            if (Settings.Instance.Preset != Preset.Normal && Settings.Instance.Preset != Preset.Custom) {
                SpecialEquips.Reset();
                Emulator.Memory.BattleSPCap = 700;
            }

            ubElementalShift = false;
            ubSharedHP = false;

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

            Settings.Instance.DualDifficulty = (Settings.Instance.Preset == Preset.NormalHard || Settings.Instance.Preset == Preset.HardHell) && !bosses.Contains(Emulator.Memory.EncounterID);

            MonsterChanges();

            CharacterChanges();

            Console.WriteLine("Updating UI...");
            UpdateUI();

            if (Settings.Instance.SoloMode) {
                SoloModeBattle();
            }

            if (Settings.Instance.DuoMode) {
                DuoModeBattle();
            }

            if (Settings.Instance.NoDart != 0 && Settings.Instance.NoDart != 255) {
                NoDart.Initialize(Settings.Instance.NoDart);
            }

            if (Settings.Instance.NoDragoon) {
                NoDragoonMode();
            }

            if (Settings.Instance.AspectRatio) {
                ChangeAspectRatio();
            }

            if (Settings.Instance.BattleRows) {
                BattleRowFormations();
            }

            if (Settings.Instance.RGBBattleUI && !Settings.Instance.RGBBattleUICharacter && !Settings.Instance.RGBBattleUICycle) {
                RGBBattleStatic();
            }

            if (Settings.Instance.EATB || Settings.Instance.ATB) {
                timePlayed = Emulator.Memory.TimePlayed;
                cooldowns[0] = cooldowns[1] = cooldowns[2] = 0;
                extraTurnBattleC = new int[3];
                extraTurnBattleM = new int[6];
            }

            if (Settings.Instance.QTB) {
                int i = 0;
                foreach (var character in Emulator.Memory.Battle.CharacterTable) {
                    currentHP[i] = character.HP;
                    i++;
                }
                qtbCount = Settings.Instance.QTBCount = 2;
                qtbLeaderTurn = false;
            }

            if (Settings.Instance.UltimateBoss) {
                ubUltimateHPSet = ubPartyAttacking = false;
                if (new ushort[] { 413, 415, 403, 390 }.Contains(Emulator.Memory.EncounterID)) {
                    UltimateZeroSPStart();
                }

                if (new ushort[] { 415 }.Contains(Emulator.Memory.EncounterID)) {
                    UltimateMPAttackStart();
                }

                if (new ushort[] { 417 }.Contains(Emulator.Memory.EncounterID)) {
                    int i = 0;
                    foreach (var character in Emulator.Memory.Battle.CharacterTable) {
                        ubWHP[i] = character.HP;
                        ubWMHP[i] = character.MaxHP;
                        i++;
                    }
                }

                if (new ushort[] { 418 }.Contains(Emulator.Memory.EncounterID)) {
                    int i = 0;
                    foreach (var monster in Emulator.Memory.Battle.MonsterTable) {
                        ubMoveChangeTurn[i] = 500;
                    }
                    ubMoveChangeSet = false;
                }

                if (new ushort[] { 416 }.Contains(Emulator.Memory.EncounterID)) {
                    ubMagicChangeTurns = 0;
                    ubElementalShift = true;
                }

                if (new ushort[] { 422 }.Contains(Emulator.Memory.EncounterID)) {
                    ubArmorShellTurns = 0;
                    ubHeartHPSave = Emulator.Memory.Battle.MonsterTable[3].HP;
                }

                if (new ushort[] { 423 }.Contains(Emulator.Memory.EncounterID)) {
                    ubSharedHP = true;
                }
                /* ULTIMATE BOSS TODO

                if (new ushort[] { 393 }.Contains(Emulator.Memory.EncounterID)) {
                    UltimateBossRemoveResistances();
                }

                */
            }

            if (Settings.Instance.RemoveHPCap) {
                if (Emulator.DirectAccess.ReadShort(Emulator.GetAddress("FIELD_HP_CAP_1")) == 10000) {
                    Emulator.DirectAccess.WriteShort(Emulator.GetAddress("FIELD_HP_CAP_4"), 30000);
                }
            }
        }

        public static void Run() {
            Settings.Instance.Dataset.Script.BattleRun();

            UpdateUI();

            if (Settings.Instance.UltimateBoss) {
                //Debug.WriteLine("Attack Move: " + Emulator.Memory.Battle.MonsterTable[0].AttackMove);
                if (!ubUltimateHPSet)
                    UltimateBossPartyAttacking();
                if (ubUltimateHPSet)
                    UltimateBossWaitForDamage();
                if (new ushort[] { 412, 416 }.Contains(Emulator.Memory.EncounterID)) {
                    if (Emulator.Memory.EncounterID == 412)
                        UltimateBossHealing(0, 3);
                    //if (Emulator.Memory.EncounterID == 416)
                    //    UltimateBossHealing(0, 1);
                }
                if (new ushort[] { 414, 415 }.Contains(Emulator.Memory.EncounterID)) {
                    if (Emulator.Memory.EncounterID == 414)
                        UltimateBossGuardBreak(0, 2);
                    if (Emulator.Memory.EncounterID == 415)
                        UltimateBossGuardBreak(0, 0);
                }
                if (new ushort[] { 417 }.Contains(Emulator.Memory.EncounterID)) {
                    UltimateBossHealthSteal(0, 2);
                }
                if (new ushort[] { 417 }.Contains(Emulator.Memory.EncounterID)) {
                    UltimateBossWoundDamage(0, 0);
                }
                if (new ushort[] { 418 }.Contains(Emulator.Memory.EncounterID)) {
                    UltimateBossMoveChange(0, 2, 25);
                }
                if (new ushort[] { 418 }.Contains(Emulator.Memory.EncounterID)) {
                    if (ultimateHP[0] > 150000)
                        UltimateBossSPAttack(0, 1, 25, 231, 2);
                    else if (ultimateHP[0] > 75000)
                        UltimateBossSPAttack(0, 1, 50, 206, 2);
                    else
                        UltimateBossSPAttack(0, 1, 125, 156, 2);
                }
                if (new ushort[] { 416, 396, 390, 395, 270, 344, 421, 443 }.Contains(Emulator.Memory.EncounterID)) {
                    if (Emulator.Memory.EncounterID == 416) {
                        double hpDamage = ultimateMaxHP[0] - ultimateHP[0];
                        if ((hpDamage / ultimateMaxHP[0]) * 10 >= ubMagicChangeTurns) {
                            UltimateBossMagicChange();
                        }
                        UltimateBossElementalShift();
                    } else if (Emulator.Memory.EncounterID == 396) {
                        double hpDamage = ultimateMaxHP[0] - ultimateHP[0];
                        if ((hpDamage / ultimateMaxHP[0]) * 20 >= ubMagicChangeTurns) {
                            UltimateBossMagicChange();
                        }
                    }
                }
                if (new ushort[] { 422 }.Contains(Emulator.Memory.EncounterID)) {
                    UltimateBossArmorBreak();
                }

                if (Emulator.Memory.EncounterID == 432) {
                    Emulator.Memory.Battle.MonsterTable[0].Pos_RL = 0;
                }/* ULTIMATE BOSS TODO

                if (new ushort[] { 399 }.Contains(Emulator.Memory.EncounterID)) {
                    UltimateBossTurnPointDamage();
                }

                if (new ushort[] { 410 }.Contains(Emulator.Memory.EncounterID)) {
                    UltimateBossBodyDamage();
                }

                if (new ushort[] { 393, 397 }.Contains(Emulator.Memory.EncounterID) && ubDragonBondMode != 999) {
                    UltimateBossDragoonBond();
                }

                if (new ushort[] { 398 }.Contains(Emulator.Memory.EncounterID)) {
                    UltimateBossMenuBlock();
                }

                if (new ushort[] { 400 }.Contains(Emulator.Memory.EncounterID)) {
                    UltimateBossAccuracyLoss();
                    UltimateBossPowerDownDefense();
                    UltimateBossSpeedDown();
                }

                if (new ushort[] { 401 }.Contains(Emulator.Memory.EncounterID)) {
                    UltimateBossPowerDownAttack();
                    UltimateBossPowerDownDefense();
                    UltimateBossPowerDownFull();
                    UltimateBossElectricCharge();
                }

                if (new ushort[] { 390 }.Contains(Emulator.Memory.EncounterID)) {
                    UltimateBossInventoryRefresh();
                    UltimateBossEnrage();
                    UltimateBossEnhancedShield();
                }

                if (new ushort[] { 411 }.Contains(Emulator.Memory.EncounterID)) {
                    UltimateBossBodyProtect();
                    UltimateBossFinalAttack();
                }

                if (new ushort[] { 395 }.Contains(Emulator.Memory.EncounterID)) {
                    UltimateBossReverseDragonBlockStaff();
                    UltimateBossEnrage();
                    UltimateBossArmorGuard();
                }

                if (new ushort[] { 393 }.Contains(Emulator.Memory.EncounterID)) {
                    UltimateBossEnrage();
                    UltimateBossRemoveResistances();
                }

                if (new ushort[] { 442 }.Contains(Emulator.Memory.EncounterID)) {
                    UltimateBossEnrage();
                    UltimateBossDragoonChangeMove();
                }

                if (new ushort[] { 443 }.Contains(Emulator.Memory.EncounterID)) {
                    UltimateMelbu();
                }*/

                UltimateBossFinishCheck();
            }

            if (Settings.Instance.RemoveDamageCaps || Settings.Instance.UltimateBoss) {
                RemoveDamageCaps();
            }

            if (Settings.Instance.ElementalBomb) {
                ElementalBomb();
            }

            if (Settings.Instance.MonsterHPAsNames) {
                MonsterHPNames();
            }

            if (Settings.Instance.NeverGuard) {
                NeverGuard();
            }

            if (Settings.Instance.DamageTracker) {
                DamageTracker();
            }

            if (Settings.Instance.EnrageMode || Settings.Instance.EnrageBossOnly) {
                for (int i = 0; i < Emulator.Memory.MonsterSize; i++) {
                    EnrageMode(i); 
                }
            }

            if (Settings.Instance.RGBBattleUICycle || Settings.Instance.RGBBattleUICharacter) {
                //TODO
            }

            if (Settings.Instance.EATB || Settings.Instance.ATB) {
                EATB();
            }

            if (Settings.Instance.QTB) {
                QTB();
            }

            foreach (var hotkey in hotkeys) {
                hotkey.Run();
            }
        }

        private static void UpdateUI() {
            for (int i = 0; i < Emulator.Memory.Battle.MonsterTable.Length; i++) { 
                MonsterUpdate mon = new UI.MonsterUpdate(i);
                if (Settings.Instance.UltimateBoss) {
                    if (ultimateMaxHP[i] > 65535) {
                        mon.HP = ultimateHP[i];
                        mon.MaxHP = ultimateMaxHP[i];
                    }
                }
                Constants.UIControl.UpdateMonster(i, mon);
            }
            for (int i = 0; i < Emulator.Memory.Battle.CharacterTable.Length; i++) {
                Constants.UIControl.UpdateCharacter(i, new UI.CharacterUpdate(i));
            }
        }

        private static void MonsterChanges() {
            if (Settings.Instance.MonsterStatChange) {
                Console.WriteLine("Changing monster stats...");
                for (int slot = 0; slot < Emulator.Memory.Battle.MonsterTable.Length; slot++) {
                    MonsterStatChange(slot);
                }
            }

            if (Settings.Instance.MonsterDropChange) {
                Console.WriteLine("Changing monster drops...");
                for (int slot = 0; slot < Emulator.Memory.UniqueMonsterSize; slot++) {
                    MonsterDropChanges(slot);
                }
            }

            if (Settings.Instance.MonsterExpGoldChange) {
                Console.WriteLine("Changing monster exp and gold Rewards...");
                for (int slot = 0; slot < Emulator.Memory.UniqueMonsterSize; slot++) {
                    MonsterExpGoldChange(slot);
                }
            }
        }

        private static void MonsterStatChange(int slot) {
            ushort id = Emulator.Memory.Battle.MonsterID[slot];
            IMonster mon = Settings.Instance.UltimateBoss ? Settings.Instance.Dataset.UltimateStats[id] : Settings.Instance.Dataset.Monster[id];
            double HP = Math.Round(mon.HP * Settings.Instance.HPMulti);

            double resup = 1;
            if (HP > 65535) {
                if (!Settings.Instance.UltimateBoss)
                    resup = HP / 65535;
                HP = 65535;
            }
            HP = Math.Round(HP);

            Core.Memory.Battle.Monster monster = Emulator.Memory.Battle.MonsterTable[slot];

            if (Settings.Instance.UltimateBoss) {
                ultimateMaxHP[slot] = (int) (mon.HP * Settings.Instance.HPMulti);
                ultimateHP[slot] = ultimateMaxHP[slot];
                if (ultimateMaxHP[slot] >= 65535) {
                    monster.HP = 65534;
                }
            }

            monster.HP = (ushort) HP;
            monster.MaxHP = (ushort) HP;
            monster.AT = (ushort) Math.Round(mon.AT * Settings.Instance.ATMulti);
            monster.OG_AT = (ushort) Math.Round(mon.AT * Settings.Instance.ATMulti);
            monster.MAT = (ushort) Math.Round(mon.MAT * Settings.Instance.MATMulti);
            monster.OG_MAT = (ushort) Math.Round(mon.MAT * Settings.Instance.MATMulti);
            monster.DF = (ushort) Math.Round(mon.DF * Settings.Instance.DFMulti * resup);
            monster.OG_DF = (ushort) Math.Round(mon.DF * Settings.Instance.DFMulti * resup);
            monster.MDF = (ushort) Math.Round(mon.MDF * Settings.Instance.MDFMulti * resup);
            monster.OG_MDF = (ushort) Math.Round(mon.MDF * Settings.Instance.MDFMulti * resup);
            monster.SPD = (ushort) Math.Round(mon.SPD * Settings.Instance.SPDMulti);
            monster.OG_SPD = (ushort) Math.Round(mon.SPD * Settings.Instance.SPDMulti);
            monster.A_AV = mon.A_AV;
            monster.M_AV = mon.M_AV;
            monster.P_Immune = mon.PhysicalImmunity;
            monster.M_Immune = mon.MagicalImmunity;
            monster.P_Half = mon.PhysicalResistance;
            monster.M_Half = mon.MagicalResistance;
            monster.Element = mon.Element;
            monster.ElementalImmunity = mon.ElementalImmunity;
            monster.ElementalResistance = mon.ElementalResistance;
            monster.StatusResist = mon.StatusResist;
            monster.SpecialEffect = mon.SpecialEffect;
        }

        private static void MonsterDropChanges(int slot) {
            ushort id = Emulator.Memory.Battle.UniqueMonsterID[slot];
            IMonster mon = Settings.Instance.UltimateBoss ? Settings.Instance.Dataset.UltimateStats[id] : Settings.Instance.Dataset.Monster[id];
            Emulator.Memory.Battle.RewardsDropChance[slot] = mon.DropChance;
            Emulator.Memory.Battle.RewardsItemDrop[slot] = mon.DropItem;
        }

        private static void MonsterExpGoldChange(int slot) {
            ushort id = Emulator.Memory.Battle.UniqueMonsterID[slot];
            IMonster mon = Settings.Instance.UltimateBoss ? Settings.Instance.Dataset.UltimateStats[id] : Settings.Instance.Dataset.Monster[id];
            Emulator.Memory.Battle.RewardsExp[slot] = mon.EXP;
            Emulator.Memory.Battle.RewardsGold[slot] = mon.Gold;
        }

        private static void CharacterChanges() {
            Console.WriteLine("Loading Character stats...");
            for (byte character = 0; character < 9; character++) {

                if (Settings.Instance.CharacterStatChange) {
                    Character.ChangeStats(character);
                }

                if (Settings.Instance.ItemStatChange) {
                    Item.BattleEquipmentChange(character);
                }
            }

            if (Settings.Instance.ItemNameDescChange && Emulator.Region == Region.NTA) { // TODO Remove Region check, when other encoding tables work.
                Console.WriteLine("Changing Usable Item names and descriptions...");
                Item.BattleItemNameDescChange();
            }

            if (Settings.Instance.NoDecaySoulEater) {
                NoHPDecaySoulEater();
            }

            Console.WriteLine("Changing Character stats...");
            uint characterID = 0;
            for (int slot = 0; slot < Emulator.Memory.Battle.CharacterTable.Length; slot++){
                if (slot == 0 && Settings.Instance.NoDart != 255) {
                    characterID = Settings.Instance.NoDart;
                } else {
                    characterID = Emulator.Memory.PartySlot[slot];
                }

                Emulator.Memory.Battle.CharacterTable[slot].SetStats(characterID);
            }

            if (Settings.Instance.AdditionChange) {
                Console.WriteLine("Changing Additions...");
                foreach (var character in Emulator.Memory.Battle.CharacterTable) {
                    Addition.ResetAdditionTable(character);
                }
            }

            if (Settings.Instance.DragoonAdditionChange) {
                Console.WriteLine("Changing Dragoon Additions...");
                for (int slot = 0; slot < Emulator.Memory.Battle.CharacterTable.Length; slot++) {
                    DragoonAddition.WriteDragoonAdditionTable(slot, Emulator.Memory.PartySlot[slot]);
                }
            }

            if (Settings.Instance.DragoonSpellChange) {
                Console.WriteLine("Changing Dragoon Spells...");
                for (int slot = 0; slot < Emulator.Memory.Battle.CharacterTable.Length; slot++) {
                    DragoonSpell.WriteDragoonSpellTable(slot, Emulator.Memory.PartySlot[slot]);
                }
            }

            if (Settings.Instance.DragoonDescriptionChange && Emulator.Region == Region.NTA) { // TODO Remove Region check, when other encoding tables work.
                Console.WriteLine("Changing Dragoon Spell Descriptions...");
                long address = Emulator.GetAddress("DRAGOON_DESC_PTR");
                long descrOffset = 0;

                Dictionary<int, IDragoonSpells> datasetSpell = Settings.Instance.Dataset.DragoonSpell;

                for (int i = 0; i < datasetSpell.Count; i++) {
                    Emulator.DirectAccess.WriteUInt(address + i * 0x4, (uint) datasetSpell[i].Description_Pointer);
                    Emulator.DirectAccess.WriteByte(address + i * 0x4 + 0x3, 0x80);
                    Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_DESC") + descrOffset, datasetSpell[i].Encoded_Description);
                    descrOffset += datasetSpell[i].Encoded_Description.Length;
                }

            }

            if (Settings.Instance.DragoonStatChange) {
                Console.WriteLine("Changing Dragoon Stats...");
                for (int slot = 0; slot < Emulator.Memory.Battle.CharacterTable.Length; slot++) {
                    DragoonStat.WriteDragoonStatTable(slot, Emulator.Memory.Battle.CharacterTable[slot]);
                }
            }

            Settings.Instance.Dataset.Script.BattleSetup();
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
                long capAddress = (long) address;
                if ((Emulator.DirectAccess.ReadUShort(capAddress) == 9999) && ((lastAddress + 0x10) == capAddress)) {
                    Emulator.DirectAccess.WriteUInt(capAddress, 50000);
                    Emulator.DirectAccess.WriteUInt(lastAddress, 50000);
                }
                lastAddress = capAddress;
            }
        }

        private static void ElementalBomb() {
            if (ubElementalShift)
                return;

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
            if (((Settings.Instance.EnrageBossOnly && CheckEnrageBoss()) || Settings.Instance.EnrageMode) && !ubUltimateEnrage) {
                var monster = Emulator.Memory.Battle.MonsterTable[i];
                if (enragedMode[i] == 0 && (monster.HP <= (monster.MaxHP / 2))) {
                    monster.AT = (ushort) Math.Round(monster.OG_AT * 1.1);
                    monster.MAT = (ushort) Math.Round(monster.OG_MAT * 1.1);
                    monster.DF = (ushort) Math.Round(monster.OG_DF * 1.1);
                    monster.MDF = (ushort) Math.Round(monster.OG_MDF * 1.1);
                    enragedMode[i] = 1;
                }

                if (enragedMode[i] == 1 && (monster.HP <= (monster.MaxHP / 4))) {
                    monster.AT = (ushort) Math.Round(monster.OG_AT * 1.25);
                    monster.MAT = (ushort) Math.Round(monster.OG_MAT * 1.25);
                    monster.DF = (ushort) Math.Round(monster.OG_DF * 1.25);
                    monster.MDF = (ushort) Math.Round(monster.OG_MDF * 1.25);
                    enragedMode[i] = 2;
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

        private static bool CheckEnrage() {
            if (((Settings.Instance.EnrageBossOnly && CheckEnrageBoss()) || Settings.Instance.EnrageMode) && !ubUltimateEnrage)
                return true;
            return false;
        }

        private static void NoHPDecaySoulEater() {
            if (!(Emulator.Memory.CharacterTable[0].Weapon == 7)) {
                return;
            }

            if (Settings.Instance.ItemStatChange) {
                Dataset.IEquipment soulEater = (Dataset.IEquipment) Settings.Instance.Dataset.Item[7];
                Emulator.Memory.SecondaryCharacterTable[0].HP_Regen -= (sbyte) soulEater.SpecialBonusAmount;
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
            try {
                for (int i = 0; i < Emulator.Memory.Battle.CharacterTable.Length; i++) {
                    byte action = Emulator.Memory.Battle.CharacterTable[i].Action;
                    if (action == 24 || action == 26 || action == 136 || action == 138) {
                        dmgTrkSlot = i;
                    }
                }

                for (int i = 0; i < Emulator.Memory.MonsterSize; i++) {
                    if (ultimateHP[i] > 0) {
                        if (ultimateHP[i] < dmgTrkHP[i]) {
                            dmgTrkChr[dmgTrkSlot] += dmgTrkHP[i] - ultimateHP[i];
                            dmgTrkHP[i] = ultimateHP[i];
                            Console.WriteLine($"Damage Track: {dmgTrkChr[0]} / {dmgTrkChr[1]} / {dmgTrkChr[2]}");
                            Constants.UIControl.WriteGLog($"Damage Track: {dmgTrkChr[0]} / {dmgTrkChr[1]} / {dmgTrkChr[2]}");
                        } else if (ultimateHP[i] > dmgTrkHP[i]) {
                            dmgTrkHP[i] = ultimateHP[i];
                        }
                    } else {
                        if (Emulator.Memory.Battle.MonsterTable[i].HP < dmgTrkHP[i]) {
                            dmgTrkChr[dmgTrkSlot] += dmgTrkHP[i] - Emulator.Memory.Battle.MonsterTable[i].HP;
                            dmgTrkHP[i] = Emulator.Memory.Battle.MonsterTable[i].HP;
                            Console.WriteLine($"Damage Track: {dmgTrkChr[0]} / {dmgTrkChr[1]} / {dmgTrkChr[2]}");
                            Constants.UIControl.WriteGLog($"Damage Track: {dmgTrkChr[0]} / {dmgTrkChr[1]} / {dmgTrkChr[2]}");
                        } else if (Emulator.Memory.Battle.MonsterTable[i].HP > dmgTrkHP[i]) {
                            dmgTrkHP[i] = Emulator.Memory.Battle.MonsterTable[i].HP;
                        }
                    }
                }
            } catch (Exception ex) { 
                //Used for battle cutscenes
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
            Console.WriteLine($"[DEBUG][Aspect Ratio] {Settings.Instance.AspectRatioMode}");

            ushort aspectRatio;
            switch (Settings.Instance.AspectRatioMode) {
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

            if (Settings.Instance.AdvancedCameraMode == 1)
                Emulator.DirectAccess.WriteUShort("ADVANCED_CAMERA", aspectRatio);
        }

        private static void RemoveHPCap() {
            if (Constants.Run && Emulator.Memory.GameState != GameState.Battle) {
                return;
            }
            Debug.WriteLine("Removing cap...");

            
           
        }

        private static void SoloModeBattle() {
            byte soloLeader = Settings.Instance.SoloLeader;

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
            if (Settings.Instance.ReduceSoloDuoEXP) {
                for (int i = 0; i < 5; i++) {
                   Emulator.DirectAccess.WriteUShort("MONSTER_REWARDS", (ushort) Math.Ceiling((double) (Emulator.DirectAccess.ReadShort(Emulator.GetAddress("MONSTER_REWARDS") + (i * 0x1A8)) * (Settings.Instance.SoloMode ? (1d / 3) : (2d / 3)))), (i * 0x1A8));
                }
            }

            if (!Settings.Instance.AlwaysAddSoloPartyMembers) {
                Settings.Instance.AddSoloPartyMembers = false;
            }
        }

        public static void BattleRowFormations() {
            foreach (var character in Emulator.Memory.Battle.CharacterTable) {
                Console.WriteLine("[DEBUG][ROWS]");
                Console.WriteLine("[DEBUG][ROWS] CID: " + (int) character.ID);
                int rowType = Settings.Instance.BattleRowsFormation[(int) character.ID];
                int boostType = Settings.Instance.BattleRowsBoost[(int) character.ID];
                double attackBoost = 0, magicBoost = 0, defenseBoost = 0;
                Console.WriteLine("[DEBUG][ROWS] Row: " + rowType);
                Console.WriteLine("[DEBUG][ROWS] Boost: " + boostType);

                if (rowType == 0) { //Stay
                    if (boostType == 0) {
                        attackBoost = 1;
                        magicBoost = 1;
                        defenseBoost = 1;
                    } else if (boostType == 1) {
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


                Console.WriteLine("[DEBUG][ROWS] AT:  " + character.AT + " / " + Math.Round(character.AT * attackBoost) + " / " + attackBoost);
                Console.WriteLine("[DEBUG][ROWS] MAT: " + character.MAT + " / " + Math.Round(character.MAT * magicBoost) + " / " + magicBoost);
                Console.WriteLine("[DEBUG][ROWS] DF:  " + character.DF + " / " + Math.Round(character.DF * defenseBoost) + " / " + defenseBoost);
                Console.WriteLine("[DEBUG][ROWS] MDF: " + character.MDF + " / " + Math.Round(character.MDF * defenseBoost) + " / " + defenseBoost);

                
                character.AT = (ushort) Math.Round(character.AT * attackBoost);
                character.MAT = (ushort) Math.Round(character.MAT * magicBoost);
                character.DF = (ushort) Math.Round(character.DF * defenseBoost);
                character.MDF = (ushort) Math.Round(character.MDF * defenseBoost);

                if (rowType == 1) {
                    character.Pos_FB = 5;
                } else if (rowType == 2) {
                    character.Pos_FB = 13;
                }
            }
        }

        public static void RGBBattleStatic() {
            Emulator.DirectAccess.WriteByte(Emulator.Memory.Battle.BattleUIColour, Settings.Instance.RGBBattleUIColour[0]);
            Emulator.DirectAccess.WriteByte(Emulator.Memory.Battle.BattleUIColour + 1, Settings.Instance.RGBBattleUIColour[1]);
            Emulator.DirectAccess.WriteByte(Emulator.Memory.Battle.BattleUIColour + 2, Settings.Instance.RGBBattleUIColour[2]);
        }

        public static void EATB() {
            if (timePlayed + 60 < Emulator.Memory.TimePlayed) {
                timePlayed = Emulator.Memory.TimePlayed;

                int i = 0;
                foreach (var character in Emulator.Memory.Battle.CharacterTable) {
                    if (Constants.UIControl.GetCTB(i) < extraTurnBattleC[i]) { //Hotkey used
                        extraTurnBattleC[i] = 0;
                        if (Settings.Instance.EATB) {
                            cooldowns[i] += 90;
                        } else {
                            foreach (var monster in Emulator.Memory.Battle.MonsterTable) {
                                monster.Turn += 50;
                            }
                        }
                    }

                    if (character.HP > 0) {
                        if (cooldowns[i] > 0) {
                            cooldowns[i] -= 1;
                            extraTurnBattleC[i] += character.SPD / 2;
                        } else {
                            extraTurnBattleC[i] += character.SPD;
                        }
                    }

                    extraTurnBattleC[i] = Math.Min(6000, extraTurnBattleC[i]);
                    Constants.UIControl.UpdateCTB(extraTurnBattleC[i], i);
                    i++;
                }

                i = 0;
                foreach (var monster in Emulator.Memory.Battle.MonsterTable) {
                    if (monster.HP > 0) {
                        if (new ushort[] { 386, 390 }.Contains(Emulator.Memory.EncounterID) && i == 1) 
                            break;
                        if (Emulator.Memory.EncounterID == 433 && i == 1)
                            if (Emulator.Memory.Battle.MonsterTable[0].HP > 0)
                                break;
                        if (Emulator.Memory.EncounterID == 433 && i == 2)
                            if (Emulator.Memory.Battle.MonsterTable[1].HP > 0)
                                break;
                        if (Emulator.Memory.EncounterID == 433 && i >= 1)
                            if (Emulator.Memory.Battle.MonsterTable[i - 1].HP > 0)
                                continue;

                        extraTurnBattleM[i] += monster.SPD;
                        if (extraTurnBattleM[i] > 7000 + (1000 * i)) {
                            extraTurnBattleM[i] = 0;
                            monster.Turn += 255;
                        }

                        Constants.UIControl.UpdateMTB(extraTurnBattleM[i], i);

                        if (new ushort[] { 385, 415, 412, 417, 394, 422, 432, 443 }.Contains(Emulator.Memory.EncounterID))
                            break;

                        i++;
                    }
                }
            }
        }

        public static void QTB() {
            byte partyMembersAttacked = 0;
            int i = 0;
            int qtbFromBar = Constants.UIControl.GetQTB();

            if (qtbCount < qtbFromBar) //Hotkey used
                qtbCount = qtbFromBar;

            foreach (var character in Emulator.Memory.Battle.CharacterTable) {
                if (character.HP > 0) {
                    if (character.HP < currentHP[i]) {
                        partyMembersAttacked++;
                        currentHP[i] = character.HP;
                    } else {
                        if (character.ID == Settings.Instance.QTBLeader) {
                            AddQTB();
                        }
                        currentHP[i] = character.HP;
                    }

                    if (character.ID == Settings.Instance.QTBLeader) {
                        if (character.Action == 8 || character.Action == 10) {
                            if (!qtbLeaderTurn)
                                AddQTB();
                            qtbLeaderTurn = true;
                        } else {
                            qtbLeaderTurn = false;
                        }
                    }
                }

                if (partyMembersAttacked > 1) {
                    if (!Settings.Instance.QTBUsedDuringEnemyTurn) {
                        AddQTB(2);
                    } else {
                        AddQTB();
                        Settings.Instance.QTBUsedDuringEnemyTurn = false;
                    }
                } else if (partyMembersAttacked == 1) {
                    if (!Settings.Instance.QTBUsedDuringEnemyTurn) {
                        AddQTB();
                    } else {
                        Settings.Instance.QTBUsedDuringEnemyTurn = false;
                    }
                }
                i++;
            }
        }

        public static void AddQTB(int value = 1) {
            qtbCount = Math.Min(qtbCount + value, 4);
            Constants.UIControl.UpdateQTB(qtbCount);
        }

        public static void UltimateBossPartyAttacking() {
            bool guardCheck = false;

            int i = 0;
            int guardSlot = 0;
            string actionBuilder = " / ";
            foreach (var character in Emulator.Memory.Battle.CharacterTable) {
                actionBuilder += character.Action + " / ";
                if (new byte[] { 24, 26, 136, 138 }.Contains(character.Action)) {
                    ubPartyAttacking = true;

                    if (character.Action == 136) {
                        guardCheck = true;
                        guardSlot = i;
                    }
                }
                i++;
            }

            foreach (var character in Emulator.Memory.Battle.CharacterTable) {
                if (guardCheck && (Emulator.Memory.PartySlot[guardSlot] != 2 && Emulator.Memory.PartySlot[guardSlot] != 8)) {
                    if (new byte[] { 0, 2 }.Contains(character.Action)) {
                        ubPartyAttacking = false;
                    }
                }

                if (new byte[] { 8, 10 }.Contains(character.Action)) {
                    ubPartyAttacking = false;
                }
            }

            if (!ubPartyAttacking) {
                i = 0;
                foreach (var monster in Emulator.Memory.Battle.MonsterTable) {
                    if (ultimateMaxHP[i] >= 65535 && ultimateHP[i] > 0) {
                        if (Emulator.Memory.EncounterID == 416) {
                            UltimateBossHealing(0, 1);
                        }
                        monster.HP = (ushort) Math.Round(((double) ultimateHP[i] / ultimateMaxHP[i]) * 65534);
                        //Debug.WriteLine($"[NA] Ultimate Boss HP Slot {i}: {monster.HP}/{monster.MaxHP} - {ultimateHP[i]}/{ultimateMaxHP[i]}");
                    }
                    i++;
                }
                ubUltimateHPSet = false;
            }


            if (ubPartyAttacking && !ubUltimateHPSet) {
                i = 0;
                foreach (var monster in Emulator.Memory.Battle.MonsterTable) {
                    if (ultimateMaxHP[i] >= 65535 && ultimateHP[i] > 0) {
                        monster.HP = 65534;
                        //Debug.WriteLine($"[PA] Ultimate Boss HP Slot {i}: {monster.HP}/{monster.MaxHP} - {ultimateHP[i]}/{ultimateMaxHP[i]}");
                    }
                    i++;
                }
                ubUltimateHPSet = true;
            }

            //Debug.WriteLine($"[PC] Ultimate Boss: Party Attack: {ubPartyAttacking} - HP Set: {ubUltimateHPSet} - Action: " + actionBuilder);
        }

        public static void UltimateBossWaitForDamage() {
            UltimateBossDamageCheck();

            if (ubPartyAttacking) {
                string actionBuilder = " / ";
                bool waitForDamage = false;
                foreach (var character in Emulator.Memory.Battle.CharacterTable) {
                    actionBuilder += character.Action + " / ";
                    if (new byte[] { 0, 2, 8, 10 }.Contains(character.Action)) {
                        waitForDamage = true;
                    }
                }

                if (waitForDamage) {
                    //Debug.WriteLine($"[WW] Ultimate Boss: Party Attack: {ubPartyAttacking} - HP Set: {ubUltimateHPSet} - W4D: {waitForDamage} - Action: " + actionBuilder);
                    ubPartyAttacking = false;
                    ubUltimateHPSet = false;
                    Thread.Sleep(500);
                    UltimateBossDamageCheck();
                }

                //Debug.WriteLine($"[WD] Ultimate Boss: Party Attack: {ubPartyAttacking} - HP Set: {ubUltimateHPSet} - W4D: {waitForDamage} - Action: " + actionBuilder);
            }
        }

        public static void UltimateBossDamageCheck() {
            int i = 0;
            int totalDamage = 0;
            foreach (var monster in Emulator.Memory.Battle.MonsterTable) {
                if (monster.HP < 65534 && ultimateHP[i] > 0 && ultimateMaxHP[i] >= 65535) {
                    int damage = 65534 - monster.HP;
                    if (!ubSharedHP) {
                        ultimateHP[i] = Math.Max(0, ultimateHP[i] - damage);

                        if (ultimateHP[i] == 0)
                            monster.HP = 1;
                        else
                            monster.HP = 65534;
                        //Debug.WriteLine($"[DC] Ultimate Boss HP Slot {i}: {monster.HP}/{monster.MaxHP} - {ultimateHP[i]}/{ultimateMaxHP[i]}");

                    } else {
                        totalDamage += damage;
                    }
                    ubPartyAttacking = ubUltimateHPSet = false;
                }
                i++;
            }

            if (ubSharedHP) {
                i = 0;
                foreach (var monster in Emulator.Memory.Battle.MonsterTable) {
                    if (monster.HP < 65534 && ultimateHP[i] > 0 && ultimateMaxHP[i] >= 65535) {
                        ultimateHP[i] = Math.Max(0, ultimateHP[i] - totalDamage);

                        if (ultimateHP[i] == 0)
                            monster.HP = 1;
                        else
                            monster.HP = 65534;
                        i++;
                    }
                }
            }
        }

        public static void UltimateBossHealing(int slot, byte attack) {
            if (Emulator.Memory.Battle.MonsterTable[slot].AttackMove == attack) {
                if (Emulator.Memory.EncounterID == 412) {
                    ultimateHP[slot] = Math.Min(ultimateMaxHP[slot], ultimateHP[slot] + 44400);
                    Console.WriteLine("Drake the Bandit healed 44,400 HP with a healing potion.");
                } else if (Emulator.Memory.EncounterID == 416) {
                    if (Emulator.Memory.Battle.MonsterTable[slot].HP > (ushort) Math.Round(((double) ultimateHP[slot] / ultimateMaxHP[slot]) * 65534)) {
                        ultimateHP[slot] = Math.Min(ultimateMaxHP[slot], ultimateHP[slot] + 101250);
                        Console.WriteLine("Grand Jewel healed 101,250 HP with healing magic.");
                    } else {
                        return;
                    }
                }
                Thread.Sleep(1500);
                Emulator.Memory.Battle.MonsterTable[slot].AttackMove = 255;
            }
        }

        public static void UltimateBossGuardBreak(int slot, byte attack) {
            if (Emulator.Memory.Battle.MonsterTable[slot].AttackMove == attack) {
                foreach (var character in Emulator.Memory.Battle.CharacterTable) {
                    character.Guard = 0;
                }
                if (Emulator.Memory.EncounterID == 415) 
                    Console.WriteLine("Fire Bird initiated a Guard Break attack.");
                Thread.Sleep(1500);
                Emulator.Memory.Battle.MonsterTable[slot].AttackMove = 255;
            }
        }

        public static void UltimateBossHealthSteal(int slot, byte attack) {
            if (Emulator.Memory.Battle.MonsterTable[slot].AttackMove == attack) {
                if (ubHealthSteal) {
                    ushort dmg = Emulator.Memory.Battle.DamageSlot;
                    if (dmg <= 50000 && dmg != ubHealthStealDamage) {
                        ubHealthStealDamage = dmg;
                        ultimateHP[slot] += dmg;
                        ubHealthSteal = false;

                        if (Emulator.Memory.EncounterID == 417)
                            Console.WriteLine("Ghost Commander initiated a Health Steal attack.");
                    }
                } else {
                    ubHealthStealDamage = Emulator.Memory.Battle.DamageSlot; ;
                    ubHealthSteal = true;
                }
            } else {
                ubHealthSteal = false;
            }
        }

        public static void UltimateBossWoundDamage(int slot, byte attack) {
            int i = 0;
            foreach (var character in Emulator.Memory.Battle.CharacterTable) {
                ushort hp = character.HP;
                if (hp < ubWHP[i] && Emulator.Memory.Battle.MonsterTable[slot].AttackMove == attack) {
                    ushort woundDamage = (ushort) (ubWHP[i] - hp);
                    character.MaxHP = (ushort) Math.Max(0, character.MaxHP - woundDamage);
                    if (character.MaxHP == 0 && character.HP > 0)
                        character.HP = 1;

                    if (Emulator.Memory.EncounterID == 417)
                        Console.WriteLine("Ghost Commander initiated a Wound Damage attack.");
                }
                ubWHP[i] = hp;
                if (character.Action == 192)
                    character.MaxHP = ubWMHP[i];
                i++;
            }
        }

        public static void UltimateBossMoveChange(int slot, byte attack, int chance) {
            var monster = Emulator.Memory.Battle.MonsterTable[slot];
            ushort turn = Emulator.Memory.Battle.MonsterTable[slot].Turn;
            if (ubMoveChangeSet) {
                bool partyAttacking = false;
                int i = 0;
                foreach (var character in Emulator.Memory.Battle.CharacterTable) {
                    if (new byte[] { 8, 10, 24, 26, 136, 138 }.Contains(character.Action))
                        partyAttacking = true;
                }

                if (!partyAttacking) {
                    monster.AttackMove = attack;
                    Debug.WriteLine("Move changed.");
                } else {
                    ubMoveChangeSet = false;
                    monster.AT = monster.OG_AT;
                    enragedMode[slot] = 0;
                    Debug.WriteLine("Move change done, resetting stats.");
                }
            } else {
                if (turn < ubMoveChangeTurn[slot]) {
                    if (new Random().Next(0, 100) < chance) {
                        ubMoveChangeSet = true;
                        if (Emulator.Memory.EncounterID == 418) {
                            monster.AT = (ushort) Math.Round(monster.AT * 1.75);
                        }
                        Debug.WriteLine("Move change chance successful.");
                    } else {
                        Debug.WriteLine("Move change chance not successful.");
                    }
                }
                ubMoveChangeTurn[slot] = turn;
            }
        }

        public static void UltimateBossSPAttack(int slot, byte attack, short spAmount, byte spOnHit, byte turns) {
            var monster = Emulator.Memory.Battle.MonsterTable[slot];
            if (monster.AttackMove == attack) {
                int i = 0;
                foreach (var character in Emulator.Memory.Battle.CharacterTable) {
                    if (character.DragoonTurns == 0) {
                        if (spOnHit > 0) {
                            character.SP_P_Hit_Increase = spOnHit;
                            character.SP_M_Hit_Increase = spOnHit;
                            character.SP_P_Hit_Increase_Turn = turns;
                            character.SP_M_Hit_Increase_Turn = turns;
                        }
                        short sp = (short) (character.SP - spAmount);
                        character.SP = (ushort) (sp > 0 ? sp : 0);
                    }
                    Thread.Sleep(1500);
                    monster.AttackMove = 255;
                }
            }
        }

        public static void UltimateBossMagicChange() {
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

            if (Emulator.Memory.EncounterID == 416) {
                index = new Random().Next(0, singleMagic.Count);
                Emulator.DirectAccess.WriteByte(Emulator.Memory.Battle.MonsterPoint - 0xF24, Convert.ToByte(singleMagic[index]));
                singleMagic.RemoveAt(index);
                index = new Random().Next(0, singleMagic.Count);
                Emulator.DirectAccess.WriteByte(Emulator.Memory.Battle.MonsterPoint - 0xEE4, Convert.ToByte(singleMagic[index]));
                singleMagic.RemoveAt(index);
                index = new Random().Next(0, singleMagic.Count);
                Emulator.DirectAccess.WriteByte(Emulator.Memory.Battle.MonsterPoint - 0xEA4, Convert.ToByte(singleMagic[index]));
                singleMagic.RemoveAt(index);


                index = new Random().Next(0, wideMagic.Count);
                Emulator.DirectAccess.WriteByte(Emulator.Memory.Battle.MonsterPoint - 0xE64, Convert.ToByte(wideMagic[index]));
                wideMagic.RemoveAt(index);
                index = new Random().Next(0, wideMagic.Count);
                Emulator.DirectAccess.WriteByte(Emulator.Memory.Battle.MonsterPoint - 0xE24, Convert.ToByte(wideMagic[index]));
                wideMagic.RemoveAt(index);


                index = new Random().Next(0, powerMagic.Count);
                Emulator.DirectAccess.WriteByte(Emulator.Memory.Battle.MonsterPoint - 0xDE4, Convert.ToByte(powerMagic[index]));
                powerMagic.RemoveAt(index);
            } else if (Emulator.Memory.EncounterID == 396) {
                index = new Random().Next(0, singleMagic.Count);
                Emulator.DirectAccess.WriteByte(Emulator.Memory.Battle.MonsterPoint - 0xB14, Convert.ToByte(singleMagic[index]));
                singleMagic.RemoveAt(index);
                index = new Random().Next(0, singleMagic.Count);
                Emulator.DirectAccess.WriteByte(Emulator.Memory.Battle.MonsterPoint - 0x9E8, Convert.ToByte(singleMagic[index]));
                singleMagic.RemoveAt(index);

                index = new Random().Next(0, wideMagic.Count);
                Emulator.DirectAccess.WriteByte(Emulator.Memory.Battle.MonsterPoint - 0xAB0, Convert.ToByte(wideMagic[index]));
                wideMagic.RemoveAt(index);
                index = new Random().Next(0, wideMagic.Count);
                Emulator.DirectAccess.WriteByte(Emulator.Memory.Battle.MonsterPoint - 0xA4C, Convert.ToByte(wideMagic[index]));
                wideMagic.RemoveAt(index);
            }

            ubMagicChangeTurns += 1;
            Debug.WriteLine("Magic Changed.");
        }

        public static void UltimateBossElementalShift() {
            int lastItem = Emulator.Memory.Battle.ItemUsed;
            if (lastItem == 0xC0 || lastItem == 0xC3 || lastItem == 0xD1 || lastItem == 0xF2) {
                Emulator.Memory.Battle.MonsterTable[0].Element = 128;
                Emulator.Memory.Battle.MonsterTable[0].Display_Element = 128;
            } else if (lastItem == 0xC6 || lastItem == 0xD6 || lastItem == 0xF3) {
                Emulator.Memory.Battle.MonsterTable[0].Element = 1;
                Emulator.Memory.Battle.MonsterTable[0].Display_Element = 1;
            } else if (lastItem == 0xC7 || lastItem == 0xDC || lastItem == 0xF4) {
                Emulator.Memory.Battle.MonsterTable[0].Element = 64;
                Emulator.Memory.Battle.MonsterTable[0].Display_Element = 64;
            } else if (lastItem == 0xC5 || lastItem == 0xD0 || lastItem == 0xF5) {
                Emulator.Memory.Battle.MonsterTable[0].Element = 2;
                Emulator.Memory.Battle.MonsterTable[0].Display_Element = 2;
            } else if (lastItem == 0xCA || lastItem == 0xD8 || lastItem == 0xF7) {
                Emulator.Memory.Battle.MonsterTable[0].Element = 4;
                Emulator.Memory.Battle.MonsterTable[0].Display_Element = 4;
            } else if (lastItem == 0xC9 || lastItem == 0xD2 || lastItem == 0xF6) {
                Emulator.Memory.Battle.MonsterTable[0].Element = 32;
                Emulator.Memory.Battle.MonsterTable[0].Display_Element = 32;
            } else if (lastItem == 0xC2 || lastItem == 0xCF || lastItem == 0xF8) {
                Emulator.Memory.Battle.MonsterTable[0].Element = 16;
                Emulator.Memory.Battle.MonsterTable[0].Display_Element = 16;
            } else if (lastItem == 0xC1 || lastItem == 0xF1) {
                Emulator.Memory.Battle.MonsterTable[0].Element = 8;
                Emulator.Memory.Battle.MonsterTable[0].Display_Element = 8;
            }
        }

        public static void UltimateBossArmorBreak() {
            if (Emulator.Memory.Battle.MonsterTable[3].HP != ubHeartHPSave) {
                ubHeartHPSave = Emulator.Memory.Battle.MonsterTable[3].HP;
                ubArmorShellTurns = 1;
                ubArmorShellTP = Emulator.Memory.Battle.MonsterTable[0].Turn;
            }

            if (ubArmorShellTurns >= 1) {
                Emulator.Memory.Battle.MonsterTable[0].DF = 36;
                Emulator.Memory.Battle.MonsterTable[0].MDF = 36;

                if (Emulator.Memory.Battle.MonsterTable[0].Turn < ubArmorShellTP) {
                    ubArmorShellTurns += 1;
                }
                ubArmorShellTP = Emulator.Memory.Battle.MonsterTable[0].Turn;
            }

            if (ubArmorShellTurns > 3) {
                Emulator.Memory.Battle.MonsterTable[0].DF = 0;
                Emulator.Memory.Battle.MonsterTable[0].MDF = 0;
                ubArmorShellTurns = 0;
            }
        }

        public static void UltimateZeroSPStart() {
            foreach (var character in Emulator.Memory.Battle.CharacterTable) {
                character.SP = 0;
            }
            Console.WriteLine("Ultimate Boss Zero SP Start.");
        }

        public static void UltimateMPAttackStart() {
            foreach (var character in Emulator.Memory.Battle.CharacterTable) {
                if (Emulator.Memory.EncounterID == 415) {
                    character.MP_M_Hit_Increase = 246;
                    character.MP_P_Hit_Increase = 246;
                    character.MP_M_Hit_Increase_Turn = 255;
                    character.MP_P_Hit_Increase_Turn = 255;
                }
            }
            Console.WriteLine("Ultimate Boss MP Attack Start.");
        }

        public static void UltimateBossFinishCheck() {
            bool finishCheck = true;
            if (new ushort[] { 387, 415, 403, 402, 422, 432 }.Contains(Emulator.Memory.EncounterID)) {
                if (Emulator.Memory.Battle.MonsterTable[0].HP > 0)
                    finishCheck = false;
            } else if (new ushort[] { 449, 448 }.Contains(Emulator.Memory.EncounterID)) {
                if (Emulator.Memory.Battle.MonsterTable[1].HP > 0)
                    finishCheck = false;
            } else {
                foreach (var monster in Emulator.Memory.Battle.MonsterTable) {
                    if (monster.HP > 0)
                        finishCheck = false;
                }
            }

            if (finishCheck) {
                Console.WriteLine("Ultimate Boss Defeated.");
                if (Constants.UltimateBossCompleted < (Settings.Instance.UltimateBossSelected + 1)) {
                    Constants.UltimateBossCompleted++;
                    if (Constants.UltimateBossCompleted == 3) {
                        Constants.InventorySize = 36;
                        Constants.UIControl.WritePLog("Ultimate Boss Zone 1 complete! Inventory expanded to 36 slots.");
                        Console.WriteLine("Ultimate Boss Zone 1 complete! Inventory expanded to 36 slots.");
                    } else if (Constants.UltimateBossCompleted == 8) {
                        Constants.InventorySize = 40;
                        Constants.UIControl.WritePLog("Ultimate Boss Zone 2 complete! Inventory expanded to 40 slots.");
                        Console.WriteLine("Ultimate Boss Zone 2 complete! Inventory expanded to 40 slots.");
                    } else if (Constants.UltimateBossCompleted == 22) {
                        Constants.InventorySize = 48;
                        Constants.UIControl.WritePLog("Ultimate Boss Zone 3 complete! Inventory expanded to 48 slots.");
                        Console.WriteLine("Ultimate Boss Zone 3 complete! Inventory expanded to 48 slots.");
                    }
                    Console.WriteLine($"Your current clear count: {Constants.UltimateBossCompleted}");
                    Constants.UIControl.WriteGLog($"Your current clear count: {Constants.UltimateBossCompleted}");
                }
                Settings.Instance.UltimateBoss = false;
            }
        }
    }
}