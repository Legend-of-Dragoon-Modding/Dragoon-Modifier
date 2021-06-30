using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Emulator.Memory.Collections {
    internal class Short : Address<short> {

        internal Short(IEmulator emulator, int baseAddress, int offset, int numberOfElements) {
            _emulator = emulator;
            _baseAddress = baseAddress;
            _offset = offset;
            Length = numberOfElements;
        }

        public override short this[int i] {
            get {
                if (i >= Length || i < 0) {
                    throw new IndexOutOfRangeException();
                }
                return _emulator.ReadShort(_baseAddress + i * _offset);
            }
            set {
                if (i >= Length || i < 0) {
                    throw new IndexOutOfRangeException();
                }
                _emulator.WriteShort(_baseAddress + i * _offset, value);
            }
        }
    }
}
