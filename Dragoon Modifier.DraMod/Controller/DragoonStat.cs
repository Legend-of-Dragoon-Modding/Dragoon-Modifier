using Dragoon_Modifier.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Controller {
    internal static class DragoonStat {
        internal static void WriteDragoonStatTable(int slot, Core.Memory.Battle.Character character) {
            long address = Emulator.GetAddress("SECONDARY_CHARACTER_TABLE");
            int dlv = character.DLV;
            uint charID = character.ID;

            Console.WriteLine(Settings.Instance.Dataset.DragoonStats[charID][dlv].DAT + " " + dlv);

            character.DAT = Settings.Instance.Dataset.DragoonStats[charID][dlv].DAT;
            character.DMAT = Settings.Instance.Dataset.DragoonStats[charID][dlv].DMAT;
            character.DDF = Settings.Instance.Dataset.DragoonStats[charID][dlv].DDF;
            character.DMDF = Settings.Instance.Dataset.DragoonStats[charID][dlv].DMDF;
            double MP_base = Settings.Instance.Dataset.DragoonStats[charID][dlv].MP;
            double MP_multi = 1 + (double) Emulator.DirectAccess.ReadByte(address + 0x64) / 100;
            ushort MP_Max = (ushort) (MP_base * MP_multi);
            ushort MP_Curr = Math.Min(Emulator.DirectAccess.ReadUShort("CHAR_TABLE", (int) (charID * 0x2C) + 0xA), MP_Max);
            character.MP = MP_Curr;
            Emulator.DirectAccess.WriteUShort(address + 0x6, MP_Curr); 
            character.MaxMP = MP_Max;
            Emulator.DirectAccess.WriteUShort(address + 0x6E, MP_Max);
        }
    }
}
