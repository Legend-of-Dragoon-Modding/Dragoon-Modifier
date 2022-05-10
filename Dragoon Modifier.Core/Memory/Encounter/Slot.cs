using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Core.Memory.Encounter {
    public class Slot {
        public Collections.IAddress<ushort> Encounter1 { get; private set; }
        public Collections.IAddress<ushort> Encounter2 { get; private set; }
        public Collections.IAddress<ushort> Encounter3 { get; private set; }
        public Collections.IAddress<ushort> Encounter4 { get; private set; }

        internal Slot() {
            var baseAddress = Emulator.GetAddress("ENCOUNTER_TABLE");
            // 295 Should be all of the slots. Confirmation needed
            Encounter1 = Collections.Factory.Create<ushort>(baseAddress, 8, 295);
            Encounter2 = Collections.Factory.Create<ushort>(baseAddress + 0x2, 8, 295);
            Encounter3 = Collections.Factory.Create<ushort>(baseAddress + 0x4, 8, 295);
            Encounter4 = Collections.Factory.Create<ushort>(baseAddress + 0x6, 8, 295);
        }
    }
}
