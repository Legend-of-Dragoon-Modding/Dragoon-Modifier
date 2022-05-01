using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Controller {
    internal static class BattleResult {
        public static void Setup(LoDDict.ILoDDictionary loDDictionary, UI.IUIControl uiControl) {
            uiControl.ResetBattle();

            if (Settings.ItemIconChange) {
                Console.WriteLine("Changing Item icons...");
                Item.IconChange(loDDictionary);
            }

            if (Settings.ItemNameDescChange) {
                Console.WriteLine("Changing Item names and descriptions...");
                Item.FieldItemNameDescChange(loDDictionary);
            }

            if (Settings.ItemStatChange) {
                Console.WriteLine("Changing Items stats...");
                Item.FieldEquipmentChange(loDDictionary);
            }

            if (Settings.AdditionChange) {
                Console.WriteLine("Changing Additions...");
                Addition.MenuTableChange(loDDictionary);
            }


            if (Settings.SoloMode || Settings.DuoMode) {
                RemoveExtraPartyMembers();
            }

            loDDictionary.ItemScript.FieldSetup(loDDictionary, uiControl);
        }

        public static void RemoveExtraPartyMembers() {
            if (Core.Emulator.DirectAccess.ReadByte("PARTY_SLOT", 0x4) != 255 && Settings.SoloMode) {
                for (int i = 0; i < 4; i++) {
                    Core.Emulator.DirectAccess.WriteByte("PARTY_SLOT", 255, i + 0x4);
                }
            }

            if (Core.Emulator.DirectAccess.ReadByte("PARTY_SLOT", 0x8) != 255) {
                for (int i = 0; i < 4; i++) {
                    Core.Emulator.DirectAccess.WriteByte("PARTY_SLOT", 255, i + 0x8);
                }
            }
        }
    }
}
