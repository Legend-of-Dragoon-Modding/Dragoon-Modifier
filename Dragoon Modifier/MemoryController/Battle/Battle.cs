using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Battle {
    public class Battle {
        uint _cPoint;
        uint _mPoint;
        int _battleOffset;
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
        public int BattleOffset { get { return _battleOffset; } }

        public Battle() {
            _cPoint = Globals.MemoryController.CharacterPoint;
            _mPoint = Globals.MemoryController.MonsterPoint;
            _battleOffset = GetOffset();
            _encounterID = Globals.MemoryController.EncounterID;
            var monsterCount = Globals.MemoryController.MonsterSize;
            var uniqueMonsterSize = Globals.MemoryController.UniqueMonsterSize;
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
            for (int i = 0; i < _characterTable.Length; i++) {
                _characterTable[i] = new MemoryController.CharacterAddress(_cPoint, i, i + monsterCount);
            }

        }

        private static int GetOffset() {
            if (Constants.REGION == Region.NTA || Constants.REGION == Region.ENG) {
                return Emulator.ReadUShort("BATTLE_OFFSET") - 0x8F44;
            } else {
                int[] discOffset = { 0xD80, 0x0, 0x1458, 0x1B0 };
                int[] charOffset = { 0x0, 0x180, -0x180, 0x420, 0x540, 0x180, 0x350, 0x2F0, -0x180 };
                int[] duoCharOffset = { 0x0, -0x180, 0x180, -0x420, -0x180, -0x540, -0x350, -0x350, -0x2F0 };
                int[] duoDartOffset = { 0x0, 0x470, 0x170, 0x710, 0x830, 0x470, 0x640, 0x640, 0x170 };
                int partyOffset = 0;

                if (Globals.PARTY_SLOT[2] == 255 && Globals.PARTY_SLOT[1] < 9) {
                    if (Globals.PARTY_SLOT[0] == 0) {
                        partyOffset = duoDartOffset[Globals.PARTY_SLOT[1]];
                    } else if (Globals.PARTY_SLOT[1] == 0) {
                        partyOffset = duoDartOffset[Globals.PARTY_SLOT[0]];
                    } else {
                        partyOffset = charOffset[Globals.PARTY_SLOT[0]] + charOffset[Globals.PARTY_SLOT[1]];
                    }
                } else {
                    if (Globals.PARTY_SLOT[0] < 9 && Globals.PARTY_SLOT[1] < 9 && Globals.PARTY_SLOT[2] < 9) {
                        partyOffset = charOffset[Globals.PARTY_SLOT[1]] + charOffset[Globals.PARTY_SLOT[2]];
                    }
                }

                return discOffset[Globals.DISC - 1] - partyOffset;
            }
        }
    }
}
