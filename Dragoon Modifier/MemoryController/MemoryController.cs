using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.MemoryController {
    public class MemoryController {
        UIntCollection _partySlot;
        long _disc;
        long _chapter;
        long _mapId;
        long _dragoonSpirits;
        long _hotkey;
        long _battleValue;
        ByteCollection _equipInventory;
        ByteCollection _itemInventory;
        long _menu;
        long _transition;
        long _gold;
        long _menuUnlock;
        CharacterTable[] _characterTable = new CharacterTable[9];
        SecondaryCharacterTable[] _secondaryCharacterTable = new SecondaryCharacterTable[9];
        Shop[] _shop = new Shop[45]; // There are most likely up to 64 shops. But most of it isn't used by the base game.
        CurrentShop _currentShop;
        UShortCollection _itemSellPrice;
        long _shopID;
        EquipmentTableEntry[] _equipTable = new EquipmentTableEntry[192];
        ItemTableEntry[] _itemTable = new ItemTableEntry[64]; // Number of items should be verified
        CharacterStatTable[] _charStatTable = new CharacterStatTable[7];
        DragoonStatTable[] _dragoonStatTable = new DragoonStatTable[9];
        AdditionTable[] _addTable = new AdditionTable[41];
        long _basePoint;

        public UIntCollection PartySlot { get { return _partySlot; } }
        public byte Disc { get { return Emulator.ReadByte(_disc); } }
        public byte Chapter { get { return (byte) (Emulator.ReadByte(_chapter) + 1); } }
        public ushort MapID { get { return Emulator.ReadUShort(_mapId); } set { Emulator.WriteUShort(_mapId, value); } }
        public byte DragoonSpirits { get { return Emulator.ReadByte(_dragoonSpirits); } set { Emulator.WriteByte(_dragoonSpirits, value); } }
        public ushort Hotkey { get { return Emulator.ReadUShort(_hotkey); } set { Emulator.WriteByte(_hotkey, value); } }
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
        public uint CharacterPoint { get { return Emulator.ReadUInt24(_basePoint); } }
        public uint MonsterPoint { get { return Emulator.ReadUInt24(_basePoint + 0x14); } }

        public MemoryController() {
            _partySlot = new UIntCollection(Constants.GetAddress("PARTY_SLOT"), 4, 3);
            _disc = Constants.GetAddress("DISC");
            _chapter = Constants.GetAddress("CHAPTER");
            _mapId = Constants.GetAddress("MAP");
            _dragoonSpirits = Constants.GetAddress("DRAGOON_SPIRITS");
            _hotkey = Constants.GetAddress("HOTKEY");
            _battleValue = Constants.GetAddress("BATTLE_VALUE");
            _equipInventory = new ByteCollection(Constants.GetAddress("ARMOR_INVENTORY"), 1, 255);
            _itemInventory = new ByteCollection(Constants.GetAddress("INVENTORY"), 1, 64);
            _menu = Constants.GetAddress("MENU");
            _transition = Constants.GetAddress("TRANSITION");
            _gold = Constants.GetAddress("GOLD");
            _menuUnlock = Constants.GetAddress("MENU_UNLOCK");
            var charTableAddr = Constants.GetAddress("CHAR_TABLE");
            var secondCharTableAddr = Constants.GetAddress("SECONDARY_CHARACTER_TABLE");
            for (int i = 0; i < 9; i++) {
                _characterTable[i] = new CharacterTable(charTableAddr, i);
                _secondaryCharacterTable[i] = new SecondaryCharacterTable(secondCharTableAddr, i);
            }
            var shopListAddr = Constants.GetAddress("SHOP_LIST");
            for (int i = 0; i < _shop.Length; i++) {
                _shop[i] = new Shop(shopListAddr, i);
            }
            _currentShop = new CurrentShop(Constants.GetAddress("SHOP_CONTENT"));
            var itemSellPriceAddr = Constants.GetAddress("SHOP_PRICE");
            _itemSellPrice = new UShortCollection(itemSellPriceAddr, 2, 256);
            _shopID = Constants.GetAddress("SHOP_ID");
            var equipTableAddr = Constants.GetAddress("ITEM_TABLE") - 1; // Fixing current incorrect start
            for (int i = 0; i < _equipTable.Length; i++) {
                _equipTable[i] = new EquipmentTableEntry(equipTableAddr, i);
            }
            var itemTableAddr = Constants.GetAddress("THROWN_ITEM_TABLE");
            for (int i = 0; i < _itemTable.Length; i++) {
                _itemTable[i] = new ItemTableEntry(itemTableAddr, i);
            }
            var charStatTableAddr = Constants.GetAddress("CHAR_STAT_TABLE");
            for (int i = 0; i < _charStatTable.Length; i++) {
                _charStatTable[i] = new CharacterStatTable(charStatTableAddr, i);
            }
            var dragoonStatTableAddr = Constants.GetAddress("DRAGOON_TABLE");
            for (int i = 0; i < _dragoonStatTable.Length; i++) {
                _dragoonStatTable[i] = new DragoonStatTable(dragoonStatTableAddr, i);
            }
            var addTableAddr = Constants.GetAddress("MENU_ADDITION_TABLE_FLAT");
            var addMultiAddr = Constants.GetAddress("MENU_ADDITION_TABLE_MULTI");
            for (int i = 0; i < _addTable.Length; i++) {
                _addTable[i] = new AdditionTable(addTableAddr, addMultiAddr, i);
            }
            _basePoint = Constants.GetAddress("C_POINT");
        }
    }
}
