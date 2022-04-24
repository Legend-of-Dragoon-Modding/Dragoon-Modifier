using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Controller {
    internal static class NoDart {
        internal static void Initialize(Emulator.IEmulator emulator, LoDDict.ILoDDictionary loDDictionary, byte character) {
            Console.WriteLine("Initializing No Dart.");
            if (emulator.Memory.PartySlot[1] == character || emulator.Memory.PartySlot[2] == character) {
                Console.WriteLine("No Dart character already present.");
                return;
            }

            var battleCharacterTable = emulator.Battle.CharacterTable[0];

            battleCharacterTable.Status = emulator.Memory.CharacterTable[character].Status;
            if (emulator.Battle.EncounterID == 413) { // Jiango has to have it's initial move. Otherwise he never gets a turn.
                emulator.Battle.MonsterTable[0].Action = 12; // Play the "This is Jiango" part
                while (emulator.Battle.MonsterTable[0].Action != 44) { // Jiango's initial move is over.
                    if (Constants.Run && emulator.Memory.GameState != Emulator.GameState.Battle) {
                        return;
                    }
                    Thread.Sleep(Settings.WaitDelay);
                }
            }

            battleCharacterTable.Action = 10; // Force character's turn in Dragoon form.

            while (battleCharacterTable.Menu != 96) { // Wait for the NoDart's character turn to start
                if (Constants.Run && emulator.Memory.GameState != Emulator.GameState.Battle) {
                    return;
                }
                Thread.Sleep(Settings.WaitDelay);
            }

            battleCharacterTable.Dragoon = 0x20; // Make sure we have Red-Eye Dragoon

            emulator.Memory.PartySlot[0] = character;
            emulator.WriteByte("PARTY_SLOT", character, 0x234E); // Secondary ID

            battleCharacterTable.Image = character;
            battleCharacterTable.Weapon = emulator.Memory.CharacterTable[character].Weapon;
            battleCharacterTable.Helmet = emulator.Memory.CharacterTable[character].Helmet;
            battleCharacterTable.Armor = emulator.Memory.CharacterTable[character].Armor;
            battleCharacterTable.Shoes = emulator.Memory.CharacterTable[character].Shoes;
            battleCharacterTable.Accessory = emulator.Memory.CharacterTable[character].Accessory;

            battleCharacterTable.LV = emulator.Memory.CharacterTable[character].Level;
            battleCharacterTable.DLV = 1;
            battleCharacterTable.SP = 100;

            byte dlv = emulator.Memory.SecondaryCharacterTable[character].DragoonLevel;

            battleCharacterTable.DragoonSpellID = character; // ID has to match, otherwise you get all Flameshots
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
                    battleCharacterTable.DragoonSpell[0] = dmagic1[character];
                    battleCharacterTable.DragoonSpell[1] = dmagic2[character];
                    if (character != 7) {
                        battleCharacterTable.DragoonSpell[2] = dmagic3[character];
                        battleCharacterTable.DragoonSpell[3] = dmagic5[character];
                        break;
                    }
                    battleCharacterTable.DragoonSpell[2] = dmagic5[character];
                    battleCharacterTable.DragoonSpell[3] = 0xFF;
                    break;
                case 4:
                case 3:
                    battleCharacterTable.DragoonSpell[0] = dmagic1[character];
                    battleCharacterTable.DragoonSpell[1] = dmagic2[character];
                    battleCharacterTable.DragoonSpell[2] = dmagic3[character];
                    battleCharacterTable.DragoonSpell[3] = 0xFF;
                    break;
                case 2:
                    battleCharacterTable.DragoonSpell[0] = dmagic1[character];
                    battleCharacterTable.DragoonSpell[1] = dmagic2[character];
                    battleCharacterTable.DragoonSpell[2] = 0xFF;
                    battleCharacterTable.DragoonSpell[3] = 0xFF;
                    break;
                case 1:
                    battleCharacterTable.DragoonSpell[0] = dmagic1[character];
                    battleCharacterTable.DragoonSpell[1] = 0xFF;
                    battleCharacterTable.DragoonSpell[2] = 0xFF;
                    battleCharacterTable.DragoonSpell[3] = 0xFF;
                    break;
                case 0:
                    battleCharacterTable.DragoonSpell[0] = 0xFF;
                    battleCharacterTable.DragoonSpell[1] = 0xFF;
                    battleCharacterTable.DragoonSpell[2] = 0xFF;
                    battleCharacterTable.DragoonSpell[3] = 0xFF;
                    break;
            }


            byte special_effect = 0; // Fix for special effects
            if (battleCharacterTable.Weapon == 45) { // Destroyer Mace
                special_effect |= 1;
            }

            if (battleCharacterTable.Accessory == 156) { // Wargod Sash
                special_effect |= 2;
            }

            if (battleCharacterTable.Accessory == 157) { // Ultimate Wargod
                special_effect |= 6;
            }

            battleCharacterTable.AdditionSpecial = special_effect;

            Addition.ResetAdditionTable(emulator, battleCharacterTable, loDDictionary);

            battleCharacterTable.SetStats(character);

            if (Settings.AutoTransform) {
                battleCharacterTable.Detransform();
            } else {
                battleCharacterTable.Menu = 16;
            }
            while (battleCharacterTable.Action != 9) {
                if (Constants.Run && emulator.Memory.GameState != Emulator.GameState.Battle) {
                    return;
                }
                Thread.Sleep(Settings.WaitDelay);
            }

            battleCharacterTable.DLV = dlv;
            if (dlv == 0) {
                battleCharacterTable.Dragoon = 0;
            }
            battleCharacterTable.SP = emulator.Memory.SecondaryCharacterTable[character].SP;

            Console.WriteLine("No Dart complete.");
        }
    }
}
