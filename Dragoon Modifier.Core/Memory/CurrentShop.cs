using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Core.Memory {
    public class CurrentShop {
        private readonly int _baseAddress;

        public byte ItemCount { get { return Emulator.ReadByte(_baseAddress + 0x44); } set { Emulator.WriteByte(_baseAddress + 0x44, value); } }
        public byte WeaponItemFlag { get { return Emulator.ReadByte(_baseAddress + 0x45); } set { Emulator.WriteByte(_baseAddress + 0x45, value); } }
        public Collections.Byte ItemID { get; private set; }
        public Collections.UShort ItemPrice { get; private set; }

        internal CurrentShop(int baseAddress) {
            _baseAddress = baseAddress;
            ItemID = new Collections.Byte(_baseAddress, 4, 16);
            ItemPrice = new Collections.UShort(_baseAddress + 0x2, 4, 16);
        }
    }
}
