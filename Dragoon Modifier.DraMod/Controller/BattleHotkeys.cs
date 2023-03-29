using Dragoon_Modifier.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Controller {
    internal static class BattleHotkeys {
        public static List<Hotkey> Hotkeys = new List<Hotkey>();

        public static List<Hotkey> Load() {
            var hotkeys = new List<Hotkey>();

            hotkeys.Add(new ExitDragoon(0, (Hotkey.L1 + Hotkey.Up)));
            hotkeys.Add(new ExitDragoon(1, (Hotkey.L1 + Hotkey.Right)));
            hotkeys.Add(new ExitDragoon(2, (Hotkey.L1 + Hotkey.Left)));
            hotkeys.Add(new AdditionSwap(Hotkey.L1 + Hotkey.R1));
           

            return hotkeys;
        }

        
        



        private class ExitDragoon : Hotkey {
            private readonly byte slot;

            internal ExitDragoon(byte slot, ushort keyPress) : base(keyPress) {
                this.slot = slot;
            }

            internal override void Func() {
                if (Emulator.Memory.Battle.CharacterTable.Length <= slot) {
                    Console.WriteLine($"Cannot exit dragoon for character slot {slot + 1}. Not enough characters.");
                    return;
                }

                if (Emulator.Memory.Battle.CharacterTable[slot].DragoonTurns < 1) {
                    Console.WriteLine($"Cannot exit dragoon for character slot {slot + 1}. No longer in dragoon form.");
                    return;
                }

                Emulator.Memory.Battle.CharacterTable[slot].DragoonTurns = 1;
            }
        }

        private class AdditionSwap : Hotkey {
            
            internal AdditionSwap(ushort keyPress) : base(keyPress) {

            }

            internal override void Func() {
                Addition.Swap();
            }
        }
    }
}
