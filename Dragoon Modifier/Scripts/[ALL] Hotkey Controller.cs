using Dragoon_Modifier;

public class HotkeyController {
    public static void Run(Emulator emulator) {
		Globals.HOTKEY = emulator.ReadShort("HOTKEY");
	}
	
	public static void Open(Emulator emulator) {}
	public static void Close(Emulator emulator) {}
	public static void Click(Emulator emulator) {}
}