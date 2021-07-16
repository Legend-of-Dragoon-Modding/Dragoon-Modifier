namespace Dragoon_Modifier.DraMod.LoDDict {
    public interface ILoDDictionary {
        IItem[] Item { get; }

        string ItemNames { get; }
        string ItemDescriptions { get; }
        string ItemBattleNames { get; }
        string ItemBattleDescriptions { get; }
    }
}