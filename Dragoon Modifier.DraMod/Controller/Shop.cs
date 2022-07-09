
using Dragoon_Modifier.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Controller {
    internal static class Shop {

        internal static void TableChange() {
            for (int shop = 0; shop < Settings.Instance.Dataset.Shop.Length; shop++) {
                if (Settings.Instance.Dataset.Shop[shop].Count != 0) {
                    Emulator.Memory.Shop[shop].WeaponItemFlag = Settings.Instance.Dataset.Shop[shop][0] >= 192 ? (byte) 1 : (byte) 0;
                }
                for (int item = 0; item < Settings.Instance.Dataset.Shop[shop].Count; item++) {
                    Emulator.Memory.Shop[shop].Item[item] = Settings.Instance.Dataset.Shop[shop][item];
                }
            }

            for (int item = 0; item < 255; item++) {
                Emulator.Memory.Item[item].SellPrice = (ushort) Settings.Instance.Dataset.Item[item].SellPrice;
            }

            Console.WriteLine("Changing Shop Table and Prices...");
        }

        internal static void ContentChange() {
            var shopID = Emulator.Memory.ShopID;

            if (Settings.Instance.Dataset.Shop[shopID].Count != 0) {
                Emulator.Memory.CurrentShop.WeaponItemFlag = Settings.Instance.Dataset.Shop[shopID][0] >= 192 ? (byte) 1 : (byte) 0;
            }

            Emulator.Memory.CurrentShop.ItemCount = (byte) Settings.Instance.Dataset.Shop[shopID].Count;

            int item = 0;
            foreach (var itemID in Settings.Instance.Dataset.Shop[shopID]) {
                Emulator.Memory.CurrentShop.ItemID[item] = itemID;
                Emulator.Memory.CurrentShop.ItemPrice[item] = (ushort) (Settings.Instance.Dataset.Item[itemID].SellPrice * 2);
                item++;
            }

            for (int rest = item; rest < 16; rest++) {
                Emulator.Memory.CurrentShop.ItemID[rest] = 255;
            }
        }
    }
}
