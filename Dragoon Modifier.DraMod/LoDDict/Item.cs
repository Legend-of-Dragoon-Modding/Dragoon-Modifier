using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.LoDDict {
    internal class Item : IItem {
        public byte ID { get; protected set; }
        public string Name { get; protected set; } = "<END>";
        public string EncodedName { get; protected set; } = "FF A0 FF A0";
        public long NamePointer { get; set; } = 0;
        public string Description { get; protected set; } = "<END>";
        public string EncodedDescription { get; protected set; } = "FF A0 FF A0";
        public long DescriptionPointer { get; set; } = 0;
        public byte Icon { get; protected set; } = 0;
        public short Sell_Price { get; protected set; } = 0;
    }
}
