﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Emulator.Memory.Battle {
    public class Controller : IBattle {
        private readonly int[] _discOffset = { 0xD80, 0x0, 0x1458, 0x1B0 };
        private readonly int[] _charOffset = { 0x0, 0x180, -0x180, 0x420, 0x540, 0x180, 0x350, 0x2F0, -0x180 };
        private readonly int[] _duoCharOffset = { 0x0, -0x180, 0x180, -0x420, -0x180, -0x540, -0x350, -0x350, -0x2F0 };
        private readonly int[] _duoDartOffset = { 0x0, 0x470, 0x170, 0x710, 0x830, 0x470, 0x640, 0x640, 0x170 };

        private readonly IEmulator _emulator;
        private readonly int _damageCap;
        private readonly int _haschelFix;
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
        public ushort BattleMenuCount { get { return _emulator.ReadUShort(MonsterPoint + 0xE3A); } set { _emulator.WriteUShort(MonsterPoint + 0xE3A, value); } }
        public byte BattleMenuChosenSlot { get { return _emulator.ReadByte(MonsterPoint + 0xE4E); } set { _emulator.WriteByte(MonsterPoint + 0xE4E, value); } }
        public byte ItemUsed { get { return _emulator.ReadByte(MonsterPoint + 0xBC4); } set { _emulator.WriteByte(MonsterPoint + 0xBC4, value); } }
        public Collections.IAddress<byte> BattleMenuSlot { get; private set; }
        public ushort DamageCap { get { return GetDamageCap(); } set { SetDamageCap(value); } }
        public byte[] HaschelFix { get { return _emulator.ReadAoB(_haschelFix + _discOffset[_emulator.Memory.Disc - 1], _haschelFix + _discOffset[_emulator.Memory.Disc - 1] + 116); } set { _emulator.WriteAoB(_haschelFix + _discOffset[_emulator.Memory.Disc - 1], value); } }

        internal Controller(IEmulator emulator) {
            _emulator = emulator;
            CharacterPoint = _emulator.Memory.CharacterPoint;
            MonsterPoint = _emulator.Memory.MonsterPoint;
            BattleOffset = GetOffset();
            EncounterID = _emulator.Memory.EncounterID;
            var monsterCount = _emulator.Memory.MonsterSize;
            var uniqueMonsterSize = _emulator.Memory.UniqueMonsterSize;
            UniqueMonsterID = Factory.AddressCollection<ushort>(_emulator, _emulator.ReadUShort("UNIQUE_SLOT"), 0x1A8, uniqueMonsterSize);
            var rewardsAddress = _emulator.GetAddress("MONSTER_REWARDS");
            RewardsExp = Factory.AddressCollection<ushort>(_emulator, rewardsAddress, 0x1A8, uniqueMonsterSize);
            RewardsGold = Factory.AddressCollection<ushort>(_emulator, rewardsAddress + 2, 0x1A8, uniqueMonsterSize);
            RewardsDropChance = Factory.AddressCollection<byte>(_emulator, rewardsAddress + 4, 0x1A8, uniqueMonsterSize);
            RewardsItemDrop = Factory.AddressCollection<byte>(_emulator, rewardsAddress + 5, 0x1A8, uniqueMonsterSize);
            MonsterTable = new Monster[monsterCount];
            MonsterID = new ushort[monsterCount];
            for (int i = 0; i < MonsterTable.Length; i++) {
                MonsterTable[i] = new Monster(_emulator, MonsterPoint, i, i, BattleOffset);
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
                CharacterTable[i] = new Character(_emulator, CharacterPoint, i, i + monsterCount, BattleOffset);
            }
            BattleMenuSlot = Factory.AddressCollection<byte>(emulator, (int) MonsterPoint + 0xE3C, 2, 9);

            _damageCap = emulator.GetAddress("DAMAGE_CAP");
            _haschelFix = emulator.GetAddress("HASCHEL_FIX");
        }

        private int GetOffset() {
            if (_emulator.Region == Region.NTA || _emulator.Region == Region.ENG) {
                return _emulator.ReadUShort("BATTLE_OFFSET") - 0x8F44;
            }

            int partyOffset = 0;

            if (_emulator.Memory.PartySlot[2] == 0xFFFFFFFF && _emulator.Memory.PartySlot[1] < 9) {
                if (_emulator.Memory.PartySlot[0] == 0) {
                    partyOffset = _duoDartOffset[_emulator.Memory.PartySlot[1]];
                } else if (_emulator.Memory.PartySlot[1] == 0) {
                    partyOffset = _duoDartOffset[_emulator.Memory.PartySlot[0]];
                } else {
                    partyOffset = _charOffset[_emulator.Memory.PartySlot[0]] + _charOffset[_emulator.Memory.PartySlot[1]];
                }
            } else {
                if (_emulator.Memory.PartySlot[0] < 9 && _emulator.Memory.PartySlot[1] < 9 && _emulator.Memory.PartySlot[2] < 9) {
                    partyOffset = _charOffset[_emulator.Memory.PartySlot[1]] + _charOffset[_emulator.Memory.PartySlot[2]];
                }
            }

            return _discOffset[_emulator.Memory.Disc - 1] - partyOffset;
        }

        private ushort GetDamageCap() {
            return new ushort[] { _emulator.ReadUShort(_damageCap), _emulator.ReadUShort(_damageCap + 0x8), _emulator.ReadUShort(_damageCap + 0x14) }.Min();
        }

        private void SetDamageCap(ushort cap) {
            if (cap > 50000) {
                Console.WriteLine("[ERROR] It's over 50000! Setting damage cap to 50k.");
                cap = 50000;
            }

            _emulator.WriteUShort(_damageCap, cap);
            _emulator.WriteUShort(_damageCap + 0x8, cap);
            _emulator.WriteUShort(_damageCap + 0x14, cap);
        }
    }
}
