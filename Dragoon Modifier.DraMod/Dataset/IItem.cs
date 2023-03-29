using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Dataset {
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
