using Dragoon_Modifier;
using System.Windows.Controls;

public class PartyChanger {
	static int R = 255;
	static int G = 0;
	static int B = 0;
	static int CYCLE = 0;
	static int SPEED = 5;
	
    public static void Run() {
		if (Globals.GAME_STATE == 1 && Globals.STATS_CHANGED) {
			if (CYCLE == 0) {
				G += SPEED;
				if (G >= 255) {
					G = 255;
					CYCLE = 1;
				}
			} else if (CYCLE == 1) {
				R -= SPEED;
				if (R <= 0) {
					R = 0;
					CYCLE = 2;
				}
			} else if (CYCLE == 2) {
				B += SPEED;
				if (B >= 255) {
					B = 255;
					CYCLE = 3;
				}
			} else if (CYCLE == 3) {
				G -= SPEED;
				if (G <= 0) {
					G = 0;
					CYCLE = 4;
				}
			} else if (CYCLE == 4) {
				R += SPEED;
				if (R >= 255) {
					R = 255;
					CYCLE = 5;
				}
			} else if (CYCLE == 5) {
				B -= SPEED;
				if (B <= 0) {
					B = 0;
					CYCLE = 0;
				}
			}
			
		
			Emulator.WriteByte(0xC7004, R);
			Emulator.WriteByte(0xC7005, G);
			Emulator.WriteByte(0xC7006, B);
		}
	}
	
	public static void Open() {}
	
	public static void Close() {}
	
	public static void Click() {}
}