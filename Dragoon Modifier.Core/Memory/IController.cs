using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Core.Memory {
    public interface IController {
        Battle.IController Battle { get; }
        void LoadBattle();
        void UnloadBattle();
        uint BattleBasePoint { get; }
        ushort BattleValue { get; set; }
        Memory.CurrentShop CurrentShop { get; }
        byte Disc { get; }
        byte DragoonSpirits { get; set; }
        ushort EncounterID { get; set; }
        Collections.IAddress<byte> EquipmentInventory { get; }
        GameState GameState { get; }
        MenuSubTypes MenuSubTypes { get; }
        uint Gold { get; set; }
        ushort Hotkey { get; set; }
        byte Chapter { get; }
        uint CharacterPoint { get; }
        Memory.CharacterTable[] CharacterTable { get; }
        Memory.IItem[] Item { get; }
        Collections.IAddress<byte> ItemInventory { get; }
        ushort MapID { get; set; }
        byte Menu { get; set; }
        byte MenuSubType { get; set; }
        Memory.AdditionTable[] MenuAdditionTable { get; }
        byte MenuUnlock { get; set; }
        uint MonsterPoint { get; }
        byte MonsterSize { get; }
        byte OverworldContinent { get; set; }
        byte OverworldCheck { get; set; }
        byte OverworldSegment { get; set; }
        Collections.IAddress<uint> PartySlot { get; }
        Memory.SecondaryCharacterTable[] SecondaryCharacterTable { get; }
        Memory.Shop[] Shop { get; }
        byte ShopID { get; set; }
        byte Transition { get; set; }
        byte UniqueMonsterSize { get; }
        byte SavePoint { get; set; }
        ushort TextSpeed { get; set; }
        ushort AutoText { get; set; }
        ushort FieldHPCap { get; set; }
        FieldPosition FieldPosition { get; }
    }
}
