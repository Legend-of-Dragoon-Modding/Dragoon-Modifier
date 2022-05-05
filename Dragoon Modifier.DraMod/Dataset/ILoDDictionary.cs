using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Dataset {
    public interface ILoDDictionary {
        IItem[] Item { get; }
        ICharacter[] Character { get; }
        byte[] ItemNames { get; }
        byte[] ItemDescriptions { get; }
        byte[] ItemBattleNames { get; }
        byte[] ItemBattleDescriptions { get; }
        Dictionary<ushort, IMonster> Monster { get; }
        List<byte>[] Shop { get; }
        Scripts.IScript Script { get; }
        bool TryEncodeItem(string name, out byte id);
        

    }
}
