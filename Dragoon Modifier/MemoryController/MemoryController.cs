using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.MemoryController {
    public class MemoryController {
        UIntCollection _partySlot;
        int _disc;
        int _chapter;
        int _mapId;
        int _dragoonSpirits;
        int _hotkey;
        int _battleValue;
        ByteCollection _equipInventory;
        ByteCollection _itemInventory;
        int _menu;
        int _transition;
        int _gold;
        int _menuUnlock;
        CharacterTable[] _characterTable = new CharacterTable[9];
        SecondaryCharacterTable[] _secondaryCharacterTable = new SecondaryCharacterTable[9];
        Shop[] _shop = new Shop[45]; // There are most likely up to 64 shops. But most of it isn't used by the base game.
        CurrentShop _currentShop;
        UShortCollection _itemSellPrice;
        int _shopID;
        EquipmentTableEntry[] _equipTable = new EquipmentTableEntry[192];
        ItemTableEntry[] _itemTable = new ItemTableEntry[64]; // Number of items should be verified
        CharacterStatTable[] _charStatTable = new CharacterStatTable[7];
        DragoonStatTable[] _dragoonStatTable = new DragoonStatTable[9];
        AdditionTable[] _addTable = new AdditionTable[41];
        int _basePoint;
        int _encounterID;
        int _monsterSize;
        int _uniqueMonsterSize;

        Emu _emulator;

        public UIntCollection PartySlot { get { return _partySlot; } }
        public byte Disc { get { return _emulator.ReadByte(_disc); } }
        public byte Chapter { get { return (byte) (_emulator.ReadByte(_chapter) + 1); } }
        public ushort MapID { get { return _emulator.ReadUShort(_mapId); } set { _emulator.WriteUShort(_mapId, value); } }
        public byte DragoonSpirits { get { return _emulator.ReadByte(_dragoonSpirits); } set { _emulator.WriteByte(_dragoonSpirits, value); } }
        public ushort Hotkey { get { return _emulator.ReadUShort(_hotkey); } set { Emulator.WriteByte(_hotkey, value); } }
        public ushort BattleValue { get { return _emulator.ReadUShort(_battleValue); } set { _emulator.WriteUShort(_battleValue, value); } }
        public ByteCollection EquipmentInventory { get { return _equipInventory; } }
        public ByteCollection ItemInventory { get { return _itemInventory; } }
        public byte Menu { get { return _emulator.ReadByte(_menu); } set { _emulator.WriteByte(_menu, value); } }
        public byte Transition { get { return _emulator.ReadByte(_transition); } set { _emulator.WriteByte(_transition, value); } }
        public uint Gold { get { return _emulator.ReadUInt(_gold); } set { _emulator.WriteUInt(_gold, value); } }
        public byte MenuUnlock { get { return _emulator.ReadByte(_menuUnlock); } set { _emulator.WriteByte(_menuUnlock, value); } }
        public CharacterTable[] CharacterTable { get { return _characterTable; } }
        public SecondaryCharacterTable[] SecondaryCharacterTable { get { return _secondaryCharacterTable; } }
        public Shop[] Shop { get { return _shop; } }
        public CurrentShop CurrentShop { get { return _currentShop; } }
        public UShortCollection ItemSellPrice { get { return _itemSellPrice; } }
        public byte ShopID { get { return _emulator.ReadByte(_shopID); } set { _emulator.WriteByte(_shopID, value); } }
        public EquipmentTableEntry[] EquipmentTable { get { return _equipTable; } }
        public CharacterStatTable[] CharacterStatTable { get { return _charStatTable; } }
        public AdditionTable[] MenuAdditionTable { get { return _addTable; } }
        public uint CharacterPoint { get { return _emulator.ReadUInt24(_basePoint); } }
        public uint MonsterPoint { get { return _emulator.ReadUInt24(_basePoint + 0x14); } }
        public ushort EncounterID { get { return _emulator.ReadUShort(_encounterID); } set { _emulator.WriteUShort(_encounterID, value); } }
        public byte MonsterSize { get { return _emulator.ReadByte(_monsterSize); } }
        public byte UniqueMonsterSize { get { return _emulator.ReadByte(_uniqueMonsterSize); } }

        public MemoryController(Emu emulator) {
            _emulator = emulator;
            _partySlot = new UIntCollection(emulator.RegionalAddresses["PARTY_SLOT"], 4, 3);
            _disc = emulator.RegionalAddresses["DISC"];
            _chapter = emulator.RegionalAddresses["CHAPTER"];
            _mapId = emulator.RegionalAddresses["MAP"];
            _dragoonSpirits = emulator.RegionalAddresses["DRAGOON_SPIRITS"];
            _hotkey = emulator.RegionalAddresses["HOTKEY"];
            _battleValue = emulator.RegionalAddresses["BATTLE_VALUE"];
            _equipInventory = new ByteCollection(emulator.RegionalAddresses["ARMOR_INVENTORY"], 1, 255);
            _itemInventory = new ByteCollection(emulator.RegionalAddresses["INVENTORY"], 1, 64);
            _menu = emulator.RegionalAddresses["MENU"];
            _transition = emulator.RegionalAddresses["TRANSITION"];
            _gold = emulator.RegionalAddresses["GOLD"];
            _menuUnlock = emulator.RegionalAddresses["MENU_UNLOCK"];
            var charTableAddr = emulator.RegionalAddresses["CHAR_TABLE"];
            var secondCharTableAddr = emulator.RegionalAddresses["SECONDARY_CHARACTER_TABLE"];
            for (int i = 0; i < 9; i++) {
                _characterTable[i] = new CharacterTable(charTableAddr, i);
                _secondaryCharacterTable[i] = new SecondaryCharacterTable(secondCharTableAddr, i);
            }
            var shopListAddr = emulator.RegionalAddresses["SHOP_LIST"];
            for (int i = 0; i < _shop.Length; i++) {
                _shop[i] = new Shop(shopListAddr, i);
            }
            _currentShop = new CurrentShop(emulator.RegionalAddresses["SHOP_CONTENT"]);
            var itemSellPriceAddr = emulator.RegionalAddresses["SHOP_PRICE"];
            _itemSellPrice = new UShortCollection(itemSellPriceAddr, 2, 256);
            _shopID = emulator.RegionalAddresses["SHOP_ID"];
            var equipTableAddr = emulator.RegionalAddresses["ITEM_TABLE"] - 1; // Fixing current incorrect start
            for (int i = 0; i < _equipTable.Length; i++) {
                _equipTable[i] = new EquipmentTableEntry(equipTableAddr, i);
            }
            var itemTableAddr = emulator.RegionalAddresses["THROWN_ITEM_TABLE"];
            for (int i = 0; i < _itemTable.Length; i++) {
                _itemTable[i] = new ItemTableEntry(itemTableAddr, i);
            }
            var charStatTableAddr = emulator.RegionalAddresses["CHAR_STAT_TABLE"];
            for (int i = 0; i < _charStatTable.Length; i++) {
                _charStatTable[i] = new CharacterStatTable(charStatTableAddr, i);
            }
            var dragoonStatTableAddr = emulator.RegionalAddresses["DRAGOON_TABLE"];
            for (int i = 0; i < _dragoonStatTable.Length; i++) {
                _dragoonStatTable[i] = new DragoonStatTable(dragoonStatTableAddr, i);
            }
            var addTableAddr = emulator.RegionalAddresses["MENU_ADDITION_TABLE_FLAT"];
            var addMultiAddr = emulator.RegionalAddresses["MENU_ADDITION_TABLE_MULTI"];
            for (int i = 0; i < _addTable.Length; i++) {
                _addTable[i] = new AdditionTable(addTableAddr, addMultiAddr, i);
            }
            _basePoint = emulator.RegionalAddresses["C_POINT"];
            _encounterID = emulator.RegionalAddresses["ENCOUNTER_ID"];
            _monsterSize = emulator.RegionalAddresses["MONSTER_SIZE"];
            _uniqueMonsterSize = emulator.RegionalAddresses["UNIQUE_MONSTER_SIZE"];
        }
    }
}
