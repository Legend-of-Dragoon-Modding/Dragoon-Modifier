using System.Collections.Generic;

namespace Dragoon_Modifier.DraMod.LoDDict {
    public interface ILoDDictionary {
        IItem[] Item { get; }

        Character[] Character { get; }

        byte[] ItemNames { get; }
        byte[] ItemDescriptions { get; }
        byte[] ItemBattleNames { get; }
        byte[] ItemBattleDescriptions { get; }
        Dictionary<ushort, Monster> Monster { get; }
        List<byte>[] Shop { get; }

        Scripts.IScript ItemScript { get;}

        bool TryItem2Num(string name, out byte id);
        void SwapMonsters(string cwd, string mod);
    }
}