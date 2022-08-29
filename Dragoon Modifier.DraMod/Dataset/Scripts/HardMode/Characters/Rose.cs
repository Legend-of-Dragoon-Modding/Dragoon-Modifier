using Dragoon_Modifier.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Dataset.Scripts.HardMode.Characters {
    internal class Rose : ICharacter {
        private const ushort _DAT = 365;
        private const ushort _specialDAT = 510;
        private const ushort _DDF = 180;
        private const ushort _DMDF = 180;

        private const ushort _astralDrain = 295;
        private const double _astralDrainHeal = 0.05;
        private const ushort _deathDimension = 395;
        private const ushort _darkDragon = 420;

        private const ushort _enhancedAstralDrain = 410;
        private const double _enhancedAstralDrainHeal = 0.04;
        private const ushort _enhancedDeathDimension = 790;
        private const ushort _enhancedDarkDragon = 290;

        private ushort currentMP = 100;
        private ushort previousMP = 100;
        private double multi = 1;

        private bool _enhancedDragoon = false;
        private bool _checkRoseDamage = false;
        private ushort _roseDamageSave = 0;


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
            if (dragoonSpecial == 3) {
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
                    case 15:
                        battleTable.DMAT = (ushort) ((_enhancedDragoon ? _astralDrain : _enhancedAstralDrain) * multi);
                        for (int character = 0; character < Emulator.Memory.Battle.CharacterTable.Length; character++) {
                            var boostedHP = Emulator.Memory.Battle.CharacterTable[character].HP * (1 + Emulator.Memory.CharacterTable[3].DragoonLevel * (_enhancedDragoon ? _astralDrainHeal : _enhancedAstralDrainHeal));
                            Emulator.Memory.CharacterTable[character].HP = (ushort) Math.Min(Emulator.Memory.Battle.CharacterTable[character].MaxHP, boostedHP);
                        }
                        break;
                    case 16:
                        battleTable.DMAT = (ushort) ((_enhancedDragoon ? _deathDimension : _enhancedDeathDimension) * multi);
                        break;
                    case 19:
                        battleTable.DMAT = (ushort) ((_enhancedDragoon ? _darkDragon : _enhancedDarkDragon) * multi);
                        // Check Damage
                        break;
                }
                battleTable.SpellCast = 255;
            }
            previousMP = currentMP;
        }
    }
}
