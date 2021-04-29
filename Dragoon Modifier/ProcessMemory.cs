using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Dragoon_Modifier {
    public static class ProcessMemory {
        const int PROCESS_ALL_ACCESS = 0x1FFFFF;

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(IntPtr hProcess, long lpBaseAddress, byte[] lpBuffer, long dwSize, out long lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        public static extern bool WriteProcessMemory(IntPtr hProcess, long lpBaseAddress, byte[] lpBuffer, int nSize, out int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        public static extern Int32 CloseHandle(IntPtr hProcess);

        public static IntPtr GetProcessHandle(Process proc) {
            return OpenProcess(PROCESS_ALL_ACCESS, false, proc.Id);
        }
    }
}
