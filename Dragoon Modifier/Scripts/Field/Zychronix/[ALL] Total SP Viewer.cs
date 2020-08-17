using Dragoon_Modifier;

public class Base {
    public static void Run(Emulator emulator) {}
	public static void Open(Emulator emulator) {}
	public static void Close(Emulator emulator) {}
	
	public static void Click(Emulator emulator) {
		string[] characters = {"Dart", "Lavitz", "Shana", "Rose", "Haschel", "Albert", "Meru", "Kongol", "Miranda"};
		for (int i = 0; i < 9; i++) {
			Constants.WriteOutput("Character: " + characters[i] + " - Total SP: "  + emulator.ReadShort("CHAR_TABLE", (i * 0x2C) + 0xE));
		}
	}
}