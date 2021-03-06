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
        long _map;
        long _dragoonSpirits;
        long _hotkey;
        long _battleValue;
        ByteCollection _equipInventory;
        ByteCollection _itemInventory;

        public ULongCollection PartySlot { get { return _partySlot; } set { _partySlot = value; } }
        public byte Disc { get { return Emulator.ReadByte(_disc); } }
        public byte Chapter { get { return (byte) (Emulator.ReadByte(_chapter) + 1); } }
        public ushort Map { get { return Emulator.ReadUShort(_map); } set { Emulator.WriteUShort(_map, value); } }
        public byte DragoonSpirits { get { return Emulator.ReadByte(_dragoonSpirits); } set { Emulator.WriteByte(_dragoonSpirits, value); } }
        public ushort Hotkey { get { return Emulator.ReadUShort(_hotkey); } set { Emulator.WriteByte(_hotkey, value); } }
        public ushort BattleValue { get { return Emulator.ReadUShort(_battleValue); } set { Emulator.WriteUShort(_battleValue, value); } }
        public ByteCollection EquipmentInventory { get { return _equipInventory; } set { _equipInventory = value; } }
        public ByteCollection ItemInventory { get { return _itemInventory; } set { _itemInventory = value; } }

        public MemoryController() {
            _partySlot = new ULongCollection(Constants.GetAddress("PARTY_SLOT"), 4);
            _disc = Constants.GetAddress("DISC");
            _map = Constants.GetAddress("MAP");
            _dragoonSpirits = Constants.GetAddress("DRAGOON_SPIRITS");
            _hotkey = Constants.GetAddress("HOTKEY");
            _battleValue = Constants.GetAddress("BATTLE_VALUE");
            _equipInventory = new ByteCollection(Constants.GetAddress("ARMOR_INVENTORY"), 1);
            _itemInventory = new ByteCollection(Constants.GetAddress("INVENTORY"), 1);
        }
    }
}
