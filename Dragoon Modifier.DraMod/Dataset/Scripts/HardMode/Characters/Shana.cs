using Dragoon_Modifier.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Dataset.Scripts.HardMode.Characters {
    internal class Shana : ICharacter {
        private const ushort _DAT = 365;
        private const ushort _specialDAT = 510;

        private const byte _moonLightMP = 20;
        private const ushort _starChildren = 332;
        private const byte _gatesOfHeavenMP = 40;
        private const ushort _wSilverDragon = 289;
        private const int _gatesOfHeavenHeal = 50;
        private const int _gatesOfHeavenHealBase = 50;
        private const int _gatesOfHeavenHellModePenalty = 15;
        private const int _gatesOfHeavenHardModePenalty = 8;

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
            if (dragoonSpecial == 2 || dragoonSpecial == 8) {
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
                    case 10:
                    case 65:
                        battleTable.DMAT = (ushort) (_starChildren * multi);
                        break;
                    case 13:
                        battleTable.DMAT = (ushort) (_wSilverDragon * multi);
                        break;
                    

                }
                battleTable.SpellCast = 255;
            }
            previousMP = currentMP;
        }
    }
}
