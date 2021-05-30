using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier {
    static public class Attach {
        const string AoBCheck = "50 53 2D 58 20 45 58 45";
        const uint versionAddr = 0x9E19;
        const uint versionStringLen = 11;

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

        public static bool Verify(long offset) {
            var start = versionAddr + offset;
            var end = start + versionStringLen;
            string version = Encoding.Default.GetString(Emulator.ReadAoB(start, end));
            if (versions.TryGetValue(version, out var key)) {
                Constants.REGION = key;
                Constants.WriteOutput($"Detected region: {key}");
                return true;
            }
            return false;
        }

        public static bool Setup(string emulatorName) {
            Process[] processList = Process.GetProcesses();

            if (emulatorName.ToLower().Contains(".exe")) {
                emulatorName = emulatorName.Replace("exe", "");
            }

            foreach (Process proc in processList) {
                if (proc.ProcessName.Equals(emulatorName, StringComparison.CurrentCultureIgnoreCase) || proc.ProcessName.Contains(emulatorName.ToLower())) { // Find (name).exe in the process list (use task manager to find the name)
                    Emulator.SetHandle(proc);
                    return Emulators(proc, emulatorName);
                }
            }
            return false;
        }

        static bool Emulators(Process proc, string emulatorName) {
            if (Verify(Constants.OFFSET)) {
                Constants.KEY.SetValue("Offset", Constants.OFFSET);
                Constants.WriteOutput("Previous offset successful.");
                return true;
            } else {
                Constants.OFFSET = 0;
                if (emulatorName.ToLower() == "retroarch") {
                    return RetroArch(proc);
                } else if (emulatorName.ToLower().Contains("duckstation")) {
                    return DuckStation(proc);
                } else if (emulatorName.Contains("ePSXe")){
                    return ePSXe(proc);
                }

                if (Constants.OFFSET <= 0) { //Fallback
                    Constants.WriteOutput("PSX EXE scan failed. Trying static offsets...");
                    long[] knownOffsets = { 0x5B6E40, 0x94C020, 0xA52EA0, 0xA579A0, 0xA8B6A0, 0x24000000 };
                    long[] baseOffsets = { 0x81A020, 0x825140, 0xA82020 };
                    bool found = false;

                    foreach (long address in knownOffsets) {
                        Constants.OFFSET = address;
                        if (Attach.Verify(address)) {
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
                            if (Attach.Verify((long) proc.MainModule.BaseAddress + address)) {
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
        }

        public static bool ePSXe(Process proc) {
            var start = (long) proc.MainModule.BaseAddress;
            var end = start + proc.MainModule.ModuleMemorySize;
            Constants.WriteOutput("Starting Scan: " + Convert.ToString(start, 16).ToUpper() + " - " + Convert.ToString(end, 16).ToUpper());
            var results = KMP.Search(AoBCheck, Emulator.ReadAoB(start, end), true);
            foreach (var result in results) {
                var tempOffset = start + result - 0xB070;
                if (Verify(tempOffset)) {
                    Constants.OFFSET = tempOffset;
                    Constants.KEY.SetValue("Offset", Constants.OFFSET);
                    Constants.WriteOutput("Base scan successful.");
                    return true;
                }
            }
            return false;
        }

        public static bool RetroArch(Process proc) {
            try {
                var start = (long) proc.MainModule.BaseAddress;
                var end = start + 0x1000008;
                for (int i = 0; i < 17; i++) {
                    Constants.WriteOutput("Start RetroArch Scan (" + i + "/16): " + Convert.ToString(start, 16).ToUpper() + " - " + Convert.ToString(end, 16).ToUpper());
                    var results = KMP.Search(AoBCheck, Emulator.ReadAoB(start, end), true);
                    foreach (var result in results) {
                        var tempOffset = start + result - 0xB070;
                        if (Verify(tempOffset)) {
                            Constants.OFFSET = tempOffset;
                            Constants.KEY.SetValue("Offset", Constants.OFFSET);
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

        public static bool DuckStation(Process proc) {
            var start = (long) proc.MainModule.BaseAddress;
            var end = start + proc.MainModule.ModuleMemorySize;
            string duckstation = "53 6F 6E 79 20 43 6F 6D 70 75 74 65 72 20 45 6E 74 65 72 74 61 69 6E 6D 65 6E 74 20 49 6E 63";
            var results = KMP.Search(duckstation, Emulator.ReadAoB(start, end), true);
            foreach (var result in results) {
                foreach (var offset in duckstationOffsets) {
                    var pointer = Emulator.ReadLong(result + start - offset);
                    if (Verify(pointer)) {
                        Constants.OFFSET = pointer;
                        Constants.KEY.SetValue("Offset", Constants.OFFSET);
                        Constants.WriteOutput("Base scan successful.");
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
