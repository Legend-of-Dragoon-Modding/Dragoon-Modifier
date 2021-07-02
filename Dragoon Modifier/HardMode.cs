using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dragoon_Modifier.Core;

namespace Dragoon_Modifier {
    public static class HardMode {

        #region Dragoon Stats

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

        #region Equipment Variables

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
        static byte soasSyphonRingSlot = 0;
        static byte soasAnkhSlot = 0;

        #endregion

        #region Dragoon Change Variables

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

        #endregion

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

        #region Chapter 3 Elemental Buffs

        static int sparkleArrowBuff = 8;
        static int heatBladeBuff = 7;
        static int shadowCutterBuff = 9;
        static int morningStarBuff = -20;

        #endregion

        public static Hotkey[] Hotkeys = new Hotkey[] {
            new DragonBeaterHotkey(Hotkey.KEY_CIRCLE + Hotkey.KEY_RIGHT),
            new JeweledHammerHotkey(Hotkey.KEY_CIRCLE + Hotkey.KEY_DOWN)
        };

        public static void Setup(string difficulty) {

            checkWingBlaster = false;
            checkFlowerStorm = false;
            checkGaspless = false;
            checkJadeDragon = false;
            checkRoseDamage = false;
            roseEnhanceDragoon = false;
            meruEnhanceDragoon = false;
            trackRainbowBreath = false;
            burnStackSelected = false;
            resetBurnStack = false;
            dartBurnMPHeal = false;
            dartBurnStacks = 0;
            dartPreviousBurnStacks = 0;

            // flowerStorm = (byte) (uiCombo["cboFlowerStorm"] + 2); TODO UI

            EquipChangesSetup(difficulty);

            if (difficulty.Contains("Hell")) {
                HellDragoonChanges();
                if (difficulty.Equals("Hell")) {
                    NoEscape();
                }
            }
            if (Globals.PARTY_SLOT.Contains((byte) 2)) {
                int index = Array.IndexOf(Globals.PARTY_SLOT, (byte) 2);
                starChildren = 0;
                recoveryRateSave = Core.Emulator.Battle.CharacterTable[index].HP_Regen;
            } else if (Globals.PARTY_SLOT.Contains((byte) 8)) {
                int index = Array.IndexOf(Globals.PARTY_SLOT, (byte) 8);
                starChildren = 0;
                recoveryRateSave = Core.Emulator.Battle.CharacterTable[index].HP_Regen;
            }

            SetupBurnStacks();
        }

        static void EquipChangesSetup(string difficulty) { // TODO remove Core.Emulator
            Chapter3Buffs();
            PercentageBuffs();
            EquipSetup();

            if (Globals.PARTY_SLOT[0] == 2 || Globals.PARTY_SLOT[0] == 8) {
                shanaSaveAT = Core.Emulator.Battle.CharacterTable[0].AT;
            }

            if (Globals.CheckDMScript("btnDivineRed") && Globals.PARTY_SLOT[0] == 0 && (difficulty.Equals("Hard") || difficulty.Equals("Hell")) && Globals.PARTY_SLOT[0] == 0) {
                Core.Emulator.WriteAoB(Constants.GetAddress("SLOT1_SPELLS"), "01 02 FF FF FF FF FF FF");
                Core.Emulator.WriteByte("SPELL_TABLE", 50, 0x7 + (1 * 0xC)); //Explosion MP
                Core.Emulator.WriteByte("SPELL_TABLE", 50, 0x7 + (2 * 0xC)); //Final Burst MP
                Core.Emulator.WriteText(Globals.DRAGOON_SPELLS[1].Description_Pointer + 0x12, "1020%");
                Core.Emulator.WriteText(Globals.DRAGOON_SPELLS[2].Description_Pointer + 0x12, "1530%");
            }
        }

        static void Chapter3Buffs() {
            for (int i = 0; i < Core.Emulator.Battle.CharacterTable.Length; i++) {
                if (Globals.CHAPTER >= 3) {
                    switch (Core.Emulator.Battle.CharacterTable[i].Weapon) {
                        case 28: // Sparkle Arrow
                            var at = (ushort) (Core.Emulator.Battle.CharacterTable[i].AT + sparkleArrowBuff);
                            Core.Emulator.Battle.CharacterTable[i].AT = at;
                            Core.Emulator.Battle.CharacterTable[i].OG_AT = at;
                            break;
                        case 2: // Heat Blade
                            at = (ushort) (Core.Emulator.Battle.CharacterTable[i].AT + heatBladeBuff);
                            Core.Emulator.Battle.CharacterTable[i].AT = at;
                            Core.Emulator.Battle.CharacterTable[i].OG_AT = at;
                            break;
                        case 14: // Shadow Cutter
                            at = (ushort) (Core.Emulator.Battle.CharacterTable[i].AT + shadowCutterBuff);
                            Core.Emulator.Battle.CharacterTable[i].AT = at;
                            Core.Emulator.Battle.CharacterTable[i].OG_AT = at;
                            break;
                        case 35: // Morning Star
                            at = (ushort) (Core.Emulator.Battle.CharacterTable[i].AT + morningStarBuff);
                            Core.Emulator.Battle.CharacterTable[i].AT = at;
                            Core.Emulator.Battle.CharacterTable[i].OG_AT = at;
                            break;
                    }
                }
            }
        }

        public static void PostBattleChapter3Buffs() {
            if (Globals.CHAPTER >= 3) {
                long address = Constants.GetAddress("ITEM_TABLE"); // TODO
                Core.Emulator.WriteByte(address + 28 * 0x1C + 0x9, (byte) (Core.Emulator.ReadByte(address + 28 * 0x1C + 0x9) + sparkleArrowBuff));
                Core.Emulator.WriteByte(address + 2 * 0x1C + 0x9, (byte) (Core.Emulator.ReadByte(address + 2 * 0x1C + 0x9) + heatBladeBuff));
                Core.Emulator.WriteByte(address + 14 * 0x1C + 0x9, (byte) (Core.Emulator.ReadByte(address + 14 * 0x1C + 0x9) + shadowCutterBuff));
                Core.Emulator.WriteByte(address + 35 * 0x1C + 0x9, (byte) (Core.Emulator.ReadByte(address + 35 * 0x1C + 0x9) + morningStarBuff));
            } else {
                Core.Emulator.WriteAoB((long) Globals.DICTIONARY.ItemList[35].DescriptionPointer, "FF A0 FF A0"); // TODO
            }
        }

        static void PercentageBuffs() {
            for (int slot = 0; slot < Core.Emulator.Battle.CharacterTable.Length; slot++) {
                var level = Core.Emulator.Memory.CharacterTable[Globals.PARTY_SLOT[slot]].Level;

                switch (Globals.PARTY_SLOT[slot]) {
                    case 2: // Shana
                    case 8: // Miranda
                        double boost = 1;
                        if (Core.Emulator.Battle.CharacterTable[slot].Weapon == 32) {
                            boost = 1.4;
                        } else if (level >= 28) {
                            boost = 2.15;
                        } else if (level >= 20) {
                            boost = 1.9;
                        } else if (level >= 10) {
                            boost = 1.6;
                        }

                        var at = (ushort) (Math.Round(Core.Emulator.Battle.CharacterTable[slot].AT * boost));
                        Core.Emulator.Battle.CharacterTable[slot].AT = at;
                        Core.Emulator.Battle.CharacterTable[slot].OG_AT = at;

                        if (level >= 30) {
                            var df = (ushort) (Math.Round(Core.Emulator.Battle.CharacterTable[slot].DF * 1.12));
                            Core.Emulator.Battle.CharacterTable[slot].DF = df;
                            Core.Emulator.Battle.CharacterTable[slot].OG_DF = df;
                        }
                        break;
                    case 3: // Rose
                        if (level >= 30) {
                            var df = (ushort) (Math.Round(Core.Emulator.Battle.CharacterTable[slot].DF * 1.11));
                            Core.Emulator.Battle.CharacterTable[slot].DF = df;
                            Core.Emulator.Battle.CharacterTable[slot].OG_DF = df;
                        }
                        break;
                    case 6: // Meru
                        if (level >= 30) {
                            var df = (ushort) (Math.Round(Core.Emulator.Battle.CharacterTable[slot].DF * 1.26));
                            Core.Emulator.Battle.CharacterTable[slot].DF = df;
                            Core.Emulator.Battle.CharacterTable[slot].OG_DF = df;
                        }
                        break;

                }
            }
        }

        static void EquipSetup() {
            soasSyphonRingSlot = 0;
            spiritEaterSlot = 0;
            spiritEaterCheck = false;
            harpoonSlot = 0;
            harpoonCheck = false;
            elementArrowSlot = 0;
            dragonBeaterSlot = 0;
            batteryGloveSlot = 0;
            batteryGloveLastAction = 0;
            batteryGloveCharge = 0;
            jeweledHammerSlot = 0;
            giantAxeSlot = 0;
            giantAxeLastAction = 0;
            fakeLegendCasqueSlot = 0;
            fakeLegendCasqueCheck = new bool[] { false, false, false };
            fakeLegendArmorSlot = 0;
            fakeLegendArmorCheck = new bool[] { false, false, false };
            legendCasqueSlot = 0;
            legendCasqueCount = new byte[] { 0, 0, 0 };
            legendCasqueCheckMDF = new bool[] { false, false, false };
            legendCasqueCheckShield = new bool[] { false, false, false };
            armorOfLegendSlot = 0;
            armorOfLegendCount = new byte[] { 0, 0, 0 };
            armorOfLegendCheckDF = new bool[] { false, false, false };
            armorOfLegendCheckShield = new bool[] { false, false, false };
            soasAnkhSlot = 0;

            for (int slot = 0; slot < Core.Emulator.Battle.CharacterTable.Length; slot++) {
                switch (Core.Emulator.Battle.CharacterTable[slot].Accessory) {
                    case 149: // Phantom Shield
                        var df = Core.Emulator.Battle.CharacterTable[slot].DF;

                        if (Core.Emulator.Battle.CharacterTable[slot].Armor == 74) {
                            Core.Emulator.Battle.CharacterTable[slot].DF = (ushort) Math.Ceiling(df * 1.1);
                            Core.Emulator.Battle.CharacterTable[slot].OG_DF = (ushort) Math.Ceiling(df * 1.1);
                        } else {
                            Core.Emulator.Battle.CharacterTable[slot].DF = (ushort) Math.Ceiling(df * 0.7);
                            Core.Emulator.Battle.CharacterTable[slot].OG_DF = (ushort) Math.Ceiling(df * 0.7);
                        }

                        var mdf = Core.Emulator.Battle.CharacterTable[slot].MDF;

                        if (Core.Emulator.Battle.CharacterTable[slot].Helmet == 89) {
                            Core.Emulator.Battle.CharacterTable[slot].MDF = (ushort) Math.Ceiling(mdf * 1.1);
                            Core.Emulator.Battle.CharacterTable[slot].OG_MDF = (ushort) Math.Ceiling(mdf * 1.1);
                        } else {
                            Core.Emulator.Battle.CharacterTable[slot].MDF = (ushort) Math.Ceiling(mdf * 0.7);
                            Core.Emulator.Battle.CharacterTable[slot].OG_MDF = (ushort) Math.Ceiling(mdf * 0.7);
                        }
                        break;

                    case 150: // Dragon Shield
                        df = Core.Emulator.Battle.CharacterTable[slot].DF;
                        if (Core.Emulator.Battle.CharacterTable[slot].Armor == 74) {
                            Core.Emulator.Battle.CharacterTable[slot].DF = (ushort) Math.Ceiling(df * 1.2);
                            Core.Emulator.Battle.CharacterTable[slot].OG_DF = (ushort) Math.Ceiling(df * 1.2);
                        } else {
                            Core.Emulator.Battle.CharacterTable[slot].DF = (ushort) Math.Ceiling(df * 0.7);
                            Core.Emulator.Battle.CharacterTable[slot].OG_DF = (ushort) Math.Ceiling(df * 0.7);
                        }
                        break;

                    case 151: // Angel Scarf
                        mdf = Core.Emulator.Battle.CharacterTable[slot].MDF;
                        if (Core.Emulator.Battle.CharacterTable[slot].Helmet == 89) {
                            Core.Emulator.Battle.CharacterTable[slot].MDF = (ushort) Math.Ceiling(mdf * 1.2);
                            Core.Emulator.Battle.CharacterTable[slot].OG_MDF = (ushort) Math.Ceiling(mdf * 1.2);
                        } else {
                            Core.Emulator.Battle.CharacterTable[slot].MDF = (ushort) Math.Ceiling(mdf * 0.7);
                            Core.Emulator.Battle.CharacterTable[slot].OG_MDF = (ushort) Math.Ceiling(mdf * 0.7);
                        }
                        break;

                    case 176: // Soa's Sash
                        for (int i = 0; i < Core.Emulator.Battle.CharacterTable.Length; i++) {
                            if (i != slot) {
                                Core.Emulator.Battle.CharacterTable[i].SP_Multi = (short) (Core.Emulator.Battle.CharacterTable[i].SP_Multi - 50);
                            }
                        }
                        break;

                    case 177: // Soa's Ankh
                        if (Globals.CheckDMScript("btnSoloMode")) {
                            Core.Emulator.Battle.CharacterTable[slot].Revive = (byte) (Core.Emulator.Battle.CharacterTable[slot].Revive - 50);
                        }
                        soasAnkhSlot |= (byte) (1 << slot);
                        break;

                    case 178: // Soas's Health Ring
                        for (int i = 0; i < Core.Emulator.Battle.CharacterTable.Length; i++) {
                            if (i != slot) {
                                Core.Emulator.Battle.CharacterTable[i].Max_HP = (ushort) Math.Round(Core.Emulator.Battle.CharacterTable[i].Max_HP * 0.75); // TODO simplify to reduce memory reads

                                if (Core.Emulator.Battle.CharacterTable[i].HP > Core.Emulator.Battle.CharacterTable[slot].Max_HP) {
                                    Core.Emulator.Battle.CharacterTable[i].HP = Core.Emulator.Battle.CharacterTable[slot].Max_HP;
                                }
                            }
                        }
                        break;

                    case 179: // Soa's Mage Ring
                        for (int i = 0; i < Core.Emulator.Battle.CharacterTable.Length; i++) {
                            if (i != slot) {
                                Core.Emulator.Battle.CharacterTable[i].Max_MP = (ushort) Math.Round(Core.Emulator.Battle.CharacterTable[i].Max_MP * 0.75); // TODO simplify to reduce memory reads

                                if (Core.Emulator.Battle.CharacterTable[i].MP > Core.Emulator.Battle.CharacterTable[slot].Max_MP) {
                                    Core.Emulator.Battle.CharacterTable[i].MP = Core.Emulator.Battle.CharacterTable[slot].Max_MP;
                                }
                            }
                        }
                        break;

                    case 181: // Soa's Syphon Ring
                        soasSyphonRingSlot = (byte) (1 << slot);
                        ushort mat = (ushort) (Core.Emulator.Battle.CharacterTable[slot].MAT * 2);
                        Core.Emulator.Battle.CharacterTable[slot].MAT = mat;
                        Core.Emulator.Battle.CharacterTable[slot].OG_MAT = mat;
                        Core.Emulator.Battle.CharacterTable[slot].DMAT = (ushort) Math.Round(Core.Emulator.Battle.CharacterTable[slot].DMAT * 0.3);
                        for (int i = 0; i < Core.Emulator.Battle.CharacterTable.Length; i++) {
                            if (i != slot) {
                                Core.Emulator.Battle.CharacterTable[i].MAT = (ushort) Math.Round(Core.Emulator.Battle.CharacterTable[i].MAT * 0.8);
                                Core.Emulator.Battle.CharacterTable[i].OG_MAT = Core.Emulator.Battle.CharacterTable[i].MAT;
                            }
                        }
                        break;

                    case 130: // Holy Ankh + Angel Robe nerf
                        if (Core.Emulator.Battle.CharacterTable[slot].Armor == 73) { // Angel Robe
                            Core.Emulator.Battle.CharacterTable[slot].Revive = (byte) (Core.Emulator.Battle.CharacterTable[slot].Revive - 20);
                        }
                        break;

                    case 180: // Soas's Shield Ring
                        Core.Emulator.Battle.CharacterTable[slot].HP = 1;
                        Core.Emulator.Battle.CharacterTable[slot].Max_HP = 1;
                        Core.Emulator.Battle.CharacterTable[slot].DF = 10;
                        Core.Emulator.Battle.CharacterTable[slot].MDF = 10;
                        Core.Emulator.Battle.CharacterTable[slot].A_AV = 90;
                        Core.Emulator.Battle.CharacterTable[slot].M_AV = 90;
                        for (int i = 0; i < Core.Emulator.Battle.CharacterTable.Length; i++) {
                            if (i != slot) {
                                Core.Emulator.Battle.CharacterTable[i].A_HIT = (byte) Math.Round(Core.Emulator.Battle.CharacterTable[i].A_HIT * 0.8);
                                Core.Emulator.Battle.CharacterTable[i].M_HIT = (byte) Math.Round(Core.Emulator.Battle.CharacterTable[i].M_HIT * 0.8);
                            }
                        }
                        break;
                }

                switch (Core.Emulator.Battle.CharacterTable[slot].Weapon) {
                    case 159: // Spirit Eater
                        spiritEaterSlot |= (byte) (1 << slot);
                        break;

                    case 161: // Element Arrow
                        elementArrowSlot |= (byte) (1 << slot);
                        ElementArrowSetup();
                        elementArrowLastAction = 255;
                        elementArrowTurns = 0;
                        Core.Emulator.Battle.CharacterTable[slot].Weapon_Element = elementArrowElement;
                        break;

                    case 162: // Dragon Beater
                        dragonBeaterSlot |= (byte) (1 << slot);
                        break;

                    case 163: // Battery Glove
                        batteryGloveSlot |= (byte) (1 << slot);
                        batteryGloveLastAction = 0;
                        batteryGloveCharge = 0;
                        break;

                    case 164: // Jeweled Hammer
                        jeweledHammerSlot |= (byte) (1 << slot);
                        break;

                    case 165: // Giant Axe
                        giantAxeSlot |= (byte) (1 << slot);
                        giantAxeLastAction = 0;
                        break;

                    case 166: // Soa's Light
                        Core.Emulator.Battle.CharacterTable[slot].SP_Multi = -100;
                        Core.Emulator.Battle.CharacterTable[slot].SP_Regen = 100;
                        for (int i = 0; i < Core.Emulator.Battle.CharacterTable.Length; i++) {
                            if (i != slot) {
                                ushort df = (ushort) Math.Round(Core.Emulator.Battle.CharacterTable[slot].DF * 0.7);
                                Core.Emulator.Battle.CharacterTable[i].DF = df;
                                Core.Emulator.Battle.CharacterTable[i].OG_DF = df;
                                ushort mdf = (ushort) Math.Round(Core.Emulator.Battle.CharacterTable[slot].MDF * 0.7);
                                Core.Emulator.Battle.CharacterTable[i].MDF = mdf;
                                Core.Emulator.Battle.CharacterTable[i].OG_MDF = mdf;
                            }
                        }
                        break;
                }

                switch (Core.Emulator.Battle.CharacterTable[slot].Helmet) {
                    case 168: // Soa's Helm
                        for (int i = 0; i < Core.Emulator.Battle.CharacterTable.Length; i++) {
                            if (i != slot) {
                                ushort at = (ushort) Math.Round(Core.Emulator.Battle.CharacterTable[i].AT * 0.7);
                                Core.Emulator.Battle.CharacterTable[i].AT = at;
                                Core.Emulator.Battle.CharacterTable[i].OG_AT = at;
                            }
                        }
                        break;

                    case 89: // Legend Casque
                        legendCasqueSlot |= (byte) (1 << slot);
                        break;
                }

                switch (Core.Emulator.Battle.CharacterTable[slot].Armor) {
                    case 170: // Divine DG armor
                        var character = Globals.PARTY_SLOT[slot];
                        short sp_p_hit = (short) (Core.Emulator.Battle.CharacterTable[slot].SP_P_Hit + 20);
                        short sp_m_hit = (short) (Core.Emulator.Battle.CharacterTable[slot].SP_M_Hit + 20);
                        short mp_p_hit = (short) (Core.Emulator.Battle.CharacterTable[slot].MP_P_Hit + 10);
                        short mp_m_hit = (short) (Core.Emulator.Battle.CharacterTable[slot].MP_M_Hit + 10);

                        Core.Emulator.Battle.CharacterTable[slot].SP_P_Hit = sp_p_hit;
                        Core.Emulator.Memory.SecondaryCharacterTable[character].SP_P_Hit = sp_p_hit;
                        Core.Emulator.Battle.CharacterTable[slot].SP_M_Hit = sp_m_hit;
                        Core.Emulator.Memory.SecondaryCharacterTable[character].SP_M_Hit = sp_m_hit;
                        Core.Emulator.Battle.CharacterTable[slot].MP_P_Hit = mp_p_hit;
                        Core.Emulator.Memory.SecondaryCharacterTable[character].MP_P_Hit = mp_p_hit;
                        Core.Emulator.Battle.CharacterTable[slot].MP_M_Hit = mp_m_hit;
                        Core.Emulator.Memory.SecondaryCharacterTable[character].MP_M_Hit = mp_m_hit;
                        break;

                    case 171: // Soa's Armor
                        for (int i = 0; i < Core.Emulator.Battle.CharacterTable.Length; i++) {
                            if (i != slot) {
                                ushort mat = (ushort) Math.Round(Core.Emulator.Battle.CharacterTable[i].MAT * 0.7);
                                Core.Emulator.Battle.CharacterTable[i].MAT = mat;
                                Core.Emulator.Battle.CharacterTable[i].OG_MAT = mat;
                            }
                        }
                        break;

                    case 74: // Armor of Legend
                        switch (Globals.PARTY_SLOT[slot]) {
                            case 0: // Dart
                                Core.Emulator.Battle.CharacterTable[slot].DF = (ushort) (Core.Emulator.Battle.CharacterTable[slot].DF + 41 - 127);
                                Core.Emulator.Battle.CharacterTable[slot].MDF = (ushort) (Core.Emulator.Battle.CharacterTable[slot].MDF + 40);
                                break;

                            case 1: // Lavitz
                            case 5: // Albert
                                Core.Emulator.Battle.CharacterTable[slot].DF = (ushort) (Core.Emulator.Battle.CharacterTable[slot].DF + 54 - 127);
                                Core.Emulator.Battle.CharacterTable[slot].MDF = (ushort) (Core.Emulator.Battle.CharacterTable[slot].MDF + 27);
                                break;

                            case 2: // Shana
                            case 8: // Miranda
                                Core.Emulator.Battle.CharacterTable[slot].DF = (ushort) (Core.Emulator.Battle.CharacterTable[slot].DF + 27 - 127);
                                Core.Emulator.Battle.CharacterTable[slot].MDF = (ushort) (Core.Emulator.Battle.CharacterTable[slot].MDF + 80);
                                break;

                            case 3: // Rose
                                Core.Emulator.Battle.CharacterTable[slot].DF = (ushort) (Core.Emulator.Battle.CharacterTable[slot].DF + 41 - 127);
                                Core.Emulator.Battle.CharacterTable[slot].MDF = (ushort) (Core.Emulator.Battle.CharacterTable[slot].MDF + 42);
                                break;

                            case 4: // Haschel
                                Core.Emulator.Battle.CharacterTable[slot].DF = (ushort) (Core.Emulator.Battle.CharacterTable[slot].DF + 45 - 127);
                                Core.Emulator.Battle.CharacterTable[slot].MDF = (ushort) (Core.Emulator.Battle.CharacterTable[slot].MDF + 40);
                                break;

                            case 6: // Meru
                                Core.Emulator.Battle.CharacterTable[slot].DF = (ushort) (Core.Emulator.Battle.CharacterTable[slot].DF + 30 - 127);
                                Core.Emulator.Battle.CharacterTable[slot].MDF = (ushort) (Core.Emulator.Battle.CharacterTable[slot].MDF + 54);
                                break;

                            case 7: // Kongol
                                Core.Emulator.Battle.CharacterTable[slot].DF = (ushort) (Core.Emulator.Battle.CharacterTable[slot].DF + 88 - 127);
                                Core.Emulator.Battle.CharacterTable[slot].MDF = (ushort) (Core.Emulator.Battle.CharacterTable[slot].MDF + 23);
                                break;
                        }
                        Core.Emulator.Battle.CharacterTable[slot].OG_DF = Core.Emulator.Battle.CharacterTable[slot].DF;
                        Core.Emulator.Battle.CharacterTable[slot].OG_MDF = Core.Emulator.Battle.CharacterTable[slot].MDF;
                        armorOfLegendSlot |= (byte) (1 << slot);
                        break;
                }

                if (Core.Emulator.Battle.CharacterTable[slot].Shoes == 174) { //Soa's Greaves
                    for (int i = 0; i < Core.Emulator.Battle.CharacterTable.Length; i++) {
                        if (i != slot) {
                            ushort spd = (ushort) (Core.Emulator.Battle.CharacterTable[i].SPD - 25);
                            Core.Emulator.Battle.CharacterTable[i].SPD = spd;
                            Core.Emulator.Battle.CharacterTable[i].OG_SPD = spd;
                            Core.Emulator.Memory.SecondaryCharacterTable[Globals.PARTY_SLOT[slot]].BodySPD = (byte) (Core.Emulator.Memory.SecondaryCharacterTable[Globals.PARTY_SLOT[slot]].BodySPD - 25);
                        }
                    }
                }

                if (Globals.PARTY_SLOT[slot] == 7) {
                    ushort equipSPD = (ushort) Math.Round((double) Core.Emulator.Memory.SecondaryCharacterTable[7].EquipSPD / 2);
                    Core.Emulator.Memory.SecondaryCharacterTable[7].EquipSPD = equipSPD;
                    ushort spd = (ushort) (equipSPD + Core.Emulator.Memory.SecondaryCharacterTable[7].BodySPD);
                    Core.Emulator.Battle.CharacterTable[slot].SPD = spd;
                    Core.Emulator.Battle.CharacterTable[slot].OG_SPD = spd;
                }
            }
        }

        static void ElementArrowSetup() {
            /* TODO UI
            switch (uiCombo["cboElement"]) {
                case 0:
                    elementArrowElement = 128;
                    elementArrowItem = 0xC3;
                    break;

                case 1:
                    elementArrowElement = 1;
                    elementArrowItem = 0xC6;
                    break;

                case 2:
                    elementArrowElement = 64;
                    elementArrowItem = 0xC7;
                    break;

                case 3:
                    elementArrowElement = 2;
                    elementArrowItem = 0xC5;
                    break;

                case 4:
                    elementArrowElement = 4;
                    elementArrowItem = 0xCA;
                    break;

                case 5:
                    elementArrowElement = 32;
                    elementArrowItem = 0xC9;
                    break;

                case 6:
                    elementArrowElement = 16;
                    elementArrowItem = 0xC2;
                    break;
            }
            */
        }

        static void HellDragoonChanges() { // TODO remove Core.Emulator.Write
            Core.Emulator.WriteByte("SPELL_TABLE", (byte) ((flowerStorm) * blossomStormTurnMP), 0x7 + (7 * 0xC)); //Lavitz's Blossom Storm MP
            Core.Emulator.WriteByte("SPELL_TABLE", (byte) ((flowerStorm) * blossomStormTurnMP), 0x7 + (26 * 0xC)); //Albert's Rose storm MP

            if (Constants.REGION == Region.NTA) {
                Core.Emulator.WriteUShort(Globals.DRAGOON_SPELLS[7].Description_Pointer + 44, (ushort) (0x15 + flowerStorm));
                Core.Emulator.WriteUShort(Globals.DRAGOON_SPELLS[26].Description_Pointer + 44, (ushort) (0x15 + flowerStorm));
            }

            Core.Emulator.WriteByte("SPELL_TABLE", moonLightMP, 0x7 + (11 * 0xC)); //Shana's Moon Light MP
            Core.Emulator.WriteByte("SPELL_TABLE", moonLightMP, 0x7 + (66 * 0xC)); //Miranda's Moon Light MP
            Core.Emulator.WriteByte("SPELL_TABLE", rainbowBreathMP, 0x7 + (25 * 0xC)); //Rainbow Breath MP
            Core.Emulator.WriteByte("SPELL_TABLE", gatesOfHeavenMP, 0x7 + (12 * 0xC)); //Shana's Gates of Heaven MP
            Core.Emulator.WriteByte("SPELL_TABLE", gatesOfHeavenMP, 0x7 + (67 * 0xC)); //Miranda's Gates of Heaven MP
        }

        public static void BattleDragoonRun(ushort encounter, bool reverseDBS, byte eleBombTurns, byte eleBombElement) {
            byte dragoonSpecialAttack = Core.Emulator.ReadByte("DRAGOON_SPECIAL_ATTACK"); // TODO
            double multi = 1;
            if (encounter == 416 || encounter == 394 || encounter == 443) {
                if (Core.Emulator.ReadByte("DRAGON_BLOCK_STAF") == 1) { // TODO
                    multi = 8;
                }
            }

            if (reverseDBS) {
                multi = 20;
            }
            for (byte slot = 0; slot < Core.Emulator.Battle.CharacterTable.Length; slot++) {

                // TODO DDF / DMDF or should it be in setup?

                switch (Core.Emulator.Memory.PartySlot[slot]) {
                    case 0:
                        DartDragoonRun(dragoonSpecialAttack, slot, multi);
                        break;
                    case 1:
                    case 5:
                        LavitzDragoonRun(dragoonSpecialAttack, slot, multi);
                        break;
                    case 2:
                    case 8:
                        ShanaDragoonRun(dragoonSpecialAttack, slot, multi);
                        break;
                    case 3:
                        RoseDragoonRun(dragoonSpecialAttack, slot, multi);
                        break;
                    case 4:
                        HaschelDragoonRun(dragoonSpecialAttack, slot, multi, eleBombTurns, eleBombElement);
                        break;
                    case 6:
                        MeruDragoonRun(dragoonSpecialAttack, slot, multi);
                        break;
                    case 7:
                        KongolDragoonRun(dragoonSpecialAttack, slot, multi);
                        break;
                }
            }
        }

        static void DartDragoonRun(byte dragoonSpecialAttack, byte slot, double multi) {
            if (dragoonSpecialAttack == 0 || dragoonSpecialAttack == 9) {
                if ((Globals.DRAGOON_SPIRITS & 128) == 128) {
                    Core.Emulator.Battle.CharacterTable[slot].DAT = (ushort) (dartDivineSpecialDAT * multi);
                } else {
                    if (burnStackSelected) {
                        multi *= 1 + (dartBurnStacks * damagePerBurn * (dartBurnStacks == dartMaxBurnStacks ? burnStackMaxAdditionMulti : burnStackAdditionMulti));
                    }
                    Core.Emulator.Battle.CharacterTable[slot].DAT = (ushort) (dartDAT * multi);
                }
            } else {
                if ((Globals.DRAGOON_SPIRITS & 128) == 128) {
                    Core.Emulator.Battle.CharacterTable[slot].DAT = (ushort) (dartDivineDAT * multi);
                } else {
                    if (Globals.CheckDMScript("btwDivineRed")) {
                        Core.Emulator.Battle.CharacterTable[slot].DAT = (ushort) (dartDREDAT * multi);
                    } else {
                        if (burnStackSelected) {
                            multi *= 1 + (dartBurnStacks * damagePerBurn * (dartBurnStacks == dartMaxBurnStacks ? burnStackMaxAdditionMulti : burnStackAdditionMulti));
                        }
                        Core.Emulator.Battle.CharacterTable[slot].DAT = (ushort) (dartDAT * multi);
                    }
                }
            }

            multi = SoasSyphonDebuff(slot, multi);

            if (currentMP[slot] < previousMP[slot]) {
                byte spell = Core.Emulator.Battle.CharacterTable[slot].Spell_Cast;
                if (Globals.CheckDMScript("btnDivineRed")) {
                    if (spell == 1) {
                        Core.Emulator.Battle.CharacterTable[slot].DMAT = (ushort) (explosionDREDMAT * multi);
                    } else {
                        Core.Emulator.Battle.CharacterTable[slot].DMAT = (ushort) (finalBurstDREMAT * multi);
                    }
                } else {
                    if (spell == 0) {
                        Core.Emulator.Battle.CharacterTable[slot].DMAT = (ushort) (flameshotDMAT * multi);
                        AddBurnStacks(burnStackFlameshot, slot);
                    } else if (spell == 1) {
                        Core.Emulator.Battle.CharacterTable[slot].DMAT = (ushort) (explosionDMAT * multi);
                        AddBurnStacks(burnStackExplosion, slot);
                    } else if (spell == 2) {
                        Core.Emulator.Battle.CharacterTable[slot].DMAT = (ushort) (finalBurstDMAT * multi);
                        AddBurnStacks(burnStackFinalBurst, slot);
                    } else if (spell == 3) {
                        Core.Emulator.Battle.CharacterTable[slot].DMAT = (ushort) (redEyeDMAT * multi);
                        AddBurnStacks(burnStackRedEye, slot);
                    } else if (spell == 4 || spell == 9) {
                        Core.Emulator.Battle.CharacterTable[slot].DMAT = (ushort) (divineDMAT * multi);
                    }
                }
                previousMP[slot] = currentMP[slot];
                Core.Emulator.Battle.CharacterTable[slot].Spell_Cast = 255;
            } else {
                if (currentMP[slot] > previousMP[slot]) {
                    previousMP[slot] = currentMP[slot];
                }
            }
        }

        static void LavitzDragoonRun(byte dragoonSpecialAttack, byte slot, double multi) {
            if (harpoonCheck) {
                multi *= 3;
            }

            if (dragoonSpecialAttack == 1 || dragoonSpecialAttack == 5) {
                Core.Emulator.Battle.CharacterTable[slot].DAT = (ushort) (lavitzSpecialDAT * multi);
            } else {
                Core.Emulator.Battle.CharacterTable[slot].DAT = (ushort) (lavitzDAT * multi);
            }

            multi = SoasSyphonDebuff(slot, multi);

            if (currentMP[slot] != previousMP[slot] && currentMP[slot] < previousMP[slot]) {
                byte spell = Core.Emulator.Battle.CharacterTable[slot].Spell_Cast;
                if (spell == 5 || spell == 14) {
                    Core.Emulator.Battle.CharacterTable[slot].DMAT = (ushort) (wingBlasterDMAT * multi);
                    checkWingBlaster = true;
                    for (int i = 0; i < Globals.MONSTER_SIZE; i++) trackMonsterHP[i] = Core.Emulator.Battle.MonsterTable[i].HP;
                } else if (spell == 6 || spell == 17) {
                    Core.Emulator.Battle.CharacterTable[slot].DMAT = (ushort) (gasplessDMAT * multi);
                    checkGaspless = true;
                    for (int i = 0; i < Globals.MONSTER_SIZE; i++) trackMonsterHP[i] = Core.Emulator.Battle.MonsterTable[i].HP;
                } else if (spell == 8) {
                    Core.Emulator.Battle.CharacterTable[slot].DMAT = (ushort) (jadeDragonDMAT * multi);
                    checkJadeDragon = true;
                    for (int i = 0; i < Globals.MONSTER_SIZE; i++) trackMonsterHP[i] = Core.Emulator.Battle.MonsterTable[i].HP;
                } else if (Globals.DIFFICULTY_MODE.Contains("Hell") && (spell == 7 || spell == 26)) {
                    checkFlowerStorm = true;
                    for (int x = 0; x < 3; x++) {
                        if (Core.Emulator.Battle.CharacterTable[x].HP > 0) {
                            Core.Emulator.Battle.CharacterTable[x].PWR_DF_Turn = 0;
                            Core.Emulator.Battle.CharacterTable[x].PWR_MDF_Turn = 0;
                        }
                    }
                }
                previousMP[slot] = currentMP[slot];
                Core.Emulator.Battle.CharacterTable[slot].Spell_Cast = 255;
            } else {
                if (currentMP[slot] > previousMP[slot]) {
                    previousMP[slot] = currentMP[slot];
                }

                if (checkWingBlaster || checkGaspless || checkJadeDragon) {
                    for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                        if (Core.Emulator.Battle.MonsterTable[i].HP < trackMonsterHP[i]) {
                            double tpDamage = checkWingBlaster ? wingBlasterTPDamage : checkGaspless ? gasplessTPDamage : jadeDragonTPDamage;
                            if (Core.Emulator.Battle.MonsterTable[i].Speed_Down_Turn > 0) tpDamage /= 3;
                            Core.Emulator.Battle.MonsterTable[i].Turn = (ushort) Math.Max(0, Core.Emulator.Battle.MonsterTable[i].Turn - tpDamage);
                            checkWingBlaster = checkGaspless = checkJadeDragon = false;
                        }
                    }

                    if (Core.Emulator.Battle.CharacterTable[slot].Action != 10 && Core.Emulator.Battle.CharacterTable[slot].Action != 26) {
                        checkWingBlaster = checkGaspless = checkJadeDragon = false;
                    }
                }

                if (checkFlowerStorm) {
                    bool changed = false;
                    for (int x = 0; x < 3; x++) {
                        if (Globals.PARTY_SLOT[x] < 9) {
                            if (Core.Emulator.Battle.CharacterTable[x].PWR_DF_Turn != 0) {
                                changed = true;
                            }
                        }
                    }

                    if (changed) {
                        for (int x = 0; x < 3; x++) {
                            if (Globals.PARTY_SLOT[x] == 1 || Globals.PARTY_SLOT[x] == 5) {
                                Core.Emulator.Battle.CharacterTable[x].PWR_DF_Turn = (byte) (flowerStorm + 1);
                                Core.Emulator.Battle.CharacterTable[x].PWR_MDF_Turn = (byte) (flowerStorm + 1);
                            } else {
                                Core.Emulator.Battle.CharacterTable[x].PWR_DF_Turn = flowerStorm;
                                Core.Emulator.Battle.CharacterTable[x].PWR_MDF_Turn = flowerStorm;
                            }
                        }
                        checkFlowerStorm = false;
                    }
                }
            }
        }

        static void ShanaDragoonRun(byte dragoonSpecialAttack, byte slot, double multi) {
            if (starChildren > 0) {
                if (starChildren == 3 && Core.Emulator.Battle.CharacterTable[slot].Action == 0)
                    starChildren = 2;
                if (starChildren == 2 && Core.Emulator.Battle.CharacterTable[slot].Action == 8)
                    starChildren = 1;
                if (starChildren == 1 && Core.Emulator.Battle.CharacterTable[slot].Action != 8) {
                    starChildren = 0;
                    Core.Emulator.Battle.CharacterTable[slot].HP_Regen = recoveryRateSave;
                }
            }

            if (slot == 0 && Core.Emulator.Battle.CharacterTable[slot].Action == 10) {
                if (dragoonSpecialAttack == 2 || dragoonSpecialAttack == 8) {
                    Core.Emulator.Battle.CharacterTable[slot].AT = (ushort) Math.Floor(shanaSaveAT * (shanaSpecialDAT / 100D));
                } else {
                    Core.Emulator.Battle.CharacterTable[slot].AT = (ushort) Math.Floor(shanaSaveAT * (shanaDAT / 100D));
                }
            } else if (slot == 0 && Core.Emulator.Battle.CharacterTable[slot].Action == 8) {
                Core.Emulator.Battle.CharacterTable[slot].AT = shanaSaveAT;
            }

            int heal = gatesOfHeavenHealBase;
            int healReduction = Globals.DIFFICULTY_MODE.Contains("Hell") ? gatesOfHeavenHellModePenalty : gatesOfHeavenHardModePenalty;
            for (int i = 0; i < 3; i++) {
                if (Globals.PARTY_SLOT[i] < 9 && Core.Emulator.Battle.CharacterTable[i].HP == 0) {
                    heal -= healReduction;
                }
            }

            if (heal != gatesOfHeavenHeal) {
                Core.Emulator.WriteByte("SPELL_TABLE", (byte) heal, 0x5 + (12 * 0xC)); //Shana's Gates of Heaven Heal %
                Core.Emulator.WriteByte("SPELL_TABLE", (byte) heal, 0x5 + (67 * 0xC)); //???'s Gates of Heaven Heal %
                Core.Emulator.WriteText(Globals.DRAGOON_SPELLS[12].Description_Pointer + 0x8, heal.ToString());
                gatesOfHeavenHeal = heal;
            }

            if (dragoonSpecialAttack == 2 || dragoonSpecialAttack == 8) {
                Core.Emulator.Battle.CharacterTable[slot].DAT = (ushort) (shanaSpecialDAT * multi);
            } else {
                Core.Emulator.Battle.CharacterTable[slot].DAT = (ushort) (shanaDAT * multi);
            }

            multi = SoasSyphonDebuff(slot, multi);

            if (currentMP[slot] < previousMP[slot]) {
                byte spell = Core.Emulator.Battle.CharacterTable[slot].Spell_Cast;
                if (spell == 10 || spell == 65) {
                    Core.Emulator.Battle.CharacterTable[slot].DMAT = (ushort) (starChildrenDMAT * multi);
                    Core.Emulator.Battle.CharacterTable[slot].HP_Regen = 100;
                    starChildren = 3;
                } else if (spell == 13) {
                    Core.Emulator.Battle.CharacterTable[slot].DMAT = (ushort) (wSilverDragonDMAT * multi);
                }
                previousMP[slot] = currentMP[slot];
                Core.Emulator.Battle.CharacterTable[slot].Spell_Cast = 255;
            } else {
                if (currentMP[slot] > previousMP[slot]) {
                    previousMP[slot] = currentMP[slot];
                }
            }
        }

        static void RoseDragoonRun(byte dragoonSpecialAttack, byte slot, double multi) {
            if ((dragonBeaterSlot & (1 << slot)) != 0) { // Dragon Beater
                multi *= 1.1;
            }

            if (dragoonSpecialAttack == 3) {
                Core.Emulator.Battle.CharacterTable[slot].DAT = (ushort) (roseSpecialDAT * multi);
            } else {
                Core.Emulator.Battle.CharacterTable[slot].DAT = (ushort) (roseDAT * multi);
            }

            multi = SoasSyphonDebuff(slot, multi);

            if (currentMP[slot] < previousMP[slot]) {
                byte spell = Core.Emulator.Battle.CharacterTable[slot].Spell_Cast;
                if (roseEnhanceDragoon) {
                    if (spell == 15) {
                        Core.Emulator.Battle.CharacterTable[slot].DMAT = (ushort) (enhancedAstralDrainDMAT * multi);
                        for (int x = 0; x < 3; x++) {
                            if (Globals.PARTY_SLOT[x] < 9 && Core.Emulator.Battle.CharacterTable[slot].HP > 0) {
                                Core.Emulator.Battle.CharacterTable[x].HP = (ushort) Math.Min(Core.Emulator.Battle.CharacterTable[x].Max_HP, Core.Emulator.Battle.CharacterTable[x].HP + Math.Round(Core.Emulator.Battle.CharacterTable[slot].HP * (Core.Emulator.ReadByte("ROSE_DRAGOON_LEVEL") * 0.04)));
                            }
                        }
                    } else if (spell == 16) {
                        Core.Emulator.Battle.CharacterTable[slot].DMAT = (ushort) (enhancedDeathDimensionDMAT * multi);
                    } else if (spell == 19) {
                        Core.Emulator.Battle.CharacterTable[slot].DMAT = (ushort) (enhancedDarkDragonDMAT * multi);
                        checkRoseDamage = true;
                        checkRoseDamageSave = Core.Emulator.ReadUShort("DAMAGE_SLOT1");
                    }
                } else {
                    if (spell == 15) {
                        Core.Emulator.Battle.CharacterTable[slot].DMAT = (ushort) (astralDrainDMAT * multi);
                        for (int x = 0; x < 3; x++) {
                            if (Globals.PARTY_SLOT[x] < 9 && Core.Emulator.Battle.CharacterTable[slot].HP > 0) {
                                Core.Emulator.Battle.CharacterTable[x].HP = (ushort) Math.Min(Core.Emulator.Battle.CharacterTable[x].Max_HP, Core.Emulator.Battle.CharacterTable[x].HP + Math.Round(Core.Emulator.Battle.CharacterTable[slot].HP * (Core.Emulator.ReadByte("ROSE_DRAGOON_LEVEL") * 0.05)));
                            }
                        }
                    } else if (spell == 16) {
                        Core.Emulator.Battle.CharacterTable[slot].DMAT = (ushort) (deathDimensionDMAT * multi);
                    } else if (spell == 19) {
                        Core.Emulator.Battle.CharacterTable[slot].DMAT = (ushort) (darkDragonDMAT * multi);
                        checkRoseDamage = true;
                        checkRoseDamageSave = Core.Emulator.ReadUShort("DAMAGE_SLOT1");
                    }
                }
                previousMP[slot] = currentMP[slot];
                Core.Emulator.Battle.CharacterTable[slot].Spell_Cast = 255;
            } else {
                if (currentMP[slot] > previousMP[slot]) {
                    previousMP[slot] = currentMP[slot];
                } else {
                    if (checkRoseDamage && Core.Emulator.ReadUShort("DAMAGE_SLOT1") != checkRoseDamageSave) {
                        checkRoseDamage = false;
                        if (roseEnhanceDragoon) {
                            Core.Emulator.Battle.CharacterTable[slot].HP = (ushort) Math.Min(Core.Emulator.Battle.CharacterTable[slot].HP + (Core.Emulator.ReadUShort("DAMAGE_SLOT1") * 0.4), Core.Emulator.Battle.CharacterTable[slot].Max_HP);
                        } else {
                            Core.Emulator.Battle.CharacterTable[slot].HP = (ushort) Math.Min(Core.Emulator.Battle.CharacterTable[slot].HP + (Core.Emulator.ReadUShort("DAMAGE_SLOT1") * 0.1), Core.Emulator.Battle.CharacterTable[slot].Max_HP);
                        }
                    }
                }
            }
        }

        static void HaschelDragoonRun(byte dragoonSpecialAttack, byte slot, double multi, byte eleBombTurns, byte eleBombElement) {
            if (dragoonSpecialAttack == 4) {
                Core.Emulator.Battle.CharacterTable[slot].DAT = (ushort) (haschelSpecialDAT * multi);
            } else {
                Core.Emulator.Battle.CharacterTable[slot].DAT = (ushort) (haschelDAT * multi);
            }

            multi = SoasSyphonDebuff(slot, multi);

            if (eleBombTurns > 0 && eleBombElement == 16) {
                multi *= 3;
            }

            if (currentMP[slot] < previousMP[slot]) {
                byte spell = Core.Emulator.Battle.CharacterTable[slot].Spell_Cast;
                if (spell == 20) {
                    Core.Emulator.Battle.CharacterTable[slot].DMAT = (ushort) (atomicMindDMAT * multi);
                } else if (spell == 21) {
                    Core.Emulator.Battle.CharacterTable[slot].DMAT = (ushort) (thunderKidDMAT * multi);
                } else if (spell == 22) {
                    Core.Emulator.Battle.CharacterTable[slot].DMAT = (ushort) (thunderGodDMAT * multi);
                } else if (spell == 23) {
                    Core.Emulator.Battle.CharacterTable[slot].DMAT = (ushort) (violetDragonDMAT * multi);
                }
                previousMP[slot] = currentMP[slot];
                Core.Emulator.Battle.CharacterTable[slot].Spell_Cast = 255;
            } else {
                if (currentMP[slot] > previousMP[slot]) {
                    previousMP[slot] = currentMP[slot];
                }
            }
        }

        static void MeruDragoonRun(byte dragoonSpecialAttack, byte slot, double multi) {
            if (dragoonSpecialAttack == 6) {
                Core.Emulator.Battle.CharacterTable[slot].DAT = (ushort) (meruSpecialDAT * multi);
            } else {
                Core.Emulator.Battle.CharacterTable[slot].DAT = (ushort) (meruDAT * multi);
            }

            multi = SoasSyphonDebuff(slot, multi);

            if (currentMP[slot] < previousMP[slot]) {
                byte spell = Core.Emulator.Battle.CharacterTable[slot].Spell_Cast;
                if (meruEnhanceDragoon) {
                    if (spell == 24) {
                        Core.Emulator.Battle.CharacterTable[slot].DMAT = (ushort) (enhancedFreezingRingDMAT * multi);
                    } else if (spell == 25) {
                        trackRainbowBreath = true;
                    } else if (spell == 27) {
                        Core.Emulator.Battle.CharacterTable[slot].DMAT = (ushort) (enhancedDiamondDustDMAT * multi);
                    } else if (spell == 28) {
                        Core.Emulator.Battle.CharacterTable[slot].DMAT = (ushort) (enhancedBlueSeaDragonDMAT * multi);
                    }
                } else {
                    if (spell == 24) {
                        Core.Emulator.Battle.CharacterTable[slot].DMAT = (ushort) (freezingRingDMAT * multi);
                    } else if (spell == 27) {
                        Core.Emulator.Battle.CharacterTable[slot].DMAT = (ushort) (diamondDustDMAT * multi);
                    } else if (spell == 28) {
                        Core.Emulator.Battle.CharacterTable[slot].DMAT = (ushort) (blueSeaDragonDMAT * multi);
                    }
                }
                previousMP[slot] = currentMP[slot];
                Core.Emulator.Battle.CharacterTable[slot].Spell_Cast = 255;
            } else {
                if (currentMP[slot] > previousMP[slot]) {
                    previousMP[slot] = currentMP[slot];
                } else {
                    if (trackRainbowBreath) {
                        if (Core.Emulator.Battle.CharacterTable[slot].Action == 2 || Core.Emulator.Battle.CharacterTable[slot].Action == 9) {
                            for (int x = 0; x < 3; x++) {
                                if (Globals.PARTY_SLOT[slot] < 9) {
                                    Core.Emulator.Battle.CharacterTable[x].HP = (ushort) Math.Min(short.MaxValue, Math.Round(Core.Emulator.Battle.CharacterTable[x].HP * 1.65));
                                }
                            }
                            trackRainbowBreath = false;
                        }
                    }
                }
            }
        }

        static void KongolDragoonRun(byte dragoonSpecialAttack, byte slot, double multi) {
            if (dragoonSpecialAttack == 7) {
                Core.Emulator.Battle.CharacterTable[slot].DAT = (ushort) (kongolSpecialDAT * multi);
            } else {
                Core.Emulator.Battle.CharacterTable[slot].DAT = (ushort) (kongolDAT * multi);
            }

            multi = SoasSyphonDebuff(slot, multi);

            if (currentMP[slot] < previousMP[slot]) {
                byte spell = Core.Emulator.Battle.CharacterTable[slot].Spell_Cast;
                if (spell == 29) {
                    Core.Emulator.Battle.CharacterTable[slot].DMAT = (ushort) (grandStreamDMAT * multi);
                } else if (spell == 30) {
                    Core.Emulator.Battle.CharacterTable[slot].DMAT = (ushort) (meteorStrike * multi);
                } else if (spell == 31) {
                    Core.Emulator.Battle.CharacterTable[slot].DMAT = (ushort) (goldDragon * multi);
                }
                previousMP[slot] = currentMP[slot];
                Core.Emulator.Battle.CharacterTable[slot].Spell_Cast = 255;
            } else {
                if (currentMP[slot] > previousMP[slot]) {
                    previousMP[slot] = currentMP[slot];
                }
            }
        }

        static double SoasSyphonDebuff(byte slot, double multi) {
            if ((soasSyphonRingSlot & (1 << slot)) != 0) { //Soa's Syphon Ring
                multi *= 0.3;
            }
            return multi;
        }

        static void SetupBurnStacks() {
            for (int slot = 0; slot < Core.Emulator.Battle.CharacterTable.Length; slot++) {
                if (Globals.PARTY_SLOT[slot] == 0) {
                    int dlv = Core.Emulator.Battle.CharacterTable[slot].DLV;
                    dartMaxBurnStacks = dlv == 1 ? 3 : dlv == 2 ? 6 : dlv == 3 ? 9 : 12;
                }
                dartPreviousAction[slot] = 0;
            }
        }

        static void AddBurnStacks(int stacks, int slot) {
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
                        if (Core.Emulator.Battle.CharacterTable[i].Action == 8 || Core.Emulator.Battle.CharacterTable[i].Action == 10) {
                            dartBurnStacks = 0;
                            dartPreviousBurnStacks = 0;
                            Core.Emulator.WriteText(Globals.DRAGOON_SPELLS[0].Description_Pointer + 0x1E, "1.00");
                            Core.Emulator.WriteText(Globals.DRAGOON_SPELLS[1].Description_Pointer + 0x1E, "1.00");
                            Core.Emulator.WriteText(Globals.DRAGOON_SPELLS[2].Description_Pointer + 0x1E, "1.00");
                            Core.Emulator.WriteText(Globals.DRAGOON_SPELLS[3].Description_Pointer + 0x20, "1.00");
                            Core.Emulator.Battle.CharacterTable[i].MP = 0;
                            Core.Emulator.Battle.CharacterTable[i].Spell_Cast = 255;
                            burnStackSelected = false;
                            resetBurnStack = false;
                            Constants.WriteGLogOutput("Dart Burn Stacks: " + dartBurnStacks + " / " + dartMaxBurnStacks);
                        }
                    }

                    if (Core.Emulator.Battle.CharacterTable[i].Action == 10 && dartPreviousAction[i] != 10) {
                        dartBurnMP[i] = Core.Emulator.Battle.CharacterTable[i].MP;
                    }

                    if (dartPreviousAction[i] == 26 && dartBurnMPHeal) {
                        if (dartBurnStacks >= 4 && dartPreviousBurnStacks < 4) {
                            Core.Emulator.Battle.CharacterTable[i].MP = (ushort) Math.Min(Core.Emulator.Battle.CharacterTable[i].MP + 10, Core.Emulator.Battle.CharacterTable[i].Max_MP);
                        } else if (dartBurnStacks >= 8 && dartPreviousBurnStacks < 8) {
                            Core.Emulator.Battle.CharacterTable[i].MP = (ushort) Math.Min(Core.Emulator.Battle.CharacterTable[i].MP + 20, Core.Emulator.Battle.CharacterTable[i].Max_MP);
                        } else if (dartBurnStacks >= 12 && dartPreviousBurnStacks < 12) {
                            Core.Emulator.Battle.CharacterTable[i].MP = (ushort) Math.Min(Core.Emulator.Battle.CharacterTable[i].MP + 30, Core.Emulator.Battle.CharacterTable[i].Max_MP);
                        }
                        dartBurnMPHeal = false;
                    }

                    if (dartBurnStacks > 0) {
                        if (Core.Emulator.Battle.CharacterTable[i].Action == 10) {
                            byte menu = Core.Emulator.Battle.CharacterTable[i].Menu;

                            if (menu == 96 || menu == 98) {
                                byte[] icons = { 1, 4, 9, 3, 3 };
                                Core.Emulator.Battle.CharacterTable[i].Menu = (byte) (225 - (96 - Core.Emulator.Battle.CharacterTable[i].Menu));
                                Thread.Sleep(60);
                                for (int x = menu == 96 ? 1 : 0; x < icons.Length; x++) {
                                    Core.Emulator.WriteByte(Globals.M_POINT + 0xD34 + (x - (menu == 96 ? 1 : 0)) * 0x2, icons[x]);
                                }
                            }

                            int iconCount = Core.Emulator.ReadByte(Globals.M_POINT + 0xD32);
                            int iconSelected = Core.Emulator.ReadByte(Globals.M_POINT + 0xD46);

                            if (burnStackSelected) {
                                if ((iconCount == 4 && (iconSelected == 0 || iconSelected == 2)) || (iconCount == 5 && (iconSelected == 1 || iconSelected == 3))) {
                                    burnStackSelected = false;
                                    double burnAmount = 1 + (dartBurnStacks * damagePerBurn);
                                    Core.Emulator.WriteText(Globals.DRAGOON_SPELLS[0].Description_Pointer + 0x1E, "1.00");
                                    Core.Emulator.WriteText(Globals.DRAGOON_SPELLS[1].Description_Pointer + 0x1E, "1.00");
                                    Core.Emulator.WriteText(Globals.DRAGOON_SPELLS[2].Description_Pointer + 0x1E, "1.00");
                                    Core.Emulator.WriteText(Globals.DRAGOON_SPELLS[3].Description_Pointer + 0x20, "1.00");

                                    if (dartBurnStacks == dartMaxBurnStacks) {
                                        Core.Emulator.WriteByte("SPELL_TABLE", 10, 0x7 + (0 * 0xC));
                                        Core.Emulator.WriteByte("SPELL_TABLE", 20, 0x7 + (1 * 0xC));
                                        Core.Emulator.WriteByte("SPELL_TABLE", 30, 0x7 + (2 * 0xC));
                                        Core.Emulator.WriteByte("SPELL_TABLE", 80, 0x7 + (3 * 0xC));
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
                                    Core.Emulator.WriteText(Globals.DRAGOON_SPELLS[0].Description_Pointer + 0x1E, flameshotDesc.Substring(0, Math.Min(4, flameshotDesc.Length)));
                                    Core.Emulator.WriteText(Globals.DRAGOON_SPELLS[1].Description_Pointer + 0x1E, explosionDesc.Substring(0, Math.Min(4, explosionDesc.Length)));
                                    Core.Emulator.WriteText(Globals.DRAGOON_SPELLS[2].Description_Pointer + 0x1E, finalBurstDesc.Substring(0, Math.Min(4, finalBurstDesc.Length)));
                                    Core.Emulator.WriteText(Globals.DRAGOON_SPELLS[3].Description_Pointer + 0x20, redEyeDesc.Substring(0, Math.Min(4, redEyeDesc.Length)));

                                    if (dartBurnStacks == dartMaxBurnStacks) {
                                        Core.Emulator.WriteByte("SPELL_TABLE", 1, 0x7 + (0 * 0xC));
                                        Core.Emulator.WriteByte("SPELL_TABLE", 1, 0x7 + (1 * 0xC));
                                        Core.Emulator.WriteByte("SPELL_TABLE", 1, 0x7 + (2 * 0xC));
                                        Core.Emulator.WriteByte("SPELL_TABLE", 1, 0x7 + (3 * 0xC));
                                    }
                                }
                            }
                        } else if (Core.Emulator.Battle.CharacterTable[i].Action == 26) {
                            if (dartPreviousAction[i] == 10 && dartBurnMP[i] == Core.Emulator.Battle.CharacterTable[i].MP) {
                                byte dlv = Core.Emulator.Battle.CharacterTable[i].DLV;
                                AddBurnStacks(dlv == 5 ? 2 : 1, i);
                            }

                            if (burnStackSelected) {
                                resetBurnStack = true;
                            }
                        }
                    } else {
                        if (Core.Emulator.Battle.CharacterTable[i].Action == 26) {
                            if (dartPreviousAction[i] == 10 && dartBurnMP[i] == Core.Emulator.Battle.CharacterTable[i].MP) {
                                byte dlv = Core.Emulator.Battle.CharacterTable[i].DLV;
                                AddBurnStacks(dlv == 5 ? 2 : 1, i);
                            }
                        }
                    }

                    dartPreviousAction[i] = Core.Emulator.Battle.CharacterTable[i].Action;
                }
            }
        } // TODO rewrite

        static void NoEscape() { // TODO remove Core.Emulator
            if (Globals.CheckDMScript("btnBlackRoom")) {
                var map = Core.Emulator.Memory.MapID;
                if (((map >= 5 && map <= 7) || (map >= 624 && map <= 625))) {
                    return;
                }
                for (int i = 0; i < Core.Emulator.Battle.CharacterTable.Length; i++) {
                    byte noEscape = Core.Emulator.ReadByte("NO_ESCAPE", (Core.Emulator.Battle.MonsterTable.Length + i) * 0x20);
                    noEscape |= 8;
                    Core.Emulator.WriteByte("NO_ESCAPE", noEscape, (Core.Emulator.Battle.MonsterTable.Length + i) * 0x20);
                }
            }
        }

        public static void EquipChangesRun(int inventorySize) {
            for (int slot = 0; slot < Core.Emulator.Battle.CharacterTable.Length; slot++) {
                if ((spiritEaterSlot & ( 1 << slot)) != 0) { // If Else performance?
                    if (Core.Emulator.Battle.CharacterTable[slot].SP == Core.Emulator.Battle.CharacterTable[slot].Max_MP) {
                        Core.Emulator.Battle.CharacterTable[slot].SP_Regen -= Globals.DICTIONARY.ItemList[159].Special_Ammount; // TODO LoDDictionary 
                        spiritEaterCheck = true;
                    } else if (spiritEaterCheck) {
                        Core.Emulator.Battle.CharacterTable[slot].SP_Regen = Core.Emulator.Memory.SecondaryCharacterTable[Globals.PARTY_SLOT[slot]].SP_Regen;
                        spiritEaterCheck = false;
                    }

                } else if ((harpoonSlot & (1 << slot)) != 0) {
                    var sp = Core.Emulator.Battle.CharacterTable[slot].SP;
                    if (sp >= 400 && Core.Emulator.Battle.CharacterTable[slot].Action == 10) {
                        if (sp == 500) {
                            Core.Emulator.WriteAoB(Globals.C_POINT - 0x388 * slot + 0xC0, "00 00 00 04"); // TODO
                            Core.Emulator.Battle.CharacterTable[slot].SP = 200;
                            Core.Emulator.Battle.CharacterTable[slot].DragoonTurns = 2;
                        } else {
                            Core.Emulator.WriteAoB(Globals.C_POINT - 0x388 * slot + 0xC0, "00 00 00 03"); // TODO
                            Core.Emulator.Battle.CharacterTable[slot].SP = 100;
                            Core.Emulator.Battle.CharacterTable[slot].DragoonTurns = 1;
                        }
                    }

                    if (harpoonCheck && Core.Emulator.Battle.CharacterTable[slot].DragoonTurns == 0) {
                        harpoonCheck = false;
                    }

                } else if ((elementArrowSlot & (1 << slot)) != 0) {
                    var currentAction = Core.Emulator.Battle.CharacterTable[slot].Action;
                    if (elementArrowLastAction != currentAction) {
                        elementArrowLastAction = currentAction;
                        if (elementArrowLastAction == 8) {
                            Core.Emulator.Battle.CharacterTable[slot].Weapon_Element = elementArrowElement;
                            elementArrowTurns += 1;
                        } else {
                            if (elementArrowLastAction == 10) {
                                Core.Emulator.Battle.CharacterTable[slot].Weapon_Element = 0;
                            }

                            if (elementArrowTurns == 4) {
                                elementArrowTurns = 0;
                                if (Core.Emulator.ReadUInt("GOLD") >= 100) { // TODO
                                    for (int x = 0; x < inventorySize; x++) {
                                        if (Core.Emulator.ReadByte("INVENTORY", x) == 255) {
                                            Core.Emulator.WriteByte("INVENTORY", elementArrowItem, x);
                                            Core.Emulator.WriteByte("INVENTORY_SIZE", (byte) (Core.Emulator.ReadByte("INVENTORY_SIZE") + 1));
                                            Core.Emulator.WriteUInt("GOLD", Core.Emulator.ReadUInt("GOLD") - 100);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                } else if ((dragonBeaterSlot & (1 << slot)) != 0) {
                    if (Core.Emulator.Battle.CharacterTable[slot].Action == 136) {
                        var damageSlot = Core.Emulator.ReadUShort("DAMAGE_SLOT1"); // TODO
                        if (damageSlot != 0) {
                            ushort HP = Core.Emulator.Battle.CharacterTable[slot].HP;
                            Core.Emulator.Battle.CharacterTable[slot].HP = (ushort) Math.Min(HP + Math.Round(damageSlot * 0.02) + 2, HP);
                            Core.Emulator.WriteUShort("DAMAGE_SLOT", 0); // TODO
                        }
                    }

                } else if ((batteryGloveSlot & (1 << slot)) != 0) {
                    if ((Core.Emulator.Battle.CharacterTable[slot].Action == 136 || Core.Emulator.Battle.CharacterTable[slot].Action == 26) && (batteryGloveLastAction != 136 || batteryGloveLastAction != 26)) {
                        batteryGloveCharge++;
                        if (batteryGloveCharge == 7) {
                            Core.Emulator.Battle.CharacterTable[slot].AT = (ushort) Math.Round(Core.Emulator.Battle.CharacterTable[slot].AT * 2.5);
                        } else if (batteryGloveCharge > 7) {
                            batteryGloveCharge = 1;
                            Core.Emulator.Battle.CharacterTable[slot].AT = Core.Emulator.Battle.CharacterTable[slot].OG_AT;
                        }

                        batteryGloveLastAction = Core.Emulator.Battle.CharacterTable[slot].Action;
                    }

                } else if ((giantAxeSlot & (1 << slot)) != 0) {
                    var action = Core.Emulator.Battle.CharacterTable[slot].Action;
                    if (action == 136 && giantAxeLastAction != action) {
                        if (new Random().Next(0, 9) < 2) {
                            Core.Emulator.Battle.CharacterTable[slot].Guard = 1;
                        }
                    }
                    giantAxeLastAction = action;
                }

                if ((fakeLegendCasqueSlot & (1 << slot)) != 0) {
                    if (fakeLegendCasqueCheck[slot] && Core.Emulator.Battle.CharacterTable[slot].Guard == 1) {
                        if (new Random().Next(0, 9) < 3) {
                            Core.Emulator.Battle.CharacterTable[slot].MDF += 40;
                        }
                        fakeLegendCasqueCheck[slot] = false;
                    }
                    if (!fakeLegendCasqueCheck[slot] && ((Core.Emulator.Battle.CharacterTable[slot].Action & 8) != 0)) {
                        Core.Emulator.Battle.CharacterTable[slot].MDF = Core.Emulator.Battle.CharacterTable[slot].OG_MDF;
                        fakeLegendCasqueCheck[slot] = true;
                    }

                } else if ((legendCasqueSlot & (1 << slot)) != 0) {
                    if (legendCasqueCheckMDF[slot] && Core.Emulator.Battle.CharacterTable[slot].Guard == 1) {
                        if (legendCasqueCount[slot] >= 3) {
                            Core.Emulator.Battle.CharacterTable[slot].MDF = (ushort) Math.Ceiling(Core.Emulator.Battle.CharacterTable[slot].MDF * 1.2);
                            legendCasqueCount[slot] = 0;
                        } else {
                            legendCasqueCount[slot] += 1;
                        }
                        legendCasqueCheckMDF[slot] = false;
                    }

                    if (legendCasqueCheckShield[slot] && Core.Emulator.Battle.CharacterTable[slot].Guard == 0 && (Core.Emulator.Battle.CharacterTable[slot].Action & 8) == 0) {
                        if (new Random().Next(0, 9) < 1) {
                            Core.Emulator.Battle.CharacterTable[slot].MagicalShield = Math.Max((byte) 1, Core.Emulator.Battle.CharacterTable[slot].MagicalShield);
                        }
                        legendCasqueCheckShield[slot] = false;
                    }
                    if ((legendCasqueCheckMDF[slot] || legendCasqueCheckShield[slot]) && (Core.Emulator.Battle.CharacterTable[slot].Action & 8) != 0) {
                        if (!legendCasqueCheckMDF[slot] && legendCasqueCount[slot] == 0) {
                            Core.Emulator.Battle.CharacterTable[slot].MDF = Core.Emulator.Battle.CharacterTable[slot].OG_MDF;
                        }
                        legendCasqueCheckMDF[slot] = true;
                        legendCasqueCheckShield[slot] = true;
                    }
                }

                if ((fakeLegendArmorSlot & (1 << slot)) != 0) {
                    if (fakeLegendArmorCheck[slot] && Core.Emulator.Battle.CharacterTable[slot].Guard == 1) {
                        if (new Random().Next(0, 9) < 3) {
                            Core.Emulator.Battle.CharacterTable[slot].DF += 40;
                        }
                        fakeLegendArmorCheck[slot] = false;
                    }
                    if (!fakeLegendArmorCheck[slot] && ((Core.Emulator.Battle.CharacterTable[slot].Action & 8) != 0)) {
                        Core.Emulator.Battle.CharacterTable[slot].DF = Core.Emulator.Battle.CharacterTable[slot].OG_DF;
                        fakeLegendArmorCheck[slot] = true;
                    }

                } else if ((armorOfLegendSlot & (1 << slot)) != 0) {
                    if (armorOfLegendCheckDF[slot] && Core.Emulator.Battle.CharacterTable[slot].Guard == 1) {
                        if (armorOfLegendCount[slot] >= 3) {
                            Core.Emulator.Battle.CharacterTable[slot].DF = (ushort) Math.Ceiling(Core.Emulator.Battle.CharacterTable[slot].DF * 1.2);
                            armorOfLegendCount[slot] = 0;
                        } else {
                            armorOfLegendCount[slot] += 1;
                        }
                        armorOfLegendCheckDF[slot] = false;
                    }
                    if (armorOfLegendCheckShield[slot] && Core.Emulator.Battle.CharacterTable[slot].Guard == 0 && (Core.Emulator.Battle.CharacterTable[slot].Action & 8) == 0) {
                        if (new Random().Next(0, 9) < 1) {
                            Core.Emulator.Battle.CharacterTable[slot].PhysicalShield = Math.Max((byte) 1, Core.Emulator.Battle.CharacterTable[slot].PhysicalShield);
                        }
                        armorOfLegendCheckShield[slot] = false;
                    }
                    if ((armorOfLegendCheckDF[slot] || armorOfLegendCheckShield[slot]) && (Core.Emulator.Battle.CharacterTable[slot].Action & 8) != 0) {
                        if (!armorOfLegendCheckDF[slot] && armorOfLegendCount[slot] == 0) {
                            Core.Emulator.Battle.CharacterTable[slot].DF = Core.Emulator.Battle.CharacterTable[slot].OG_DF;
                        }
                        armorOfLegendCheckDF[slot] = true;
                        armorOfLegendCheckShield[slot] = true;
                    }
                }

                if ((soasAnkhSlot & (1 << slot)) != 0) {
                    bool alive = false;
                    int kill = -1;
                    int lastPartyID = -1;
                    if (Core.Emulator.Battle.CharacterTable[slot].HP == 0) {
                        for (int x = 0; x < 3; x++) {
                            if (x != slot && Globals.PARTY_SLOT[slot] < 9 && Core.Emulator.Battle.CharacterTable[x].HP > 0) {
                                alive = true;
                            }
                        }

                        if (alive) {
                            for (int x = 0; x < 3; x++) {
                                if (kill == -1 && new Random().Next(0, 9) < 5 && Core.Emulator.Battle.CharacterTable[x].HP > 0) {
                                    kill = x;
                                } else {
                                    lastPartyID = x;
                                }
                            }
                        }

                        if (kill != -1) {
                            Core.Emulator.Battle.CharacterTable[kill].HP = 0;
                            Core.Emulator.Battle.CharacterTable[kill].Action = 192;
                        } else {
                            Core.Emulator.Battle.CharacterTable[lastPartyID].HP = 0;
                            Core.Emulator.Battle.CharacterTable[lastPartyID].Action = 192;
                        }
                        Core.Emulator.Battle.CharacterTable[slot].HP = 1;
                    } else {
                        Core.Emulator.Battle.CharacterTable[slot].Max_HP = 0;
                        Core.Emulator.Battle.CharacterTable[slot].Revive = 0;
                        Core.Emulator.Battle.CharacterTable[slot].Action = 192;
                    }
                }
            }
        }

        public static void MagicInventoryHandler() {
            for (int i = 0; i < 3; i++) {
                if (Globals.PARTY_SLOT[i] == 2 || Globals.PARTY_SLOT[i] == 8) {
                    if (Core.Emulator.Battle.CharacterTable[i].Action == 10) {

                    }

                    /*if (dartBurnStacks > 0) {
                    if (Core.Emulator.Battle.CharacterTable[i].Read("Action") == 10) {
                        int menu = Core.Emulator.Battle.CharacterTable[i].Read("Menu");

                        if (menu == 96 || menu == 98) {
                            byte[] icons = { 1, 4, 9, 3, 3 };
                            Core.Emulator.Battle.CharacterTable[i].Write("Menu", 225 - (96 - Core.Emulator.Battle.CharacterTable[i].Read("Menu")));
                            Thread.Sleep(60);
                            for (int x = menu == 96 ? 1 : 0; x < icons.Length; x++) {
                                Core.Emulator.WriteByte(Globals.M_POINT + 0xD34 + (x - (menu == 96 ? 1 : 0)) * 0x2, icons[x]);
                            }
                        }

                        int iconCount = Core.Emulator.ReadByte(Globals.M_POINT + 0xD32);
                        int iconSelected = Core.Emulator.ReadByte(Globals.M_POINT + 0xD46);

                        if (burnStackSelected) {
                            if ((iconCount == 4 && (iconSelected == 0 || iconSelected == 2)) || (iconCount == 5 && (iconSelected == 1 || iconSelected == 3))) {
                                burnStackSelected = false;
                                double burnAmount = 1 + (dartBurnStacks * damagePerBurn);
                                Core.Emulator.WriteText(Globals.DRAGOON_SPELLS[0].Description_Pointer + 0x1E, "1.00");
                                Core.Emulator.WriteText(Globals.DRAGOON_SPELLS[1].Description_Pointer + 0x1E, "1.00");
                                Core.Emulator.WriteText(Globals.DRAGOON_SPELLS[2].Description_Pointer + 0x1E, "1.00");
                                Core.Emulator.WriteText(Globals.DRAGOON_SPELLS[3].Description_Pointer + 0x20, "1.00");

                                if (dartBurnStacks == dartMaxBurnStacks) {
                                    Core.Emulator.WriteByte("SPELL_TABLE", 10, 0x7 + (0 * 0xC));
                                    Core.Emulator.WriteByte("SPELL_TABLE", 20, 0x7 + (1 * 0xC));
                                    Core.Emulator.WriteByte("SPELL_TABLE", 30, 0x7 + (2 * 0xC));
                                    Core.Emulator.WriteByte("SPELL_TABLE", 80, 0x7 + (3 * 0xC));
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
                                Core.Emulator.WriteText(Globals.DRAGOON_SPELLS[0].Description_Pointer + 0x1E, flameshotDesc.Substring(0, Math.Min(4, flameshotDesc.Length)));
                                Core.Emulator.WriteText(Globals.DRAGOON_SPELLS[1].Description_Pointer + 0x1E, explosionDesc.Substring(0, Math.Min(4, explosionDesc.Length)));
                                Core.Emulator.WriteText(Globals.DRAGOON_SPELLS[2].Description_Pointer + 0x1E, finalBurstDesc.Substring(0, Math.Min(4, finalBurstDesc.Length)));
                                Core.Emulator.WriteText(Globals.DRAGOON_SPELLS[3].Description_Pointer + 0x20, redEyeDesc.Substring(0, Math.Min(4, redEyeDesc.Length)));

                                if (dartBurnStacks == dartMaxBurnStacks) {
                                    Core.Emulator.WriteByte("SPELL_TABLE", 1, 0x7 + (0 * 0xC));
                                    Core.Emulator.WriteByte("SPELL_TABLE", 1, 0x7 + (1 * 0xC));
                                    Core.Emulator.WriteByte("SPELL_TABLE", 1, 0x7 + (2 * 0xC));
                                    Core.Emulator.WriteByte("SPELL_TABLE", 1, 0x7 + (3 * 0xC));
                                }
                            }
                        }
                    }
                }*/
                }
            }
        }

        class JeweledHammerHotkey : Hotkey {
            public JeweledHammerHotkey(int buttonPress) : base(buttonPress) {

            }

            public override void Init() {
                if (jeweledHammerSlot != 0) {
                    if (meruEnhanceDragoon) {
                        if (Constants.REGION == Region.NTA) {
                            Core.Emulator.WriteAoB(Globals.DRAGOON_SPELLS[24].Description_Pointer, "35 00 39 00 4C 00 3D 00 4A 00 00 00 31 00 32 00 30 00 00 00 1A 00 16 00 15 00 0F 00 FF A0");
                            Core.Emulator.WriteAoB(Globals.DRAGOON_SPELLS[27].Description_Pointer, "35 00 39 00 4C 00 3D 00 4A 00 00 00 31 00 32 00 30 00 00 00 1A 00 18 00 15 00 0F 00 FF A0");
                            Core.Emulator.WriteAoB(Globals.DRAGOON_SPELLS[28].Description_Pointer, "35 00 39 00 4C 00 3D 00 4A 00 00 00 31 00 32 00 30 00 00 00 16 00 19 00 15 00 15 00 0F 00 FF A0");
                        }
                        Core.Emulator.WriteByte("SPELL_TABLE", freezingRingMP, 0x7 + (24 * 0xC)); //Freezing Ring MP
                        Core.Emulator.WriteByte("SPELL_TABLE", rainbowBreathMP, 0x7 + (25 * 0xC)); //Rainbow Breath MP
                        Core.Emulator.WriteByte("SPELL_TABLE", diamonDustMP, 0x7 + (27 * 0xC)); //Diamond Dust MP
                        Core.Emulator.WriteByte("SPELL_TABLE", blueSeaDragonMP, 0x7 + (28 * 0xC)); //Blue Sea Dragon MP
                        meruEnhanceDragoon = false;
                        Constants.WriteGLogOutput("Meru's dragoon magic has returned to normal.");
                    } else {
                        if (Constants.REGION == Region.NTA) {
                            Core.Emulator.WriteAoB(Globals.DRAGOON_SPELLS[24].Description_Pointer, "35 00 39 00 4C 00 3D 00 4A 00 00 00 31 00 32 00 30 00 00 00 1D 00 15 00 15 00 0F 00 FF A0");
                            Core.Emulator.WriteAoB(Globals.DRAGOON_SPELLS[27].Description_Pointer, "35 00 39 00 4C 00 3D 00 4A 00 00 00 31 00 32 00 30 00 00 00 1D 00 1D 00 15 00 0F 00 FF A0");
                            Core.Emulator.WriteAoB(Globals.DRAGOON_SPELLS[28].Description_Pointer, "35 00 39 00 4C 00 3D 00 4A 00 00 00 31 00 32 00 30 00 00 00 17 00 16 00 15 00 15 00 0F 00 FF A0");
                        }
                        Core.Emulator.WriteByte("SPELL_TABLE", enhancedFreezingRingMP, 0x7 + (24 * 0xC)); //Freezing Ring MP
                        Core.Emulator.WriteByte("SPELL_TABLE", enhancedRainbowBreathMP, 0x7 + (25 * 0xC)); //Rainbow Breath MP
                        Core.Emulator.WriteByte("SPELL_TABLE", enhancedDiamondDustMP, 0x7 + (27 * 0xC)); //Diamond Dust MP
                        Core.Emulator.WriteByte("SPELL_TABLE", enhancedBlueSeaDragonMP, 0x7 + (28 * 0xC)); //Blue Sea Dragon MP
                        meruEnhanceDragoon = true;
                        Constants.WriteGLogOutput("Meru will now consume more MP for bonus effects.");
                    }
                } else {
                    Constants.WriteGLogOutput("Jeweled Hammer not equipped.");
                }
                Globals.LAST_HOTKEY = Constants.GetTime();
                return;
            }
        }

        class DragonBeaterHotkey: Hotkey {
            public DragonBeaterHotkey(int buttonPress) : base(buttonPress) {

            }

            public override void Init() {
                if (dragonBeaterSlot != 0) {
                    if (!checkRoseDamage) {
                        if (roseEnhanceDragoon) {
                            if (Constants.REGION == Region.NTA) {
                                Core.Emulator.WriteAoB(Globals.DRAGOON_SPELLS[15].Description_Pointer, "22 00 39 00 4A 00 43 00 00 00 31 00 32 00 30 00 00 00 1A 00 1E 00 15 00 0F 00 00 00 10 00 00 00 26 00 2E 00 FF A0");
                                Core.Emulator.WriteAoB(Globals.DRAGOON_SPELLS[16].Description_Pointer, "22 00 39 00 4A 00 43 00 00 00 31 00 32 00 30 00 00 00 18 00 1E 00 1A 00 0F 00 FF A0");
                                Core.Emulator.WriteAoB(Globals.DRAGOON_SPELLS[19].Description_Pointer, "22 00 39 00 4A 00 43 00 00 00 31 00 32 00 30 00 00 00 16 00 1B 00 1D 00 15 00 0F 00 00 00 10 00 00 00 26 00 2E 00 FF A0");
                            }
                            Core.Emulator.WriteByte("SPELL_TABLE", astralDrainMP, 0x7 + (15 * 0xC)); //Astral Drain MP
                            Core.Emulator.WriteByte("SPELL_TABLE", deathDimensionMP, 0x7 + (16 * 0xC)); //Death Dimension MP
                            Core.Emulator.WriteByte("SPELL_TABLE", darkDragonMP, 0x7 + (19 * 0xC)); //Dark Dragon MP
                            roseEnhanceDragoon = false;
                            Constants.WriteGLogOutput("Rose's dragoon magic has returned to normal.");
                        } else {
                            if (Constants.REGION == Region.NTA) {
                                Core.Emulator.WriteAoB(Globals.DRAGOON_SPELLS[15].Description_Pointer, "22 00 39 00 4A 00 43 00 00 00 31 00 32 00 30 00 00 00 1D 00 17 00 1A 00 0F 00 00 00 10 00 00 00 26 00 2E 00 FF A0");
                                Core.Emulator.WriteAoB(Globals.DRAGOON_SPELLS[16].Description_Pointer, "22 00 39 00 4A 00 43 00 00 00 31 00 32 00 30 00 00 00 1C 00 1E 00 1A 00 0F 00 FF A0");
                                Core.Emulator.WriteAoB(Globals.DRAGOON_SPELLS[19].Description_Pointer, "22 00 39 00 4A 00 43 00 00 00 31 00 32 00 30 00 00 00 16 00 16 00 1A 00 15 00 0F 00 00 00 10 00 00 00 26 00 2E 00 FF A0");
                            }
                            Core.Emulator.WriteByte("SPELL_TABLE", enhancedAstralDrainMP, 0x7 + (15 * 0xC)); //Astral Drain MP
                            Core.Emulator.WriteByte("SPELL_TABLE", enhancedDeathDimensionMP, 0x7 + (16 * 0xC)); //Death Dimension MP
                            Core.Emulator.WriteByte("SPELL_TABLE", enhancedDarkDragonMP, 0x7 + (19 * 0xC)); //Dark Dragon MP
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
            }
        }
    }
}
