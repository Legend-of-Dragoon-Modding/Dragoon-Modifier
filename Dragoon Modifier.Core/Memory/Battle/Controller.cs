using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Core.Memory.Battle {
    public class Controller : IController {
        private readonly int[] _discOffset = { 0xD80, 0x0, 0x1458, 0x1B0 };
        private readonly int[] _charOffset = { 0x0, 0x180, -0x180, 0x420, 0x540, 0x180, 0x350, 0x2F0, -0x180 };
        private readonly int[] _duoCharOffset = { 0x0, -0x180, 0x180, -0x420, -0x180, -0x540, -0x350, -0x350, -0x2F0 };
        private readonly int[] _duoDartOffset = { 0x0, 0x470, 0x170, 0x710, 0x830, 0x470, 0x640, 0x640, 0x170 };

        private readonly int _damageCap;
        private readonly int _haschelFix;
        private readonly int _dragoonSpecial;
        private readonly int _dragonBlockStaff;
        public uint CharacterPoint { get; private set; }
        public uint MonsterPoint { get; private set; }
        public ushort EncounterID { get; private set; }
        public ushort[] MonsterID { get; private set; }
        public Collections.IAddress<ushort> UniqueMonsterID { get; private set; }
        public Collections.IAddress<ushort> RewardsExp { get; private set; }
        public Collections.IAddress<ushort> RewardsGold { get; private set; }
        public Collections.IAddress<byte> RewardsItemDrop { get; private set; }
        public Collections.IAddress<byte> RewardsDropChance { get; private set; }
        public Monster[] MonsterTable { get; private set; }
        public Character[] CharacterTable { get; private set; }
        public int BattleOffset { get; private set; }
        public ushort BattleMenuCount { get { return Emulator.DirectAccess.ReadUShort(MonsterPoint + 0xE3A); } set { Emulator.DirectAccess.WriteUShort(MonsterPoint + 0xE3A, value); } }
        public byte BattleMenuChosenSlot { get { return Emulator.DirectAccess.ReadByte(MonsterPoint + 0xE4E); } set { Emulator.DirectAccess.WriteByte(MonsterPoint + 0xE4E, value); } }
        public byte ItemUsed { get { return Emulator.DirectAccess.ReadByte(MonsterPoint + 0xBC4); } set { Emulator.DirectAccess.WriteByte(MonsterPoint + 0xBC4, value); } }
        public Collections.IAddress<byte> BattleMenuSlot { get; private set; }
        public ushort DamageCap { get { return GetDamageCap(); } set { SetDamageCap(value); } }
        public byte[] HaschelFix { get { return Emulator.DirectAccess.ReadAoB(_haschelFix + _discOffset[Emulator.Memory.Disc - 1], _haschelFix + _discOffset[Emulator.Memory.Disc - 1] + 116); } set { Emulator.DirectAccess.WriteAoB(_haschelFix + _discOffset[Emulator.Memory.Disc - 1], value); } }
        public byte DragoonSpecial { get { return Emulator.DirectAccess.ReadByte(_dragoonSpecial); } set { Emulator.DirectAccess.WriteByte(_dragoonSpecial, value); } }
        public uint DragoonAdditionTable { get; private set; }
        public byte IconCount { get { return Emulator.DirectAccess.ReadByte(MonsterPoint + 0xE3A); } }
        public byte IconSelected { get { return Emulator.DirectAccess.ReadByte(MonsterPoint + 0xE4E); } }
        public byte DragonBlockStaff { get { return Emulator.DirectAccess.ReadByte(_dragonBlockStaff); } }
        public uint BattleUIColour { get; private set; }

        internal Controller() {
            CharacterPoint = Emulator.Memory.CharacterPoint;
            MonsterPoint = Emulator.Memory.MonsterPoint;
            BattleOffset = GetOffset();
            EncounterID = Emulator.Memory.EncounterID;
            var monsterCount = Emulator.Memory.MonsterSize;
            var uniqueMonsterSize = Emulator.Memory.UniqueMonsterSize;
            UniqueMonsterID = Collections.Factory.Create<ushort>(Emulator.GetAddress("UNIQUE_SLOT"), 0x1A8, uniqueMonsterSize);
            var rewardsAddress = Emulator.GetAddress("MONSTER_REWARDS");
            RewardsExp = Collections.Factory.Create<ushort>(rewardsAddress, 0x1A8, uniqueMonsterSize);
            RewardsGold = Collections.Factory.Create<ushort>(rewardsAddress + 2, 0x1A8, uniqueMonsterSize);
            RewardsDropChance = Collections.Factory.Create<byte>(rewardsAddress + 4, 0x1A8, uniqueMonsterSize);
            RewardsItemDrop = Collections.Factory.Create<byte>(rewardsAddress + 5, 0x1A8, uniqueMonsterSize);
            MonsterTable = new Monster[monsterCount];
            MonsterID = new ushort[monsterCount];
            for (int i = 0; i < MonsterTable.Length; i++) {
                MonsterTable[i] = new Monster(MonsterPoint, i, i, BattleOffset);
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
                CharacterTable[i] = new Character(CharacterPoint, i, i + monsterCount, BattleOffset);
            }
            BattleMenuSlot = Collections.Factory.Create<byte>((int) MonsterPoint + 0xE3C, 2, 9);

            _damageCap = Emulator.GetAddress("DAMAGE_CAP");
            _haschelFix = Emulator.GetAddress("HASCHEL_FIX");
            _dragoonSpecial = Emulator.GetAddress("DRAGOON_SPECIAL");
            DragoonAdditionTable = (uint) (Emulator.GetAddress("ADDITION") + GetOffset() + 0x300);
            BattleUIColour = (uint) Emulator.GetAddress("BATTLE_UI_COLOUR");
            _dragonBlockStaff = Emulator.GetAddress("DRAGON_BLOCK_STAFF");
        }

        private int GetOffset() {
            if (Emulator.Region == Region.NTA || Emulator.Region == Region.ENG) {
                return Emulator.DirectAccess.ReadUShort("BATTLE_OFFSET") - 0x8F44;
            }

            int partyOffset = 0;

            if (Emulator.Memory.PartySlot[2] == 0xFFFFFFFF && Emulator.Memory.PartySlot[1] < 9) {
                if (Emulator.Memory.PartySlot[0] == 0) {
                    partyOffset = _duoDartOffset[Emulator.Memory.PartySlot[1]];
                } else if (Emulator.Memory.PartySlot[1] == 0) {
                    partyOffset = _duoDartOffset[Emulator.Memory.PartySlot[0]];
                } else {
                    partyOffset = _charOffset[Emulator.Memory.PartySlot[0]] + _charOffset[Emulator.Memory.PartySlot[1]];
                }
            } else {
                if (Emulator.Memory.PartySlot[0] < 9 && Emulator.Memory.PartySlot[1] < 9 && Emulator.Memory.PartySlot[2] < 9) {
                    partyOffset = _charOffset[Emulator.Memory.PartySlot[1]] + _charOffset[Emulator.Memory.PartySlot[2]];
                }
            }

            return _discOffset[Emulator.Memory.Disc - 1] - partyOffset;
        }

        private ushort GetDamageCap() {
            return new ushort[] { Emulator.DirectAccess.ReadUShort(_damageCap), Emulator.DirectAccess.ReadUShort(_damageCap + 0x8), Emulator.DirectAccess.ReadUShort(_damageCap + 0x14) }.Min();
        }

        private void SetDamageCap(ushort cap) {
            if (cap > 50000) {
                Console.WriteLine("[ERROR] It's over 50000! Setting damage cap to 50k.");
                cap = 50000;
            }

            Emulator.DirectAccess.WriteUInt(_damageCap, cap);
            Emulator.DirectAccess.WriteUInt(_damageCap + 0x8, cap);
            Emulator.DirectAccess.WriteUInt(_damageCap + 0x14, cap);
        }
    }
}
