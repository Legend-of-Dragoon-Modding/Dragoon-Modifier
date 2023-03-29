using Dragoon_Modifier.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Controller {
    internal class SpecialEquips {
        static byte spiritEaterSlot = 0;
        static bool spiritEaterCheck = false;
        static short spirtEaterSPSave = 0;
        static byte harpoonSlot = 0;
        public static bool harpoonCheck = false;
        static byte elementArrowSlot = 0;
        static byte elementArrowElement = 0x80;
        static byte elementArrowItem = 0xC3;
        static byte elementArrowLastAction = 255;
        static byte elementArrowTurns = 0;
        static byte dragonBeaterSlot = 0;
        static byte batteryGloveSlot = 0;
        static byte batteryGloveLastAction = 0;
        static byte batteryGloveCharge = 0;
        public static byte jeweledHammerSlot = 0;
        static byte giantAxeSlot = 0;
        static byte giantAxeLastAction = 0;
        static byte fakeLegendCasqueSlot = 0;
        static bool[] fakeLegendCasqueCheck = { false, false, false };
        static byte fakeLegendArmorSlot = 0;
        static bool[] fakeLegendArmorCheck = { false, false, false };
        static byte legendCasqueSlot = 0;
        static byte[] legendCasqueCount = { 0, 0, 0 };
        static bool[] legendCasqueCheckMDF = { false, false, false };
        static bool[] legendCasqueCheckShield = { false, false, false };
        static byte armorOfLegendSlot = 0;
        static byte[] armorOfLegendCount = { 0, 0, 0 };
        static bool[] armorOfLegendCheckDF = { false, false, false };
        static bool[] armorOfLegendCheckShield = { false, false, false };
        public static byte soasSiphonRingSlot = 0;
        static byte soasAnkhSlot = 0;

        public static void Reset() {
            soasSiphonRingSlot = 0;
            spiritEaterSlot = 0;
            spiritEaterCheck = false;
            spirtEaterSPSave = 0;
            harpoonSlot = 0;
            harpoonCheck = false;
            elementArrowSlot = 0;
            dragonBeaterSlot = 0;
            batteryGloveSlot = 0;
            batteryGloveLastAction = 0;
            batteryGloveCharge = 0;
            jeweledHammerSlot = 0;
            giantAxeSlot = 0;
            giantAxeLastAction = 0;
            fakeLegendCasqueSlot = 0;
            fakeLegendCasqueCheck = new bool[] { false, false, false };
            fakeLegendArmorSlot = 0;
            fakeLegendArmorCheck = new bool[] { false, false, false };
            legendCasqueSlot = 0;
            legendCasqueCount = new byte[] { 0, 0, 0 };
            legendCasqueCheckMDF = new bool[] { false, false, false };
            legendCasqueCheckShield = new bool[] { false, false, false };
            armorOfLegendSlot = 0;
            armorOfLegendCount = new byte[] { 0, 0, 0 };
            armorOfLegendCheckDF = new bool[] { false, false, false };
            armorOfLegendCheckShield = new bool[] { false, false, false };
            soasAnkhSlot = 0;
        }

        public static void Setup(byte slot) {
            var c = Emulator.Memory.Battle.CharacterTable[slot];
            var s = Emulator.Memory.SecondaryCharacterTable[slot];

            if (c.Accessory == 149) { //Phantom Shield
                c.DF *= (ushort) (c.Armor == 74 ? 1.1 : 0.7); //if armor of legend
                c.MDF *= (ushort) (c.Helmet == 89 ? 1.1 : 0.7); //if legend casque
                c.OG_DF = c.DF;
                c.OG_MDF = c.MDF;
            }

            if (c.Accessory == 150) { //Dragon Shield
                c.DF *= (ushort) (c.Armor == 74 ? 1.2 : 0.7); //if armor of legend
                c.OG_DF = c.DF;
            }

            if (c.Accessory == 150) { //Dragon Shield
                c.MDF *= (ushort) (c.Helmet == 89 ? 1.2 : 0.7); //if legend casque
                c.OG_MDF = c.MDF;
            }

            if (c.Weapon == 159) { //Spirit Eater
                spiritEaterSlot |= (byte) (1 << slot);
                spirtEaterSPSave = c.SP_Regen;
            }

            if (c.Weapon == 160) { //Harpoon
                harpoonSlot |= (byte) (1 << slot);
            }

            if (c.Weapon == 161) { //Element Arrow
                elementArrowSlot |= (byte) (1 << slot);
            }

            if (c.Weapon == 162) { //Dragon Beater
                dragonBeaterSlot |= (byte) (1 << slot);
            }

            if (c.Weapon == 163) { //Battery Glove
                batteryGloveSlot |= (byte) (1 << slot);
                batteryGloveLastAction = 0;
                batteryGloveCharge = 0;
            }

            if (c.Weapon == 164) { //Jeweled Hammer
                jeweledHammerSlot |= (byte) (1 << slot);
            }

            if (c.Weapon == 165) { //Giant Axe
                giantAxeSlot |= (byte) (1 << slot);
                giantAxeLastAction = 0;
            }

            if (c.Weapon == 166) { //Soa's Light
                c.SP_Multi = 65436;
                c.SP_Regen = 100;

                foreach (var o in Emulator.Memory.Battle.CharacterTable) {
                    if (o.ID != c.ID) {
                        o.DF *= (ushort) 0.7;
                        o.MDF *= (ushort) 0.7;
                        o.OG_DF = o.DF;
                        o.OG_MDF = o.MDF;
                    }
                }
            }

            if (c.Helmet == 168) { //Soa's Helm
                foreach (var o in Emulator.Memory.Battle.CharacterTable) {
                    if (o.ID != c.ID) {
                        o.AT *= (ushort) 0.7;
                        o.OG_AT = o.AT;
                    }
                }
            }

            if (c.Armor == 170) { //Divine DG Armor
                c.SP_P_Hit += 20;
                c.SP_M_Hit += 20;
                c.MP_P_Hit += 10;
                c.MP_M_Hit += 10;
                s.SP_P_Hit = c.SP_P_Hit;
                s.SP_M_Hit = c.SP_M_Hit;
                s.MP_P_Hit = c.MP_P_Hit;
                s.MP_M_Hit = c.MP_M_Hit;
            }


            if (c.Armor == 171) { //Soa's Helm
                foreach (var o in Emulator.Memory.Battle.CharacterTable) {
                    if (o.ID != c.ID) {
                        o.MAT *= (ushort) 0.7;
                        o.OG_MAT = o.MAT;
                    }
                }
            }

            if (c.Shoes == 174) { //Soa's Greaves
                int i = 0;
                foreach (var o in Emulator.Memory.Battle.CharacterTable) {
                    if (o.ID != c.ID) {
                        var os = Emulator.Memory.SecondaryCharacterTable[i];
                        o.SPD -= 25;
                        o.OG_SPD = o.SPD;
                        os.BodySPD -= 25;
                    }
                    i++;
                }
            }

            if (c.Accessory == 176) { //Soa's Sash
                foreach (var o in Emulator.Memory.Battle.CharacterTable) {
                    if (o.ID != c.ID) {
                        o.SP_Multi -= 50;
                    }
                }
            }

            if (c.Accessory == 177) { //Soa's Ankh
                if (Emulator.Memory.Battle.CharacterTable.Length == 1) {
                    c.Revive -= 50;
                }

                soasAnkhSlot |= (byte) (1 << slot);
            }

            if (c.Accessory == 178) { //Soa's Health Ring
                foreach (var o in Emulator.Memory.Battle.CharacterTable) {
                    if (o.ID != c.ID) {
                        o.MaxHP *= (ushort) 0.75;
                        o.HP = Math.Min(o.HP, o.MaxHP);
                    }
                }
            }

            if (c.Accessory == 179) { //Soa's Mage Ring
                foreach (var o in Emulator.Memory.Battle.CharacterTable) {
                    if (o.ID != c.ID) {
                        o.MaxMP *= (ushort) 0.5;
                        o.MP = Math.Min(o.MP, o.MaxMP);
                    }
                }
            }

            if (c.Accessory == 181) { //Soa's Siphon Ring
                c.MAT *= 2;
                c.OG_MAT = c.MAT;
                c.DMAT *= (ushort) 0.3;

                foreach (var o in Emulator.Memory.Battle.CharacterTable) {
                    if (o.ID != c.ID) {
                        o.MAT *= (ushort) 0.8;
                        o.OG_MAT = o.MAT;
                    }
                }

                soasSiphonRingSlot = (byte) (1 << slot);
            }

            if (c.Helmet == 89) { //Legend Casque
                legendCasqueSlot |= (byte) (1 << slot);
            }

            if (c.Armor == 74) { //Armor of Legend
                if (c.ID == 0) {
                    c.DF = (ushort) (c.DF + 41 - 127);
                    c.MDF = (ushort) (c.MDF + 40);
                } else if (c.ID == 1 || c.ID == 5) {
                    c.DF = (ushort) (c.DF + 54 - 127);
                    c.MDF = (ushort) (c.MDF + 27);
                } else if (c.ID == 2 || c.ID == 8) {
                    c.DF = (ushort) (c.DF + 27 - 127);
                    c.MDF = (ushort) (c.MDF + 80);
                } else if (c.ID == 3) {
                    c.DF = (ushort) (c.DF + 41 - 127);
                    c.MDF = (ushort) (c.MDF + 42);
                } else if (c.ID == 4) {
                    c.DF = (ushort) (c.DF + 45 - 127);
                    c.MDF = (ushort) (c.MDF + 40);
                } else if (c.ID == 6) {
                    c.DF = (ushort) (c.DF + 30 - 127);
                    c.MDF = (ushort) (c.MDF + 54);
                } else if (c.ID == 7) {
                    c.DF = (ushort) (c.DF + 88 - 127);
                    c.MDF = (ushort) (c.MDF + 23);
                }
                c.OG_DF = c.DF;
                c.OG_MDF = c.MDF;
                armorOfLegendSlot |= (byte) (1 << slot);
            }

            if (c.Accessory == 130 && c.Armor == 73) { //Holy Ahnk + Angel Robe
                c.Revive -= 15;
            }

            if (c.Accessory == 180) {
                c.HP = 1;
                c.MaxHP = 1;
                c.DF = 10;
                c.MDF = 10;
                c.A_AV = 90;
                c.M_AV = 90;

                foreach (var o in Emulator.Memory.Battle.CharacterTable) {
                    if (o.ID != c.ID) {
                        o.A_HIT *= (byte) 0.8;
                        o.M_HIT *= (byte) 0.8;
                    }
                }
            }

            if (c.ID == 7) {
                ushort equipSpd = (ushort) Math.Round(s.EquipSPD / 2d);
                s.EquipSPD = equipSpd;
                c.SPD -= equipSpd;
                c.OG_SPD = c.SPD;
            }
        }



        public static void Run(byte slot) {
            var c = Emulator.Memory.Battle.CharacterTable[slot];
            var s = Emulator.Memory.SecondaryCharacterTable[slot];

            if ((spiritEaterSlot & (1 << slot)) != 0) {
                Dataset.IEquipment spiritEater = (Dataset.IEquipment) Settings.Instance.Dataset.Item[159];
                if (c.SP == (ushort) (c.DLV * 100)) {
                    c.SP_Regen = 0;
                    spiritEaterCheck = true;
                } else {
                    if (spiritEaterCheck) {
                        c.SP_Regen = spirtEaterSPSave;
                    }
                }
            }

            if ((harpoonSlot & (1 << slot)) != 0) {
                if (c.Action == 10 && c.SP >= 400 && !harpoonCheck) {
                    harpoonCheck = true;
                    if (c.SP == 700) {
                        c.SP = 300;
                        c.DragoonTurns = 3;
                        c.Speed_Down_Turn = 5;
                    } else if (c.SP >= 500) {
                        c.SP = 200;
                        c.DragoonTurns = 2;
                        c.Speed_Down_Turn = 4;
                    } else {
                        c.SP = 100;
                        c.DragoonTurns = 1;
                        c.Speed_Down_Turn = 3;
                    }
                }

                if (harpoonCheck && c.DragoonTurns == 0) {
                    harpoonCheck = false;
                }
            }

            if ((elementArrowSlot & (1 << slot)) != 0) {
                if (elementArrowLastAction != c.Action) {
                    elementArrowLastAction = c.Action;
                    if (elementArrowLastAction == 8) {
                        c.Weapon_Element = elementArrowElement;
                        elementArrowTurns += 1;
                    } else {
                        if (elementArrowLastAction == 10) {
                            c.Weapon_Element = 0;
                        }
                        if (elementArrowTurns == 4) {
                            elementArrowTurns = 0;
                            if (Emulator.Memory.Gold >= 100) {
                                for (int x = 0; x < Constants.InventorySize; x++) {
                                    if (Emulator.Memory.ItemInventory[x] == 255) {
                                        Emulator.Memory.ItemInventory[x] = elementArrowItem;
                                        Emulator.Memory.ItemInventorySize += 1;
                                        Emulator.Memory.Gold -= 100;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if ((dragonBeaterSlot & (1 << slot)) != 0) {
                if (c.Action == 136) {
                    ushort damageSlot = Emulator.Memory.Battle.DamageSlot;
                    if (damageSlot != 0) {
                        ushort HP = c.HP;
                        c.HP = (ushort) Math.Min(c.HP + Math.Round(damageSlot * 0.02) + 2, c.HP);
                        Emulator.Memory.Battle.DamageSlot = 0;
                    }
                }
            }

            if ((batteryGloveSlot & (1 << slot)) != 0) {
                if ((c.Action == 136 || c.Action == 26) &&
                    (batteryGloveLastAction != 136 && batteryGloveLastAction != 26)) {
                    batteryGloveCharge += 1;
                    if (batteryGloveCharge == 7) {
                        c.AT *= (ushort) 2.5;
                    } else {
                        if (batteryGloveCharge > 7) {
                            batteryGloveCharge = 1;
                            c.AT = c.OG_AT;
                        }
                    }
                }

                batteryGloveLastAction = c.Action;
            }

            if ((giantAxeSlot & (1 << slot)) != 0) {
                byte action = c.Action;
                if (action == 136 && giantAxeLastAction != action) {
                    if (new Random().Next(0, 9) < 2) {
                        c.Guard = 1;
                    }
                }
                giantAxeLastAction = action;
            }

            if ((fakeLegendCasqueSlot & (1 << slot)) != 0) {
                if (fakeLegendCasqueCheck[slot] && c.Guard == 1) {
                    if (new Random().Next(0, 9) < 3) {
                        c.MDF += 40;
                    }
                    fakeLegendCasqueCheck[slot] = false;
                }
                if (!fakeLegendCasqueCheck[slot] && (c.Action & 8) != 0) {
                    c.MDF = c.OG_MDF;
                    fakeLegendCasqueCheck[slot] = true;
                }
            }


            if ((fakeLegendArmorSlot & (1 << slot)) != 0) {
                if (fakeLegendArmorCheck[slot] && c.Guard == 1) {
                    if (new Random().Next(0, 9) < 3) {
                        c.DF += 40;
                    }
                    fakeLegendArmorCheck[slot] = false;
                }
                if (!fakeLegendArmorCheck[slot] && (c.Action & 8) != 0) {
                    c.DF = c.OG_DF;
                    fakeLegendArmorCheck[slot] = true;
                }
            }

            if ((soasAnkhSlot & (1 << slot)) != 0) {
                bool alive = false;
                int kill = -1;
                int lastPartyID = -1;
                int x = 0;
                if (c.HP == 0) {
                    foreach (var o in Emulator.Memory.Battle.CharacterTable) {
                        if (o.ID != c.ID && o.HP > 0) {
                            alive = true;
                        }
                    }

                    if (alive) {
                        foreach (var o in Emulator.Memory.Battle.CharacterTable) {
                            if (o.ID != c.ID && c.HP > 0) {
                                if (kill == -1 && new Random().Next(0, 9) < 5 && o.HP > 0) {
                                    kill = x;
                                } else {
                                    lastPartyID = x;
                                }
                            }
                            x++;
                        }
                    }

                    if (kill != -1) {
                        Emulator.Memory.Battle.CharacterTable[kill].HP = 0;
                        Emulator.Memory.Battle.CharacterTable[kill].Action = 192; //TODO point to the death pointer animation
                    } else {
                        Emulator.Memory.Battle.CharacterTable[lastPartyID].HP = 0;
                        Emulator.Memory.Battle.CharacterTable[lastPartyID].Action = 192;
                    }

                    c.HP = 1;
                } else {
                    c.MaxHP = 0;
                    c.Revive = 0;
                    c.Action = 192;
                }
            }

            if ((legendCasqueSlot & (1 << slot)) != 0) {
                if (legendCasqueCheckMDF[slot] && c.Guard == 1) {
                    if (legendCasqueCount[slot] >= 3) {
                        c.MDF *= (ushort) 1.2;
                        legendCasqueCount[slot] = 0;
                    } else {
                        legendCasqueCount[slot] += 1;
                    }
                    legendCasqueCheckMDF[slot] = false;
                }
                if (legendCasqueCheckShield[slot] && c.Guard == 0 && (c.Action & 8) == 0) {
                    if (new Random().Next(0, 9) < 1) {
                        c.SpecialEffect |= 4;
                    }
                    legendCasqueCheckShield[slot] = false;
                }
                if ((legendCasqueCheckMDF[slot] || legendCasqueCheckShield[slot]) && (c.Action & 8) != 0) {
                    if (!legendCasqueCheckMDF[slot] && legendCasqueCount[slot] == 0) {
                        c.MDF = c.OG_MDF;
                    }
                    legendCasqueCheckMDF[slot] = true;
                    legendCasqueCheckShield[slot] = true;
                }
            }

            if ((armorOfLegendSlot & (1 << slot)) != 0) {
                if (armorOfLegendCheckDF[slot] && c.Guard == 1) {
                    if (armorOfLegendCount[slot] >= 3) {
                        c.DF *= (ushort) 1.2;
                        armorOfLegendCount[slot] = 0;
                    } else {
                        armorOfLegendCount[slot] += 1;
                    }
                    armorOfLegendCheckDF[slot] = false;
                }
                if (armorOfLegendCheckShield[slot] && c.Guard == 0 && (c.Action & 8) == 0) {
                    if (new Random().Next(0, 9) < 1) {
                        c.SpecialEffect |= 1;
                    }
                    armorOfLegendCheckShield[slot] = false;
                }
                if ((armorOfLegendCheckDF[slot] || armorOfLegendCheckShield[slot]) && (c.Action & 8) != 0) {
                    if (!armorOfLegendCheckDF[slot] && armorOfLegendCount[slot] == 0) {
                        c.DF = c.OG_DF;
                    }
                    armorOfLegendCheckDF[slot] = true;
                    armorOfLegendCheckShield[slot] = true;
                }
            }
        }
    }
}
