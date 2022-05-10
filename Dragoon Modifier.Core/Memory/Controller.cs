using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Core.Memory {
    internal class Controller : IController {
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
        private readonly int _menuSubType;
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

        private Battle.IController _battle = null;
        public Battle.IController Battle {
            get {
                if (_battle == null) {
                    throw new BattleNotInitializedException();
                }
                return _battle;
            }
        }

        public Collections.IAddress<uint> PartySlot { get; private set; }
        public byte Disc { get { return Emulator.DirectAccess.ReadByte(_disc); } }
        public byte Chapter { get { return (byte) (Emulator.DirectAccess.ReadByte(_chapter) + 1); } }
        public ushort MapID { get { return Emulator.DirectAccess.ReadUShort(_mapId); } set { Emulator.DirectAccess.WriteUShort(_mapId, value); } }
        public byte OverworldContinent { get { return Emulator.DirectAccess.ReadByte(_overworldContinent); } set { Emulator.DirectAccess.WriteByte(_overworldContinent, value); } }
        public byte OverworldSegment { get { return Emulator.DirectAccess.ReadByte(_overworldSegment); } set { Emulator.DirectAccess.WriteByte(_overworldSegment, value); } }
        public byte OverworldCheck { get { return Emulator.DirectAccess.ReadByte(_overworldCheck); } set { Emulator.DirectAccess.WriteByte(_overworldCheck, value); } }
        public byte DragoonSpirits { get { return Emulator.DirectAccess.ReadByte(_dragoonSpirits); } set { Emulator.DirectAccess.WriteByte(_dragoonSpirits, value); } }
        public ushort Hotkey { get { return Emulator.DirectAccess.ReadUShort(_hotkey); } set { Emulator.DirectAccess.WriteUShort(_hotkey, value); } } // Should writing here be allowed?
        public ushort BattleValue { get { return Emulator.DirectAccess.ReadUShort(_battleValue); } set { Emulator.DirectAccess.WriteUShort(_battleValue, value); } }
        public Collections.IAddress<byte> EquipmentInventory { get; private set; }
        public Collections.IAddress<byte> ItemInventory { get; private set; }
        public byte Menu { get { return Emulator.DirectAccess.ReadByte(_menu); } set { Emulator.DirectAccess.WriteByte(_menu, value); } }
        public byte MenuSubType { get { return Emulator.DirectAccess.ReadByte(_menuSubType); } set { Emulator.DirectAccess.WriteByte(_menuSubType, value); } }
        public byte Transition { get { return Emulator.DirectAccess.ReadByte(_transition); } set { Emulator.DirectAccess.WriteByte(_transition, value); } }
        public uint Gold { get { return Emulator.DirectAccess.ReadUInt(_gold); } set { Emulator.DirectAccess.WriteUInt(_gold, value); } }
        public byte MenuUnlock { get { return Emulator.DirectAccess.ReadByte(_menuUnlock); } set { Emulator.DirectAccess.WriteByte(_menuUnlock, value); } }
        public CharacterTable[] CharacterTable { get; private set; }
        public SecondaryCharacterTable[] SecondaryCharacterTable { get; private set; }
        public Shop[] Shop { get; private set; } = new Shop[45]; // Most likely up to 64 shops. But most of it is unused, so I chose a safe number
        public CurrentShop CurrentShop { get; private set; }
        public byte ShopID { get { return Emulator.DirectAccess.ReadByte(_shopID); } set { Emulator.DirectAccess.WriteByte(_shopID, value); } }
        public IItem[] Item { get; private set; } = new IItem[256];
        public byte SavePoint { get { return Emulator.DirectAccess.ReadByte(_savePoint); } set { Emulator.DirectAccess.WriteByte(_savePoint, value); } }
        public ushort TextSpeed { get { return Emulator.DirectAccess.ReadUShort(_textSpeed); } set { Emulator.DirectAccess.WriteUShort(_textSpeed, value); } }
        public ushort AutoText { get { return Emulator.DirectAccess.ReadUShort(_autoText); } set { Emulator.DirectAccess.WriteUShort(_autoText, value); } }
        public CharacterStatTable CharacterStatTable { get; private set; }
        public DragoonStatTable DragoonStatTable { get; private set; }
        public AdditionTable[] MenuAdditionTable { get; private set; }
        public uint BattleBasePoint { get { return Emulator.DirectAccess.ReadUInt24(_basePoint); } }
        public uint CharacterPoint { get { return Emulator.DirectAccess.ReadUInt24(_basePoint + 0x18); } }
        public uint MonsterPoint { get { return Emulator.DirectAccess.ReadUInt24(_basePoint + 0x2C); } }
        public ushort EncounterID { get { return Emulator.DirectAccess.ReadUShort(_encounterID); } set { Emulator.DirectAccess.WriteUShort(_encounterID, value); } }
        public byte MonsterSize { get { return Emulator.DirectAccess.ReadByte(_monsterSize); } }
        public byte UniqueMonsterSize { get { return Emulator.DirectAccess.ReadByte(_uniqueMonsterSize); } }
        public GameState GameState { get { return GetGameState(); } }
        public MenuSubTypes MenuSubTypes { get { return GetMenuSubType(); } }
        public byte DiscGameCheck { get { return Emulator.DirectAccess.ReadByte(_discChangeCheck); } }
        public ushort FieldHPCap { get { return Emulator.DirectAccess.ReadUShort(_fieldHPCap1); } set { SetFieldHPCap(value); } }
        public FieldPosition FieldPosition { get; }
        public Encounter.Map FieldEncounterMap { get; }
        public Encounter.Slot FieldEncounterSlot { get; }


        internal Controller() {
            PartySlot = Collections.Factory.Create<uint>(Emulator.GetAddress("PARTY_SLOT"), 4, 3);
            _disc = Emulator.GetAddress("DISC");
            _chapter = Emulator.GetAddress("CHAPTER");
            _mapId = Emulator.GetAddress("MAP");
            _overworldContinent = Emulator.GetAddress("OVERWORLD_CONTINENT");
            _overworldSegment = Emulator.GetAddress("OVERWORLD_SEGMENT");
            _overworldCheck = Emulator.GetAddress("OVERWORLD_CHECK");
            _dragoonSpirits = Emulator.GetAddress("DRAGOON_SPIRITS");
            _hotkey = Emulator.GetAddress("HOTKEY");
            _battleValue = Emulator.GetAddress("BATTLE_VALUE");
            EquipmentInventory = Collections.Factory.Create<Byte>(Emulator.GetAddress("ARMOR_INVENTORY"), 1, 256);
            ItemInventory = Collections.Factory.Create<byte>(Emulator.GetAddress("INVENTORY"), 1, 64);
            _menu = Emulator.GetAddress("MENU");
            _menuSubType = Emulator.GetAddress("MENU_SUBTYPE");
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
            var shopListAddr = Emulator.GetAddress("SHOP_LIST");
            for (int i = 0; i < Shop.Length; i++) {
                Shop[i] = new Shop(shopListAddr, i);
            }
            CurrentShop = new CurrentShop(Emulator.GetAddress("SHOP_CONTENT"));
            _shopID = Emulator.GetAddress("SHOP_ID");
            _savePoint = Emulator.GetAddress("SAVE_POINT");
            _textSpeed = Emulator.GetAddress("TEXT_SPEED");
            _autoText = Emulator.GetAddress("AUTO_TEXT");
            var equipTableAddr = Emulator.GetAddress("ITEM_TABLE");
            var itemTableAddr = Emulator.GetAddress("THROWN_ITEM_TABLE");
            var itemNamePtr = Emulator.GetAddress("ITEM_NAME_PTR");
            var itemDescPtr = Emulator.GetAddress("ITEM_DESC_PTR");
            var itemBattleNamePtr = Emulator.GetAddress("ITEM_BTL_NAME_PTR");
            var itemBattleDescPtr = Emulator.GetAddress("ITEM_BTL_DESC_PTR");
            var itemSellPriceAddr = Emulator.GetAddress("SHOP_PRICE");
            for (int i = 0; i < 192; i++) {
                Item[i] = new Equipment(equipTableAddr, itemNamePtr, itemDescPtr, itemSellPriceAddr, i);
            }
            for (int i = 192; i < 256; i++) {
                Item[i] = new UsableItem(itemTableAddr, itemNamePtr, itemDescPtr, itemBattleNamePtr, itemBattleDescPtr, itemSellPriceAddr, i);
            }
            CharacterStatTable = new CharacterStatTable();
            DragoonStatTable = new DragoonStatTable();
            var addTableAddr = Emulator.GetAddress("MENU_ADDITION_TABLE_FLAT");
            var addMultiAddr = Emulator.GetAddress("MENU_ADDITION_TABLE_MULTI");
            MenuAdditionTable = new AdditionTable[41];
            for (int i = 0; i < MenuAdditionTable.Length; i++) {
                MenuAdditionTable[i] = new AdditionTable(addTableAddr, addMultiAddr, i);
            }
            _basePoint = Emulator.GetAddress("BATTLE_BASE_POINT");
            _encounterID = Emulator.GetAddress("ENCOUNTER_ID");
            _monsterSize = Emulator.GetAddress("MONSTER_SIZE");
            _uniqueMonsterSize = Emulator.GetAddress("UNIQUE_MONSTER_SIZE");
            FieldEncounterMap = new Encounter.Map();
            FieldEncounterSlot = new Encounter.Slot();
            _discChangeCheck = Emulator.GetAddress("DISC_CHANGE_CHECK");
            _fieldHPCap1 = Emulator.GetAddress("FIELD_HP_CAP_1");
            _fieldHPCap2 = Emulator.GetAddress("FIELD_HP_CAP_2");
            _fieldHPCap3 = Emulator.GetAddress("FIELD_HP_CAP_3");
            _fieldHPCap4 = Emulator.GetAddress("FIELD_HP_CAP_4");
            FieldPosition = new FieldPosition();

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

        private MenuSubTypes GetMenuSubType() {
            switch (MenuSubType) {
                case 20:
                    return MenuSubTypes.Status;
                case 26:
                    return MenuSubTypes.UseIt;
                case 31:
                    return MenuSubTypes.Discard;
                case 16:
                    return MenuSubTypes.List;
                case 35:
                    return MenuSubTypes.Goods;
                case 12:
                    return MenuSubTypes.Armed;
                case 23:
                    return MenuSubTypes.Addition;
                case 8:
                    return MenuSubTypes.Replace;
                case 125:
                    return MenuSubTypes.FirstMenu;
                default:
                    return MenuSubTypes.Default;
            }
        }

        private void SetFieldHPCap(ushort value) {
            Emulator.DirectAccess.WriteUShort(_fieldHPCap1, value);
            Emulator.DirectAccess.WriteUShort(_fieldHPCap2, value);
            Emulator.DirectAccess.WriteUShort(_fieldHPCap3, value);
            Emulator.DirectAccess.WriteUShort(_fieldHPCap4, value);
        }

        public void LoadBattle() {
            _battle = new Battle.Controller();
        }

        public void UnloadBattle() {
            _battle = null;
        }
    }
}
