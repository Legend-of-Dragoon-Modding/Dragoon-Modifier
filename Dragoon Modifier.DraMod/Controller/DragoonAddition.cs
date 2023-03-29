using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Controller {
    internal static class DragoonAddition {
        internal static void WriteDragoonAdditionTable(int slot, uint characterId) {
            long dragoonAddress = Core.Emulator.Memory.Battle.DragoonAdditionTable + slot * 0x100;
            Core.Emulator.DirectAccess.WriteUShort(dragoonAddress + 0x8, Settings.Instance.Dataset.DragoonAddition[(int) characterId].HIT1);
            Core.Emulator.DirectAccess.WriteUShort(dragoonAddress + 0x8 + 0x20, Settings.Instance.Dataset.DragoonAddition[(int) characterId].HIT2);
            Core.Emulator.DirectAccess.WriteUShort(dragoonAddress + 0x8 + 0x40, Settings.Instance.Dataset.DragoonAddition[(int) characterId].HIT3);
            Core.Emulator.DirectAccess.WriteUShort(dragoonAddress + 0x8 + 0x60, Settings.Instance.Dataset.DragoonAddition[(int) characterId].HIT4);
            Core.Emulator.DirectAccess.WriteUShort(dragoonAddress + 0x8 + 0x80, Settings.Instance.Dataset.DragoonAddition[(int) characterId].HIT5);
        }
    }
}
