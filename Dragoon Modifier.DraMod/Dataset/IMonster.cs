using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Dataset {
    public interface IMonster {
        string Name { get; }
        byte Element { get; }
        uint HP { get; }
        ushort AT { get; }
        ushort MAT { get; }
        ushort DF { get; }
        ushort MDF { get; }
        ushort SPD { get; }
        short A_AV { get; }
        short M_AV { get; }
        byte PhysicalImmunity { get; }
        byte PhysicalResistance { get; }
        byte MagicalImmunity { get; }
        byte MagicalResistance { get; }
        byte ElementalImmunity { get; }
        byte ElementalResistance { get; }
        byte StatusResist { get; }
        byte SpecialEffect { get; }
        ushort EXP { get; }
        ushort Gold { get; }
        byte DropItem { get; }
        byte DropChance { get; }
    }
}
