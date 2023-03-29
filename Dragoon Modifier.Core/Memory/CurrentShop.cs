using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Core.Memory {
    public sealed class CurrentShop {
        private readonly int _baseAddress;

        public byte ItemCount { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x44); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x44, value); } }
        public byte WeaponItemFlag { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x45); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x45, value); } }
        public Collections.IAddress<byte> ItemID { get; private set; }
        public Collections.IAddress<ushort> ItemPrice { get; private set; }

        internal CurrentShop(int baseAddress) {
            _baseAddress = baseAddress;
            ItemID = Collections.Factory.Create<byte>(_baseAddress, 4, 16);
            ItemPrice = Collections.Factory.Create<ushort>(_baseAddress + 0x2, 4, 16);
        }
    }
}
