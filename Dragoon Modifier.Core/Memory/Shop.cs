using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Core.Memory {
    public class Shop {
        private readonly int _baseAddress;

        public byte WeaponItemFlag { get { return Emulator.ReadByte(_baseAddress); } set { Emulator.WriteByte(_baseAddress, value); } }
        public Collections.Byte Item { get; private set; }

        internal Shop(int baseAddress, int shop) {
            _baseAddress = baseAddress + shop * 0x40;
            Item = new Collections.Byte(_baseAddress + 1, 4, 16);

        }
    }
}
