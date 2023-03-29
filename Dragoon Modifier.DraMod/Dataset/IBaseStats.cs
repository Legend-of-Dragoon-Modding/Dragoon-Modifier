using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Dataset {
    public  interface IBaseStats {
        ushort[] HP { get; }
        byte[] AT { get; }
        byte[] MAT { get; }
        byte[] DF { get; }
        byte[] MDF { get; }
        byte[] SPD { get; }
    }
}
