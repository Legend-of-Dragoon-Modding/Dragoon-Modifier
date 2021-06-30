using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Emulator {
    internal class Emulator : IEmulator {
        private const uint _versionAddr = 0x9E19;
        private const uint _versionStringLen = 11;
        private static readonly byte[] _AoBCheck = new byte[] { 0x50, 0x53, 0x2D, 0x58, 0x20, 0x45, 0x58, 0x45 };
        private static readonly byte[] _duckstationCheck = new byte[] { 0x53, 0x6F, 0x6E, 0x79, 0x20, 0x43, 0x6F, 0x6D, 0x70, 0x75, 0x74, 0x65, 0x72, 0x20, 0x45, 0x6E, 0x74, 0x65, 0x72, 0x74, 0x61, 0x69, 0x6E, 0x6D, 0x65, 0x6E, 0x74, 0x20, 0x49, 0x6E, 0x63 };
        private static readonly int[] _duckstationOffsets = new int[] { 0x110, 0x118 };
        private static readonly Dictionary<string, Region> _versions = new Dictionary<string, Region> {
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

        private static readonly Dictionary<string, int[]> _addressList = new Dictionary<string, int[]> {
            { "ITEM_TABLE", new int[] { 0x111FF1, 0x110785, 0x0, 0x0, 0x0, 0x11229D, 0x11213D } },
            { "PARTY_SLOT", new int[] { 0xBAC50, 0xB9950, 0xBAF38, 0x0, 0x0, 0xBAF78, 0xBAE88 } }
        };

        private readonly IntPtr _processHandle;

        private ILoDEncoding LoDEncoding;

        public long EmulatorOffset { get; private set; }
        public Region Region { get; private set; }
        public Memory.IMemory Memory { get; private set; }
        public Memory.Battle.IBattle Battle { get { return GetBattle(); } private set { Battle = value; } }

        internal Emulator(string emulatorName, long previousOffset) {
            EmulatorOffset = previousOffset;

            if (emulatorName.ToLower().Contains(".exe")) {
                emulatorName = emulatorName.Replace("exe", "");
            }

            Process proc = FindEmulatorProcess(emulatorName);

            _processHandle = ProcessMemory.GetProcessHandle(proc);


            if (!Emulators(proc, emulatorName)) {
                throw new EmulatorAttachException(emulatorName);
            }

            LoDEncoding = Factory.Encoding(Region);

            Memory = Factory.MemoryController(this);

            Debug.WriteLine($"[DEBUG] Succesfully attached to emulator {emulatorName}");
        }

        internal Emulator(string emulatorName, long previousOffset, Dictionary<string, int[]> addresses) {
            EmulatorOffset = previousOffset;

            if (emulatorName.ToLower().Contains(".exe")) {
                emulatorName = emulatorName.Replace("exe", "");
            }

            Process proc = FindEmulatorProcess(emulatorName);

            _processHandle = ProcessMemory.GetProcessHandle(proc);


            if (!Emulators(proc, emulatorName)) {
                throw new EmulatorAttachException(emulatorName);
            }

            LoadRegionalAddresses(addresses);

            Memory = Factory.MemoryController(this);

            Debug.WriteLine($"[DEBUG] Succesfully attached to emulator {emulatorName}");
        }


        #region Byte

        public byte ReadByte(long address) {
            byte[] buffer = new byte[1];
            ProcessMemory.ReadProcessMemory(_processHandle, address + EmulatorOffset, buffer, 1, out long lpNumberOfBytesRead);
            return buffer[0];
        }

        public byte ReadByte(string address, int offset = 0) {
            if (TryGetAddress(address, out var key)) {
                byte[] buffer = new byte[1];
                ProcessMemory.ReadProcessMemory(_processHandle, key + EmulatorOffset + offset, buffer, 1, out long lpNumberOfBytesRead);
                return buffer[0];
            }
            Console.WriteLine($"[ERROR] Incorrect addres key {address}.");
            return 0;
        }

        public void WriteByte(long address, byte value) {
            var val = BitConverter.GetBytes(value);
            ProcessMemory.WriteProcessMemory(_processHandle, address + EmulatorOffset, val, 1, out int lpNumberOfBytesWritten);
        }

        public void WriteByte(string address, byte value, int offset = 0) {
            if (TryGetAddress(address, out var key)) {
                var val = BitConverter.GetBytes(value);
                ProcessMemory.WriteProcessMemory(_processHandle, key + EmulatorOffset + offset, val, 1, out int lpNumberOfBytesWritten);
                return;
            }
            Console.WriteLine($"[ERROR] Incorrect addres key {address}.");
        }

        #endregion

        #region SByte
        public sbyte ReadSByte(long address) {
            byte[] buffer = new byte[1];
            ProcessMemory.ReadProcessMemory(_processHandle, address + EmulatorOffset, buffer, 1, out long lpNumberOfBytesRead);
            return (sbyte) buffer[0];
        }

        public sbyte ReadSByte(string address, int offset = 0) {
            if (TryGetAddress(address, out var key)) {
                byte[] buffer = new byte[1];
                ProcessMemory.ReadProcessMemory(_processHandle, key + EmulatorOffset + offset, buffer, 1, out long lpNumberOfBytesRead);
                return (sbyte) buffer[0];
            }
            Console.WriteLine($"[ERROR] Incorrect addres key {address}.");
            return 0;
        }

        public void WriteSByte(long address, sbyte value) {
            var val = BitConverter.GetBytes(value);
            ProcessMemory.WriteProcessMemory(_processHandle, address + EmulatorOffset, val, 1, out int lpNumberOfBytesWritten);
        }

        public void WriteSByte(string address, sbyte value, int offset = 0) {
            if (TryGetAddress(address, out var key)) {
                var val = BitConverter.GetBytes(value);
                ProcessMemory.WriteProcessMemory(_processHandle, key + EmulatorOffset + offset, val, 1, out int lpNumberOfBytesWritten);
                return;
            }
            Console.WriteLine($"[ERROR] Incorrect addres key {address}.");
        }

        #endregion

        #region Short

        public short ReadShort(long address) {
            byte[] buffer = new byte[2];
            ProcessMemory.ReadProcessMemory(_processHandle, address + EmulatorOffset, buffer, 2, out long lpNumberOfBytesRead);
            return BitConverter.ToInt16(buffer, 0);
        }
        public short ReadShort(string address, int offset = 0) {
            if (TryGetAddress(address, out var key)) {
                byte[] buffer = new byte[2];
                ProcessMemory.ReadProcessMemory(_processHandle, key + EmulatorOffset + offset, buffer, 2, out long lpNumberOfBytesRead);
                return BitConverter.ToInt16(buffer, 0);
            }
            Console.WriteLine($"[ERROR] Incorrect addres key {address}.");
            return 0;
        }

        public void WriteShort(long address, short value) {
            var val = BitConverter.GetBytes(value);
            ProcessMemory.WriteProcessMemory(_processHandle, address + EmulatorOffset, val, 2, out int lpNumberOfBytesWritten);
        }
        public void WriteShort(string address, short value, int offset = 0) {
            if (TryGetAddress(address, out var key)) {
                var val = BitConverter.GetBytes(value);
                ProcessMemory.WriteProcessMemory(_processHandle, key + EmulatorOffset + offset, val, 2, out int lpNumberOfBytesWritten);
                return;
            }
            Console.WriteLine($"[ERROR] Incorrect addres key {address}.");
        }

        #endregion

        #region UShort

        public ushort ReadUShort(long address) {
            byte[] buffer = new byte[2];
            ProcessMemory.ReadProcessMemory(_processHandle, address + EmulatorOffset, buffer, 2, out long lpNumberOfBytesRead);
            return BitConverter.ToUInt16(buffer, 0);
        }

        public ushort ReadUShort(string address, int offset = 0) {
            if (TryGetAddress(address, out var key)) {
                byte[] buffer = new byte[2];
                ProcessMemory.ReadProcessMemory(_processHandle, key + EmulatorOffset + offset, buffer, 2, out long lpNumberOfBytesRead);
                return BitConverter.ToUInt16(buffer, 0);
            }
            Console.WriteLine($"[ERROR] Incorrect addres key {address}.");
            return 0;
        }

        public void WriteUShort(long address, ushort value) {
            var val = BitConverter.GetBytes(value);
            ProcessMemory.WriteProcessMemory(_processHandle, address + EmulatorOffset, val, 2, out int lpNumberOfBytesWritten);
        }

        public void WriteUShort(string address, ushort value, int offset = 0) {
            if (TryGetAddress(address, out var key)) {
                var val = BitConverter.GetBytes(value);
                ProcessMemory.WriteProcessMemory(_processHandle, key + EmulatorOffset + offset, val, 2, out int lpNumberOfBytesWritten);
                return;
            }
            Console.WriteLine($"[ERROR] Incorrect addres key {address}.");
        }

        #endregion

        #region UInt24

        public UInt32 ReadUInt24(long address) {
            byte[] buffer = new byte[4];
            ProcessMemory.ReadProcessMemory(_processHandle, address + EmulatorOffset, buffer, 3, out long lpNumberOfBytesRead);
            return BitConverter.ToUInt32(buffer, 0);
        }

        public UInt32 ReadUInt24(string address, int offset = 0) {
            if (TryGetAddress(address, out var key)) {
                byte[] buffer = new byte[4];
                ProcessMemory.ReadProcessMemory(_processHandle, key + EmulatorOffset + offset, buffer, 3, out long lpNumberOfBytesRead);
                return BitConverter.ToUInt32(buffer, 0);
            }
            Console.WriteLine($"[ERROR] Incorrect addres key {address}.");
            return 0;
        }

        public void WriteUInt24(long address, UInt32 value) {
            var val = BitConverter.GetBytes(value);
            val = val.Take(val.Count() - 1).ToArray();
            ProcessMemory.WriteProcessMemory(_processHandle, address + EmulatorOffset, val, 3, out int lpNumberOfBytesWritten);
        }

        public void WriteUInt24(string address, UInt32 value, int offset = 0) {
            if (TryGetAddress(address, out var key)) {
                var val = BitConverter.GetBytes(value);
                val = val.Take(val.Count() - 1).ToArray();
                ProcessMemory.WriteProcessMemory(_processHandle, key + EmulatorOffset + offset, val, 3, out int lpNumberOfBytesWritten);
                return;
            }
            Console.WriteLine($"[ERROR] Incorrect addres key {address}.");
        }

        #endregion

        #region Int

        public Int32 ReadInt(long address) {
            byte[] buffer = new byte[4];
            ProcessMemory.ReadProcessMemory(_processHandle, address + EmulatorOffset, buffer, 4, out long lpNumberOfBytesRead);
            return BitConverter.ToInt32(buffer, 0);
        }

        public Int32 ReadInt(string address, int offset = 0) {
            if (TryGetAddress(address, out var key)) {
                byte[] buffer = new byte[4];
                ProcessMemory.ReadProcessMemory(_processHandle, key + EmulatorOffset + offset, buffer, 4, out long lpNumberOfBytesRead);
                return BitConverter.ToInt32(buffer, 0);
            }
            Console.WriteLine($"[ERROR] Incorrect addres key {address}.");
            return 0;
        }

        public void WriteInt(long address, Int32 value) {
            var val = BitConverter.GetBytes(value);
            ProcessMemory.WriteProcessMemory(_processHandle, address + EmulatorOffset, val, 4, out int lpNumberOfBytesWritten);
        }

        public void WriteInt(string address, Int32 value, int offset = 0) {
            if (TryGetAddress(address, out var key)) {
                var val = BitConverter.GetBytes(value);
                ProcessMemory.WriteProcessMemory(_processHandle, key + EmulatorOffset + offset, val, 4, out int lpNumberOfBytesWritten);
                return;
            }
            Console.WriteLine($"[ERROR] Incorrect addres key {address}.");
        }

        #endregion

        #region UInt

        public UInt32 ReadUInt(long address) {
            byte[] buffer = new byte[4];
            ProcessMemory.ReadProcessMemory(_processHandle, address + EmulatorOffset, buffer, 4, out long lpNumberOfBytesRead);
            return BitConverter.ToUInt32(buffer, 0);
        }

        public UInt32 ReadUInt(string address, int offset = 0) {
            if (TryGetAddress(address, out var key)) {
                byte[] buffer = new byte[4];
                ProcessMemory.ReadProcessMemory(_processHandle, key + EmulatorOffset + offset, buffer, 4, out long lpNumberOfBytesRead);
                return BitConverter.ToUInt32(buffer, 0);
            }
            Console.WriteLine($"[ERROR] Incorrect addres key {address}.");
            return 0;
        }

        public void WriteUInt(long address, UInt32 value) {
            var val = BitConverter.GetBytes(value);
            ProcessMemory.WriteProcessMemory(_processHandle, address + EmulatorOffset, val, 4, out int lpNumberOfBytesWritten);
        }

        public void WriteUInt(string address, UInt32 value, int offset = 0) {
            if (TryGetAddress(address, out var key)) {
                var val = BitConverter.GetBytes(value);
                ProcessMemory.WriteProcessMemory(_processHandle, key + EmulatorOffset + offset, val, 4, out int lpNumberOfBytesWritten);
                return;
            }
            Console.WriteLine($"[ERROR] Incorrect addres key {address}.");
        }

        #endregion

        #region Long

        public long ReadLong(long address) {
            byte[] buffer = new byte[8];
            ProcessMemory.ReadProcessMemory(_processHandle, address + EmulatorOffset, buffer, 8, out long lpNumberOfBytesRead);
            return BitConverter.ToInt64(buffer, 0);
        }

        public long ReadLong(string address, int offset = 0) {
            if (TryGetAddress(address, out var key)) {
                byte[] buffer = new byte[8];
                ProcessMemory.ReadProcessMemory(_processHandle, key + EmulatorOffset + offset, buffer, 8, out long lpNumberOfBytesRead);
                return BitConverter.ToInt64(buffer, 0);
            }
            Console.WriteLine($"[ERROR] Incorrect addres key {address}.");
            return 0;
        }

        public void WriteLong(long address, long value) {
            var val = BitConverter.GetBytes(value);
            ProcessMemory.WriteProcessMemory(_processHandle, address + EmulatorOffset, val, 8, out int lpNumberOfBytesWritten);
        }

        public void WriteLong(string address, long value, int offset = 0) {
            if (TryGetAddress(address, out var key)) {
                var val = BitConverter.GetBytes(value);
                ProcessMemory.WriteProcessMemory(_processHandle, key + EmulatorOffset + offset, val, 8, out int lpNumberOfBytesWritten);
                return;
            }
            Console.WriteLine($"[ERROR] Incorrect addres key {address}.");
        }

        #endregion

        #region ULong

        public ulong ReadULong(long address) {
            byte[] buffer = new byte[8];
            ProcessMemory.ReadProcessMemory(_processHandle, address + EmulatorOffset, buffer, 8, out long lpNumberOfBytesRead);
            return BitConverter.ToUInt64(buffer, 0);
        }

        public ulong ReadULong(string address, int offset = 0) {
            if (TryGetAddress(address, out var key)) {
                byte[] buffer = new byte[8];
                ProcessMemory.ReadProcessMemory(_processHandle, key + EmulatorOffset + offset, buffer, 8, out long lpNumberOfBytesRead);
                return BitConverter.ToUInt64(buffer, 0);
            }
            Console.WriteLine($"[ERROR] Incorrect addres key {address}.");
            return 0;
        }

        public void WriteULong(long address, ulong value) {
            var val = BitConverter.GetBytes(value);
            ProcessMemory.WriteProcessMemory(_processHandle, address + EmulatorOffset, val, 8, out int lpNumberOfBytesWritten);
        }

        public void WriteULong(string address, ulong value, int offset = 0) {
            if (TryGetAddress(address, out var key)) {
                var val = BitConverter.GetBytes(value);
                ProcessMemory.WriteProcessMemory(_processHandle, key + EmulatorOffset + offset, val, 8, out int lpNumberOfBytesWritten);
            }
            Console.WriteLine($"[ERROR] Incorrect addres key {address}.");
        }

        #endregion

        public byte[] ReadAoB(long startAddress, long endAddress) {
            long length = endAddress - startAddress;
            byte[] buffer = new byte[length];
            ProcessMemory.ReadProcessMemory(_processHandle, startAddress + EmulatorOffset, buffer, length, out long lpNumberOfBytesRead);
            return buffer;
        }

        public void WriteAoB(long startAddress, byte[] bytes) {
            ProcessMemory.WriteProcessMemory(_processHandle, startAddress + EmulatorOffset, bytes, bytes.Length, out int lpNumberOfBytesWritten);
        }

        public string ReadText(long startAddress, long endAddress) {
            return LoDEncoding.GetString(ReadAoB(startAddress, endAddress));
        }

        public string ReadText(long startAddress) {
            var result = String.Empty;
            int i = 0;
            while (true) {
                var temp = ReadUShort(startAddress + i);
                if (temp == 0xA0FF) {
                    return result;
                }
                result += LoDEncoding.GetChar(temp);
                i += 2;
                if (i > 499) {
                    return result;
                }
            }
        }

        public void WriteText(long address, string text) {
            WriteAoB(address, LoDEncoding.GetBytes(text));
        }

        public int GetAddress(string address) {
            if (TryGetAddress(address, out var result)) {
                return result;
            }
            Console.WriteLine($"[ERROR] Incorrect addres key {address}.");
            return 0;
        }

        private bool TryGetAddress(string address, out int result) {
            if (_addressList.TryGetValue(address, out var key)) {
                result = key[(byte) Region];
                return true;
            }
            result = 0;
            return false;
        }

        public void LoadBattle() {
            // Battle = Factory.MemoryController<Memory.Battle.IBattle>(this);
        }

        private Memory.Battle.IBattle GetBattle() {
            if (Battle != null) {
                return Battle;
            }
            throw new BattleNotInitializedException();
        }

        private bool Emulators(Process proc, string emulatorName) {
            if (Verify(EmulatorOffset)) {
                Debug.WriteLine($"[DEBUG] Previous offset {Convert.ToString(EmulatorOffset, 16).ToUpper()} succesful.");
                return true;
            } else {
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

        private bool DuckStation(Process proc) {
            EmulatorOffset = 0;
            var start = (long) proc.MainModule.BaseAddress;
            var end = start + proc.MainModule.ModuleMemorySize;
            var results = KMP.UnmaskedSearch(_duckstationCheck, ReadAoB(start, end), true);
            foreach (var result in results) {
                foreach (var offset in _duckstationOffsets) {
                    var pointer = ReadLong(result + start - offset);
                    if (Verify(pointer)) {
                        EmulatorOffset = pointer;
                        return true;
                    }
                }
            }
            return false;
        }

        private bool ePSXe(Process proc) {
            EmulatorOffset = 0;
            var start = (long) proc.MainModule.BaseAddress;
            var end = start + proc.MainModule.ModuleMemorySize;
            var results = KMP.UnmaskedSearch(_AoBCheck, ReadAoB(start, end), true);
            foreach (var result in results) {
                var tempOffset = start + result - 0xB070;
                if (Verify(tempOffset)) {
                    EmulatorOffset = tempOffset;
                    return true;
                }
            }
            return false;
        }

        private bool RetroArch(Process proc) {
            try {
                EmulatorOffset = 0;
                var start = (long) proc.MainModule.BaseAddress;
                var end = start + 0x1000008;
                for (int i = 0; i < 17; i++) {
                    var results = KMP.UnmaskedSearch(_AoBCheck, ReadAoB(start, end), true);
                    foreach (var result in results) {
                        var tempOffset = start + result - 0xB070;
                        if (Verify(tempOffset)) {
                            EmulatorOffset = tempOffset;
                            return true;
                        }
                    }
                    start += 0x10000000;
                    end += 0x10000000;
                }
                return false;

            } catch (Exception e) {
                return false;
            }
        }

        private bool Verify(long offset) {
            var start = _versionAddr + offset;
            var end = start + _versionStringLen;
            string version = Encoding.Default.GetString(ReadAoB(start - EmulatorOffset, end - EmulatorOffset));
            if (_versions.TryGetValue(version, out var key)) {
                Region = key;
                return true;
            }
            return false;
        }

        private void LoadRegionalAddresses(Dictionary<string, int[]> addresses) {
            foreach (var address in addresses) {


                if (_addressList.ContainsKey(address.Key)) {
                    for (int i = 0; i < address.Value.Length; i++) {
                        if (address.Value[i] != 0) {
                            _addressList[address.Key][i] = address.Value[i];
                        }
                        if (i > 5) {
                            break;
                        }
                    }
                } else {
                    var buffer = new int[7];
                    for (int i = 0; i < buffer.Length; i++) {
                        buffer[i] = 0x0;
                    }
                    for (int i = 0; i < address.Value.Length; i++) {
                        buffer[i] = address.Value[i];
                        if (i > 5) {
                            break;
                        }
                    }
                    _addressList.Add(address.Key, buffer);
                }
            }

        }

        private static Process FindEmulatorProcess(string emulatorName) {
            Process[] processes = Process.GetProcesses();

            foreach (Process proc in processes) {
                if (proc.ProcessName.Equals(emulatorName, StringComparison.CurrentCultureIgnoreCase) || proc.ProcessName.Contains(emulatorName.ToLower())) { // Find (name).exe in the process list (use task manager to find the name)
                    return proc;
                }
            }
            throw new EmulatorNotFoundException(emulatorName);
        }
    }
}
