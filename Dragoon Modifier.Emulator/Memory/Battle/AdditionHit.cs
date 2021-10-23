using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Emulator.Memory.Battle {
    public class AdditionHit {
        private readonly int _baseAddress;

        private readonly IEmulator _emulator;

        public ushort MasterAddition { get { return _emulator.ReadUShort(_baseAddress); } set { _emulator.WriteUShort(_baseAddress, value); } }
        public ushort NextHit { get { return _emulator.ReadUShort(_baseAddress + 0x2); } set { _emulator.WriteUShort(_baseAddress + 0x2, value); } }
        public ushort BlueSquare { get { return _emulator.ReadUShort(_baseAddress + 0x4); } set { _emulator.WriteUShort(_baseAddress + 0x4, value); } }
        public ushort GrayHit { get { return _emulator.ReadUShort(_baseAddress + 0x6); } set { _emulator.WriteUShort(_baseAddress + 0x6, value); } }
        public ushort Damage { get { return _emulator.ReadUShort(_baseAddress + 0x8); } set { _emulator.WriteUShort(_baseAddress + 0x8, value); } }
        public ushort SP { get { return _emulator.ReadUShort(_baseAddress + 0xA); } set { _emulator.WriteUShort(_baseAddress + 0xA, value); } }
        public ushort ID { get { return _emulator.ReadUShort(_baseAddress + 0xC); } set { _emulator.WriteUShort(_baseAddress + 0xC, value); } }
        public ushort FinalHit { get { return _emulator.ReadUShort(_baseAddress + 0xE); } set { _emulator.WriteUShort(_baseAddress + 0xE, value); } }
        public ushort PanCameraDistance { get { return _emulator.ReadUShort(_baseAddress + 0x10); } set { _emulator.WriteUShort(_baseAddress + 0x10, value); } }
        public ushort LockOnCameraDistance1 { get { return _emulator.ReadUShort(_baseAddress + 0x12); } set { _emulator.WriteUShort(_baseAddress + 0x12, value); } }
        public ushort LockOnCameraDistance2 { get { return _emulator.ReadUShort(_baseAddress + 0x14); } set { _emulator.WriteUShort(_baseAddress + 0x14, value); } }
        public ushort MonsterDistance { get { return _emulator.ReadUShort(_baseAddress + 0x16); } set { _emulator.WriteUShort(_baseAddress + 0x16, value); } }
        public ushort VerticalDistance { get { return _emulator.ReadUShort(_baseAddress + 0x18); } set { _emulator.WriteUShort(_baseAddress + 0x18, value); } }
        public ushort Unknown1 { get { return _emulator.ReadUShort(_baseAddress + 0x1A); } set { _emulator.WriteUShort(_baseAddress + 0x1A, value); } }
        public ushort Unknown2 { get { return _emulator.ReadUShort(_baseAddress + 0x1C); } set { _emulator.WriteUShort(_baseAddress + 0x1C, value); } }
        public ushort StartTime { get { return _emulator.ReadUShort(_baseAddress + 0x1E); } set { _emulator.WriteUShort(_baseAddress + 0x1E, value); } }


        internal AdditionHit(IEmulator emulator, int baseAddress) {
            _emulator = emulator;
            _baseAddress = baseAddress;
        }
    }
}
