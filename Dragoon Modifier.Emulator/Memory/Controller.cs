using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Emulator.Memory {
    internal class Controller : IMemory {
        private readonly IEmulator _emulator;

        private readonly int _disc;
        private readonly int _chapter;
        private readonly int _mapId;
        private readonly int _overworldContinent;
        private readonly int _overworldSegment;
        private readonly int _overworldCheck;
        private readonly int _dragoonSpirits;
        private readonly int _hotkey;
        private readonly int _battleValue;
        private readonly int _menu;
        private readonly int _transition;
        private readonly int _gold;
        private readonly int _menuUnlock;
        private readonly int _shopID;
        private readonly int _savePoint;
        private readonly int _textSpeed;
        private readonly int _autoText;
        private readonly int _basePoint;
        private readonly int _encounterID;
        private readonly int _monsterSize;
        private readonly int _uniqueMonsterSize;
        private readonly int _discChangeCheck;
        private readonly int _fieldHPCap1;
        private readonly int _fieldHPCap2;
        private readonly int _fieldHPCap3;
        private readonly int _fieldHPCap4;

        public Collections.IAddress<uint> PartySlot { get; private set; }
        public byte Disc { get { return _emulator.ReadByte(_disc); } }
        public byte Chapter { get { return (byte) (_emulator.ReadByte(_chapter) + 1); } }
        public ushort MapID { get { return _emulator.ReadUShort(_mapId); } set { _emulator.WriteUShort(_mapId, value); } }
        public byte OverworldContinent { get { return _emulator.ReadByte(_overworldContinent); } set { _emulator.WriteByte(_overworldContinent, value); } }
        public byte OverworldSegment { get { return _emulator.ReadByte(_overworldSegment); } set { _emulator.WriteByte(_overworldSegment, value); } }
        public byte OverworldCheck { get { return _emulator.ReadByte(_overworldCheck); } set { _emulator.WriteByte(_overworldCheck, value); } }
        public byte DragoonSpirits { get { return _emulator.ReadByte(_dragoonSpirits); } set { _emulator.WriteByte(_dragoonSpirits, value); } }
        public ushort Hotkey { get { return _emulator.ReadUShort(_hotkey); } set { _emulator.WriteUShort(_hotkey, value); } } // Should writing here be allowed?
        public ushort BattleValue { get { return _emulator.ReadUShort(_battleValue); } set { _emulator.WriteUShort(_battleValue, value); } }
        public Collections.IAddress<byte> EquipmentInventory { get; private set; }
        public Collections.IAddress<byte> ItemInventory { get; private set; }
        public byte Menu { get { return _emulator.ReadByte(_menu); } set { _emulator.WriteByte(_menu, value); } }
        public byte Transition { get { return _emulator.ReadByte(_transition); } set { _emulator.WriteByte(_transition, value); } }
        public uint Gold { get { return _emulator.ReadUInt(_gold); } set { _emulator.WriteUInt(_gold, value); } }
        public byte MenuUnlock { get { return _emulator.ReadByte(_menuUnlock); } set { _emulator.WriteByte(_menuUnlock, value); } }
        public CharacterTable[] CharacterTable { get; private set; }
        public SecondaryCharacterTable[] SecondaryCharacterTable { get; private set; }
        public Shop[] Shop { get; private set; } = new Shop[45]; // Most likely up to 64 shops. But most of it is unused, so I chose a safe number
        public CurrentShop CurrentShop { get; private set; }
        public byte ShopID { get { return _emulator.ReadByte(_shopID); } set { _emulator.WriteByte(_shopID, value); } }
        public IItem[] Item { get; private set; } = new IItem[256];
        public byte SavePoint { get { return _emulator.ReadByte(_savePoint); } set { _emulator.WriteByte(_savePoint, value); } }
        public ushort TextSpeed { get { return _emulator.ReadUShort(_textSpeed); } set { _emulator.WriteUShort(_textSpeed, value); } }
        public ushort AutoText { get { return _emulator.ReadUShort(_autoText); } set { _emulator.WriteUShort(_autoText, value); } }
        // public CharacterStatTable[] CharacterStatTable { get; private set; }
        public AdditionTable[] MenuAdditionTable { get; private set; }
        public uint BattleBasePoint { get { return _emulator.ReadUInt24(_basePoint); } }
        public uint CharacterPoint { get { return _emulator.ReadUInt24(_basePoint + 0x18 ); } }
        public uint MonsterPoint { get { return _emulator.ReadUInt24(_basePoint + 0x2C); } }
        public ushort EncounterID { get { return _emulator.ReadUShort(_encounterID); } set { _emulator.WriteUShort(_encounterID, value); } }
        public byte MonsterSize { get { return _emulator.ReadByte(_monsterSize); } }
        public byte UniqueMonsterSize { get { return _emulator.ReadByte(_uniqueMonsterSize); } }
        public GameState GameState { get { return GetGameState(); } }
        public byte DiscGameCheck { get { return _emulator.ReadByte(_discChangeCheck); } }
        public ushort FieldHPCap { get { return _emulator.ReadUShort(_fieldHPCap1); } set { SetFieldHPCap(value); } }


        internal Controller(IEmulator emulator) {
            _emulator = emulator;
            PartySlot = Factory.AddressCollection<uint>(_emulator, _emulator.GetAddress("PARTY_SLOT"), 4, 3);
            _disc = _emulator.GetAddress("DISC");
            _chapter = _emulator.GetAddress("CHAPTER");
            _mapId = _emulator.GetAddress("MAP");
            _overworldContinent = _emulator.GetAddress("OVERWORLD_CONTINENT");
            _overworldSegment = _emulator.GetAddress("OVERWORLD_SEGMENT");
            _overworldCheck = _emulator.GetAddress("OVERWORLD_CHECK");
            _dragoonSpirits = _emulator.GetAddress("DRAGOON_SPIRITS");
            _hotkey = _emulator.GetAddress("HOTKEY");
            _battleValue = _emulator.GetAddress("BATTLE_VALUE");
            EquipmentInventory = Factory.AddressCollection<Byte>(_emulator, _emulator.GetAddress("ARMOR_INVENTORY"), 1, 256);
            ItemInventory = Factory.AddressCollection<byte>(_emulator, _emulator.GetAddress("INVENTORY"), 1, 64);
            _menu = _emulator.GetAddress("MENU");
            _transition = _emulator.GetAddress("TRANSITION");
            _gold = _emulator.GetAddress("GOLD");
            _menuUnlock = _emulator.GetAddress("MENU_UNLOCK");
            var charTableAddr = _emulator.GetAddress("CHAR_TABLE");
            var secondCharTableAddr = _emulator.GetAddress("SECONDARY_CHARACTER_TABLE");
            CharacterTable = new CharacterTable[9];
            SecondaryCharacterTable = new SecondaryCharacterTable[9];
            for (int i = 0; i < CharacterTable.Length; i++) {
                CharacterTable[i] = new CharacterTable(_emulator, charTableAddr, i);
                SecondaryCharacterTable[i] = new SecondaryCharacterTable(_emulator, secondCharTableAddr, i);
            }
            var shopListAddr = _emulator.GetAddress("SHOP_LIST");
            for (int i = 0; i < Shop.Length; i++) {
                Shop[i] = new Shop(_emulator, shopListAddr, i);
            }
            CurrentShop = new CurrentShop(_emulator, _emulator.GetAddress("SHOP_CONTENT"));
            _shopID = _emulator.GetAddress("SHOP_ID");
            _savePoint = _emulator.GetAddress("SAVE_POINT");
            _textSpeed = _emulator.GetAddress("TEXT_SPEED");
            _autoText = _emulator.GetAddress("AUTO_TEXT");
            var equipTableAddr = _emulator.GetAddress("ITEM_TABLE");
            var itemTableAddr = _emulator.GetAddress("THROWN_ITEM_TABLE");
            var itemNamePtr = _emulator.GetAddress("ITEM_NAME_PTR");
            var itemDescPtr = _emulator.GetAddress("ITEM_DESC_PTR");
            var itemBattleNamePtr = _emulator.GetAddress("ITEM_BTL_NAME_PTR");
            var itemBattleDescPtr = _emulator.GetAddress("ITEM_BTL_DESC_PTR");
            var itemSellPriceAddr = _emulator.GetAddress("SHOP_PRICE");
            for (int i = 0; i < 192; i++) {
                Item[i] = Factory.Equipment(_emulator, equipTableAddr, itemNamePtr, itemDescPtr, itemSellPriceAddr, i);
            }
            for (int i = 192; i < 256; i++) {
                Item[i] = Factory.UsableItem(_emulator, itemTableAddr, itemNamePtr, itemDescPtr, itemBattleNamePtr, itemBattleDescPtr, itemSellPriceAddr, i);
            }

            /*
            var charStatTableAddr = Emulator.GetAddress("CHAR_STAT_TABLE");
            for (int i = 0; i < _charStatTable.Length; i++) {
                _charStatTable[i] = new CharacterStatTable(charStatTableAddr, i);
            }
            var dragoonStatTableAddr = Emulator.GetAddress("DRAGOON_TABLE");
            for (int i = 0; i < _dragoonStatTable.Length; i++) {
                _dragoonStatTable[i] = new DragoonStatTable(dragoonStatTableAddr, i);
            }
            */
            var addTableAddr = _emulator.GetAddress("MENU_ADDITION_TABLE_FLAT");
            var addMultiAddr = _emulator.GetAddress("MENU_ADDITION_TABLE_MULTI");
            MenuAdditionTable = new AdditionTable[41];
            for (int i = 0; i < MenuAdditionTable.Length; i++) {
                MenuAdditionTable[i] = new AdditionTable(_emulator, addTableAddr, addMultiAddr, i);
            }
            _basePoint = _emulator.GetAddress("BATTLE_BASE_POINT");
            _encounterID = _emulator.GetAddress("ENCOUNTER_ID");
            _monsterSize = _emulator.GetAddress("MONSTER_SIZE");
            _uniqueMonsterSize = _emulator.GetAddress("UNIQUE_MONSTER_SIZE");
            var encounterMapAddr = _emulator.GetAddress("ENCOUNTER_MAP");
            var encounterTableAddr = _emulator.GetAddress("ENCOUNTER_TABLE");
            _discChangeCheck = _emulator.GetAddress("DISC_CHANGE_CHECK");
            _fieldHPCap1 = _emulator.GetAddress("FIELD_HP_CAP_1");
            _fieldHPCap2 = _emulator.GetAddress("FIELD_HP_CAP_2");
            _fieldHPCap3 = _emulator.GetAddress("FIELD_HP_CAP_3");
            _fieldHPCap4 = _emulator.GetAddress("FIELD_HP_CAP_4");

        }

        private GameState GetGameState() {
            switch (Menu) {
                case 0:
                    if (BattleValue == 41215) {
                        return GameState.Battle;
                    }

                    if (DiscGameCheck == 1) {
                        return GameState.ChangeDisc;
                    }

                    var overworldSegment = OverworldSegment; // 0 on field, or when behind Seles (unaccessible part of overworld map)
                    var overwoldCheck = OverworldCheck; // Added extra check to cover behind Seles and transitions

                    if (overworldSegment == 0 && overwoldCheck == 1) {
                        return GameState.Field;
                    }

                    if (overworldSegment != 0 && overwoldCheck == 3) {
                        return GameState.Overworld;
                    }
                    return GameState.None;
                case 4:
                    return GameState.Menu;
                case 9:
                    return GameState.Shop;
                case 14:
                    return GameState.LoadingScreen;
                case 19:
                    return GameState.EndOfDisc;
                case 24:
                    return GameState.ReplacePrompt;
                case 29:
                    return GameState.BattleResult;
                default:
                    return GameState.None;

            }
        }

        private void SetFieldHPCap(ushort value) {
            _emulator.WriteUShort(_fieldHPCap1, value);
            _emulator.WriteUShort(_fieldHPCap2, value);
            _emulator.WriteUShort(_fieldHPCap3, value);
            _emulator.WriteUShort(_fieldHPCap4, value);
        }
    }
}
