using Dragoon_Modifier.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.UI {
    public sealed class MonsterUpdate {
        public string Name;
        public int HP;
        public int MaxHP;
        public ushort AT;
        public ushort MAT;
        public ushort DF;
        public ushort MDF;
        public ushort SPD;
        public ushort Turn;

        internal MonsterUpdate(int index) {
            Name = Emulator.Memory.Battle.MonsterTable[index].Name.Substring(0, Emulator.Memory.Battle.MonsterTable[index].Name.Length - 5);
            HP = Emulator.Memory.Battle.MonsterTable[index].HP;
            MaxHP = Emulator.Memory.Battle.MonsterTable[index].MaxHP;
            AT = Emulator.Memory.Battle.MonsterTable[index].AT;
            MAT = Emulator.Memory.Battle.MonsterTable[index].MAT;
            DF = Emulator.Memory.Battle.MonsterTable[index].DF;
            MDF = Emulator.Memory.Battle.MonsterTable[index].MDF;
            SPD = Emulator.Memory.Battle.MonsterTable[index].SPD;
            Turn = Emulator.Memory.Battle.MonsterTable[index].Turn;
        }
    }
}
