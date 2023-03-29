using Dragoon_Modifier.Core;
using Dragoon_Modifier.DraMod.Controller;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Dataset.Scripts.HellMode.Characters {
    internal class Dart : ICharacter {
        private const ushort _DAT = 281;
        private const ushort _specialDAT = 422;
        private const ushort _DDF = 180;
        private const ushort _DMDF = 180;
        private const double _damagePerBurn = 0.1;

        private const ushort _flameshot = 255;
        private const ushort _explosion = 340;
        private const ushort _finalBurst = 255;
        private const ushort _burnOut = 100;
        private const ushort _redEyeDragon = 340;
        private const ushort _fireAttack = 337;
        private const ushort _specialFireAttack = 505;

        private const byte _flameshotMP = 10;
        private const byte _explosionMP = 20;
        private const byte _finalBurstMP = 30;
        private const byte _burnOutMP = 1;
        private const byte _redEyeMP = 80;
        private const byte _fireAttackMP = 100;

        private const ushort _divineDAT = 204;
        private const ushort _divineSpecialDAT = 306;
        private const ushort _divineBall = 255;
        private const ushort _divineCannon = 255;

        private const ushort _divineRedEyeDAT = 408;
        private const ushort _divineRedEyeSpecialDAT = 612;
        private const ushort _explosionDivineRedEye = 1020;
        private const ushort _finalBurstDivineRedEye = 510;

        /*private const double _burnStackAdditionMulti = 1.4;
        private const double _burnStackFlameshotMulti = 1.5;
        private const double _burnStackExplosionMulti = 1.5;
        private const double _burnStackFinalBurstMulti = 1.33;
        private const double _burnStackRedEyeMulti = 0.75;*/
        private const double _burnStackMaxAdditionMulti = 1.4;
        private const double _burnStackMaxFlameshotMulti = 2.4;
        private const double _burnStackMaxExplosionMulti = 3;
        private const double _burnStackMaxFinalBurstMulti = 1.6;
        private const double _burnStackMaxRedEyeMulti = 1;

        private const int _burnStackFlameshot = 1;
        private const int _burnStackExplosion = 2;
        private const int _burnStackFinalBurst = 3;
        private const int _burnStackRedEye = 4;
        private const int _burnStackBurnOut = 1;

        private bool divineDragoon = false;
        private bool divineRedEye = false;
        private int dlv = 0;

        private int burnStacks = 0;
        private int previousBurnStacks = 0;
        private int _maxBurnStacks = 15;
        private bool burnMPHeal = false;
        private bool burnStackSelected = false;
        private byte dartPreviousAction = 0;
        private byte dartPreviousElement = 0;

        private ushort currentMP = 100;
        private ushort previousMP = 100;
        private double multi = 1;

        private bool dragoonGuard = false;
        private bool autoDragoon = false;
        private bool turnAutoOn = false;

        public void Run(byte slot, byte dragoonSpecial) {
            var battleTable = Emulator.Memory.Battle.CharacterTable[slot];

            if (Emulator.Memory.Battle.DragonBlockStaff == 1) {
                multi = 8;
            } else {
                multi = 1;
            }

            if (battleTable.Action == 10) { //Active 
                DragoonAttack(battleTable, dragoonSpecial, slot);
                Spells(battleTable, dragoonSpecial);
                BurnMenu(battleTable);
                NewSpells();
            }

            if (battleTable.Action == 2) { //Idle
                DragoonDefence(battleTable);
                battleTable.Weapon_Element = dartPreviousElement;
            }

            if (battleTable.Action == 26) {
                if (dartPreviousAction == 10 && battleTable.CastingSpell != 84)
                    AddBurnStacks(battleTable.DLV >= 5 ? 2 : 1);

                if (burnMPHeal)
                    BurnStacksHealMP(battleTable);

                if (!autoDragoon && battleTable.CastingSpell == 48 && turnAutoOn) {
                    turnAutoOn = false;
                    Task.Factory.StartNew(() => {
                        Thread.Sleep(5000);
                        Emulator.Memory.Battle.AutoDragoon1 = 4194;
                        //Emulator.Memory.Battle.AutoDragoon2 = 4160;
                    });
                }

            }

            dartPreviousAction = battleTable.Action;
            SpecialEquips.Run(slot);
        }

        public void BattleSetup(byte slot) {
            var battleTable = Emulator.Memory.Battle.CharacterTable[slot];
            divineDragoon = Emulator.Memory.DragoonSpirits > 254 ? true : false;
            dlv = battleTable.DLV;
            burnStacks = previousBurnStacks = 0;
            _maxBurnStacks = dlv == 1 ? 3 : dlv == 2 ? 6 : dlv == 3 ? 9 : dlv == 7 ? 15 : 12;
            burnMPHeal = burnStackSelected = dragoonGuard = turnAutoOn = false;
            if (Settings.Instance.UltimateBoss && Settings.Instance.UltimateBossSelected == 37)
                dragoonGuard = true;
            if (dlv == 7)
                dragoonGuard = true;
            Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "FF FF FF FF FF FF FF FF");

            switch (dlv) {
                case 1:
                    Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "00 FF FF FF FF FF FF FF");
                    break;
                case 2:
                    Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "00 01 FF FF FF FF FF FF");
                    break;
                case 3:
                    Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "00 01 02 FF FF FF FF FF");
                    break;
                case 4:
                    Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "00 01 02 20 FF FF FF FF");
                    break;
                case 5:
                case 6:
                    Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "00 01 02 20 03 FF FF FF");
                    break;
                case 7:
                    Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "00 01 02 20 03 21 FF FF");
                    break;
            }

            Core.Memory.Battle.DragoonSpell burnOut = new Core.Memory.Battle.DragoonSpell(32);
            Core.Memory.Battle.DragoonSpell fireAttack = new Core.Memory.Battle.DragoonSpell(33);
            burnOut.MP = _burnOutMP;
            fireAttack.MP = _fireAttackMP;
            SpecialEquips.Setup(slot);

        }

        private void DragoonAttack(Core.Memory.Battle.Character battleTable, byte dragoonSpecial, byte slot) {
            double tMulti = multi;

            if ((SpecialEquips.soasSiphonRingSlot & (1 << slot)) != 0) {
                multi *= 0.3;
            }

            if (burnStackSelected) {
                tMulti *= (1 + (burnStacks * _damagePerBurn)) * (dlv < 5 ? 1 : burnStacks == _maxBurnStacks ? _burnStackMaxAdditionMulti : 1);
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

        private void Spells(Core.Memory.Battle.Character battleTable, byte dragoonSpecial) {
            var spell = battleTable.SpellCast;
            currentMP = battleTable.MP;
            if (currentMP < previousMP) {
                if (divineRedEye) {
                    DivineRedEyeSpells(battleTable, spell);
                } else {
                    RegularSpells(battleTable, spell, dragoonSpecial);
                }
                battleTable.SpellCast = 255;
            }

            switch (battleTable.CastingSpell) {
                case 32:
                    battleTable.CastingSpell = 84;
                    break;
                case 33:
                    battleTable.CastingSpell = 48;
                    break;
                case 48: //Fire Attack
                    if (Emulator.Memory.ScreenFade == 3) {
                        Emulator.Memory.ScreenFade = 0;
                        dartPreviousElement = battleTable.Weapon_Element;
                        battleTable.Weapon_Element = 128;
                        Emulator.Memory.Battle.AutoDragoon1 = 4096;
                        //Emulator.Memory.Battle.AutoDragoon2 = 4096;
                        turnAutoOn = true;
                    }
                    break;
                case 84: //Burn Out
                    if (Emulator.Memory.ScreenFade == 3)
                        Emulator.Memory.ScreenFade = 0; 
                    break;
            }

            previousMP = currentMP;

        }

        private void RegularSpells(Core.Memory.Battle.Character battleTable, byte spell, byte dragoonSpecial) {
            double tMulti = multi;
            switch (spell) {
                case 0: // Flameshot
                    if (burnStackSelected) {
                        tMulti *= (1 + (burnStacks * _damagePerBurn)) * (dlv < 5 ? 1 : burnStacks == _maxBurnStacks ? _burnStackMaxFlameshotMulti : 1);
                    }

                    battleTable.DMAT = (ushort) (_flameshot * tMulti);
                    AddBurnStacks(_burnStackFlameshot);
                    break;
                case 1: // Explosion
                    if (burnStackSelected) {
                        tMulti *=  (1 + (burnStacks * _damagePerBurn)) * (dlv < 5 ? 1 : burnStacks == _maxBurnStacks ? _burnStackMaxExplosionMulti : 1);
                    }
                    battleTable.DMAT = (ushort) (_explosion * tMulti);
                    AddBurnStacks(_burnStackExplosion);
                    break;
                case 2: // Final Burst
                    if (burnStackSelected) {
                        tMulti *= (1 + (burnStacks * _damagePerBurn)) * (dlv < 5 ? 1 : burnStacks == _maxBurnStacks ? _burnStackMaxFinalBurstMulti : 1);
                    }
                    battleTable.DMAT = (ushort) (_finalBurst * tMulti);
                    AddBurnStacks(_burnStackFinalBurst);
                    break;
                case 3: // Red Eye Dragon
                    if (burnStackSelected) {
                        tMulti *= (1 + (burnStacks * _damagePerBurn)) * (dlv < 5 ? 1 : burnStacks == _maxBurnStacks ? _burnStackMaxRedEyeMulti : 1);
                    }
                    battleTable.DMAT = (ushort) (_redEyeDragon * tMulti);
                    AddBurnStacks(_burnStackRedEye);
                    break;
                case 4: // Divine Dragon Cannon
                    battleTable.DMAT = (ushort) (_divineCannon * multi);
                    break;
                case 9: // Divine Dragon Ball
                    battleTable.DMAT = (ushort) (_divineBall * multi);
                    break;
                case 32:
                case 84:
                    battleTable.DMAT = _burnOut;
                    battleTable.SP += 50;
                    battleTable.MP += _burnOutMP;

                    if (battleTable.SP % 100 == 0) {
                        battleTable.DragoonTurns += 1;
                    }
                    AddBurnStacks(_burnStackBurnOut);
                    break;
                case 33:
                case 48:
                    if (burnStackSelected) {
                        tMulti *= (1 + (burnStacks * _damagePerBurn)) * (dlv < 5 ? 1 : burnStacks == _maxBurnStacks ? _burnStackMaxAdditionMulti : 1);
                    }

                    if (dragoonSpecial == 0) {
                        battleTable.DAT = (ushort) (_specialFireAttack * tMulti);
                    } else {
                        battleTable.DAT = (ushort) (_fireAttack * tMulti);
                    }
                    break;
                default:
                    battleTable.DMAT = 100;
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
                Core.Memory.Battle.DragoonSpell burnOut = new Core.Memory.Battle.DragoonSpell(32);
                Core.Memory.Battle.DragoonSpell fireAttack = new Core.Memory.Battle.DragoonSpell(33);
                Core.Memory.Battle.DragoonSpell spell = new Core.Memory.Battle.DragoonSpell(0);

                if (iconSelected == 1 || iconSelected == 3) {
                    burnStackSelected = true;
                    double burnAmount = 1 + (burnStacks * _damagePerBurn);
                    string flameshotDesc = Convert.ToString(max ? (burnAmount * _burnStackMaxFlameshotMulti) : (burnAmount * 1));
                    string explosionDesc = Convert.ToString(max ? (burnAmount * _burnStackMaxExplosionMulti) : (burnAmount * 1));
                    string finalBurstDesc = Convert.ToString(max ? (burnAmount * _burnStackMaxFinalBurstMulti) : (burnAmount * 1));
                    string redEyeDesc = Convert.ToString(max ? (burnAmount * _burnStackMaxRedEyeMulti) : (burnAmount * 1));
                    
                    if (Emulator.Region == Region.NTA) { // TODO Remove Region check, when other encoding tables work.
                        Emulator.DirectAccess.WriteText(datasetSpell[0].Description_Pointer + 0x1E, flameshotDesc.Substring(0, Math.Min(4, flameshotDesc.Length)));
                        Emulator.DirectAccess.WriteText(datasetSpell[1].Description_Pointer + 0x1E, explosionDesc.Substring(0, Math.Min(4, explosionDesc.Length)));
                        Emulator.DirectAccess.WriteText(datasetSpell[2].Description_Pointer + 0x1E, finalBurstDesc.Substring(0, Math.Min(4, finalBurstDesc.Length)));
                        Emulator.DirectAccess.WriteText(datasetSpell[3].Description_Pointer + 0x20, redEyeDesc.Substring(0, Math.Min(4, redEyeDesc.Length)));
                    }

                    if (max) {
                        datasetSpell[0].MP = 1;
                        datasetSpell[1].MP = 1;
                        datasetSpell[2].MP = 1;
                        datasetSpell[3].MP = 1;
                        burnOut.MP = 1;
                        fireAttack.MP = 1;
                    } else {
                        datasetSpell[0].MP = _flameshotMP;
                        datasetSpell[1].MP = _explosionMP;
                        datasetSpell[2].MP = _finalBurstMP;
                        datasetSpell[3].MP = _redEyeMP;
                        burnOut.MP = _burnOutMP;
                        fireAttack.MP = _fireAttackMP;
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
                        burnOut.MP = _burnOutMP;
                        fireAttack.MP = _fireAttackMP;
                    }
                }

                for (int i = 0; i < 4; i++) {
                    spell = new Core.Memory.Battle.DragoonSpell(i);
                    spell.MP = datasetSpell[i].MP;
                }
                
            }
        }

        private void NewSpells() {
            if (Emulator.Region == Region.NTA) { // TODO Remove Region check, when other encoding tables work.
                long text = Emulator.GetAddress("UNUSED_RAM_1");
                Emulator.DirectAccess.WriteText(text, "Burn Out<END>");
                Emulator.DirectAccess.WriteText(text + 0x40, "Fire STR 100%<END>");
                Emulator.DirectAccess.WriteText(text + 0x80, "Fire Attack<END>");
                if (burnStackSelected) {
                    double burnAmount = 1 + (burnStacks * _damagePerBurn);
                    bool max = burnStacks == _maxBurnStacks;
                    string redEyeAttack = Convert.ToString(max ? (burnAmount * _burnStackMaxAdditionMulti) : (burnAmount * 1)) + "0";
                    Emulator.DirectAccess.WriteText(text + 0xC0, "Physical Fire STR 670% x" + redEyeAttack + "<END>");
                } else {
                    Emulator.DirectAccess.WriteText(text + 0xC0, "Physical Fire STR 670% x1.00<END>");
                }
                long namePointer = Emulator.GetAddress("DRAGOON_SPELL_NAME_PTR");
                Emulator.DirectAccess.WriteUInt(namePointer + 32 * 0x4, (uint) text);
                Emulator.DirectAccess.WriteByte(namePointer + 32 * 0x4 + 0x3, 0x80);
                Emulator.DirectAccess.WriteUInt(namePointer + 33 * 0x4, (uint) text + 0x80);
                Emulator.DirectAccess.WriteByte(namePointer + 33 * 0x4 + 0x3, 0x80);

                long descriptionPointer = Emulator.GetAddress("DRAGOON_DESC_PTR");
                Emulator.DirectAccess.WriteUInt(descriptionPointer + 32 * 0x4, (uint) text + 0x40);
                Emulator.DirectAccess.WriteByte(descriptionPointer + 32 * 0x4 + 0x3, 0x80);
                Emulator.DirectAccess.WriteUInt(descriptionPointer + 33 * 0x4, (uint) text + 0xC0);
                Emulator.DirectAccess.WriteByte(descriptionPointer + 33 * 0x4 + 0x3, 0x80);
            }
        }
    }
}
