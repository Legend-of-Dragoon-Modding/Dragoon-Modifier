using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.MemoryController {
    public class UShortCollection {
        long _baseAddr;
        int _offset;
        uint _size;

        public uint Length { get { return _size; } }

        public UShortCollection(long baseAddress, int offset, uint numberOfElements) {
            _baseAddr = baseAddress;
            _offset = offset;
            _size = numberOfElements;
        }

        public ushort this[uint i] {
            get {
                if (i >= _size) {
                    throw new IndexOutOfRangeException();
                }
                return Emulator.ReadUShort(_baseAddr + i * _offset);
            }
            set {
                if (i >= _size) {
                    throw new IndexOutOfRangeException();
                }
                Emulator.WriteUShort(_baseAddr + i * _offset, value);
            }
        }

        public IEnumerator<ushort> GetEnumerator() {
            for (uint i = 0; i < _size; i++) {
                yield return this[i];
            }
        }
    }
}
