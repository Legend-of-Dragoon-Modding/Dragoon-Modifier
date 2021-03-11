using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.MemoryController {
    class MonsterAddress : BattleEntityAddress {
        int _rewardsAddress;
        int _idAddress;

        // exp
        // gold;
        // dropChance;
        // dropItem;
        // attackMove;

        public byte Element { get { return Emulator.ReadByte(_baseAddress + 0x6A); } set { Emulator.WriteByte(_baseAddress + 0x6A, value); } }
        public byte Display_Element { get { return Emulator.ReadByte(_baseAddress + 0x14); } set { Emulator.WriteByte(_baseAddress + 0x14, value); } }
        public byte SigStone { get { return (byte) ((Emulator.ReadByte(_pShieldMshieldSigStone) >> 2) & 3); } set { Emulator.WriteByte(_pShieldMshieldSigStone, Emulator.ReadByte(_pShieldMshieldSigStone) | (Math.Max(value, (byte) 3) << 2)); } }
        public byte ChargingSpirit { get { return Emulator.ReadByte(_pShieldMshieldSigStone + 0x2); } set { Emulator.WriteByte(_pShieldMshieldSigStone + 0x2, value); } }
        public ushort Expirience { get { return Emulator.ReadUShort(_rewardsAddress + UniqueIndex * 0x1A8); } set { Emulator.WriteUShort(_rewardsAddress + UniqueIndex * 0x1A8, value); } }
        public ushort Gold { get { return Emulator.ReadUShort(_rewardsAddress + 0x2 + UniqueIndex * 0x1A8); } set { Emulator.WriteUShort(_rewardsAddress + 0x2 + UniqueIndex * 0x1A8, value); } }
        public byte DropChance { get { return Emulator.ReadByte(_rewardsAddress + 0x4 + UniqueIndex * 0x1A8); } set { Emulator.WriteByte(_rewardsAddress + 0x4 + UniqueIndex * 0x1A8, value); } }
        public byte ItemDrop { get { return Emulator.ReadByte(_rewardsAddress + 0x5 + UniqueIndex * 0x1A8); } set { Emulator.WriteByte(_rewardsAddress + 0x5 + UniqueIndex * 0x1A8, value); } }
        public ushort ID { get { return Emulator.ReadUShort(_idAddress); } }

        public MonsterAddress(int m_point, int slot, int position, int battleOffset) : base(m_point, slot, position) {
            _rewardsAddress = Constants.GetAddress("MONSTER_REWARDS");
            _idAddress = Constants.GetAddress("MONSTER_ID") + battleOffset + slot * 0x8;
        }
    }
}
