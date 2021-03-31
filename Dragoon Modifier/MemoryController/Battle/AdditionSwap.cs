using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dragoon_Modifier.MemoryController.Battle {
    public static class AdditionSwap {
        public static void Init() {
            byte slot = 3;
            for (byte i = 0; i < Globals.BattleController.CharacterTable.Length; i++) {
                if (Globals.BattleController.CharacterTable[i].Action == 8) {
                    slot = i;
                    break;
                }
                if (slot == Globals.BattleController.CharacterTable.Length - 1) {
                    Constants.WriteOutput("Cannot swap addition right now.");
                    return;
                }
            }

            uint character = Globals.MemoryController.PartySlot[slot]; ;

            byte additionCount = 0;
            foreach (var additionLevel in Globals.MemoryController.SecondaryCharacterTable[character].AdditionLevel) {
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
                var SP = Globals.BattleController.CharacterTable[slot].SP;
                Globals.BattleController.CharacterTable[slot].SP = 100;
                Globals.BattleController.CharacterTable[slot].Action = 10;
                Globals.BattleController.CharacterTable[slot].DragoonTurns = 1;
                byte menu = 0;
                for (byte i = 0; i < additionCount; i++) {
                    menu += (byte) Math.Pow(2, i);
                }
                Globals.BattleController.CharacterTable[slot].Menu = menu;
                Thread.Sleep(50);


                Emulator.WriteByte(Globals.M_POINT + 0xD32, additionCount);
                for (byte i = 0; i < additionCount; i++) {
                    Emulator.WriteByte(Globals.M_POINT + 0xD34 + i * 0x2, 2);
                }

                while (Globals.BattleController.CharacterTable[0].Action != 9) {
                    if (Globals.GAME_STATE != 1) { // Exit function if battle ends
                        return;
                    }
                    Thread.Sleep(50);
                }

                // TODO Actually swap the addition

                var turn = Globals.BattleController.CharacterTable[slot].Turn;
                Globals.BattleController.CharacterTable[slot].Turn = 800;
                var MP = Globals.BattleController.CharacterTable[slot].MP;
                Globals.BattleController.CharacterTable[slot].MP = 0;
                var HP_Regen = Globals.BattleController.CharacterTable[slot].HP_Regen;
                Globals.BattleController.CharacterTable[slot].HP_Regen = 0;
                var MP_Regen = Globals.BattleController.CharacterTable[slot].MP_Regen;
                Globals.BattleController.CharacterTable[slot].MP_Regen = 1; // Used for catching the turn
                var SP_Regen = Globals.BattleController.CharacterTable[slot].SP_Regen;
                Globals.BattleController.CharacterTable[slot].SP_Regen = 0;


                while (Globals.BattleController.CharacterTable[0].Action != 9) {
                    if (Globals.GAME_STATE != 1) { // Exit function if battle ends
                        return;
                    }
                    Thread.Sleep(50);
                }

                Globals.BattleController.CharacterTable[slot].SP = SP;
                Globals.BattleController.CharacterTable[slot].MP = MP;
                Globals.BattleController.CharacterTable[slot].Turn = turn;
                Globals.BattleController.CharacterTable[slot].HP_Regen = HP_Regen;
                Globals.BattleController.CharacterTable[slot].MP_Regen = MP_Regen;
                Globals.BattleController.CharacterTable[slot].SP_Regen = SP_Regen;

                Globals.ADDITION_SWAP = false;
                Constants.WriteGLogOutput("Addition swap complete.");
            }
        }
    }
}
