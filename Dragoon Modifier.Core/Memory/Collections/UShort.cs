using System;

namespace Dragoon_Modifier.Core.Memory.Collections {
    internal class UShort : Address<ushort> {

        internal UShort(int baseAddress, int offset, int numberOfElements) {
            _baseAddress = baseAddress;
            _offset = offset;
            Length = numberOfElements;
        }

        public override ushort this[int i] {
            get {
                if (i >= Length || i < 0) {
                    throw new IndexOutOfRangeException();
                }
                return Emulator.DirectAccess.ReadUShort(_baseAddress + i * _offset);
            }
            set {
                if (i >= Length || i < 0) {
                    throw new IndexOutOfRangeException();
                }
                Emulator.DirectAccess.WriteUShort(_baseAddress + i * _offset, value);
            }
        }
    }
}
