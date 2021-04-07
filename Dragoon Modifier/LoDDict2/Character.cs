using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.LoDDict2 {
    public class Character {
        Level[] _baseStats = new Level[61];
        Addition[] _additions;

        public Level[] BaseStats { get { return _baseStats; } }
        public Addition[] Addition { get { return _additions; } }

        public Character(Level[] baseStats, Addition[] additionData) {
            _baseStats = baseStats;
            _additions = additionData;
        }
    }
}
