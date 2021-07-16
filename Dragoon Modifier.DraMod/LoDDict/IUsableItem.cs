namespace Dragoon_Modifier.DraMod.LoDDict {
    public interface IUsableItem : IItem {
        byte BaseSwitch { get; }
        string BattleDescription { get; }
        int BattleDescriptionPointer { get; set; }
        int BattleNamePointer { get; set; }
        byte Damage { get; }
        byte Element { get; }
        string EncodedBattleDescription { get; }
        byte Percentage { get; }
        byte Special1 { get; }
        byte Special2 { get; }
        byte SpecialAmmount { get; }
        byte Status { get; }
        byte Target { get; }
        byte Unknown1 { get; }
        byte Unknown2 { get; }
    }
}