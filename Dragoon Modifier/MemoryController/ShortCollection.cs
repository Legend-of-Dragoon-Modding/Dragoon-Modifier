using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.MemoryController {
    public class ShortCollection {
        long _baseAddr;
        int _offset;

        public ShortCollection(long baseAddress, int offset) {
            _baseAddr = baseAddress;
            _offset = offset;
        }

        public short this[int i] {
            get { return Emulator.ReadShort(_baseAddr + i * _offset); }
            set { Emulator.WriteShort(_baseAddr + i * _offset, value); }
        }
    }
}
