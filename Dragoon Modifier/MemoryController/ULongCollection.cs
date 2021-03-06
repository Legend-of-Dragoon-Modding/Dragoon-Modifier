using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.MemoryController {
    public class ULongCollection {
        long _baseAddr;
        int _offset;
        public ULongCollection(long baseAddress, int offset) {
            _baseAddr = baseAddress;
            _offset = offset;
        }

        public ulong this[int i] {
            get { return Emulator.ReadULong(_baseAddr + i * _offset); }
            set { Emulator.WriteULong(_baseAddr + i * _offset, value); }
        }
    }
}
