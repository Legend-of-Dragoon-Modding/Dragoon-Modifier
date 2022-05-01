using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Core.Memory {
    public interface IEquipment : IItem {
        byte A_AV { get; set; }
        byte A_HIT { get; set; }
        byte AT { get; set; }
        byte AT2 { get; set; }
        byte DF { get; set; }
        byte E_Half { get; set; }
        byte E_Immune { get; set; }
        byte ItemType { get; set; }
        byte M_AV { get; set; }
        byte M_HIT { get; set; }
        byte MAT { get; set; }
        byte MDF { get; set; }
        byte SPD { get; set; }
        byte Special1 { get; set; }
        byte Special2 { get; set; }
        byte SpecialAmmount { get; set; }
        byte SpecialEffect { get; set; }
        byte Status { get; set; }
        byte StatusChance { get; set; }
        byte StatusResist { get; set; }
        byte WeaponElement { get; set; }
        byte WhoEquips { get; set; }
    }
}
