
using Dragoon_Modifier.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Controller {
    internal static class Shop {

        internal static void TableChange(LoDDict.ILoDDictionary LoDDictionary) {
            for (int shop = 0; shop < LoDDictionary.Shop.Length; shop++) {
                if (LoDDictionary.Shop[shop].Count != 0) {
                    Emulator.Memory.Shop[shop].WeaponItemFlag = LoDDictionary.Shop[shop][0] >= 192 ? (byte) 1 : (byte) 0;
                }
                for (int item = 0; item < LoDDictionary.Shop[shop].Count; item++) {
                    Emulator.Memory.Shop[shop].Item[item] = LoDDictionary.Shop[shop][item];
                }
            }

            for (int item = 0; item < 255; item++) {
                Emulator.Memory.Item[item].SellPrice = (ushort) LoDDictionary.Item[item].SellPrice;
            }

            Console.WriteLine("Changing Shop Table and Prices...");
        }

        internal static void ContentChange(LoDDict.ILoDDictionary LoDDictionary) {
            var shopID = Emulator.Memory.ShopID;

            if (LoDDictionary.Shop[shopID].Count != 0) {
                Emulator.Memory.CurrentShop.WeaponItemFlag = LoDDictionary.Shop[shopID][0] >= 192 ? (byte) 1 : (byte) 0;
            }

            Emulator.Memory.CurrentShop.ItemCount = (byte) LoDDictionary.Shop[shopID].Count;

            int item = 0;
            foreach (var itemID in LoDDictionary.Shop[shopID]) {
                Emulator.Memory.CurrentShop.ItemID[item] = itemID;
                Emulator.Memory.CurrentShop.ItemPrice[item] = (ushort) (LoDDictionary.Item[itemID].SellPrice * 2);
                item++;
            }

            for (int rest = item; rest < 16; rest++) {
                Emulator.Memory.CurrentShop.ItemID[rest] = 255;
            }
        }
    }
}
