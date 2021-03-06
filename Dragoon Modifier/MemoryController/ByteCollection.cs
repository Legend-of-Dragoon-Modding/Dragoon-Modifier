using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.MemoryController {
    public class ByteCollection {
        long _baseAddr;
        int _offset;

        public ByteCollection(long baseAddress, int offset) {
            _baseAddr = baseAddress;
            _offset = offset;
        }

        public byte this[int i] {
            get { return Emulator.ReadByte(_baseAddr + i * _offset); }
            set { Emulator.WriteByte(_baseAddr + i * _offset, value); }
        }
    }
}
