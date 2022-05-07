using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Core.Memory {
    public class DragoonStatTable {
        private static readonly byte[] _reorderCharacter = new byte[] { 2, 6, 8, 7, 3, 0, 4, 1, 5 };
        private readonly StatTable[] _statTable = new StatTable[9];

        public StatTable this[int i] {
            get {
                return _statTable[_reorderCharacter[i]];
            }
        }

        internal DragoonStatTable(int baseAddress) {
            for (int i = 0; i < _statTable.Length; i++) {
                _statTable[i] = new StatTable(baseAddress + i * 0x30);
            }
        }

        public class StatTable {
            public Collections.IAddress<ushort> MaxMP;
            public Collections.IAddress<byte> SpellLearned;
            public Collections.IAddress<byte> DAT;
            public Collections.IAddress<byte> DMAT;
            public Collections.IAddress<byte> DDF;
            public Collections.IAddress<byte> DMDF;

            internal StatTable(int baseAddress) {
                MaxMP = Collections.Factory.Create<ushort>(baseAddress, 8, 6);
                DAT = Collections.Factory.Create<byte>(baseAddress + 0x2, 8, 6);
                DAT = Collections.Factory.Create<byte>(baseAddress + 0x4, 8, 6);
                DMAT = Collections.Factory.Create<byte>(baseAddress + 0x5, 8, 6);
                DDF = Collections.Factory.Create<byte>(baseAddress + 0x6, 8, 6);
                DMDF = Collections.Factory.Create<byte>(baseAddress + 0x7, 8, 6);
            }
        }
    }
}
