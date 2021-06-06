using System;
using System.Collections.Generic;

namespace Dragoon_Modifier.Core.Memory.Collections {
    public class Byte {
        private readonly int _baseAddress;
        private readonly int _offset;

        public uint Length { get; private set; }

        internal Byte(int baseAddress, int offset, uint numberOfElements) {
            _baseAddress = baseAddress;
            _offset = offset;
            Length = numberOfElements;
        }

        public byte this[int i] {
            get {
                if (i >= Length || i < 0) {
                    throw new IndexOutOfRangeException();
                }
                return Emulator.ReadByte(_baseAddress + i * _offset);
            }
            set {
                if (i >= Length || i < 0) {
                    throw new IndexOutOfRangeException();
                }
                Emulator.WriteByte(_baseAddress + i * _offset, value);
            }
        }

        public IEnumerator<byte> GetEnumerator() {
            for (int i = 0; i < Length; i++) {
                yield return this[i];
            }
        }
    }
}
