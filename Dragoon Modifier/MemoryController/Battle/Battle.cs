using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Battle {
    public class Battle {
        public uint CharacterPoint { get; private set; }
        public uint MonsterPoint { get; private set; }
        public ushort EncounterID { get; private set; }
        public ushort[] MonsterID { get; private set; }
        public ushort[] UniqueMonsterID { get; private set; }
        public MemoryController.MonsterAddress[] MonsterTable { get; private set; }
        public MemoryController.CharacterAddress[] CharacterTable { get; private set; }
        public int BattleOffset { get; private set; }
        public ushort BattleMenuCount { get { return Emulator.ReadUShort(MonsterPoint + 0xE3A); } set { Emulator.WriteUShort(MonsterPoint + 0xE3A, value); } }
        public byte BattleMenuChosenSlot { get { return Emulator.ReadByte(MonsterPoint + 0xE4E); } set { Emulator.WriteByte(MonsterPoint + 0xE4E, value); } }
        public MemoryController.ByteCollection BattleMenuSlot { get; private set; }

        public Battle() {
            CharacterPoint = Emulator.MemoryController.CharacterPoint;
            MonsterPoint = Emulator.MemoryController.MonsterPoint;
            BattleOffset = GetOffset();
            EncounterID = Emulator.MemoryController.EncounterID;
            var monsterCount = Emulator.MemoryController.MonsterSize;
            var uniqueMonsterSize = Emulator.MemoryController.UniqueMonsterSize;
            UniqueMonsterID = new ushort[uniqueMonsterSize];
            for (int i = 0; i < UniqueMonsterID.Length; i++) {
                // TODO
            }
            MonsterTable = new MemoryController.MonsterAddress[monsterCount];
            MonsterID = new ushort[monsterCount];
            for (int i = 0; i < MonsterTable.Length; i++) {
                MonsterTable[i] = new MemoryController.MonsterAddress(MonsterPoint, i, i, 0);
                MonsterID[i] = MonsterTable[i].ID;
            }
            int partySize = 0;
            for (int i = 0; i < 3; i++) {
                if (Emulator.MemoryController.PartySlot[i] > 8) {
                    break;
                }
                partySize++;
            }
            CharacterTable = new MemoryController.CharacterAddress[partySize];
            for (int i = 0; i < CharacterTable.Length; i++) {
                CharacterTable[i] = new MemoryController.CharacterAddress(CharacterPoint, i, i + monsterCount);
            }
            BattleMenuSlot = new MemoryController.ByteCollection((int) MonsterPoint + 0xE3C, 2, 9);

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
