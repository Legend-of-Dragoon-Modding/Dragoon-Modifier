using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.LoDDict.Scripts.HardMode.Characters {
    internal interface ICharacter {
        void Run(byte slot, byte dragoonSpecialAttack);
    }
}
