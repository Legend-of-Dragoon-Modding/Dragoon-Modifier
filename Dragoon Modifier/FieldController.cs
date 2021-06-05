using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier {
    public static class FieldController {
        static readonly ushort[] shopMaps = new ushort[] { 16, 23, 83, 84, 122, 145, 175, 180, 193, 204, 211, 214, 247,
        287, 309, 329, 332, 349, 357, 384, 435, 479, 515, 530, 564, 619, 624}; // Some maps missing??



        static bool shopDiscSwap = false;
        static bool shopListChanged = false;


        public static void Field() {
            try {
                if (GameController.StatsChanged) {
                    ItemChange();
                    GameController.StatsChanged = false;
                }
                if (GameController.InventorySize != 32) {
                    ExtendInventory(GameController.InventorySize);
                }

                if (Globals.SHOP_CHANGE) {
                    ShopTableChange();
                }

                if (UIControls.SaveAnywhere) {

                }

                if (UIControls.SoloMode) {

                }

                if (UIControls.DuoMode) {

                }

                if (UIControls.HPCapBreak) {

                }

                if (UIControls.KillBGM) {

                }

                if (UIControls.AutoCharmPotion) {

                }

                if (UIControls.EarlyAdditions) {

                }

                // UltimateBossFiled

                if (UIControls.IncreaseTextSpeed) {

                }

                if (UIControls.AutoText) {

                }

            } catch (Exception ex) {
                Constants.RUN = false;
                Constants.WriteGLog("Program stopped.");
                Constants.WritePLogOutput("INTERNAL FIELD SCRIPT ERROR");
                Constants.WriteOutput("Fatal Error. Closing all threads. Please see error log in Settings console.");
                Constants.WriteError(ex.ToString());
            }
        }

        public static void Overworld() {
            if (GameController.InventorySize != 32) {
                ExtendInventory(GameController.InventorySize);
            }

            if (UIControls.SoloMode) {

            }

            if (UIControls.DuoMode) {

            }

            if (UIControls.HPCapBreak) {

            }

            if (UIControls.KillBGM) {

            }

            if (UIControls.AutoCharmPotion) {

            }

            if (UIControls.EarlyAdditions) {

            }

            // UltimateBossFiled
        }

        static void ExtendInventory(byte inventorySize) { // TODO account for UltimateBossDefeatCheck

        }

        static void ShopTableChange() {
            if (!shopListChanged && shopMaps.Contains(Emulator.MemoryController.MapID)) {
                if (Emulator.MemoryController.Transition != 12) { // Map transition in progress
                    return;
                }
                // TODO run
                return;
            }
            shopListChanged = false;
        }

        static void ItemChange() {
            if (Globals.ITEM_ICON_CHANGE) {
                ItemIconChange();
            }
            if (Globals.ITEM_NAMEDESC_CHANGE) {
                ItemNameDescChange();
            }
            if (Globals.ITEM_STAT_CHANGE) {
                ItemStatChange();
            }
            if (Globals.THROWN_ITEM_CHANGE) {
                ThrownItemChange();
            }
        }

        static void ItemIconChange() {
            Constants.WriteOutput("Changing Item Icons...");
            for (int i = 0; i < Emulator.MemoryController.EquipmentTable.Length; i++) {
                Emulator.MemoryController.EquipmentTable[i].Icon = LoDDictionary.Dictionary.Items[i].Icon;
            }
            for (int i = 0; i < Emulator.MemoryController.UsableItemTable.Length; i++) {
                Emulator.MemoryController.UsableItemTable[i].Icon = LoDDictionary.Dictionary.Items[i + 192].Icon;
            }
        }

        static void ItemNameDescChange() {
            Constants.WriteOutput("Changing Item Names and Descriptions...");

            int address = Emulator.GetAddress("ITEM_NAME");
            int address2 = Emulator.GetAddress("ITEM_DESC");
            Emulator.WriteAoB(address, LoDDictionary.Dictionary.EncodedNames);
            Emulator.WriteAoB(address2, LoDDictionary.Dictionary.EncodedDescriptions);
 

            for (int i = 0; i < Emulator.MemoryController.EquipmentTable.Length; i++) {
                Emulator.MemoryController.EquipmentTable[i].NamePointer = (uint) LoDDictionary.Dictionary.Items[i].NamePointer;
                Emulator.MemoryController.EquipmentTable[i].DescriptionPointer = (uint) LoDDictionary.Dictionary.Items[i].DescriptionPointer;
            }

            for (int i = 0; i < Emulator.MemoryController.UsableItemTable.Length; i++) {
                Emulator.MemoryController.UsableItemTable[i].NamePointer = (uint) LoDDictionary.Dictionary.Items[i + 192].NamePointer;
                Emulator.MemoryController.UsableItemTable[i].DescriptionPointer = (uint) LoDDictionary.Dictionary.Items[i+ 192].DescriptionPointer;
            }
        }

        static void ItemStatChange() {
            for (int i = 0; i < 192; i++) {
                var equip = (LoDDictionary.Equipment) LoDDictionary.Dictionary.Items[i];
                Emulator.MemoryController.EquipmentTable[i].WhoEquips = equip.WhoEquips;
                Emulator.MemoryController.EquipmentTable[i].ItemType = equip.Type;
                Emulator.MemoryController.EquipmentTable[i].WeaponElement = equip.WeaponElement;
                Emulator.MemoryController.EquipmentTable[i].Status = equip.OnHitStatus;
                Emulator.MemoryController.EquipmentTable[i].StatusChance = equip.OnHitStatusChance;
                Emulator.MemoryController.EquipmentTable[i].AT = (byte) Math.Min(equip.AT, (ushort) 255);
                Emulator.MemoryController.EquipmentTable[i].AT2 = (byte) Math.Max(Math.Min(equip.AT - 255, 255), 0);
                Emulator.MemoryController.EquipmentTable[i].MAT = equip.MAT;
                Emulator.MemoryController.EquipmentTable[i].DF = equip.DF;
                Emulator.MemoryController.EquipmentTable[i].MDF = equip.MDF;
                Emulator.MemoryController.EquipmentTable[i].SPD = equip.SPD;
                Emulator.MemoryController.EquipmentTable[i].A_HIT = equip.A_HIT;
                Emulator.MemoryController.EquipmentTable[i].M_HIT = equip.M_HIT;
                Emulator.MemoryController.EquipmentTable[i].A_AV = equip.A_AV;
                Emulator.MemoryController.EquipmentTable[i].M_AV = equip.M_AV;
                Emulator.MemoryController.EquipmentTable[i].E_Half = equip.ElementalResistance;
                Emulator.MemoryController.EquipmentTable[i].E_Immune = equip.ElementalImmunity;
                Emulator.MemoryController.EquipmentTable[i].StatusResist = equip.StatusResistance;
                Emulator.MemoryController.EquipmentTable[i].Special1 = equip.SpecialBonus1;
                Emulator.MemoryController.EquipmentTable[i].Special2 = equip.SpecialBonus2;
                Emulator.MemoryController.EquipmentTable[i].SpecialAmmount = equip.SpecialBonusAmmount;
                Emulator.MemoryController.EquipmentTable[i].SpecialEffect = equip.SpecialEffect;
            }
        }

        static void ThrownItemChange() {
            for (int i = 0; i < 64; i++) {
                var item = (LoDDictionary.UsableItem) LoDDictionary.Dictionary.Items[i + 192];
                Emulator.MemoryController.UsableItemTable[i].Target = item.Target;
                Emulator.MemoryController.UsableItemTable[i].Element = item.Element;
                Emulator.MemoryController.UsableItemTable[i].Damage = item.Damage;
                Emulator.MemoryController.UsableItemTable[i].Special1 = item.Special1;
                Emulator.MemoryController.UsableItemTable[i].Special2 = item.Special2;
                Emulator.MemoryController.UsableItemTable[i].Unknown1 = item.Unknown1;
                Emulator.MemoryController.UsableItemTable[i].SpecialAmmount = item.SpecialAmmount;
                Emulator.MemoryController.UsableItemTable[i].Status = item.Status;
                Emulator.MemoryController.UsableItemTable[i].Percentage = item.Percentage;
                Emulator.MemoryController.UsableItemTable[i].Unknown2 = item.Unknown2;
                Emulator.MemoryController.UsableItemTable[i].BaseSwitch = item.BaseSwitch;
            }
        }

    }
}
