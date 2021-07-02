using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dragoon_Modifier.Core;

namespace Dragoon_Modifier.MemoryController {
    public class DragoonLevel {
        int _baseAddress;

        public ushort MaxMP { get { return Core.Emulator.ReadUShort(_baseAddress); } set { Core.Emulator.WriteUShort(_baseAddress, value); } }
        public byte SpellLearned { get { return Core.Emulator.ReadByte(_baseAddress + 0x2); } set { Core.Emulator.WriteByte(_baseAddress + 0x2, value); } }
        public byte DAT { get { return Core.Emulator.ReadByte(_baseAddress + 0x4); } set { Core.Emulator.WriteByte(_baseAddress + 0x4, value); } }
        public byte DMAT { get { return Core.Emulator.ReadByte(_baseAddress + 0x5); } set { Core.Emulator.WriteByte(_baseAddress + 0x5, value); } }
        public byte DDF { get { return Core.Emulator.ReadByte(_baseAddress + 0x6); } set { Core.Emulator.WriteByte(_baseAddress + 0x6, value); } }
        public byte DMDF { get { return Core.Emulator.ReadByte(_baseAddress + 0x7); } set { Core.Emulator.WriteByte(_baseAddress + 0x7, value); } }

        public DragoonLevel(int baseAddress, int level) {
            _baseAddress = baseAddress + level * 0x8;
        }
    }
}
