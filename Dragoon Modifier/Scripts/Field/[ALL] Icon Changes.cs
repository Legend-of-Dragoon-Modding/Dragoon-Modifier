/*
 Author: Zychronix
 Description: Changes icons used in menus. 
*/
using Dragoon_Modifier;

public class IconChanges {
	
	static bool wrote = false;
	static bool firstWrite = false;
	
    public static void Run(Emulator emulator) {
		byte menu = emulator.ReadByte(Constants.GetAddress("MENU"));
		if (menu == 12 || menu == 16 || menu == 19) {
			WriteIcons(emulator);
			wrote = true;
		} else {
			if (menu == 125) {
				wrote = false;
			}
		}
		
		if (!firstWrite && !Globals.IN_BATTLE && Globals.BATTLE_VALUE > 0) {
			WriteIcons(emulator);
			firstWrite = true;
		} else {
			if (Globals.IN_BATTLE) {
				firstWrite = false;
			}
		}
	}
	
	public static void WriteIcons(Emulator emulator) {
		emulator.WriteByte(0x1120DE + GetOffset(), 1);
		emulator.WriteByte(0x1120FA + GetOffset(), 1);
		emulator.WriteByte(0x112116 + GetOffset(), 1);
		emulator.WriteByte(0x112132 + GetOffset(), 1);
		emulator.WriteByte(0x11214E + GetOffset(), 1);
		emulator.WriteByte(0x1121F6 + GetOffset(), 56);
		emulator.WriteByte(0x11222E + GetOffset(), 3);
		emulator.WriteByte(0x11224A + GetOffset(), 3);
		emulator.WriteByte(0x112266 + GetOffset(), 3);
		emulator.WriteByte(0x112282 + GetOffset(), 3);
		emulator.WriteByte(0x11229E + GetOffset(), 3);
		emulator.WriteByte(0x1122BA + GetOffset(), 3);
		emulator.WriteByte(0x1122D6 + GetOffset(), 3);
		emulator.WriteByte(0x1122F2 + GetOffset(), 4);
		emulator.WriteByte(0x11230E + GetOffset(), 4);
		emulator.WriteByte(0x11232A + GetOffset(), 4);
		emulator.WriteByte(0x112346 + GetOffset(), 4);
		emulator.WriteByte(0x112362 + GetOffset(), 4);
		emulator.WriteByte(0x11237E + GetOffset(), 4);
		emulator.WriteByte(0x11239A + GetOffset(), 4);
		emulator.WriteByte(0x1123B6 + GetOffset(), 2);
		emulator.WriteByte(0x1123D2 + GetOffset(), 2);
		emulator.WriteByte(0x1123EE + GetOffset(), 2);
		emulator.WriteByte(0x11240A + GetOffset(), 2);
		emulator.WriteByte(0x112426 + GetOffset(), 2);
		emulator.WriteByte(0x112442 + GetOffset(), 2);
		emulator.WriteByte(0x11245E + GetOffset(), 7);
		emulator.WriteByte(0x11247A + GetOffset(), 7);
		emulator.WriteByte(0x112496 + GetOffset(), 7);
		emulator.WriteByte(0x1124B2 + GetOffset(), 7);
		emulator.WriteByte(0x1124CE + GetOffset(), 7);
		emulator.WriteByte(0x1124EA + GetOffset(), 7);
		emulator.WriteByte(0x112506 + GetOffset(), 8);
		emulator.WriteByte(0x1125E6 + GetOffset(), 11);
		emulator.WriteByte(0x11263A + GetOffset(), 14);
		emulator.WriteByte(0x112656 + GetOffset(), 14);
		emulator.WriteByte(0x112672 + GetOffset(), 14);
		emulator.WriteByte(0x11268E + GetOffset(), 14);
		emulator.WriteByte(0x1126C6 + GetOffset(), 9);
		emulator.WriteByte(0x1126E2 + GetOffset(), 9);
		emulator.WriteByte(0x11271A + GetOffset(), 12);
		emulator.WriteByte(0x1127C2 + GetOffset(), 14);
		emulator.WriteByte(0x1127DE + GetOffset(), 12);
		emulator.WriteByte(0x11292E + GetOffset(), 15);
		emulator.WriteByte(0x11294A + GetOffset(), 15);
		emulator.WriteByte(0x11299E + GetOffset(), 17);
		emulator.WriteByte(0x112A46 + GetOffset(), 20);
		emulator.WriteByte(0x112A62 + GetOffset(), 19);
		emulator.WriteByte(0x112A7E + GetOffset(), 19);
		emulator.WriteByte(0x112AEE + GetOffset(), 19);
		emulator.WriteByte(0x112B0A + GetOffset(), 19);
		emulator.WriteByte(0x112B42 + GetOffset(), 26);
		emulator.WriteByte(0x112B5E + GetOffset(), 26);
		emulator.WriteByte(0x112B7A + GetOffset(), 26);
		emulator.WriteByte(0x112B96 + GetOffset(), 26);
		emulator.WriteByte(0x112BB2 + GetOffset(), 26);
		emulator.WriteByte(0x112BCE + GetOffset(), 26);
		emulator.WriteByte(0x112BEA + GetOffset(), 28);
		emulator.WriteByte(0x112C06 + GetOffset(), 26);
		emulator.WriteByte(0x112C22 + GetOffset(), 22);
		emulator.WriteByte(0x112C3E + GetOffset(), 22);
		emulator.WriteByte(0x112C5A + GetOffset(), 22);
		emulator.WriteByte(0x112C76 + GetOffset(), 22);
		emulator.WriteByte(0x112C92 + GetOffset(), 22);
		emulator.WriteByte(0x112CAE + GetOffset(), 22);
		emulator.WriteByte(0x112CCA + GetOffset(), 22);
		emulator.WriteByte(0x112CE6 + GetOffset(), 29);
		emulator.WriteByte(0x112D02 + GetOffset(), 29);
		emulator.WriteByte(0x112D1E + GetOffset(), 29);
		emulator.WriteByte(0x112D56 + GetOffset(), 24);
		emulator.WriteByte(0x112D72 + GetOffset(), 30);
		emulator.WriteByte(0x112DE2 + GetOffset(), 24);
		emulator.WriteByte(0x112DFE + GetOffset(), 24);
		emulator.WriteByte(0x112E36 + GetOffset(), 27);
		emulator.WriteByte(0x112EA6 + GetOffset(), 25);
		emulator.WriteByte(0x112EC2 + GetOffset(), 25);
		emulator.WriteByte(0x112EDE + GetOffset(), 25);
		emulator.WriteByte(0x112EFA + GetOffset(), 25);
		emulator.WriteByte(0x112F16 + GetOffset(), 25);
		emulator.WriteByte(0x112F32 + GetOffset(), 25);
		emulator.WriteByte(0x112F4E + GetOffset(), 25);
		emulator.WriteByte(0x11304A + GetOffset(), 32);
		emulator.WriteByte(0x113066 + GetOffset(), 32);
		emulator.WriteByte(0x113082 + GetOffset(), 32);
		emulator.WriteByte(0x1130BA + GetOffset(), 22);
		emulator.WriteByte(0x1130D6 + GetOffset(), 22);	
	}
	
	public static int GetOffset() {		
		int offset = 0x0;
		if (Constants.REGION == Region.JPN) {
			offset = -0x186C;
		} else if (Constants.REGION == Region.EUR) {
			offset = 0x23C;
		}
		return offset;
	}
	
	public static void Click(Emulator emulator) {
		if (!Globals.IN_BATTLE) {
			WriteIcons(emulator);
		}
	}
	
	public static void Open(Emulator emulator) {}
	public static void Close(Emulator emulator) {}
}