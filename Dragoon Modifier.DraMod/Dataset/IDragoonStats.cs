
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Dataset {
    public interface IDragoonStats {
        ushort DAT { get; set; }
        ushort DDF { get; set; }
        ushort DMAT { get; set; }
        ushort DMDF { get; set; }
        ushort MP { get; set; }
    }
}
