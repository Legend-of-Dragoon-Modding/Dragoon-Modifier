using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Dataset {
    public interface IDragoonSpells {
        bool Percentage { get; set; }
        double Damage { get; set; }
        byte Accuracy { get; set; }
        byte MP { get; set; }
        byte Element { get; set; }
        string Description { get; set; }
        byte[] Encoded_Description { get; set; }
        long Description_Pointer { get; set; }
    }
}
