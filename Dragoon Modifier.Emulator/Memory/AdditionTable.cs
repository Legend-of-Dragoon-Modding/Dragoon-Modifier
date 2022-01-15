using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Emulator.Memory {
    public class AdditionTable {
        private readonly int _baseAddress;
        private readonly int _multiAddress;
        private readonly IEmulator _emulator;

        public byte Level { get { return _emulator.ReadByte(_baseAddress); } set { _emulator.WriteByte(_baseAddress, value); } }
        public byte Hits { get { return _emulator.ReadByte(_baseAddress + 0x1); } set { _emulator.WriteByte(_baseAddress + 0x1, value); } }
        public Collections.IAddress<ushort> SP { get; private set; }
        public ushort Damage { get { return _emulator.ReadUShort(_baseAddress + 0xC); } set { _emulator.WriteUShort(_baseAddress + 0xC, value); } }
        public Collections.IAddress<byte> DamageLevelMultiplier { get; private set; }

        internal AdditionTable(IEmulator emulator, int baseAddress, int multiAddress, int addition) {
            _emulator = emulator;
            _baseAddress = baseAddress + addition * 0xE;
            _multiAddress = multiAddress + addition * 0x18;
            SP = Factory.AddressCollection<ushort>(_emulator, (_baseAddress + 0x2), 2, 5);
            DamageLevelMultiplier = Factory.AddressCollection<byte>(_emulator, _multiAddress, 4, 5);
        }
    }
}
