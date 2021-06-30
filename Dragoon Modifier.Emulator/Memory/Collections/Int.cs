using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Emulator.Memory.Collections {
    internal class Int : Address<int> {

        internal Int(IEmulator emulator, int baseAddress, int offset, int numberOfElements) {
            _emulator = emulator;
            _baseAddress = baseAddress;
            _offset = offset;
            Length = numberOfElements;
        }

        public override int this[int i] {
            get {
                if (i >= Length || i < 0) {
                    throw new IndexOutOfRangeException();
                }
                return _emulator.ReadInt(_baseAddress + i * _offset);
            }
            set {
                if (i >= Length || i < 0) {
                    throw new IndexOutOfRangeException();
                }
                _emulator.WriteInt(_baseAddress + i * _offset, value);
            }
        }
    }
}
