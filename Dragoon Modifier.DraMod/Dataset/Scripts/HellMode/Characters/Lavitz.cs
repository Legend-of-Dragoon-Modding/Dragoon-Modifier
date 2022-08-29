using Dragoon_Modifier.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Dataset.Scripts.HellMode.Characters {
    internal class Lavitz : ICharacter {
        private const ushort _DAT = 400;
        private const ushort _specialDAT = 600;
        private const ushort _DDF = 180;
        private const ushort _DMDF = 180;

        private const ushort _wingBlaster = 440;
        private const byte _blossomStormTurnMP = 20;
        private const ushort _gaspless = 330;
        private const ushort _jadeDragon = 440;
        private const byte _wingBlasterTPDamage = 30;
        private const byte _gasplessTPDamage = 90;
        private const byte _jadeDragonTPDamage = 60;

        private ushort currentMP = 100;
        private ushort previousMP = 100;
        private double multi = 1;

        private bool _harpoonCheck = false;
        private bool _checkWingBlaster = false;
        private bool _checkFlowerStorm = false;
        private bool _checkGaspless = false;
        private bool _checkJadeDragon = false;

        public void Run(byte slot, byte dragoonSpecial) {
            multi = 1;

            if (_harpoonCheck) {
                multi *= 3;
            }

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
            if (dragoonSpecial == 1 || dragoonSpecial == 5) {
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
                    case 5:
                    case 14:
                        battleTable.DMAT = (ushort) (_wingBlaster * multi);
                        _checkWingBlaster = true;
                        // track monster HP
                        break;
                    case 6:
                    case 17:
                        battleTable.DMAT = (ushort) (_gaspless * multi);
                        _checkGaspless = true;
                        // track monster HP
                        break;
                    case 8:
                        battleTable.DMAT = (ushort) (_jadeDragon * multi);
                        _checkJadeDragon = true;
                        // track monster HP;
                        break;
                    case 7:
                    case 26:
                        _checkFlowerStorm = true;
                        for (int i = 0; i < Emulator.Memory.Battle.CharacterTable.Length; i++) {
                            var table = Emulator.Memory.Battle.CharacterTable[i];
                            if (table.HP > 0) {
                                table.PWR_DF_Turn = 0;
                                table.PWR_MDF_Turn = 0;
                            }
                        }
                        break;

                }
                battleTable.SpellCast = 255;
            }
            previousMP = currentMP;
        }
    }
}
