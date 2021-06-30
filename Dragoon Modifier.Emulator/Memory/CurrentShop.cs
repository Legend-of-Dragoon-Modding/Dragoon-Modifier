using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Emulator.Memory {
    public class CurrentShop {
        private readonly IEmulator _emulator;
        private readonly int _baseAddress;

        public byte ItemCount { get { return _emulator.ReadByte(_baseAddress + 0x44); } set { _emulator.WriteByte(_baseAddress + 0x44, value); } }
        public byte WeaponItemFlag { get { return _emulator.ReadByte(_baseAddress + 0x45); } set { _emulator.WriteByte(_baseAddress + 0x45, value); } }
        public Collections.IAddress<byte> ItemID { get; private set; }
        public Collections.IAddress<ushort> ItemPrice { get; private set; }

        internal CurrentShop(IEmulator emulator, int baseAddress) {
            _emulator = emulator;
            _baseAddress = baseAddress;
            ItemID = Factory.AddressCollection<byte>(emulator, _baseAddress, 4, 16);
            ItemPrice = Factory.AddressCollection<ushort>(emulator, _baseAddress + 0x2, 4, 16);
        }
    }
}
