namespace Dragoon_Modifier.Emulator.Memory {
    public interface IUsableItem : IItem {
        byte BaseSwitch { get; set; }
        byte Damage { get; set; }
        byte Element { get; set; }
        byte Percentage { get; set; }
        byte Special1 { get; set; }
        byte Special2 { get; set; }
        byte SpecialAmmount { get; set; }
        byte Status { get; set; }
        byte Target { get; set; }
        byte Unknown1 { get; set; }
        byte Unknown2 { get; set; }
    }
}