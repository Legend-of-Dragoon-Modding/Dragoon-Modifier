using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Emulator.Memory.Battle {
    public class Controller : IBattle {
        private readonly IEmulator _emulator;
        public uint CharacterPoint { get; private set; }
        public uint MonsterPoint { get; private set; }
        public ushort EncounterID { get; private set; }
        public ushort[] MonsterID { get; private set; }
        public ushort[] UniqueMonsterID { get; private set; }
        public Monster[] MonsterTable { get; private set; }
        public Character[] CharacterTable { get; private set; }
        public int BattleOffset { get; private set; }
        public ushort BattleMenuCount { get { return _emulator.ReadUShort(MonsterPoint + 0xE3A); } set { _emulator.WriteUShort(MonsterPoint + 0xE3A, value); } }
        public byte BattleMenuChosenSlot { get { return _emulator.ReadByte(MonsterPoint + 0xE4E); } set { _emulator.WriteByte(MonsterPoint + 0xE4E, value); } }
        public Collections.IAddress<byte> BattleMenuSlot { get; private set; }

        internal Controller(IEmulator emulator) {
            _emulator = emulator;
            CharacterPoint = _emulator.Memory.CharacterPoint;
            MonsterPoint = _emulator.Memory.MonsterPoint;
            BattleOffset = GetOffset();
            EncounterID = _emulator.Memory.EncounterID;
            var monsterCount = _emulator.Memory.MonsterSize;
            var uniqueMonsterSize = _emulator.Memory.UniqueMonsterSize;
            UniqueMonsterID = new ushort[uniqueMonsterSize];
            for (int i = 0; i < UniqueMonsterID.Length; i++) {
                // TODO
            }
            MonsterTable = new Monster[monsterCount];
            MonsterID = new ushort[monsterCount];
            for (int i = 0; i < MonsterTable.Length; i++) {
                MonsterTable[i] = new Monster(_emulator, MonsterPoint, i, i, 0);
                MonsterID[i] = MonsterTable[i].ID;
            }
            int partySize = 0;
            for (int i = 0; i < 3; i++) {
                if (_emulator.Memory.PartySlot[i] > 8) {
                    break;
                }
                partySize++;
            }
            CharacterTable = new Character[partySize];
            for (int i = 0; i < CharacterTable.Length; i++) {
                CharacterTable[i] = new Character(_emulator, CharacterPoint, i, i + monsterCount);
            }
            BattleMenuSlot = Factory.AddressCollection<byte>(emulator, (int) MonsterPoint + 0xE3C, 2, 9);
        }

        private int GetOffset() {
            if (_emulator.Region == Region.NTA || _emulator.Region == Region.ENG) {
                return _emulator.ReadUShort("BATTLE_OFFSET") - 0x8F44;
            } else {
                int[] discOffset = { 0xD80, 0x0, 0x1458, 0x1B0 };
                int[] charOffset = { 0x0, 0x180, -0x180, 0x420, 0x540, 0x180, 0x350, 0x2F0, -0x180 };
                int[] duoCharOffset = { 0x0, -0x180, 0x180, -0x420, -0x180, -0x540, -0x350, -0x350, -0x2F0 };
                int[] duoDartOffset = { 0x0, 0x470, 0x170, 0x710, 0x830, 0x470, 0x640, 0x640, 0x170 };
                int partyOffset = 0;

                if (_emulator.Memory.PartySlot[2] == 0xFFFFFFFF && _emulator.Memory.PartySlot[1] < 9) {
                    if (_emulator.Memory.PartySlot[0] == 0) {
                        partyOffset = duoDartOffset[_emulator.Memory.PartySlot[1]];
                    } else if (_emulator.Memory.PartySlot[1] == 0) {
                        partyOffset = duoDartOffset[_emulator.Memory.PartySlot[0]];
                    } else {
                        partyOffset = charOffset[_emulator.Memory.PartySlot[0]] + charOffset[_emulator.Memory.PartySlot[1]];
                    }
                } else {
                    if (_emulator.Memory.PartySlot[0] < 9 && _emulator.Memory.PartySlot[1] < 9 && _emulator.Memory.PartySlot[2] < 9) {
                        partyOffset = charOffset[_emulator.Memory.PartySlot[1]] + charOffset[_emulator.Memory.PartySlot[2]];
                    }
                }

                return discOffset[_emulator.Memory.Disc - 1] - partyOffset;
            }
        }
    }
}
