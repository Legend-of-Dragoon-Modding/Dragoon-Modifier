using System.Collections.Generic;

namespace Dragoon_Modifier.Emulator.Memory.Collections {
    public interface IAddress<T> {
        T this[int i] { get; set; }

        int Length { get; }

        IEnumerator<T> GetEnumerator();
    }
}