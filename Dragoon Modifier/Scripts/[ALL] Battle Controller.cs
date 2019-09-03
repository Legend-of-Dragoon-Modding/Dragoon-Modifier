using Dragoon_Modifier;
using System;
using System.Threading;

public class BattleController {
    public static void Run(Emulator emulator) {
		int encounterValue = emulator.ReadShort(Constants.GetAddress("BATTLE_VALUE"));
		if (Globals.IN_BATTLE && !Globals.STATS_CHANGED && encounterValue == 41215) {
		    Constants.WriteOutput("Battle detected. Loading...");
			Globals.C_POINT = 0;
			for (int i = 0; i < 5; i++) {
				Globals.MONSTER_IDS[i] = 32767;
			}
			Thread.Sleep(3000);
			Globals.MONSTER_SIZE = emulator.ReadByte(Constants.GetAddress("MONSTER_SIZE"));
			Globals.UNIQUE_MONSTERS = emulator.ReadByte(Constants.GetAddress("UNIQUE_MONSTERS"));
			
			if (Constants.REGION == Region.USA) {
				Globals.M_POINT = 0x1A439C + emulator.ReadShort(Constants.GetAddress("M_POINT"));
			} else {
				Globals.M_POINT = 0x1A43B4 + emulator.ReadShort(Constants.GetAddress("M_POINT"));
			}
			
			Globals.C_POINT = (int) (emulator.ReadInteger(Constants.GetAddress("C_POINT")) - 0x7F5A8558 - (uint) Constants.OFFSET);
			for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
				Globals.MONSTER_IDS[i] = emulator.ReadShort(Constants.GetAddress("MONSTER_ID") + GetOffset() + (i * 0x8));
			}
			Globals.STATS_CHANGED = true;
			
			Constants.WriteDebug("Monster Size:      " + Globals.MONSTER_SIZE);
			Constants.WriteDebug("Unique Monsters:   " + Globals.UNIQUE_MONSTERS);
			Constants.WriteDebug("Monster Point:     " + Convert.ToString(Globals.M_POINT + Constants.OFFSET, 16).ToUpper());
			Constants.WriteDebug("Character Point:   " + Convert.ToString(Globals.C_POINT + Constants.OFFSET, 16).ToUpper());
			Constants.WriteDebug("Monster HP:        " + emulator.ReadShort(Globals.M_POINT));
			Constants.WriteDebug("Character HP:      " + emulator.ReadShort(Globals.C_POINT));
			for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
				Constants.WriteDebug("Monster ID Slot " + (i + 1) + ": " + Globals.MONSTER_IDS[i]);
			}
		    Constants.WriteOutput("Finished loading.");
		} else {
			if (Globals.STATS_CHANGED && encounterValue < 9999) {
				Globals.STATS_CHANGED = false;
				Constants.WriteOutput("Exiting out of battle.");
			}
		}
	}
	
	public static int GetOffset() {
		int[] discOffset = {0xDB0, 0x0, 0x1458, 0x1B0};
		int[] charOffset = {0x0, 0x180, -0x180, 0x420, 0x540, 0x180, 0x350, 0x2F0, -0x180};
		int partyOffset = 0;
		if (Globals.PARTY_SLOT[0] < 9 && Globals.PARTY_SLOT[1] < 9 && Globals.PARTY_SLOT[2] < 9) {
			partyOffset = charOffset[Globals.PARTY_SLOT[1]] + charOffset[Globals.PARTY_SLOT[2]];
		}
		return discOffset[Globals.DISC - 1] - partyOffset;
	}
	
	public static void Open(Emulator emulator) {}
	public static void Close(Emulator emulator) {}
	public static void Click(Emulator emulator) {}
}