using Dragoon_Modifier.Core;
using Dragoon_Modifier.Core.Memory.Battle;
using Dragoon_Modifier.DraMod.Controller;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static System.Net.Mime.MediaTypeNames;

namespace Dragoon_Modifier.DraMod.Dataset.Scripts.HellMode.Characters {
    internal class Haschel : ICharacter {
        private const ushort _DAT = 281;
        private const ushort _specialDAT = 422;
        private const ushort _DDF = 180;
        private const ushort _DMDF = 180;

        private const ushort _atomicMind = 330;
        private const ushort _thunderKid = 330;
        private const ushort _thunderGod = 330;
        private const ushort _violetDragon = 374;
        private const ushort _thunderAttack = 337;
        private const ushort _specialThunderAttack = 505;
        private const byte _sparkNetMP = 10;
        private const byte _thunderAttackMP = 100;
        private const ushort _sparkNet = 150;
        private const ushort _sparkNetMax = 440;
        private const ushort _sparkNetThunder = 1320;
        private const int _maxThunderCharge = 10;

        private ushort currentMP = 100;
        private ushort previousMP = 100;
        private double multi = 1;
        private bool dragoonGuard = false;
        private byte previousElement = 0;
        private bool turnAutoOn = false;
        private int dlv = 0;
        private bool thunderUsed = false;
        private int thunderCharge = 0;
        private bool haschelAttacking = false;
        private bool thunderSelected = false;

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
                NewSpells(battleTable);
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

            if (Emulator.Memory.Battle.ItemUsed == 194 || Emulator.Memory.Battle.ItemUsed == 207 || Emulator.Memory.Battle.ItemUsed == 247) {
                CheckElectricCharges();
            }

            if (!haschelAttacking && new byte[] { 24, 26/*, 136, 138*/ }.Contains(battleTable.Action)) {
                haschelAttacking = true;
                int chance = new Random().Next(0, 100);

                if (chance < 30) {
                    for (int x = 0; x < Constants.InventorySize; x++) {
                        if (Emulator.Memory.ItemInventory[x] == 255) {
                            Emulator.Memory.ItemInventory[x] = (byte) (chance < 18 ? 207 : 194);
                            Emulator.Memory.ItemInventorySize += 1;
                            break;
                        }
                    }
                }
            }

            if (haschelAttacking && (battleTable.Action == 8 || battleTable.Action == 10)) {
                haschelAttacking = false;
            }
        }

        public void BattleSetup(byte slot) {
            var battleTable = Emulator.Memory.Battle.CharacterTable[slot];
            dragoonGuard = turnAutoOn = haschelAttacking = false;
            thunderCharge = 0;
            dlv = battleTable.DLV;

            if (Settings.Instance.UltimateBoss && Settings.Instance.UltimateBossSelected == 37)
                dragoonGuard = true;
            if (dlv == 7)
                dragoonGuard = true;
            Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "FF FF FF FF FF FF FF FF");

            switch (dlv) {
                case 1:
                    Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "20 FF FF FF FF FF FF FF");
                    break;
                case 2:
                    Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "20 14 FF FF FF FF FF FF");
                    break;
                case 3:
                    Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "20 14 15 FF FF FF FF FF");
                    break;
                case 4:
                    Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "20 14 15 16 FF FF FF FF");
                    break;
                case 5:
                case 6:
                    Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "20 14 15 16 17 FF FF FF");
                    break;
                case 7:
                    Emulator.DirectAccess.WriteAoB(Emulator.GetAddress("DRAGOON_SPELL_SLOT") + slot * 0x9 + 1, "20 14 15 16 17 21 FF FF");
                    break;
            }

            SpecialEquips.Setup(slot);
        }

        private void DragoonAttack(Core.Memory.Battle.Character battleTable, byte dragoonSpecial) {
            if (dragoonSpecial == 4) {
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

            Core.Memory.Battle.DragoonSpell sparkNet = new Core.Memory.Battle.DragoonSpell(32);
            Core.Memory.Battle.DragoonSpell thunderAttack = new Core.Memory.Battle.DragoonSpell(33);
            sparkNet.MP = _sparkNetMP;
            thunderAttack.MP = _thunderAttackMP;

            if (currentMP < previousMP) {
                switch (spell) {
                    case 32:
                    case 86:
                        if (thunderCharge == _maxThunderCharge) {
                            if (thunderSelected) {
                                battleTable.DMAT = (ushort) (_sparkNetThunder * multi);
                            } else {
                                battleTable.DMAT = (ushort) (_sparkNetMax * multi);
                            }
                        } else {
                            battleTable.DMAT = (ushort) (_sparkNet * multi);
                        }
                        AddThunderCharge(1);
                        //target info
                        break;
                    case 20:
                        battleTable.DMAT = _atomicMind;
                        AddThunderCharge(1);
                        break;
                    case 21:
                        battleTable.DMAT = _thunderKid;
                        AddThunderCharge(1);
                        break;
                    case 22:
                        battleTable.DMAT = _thunderGod;
                        AddThunderCharge(1);
                        break;
                    case 23:
                        battleTable.DMAT = _violetDragon;
                        AddThunderCharge(1);
                        break;
                    case 33:
                    case 52:
                        if (dragoonSpecial == 4) {
                            battleTable.DAT = (ushort) (_specialThunderAttack * multi);
                        } else {
                            battleTable.DAT = (ushort) (_thunderAttack * multi);
                        }
                        break;
                }
                battleTable.SpellCast = 255;
            }

            switch (battleTable.CastingSpell) {
                case 32:
                    battleTable.CastingSpell = 86;
                    break;
                case 33:
                    battleTable.CastingSpell = 52;
                    break;
                case 52: //Thunder Attack
                    if (Emulator.Memory.ScreenFade == 3) {
                        Emulator.Memory.ScreenFade = 0;
                        previousElement = battleTable.Weapon_Element;
                        battleTable.Weapon_Element = 16;
                        Emulator.Memory.Battle.AutoDragoon1 = 4096;
                        //Emulator.Memory.Battle.AutoDragoon2 = 4096;
                        turnAutoOn = true;
                    }
                    break;
                case 86: //Dark Mist
                    if (Emulator.Memory.ScreenFade == 3)
                        Emulator.Memory.ScreenFade = 0;
                    break;
            }

            previousMP = currentMP;
        }

        private void WriteMP(Core.Memory.Battle.Character battleTable) {
            Core.Memory.Battle.DragoonSpell sparkNet = new Core.Memory.Battle.DragoonSpell(32);
            Core.Memory.Battle.DragoonSpell thunderAttack = new Core.Memory.Battle.DragoonSpell(33);
            sparkNet.MP = _sparkNetMP;
            thunderAttack.MP = _thunderAttackMP;

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

        private void NewSpells(Core.Memory.Battle.Character battleTable) {
            if (Emulator.Region == Region.NTA) { // TODO Remove Region check, when other encoding tables work.
                long text = Emulator.GetAddress("UNUSED_RAM_1");
                Emulator.DirectAccess.WriteText(text, "Spark Net<END>");
                if (thunderCharge == _maxThunderCharge) {
                    if (battleTable.SpellCast == 32 || battleTable.SpellCast == 86) {
                        if (Emulator.Memory.Battle.CursorTarget < 6 && Emulator.Memory.Battle.MonsterTable[Emulator.Memory.Battle.CursorTarget].Element == 16) {
                            Emulator.DirectAccess.WriteText(text + 0x40, "Thunder STR 1320%<END>");
                            thunderSelected = true;
                        } else {
                            Emulator.DirectAccess.WriteText(text + 0x40, "Thunder STR 440%<END>");
                            thunderSelected = false;
                        }
                    }
                } else {
                    Emulator.DirectAccess.WriteText(text + 0x40, "Thunder STR 150% TC " + thunderCharge + " <END>");
                }
                Emulator.DirectAccess.WriteText(text + 0x80, "Thunder Attack<END>");
                Emulator.DirectAccess.WriteText(text + 0xC0, "Physical Thunder STR 790%<END>");

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

        private void CheckElectricCharges() {
            byte player1Action;
            byte player2Action;
            byte player3Action;
            if (!thunderUsed) {
                switch (Emulator.Memory.Battle.CharacterTable.Length) {
                    case 3:
                        player1Action = Emulator.Memory.Battle.CharacterTable[0].Action;
                        player2Action = Emulator.Memory.Battle.CharacterTable[1].Action;
                        player3Action = Emulator.Memory.Battle.CharacterTable[2].Action;
                        if (player1Action == 24 && (player2Action == 16 || player2Action == 18 || player2Action == 208) && (player3Action == 16 || player3Action == 18 || player3Action == 208)) {
                            thunderUsed = true;
                        }
                        if (player2Action == 24 && (player1Action == 16 || player1Action == 18 || player1Action == 208) && (player3Action == 16 || player3Action == 18 || player3Action == 208)) {
                            thunderUsed = true;
                        }
                        if (player3Action == 24 && (player1Action == 16 || player1Action == 18 || player1Action == 208) && (player2Action == 16 || player2Action == 18 || player2Action == 208)) {
                            thunderUsed = true;
                        }
                        break;
                    case 2:
                        player1Action = Emulator.Memory.Battle.CharacterTable[0].Action;
                        player2Action = Emulator.Memory.Battle.CharacterTable[1].Action;
                        if (player1Action == 24 && (player2Action == 16 || player2Action == 18 || player2Action == 208)) {
                            thunderUsed = true;
                        }
                        if (player2Action == 24 && (player1Action == 16 || player1Action == 18 || player1Action == 208)) {
                            thunderUsed = true;
                        }
                        break;
                    default:
                        player1Action = Emulator.Memory.Battle.CharacterTable[0].Action;
                        if (player1Action == 24) {
                            thunderUsed = true;
                        }
                        break;
                }

                if (thunderUsed) {
                    AddThunderCharge(1);
                }
            } else {
                switch (Emulator.Memory.Battle.CharacterTable.Length) {
                    case 3:
                        player1Action = Emulator.Memory.Battle.CharacterTable[0].Action;
                        player2Action = Emulator.Memory.Battle.CharacterTable[1].Action;
                        player3Action = Emulator.Memory.Battle.CharacterTable[2].Action;
                        if (player1Action == 8 || player1Action == 10 || player2Action == 8 || player2Action == 10 || player3Action == 8 || player3Action == 10) {
                            thunderUsed = false;
                        }
                        break;
                    case 2:
                        player1Action = Emulator.Memory.Battle.CharacterTable[0].Action;
                        player2Action = Emulator.Memory.Battle.CharacterTable[1].Action;
                        if (player1Action == 8 || player1Action == 10 || player2Action == 8 || player2Action == 10) {
                            thunderUsed = false;
                        }
                        break;
                    default:
                        player1Action = Emulator.Memory.Battle.CharacterTable[0].Action;
                        if (player1Action == 8 || player1Action == 10) {
                            thunderUsed = false;
                        }
                        break;
                }
            }
        }

        private void AddThunderCharge(int charge) {
            thunderCharge = Math.Min(_maxThunderCharge, thunderCharge + charge);
        }
    }
}
