using Dragoon_Modifier;
using System;
using System.Collections.Generic;
using System.IO;

public class MonsterStatTSV {
	static bool WRITE = false;
	static bool UPDATE_MODE = false;

    public static void Run() {
		if (Globals.GAME_STATE == Globals.GameStateEnum.Battle && Globals.STATS_CHANGED && !WRITE) {
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
						monsterData.WriteLine(GetStats(offset, monster));
					}
					mid.Add(monster);
				} else {
					Constants.WriteDebug("[TSV - Monster] Existing monster: " + monster);
					if (UPDATE_MODE) {
						using (var monsterData = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "/Mods/Monster_Data.tsv")) {
							monsterData.WriteLine(header);
							foreach (string txt in tsvData) {
								if (monster == Int32.Parse(txt.Split('\t')[0])) {
									monsterData.WriteLine(GetStats(offset, monster));
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
			if (Globals.GAME_STATE != Globals.GameStateEnum.Battle) {
				WRITE = false;
            }
        }
	}

	public static string GetStats(int offset, int monster) {
		string stats = "";
		string temp = "";
		stats += monster;
		stats += '\t';
		stats += Emulator.ReadName(0xC69D0 + (0x2C * offset));
		stats += '\t';
		switch ((int) Globals.BattleController.MonsterTable[offset].Element) {
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
		stats += Globals.BattleController.MonsterTable[offset].Max_HP;
		stats += '\t';
		stats += Globals.BattleController.MonsterTable[offset].AT;
		stats += '\t';
		stats += Globals.BattleController.MonsterTable[offset].MAT;
		stats += '\t';
		stats += Globals.BattleController.MonsterTable[offset].DF;
		stats += '\t';
		stats += Globals.BattleController.MonsterTable[offset].MDF;
		stats += '\t';
		stats += Globals.BattleController.MonsterTable[offset].SPD;
		stats += '\t';
		stats += Globals.BattleController.MonsterTable[offset].A_AV;
		stats += '\t';
		stats += Globals.BattleController.MonsterTable[offset].M_AV;
		stats += '\t';
		stats += Globals.BattleController.MonsterTable[offset].P_Immune;
		stats += '\t';
		stats += Globals.BattleController.MonsterTable[offset].M_Immune;
		stats += '\t';
		stats += Globals.BattleController.MonsterTable[offset].P_Half;
		stats += '\t';
		stats += Globals.BattleController.MonsterTable[offset].M_Half;
		stats += '\t';
		switch ((int) Globals.BattleController.MonsterTable[offset].E_Immune) {
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
		switch ((int) Globals.BattleController.MonsterTable[offset].E_Half) {
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
		stats += Globals.BattleController.MonsterTable[offset].StatusResist;
		stats += '\t';
		stats += Globals.BattleController.MonsterTable[offset].Special_Effect;
		stats += '\t';
		stats += Emulator.ReadShort(Constants.GetAddress("MONSTER_REWARDS") + (int) Globals.UNIQUE_MONSTER_IDS.IndexOf(monster) * 0x1A8);
		stats += '\t';
		stats += Emulator.ReadShort(Constants.GetAddress("MONSTER_REWARDS") + (int) 0x2 + Globals.UNIQUE_MONSTER_IDS.IndexOf(monster) * 0x1A8);
		stats += '\t';
		try {
			stats += Globals.DICTIONARY.Num2Item[Emulator.ReadByte(Constants.GetAddress("MONSTER_REWARDS") + (int) 0x5 + Globals.UNIQUE_MONSTER_IDS.IndexOf(monster) * 0x1A8)];
		} catch (Exception e) {
			Console.WriteLine("Key Failure: " + Emulator.ReadByte(Constants.GetAddress("MONSTER_REWARDS") + (int) 0x5 + Globals.UNIQUE_MONSTER_IDS.IndexOf(monster) * 0x1A8));
			stats += Globals.DICTIONARY.Num2Item[0];
		}
		stats += '\t';
		stats += Emulator.ReadByte(Constants.GetAddress("MONSTER_REWARDS") + (int) 0x4 + Globals.UNIQUE_MONSTER_IDS.IndexOf(monster) * 0x1A8);
		stats += '\t';
		//stats += Globals.MONSTER_TABLE[offset].Read("Counter");
		//stats += '\t';
		stats += Emulator.ReadName(0xC69D0 + (0x2C * offset));
		Console.WriteLine(stats);
		return stats;
	}

	public static void Open() {
		WRITE = false;
	}
	
	public static void Close() {
		WRITE = false;
	}
	
	public static void Click() {}
}