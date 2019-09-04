using Dragoon_Modifier;

public class Base {
	
	static bool wrote = false;
	
    public static void Run(Emulator emulator) {
		byte menu = emulator.ReadByte(Constants.GetAddress("MENU"));
		if (menu == 12 || menu == 16) {
			int offset = 0x0;
			if (Constants.REGION == Region.JPN) {
				offset = -0x186C;
			} else if (Constants.REGION == Region.EUR) {
				offset = 0x23C;
			}
            emulator.WriteByte(0x1120DE + offset, 1);
            emulator.WriteByte(0x1120FA + offset, 1);
            emulator.WriteByte(0x112116 + offset, 1);
            emulator.WriteByte(0x112132 + offset, 1);
            emulator.WriteByte(0x11214E + offset, 1);
            emulator.WriteByte(0x1121F6 + offset, 56);
            emulator.WriteByte(0x11222E + offset, 3);
            emulator.WriteByte(0x11224A + offset, 3);
            emulator.WriteByte(0x112266 + offset, 3);
            emulator.WriteByte(0x112282 + offset, 3);
            emulator.WriteByte(0x11229E + offset, 3);
            emulator.WriteByte(0x1122BA + offset, 3);
            emulator.WriteByte(0x1122D6 + offset, 3);
            emulator.WriteByte(0x1122F2 + offset, 4);
            emulator.WriteByte(0x11230E + offset, 4);
            emulator.WriteByte(0x11232A + offset, 4);
            emulator.WriteByte(0x112346 + offset, 4);
            emulator.WriteByte(0x112362 + offset, 4);
            emulator.WriteByte(0x11237E + offset, 4);
            emulator.WriteByte(0x11239A + offset, 4);
            emulator.WriteByte(0x1123B6 + offset, 2);
            emulator.WriteByte(0x1123D2 + offset, 2);
            emulator.WriteByte(0x1123EE + offset, 2);
            emulator.WriteByte(0x11240A + offset, 2);
            emulator.WriteByte(0x112426 + offset, 2);
            emulator.WriteByte(0x112442 + offset, 2);
            emulator.WriteByte(0x11245E + offset, 7);
            emulator.WriteByte(0x11247A + offset, 7);
            emulator.WriteByte(0x112496 + offset, 7);
            emulator.WriteByte(0x1124B2 + offset, 7);
            emulator.WriteByte(0x1124CE + offset, 7);
            emulator.WriteByte(0x1124EA + offset, 7);
            emulator.WriteByte(0x112506 + offset, 8);
            emulator.WriteByte(0x1125E6 + offset, 11);
            emulator.WriteByte(0x11263A + offset, 14);
            emulator.WriteByte(0x112656 + offset, 14);
            emulator.WriteByte(0x112672 + offset, 14);
            emulator.WriteByte(0x11268E + offset, 14);
            emulator.WriteByte(0x1126C6 + offset, 9);
            emulator.WriteByte(0x1126E2 + offset, 9);
            emulator.WriteByte(0x11271A + offset, 12);
            emulator.WriteByte(0x1127C2 + offset, 14);
            emulator.WriteByte(0x1127DE + offset, 12);
            emulator.WriteByte(0x11292E + offset, 15);
            emulator.WriteByte(0x11294A + offset, 15);
            emulator.WriteByte(0x11299E + offset, 17);
            emulator.WriteByte(0x112A46 + offset, 20);
            emulator.WriteByte(0x112A62 + offset, 19);
            emulator.WriteByte(0x112A7E + offset, 19);
            emulator.WriteByte(0x112AEE + offset, 19);
            emulator.WriteByte(0x112B0A + offset, 19);
            emulator.WriteByte(0x112B42 + offset, 26);
            emulator.WriteByte(0x112B5E + offset, 26);
            emulator.WriteByte(0x112B7A + offset, 26);
            emulator.WriteByte(0x112B96 + offset, 26);
            emulator.WriteByte(0x112BB2 + offset, 26);
            emulator.WriteByte(0x112BCE + offset, 26);
            emulator.WriteByte(0x112BEA + offset, 28);
            emulator.WriteByte(0x112C06 + offset, 26);
            emulator.WriteByte(0x112C22 + offset, 22);
            emulator.WriteByte(0x112C3E + offset, 22);
            emulator.WriteByte(0x112C5A + offset, 22);
            emulator.WriteByte(0x112C76 + offset, 22);
            emulator.WriteByte(0x112C92 + offset, 22);
            emulator.WriteByte(0x112CAE + offset, 22);
            emulator.WriteByte(0x112CCA + offset, 22);
            emulator.WriteByte(0x112CE6 + offset, 29);
            emulator.WriteByte(0x112D02 + offset, 29);
            emulator.WriteByte(0x112D1E + offset, 29);
            emulator.WriteByte(0x112D56 + offset, 24);
            emulator.WriteByte(0x112D72 + offset, 30);
            emulator.WriteByte(0x112DE2 + offset, 24);
            emulator.WriteByte(0x112DFE + offset, 24);
            emulator.WriteByte(0x112E36 + offset, 27);
            emulator.WriteByte(0x112EA6 + offset, 25);
            emulator.WriteByte(0x112EC2 + offset, 25);
            emulator.WriteByte(0x112EDE + offset, 25);
            emulator.WriteByte(0x112EFA + offset, 25);
            emulator.WriteByte(0x112F16 + offset, 25);
            emulator.WriteByte(0x112F32 + offset, 25);
            emulator.WriteByte(0x112F4E + offset, 25);
            emulator.WriteByte(0x11304A + offset, 32);
            emulator.WriteByte(0x113066 + offset, 32);
            emulator.WriteByte(0x113082 + offset, 32);
            emulator.WriteByte(0x1130BA + offset, 22);
            emulator.WriteByte(0x1130D6 + offset, 22);
			wrote = true;
		} else {
			if (menu == 125) {
				wrote = false;
			}
		}
	}
	public static void Open(Emulator emulator) {}
	public static void Close(Emulator emulator) {}
	public static void Click(Emulator emulator) {}
}