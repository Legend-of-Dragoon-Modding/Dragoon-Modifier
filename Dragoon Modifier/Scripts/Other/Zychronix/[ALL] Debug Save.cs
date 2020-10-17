using Dragoon_Modifier;
using Microsoft.Win32;
using System;
using System.IO;

public class DebugSave {
    public static void Run(Emulator emulator) {}
	public static void Open(Emulator emulator) {}
	public static void Close(Emulator emulator) {}
	public static void Click(Emulator emulator) {
		using (var registryData = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "/Registry_Data.txt", false)) {
			registryData.WriteLine("REGISTRY DATA");
		}
		
		OutputRegKey(Constants.KEY);
		Constants.WriteOutput("Complete.");
	}
	
	private static void ProcessValueNames(RegistryKey Key) {
		string[] valuenames = Key.GetValueNames();
		if (valuenames == null || valuenames.Length <= 0)
			return;
		foreach (string valuename in valuenames) {
			object obj = Key.GetValue(valuename);
			if (obj != null) {
				using (var registryData = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "/Registry_Data.txt", true)) {
					registryData.WriteLine(Key.Name + " " + valuename + " " + obj.ToString());
				}
			}
		}
	}

	public static void OutputRegKey(RegistryKey Key) {
		try {
			string[] subkeynames = Key.GetSubKeyNames(); 
			if (subkeynames == null || subkeynames.Length <= 0) { 
				ProcessValueNames(Key);
				return;
			}
			foreach (string keyname in subkeynames) { 
				using (RegistryKey key2 = Key.OpenSubKey(keyname))
					OutputRegKey(key2);
			}
			ProcessValueNames(Key); 
		} catch (Exception e) {}
	}
}