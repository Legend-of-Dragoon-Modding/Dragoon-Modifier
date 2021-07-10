using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.UI {
    public class CharacterUpdate {
        public ushort HP;
        public ushort MaxHP;
        public ushort MP;
        public ushort MaxMP;
        public ushort AT;
        public ushort MAT;
        public ushort DF;
        public ushort MDF;
        public short A_HIT;
        public short M_HIT;
        public short A_AV;
        public short M_AV;
        public ushort DAT;
        public ushort DMAT;
        public ushort DDF;
        public ushort DMDF;
        public ushort SPD;
        public ushort SP;
        public ushort Turn;

        internal CharacterUpdate(Emulator.IEmulator emulator, int index) {
            HP = emulator.Battle.CharacterTable[index].HP;
            MaxHP = emulator.Battle.CharacterTable[index].MaxHP;
            MP = emulator.Battle.CharacterTable[index].MP;
            MaxMP = emulator.Battle.CharacterTable[index].MaxMP;
            AT = emulator.Battle.CharacterTable[index].AT;
            MAT = emulator.Battle.CharacterTable[index].MAT;
            DF = emulator.Battle.CharacterTable[index].DF;
            MDF = emulator.Battle.CharacterTable[index].MDF;
            A_HIT = emulator.Battle.CharacterTable[index].A_HIT;
            M_HIT = emulator.Battle.CharacterTable[index].M_HIT;
            A_AV = emulator.Battle.CharacterTable[index].A_AV;
            M_AV = emulator.Battle.CharacterTable[index].M_AV;
            DAT = emulator.Battle.CharacterTable[index].DAT;
            DMAT = emulator.Battle.CharacterTable[index].DMAT;
            DDF = emulator.Battle.CharacterTable[index].DDF;
            DMDF = emulator.Battle.CharacterTable[index].DMDF;
            SPD = emulator.Battle.CharacterTable[index].SPD;
            SP = emulator.Battle.CharacterTable[index].SP;
            Turn = emulator.Battle.CharacterTable[index].Turn;
        }
    }
}
