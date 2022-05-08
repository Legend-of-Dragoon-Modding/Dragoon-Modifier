using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Core.Memory {
    public class FieldPosition {
        private int _baseAddress;

        public int X { get { return Emulator.DirectAccess.ReadInt(GetPointer() + 0x12C); } set { Emulator.DirectAccess.WriteInt(GetPointer() + 0x12C, value); } }
        public int Y { get { return Emulator.DirectAccess.ReadInt(GetPointer() + 0x134); } set { Emulator.DirectAccess.WriteInt(GetPointer() + 0x134, value); } }
        public int Z { get { return Emulator.DirectAccess.ReadInt(GetPointer() + 0x130); } set { Emulator.DirectAccess.WriteInt(GetPointer() + 0x130, value); } }


        private uint GetPointer() {
            return Emulator.DirectAccess.ReadUInt24(_baseAddress);
        }

        internal FieldPosition() {
            _baseAddress = Emulator.GetAddress("FIELD_POS_PTR");
        }
    }
}
