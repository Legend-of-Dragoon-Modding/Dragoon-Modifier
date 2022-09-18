using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Core.Memory {
    public sealed class Shop {
        private readonly int _baseAddress;

        public byte WeaponItemFlag { get { return Emulator.DirectAccess.ReadByte(_baseAddress); } set { Emulator.DirectAccess.WriteByte(_baseAddress, value); } }
        public Collections.IAddress<byte> Item { get; private set; }

        internal Shop(int baseAddress, int shop) {
            _baseAddress = baseAddress + shop * 0x40;
            Item = Collections.Factory.Create<byte>(_baseAddress + 1, 4, 16);

        }
    }
}
