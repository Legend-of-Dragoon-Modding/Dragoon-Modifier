using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Core.Memory.Encounter {
    public class Map {
        public Collections.IAddress<ushort> Slot { get; private set; }
        public Collections.IAddress<byte> EncounterRate { get; private set; }
        public Collections.IAddress<byte> Background { get; private set; }



        internal Map() {
            var baseAddress = Emulator.GetAddress("ENCOUNTER_MAP");
            // 740 Maps are hopefully all, confirmation needed
            Slot = Collections.Factory.Create<ushort>(baseAddress, 4, 740);
            EncounterRate = Collections.Factory.Create<byte>(baseAddress + 2, 4, 740);
            Background = Collections.Factory.Create<byte>(baseAddress + 3, 4, 740);
        }
    }
}
