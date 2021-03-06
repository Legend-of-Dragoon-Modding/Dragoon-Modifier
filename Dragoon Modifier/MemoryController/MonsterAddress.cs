using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.MemoryController {
    class MonsterAddress : BattleEntityAddress {
        long _element;
        long _displayElement;
        long _exp;
        long _gold;
        long _dropChance;
        long _dropItem;
        long _attackMove;

        public byte Element { get { return Emulator.ReadByte(_element); } set { Emulator.WriteByte(_element, value); } }
        public byte Display_Element { get { return Emulator.ReadByte(_displayElement); } set { Emulator.WriteByte(_displayElement, value); } }
        //public byte MagicalShield { get { return (byte) ((Emulator.ReadByte(_pShieldMshieldSigStone) >> 2) & 3); } set { Emulator.WriteByte(_pShieldMshieldSigStone, Emulator.ReadByte(_pShieldMshieldSigStone) | (Math.Max(value, (byte) 3) << 2)); } }
        // Gotta figure out how to access private of base class
        public MonsterAddress(long m_point, int slot, int position) : base(m_point, slot, position) {
            long curr_point = m_point - slot * 0x388;
            _element = curr_point + 0x6A;
            _displayElement = curr_point + 0x14;
        }
    }
}
