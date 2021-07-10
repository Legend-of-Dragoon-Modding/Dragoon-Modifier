using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.UI {
    public class MonsterUpdate {
        public string Name;
        public ushort HP;
        public ushort MaxHP;
        public ushort AT;
        public ushort MAT;
        public ushort DF;
        public ushort MDF;
        public ushort SPD;
        public ushort Turn;

        internal MonsterUpdate(Emulator.IEmulator emulator, int index) {
            Name = emulator.Battle.MonsterTable[index].Name;
            HP = emulator.Battle.MonsterTable[index].HP;
            MaxHP = emulator.Battle.MonsterTable[index].MaxHP;
            AT = emulator.Battle.MonsterTable[index].AT;
            MAT = emulator.Battle.MonsterTable[index].MAT;
            DF = emulator.Battle.MonsterTable[index].DF;
            MDF = emulator.Battle.MonsterTable[index].MDF;
            SPD = emulator.Battle.MonsterTable[index].SPD;
            Turn = emulator.Battle.MonsterTable[index].Turn;
        }
    }
}
