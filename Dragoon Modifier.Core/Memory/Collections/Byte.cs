using System;

namespace Dragoon_Modifier.Core.Memory.Collections {
    internal class Byte : Address<byte> {
        internal Byte(int baseAddress, int offset, int numberOfElements) {
            _baseAddress = baseAddress;
            _offset = offset;
            Length = numberOfElements;
        }

        public override byte this[int i] {
            get {
                if (i >= Length || i < 0) {
                    throw new IndexOutOfRangeException();
                }
                return Emulator.DirectAccess.ReadByte(_baseAddress + i * _offset);
            }
            set {
                if (i >= Length || i < 0) {
                    throw new IndexOutOfRangeException();
                }
                Emulator.DirectAccess.WriteByte(_baseAddress + i * _offset, value);
            }
        }
    }
}
