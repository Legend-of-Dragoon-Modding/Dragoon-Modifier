using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Core.Memory.Collections {
    internal static class Factory {
        internal static IAddress<T> Create<T>(int baseAddress, int offset, int numberOfElements) {
            if (typeof(T) == typeof(byte)) {
                return (IAddress<T>) new Byte(baseAddress, offset, numberOfElements);
            } else if (typeof(T) == typeof(sbyte)) {
                return (IAddress<T>) new SByte(baseAddress, offset, numberOfElements);
            } else if (typeof(T) == typeof(short)) {
                return (IAddress<T>) new Short(baseAddress, offset, numberOfElements);
            } else if (typeof(T) == typeof(ushort)) {
                return (IAddress<T>) new UShort(baseAddress, offset, numberOfElements);
            } else if (typeof(T) == typeof(int)) {
                return (IAddress<T>) new Int(baseAddress, offset, numberOfElements);
            } else if (typeof(T) == typeof(uint)) {
                return (IAddress<T>) new UInt(baseAddress, offset, numberOfElements);
            }
            throw new NotImplementedException();
        }
    }
}
