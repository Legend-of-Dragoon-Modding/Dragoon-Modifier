using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.MemoryController {
    class MonsterAddress : BattleAddress {
        long _exp;
        long _gold;
        long _dropChance;
        long _dropItem;


        public MonsterAddress(long m_point, int position) : base(m_point, position) {

        }
    }
}
