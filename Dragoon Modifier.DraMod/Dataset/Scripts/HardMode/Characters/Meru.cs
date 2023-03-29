using Dragoon_Modifier.Core;
using Dragoon_Modifier.Core.Memory.Encounter;
using Dragoon_Modifier.DraMod.Controller;

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
        private const ushort _spearFrost = 100;
        private const ushort _blueSeaDragon = 350;
        private const ushort _waterAttack = 395;
        private const ushort _specialWaterAttack = 595;
        private const byte _spearFrostMP = 1;
        private const byte _boostMP = 60;
        private const byte _waterAttackMP = 100;

        private const ushort _enhancedFreezingRing = 400;
        private const ushort _enhancedDiamondDust = 440;
        private const ushort _enhancedBlueSeaDragon = 525;

        private ushort currentMP = 100;
        private ushort previousMP = 100;
        private double multi = 1;
        private bool dragoonGuard = false;
        private byte previousElement = 0;
        private bool turnAutoOn = false;
        private int dlv = 0;
        private int boostTurnsLeft = 0;
        private bool boostTurnTaken = false;
        private ushort beforeMaxHP = 0;
        private ushort beforeMDF = 0;
        private ushort boostHP = 0;

        private bool _enhancedDragoon = false;
        private bool _trackRainbowBreath = false;

        public void Run(byte slot, byte dragoonSpecial) {
            if (Emulator.Memory.Battle.DragonBlockStaff == 1) {
                multi = 8;
            } else {
                multi = 1;
            }

            if (boostTurnsLeft > 0) {
                multi *= 1.1;
            }

            var battleTable = Emulator.Memory.Battle.CharacterTable[slot];

            if (battleTable.Action == 10) {
                DragoonAttack(battleTable, dragoonSpecial);
                Spells(battleTable, dragoonSpecial, slot);
                WriteMP(battleTable);
                NewSpells();
            }

            if (battleTable.Action == 2) {
                DragoonDefence(battleTable);
                battleTable.Weapon_Element = previousElement;
            }

            if (battleTable.Action == 26) {
                if (battleTable.CastingSpell == 51 && turnAutoOn) {
                    turnAutoOn = false;
                    Task.Factory.StartNew(() => {
                        Thread.Sleep(5000);
                        Emulator.Memory.Battle.AutoDragoon1 = 4194;
                        //Emulator.Memory.Battle.AutoDragoon2 = 4160;
                    });
                }
            }

            if (boostTurnsLeft > 0) {
                if (boostHP > battleTable.HP) {
                    battleTable.HP = boostHP;
                } else {
                    boostHP = battleTable.HP;
                }
                if ((battleTable.Action == 8 || battleTable.Action == 10) && (battleTable.Menu == 31 || battleTable.Menu == 159 || battleTable.Menu == 96 || battleTable.Menu == 31 || battleTable.Menu == 15 || battleTable.Menu == 7)) {
                    if (!boostTurnTaken) {
                        boostTurnsLeft--;
                        boostTurnTaken = true;
                        if (boostTurnsLeft == 0) {
                            battleTable.MaxHP = beforeMaxHP;
                            battleTable.HP = Math.Min(battleTable.HP, battleTable.MaxHP);
                            battleTable.OG_MDF = beforeMDF;
                            battleTable.MDF = beforeMDF;

                            switch (dlv) {
                                case 6:
                                    Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "18 19 1B 20 1C 21 FF FF");
                                    break;
                                case 7:
                                    Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "18 19 1B 20 1C 21 22 FF");
                                    break;
                            }

                        }
                    }
                } else {
                    if (battleTable.Action != 8 && battleTable.Action != 10) {
                        boostTurnTaken = false;
                    }
                }
            }
        }

        public void BattleSetup(byte slot) {
            var battleTable = Emulator.Memory.Battle.CharacterTable[slot];
            dragoonGuard = turnAutoOn = boostTurnTaken = false;
            dlv = battleTable.DLV;
            boostTurnsLeft = 0;

            if (Settings.Instance.UltimateBoss && Settings.Instance.UltimateBossSelected == 37)
                dragoonGuard = true;
            if (dlv == 7)
                dragoonGuard = true;
            Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "FF FF FF FF FF FF FF FF");

            switch (dlv) {
                case 1:
                    Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "18 FF FF FF FF FF FF FF");
                    break;
                case 2:
                    Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "18 19 FF FF FF FF FF FF");
                    break;
                case 3:
                    Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "18 19 1B FF FF FF FF FF");
                    break;
                case 4:
                    Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "18 19 1B 20 FF FF FF FF");
                    break;
                case 5:
                    Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "18 19 1B 20 1C FF FF FF");
                    break;
                case 6:
                    Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "18 19 1B 20 1C 21 FF FF");
                    break;
                case 7:
                    Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "18 19 1B 20 1C 21 22 FF");
                    break;
            }

            SpecialEquips.Setup(slot);
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

        private void Spells(Core.Memory.Battle.Character battleTable, byte dragoonSpecial, byte slot) {
            var spell = battleTable.SpellCast;
            currentMP = battleTable.MP;

            if (currentMP < previousMP) {
                switch (spell) {
                    case 24:
                        battleTable.DMAT = (ushort) ((!_enhancedDragoon ? _freezingRing : _enhancedFreezingRing ) * multi);
                        break;
                    case 25:
                        if (_enhancedDragoon) {
                            _trackRainbowBreath = true;
                        }
                        break;
                    case 27:
                        battleTable.DMAT = (ushort) ((!_enhancedDragoon ? _diamondDust : _enhancedDiamondDust) * multi);
                        break;
                    case 28:
                        battleTable.DMAT = (ushort) ((!_enhancedDragoon ? _blueSeaDragon : _enhancedBlueSeaDragon) * multi);
                        break;
                    case 32:
                    case 90:
                        battleTable.DMAT = (ushort) (_spearFrost * multi);
                        battleTable.SP += 50;
                        battleTable.MP += _spearFrostMP;

                        if (battleTable.SP % 100 == 0)
                            battleTable.DragoonTurns += 1;
                        break;
                    case 33:
                    case 43:
                        beforeMaxHP = battleTable.MaxHP;
                        beforeMDF = battleTable.MDF;
                        boostTurnsLeft = 6;

                        battleTable.MaxHP = (ushort) Math.Min(30000, battleTable.MaxHP * 3);
                        battleTable.HP = battleTable.MaxHP;
                        boostHP = battleTable.MaxHP;
                        battleTable.OG_MDF = battleTable.DF;
                        battleTable.MDF = battleTable.DF;


                        switch (dlv) {
                            case 6:
                                Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "18 19 1B 20 1C FF FF FF");
                                break;
                            case 7:
                                Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "18 19 1B 20 1C 22 FF FF");
                                break;
                        }
                        break;
                    case 34:
                    case 54:
                        if (dragoonSpecial == 6) {
                            battleTable.DAT = (ushort) (_specialWaterAttack * multi);
                        } else {
                            battleTable.DAT = (ushort) (_waterAttack * multi);
                        }
                        break;
                }
                battleTable.SpellCast = 255;
            }

            switch (battleTable.CastingSpell) {
                case 32:
                    battleTable.CastingSpell = 90;
                    break;
                case 33:
                    //battleTable.CastingSpell = 43;
                    break;
                case 34:
                    battleTable.CastingSpell = 54;
                    break;
                case 54: //Water Attack
                    if (Emulator.Memory.ScreenFade == 3) {
                        Emulator.Memory.ScreenFade = 0;
                        previousElement = battleTable.Weapon_Element;
                        battleTable.Weapon_Element = 1;
                        Emulator.Memory.Battle.AutoDragoon1 = 4096;
                        //Emulator.Memory.Battle.AutoDragoon2 = 4096;
                        turnAutoOn = true;
                    }
                    break;
                case 90: //Spear Frost
                    if (Emulator.Memory.ScreenFade == 3)
                        Emulator.Memory.ScreenFade = 0;
                    break;
            }

            previousMP = currentMP;
        }

        private void WriteMP(Core.Memory.Battle.Character battleTable) {
            Core.Memory.Battle.DragoonSpell spearFrost = new Core.Memory.Battle.DragoonSpell(32);
            Core.Memory.Battle.DragoonSpell boost = new Core.Memory.Battle.DragoonSpell(33);
            Core.Memory.Battle.DragoonSpell waterAttack = new Core.Memory.Battle.DragoonSpell(34);
            spearFrost.MP = _spearFrostMP;
            boost.MP = _boostMP;
            waterAttack.MP = _waterAttackMP;

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
                Emulator.DirectAccess.WriteText(text, "Spear Frost<END>");
                Emulator.DirectAccess.WriteText(text + 0x40, "Water STR 100%<END>");
                Emulator.DirectAccess.WriteText(text + 0x80, "Boost<END>");
                Emulator.DirectAccess.WriteText(text + 0xC0, "Max HP Tripled Healing Disabled<END>");
                Emulator.DirectAccess.WriteText(text + 0x100, "Water Attack<END>");
                Emulator.DirectAccess.WriteText(text + 0x140, "Physical Water STR 530%<END>");

                long namePointer = Emulator.GetAddress("DRAGOON_SPELL_NAME_PTR");
                Emulator.DirectAccess.WriteUInt(namePointer + 32 * 0x4, (uint) text);
                Emulator.DirectAccess.WriteByte(namePointer + 32 * 0x4 + 0x3, 0x80);
                Emulator.DirectAccess.WriteUInt(namePointer + 33 * 0x4, (uint) text + 0x80);
                Emulator.DirectAccess.WriteByte(namePointer + 33 * 0x4 + 0x3, 0x80);
                Emulator.DirectAccess.WriteUInt(namePointer + 34 * 0x4, (uint) text + 0x100);
                Emulator.DirectAccess.WriteByte(namePointer + 34 * 0x4 + 0x3, 0x80);

                long descriptionPointer = Emulator.GetAddress("DRAGOON_DESC_PTR");
                Emulator.DirectAccess.WriteUInt(descriptionPointer + 32 * 0x4, (uint) text + 0x40);
                Emulator.DirectAccess.WriteByte(descriptionPointer + 32 * 0x4 + 0x3, 0x80);
                Emulator.DirectAccess.WriteUInt(descriptionPointer + 33 * 0x4, (uint) text + 0xC0);
                Emulator.DirectAccess.WriteByte(descriptionPointer + 33 * 0x4 + 0x3, 0x80);
                Emulator.DirectAccess.WriteUInt(descriptionPointer + 34 * 0x4, (uint) text + 0x140);
                Emulator.DirectAccess.WriteByte(descriptionPointer + 34 * 0x4 + 0x3, 0x80);
            }
        }
    }
}
