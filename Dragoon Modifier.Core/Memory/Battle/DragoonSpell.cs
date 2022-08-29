using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Core.Memory.Battle {
    public class DragoonSpell {
        protected int _baseAddress;
        protected int spellID;


        public byte IntValue { get { return Emulator.DirectAccess.ReadByte(_baseAddress + (spellID * 0xC) + 0x2); } set { Emulator.DirectAccess.WriteByte(_baseAddress + (spellID * 0xC) + 0x2, value); } }
        public byte DamageBase { get { return Emulator.DirectAccess.ReadByte(_baseAddress + (spellID * 0xC) + 0x4); } set { Emulator.DirectAccess.WriteByte(_baseAddress + (spellID * 0xC) + 0x4, value); } }
        public byte DamageMultiplier { get { return Emulator.DirectAccess.ReadByte(_baseAddress + (spellID * 0xC) + 0x5); } set { Emulator.DirectAccess.WriteByte(_baseAddress + (spellID * 0xC) + 0x5, value); } }
        public byte Accuracy { get { return Emulator.DirectAccess.ReadByte(_baseAddress + (spellID * 0xC) + 0x6); } set { Emulator.DirectAccess.WriteByte(_baseAddress + (spellID * 0xC) + 0x6, value); } }
        public byte MP { get { return Emulator.DirectAccess.ReadByte(_baseAddress + (spellID * 0xC) + 0x7); } set { Emulator.DirectAccess.WriteByte(_baseAddress + (spellID * 0xC) + 0x7, value); } }
        public byte Element { get { return Emulator.DirectAccess.ReadByte(_baseAddress + (spellID * 0xC) + 0x9); } set { Emulator.DirectAccess.WriteByte(_baseAddress + (spellID * 0xC) + 0x9, value); } }
        

        public DragoonSpell(int id) {
            _baseAddress = Emulator.GetAddress("SPELL_TABLE");
            spellID = id;
        }
    }
}
