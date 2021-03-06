using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.MemoryController {
    public class UShortCollection {
        long[] _addrArr;
        public UShortCollection(long[] addrArr) {
            _addrArr = addrArr;
        }

        public ushort this[int i] {
            get { return Emulator.ReadUShort(_addrArr[i]); }
            set { Emulator.WriteUShort(_addrArr[i], value); }
        }
    }
}
