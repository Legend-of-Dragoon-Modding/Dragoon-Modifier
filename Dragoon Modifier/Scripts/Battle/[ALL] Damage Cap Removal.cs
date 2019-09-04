//Author: Zychronix
using Dragoon_Modifier;
using System;
using System.Collections;

public class DamageCapRemoval {
	static bool FIRST_CAP_REMOVAL = false;
	static int lastItemUsed = 0;
	
    public static void Run(Emulator emulator) {
		if (Globals.IN_BATTLE && Globals.STATS_CHANGED) {
			if (!FIRST_CAP_REMOVAL) {
				emulator.WriteInteger(0xF2A5C, 50000);
				emulator.WriteInteger(0xF2A64, 50000);
				emulator.WriteInteger(0xF2A70, 50000);
				DamageCapScan(emulator);
				FIRST_CAP_REMOVAL = true;
			} else {
				ushort currentItem = emulator.ReadShort(Globals.M_POINT + 0xABC);
				if (lastItemUsed != currentItem) {
					lastItemUsed = currentItem;
					if ((lastItemUsed >= 0xC1 && lastItemUsed <= 0xCA) || (lastItemUsed >= 0xCF && lastItemUsed <= 0xD2) || lastItemUsed == 0xD6 || lastItemUsed == 0xD8 || lastItemUsed == 0xDC || (lastItemUsed >= 0xF1 && lastItemUsed <= 0xF8) || lastItemUsed == 0xFA) {
						DamageCapScan(emulator);
					}
				}
				for (int i = 0; i < 3; i++) {
					if (Globals.PARTY_SLOT[i] < 9) {
						if (emulator.ReadByte(Globals.C_POINT - 0xA8 - (0x388 * i)) == 24) {
							DamageCapScan(emulator);
						}
					}
				}
			}
		} else {
			FIRST_CAP_REMOVAL = false;
			lastItemUsed = 0;
		}
	}
	
	public static void DamageCapScan(Emulator emulator) {
		ArrayList damageCapScan = emulator.ScanAllAOB("0F 27", 0xA8660, 0x2A865F);
		long lastAddress = 0;
		foreach (var address in damageCapScan) {
			long capAddress = (long) address;
			if (emulator.ReadShortU(capAddress) == 9999 && (lastAddress + 0x10) == capAddress) {
				emulator.WriteIntegerU(capAddress, 50000);
				emulator.WriteIntegerU(lastAddress, 50000);
			}
			lastAddress = capAddress;
		}
	}
	
	public static void Open(Emulator emulator) {}
	public static void Close(Emulator emulator) {}
	public static void Click(Emulator emulator) {}
}