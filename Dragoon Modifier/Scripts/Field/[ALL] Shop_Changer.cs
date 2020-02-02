using Dragoon_Modifier;
using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

public class Shop_Changer {
    static bool SHOP_CHANGED = true;
    static int[] SHOP_MAPS = new int[] { 16, 23, 83, 84, 122, 145, 175, 180, 193, 204, 211, 214, 247,
        287, 309, 329, 332, 349, 357, 384, 435, 479, 515, 530, 564, 619, 624}; // Some maps missing??
    public static void Run(Emulator emulator) {
        if (SHOP_MAPS.Contains((int)Globals.MAP)) {
            if (Globals.SHOP_CHANGE == true) {
                if (SHOP_CHANGED == false && emulator.ReadByteU(0xBDC28 + Constants.OFFSET) == 3) {
                    int shop_og_size = ReadShop(0x11E13C, emulator);
                    int shop = emulator.ReadByteU(0x7A3B4 + Constants.OFFSET); // Shop ID
                    Constants.WriteDebug("Shop " + shop + " Changed");
                    SHOP_CHANGED = true;
                    int i = 0;
                    foreach (int[] item in Globals.DICTIONARY.ShopList[shop]) {
                        WriteShop(0x11E0F8 + i * 4, (ushort)item[0], emulator);
                        WriteShop(0x11E0F8 + 0x2 + i * 4, (ushort)item[1], emulator);
                        i += 1;
                    }
                    int shop_size = Globals.DICTIONARY.ShopList[shop].Count;
                    WriteShop(0x11E13C, (ushort)shop_size, emulator);
                    if (shop_og_size > shop_size) {
                        for (int z = shop_size; z < 17; z++) {
                            WriteShop(0x11E0F8 + z * 4, (ushort)255, emulator);
                            WriteShop(0x11E0F8 + 2 + z * 4, (ushort)0, emulator);
                        }
                    }
                } else if (emulator.ReadByteU(0xBDC28 + Constants.OFFSET) != 3) // Shop Buy/Sell screen
                  {
                    SHOP_CHANGED = false;
                }
            }
        }
    }

    public static void Open(Emulator emulator) { }
    public static void Close(Emulator emulator) { }
    public static void Click(Emulator emulator) { }

    public static int ReadShop(int address, Emulator emulator) {
        return emulator.ReadShort(address + ShopOffset());
    }

    public static void WriteShop(int address, ushort value, Emulator emulator) {
        emulator.WriteShort(address + ShopOffset(), value);
    }

    public static int ShopOffset() {
        int offset = 0x0;
        if (Constants.REGION == Region.JPN) {
            offset -= 0x4D90;
        } else if (Constants.REGION == Region.EUR_GER) {
            offset += 0x120;
        }
        return offset;
    }
}