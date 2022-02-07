using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Emulator.Memory.Battle {
    public class Monster : Entity {
        private readonly int _rewardsAddress;
        private readonly int _idAddress;
        private readonly int _nameAddress;

        public string Name { get { return _emulator.ReadText(_nameAddress); } } // TODO writing. Might require custom function with limit of 20 characters
        public byte Element { get { return _emulator.ReadByte(_baseAddress + 0x6A); } set { _emulator.WriteByte(_baseAddress + 0x6A, value); } }
        public byte Display_Element { get { return _emulator.ReadByte(_baseAddress + 0x14); } set { _emulator.WriteByte(_baseAddress + 0x14, value); } }
        public byte SigStone { get { return (byte) ((_emulator.ReadByte(_pShieldMShieldSigStone) >> 2) & 3); } set { _emulator.WriteByte(_pShieldMShieldSigStone, (byte) (_emulator.ReadByte(_pShieldMShieldSigStone) | (Math.Max(value, (byte) 3) << 2))); } }
        public byte ChargingSpirit { get { return _emulator.ReadByte(_pShieldMShieldSigStone + 0x2); } set { _emulator.WriteByte(_pShieldMShieldSigStone + 0x2, value); } }
        public ushort Expirience { get { return _emulator.Battle.RewardsExp[UniqueIndex]; } set { _emulator.Battle.RewardsExp[UniqueIndex] = value; } }
        public ushort Gold { get { return _emulator.Battle.RewardsGold[UniqueIndex]; } set { _emulator.Battle.RewardsGold[UniqueIndex] = value; } }
        public byte DropChance { get { return _emulator.Battle.RewardsDropChance[UniqueIndex]; } set { _emulator.Battle.RewardsDropChance[UniqueIndex] = value; } }
        public byte ItemDrop { get { return _emulator.Battle.RewardsItemDrop[UniqueIndex]; } set { _emulator.Battle.RewardsItemDrop[UniqueIndex] = value; } }
        public ushort ID { get { return _emulator.ReadUShort(_idAddress); } }
        public byte AttackMove { get { return _emulator.ReadByte(_baseAddress + 0xACC); } set { _emulator.WriteByte(_baseAddress + 0xACC, value); } }

        public Monster(IEmulator emulator, uint m_point, int slot, int position, int battleOffset) : base(emulator, m_point, slot, position) {
            _idAddress = _emulator.GetAddress("MONSTER_ID") + battleOffset + slot * 0x8;
            _nameAddress = _emulator.GetAddress("MONSTER_NAMES") + (0x2C * slot);
        }
    }
}