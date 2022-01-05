using Dragoon_Modifier.DraMod.UI;
using Dragoon_Modifier.Emulator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.LoDDict.Scripts.HardMode {
    class ItemScript : IItemScript {
        
        const short heatBladeBuff = 7;
        const short shadowCutterBuff = 9;
        const short sparkleArrowBuff = 8;
        const short morningStarBuff = -20;

        const byte heatBladeID = 2;
        const byte shadowCutterID = 14;
        const byte sparkleArrowID = 28;
        const byte morningStarID = 35;
        
        public void BattleRun(IEmulator emulator, IUIControl uiControl) {
            throw new NotImplementedException();
        }

        public void BattleSetup(IEmulator emulator, IUIControl uiControl) {
            BattleChapter3Buffs(emulator);
        }

        public void FieldRun(IEmulator emulator, IUIControl uiControl) {
            throw new NotImplementedException();
        }

        public void FieldSetup(IEmulator emulator, IUIControl uiControl) {
            FieldChapter3Buffs(emulator);
        }

        private static void BattleChapter3Buffs(IEmulator emulator) {
            if (emulator.Memory.Chapter < 3) {
                return;
            }

            for (int character = 0; character < 9; character++) {
                short modif = 0;
    
                switch (emulator.Memory.CharacterTable[character].Weapon) {
                    case heatBladeID:
                        modif = heatBladeBuff;
                        break;

                    case shadowCutterID:
                        modif = shadowCutterBuff;
                        break;

                    case sparkleArrowID:
                        modif = sparkleArrowBuff;
                        break;

                    case morningStarID:
                        modif = morningStarBuff;
                        break;
                }

                emulator.Memory.SecondaryCharacterTable[character].EquipAT = (ushort) (emulator.Memory.SecondaryCharacterTable[character].EquipAT + modif);
            }
        }

        private static void FieldChapter3Buffs(IEmulator emulator) {
            if (emulator.Memory.Chapter < 3) {
                var morningStar = emulator.Memory.Item[morningStarID];
                emulator.WriteUInt(morningStar.DescriptionPointer, 0xFFA0FFA0);
                return;
            }

            Emulator.Memory.IEquipment item = (Emulator.Memory.IEquipment) emulator.Memory.Item[heatBladeID];
            item.AT = (byte) (item.AT + heatBladeBuff);

            item = (Emulator.Memory.IEquipment) emulator.Memory.Item[shadowCutterID];
            item.AT = (byte) (item.AT + shadowCutterBuff);

            item = (Emulator.Memory.IEquipment) emulator.Memory.Item[sparkleArrowID];
            item.AT = (byte) (item.AT + shadowCutterBuff);

            item = (Emulator.Memory.IEquipment) emulator.Memory.Item[morningStarID];
            item.AT = (byte) (item.AT + morningStarID);
        }
    }
}
