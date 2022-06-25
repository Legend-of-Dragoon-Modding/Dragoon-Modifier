using Dragoon_Modifier.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Dataset.Scripts.HardMode.Characters {
    internal class Haschel : ICharacter {
        private const ushort _DAT = 281;
        private const ushort _specialDAT = 422;

        private const ushort _atomicMind = 330;
        private const ushort _thunderKid = 330;
        private const ushort _thunderGod = 330;
        private const ushort _violetDragon = 374;

        private ushort currentMP = 100;
        private ushort previousMP = 100;
        private double multi = 1;

        public void Run(byte slot, byte dragoonSpecial) {
            multi = 1;


            var battleTable = Emulator.Memory.Battle.CharacterTable[slot];

            if (battleTable.Action == 10) {
                DragoonAttack(battleTable, dragoonSpecial);

                Spells(battleTable);
            }
        }

        private void DragoonAttack(Core.Memory.Battle.Character battleTable, byte dragoonSpecial) {
            if (dragoonSpecial == 4) {
                battleTable.DAT = (ushort) (_specialDAT * multi);
                return;
            }

            battleTable.DAT = (ushort) (_DAT * multi);
        }

        private void Spells(Core.Memory.Battle.Character battleTable) {
            var spell = battleTable.SpellCast;
            currentMP = battleTable.MP;

            if (currentMP < previousMP) {
                switch (spell) {
                    case 20:
                        battleTable.DMAT = _atomicMind;
                        break;
                    case 21:
                        battleTable.DMAT = _thunderKid;
                        break;
                    case 22:
                        battleTable.DMAT = _thunderGod;
                        break;
                    case 23:
                        battleTable.DMAT = _violetDragon;
                        break;
                }
                battleTable.SpellCast = 255;
            }
            previousMP = currentMP;
        }
    }
}
