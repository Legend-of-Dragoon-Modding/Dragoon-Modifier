using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.LoDDictionary {
    public class UsableItem : Item {
        string _battleDescription = " ";
        string _encodedBattleDescription = "00 00 FF A0";



        public string BattleDescription { get { return _battleDescription; } private set { _battleDescription = value; } }
        public string EncodedBattleDescription { get { return _encodedBattleDescription; } private set { _encodedBattleDescription = value; } }
        public long BattleDescriptionPointer { get; set; }
        public long BattleNamePointer { get; set; }
    }
}
