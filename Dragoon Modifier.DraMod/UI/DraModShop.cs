using Dragoon_Modifier.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.UI {
    public sealed class DraModShop {

        //Ultimate Boss Shop
        private static int[] uShopPrices = {
            70000,   //Spirit Eater
            70000,   //Harpoon
            70000,   //Element Arrow
            70000,   //Dragon Beater
            70000,   //Battery Glove
            70000,   //Jeweled Hammer
            70000,   //Giant Axe
            280000,  //Soa's Light
            30000,   //Fake Legend Casque
            120000,  //Soa's Helm
            30000,   //Fake Legend Armor
            60000,   //Divine DG Armor
            120000,  //Soa's Armor
            40000,   //Lloyd's Boots
            40000,   //Winged Shoes
            120000,  //Soa's Greveas
            20000,   //Heal Ring
            40000,   //Soa's Sash
            50000,   //Soa's Ahnk
            50000,   //Soa's Health Ring
            50000,   //Soa's Mage Ring
            50000,   //Soa's Shield Ring
            50000,   //Soa's Siphon Ring
            100000,  //Power Up
            100000,  //Power Down
            100000,  //Speed Up
            100000,  //Speed Down
            100000,  //Magic Shield
            100000,  //Material Shield
            75000,   //Magic Stone of Signet
            25000,   //Pandemonium
            1000000, //Psychedelic Bomb X
            250000,  //Empty Dragoon Crystal
            500000,  //Soa's Wargod
            500000   //Soa's Dragoon Boost
        };

        private static int[] uLimited = {
            1,          //Spirit Eater
            2,          //Harpoon
            4,          //Element Arrow
            8,          //DB2
            16,         //Battery Glove
            32,         //Jeweled Hammer
            64,         //Giant Axe
            128,        //Soa's Light
            0,          //Fake Legend Casque
            256,        //Soa's Helm
            0,          //Fake Legend Armor
            0,          //Divine DG Armor
            512,        //Soa's Armor
            0,          //Lloyd's Boots
            0,          //Winged Shoes
            1024,       //Soa's Greveas
            0,          //Heal Ring
            2048,       //Soa's Sash
            4096,       //Soa's Ahnk
            8192,       //Soa's Health Ring
            16384,      //Soa's Mage Ring
            32768,      //Soa's Shield Ring
            65536,      //Soa's Siphon Ring
            131072,     //Power Up
            262144,     //Power Down
            524288,     //Speed Up
            1048576,    //Speed Down
            2097152,    //Magic Shield
            4194304,    //Material Shield
            8388608,    //Magic Stone of Signet
            16777216,   //Pandemonium
            33554432,   //Psychedelic Bomb X
            67108864,   //Empty Dragon Crystal
            134217728,  //Soa's Wargod
            268435456   //Soa's Dragoon Boost
        };

        private static int[] uItemId = {
            159, //Spirit Eater
            160, //Harpoon
            161, //Element Arrow
            162, //Dragon Beater
            163, //Battery Glove
            164, //Jeweled Hammer
            165, //Giant Axe
            166, //Soa's Light
            167, //Fake Legend Casque
            168, //Soa's Helm
            169, //Fake Legend Armor
            170, //Divine DG Armor
            171, //Soa's Armor
            172, //Lloyd's Boots
            173, //Winged Shoes
            174, //Soa's Greveas
            175, //Heal Ring
            176, //Soa's Sash
            177, //Soa's Ahnk
            178, //Soa's Health Ring
            179, //Soa's Mage Ring
            180, //Soa's Shield Ring
            181, //Soa's Siphon Ring
            0,   //Power Up
            0,   //Power Down
            0,   //Speed Up
            0,   //Speed Down
            0,   //Magic Shield
            0,   //Material Shield
            0,   //Magic Stone of Signet
            0,   //Pandemonium
            250, //Psychedelic Bomb X
            0,   //Empty Dragon Crystal
            0,   //Soa's Wargod
            0    //Soa's Dragoon Boost
        };

        public static void BuyTicket(uint cost, int tickets) {
            if (Emulator.Memory.Gold >= cost) {
                Emulator.Memory.Gold -= cost;
                Emulator.Memory.HeroTickets += tickets;
            }
            Constants.UIControl.WriteGLog("You have " + Emulator.Memory.HeroTickets + " tickets. Gold: " + Emulator.Memory.Gold);
        }

        public static void UltimateItemShop(int selectedItem) {
            if (Emulator.Memory.Chapter < 4) {
                Constants.UIControl.WriteGLog("You have must have completed Chapter 3 to purchase from this shop.");
                return;
            }

            uint gold = Emulator.Memory.Gold;
            int price = uShopPrices[selectedItem];
            int item = uItemId[selectedItem];
            int oneLimited = uLimited[selectedItem];

            if (gold < price) {
                Constants.UIControl.WriteGLog("Not enough gold.");
                return;
            }

            if (oneLimited > 0) {
                if ((Constants.UltimateShopLimited & oneLimited) == oneLimited) {
                    Constants.UIControl.WriteGLog("This item can only be bought once.");
                    return;
                }
            }

            if (item > 0) {
                bool shop = BuyShopItem((byte) item, (uint) price, 0, true);
                if (shop)
                    Constants.UltimateShopLimited += oneLimited;
            } else {
                if (oneLimited > 0)
                    Constants.UltimateShopLimited += oneLimited;
                Emulator.Memory.Gold -= (uint) price;
                Constants.UIControl.WriteGLog("Bought item.");
            }
        }

        public static bool BuyShopItem(byte item, uint goldCost, int ticketCost, bool armor = false) {
            uint gold = Emulator.Memory.Gold;
            int tickets = Emulator.Memory.HeroTickets;

            if (gold < goldCost) {
                Constants.UIControl.WriteGLog("Not enough gold.");
                return false;
            }

            if (tickets < ticketCost) {
                Constants.UIControl.WriteGLog("Not enough tickets.");
                return false;
            }

            bool slots = false;
            int i = 1;
            foreach (var slot in armor ? Emulator.Memory.EquipmentInventory : Emulator.Memory.ItemInventory) {
                if (slot == 255) {
                    if (!armor && i > Constants.InventorySize) {
                        Constants.UIControl.WriteGLog("Inventory cap is currently set to " + Constants.InventorySize + ", you can unlock more slots via Ultimate Boss.");
                        return false;
                    }

                    slots = true;

                    if (!armor)
                        Emulator.Memory.ItemInventory[i - 1] = item;
                    else
                        Emulator.Memory.EquipmentInventory[i - 1] = item ;

                    if (goldCost > 0)
                        Emulator.Memory.Gold -= goldCost;
                    if (ticketCost > 0)
                        Emulator.Memory.HeroTickets -= ticketCost;

                    break;
                }
                i++;
            }

            if (slots) {
                Constants.SaveRegistry();
                Constants.UIControl.WriteGLog("Item bought. Gold: " + (gold - goldCost) + " | Tickets: " + (tickets - ticketCost));
            } else {
                Constants.UIControl.WriteGLog("Inventory full.");
            }

            return slots;
        } 
    }
}
