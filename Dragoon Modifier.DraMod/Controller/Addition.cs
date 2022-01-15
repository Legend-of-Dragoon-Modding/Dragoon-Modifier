using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Controller {
    internal static class Addition {

        /// <summary>
        /// Sets addition table of <paramref name="character"/> according to <paramref name="LoDDictionary"/>.
        /// For Shana and Miranda, it sets the addition table to blank and reloads their SP per hit.
        /// </summary>
        /// <param name="emulator"></param>
        /// <param name="character"></param>
        /// <param name="LoDDictionary"></param>
        internal static void ResetAdditionTable(Emulator.IEmulator emulator, Emulator.Memory.Battle.Character character, LoDDict.ILoDDictionary LoDDictionary) {
            var characterID = character.ID;

            if (characterID == 2 || characterID == 8) {
                for (byte i = 0; i < 8; i++) {
                    BlankAdditionHit(character, i);
                }
                // TODO: Set Sharanda SP per hit table
                return;
            }

            var additionID = emulator.Memory.CharacterTable[characterID].ChosenAddition;
            var additionIndex = Array.IndexOf(LoDDict.Addition.AdditionIDs[characterID], additionID);
            var addition = LoDDictionary.Character[characterID].Additions[additionIndex];

            byte hitIndex = 0;
            foreach (var hit in addition.AdditionHit) {
                character.Addition[hitIndex].MasterAddition = LoDDict.Addition.RegularAddition;
                character.Addition[hitIndex].NextHit = hit.NextHit;
                character.Addition[hitIndex].BlueSquare = hit.BlueTime;
                character.Addition[hitIndex].GrayHit = hit.GrayTime;
                character.Addition[hitIndex].Damage = hit.Damage;
                character.Addition[hitIndex].SP = hit.SP;
                character.Addition[hitIndex].ID = 0;
                character.Addition[hitIndex].FinalHit = 0;
                character.Addition[hitIndex].PanCameraDistance = hit.CameraPanDistance;
                character.Addition[hitIndex].LockOnCameraDistance1 = hit.LockOnCameraDistance;
                character.Addition[hitIndex].LockOnCameraDistance2 = hit.LockOnCameraDistance2;
                character.Addition[hitIndex].MonsterDistance = hit.MonsterDistance;
                character.Addition[hitIndex].VerticalDistance = hit.VerticaDistance;
                character.Addition[hitIndex].Unknown1 = hit.Unknown;
                character.Addition[hitIndex].Unknown2 = hit.Unknown2;
                character.Addition[hitIndex].StartTime = 0;

                hitIndex++;
            }

            for (byte rest = hitIndex; rest < 8; rest++) {
                BlankAdditionHit(character, rest);
            }

            character.Addition[0].ID = addition.ID;
            character.Addition[0].StartTime = addition.StartTime;
            character.Addition[addition.AdditionHit.Count - 1].FinalHit = LoDDict.Addition.EndFlag;

            var additionLevel = emulator.Memory.CharacterTable[characterID].AdditionLevel[additionIndex] - 1;
            character.Add_DMG_Multi = addition.DamageIncrease[additionLevel];
            character.Add_SP_Multi = addition.SPIncrease[additionLevel];
        }

        private static void BlankAdditionHit(Emulator.Memory.Battle.Character character, byte hit) {
            character.Addition[hit].MasterAddition = 0;
            character.Addition[hit].NextHit = 0;
            character.Addition[hit].BlueSquare = 0;
            character.Addition[hit].GrayHit = 0;
            character.Addition[hit].Damage = 0;
            character.Addition[hit].SP = 0;
            character.Addition[hit].ID = 0;
            character.Addition[hit].FinalHit = 0;
            character.Addition[hit].PanCameraDistance = 0;
            character.Addition[hit].LockOnCameraDistance1 = 0;
            character.Addition[hit].LockOnCameraDistance2 = 0;
            character.Addition[hit].MonsterDistance = 0;
            character.Addition[hit].VerticalDistance = 0;
            character.Addition[hit].Unknown1 = 0;
            character.Addition[hit].Unknown2 = 0;
            character.Addition[hit].StartTime = 0;
        }

        /// <summary>
        /// Finds the character whose turn it currently is. Replaces all menu options to Dragoon transformation, then switches the addition based on the index of Dragoon transformation chosen.
        /// </summary>
        /// <param name="emulator"></param>
        /// <param name="LoDDictionary"></param>
        internal static void Swap(Emulator.IEmulator emulator, LoDDict.ILoDDictionary LoDDictionary) {
            if (GetActionSlot(emulator, out var slot)) {
                var character = emulator.Battle.CharacterTable[slot];
                uint characterID = character.ID;

                byte additionCount = 0;
                foreach (var additionLevel in emulator.Memory.SecondaryCharacterTable[characterID].AdditionLevel) {
                    if (additionLevel != 0) {
                        additionCount++;
                    }
                }

                if (additionCount > 1) {
                    var SP = character.SP;
                    character.SP = 100;
                    character.Action = 10;
                    character.IsDragoon = 1;
                    character.DragoonTurns = 1;

                    byte menu = 0;

                    for (byte i = 0; i < additionCount; i++) {
                        menu += (byte) Math.Pow(2, i);
                    }
                    character.Menu = menu;

                    emulator.Battle.BattleMenuCount = additionCount;
                    for (byte addition = 0; addition < additionCount; addition++) {
                        emulator.Battle.BattleMenuSlot[addition] = 2; // Set each slot to Dragoon Transform
                    }

                    byte additionIndex = 0;

                    while (true) {
                        if (emulator.Memory.GameState != Emulator.GameState.Battle) {
                            return;
                        }

                        if (character.Action == 9) {
                            break;
                        }

                        additionIndex = emulator.Battle.BattleMenuChosenSlot;
                        Thread.Sleep(Settings.WaitDelay);
                    }

                    emulator.Memory.CharacterTable[characterID].ChosenAddition = LoDDict.Addition.AdditionIDs[characterID][additionIndex];

                    ResetAdditionTable(emulator, character, LoDDictionary);

                    var turn = character.Turn;
                    character.Turn = 800;
                    var MP = character.MP;
                    character.MP = 0;
                    var HP_reg = character.HP_Regen;
                    var MP_reg = character.MP_Regen;
                    var SP_reg = character.SP_Regen;

                    character.HP_Regen = 0;
                    character.SP_Regen = 0;
                    character.MP_Regen = 1;

                    while (true) {
                        if (emulator.Memory.GameState != Emulator.GameState.Battle) {
                            return;
                        }

                        if (character.MP == 1) {
                            break;
                        }

                        Thread.Sleep(Settings.WaitDelay);
                    }

                    character.SP = SP;
                    character.MP = MP;
                    character.Turn = turn;
                    character.HP_Regen = HP_reg;
                    character.MP_Regen = MP_reg;
                    character.SP_Regen = SP_reg;

                    Console.WriteLine($"Addition swap complete.");

                    return;
                }
                Console.WriteLine("No Addition to swap to.");
            }
        }

        private static bool GetActionSlot(Emulator.IEmulator emulator, out byte turnSlot) {
            for (byte slot = 0; slot < emulator.Battle.CharacterTable.Length; slot++) {
                if (emulator.Battle.CharacterTable[slot].Action == 8) {
                    turnSlot = slot;
                    return true;
                }
            }
            turnSlot = 0;
            return false;
        }

        /// <summary>
        /// Sets all menu addition tables according to <paramref name="LoDDictionary"/>. Albert is skipped, since it's a duplicate of Lavitz's additions.
        /// </summary>
        /// <param name="emulator"></param>
        /// <param name="LoDDictionary"></param>
        internal static void MenuTableChange(Emulator.IEmulator emulator, LoDDict.ILoDDictionary LoDDictionary) {
            for (int character = 0; character < 8; character++) {
                if (character == 5) { // Skip Albert
                    continue;
                }

                foreach (var addition in LoDDictionary.Character[character].Additions) {
                    var table = emulator.Memory.MenuAdditionTable[addition.ID];

                    table.Damage = (ushort) addition.AdditionHit.Sum(add => add.Damage);
                    for (int addLvl = 0; addLvl < 5; addLvl++) {
                        
                        ushort sp = 0;
                        foreach (var hit in addition.AdditionHit) {
                            sp += (ushort) ((hit.SP * (100 + addition.SPIncrease[addLvl])) / 100);
                        }
                        table.SP[addLvl] = sp;
                        table.DamageLevelMultiplier[addLvl] = addition.DamageIncrease[addLvl];
                    }
                }
            }
        }
    }
}
