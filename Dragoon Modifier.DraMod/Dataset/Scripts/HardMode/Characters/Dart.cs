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
        private const ushort _DDF = 180;
        private const ushort _DMDF = 180;

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


        private const int _burnStackFlameshot = 1;
        private const int _burnStackExplosion = 2;
        private const int _burnStackFinalBurst = 3;
        private const int _burnStackRedEye = 4;

        private bool divineDragoon = false;
        private bool divineRedEye = false;
        private int dlv = 0;

        private int burnStacks = 0;
        private int previousBurnStacks = 0;
        private int _maxBurnStacks = 12;
        private bool burnMPHeal = false;

        private ushort currentMP = 100;
        private ushort previousMP = 100;
        private double multi = 1;

        public void Run(byte slot, byte dragoonSpecial) {
            var battleTable = Emulator.Memory.Battle.CharacterTable[slot];
            // Let's also ignore DragonBlockStuff for now.

            if (battleTable.Action == 10) { //Active 
                DragoonAttack(battleTable, dragoonSpecial);
                Spells(battleTable);
            }            

            if (battleTable.Action == 2) { //Idle
                DragoonDefence(battleTable);
            }

            if (battleTable.Action == 26 && burnMPHeal) {
                BurnStacksHealMP(battleTable);
            }
        }

        public void BattleSetup(byte slot) {
            var battleTable = Emulator.Memory.Battle.CharacterTable[slot];
            divineDragoon = Emulator.Memory.DragoonSpirits > 254 ? true : false;
            dlv = battleTable.DLV;
            burnStacks = previousBurnStacks = 0;
            _maxBurnStacks = dlv == 1 ? 3 : dlv == 2 ? 6 : dlv == 3 ? 9 : 12;
            burnMPHeal = false;
        }

        private void DragoonAttack(Core.Memory.Battle.Character battleTable, byte dragoonSpecial) {
            if (dragoonSpecial == 0 || dragoonSpecial == 9) {
                if (divineDragoon) {
                    battleTable.DAT = (ushort) (_divineSpecialDAT * multi);
                    return;
                }

                if (divineRedEye) {
                    battleTable.DAT = (ushort) (_divineRedEyeSpecialDAT * multi);
                }

                battleTable.DAT = (ushort) (_specialDAT * multi);
                return;
            }

            if (divineDragoon) {
                battleTable.DAT = (ushort) (_divineDAT * multi);
                return;
            }

            if (divineRedEye) {
                battleTable.DAT = (ushort) (_divineRedEyeDAT * multi);
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
                if (divineRedEye) {
                    DivineRedEyeSpells(battleTable, spell);
                } else {
                    RegularSpells(battleTable, spell);
                }
                battleTable.SpellCast = 255;
            }
            previousMP = currentMP;

        }

        private void RegularSpells(Core.Memory.Battle.Character battleTable, byte spell) {
            switch (spell) {
                case 0: // Flameshot
                    battleTable.DMAT = (ushort) (_flameshot * multi);
                    AddBurnStacks(_burnStackFlameshot);
                    break;
                case 1: // Explosion
                    battleTable.DMAT = (ushort) (_explosion * multi);
                    AddBurnStacks(_burnStackExplosion);
                    break;
                case 2: // Final Burst
                    battleTable.DMAT = (ushort) (_finalBurst * multi);
                    AddBurnStacks(_burnStackFinalBurst);
                    break;
                case 3: // Red Eye Dragon
                    battleTable.DMAT = (ushort) (_redEyeDragon * multi);
                    AddBurnStacks(_burnStackRedEye);
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

        private void AddBurnStacks(int stacks) {
            previousBurnStacks = burnStacks;
            burnStacks = Math.Min(_maxBurnStacks, burnStacks + stacks);
            if (burnStacks >= 4 && previousBurnStacks < 4) {
                burnMPHeal = true;
            } else if (burnStacks >= 8 && previousBurnStacks < 8) {
                burnMPHeal = true;
            } else if (burnStacks >= 12 && previousBurnStacks < 12) {
                burnMPHeal = true;
            }

            Constants.UIControl.WriteGLog("Burn Stacks: " + burnStacks + " / " + _maxBurnStacks);
        }

        private void BurnStacksHealMP(Core.Memory.Battle.Character battleTable) {
            if (burnStacks >= 4 && previousBurnStacks < 4) {
                battleTable.MP = (ushort) Math.Min(battleTable.MaxMP, battleTable.MP + 10);
            } else if (burnStacks >= 8 && previousBurnStacks < 8) {
                battleTable.MP = (ushort) Math.Min(battleTable.MaxMP, battleTable.MP + 20);
            } else if (burnStacks >= 12 && previousBurnStacks < 12) {
                battleTable.MP = (ushort) Math.Min(battleTable.MaxMP, battleTable.MP + 30);
            }
            burnMPHeal = false;
        }
    }
}
