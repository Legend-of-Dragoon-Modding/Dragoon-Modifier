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

        public static void Setup(Emulator.IEmulator emulator, UI.IUIControl uiControl, LoDDict.ILoDDictionary LoDDict) {
            Console.WriteLine("Battle detected. Loading...");

            uint tableBase = emulator.Memory.BattleBasePoint;
            while (tableBase == emulator.Memory.CharacterPoint || tableBase == emulator.Memory.MonsterPoint) { // Wait until both C_Point and M_Point were set
                if (Constants.Run && emulator.Memory.GameState != Emulator.GameState.Battle) {
                    return;
                }
                Thread.Sleep(50);
            }

            emulator.LoadBattle();

            uiControl.UpdateField(emulator.Memory.BattleValue, emulator.Memory.EncounterID, emulator.Memory.MapID);

            if (DraMod.Settings.DualDifficulty) {
                Console.WriteLine("[DEBUG] [Dual Difficulty] Changing monster stats...");
                SwitchDualDifficulty(emulator, LoDDict);
            }

            MonsterChanges(emulator, LoDDict);

            UpdateUI(emulator, uiControl);

            if (Settings.NoDart != 0 && Settings.NoDart != 255) {
                NoDart.Initialize(emulator, Settings.NoDart);
            }
        }

        public static void Run(Emulator.IEmulator emulator, UI.IUIControl uiControl) {
            UpdateUI(emulator, uiControl);
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
            bool boss = false;
            string cwd = Directory.GetCurrentDirectory();
            string mod;

            if (emulator.Battle.EncounterID == 384 || //Commander
                emulator.Battle.EncounterID == 386 || //Fruegel I
                emulator.Battle.EncounterID == 414 || //Urobolus
                emulator.Battle.EncounterID == 385 || //Sandora Elite
                emulator.Battle.EncounterID == 388 || //Kongol I
                emulator.Battle.EncounterID == 408 || //Virage I
                emulator.Battle.EncounterID == 415 || //Fire Bird
                emulator.Battle.EncounterID == 393 || //Greham + Feyrbrand
                emulator.Battle.EncounterID == 412 || //Drake the Bandit
                emulator.Battle.EncounterID == 413 || //Jiango
                emulator.Battle.EncounterID == 387 || //Fruegel II
                emulator.Battle.EncounterID == 461 || //Sandora Elite II
                emulator.Battle.EncounterID == 389 || //Kongol II
                emulator.Battle.EncounterID == 390 || //Emperor Doel
                emulator.Battle.EncounterID == 402 || //Mappi
                emulator.Battle.EncounterID == 409 || //Virage II
                emulator.Battle.EncounterID == 403 || //Gehrich + Mappi
                emulator.Battle.EncounterID == 396 || //Lenus
                emulator.Battle.EncounterID == 417 || //Ghost Commander
                emulator.Battle.EncounterID == 397 || //Lenus + Regole
                emulator.Battle.EncounterID == 418 || //Kamuy
                emulator.Battle.EncounterID == 410 || //S Virage
                emulator.Battle.EncounterID == 416 || //Grand Jewel
                emulator.Battle.EncounterID == 394 || //Divine Dragon
                emulator.Battle.EncounterID == 422 || //Windigo
                emulator.Battle.EncounterID == 392 || //Lloyd
                emulator.Battle.EncounterID == 423 || //Polter Set
                emulator.Battle.EncounterID == 398 || //Damia
                emulator.Battle.EncounterID == 399 || //Syuveil
                emulator.Battle.EncounterID == 400 || //Belzac
                emulator.Battle.EncounterID == 401 || //Kanzas
                emulator.Battle.EncounterID == 420 || //Magician Faust
                emulator.Battle.EncounterID == 432 || //Last Kraken
                emulator.Battle.EncounterID == 430 || //Executioners
                emulator.Battle.EncounterID == 449 || //Spirit (Feyrbrand)
                emulator.Battle.EncounterID == 448 || //Spirit (Regole)
                emulator.Battle.EncounterID == 447 || //Spirit (Divine Dragon)
                emulator.Battle.EncounterID == 431 || //Zackwell
                emulator.Battle.EncounterID == 433 || //Imago
                emulator.Battle.EncounterID == 411 || //S Virage II
                emulator.Battle.EncounterID == 442 || //Zieg
                emulator.Battle.EncounterID == 443) { //Melbu Fraahma
                boss = true;
            }

            if (!boss) {
                mod = DraMod.Settings.Mod.Equals("Hell_Mode") ? "Hard_Mode" : "US_Base";
                LoDDict.SwapMonsters(cwd, mod);
                Console.WriteLine("[DEBUG] [Dual Difficulty] Mod selected: " +  mod); 
            } else {
                LoDDict.SwapMonsters(cwd, DraMod.Settings.Mod);
                Console.WriteLine("[DEBUG] [Dual Difficulty] Mod selected: " + DraMod.Settings.Mod);
            }

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
            double HP = LoDDict.Monster[id].HP; // TODO add the slider

            double resup = 1;
            if (HP > 65535) {
                resup = HP / 65535;
                HP = 65535;
            }
            HP = Math.Round(HP);

            Console.WriteLine("Monster ID: " + id + " HP: " + HP);

            emulator.Battle.MonsterTable[slot].HP = (ushort) HP;
            emulator.Battle.MonsterTable[slot].MaxHP = (ushort) HP;
            emulator.Battle.MonsterTable[slot].AT = (ushort) Math.Round(LoDDict.Monster[id].AT * Settings.ATMulti);
            emulator.Battle.MonsterTable[slot].OG_AT = (ushort) Math.Round(LoDDict.Monster[id].AT * Settings.ATMulti);
            emulator.Battle.MonsterTable[slot].MAT = (ushort) Math.Round(LoDDict.Monster[id].MAT * Settings.MATMulti);
            emulator.Battle.MonsterTable[slot].OG_MAT = (ushort) Math.Round(LoDDict.Monster[id].MAT * Settings.MATMulti);
            emulator.Battle.MonsterTable[slot].DF = (ushort) Math.Round(LoDDict.Monster[id].DF * Settings.DFMulti * resup);
            emulator.Battle.MonsterTable[slot].OG_DF = (ushort) Math.Round(LoDDict.Monster[id].DF * Settings.DFMulti * resup);
            emulator.Battle.MonsterTable[slot].MDF = (ushort) Math.Round(LoDDict.Monster[id].MDF * Settings.MDFMulti * resup);
            emulator.Battle.MonsterTable[slot].OG_MDF = (ushort) Math.Round(LoDDict.Monster[id].MDF * Settings.MDFMulti * resup);
            emulator.Battle.MonsterTable[slot].SPD = (ushort) Math.Round(LoDDict.Monster[id].SPD * Settings.SPDMulti);
            emulator.Battle.MonsterTable[slot].OG_SPD = (ushort) Math.Round(LoDDict.Monster[id].SPD * Settings.SPDMulti);
            emulator.Battle.MonsterTable[slot].A_AV = LoDDict.Monster[id].A_AV;
            emulator.Battle.MonsterTable[slot].M_AV = LoDDict.Monster[id].M_AV;
            emulator.Battle.MonsterTable[slot].P_Immune = LoDDict.Monster[id].PhysicalImmunity;
            emulator.Battle.MonsterTable[slot].M_Immune = LoDDict.Monster[id].MagicalImmunity;
            emulator.Battle.MonsterTable[slot].P_Half = LoDDict.Monster[id].PhysicalResistance;
            emulator.Battle.MonsterTable[slot].M_Half = LoDDict.Monster[id].MagicalResistance;
            emulator.Battle.MonsterTable[slot].Element = LoDDict.Monster[id].Element;
            emulator.Battle.MonsterTable[slot].ElementalImmunity = LoDDict.Monster[id].ElementalImmunity;
            emulator.Battle.MonsterTable[slot].ElementalResistance = LoDDict.Monster[id].ElementalResistance;
            emulator.Battle.MonsterTable[slot].StatusResist = LoDDict.Monster[id].StatusResist;
            emulator.Battle.MonsterTable[slot].SpecialEffect = LoDDict.Monster[id].SpecialEffect;
        }
    }
}
