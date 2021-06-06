namespace Dragoon_Modifier.Core.Memory {
    public class Controller {
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

        public Collections.UInt PartySlot { get; private set; }
        public byte Disc { get { return Emulator.ReadByte(_disc); } }
        public byte Chapter { get { return (byte) (Emulator.ReadByte(_chapter) + 1); } }
        public ushort MapID { get { return Emulator.ReadUShort(_mapId); } set { Emulator.WriteUShort(_mapId, value); } }
        public byte OverworldContinent { get { return Emulator.ReadByte(_overworldContinent); } set { Emulator.WriteByte(_overworldContinent, value); } }
        public byte OverworldSegment { get { return Emulator.ReadByte(_overworldSegment); } set { Emulator.WriteByte(_overworldSegment, value); } }
        public byte OverworldCheck { get { return Emulator.ReadByte(_overworldCheck); } set { Emulator.WriteByte(_overworldCheck, value); } }
        public byte DragoonSpirits { get { return Emulator.ReadByte(_dragoonSpirits); } set { Emulator.WriteByte(_dragoonSpirits, value); } }
        public ushort Hotkey { get { return Emulator.ReadUShort(_hotkey); } set { Emulator.WriteUShort(_hotkey, value); } } // Should be writing here allowed?
        public ushort BattleValue { get { return Emulator.ReadUShort(_battleValue); } set { Emulator.WriteUShort(_battleValue, value); } }
        public Collections.Byte EquipmentInventory { get; private set; }
        public Collections.Byte ItemInventory { get; private set; }
        public byte Menu { get { return Emulator.ReadByte(_menu); } set { Emulator.WriteByte(_menu, value); } }
        public byte Transition { get { return Emulator.ReadByte(_transition); } set { Emulator.WriteByte(_transition, value); } }
        public uint Gold { get { return Emulator.ReadUInt(_gold); } set { Emulator.WriteUInt(_gold, value); } }
        public byte MenuUnlock { get { return Emulator.ReadByte(_menuUnlock); } set { Emulator.WriteByte(_menuUnlock, value); } }
        public CharacterTable[] CharacterTable { get; private set; }
        public SecondaryCharacterTable[] SecondaryCharacterTable { get; private set; }
        // Shop
        // CurrentShop
        // ShopID
        public Item[] Item { get; private set; }

        internal Controller() {
            PartySlot = new Collections.UInt(Emulator.GetAddress("PARTY_SLOT"), 4, 3);
            _disc = Emulator.GetAddress("DISC");
            _chapter = Emulator.GetAddress("CHAPTER");
            _mapId = Emulator.GetAddress("MAP");
            _overworldContinent = 0xBF0B0; // TODO
            _overworldSegment = 0xC67AC; // TODO
            _overworldCheck = 0xBB10C; // TODO
            _dragoonSpirits = Emulator.GetAddress("DRAGOON_SPIRITS");
            _hotkey = Emulator.GetAddress("HOTKEY");
            _battleValue = Emulator.GetAddress("BATTLE_VALUE");
            EquipmentInventory = new Collections.Byte(Emulator.GetAddress("ARMOR_INVENTORY"), 1, 255);
            ItemInventory = new Collections.Byte(Emulator.GetAddress("INVENTORY"), 1, 64);
            _menu = Emulator.GetAddress("MENU");
            _transition = Emulator.GetAddress("TRANSITION");
            _gold = Emulator.GetAddress("GOLD");
            _menuUnlock = Emulator.GetAddress("MENU_UNLOCK");
            var charTableAddr = Emulator.GetAddress("CHAR_TABLE");
            var secondCharTableAddr = Emulator.GetAddress("SECONDARY_CHARACTER_TABLE");
            CharacterTable = new CharacterTable[9];
            SecondaryCharacterTable = new SecondaryCharacterTable[9];
            for (int i = 0; i < CharacterTable.Length; i++) {
                CharacterTable[i] = new CharacterTable(charTableAddr, i);
                SecondaryCharacterTable[i] = new SecondaryCharacterTable(secondCharTableAddr, i);
            }

            // shop stuff

            var equipTableAddr = Emulator.GetAddress("ITEM_TABLE") - 1; // Fix for wrong address
            var itemTableAddr = Emulator.GetAddress("THROWN_ITEM_TABLE");
            int itemNamePtr = Emulator.GetAddress("ITEM_NAME_PTR");
            int itemDescPtr = Emulator.GetAddress("ITEM_DESC_PTR");
            var itemSellPriceAddr = Emulator.GetAddress("SHOP_PRICE");

            Item = new Item[256];
            for (int i = 0; i < 192; i++) {
                Item[i] = new Equipment(equipTableAddr, itemNamePtr, itemDescPtr, itemSellPriceAddr, i);
            }
            for (int i = 192; i < 256; i++) {
                Item[i] = new UsableItem(itemTableAddr, itemNamePtr, itemDescPtr, itemSellPriceAddr, i);
            }


        }
    }
}
