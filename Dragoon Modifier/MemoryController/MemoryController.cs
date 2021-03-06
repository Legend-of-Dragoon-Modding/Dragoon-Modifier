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
            var partyAddr = Constants.GetAddress("PARTY_SLOT");
            _partySlot = new ULongCollection(new long[] { partyAddr, partyAddr + 4, partyAddr + 8});
            _disc = Constants.GetAddress("DISC");
            _map = Constants.GetAddress("MAP");
            _dragoonSpirits = Constants.GetAddress("DRAGOON_SPIRITS");
            _hotkey = Constants.GetAddress("HOTKEY");
            _battleValue = Constants.GetAddress("BATTLE_VALUE");
            var equipInvAddr = Constants.GetAddress("ARMOR_INVENTORY");
            var temp = new long[255];
            for (int i = 0; i <255; i++) {
                temp[i] = equipInvAddr;
                equipInvAddr++;
            }
            _equipInventory = new ByteCollection(temp);
            var itemInvAddr = Constants.GetAddress("INVENTORY");
            temp = new long[64];
            for (int i = 0; i < 64; i++) {
                temp[i] = itemInvAddr;
                itemInvAddr++;
            }
            _itemInventory = new ByteCollection(temp);
        }
    }
}
