using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Dataset {
    internal class Item : IItem {
        public byte ID { get; protected set; }
        public string Name { get; protected set; } = "<END>";
        public byte[] EncodedName { get; protected set; } = new byte[] { 0xFF, 0xA0, 0xFF, 0xA0 };
        public int NamePointer { get; set; } = 0;
        public string Description { get; protected set; } = "<END>";
        public byte[] EncodedDescription { get; protected set; } = new byte[] { 0xFF, 0xA0, 0xFF, 0xA0 };
        public int DescriptionPointer { get; set; } = 0;
        public byte Icon { get; protected set; } = 0;
        public short SellPrice { get; protected set; } = 0;
    }
}
