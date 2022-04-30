using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.LoDDict.Scripts.HardMode.Characters {
    internal class Dart : ICharacter {
        private static readonly ushort DAT = 281;
        private static readonly ushort SpecialDAT = 422;
        private static readonly ushort flameshot = 255;
        private static readonly ushort explosion = 340;
        private static readonly ushort finalBurst = 255;
        private static readonly ushort redEyeDragon = 340;

        private static readonly ushort DivineDAT = 204;
        private static readonly ushort DivineSpecialDAT = 306;
        private static readonly ushort DivineBall = 255;
        private static readonly ushort DivineCannon = 255;

        private static readonly ushort DivineRedEyeDAT = 408;
        private static readonly ushort DivineRedEyeSpecialDAT = 612;
        private static readonly ushort explosionDivineRedEye = 1020;
        private static readonly ushort finalBurstDivineRedEye = 510;

        private bool DivineDragoon = false;
        private bool DivineRedEye = false;
        private byte BurnStacks = 0;

        private ushort currentMP = 100;
        private ushort previousMP = 100;
        private double multi = 1;

        public void Run(Emulator.IEmulator emulator, byte slot, byte dragoonSpecialAttack) {
            var battleTable = emulator.Battle.CharacterTable[slot];
            // Let's also ignore DragonBlockStuff for now.

            DragoonAttack(battleTable, dragoonSpecialAttack);

            currentMP = battleTable.MP;
            if (currentMP < previousMP) {
                byte spell = emulator.Battle.CharacterTable[slot].SpellCast;

                if (DivineRedEye) {
                    DivineRedEyeSpells(battleTable, spell);
                } else {
                    RegularSpells(battleTable, spell);
                }
                battleTable.SpellCast = 255;
            }
            previousMP = currentMP;
        }

        private void DragoonAttack(Emulator.Memory.Battle.Character battleTable, byte dragoonSpecialAttack) {
            if (dragoonSpecialAttack == 0 || dragoonSpecialAttack == 9) {
                if (DivineDragoon) {
                    battleTable.DAT = (ushort) (DivineSpecialDAT * multi);
                    return;
                }

                if (DivineRedEye) {
                    battleTable.DAT = (ushort) (DivineRedEyeSpecialDAT * multi);
                }

                // if Burn Stacks


                battleTable.DAT = (ushort) (SpecialDAT * multi);
                return;
            }

            if (DivineDragoon) {
                battleTable.DAT = (ushort) (DivineDAT * multi);
                return;
            }

            if (DivineRedEye) {
                battleTable.DAT = (ushort) (DivineRedEyeDAT * multi);
                return;
            }

            // if Burn Stacks

            battleTable.DAT = (ushort) (DAT * multi);
        }

        private void RegularSpells(Emulator.Memory.Battle.Character battleTable, byte spell) {
            switch (spell) {
                case 0: // Flameshot
                    battleTable.DMAT = (ushort) (flameshot * multi);
                    // Burn Stacks
                    break;
                case 1: // Explosion
                    battleTable.DMAT = (ushort) (explosion * multi);
                    // Burn Stacks
                    break;
                case 2: // Final Burst
                    battleTable.DMAT = (ushort) (finalBurst * multi);
                    // Burn Stacks
                    break;
                case 3: // Red Eye Dragon
                    battleTable.DMAT = (ushort) (redEyeDragon * multi);
                    break;
                case 4: // Divine Dragon Cannon
                    battleTable.DMAT = (ushort) (DivineCannon * multi);
                    break;
                case 9: // Divine Dragon Ball
                    battleTable.DMAT = (ushort) (DivineBall * multi);
                    break;

            }
        }

        private void DivineRedEyeSpells(Emulator.Memory.Battle.Character battleTable, byte spell) {
            if (spell == 1) {
                battleTable.DMAT = (ushort) (explosionDivineRedEye * multi);
                return;
            }
            battleTable.DMAT = (ushort) (finalBurstDivineRedEye * multi);
        }
    }
}
