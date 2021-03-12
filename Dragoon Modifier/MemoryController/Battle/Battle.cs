using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Battle {
    public class Battle {
        uint _cPoint;
        uint _mPoint;
        uint _battleOffset;
        ushort _encounterID;
        ushort[] _monsterIDs;
        ushort[] _uniqueMonsterIDs;
        MemoryController.MonsterAddress[] _monsterTable;
        MemoryController.CharacterAddress[] _characterTable;

        public uint CharacterPoint { get { return _cPoint; } }
        public uint MonsterPoint { get { return _mPoint; } }
        public ushort EncounterID { get { return _encounterID; } }
        public ushort[] MonsterID { get { return _monsterIDs; } }
        public MemoryController.MonsterAddress[] MonsterTable { get { return _monsterTable; } }
        public MemoryController.CharacterAddress[] CharacterTable { get { return _characterTable; } }

        public Battle(uint battleOffset, ushort encounterID, byte monsterCount, byte uniqueMonsterSize) {
            _cPoint = Globals.MemoryController.CharacterPoint;
            _mPoint = Globals.MemoryController.MonsterPoint;
            _battleOffset = battleOffset;
            _encounterID = encounterID;
            _uniqueMonsterIDs = new ushort[uniqueMonsterSize];
            for (int i = 0; i < _uniqueMonsterIDs.Length; i++) {

            }
            _monsterTable = new MemoryController.MonsterAddress[monsterCount];
            _monsterIDs = new ushort[monsterCount];
            for (int i = 0; i < _monsterTable.Length; i++) {
                _monsterTable[i] = new MemoryController.MonsterAddress(_mPoint, i, i, 0);
                _monsterIDs[i] = _monsterTable[i].ID;
            }
            int partySize = 0;
            for (int i = 0; i < 3; i++) {
                if (Globals.MemoryController.PartySlot[i] > 8) {
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
