using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.MemoryController {
    public class Shop {
        long _baseAddress;
        ByteCollection _item;

        public byte WeaponItemFlag { get { return Emulator.ReadByte(_baseAddress); } set { Emulator.WriteByte(_baseAddress, value); } }
        public ByteCollection Item { get { return _item; } }

        public Shop(long baseAddress, int shop) {
            _baseAddress = baseAddress + shop * 0x40;
            _item = new ByteCollection(_baseAddress + 1, 4, 16);

        }
    }
}
