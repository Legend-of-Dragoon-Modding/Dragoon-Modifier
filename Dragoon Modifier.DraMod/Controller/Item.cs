using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Controller {
    internal static class Item {
        private const byte equipmentAmmount = 192;

        /// <summary>
        /// Changes Secondary Character table of <paramref name="characterID"/> according to <paramref name="LoDDictionary"/>. 
        /// </summary>
        /// <param name="emulator"></param>
        /// <param name="LoDDictionary"></param>
        /// <param name="characterID"></param>
        internal static void BattleEquipmentChange(Emulator.IEmulator emulator, LoDDict.ILoDDictionary LoDDictionary, uint characterID) {
            LoDDict.IEquipment weapon = (LoDDict.IEquipment) LoDDictionary.Item[emulator.Memory.CharacterTable[characterID].Weapon];
            LoDDict.IEquipment helmet = (LoDDict.IEquipment) LoDDictionary.Item[emulator.Memory.CharacterTable[characterID].Helmet];
            LoDDict.IEquipment armor = (LoDDict.IEquipment) LoDDictionary.Item[emulator.Memory.CharacterTable[characterID].Armor];
            LoDDict.IEquipment shoes = (LoDDict.IEquipment) LoDDictionary.Item[emulator.Memory.CharacterTable[characterID].Shoes];
            LoDDict.IEquipment accessory = (LoDDict.IEquipment) LoDDictionary.Item[emulator.Memory.CharacterTable[characterID].Accessory];
            LoDDict.IEquipment[] equipment = new LoDDict.IEquipment[5] { weapon, helmet, armor, shoes, accessory };

            Emulator.Memory.SecondaryCharacterTable secondaryTable = emulator.Memory.SecondaryCharacterTable[characterID];

            secondaryTable.EquipAT = (ushort) equipment.Sum(item => item.AT);
            secondaryTable.EquipMAT = (ushort) equipment.Sum(item => item.MAT);
            secondaryTable.EquipDF = (ushort) equipment.Sum(item => item.DF);
            secondaryTable.EquipMDF = (ushort) equipment.Sum(item => item.MDF);
            secondaryTable.EquipSPD = (ushort) equipment.Sum(item => item.SPD);

            secondaryTable.StatusResist = (byte) (weapon.StatusResistance | helmet.StatusResistance | armor.StatusResistance | shoes.StatusResistance | accessory.StatusResistance);
            secondaryTable.E_Half = (byte) (weapon.ElementalResistance | helmet.ElementalResistance | armor.ElementalResistance | shoes.ElementalResistance | accessory.ElementalResistance);
            secondaryTable.E_Immune = (byte) (weapon.ElementalImmunity | helmet.ElementalImmunity | armor.ElementalImmunity | shoes.ElementalImmunity | accessory.ElementalImmunity);
            secondaryTable.EquipA_AV = (short) equipment.Sum(item => item.A_AV);
            secondaryTable.EquipM_AV = (short) equipment.Sum(item => item.M_AV);
            secondaryTable.EquipA_HIT = (short) equipment.Sum(item => item.A_HIT);
            secondaryTable.EquipM_HIT = (short) equipment.Sum(item => item.M_HIT);
            secondaryTable.P_Half = (byte) (((weapon.SpecialBonus1 | helmet.SpecialBonus1 | armor.SpecialBonus1 | shoes.SpecialBonus1 | accessory.SpecialBonus1) >> 5) & 0x1);
            secondaryTable.M_Half = (byte) (((weapon.SpecialBonus2 | helmet.SpecialBonus2 | armor.SpecialBonus2 | shoes.SpecialBonus2 | accessory.SpecialBonus2) >> 4) & 0x1);

            secondaryTable.MP_M_Hit = (short) equipment.Sum(item => (item.SpecialBonus1 & 0x1) * item.SpecialBonusAmmount);
            secondaryTable.SP_M_Hit = (short) equipment.Sum(item => ((item.SpecialBonus1 >> 1) & 0x1) * item.SpecialBonusAmmount);
            secondaryTable.MP_P_Hit = (short) equipment.Sum(item => ((item.SpecialBonus1 >> 2) & 0x1) * item.SpecialBonusAmmount);
            secondaryTable.SP_P_Hit = (short) equipment.Sum(item => ((item.SpecialBonus1 >> 3) & 0x1) * item.SpecialBonusAmmount);
            secondaryTable.SP_Regen = (short) equipment.Sum(item => ((item.SpecialBonus2 >> 4) & 0x1) * item.SpecialBonusAmmount);
            secondaryTable.MP_Regen = (short) equipment.Sum(item => ((item.SpecialBonus2 >> 5) & 0x1) * item.SpecialBonusAmmount);
            secondaryTable.HP_Regen = (short) equipment.Sum(item => ((item.SpecialBonus2 >> 6) & 0x1) * item.SpecialBonusAmmount);

            secondaryTable.SP_Multi = (short) equipment.Sum(item => ((item.SpecialBonus1 >> 4) & 0x1) * item.SpecialBonusAmmount);
            secondaryTable.MP_Multi = (byte) equipment.Sum(item => (item.SpecialBonus2 & 0x1) * item.SpecialBonusAmmount);
            secondaryTable.HP_Multi = (byte) equipment.Sum(item => ((item.SpecialBonus2 >> 2) & 0x1) * item.SpecialBonusAmmount);

            secondaryTable.Revive = (byte) equipment.Sum(item => ((item.SpecialBonus2 >> 3) & 0x1) * item.SpecialBonusAmmount);
            secondaryTable.SpecialEffect = (byte) (weapon.SpecialEffect | helmet.SpecialEffect | armor.SpecialEffect | shoes.SpecialEffect | accessory.SpecialEffect);

            secondaryTable.WeaponElement = weapon.WeaponElement;
            secondaryTable.OnHitStatus = weapon.OnHitStatus;
            secondaryTable.OnHitStatusChance = weapon.OnHitStatusChance;
        }

        /// <summary>
        /// Changes all equipment slots according to <paramref name="LoDDictionary"/>. 
        /// </summary>
        /// <param name="emulator"></param>
        /// <param name="LoDDictionary"></param>
        internal static void FieldEquipmentChange(Emulator.IEmulator emulator, LoDDict.ILoDDictionary LoDDictionary) {
            for (int i = 0; i < equipmentAmmount; i++) {
                var equipment = (LoDDict.IEquipment) LoDDictionary.Item[i];
                var itemSlot = (Emulator.Memory.IEquipment) emulator.Memory.Item[i];

                itemSlot.WhoEquips = equipment.WhoEquips;
                itemSlot.ItemType = equipment.Type;
                itemSlot.WeaponElement = equipment.WeaponElement;
                itemSlot.Status = equipment.OnHitStatus;
                itemSlot.StatusChance = equipment.OnHitStatusChance;
                itemSlot.AT = (byte) Math.Min(equipment.AT, (ushort) 255);
                itemSlot.AT2 = (byte) Math.Max(Math.Min(equipment.AT - 255, 255), 0);
                itemSlot.MAT = equipment.MAT;
                itemSlot.DF = equipment.DF;
                itemSlot.MDF = equipment.MDF;
                itemSlot.SPD = equipment.SPD;
                itemSlot.A_HIT = equipment.A_HIT;
                itemSlot.M_HIT = equipment.M_HIT;
                itemSlot.A_AV = equipment.A_AV;
                itemSlot.M_AV = equipment.M_AV;
                itemSlot.E_Half = equipment.ElementalResistance;
                itemSlot.E_Immune = equipment.ElementalImmunity;
                itemSlot.StatusResist = equipment.StatusResistance;
                itemSlot.Special1 = equipment.SpecialBonus1;
                itemSlot.Special2 = equipment.SpecialBonus2;
                itemSlot.SpecialAmmount = equipment.SpecialBonusAmmount;
                itemSlot.SpecialEffect = equipment.SpecialEffect;
            }
        }

        /// <summary>
        /// Changes all item icons according to <paramref name="LoDDictionary"/>.
        /// </summary>
        /// <param name="emulator"></param>
        /// <param name="LoDDictionary"></param>
        internal static void IconChange(Emulator.IEmulator emulator, LoDDict.ILoDDictionary LoDDictionary) {
            for (int itemID = 0; itemID < LoDDictionary.Item.Length; itemID++) {
                emulator.Memory.Item[itemID].Icon = LoDDictionary.Item[itemID].Icon;
            }
        }

        /// <summary>
        /// Changes all usable item names and descriptions according to <paramref name="LoDDictionary"/>.
        /// </summary>
        /// <param name="emulator"></param>
        /// <param name="LoDDictionary"></param>
        internal static void BattleItemNameDescChange(Emulator.IEmulator emulator, LoDDict.ILoDDictionary LoDDictionary) {
            emulator.WriteAoB(emulator.GetAddress("ITEM_BTL_NAME"), LoDDictionary.ItemBattleNames);
            emulator.WriteAoB(emulator.GetAddress("ITEM_BTL_DESC"), LoDDictionary.ItemBattleDescriptions);

            for (int itemID = equipmentAmmount; itemID < 256; itemID++) {
                var memory = (Emulator.Memory.IUsableItem) emulator.Memory.Item[itemID];
                var itemData = (LoDDict.IUsableItem) LoDDictionary.Item[itemID];

                memory.BattleNamePointer = (uint) itemData.BattleNamePointer;
                memory.BattleDescriptionPointer = (uint) itemData.BattleDescriptionPointer;
            }
        }

        /// <summary>
        /// Changes all item names and descriptions according to <paramref name="LoDDictionary"/>.
        /// </summary>
        /// <param name="emulator"></param>
        /// <param name="LoDDictionary"></param>
        internal static void FieldItemNameDescChange(Emulator.IEmulator emulator, LoDDict.ILoDDictionary LoDDictionary) {
            emulator.WriteAoB(emulator.GetAddress("ITEM_NAME"), LoDDictionary.ItemNames); // TODO Make these into a byte[] to improve performance
            emulator.WriteAoB(emulator.GetAddress("ITEM_DESC"), LoDDictionary.ItemDescriptions);

            for (int itemID = 0; itemID < emulator.Memory.Item.Length; itemID++) {
                emulator.Memory.Item[itemID].NamePointer = (uint) LoDDictionary.Item[itemID].NamePointer;
                emulator.Memory.Item[itemID].DescriptionPointer = (uint) LoDDictionary.Item[itemID].DescriptionPointer;
            }
        }
    }
}
