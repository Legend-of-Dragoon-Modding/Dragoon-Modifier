using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Dragoon_Modifier {
    public class Emu {
        const int PROCESS_ALL_ACCESS = 0x1FFFFF;
        const uint versionAddr = 0x9E19;
        const uint versionStringLen = 11;
        const string AoBCheck = "50 53 2D 58 20 45 58 45";
        const string duckstationCheck = "53 6F 6E 79 20 43 6F 6D 70 75 74 65 72 20 45 6E 74 65 72 74 61 69 6E 6D 65 6E 74 20 49 6E 63";

        static readonly int[] duckstationOffsets = new int[] { 0x110, 0x118 };
        static readonly Dictionary<string, Region> versions = new Dictionary<string, Region> {
            { "SCUS_944.91", Region.NTA },
            { "SCUS_945.84", Region.NTA },
            { "SCUS_945.85", Region.NTA },
            { "SCUS_945.86", Region.NTA },

            { "SCPS_101.19", Region.JPN },
            { "SCPS_101.20", Region.JPN },
            { "SCPS_101.21", Region.JPN },
            { "SCPS_101.22", Region.JPN },

            { "SCES_030.43", Region.ENG },
            { "SCES_130.43", Region.ENG },
            { "SCES_230.43", Region.ENG },
            { "SCES_330.43", Region.ENG },

            { "SCES_030.44", Region.FRN },
            { "SCES_130.44", Region.FRN },
            { "SCES_230.44", Region.FRN },
            { "SCES_330.44", Region.FRN },

            { "SCES_030.45", Region.GER },
            { "SCES_130.45", Region.GER },
            { "SCES_230.45", Region.GER },
            { "SCES_330.45", Region.GER },

            { "SCES_030.46", Region.ITL },
            { "SCES_130.46", Region.ITL },
            { "SCES_230.46", Region.ITL },
            { "SCES_330.46", Region.ITL },

            { "SCES_030.47", Region.SPN },
            { "SCES_130.47", Region.SPN },
            { "SCES_230.47", Region.SPN },
            { "SCES_330.47", Region.SPN },

        };
        static readonly Dictionary<string, byte[]> KMPMask = new Dictionary<string, byte[]>() {
            {"??", new byte[] { 0x00, 0x00} },
            {"0?", new byte[] { 0x00, 0xF0} },
            {"1?", new byte[] { 0x10, 0xF0} },
            {"2?", new byte[] { 0x20, 0xF0} },
            {"3?", new byte[] { 0x30, 0xF0} },
            {"4?", new byte[] { 0x40, 0xF0} },
            {"5?", new byte[] { 0x50, 0xF0} },
            {"6?", new byte[] { 0x60, 0xF0} },
            {"7?", new byte[] { 0x70, 0xF0} },
            {"8?", new byte[] { 0x80, 0xF0} },
            {"9?", new byte[] { 0x90, 0xF0} },
            {"A?", new byte[] { 0xA0, 0xF0} },
            {"B?", new byte[] { 0xB0, 0xF0} },
            {"C?", new byte[] { 0xC0, 0xF0} },
            {"D?", new byte[] { 0xD0, 0xF0} },
            {"E?", new byte[] { 0xE0, 0xF0} },
            {"F?", new byte[] { 0xF0, 0xF0} },
            {"?0", new byte[] { 0x00, 0x0F} },
            {"?1", new byte[] { 0x01, 0x0F} },
            {"?2", new byte[] { 0x02, 0x0F} },
            {"?3", new byte[] { 0x03, 0x0F} },
            {"?4", new byte[] { 0x04, 0x0F} },
            {"?5", new byte[] { 0x05, 0x0F} },
            {"?6", new byte[] { 0x06, 0x0F} },
            {"?7", new byte[] { 0x07, 0x0F} },
            {"?8", new byte[] { 0x08, 0x0F} },
            {"?9", new byte[] { 0x09, 0x0F} },
            {"?A", new byte[] { 0x0A, 0x0F} },
            {"?B", new byte[] { 0x0B, 0x0F} },
            {"?C", new byte[] { 0x0C, 0x0F} },
            {"?D", new byte[] { 0x0D, 0x0F} },
            {"?E", new byte[] { 0x0E, 0x0F} },
            {"?F", new byte[] { 0x0F, 0x0F} },
        };

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        static extern bool ReadProcessMemory(IntPtr hProcess, long lpBaseAddress, byte[] lpBuffer, long dwSize);

        [DllImport("kernel32.dll")]
        static extern bool WriteProcessMemory(IntPtr hProcess, long lpBaseAddress, byte[] lpBuffer, int nSize);

        [DllImport("kernel32.dll")]
        static extern Int32 CloseHandle(IntPtr hProcess);

        IntPtr _handle;
        long _emulatorOffset = 0;

        public MemoryController.MemoryController MemoryController { get; private set; }
        public Battle.Battle BattleController { get; private set; }

        public Emu(string emulatorName) {
            _emulatorOffset = Constants.OFFSET; // TODO load previous offset

            if (emulatorName.ToLower().Contains(".exe")) {
                emulatorName = emulatorName.Replace("exe", "");
            }

            Process[] processList = Process.GetProcesses();

            foreach (Process proc in processList) {
                if (proc.ProcessName.Equals(emulatorName, StringComparison.CurrentCultureIgnoreCase) || proc.ProcessName.Contains(emulatorName.ToLower())) { // Find (name).exe in the process list (use task manager to find the name)
                    _handle = OpenProcess(PROCESS_ALL_ACCESS, false, proc.Id);
                    if (Emulators(proc, emulatorName)) {
                        break;
                    }
                    throw new EmulatorAttachException();
                }
            }

            MemoryController = new MemoryController.MemoryController();

        }


        public byte ReadByte(long address) {
            byte[] buffer = new byte[1];
            ReadProcessMemory(_handle, address + _emulatorOffset, buffer, 1);
            return buffer[0];
        }

        public byte ReadByte(string address, int offset = 0) {
            byte[] buffer = new byte[1];
            ReadProcessMemory(_handle, Constants.GetAddress(address) + _emulatorOffset + offset, buffer, 1);
            return buffer[0];
        }

        public void WriteByte(long address, byte value) {
            var val = BitConverter.GetBytes(value);
            WriteProcessMemory(_handle, address + _emulatorOffset, val, 1);
        }

        public void WriteByte(string address, byte value, int offset = 0) {
            var val = BitConverter.GetBytes(value);
            WriteProcessMemory(_handle, Constants.GetAddress(address) + _emulatorOffset + offset, val, 1);
        }

        public sbyte ReadSByte(long address) {
            byte[] buffer = new byte[1];
            ReadProcessMemory(_handle, address + _emulatorOffset, buffer, 1);
            return (sbyte) buffer[0];
        }

        public sbyte ReadSByte(string address, int offset = 0) {
            byte[] buffer = new byte[1];
            ReadProcessMemory(_handle, Constants.GetAddress(address) + _emulatorOffset + offset, buffer, 1);
            return (sbyte) buffer[0];
        }

        public void WriteSByte(long address, sbyte value) {
            var val = BitConverter.GetBytes(value);
            WriteProcessMemory(_handle, address + _emulatorOffset, val, 1);
        }

        public void WriteSByte(string address, sbyte value, int offset = 0) {
            var val = BitConverter.GetBytes(value);
            WriteProcessMemory(_handle, Constants.GetAddress(address) + _emulatorOffset + offset, val, 1);
        }

        public short ReadShort(long address) {
            byte[] buffer = new byte[2];
            ReadProcessMemory(_handle, address + _emulatorOffset, buffer, 2);
            return BitConverter.ToInt16(buffer, 0);
        }
        public short ReadShort(string address, int offset = 0) {
            byte[] buffer = new byte[2];
            ReadProcessMemory(_handle, Constants.GetAddress(address) + _emulatorOffset + offset, buffer, 2);
            return BitConverter.ToInt16(buffer, 0);
        }

        public ushort ReadUShort(long address) {
            byte[] buffer = new byte[2];
            ReadProcessMemory(_handle, address + _emulatorOffset, buffer, 2);
            return BitConverter.ToUInt16(buffer, 0);
        }

        public ushort ReadUShort(string address, int offset = 0) {
            byte[] buffer = new byte[2];
            ReadProcessMemory(_handle, Constants.GetAddress(address) + _emulatorOffset + offset, buffer, 2);
            return BitConverter.ToUInt16(buffer, 0);
        }

        public void WriteShort(long address, short value) {
            var val = BitConverter.GetBytes(value);
            WriteProcessMemory(_handle, address + _emulatorOffset, val, 2);
        }
        public void WriteShort(string address, short value, int offset = 0) {
            var val = BitConverter.GetBytes(value);
            WriteProcessMemory(_handle, Constants.GetAddress(address) + _emulatorOffset + offset, val, 2);
        }

        public void WriteUShort(long address, ushort value) {
            var val = BitConverter.GetBytes(value);
            WriteProcessMemory(_handle, address + _emulatorOffset, val, 2);
        }

        public void WriteUShort(string address, ushort value, int offset = 0) {
            var val = BitConverter.GetBytes(value);
            WriteProcessMemory(_handle, Constants.GetAddress(address) + _emulatorOffset + offset, val, 2);
        }

        public UInt32 ReadUInt24(long address) {
            byte[] buffer = new byte[3];
            ReadProcessMemory(_handle, address + _emulatorOffset, buffer, 3);
            return BitConverter.ToUInt32(buffer, 0);
        }

        public UInt32 ReadUInt24(string address, int offset = 0) {
            byte[] buffer = new byte[3];
            ReadProcessMemory(_handle, Constants.GetAddress(address) + _emulatorOffset + offset, buffer, 3);
            return BitConverter.ToUInt32(buffer, 0);
        }

        public void WriteUInt24(long address, UInt32 value) {
            var val = BitConverter.GetBytes(value);
            val = val.Take(val.Count() - 1).ToArray();
            WriteProcessMemory(_handle, address + _emulatorOffset, val, 3);
        }

        public void WriteUInt24(string address, UInt32 value, int offset = 0) {
            var val = BitConverter.GetBytes(value);
            val = val.Take(val.Count() - 1).ToArray();
            WriteProcessMemory(_handle, Constants.GetAddress(address) + _emulatorOffset + offset, val, 3);
        }

        public Int32 ReadInt(long address) {
            byte[] buffer = new byte[4];
            ReadProcessMemory(_handle, address + _emulatorOffset, buffer, 4);
            return BitConverter.ToInt32(buffer, 0);
        }

        public Int32 ReadInt(string address, int offset = 0) {
            byte[] buffer = new byte[4];
            ReadProcessMemory(_handle, Constants.GetAddress(address) + _emulatorOffset + offset, buffer, 4);
            return BitConverter.ToInt32(buffer, 0);
        }

        public void WriteInt(long address, Int32 value) {
            var val = BitConverter.GetBytes(value);
            WriteProcessMemory(_handle, address + _emulatorOffset, val, 4);
        }

        public void WriteInt(string address, Int32 value, int offset = 0) {
            var val = BitConverter.GetBytes(value);
            WriteProcessMemory(_handle, Constants.GetAddress(address) + _emulatorOffset + offset, val, 4);
        }

        public UInt32 ReadUInt(string address, int offset = 0) {
            byte[] buffer = new byte[4];
            ReadProcessMemory(_handle, Constants.GetAddress(address) + _emulatorOffset + offset, buffer, 4);
            return BitConverter.ToUInt32(buffer, 0);
        }

        public UInt32 ReadUInt(long address) {
            byte[] buffer = new byte[4];
            ReadProcessMemory(_handle, address + _emulatorOffset, buffer, 4);
            return BitConverter.ToUInt32(buffer, 0);
        }

        public void WriteUInt(long address, UInt32 value) {
            var val = BitConverter.GetBytes(value);
            WriteProcessMemory(_handle, address + _emulatorOffset, val, 4);
        }

        public void WriteUInt(string address, UInt32 value, int offset = 0) {
            var val = BitConverter.GetBytes(value);
            WriteProcessMemory(_handle, Constants.GetAddress(address) + _emulatorOffset + offset, val, 4);
        }

        public long ReadLong(long address) {
            byte[] buffer = new byte[8];
            ReadProcessMemory(_handle, address + _emulatorOffset, buffer, 8);
            return BitConverter.ToInt64(buffer, 0);
        }

        public long ReadLong(string address, int offset = 0) {
            byte[] buffer = new byte[8];
            ReadProcessMemory(_handle, Constants.GetAddress(address) + _emulatorOffset + offset, buffer, 8);
            return BitConverter.ToInt64(buffer, 0);
        }

        public void WriteLong(long address, long value) {
            var val = BitConverter.GetBytes(value);
            WriteProcessMemory(_handle, address + _emulatorOffset, val, 8);
        }

        public void WriteLong(string address, long value, int offset = 0) {
            var val = BitConverter.GetBytes(value);
            WriteProcessMemory(_handle, Constants.GetAddress(address) + _emulatorOffset + offset, val, 8);
        }

        public ulong ReadULong(long address) {
            byte[] buffer = new byte[8];
            ReadProcessMemory(_handle, address + _emulatorOffset, buffer, 8);
            return BitConverter.ToUInt64(buffer, 0);
        }

        public ulong ReadULong(string address, int offset = 0) {
            byte[] buffer = new byte[8];
            ReadProcessMemory(_handle, Constants.GetAddress(address) + _emulatorOffset + offset, buffer, 8);
            return BitConverter.ToUInt64(buffer, 0);
        }

        public void WriteULong(long address, ulong value) {
            var val = BitConverter.GetBytes(value);
            WriteProcessMemory(_handle, address + _emulatorOffset, val, 8);
        }

        public void WriteULong(string address, ulong value, int offset = 0) {
            var val = BitConverter.GetBytes(value);
            WriteProcessMemory(_handle, Constants.GetAddress(address) + _emulatorOffset + offset, val, 8);
        }

        public byte[] ReadAoB(long startAddr, long endAddr) {
            long len = endAddr - startAddr;
            byte[] buffer = new byte[len];
            ReadProcessMemory(_handle, startAddr + _emulatorOffset, buffer, len);
            return buffer;
        }

        public byte[] ReadAoB(string address, long length) {
            byte[] buffer = new byte[length];
            ReadProcessMemory(_handle, Constants.GetAddress(address) + _emulatorOffset, buffer, length);
            return buffer;
        }

        public void WriteAoB(long address, string values) {


            string[] strArr = values.Split(' ');
            byte[] arr = new byte[strArr.Length];
            for (int i = 0; i < strArr.Length; i++) {
                arr[i] = Convert.ToByte(strArr[i], 16);
            }
            WriteProcessMemory(_handle, address + _emulatorOffset, arr, arr.Length);
        }

        public void WriteAoB(string address, string values) {
            string[] strArr = values.Split(' ');
            byte[] arr = new byte[strArr.Length];
            for (int i = 0; i < strArr.Length; i++) {
                arr[i] = Convert.ToByte(strArr[i], 16);
            }
            WriteProcessMemory(_handle, Constants.GetAddress(address) + _emulatorOffset, arr, arr.Length);
        }

        public List<long> ScanAoB(long start, long end, string pattern, bool useOffset = true, bool addOffset = false) {
            long offset = 0;
            if (!useOffset) {
                offset -= _emulatorOffset;
            }

            List<long> results = KMPSearch(pattern, ReadAoB(start + offset, end + offset), true);

            for (int i = 0; i < results.Count; i++) {
                results[i] += start;
                if (addOffset) {
                    results[i] += _emulatorOffset;
                }
            }

            return results;
        }

        // Write text

        // Read name

        // GetCharacterByNumber

        // GetCharacterbyChar

        bool Emulators(Process proc, string emulatorName) {
            if (Verify(_emulatorOffset)) {
                Constants.WriteOutput("Previous offset successful.");
                return true;
            } else {
                _emulatorOffset = 0;
                if (emulatorName.ToLower() == "retroarch") {
                    return RetroArch(proc);
                } else if (emulatorName.ToLower().Contains("duckstation")) {
                    return DuckStation(proc);
                } else if (emulatorName.Contains("ePSXe")) {
                    return ePSXe(proc);
                }
                return false;
            }
        }

        bool Verify(long offset) {
            var start = versionAddr + offset;
            var end = start + versionStringLen;
            string version = Encoding.Default.GetString(Emulator.ReadAoB(start, end));
            if (versions.TryGetValue(version, out var key)) {
                Constants.REGION = key; // TODO actually load addresses
                Constants.WriteOutput($"Detected region: {key}");
                return true;
            }
            return false;
        }

        bool ePSXe(Process proc) {
            var start = (long) proc.MainModule.BaseAddress;
            var end = start + proc.MainModule.ModuleMemorySize;
            Constants.WriteOutput("Starting Scan: " + Convert.ToString(start, 16).ToUpper() + " - " + Convert.ToString(end, 16).ToUpper());
            var results = Emulator.KMPSearch(AoBCheck, Emulator.ReadAoB(start, end), true);
            foreach (var result in results) {
                var tempOffset = start + result - 0xB070;
                if (Verify(tempOffset)) {
                    _emulatorOffset = tempOffset;
                    Constants.KEY.SetValue("Offset", _emulatorOffset);
                    Constants.WriteOutput("Base scan successful.");
                    return true;
                }
            }
            return false;
        }

        bool RetroArch(Process proc) {
            try {
                var start = (long) proc.MainModule.BaseAddress;
                var end = start + 0x1000008;
                for (int i = 0; i < 17; i++) {
                    Constants.WriteOutput("Start RetroArch Scan (" + i + "/16): " + Convert.ToString(start, 16).ToUpper() + " - " + Convert.ToString(end, 16).ToUpper());
                    var results = Emulator.KMPSearch(AoBCheck, Emulator.ReadAoB(start, end), true);
                    foreach (var result in results) {
                        var tempOffset = start + result - 0xB070;
                        if (Verify(tempOffset)) {
                            _emulatorOffset = tempOffset;
                            Constants.KEY.SetValue("Offset", _emulatorOffset);
                            Constants.WriteOutput("Base scan successful.");
                            return true;
                        }
                    }
                    start += 0x10000000;
                    end += 0x10000000;
                }
                return false;

            } catch (Exception e) {
                Constants.WriteOutput("RetroArch scan failed.");
                return false;
            }
        }

        bool DuckStation(Process proc) {
            var start = (long) proc.MainModule.BaseAddress;
            var end = start + proc.MainModule.ModuleMemorySize;
            var results = Emulator.KMPSearch(duckstationCheck, Emulator.ReadAoB(start, end), true);
            foreach (var result in results) {
                foreach (var offset in duckstationOffsets) {
                    var pointer = Emulator.ReadLong(result + start - offset);
                    if (Verify(pointer)) {
                        _emulatorOffset = pointer;
                        Constants.KEY.SetValue("Offset", _emulatorOffset);
                        Constants.WriteOutput("Base scan successful.");
                        return true;
                    }
                }
            }
            return false;
        }

        static List<long> KMPSearch(string pattern, byte[] array, bool findAll = false) {
            var splitString = pattern.Split(' ');

            var patternValue = new byte[splitString.Length];
            var patternMask = new byte[splitString.Length];
            for (int i = 0; i < splitString.Length; i++) {
                if (Byte.TryParse(splitString[i], NumberStyles.HexNumber, null, out byte key)) {
                    patternValue[i] = key; // Can be parsed, unless theres a ? mask
                    patternMask[i] = 0xFF; // & with 0xFF doesn't change the value
                } else {
                    patternValue[i] = KMPMask[splitString[i]][0]; // For masked nibbles, value and mask is set to 0, so it always passes
                    patternMask[i] = KMPMask[splitString[i]][1];
                }
            }

            var indexList = new List<long>();

            var substringIndex = CalculateSubstringIndexes(patternValue, patternMask, patternMask.Length);

            int arrayIndex = 0;
            int patternIndex = 0;
            while (arrayIndex < array.Length - patternValue.Length + 1) {
                if ((array[arrayIndex] & patternMask[patternIndex]) == patternValue[patternIndex]) {
                    arrayIndex++;
                    patternIndex++;
                } else {
                    if (patternIndex != 0) {
                        patternIndex = substringIndex[patternIndex - 1];
                    } else {
                        arrayIndex++;
                    }
                }
                if (patternIndex == patternValue.Length) {
                    indexList.Add(arrayIndex - patternIndex);
                    if (!findAll) {
                        break;
                    }
                    patternIndex = substringIndex[patternIndex - 1];
                }
            }

            return indexList;
        }

        static byte[] CalculateSubstringIndexes(byte[] patternValue, byte[] patternMask, int patternLength) {
            var substringIndex = new byte[patternLength];
            substringIndex[0] = 0;
            int len = 0;
            int i = 1;
            while (i < patternLength) {
                if (patternValue[i] == (patternValue[len] & patternMask[i])) {
                    substringIndex[i] = (byte) (len + 1);
                    len++;
                    i++;
                } else {
                    if (len != 0) {
                        len = substringIndex[len - 1];
                    } else {
                        substringIndex[i] = 0;
                        i++;
                    }
                }
            }
            return substringIndex;
        }
    }

    class EmulatorAttachException : Exception {

    }
}
