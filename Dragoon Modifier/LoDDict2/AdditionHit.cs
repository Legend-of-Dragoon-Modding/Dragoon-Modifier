using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.LoDDict2 {
    public class AdditionHit {
        short _unknown1 = 0;
        short _nextHit = 0;
        short _blueTime = 0;
        short _grayTime = 0;
        short _damage = 0;
        short _sp = 0;
        short _id = 0;
        byte _finalHit = 0;
        byte _unknown2 = 0;
        byte _unknown3 = 0;
        byte _unknown4 = 0;
        byte _unknown5 = 0;
        byte _unknown6 = 0;
        byte _unknown7 = 0;
        byte _unknown8 = 0;
        byte _unknown9 = 0;
        byte _unknown10 = 0;
        short _verticalDistance = 0;
        byte _unknown11 = 0;
        byte _unknown12 = 0;
        byte _unknown13 = 0;
        byte _unknown14 = 0;
        byte _startTime = 0;
        byte _unknown15 = 0;

        public AdditionHit(string[] values) {
            if (Int16.TryParse(values[0], out short skey)) {

            }
        }
    }
}
