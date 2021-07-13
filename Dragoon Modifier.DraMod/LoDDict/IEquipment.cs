namespace Dragoon_Modifier.DraMod.LoDDict {
    public interface IEquipment : IItem {
        byte A_AV { get; }
        byte A_HIT { get; }
        ushort AT { get; }
        byte DF { get; }
        byte ElementalImmunity { get; }
        byte ElementalResistance { get; }
        byte M_AV { get; }
        byte M_HIT { get; }
        byte MAT { get; }
        byte MDF { get; }
        byte OnHitStatus { get; }
        byte OnHitStatusChance { get; }
        byte SPD { get; }
        byte SpecialBonus1 { get; }
        byte SpecialBonus2 { get; }
        byte SpecialBonusAmmount { get; }
        byte SpecialEffect { get; }
        byte StatusResistance { get; }
        byte Type { get; }
        byte WeaponElement { get; }
        byte WhoEquips { get; }
    }
}