using Dragoon_Modifier.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Dataset.Scripts.HardMode.Characters {
    internal class Meru : ICharacter {
        private const ushort _DAT = 330;
        private const ushort _specialDAT = 495;
        private const ushort _DDF = 180;
        private const ushort _DMDF = 180;

        private const ushort _freezingRing = 255;
        private const ushort _diamondDust = 264;
        private const ushort _blueSeaDragon = 350;

        private const ushort _enhancedFreezingRing = 400;
        private const ushort _enhancedDiamondDust = 440;
        private const ushort _enhancedBlueSeaDragon = 525;

        private ushort currentMP = 100;
        private ushort previousMP = 100;
        private double multi = 1;

        private bool _enhancedDragoon = false;
        private bool _trackRainbowBreath = false;

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
            if (dragoonSpecial == 6) {
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
                    case 24:
                        battleTable.DMAT = (ushort) ((_enhancedDragoon ? _freezingRing : _enhancedFreezingRing ) * multi);
                        break;
                    case 25:
                        if (_enhancedDragoon) {
                            _trackRainbowBreath = true;
                        }
                        break;
                    case 27:
                        battleTable.DMAT = (ushort) ((_enhancedDragoon ? _diamondDust : _enhancedDiamondDust) * multi);
                        break;
                    case 28:
                        battleTable.DMAT = (ushort) ((_enhancedDragoon ? _blueSeaDragon : _enhancedBlueSeaDragon) * multi);
                        break;
                }
                battleTable.SpellCast = 255;
            }
            previousMP = currentMP;
        }
    }
}
