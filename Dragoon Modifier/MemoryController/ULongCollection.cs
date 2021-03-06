using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.MemoryController {
    public class ULongCollection {
        long[] _addrArr;
        public ULongCollection(long[] addrArr) {
            _addrArr = addrArr;
        }

        public ulong this[int i] {
            get { return Emulator.ReadULong(_addrArr[i]); }
            set { Emulator.WriteULong(_addrArr[i], value); }
        }
    }
}
