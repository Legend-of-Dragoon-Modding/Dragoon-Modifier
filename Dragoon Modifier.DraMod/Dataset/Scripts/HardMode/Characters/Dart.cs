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
        private const double _damagePerBurn = 0.1;

        private const ushort _flameshot = 255;
        private const ushort _explosion = 3400;
        private const ushort _finalBurst = 255;
        private const ushort _redEyeDragon = 340;

        private const byte _flameshotMP = 10;
        private const byte _explosionMP = 20;
        private const byte _finalBurstMP = 30;
        private const byte _redEyeMP = 80;

        private const ushort _divineDAT = 204;
        private const ushort _divineSpecialDAT = 306;
        private const ushort _divineBall = 255;
        private const ushort _divineCannon = 255;

        private const ushort _divineRedEyeDAT = 408;
        private const ushort _divineRedEyeSpecialDAT = 612;
        private const ushort _explosionDivineRedEye = 1020;
        private const ushort _finalBurstDivineRedEye = 510;

        private const double _burnStackAdditionMulti = 1.4;
        private const double _burnStackFlameshotMulti = 1.5;
        private const double _burnStackExplosionMulti = 1.5;
        private const double _burnStackFinalBurstMulti = 1.33;
        private const double _burnStackRedEyeMulti = 0.75;
        private const double _burnStackMaxAdditionMulti = 1.6;
        private const double _burnStackMaxFlameshotMulti = 2.0;
        private const double _burnStackMaxExplosionMulti = 2.25;
        private const double _burnStackMaxFinalBurstMulti = 1.33;
        private const double _burnStackMaxRedEyeMulti = 0.75;

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
        private bool burnStackSelected = false;

        private ushort currentMP = 100;
        private ushort previousMP = 100;
        private double multi = 1;

        private bool dragoonGuard = false;

        public void Run(byte slot, byte dragoonSpecial) {
            var battleTable = Emulator.Memory.Battle.CharacterTable[slot];
            
            if (Emulator.Memory.Battle.DragonBlockStaff == 1) {
                multi = 8;
            } else {
                multi = 1;
            }

            if (battleTable.Action == 10) { //Active 
                DragoonAttack(battleTable, dragoonSpecial);
                Spells(battleTable);
                BurnMenu(battleTable);
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
            burnMPHeal = burnStackSelected  = false;
        }

        private void DragoonAttack(Core.Memory.Battle.Character battleTable, byte dragoonSpecial) {
            double tMulti = multi;
            if (burnStackSelected) {
                tMulti *= 1 + (burnStacks * _damagePerBurn * (burnStacks == _maxBurnStacks ? _burnStackMaxAdditionMulti : _burnStackAdditionMulti));
            }

            if (dragoonSpecial == 0 || dragoonSpecial == 9) {
                if (divineDragoon) {
                    battleTable.DAT = (ushort) (_divineSpecialDAT * multi);
                    return;
                }

                if (divineRedEye) {
                    battleTable.DAT = (ushort) (_divineRedEyeSpecialDAT * multi);
                    return;
                }

                battleTable.DAT = (ushort) (_specialDAT * tMulti);
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

            battleTable.DAT = (ushort) (_DAT * tMulti);
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

                    if (burnStackSelected) {
                        multi *= 1 + (burnStacks * _damagePerBurn * (burnStacks == _maxBurnStacks ? _burnStackMaxAdditionMulti : _burnStackAdditionMulti));
                    }

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

        private void BurnMenu(Core.Memory.Battle.Character battleTable) {
            if (burnStacks > 0) {
                byte menu = 0;
                ushort icons = (ushort) (dragoonGuard ? 5 : 4);
                int iconSelected = Emulator.Memory.Battle.IconSelected;

               if (battleTable.Menu == 96 || battleTable.Menu == 31 || battleTable.Menu == 15) {
                    for (byte i = 0; i < icons; i++) {
                        menu += (byte) Math.Pow(2, i);
                    }

                    battleTable.Menu = menu;
                    
                    Emulator.Memory.Battle.BattleMenuCount = icons;

                    if (dragoonGuard) {
                        Emulator.Memory.Battle.BattleMenuSlot[0] = 9;
                        Emulator.Memory.Battle.BattleMenuSlot[1] = 9;
                        Emulator.Memory.Battle.BattleMenuSlot[2] = 3;
                        Emulator.Memory.Battle.BattleMenuSlot[3] = 3;
                        Emulator.Memory.Battle.BattleMenuSlot[4] = 8;
                    } else {
                        Emulator.Memory.Battle.BattleMenuSlot[0] = 9;
                        Emulator.Memory.Battle.BattleMenuSlot[1] = 9;
                        Emulator.Memory.Battle.BattleMenuSlot[2] = 3;
                        Emulator.Memory.Battle.BattleMenuSlot[3] = 3;
                    }
                }

                
                Dictionary<int, IDragoonSpells> datasetSpell = Settings.Instance.Dataset.DragoonSpell;
                bool max = burnStacks == _maxBurnStacks;

                if (iconSelected == 1 || iconSelected == 3) {
                    burnStackSelected = true;
                    double burnAmount = 1 + (burnStacks * _damagePerBurn);
                    string flameshotDesc = Convert.ToString(max ? (burnAmount * _burnStackMaxFlameshotMulti) : (burnAmount * _burnStackFlameshotMulti));
                    string explosionDesc = Convert.ToString(max ? (burnAmount * _burnStackMaxExplosionMulti) : (burnAmount * _burnStackExplosionMulti));
                    string finalBurstDesc = Convert.ToString(max ? (burnAmount * _burnStackMaxFinalBurstMulti) : (burnAmount * _burnStackFinalBurstMulti));
                    string redEyeDesc = Convert.ToString(max ? (burnAmount * _burnStackMaxRedEyeMulti) : (burnAmount * _burnStackRedEyeMulti));
                    
                    if (Emulator.Region == Region.NTA) { // TODO Remove Region check, when other encoding tables work.
                        Emulator.DirectAccess.WriteText(datasetSpell[0].Description_Pointer + 0x1E, flameshotDesc.Substring(0, Math.Min(4, flameshotDesc.Length)));
                        Emulator.DirectAccess.WriteText(datasetSpell[1].Description_Pointer + 0x1E, explosionDesc.Substring(0, Math.Min(4, explosionDesc.Length)));
                        Emulator.DirectAccess.WriteText(datasetSpell[2].Description_Pointer + 0x1E, finalBurstDesc.Substring(0, Math.Min(4, finalBurstDesc.Length)));
                        Emulator.DirectAccess.WriteText(datasetSpell[3].Description_Pointer + 0x20, redEyeDesc.Substring(0, Math.Min(4, redEyeDesc.Length)));
                    }
                    
                    if (max) {
                        datasetSpell[0].MP = 0;
                        datasetSpell[1].MP = 0;
                        datasetSpell[2].MP = 0;
                        datasetSpell[3].MP = 0;
                    }
                } else {
                    burnStackSelected = false;
                    if (Emulator.Region == Region.NTA) { // TODO Remove Region check, when other encoding tables work.
                        Emulator.DirectAccess.WriteText(datasetSpell[0].Description_Pointer + 0x1E, "1.00");
                        Emulator.DirectAccess.WriteText(datasetSpell[1].Description_Pointer + 0x1E, "1.00");
                        Emulator.DirectAccess.WriteText(datasetSpell[2].Description_Pointer + 0x1E, "1.00");
                        Emulator.DirectAccess.WriteText(datasetSpell[3].Description_Pointer + 0x20, "1.00");
                    }

                    if (max) {
                        datasetSpell[0].MP = _flameshotMP;
                        datasetSpell[1].MP = _explosionMP;
                        datasetSpell[2].MP = _finalBurstMP;
                        datasetSpell[3].MP = _redEyeMP;
                    }
                }
            }
        }
    }
}
