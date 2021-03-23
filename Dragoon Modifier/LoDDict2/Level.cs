using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.LoDDict2 {
    public class Level {
        ushort _maxHP;
        byte _spd;
        byte _at;
        byte _mat;
        byte _df;
        byte _mdf;

        public ushort MaxHP { get { return _maxHP; } }
        public byte SPD { get { return _spd; } }
        public byte AT { get { return _at; } }
        public byte MAT { get { return _mat; } }
        public byte DF { get { return _df; } }
        public byte MDF { get { return _mdf; } }

        public Level (ushort maxHP, byte spd, byte at, byte mat, byte df, byte mdf) {
            _maxHP = maxHP;
            _spd = spd;
            _at = at;
            _mat = mat;
            _df = df;
            _mdf = mdf;
        }

        public Level(string maxHP, string spd, string at, string mat, string df, string mdf) {
            if (UInt16.TryParse(maxHP, out ushort uskey)) {
                _maxHP = uskey;
            }
            if (Byte.TryParse(spd, out byte bkey)) {
                _spd = bkey;
            }
            if (Byte.TryParse(at, out bkey)) {
                _at = bkey;
            }
            if (Byte.TryParse(mat, out bkey)) {
                _mat = bkey;
            }
            if (Byte.TryParse(df, out bkey)) {
                _df = bkey;
            }
            if (Byte.TryParse(mdf, out bkey)) {
                _mdf = bkey;
            }
        }
    }
}
