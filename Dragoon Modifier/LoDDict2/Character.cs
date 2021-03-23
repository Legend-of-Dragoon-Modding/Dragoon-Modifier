using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.LoDDict2 {
    public class Character {
        Level[] _baseStats = new Level[61];

        public Level[] BaseStats { get { return _baseStats; } }

        public Character(Level[] baseStats) {
            _baseStats = baseStats;
        }
    }
}
