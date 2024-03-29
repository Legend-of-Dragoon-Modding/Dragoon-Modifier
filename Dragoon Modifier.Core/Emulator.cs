﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Dragoon_Modifier.Core {
    public sealed class Emulator {
        private const uint _versionAddr = 0x9E19;
        private const uint _versionStringLen = 11;
        private static readonly byte[] _AoBCheck = new byte[] { 0x50, 0x53, 0x2D, 0x58, 0x20, 0x45, 0x58, 0x45 };
        private static readonly byte[] _duckstationCheck = new byte[] { 0x53, 0x6F, 0x6E, 0x79, 0x20, 0x43, 0x6F, 0x6D, 0x70, 0x75, 0x74, 0x65, 0x72, 0x20, 0x45, 0x6E, 0x74, 0x65, 0x72, 0x74, 0x61, 0x69, 0x6E, 0x6D, 0x65, 0x6E, 0x74, 0x20, 0x49, 0x6E, 0x63 };
        private static readonly int[] _duckstationOffsets = new int[] { 0x110, 0x118 };
        private static readonly Dictionary<string, Region> _versions = new() {
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

        private static readonly Dictionary<string, int[]> _addressList = new() {
            { "PARTY_SLOT", new int[] { 0xBAC50, 0xB9950, 0xBAF38, 0xBAF48, 0xBAF70, 0xBAF78, 0xBAE88 } },
            { "DISC", new int[] { 0xBC058, 0xBAD58 ,0xBC340, 0xBC350, 0xBC378, 0xBC380 ,0xBC290 } },
            { "CHAPTER", new int[] { 0xBAC60, 0xB9960, 0xBAF48, 0xBAF58, 0xBAF80, 0xBAF88, 0xBAE98 } },
            { "MAP", new int[] { 0x52C30, 0x51930, 0x52F18, 0x52F2C, 0x52F54, 0x52F5C, 0x52E6C } },
            { "DRAGOON_SPIRITS", new int[] { 0xBAD64, 0xB9A64, 0xBAF9C, 0xBB05C, 0xBB084, 0xBB08C, 0xBAF9C } },
            { "HOTKEY", new int[] { 0x7A39C, 0x7909C, 0x7A684, 0x547E8, 0x7A6BC, 0x7A6C4, 0x7A5D4 } },
            { "BATTLE_VALUE", new int[] { 0xC6AE8, 0xC5808, 0xC6DD0, 0xC6DE0, 0xC6E08, 0xC6E10, 0xC6D20 } },
            { "ARMOR_INVENTORY", new int[] { 0xBADB0, 0xB9AB0, 0x0, 0xBB0A8, 0xBB0D0, 0xBB0D8, 0xBAFE8 } },
            { "INVENTORY", new int[] { 0xBAEB1, 0xB9BB1, 0x0, 0xBB1A9, 0xBB1D1, 0xBB1D9, 0xBB0E9 } },
            { "INVENTORY_SIZE", new int[] { 0xBADAE, 0xB9AAE, 0x0, 0x0, 0x0, 0xBB0D6, 0xBAFE6 } },
            { "MENU", new int[] { 0xBDC38, 0xBC938, 0xBDF20, 0xBDF30, 0xBDF58, 0x0, 0xBDE70 } },
            { "MENU_SUBTYPE", new int[] { 0xBDC30, 0xBC930, 0xBDF18, 0x0, 0x0, 0xBDF58, 0xBDE68 } },
            { "TRANSITION", new int[] { 0xCB430 ,0xCA150, 0xCB718, 0xCB728, 0x0, 0x0, 0xCB668 } },
            { "GOLD", new int[] { 0xBAC5C, 0xB995C, 0xBAE94, 0xBAF54, 0x0, 0xBAF84, 0xBAE94 } },
            { "MENU_UNLOCK", new int[] { 0xFD4B8, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 } },
            { "CHAR_TABLE", new int[] { 0xBAEF4, 0xB9BF4, 0x0, 0xBB1EC, 0x0, 0xBB21C, 0xBB12C } },
            { "SECONDARY_CHARACTER_TABLE", new int[] { 0xBE5F8, 0xBD2F8, 0x0, 0xB16290, 0x0, 0xBE920, 0xBE830} },
            { "MENU_ADDITION_TABLE_FLAT", new int[] { 0x52884, 0x51584, 0x0, 0x52B80, 0x0, 0x52BB0, 0x52ABC } },
            { "MENU_ADDITION_TABLE_MULTI", new int[] { 0x113BF7, 0x11238B, 0x0, 0x113E63, 0x0, 0x113EA3, 0x113D43 } },
            { "SHOP_LIST", new int[] { 0xF4930, 0xF3AF8, 0x0, 0xF51AC, 0x0, 0xF51B0, 0xF4B98 } },
            { "SHOP_CONTENT", new int[] { 0x11E0F8, 0x119368, 0x11E3E0, 0x11DD10, 0x0, 0x11E6A8, 0x11E438 } },
            { "SHOP_ID", new int[] { 0x7A3B4, 0x790B4, 0x0, 0x7A6AC, 0x0, 0x7A6DC, 0x7A5EC } },
            { "SHOP_PRICE", new int[] { 0x114310, 0x112AA4, 0x0, 0x11457C, 0x0, 0x1145BC, 0x11145C } },
            { "ITEM_TABLE", new int[] { 0x111FF0, 0x110784, 0x0, 0x11225C, 0x0, 0x11229E, 0x11213E } },
            { "THROWN_ITEM_TABLE", new int[] { 0x4F2AC, 0x4EA8C, 0x0, 0x4F618, 0x0, 0x4F614 ,0x4F4D0 } },
            { "ITEM_NAME", new int[] { 0x117E10, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 } },
            { "ITEM_NAME_PTR", new int[] { 0x11972C, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 } },
            { "ITEM_BTL_NAME", new int[] { 0x50450, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 } },
            { "ITEM_BTL_NAME_PTR", new int[] { 0x50AE8, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 } },
            { "ITEM_DESC", new int[] { 0x114574, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 } },
            { "ITEM_DESC_PTR", new int[] { 0x117A10, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 } },
            { "ITEM_BTL_DESC", new int[] { 0x50BF0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 } },
            { "ITEM_BTL_DESC_PTR", new int[] { 0x51758, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 } },
            { "BATTLE_BASE_POINT", new int[] { 0xBC1C0, 0xBAEC0, 0xBC4A8, 0xBC4D0, 0x0, 0xBC4E8, 0xBC3F8 } },
            { "MONSTER_NAMES", new int[] { 0xC69D0, 0xC56F0, 0xC6C08, 0xC6CC8, 0x0, 0xC6CF8, 0xC6C08 } },
            { "MONSTER_SIZE", new int[] { 0xC6768, 0xC5488, 0xC6A50, 0xC6A50, 0x0, 0xC6A90, 0xC69A0 } },
            { "MONSTER_REWARDS", new int[] { 0x5E52C, 0x5D22C, 0x5E764, 0x5E824, 0x0, 0x5E854, 0x5E764 } },
            { "BATTLE_OFFSET", new int[] { 0xBDA0C, 0x0, 0x0, 0x0 ,0x0 ,0x1B3BB0 ,0xBDC44 } },
            { "UNIQUE_MONSTER_SIZE", new int[] { 0xC6698, 0xC53B8, 0xC6980, 0xC6990, 0x0, 0xC69C0, 0xC68D0 } },
            { "UNIQUE_SLOT", new int[] { 0x5E53A, 0x5D23A, 0x5E252, 0x5E832, 0x0, 0x5E862, 0x5E772 } },
            { "ENCOUNTER_ID", new int[] { 0xBB0F8, 0xB9DF8, 0xBB3E0, 0xBB3F0, 0x0, 0xBB420, 0xBB330 } },
            { "BATTLE_FIELD", new int[] { 0xBB0F4, 0xB9DF4, 0xBB3DC, 0x0, 0x0, 0xBB41C, 0xBB32C } },
            { "MONSTER_ID", new int[] { 0x1CF910, 0x1CE918, 0x1CF8E0, 0x1CF910, 0x0, 0x1CF910, 0x1CF910 } },
            { "DRAGOON_TURNS", new int[] { 0x6E62C, 0x6D32C, 0x0, 0x6E924, 0x0, 0x6E954, 0x6E864 } },
            { "DRAGOON_SPELL_SLOT", new int[] { 0xC6960, 0xC5680, 0x0, 0xC6C58, 0x0, 0x0, 0xC6B98 } },
            { "DRAGOON_STAT_TABLE", new int[] { 0x111B4C, 0x1102E0, 0x0, 0x111DB8, 0x0, 0x111DF8, 0x111C98 } },
            { "DRAGOON_STAT_TABLE_POINTER", new int[] { 0xFBD54, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 } },
            { "DRAGOON_SP_TABLE", new int[] { 0x113B9E, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 } },
            { "DRAGOON_DESC_PTR", new int[] { 0x52018, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 } },
            { "DRAGOON_DESC", new int[] { 0x51858, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 } },
            { "HASCHEL_FIX", new int[] { 0x1E9E30, 0x1E8E38, 0x0, 0x0, 0x0, 0x0, 0x0 } },
            { "HASCHEL_FIX1", new int[] { 0x1EABB0, 0x1E9BB8, 0x0, 0x0, 0x0, 0x0, 0x0 } },
            { "HASCHEL_FIX2", new int[] { 0x1E9E30, 0x1E8E38, 0x0, 0x0, 0x0, 0x0, 0x0 } },
            { "HASCHEL_FIX3", new int[] { 0x1EB288, 0x1EA290, 0x0, 0x0, 0x0, 0x0, 0x0 } },
            { "HASCHEL_FIX4", new int[] { 0x1E9FE0, 0x1E8FE8, 0x0, 0x0, 0x0, 0x0, 0x0 } },
            { "SPELL_TABLE", new int[] { 0xFA0B7, 0xF8BA3, 0x0, 0x0, 0x0, 0xFA323, 0xFA233 } },
            { "WARGOD", new int[] { 0x6E814, 0x6D514, 0x0, 0x6EB08, 0x0, 0x0, 0x6EA4C } },
            { "SAVE_POINT", new int[] { 0x5A368, 0x59068, 0x5A5A0, 0x5A660, 0x0, 0x5A690, 0x5A5A0 } },
            { "TEXT_SPEED", new int[] { 0x26948, 0x26530, 0x0, 0x26CC4, 0x0, 0x26CC0, 0x26B7C } },
            { "AUTO_TEXT", new int[] { 0x26CDA, 0x268C2, 0x0, 0x27056, 0x0, 0x27052, 0x26F0E } },
            { "DAMAGE_CAP", new int[] { 0xF2A5C, 0xF15A4, 0x0, 0xF2C98, 0x0, 0xF32DC, 0xF72CC } },
            { "ASPECT_RATIO", new int[] { 0xC3568, 0xC22A8, 0xC37A0, 0xC3860, 0x0, 0xC3890, 0xC37A0 } },
            { "ADVANCED_CAMERA", new int[] { 0xE8208, 0xE6F08, 0xE8440, 0xE84A4, 0x0, 0xE84D4, 0xE83E4 } },
            { "ADDITION", new int[] { 0x1CF940, 0x1CE948, 0x0, 0x1CFC60, 0x0, 0x1CFCD0, 0x1CFCD0 } },
            { "OVERWORLD_CONTINENT", new int[] { 0xBF0B0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 } },
            { "OVERWORLD_SEGMENT", new int[] { 0xC67AC, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 } },
            { "OVERWORLD_CHECK", new int[] { 0xBB10C, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 } },
            { "ENCOUNTER_MAP", new int[] { 0xF64C4, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 } },
            { "ENCOUNTER_TABLE", new int[] { 0xF74C4, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 } },
            { "FIELD_HP_CAP_1", new int[] { 0x10536C, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 } },
            { "FIELD_HP_CAP_2", new int[] { 0x1056B0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 } },
            { "FIELD_HP_CAP_3", new int[] { 0x1056D0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 } },
            { "FIELD_HP_CAP_4", new int[] { 0x1105E0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 } },
            { "DISC_CHANGE_CHECK", new int[] { 0x4DD30, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 } },
            { "CHAR_STAT_TABLE", new int[] { 0x110DF4, 0x10F588, 0x0, 0x0, 0x0, 0x1110A0, 0x110F40 } },
            { "CHAR_LVL_TABLE", new int[] { 0x1134F4, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 } },
            { "FIELD_POS_PTR", new int[] { 0xBC1E8, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 } },
            { "DRAGOON_SPECIAL", new int[] { 0x6E648, 0x6D348, 0x0, 0x0, 0x0, 0x6E870, 0x6E880 } },
            { "DRAGON_BLOCK_STAFF", new int[] { 0x6E8E8, 0x6D5E8, 0x0, 0x0, 0x0, 0x6EB10, 0x6EB20 } },
            { "BATTLE_UI_COLOUR", new int[] { 0xC7004, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 } },
            { "TIME_PLAYED", new int[] { 0xBAC68, 0xB9968, 0x0, 0x0, 0x0, 0xBAF90, 0xBAEA0 } },
            { "HERO_TICKETS", new int[] { 0xBAC3C, 0xB993C, 0x0, 0x0, 0x0, 0xBAF64, 0xBAE74 } },
            { "DAMAGE_SLOT", new int[] { 0x6E898, 0x6D598, 0x0, 0x0, 0x0, 0x6EBC0, 0x6EAD0 } },
            { "SCREEN_FADE", new int[] { 0xBB140, 0xB9E40, 0xBB428, 0x0, 0x0, 0x0, 0xBB378 } },
            { "BATTLE_SP_CAP_1", new int[] { 0xF44C0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 } },
            { "BATTLE_SP_CAP_2", new int[] { 0xF44CC, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 } },
            { "BATTLE_SP_CAP_3", new int[] { 0xF44DC, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 } },
            { "BATTLE_SP_CAP_4", new int[] { 0xF44E4, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 } },
            { "TOTAL_SP_CAP_1", new int[] { 0x10CEA0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 } },
            { "TOTAL_SP_CAP_2", new int[] { 0x10CEA8, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 } },
            { "UNUSED_RAM", new int[] { 0x75E0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 } },
            { "UNUSED_RAM_1", new int[] { 0x7820, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 } },
            { "DRAGOON_SPELL_NAME_PTR", new int[] { 0x52734, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 } },
            { "AUTO_DRAGOON_1", new int[] { 0x1086DA, 0x107082, 0x0, 0x0, 0x0, 0x10899A, 0x10883A } },
            { "AUTO_DRAGOON_2", new int[] { 0x1086F6, 0x10709E, 0x0, 0x0, 0x0, 0x1089B6, 0x108856 } },
            { "CURSOR_TARGET", new int[] { 0x20062C, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 } }
        };

        private static Emulator? _instance = null;
        public static Emulator DirectAccess {
            get {
                if (_instance == null) {
                    throw new EmulatorNotAttachedException();
                }
                return _instance;
            }
        }

        private Region _region = Region.NTA;
        public static Region Region { get { return DirectAccess._region; } }

        private long _emulatorOffset = 0;
        public static long EmulatorOffset { get { return DirectAccess._emulatorOffset; } }

        private readonly IntPtr _processHandle;

        private Memory.IController _memory;
        private LoDEncoding _loDEncoding;

        public static LoDEncoding TextEncoding {  get { return DirectAccess._loDEncoding; }  }

        public static Memory.IController Memory { get { return DirectAccess._memory; } }
  
        private Emulator(string emulatorName, long previousOffset) {
            _emulatorOffset = previousOffset;

            if (emulatorName.ToLower().Contains(".exe")) {
                emulatorName = emulatorName.Replace(".exe", "");
            }

            Process proc = FindEmulatorProcess(emulatorName);

            _processHandle = ProcessMemory.GetProcessHandle(proc);

            if (!Emulators(proc, emulatorName)) {
                throw new EmulatorAttachException(emulatorName);
            }
        }

        public static void Attach(string emulatorName, long previousOffset, Region region) {
            _instance = new(emulatorName, previousOffset);
            _instance._region = region;
            _instance._memory = new Memory.Controller();
            _instance._loDEncoding = new LoDEncoding(_instance._region);

            Console.WriteLine($"[DEBUG] Succesfully attached to emulator {emulatorName}");
        }

        public static void Detach() {
            _instance = null;
        }

        private bool Emulators(Process proc, string emulatorName) {
            if (Verify(_emulatorOffset)) {
                Console.WriteLine($"[DEBUG] Previous offset {Convert.ToString(_emulatorOffset, 16).ToUpper()} succesful.");
                return true;
            }

            if (emulatorName.ToLower() == "retroarch") {
                return RetroArch(proc);
            }

            if (emulatorName.ToLower().Contains("duckstation")) {
                return DuckStation(proc);
            }

            if (emulatorName.Contains("ePSXe")) {
                return ePSXe(proc);
            }

            return false;
        }

        private bool DuckStation(Process proc) {
            _emulatorOffset = 0;
            var start = (long) proc.MainModule.BaseAddress;
            var end = start + proc.MainModule.ModuleMemorySize;
            var results = KMP.UnmaskedSearch(_duckstationCheck, ReadAoB(start, end), true);
            foreach (var result in results) {
                foreach (var offset in _duckstationOffsets) {
                    var pointer = ReadLong(result + start - offset);
                    if (Verify(pointer)) {
                        _emulatorOffset = pointer;
                        return true;
                    }
                }
            }
            return false;
        }

        private bool ePSXe(Process proc) {
            _emulatorOffset = 0;
            var start = (long) proc.MainModule.BaseAddress;
            var end = start + proc.MainModule.ModuleMemorySize;
            var results = KMP.UnmaskedSearch(_AoBCheck, ReadAoB(start, end), true);
            foreach (var result in results) {
                var tempOffset = start + result - 0xB070;
                if (Verify(tempOffset)) {
                    _emulatorOffset = tempOffset;
                    return true;
                }
            }
            return false;
        }

        private bool RetroArch(Process proc) {
            try {
                _emulatorOffset = 0;
                var start = (long) proc.MainModule.BaseAddress;
                var end = start + 0x1000008;
                for (int i = 0; i < 17; i++) {
                    var results = KMP.UnmaskedSearch(_AoBCheck, ReadAoB(start, end), true);
                    foreach (var result in results) {
                        var tempOffset = start + result - 0xB070;
                        if (Verify(tempOffset)) {
                            _emulatorOffset = tempOffset;
                            return true;
                        }
                    }
                    start += 0x10000000;
                    end += 0x10000000;
                }
                return false;

            } catch (Exception) {
                return false;
            }
        }

        private bool Verify(long offset) {
            var start = _versionAddr + offset;
            var end = start + _versionStringLen;
            string version = Encoding.Default.GetString(ReadAoB(start - _emulatorOffset, end - _emulatorOffset));
            if (_versions.TryGetValue(version, out var key)) {
                _region = key;
                return true;
            }
            return false;
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

        #region Byte

        public byte ReadByte(long address) {
            byte[] buffer = new byte[1];
            ProcessMemory.ReadProcessMemory(_processHandle, address + _emulatorOffset, buffer, 1, out long lpNumberOfBytesRead);
            return buffer[0];
        }

        public byte ReadByte(string address, int offset = 0) {
            if (TryGetAddress(address, out var key)) {
                byte[] buffer = new byte[1];
                ProcessMemory.ReadProcessMemory(_processHandle, key + _emulatorOffset + offset, buffer, 1, out long lpNumberOfBytesRead);
                return buffer[0];
            }
            Console.WriteLine($"[ERROR] Incorrect address key {address}.");
            return 0;
        }

        public void WriteByte(long address, byte value) {
            var val = BitConverter.GetBytes(value);
            ProcessMemory.WriteProcessMemory(_processHandle, address + _emulatorOffset, val, 1, out int lpNumberOfBytesWritten);
        }

        public void WriteByte(string address, byte value, int offset = 0) {
            if (TryGetAddress(address, out var key)) {
                var val = BitConverter.GetBytes(value);
                ProcessMemory.WriteProcessMemory(_processHandle, key + _emulatorOffset + offset, val, 1, out int lpNumberOfBytesWritten);
                return;
            }
            Console.WriteLine($"[ERROR] Incorrect address key {address}.");
        }

        #endregion

        #region SByte
        public sbyte ReadSByte(long address) {
            byte[] buffer = new byte[1];
            ProcessMemory.ReadProcessMemory(_processHandle, address + _emulatorOffset, buffer, 1, out long lpNumberOfBytesRead);
            return (sbyte) buffer[0];
        }

        public sbyte ReadSByte(string address, int offset = 0) {
            if (TryGetAddress(address, out var key)) {
                byte[] buffer = new byte[1];
                ProcessMemory.ReadProcessMemory(_processHandle, key + _emulatorOffset + offset, buffer, 1, out long lpNumberOfBytesRead);
                return (sbyte) buffer[0];
            }
            Console.WriteLine($"[ERROR] Incorrect address key {address}.");
            return 0;
        }

        public void WriteSByte(long address, sbyte value) {
            var val = BitConverter.GetBytes(value);
            ProcessMemory.WriteProcessMemory(_processHandle, address + _emulatorOffset, val, 1, out int lpNumberOfBytesWritten);
        }

        public void WriteSByte(string address, sbyte value, int offset = 0) {
            if (TryGetAddress(address, out var key)) {
                var val = BitConverter.GetBytes(value);
                ProcessMemory.WriteProcessMemory(_processHandle, key + _emulatorOffset + offset, val, 1, out int lpNumberOfBytesWritten);
                return;
            }
            Console.WriteLine($"[ERROR] Incorrect address key {address}.");
        }

        #endregion

        #region Short

        public short ReadShort(long address) {
            byte[] buffer = new byte[2];
            ProcessMemory.ReadProcessMemory(_processHandle, address + _emulatorOffset, buffer, 2, out long lpNumberOfBytesRead);
            return BitConverter.ToInt16(buffer, 0);
        }
        public short ReadShort(string address, int offset = 0) {
            if (TryGetAddress(address, out var key)) {
                byte[] buffer = new byte[2];
                ProcessMemory.ReadProcessMemory(_processHandle, key + _emulatorOffset + offset, buffer, 2, out long lpNumberOfBytesRead);
                return BitConverter.ToInt16(buffer, 0);
            }
            Console.WriteLine($"[ERROR] Incorrect address key {address}.");
            return 0;
        }

        public void WriteShort(long address, short value) {
            var val = BitConverter.GetBytes(value);
            ProcessMemory.WriteProcessMemory(_processHandle, address + _emulatorOffset, val, 2, out int lpNumberOfBytesWritten);
        }
        public void WriteShort(string address, short value, int offset = 0) {
            if (TryGetAddress(address, out var key)) {
                var val = BitConverter.GetBytes(value);
                ProcessMemory.WriteProcessMemory(_processHandle, key + _emulatorOffset + offset, val, 2, out int lpNumberOfBytesWritten);
                return;
            }
            Console.WriteLine($"[ERROR] Incorrect address key {address}.");
        }

        #endregion

        #region UShort

        public ushort ReadUShort(long address) {
            byte[] buffer = new byte[2];
            ProcessMemory.ReadProcessMemory(_processHandle, address + _emulatorOffset, buffer, 2, out long lpNumberOfBytesRead);
            return BitConverter.ToUInt16(buffer, 0);
        }

        public ushort ReadUShort(string address, int offset = 0) {
            if (TryGetAddress(address, out var key)) {
                byte[] buffer = new byte[2];
                ProcessMemory.ReadProcessMemory(_processHandle, key + _emulatorOffset + offset, buffer, 2, out long lpNumberOfBytesRead);
                return BitConverter.ToUInt16(buffer, 0);
            }
            Console.WriteLine($"[ERROR] Incorrect address key {address}.");
            return 0;
        }

        public void WriteUShort(long address, ushort value) {
            var val = BitConverter.GetBytes(value);
            ProcessMemory.WriteProcessMemory(_processHandle, address + _emulatorOffset, val, 2, out int lpNumberOfBytesWritten);
        }

        public void WriteUShort(string address, ushort value, int offset = 0) {
            if (TryGetAddress(address, out var key)) {
                var val = BitConverter.GetBytes(value);
                ProcessMemory.WriteProcessMemory(_processHandle, key + _emulatorOffset + offset, val, 2, out int lpNumberOfBytesWritten);
                return;
            }
            Console.WriteLine($"[ERROR] Incorrect address key {address}.");
        }

        #endregion

        #region UInt24

        public UInt32 ReadUInt24(long address) {
            byte[] buffer = new byte[4];
            ProcessMemory.ReadProcessMemory(_processHandle, address + _emulatorOffset, buffer, 3, out long lpNumberOfBytesRead);
            return BitConverter.ToUInt32(buffer, 0);
        }

        public UInt32 ReadUInt24(string address, int offset = 0) {
            if (TryGetAddress(address, out var key)) {
                byte[] buffer = new byte[4];
                ProcessMemory.ReadProcessMemory(_processHandle, key + _emulatorOffset + offset, buffer, 3, out long lpNumberOfBytesRead);
                return BitConverter.ToUInt32(buffer, 0);
            }
            Console.WriteLine($"[ERROR] Incorrect address key {address}.");
            return 0;
        }

        public void WriteUInt24(long address, UInt32 value) {
            var val = BitConverter.GetBytes(value);
            val = val.Take(val.Count() - 1).ToArray();
            ProcessMemory.WriteProcessMemory(_processHandle, address + _emulatorOffset, val, 3, out int lpNumberOfBytesWritten);
        }

        public void WriteUInt24(string address, UInt32 value, int offset = 0) {
            if (TryGetAddress(address, out var key)) {
                var val = BitConverter.GetBytes(value);
                val = val.Take(val.Count() - 1).ToArray();
                ProcessMemory.WriteProcessMemory(_processHandle, key + _emulatorOffset + offset, val, 3, out int lpNumberOfBytesWritten);
                return;
            }
            Console.WriteLine($"[ERROR] Incorrect address key {address}.");
        }

        #endregion

        #region Int

        public Int32 ReadInt(long address) {
            byte[] buffer = new byte[4];
            ProcessMemory.ReadProcessMemory(_processHandle, address + _emulatorOffset, buffer, 4, out long lpNumberOfBytesRead);
            return BitConverter.ToInt32(buffer, 0);
        }

        public Int32 ReadInt(string address, int offset = 0) {
            if (TryGetAddress(address, out var key)) {
                byte[] buffer = new byte[4];
                ProcessMemory.ReadProcessMemory(_processHandle, key + _emulatorOffset + offset, buffer, 4, out long lpNumberOfBytesRead);
                return BitConverter.ToInt32(buffer, 0);
            }
            Console.WriteLine($"[ERROR] Incorrect address key {address}.");
            return 0;
        }

        public void WriteInt(long address, Int32 value) {
            var val = BitConverter.GetBytes(value);
            ProcessMemory.WriteProcessMemory(_processHandle, address + _emulatorOffset, val, 4, out int lpNumberOfBytesWritten);
        }

        public void WriteInt(string address, Int32 value, int offset = 0) {
            if (TryGetAddress(address, out var key)) {
                var val = BitConverter.GetBytes(value);
                ProcessMemory.WriteProcessMemory(_processHandle, key + _emulatorOffset + offset, val, 4, out int lpNumberOfBytesWritten);
                return;
            }
            Console.WriteLine($"[ERROR] Incorrect address key {address}.");
        }

        #endregion

        #region UInt

        public UInt32 ReadUInt(long address) {
            byte[] buffer = new byte[4];
            ProcessMemory.ReadProcessMemory(_processHandle, address + _emulatorOffset, buffer, 4, out long lpNumberOfBytesRead);
            return BitConverter.ToUInt32(buffer, 0);
        }

        public UInt32 ReadUInt(string address, int offset = 0) {
            if (TryGetAddress(address, out var key)) {
                byte[] buffer = new byte[4];
                ProcessMemory.ReadProcessMemory(_processHandle, key + _emulatorOffset + offset, buffer, 4, out long lpNumberOfBytesRead);
                return BitConverter.ToUInt32(buffer, 0);
            }
            Console.WriteLine($"[ERROR] Incorrect address key {address}.");
            return 0;
        }

        public void WriteUInt(long address, UInt32 value) {
            var val = BitConverter.GetBytes(value);
            ProcessMemory.WriteProcessMemory(_processHandle, address + _emulatorOffset, val, 4, out int lpNumberOfBytesWritten);
        }

        public void WriteUInt(string address, UInt32 value, int offset = 0) {
            if (TryGetAddress(address, out var key)) {
                var val = BitConverter.GetBytes(value);
                ProcessMemory.WriteProcessMemory(_processHandle, key + _emulatorOffset + offset, val, 4, out int lpNumberOfBytesWritten);
                return;
            }
            Console.WriteLine($"[ERROR] Incorrect address key {address}.");
        }

        #endregion

        #region Long

        public long ReadLong(long address) {
            byte[] buffer = new byte[8];
            ProcessMemory.ReadProcessMemory(_processHandle, address + _emulatorOffset, buffer, 8, out long lpNumberOfBytesRead);
            return BitConverter.ToInt64(buffer, 0);
        }

        public long ReadLong(string address, int offset = 0) {
            if (TryGetAddress(address, out var key)) {
                byte[] buffer = new byte[8];
                ProcessMemory.ReadProcessMemory(_processHandle, key + _emulatorOffset + offset, buffer, 8, out long lpNumberOfBytesRead);
                return BitConverter.ToInt64(buffer, 0);
            }
            Console.WriteLine($"[ERROR] Incorrect address key {address}.");
            return 0;
        }

        public void WriteLong(long address, long value) {
            var val = BitConverter.GetBytes(value);
            ProcessMemory.WriteProcessMemory(_processHandle, address + _emulatorOffset, val, 8, out int lpNumberOfBytesWritten);
        }

        public void WriteLong(string address, long value, int offset = 0) {
            if (TryGetAddress(address, out var key)) {
                var val = BitConverter.GetBytes(value);
                ProcessMemory.WriteProcessMemory(_processHandle, key + _emulatorOffset + offset, val, 8, out int lpNumberOfBytesWritten);
                return;
            }
            Console.WriteLine($"[ERROR] Incorrect address key {address}.");
        }

        #endregion

        #region ULong

        public ulong ReadULong(long address) {
            byte[] buffer = new byte[8];
            ProcessMemory.ReadProcessMemory(_processHandle, address + _emulatorOffset, buffer, 8, out long lpNumberOfBytesRead);
            return BitConverter.ToUInt64(buffer, 0);
        }

        public ulong ReadULong(string address, int offset = 0) {
            if (TryGetAddress(address, out var key)) {
                byte[] buffer = new byte[8];
                ProcessMemory.ReadProcessMemory(_processHandle, key + _emulatorOffset + offset, buffer, 8, out long lpNumberOfBytesRead);
                return BitConverter.ToUInt64(buffer, 0);
            }
            Console.WriteLine($"[ERROR] Incorrect address key {address}.");
            return 0;
        }

        public void WriteULong(long address, ulong value) {
            var val = BitConverter.GetBytes(value);
            ProcessMemory.WriteProcessMemory(_processHandle, address + _emulatorOffset, val, 8, out int lpNumberOfBytesWritten);
        }

        public void WriteULong(string address, ulong value, int offset = 0) {
            if (TryGetAddress(address, out var key)) {
                var val = BitConverter.GetBytes(value);
                ProcessMemory.WriteProcessMemory(_processHandle, key + _emulatorOffset + offset, val, 8, out int lpNumberOfBytesWritten);
            }
            Console.WriteLine($"[ERROR] Incorrect address key {address}.");
        }

        #endregion

        #region AoB

        public byte[] ReadAoB(long startAddress, long endAddress) {
            long length = endAddress - startAddress;
            byte[] buffer = new byte[length];
            ProcessMemory.ReadProcessMemory(_processHandle, startAddress + _emulatorOffset, buffer, length, out long lpNumberOfBytesRead);
            return buffer;
        }

        public void WriteAoB(long startAddress, byte[] bytes) {
            ProcessMemory.WriteProcessMemory(_processHandle, startAddress + _emulatorOffset, bytes, bytes.Length, out int lpNumberOfBytesWritten);
        }

        public void WriteAoB(long startAddress, string byteString) {
            string[] str = byteString.Split(' ');

            byte[] bytes = new byte[str.Length];
            for (int i = 0; i < str.Length; i++) {
                bytes[i] = Convert.ToByte(str[i], 16);
            }
            ProcessMemory.WriteProcessMemory(_processHandle, startAddress + _emulatorOffset, bytes, bytes.Length, out int lpNumberOfBytesWritten);
        }

        public List<long> ScanAoB(long start, long end, string pattern, bool useOffset = true, bool addOffset = false) {
            long offset = 0;
            if (!useOffset) {
                offset -= _emulatorOffset;
            }

            List<long> results = KMP.Search(pattern, ReadAoB(start, end), true);

            for (int i = 0; i < results.Count; i++) {
                results[i] += start;
                if (addOffset) {
                    results[i] += _emulatorOffset;
                }
            }

            return results;
        }

        public List<long> ScanAoB(long start, long end, byte[] pattern, bool useOffset = true, bool addOffset = false) {
            long offset = 0;
            if (!useOffset) {
                offset -= _emulatorOffset;
            }

            List<long> results = KMP.UnmaskedSearch(pattern, ReadAoB(start, end), true);

            for (int i = 0; i < results.Count; i++) {
                results[i] += start;
                if (addOffset) {
                    results[i] += _emulatorOffset;
                }
            }

            return results;
        }

        #endregion

        #region Text
        
        public string ReadText(long startAddress, long endAddress) {
            return _loDEncoding.GetString(ReadAoB(startAddress, endAddress));
        }

        public string ReadText(long startAddress) {
            var arr = new List<ushort>();
            int i = 0;
            while (i < 200) {
                i += 2;
                if (ReadUShort(startAddress + i) == 0xA0FF) { // END
                    break;
                }
            }
            return _loDEncoding.GetString(ReadAoB(startAddress, startAddress + i + 2));
        }

        public void WriteText(long address, string text) {
            WriteAoB(address, _loDEncoding.GetBytes(text));
        }
 
        #endregion

        private bool TryGetAddress(string address, out int result) {
            if (_addressList.TryGetValue(address, out var key)) {
                result = key[(byte) _region];
                return true;
            }
            result = 0;
            return false;
        }

        public static int GetAddress(string address) {
            if (_instance.TryGetAddress(address, out var result)) {
                return result;
            }
            Console.WriteLine($"[ERROR] Incorrect address key {address}.");
            return 0;
        }


    }
}
