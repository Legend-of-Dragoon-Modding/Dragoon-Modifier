using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Battle {
    public class Battle {
        int _cPoint;
        int _mPoint;
        int _battleOffset;
        ushort[] _monsterIDs;
        ushort[] _uniqueMonsterIDs;
        MemoryController.MonsterAddress[] _monsterTable;
        MemoryController.CharacterAddress[] _characterTable;

        public int CharacterPoint { get { return _cPoint; } }
        public int MonsterPoint { get { return _mPoint; } }
        public ushort[] ID { get { return _monsterIDs; } }

        public Battle(int cPoint, int mPoint, int battleOffset, byte monsterCount, byte uniqueMonsterSize) {
            _cPoint = cPoint;
            _mPoint = mPoint;
            _battleOffset = battleOffset;
            _monsterTable = new MemoryController.MonsterAddress[monsterCount];
            _monsterIDs = new ushort[monsterCount];
            for (int i = 0; i < _monsterTable.Length; i++) {
                _monsterTable[i] = new MemoryController.MonsterAddress(_mPoint, i, i, 0);
                _monsterIDs[i] = _monsterTable[i].ID;
            }
            int partySize = 0;
            for (int i = 0; i < 3; i++) {

                if (Globals.PARTY_SLOT[i] > 8) {
                    break;
                }
                partySize++;
            }
            _characterTable = new MemoryController.CharacterAddress[partySize];
            for (int i = 0; i < _monsterTable.Length; i++) {
                _characterTable[i] = new MemoryController.CharacterAddress(_cPoint, i, i + monsterCount);
            }

        }
    }
}
