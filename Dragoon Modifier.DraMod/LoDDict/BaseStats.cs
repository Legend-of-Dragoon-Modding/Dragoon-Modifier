using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.LoDDict {
    internal class BaseStats {
        public ushort[] HP = new ushort[61];
        public byte[] AT = new byte[61];
        public byte[] MAT = new byte[61];
        public byte[] DF = new byte[61];
        public byte[] MDF = new byte[61];
        public byte[] SPD = new byte[61];

        internal BaseStats(string path) {
            using (var itemData = new StreamReader(path)) {
                itemData.ReadLine(); // Skip first line
                int index = 0;
                while (!itemData.EndOfStream && index < 61) {
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
        }
    }

    
}
