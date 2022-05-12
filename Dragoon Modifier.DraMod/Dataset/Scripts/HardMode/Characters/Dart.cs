using Dragoon_Modifier.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Dataset.Scripts.HardMode.Characters {
    internal class Dart : ICharacter {
        private const ushort _DAT = 281;
        private const ushort _specialDAT = 422;
        private const ushort _flameshot = 255;
        private const ushort _explosion = 340;
        private const ushort _finalBurst = 255;
        private const ushort _redEyeDragon = 340;

        private const ushort _divineDAT = 204;
        private const ushort _divineSpecialDAT = 306;
        private const ushort _divineBall = 255;
        private const ushort _divineCannon = 255;

        private const ushort _divineRedEyeDAT = 408;
        private const ushort _divineRedEyeSpecialDAT = 612;
        private const ushort _explosionDivineRedEye = 1020;
        private const ushort _finalBurstDivineRedEye = 510;

        private bool DivineDragoon = false;
        private bool DivineRedEye = false;
        private byte BurnStacks = 0;

        private ushort currentMP = 100;
        private ushort previousMP = 100;
        private double multi = 1;

        public void Run(byte slot, byte dragoonSpecial) {
            var battleTable = Emulator.Memory.Battle.CharacterTable[slot];
            // Let's also ignore DragonBlockStuff for now.

            DragoonAttack(battleTable, dragoonSpecial);

            currentMP = battleTable.MP;
            if (currentMP < previousMP) {
                byte spell = Emulator.Memory.Battle.CharacterTable[slot].SpellCast;

                if (DivineRedEye) {
                    DivineRedEyeSpells(battleTable, spell);
                } else {
                    RegularSpells(battleTable, spell);
                }
                battleTable.SpellCast = 255;
            }
            previousMP = currentMP;
        }

        private void DragoonAttack(Core.Memory.Battle.Character battleTable, byte dragoonSpecial) {
            if (dragoonSpecial == 0 || dragoonSpecial == 9) {
                if (DivineDragoon) {
                    battleTable.DAT = (ushort) (_divineSpecialDAT * multi);
                    return;
                }

                if (DivineRedEye) {
                    battleTable.DAT = (ushort) (_divineRedEyeSpecialDAT * multi);
                }

                // if Burn Stacks


                battleTable.DAT = (ushort) (_specialDAT * multi);
                return;
            }

            if (DivineDragoon) {
                battleTable.DAT = (ushort) (_divineDAT * multi);
                return;
            }

            if (DivineRedEye) {
                battleTable.DAT = (ushort) (_divineRedEyeDAT * multi);
                return;
            }

            // if Burn Stacks

            battleTable.DAT = (ushort) (_DAT * multi);
        }

        private void RegularSpells(Core.Memory.Battle.Character battleTable, byte spell) {
            switch (spell) {
                case 0: // Flameshot
                    battleTable.DMAT = (ushort) (_flameshot * multi);
                    // Burn Stacks
                    break;
                case 1: // Explosion
                    battleTable.DMAT = (ushort) (_explosion * multi);
                    // Burn Stacks
                    break;
                case 2: // Final Burst
                    battleTable.DMAT = (ushort) (_finalBurst * multi);
                    // Burn Stacks
                    break;
                case 3: // Red Eye Dragon
                    battleTable.DMAT = (ushort) (_redEyeDragon * multi);
                    break;
                case 4: // Divine Dragon Cannon
                    battleTable.DMAT = (ushort) (_divineCannon * multi);
                    break;
                case 9: // Divine Dragon Ball
                    battleTable.DMAT = (ushort) (_divineBall * multi);
                    break;

            }
        }

        private void DivineRedEyeSpells(Core.Memory.Battle.Character battleTable, byte spell) {
            if (spell == 1) {
                battleTable.DMAT = (ushort) (_explosionDivineRedEye * multi);
                return;
            }
            battleTable.DMAT = (ushort) (_finalBurstDivineRedEye * multi);
        }
    }
}
