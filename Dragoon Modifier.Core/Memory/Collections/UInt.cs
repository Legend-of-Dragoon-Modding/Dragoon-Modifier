using System;

namespace Dragoon_Modifier.Core.Memory.Collections {
    internal class UInt : Address<uint> {

        internal UInt(int baseAddress, int offset, int numberOfElements) {
            _baseAddress = baseAddress;
            _offset = offset;
            Length = numberOfElements;
        }

        public override uint this[int i] {
            get {
                if (i >= Length || i < 0) {
                    throw new IndexOutOfRangeException();
                }
                return Emulator.DirectAccess.ReadUInt(_baseAddress + i * _offset);
            }
            set {
                if (i >= Length || i < 0) {
                    throw new IndexOutOfRangeException();
                }
                Emulator.DirectAccess.WriteUInt(_baseAddress + i * _offset, value);
            }
        }
    }
}
