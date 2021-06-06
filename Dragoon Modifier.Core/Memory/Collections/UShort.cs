using System;
using System.Collections.Generic;

namespace Dragoon_Modifier.Core.Memory.Collections {
    public class UShort {
        private readonly int _baseAddress;
        private readonly int _offset;

        public uint Length { get; private set; }

        internal UShort(int baseAddress, int offset, uint numberOfElements) {
            _baseAddress = baseAddress;
            _offset = offset;
            Length = numberOfElements;
        }

        public ushort this[int i] {
            get {
                if (i >= Length || i < 0) {
                    throw new IndexOutOfRangeException();
                }
                return Emulator.ReadUShort(_baseAddress + i * _offset);
            }
            set {
                if (i >= Length || i < 0) {
                    throw new IndexOutOfRangeException();
                }
                Emulator.WriteUShort(_baseAddress + i * _offset, value);
            }
        }

        public IEnumerator<ushort> GetEnumerator() {
            for (int i = 0; i < Length; i++) {
                yield return this[i];
            }
        }
    }
}
