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

            return hotkeys;
        }

        




        private class ExitDragoon : Hotkey {
            private readonly byte slot;

            internal ExitDragoon(byte slot, ushort keyPress) : base(keyPress) {
                this.slot = slot;
            }

            internal override void Func(Emulator.IEmulator emulator) {
                if (emulator.Battle.CharacterTable.Length <= slot) {
                    Console.WriteLine($"Cannot exit dragoon for character slot {slot + 1}. Not enough characters.");
                    return;
                }

                if (emulator.Battle.CharacterTable[slot].DragoonTurns < 1) {
                    Console.WriteLine($"Cannot exit dragoon for character slot {slot + 1}. No longer in dragoon form.");
                    return;
                }

                emulator.Battle.CharacterTable[slot].DragoonTurns = 1;
            }
        }
    }
}
