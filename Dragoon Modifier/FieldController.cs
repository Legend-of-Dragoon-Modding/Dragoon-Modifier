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


    }
}
