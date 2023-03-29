using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Controller {
    internal static class BattleResult {
        public static void Setup() {
            Constants.UIControl.ResetBattle();


            if (Settings.Instance.ItemIconChange) {
                Console.WriteLine("Changing Item icons...");
                Item.IconChange();
            }

            if (Settings.Instance.ItemNameDescChange) {
                Console.WriteLine("Changing Item names and descriptions...");
                Item.FieldItemNameDescChange();
            }

            if (Settings.Instance.ItemStatChange) {
                Console.WriteLine("Changing Items stats...");
                Item.FieldEquipmentChange();
            }

            if (Settings.Instance.AdditionChange) {
                Console.WriteLine("Changing Additions...");
                Addition.MenuTableChange();
            }


            if (Settings.Instance.SoloMode || Settings.Instance.DuoMode) {
                RemoveExtraPartyMembers();
            }

            Settings.Instance.Dataset.Script.FieldSetup();
        }

        public static void RemoveExtraPartyMembers() {
            if (Core.Emulator.DirectAccess.ReadByte("PARTY_SLOT", 0x4) != 255 && Settings.Instance.SoloMode) {
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
