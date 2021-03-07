using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.MemoryController {
    public class CurrentShop {
        long _baseAddress;
        ByteCollection _itemID; // Should these just be an object?
        UShortCollection _itemPrice;

        public byte ItemCount { get { return Emulator.ReadByte(_baseAddress + 0x44); } set { Emulator.WriteByte(_baseAddress + 0x44, value); } }
        public byte WeaponItemFlag { get { return Emulator.ReadByte(_baseAddress + 0x45); } set { Emulator.WriteByte(_baseAddress + 0x45, value); } }
        public ByteCollection ItemID { get { return _itemID; } set { _itemID = value; } }
        public UShortCollection ItemPrice { get { return _itemPrice; } set { _itemPrice = value; } }

        public CurrentShop(long baseAddress) {
            _baseAddress = baseAddress;
            _itemID = new ByteCollection(_baseAddress, 4, 16);
            _itemPrice = new UShortCollection(_baseAddress + 0x2, 4, 16);

        }
    }
}
