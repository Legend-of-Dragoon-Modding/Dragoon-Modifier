using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;



namespace Dragoon_Modifier {
    public static class Emulator {
        const int PROCESS_ALL_ACCESS = 0x1F0FFF;

        static readonly int[] Empty = new int[0];

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(IntPtr hProcess, long lpBaseAddress, byte[] lpBuffer, long dwSize, out long lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        public static extern Int32 CloseHandle(IntPtr hProcess);

        static Process process = new Process();
        static IntPtr startOffset = new IntPtr();
        static long start = 0x0;
        static IntPtr endOffset = new IntPtr();
        static long end = 0x0;

        static IntPtr processHandle = new IntPtr();

        public static int GetProcIdFromName(string name) //new 1.0.2 function
        {
            Process[] processlist = Process.GetProcesses();

            if (name.ToLower().Contains(".exe"))
                name = name.Replace(".exe", "");

            foreach (Process theprocess in processlist) {
                if (theprocess.ProcessName.Equals(name, StringComparison.CurrentCultureIgnoreCase)) //find (name).exe in the process list (use task manager to find the name)

                    return theprocess.Id;
            }

            return 0; //if we fail to find it
        }

        public static bool Setup(string emuName, bool baseScan) {
            Process[] processList = Process.GetProcesses();
            long baseAddress = 0;
            if (emuName.ToLower().Contains(".exe"))
                emuName = emuName.Replace(".exe", "");

            foreach (Process theprocess in processList) {
                if (theprocess.ProcessName.Equals(emuName, StringComparison.CurrentCultureIgnoreCase) || theprocess.ProcessName.Contains(emuName.ToLower())) { //find (name).exe in the process list (use task manager to find the name)
                    process = theprocess;
                    startOffset = process.MainModule.BaseAddress;
                    baseAddress = (long) process.MainModule.BaseAddress;
                    start = startOffset.ToInt64();
                    endOffset = IntPtr.Add(startOffset, process.MainModule.ModuleMemorySize);
                    end = endOffset.ToInt64();
                    processHandle = OpenProcess(PROCESS_ALL_ACCESS, false, process.Id);
                    break;
                }
            }

            if (start == 0x0 && end == 0x0) {
                Constants.WriteDebug("Failed to attach, no process found.");
                return false;
            } else if(baseScan) {
                string AoBCheck = "50 53 2D 58 20 45 58 45";
                if (emuName.ToLower() == "retroarch") { // RetroArch hotfix
                    start = 0x40000000;
                    end = 0x401F4000;
                }
                Constants.WriteOutput("Start Scan: " + Convert.ToString(start, 16).ToUpper() + " - " + Convert.ToString(end, 16).ToUpper());
                var results = ScanAoB(start, end, AoBCheck, false, true);
                foreach (long x in results) {
                    Constants.OFFSET = x - (long) 0xB070;
                    if (ReadUInt("STARTUP_SEARCH") == 320386 || ReadUShort("BATTLE_VALUE") == 32776 || ReadUShort("BATTLE_VALUE") == 41215) {
                        Constants.KEY.SetValue("Offset", Constants.OFFSET);
                        Constants.WriteOutput("Base scan success.");
                        break;
                    } else {
                        Constants.OFFSET = 0;
                    }
                }

                if (Constants.OFFSET <= 0) { //Fallback
                    Constants.WriteOutput("PSX EXE scan failed. Trying static offsets...");
                    long[] knownOffsets = { 0x5B6E40, 0x94C020, 0xA52EA0, 0xA579A0, 0xA8B6A0, 0x24000000 };
                    long[] baseOffsets = { 0x81A020, 0x825140, 0xA82020 };
                    bool found = false;

                    foreach (long address in knownOffsets) {
                        Constants.OFFSET = address;
                        if (ReadUInt("STARTUP_SEARCH") == 320386 || ReadUShort("BATTLE_VALUE") == 32776 || ReadUShort("BATTLE_VALUE") == 41215) {
                            Constants.KEY.SetValue("Offset", Constants.OFFSET);
                            Constants.WriteOutput("Static manual offset scan success.");
                            found = true;
                            break;
                        } else {
                            Constants.OFFSET = 0;
                        }
                    }

                    if (!found) {
                        foreach (long address in baseOffsets) {
                            Constants.OFFSET = baseAddress + address;
                            if (ReadUInt("STARTUP_SEARCH") == 320386 || ReadUShort("BATTLE_VALUE") == 32776 || ReadUShort("BATTLE_VALUE") == 41215) {
                                Constants.KEY.SetValue("Offset", Constants.OFFSET);
                                Constants.WriteOutput("Static base offset scan success.");
                                break;
                            } else {
                                Constants.OFFSET = 0;
                            }
                        }
                    }
                }

                if (Constants.OFFSET <= 0) {
                    Constants.WriteOutput("Scan failed. Please try opening Dragoon Modifier on the game's title or load screen.");
                    return false;
                } else {
                    Constants.WriteDebug($"Calculated offset: {Constants.OFFSET.ToString("X2")}");
                    return true;
                }
            }
            return false;
        }
        public static byte ReadByte(long address) {
            byte[] buffer = new byte[1];
            ReadProcessMemory(processHandle, address + Constants.OFFSET, buffer, 1, out long bytesRead);
            return buffer[0];
        }
        public static byte ReadByte(string address, int offset = 0) {
            byte[] buffer = new byte[1];
            ReadProcessMemory(processHandle, Constants.GetAddress(address) + Constants.OFFSET + offset, buffer, 1, out long bytesRead);
            return buffer[0];
        }
        public static short ReadShort(long address) {
            byte[] buffer = new byte[2];
            ReadProcessMemory(processHandle, address + Constants.OFFSET, buffer, 2, out long bytesRead);
            return BitConverter.ToInt16(buffer, 0);
        }
        public static short ReadShort(string address, int offset = 0) {
            byte[] buffer = new byte[2];
            ReadProcessMemory(processHandle, Constants.GetAddress(address) + Constants.OFFSET + offset, buffer, 2, out long bytesRead);
            return BitConverter.ToInt16(buffer, 0);
        }
        public static ushort ReadUShort(long address) {
            byte[] buffer = new byte[2];
            ReadProcessMemory(processHandle, address + Constants.OFFSET, buffer, 2, out long bytesRead);
            return BitConverter.ToUInt16(buffer, 0);
        }
        public static ushort ReadUShort(string address, int offset = 0) {
            byte[] buffer = new byte[2];
            ReadProcessMemory(processHandle, Constants.GetAddress(address) + Constants.OFFSET + offset, buffer, 2, out long bytesRead);
            return BitConverter.ToUInt16(buffer, 0);
        }
        public static Int32 ReadInt(long address) {
            byte[] buffer = new byte[4];
            ReadProcessMemory(processHandle, address + Constants.OFFSET, buffer, 4, out long bytesRead);
            return BitConverter.ToInt32(buffer, 0);
        }
        public static Int32 ReadInt(string address, int offset = 0) {
            byte[] buffer = new byte[4];
            ReadProcessMemory(processHandle, Constants.GetAddress(address) + Constants.OFFSET + offset, buffer, 4, out long bytesRead);
            return BitConverter.ToInt32(buffer, 0);
        }
        public static UInt32 ReadUInt(long address) {
            byte[] buffer = new byte[4];
            ReadProcessMemory(processHandle, address + Constants.OFFSET, buffer, 4, out long bytesRead);
            return BitConverter.ToUInt32(buffer, 0);
        }
        public static UInt32 ReadUInt(string address, int offset = 0) {
            byte[] buffer = new byte[4];
            ReadProcessMemory(processHandle, Constants.GetAddress(address) + Constants.OFFSET + offset, buffer, 4, out long bytesRead);
            return BitConverter.ToUInt32(buffer, 0);
        }
        public static long ReadLong(long address) {
            byte[] buffer = new byte[8];
            ReadProcessMemory(processHandle, address + Constants.OFFSET, buffer, 4, out long bytesRead);
            return BitConverter.ToInt64(buffer, 0);
        }
        public static long ReadLong(string address, int offset = 0) {
            byte[] buffer = new byte[8];
            ReadProcessMemory(processHandle, Constants.GetAddress(address) + Constants.OFFSET + offset, buffer, 4, out long bytesRead);
            return BitConverter.ToInt64(buffer, 0);
        }
        public static ulong ReadULong(long address) {
            byte[] buffer = new byte[8];
            ReadProcessMemory(processHandle, address + Constants.OFFSET, buffer, 4, out long bytesRead);
            return BitConverter.ToUInt64(buffer, 0);
        }
        public static ulong ReadULong(string address, int offset = 0) {
            byte[] buffer = new byte[8];
            ReadProcessMemory(processHandle, Constants.GetAddress(address) + Constants.OFFSET + offset, buffer, 4, out long bytesRead);
            return BitConverter.ToUInt64(buffer, 0);
        }
        public static byte[] ReadAoB(long startAddr, long endAddr) {
            long len = (long) (endAddr - startAddr);
            byte[] buffer = new byte[len];
            ReadProcessMemory(processHandle, startAddr, buffer, len, out long bytesRead);
            return buffer;
        }
        public static void WriteByte(long address, int value) {
            var val = BitConverter.GetBytes((byte) value);
            WriteProcessMemory(processHandle, new IntPtr(address + Constants.OFFSET), val, 1, out int error);
        }
        public static void WriteByteDirect(long address, int value) {
            var val = BitConverter.GetBytes((byte) value);
            WriteProcessMemory(processHandle, new IntPtr(address), val, 1, out int error);
        }
        public static void WriteByte(string address, int value, int offset = 0) {
            var val = BitConverter.GetBytes((byte) value);
            WriteProcessMemory(processHandle, new IntPtr(Constants.GetAddress(address) + Constants.OFFSET + offset), val, 1, out int error);
        }
        public static void WriteShort(long address, int value) {
            var val = BitConverter.GetBytes((short) value);
            WriteProcessMemory(processHandle, new IntPtr(address + Constants.OFFSET), val, 2, out int error);
        }
        public static void WriteShort(string address, int value, int offset = 0) {
            var val = BitConverter.GetBytes((short) value);
            WriteProcessMemory(processHandle, new IntPtr(Constants.GetAddress(address) + Constants.OFFSET + offset), val, 2, out int error);
        }
        public static void WriteUShort(long address, int value) {
            var val = BitConverter.GetBytes((ushort) value);
            WriteProcessMemory(processHandle, new IntPtr(address + Constants.OFFSET), val, 2, out int error);
        }
        public static void WriteUShort(string address, int value, int offset = 0) {
            var val = BitConverter.GetBytes((ushort) value);
            WriteProcessMemory(processHandle, new IntPtr(Constants.GetAddress(address) + Constants.OFFSET + offset), val, 2, out int error);
        }
        public static void WriteInt(long address, Int32 value) {
            var val = BitConverter.GetBytes(value);
            WriteProcessMemory(processHandle, new IntPtr(address + Constants.OFFSET), val, 4, out int error);
        }
        public static void WriteInt(string address, Int32 value, int offset = 0) {
            var val = BitConverter.GetBytes(value);
            WriteProcessMemory(processHandle, new IntPtr(Constants.GetAddress(address) + Constants.OFFSET + offset), val, 4, out int error);
        }
        public static void WriteUInt(long address, UInt32 value) {
            var val = BitConverter.GetBytes(value);
            WriteProcessMemory(processHandle, new IntPtr(address + Constants.OFFSET), val, 4, out int error);
        }
        public static void WriteUInt(string address, UInt32 value, int offset = 0) {
            var val = BitConverter.GetBytes(value);
            WriteProcessMemory(processHandle, new IntPtr(Constants.GetAddress(address) + Constants.OFFSET + offset), val, 4, out int error);
        }
        public static void WriteLong(long address, long value) {
            var val = BitConverter.GetBytes(value);
            WriteProcessMemory(processHandle, new IntPtr(address + Constants.OFFSET), val, 8, out int error);
        }
        public static void WriteLong(string address, long value, int offset = 0) {
            var val = BitConverter.GetBytes(value);
            WriteProcessMemory(processHandle, new IntPtr(Constants.GetAddress(address) + Constants.OFFSET + offset), val, 8, out int error);
        }
        public static void WriteULong(long address, ulong value) {
            var val = BitConverter.GetBytes(value);
            WriteProcessMemory(processHandle, new IntPtr(address + Constants.OFFSET), val, 8, out int error);
        }
        public static void WriteULong(string address, ulong value, int offset = 0) {
            var val = BitConverter.GetBytes(value);
            WriteProcessMemory(processHandle, new IntPtr(Constants.GetAddress(address) + Constants.OFFSET + offset), val, 8, out int error);
        }
        public static void WriteAoB(long address, string values) {
            string[] strArr = values.Split(' ');
            byte[] arr = new byte[strArr.Length];
            for (int i = 0; i < strArr.Length; i++) {
                arr[i] = Convert.ToByte(strArr[i], 16);
            }
            WriteProcessMemory(processHandle, new IntPtr(address + Constants.OFFSET), arr, arr.Length, out int error);
        }
        public static void WriteAoB(string address, string values) {
            string[] strArr = values.Split(' ');
            byte[] arr = new byte[strArr.Length];
            for (int i = 0; i < strArr.Length; i++) {
                arr[i] = Convert.ToByte(strArr[i], 16);
            }
            WriteProcessMemory(processHandle, new IntPtr(Constants.GetAddress(address) + Constants.OFFSET), arr, arr.Length, out int error);
        }
        public static void WriteText(long address, string values) {
            string[] strArr = LoDDict.StringEncode(values).Split(' ');
            byte[] arr = new byte[strArr.Length];
            for (int i = 0; i < strArr.Length; i++) {
                arr[i] = Convert.ToByte(strArr[i], 16);
            }
            WriteProcessMemory(processHandle, new IntPtr(address + Constants.OFFSET), arr, arr.Length, out int error);
        }
        public static void WriteText(string address, string values) {
            string[] strArr = LoDDict.StringEncode(values).Split(' ');
            byte[] arr = new byte[strArr.Length];
            for (int i = 0; i < strArr.Length; i++) {
                arr[i] = Convert.ToByte(strArr[i], 16);
            }
            WriteProcessMemory(processHandle, new IntPtr(Constants.GetAddress(address) + Constants.OFFSET), arr, arr.Length, out int error);
        }
        public static List<long> ScanAoB(long startAddr, long endAddr, string values, bool useOffset = true, bool addOffset = false) {
            List<long> results = new List<long>();
            if (useOffset) {
                startAddr += Constants.OFFSET;
                endAddr += Constants.OFFSET;
            }
            if (endAddr > end) {
                endAddr = end;
                Constants.WriteDebug("Limiting AoB Scan to emulator memory");
            }
            if (startAddr >= endAddr) {
                Constants.WriteDebug("Incorrect start/end address");
                return results;
            }
            string[] maskStr = values.Split(' ');
            byte[] pattern = new byte[maskStr.Length];
            byte[] maskArr = new byte[maskStr.Length];
            int i = 0;
            foreach (string element in maskStr) {
                if (element.Contains('?')) {
                    if (element == "??") {
                        maskArr[i] = 0xFF;
                    } else {
                        char[] toPattern = element.ToCharArray();
                        if (element[0] == '?') {
                            toPattern[0] = '0';
                            maskArr[i] = 0x0F;
                        } else {
                            toPattern[1] = '0';
                            maskArr[i] = 0xF0;
                        }
                        string str = new string(toPattern);
                        pattern[i] = Convert.ToByte(str, 16);

                    }
                } else {
                    pattern[i] = Convert.ToByte(element, 16);
                    maskArr[i] = 0x00;
                }
                i++;
            }

            byte[] data = ReadAoB(startAddr, endAddr);
            foreach (var position in data.Locate(pattern, maskArr)) {
                var temp = position + startAddr;
                if (!addOffset) {
                    temp -= Constants.OFFSET;
                }
                results.Add(temp);
            }

            return results;
        }
        public static int[] Locate(this byte[] self, byte[] candidate, byte[] mask) {
            if (IsEmptyLocate(self, candidate))
                return Empty;
            var list = new List<int>();
            for (int i = 0; i < self.Length; i++) {
                if (!IsMatch(self, i, candidate, mask)) {
                    continue;
                }
                list.Add(i);
            }
            return list.Count == 0 ? Empty : list.ToArray();
        }
        static bool IsMatch(byte[] array, int position, byte[] candidate, byte[] mask) {
            if (candidate.Length > (array.Length - position)) {
                return false;
            }
            for (int i = 0; i < candidate.Length; i++) {
                if (mask[i] == 0x00) {
                    if (array[position + i] != candidate[i]) {
                        return false;
                    }
                } else if (mask[i] != 0xFF) {
                    if ((array[position + i] & mask[i]) != (array[position + i] & candidate[i])) {
                        return false;
                    }
                }

            }
            return true;
        }
        static bool IsEmptyLocate(byte[] array, byte[] candidate) {
            return array == null
                || candidate == null
                || array.Length == 0
                || candidate.Length == 0
                || candidate.Length > array.Length;
        }
        public static string ReadName(int nameAddress) {
            string name = "";
            for (int i = 0; i <= 20; i++) {
                if (GetCharacterByNumber(nameAddress + (i * 2)) == "")
                    break;
                name += GetCharacterByNumber(nameAddress + (i * 2));
            }
            return name;
        }
        public static string GetCharacterByNumber(int charAddress) {
            byte character = ReadByte(charAddress);
            if (character == 0) {
                return " ";
            } else if (character == 1) {
                return ",";
            } else if (character == 2) {
                return ".";
            } else if (character == 3) {
                return "-";
            } else if (character == 4) {
                return ":";
            } else if (character == 5) {
                return "?";
            } else if (character == 6) {
                return "!";
            } else if (character == 7) {
                return "_";
            } else if (character == 8) {
                return "/";
            } else if (character == 9) {
                return "'";
            } else if (character == 10) {
                return "\"";
            } else if (character == 11) {
                return "(";
            } else if (character == 12) {
                return ")";
            } else if (character == 13) {
                return "—";
            } else if (character == 14) {
                return "@";
            } else if (character == 15) {
                return "%";
            } else if (character == 16) {
                return "&";
            } else if (character == 17) {
                return "*";
            } else if (character == 18) {
                return "#";
            } else if (character == 19) {
                return ">";
            } else if (character == 20) {
                return ",";
            } else if (character == 21) {
                return "0";
            } else if (character == 22) {
                return "1";
            } else if (character == 23) {
                return "2";
            } else if (character == 24) {
                return "3";
            } else if (character == 25) {
                return "4";
            } else if (character == 26) {
                return "5";
            } else if (character == 27) {
                return "6";
            } else if (character == 28) {
                return "7";
            } else if (character == 29) {
                return "8";
            } else if (character == 30) {
                return "9";
            } else if (character == 31) {
                return "A";
            } else if (character == 32) {
                return "B";
            } else if (character == 33) {
                return "C";
            } else if (character == 34) {
                return "D";
            } else if (character == 35) {
                return "E";
            } else if (character == 36) {
                return "F";
            } else if (character == 37) {
                return "G";
            } else if (character == 38) {
                return "H";
            } else if (character == 39) {
                return "I";
            } else if (character == 40) {
                return "J";
            } else if (character == 41) {
                return "K";
            } else if (character == 42) {
                return "L";
            } else if (character == 43) {
                return "M";
            } else if (character == 44) {
                return "N";
            } else if (character == 45) {
                return "O";
            } else if (character == 46) {
                return "P";
            } else if (character == 47) {
                return "Q";
            } else if (character == 48) {
                return "R";
            } else if (character == 49) {
                return "S";
            } else if (character == 50) {
                return "T";
            } else if (character == 51) {
                return "U";
            } else if (character == 52) {
                return "V";
            } else if (character == 53) {
                return "W";
            } else if (character == 54) {
                return "X";
            } else if (character == 55) {
                return "Y";
            } else if (character == 56) {
                return "Z";
            } else if (character == 57) {
                return "a";
            } else if (character == 58) {
                return "b";
            } else if (character == 59) {
                return "c";
            } else if (character == 60) {
                return "d";
            } else if (character == 61) {
                return "e";
            } else if (character == 62) {
                return "f";
            } else if (character == 63) {
                return "g";
            } else if (character == 64) {
                return "h";
            } else if (character == 65) {
                return "i";
            } else if (character == 66) {
                return "j";
            } else if (character == 67) {
                return "k";
            } else if (character == 68) {
                return "l";
            } else if (character == 69) {
                return "m";
            } else if (character == 70) {
                return "n";
            } else if (character == 71) {
                return "o";
            } else if (character == 72) {
                return "p";
            } else if (character == 73) {
                return "q";
            } else if (character == 74) {
                return "r";
            } else if (character == 75) {
                return "s";
            } else if (character == 76) {
                return "t";
            } else if (character == 77) {
                return "u";
            } else if (character == 78) {
                return "v";
            } else if (character == 79) {
                return "w";
            } else if (character == 80) {
                return "x";
            } else if (character == 81) {
                return "y";
            } else if (character == 82) {
                return "z";
            } else if (character == 83) {
                return "[";
            } else if (character == 84) {
                return "]";
            } else {
                return "";
            }
        }
        public static byte GetCharacterByChar(char character) {
            if (character == ' ') {
                return 0;
            } else if (character == ',') {
                return 1;
            } else if (character == '.') {
                return 2;
            } else if (character == '-') {
                return 3;
            } else if (character == ':') {
                return 4;
            } else if (character == '?') {
                return 5;
            } else if (character == '!') {
                return 6;
            } else if (character == '_') {
                return 7;
            } else if (character == '/') {
                return 8;
            } else if (character == '\'') {
                return 9;
            } else if (character == '"') {
                return 10;
            } else if (character == '(') {
                return 11;
            } else if (character == ')') {
                return 12;
            } else if (character == '—') {
                return 13;
            } else if (character == '@') {
                return 14;
            } else if (character == '%') {
                return 15;
            } else if (character == '&') {
                return 16;
            } else if (character == '*') {
                return 17;
            } else if (character == '#') {
                return 18;
            } else if (character == '>') {
                return 19;
            } else if (character == '<') {
                return 20;
            } else if (character == '0') {
                return 21;
            } else if (character == '1') {
                return 22;
            } else if (character == '2') {
                return 23;
            } else if (character == '3') {
                return 24;
            } else if (character == '4') {
                return 25;
            } else if (character == '5') {
                return 26;
            } else if (character == '6') {
                return 27;
            } else if (character == '7') {
                return 28;
            } else if (character == '8') {
                return 29;
            } else if (character == '9') {
                return 30;
            } else if (character == 'A') {
                return 31;
            } else if (character == 'B') {
                return 32;
            } else if (character == 'C') {
                return 33;
            } else if (character == 'D') {
                return 34;
            } else if (character == 'E') {
                return 35;
            } else if (character == 'F') {
                return 36;
            } else if (character == 'G') {
                return 37;
            } else if (character == 'H') {
                return 38;
            } else if (character == 'I') {
                return 39;
            } else if (character == 'J') {
                return 40;
            } else if (character == 'K') {
                return 41;
            } else if (character == 'L') {
                return 42;
            } else if (character == 'M') {
                return 43;
            } else if (character == 'N') {
                return 44;
            } else if (character == 'O') {
                return 45;
            } else if (character == 'P') {
                return 46;
            } else if (character == 'Q') {
                return 47;
            } else if (character == 'R') {
                return 48;
            } else if (character == 'S') {
                return 49;
            } else if (character == 'T') {
                return 50;
            } else if (character == 'U') {
                return 51;
            } else if (character == 'V') {
                return 52;
            } else if (character == 'W') {
                return 53;
            } else if (character == 'X') {
                return 54;
            } else if (character == 'Y') {
                return 55;
            } else if (character == 'Z') {
                return 56;
            } else if (character == 'a') {
                return 57;
            } else if (character == 'b') {
                return 58;
            } else if (character == 'c') {
                return 59;
            } else if (character == 'd') {
                return 60;
            } else if (character == 'e') {
                return 61;
            } else if (character == 'f') {
                return 62;
            } else if (character == 'g') {
                return 63;
            } else if (character == 'h') {
                return 64;
            } else if (character == 'i') {
                return 65;
            } else if (character == 'j') {
                return 66;
            } else if (character == 'k') {
                return 67;
            } else if (character == 'l') {
                return 68;
            } else if (character == 'm') {
                return 69;
            } else if (character == 'n') {
                return 70;
            } else if (character == 'o') {
                return 71;
            } else if (character == 'p') {
                return 72;
            } else if (character == 'q') {
                return 73;
            } else if (character == 'r') {
                return 74;
            } else if (character == 's') {
                return 75;
            } else if (character == 't') {
                return 76;
            } else if (character == 'u') {
                return 77;
            } else if (character == 'v') {
                return 78;
            } else if (character == 'w') {
                return 79;
            } else if (character == 'x') {
                return 80;
            } else if (character == 'y') {
                return 81;
            } else if (character == 'z') {
                return 82;
            } else if (character == '[') {
                return 83;
            } else if (character == ']') {
                return 84;
            } else {
                return 0;
            }
        }
    }
}
