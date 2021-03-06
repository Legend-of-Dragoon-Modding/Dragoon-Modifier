using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.MemoryController {
    public class ByteCollection {
        long[] _addrArr;
        public ByteCollection(long[] addrArr) {
            _addrArr = addrArr;
        }

        public byte this[int i] {
            get { return Emulator.ReadByte(_addrArr[i]); }
            set { Emulator.WriteByte(_addrArr[i], value); }
        }
    }
}
