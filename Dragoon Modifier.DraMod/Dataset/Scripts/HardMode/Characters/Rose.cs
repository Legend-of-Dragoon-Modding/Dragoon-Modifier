using Dragoon_Modifier.Core;
using Dragoon_Modifier.DraMod.Controller;

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
        private const ushort _darkMist = 170;
        private const ushort _darkDragon = 420;
        private const byte _darkMistMP = 1;
        private const byte _darkAttackMP = 100;
        private const ushort _darkAttack = 440;
        private const ushort _specialDarkAttack = 660;

        private const ushort _enhancedAstralDrain = 410;
        private const double _enhancedAstralDrainHeal = 0.04;
        private const ushort _enhancedDeathDimension = 790;
        private const ushort _enhancedDarkDragon = 290;

        private ushort currentMP = 100;
        private ushort previousMP = 100;
        private double multi = 1;
        private bool checkDamage = false;
        private bool dragoonGuard = false;
        private byte previousElement = 0;
        private bool turnAutoOn = false;
        private int dlv = 0;
        private ushort roseDamageSave = 0;

        private bool _enhancedDragoon = false;
        private bool _checkRoseDamage = false;


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


            if (checkDamage) {
                ushort damage = Emulator.DirectAccess.ReadUShort(Emulator.GetAddress("DAMAGE_SLOT"));
                int spellCast = battleTable.CastingSpell;
                if (damage != roseDamageSave) {
                    checkDamage = false;
                    if (spellCast == 32 || spellCast == 33 || spellCast == 51 || spellCast == 94) { //Dark Mist
                        battleTable.HP = (ushort) Math.Min(battleTable.HP + (damage * 0.05), battleTable.MaxHP);
                    } else {
                        if (_enhancedDragoon) {
                            battleTable.HP = (ushort) Math.Min(battleTable.HP + (damage * 0.4), battleTable.MaxHP);
                        } else {
                            battleTable.HP = (ushort) Math.Min(battleTable.HP + (damage * 0.1), battleTable.MaxHP);
                        }
                    }
                }
            }
        }

        public void BattleSetup(byte slot) {
            var battleTable = Emulator.Memory.Battle.CharacterTable[slot];
            dragoonGuard = turnAutoOn = checkDamage = false;
            dlv = battleTable.DLV;

            if (Settings.Instance.UltimateBoss && Settings.Instance.UltimateBossSelected == 37)
                dragoonGuard = true;
            if (dlv == 7)
                dragoonGuard = true;
            Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "FF FF FF FF FF FF FF FF");

            switch (dlv) {
                case 1:
                    Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "0F FF FF FF FF FF FF FF");
                    break;
                case 2:
                    Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "0F 10 FF FF FF FF FF FF");
                    break;
                case 3:
                    Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "0F 10 12 FF FF FF FF FF");
                    break;
                case 4:
                    Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "0F 10 12 20 FF FF FF FF");
                    break;
                case 5:
                case 6:
                    Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "0F 10 12 20 13 FF FF FF");
                    break;
                case 7:
                    Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "0F 10 12 20 13 21 FF FF");
                    break;
            }

            SpecialEquips.Setup(slot);
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

        private void Spells(Core.Memory.Battle.Character battleTable, byte dragoonSpecial) {
            var spell = battleTable.SpellCast;
            currentMP = battleTable.MP;

            Core.Memory.Battle.DragoonSpell darkMist = new Core.Memory.Battle.DragoonSpell(32);
            Core.Memory.Battle.DragoonSpell darkAttack = new Core.Memory.Battle.DragoonSpell(33);
            darkMist.MP = _darkMistMP;
            darkAttack.MP = _darkAttackMP;

            if (currentMP < previousMP) {
                switch (spell) {
                    case 15:
                        battleTable.DMAT = (ushort) ((!_enhancedDragoon ? _astralDrain : _enhancedAstralDrain) * multi);
                        for (int character = 0; character < Emulator.Memory.Battle.CharacterTable.Length; character++) {
                            var boostedHP = Emulator.Memory.Battle.CharacterTable[character].HP * (1 + dlv * (_enhancedDragoon ? _astralDrainHeal : _enhancedAstralDrainHeal));
                            Emulator.Memory.CharacterTable[character].HP = (ushort) Math.Min(Emulator.Memory.Battle.CharacterTable[character].MaxHP, boostedHP);
                        }
                        break;
                    case 16:
                        battleTable.DMAT = (ushort) ((!_enhancedDragoon ? _deathDimension : _enhancedDeathDimension) * multi);
                        break;
                    case 19:
                        battleTable.DMAT = (ushort) ((!_enhancedDragoon ? _darkDragon : _enhancedDarkDragon) * multi);
                        checkDamage = true;
                        roseDamageSave = Emulator.DirectAccess.ReadUShort(Emulator.GetAddress("DAMAGE_SLOT"));
                        break;
                    case 32:
                    case 94:
                        battleTable.DMAT = (ushort) (_darkMist * multi);
                        checkDamage = true;
                        roseDamageSave = Emulator.DirectAccess.ReadUShort(Emulator.GetAddress("DAMAGE_SLOT"));
                        battleTable.SP += 50;
                        battleTable.MP += _darkMistMP;

                        if (battleTable.SP % 100 == 0)
                            battleTable.DragoonTurns += 1;
                        break;
                    case 33:
                    case 51:
                        if (dragoonSpecial == 3) {
                            battleTable.DAT = (ushort) (_specialDarkAttack * multi);
                        } else {
                            battleTable.DAT = (ushort) (_darkAttack * multi);
                        }
                        break;
                }
                battleTable.SpellCast = 255;
            }

            switch (battleTable.CastingSpell) {
                case 32:
                    battleTable.CastingSpell = 94;
                    break;
                case 33:
                    battleTable.CastingSpell = 51;
                    break;
                case 51: //Dark Attack
                    if (Emulator.Memory.ScreenFade == 3) {
                        Emulator.Memory.ScreenFade = 0;
                        previousElement = battleTable.Weapon_Element;
                        battleTable.Weapon_Element = 4;
                        Emulator.Memory.Battle.AutoDragoon1 = 4096;
                        //Emulator.Memory.Battle.AutoDragoon2 = 4096;
                        turnAutoOn = true;
                    }
                    break;
                case 94: //Dark Mist
                    if (Emulator.Memory.ScreenFade == 3)
                        Emulator.Memory.ScreenFade = 0;
                    break;
            }

            previousMP = currentMP;
        }

        private void WriteMP(Core.Memory.Battle.Character battleTable) {
            Core.Memory.Battle.DragoonSpell darkMist = new Core.Memory.Battle.DragoonSpell(32);
            Core.Memory.Battle.DragoonSpell darkAttack = new Core.Memory.Battle.DragoonSpell(33);
            darkMist.MP = _darkMistMP;
            darkAttack.MP = _darkAttackMP;

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
                Emulator.DirectAccess.WriteText(text, "Dark Mist<END>");
                Emulator.DirectAccess.WriteText(text + 0x40, "Dark STR 170%<END>");
                Emulator.DirectAccess.WriteText(text + 0x80, "Dark Attack<END>");
                Emulator.DirectAccess.WriteText(text + 0xC0, "Physical Dark STR 660%<END>");

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
