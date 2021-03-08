using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.MemoryController {
    public class CharacterStatTable {
        long _baseAddress;
        Level[] _level = new Level[61];



        public CharacterStatTable(long baseAddress, int slot) {
            _baseAddress = baseAddress + slot * 0x1E8;
            for (int i = 0; i < _level.Length; i++) {
                _level[i] = new Level(_baseAddress, i);
            }
        }
    }
}
