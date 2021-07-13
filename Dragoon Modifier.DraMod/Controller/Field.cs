using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Controller {
    internal static class Field {
        private static readonly ushort[] shopMaps = new ushort[] { 16, 23, 83, 84, 122, 145, 175, 180, 193, 204, 211, 214, 247,
        287, 309, 329, 332, 349, 357, 384, 435, 479, 515, 530, 564, 619, 624}; // Some maps missing?? 

        internal static void Setup(Emulator.IEmulator emulator, UI.IUIControl uiControl) {
            uiControl.ResetBattle();
        }

        internal static void Run(Emulator.IEmulator emulator, UI.IUIControl uiControl) {
            if (Settings.AutoCharmPotion) {
                AutoCharmPotion(emulator);
            }

            uiControl.UpdateField(emulator.Memory.BattleValue, emulator.Memory.EncounterID, emulator.Memory.MapID);
        }

        private static void AutoCharmPotion(Emulator.IEmulator emulator) {
            if (emulator.Memory.BattleValue > 3850 && emulator.Memory.Gold >= 8) {
                emulator.Memory.Gold -= 8;
                emulator.Memory.BattleValue = 0;
            }
        }

        private static void ItemStatChange(Emulator.IEmulator emulator, LoDDict.ILoDDictionary LoDDictionary) {
            for (int i = 0; i < 192; i++) {
                var equip = (LoDDict.IEquipment) LoDDictionary.Item[i];
                var mem = (Emulator.Memory.IEquipment) emulator.Memory.Item[i];
                mem.WhoEquips = equip.WhoEquips;
                mem.ItemType = equip.Type;
                mem.WeaponElement = equip.WeaponElement;
                mem.Status = equip.OnHitStatus;
                mem.StatusChance = equip.OnHitStatusChance;
                mem.AT = (byte) Math.Min(equip.AT, (ushort) 255);
                mem.AT2 = (byte) Math.Max(Math.Min(equip.AT - 255, 255), 0);
                mem.MAT = equip.MAT;
                mem.DF = equip.DF;
                mem.MDF = equip.MDF;
                mem.SPD = equip.SPD;
                mem.A_HIT = equip.A_HIT;
                mem.M_HIT = equip.M_HIT;
                mem.A_AV = equip.A_AV;
                mem.M_AV = equip.M_AV;
                mem.E_Half = equip.ElementalResistance;
                mem.E_Immune = equip.ElementalImmunity;
                mem.StatusResist = equip.StatusResistance;
                mem.Special1 = equip.SpecialBonus1;
                mem.Special2 = equip.SpecialBonus2;
                mem.SpecialAmmount = equip.SpecialBonusAmmount;
                mem.SpecialEffect = equip.SpecialEffect;
            }
        }

        private static void ThrownItemChange(Emulator.IEmulator emulator, LoDDict.ILoDDictionary LoDDictionary) {
            for (int i = 192; i < 255; i++) {
                var item = (LoDDict.IUsableItem) LoDDictionary.Item[i];
                var mem = (Emulator.Memory.IUsableItem) emulator.Memory.Item[i];
                mem.Target = item.Target;
                mem.Element = item.Element;
                mem.Damage = item.Damage;
                mem.Special1 = item.Special1;
                mem.Special2 = item.Special2;
                mem.Unknown1 = item.Unknown1;
                mem.SpecialAmmount = item.SpecialAmmount;
                mem.Status = item.Status;
                mem.Percentage = item.Percentage;
                mem.Unknown2 = item.Unknown2;
                mem.BaseSwitch = item.BaseSwitch;
            }
        }
    }
}
