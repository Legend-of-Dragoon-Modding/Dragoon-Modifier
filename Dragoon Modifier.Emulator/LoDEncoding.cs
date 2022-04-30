using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Emulator {
    internal class LoDEncoding : ILoDEncoding {
        private static readonly Regex _regex = new Regex(@"(<[\s\S]+?>)", RegexOptions.Compiled);
        private static readonly Dictionary<char, ushort> NTA = new Dictionary<char, ushort>() {
            {' ', 0x00 },
            {',', 0x01 },
            {'.', 0x02 },
            {'·', 0x03 },
            {':', 0x04 },
            {'?', 0x05 },
            {'!', 0x06 },
            {'_', 0x07 },
            {'/', 0x08 },
            {'\'', 0x09 },
            {'"', 0x0A },
            {'(', 0x0B },
            {')', 0x0C },
            {'-', 0x0D },
            {'`', 0x0E },
            {'%', 0x0F },
            {'&', 0x10 },
            {'*', 0x11 },
            {'@', 0x12 },
            {'+', 0x13 },
            {'~', 0x14 },
            {'0', 0x15 },
            {'1', 0x16 },
            {'2', 0x17 },
            {'3', 0x18 },
            {'4', 0x19 },
            {'5', 0x1A },
            {'6', 0x1B },
            {'7', 0x1C },
            {'8', 0x1D },
            {'9', 0x1E },
            {'A', 0x1F },
            {'B', 0x20 },
            {'C', 0x21 },
            {'D', 0x22 },
            {'E', 0x23 },
            {'F', 0x24 },
            {'G', 0x25 },
            {'H', 0x26 },
            {'I', 0x27 },
            {'J', 0x28 },
            {'K', 0x29 },
            {'L', 0x2A },
            {'M', 0x2B },
            {'N', 0x2C },
            {'O', 0x2D },
            {'P', 0x2E },
            {'Q', 0x2F },
            {'R', 0x30 },
            {'S', 0x31 },
            {'T', 0x32 },
            {'U', 0x33 },
            {'V', 0x34 },
            {'W', 0x35 },
            {'X', 0x36 },
            {'Y', 0x37 },
            {'Z', 0x38 },
            {'a', 0x39 },
            {'b', 0x3A },
            {'c', 0x3B },
            {'d', 0x3C },
            {'e', 0x3D },
            {'f', 0x3E },
            {'g', 0x3F },
            {'h', 0x40 },
            {'i', 0x41 },
            {'j', 0x42 },
            {'k', 0x43 },
            {'l', 0x44 },
            {'m', 0x45 },
            {'n', 0x46 },
            {'o', 0x47 },
            {'p', 0x48 },
            {'q', 0x49 },
            {'r', 0x4A },
            {'s', 0x4B },
            {'t', 0x4C },
            {'u', 0x4D },
            {'v', 0x4E },
            {'w', 0x4F },
            {'x', 0x50 },
            {'y', 0x51 },
            {'z', 0x52 },
            {'[', 0x53 },
            {']', 0x54 }


        };
        private static readonly Dictionary<string, ushort> _textCodes = new Dictionary<string, ushort>() {
            { "<END>", 0xA0FF },
            { "<LINE>", 0xA1FF },
            { "<GOLD>", 0x00A8 },
            { "<WHITE>", 0x00A7 },
            { "<RED>", 0xA705 },
            { "<YELLOW>", 0xA708 }
        };
        private readonly Dictionary<ushort, string> _textCodesReversed;
        private static readonly byte[] _empty = new byte[] { 0x0, 0x0 };
        private readonly Dictionary<char, ushort> _char2ushort;
        private readonly Dictionary<ushort, char> _ushort2char;

        internal LoDEncoding(Dictionary<char, ushort> char2ushort) {
            _char2ushort = char2ushort;
            _ushort2char = _char2ushort.ToDictionary(x => x.Value, x => x.Key);
            _textCodesReversed = _textCodes.ToDictionary(x => x.Value, x => x.Key);
        }

        internal LoDEncoding(Region region) {
            switch (region) {
                case Region.NTA:
                    _char2ushort = NTA;
                    break;
            }
            _ushort2char = _char2ushort.ToDictionary(x => x.Value, x => x.Key);
            _textCodesReversed = _textCodes.ToDictionary(x => x.Value, x => x.Key);
        }

        public byte[] GetBytes(string text) {
            var result = new List<byte>();
            foreach (var segment in SplitOnCodes(text)) {
                if (segment.StartsWith("<")) {
                    if (TryEncodeCode(segment, out var key)) {
                        result.AddRange(key);
                    }
               
                } else {
                    foreach (char c in segment) {
                        if (TryEncodeChar(c, out var key)) {
                            result.AddRange(key);
                        }
                    }
                }
            }
            return result.ToArray();
        }

        private static string[] SplitOnCodes(string text) {
            return _regex.Split(text).Where(l => l != string.Empty).ToArray();
        }


        private bool TryEncodeCode(string code, out byte[] encoded) {
            if (_textCodes.TryGetValue(code, out var key)) {
                encoded = BitConverter.GetBytes(key);
                return true;
            }

            Console.WriteLine($"[ERROR] Code {code} couldn't be encoded.");
            encoded = _empty;
            return false;
        }

        private bool TryEncodeChar(char character, out byte[] encoded) {
            if (_char2ushort.TryGetValue(character, out var key)) {
                encoded = BitConverter.GetBytes(key);
                return true;
            }

            Console.WriteLine($"[ERROR] Character {character} couldn't be encoded.");
            encoded = _empty;
            return false;
        }

        public string GetString(byte[] bytes) {
            string result = String.Empty;
            for (int i = 0; i < bytes.Length / 2; i++) {
                var val = BitConverter.ToUInt16(bytes, i * 2);
                if (_ushort2char.TryGetValue(val, out var key)) {
                    result += key;
                    continue;
                }

                if (_textCodesReversed.TryGetValue(val, out var stringKey)) {
                    result += stringKey;
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
