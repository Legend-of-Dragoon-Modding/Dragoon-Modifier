using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Emulator.Memory.Battle {
    public class Monster : Entity {
        private readonly int _rewardsAddress;
        private readonly int _idAddress;

        public byte Element { get { return _emulator.ReadByte(_baseAddress + 0x6A); } set { _emulator.WriteByte(_baseAddress + 0x6A, value); } }
        public byte Display_Element { get { return _emulator.ReadByte(_baseAddress + 0x14); } set { _emulator.WriteByte(_baseAddress + 0x14, value); } }
        public byte SigStone { get { return (byte) ((_emulator.ReadByte(_pShieldMShieldSigStone) >> 2) & 3); } set { _emulator.WriteByte(_pShieldMShieldSigStone, (byte) (_emulator.ReadByte(_pShieldMShieldSigStone) | (Math.Max(value, (byte) 3) << 2))); } }
        public byte ChargingSpirit { get { return _emulator.ReadByte(_pShieldMShieldSigStone + 0x2); } set { _emulator.WriteByte(_pShieldMShieldSigStone + 0x2, value); } }
        public ushort Expirience { get { return _emulator.ReadUShort(_rewardsAddress + UniqueIndex * 0x1A8); } set { _emulator.WriteUShort(_rewardsAddress + UniqueIndex * 0x1A8, value); } }
        public ushort Gold { get { return _emulator.ReadUShort(_rewardsAddress + 0x2 + UniqueIndex * 0x1A8); } set { _emulator.WriteUShort(_rewardsAddress + 0x2 + UniqueIndex * 0x1A8, value); } }
        public byte DropChance { get { return _emulator.ReadByte(_rewardsAddress + 0x4 + UniqueIndex * 0x1A8); } set { _emulator.WriteByte(_rewardsAddress + 0x4 + UniqueIndex * 0x1A8, value); } }
        public byte ItemDrop { get { return _emulator.ReadByte(_rewardsAddress + 0x5 + UniqueIndex * 0x1A8); } set { _emulator.WriteByte(_rewardsAddress + 0x5 + UniqueIndex * 0x1A8, value); } }
        public ushort ID { get { return _emulator.ReadUShort(_idAddress); } }
        public byte AttackMove { get { return _emulator.ReadByte(_baseAddress + 0xACC); } set { _emulator.WriteByte(_baseAddress + 0xACC, value); } }

        public Monster(IEmulator emulator, uint m_point, int slot, int position, int battleOffset) : base(emulator, m_point, slot, position) {
            _rewardsAddress = _emulator.GetAddress("MONSTER_REWARDS");
            _idAddress = _emulator.GetAddress("MONSTER_ID") + battleOffset + slot * 0x8;
        }
    }
}
