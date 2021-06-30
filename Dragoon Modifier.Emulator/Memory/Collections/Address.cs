using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Emulator.Memory.Collections {
    internal abstract class Address<T> : IAddress<T> {
        protected IEmulator _emulator;
        protected int _baseAddress;
        protected int _offset;

        public int Length { get; protected set; }

        public abstract T this[int i] {
            get; set;
        }

        public IEnumerator<T> GetEnumerator() {
            for (int i = 0; i < Length; i++) {
                yield return this[i];
            }
        }
    }
}
