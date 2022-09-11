using Dragoon_Modifier.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Controller {
    internal static class GreenButton {
        internal static void Run() {
            if (Settings.Instance.BtnAddPartyMembers && (Settings.Instance.SoloMode || Settings.Instance.DuoMode)) {
                AddSoloPartyMembers();
            }

            if (Settings.Instance.SwitchSlot1) {
                SwitchSlotOne();
            }

            if (Settings.Instance.BtnSwitchExp) {
                SwitchEXP();
            }
        }

        private static void AddSoloPartyMembers() {
            if (Settings.Instance.SoloMode) {
                Settings.Instance.AddSoloPartyMembers = true;
                Emulator.DirectAccess.WriteByte("PARTY_SLOT", Emulator.DirectAccess.ReadByte("PARTY_SLOT"), 0x4);
                Emulator.DirectAccess.WriteByte("PARTY_SLOT", 0, 0x5);
                Emulator.DirectAccess.WriteByte("PARTY_SLOT", 0, 0x6);
                Emulator.DirectAccess.WriteByte("PARTY_SLOT", 0, 0x7);
                Emulator.DirectAccess.WriteByte("PARTY_SLOT", Emulator.DirectAccess.ReadByte("PARTY_SLOT"), 0x8);
                Emulator.DirectAccess.WriteByte("PARTY_SLOT", 0, 0x9);
                Emulator.DirectAccess.WriteByte("PARTY_SLOT", 0, 0xA);
                Emulator.DirectAccess.WriteByte("PARTY_SLOT", 0, 0xB);
                Emulator.DirectAccess.WriteByte("CHAR_TABLE", 3, Emulator.DirectAccess.ReadByte("PARTY_SLOT") * 0x2C + 0x4);
            } else if (Settings.Instance.DuoMode) {
                Settings.Instance.AddSoloPartyMembers = true;
                Emulator.DirectAccess.WriteByte("PARTY_SLOT", Emulator.DirectAccess.ReadByte("PARTY_SLOT"), 0x8);
                Emulator.DirectAccess.WriteByte("PARTY_SLOT", 0, 0x9);
                Emulator.DirectAccess.WriteByte("PARTY_SLOT", 0, 0xA);
                Emulator.DirectAccess.WriteByte("PARTY_SLOT", 0, 0xB);
                Emulator.DirectAccess.WriteByte("CHAR_TABLE", 3, Emulator.DirectAccess.ReadByte("PARTY_SLOT") * 0x2C + 0x4);
            }

            Settings.Instance.BtnAddPartyMembers = false;
        }

        private static void SwitchSlotOne() {
            if (Emulator.DirectAccess.ReadByte("CHAR_TABLE", 0x2C * Settings.Instance.Slot1Select + 0x4) == 0x3 || Emulator.DirectAccess.ReadByte("CHAR_TABLE", 0x2C * Settings.Instance.Slot1Select + 0x4) == 0x23) {
                //if (Settings.SoloMode || Settings.DuoMode) {
                Emulator.DirectAccess.WriteByte("PARTY_SLOT", Settings.Instance.Slot1Select);
                Console.WriteLine("Switched character " + Settings.Instance.Slot1Select + ".");
                Constants.UIControl.WritePLog("Switched character " + Settings.Instance.Slot1Select + ".");
                //} else {
                    //TODO: No Dart
                //}
            } else {
                Console.WriteLine("The selected character is not in the party.");
                Constants.UIControl.WritePLog("The selected character is not in the party.");
            }

            Settings.Instance.SwitchSlot1 = false;
        }

        private static void SwitchEXP() {
            long char1 = Emulator.GetAddress("CHAR_TABLE") + (0x2C * Settings.Instance.SwitchEXPSlot1);
            long char2 = Emulator.GetAddress("CHAR_TABLE") + (0x2C * Settings.Instance.SwitchEXPSlot2);
            int maxEXP = Settings.Instance.Difficulty.Equals("Hell") ? 160000 : 80000;

            if (char1 != char2) {
                if ((Emulator.DirectAccess.ReadByte(char1 + 0x4) == 0x3 || Emulator.DirectAccess.ReadByte(char1 + 0x4) == 0x23) && (Emulator.DirectAccess.ReadByte(char2 + 0x4) == 0x3 || Emulator.DirectAccess.ReadByte(char2 + 0x4) == 0x23)) {
                    if (Emulator.DirectAccess.ReadInt(char1) <= maxEXP && Emulator.DirectAccess.ReadInt(char2) < maxEXP) {
                        int tempEXP = Emulator.DirectAccess.ReadInt(char1);
                        Emulator.DirectAccess.WriteInt(char1, Emulator.DirectAccess.ReadInt(char2));
                        Emulator.DirectAccess.WriteInt(char2, tempEXP);
                        Console.WriteLine("EXP switched.");
                        Constants.UIControl.WritePLog("EXP switched.");
                    } else {
                        Console.WriteLine("One of the characters has more than " + maxEXP + " EXP and can't be switched.");
                        Constants.UIControl.WritePLog("One of the characters has more than " + maxEXP + " EXP and can't be switched.");
                    }
                } else {
                    Console.WriteLine("One of the characters are not in the party.");
                    Constants.UIControl.WritePLog("One of the characters are not in the party.");
                }
            }

            Settings.Instance.BtnSwitchExp = false;
        }
    }
}
