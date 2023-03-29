using Dragoon_Modifier.Core;
using Dragoon_Modifier.DraMod.Dataset;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Controller {
    internal static class DragoonSpell {
        static double[] bases = new double[] { 800, 600, 500, 400, 300, 200, 150, 100, 50 };
        static byte[] base_table = new byte[] { 0x1, 0x2, 0x4, 0x8, 0x10, 0x20, 0x40, 0, 0x80 };
        static byte[][] Spells = new byte[9][] {
            new byte[] {0, 1, 2, 3, 4, 9 },
            new byte[]{14, 17, 26, 8 },
            new byte[]{10, 11, 12, 13 },
            new byte[]{15, 16, 18, 19 },
            new byte[]{20, 21, 22, 23 },
            new byte[]{5, 6, 7, 8 },
            new byte[]{24, 25, 27, 28 },
            new byte[]{29, 30, 31 },
            new byte[]{10, 11, 12, 13 }
        };

        internal static void WriteDragoonSpellTable(int slot, uint characterId) {
            Dictionary<int, IDragoonSpells> datasetSpell = Settings.Instance.Dataset.DragoonSpell;

            foreach (byte id in Spells[characterId]) {
                byte dmg_base = 0;
                byte multi = 0;
                Core.Memory.Battle.DragoonSpell spell = new Core.Memory.Battle.DragoonSpell(id);
                int intValue = spell.IntValue;

                if (datasetSpell[id].Percentage) {
                    intValue |= 1 << 2;
                    dmg_base = 0;
                    multi = (byte) Math.Round(datasetSpell[id].Damage);
                } else {
                    intValue &= ~(1 << 2);
                    double stat = (double) Emulator.Memory.Battle.CharacterTable[slot].DMAT;
                    double[] nearest_list = new double[9];
                    byte[] multi_list = new byte[9];

                    for (int i = 0; i < 9; i++) {
                        if (datasetSpell[id].Damage < bases[i]) {
                            multi_list[i] = 0;
                        } else if (datasetSpell[id].Damage > ((stat + 255) * bases[i]) / stat) {
                            multi_list[i] = 255;
                        } else {
                            multi_list[i] = (byte) Math.Round((datasetSpell[id].Damage * stat - bases[i] * stat) / bases[i]);
                        }
                        nearest_list[i] = Math.Abs(datasetSpell[id].Damage - (stat + multi_list[i]) * bases[i] / stat);
                    }

                    int index = Array.IndexOf(nearest_list, nearest_list.Min());
                    dmg_base = base_table[index];
                    multi = multi_list[index];

                    spell.IntValue = (byte) intValue;
                    spell.DamageBase = dmg_base;
                    spell.DamageMultiplier = multi;
                    spell.Accuracy = datasetSpell[id].Accuracy;
                    spell.MP = datasetSpell[id].MP;
                    spell.Element = datasetSpell[id].Element;
                }
            }
        }
    }
}
