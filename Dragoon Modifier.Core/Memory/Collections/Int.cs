using System;

namespace Dragoon_Modifier.Core.Memory.Collections {
    internal class Int : Address<int> {

        internal Int(int baseAddress, int offset, int numberOfElements) {
            _baseAddress = baseAddress;
            _offset = offset;
            Length = numberOfElements;
        }

        public override int this[int i] {
            get {
                if (i >= Length || i < 0) {
                    throw new IndexOutOfRangeException();
                }
                return Emulator.DirectAccess.ReadInt(_baseAddress + i * _offset);
            }
            set {
                if (i >= Length || i < 0) {
                    throw new IndexOutOfRangeException();
                }
                Emulator.DirectAccess.WriteInt(_baseAddress + i * _offset, value);
            }
        }
    }
}
