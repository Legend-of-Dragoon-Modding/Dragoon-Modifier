using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Dragoon_Modifier.Core;

namespace Dragoon_Modifier.MemoryController.Battle {
    public static class AdditionSwap {
        public static void Init() {
            byte slot = 0;
            bool exit = true;
            for (byte i = 0; i < Core.Emulator.Battle.CharacterTable.Length; i++) {
                if (Core.Emulator.Battle.CharacterTable[i].Action == 8) {
                    slot = i;
                    exit = false;
                    break;
                }
            }

            if (exit) {
                Constants.WriteOutput("Cannot swap addition right now.");
                return;
            }

            uint character = Core.Emulator.Memory.PartySlot[slot]; ;

            byte additionCount = 0;
            foreach (var additionLevel in Core.Emulator.Memory.SecondaryCharacterTable[character].AdditionLevel) {
                if (additionLevel != 0) {
                    additionCount++;
                }
            }

            if (additionCount > 1) {
                SwapAddition(slot, character, additionCount);
            } else {
                Constants.WriteOutput("No Addition to swap to");
            }
        }

        private static void SwapAddition(byte slot, uint character, byte additionCount) {
            if (additionCount > 1) {
                var SP = Core.Emulator.Battle.CharacterTable[slot].SP;
                Core.Emulator.Battle.CharacterTable[slot].SP = 100;
                Core.Emulator.Battle.CharacterTable[slot].Action = 10;
                Core.Emulator.Battle.CharacterTable[slot].IsDragoon = 1;
                Core.Emulator.Battle.CharacterTable[slot].DragoonTurns = 1;
                byte menu = 0;
                for (byte i = 0; i < additionCount; i++) {
                    menu += (byte) Math.Pow(2, i);
                }
                Core.Emulator.Battle.CharacterTable[slot].Menu = menu;
                Thread.Sleep(50);

                Core.Emulator.Battle.BattleMenuCount = additionCount; // Make sure we have correct ammount of items. Probably not needed after setting the menu.

                for (byte i = 0; i < additionCount; i++) {
                    Core.Emulator.Battle.BattleMenuSlot[i] = 2; // Sets slot i to Dragoon Transformation
                }

                byte chosenAddition = 0;
                while (Core.Emulator.Battle.CharacterTable[slot].Action != 9) {
                    if (Core.Emulator.Memory.GameState != GameState.Battle) { // Exit function if battle ends
                        return;
                    }
                    chosenAddition = Core.Emulator.Battle.BattleMenuChosenSlot; // Reads the index of the selected battle menu slot
                    Thread.Sleep(50);
                }

                // TODO Actually swap the addition
                Thread.Sleep(500);

                var turn = Core.Emulator.Battle.CharacterTable[slot].Turn;
                Core.Emulator.Battle.CharacterTable[slot].Turn = 800;
                var MP = Core.Emulator.Battle.CharacterTable[slot].MP;
                Core.Emulator.Battle.CharacterTable[slot].MP = 0;
                var HP_Regen = Core.Emulator.Battle.CharacterTable[slot].HP_Regen;
                Core.Emulator.Battle.CharacterTable[slot].HP_Regen = 0;
                var MP_Regen = Core.Emulator.Battle.CharacterTable[slot].MP_Regen;
                Core.Emulator.Battle.CharacterTable[slot].MP_Regen = 1; // Used for catching the turn
                var SP_Regen = Core.Emulator.Battle.CharacterTable[slot].SP_Regen;
                Core.Emulator.Battle.CharacterTable[slot].SP_Regen = 0;

                while (Core.Emulator.Battle.CharacterTable[slot].MP != 1) {
                    if (Core.Emulator.Memory.GameState != GameState.Battle) { // Exit function if battle ends
                        return;
                    }
                    Thread.Sleep(50);
                }

                Core.Emulator.Battle.CharacterTable[slot].SP = SP;
                Core.Emulator.Battle.CharacterTable[slot].MP = MP;
                Core.Emulator.Battle.CharacterTable[slot].Turn = turn;
                Core.Emulator.Battle.CharacterTable[slot].HP_Regen = HP_Regen;
                Core.Emulator.Battle.CharacterTable[slot].MP_Regen = MP_Regen;
                Core.Emulator.Battle.CharacterTable[slot].SP_Regen = SP_Regen;

                Constants.WriteGLogOutput("Addition swap complete.");
            }
        }
    }
}
