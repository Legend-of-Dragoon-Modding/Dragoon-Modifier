using Dragoon_Modifier;
using System;

public class FieldController {
    public static void Run(Emulator emulator) {
		Globals.BATTLE_VALUE = emulator.ReadShort("BATTLE_VALUE");
		Globals.DISC = emulator.ReadByte("DISC");
		Globals.CHAPTER = (byte) (emulator.ReadByte("CHAPTER") + 1);
		Globals.ENCOUNTER_ID = emulator.ReadShort("ENCOUNTER_ID");
		Globals.MAP = emulator.ReadShort("MAP");
        if (Globals.NO_DART != null) {
            Globals.PARTY_SLOT[0] = (byte) Globals.NO_DART;
        } else {
            Globals.PARTY_SLOT[0] = emulator.ReadByte("PARTY_SLOT");
        }
		Globals.PARTY_SLOT[1] = emulator.ReadByte("PARTY_SLOT", 4);
        Globals.PARTY_SLOT[2] = emulator.ReadByte("PARTY_SLOT", 8);
		Globals.DRAGOON_SPIRITS = emulator.ReadByte("DRAGOON_SPIRITS");
		Globals.IN_BATTLE = Globals.BATTLE_VALUE == 41215 ? true : Globals.BATTLE_VALUE == 9999 ? true : false;
		
		/*if (!Globals.STATS_CHANGED && Globals.BATTLE_VALUE != 9999) {
			Constants.WriteDebug("Battle Value: " + Globals.BATTLE_VALUE + " / In Battle: " + 
			 Globals.IN_BATTLE + " / Disc: " + Globals.DISC + " / " + "Chapter: " + (Globals.CHAPTER) +
			 " / Encounter: " + Globals.ENCOUNTER_ID + " / Map: " + Globals.MAP);
			 Constants.WriteDebug("Current Time: " + DateTime.Now.ToString("h:mm:ss tt"));
		}*/
	}
	
	public static void Open(Emulator emulator) {}
	public static void Close(Emulator emulator) {}
	public static void Click(Emulator emulator) {}
}