using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.LoDDictionary {
    public class Item {
        static protected readonly Dictionary<string, byte> icons = new Dictionary<string, byte>() {
                { "sword", 0 },
                { "axe", 1 },
                { "hammer", 2 },
                { "spear", 3 },
                { "bow", 4 },
                { "mace", 5 },
                { "knuckle", 6 },
                { "boxing glove", 7 },
                { "clothes", 8 },
                { "robe", 9 },
                { "armor", 10 },
                { "breastplate", 11 },
                { "red dress", 12 },
                { "loincloth", 13 },
                { "warrior dress", 14 },
                { "crown", 15 },
                { "hairband", 16 },
                { "bandana", 16 },
                { "hat", 17 },
                { "helm", 18 },
                { "shoes", 19 },
                { "kneepiece", 20 },
                { "boots", 21 },
                { "bracelet", 22 },
                { "ring", 23 },
                { "amulet", 24 },
                { "stone", 25 },
                { "jewellery", 26 },
                { "ankh", 27 },
                { "bell", 28 },
                { "bag", 29 },
                { "cloak", 30 },
                { "scarf", 30 },
                { "glove", 31 },
                { "horn", 32 },
                { "blue potion", 33 },
                { "yellow potion", 34 },
                { "red potion", 35 },
                { "angel's prayer", 36 },
                { "green potion", 37 },
                { "magic", 38 },
                { "skull", 39 },
                { "up", 40 },
                { "down", 41 },
                { "shield", 42 },
                { "smoke ball", 43 },
                { "sig stone", 44 },
                { "charm", 45 },
                { "sack", 46 },
                { "invalid", 57 },
                { "waring", 58 },
                {"none", 64 },
                {"", 64 }
            };

        string _name = " ";
        string _encodedName = "00 00 FF A0";
        string _description = " ";
        string _encodedDescription = "00 00 FF A0";
        byte _icon = 64;
        short _sellPrice = 0;


        public byte Id { get; protected set; }
        public string Name { get { return _name; } protected set { _name = value; ; } }
        public string EncodedName { get { return _encodedName; } protected set { _encodedName = value; } }
        public uint NamePointer { get; set; }
        public string Description { get { return _description; } protected set { _description = value; } }
        public string EncodedDescription { get { return _encodedDescription; } protected set { _encodedDescription = value; } }
        public uint DescriptionPointer { get; set; }
        public byte Icon { get { return _icon; } set { _icon = value; } }
        public short SellPrice { get { return _sellPrice; } protected set { _sellPrice = value; } }
    }
}
