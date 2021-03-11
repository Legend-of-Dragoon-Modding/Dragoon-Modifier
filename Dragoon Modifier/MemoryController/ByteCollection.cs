using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.MemoryController {
    public class ByteCollection {
        int _baseAddr;
        int _offset;
        uint _size;

        public uint Length { get { return _size; } }

        public ByteCollection(int baseAddress, int offset, uint numberOfElements) {
            _baseAddr = baseAddress;
            _offset = offset;
            _size = numberOfElements;
        }

        public byte this[int i] {
            get {
                if (i >= _size || i < 0) {
                    throw new IndexOutOfRangeException();
                }
                return Emulator.ReadByte(_baseAddr + i * _offset);
            }
            set {
                if (i >= _size || i < 0) {
                    throw new IndexOutOfRangeException();
                }
                Emulator.WriteByte(_baseAddr + i * _offset, value);
            }
        }

        public IEnumerator<byte> GetEnumerator() {
            for (int i = 0; i < _size; i++) {
                yield return this[i];
            }
        }
    }
}
