using Dragoon_Modifier.Emulator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Controller {
    internal static class Field {
        private static readonly ushort[] shopMaps = new ushort[] { 16, 23, 83, 84, 122, 145, 175, 180, 193, 204, 211, 214, 247,
        287, 309, 329, 332, 349, 357, 384, 435, 479, 515, 530, 564, 619, 624}; // Some maps missing?? 

        internal static void Setup(Emulator.IEmulator emulator, LoDDict.ILoDDictionary LoDDict, UI.IUIControl uiControl) {
            uiControl.ResetBattle();


            if (Settings.ItemIconChange) {
                Console.WriteLine("Changing Item Icons...");
                Item.IconChange(emulator, LoDDict);
            }

            if (Settings.ItemNameDescChange) {
                Console.WriteLine("Changing Item names and descriptions...");
                Item.FieldItemNameDescChange(emulator, LoDDict);
            }

            if (Settings.ItemStatChange) {
                Console.WriteLine("Changing Equipment stats...");
                Item.FieldEquipmentChange(emulator, LoDDict);
            }

            if (Settings.AdditionChange) {
                Console.WriteLine("Changing Additions...");
                Addition.MenuTableChange(emulator, LoDDict);
            }

            if (Settings.EarlyAdditions) {
                EarlyAdditions(emulator);
            }
            
        }

        internal static void Run(Emulator.IEmulator emulator, UI.IUIControl uiControl) {
            if (Settings.AutoCharmPotion) {
                AutoCharmPotion(emulator);
            }

            if (Settings.SaveAnywhere) {
                SaveAnywhere(emulator);
            }

            if (Settings.IncreaseTextSpeed) {
                IncreaseTextSpeed(emulator);
            }

            if (Settings.AutoAdvanceText) {
                AutoText(emulator);
            }

            if (Settings.SoloMode) {
                SoloModeField(emulator);
            }

            if (Settings.DuoMode) {
                DuoModeField(emulator);
            }

            if (Settings.AlwaysAddSoloPartyMembers) {
                AddSoloPartyMembers(emulator);
            }

            uiControl.UpdateField(emulator.Memory.BattleValue, emulator.Memory.EncounterID, emulator.Memory.MapID);
        }

        private static void SoloModeField(Emulator.IEmulator emulator) {
            if (!Settings.AddSoloPartyMembers) {
                if (emulator.ReadByte("PARTY_SLOT", 0x4) != 255 || emulator.ReadByte("PARTY_SLOT", 0x8) != 255) {
                    for (int i = 0; i < 8; i++) {
                        emulator.WriteByte("PARTY_SLOT", 255, i + 0x4);
                    }
                    emulator.WriteByte("CHAR_TABLE", 3, emulator.ReadByte("PARTY_SLOT") * 0x2C + 0x4);
                }
            }
        }

        private static void DuoModeField(Emulator.IEmulator emulator) {
            if (!Settings.AddSoloPartyMembers) {
                if (emulator.ReadByte("PARTY_SLOT", 0x4) == 255) {
                    emulator.WriteByte("PARTY_SLOT", emulator.ReadByte("PARTY_SLOT"), 0x4);
                    emulator.WriteByte("PARTY_SLOT", 0, 0x5);
                    emulator.WriteByte("PARTY_SLOT", 0, 0x6);
                    emulator.WriteByte("PARTY_SLOT", 0, 0x7);
                    emulator.WriteByte("CHAR_TABLE", 3, emulator.ReadByte("PARTY_SLOT") * 0x2C + 0x4);
                }

                if (emulator.ReadByte("PARTY_SLOT", 0x8) != 255) {
                    for (int i = 0; i < 4; i++) {
                        emulator.WriteByte("PARTY_SLOT", 255, i + 0x8);
                    }
                }
            }
        }

        private static void AddSoloPartyMembers(Emulator.IEmulator emulator) {
            if (Settings.SoloMode && emulator.Memory.PartySlot[1] > 8) {
                Settings.AddSoloPartyMembers = true;
                emulator.WriteByte("PARTY_SLOT", emulator.ReadByte("PARTY_SLOT"), 0x4);
                emulator.WriteByte("PARTY_SLOT", 0, 0x5);
                emulator.WriteByte("PARTY_SLOT", 0, 0x6);
                emulator.WriteByte("PARTY_SLOT", 0, 0x7);
                emulator.WriteByte("PARTY_SLOT", emulator.ReadByte("PARTY_SLOT"), 0x8);
                emulator.WriteByte("PARTY_SLOT", 0, 0x9);
                emulator.WriteByte("PARTY_SLOT", 0, 0xA);
                emulator.WriteByte("PARTY_SLOT", 0, 0xB);
            } else if (Settings.DuoMode && emulator.Memory.PartySlot[2] > 8) {
                Settings.AddSoloPartyMembers = true;
                emulator.WriteByte("PARTY_SLOT", emulator.ReadByte("PARTY_SLOT"), 0x8);
                emulator.WriteByte("PARTY_SLOT", 0, 0x9);
                emulator.WriteByte("PARTY_SLOT", 0, 0xA);
                emulator.WriteByte("PARTY_SLOT", 0, 0xB);
            }
        }

        private static void EarlyAdditions(Emulator.IEmulator emulator) {
            long address = emulator.GetAddress("MENU_ADDITION_TABLE_FLAT");
            long address2 = emulator.GetAddress("CHAR_TABLE") + 0x22;
            //Dart
            emulator.WriteByte(address + 0xE * 3, 13); //Crush Dance
            emulator.WriteByte(address + 0xE * 4, 18); //Madness Hero
            emulator.WriteByte(address + 0xE * 5, 23); //Moon Strike
            emulator.WriteByte(address + 0xE * 6, 60); //Blazying Dynamo
            if (emulator.ReadByte(address2) >= 80 &&
                emulator.ReadByte(address2 + 0x1) >= 80 &&
                emulator.ReadByte(address2 + 0x2) >= 80 &&
                emulator.ReadByte(address2 + 0x3) >= 80 &&
                emulator.ReadByte(address2 + 0x4) >= 80 &&
                emulator.ReadByte(address2 + 0x5) >= 80) {
                emulator.WriteByte(address + 0xE * 6, 29); //Blazying Dynamo
            }
            //Lavitz
            emulator.WriteByte(address + 0xE * 2 + 0x70, 10); //Rod Typhoon
            emulator.WriteByte(address + 0xE * 3 + 0x70, 16); //Gust of Wind Dance
            emulator.WriteByte(address + 0xE * 4 + 0x70, 60); //Flower Storm
            if (emulator.ReadByte(address2 + 0x2C) >= 80 &&
                emulator.ReadByte(address2 + 0x2C + 0x1) >= 80 &&
                emulator.ReadByte(address2 + 0x2C + 0x2) >= 80 &&
                emulator.ReadByte(address2 + 0x2C + 0x3) >= 80) {
                emulator.WriteByte(address + 0xE * 4 + 0x70, 21); //Flower Storm
            }
            //Albert
            emulator.WriteByte(address + 0xE * 2 + 0x70, 10); //Rod Typhoon
            emulator.WriteByte(address + 0xE * 3 + 0x70, 16); //Gust of Wind Dance
            emulator.WriteByte(address + 0xE * 4 + 0x70, 60); //Flower Storm
            if (emulator.ReadByte(address2 + 0xDC) >= 80 &&
                emulator.ReadByte(address2 + 0xDC + 0x1) >= 80 &&
                emulator.ReadByte(address2 + 0xDC + 0x2) >= 80 &&
                emulator.ReadByte(address2 + 0xDC + 0x3) >= 80) {
                emulator.WriteByte(address + 0xE * 4 + 0x70, 21); //Flower Storm
            }
            //Rose
            emulator.WriteByte(address + 0xE * 1 + 0xC4, 8); //More & More
            emulator.WriteByte(address + 0xE * 2 + 0xC4, 15); //Hard Blade
            emulator.WriteByte(address + 0xE * 3 + 0xC4, 60); //Demon's Dance
            if (emulator.ReadByte(address2 + 0x84) >= 80 &&
                emulator.ReadByte(address2 + 0x84 + 0x1) >= 80 &&
                emulator.ReadByte(address2 + 0x84 + 0x2) >= 80) {
                emulator.WriteByte(address + 0xE * 3 + 0xC4, 21); //Demon's Dance
            }
            //Kongol
            emulator.WriteByte(address + 0xE * 1 + 0x10A, 10); //Inferno
            emulator.WriteByte(address + 0xE * 2 + 0x10A, 60); //Bone Crush
            if (emulator.ReadByte(address2 + 0x134) >= 80 &&
                emulator.ReadByte(address2 + 0x134 + 0x1) >= 80) {
                emulator.WriteByte(address + 0xE * 2 + 0x10A, 20); //Bone Crush
            }
            //Meru
            emulator.WriteByte(address + 0xE * 1 + 0x142, 6); //Hammer Spin
            emulator.WriteByte(address + 0xE * 2 + 0x142, 12); //Cool Boogie
            emulator.WriteByte(address + 0xE * 3 + 0x142, 18); //Cat's Cradle
            emulator.WriteByte(address + 0xE * 4 + 0x142, 60); //Perky Step
            if (emulator.ReadByte(address2 + 0x108) >= 80 &&
                emulator.ReadByte(address2 + 0x108 + 0x1) >= 80 &&
                emulator.ReadByte(address2 + 0x108 + 0x2) >= 80 &&
                emulator.ReadByte(address2 + 0x108 + 0x3) >= 80) {
                emulator.WriteByte(address + 0xE * 4 + 0x142, 22);  //Perky Step
            }
            //Haschel
            emulator.WriteByte(address + 0xE * 1 + 0x196, 5); // Flurry of Styx
            emulator.WriteByte(address + 0xE * 2 + 0x196, 10); //Summon 4 Gods
            emulator.WriteByte(address + 0xE * 3 + 0x196, 16); //5 Ring Shattering
            emulator.WriteByte(address + 0xE * 4 + 0x196, 22); //Hex Hammer
            emulator.WriteByte(address + 0xE * 5 + 0x196, 60); //Omni-Sweep        
            if (emulator.ReadByte(address2 + 0xB0) >= 80 &&
                emulator.ReadByte(address2 + 0xB0 + 0x1) >= 80 &&
                emulator.ReadByte(address2 + 0xB0 + 0x2) >= 80 &&
                emulator.ReadByte(address2 + 0xB0 + 0x3) >= 80 &&
                emulator.ReadByte(address2 + 0xB0 + 0x4) >= 80) {
                emulator.WriteByte(address + 0xE * 5 + 0x196, 25); //Omni-Sweep 
            }
        }

        private static void TurnOffEarlyAdditions(Emulator.IEmulator emulator) {
            long address = emulator.GetAddress("MENU_ADDITION_TABLE_FLAT");
            //Dart
            emulator.WriteByte(address * 0xE * 3, 15); //Crush Dance
            emulator.WriteByte(address * 0xE * 4, 22); //Madness Hero
            emulator.WriteByte(address * 0xE * 5, 29); //Moon Strike
            emulator.WriteByte(address * 0xE * 6, 255); //Blazying Dynamo
            //Lavitz
            emulator.WriteByte(address * 0xE * 2 + 0x70, 7); //Rod Typhoon
            emulator.WriteByte(address * 0xE * 3 + 0x70, 11); //Gust of Wind Dance
            emulator.WriteByte(address * 0xE * 4 + 0x70, 255); //Flower Storm
            //Rose
            emulator.WriteByte(address + 0xE * 1 + 0xC4, 14); //More & More
            emulator.WriteByte(address + 0xE * 2 + 0xC4, 19); //Hard Blade
            emulator.WriteByte(address + 0xE * 3 + 0xC4, 255); //Demon//s Dance
            //Kongol
            emulator.WriteByte(address + 0xE * 1 + 0x10A, 23); //Inferno
            emulator.WriteByte(address + 0xE * 2 + 0x10A, 255); //Bone Crush
            //Meru
            emulator.WriteByte(address * 0xE * 1 + 0x142, 21); //Hammer Spin
            emulator.WriteByte(address * 0xE * 2 + 0x142, 26); //Cool Boogie
            emulator.WriteByte(address * 0xE * 3 + 0x142, 30); //Cat//s Cradle
            emulator.WriteByte(address * 0xE * 4 + 0x142, 255); //Perky Step
            //Haschel
            emulator.WriteByte(address * 0xE * 1 + 0x196, 14); // Flurry of Styx
            emulator.WriteByte(address * 0xE * 2 + 0x196, 18); //Summon 4 Gods
            emulator.WriteByte(address * 0xE * 3 + 0x196, 22); //5 Ring Shattering
            emulator.WriteByte(address * 0xE * 4 + 0x196, 26); //Hex Hammer
            emulator.WriteByte(address * 0xE * 5 + 0x196, 255); //Omni-Sweep   
        }

        private static void AutoText(Emulator.IEmulator emulator) {
            if (emulator.Memory.AutoText != 13378) {
                emulator.Memory.AutoText = 13378;
            }
        }

        private static void IncreaseTextSpeed(Emulator.IEmulator emulator) {
            if (emulator.Memory.TextSpeed != 1) {
                emulator.Memory.TextSpeed = 1;
            }
        }

        private static void SaveAnywhere(Emulator.IEmulator emulator) {
            if (emulator.Memory.SavePoint == 0) {
                emulator.Memory.SavePoint = 1;
            }
        }

        private static void AutoCharmPotion(Emulator.IEmulator emulator) {
            if (emulator.Memory.BattleValue > 3850 && emulator.Memory.Gold >= 8) {
                emulator.Memory.Gold -= 8;
                emulator.Memory.BattleValue = 0;
            }
        }

        private static void ThrownItemChange(Emulator.IEmulator emulator, LoDDict.ILoDDictionary LoDDict) {
            for (int i = 192; i < 255; i++) {
                var item = (LoDDict.IUsableItem) LoDDict.Item[i];
                var mem = (Emulator.Memory.IUsableItem) emulator.Memory.Item[i];
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

        private static void ItemNameDescChange(Emulator.IEmulator emulator, LoDDict.ILoDDictionary LoDDict) {
            Console.WriteLine("Changing Item Names and Descriptions...");

            int address = emulator.GetAddress("ITEM_NAME");
            int address2 = emulator.GetAddress("ITEM_DESC");
            emulator.WriteAoB(address, LoDDict.ItemNames);
            emulator.WriteAoB(address2, LoDDict.ItemDescriptions);

            for (int i = 0; i < emulator.Memory.Item.Length; i++) {
                emulator.Memory.Item[i].NamePointer = (uint) LoDDict.Item[i].NamePointer;
                emulator.Memory.Item[i].DescriptionPointer = (uint) LoDDict.Item[i].DescriptionPointer;
            }
        }
    }
}
