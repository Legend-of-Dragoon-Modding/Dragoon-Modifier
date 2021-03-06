using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.MemoryController {
    public class LongCollection {
        long[] _addrArr;
        public LongCollection(long[] addrArr) {
            _addrArr = addrArr;
        }

        public long this[int i] {
            get { return Emulator.ReadLong(_addrArr[i]); }
            set { Emulator.WriteLong(_addrArr[i], value); }
        }
    }
}
