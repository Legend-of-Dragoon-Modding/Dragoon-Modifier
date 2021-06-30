using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Emulator.Memory.Collections {
    internal class UInt : Address<uint> {

        internal UInt(IEmulator emulator, int baseAddress, int offset, int numberOfElements) {
            _emulator = emulator;
            _baseAddress = baseAddress;
            _offset = offset;
            Length = numberOfElements;
        }

        public override uint this[int i] {
            get {
                if (i >= Length || i < 0) {
                    throw new IndexOutOfRangeException();
                }
                return _emulator.ReadUInt(_baseAddress + i * _offset);
            }
            set {
                if (i >= Length || i < 0) {
                    throw new IndexOutOfRangeException();
                }
                _emulator.WriteUInt(_baseAddress + i * _offset, value);
            }
        }
    }
}
