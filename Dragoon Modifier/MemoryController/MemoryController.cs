using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.MemoryController {
    public class MemoryController {
        ULongCollection _partySlot;
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

        public ULongCollection PartySlot { get { return _partySlot; } set { _partySlot = value; } }
        public byte Disc { get { return Emulator.ReadByte(_disc); } }
        public byte Chapter { get { return (byte) (Emulator.ReadByte(_chapter) + 1); } }
        public ushort MapID { get { return Emulator.ReadUShort(_mapId); } set { Emulator.WriteUShort(_mapId, value); } }
        public byte DragoonSpirits { get { return Emulator.ReadByte(_dragoonSpirits); } set { Emulator.WriteByte(_dragoonSpirits, value); } }
        public ushort Hotkey { get { return Emulator.ReadUShort(_hotkey); } set { Emulator.WriteByte(_hotkey, value); } }
        public ushort BattleValue { get { return Emulator.ReadUShort(_battleValue); } set { Emulator.WriteUShort(_battleValue, value); } }
        public ByteCollection EquipmentInventory { get { return _equipInventory; } set { _equipInventory = value; } }
        public ByteCollection ItemInventory { get { return _itemInventory; } set { _itemInventory = value; } }
        public byte Menu { get { return Emulator.ReadByte(_menu); } set { Emulator.WriteByte(_menu, value); } }
        public byte Transition { get { return Emulator.ReadByte(_transition); } set { Emulator.WriteByte(_transition, value); } }
        public ulong Gold { get { return Emulator.ReadULong(_gold); } set { Emulator.WriteULong(_gold, value); } }
        public byte MenuUnlock { get { return Emulator.ReadByte(_menuUnlock); } set { Emulator.WriteByte(_menuUnlock, value); } }

        public MemoryController() {
            _partySlot = new ULongCollection(Constants.GetAddress("PARTY_SLOT"), 4, 3);
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
        }
    }
}
