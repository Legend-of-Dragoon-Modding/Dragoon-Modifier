using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Core.Memory.Battle {
    public class AdditionHit {
        private readonly int _baseAddress;

        public ushort MasterAddition { get { return Emulator.DirectAccess.ReadUShort(_baseAddress); } set { Emulator.DirectAccess.WriteUShort(_baseAddress, value); } }
        public ushort NextHit { get { return Emulator.DirectAccess.ReadUShort(_baseAddress + 0x2); } set { Emulator.DirectAccess.WriteUShort(_baseAddress + 0x2, value); } }
        public ushort BlueSquare { get { return Emulator.DirectAccess.ReadUShort(_baseAddress + 0x4); } set { Emulator.DirectAccess.WriteUShort(_baseAddress + 0x4, value); } }
        public ushort GrayHit { get { return Emulator.DirectAccess.ReadUShort(_baseAddress + 0x6); } set { Emulator.DirectAccess.WriteUShort(_baseAddress + 0x6, value); } }
        public ushort Damage { get { return Emulator.DirectAccess.ReadUShort(_baseAddress + 0x8); } set { Emulator.DirectAccess.WriteUShort(_baseAddress + 0x8, value); } }
        public ushort SP { get { return Emulator.DirectAccess.ReadUShort(_baseAddress + 0xA); } set { Emulator.DirectAccess.WriteUShort(_baseAddress + 0xA, value); } }
        public ushort ID { get { return Emulator.DirectAccess.ReadUShort(_baseAddress + 0xC); } set { Emulator.DirectAccess.WriteUShort(_baseAddress + 0xC, value); } }
        public ushort FinalHit { get { return Emulator.DirectAccess.ReadUShort(_baseAddress + 0xE); } set { Emulator.DirectAccess.WriteUShort(_baseAddress + 0xE, value); } }
        public ushort PanCameraDistance { get { return Emulator.DirectAccess.ReadUShort(_baseAddress + 0x10); } set { Emulator.DirectAccess.WriteUShort(_baseAddress + 0x10, value); } }
        public ushort LockOnCameraDistance1 { get { return Emulator.DirectAccess.ReadUShort(_baseAddress + 0x12); } set { Emulator.DirectAccess.WriteUShort(_baseAddress + 0x12, value); } }
        public ushort LockOnCameraDistance2 { get { return Emulator.DirectAccess.ReadUShort(_baseAddress + 0x14); } set { Emulator.DirectAccess.WriteUShort(_baseAddress + 0x14, value); } }
        public ushort MonsterDistance { get { return Emulator.DirectAccess.ReadUShort(_baseAddress + 0x16); } set { Emulator.DirectAccess.WriteUShort(_baseAddress + 0x16, value); } }
        public ushort VerticalDistance { get { return Emulator.DirectAccess.ReadUShort(_baseAddress + 0x18); } set { Emulator.DirectAccess.WriteUShort(_baseAddress + 0x18, value); } }
        public ushort Unknown1 { get { return Emulator.DirectAccess.ReadUShort(_baseAddress + 0x1A); } set { Emulator.DirectAccess.WriteUShort(_baseAddress + 0x1A, value); } }
        public ushort Unknown2 { get { return Emulator.DirectAccess.ReadUShort(_baseAddress + 0x1C); } set { Emulator.DirectAccess.WriteUShort(_baseAddress + 0x1C, value); } }
        public ushort StartTime { get { return Emulator.DirectAccess.ReadUShort(_baseAddress + 0x1E); } set { Emulator.DirectAccess.WriteUShort(_baseAddress + 0x1E, value); } }


        internal AdditionHit(int baseAddress) {
            _baseAddress = baseAddress;
        }
    }
}
