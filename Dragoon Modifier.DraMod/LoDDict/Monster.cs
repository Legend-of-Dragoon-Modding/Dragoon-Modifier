using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.LoDDict {
    public class Monster {
        public string Name { get; private set; } = "Monster";
        public byte Element { get; private set; } = 128;
        public uint HP { get; private set; } = 1;
        public ushort AT { get; private set; } = 1;
        public ushort MAT { get; private set; } = 1;
        public ushort DF { get; private set; } = 1;
        public ushort MDF { get; private set; } = 1;
        public ushort SPD { get; private set; } = 1;
        public short A_AV { get; private set; } = 0;
        public short M_AV { get; private set; } = 0;
        public byte PhysicalImmunity { get; private set; } = 0;
        public byte PhysicalResistance { get; private set; } = 0;
        public byte MagicalImmunity { get; private set; } = 0;
        public byte MagicalResistance { get; private set; } = 0;
        public byte ElementalImmunity { get; private set; } = 0;
        public byte ElementalResistance { get; private set; } = 0;
        public byte StatusResist { get; private set; } = 99;
        public byte SpecialEffect { get; private set; } = 0;
        public ushort EXP { get; private set; } = 0;
        public ushort Gold { get; private set; } = 0;
        public byte DropItem { get; private set; } = 255;
        public byte DropChance { get; private set; } = 0;

        internal Monster() {

        }
        
    }
}
