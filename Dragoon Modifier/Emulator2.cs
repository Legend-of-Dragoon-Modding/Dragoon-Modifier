using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;



namespace Dragoon_Modifier {
    static class Emulator2 {
        static string[] emuList = {
            "ePSXe",
            "retroarch",

        };

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
            if (name.ToLower().Contains(".bin")) // test
                name = name.Replace(".bin", "");

            foreach (Process theprocess in processlist) {
                if (theprocess.ProcessName.Equals(name, StringComparison.CurrentCultureIgnoreCase)) //find (name).exe in the process list (use task manager to find the name)

                    return theprocess.Id;
            }

            return 0; //if we fail to find it
        }

        public static void Setup(string emuName, bool baseScan) {
            
            Process[] processList = Process.GetProcesses();

            if (emuName.ToLower().Contains(".exe"))
                emuName = emuName.Replace(".exe", "");
            if (emuName.ToLower().Contains(".bin")) // test
                emuName = emuName.Replace(".bin", "");

            foreach (Process theprocess in processList) {
                if (theprocess.ProcessName.Equals(emuName, StringComparison.CurrentCultureIgnoreCase)) { //find (name).exe in the process list (use task manager to find the name)
                    process = theprocess;
                    startOffset = process.MainModule.BaseAddress;
                    start = startOffset.ToInt64();
                    endOffset = IntPtr.Add(startOffset, process.MainModule.ModuleMemorySize);
                    end = endOffset.ToInt64();
                    processHandle = OpenProcess(PROCESS_ALL_ACCESS, false, process.Id);
                    break;
                }
            }

            if (start == 0x0 && end == 0x0) {
                Constants.WriteDebug("Failed to attach.");
            } else if(baseScan) {
                string AoBCheck = "50 53 2D 58 20 45 58 45";
                if (emuName.ToLower() == "retroarch") { // RetroArch hotfix
                    start = 0x40000000;
                    end = 0x401F4000;
                }
                var results = ScanAoB(start, end, AoBCheck, false);
                foreach (long x in results) {
                    Constants.OFFSET = x - (long) 0xB070;
                    if (ReadUInt("STARTUP_SEARCH") == 320386 || ReadUShort("BATTLE_VALUE") == 32776 || ReadUShort("BATTLE_VALUE") == 41215) {
                        Constants.KEY.SetValue("Offset", Constants.OFFSET);
                        break;
                    } else {
                        Constants.OFFSET = 0;
                    }
                }
                if (Constants.OFFSET <= 0) {
                    Constants.WriteDebug("Failed to attach.");
                } else {
                    Constants.WriteDebug($"Calculated offset: {Constants.OFFSET.ToString("X2")}");
                }
            }
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

        public static void WriteByte(long address, byte value) {
            var val = BitConverter.GetBytes(value);
            WriteProcessMemory(processHandle, new IntPtr(address + Constants.OFFSET), val, 1, out int error);
        }

        public static void WriteByte(string address, byte value, int offset = 0) {
            var val = BitConverter.GetBytes(value);
            WriteProcessMemory(processHandle, new IntPtr(Constants.GetAddress(address) + Constants.OFFSET + offset), val, 1, out int error);
        }

        public static void WriteShort(long address, short value) {
            var val = BitConverter.GetBytes(value);
            WriteProcessMemory(processHandle, new IntPtr(address + Constants.OFFSET), val, 2, out int error);
        }

        public static void WriteShort(string address, short value, int offset = 0) {
            var val = BitConverter.GetBytes(value);
            WriteProcessMemory(processHandle, new IntPtr(Constants.GetAddress(address) + Constants.OFFSET + offset), val, 2, out int error);
        }

        public static void WriteUShort(long address, ushort value) {
            var val = BitConverter.GetBytes(value);
            WriteProcessMemory(processHandle, new IntPtr(address + Constants.OFFSET), val, 2, out int error);
        }

        public static void WriteUShort(string address, ushort value, int offset = 0) {
            var val = BitConverter.GetBytes(value);
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

        public static List<long> ScanAoB(long startAddr, long endAddr, string values, bool useOffset = true) {
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
                results.Add(position + startAddr);
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

    }
}
