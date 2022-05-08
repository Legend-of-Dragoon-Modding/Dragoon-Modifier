using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Core.Memory {
    public class CharacterStatTable {
        private static readonly byte[] _reorderCharacter = new byte[] { 1, 4, 6, 5, 2, 4, 3, 0, 6 };

        private readonly StatTable[] _statTable = new StatTable[7];

        internal CharacterStatTable() {
            var statTableAddr = Emulator.GetAddress("CHAR_STAT_TABLE");
            var expTableAddr = Emulator.GetAddress("CHAR_LVL_TABLE");

            for (int i = 0; i < _statTable.Length; i++) {
                _statTable[i] = new StatTable(statTableAddr + i * 0x1E8, expTableAddr + i * 0xF4);
            }
        }

        public StatTable this[int i] {
            get {
                return _statTable[_reorderCharacter[i]];
            }
        }


        public class StatTable {
            public Collections.IAddress<ushort> MaxHP;
            public Collections.IAddress<byte> SPD;
            public Collections.IAddress<byte> AT;
            public Collections.IAddress<byte> MAT;
            public Collections.IAddress<byte> DF;
            public Collections.IAddress<byte> MDF;
            public Collections.IAddress<uint> NextLvlExp;

            internal StatTable(int statTableAddr, int expTableAddr) {
                MaxHP = Collections.Factory.Create<ushort>(statTableAddr, 8, 61);
                SPD = Collections.Factory.Create<byte>(statTableAddr + 0x3, 8, 61);
                AT = Collections.Factory.Create<byte>(statTableAddr + 0x4, 8, 61);
                MAT = Collections.Factory.Create<byte>(statTableAddr + 0x5, 8, 61);
                DF = Collections.Factory.Create<byte>(statTableAddr + 0x6, 8, 61);
                MDF = Collections.Factory.Create<byte>(statTableAddr + 0x7, 8, 61);
                NextLvlExp = Collections.Factory.Create<uint>(expTableAddr, 4, 61);
            }


        }

    }
}
