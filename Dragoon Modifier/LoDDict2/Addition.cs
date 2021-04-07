using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.LoDDict2 {
    public class Addition {
        byte _level;
        byte _id;
        AdditionHit[] _hits;
        byte[] _spMultiplier = new byte[6];
        byte[] _damageMultiplier = new byte[6];

        public byte Level { get { return _level; } }
        public byte ID { get { return _id; } }
        public AdditionHit[] Hits { get { return _hits; } }
        public byte[] SPMultiplier { get { return _spMultiplier; } }
        public byte[] DamageMultiplier { get { return _damageMultiplier; } }

        public Addition(string filePath, string additionName) {
            _spMultiplier[0] = 0;
            _damageMultiplier[0] = 0;
            List<AdditionHit> hits = new List<AdditionHit>();
            using (var additionData = new StreamReader(filePath)) {
                var line = additionData.ReadLine();
                var values = line.Split('\t').ToArray();
                if (Byte.TryParse(values[1], out byte bkey)) {
                    _level = bkey;
                } else {
                    if (values[1].ToLower() == "final") {
                        _level = 0xFF;
                    } else {
                        // TODO error
                    }
                }

                line = additionData.ReadLine();
                values = line.Split('\t').ToArray();
                for (int i = 0; i < values.Length; i++) {

                    if (Byte.TryParse(values[i], out bkey)) {
                        _spMultiplier[i] = bkey;
                    } else {
                        // TODO error
                    }
                }

                line = additionData.ReadLine();
                values = line.Split('\t').ToArray();
                for (int i = 0; i < values.Length; i++) {

                    if (Byte.TryParse(values[i], out bkey)) {
                        _damageMultiplier[i] = bkey;
                    } else {
                        // TODO error
                    }
                }


                additionData.ReadLine(); // Skip a line;
                while (!additionData.EndOfStream) {
                    line = additionData.ReadLine();
                    values = line.Split('\t').ToArray();
                    hits.Add(new AdditionHit(values));
                }
                _hits = hits.ToArray();
            }
        }
    }
}
