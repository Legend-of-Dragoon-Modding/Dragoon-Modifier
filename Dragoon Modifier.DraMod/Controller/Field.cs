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

        //Early Additions
        static bool earlyAdditionsOnFieldEntry = false;

        internal static void Setup(Emulator.IEmulator emulator, LoDDict.ILoDDictionary LoDDict, UI.IUIControl uiControl) {
            uiControl.ResetBattle();

            earlyAdditionsOnFieldEntry = false;

            if (Settings.ItemIconChange) {
                ItemIconChange(emulator, LoDDict);
            }

            if (Settings.ItemNameDescChange) {
                ItemNameDescChange(emulator, LoDDict);
            }

            if (Settings.ItemStatChange) {
                ItemStatChange(emulator, LoDDict);
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

            if (Settings.EarlyAdditions) {
                if (!earlyAdditionsOnFieldEntry && emulator.Memory.GameState == GameState.Field) {
                    EarlyAdditions(emulator);
                }
            } else {
                if (earlyAdditionsOnFieldEntry && emulator.Memory.GameState == GameState.Field) {
                    TurnOffEarlyAdditions(emulator);
                }
            }

            uiControl.UpdateField(emulator.Memory.BattleValue, emulator.Memory.EncounterID, emulator.Memory.MapID);
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
            earlyAdditionsOnFieldEntry = true;
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
            earlyAdditionsOnFieldEntry = false;
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

        private static void ItemStatChange(Emulator.IEmulator emulator, LoDDict.ILoDDictionary LoDDict) {
            Console.WriteLine("Changing Item Stats...");
            for (int i = 0; i < 192; i++) {
                var equip = (LoDDict.IEquipment) LoDDict.Item[i];
                var mem = (Emulator.Memory.IEquipment) emulator.Memory.Item[i];
                mem.WhoEquips = equip.WhoEquips;
                mem.ItemType = equip.Type;
                mem.WeaponElement = equip.WeaponElement;
                mem.Status = equip.OnHitStatus;
                mem.StatusChance = equip.OnHitStatusChance;
                mem.AT = (byte) Math.Min(equip.AT, (ushort) 255);
                mem.AT2 = (byte) Math.Max(Math.Min(equip.AT - 255, 255), 0);
                mem.MAT = equip.MAT;
                mem.DF = equip.DF;
                mem.MDF = equip.MDF;
                mem.SPD = equip.SPD;
                mem.A_HIT = equip.A_HIT;
                mem.M_HIT = equip.M_HIT;
                mem.A_AV = equip.A_AV;
                mem.M_AV = equip.M_AV;
                mem.E_Half = equip.ElementalResistance;
                mem.E_Immune = equip.ElementalImmunity;
                mem.StatusResist = equip.StatusResistance;
                mem.Special1 = equip.SpecialBonus1;
                mem.Special2 = equip.SpecialBonus2;
                mem.SpecialAmmount = equip.SpecialBonusAmmount;
                mem.SpecialEffect = equip.SpecialEffect;
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

        private static void ItemIconChange(Emulator.IEmulator emulator, LoDDict.ILoDDictionary LoDDict) {
            Console.WriteLine("Changing Item Icons...");
            for (int i = 0; i < emulator.Memory.Item.Length; i++) {
                emulator.Memory.Item[i].Icon = LoDDict.Item[i].Icon;
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
