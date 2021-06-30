using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Emulator {
    internal static class KMP {
        private static readonly Dictionary<string, byte[]> _mask = new Dictionary<string, byte[]>() {
            {"??", new byte[] { 0x00, 0x00} },
            {"0?", new byte[] { 0x00, 0xF0} },
            {"1?", new byte[] { 0x10, 0xF0} },
            {"2?", new byte[] { 0x20, 0xF0} },
            {"3?", new byte[] { 0x30, 0xF0} },
            {"4?", new byte[] { 0x40, 0xF0} },
            {"5?", new byte[] { 0x50, 0xF0} },
            {"6?", new byte[] { 0x60, 0xF0} },
            {"7?", new byte[] { 0x70, 0xF0} },
            {"8?", new byte[] { 0x80, 0xF0} },
            {"9?", new byte[] { 0x90, 0xF0} },
            {"A?", new byte[] { 0xA0, 0xF0} },
            {"B?", new byte[] { 0xB0, 0xF0} },
            {"C?", new byte[] { 0xC0, 0xF0} },
            {"D?", new byte[] { 0xD0, 0xF0} },
            {"E?", new byte[] { 0xE0, 0xF0} },
            {"F?", new byte[] { 0xF0, 0xF0} },
            {"?0", new byte[] { 0x00, 0x0F} },
            {"?1", new byte[] { 0x01, 0x0F} },
            {"?2", new byte[] { 0x02, 0x0F} },
            {"?3", new byte[] { 0x03, 0x0F} },
            {"?4", new byte[] { 0x04, 0x0F} },
            {"?5", new byte[] { 0x05, 0x0F} },
            {"?6", new byte[] { 0x06, 0x0F} },
            {"?7", new byte[] { 0x07, 0x0F} },
            {"?8", new byte[] { 0x08, 0x0F} },
            {"?9", new byte[] { 0x09, 0x0F} },
            {"?A", new byte[] { 0x0A, 0x0F} },
            {"?B", new byte[] { 0x0B, 0x0F} },
            {"?C", new byte[] { 0x0C, 0x0F} },
            {"?D", new byte[] { 0x0D, 0x0F} },
            {"?E", new byte[] { 0x0E, 0x0F} },
            {"?F", new byte[] { 0x0F, 0x0F} },
        };

        internal static List<long> Search(string pattern, byte[] array, bool findAll = false) {
            var splitString = pattern.Split(' ');

            var patternValue = new byte[splitString.Length];
            var patternMask = new byte[splitString.Length];
            for (int i = 0; i < splitString.Length; i++) {
                if (Byte.TryParse(splitString[i], NumberStyles.HexNumber, null, out byte key)) {
                    patternValue[i] = key; // Can be parsed, unless theres a ? mask
                    patternMask[i] = 0xFF; // & with 0xFF doesn't change the value
                } else {
                    patternValue[i] = _mask[splitString[i]][0]; // For masked nibbles, value and mask is set to 0, so it always passes
                    patternMask[i] = _mask[splitString[i]][1];
                }
            }

            var indexList = new List<long>();

            var substringIndex = CalculateSubstringIndexes(patternValue, patternMask, patternMask.Length);

            int arrayIndex = 0;
            int patternIndex = 0;
            while (arrayIndex < array.Length - patternValue.Length + 1) {
                if ((array[arrayIndex] & patternMask[patternIndex]) == patternValue[patternIndex]) {
                    arrayIndex++;
                    patternIndex++;
                } else {
                    if (patternIndex != 0) {
                        patternIndex = substringIndex[patternIndex - 1];
                    } else {
                        arrayIndex++;
                    }
                }
                if (patternIndex == patternValue.Length) {
                    indexList.Add(arrayIndex - patternIndex);
                    if (!findAll) {
                        break;
                    }
                    patternIndex = substringIndex[patternIndex - 1];
                }
            }

            return indexList;
        }

        private static byte[] CalculateSubstringIndexes(byte[] patternValue, byte[] patternMask, int patternLength) {
            var substringIndex = new byte[patternLength];
            substringIndex[0] = 0;
            int len = 0;
            int i = 1;
            while (i < patternLength) {
                if (patternValue[i] == (patternValue[len] & patternMask[i])) {
                    substringIndex[i] = (byte) (len + 1);
                    len++;
                    i++;
                } else {
                    if (len != 0) {
                        len = substringIndex[len - 1];
                    } else {
                        substringIndex[i] = 0;
                        i++;
                    }
                }
            }
            return substringIndex;
        }

        internal static List<long> UnmaskedSearch(byte[] pattern, byte[] array, bool findAll = false) {
            var indexList = new List<long>();
            var substringIndex = CalculateUnmaskedSubstringIndexes(pattern);

            int arrayIndex = 0;
            int patternIndex = 0;
            while (arrayIndex < array.Length - pattern.Length + 1) {
                if (array[arrayIndex] == pattern[patternIndex]) {
                    arrayIndex++;
                    patternIndex++;
                } else {
                    if (patternIndex != 0) {
                        patternIndex = substringIndex[patternIndex - 1];
                    } else {
                        arrayIndex++;
                    }
                }
                if (patternIndex == pattern.Length) {
                    indexList.Add(arrayIndex - patternIndex);
                    if (!findAll) {
                        break;
                    }
                    patternIndex = substringIndex[patternIndex - 1];
                }
            }

            return indexList;
        }

        private static byte[] CalculateUnmaskedSubstringIndexes(byte[] pattern) {
            var substringIndex = new byte[pattern.Length];
            substringIndex[0] = 0;
            int len = 0;
            int i = 1;
            while (i < pattern.Length) {
                if (pattern[i] == pattern[len]) {
                    substringIndex[i] = (byte) (len + 1);
                    len++;
                    i++;
                } else {
                    if (len != 0) {
                        len = substringIndex[len - 1];
                    } else {
                        substringIndex[i] = 0;
                        i++;
                    }
                }
            }
            return substringIndex;
        }
    }
}
