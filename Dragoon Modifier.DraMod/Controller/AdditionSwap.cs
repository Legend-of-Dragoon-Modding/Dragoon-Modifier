using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Controller {
    internal static class AdditionSwap {

        internal static void Init(Emulator.IEmulator emulator) {
            if (GetActionSlot(emulator, out var slot)) {
                uint character = emulator.Memory.PartySlot[slot];

                byte additionCount = 0;
                foreach (var additionLevel in emulator.Memory.SecondaryCharacterTable[character].AdditionLevel) {
                    if (additionLevel != 0) {
                        additionCount++;
                    }
                }

                if (additionCount > 1) {
                    // Swap addition
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
    }
}
