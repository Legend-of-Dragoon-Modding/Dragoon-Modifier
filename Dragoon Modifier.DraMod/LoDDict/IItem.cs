namespace Dragoon_Modifier.DraMod.LoDDict {
    public interface IItem {
        string Description { get; }
        int DescriptionPointer { get; set; }
        byte[] EncodedDescription { get; }
        byte[] EncodedName { get; }
        byte Icon { get; }
        byte ID { get; }
        string Name { get; }
        int NamePointer { get; set; }
        short SellPrice { get; }
    }
}