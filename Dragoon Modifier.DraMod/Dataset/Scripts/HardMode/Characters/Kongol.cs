using Dragoon_Modifier.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Dataset.Scripts.HardMode.Characters {
    internal class Kongol : ICharacter {
        private const ushort _DAT = 500;
        private const ushort _specialDAT = 600;
        private const ushort _DDF = 70;
        private const ushort _DMDF = 100;

        private const ushort _grandStream = 450;
        private const ushort _meteorStrike = 560;
        private const ushort _goldDragon = 740;

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

            if (battleTable.Action == 2) {
                DragoonDefence(battleTable);
            }
        }

        public void BattleSetup(byte slot) {
            var battleTable = Emulator.Memory.Battle.CharacterTable[slot];
        }

        private void DragoonAttack(Core.Memory.Battle.Character battleTable, byte dragoonSpecial) {
            if (dragoonSpecial == 7) {
                battleTable.DAT = (ushort) (_specialDAT * multi);
                return;
            }

            battleTable.DAT = (ushort) (_DAT * multi);
        }

        private void DragoonDefence(Core.Memory.Battle.Character battleTable) {
            battleTable.DDF = (ushort) (_DDF * multi);
            battleTable.DMDF = (ushort) (_DMDF * multi);
        }

        private void Spells(Core.Memory.Battle.Character battleTable) {
            var spell = battleTable.SpellCast;
            currentMP = battleTable.MP;

            if (currentMP < previousMP) {
                switch (spell) {
                    case 29:
                        battleTable.DMAT = _grandStream;
                        break;
                    case 30:
                        battleTable.DMAT = _meteorStrike;
                        break;
                    case 31:
                        battleTable.DMAT = _goldDragon;
                        break;
                }
                battleTable.SpellCast = 255;
            }
            previousMP = currentMP;
        }
    }
}
