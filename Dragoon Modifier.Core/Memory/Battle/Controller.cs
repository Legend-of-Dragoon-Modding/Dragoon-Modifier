using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Core.Memory.Battle {
    public class Controller {
        public uint CharacterPoint { get; private set; }
        public uint MonsterPoint { get; private set; }
        public ushort EncounterID { get; private set; }
        public ushort[] MonsterID { get; private set; }
        public ushort[] UniqueMonsterID { get; private set; }
        public Monster[] MonsterTable { get; private set; }
        public Character[] CharacterTable { get; private set; }
        public int BattleOffset { get; private set; }
        public ushort BattleMenuCount { get { return Emulator.ReadUShort(MonsterPoint + 0xE3A); } set { Emulator.WriteUShort(MonsterPoint + 0xE3A, value); } }
        public byte BattleMenuChosenSlot { get { return Emulator.ReadByte(MonsterPoint + 0xE4E); } set { Emulator.WriteByte(MonsterPoint + 0xE4E, value); } }
        public Collections.Byte BattleMenuSlot { get; private set; }

        internal Controller() {
            CharacterPoint = Emulator.Memory.CharacterPoint;
            MonsterPoint = Emulator.Memory.MonsterPoint;
            BattleOffset = GetOffset();
            EncounterID = Emulator.Memory.EncounterID;
            var monsterCount = Emulator.Memory.MonsterSize;
            var uniqueMonsterSize = Emulator.Memory.UniqueMonsterSize;
            UniqueMonsterID = new ushort[uniqueMonsterSize];
            for (int i = 0; i < UniqueMonsterID.Length; i++) {
                // TODO
            }
            MonsterTable = new Monster[monsterCount];
            MonsterID = new ushort[monsterCount];
            for (int i = 0; i < MonsterTable.Length; i++) {
                MonsterTable[i] = new Monster(MonsterPoint, i, i, 0);
                MonsterID[i] = MonsterTable[i].ID;
            }
            int partySize = 0;
            for (int i = 0; i < 3; i++) {
                if (Emulator.Memory.PartySlot[i] > 8) {
                    break;
                }
                partySize++;
            }
            CharacterTable = new Character[partySize];
            for (int i = 0; i < CharacterTable.Length; i++) {
                CharacterTable[i] = new Character(CharacterPoint, i, i + monsterCount);
            }
            BattleMenuSlot = new Collections.Byte((int) MonsterPoint + 0xE3C, 2, 9);
        }

        private static int GetOffset() {
            if (Emulator.Region == Region.NTA || Emulator.Region == Region.ENG) {
                return Emulator.ReadUShort("BATTLE_OFFSET") - 0x8F44;
            } else {
                int[] discOffset = { 0xD80, 0x0, 0x1458, 0x1B0 };
                int[] charOffset = { 0x0, 0x180, -0x180, 0x420, 0x540, 0x180, 0x350, 0x2F0, -0x180 };
                int[] duoCharOffset = { 0x0, -0x180, 0x180, -0x420, -0x180, -0x540, -0x350, -0x350, -0x2F0 };
                int[] duoDartOffset = { 0x0, 0x470, 0x170, 0x710, 0x830, 0x470, 0x640, 0x640, 0x170 };
                int partyOffset = 0;

                if (Emulator.Memory.PartySlot[2] == 0xFFFFFFFF && Emulator.Memory.PartySlot[1] < 9) {
                    if (Emulator.Memory.PartySlot[0] == 0) {
                        partyOffset = duoDartOffset[Emulator.Memory.PartySlot[1]];
                    } else if (Emulator.Memory.PartySlot[1] == 0) {
                        partyOffset = duoDartOffset[Emulator.Memory.PartySlot[0]];
                    } else {
                        partyOffset = charOffset[Emulator.Memory.PartySlot[0]] + charOffset[Emulator.Memory.PartySlot[1]];
                    }
                } else {
                    if (Emulator.Memory.PartySlot[0] < 9 && Emulator.Memory.PartySlot[1] < 9 && Emulator.Memory.PartySlot[2] < 9) {
                        partyOffset = charOffset[Emulator.Memory.PartySlot[1]] + charOffset[Emulator.Memory.PartySlot[2]];
                    }
                }

                return discOffset[Emulator.Memory.Disc - 1] - partyOffset;
            }
        }
    }
}
