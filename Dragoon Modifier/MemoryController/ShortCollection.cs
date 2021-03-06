using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.MemoryController {
    public class ShortCollection {
        long[] _addrArr;
        public ShortCollection(long[] addrArr) {
            _addrArr = addrArr;
        }

        public short this[int i] {
            get { return Emulator.ReadShort(_addrArr[i]); }
            set { Emulator.WriteShort(_addrArr[i], value); }
        }
    }
}
