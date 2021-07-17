using System.Collections.Generic;

namespace Dragoon_Modifier.DraMod.LoDDict {
    public interface ILoDDictionary {
        IItem[] Item { get; }

        string ItemNames { get; }
        string ItemDescriptions { get; }
        string ItemBattleNames { get; }
        string ItemBattleDescriptions { get; }
        Dictionary<ushort, Monster> Monster { get; }

        bool TryItem2Num(string name, out byte id);
    }
}