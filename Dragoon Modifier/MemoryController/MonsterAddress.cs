using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.MemoryController {
    class MonsterAddress : BattleEntityAddress {

        // exp
        // gold;
        // dropChance;
        // dropItem;
        // attackMove;

        public byte Element { get { return Emulator.ReadByte(_baseAddress + 0x6A); } set { Emulator.WriteByte(_baseAddress + 0x6A, value); } }
        public byte Display_Element { get { return Emulator.ReadByte(_baseAddress + 0x14); } set { Emulator.WriteByte(_baseAddress + 0x14, value); } }
        public byte SigStone { get { return (byte) ((Emulator.ReadByte(_pShieldMshieldSigStone) >> 2) & 3); } set { Emulator.WriteByte(_pShieldMshieldSigStone, Emulator.ReadByte(_pShieldMshieldSigStone) | (Math.Max(value, (byte) 3) << 2)); } }
        public byte ChargingSpirit { get { return Emulator.ReadByte(_pShieldMshieldSigStone + 0x2); } set { Emulator.WriteByte(_pShieldMshieldSigStone + 0x2, value); } }
    
        public MonsterAddress(long m_point, int slot, int position) : base(m_point, slot, position) {
        }
    }
}
