using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.LoDDict {
    public class AdditionHit {
        public readonly ushort Damage = 0;
        public readonly ushort SP = 0;
        public readonly byte BlueTime = 0;
        public readonly byte GrayTime = 0;
        public readonly byte CameraPanDistance = 0;
        public readonly byte LockOnCameraDistance = 0;
        public readonly byte LockOnCameraDistance2 = 0;
        public readonly byte MonsterDistance = 0;
        public readonly byte VerticaDistance = 0;
        public readonly byte Unknown = 0;
        public readonly byte Unknown2 = 0;


        /// <summary>
        /// Used to generate an empty addition hit data.
        /// </summary>
        internal AdditionHit() {

        }

        internal AdditionHit(string[] values) {
            if (UInt16.TryParse(values[1], out var uskey)) {
                Damage = uskey;
            }

            if (UInt16.TryParse(values[2], out uskey)) {
                SP = uskey;
            }

            if (Byte.TryParse(values[3], out var bkey)) {
                BlueTime = bkey;
            }

            if (Byte.TryParse(values[4], out bkey)) {
                GrayTime = bkey;
            }

            if (Byte.TryParse(values[5], out bkey)) {
                CameraPanDistance = bkey;
            }

            if (Byte.TryParse(values[6], out bkey)) {
                LockOnCameraDistance = bkey;
            }

            if (Byte.TryParse(values[7], out bkey)) {
                LockOnCameraDistance2 = bkey;
            }

            if (Byte.TryParse(values[8], out bkey)) {
                MonsterDistance = bkey;
            }

            if (Byte.TryParse(values[9], out bkey)) {
                VerticaDistance = bkey;
            }

            if (Byte.TryParse(values[10], out bkey)) {
                Unknown = bkey;
            }

            if (Byte.TryParse(values[11], out bkey)) {
                Unknown2 = bkey;
            }

        }
    }
}
