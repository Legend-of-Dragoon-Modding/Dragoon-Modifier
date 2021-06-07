using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dragoon_Modifier.Core;

namespace Dragoon_Modifier.MemoryController {
    public class DragoonLevel {
        int _baseAddress;

        public ushort MaxMP { get { return Emulator.ReadUShort(_baseAddress); } set { Emulator.WriteUShort(_baseAddress, value); } }
        public byte SpellLearned { get { return Emulator.ReadByte(_baseAddress + 0x2); } set { Emulator.WriteByte(_baseAddress + 0x2, value); } }
        public byte DAT { get { return Emulator.ReadByte(_baseAddress + 0x4); } set { Emulator.WriteByte(_baseAddress + 0x4, value); } }
        public byte DMAT { get { return Emulator.ReadByte(_baseAddress + 0x5); } set { Emulator.WriteByte(_baseAddress + 0x5, value); } }
        public byte DDF { get { return Emulator.ReadByte(_baseAddress + 0x6); } set { Emulator.WriteByte(_baseAddress + 0x6, value); } }
        public byte DMDF { get { return Emulator.ReadByte(_baseAddress + 0x7); } set { Emulator.WriteByte(_baseAddress + 0x7, value); } }

        public DragoonLevel(int baseAddress, int level) {
            _baseAddress = baseAddress + level * 0x8;
        }
    }
}
