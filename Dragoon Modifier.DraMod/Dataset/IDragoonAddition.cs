using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Dataset {
    public interface IDragoonAddition {
        ushort HIT1 { get; }
        ushort HIT2 { get; }
        ushort HIT3 { get; }
        ushort HIT4 { get; }
        ushort HIT5 { get; }
    }
}
