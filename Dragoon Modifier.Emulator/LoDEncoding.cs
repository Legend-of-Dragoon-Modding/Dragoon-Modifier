using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Emulator {
    internal class LoDEncoding : ILoDEncoding {
        private static readonly Dictionary<char, ushort> NTA = new Dictionary<char, ushort>() {
            { ' ', 0x0 },
            { ',', 0x1 },
            { '.', 0x2 },
            { '-', 0x3 },
            { ':', 0x4 },
            { '?', 0x5 },
            { ':', 0x6 },
            { '_', 0x7 },
            { '/', 0x8 },
            { '\'', 0x9 },
            { '"', 0xA },
            { '(', 0xB },
            { ')', 0xC },
            { '—', 0xD },
            { '@', 0xE },
            { '%', 0xF },
            { '&', 0x10 },
            { '*', 0x11 },
            { '#', 0x12 },
            { '+', 0x13 },
            { '~', 0x14 },
            { '0', 0x15 },
            { '1', 0x16 },
            { '2', 0x17 },
            { '3', 0x18 },
            { '4', 0x19 },
            { '5', 0x1A },
            { '6', 0x1B },
            { '7', 0x1C },
            { '8', 0x1D },
            { '9', 0x1E },
            { 'A', 0x1F },
            { 'B', 0x20 },
            { 'C', 0x21 },
            { 'D', 0x22 },
            { 'E', 0x23 },
            { 'F', 0x24 },
            { 'G', 0x25 },
            { 'H', 0x26 },
            { 'I', 0x27 },
            { 'J', 0x28 },
            { 'K', 0x29 },
            { 'L', 0x2A },
            { 'M', 0x2B },
            { 'N', 0x2C },
            { 'O', 0x2D },
            { 'P', 0x2E },
            { 'Q', 0x2F },
            { 'R', 0x30 },
            { 'S', 0x31 },
            { 'T', 0x32 },
            { 'U', 0x33 },
            { 'V', 0x34 },
            { 'W', 0x35 },
            { 'X', 0x36 },
            { 'Y', 0x37 },
            { 'Z', 0x38 },
            { 'a', 0x39 },
            { 'b', 0x3A },
            { 'c', 0x3B },
            { 'd', 0x3C },
            { 'e', 0x3D },
            { 'f', 0x3E },
            { 'g', 0x3F },
            { 'h', 0x40 },
            { 'i', 0x41 },
            { 'j', 0x42 },
            { 'k', 0x43 },
            { 'l', 0x44 },
            { 'm', 0x45 },
            { 'n', 0x46 },
            { 'o', 0x47 },
            { 'p', 0x48 },
            { 'q', 0x49 },
            { 'r', 0x4A },
            { 's', 0x4B },
            { 't', 0x4C },
            { 'u', 0x4D },
            { 'v', 0x4E },
            { 'w', 0x4F },
            { 'x', 0x50 },
            { 'y', 0x51 },
            { 'z', 0x52 },
            { '[', 0x53 },
            { ']', 0x54 }


        };
        private static readonly byte[] empty = new byte[] { 0x0, 0x0 };
        private readonly Dictionary<char, ushort> _char2ushort;
        private readonly Dictionary<ushort, char> _ushort2char;

        internal LoDEncoding(Dictionary<char, ushort> char2ushort) {
            _char2ushort = char2ushort;
            _ushort2char = _char2ushort.ToDictionary(x => x.Value, x => x.Key);
        }

        internal LoDEncoding(Region region) {
            switch (region) {
                case Region.NTA:
                    _char2ushort = NTA;
                    break;
            }
            _ushort2char = _char2ushort.ToDictionary(x => x.Value, x => x.Key);
        }

        public byte[] GetBytes(string text) {
            var result = new List<byte>();
            foreach (char c in text) {
                if (_char2ushort.TryGetValue(c, out var key)) {
                    result.AddRange(BitConverter.GetBytes(key));
                    continue;
                }
                result.AddRange(empty);
            }
            return result.ToArray();
        }

        public string GetString(byte[] bytes) {
            string result = String.Empty;
            for (int i = 0; i < bytes.Length / 2; i++) {
                if (_ushort2char.TryGetValue(BitConverter.ToUInt16(bytes, i * 2), out var key)) {
                    result += key;
                    continue;
                }
                result += string.Empty;
            }
            return result;
        }

        public char GetChar(ushort value) {
            if (_ushort2char.TryGetValue(value, out var key)) {
                return key;
            }
            return ' ';
        }
    }
}
