using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.LoDDict {
    internal class Equipment : Item, IEquipment {
        public byte WhoEquips { get; private set; } = 0;
        public byte Type { get; private set; } = 0;
        public byte WeaponElement { get; private set; } = 0;
        public byte OnHitStatus { get; private set; } = 0;
        public byte OnHitStatusChance { get; private set; } = 0;
        public ushort AT { get; private set; } = 0;
        public byte MAT { get; private set; } = 0;
        public byte DF { get; private set; } = 0;
        public byte MDF { get; private set; } = 0;
        public byte SPD { get; private set; } = 0;
        public byte A_HIT { get; private set; } = 0;
        public byte M_HIT { get; private set; } = 0;
        public byte A_AV { get; private set; } = 0;
        public byte M_AV { get; private set; } = 0;
        public byte ElementalResistance { get; private set; } = 0;
        public byte ElementalImmunity { get; private set; } = 0;
        public byte StatusResistance { get; private set; } = 0;
        public byte SpecialBonus1 { get; private set; } = 0;
        public byte SpecialBonus2 { get; private set; } = 0;
        public byte SpecialBonusAmmount { get; private set; } = 0;
        public byte SpecialEffect { get; private set; } = 0;
    }
}
