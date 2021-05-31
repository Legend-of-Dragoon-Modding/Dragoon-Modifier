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
        int _overworldContinent;
        int _overworldSegment;
        int _overworldCheck;
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

        public UIntCollection PartySlot { get { return _partySlot; } }
        public byte Disc { get { return Emulator.ReadByte(_disc); } }
        public byte Chapter { get { return (byte) (Emulator.ReadByte(_chapter) + 1); } }
        public ushort MapID { get { return Emulator.ReadUShort(_mapId); } set { Emulator.WriteUShort(_mapId, value); } }
        public byte OverworldContinent { get { return Emulator.ReadByte(_overworldContinent); } set { Emulator.WriteByte(_overworldContinent, value); } }
        public byte OverworldSegment { get { return Emulator.ReadByte(_overworldSegment); } set { Emulator.WriteByte(_overworldSegment, value); } }
        public byte OverworldCheck { get { return Emulator.ReadByte(_overworldCheck); } set { Emulator.WriteByte(_overworldCheck, value); } }
        public byte DragoonSpirits { get { return Emulator.ReadByte(_dragoonSpirits); } set { Emulator.WriteByte(_dragoonSpirits, value); } }
        public ushort Hotkey { get { return Emulator.ReadUShort(_hotkey); } set { Emulator.WriteUShort(_hotkey, value); } } // Should be writing here allowed?
        public ushort BattleValue { get { return Emulator.ReadUShort(_battleValue); } set { Emulator.WriteUShort(_battleValue, value); } }
        public ByteCollection EquipmentInventory { get { return _equipInventory; } }
        public ByteCollection ItemInventory { get { return _itemInventory; } }
        public byte Menu { get { return Emulator.ReadByte(_menu); } set { Emulator.WriteByte(_menu, value); } }
        public byte Transition { get { return Emulator.ReadByte(_transition); } set { Emulator.WriteByte(_transition, value); } }
        public uint Gold { get { return Emulator.ReadUInt(_gold); } set { Emulator.WriteUInt(_gold, value); } }
        public byte MenuUnlock { get { return Emulator.ReadByte(_menuUnlock); } set { Emulator.WriteByte(_menuUnlock, value); } }
        public CharacterTable[] CharacterTable { get { return _characterTable; } }
        public SecondaryCharacterTable[] SecondaryCharacterTable { get { return _secondaryCharacterTable; } }
        public Shop[] Shop { get { return _shop; } }
        public CurrentShop CurrentShop { get { return _currentShop; } }
        public UShortCollection ItemSellPrice { get { return _itemSellPrice; } }
        public byte ShopID { get { return Emulator.ReadByte(_shopID); } set { Emulator.WriteByte(_shopID, value); } }
        public EquipmentTableEntry[] EquipmentTable { get { return _equipTable; } }
        public CharacterStatTable[] CharacterStatTable { get { return _charStatTable; } }
        public AdditionTable[] MenuAdditionTable { get { return _addTable; } }
        public uint BattlePointBase { get { return Emulator.ReadUInt24(_basePoint - 0x18); } }
        public uint CharacterPoint { get { return Emulator.ReadUInt24(_basePoint); } }
        public uint MonsterPoint { get { return Emulator.ReadUInt24(_basePoint + 0x14); } }
        public ushort EncounterID { get { return Emulator.ReadUShort(_encounterID); } set { Emulator.WriteUShort(_encounterID, value); } }
        public byte MonsterSize { get { return Emulator.ReadByte(_monsterSize); } }
        public byte UniqueMonsterSize { get { return Emulator.ReadByte(_uniqueMonsterSize); } }
        public GameState GameState { get { return GetGameState(); } }

        public MemoryController() {
            _partySlot = new UIntCollection(Emulator.GetAddress("PARTY_SLOT"), 4, 3);
            _disc = Emulator.GetAddress("DISC");
            _chapter = Emulator.GetAddress("CHAPTER");
            _mapId = Emulator.GetAddress("MAP");
            _overworldContinent = 0xBF0B0; // TODO
            _overworldSegment = 0xC67AC; // TODO
            _overworldCheck = 0xBB10C; // TODO
            _dragoonSpirits = Emulator.GetAddress("DRAGOON_SPIRITS");
            _hotkey = Emulator.GetAddress("HOTKEY");
            _battleValue = Emulator.GetAddress("BATTLE_VALUE");
            _equipInventory = new ByteCollection(Emulator.GetAddress("ARMOR_INVENTORY"), 1, 255);
            _itemInventory = new ByteCollection(Emulator.GetAddress("INVENTORY"), 1, 64);
            _menu = Emulator.GetAddress("MENU");
            _transition = Emulator.GetAddress("TRANSITION");
            _gold = Emulator.GetAddress("GOLD");
            _menuUnlock = Emulator.GetAddress("MENU_UNLOCK");
            var charTableAddr = Emulator.GetAddress("CHAR_TABLE");
            var secondCharTableAddr = Emulator.GetAddress("SECONDARY_CHARACTER_TABLE");
            for (int i = 0; i < 9; i++) {
                _characterTable[i] = new CharacterTable(charTableAddr, i);
                _secondaryCharacterTable[i] = new SecondaryCharacterTable(secondCharTableAddr, i);
            }
            var shopListAddr = Emulator.GetAddress("SHOP_LIST");
            for (int i = 0; i < _shop.Length; i++) {
                _shop[i] = new Shop(shopListAddr, i);
            }
            _currentShop = new CurrentShop(Emulator.GetAddress("SHOP_CONTENT"));
            var itemSellPriceAddr = Emulator.GetAddress("SHOP_PRICE");
            _itemSellPrice = new UShortCollection(itemSellPriceAddr, 2, 256);
            _shopID = Emulator.GetAddress("SHOP_ID");
            var equipTableAddr = Emulator.GetAddress("ITEM_TABLE") - 1; // Fixing current incorrect start
            for (int i = 0; i < _equipTable.Length; i++) {
                _equipTable[i] = new EquipmentTableEntry(equipTableAddr, i);
            }
            var itemTableAddr = Emulator.GetAddress("THROWN_ITEM_TABLE");
            for (int i = 0; i < _itemTable.Length; i++) {
                _itemTable[i] = new ItemTableEntry(itemTableAddr, i);
            }
            var charStatTableAddr = Emulator.GetAddress("CHAR_STAT_TABLE");
            for (int i = 0; i < _charStatTable.Length; i++) {
                _charStatTable[i] = new CharacterStatTable(charStatTableAddr, i);
            }
            var dragoonStatTableAddr = Emulator.GetAddress("DRAGOON_TABLE");
            for (int i = 0; i < _dragoonStatTable.Length; i++) {
                _dragoonStatTable[i] = new DragoonStatTable(dragoonStatTableAddr, i);
            }
            var addTableAddr = Emulator.GetAddress("MENU_ADDITION_TABLE_FLAT");
            var addMultiAddr = Emulator.GetAddress("MENU_ADDITION_TABLE_MULTI");
            for (int i = 0; i < _addTable.Length; i++) {
                _addTable[i] = new AdditionTable(addTableAddr, addMultiAddr, i);
            }
            _basePoint = Emulator.GetAddress("C_POINT");
            _encounterID = Emulator.GetAddress("ENCOUNTER_ID");
            _monsterSize = Emulator.GetAddress("MONSTER_SIZE");
            _uniqueMonsterSize = Emulator.GetAddress("UNIQUE_MONSTER_SIZE");
        }

        private GameState GetGameState() {
            switch (Menu) {
                case 0:
                    if (BattleValue == 41215) {
                        return GameState.Battle;
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
    }
}
