using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Dataset.Scripts {
    public interface IScript {
        void BattleSetup();
        void BattleRun();
        void FieldSetup();
        void FieldRun();
    }
}
