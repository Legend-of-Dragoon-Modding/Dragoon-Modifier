/*
 Author: Zychronix
 Description: Scans for string "SSsq" and mutes music.
*/
using Dragoon_Modifier;
using System.Collections;
using System.Windows.Controls;

public class KillBGM {
    static int killMode = 0;
	static bool muteField = false;
	static bool muteBattle = false;
	
	public static void Run(Emulator emulator) {
		if (killMode > 0) {
			if (!muteField && (killMode == 1 || killMode == 3) && !Globals.IN_BATTLE && Globals.BATTLE_VALUE > 0) {
				KillMusic(emulator);
				muteField = true;
			} else if (!muteBattle && (killMode == 2 || killMode == 3) && Globals.IN_BATTLE) {
				KillMusic(emulator);
				emulator.WriteShort(0xC4CF4, 0);
				muteBattle = true;
			}
			
			if (Globals.IN_BATTLE) {
				muteField = false;
			} else {
				muteBattle = false;
			}
		}
	}
	
	public static void KillMusic(Emulator emulator) {
		ArrayList killBGMScan = emulator.ScanAllAOB("53 53 73 71", 0xA8660, 0x2A865F);
		foreach (var address in killBGMScan) {
			long bgm = (long) address;
			for (int i = 0; i < 255; i++) {
				emulator.WriteByteU(bgm + i, 0);
			}
		}
	}
	
	public static void Click(Emulator emulator) {
		InputWindow openBGMWindow = new InputWindow("Kill BGM");
		ComboBox cboMode = new ComboBox();
		cboMode.Items.Add("Field");
		cboMode.Items.Add("Battle");
		cboMode.Items.Add("Both");
		cboMode.SelectedIndex = 0;
		openBGMWindow.AddObject(cboMode);
		openBGMWindow.AddTextBlock("Select a Kill BGM Mode.");
		openBGMWindow.ShowDialog();
		killMode = cboMode.SelectedIndex + 1;
	}
	
	public static void Open(Emulator emulator) {}
	public static void Close(Emulator emulator) {}
}