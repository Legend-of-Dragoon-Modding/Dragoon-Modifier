using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Core.Memory {
    public sealed class DragoonStatTable {
        private static readonly byte[] _reorderCharacter = new byte[] { 2, 6, 8, 7, 3, 0, 4, 1, 5 };
        private static readonly byte[] _reorderCharacterLvl = new byte[] { 1, 4, 6, 5, 2, 4, 3, 0, 6 };
        private readonly StatTable[] _statTable = new StatTable[9];

        public StatTable this[int i] {
            get {
                return _statTable[i];
            }
        }

        internal DragoonStatTable() {
            var dragoonStatTableAddr = Emulator.GetAddress("DRAGOON_STAT_TABLE");
            var dragoonExpTableAddr = Emulator.GetAddress("DRAGOON_SP_TABLE");
            for (int i = 0; i < _statTable.Length; i++) {
                _statTable[i] = new StatTable(dragoonStatTableAddr + _reorderCharacter[i] * 0x30, dragoonExpTableAddr + _reorderCharacterLvl[i] * 0xC);
            }
        }

        public sealed class StatTable {
            public Collections.IAddress<ushort> MaxMP;
            public Collections.IAddress<byte> SpellLearned;
            public Collections.IAddress<byte> DAT;
            public Collections.IAddress<byte> DMAT;
            public Collections.IAddress<byte> DDF;
            public Collections.IAddress<byte> DMDF;
            public Collections.IAddress<ushort> NextLvlExp;

            internal StatTable(int dragoonStatTableAddr, int dragoonExpTableAddr) {
                MaxMP = Collections.Factory.Create<ushort>(dragoonStatTableAddr, 8, 6);
                DAT = Collections.Factory.Create<byte>(dragoonStatTableAddr + 0x2, 8, 6);
                DAT = Collections.Factory.Create<byte>(dragoonStatTableAddr + 0x4, 8, 6);
                DMAT = Collections.Factory.Create<byte>(dragoonStatTableAddr + 0x5, 8, 6);
                DDF = Collections.Factory.Create<byte>(dragoonStatTableAddr + 0x6, 8, 6);
                DMDF = Collections.Factory.Create<byte>(dragoonStatTableAddr + 0x7, 8, 6);
                NextLvlExp = Collections.Factory.Create<ushort>(dragoonExpTableAddr, 2, 6);
            }
        }
    }
}
