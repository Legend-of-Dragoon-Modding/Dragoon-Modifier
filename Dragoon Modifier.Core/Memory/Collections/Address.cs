using System;
using System.Collections.Generic;

namespace Dragoon_Modifier.Core.Memory.Collections {
    internal abstract class Address<T> : IAddress<T> {
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
