using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dragoon_Modifier.MemoryController.Battle {
    class NoDart {
        public static void HaschelFix(byte disc) {
            Constants.WriteDebug("Haschel Fix - " + disc);
            switch (Constants.REGION) {
                case Region.NTA:
                    Emulator.WriteAoB(Constants.GetAddress("HASCHEL_FIX" + disc), $"0x80 0x80 0x80 0x00 {(disc == 1 ? "0x90 0xA0" : disc == 2 ? "0x10 0x93" : disc == 3 ? "0x68 0xA7" : "0xC0 0x94")} 0x1E 0x80" +
                    $"0x74 0x12 0x00 0x00 0x02 0x00 0x8C 0x8C 0x4D 0x52 0x47 0x1A 0x04 0x00 0x00 0x00 0x28 0x00 0x00 0x00 0x02 0x00 0x00 0x00 0x2C 0x00 0x00 0x00 0x68 0x00 0x00 0x00 0x94 0x00 0x00 0x00 0xD4" +
                    $"0x11 0x00 0x00 0x68 0x12 0x00 0x00 0xB0 0x05 0x02 0x00 0x00 0x00 0x8C 0x8C 0x00 0x00 0x00 0x01 0x00 0x02 0x00 0x03 0x00 0x04 0x00 0x05 0x00 0x06 0x00 0x07 0x00 0x08 0x00 0x09 0x00 0x0A" +
                    $"0x00 0x0B 0x00 0x0C 0x00 0x0D 0x00 0x0E 0x00 0x0F 0x00 0x10 0x00 0x11 0x00 0x12 0x00 0x13 0x00 0x14 0x00 0x15 0x00 0x16 0x00 0x17 0x00 0x18 0x00 0x19 0x00 0x1A 0x00 0x1B");
                    break;
                case Region.JPN:
                    Emulator.WriteAoB(Constants.GetAddress("HASCHEL_FIX" + disc), $"80 80 80 00 {(disc == 1 ? "98 90" : disc == 2 ? "18 83" : disc == 3 ? "70 97" : "C8 84")} 1E 80 74 12 00 00 02 00 00 00 4D 52" +
                    $"47 1A 04 00 00 00 28 00 00 00 02 00 00 00 2C 00 00 00 68 00 00 00 94 00 00 00 D4 11 00 00 68 12 00 00 B0 05 02 00 00 00 8C 8C 00 00 00 01 00 02 00 03 00 04 00 05 00 06 00 07 00 08 00 09" +
                    $"00 0A 00 0B 00 0C 00 0D 00 0E 00 0F 00 10 00 11 00 12 00 13 00 14 00 15 00 16 00 17 00 18 00 19 00 1A 00 1B 00 1C 00 1D 00 1E 00 1F 00 20 00 21 00 22 00 23 00 24 00 25 00 26 00 27 00 28" +
                    $"00 29 00 2A 00 2B 00 2C 00 2D 00 2E 00 2F 00 30 00 31 00 32 00 33 D4 11 00 00 B0 05 02 00 00 00 00 00 53 53 68 64 FF FF FF FF 80 00 00 00 0C 11 00 00 02 01 00 00 B6 07 00 00");
                    break;
            }
        }

        public static void Initialize(byte character) {
            Globals.BattleController.CharacterTable[0].Status = Globals.MemoryController.CharacterTable[character].Status;
            if (Globals.BattleController.EncounterID == 413) {
                Globals.BattleController.MonsterTable[0].Action = 12;
                Thread.Sleep(1500);
            }
            Globals.BattleController.CharacterTable[0].Action = 10;

            while (true) { // Wait until Dart's turn starts
                if (Globals.GAME_STATE != 1) {
                    return;
                }
                if (Globals.BattleController.CharacterTable[0].Menu == 96) {
                    break;
                }
                Thread.Sleep(50);
            }

            Globals.BattleController.CharacterTable[0].DragoonTurns = 1;
            Globals.BattleController.CharacterTable[0].Dragoon = 0x20; // Make sure we don't have Red-Eye
            Globals.MemoryController.PartySlot[0] = character;

            Emulator.WriteByte("PARTY_SLOT", character, 0x234E); // Secondary ID

            Globals.BattleController.CharacterTable[0].Image = character;
            Globals.BattleController.CharacterTable[0].Weapon = Globals.MemoryController.CharacterTable[character].Weapon;
            Globals.BattleController.CharacterTable[0].Helmet = Globals.MemoryController.CharacterTable[character].Helmet;
            Globals.BattleController.CharacterTable[0].Armor = Globals.MemoryController.CharacterTable[character].Armor;
            Globals.BattleController.CharacterTable[0].Shoes = Globals.MemoryController.CharacterTable[character].Shoes;
            Globals.BattleController.CharacterTable[0].Accessory = Globals.MemoryController.CharacterTable[character].Accessory;

            Globals.BattleController.CharacterTable[0].LV = Globals.MemoryController.CharacterTable[character].Level;
            Globals.BattleController.CharacterTable[0].DLV = 1;
            Globals.BattleController.CharacterTable[0].SP = 100;


            Globals.CHARACTER_TABLE[0].LV = Emulator.ReadByte("CHAR_TABLE", 0x12 + (character * 0x2C));
            Globals.CHARACTER_TABLE[0].DLV = 1;
            Globals.CHARACTER_TABLE[0].SP = 100;

            byte dlv = Globals.MemoryController.CharacterTable[character].DragoonLevel;

            /*
            #region Dragoon Magic
            Emulator.WriteByte("DRAGOON_SPELL_SLOT", character); // Magic
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
                    Emulator.WriteByte("DRAGOON_SPELL_SLOT", dmagic5[character], 4);
                    Emulator.WriteByte("DRAGOON_SPELL_SLOT", dmagic3[character], 3);
                } else {
                    Emulator.WriteByte("DRAGOON_SPELL_SLOT", 0xFF, 4);
                    Emulator.WriteByte("DRAGOON_SPELL_SLOT", dmagic5[character], 3);
                }
                Emulator.WriteByte("DRAGOON_SPELL_SLOT", dmagic2[character], 2);
                Emulator.WriteByte("DRAGOON_SPELL_SLOT", dmagic1[character], 1);
            } else if (dlv > 2) {
                Emulator.WriteByte("DRAGOON_SPELL_SLOT", 0xFF, 4);
                Emulator.WriteByte("DRAGOON_SPELL_SLOT", dmagic3[character], 3);
                Emulator.WriteByte("DRAGOON_SPELL_SLOT", dmagic2[character], 2);
                Emulator.WriteByte("DRAGOON_SPELL_SLOT", dmagic1[character], 1);
            } else if (dlv > 1) {
                Emulator.WriteByte("DRAGOON_SPELL_SLOT", 0xFF, 4);
                Emulator.WriteByte("DRAGOON_SPELL_SLOT", 0xFF, 3);
                Emulator.WriteByte("DRAGOON_SPELL_SLOT", dmagic2[character], 2);
                Emulator.WriteByte("DRAGOON_SPELL_SLOT", dmagic1[character], 1);
            } else if (dlv > 0) {
                Emulator.WriteByte("DRAGOON_SPELL_SLOT", 0xFF, 4);
                Emulator.WriteByte("DRAGOON_SPELL_SLOT", 0xFF, 3);
                Emulator.WriteByte("DRAGOON_SPELL_SLOT", 0xFF, 2);
                Emulator.WriteByte("DRAGOON_SPELL_SLOT", dmagic1[character], 1);
            } else {
                Emulator.WriteByte("DRAGOON_SPELL_SLOT", 0xFF, 4);
                Emulator.WriteByte("DRAGOON_SPELL_SLOT", 0xFF, 3);
                Emulator.WriteByte("DRAGOON_SPELL_SLOT", 0xFF, 2);
                Emulator.WriteByte("DRAGOON_SPELL_SLOT", 0xFF, 1);
            }
            #endregion

            #region Wargod/Destroyer Mace fix
            byte special_effect = 0;
            if (Globals.CHARACTER_TABLE[0].Weapon == 45) {
                special_effect |= 1;
            }

            if (Globals.CHARACTER_TABLE[0].Accessory == 157) {
                special_effect |= 2;
            }

            if (Globals.CHARACTER_TABLE[0].Accessory == 158) {
                special_effect |= 6;
            }

            Emulator.WriteByte("WARGOD", special_effect);

            #endregion

            /*
            if (!Globals.ADDITION_CHANGE) {
                AdditionsBattleChanges(0, character);
            }

            if (!Globals.DRAGOON_STAT_CHANGE) {
                DragoonStatChanges(0, character);
            }

            NoDartSetStats(0, character);
            */

            if (Globals.AUTO_TRANSFORM) {
                Globals.BattleController.CharacterTable[0].Detransform();
            } else {
                Globals.BattleController.CharacterTable[0].Menu = 16;
            }
            while (true) {
                if (Globals.GAME_STATE != 1) {
                    return;
                }
                if (Globals.BattleController.CharacterTable[0].Action == 9) {
                    break;
                }
                Thread.Sleep(50);
            }
            Globals.BattleController.CharacterTable[0].DLV = dlv;
            if (dlv == 0) {
                Globals.BattleController.CharacterTable[0].Dragoon = 0;
            }
            Globals.BattleController.CharacterTable[0].SP = Globals.MemoryController.CharacterTable[0].SP;

            Constants.WriteOutput("No Dart complete.");
        }
    }
}
