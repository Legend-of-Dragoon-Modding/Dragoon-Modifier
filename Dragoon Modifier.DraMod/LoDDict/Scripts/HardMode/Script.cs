using Dragoon_Modifier.DraMod.UI;
using Dragoon_Modifier.Emulator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.LoDDict.Scripts.HardMode {
    class Script : IScript {
        
        const short heatBladeBuff = 7;
        const short shadowCutterBuff = 9;
        const short sparkleArrowBuff = 8;
        const short morningStarBuff = -20;

        const byte heatBladeID = 2;
        const byte shadowCutterID = 14;
        const byte sparkleArrowID = 28;
        const byte morningStarID = 35;

        private byte spiritEaterSlot = 0;
        private bool spiritEaterCheck = false;
        private byte harpoonSlot = 0;
        private bool harpoonCheck = false;
        private byte elementArrowSlot = 0;
        private byte elementArrowElement = 0x80;
        private byte elementArrowItem = 0xC3;
        private byte dragonBeaterSlot = 0;
        private byte batteryGloveSlot = 0;
        private byte batteryGloveLastAction = 0;
        private byte batteryGloveCharge = 0;
        private byte jeweledHammerSlot = 0;
        private byte giantAxeSlot = 0;
        private byte giantAxeLastAction = 0;

        private byte fakeLegendCasqueSlot = 0;
        private bool[] fakeLegendCasqueCheck = { false, false, false };
        private byte fakeLegendArmorSlot = 0;
        private bool[] fakeLegendArmorCheck = { false, false, false };
        private byte legendCasqueSlot = 0;
        private byte[] legendCasqueCount = { 0, 0, 0 };
        private bool[] legendCasqueCheckMDF = { false, false, false };
        private bool[] legendCasqueCheckShield = { false, false, false };
        private byte armorOfLegendSlot = 0;
        private byte[] armorOfLegendCount = { 0, 0, 0 };
        private bool[] armorOfLegendCheckDF = { false, false, false };
        private bool[] armorOfLegendCheckShield = { false, false, false };
        private byte soasSiphonRingSlot = 0;
        private byte soasAnkhSlot = 0;

        private Characters.ICharacter[] _character = new Characters.ICharacter[9] {
            new Characters.Dart(),
            new Characters.Dart(), // Placeholder
            new Characters.Dart(), // Placeholder
            new Characters.Dart(), // Placeholder
            new Characters.Dart(), // Placeholder
            new Characters.Dart(), // Placeholder
            new Characters.Dart(), // Placeholder
            new Characters.Dart(), // Placeholder
            new Characters.Dart() // Placeholder
        };


        public void BattleRun(IEmulator emulator, ILoDDictionary loDDictionary, IUIControl uiControl) {
            for (byte slot = 0; slot < emulator.Battle.CharacterTable.Length; slot++) {
                var character = emulator.Battle.CharacterTable[slot];
                byte dragoonSpecialAttack = 0;
                _character[character.ID].Run(emulator, slot, dragoonSpecialAttack);
                switch (character.ID) {
                    case 0: // Dart
                        SoulEater(emulator, loDDictionary, character, slot);
                        break;
                    case 1: // Lavitz
                    case 5: // Albert
                        Harpoon(emulator, loDDictionary, character, slot);
                        break;
                    case 2: // Shana
                    case 8: // Miranda
                        // Elemental Arrow
                        break;
                    case 3: // Rose
                        DragonBeater(emulator, loDDictionary, character, slot);
                        break;
                    case 4: // Haschel
                        BatteryGlove(emulator, loDDictionary, character, slot);
                        break;
                    case 6: // Meru
                        break;
                    case 7: // Kongol
                        GiantAxe(emulator, loDDictionary, character, slot);
                        break;
                }

            }
        }

        public void BattleSetup(IEmulator emulator, ILoDDictionary loDDictionary, IUIControl uiControl) {
            

            BattleChapter3Buffs(emulator);
            KongolSpeedNerf(emulator);
        }


        public void FieldRun(IEmulator emulator, ILoDDictionary loDDictionary, IUIControl uiControl) {
            throw new NotImplementedException();
        }

        public void FieldSetup(IEmulator emulator, ILoDDictionary loDDictionary, IUIControl uiControl) {
            FieldChapter3Buffs(emulator);
        }

        private static void BattleChapter3Buffs(IEmulator emulator) {
            if (emulator.Memory.Chapter < 3) {
                return;
            }

            for (int character = 0; character < 9; character++) {
                short modif = 0;
    
                switch (emulator.Memory.CharacterTable[character].Weapon) {
                    case heatBladeID:
                        modif = heatBladeBuff;
                        break;

                    case shadowCutterID:
                        modif = shadowCutterBuff;
                        break;

                    case sparkleArrowID:
                        modif = sparkleArrowBuff;
                        break;

                    case morningStarID:
                        modif = morningStarBuff;
                        break;
                }

                emulator.Memory.SecondaryCharacterTable[character].EquipAT = (ushort) (emulator.Memory.SecondaryCharacterTable[character].EquipAT + modif);
            }
        }

        private static void FieldChapter3Buffs(IEmulator emulator) {
            if (emulator.Memory.Chapter < 3) {
                var morningStar = emulator.Memory.Item[morningStarID];
                emulator.WriteUInt(morningStar.DescriptionPointer, 0xA0FFA0FF); // Double end flag
                return;
            }

            Emulator.Memory.IEquipment item = (Emulator.Memory.IEquipment) emulator.Memory.Item[heatBladeID];
            item.AT = (byte) (item.AT + heatBladeBuff);

            item = (Emulator.Memory.IEquipment) emulator.Memory.Item[shadowCutterID];
            item.AT = (byte) (item.AT + shadowCutterBuff);

            item = (Emulator.Memory.IEquipment) emulator.Memory.Item[sparkleArrowID];
            item.AT = (byte) (item.AT + shadowCutterBuff);

            item = (Emulator.Memory.IEquipment) emulator.Memory.Item[morningStarID];
            item.AT = (byte) (item.AT + morningStarID);
        }

        private static void KongolSpeedNerf(IEmulator emulator) {
            emulator.Memory.SecondaryCharacterTable[7].EquipSPD = (ushort) (emulator.Memory.SecondaryCharacterTable[7].EquipSPD / 2);
        }

        private void SoulEater(IEmulator emulator, ILoDDictionary loDDictionary, Emulator.Memory.Battle.Character character, byte slot) {
            if ((spiritEaterSlot & (1 << slot)) == 0) {
                return;
            }

            if (character.SP == character.DLV * 100) {
                var spiritEater = (IEquipment) loDDictionary.Item[159];
                character.SP_Regen = (short) (emulator.Memory.SecondaryCharacterTable[0].SP_Regen - spiritEater.SpecialBonusAmmount);
                spiritEaterCheck = true;
                return;
            }

            if (spiritEaterCheck) {
                character.SP_Regen = emulator.Memory.SecondaryCharacterTable[character.ID].SP_Regen;
            }
        }

        private void Harpoon(IEmulator emulator, ILoDDictionary loDDictionary, Emulator.Memory.Battle.Character character, byte slot) {
            if ((harpoonSlot & (1 << slot)) == 0) {
                return;
            }

            ushort sp = character.SP;

            if (sp >= 400 && character.Action == 10) {
                harpoonCheck = true;

                if (sp == 500) {
                    character.Speed_Up_Turn = 0;
                    character.Speed_Down_Turn = 4;
                    character.SP = 200;
                    character.DragoonTurns = 2;
                } else {
                    character.Speed_Up_Turn = 0;
                    character.Speed_Down_Turn = 0;
                    character.SP = 100;
                    character.DragoonTurns = 1;
                }
            }

            if (harpoonCheck && character.DragoonTurns == 0) {
                harpoonCheck = false;
            }

        }

        // Elemental Arrow

        private void DragonBeater(IEmulator emulator, ILoDDictionary loDDictionary, Emulator.Memory.Battle.Character character, byte slot) {
            if ((dragonBeaterSlot & (1 << slot)) == 0) {
                return;
            }

            ushort damageSlot = emulator.ReadUShort("DAMAGE_SLOT1");

            if (damageSlot == 0) {
                return;
            }

            ushort HP = character.HP;
            character.HP = (ushort) Math.Min(HP + Math.Round(damageSlot * 0.02) + 2, HP);
            emulator.WriteUShort("DAMAGE_SLOT1", 0);
        }

        private void BatteryGlove(IEmulator emulator, ILoDDictionary loDDictionary, Emulator.Memory.Battle.Character character, byte slot) {
            if ((batteryGloveSlot & (1 << slot)) == 0) {
                return;
            }

            var action = character.Action;

            if ((action == 136 || action == 26) && !(batteryGloveLastAction == 136 || batteryGloveLastAction == 26)) {
                batteryGloveCharge += 1;
                if (batteryGloveCharge == 7) {
                    character.AT = (ushort) Math.Round(character.AT * 2.5);
                } else if (batteryGloveCharge > 7) {
                    batteryGloveCharge = 1;
                    character.AT = character.OG_AT;
                }
            }

            batteryGloveLastAction = action;
        }

        private void GiantAxe(IEmulator emulator, ILoDDictionary loDDictionary, Emulator.Memory.Battle.Character character, byte slot) {
            if ((giantAxeSlot & (1 << slot)) == 0) {
                return;
            }

            var action = character.Action;
            if (action == 136 && giantAxeLastAction != action) {
                if (new Random().Next(0,9) < 2) {
                    character.Guard = 1;
                }
            }

            giantAxeLastAction = action;
        }

        private void ResetItems() {
            soasSiphonRingSlot = 0;
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
        }

        private void SetItems(IEmulator emulator, ILoDDictionary loDDictionary) {
            for (byte slot = 0; slot < emulator.Battle.CharacterTable.Length; slot++) {
                var character = emulator.Battle.CharacterTable[slot];
                switch (character.ID) {
                    case 0: // Dart
                        if (character.Weapon == 159) { // Spirit Eater
                            spiritEaterSlot |= (byte) (1 << slot);
                        }
                        break;
                    case 1: // Lavitz
                    case 5: // Albert
                        if (character.Weapon == 160) { // Harpoon
                            harpoonSlot |= (byte) (1 << slot);
                        }
                        break;
                    case 2: // Shana
                    case 8: // Miranda
                        // Elemental Arrow
                        break;
                    case 3: // Rose
                        if (character.Weapon == 162) { // Dragon Beater
                            dragonBeaterSlot |= (byte) (1 << slot);
                        }
                        break;
                    case 4: // Haschel
                        if (character.Weapon == 163) { // Battery Glove
                            batteryGloveSlot |= (byte) (1 << slot);
                            batteryGloveCharge = 0;
                            batteryGloveLastAction = 0;
                        }
                        break;
                    case 6: // Meru
                        if (character.Weapon == 164) { // Jeweled Hammer
                            jeweledHammerSlot |= (byte) (1 << slot);
                        }
                        break;
                    case 7: // Kongol
                        if (character.Weapon == 160) { // Giant Axe
                            giantAxeSlot |= (byte) (1 << slot);
                            giantAxeLastAction = 0;
                        }
                        break;
                }

            }
        }
    }
}
