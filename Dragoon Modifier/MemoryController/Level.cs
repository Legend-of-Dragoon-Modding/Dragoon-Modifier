using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dragoon_Modifier.Core;

namespace Dragoon_Modifier.MemoryController {
    public class Level {
        int _baseAddress;

        public ushort MaxHP { get { return Emulator.ReadUShort(_baseAddress); } set { Emulator.WriteUShort(_baseAddress, value); } }
        public byte SPD { get { return Emulator.ReadByte(_baseAddress + 0x3); } set { Emulator.WriteByte(_baseAddress + 0x3, value); } }
        public byte AT { get { return Emulator.ReadByte(_baseAddress + 0x4); } set { Emulator.WriteByte(_baseAddress + 0x4, value); } }
        public byte MAT { get { return Emulator.ReadByte(_baseAddress + 0x5); } set { Emulator.WriteByte(_baseAddress + 0x5, value); } }
        public byte DF { get { return Emulator.ReadByte(_baseAddress + 0x6); } set { Emulator.WriteByte(_baseAddress + 0x6, value); } }
        public byte MDF { get { return Emulator.ReadByte(_baseAddress + 0x7); } set { Emulator.WriteByte(_baseAddress + 0x7, value); } }

        public Level(int baseAddress, int level) {
            _baseAddress = baseAddress + level * 0x8;
        }
    }
}
