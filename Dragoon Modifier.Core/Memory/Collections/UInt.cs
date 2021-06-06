using System;
using System.Collections.Generic;

namespace Dragoon_Modifier.Core.Memory.Collections {
    public class UInt {
        private readonly int _baseAddress;
        private readonly int _offset;

        public uint Length { get; private set; }

        internal UInt(int baseAddress, int offset, uint numberOfElements) {
            _baseAddress = baseAddress;
            _offset = offset;
            Length = numberOfElements;
        }

        public uint this[int i] {
            get {
                if (i >= Length || i < 0) {
                    throw new IndexOutOfRangeException();
                }
                return Emulator.ReadUInt(_baseAddress + i * _offset);
            }
            set {
                if (i >= Length || i < 0) {
                    throw new IndexOutOfRangeException();
                }
                Emulator.WriteUInt(_baseAddress + i * _offset, value);
            }
        }

        public IEnumerator<uint> GetEnumerator() {
            for (int i = 0; i < Length; i++) {
                yield return this[i];
            }
        }
    }
}
