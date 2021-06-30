using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Emulator.Memory {
    public class Shop {
        private readonly int _baseAddress;

        private readonly IEmulator _emulator;

        public byte WeaponItemFlag { get { return _emulator.ReadByte(_baseAddress); } set { _emulator.WriteByte(_baseAddress, value); } }
        public Collections.IAddress<byte> Item { get; private set; }

        internal Shop(IEmulator emulator, int baseAddress, int shop) {
            _emulator = emulator;
            _baseAddress = baseAddress + shop * 0x40;
            Item = Factory.AddressCollection<byte>(emulator, _baseAddress + 1, 4, 16);

        }
    }
}
