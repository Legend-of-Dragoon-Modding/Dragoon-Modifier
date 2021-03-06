using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.MemoryController {
    public class UShortCollection {
        long _baseAddr;
        int _offset;

        public UShortCollection(long baseAddress, int offset) {
            _baseAddr = baseAddress;
            _offset = offset;
        }

        public ushort this[int i] {
            get { return Emulator.ReadUShort(_baseAddr + i * _offset); }
            set { Emulator.WriteUShort(_baseAddr + i * _offset, value); }
        }
    }
}
