using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.MemoryController {
    public class DragoonStatTable {
        int _baseAddress;
        DragoonLevel[] _level = new DragoonLevel[6];
        public DragoonLevel[] Level { get { return _level; } }

        public DragoonStatTable(int baseAddress, int slot) {
            _baseAddress = baseAddress + slot * 0x30;
            for (int i = 0; i < _level.Length; i++) {
                _level[i] = new DragoonLevel(_baseAddress, i);
            }
        }
    }
}
