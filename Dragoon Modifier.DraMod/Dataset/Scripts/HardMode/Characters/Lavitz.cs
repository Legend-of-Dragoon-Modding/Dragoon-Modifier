using Dragoon_Modifier.Core;
using Dragoon_Modifier.DraMod.Controller;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Dataset.Scripts.HardMode.Characters {
    internal class Lavitz : ICharacter {
        private const ushort _DAT = 400;
        private const ushort _specialDAT = 600;
        private const ushort _DDF = 180;
        private const ushort _DMDF = 180;

        private const ushort _wingBlaster = 440;
        private const byte _blossomStormTurnMP = 20;
        private const ushort _gaspless = 330;
        private const ushort _spinningGale = 100;
        private const ushort _jadeDragon = 440;
        private const byte _spinningGaleMP = 1;
        private const byte _windAttackMP = 100;
        private const byte _spinningGaleMark = 1;
        private const byte _wingBlasterMark = 1;
        private const byte _gasplessMark = 2;
        private const byte _jadeDragonMark = 3;
        private const byte _windMarkTPDamage = 5;
        private const ushort _windAttack = 480;
        private const ushort _specialWindAttack = 720;

        private ushort currentMP = 100;
        private ushort previousMP = 100;
        private double multi = 1;

        private bool _harpoonCheck = false;
        private bool _checkWingBlaster = false;
        private bool _checkSpinningGale = false;
        private bool _checkFlowerStorm = false;
        private bool _checkGaspless = false;
        private bool _checkJadeDragon = false;

        private ushort[] monsterHPTracker = new ushort[6];
        private ushort[] monsterTPTracker = new ushort[6];
        private int[] windMarkTurns = new int[6];
        private int[] windMarkCooldowns = new int[6];
        private byte selectedSpells = 0;
        private int dlv = 0;
        private bool dragoonGuard = false;
        private bool autoDragoon = false;
        private byte previousElement = 0;
        private bool turnAutoOn = false;

        public void Run(byte slot, byte dragoonSpecial) {
            if (Emulator.Memory.Battle.DragonBlockStaff == 1) {
                multi = 8;
            } else {
                multi = 1;
            }

            if (_harpoonCheck) {
                multi *= 3;
            }

            var battleTable = Emulator.Memory.Battle.CharacterTable[slot];

            if (battleTable.Action == 10) {
                DragoonAttack(battleTable, dragoonSpecial);
                Spells(battleTable, dragoonSpecial);
                WriteMP(battleTable);
                NewSpells();
            }

            if (battleTable.Action == 2) {
                DragoonDefence(battleTable);
                battleTable.Weapon_Element = previousElement;
                _checkWingBlaster = _checkSpinningGale = _checkGaspless = _checkJadeDragon = false;
                if (Emulator.Memory.Battle.IconSelected == 2) {
                    battleTable.Guard = 1;
                }
            }

            if (battleTable.Action == 26) {
                if (!autoDragoon && battleTable.CastingSpell == 48 && turnAutoOn) {
                    turnAutoOn = false;
                    Task.Factory.StartNew(() => {
                        Thread.Sleep(5000);
                        Emulator.Memory.Battle.AutoDragoon1 = 4194;
                        //Emulator.Memory.Battle.AutoDragoon2 = 4160;
                    });
                }
            }

            int i = 0;
            if (_checkWingBlaster || _checkSpinningGale || _checkGaspless || _checkJadeDragon) {
                foreach (var monster in Emulator.Memory.Battle.MonsterTable) {
                    if (monster.HP < monsterHPTracker[i]) {
                        if (_checkWingBlaster) {
                            windMarkTurns[i] = _wingBlasterMark;
                            windMarkCooldowns[i] = _wingBlasterMark + 1;
                            monsterTPTracker[i] = monster.Turn;
                        } else if (_checkSpinningGale) {
                            windMarkTurns[i] = _spinningGaleMark;
                            windMarkCooldowns[i] = _spinningGaleMark + 1;
                            monsterTPTracker[i] = monster.Turn;
                        } else if (_checkGaspless) {
                            windMarkTurns[i] = _gasplessMark;
                            windMarkCooldowns[i] = _gasplessMark + 1;
                            monsterTPTracker[i] = monster.Turn;
                        } else if (_checkJadeDragon) {
                            windMarkTurns[i] = _jadeDragonMark;
                            windMarkCooldowns[i] = _jadeDragonMark + 1;
                            monsterTPTracker[i] = monster.Turn;
                        }
                    }
                    monsterHPTracker[i] = monster.HP;
                    i++;
                }
            }

            foreach (var monster in Emulator.Memory.Battle.MonsterTable) {
                if (monster.Turn < monsterTPTracker[i]) {
                    if (windMarkTurns[i] > 0) {
                        windMarkTurns[i]--;
                        if (monster.Turn >= _windMarkTPDamage) {
                            monster.Turn -= _windMarkTPDamage;
                        } else {
                            monster.Turn = 0;
                        }
                    }
                    if (windMarkCooldowns[i] > 0) {
                        windMarkCooldowns[i]--;
                    }
                }
                monsterHPTracker[i] = monster.HP;
                i++;
            }

            SpecialEquips.Run(slot);
        }

        public void BattleSetup(byte slot) {
            var battleTable = Emulator.Memory.Battle.CharacterTable[slot];

            int i = 0;
            foreach (var monster in Emulator.Memory.Battle.MonsterTable) {
                monsterHPTracker[i] = monster.HP;
                windMarkTurns[i] = 0;
                i++;
            }

            dlv = battleTable.DLV;
            dragoonGuard = turnAutoOn = false;

            if (Settings.Instance.UltimateBoss && Settings.Instance.UltimateBossSelected == 37)
                dragoonGuard = true;
            if (dlv == 7)
                dragoonGuard = true;
            Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "FF FF FF FF FF FF FF FF");

            if (battleTable.ID == 1) {
                switch (dlv) {
                    case 1:
                        Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "05 FF FF FF FF FF FF FF");
                        break;
                    case 2:
                        Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "05 07 FF FF FF FF FF FF");
                        break;
                    case 3:
                        Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "05 07 06 FF FF FF FF FF");
                        break;
                    case 4:
                        Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "05 07 06 20 FF FF FF FF");
                        break;
                    case 5:
                    case 6:
                        Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "05 07 06 20 08 FF FF FF");
                        break;
                    case 7:
                        Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "05 07 06 20 08 21 FF FF");
                        break;
                }
            } else {
                switch (dlv) {
                    case 1:
                        Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "0E FF FF FF FF FF FF FF");
                        break;
                    case 2:
                        Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "0E 1A FF FF FF FF FF FF");
                        break;
                    case 3:
                        Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "0E 1A 11 FF FF FF FF FF");
                        break;
                    case 4:
                        Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "0E 1A 11 20 FF FF FF FF");
                        break;
                    case 5:
                    case 6:
                        Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "0E 1A 11 20 08 FF FF FF");
                        break;
                    case 7:
                        Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "0E 1A 11 20 08 21 FF FF");
                        break;
                }
            }

            SpecialEquips.Setup(slot);
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

        private void Spells(Core.Memory.Battle.Character battleTable, byte dragoonSpecial) {
            var spell = battleTable.SpellCast;
            currentMP = battleTable.MP;

            Core.Memory.Battle.DragoonSpell spinningGale = new Core.Memory.Battle.DragoonSpell(32);
            Core.Memory.Battle.DragoonSpell windAttack = new Core.Memory.Battle.DragoonSpell(33);
            spinningGale.MP = _spinningGaleMP;
            windAttack.MP = _windAttackMP;

            if (currentMP < previousMP) {
                switch (spell) {
                    case 5:
                    case 14:
                        battleTable.DMAT = (ushort) (_wingBlaster * multi);
                        _checkWingBlaster = true;
                        break;
                    case 6:
                    case 17:
                        battleTable.DMAT = (ushort) (_gaspless * multi);
                        _checkGaspless = true;
                        break;
                    case 8:
                        battleTable.DMAT = (ushort) (_jadeDragon * multi);
                        _checkJadeDragon = true;
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
                    case 32:
                    case 91:
                        _checkSpinningGale = true;
                        battleTable.DMAT = _spinningGale;
                        battleTable.SP += 50;
                        battleTable.MP += _spinningGaleMP;

                        if (battleTable.SP % 100 == 0) 
                            battleTable.DragoonTurns += 1;
                        break;
                    case 33:
                    case 49:
                        if (dragoonSpecial == 1 || dragoonSpecial == 5) {
                            battleTable.DAT = (ushort) (_specialWindAttack * multi);
                        } else {
                            battleTable.DAT = (ushort) (_windAttack * multi);
                        }
                        break;

                }
                battleTable.SpellCast = 255;
            }

            switch (battleTable.CastingSpell) {
                case 32:
                    battleTable.CastingSpell = 91;
                    break;
                case 33:
                    battleTable.CastingSpell = 49;
                    break;
                case 49: //Wind Attack
                    if (Emulator.Memory.ScreenFade == 3) {
                        Emulator.Memory.ScreenFade = 0;
                        previousElement = battleTable.Weapon_Element;
                        battleTable.Weapon_Element = 64;
                        Emulator.Memory.Battle.AutoDragoon1 = 4096;
                        //Emulator.Memory.Battle.AutoDragoon2 = 4096;
                        turnAutoOn = true;
                    }
                    break;
                case 91: //Spinning Gale
                    if (Emulator.Memory.ScreenFade == 3)
                        Emulator.Memory.ScreenFade = 0;
                    break;
            }

            previousMP = currentMP;
        }

        private void WriteMP(Core.Memory.Battle.Character battleTable) {
            Core.Memory.Battle.DragoonSpell spinningGale = new Core.Memory.Battle.DragoonSpell(32);
            Core.Memory.Battle.DragoonSpell windAttack = new Core.Memory.Battle.DragoonSpell(33);
            spinningGale.MP = _spinningGaleMP;
            windAttack.MP = _windAttackMP;

            if (dragoonGuard) {
                if (battleTable.Menu == 96 || battleTable.Menu == 31 || battleTable.Menu == 15) {
                    byte menu = 0;
                    ushort icons = 3;

                    for (byte i = 0; i < icons; i++) {
                        menu += (byte) Math.Pow(2, i);
                    }

                    battleTable.Menu = menu;

                    Emulator.Memory.Battle.BattleMenuCount = icons;
                }

                Emulator.Memory.Battle.BattleMenuSlot[0] = 9;
                Emulator.Memory.Battle.BattleMenuSlot[1] = 3;
                Emulator.Memory.Battle.BattleMenuSlot[2] = 8;
            }
        }

        private void NewSpells() {
            if (Emulator.Region == Region.NTA) { // TODO Remove Region check, when other encoding tables work.
                long text = Emulator.GetAddress("UNUSED_RAM_1");
                Emulator.DirectAccess.WriteText(text, "Spinning Gale<END>");
                Emulator.DirectAccess.WriteText(text + 0x40, "Wind STR 100%<END>");
                Emulator.DirectAccess.WriteText(text + 0x80, "Wind Attack<END>");
                Emulator.DirectAccess.WriteText(text + 0xC0, "Physical Wind STR 800%<END>");

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
