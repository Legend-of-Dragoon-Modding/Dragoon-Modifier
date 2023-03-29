using Dragoon_Modifier.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.UI {
    public sealed class CharacterUpdate {
        private static readonly string[] _characterNames = new string[] { "Dart", "Lavitz", "Shana", "Rose", "Haschel", "Albert", "Meru", "Kongol", "Miranda" };

        public string Name;
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

        internal CharacterUpdate(int index) {
            Name = _characterNames[Emulator.Memory.PartySlot[index]];
            HP = Emulator.Memory.Battle.CharacterTable[index].HP;
            MaxHP = Emulator.Memory.Battle.CharacterTable[index].MaxHP;
            MP = Emulator.Memory.Battle.CharacterTable[index].MP;
            MaxMP = Emulator.Memory.Battle.CharacterTable[index].MaxMP;
            AT = Emulator.Memory.Battle.CharacterTable[index].AT;
            MAT = Emulator.Memory.Battle.CharacterTable[index].MAT;
            DF = Emulator.Memory.Battle.CharacterTable[index].DF;
            MDF = Emulator.Memory.Battle.CharacterTable[index].MDF;
            A_HIT = Emulator.Memory.Battle.CharacterTable[index].A_HIT;
            M_HIT = Emulator.Memory.Battle.CharacterTable[index].M_HIT;
            A_AV = Emulator.Memory.Battle.CharacterTable[index].A_AV;
            M_AV = Emulator.Memory.Battle.CharacterTable[index].M_AV;
            DAT = Emulator.Memory.Battle.CharacterTable[index].DAT;
            DMAT = Emulator.Memory.Battle.CharacterTable[index].DMAT;
            DDF = Emulator.Memory.Battle.CharacterTable[index].DDF;
            DMDF = Emulator.Memory.Battle.CharacterTable[index].DMDF;
            SPD = Emulator.Memory.Battle.CharacterTable[index].SPD;
            SP = Emulator.Memory.Battle.CharacterTable[index].SP;
            Turn = Emulator.Memory.Battle.CharacterTable[index].Turn;
        }
    }
}
