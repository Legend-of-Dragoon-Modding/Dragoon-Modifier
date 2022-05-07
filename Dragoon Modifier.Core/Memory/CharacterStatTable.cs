using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Core.Memory {
    public class CharacterStatTable {
        private static readonly byte[] _reorderCharacter = new byte[] { 1, 4, 6, 5, 2, 4, 3, 0, 6 };

        private readonly StatTable[] _statTable = new StatTable[7];

        internal CharacterStatTable(int baseAddress) {
            for (int i = 0; i < _statTable.Length; i++) {
                _statTable[i] = new StatTable(baseAddress + i * 0x1E8);
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

            internal StatTable(int baseAddress) {
                MaxHP = Collections.Factory.Create<ushort>(baseAddress, 8, 61);
                SPD = Collections.Factory.Create<byte>(baseAddress + 0x3, 8, 61);
                AT = Collections.Factory.Create<byte>(baseAddress + 0x4, 8, 61);
                MAT = Collections.Factory.Create<byte>(baseAddress + 0x5, 8, 61);
                DF = Collections.Factory.Create<byte>(baseAddress + 0x6, 8, 61);
                MDF = Collections.Factory.Create<byte>(baseAddress + 0x7, 8, 61);
            }


        }

    }
}
