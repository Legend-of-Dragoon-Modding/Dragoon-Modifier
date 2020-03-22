using Dragoon_Modifier;
using System;
using System.Collections.Generic;
using System.IO;

public class MonsterStatTSV {
	static bool WRITE = false;
	static bool UPDATE_MODE = false;

    public static void Run(Emulator emulator) {
		if (Globals.IN_BATTLE && Globals.STATS_CHANGED && !WRITE) {
			List<string> tsvData = new List<string>();
			List<int> mid = new List<int>();
			string line;
			string header;
			int offset = 0;

			using (var monsterData = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "/Mods/Monster_Data.tsv")) {
				header = monsterData.ReadLine();
				while ((line = monsterData.ReadLine()) != null) {
					tsvData.Add(line);
					mid.Add(Int32.Parse(line.Split('\t')[0]));
                }
			}

			offset = 0;
			foreach (int monster in Globals.MONSTER_IDS) {
				if (!mid.Contains(monster)) {
					Constants.WriteDebug("[TSV - Monster] Writing new monster: " + monster);
					using (var monsterData = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "/Mods/Monster_Data.tsv", true)) {
						monsterData.WriteLine(GetStats(offset, emulator, monster));
					}
					mid.Add(monster);
				} else {
					Constants.WriteDebug("[TSV - Monster] Existing monster: " + monster);
					if (UPDATE_MODE) {
						using (var monsterData = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "/Mods/Monster_Data.tsv")) {
							monsterData.WriteLine(header);
							foreach (string txt in tsvData) {
								if (monster == Int32.Parse(txt.Split('\t')[0])) {
									monsterData.WriteLine(GetStats(offset, emulator, monster));
								} else {
									monsterData.WriteLine(txt);
								}
							}
						}
					}
				}
				offset++;
			}

			WRITE = true;
		} else {
			if (!Globals.IN_BATTLE) {
				WRITE = false;
            }
        }
	}

	public static string GetStats(int offset, Emulator emulator, int monster) {
		string stats = "";
		string temp = "";
		stats += monster;
		stats += '\t';
		stats += emulator.ReadName(0xC69D0 + (0x2C * offset));
		stats += '\t';
		switch ((int) Globals.MONSTER_TABLE[offset].Read("Element")) {
			case 0:
				temp = "Null";
				break;
			case 1:
				temp = "Water";
				break;
			case 2:
				temp = "Earth";
				break;
			case 4:
				temp = "Dark";
				break;
			case 8:
				temp = "Non-Elemental";
				break;
			case 16:
				temp = "Thunder";
				break;
			case 32:
				temp = "Light";
				break;
			case 64:
				temp = "Wind";
				break;
			case 128:
				temp = "Fire";
				break;
			default:
				temp = "NEW";
				break;
        }
		stats += temp;
		stats += '\t';
		stats += Globals.MONSTER_TABLE[offset].Read("Max_HP");
		stats += '\t';
		stats += Globals.MONSTER_TABLE[offset].Read("AT");
		stats += '\t';
		stats += Globals.MONSTER_TABLE[offset].Read("MAT");
		stats += '\t';
		stats += Globals.MONSTER_TABLE[offset].Read("DF");
		stats += '\t';
		stats += Globals.MONSTER_TABLE[offset].Read("MDF");
		stats += '\t';
		stats += Globals.MONSTER_TABLE[offset].Read("SPD");
		stats += '\t';
		stats += Globals.MONSTER_TABLE[offset].Read("A_AV");
		stats += '\t';
		stats += Globals.MONSTER_TABLE[offset].Read("M_AV");
		stats += '\t';
		stats += Globals.MONSTER_TABLE[offset].Read("P_Immune");
		stats += '\t';
		stats += Globals.MONSTER_TABLE[offset].Read("M_Immune");
		stats += '\t';
		stats += Globals.MONSTER_TABLE[offset].Read("P_Half");
		stats += '\t';
		stats += Globals.MONSTER_TABLE[offset].Read("M_Half");
		stats += '\t';
		switch ((int) Globals.MONSTER_TABLE[offset].Read("E_Immune")) {
			case 0:
				temp = "None";
				break;
			case 1:
				temp = "Water";
				break;
			case 2:
				temp = "Earth";
				break;
			case 4:
				temp = "Dark";
				break;
			case 8:
				temp = "Non-Elemental";
				break;
			case 16:
				temp = "Thunder";
				break;
			case 32:
				temp = "Light";
				break;
			case 64:
				temp = "Wind";
				break;
			case 128:
				temp = "Fire";
				break;
			default:
				temp = "NEW";
				break;
		}
		stats += temp;
		stats += '\t';
		switch ((int) Globals.MONSTER_TABLE[offset].Read("E_Half")) {
			case 0:
				temp = "None";
				break;
			case 1:
				temp = "Water";
				break;
			case 2:
				temp = "Earth";
				break;
			case 4:
				temp = "Dark";
				break;
			case 8:
				temp = "Non-Elemental";
				break;
			case 16:
				temp = "Thunder";
				break;
			case 32:
				temp = "Light";
				break;
			case 64:
				temp = "Wind";
				break;
			case 128:
				temp = "Fire";
				break;
			default:
				temp = "NEW";
				break;
		}
		stats += temp;
		stats += '\t';
		stats += Globals.MONSTER_TABLE[offset].Read("Stat_Res");
		stats += '\t';
		stats += Globals.MONSTER_TABLE[offset].Read("Death_Res");
		stats += '\t';
		stats += emulator.ReadShort(Constants.GetAddress("MONSTER_REWARDS") + (int) Globals.UNIQUE_MONSTER_IDS.IndexOf(monster) * 0x1A8);
		stats += '\t';
		stats += emulator.ReadShort(Constants.GetAddress("MONSTER_REWARDS") + (int) 0x2 + Globals.UNIQUE_MONSTER_IDS.IndexOf(monster) * 0x1A8);
		stats += '\t';
		stats += Globals.DICTIONARY.Num2Item[emulator.ReadByte(Constants.GetAddress("MONSTER_REWARDS") + (int) 0x5 + Globals.UNIQUE_MONSTER_IDS.IndexOf(monster) * 0x1A8)];
		stats += '\t';
		stats += emulator.ReadByte(Constants.GetAddress("MONSTER_REWARDS") + (int) 0x4 + Globals.UNIQUE_MONSTER_IDS.IndexOf(monster) * 0x1A8);
		stats += '\t';
		stats += emulator.ReadName(0xC69D0 + (0x2C * offset));
		Console.WriteLine(stats);
		return stats;
	}

	public static void Open(Emulator emulator) {}
	public static void Close(Emulator emulator) {}
	public static void Click(Emulator emulator) {}
}