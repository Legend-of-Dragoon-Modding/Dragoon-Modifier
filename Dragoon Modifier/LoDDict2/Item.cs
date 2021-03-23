using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.LoDDict2 {
    public class Item {
        protected byte id;
        protected string name = "<END>";
        protected string encodedName = "FF A0 FF A0";
        long namePointer = 0;
        protected string description = "<END>";
        protected string encodedDescription = "FF A0 FF A0";
        long descriptionPointer = 0;
        protected byte icon = 0;
        protected short sell_price = 0;

        Dictionary<string, byte> iconDict = new Dictionary<string, byte>() {
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

        public byte ID { get { return id; } }
        public string Name { get { return name; } }
        public string EncodedName { get { return encodedName; } }
        public long NamePointer { get; set; }
        public string Description { get { return description; } }
        public string EncodedDescription { get { return encodedDescription; } }
        public long DescriptionPointer { get; set; }
        public byte Icon { get { return icon; } }
        public short Sell_Price { get { return sell_price; } }

        public Dictionary<string, byte> IconDict { get { return iconDict; } }
    }

    

    
}
