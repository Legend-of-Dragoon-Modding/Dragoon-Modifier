using Dragoon_Modifier.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Controller {
    internal static class Field {
        private static readonly ushort[] shopMaps = new ushort[] { 16, 23, 83, 84, 122, 145, 175, 180, 193, 204, 211, 214, 247,
        287, 309, 329, 332, 349, 357, 384, 435, 479, 515, 530, 564, 619, 624}; // Some maps missing?? 

        private static bool EarlyAdditionsSet = false;

        internal static void Setup() {
            Constants.UIControl.ResetBattle();
            EarlyAdditionsSet = false;

            if (Settings.Instance.DragoonStatChange) {
                Console.WriteLine("Changing Dragoon Stats...");
                long address = Emulator.GetAddress("DRAGOON_STAT_TABLE");
                int[] charReorder = new int[] { 5, 7, 0, 4, 6, 8, 1, 3, 2 };
                for (int character = 0; character < 8; character++) {
                    int reorderedChar = charReorder[character];
                    for (int level = 1; level < 6; level++) {
                        Emulator.DirectAccess.WriteUShort(address + character * 0x30 + level * 0x8, Settings.Instance.Dataset.DragoonStats[reorderedChar][level].MP);
                        Emulator.DirectAccess.WriteByte(address + character * 0x30 + level * 0x8 + 0x4, Settings.Instance.Dataset.DragoonStats[reorderedChar][level].DAT);
                        Emulator.DirectAccess.WriteByte(address + character * 0x30 + level * 0x8 + 0x5, Settings.Instance.Dataset.DragoonStats[reorderedChar][level].DMAT);
                        Emulator.DirectAccess.WriteByte(address + character * 0x30 + level * 0x8 + 0x6, Settings.Instance.Dataset.DragoonStats[reorderedChar][level].DDF);
                        Emulator.DirectAccess.WriteByte(address + character * 0x30 + level * 0x8 + 0x7, Settings.Instance.Dataset.DragoonStats[reorderedChar][level].DMDF);
                    }
                }
            }

            Settings.Instance.Dataset.Script.FieldSetup();
        }

        internal static void ItemSetup() {
            if (Settings.Instance.ItemIconChange) {
                Console.WriteLine("Changing Item Icons...");
                Item.IconChange();
            }

            if (Settings.Instance.ItemNameDescChange) {
                Console.WriteLine("Changing Item names and descriptions...");
                Item.FieldItemNameDescChange();
            }

            if (Settings.Instance.ItemStatChange) {
                Console.WriteLine("Changing Equipment stats...");
                Item.FieldEquipmentChange();
            }
        }

        internal static void AdditionSetup() {
            if (Settings.Instance.AdditionChange) {
                Console.WriteLine("Changing Additions...");
                Addition.MenuTableChange();
            }
        }

        internal static void Run() {
            if (Settings.Instance.AutoCharmPotion) {
                AutoCharmPotion();
            }

            if (Settings.Instance.SaveAnywhere) {
                SaveAnywhere();
            }

            if (Settings.Instance.IncreaseTextSpeed) {
                IncreaseTextSpeed();
            }

            if (Settings.Instance.AutoAdvanceText) {
                AutoText();
            }

            if (Settings.Instance.SoloMode) {
                SoloModeField();
            }

            if (Settings.Instance.DuoMode) {
                DuoModeField();
            }

            if (Settings.Instance.AlwaysAddSoloPartyMembers) {
                AddSoloPartyMembers();
            }

            if (Settings.Instance.EarlyAdditions && !EarlyAdditionsSet) {
                EarlyAdditions();
                EarlyAdditionsSet = true;
            }

            if (EarlyAdditionsSet && !Settings.Instance.EarlyAdditions) {
                TurnOffEarlyAdditions();
                EarlyAdditionsSet = false;
            }

            Constants.UIControl.UpdateField(Emulator.Memory.BattleValue, Emulator.Memory.EncounterID, Emulator.Memory.MapID);
        }

        private static void SoloModeField() {
            if (!Settings.Instance.AddSoloPartyMembers) {
                if (Emulator.DirectAccess.ReadByte("PARTY_SLOT", 0x4) != 255 || Emulator.DirectAccess.ReadByte("PARTY_SLOT", 0x8) != 255) {
                    for (int i = 0; i < 8; i++) {
                        Emulator.DirectAccess.WriteByte("PARTY_SLOT", 255, i + 0x4);
                    }
                    Emulator.DirectAccess.WriteByte("CHAR_TABLE", 3, Emulator.DirectAccess.ReadByte("PARTY_SLOT") * 0x2C + 0x4);
                }
            }
        }

        private static void DuoModeField() {
            if (!Settings.Instance.AddSoloPartyMembers) {
                if (Emulator.DirectAccess.ReadByte("PARTY_SLOT", 0x4) == 255) {
                    Emulator.DirectAccess.WriteByte("PARTY_SLOT", Emulator.DirectAccess.ReadByte("PARTY_SLOT"), 0x4);
                    Emulator.DirectAccess.WriteByte("PARTY_SLOT", 0, 0x5);
                    Emulator.DirectAccess.WriteByte("PARTY_SLOT", 0, 0x6);
                    Emulator.DirectAccess.WriteByte("PARTY_SLOT", 0, 0x7);
                    Emulator.DirectAccess.WriteByte("CHAR_TABLE", 3, Emulator.DirectAccess.ReadByte("PARTY_SLOT") * 0x2C + 0x4);
                }

                if (Emulator.DirectAccess.ReadByte("PARTY_SLOT", 0x8) != 255) {
                    for (int i = 0; i < 4; i++) {
                        Emulator.DirectAccess.WriteByte("PARTY_SLOT", 255, i + 0x8);
                    }
                }
            }
        }

        private static void AddSoloPartyMembers() {
            if (Settings.Instance.SoloMode && Emulator.Memory.PartySlot[1] > 8) {
                Settings.Instance.AddSoloPartyMembers = true;
                Emulator.DirectAccess.WriteByte("PARTY_SLOT", Emulator.DirectAccess.ReadByte("PARTY_SLOT"), 0x4);
                Emulator.DirectAccess.WriteByte("PARTY_SLOT", 0, 0x5);
                Emulator.DirectAccess.WriteByte("PARTY_SLOT", 0, 0x6);
                Emulator.DirectAccess.WriteByte("PARTY_SLOT", 0, 0x7);
                Emulator.DirectAccess.WriteByte("PARTY_SLOT", Emulator.DirectAccess.ReadByte("PARTY_SLOT"), 0x8);
                Emulator.DirectAccess.WriteByte("PARTY_SLOT", 0, 0x9);
                Emulator.DirectAccess.WriteByte("PARTY_SLOT", 0, 0xA);
                Emulator.DirectAccess.WriteByte("PARTY_SLOT", 0, 0xB);
            } else if (Settings.Instance.DuoMode && Emulator.Memory.PartySlot[2] > 8) {
                Settings.Instance.AddSoloPartyMembers = true;
                Emulator.DirectAccess.WriteByte("PARTY_SLOT", Emulator.DirectAccess.ReadByte("PARTY_SLOT"), 0x8);
                Emulator.DirectAccess.WriteByte("PARTY_SLOT", 0, 0x9);
                Emulator.DirectAccess.WriteByte("PARTY_SLOT", 0, 0xA);
                Emulator.DirectAccess.WriteByte("PARTY_SLOT", 0, 0xB);
            }
        }

        private static void EarlyAdditions() {
            long address = Emulator.GetAddress("MENU_ADDITION_TABLE_FLAT");
            long address2 = Emulator.GetAddress("CHAR_TABLE") + 0x22;
            //Dart
            Emulator.DirectAccess.WriteByte(address + 0xE * 3, 13); //Crush Dance
            Emulator.DirectAccess.WriteByte(address + 0xE * 4, 18); //Madness Hero
            Emulator.DirectAccess.WriteByte(address + 0xE * 5, 23); //Moon Strike
            Emulator.DirectAccess.WriteByte(address + 0xE * 6, 60); //Blazying Dynamo
            if (Emulator.DirectAccess.ReadByte(address2) >= 80 &&
                Emulator.DirectAccess.ReadByte(address2 + 0x1) >= 80 &&
                Emulator.DirectAccess.ReadByte(address2 + 0x2) >= 80 &&
                Emulator.DirectAccess.ReadByte(address2 + 0x3) >= 80 &&
                Emulator.DirectAccess.ReadByte(address2 + 0x4) >= 80 &&
                Emulator.DirectAccess.ReadByte(address2 + 0x5) >= 80) {
                Emulator.DirectAccess.WriteByte(address + 0xE * 6, 29); //Blazying Dynamo
            }
            //Lavitz & Albert
            Emulator.DirectAccess.WriteByte(address + 0xE * 2 + 0x70, 10); //Rod Typhoon
            Emulator.DirectAccess.WriteByte(address + 0xE * 3 + 0x70, 16); //Gust of Wind Dance
            Emulator.DirectAccess.WriteByte(address + 0xE * 4 + 0x70, 60); //Flower Storm

            if ((Emulator.DirectAccess.ReadByte(address2 + 0x2C) >= 80 &&
                Emulator.DirectAccess.ReadByte(address2 + 0x2C + 0x1) >= 80 &&
                Emulator.DirectAccess.ReadByte(address2 + 0x2C + 0x2) >= 80 &&
                Emulator.DirectAccess.ReadByte(address2 + 0x2C + 0x3) >= 80) || 
                (Emulator.DirectAccess.ReadByte(address2 + 0xDC) >= 80 &&
                Emulator.DirectAccess.ReadByte(address2 + 0xDC + 0x1) >= 80 &&
                Emulator.DirectAccess.ReadByte(address2 + 0xDC + 0x2) >= 80 &&
                Emulator.DirectAccess.ReadByte(address2 + 0xDC + 0x3) >= 80)) {
                Emulator.DirectAccess.WriteByte(address + 0xE * 4 + 0x70, 21); //Flower Storm
            }

            //Rose
            Emulator.DirectAccess.WriteByte(address + 0xE * 1 + 0xC4, 8); //More & More
            Emulator.DirectAccess.WriteByte(address + 0xE * 2 + 0xC4, 15); //Hard Blade
            Emulator.DirectAccess.WriteByte(address + 0xE * 3 + 0xC4, 60); //Demon's Dance
            if (Emulator.DirectAccess.ReadByte(address2 + 0x84) >= 80 &&
                Emulator.DirectAccess.ReadByte(address2 + 0x84 + 0x1) >= 80 &&
                Emulator.DirectAccess.ReadByte(address2 + 0x84 + 0x2) >= 80) {
                Emulator.DirectAccess.WriteByte(address + 0xE * 3 + 0xC4, 21); //Demon's Dance
            }
            //Kongol
            Emulator.DirectAccess.WriteByte(address + 0xE * 1 + 0x10A, 10); //Inferno
            Emulator.DirectAccess.WriteByte(address + 0xE * 2 + 0x10A, 60); //Bone Crush
            if (Emulator.DirectAccess.ReadByte(address2 + 0x134) >= 80 &&
                Emulator.DirectAccess.ReadByte(address2 + 0x134 + 0x1) >= 80) {
                Emulator.DirectAccess.WriteByte(address + 0xE * 2 + 0x10A, 20); //Bone Crush
            }
            //Meru
            Emulator.DirectAccess.WriteByte(address + 0xE * 1 + 0x142, 6); //Hammer Spin
            Emulator.DirectAccess.WriteByte(address + 0xE * 2 + 0x142, 12); //Cool Boogie
            Emulator.DirectAccess.WriteByte(address + 0xE * 3 + 0x142, 18); //Cat's Cradle
            Emulator.DirectAccess.WriteByte(address + 0xE * 4 + 0x142, 60); //Perky Step
            if (Emulator.DirectAccess.ReadByte(address2 + 0x108) >= 80 &&
                Emulator.DirectAccess.ReadByte(address2 + 0x108 + 0x1) >= 80 &&
                Emulator.DirectAccess.ReadByte(address2 + 0x108 + 0x2) >= 80 &&
                Emulator.DirectAccess.ReadByte(address2 + 0x108 + 0x3) >= 80) {
                Emulator.DirectAccess.WriteByte(address + 0xE * 4 + 0x142, 22);  //Perky Step
            }
            //Haschel
            Emulator.DirectAccess.WriteByte(address + 0xE * 1 + 0x196, 5); // Flurry of Styx
            Emulator.DirectAccess.WriteByte(address + 0xE * 2 + 0x196, 10); //Summon 4 Gods
            Emulator.DirectAccess.WriteByte(address + 0xE * 3 + 0x196, 16); //5 Ring Shattering
            Emulator.DirectAccess.WriteByte(address + 0xE * 4 + 0x196, 22); //Hex Hammer
            Emulator.DirectAccess.WriteByte(address + 0xE * 5 + 0x196, 60); //Omni-Sweep        
            if (Emulator.DirectAccess.ReadByte(address2 + 0xB0) >= 80 &&
                Emulator.DirectAccess.ReadByte(address2 + 0xB0 + 0x1) >= 80 &&
                Emulator.DirectAccess.ReadByte(address2 + 0xB0 + 0x2) >= 80 &&
                Emulator.DirectAccess.ReadByte(address2 + 0xB0 + 0x3) >= 80 &&
                Emulator.DirectAccess.ReadByte(address2 + 0xB0 + 0x4) >= 80) {
                Emulator.DirectAccess.WriteByte(address + 0xE * 5 + 0x196, 25); //Omni-Sweep 
            }
        }

        private static void TurnOffEarlyAdditions() {
            long address = Emulator.GetAddress("MENU_ADDITION_TABLE_FLAT");
            //Dart
            Emulator.DirectAccess.WriteByte(address * 0xE * 3, 15); //Crush Dance
            Emulator.DirectAccess.WriteByte(address * 0xE * 4, 22); //Madness Hero
            Emulator.DirectAccess.WriteByte(address * 0xE * 5, 29); //Moon Strike
            Emulator.DirectAccess.WriteByte(address * 0xE * 6, 255); //Blazying Dynamo
            //Lavitz
            Emulator.DirectAccess.WriteByte(address * 0xE * 2 + 0x70, 7); //Rod Typhoon
            Emulator.DirectAccess.WriteByte(address * 0xE * 3 + 0x70, 11); //Gust of Wind Dance
            Emulator.DirectAccess.WriteByte(address * 0xE * 4 + 0x70, 255); //Flower Storm
            //Rose
            Emulator.DirectAccess.WriteByte(address + 0xE * 1 + 0xC4, 14); //More & More
            Emulator.DirectAccess.WriteByte(address + 0xE * 2 + 0xC4, 19); //Hard Blade
            Emulator.DirectAccess.WriteByte(address + 0xE * 3 + 0xC4, 255); //Demon//s Dance
            //Kongol
            Emulator.DirectAccess.WriteByte(address + 0xE * 1 + 0x10A, 23); //Inferno
            Emulator.DirectAccess.WriteByte(address + 0xE * 2 + 0x10A, 255); //Bone Crush
            //Meru
            Emulator.DirectAccess.WriteByte(address * 0xE * 1 + 0x142, 21); //Hammer Spin
            Emulator.DirectAccess.WriteByte(address * 0xE * 2 + 0x142, 26); //Cool Boogie
            Emulator.DirectAccess.WriteByte(address * 0xE * 3 + 0x142, 30); //Cat//s Cradle
            Emulator.DirectAccess.WriteByte(address * 0xE * 4 + 0x142, 255); //Perky Step
            //Haschel
            Emulator.DirectAccess.WriteByte(address * 0xE * 1 + 0x196, 14); // Flurry of Styx
            Emulator.DirectAccess.WriteByte(address * 0xE * 2 + 0x196, 18); //Summon 4 Gods
            Emulator.DirectAccess.WriteByte(address * 0xE * 3 + 0x196, 22); //5 Ring Shattering
            Emulator.DirectAccess.WriteByte(address * 0xE * 4 + 0x196, 26); //Hex Hammer
            Emulator.DirectAccess.WriteByte(address * 0xE * 5 + 0x196, 255); //Omni-Sweep   
        }

        private static void AutoText() {
            if (Emulator.Memory.AutoText != 13378) {
                Emulator.Memory.AutoText = 13378;
            }
        }

        private static void IncreaseTextSpeed() {
            if (Emulator.Memory.TextSpeed != 1) {
                Emulator.Memory.TextSpeed = 1;
            }
        }

        private static void SaveAnywhere() {
            if (Emulator.Memory.SavePoint == 0) {
                Emulator.Memory.SavePoint = 1;
            }
        }

        private static void AutoCharmPotion() {
            if (Emulator.Memory.BattleValue > 3850 && Emulator.Memory.Gold >= 8) {
                Emulator.Memory.Gold -= 8;
                Emulator.Memory.BattleValue = 0;
            }
        }

        private static void ThrownItemChange() {
            for (int i = 192; i < 255; i++) {
                var item = (Dataset.IUsableItem) Settings.Instance.Dataset.Item[i];
                var mem = (Core.Memory.IUsableItem) Emulator.Memory.Item[i];
                mem.Target = item.Target;
                mem.Element = item.Element;
                mem.Damage = item.Damage;
                mem.Special1 = item.Special1;
                mem.Special2 = item.Special2;
                mem.Unknown1 = item.Unknown1;
                mem.SpecialAmmount = item.SpecialAmmount;
                mem.Status = item.Status;
                mem.Percentage = item.Percentage;
                mem.Unknown2 = item.Unknown2;
                mem.BaseSwitch = item.BaseSwitch;
            }
        }

        private static void ItemNameDescChange() {
            Console.WriteLine("Changing Item Names and Descriptions...");

            int address = Emulator.GetAddress("ITEM_NAME");
            int address2 = Emulator.GetAddress("ITEM_DESC");
            Emulator.DirectAccess.WriteAoB(address, Settings.Instance.Dataset.ItemNames);
            Emulator.DirectAccess.WriteAoB(address2, Settings.Instance.Dataset.ItemDescriptions);

            for (int i = 0; i < Emulator.Memory.Item.Length; i++) {
                Emulator.Memory.Item[i].NamePointer = (uint) Settings.Instance.Dataset.Item[i].NamePointer;
                Emulator.Memory.Item[i].DescriptionPointer = (uint) Settings.Instance.Dataset.Item[i].DescriptionPointer;
            }
        }
    }
}
