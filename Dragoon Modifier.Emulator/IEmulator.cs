using Dragoon_Modifier.Emulator.Memory;
using Dragoon_Modifier.Emulator.Memory.Battle;

namespace Dragoon_Modifier.Emulator {
    public interface IEmulator {
        IBattle Battle { get; }
        long EmulatorOffset { get; }
        IMemory Memory { get; }
        Region Region { get; }
        ILoDEncoding LoDEncoding { get; }

        int GetAddress(string address);
        void LoadBattle();
        byte[] ReadAoB(long startAddress, long endAddress);
        byte ReadByte(long address);
        byte ReadByte(string address, int offset = 0);
        int ReadInt(long address);
        int ReadInt(string address, int offset = 0);
        long ReadLong(long address);
        long ReadLong(string address, int offset = 0);
        sbyte ReadSByte(long address);
        sbyte ReadSByte(string address, int offset = 0);
        short ReadShort(long address);
        short ReadShort(string address, int offset = 0);
        string ReadText(long startAddress);
        string ReadText(long startAddress, long endAddress);
        uint ReadUInt(long address);
        uint ReadUInt(string address, int offset = 0);
        uint ReadUInt24(long address);
        uint ReadUInt24(string address, int offset = 0);
        ulong ReadULong(long address);
        ulong ReadULong(string address, int offset = 0);
        ushort ReadUShort(long address);
        ushort ReadUShort(string address, int offset = 0);
        void WriteAoB(long startAddress, byte[] bytes);
        void WriteAoB(long startAddress, string byteString);
        void WriteByte(long address, byte value);
        void WriteByte(string address, byte value, int offset = 0);
        void WriteInt(long address, int value);
        void WriteInt(string address, int value, int offset = 0);
        void WriteLong(long address, long value);
        void WriteLong(string address, long value, int offset = 0);
        void WriteSByte(long address, sbyte value);
        void WriteSByte(string address, sbyte value, int offset = 0);
        void WriteShort(long address, short value);
        void WriteShort(string address, short value, int offset = 0);
        void WriteText(long address, string text);
        void WriteUInt(long address, uint value);
        void WriteUInt(string address, uint value, int offset = 0);
        void WriteUInt24(long address, uint value);
        void WriteUInt24(string address, uint value, int offset = 0);
        void WriteULong(long address, ulong value);
        void WriteULong(string address, ulong value, int offset = 0);
        void WriteUShort(long address, ushort value);
        void WriteUShort(string address, ushort value, int offset = 0);
    }
}