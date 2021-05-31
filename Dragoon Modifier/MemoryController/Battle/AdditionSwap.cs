using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dragoon_Modifier.MemoryController.Battle {
    public static class AdditionSwap {
        public static void Init() {
            byte slot = 0;
            bool exit = true;
            for (byte i = 0; i < Emulator.BattleController.CharacterTable.Length; i++) {
                if (Emulator.BattleController.CharacterTable[i].Action == 8) {
                    slot = i;
                    exit = false;
                    break;
                }
            }

            if (exit) {
                Constants.WriteOutput("Cannot swap addition right now.");
                return;
            }

            uint character = Emulator.MemoryController.PartySlot[slot]; ;

            byte additionCount = 0;
            foreach (var additionLevel in Emulator.MemoryController.SecondaryCharacterTable[character].AdditionLevel) {
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
                var SP = Emulator.BattleController.CharacterTable[slot].SP;
                Emulator.BattleController.CharacterTable[slot].SP = 100;
                Emulator.BattleController.CharacterTable[slot].Action = 10;
                Emulator.BattleController.CharacterTable[slot].IsDragoon = 1;
                Emulator.BattleController.CharacterTable[slot].DragoonTurns = 1;
                byte menu = 0;
                for (byte i = 0; i < additionCount; i++) {
                    menu += (byte) Math.Pow(2, i);
                }
                Emulator.BattleController.CharacterTable[slot].Menu = menu;
                Thread.Sleep(50);

                Emulator.BattleController.BattleMenuCount = additionCount; // Make sure we have correct ammount of items. Probably not needed after setting the menu.

                for (byte i = 0; i < additionCount; i++) {
                    Emulator.BattleController.BattleMenuSlot[i] = 2; // Sets slot i to Dragoon Transformation
                }

                byte chosenAddition = 0;
                while (Emulator.BattleController.CharacterTable[slot].Action != 9) {
                    if (Emulator.MemoryController.GameState != GameState.Battle) { // Exit function if battle ends
                        return;
                    }
                    chosenAddition = Emulator.BattleController.BattleMenuChosenSlot; // Reads the index of the selected battle menu slot
                    Thread.Sleep(50);
                }

                // TODO Actually swap the addition
                Thread.Sleep(500);

                var turn = Emulator.BattleController.CharacterTable[slot].Turn;
                Emulator.BattleController.CharacterTable[slot].Turn = 800;
                var MP = Emulator.BattleController.CharacterTable[slot].MP;
                Emulator.BattleController.CharacterTable[slot].MP = 0;
                var HP_Regen = Emulator.BattleController.CharacterTable[slot].HP_Regen;
                Emulator.BattleController.CharacterTable[slot].HP_Regen = 0;
                var MP_Regen = Emulator.BattleController.CharacterTable[slot].MP_Regen;
                Emulator.BattleController.CharacterTable[slot].MP_Regen = 1; // Used for catching the turn
                var SP_Regen = Emulator.BattleController.CharacterTable[slot].SP_Regen;
                Emulator.BattleController.CharacterTable[slot].SP_Regen = 0;

                while (Emulator.BattleController.CharacterTable[slot].MP != 1) {
                    if (Emulator.MemoryController.GameState != GameState.Battle) { // Exit function if battle ends
                        return;
                    }
                    Thread.Sleep(50);
                }

                Emulator.BattleController.CharacterTable[slot].SP = SP;
                Emulator.BattleController.CharacterTable[slot].MP = MP;
                Emulator.BattleController.CharacterTable[slot].Turn = turn;
                Emulator.BattleController.CharacterTable[slot].HP_Regen = HP_Regen;
                Emulator.BattleController.CharacterTable[slot].MP_Regen = MP_Regen;
                Emulator.BattleController.CharacterTable[slot].SP_Regen = SP_Regen;

                Constants.WriteGLogOutput("Addition swap complete.");
            }
        }
    }
}
