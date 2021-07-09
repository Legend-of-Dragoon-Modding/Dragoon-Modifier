namespace Dragoon_Modifier.Emulator.Memory {
    public interface IItem {
        string Description { get; }
        uint DescriptionPointer { get; set; }
        byte Icon { get; set; }
        string Name { get; }
        uint NamePointer { get; set; }
        ushort SellPrice { get; set; }
    }
}