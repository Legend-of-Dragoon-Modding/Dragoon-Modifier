using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.LoDDict {
    public class Addition {
        public const ushort EndFlag = 0x4;
        public const ushort RegularAddition = 0xC0;
        public const ushort MasterAddition = 0xE0;

        public readonly byte ID = 0;
        public readonly byte Level = 0;
        public readonly byte[] DamageIncrease = new byte[5];
        public readonly byte[] SPIncrease = new byte[5];
        public readonly byte StartTime = 0;

        public readonly List<AdditionHit> AdditionHit = new List<AdditionHit>();

        public Addition(string filePath) {
            using(var file = new StreamReader(filePath)) {
                var line = file.ReadLine().Split('\t').ToArray();

                if (Byte.TryParse(line[1], out var bkey)) {
                    ID = bkey;
                }

                line = file.ReadLine().Split('\t').ToArray();

                if (Byte.TryParse(line[1], out bkey)) {
                    Level = bkey;
                }

                file.ReadLine();
                file.ReadLine();

                line = file.ReadLine().Split('\t').ToArray();

                for (int level = 0; level < 5; level++) {
                    if (Byte.TryParse(line[level + 1], out bkey)) {
                        DamageIncrease[level] = bkey;
                    }
                }

                line = file.ReadLine().Split('\t').ToArray();

                for (int level = 0; level < 5; level++) {
                    if (Byte.TryParse(line[level + 1], out bkey)) {
                        SPIncrease[level] = bkey;
                    }
                }

                file.ReadLine();

                line = file.ReadLine().Split('\t').ToArray();

                if (Byte.TryParse(line[1], out bkey)) {
                    StartTime = bkey;
                }

                file.ReadLine();

                while (!file.EndOfStream) {
                    line = file.ReadLine().Split('\t').ToArray();
                    AdditionHit.Add(new AdditionHit(line));
                }
            }
        }

    }
}
