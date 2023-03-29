using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Core.Memory.Collections {
    public interface IAddress<T> {
        T this[int i] { get; set; }

        int Length { get; }

        IEnumerator<T> GetEnumerator();
    }
}
