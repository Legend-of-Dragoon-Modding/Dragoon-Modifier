namespace Dragoon_Modifier.DraMod.LoDDict {
    public interface IItem {
        string Description { get; }
        long DescriptionPointer { get; set; }
        string EncodedDescription { get; }
        string EncodedName { get; }
        byte Icon { get; }
        byte ID { get; }
        string Name { get; }
        long NamePointer { get; set; }
        short Sell_Price { get; }
    }
}