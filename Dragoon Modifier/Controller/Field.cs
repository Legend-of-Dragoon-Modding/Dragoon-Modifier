using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dragoon_Modifier.Core;

namespace Dragoon_Modifier.Controller {
    public static class Field {
        static readonly ushort[] shopMaps = new ushort[] { 16, 23, 83, 84, 122, 145, 175, 180, 193, 204, 211, 214, 247,
        287, 309, 329, 332, 349, 357, 384, 435, 479, 515, 530, 564, 619, 624}; // Some maps missing??



        static bool shopDiscSwap = false;
        static bool shopListChanged = false;

        public static void Setup() {
            ItemChange();
        }

        public static void Run() {
            if (Main.InventorySize != 32) {
                ExtendInventory(Main.InventorySize);
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
        }

        private static void ItemChange() {
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

        private static void ItemIconChange() {
            Constants.WriteOutput("Changing Item Icons...");
            for (int i = 0; i < Core.Emulator.Memory.Item.Length; i++) {
                Core.Emulator.Memory.Item[i].Icon = LoDDictionary.Dictionary.Items[i].Icon;
            }
        }

        private static void ItemNameDescChange() {
            Constants.WriteOutput("Changing Item Names and Descriptions...");

            int address = Core.Emulator.GetAddress("ITEM_NAME");
            int address2 = Core.Emulator.GetAddress("ITEM_DESC");
            Core.Emulator.WriteAoB(address, LoDDictionary.Dictionary.EncodedNames);
            Core.Emulator.WriteAoB(address2, LoDDictionary.Dictionary.EncodedDescriptions);


            for (int i = 0; i < Core.Emulator.Memory.Item.Length; i++) {
                Core.Emulator.Memory.Item[i].NamePointer = (uint) LoDDictionary.Dictionary.Items[i].NamePointer;
                Core.Emulator.Memory.Item[i].DescriptionPointer = (uint) LoDDictionary.Dictionary.Items[i].DescriptionPointer;
            }
        }

        private static void ItemStatChange() {
            for (int i = 0; i < 192; i++) {
                var equip = (LoDDictionary.Equipment) LoDDictionary.Dictionary.Items[i];
                var mem = (Core.Memory.Equipment) Core.Emulator.Memory.Item[i];
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

        private static void ThrownItemChange() {
            for (int i = 192; i < 255; i++) {
                var item = (LoDDictionary.UsableItem) LoDDictionary.Dictionary.Items[i];
                var mem = (Core.Memory.UsableItem) Core.Emulator.Memory.Item[i];
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

        private static void ExtendInventory(byte inventorySize) { // TODO account for UltimateBossDefeatCheck

        }

        private static void ShopTableChange() {
            if (!shopListChanged && shopMaps.Contains(Core.Emulator.Memory.MapID)) {
                if (Core.Emulator.Memory.Transition != 12) { // Map transition in progress
                    return;
                }
                // TODO run
                return;
            }
            shopListChanged = false;
        }
    }
}
