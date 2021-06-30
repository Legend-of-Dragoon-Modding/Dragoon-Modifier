using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Emulator {
    public class Factory {
        public static IEmulator Create(string emulatorName, int previousOffset, Dictionary<string, int[]> addresses) {
            return new Emulator(emulatorName, previousOffset, addresses);
        }

        public static IEmulator Emulator(string emulatorName, int previousOffset) {
            return new Emulator(emulatorName, previousOffset);
        }

        internal static Memory.Collections.IAddress<T> AddressCollection<T>(IEmulator emulator, int baseAddress, int offset, int numberOfElements) {
            if (typeof(T) == typeof(byte)) {
                return (Memory.Collections.IAddress<T>) new Memory.Collections.Byte(emulator, baseAddress, offset, numberOfElements);
            } else if (typeof(T) == typeof(sbyte)) {
                return (Memory.Collections.IAddress<T>) new Memory.Collections.SByte(emulator, baseAddress, offset, numberOfElements);
            } else if (typeof(T) == typeof(short)) {
                return (Memory.Collections.IAddress<T>) new Memory.Collections.Short(emulator, baseAddress, offset, numberOfElements);
            } else if (typeof(T) == typeof(ushort)) {
                return (Memory.Collections.IAddress<T>) new Memory.Collections.UShort(emulator, baseAddress, offset, numberOfElements);
            } else if (typeof(T) == typeof(int)) {
                return (Memory.Collections.IAddress<T>) new Memory.Collections.Int(emulator, baseAddress, offset, numberOfElements);
            } else if (typeof(T) == typeof(uint)) {
                return (Memory.Collections.IAddress<T>) new Memory.Collections.UInt(emulator, baseAddress, offset, numberOfElements);
            }
            throw new NotImplementedException();
        }

        internal static Memory.IMemory MemoryController(IEmulator emulator) {
            return new Memory.Controller(emulator);
        }

        internal static Memory.Battle.IBattle BattleController(IEmulator emulator) {
            return new Memory.Battle.Controller(emulator);
        }

        internal static ILoDEncoding Encoding(Region region) {
            return new LoDEncoding(region);
        }

        internal static ILoDEncoding CustomEncoding(Dictionary<char, ushort> char2ushort) {
            return new LoDEncoding(char2ushort);
        }
    }
}
