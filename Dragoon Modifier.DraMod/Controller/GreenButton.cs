using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Controller {
    internal static class GreenButton {
        internal static void Run(Emulator.IEmulator emulator, UI.IUIControl uiControl) {
            if (Settings.BtnAddPartyMembers && (Settings.SoloMode || Settings.DuoMode)) {
                AddSoloPartyMembers(emulator, uiControl);
            }

            if (Settings.SwitchSlot1) {
                SwitchSlotOne(emulator, uiControl);
            }

            if (Settings.BtnSwitchExp) {
                SwitchEXP(emulator, uiControl);
            }
        }

        private static void AddSoloPartyMembers(Emulator.IEmulator emulator, UI.IUIControl uiControl) {
            if (Settings.SoloMode) {
                Settings.AddSoloPartyMembers = true;
                emulator.WriteByte("PARTY_SLOT", emulator.ReadByte("PARTY_SLOT"), 0x4);
                emulator.WriteByte("PARTY_SLOT", 0, 0x5);
                emulator.WriteByte("PARTY_SLOT", 0, 0x6);
                emulator.WriteByte("PARTY_SLOT", 0, 0x7);
                emulator.WriteByte("PARTY_SLOT", emulator.ReadByte("PARTY_SLOT"), 0x8);
                emulator.WriteByte("PARTY_SLOT", 0, 0x9);
                emulator.WriteByte("PARTY_SLOT", 0, 0xA);
                emulator.WriteByte("PARTY_SLOT", 0, 0xB);
                emulator.WriteByte("CHAR_TABLE", 3, emulator.ReadByte("PARTY_SLOT") * 0x2C + 0x4);
            } else if (Settings.DuoMode) {
                Settings.AddSoloPartyMembers = true;
                emulator.WriteByte("PARTY_SLOT", emulator.ReadByte("PARTY_SLOT"), 0x8);
                emulator.WriteByte("PARTY_SLOT", 0, 0x9);
                emulator.WriteByte("PARTY_SLOT", 0, 0xA);
                emulator.WriteByte("PARTY_SLOT", 0, 0xB);
                emulator.WriteByte("CHAR_TABLE", 3, emulator.ReadByte("PARTY_SLOT") * 0x2C + 0x4);
            }

            Settings.BtnAddPartyMembers = false;
        }

        private static void SwitchSlotOne(Emulator.IEmulator emulator, UI.IUIControl uiControl) {
            if (emulator.ReadByte("CHAR_TABLE", 0x2C * Settings.Slot1Select + 0x4) == 0x3 || emulator.ReadByte("CHAR_TABLE", 0x2C * Settings.Slot1Select + 0x4) == 0x23) {
                //if (Settings.SoloMode || Settings.DuoMode) {
                    emulator.WriteByte("PARTY_SLOT", Settings.Slot1Select);
                    Console.WriteLine("Switched character " + Settings.Slot1Select + ".");
                    uiControl.WritePLog("Switched character " + Settings.Slot1Select + ".");
                //} else {
                    //TODO: No Dart
                //}
            } else {
                Console.WriteLine("The selected character is not in the party.");
                uiControl.WritePLog("The selected character is not in the party.");
            }

            Settings.SwitchSlot1 = false;
        }

        private static void SwitchEXP(Emulator.IEmulator emulator, UI.IUIControl uiControl) {
            long char1 = emulator.GetAddress("CHAR_TABLE") + (0x2C * Settings.SwitchEXPSlot1);
            long char2 = emulator.GetAddress("CHAR_TABLE") + (0x2C * Settings.SwitchEXPSlot2);
            int maxEXP = Settings.Difficulty.Equals("Hell") ? 160000 : 80000;

            if (char1 != char2) {
                if ((emulator.ReadByte(char1 + 0x4) == 0x3 || emulator.ReadByte(char1 + 0x4) == 0x23) && (emulator.ReadByte(char2 + 0x4) == 0x3 || emulator.ReadByte(char2 + 0x4) == 0x23)) {
                    if (emulator.ReadInt(char1) <= maxEXP && emulator.ReadInt(char2) < maxEXP) {
                        int tempEXP = emulator.ReadInt(char1);
                        emulator.WriteInt(char1, emulator.ReadInt(char2));
                        emulator.WriteInt(char2, tempEXP);
                        Console.WriteLine("EXP switched.");
                        uiControl.WritePLog("EXP switched.");
                    } else {
                        Console.WriteLine("One of the characters has more than " + maxEXP + " EXP and can't be switched.");
                        uiControl.WritePLog("One of the characters has more than " + maxEXP + " EXP and can't be switched.");
                    }
                } else {
                    Console.WriteLine("One of the characters are not in the party.");
                    uiControl.WritePLog("One of the characters are not in the party.");
                }
            }

            Settings.BtnSwitchExp = false;
        }
    }
}
