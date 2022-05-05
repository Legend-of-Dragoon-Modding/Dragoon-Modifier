using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Dataset {
    internal class BaseStats : IBaseStats {
        public ushort[] HP { get; private set; } = new ushort[61];
        public byte[] AT { get; private set; } = new byte[61];
        public byte[] MAT { get; private set; } = new byte[61];
        public byte[] DF { get; private set; } = new byte[61];
        public byte[] MDF { get; private set; } = new byte[61];
        public byte[] SPD { get; private set; } = new byte[61];

        internal BaseStats(string path) {
            path += "\\BaseStats.tsv";
            HP[0] = 0;
            AT[0] = 0;
            MAT[0] = 0;
            DF[0] = 0;
            MDF[0] = 0;
            SPD[0] = 0;
            try {
                using (var itemData = new StreamReader(path)) {
                    itemData.ReadLine(); // Skip first line
                    int index = 1;
                    while (!itemData.EndOfStream && index < 62) {
                        var line = itemData.ReadLine();
                        var values = line.Split('\t').ToArray();

                        if (UInt16.TryParse(values[0], out var uskey)) {
                            HP[index] = uskey;
                        } else {
                            // error
                        }

                        if (Byte.TryParse(values[1], out var bkey)) {
                            AT[index] = bkey;
                        } else {
                            // error
                        }

                        if (Byte.TryParse(values[2], out bkey)) {
                            MAT[index] = bkey;
                        } else {
                            // error
                        }

                        if (Byte.TryParse(values[3], out bkey)) {
                            DF[index] = bkey;
                        } else {
                            // error
                        }

                        if (Byte.TryParse(values[4], out bkey)) {
                            MDF[index] = bkey;
                        } else {
                            // error
                        }

                        if (Byte.TryParse(values[5], out bkey)) {
                            SPD[index] = bkey;
                        } else {
                            // error
                        }

                        index++;
                    }
                }
            } catch (FileNotFoundException) {
            Console.WriteLine($"[ERROR] File {path} not found.");
            }
        }
    }
}
