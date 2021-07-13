using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Controller {
    internal static class NoDart {
        internal static void Initialize(Emulator.IEmulator emulator, byte character) {
            Console.WriteLine("Initializing No Dart.");
            if (emulator.Memory.PartySlot[1] == character || emulator.Memory.PartySlot[2] == character) {
                Console.WriteLine("No Dart character already present.");
                return;
            }
            emulator.Battle.CharacterTable[0].Status = emulator.Memory.CharacterTable[character].Status;
            if (emulator.Battle.EncounterID == 413) { // Jiango has to have it's initial move. Otherwise he never gets a turn.
                emulator.Battle.MonsterTable[0].Action = 12; // Play the "This is Jiango" part
                while (emulator.Battle.MonsterTable[0].Action != 44) { // Jiango's initial move is over.
                    if (Constants.Run && emulator.Memory.GameState != Emulator.GameState.Battle) {
                        return;
                    }
                    Thread.Sleep(50);
                }
            }

            emulator.Battle.CharacterTable[0].Action = 10; // Force character's turn in Dragoon form.

            while (emulator.Battle.CharacterTable[0].Menu != 96) { // Wait for the NoDart's character turn to start
                if (Constants.Run && emulator.Memory.GameState != Emulator.GameState.Battle) {
                    return;
                }
                Thread.Sleep(50);
            }

            emulator.Battle.CharacterTable[0].Dragoon = 0x20; // Make sure we have Red-Eye Dragoon

            emulator.Memory.PartySlot[0] = character;
            emulator.WriteByte("PARTY_SLOT", character, 0x234E); // Secondary ID

            emulator.Battle.CharacterTable[0].Image = character;
            emulator.Battle.CharacterTable[0].Weapon = emulator.Memory.CharacterTable[character].Weapon;
            emulator.Battle.CharacterTable[0].Helmet = emulator.Memory.CharacterTable[character].Helmet;
            emulator.Battle.CharacterTable[0].Armor = emulator.Memory.CharacterTable[character].Armor;
            emulator.Battle.CharacterTable[0].Shoes = emulator.Memory.CharacterTable[character].Shoes;
            emulator.Battle.CharacterTable[0].Accessory = emulator.Memory.CharacterTable[character].Accessory;

            emulator.Battle.CharacterTable[0].LV = emulator.Memory.CharacterTable[character].Level;
            emulator.Battle.CharacterTable[0].DLV = 1;
            emulator.Battle.CharacterTable[0].SP = 100;

            byte dlv = emulator.Memory.SecondaryCharacterTable[character].DragoonLevel;

            emulator.Battle.CharacterTable[0].DragoonSpellID = character; // ID has to match, otherwise you get all Flameshots
            Dictionary<byte, byte> dmagic5 = new Dictionary<byte, byte> { // TODO this shouldn't be hardcoded, in case we want to change Dragoon Magic
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

            switch (dlv) { // Setting Dragoon Spell slots based on the D'LV
                case 5:
                    emulator.Battle.CharacterTable[0].DragoonSpell[0] = dmagic1[character];
                    emulator.Battle.CharacterTable[0].DragoonSpell[1] = dmagic2[character];
                    if (character != 7) {
                        emulator.Battle.CharacterTable[0].DragoonSpell[2] = dmagic3[character];
                        emulator.Battle.CharacterTable[0].DragoonSpell[3] = dmagic5[character];
                        break;
                    }
                    emulator.Battle.CharacterTable[0].DragoonSpell[2] = dmagic5[character];
                    emulator.Battle.CharacterTable[0].DragoonSpell[3] = 0xFF;
                    break;
                case 4:
                case 3:
                    emulator.Battle.CharacterTable[0].DragoonSpell[0] = dmagic1[character];
                    emulator.Battle.CharacterTable[0].DragoonSpell[1] = dmagic2[character];
                    emulator.Battle.CharacterTable[0].DragoonSpell[2] = dmagic3[character];
                    emulator.Battle.CharacterTable[0].DragoonSpell[3] = 0xFF;
                    break;
                case 2:
                    emulator.Battle.CharacterTable[0].DragoonSpell[0] = dmagic1[character];
                    emulator.Battle.CharacterTable[0].DragoonSpell[1] = dmagic2[character];
                    emulator.Battle.CharacterTable[0].DragoonSpell[2] = 0xFF;
                    emulator.Battle.CharacterTable[0].DragoonSpell[3] = 0xFF;
                    break;
                case 1:
                    emulator.Battle.CharacterTable[0].DragoonSpell[0] = dmagic1[character];
                    emulator.Battle.CharacterTable[0].DragoonSpell[1] = 0xFF;
                    emulator.Battle.CharacterTable[0].DragoonSpell[2] = 0xFF;
                    emulator.Battle.CharacterTable[0].DragoonSpell[3] = 0xFF;
                    break;
                case 0:
                    emulator.Battle.CharacterTable[0].DragoonSpell[0] = 0xFF;
                    emulator.Battle.CharacterTable[0].DragoonSpell[1] = 0xFF;
                    emulator.Battle.CharacterTable[0].DragoonSpell[2] = 0xFF;
                    emulator.Battle.CharacterTable[0].DragoonSpell[3] = 0xFF;
                    break;
            }


            byte special_effect = 0; // Fix for special effects
            if (emulator.Battle.CharacterTable[0].Weapon == 45) { // Destroyer Mace
                special_effect |= 1;
            }

            if (emulator.Battle.CharacterTable[0].Accessory == 157) { // Wargod Sash
                special_effect |= 2;
            }

            if (emulator.Battle.CharacterTable[0].Accessory == 158) { // Ultimate Wargod
                special_effect |= 6;
            }

            emulator.Battle.CharacterTable[0].AdditionSpecial = special_effect;

            /*
            if (!Globals.ADDITION_CHANGE) {
                AdditionsBattleChanges(0, character);
            }
            */

            // emulator.Battle.CharacterTable[0].ResetStats(character); TODO

            if (Settings.AutoTransform) {
                emulator.Battle.CharacterTable[0].Detransform();
            } else {
                emulator.Battle.CharacterTable[0].Menu = 16;
            }
            while (emulator.Battle.CharacterTable[0].Action != 9) {
                if (Constants.Run && emulator.Memory.GameState != Emulator.GameState.Battle) {
                    return;
                }
                Thread.Sleep(50);
            }

            emulator.Battle.CharacterTable[0].DLV = dlv;
            if (dlv == 0) {
                emulator.Battle.CharacterTable[0].Dragoon = 0;
            }
            emulator.Battle.CharacterTable[0].SP = emulator.Memory.SecondaryCharacterTable[character].SP;

            Console.WriteLine("No Dart complete.");
        }
    }
}
