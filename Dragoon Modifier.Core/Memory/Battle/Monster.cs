using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Core.Memory.Battle {
    public class Monster : Entity {
        private readonly int _rewardsAddress;
        private readonly int _idAddress;
        private readonly int _nameAddress;

        public string Name { get { return Emulator.DirectAccess.ReadText(_nameAddress); } } // TODO writing. Might require custom function with limit of 20 characters
        public byte Element { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x6A); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x6A, value); } }
        public byte Display_Element { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0x14); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0x14, value); } }
        public byte SigStone { get { return (byte) ((Emulator.DirectAccess.ReadByte(_pShieldMShieldSigStone) >> 2) & 3); } set { Emulator.DirectAccess.WriteByte(_pShieldMShieldSigStone, (byte) (Emulator.DirectAccess.ReadByte(_pShieldMShieldSigStone) | (Math.Max(value, (byte) 3) << 2))); } }
        public byte ChargingSpirit { get { return Emulator.DirectAccess.ReadByte(_pShieldMShieldSigStone + 0x2); } set { Emulator.DirectAccess.WriteByte(_pShieldMShieldSigStone + 0x2, value); } }
        public ushort Expirience { get { return Emulator.Memory.Battle.RewardsExp[UniqueIndex]; } set { Emulator.Memory.Battle.RewardsExp[UniqueIndex] = value; } }
        public ushort Gold { get { return Emulator.Memory.Battle.RewardsGold[UniqueIndex]; } set { Emulator.Memory.Battle.RewardsGold[UniqueIndex] = value; } }
        public byte DropChance { get { return Emulator.Memory.Battle.RewardsDropChance[UniqueIndex]; } set { Emulator.Memory.Battle.RewardsDropChance[UniqueIndex] = value; } }
        public byte ItemDrop { get { return Emulator.Memory.Battle.RewardsItemDrop[UniqueIndex]; } set { Emulator.Memory.Battle.RewardsItemDrop[UniqueIndex] = value; } }
        public ushort ID { get { return Emulator.DirectAccess.ReadUShort(_idAddress); } }
        public byte AttackMove { get { return Emulator.DirectAccess.ReadByte(_baseAddress + 0xACC); } set { Emulator.DirectAccess.WriteByte(_baseAddress + 0xACC, value); } }

        public Monster(uint m_point, int slot, int position, int battleOffset) : base(m_point, slot, position) {
            _idAddress = Emulator.GetAddress("MONSTER_ID") + battleOffset + slot * 0x8;
            _nameAddress = Emulator.GetAddress("MONSTER_NAMES") + (0x2C * slot);
        }
    }
}
