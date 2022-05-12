using Dragoon_Modifier.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Dataset.Scripts.HardMode.Characters {
    internal class Lavitz : ICharacter {
        private const ushort _DAT = 400;
        private const ushort _specialDAT = 600;

        private const ushort _wingBlasterDMAT = 440;
        private const byte _blossomStormTurnMP = 20;
        private const ushort _gasplessDMAT = 330;
        private const ushort _jadeDragonDMAT = 440;
        private const byte _wingBlasterTPDamage = 30;
        private const byte _gasplessTPDamage = 90;
        private const byte _jadeDragonTPDamage = 60;

        private ushort currentMP = 100;
        private ushort previousMP = 100;
        private double multi = 1;

        private bool _harpoonCheck = false;

        public void Run(byte slot, byte dragoonSpecial) {
            multi = 1;

            if (_harpoonCheck) {
                multi *= 3;
            }

            var battleTable = Emulator.Memory.Battle.CharacterTable[slot];
            DragoonAttack(battleTable, dragoonSpecial);
        }



        private void DragoonAttack(Core.Memory.Battle.Character battleTable, byte dragoonSpecial) {
            if (dragoonSpecial == 1 || dragoonSpecial == 5) {
                battleTable.DAT = (ushort) (_specialDAT * multi);
                return;
            }

            battleTable.DAT = (ushort) (_DAT * multi);
        }
    }
}
