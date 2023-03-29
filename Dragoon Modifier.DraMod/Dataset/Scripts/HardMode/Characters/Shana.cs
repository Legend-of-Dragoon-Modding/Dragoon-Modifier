using Dragoon_Modifier.Core;
using Dragoon_Modifier.DraMod.Controller;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Dataset.Scripts.HardMode.Characters {
    internal class Shana : ICharacter {
        private const ushort _DAT = 365;
        private const ushort _specialDAT = 510;
        private const ushort _DDF = 180;
        private const ushort _DMDF = 180;

        private const byte _moonLightMP = 20;
        private const ushort _starChildren = 332;
        private const byte _gatesOfHeavenMP = 30;
        private const ushort _transLight = 100;
        private const byte _transLightMP = 1;
        private const ushort _wSilverDragon = 289;
        private const byte _rapidFireMP = 20;
        private const ushort _rapidFire = 85;
        private const byte _magicArrowMP = 100;
        private const int _gatesOfHeavenHealBase = 50;
        private const int _gatesOfHeavenHardModePenalty = 8;
        private const ushort _lightAttack = 440;
        private const ushort _specialLightAttack = 660;

        private ushort currentMP = 100;
        private ushort previousMP = 100;
        private double multi = 1;

        private int dlv = 0;
        private byte attackType = 0;
        private bool dragoonGuard = false;
        private bool autoDragoon = false;
        private byte previousElement = 0;
        private bool changedToAttackPointer = false;
        private uint savePointerAddress = 0;
        private bool rapidFire = false;
        public int rapidFireCount = 0;
        private bool rapidFirePressed = false;
        private bool rapidFireContinue = false;
        private bool magicArrow = false;
        private bool writeTargetInfo = false;

        public void Run(byte slot, byte dragoonSpecial) {
            if (Emulator.Memory.Battle.DragonBlockStaff == 1) {
                multi = 8;
            } else {
                multi = 1;
            }

            var battleTable = Emulator.Memory.Battle.CharacterTable[slot];

            if (battleTable.Action == 10) {
                DragoonAttack(battleTable, dragoonSpecial);
                Spells(battleTable, dragoonSpecial);
                WriteMP(battleTable);
                NewSpells();
                if (writeTargetInfo && battleTable.SpecialAnimationFrameCounter > 9) {
                    writeTargetInfo = rapidFirePressed = false;
                    battleTable.BattleScriptPtr7 = savePointerAddress;
                }

                if (Emulator.Memory.Battle.IconSelected == 2) {
                    battleTable.Guard = 1;
                } else {
                    battleTable.Guard = 0;
                }
            }

            if (battleTable.Action == 2) {
                DragoonDefence(battleTable);
                battleTable.Weapon_Element = previousElement;
                rapidFire = rapidFirePressed = writeTargetInfo = magicArrow = false;
            }

            if (battleTable.Action == 138) {
                if (rapidFire) {
                    Constants.UIControl.WriteGLog("Shana Rapid Fire Frame: " + battleTable.SpecialAnimationFrameCounter + "/" + battleTable.UnknownShanaBowValue);
                    if (Emulator.Memory.Hotkey == 8) {
                        RapidFireCheck(battleTable);
                    }
                }
            }

            SpecialEquips.Run(slot);
        }

        public void BattleSetup(byte slot) {
            var battleTable = Emulator.Memory.Battle.CharacterTable[slot];

            dlv = battleTable.DLV;
            dragoonGuard = changedToAttackPointer = rapidFireContinue = rapidFirePressed = magicArrow = false;
            rapidFireCount = 0;
            previousElement = battleTable.Weapon_Element;

            if (Settings.Instance.UltimateBoss && Settings.Instance.UltimateBossSelected == 37)
                dragoonGuard = true;
            if (dlv == 7)
                dragoonGuard = true;
            Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "FF FF FF FF FF FF FF FF");

            if (battleTable.ID == 2) {
                switch (dlv) {
                    case 1:
                        Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "0B FF FF FF FF FF FF FF");
                        break;
                    case 2:
                        Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "0B 0A FF FF FF FF FF FF");
                        break;
                    case 3:
                        Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "0B 0A 0C FF FF FF FF FF");
                        break;
                    case 4:
                        Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "0B 0A 0C 20 FF FF FF FF");
                        break;
                    case 5:
                        Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "0B 0A 0C 20 0D FF FF FF");
                        break;
                    case 6:
                        Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "0B 0A 0C 20 0D 21 FF FF");
                        break;
                    case 7:
                        Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "0B 0A 0C 20 0D 21 22 FF");
                        break;
                }
            } else {
                switch (dlv) {
                    case 1:
                        Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "43 FF FF FF FF FF FF FF");
                        break;
                    case 2:
                        Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "43 42 FF FF FF FF FF FF");
                        break;
                    case 3:
                        Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "43 42 44 FF FF FF FF FF");
                        break;
                    case 4:
                        Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "43 42 44 20 FF FF FF FF");
                        break;
                    case 5:
                        Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "43 42 44 20 0D FF FF FF");
                        break;
                    case 6:
                        Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "43 42 44 20 0D 21 FF FF");
                        break;
                    case 7:
                        Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "43 42 44 20 0D 21 22 FF");
                        break;
                }
            }

            SpecialEquips.Setup(slot);
        }

        private void DragoonAttack(Core.Memory.Battle.Character battleTable, byte dragoonSpecial) {
            if (dragoonSpecial == 2 || dragoonSpecial == 8) {
                if (rapidFire) {
                    battleTable.DAT = (ushort) (_rapidFire * multi);
                } else {
                    if (magicArrow) {
                        battleTable.DAT = (ushort) (_specialLightAttack * multi);
                    } else {
                        battleTable.DAT = (ushort) (_specialDAT * multi);
                    }
                }
                return;
            }

            if (rapidFire) {
                battleTable.DAT = (ushort) (_rapidFire * multi);
            } else {
                if (magicArrow) {
                    battleTable.DAT = (ushort) (_lightAttack * multi);
                } else {
                    battleTable.DAT = (ushort) (_DAT * multi);
                }
            }
        }

        private void DragoonDefence(Core.Memory.Battle.Character battleTable) {
            battleTable.DDF = (ushort) (_DDF * multi);
            battleTable.DMDF = (ushort) (_DMDF * multi);
        }

        private void Spells(Core.Memory.Battle.Character battleTable, byte dragoonSpecial) {
            var spell = battleTable.SpellCast;
            currentMP = battleTable.MP;
            if (currentMP < previousMP) {
                switch (spell) {
                    case 10:
                    case 65:
                        battleTable.DMAT = (ushort) (_starChildren * multi);
                        break;
                    case 13:
                        battleTable.DMAT = (ushort) (_wSilverDragon * multi);
                        break;
                    case 32:
                    case 91:
                        battleTable.DMAT = (ushort) (_transLight * multi);
                        battleTable.SP += 50;
                        battleTable.MP += _transLightMP;

                        if (battleTable.SP % 100 == 0)
                            battleTable.DragoonTurns += 1;
                        break;
                    case 33:
                    case 50:
                    case 56:
                        break;
                }
                battleTable.SpellCast = 255;
            }

            switch (battleTable.CastingSpell) {
                case 32:
                    battleTable.CastingSpell = 93;
                    break;
                case 33:
                case 34:
                    if (battleTable.SpellTarget > (byte) 0 && battleTable.SpellTarget < (byte) 255) {
                        if (!changedToAttackPointer) {
                            battleTable.BattleScriptPtr7 += 4444;
                            savePointerAddress = battleTable.BattleScriptPtr7;
                            changedToAttackPointer = true;
                            rapidFireContinue = true;
                            rapidFireCount = 0;
                        }
                    } else {
                        changedToAttackPointer = false;
                    }

                    if (battleTable.CastingSpell == 33) { 
                        rapidFire = true;
                        magicArrow = false;
                        battleTable.Weapon_Element = previousElement;
                    } else {
                        rapidFire = false;
                        magicArrow = true;
                        battleTable.Weapon_Element = 32;
                    }


                    if (battleTable.ID == 2) {
                        battleTable.CastingSpell = 50;
                    } else {
                        battleTable.CastingSpell = 56;
                    }
                    break;
                case 50:
                case 56:
                    if (!rapidFire) {
                        Emulator.Memory.ScreenFade = 0;
                    }
                    break;
                case 93: //Trans Light
                    if (Emulator.Memory.ScreenFade == 3)
                        Emulator.Memory.ScreenFade = 0;
                    break;
            }

            previousMP = currentMP;
        }

        private void WriteMP(Core.Memory.Battle.Character battleTable) {
            Core.Memory.Battle.DragoonSpell transLight = new Core.Memory.Battle.DragoonSpell(32);
            Core.Memory.Battle.DragoonSpell rapidFire = new Core.Memory.Battle.DragoonSpell(33);
            Core.Memory.Battle.DragoonSpell magicArrow = new Core.Memory.Battle.DragoonSpell(34);
            Core.Memory.Battle.DragoonSpell gatesOfHeaven = new Core.Memory.Battle.DragoonSpell(11);
            Core.Memory.Battle.DragoonSpell gatesOfHeaven2 = new Core.Memory.Battle.DragoonSpell(67);
            transLight.MP = _transLightMP;
            rapidFire.MP = _rapidFireMP;
            magicArrow.MP = _magicArrowMP;
            gatesOfHeaven.MP = (byte) (battleTable.MaxMP > 150 ? (battleTable.MaxMP / 5) : _gatesOfHeavenMP);
            gatesOfHeaven2.MP = (byte) (battleTable.MaxMP > 150 ? (battleTable.MaxMP / 5) : _gatesOfHeavenMP);

            int deadPartyMembers = 0;
            foreach (var charater in Emulator.Memory.Battle.CharacterTable) {
                if (charater.HP == 0) deadPartyMembers++;
            }

            if (deadPartyMembers > 0) {
                gatesOfHeaven.DamageMultiplier = (byte) (_gatesOfHeavenHealBase - (deadPartyMembers * _gatesOfHeavenHardModePenalty));
            }

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
                Emulator.DirectAccess.WriteText(text, "Trans Light<END>");
                Emulator.DirectAccess.WriteText(text + 0x40, "Light STR 100%<END>");
                Emulator.DirectAccess.WriteText(text + 0x80, "Rapid Fire<END>");
                Emulator.DirectAccess.WriteText(text + 0xC0, "Physical STR 85% x1-x5<END>");
                Emulator.DirectAccess.WriteText(text + 0x100, "Light Arrow<END>");
                Emulator.DirectAccess.WriteText(text + 0x140, "Physical Light STR 550%<END>");

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

        private void RapidFireCheck(Core.Memory.Battle.Character battleTable) {
            if (!rapidFirePressed && rapidFireContinue) {
                rapidFireContinue = false;
                rapidFirePressed = true;

                if (battleTable.UnknownShanaBowValue >= 2) {
                    if (rapidFireCount == 0 && battleTable.SpecialAnimationFrameCounter < 40) {
                        RapidFireAttack(battleTable);
                    } else if (rapidFireCount == 1 && battleTable.SpecialAnimationFrameCounter < 32) {
                        RapidFireAttack(battleTable);
                    } else if (rapidFireCount == 2 && battleTable.SpecialAnimationFrameCounter < 24) {
                        RapidFireAttack(battleTable);
                    } else if (rapidFireCount == 3 && battleTable.SpecialAnimationFrameCounter < 16) {
                        RapidFireAttack(battleTable);
                    } else if (rapidFireCount == 4 && battleTable.SpecialAnimationFrameCounter < 8) {
                        RapidFireAttack(battleTable);
                    }
                }
            }
        }

        private void RapidFireAttack(Core.Memory.Battle.Character battleTable) {

            rapidFireCount++;
            rapidFireContinue = writeTargetInfo = true;
            battleTable.UnknownShanaBowValue = 8;
            if (rapidFireCount == 5) {
                rapidFireContinue = rapidFirePressed = writeTargetInfo = false;
            }
        }
    }
}
