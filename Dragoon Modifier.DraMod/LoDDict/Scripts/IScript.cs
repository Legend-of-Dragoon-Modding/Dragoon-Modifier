using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.LoDDict.Scripts {
    public interface IScript {
        void BattleSetup(ILoDDictionary loDDictionary);
        void BattleRun(ILoDDictionary loDDictionary);
        void FieldSetup(ILoDDictionary loDDictionary);
        void FieldRun(ILoDDictionary loDDictionary);
    }
}
