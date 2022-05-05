using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Dataset {
    public class Addition {
        public const ushort EndFlag = 0x4;
        public const ushort RegularAddition = 0xC0;
        public const ushort MasterAddition = 0xE0;
        public static readonly AdditionHit EmptyAddition = new AdditionHit();
        public static readonly byte[][] AdditionIDs = new byte[][] {
            new byte[] { 0, 1, 2, 3, 4, 5, 6 },
            new byte[] { 8, 9, 10, 11, 12 },
            new byte[] { 255 },
            new byte[] { 14, 15, 16, 17 },
            new byte[] { 29, 30, 31, 32, 33, 34 },
            new byte[] { 8, 9, 10, 11, 12 },
            new byte[] { 23, 24, 25, 26, 27 },
            new byte[] { 19, 20, 21 },
            new byte[] { 255 }
        };

        public readonly byte ID = 0;
        public readonly byte Level = 0;
        public readonly byte[] DamageIncrease = new byte[5] { 0, 0, 0, 0, 0 };
        public readonly byte[] SPIncrease = new byte[5] { 0, 0, 0, 0, 0 };
        public readonly byte StartTime = 0;

        public readonly List<AdditionHit> AdditionHit = new List<AdditionHit>();

        internal Addition(string filePath) {
            using (var file = new StreamReader(filePath)) {
                var line = file.ReadLine().Split('\t').ToArray();

                if (Byte.TryParse(line[1], out var bkey)) {
                    ID = bkey;
                }

                line = file.ReadLine().Split('\t').ToArray();


                if (line[1] == "Final") {
                    Level = 255;
                } else if (Byte.TryParse(line[1], out bkey)) {
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
