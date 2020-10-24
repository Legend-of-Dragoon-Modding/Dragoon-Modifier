using Dragoon_Modifier;

public class Base {
    public static void Run() {}
	public static void Open() {}
	public static void Close() {}
	
	public static void Click() {
		string[] characters = {"Dart", "Lavitz", "Shana", "Rose", "Haschel", "Albert", "Meru", "Kongol", "Miranda"};
		for (int i = 0; i < 9; i++) {
			Constants.WriteOutput("Character: " + characters[i] + " - Total SP: "  + Emulator.ReadShort("CHAR_TABLE", (i * 0x2C) + 0xE));
		}
	}
}