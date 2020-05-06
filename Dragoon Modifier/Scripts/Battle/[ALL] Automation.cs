using Dragoon_Modifier;
using System;
using System.Collections.Generic;
using System.IO;

public class Automation {

    public static void Run(Emulator emulator) {
		if (emulator.ReadShort(0xC6AE8) == 0xA0FF) 
			emulator.WriteByte(0x1075F8, 0x1);
		
		if (emulator.ReadShort(0x1086D0) == 0x13)
			emulator.WriteShort(0x1085D8, 0x2);
		if (emulator.ReadShort(0x1086D0) == 0x13)
			emulator.WriteShort(0x1086DA, 0x1000);
		if (emulator.ReadShort(0x1086D0) == 0x13)
			emulator.WriteShort(0x1086F6, 0x1000);
		if (emulator.ReadShort(0x1086D0) == 0x13)
			emulator.WriteShort(0x1079D4, 0x4);
		if (emulator.ReadShort(0x1086D0) == 0x13)
			emulator.WriteShort(0x1079D6, 0x2402);
		
	}

	public static void Open(Emulator emulator) {}
	public static void Close(Emulator emulator) {}
	public static void Click(Emulator emulator) {}
}