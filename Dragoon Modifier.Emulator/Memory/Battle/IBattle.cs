using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Emulator.Memory.Battle {
    public interface IBattle {
        ushort BattleMenuCount { get; set; }
        byte BattleMenuChosenSlot { get; set; }
        Collections.IAddress<byte> BattleMenuSlot { get; }
        int BattleOffset { get; }
        ushort EncounterID { get; }
        uint CharacterPoint { get; }
        Character[] CharacterTable { get; }
        ushort[] MonsterID { get; }
        uint MonsterPoint { get; }
        Monster[] MonsterTable { get; }
        Collections.IAddress<ushort> UniqueMonsterID { get; }
        byte ItemUsed { get; set; }
        ushort DamageCap { get; set; }
    }
}
