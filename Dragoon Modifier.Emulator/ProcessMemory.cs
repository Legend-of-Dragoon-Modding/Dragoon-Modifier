using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Emulator {
    internal static class ProcessMemory {
        private const int PROCESS_ALL_ACCESS = 0x1FFFFF;

        [DllImport("kernel32.dll")]
        internal static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        internal static extern bool ReadProcessMemory(IntPtr hProcess, long lpBaseAddress, byte[] lpBuffer, long dwSize, out long lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        internal static extern bool WriteProcessMemory(IntPtr hProcess, long lpBaseAddress, byte[] lpBuffer, int nSize, out int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        internal static extern Int32 CloseHandle(IntPtr hProcess);

        internal static IntPtr GetProcessHandle(Process proc) {
            return OpenProcess(PROCESS_ALL_ACCESS, false, proc.Id);
        }
    }
}
