using System;

namespace Dragoon_Modifier.Core.Memory.Collections {
    internal class SByte : Address<sbyte> {

        internal SByte(int baseAddress, int offset, int numberOfElements) {
            _baseAddress = baseAddress;
            _offset = offset;
            Length = numberOfElements;
        }

        public override sbyte this[int i] {
            get {
                if (i >= Length || i < 0) {
                    throw new IndexOutOfRangeException();
                }
                return Emulator.DirectAccess.ReadSByte(_baseAddress + i * _offset);
            }
            set {
                if (i >= Length || i < 0) {
                    throw new IndexOutOfRangeException();
                }
                Emulator.DirectAccess.WriteSByte(_baseAddress + i * _offset, value);
            }
        }
    }
}
