using Dragoon_Modifier.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Dataset.Scripts.HardMode {
    internal class Script : IScript {
        private Characters.ICharacter[] _character = new Characters.ICharacter[9] {
            new Characters.Dart(),
            new Characters.Lavitz(),
            new Characters.Dart(), // Placeholder
            new Characters.Dart(), // Placeholder
            new Characters.Dart(), // Placeholder
            new Characters.Lavitz(),
            new Characters.Dart(), // Placeholder
            new Characters.Dart(), // Placeholder
            new Characters.Dart() // Placeholder
        };


        public void BattleRun() {
            var dragoonSpecial = Emulator.Memory.Battle.DragoonSpecial;
            for (byte slot = 0; slot < Emulator.Memory.Battle.CharacterTable.Length; slot++) {
                var character = Emulator.Memory.Battle.CharacterTable[slot];
                _character[character.ID].Run(slot, dragoonSpecial);
            }
        }

        public void BattleSetup() {

        }

        public void FieldRun() {

        }

        public void FieldSetup() {

        }
    }
}
