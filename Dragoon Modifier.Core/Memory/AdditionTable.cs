using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Core.Memory {
    public class AdditionTable {
        private readonly int _baseAddress;
        private readonly int _multiAddress;

        public byte Level { get { return Emulator.DirectAccess.ReadByte(_baseAddress); } set { Emulator.DirectAccess.WriteByte(_baseAddress, value); } }
        public byte Hits { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x1); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x1, value); } }
        public Collections.IAddress<ushort> SP { get; private set; }
        public ushort Damage { get { return Emulator.DirectAccess.ReadUShort(_baseAddress + 0xC); } set { Emulator.DirectAccess.WriteUShort(_baseAddress + 0xC, value); } }
        public Collections.IAddress<byte> DamageLevelMultiplier { get; private set; }

        internal AdditionTable(int baseAddress, int multiAddress, int addition) {
            _baseAddress = baseAddress + addition * 0xE;
            _multiAddress = multiAddress + addition * 0x18;
            SP = Collections.Factory.Create<ushort>(_baseAddress + 0x2, 2, 5);
            DamageLevelMultiplier = Collections.Factory.Create<byte>(_multiAddress, 4, 5);
        }
    }
}
