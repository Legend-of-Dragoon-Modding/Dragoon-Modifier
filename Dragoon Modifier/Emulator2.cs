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
        static long start = new long();
        static IntPtr endOffset = new IntPtr();
        static long end = new long();

        static IntPtr processHandle = new IntPtr();

        public static void Setup(string emulator, bool baseScan) {
            process = Process.GetProcessesByName(emulator)[0];
            startOffset = process.MainModule.BaseAddress;
            start = startOffset.ToInt64();
            endOffset = IntPtr.Add(startOffset, process.MainModule.ModuleMemorySize);
            end = endOffset.ToInt64();
            processHandle = OpenProcess(PROCESS_ALL_ACCESS, false, process.Id);

            if (baseScan) {
                var results = ScanAoB(start, end, "50 53 2D 58 20 45 58 45", false);
                foreach (long result in results) {
                    Constants.WriteDebug(result.ToString("X2"));
                }
            }
        }

        public static byte ReadByte(long address) {
            byte[] buffer = new byte[1];
            ReadProcessMemory(processHandle, address + Constants.OFFSET, buffer, 1, out long bytesRead);
            return buffer[0];
        }

        public static byte ReadByte(string address) {
            byte[] buffer = new byte[1];
            ReadProcessMemory(processHandle, Constants.GetAddress(address) + Constants.OFFSET, buffer, 1, out long bytesRead);
            return buffer[0];
        }

        public static short ReadShort(long address) {
            byte[] buffer = new byte[2];
            ReadProcessMemory(processHandle, address + Constants.OFFSET, buffer, 2, out long bytesRead);
            return BitConverter.ToInt16(buffer, 0);
        }

        public static short ReadShort(string address) {
            byte[] buffer = new byte[2];
            ReadProcessMemory(processHandle, Constants.GetAddress(address) + Constants.OFFSET, buffer, 2, out long bytesRead);
            return BitConverter.ToInt16(buffer, 0);
        }

        public static ushort ReadUShort(long address) {
            byte[] buffer = new byte[2];
            ReadProcessMemory(processHandle, address + Constants.OFFSET, buffer, 2, out long bytesRead);
            return BitConverter.ToUInt16(buffer, 0);
        }

        public static ushort ReadUShort(string address) {
            byte[] buffer = new byte[2];
            ReadProcessMemory(processHandle, Constants.GetAddress(address) + Constants.OFFSET, buffer, 2, out long bytesRead);
            return BitConverter.ToUInt16(buffer, 0);
        }

        public static Int32 ReadInt(long address) {
            byte[] buffer = new byte[4];
            ReadProcessMemory(processHandle, address + Constants.OFFSET, buffer, 4, out long bytesRead);
            return BitConverter.ToInt32(buffer, 0);
        }

        public static Int32 ReadInt(string address) {
            byte[] buffer = new byte[4];
            ReadProcessMemory(processHandle, Constants.GetAddress(address) + Constants.OFFSET, buffer, 4, out long bytesRead);
            return BitConverter.ToInt32(buffer, 0);
        }

        public static UInt32 ReadUInt(long address) {
            byte[] buffer = new byte[4];
            ReadProcessMemory(processHandle, address + Constants.OFFSET, buffer, 4, out long bytesRead);
            return BitConverter.ToUInt32(buffer, 0);
        }

        public static UInt32 ReadUInt(string address) {
            byte[] buffer = new byte[4];
            ReadProcessMemory(processHandle, Constants.GetAddress(address) + Constants.OFFSET, buffer, 4, out long bytesRead);
            return BitConverter.ToUInt32(buffer, 0);
        }

        public static long ReadLong(long address) {
            byte[] buffer = new byte[8];
            ReadProcessMemory(processHandle, address + Constants.OFFSET, buffer, 4, out long bytesRead);
            return BitConverter.ToInt64(buffer, 0);
        }

        public static long ReadLong(string address) {
            byte[] buffer = new byte[8];
            ReadProcessMemory(processHandle, Constants.GetAddress(address) + Constants.OFFSET, buffer, 4, out long bytesRead);
            return BitConverter.ToInt64(buffer, 0);
        }

        public static ulong ReadULong(long address) {
            byte[] buffer = new byte[8];
            ReadProcessMemory(processHandle, address + Constants.OFFSET, buffer, 4, out long bytesRead);
            return BitConverter.ToUInt64(buffer, 0);
        }

        public static ulong ReadULong(string address) {
            byte[] buffer = new byte[8];
            ReadProcessMemory(processHandle, Constants.GetAddress(address) + Constants.OFFSET, buffer, 4, out long bytesRead);
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

        public static void WriteByte(string address, byte value) {
            var val = BitConverter.GetBytes(value);
            WriteProcessMemory(processHandle, new IntPtr(Constants.GetAddress(address) + Constants.OFFSET), val, 1, out int error);
        }

        public static void WriteShort(long address, short value) {
            var val = BitConverter.GetBytes(value);
            WriteProcessMemory(processHandle, new IntPtr(address + Constants.OFFSET), val, 2, out int error);
        }

        public static void WriteShort(string address, short value) {
            var val = BitConverter.GetBytes(value);
            WriteProcessMemory(processHandle, new IntPtr(Constants.GetAddress(address) + Constants.OFFSET), val, 2, out int error);
        }

        public static void WriteUShort(long address, ushort value) {
            var val = BitConverter.GetBytes(value);
            WriteProcessMemory(processHandle, new IntPtr(address + Constants.OFFSET), val, 2, out int error);
        }

        public static void WriteUShort(string address, ushort value) {
            var val = BitConverter.GetBytes(value);
            WriteProcessMemory(processHandle, new IntPtr(Constants.GetAddress(address) + Constants.OFFSET), val, 2, out int error);
        }

        public static void WriteInt(long address, Int32 value) {
            var val = BitConverter.GetBytes(value);
            WriteProcessMemory(processHandle, new IntPtr(address + Constants.OFFSET), val, 4, out int error);
        }

        public static void WriteInt(string address, Int32 value) {
            var val = BitConverter.GetBytes(value);
            WriteProcessMemory(processHandle, new IntPtr(Constants.GetAddress(address) + Constants.OFFSET), val, 4, out int error);
        }

        public static void WriteUInt(long address, UInt32 value) {
            var val = BitConverter.GetBytes(value);
            WriteProcessMemory(processHandle, new IntPtr(address + Constants.OFFSET), val, 4, out int error);
        }

        public static void WriteUInt(string address, UInt32 value) {
            var val = BitConverter.GetBytes(value);
            WriteProcessMemory(processHandle, new IntPtr(Constants.GetAddress(address) + Constants.OFFSET), val, 4, out int error);
        }

        public static void WriteLong(long address, long value) {
            var val = BitConverter.GetBytes(value);
            WriteProcessMemory(processHandle, new IntPtr(address + Constants.OFFSET), val, 8, out int error);
        }

        public static void WriteLong(string address, long value) {
            var val = BitConverter.GetBytes(value);
            WriteProcessMemory(processHandle, new IntPtr(Constants.GetAddress(address) + Constants.OFFSET), val, 8, out int error);
        }

        public static void WriteULong(long address, ulong value) {
            var val = BitConverter.GetBytes(value);
            WriteProcessMemory(processHandle, new IntPtr(address + Constants.OFFSET), val, 8, out int error);
        }

        public static void WriteULong(string address, ulong value) {
            var val = BitConverter.GetBytes(value);
            WriteProcessMemory(processHandle, new IntPtr(Constants.GetAddress(address) + Constants.OFFSET), val, 8, out int error);
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

        public static List<long> ScanAoB(long startAddr, long endAddr, string values, bool useOffset) {
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

            int iter = 1000000;
            var tasks = new List<Task>();

            for (int chunk = 0; chunk < 1 + (endAddr - startAddr) / iter; chunk++) { //Splits addresses into 1mil + mask size (to check overlap) chunks and runs them in parallel.
                long start = Math.Max(startAddr, startAddr + chunk * iter - maskStr.Length);
                long end = Math.Min(endAddr, startAddr + iter + chunk * iter);

                Task t = new Task(() => {
                    byte[] data = ReadAoB(start, end);
                    foreach (var position in data.Locate(pattern, maskArr)) {
                        results.Add(position + start);
                    }
                });
                tasks.Add(t);
            }

            foreach (Task t in tasks) {
                t.Start();
            }
            Task.WaitAll(tasks.ToArray());
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
