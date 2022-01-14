using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Controller {
    internal static class BattleResult {
        public static void Setup(Emulator.IEmulator emulator, LoDDict.ILoDDictionary LoDDict, UI.IUIControl uiControl) {
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
                Item.FieldEquipmentChange(emulator, LoDDict);
            }


            if (Settings.SoloMode || Settings.DuoMode) {
                RemoveExtraPartyMembers(emulator);
            }
        }

        public static void RemoveExtraPartyMembers(Emulator.IEmulator emulator) {
            if (emulator.ReadByte("PARTY_SLOT", 0x4) != 255 && Settings.SoloMode) {
                for (int i = 0; i < 4; i++) {
                    emulator.WriteByte("PARTY_SLOT", 255, i + 0x4);
                }
            }

            if (emulator.ReadByte("PARTY_SLOT", 0x8) != 255) {
                for (int i = 0; i < 4; i++) {
                    emulator.WriteByte("PARTY_SLOT", 255, i + 0x8);
                }
            }
        }
    }
}
