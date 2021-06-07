using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Core.Memory.Battle {
    public class Monster : BattleEntity {
        private readonly int _rewardsAddress;
        private readonly int _idAddress;

        public byte Element { get { return Emulator.ReadByte(_baseAddress + 0x6A); } set { Emulator.WriteByte(_baseAddress + 0x6A, value); } }
        public byte Display_Element { get { return Emulator.ReadByte(_baseAddress + 0x14); } set { Emulator.WriteByte(_baseAddress + 0x14, value); } }
        public byte SigStone { get { return (byte) ((Emulator.ReadByte(_pShieldMShieldSigStone) >> 2) & 3); } set { Emulator.WriteByte(_pShieldMShieldSigStone, (byte) (Emulator.ReadByte(_pShieldMShieldSigStone) | (Math.Max(value, (byte) 3) << 2))); } }
        public byte ChargingSpirit { get { return Emulator.ReadByte(_pShieldMShieldSigStone + 0x2); } set { Emulator.WriteByte(_pShieldMShieldSigStone + 0x2, value); } }
        public ushort Expirience { get { return Emulator.ReadUShort(_rewardsAddress + UniqueIndex * 0x1A8); } set { Emulator.WriteUShort(_rewardsAddress + UniqueIndex * 0x1A8, value); } }
        public ushort Gold { get { return Emulator.ReadUShort(_rewardsAddress + 0x2 + UniqueIndex * 0x1A8); } set { Emulator.WriteUShort(_rewardsAddress + 0x2 + UniqueIndex * 0x1A8, value); } }
        public byte DropChance { get { return Emulator.ReadByte(_rewardsAddress + 0x4 + UniqueIndex * 0x1A8); } set { Emulator.WriteByte(_rewardsAddress + 0x4 + UniqueIndex * 0x1A8, value); } }
        public byte ItemDrop { get { return Emulator.ReadByte(_rewardsAddress + 0x5 + UniqueIndex * 0x1A8); } set { Emulator.WriteByte(_rewardsAddress + 0x5 + UniqueIndex * 0x1A8, value); } }
        public ushort ID { get { return Emulator.ReadUShort(_idAddress); } }
        public byte AttackMove { get { return Emulator.ReadByte(_baseAddress + 0xACC); } set { Emulator.WriteByte(_baseAddress + 0xACC, value); } }

        public Monster(uint m_point, int slot, int position, int battleOffset) : base(m_point, slot, position) {
            _rewardsAddress = Emulator.GetAddress("MONSTER_REWARDS");
            _idAddress = Emulator.GetAddress("MONSTER_ID") + battleOffset + slot * 0x8;
        }
    }
}
