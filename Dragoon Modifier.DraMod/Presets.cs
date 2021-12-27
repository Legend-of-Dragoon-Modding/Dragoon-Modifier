using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod {
    public enum Preset {
        [Description("US_Base")]
        Normal = 1,
        [Description("Hard_Mode")]
        NormalHard = 2,
        [Description("Hard_Mode")]
        Hard = 3,
        [Description("Hell_Mode")]
        HardHell = 4,
        [Description("Hell_Mode")]
        Hell = 5
    }
}
