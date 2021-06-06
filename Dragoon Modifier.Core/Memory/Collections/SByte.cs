using System;
using System.Collections.Generic;

namespace Dragoon_Modifier.Core.Memory.Collections {
    public class SByte {
        private readonly int _baseAddress;
        private readonly int _offset;

        public uint Length { get; private set; }

        internal SByte(int baseAddress, int offset, uint numberOfElements) {
            _baseAddress = baseAddress;
            _offset = offset;
            Length = numberOfElements;
        }

        public sbyte this[int i] {
            get {
                if (i >= Length || i < 0) {
                    throw new IndexOutOfRangeException();
                }
                return Emulator.ReadSByte(_baseAddress + i * _offset);
            }
            set {
                if (i >= Length || i < 0) {
                    throw new IndexOutOfRangeException();
                }
                Emulator.WriteSByte(_baseAddress + i * _offset, value);
            }
        }

        public IEnumerator<sbyte> GetEnumerator() {
            for (int i = 0; i < Length; i++) {
                yield return this[i];
            }
        }
    }
}
