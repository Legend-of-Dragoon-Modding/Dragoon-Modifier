using Dragoon_Modifier.Core;
using Dragoon_Modifier.DraMod.Controller;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Dataset.Scripts.HardMode.Characters {
    internal class Kongol : ICharacter {
        private const ushort _DAT = 500;
        private const ushort _specialDAT = 600;
        private const ushort _DDF = 70;
        private const ushort _DMDF = 100;

        private const ushort _grandStream = 450;
        private const ushort _meteorStrike = 560;
        private const ushort _goldDragon = 740;
        private const ushort _pellet = 220;
        private const byte _pelletMP = 1;
        private const ushort _earthAttack = 600;
        private const byte _earthAttackMP = 100;
        private const ushort _specialEarthAttack = 900;

        private ushort currentMP = 100;
        private ushort previousMP = 100;
        private double multi = 1;
        private bool dragoonGuard = false;
        private byte previousElement = 0;
        private bool turnAutoOn = false;
        private int dlv = 0;

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
            }

            if (battleTable.Action == 2) {
                DragoonDefence(battleTable);
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
        }

        public void BattleSetup(byte slot) {
            var battleTable = Emulator.Memory.Battle.CharacterTable[slot];
            dragoonGuard = turnAutoOn = false;
            dlv = battleTable.DLV;

            if (Settings.Instance.UltimateBoss && Settings.Instance.UltimateBossSelected == 37)
                dragoonGuard = true;
            if (dlv == 7)
                dragoonGuard = true;
            Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "FF FF FF FF FF FF FF FF");

            switch (dlv) {
                case 1:
                    Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "1D FF FF FF FF FF FF FF");
                    break;
                case 2:
                    Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "1D 20 FF FF FF FF FF FF");
                    break;
                case 3:
                case 4:
                    Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "1D 20 1E 20 FF FF FF FF");
                    break;
                case 5:
                case 6:
                    Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "1D 20 1E 1F 1C FF FF FF");
                    break;
                case 7:
                    Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "1D 20 1E 1F 21 FF FF FF");
                    break;
            }

            SpecialEquips.Setup(slot);
        }

        private void DragoonAttack(Core.Memory.Battle.Character battleTable, byte dragoonSpecial) {
            if (dragoonSpecial == 7) {
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

            if (currentMP < previousMP) {
                switch (spell) {
                    case 29:
                        battleTable.DMAT = (ushort) (_grandStream * multi);
                        break;
                    case 30:
                        battleTable.DMAT = (ushort) (_meteorStrike * multi);
                        break;
                    case 31:
                        battleTable.DMAT = (ushort) (_goldDragon * multi);
                        break;
                    case 32:
                    case 89:
                        battleTable.DMAT = (ushort) (_pellet * multi);
                        battleTable.SP += 50;
                        battleTable.MP += _pelletMP;
                        break;
                    case 33:
                    case 55:
                        if (dragoonSpecial == 7) {
                            battleTable.DAT = (ushort) (_specialEarthAttack * multi);
                        } else {
                            battleTable.DAT = (ushort) (_earthAttack * multi);
                        }
                        break;

                }
                battleTable.SpellCast = 255;
            }

            switch (battleTable.CastingSpell) {
                case 32:
                    battleTable.CastingSpell = 89;
                    break;
                case 33:
                    battleTable.CastingSpell = 55;
                    break;
                case 55: //Earth Attack
                    if (Emulator.Memory.ScreenFade == 3) {
                        Emulator.Memory.ScreenFade = 0;
                        previousElement = battleTable.Weapon_Element;
                        battleTable.Weapon_Element = 2;
                        Emulator.Memory.Battle.AutoDragoon1 = 4096;
                        //Emulator.Memory.Battle.AutoDragoon2 = 4096;
                        turnAutoOn = true;
                    }
                    break;
                case 89: //Pellet
                    if (Emulator.Memory.ScreenFade == 3)
                        Emulator.Memory.ScreenFade = 0;
                    break;
            }

            previousMP = currentMP;
        }

        private void WriteMP(Core.Memory.Battle.Character battleTable) {
            Core.Memory.Battle.DragoonSpell pellet = new Core.Memory.Battle.DragoonSpell(32);
            Core.Memory.Battle.DragoonSpell earthAttack = new Core.Memory.Battle.DragoonSpell(33);
            pellet.MP = _pelletMP;
            earthAttack.MP = _earthAttackMP;

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
                Emulator.DirectAccess.WriteText(text, "Pellet<END>");
                Emulator.DirectAccess.WriteText(text + 0x40, "Earth STR 220%<END>");
                Emulator.DirectAccess.WriteText(text + 0x80, "Earth Attack<END>");
                Emulator.DirectAccess.WriteText(text + 0xC0, "Physical Earth STR 960%<END>");

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
