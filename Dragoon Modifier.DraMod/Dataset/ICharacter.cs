using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Dataset {
    public interface ICharacter {
        IBaseStats BaseStats { get; }
        Addition[] Additions { get; }
    }
}
