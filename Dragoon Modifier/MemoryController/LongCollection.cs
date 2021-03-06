using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.MemoryController {
    public class LongCollection {
        long _baseAddr;
        int _offset;

        public LongCollection(long baseAddress, int offset) {
            _baseAddr = baseAddress;
            _offset = offset;
        }

        public long this[int i] {
            get { return Emulator.ReadLong(_baseAddr + i * _offset); }
            set { Emulator.WriteLong(_baseAddr + i * _offset, value); }
        }
    }
}
