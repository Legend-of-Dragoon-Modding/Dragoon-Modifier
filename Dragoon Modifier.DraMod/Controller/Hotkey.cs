using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Controller {
    abstract internal class Hotkey {
        internal const ushort L2 = 1;
        internal const ushort R2 = 2;
        internal const ushort L1 = 4;
        internal const ushort R1 = 8;
        internal const ushort Triangle = 16;
        internal const ushort Cross = 32;
        internal const ushort Circle = 64;
        internal const ushort Square = 128;
        internal const ushort Select = 256;
        internal const ushort L3 = 512;
        internal const ushort R3 = 1024;
        internal const ushort Start = 2048;
        internal const ushort Up = 4096;
        internal const ushort Right = 8192;
        internal const ushort Down = 16384;
        internal const ushort Left = 32768;

        public readonly ushort KeyPress;

        internal Hotkey(ushort keyPress) {
            this.KeyPress = keyPress;
        }

        abstract public void Run(Emulator.IEmulator emulator);
    }

    

}
