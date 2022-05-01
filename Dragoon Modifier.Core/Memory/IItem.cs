using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Core.Memory {
    public interface IItem {
        string Description { get; }
        uint DescriptionPointer { get; set; }
        byte Icon { get; set; }
        string Name { get; }
        uint NamePointer { get; set; }
        ushort SellPrice { get; set; }
    }
}
