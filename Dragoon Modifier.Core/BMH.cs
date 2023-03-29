using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Core {
    public static class BMH {
        public static List<long> UnmaskedSearch(byte[] pattern, byte[] array, bool findAll = false) {
            List<long> indexList = new();

            var arrayLenght = array.Length;
            var patternLenght = pattern.Length;

            Dictionary<byte, int> badMatchTable = new();

            for (int i = 0; i < patternLenght - 1; i++) {
                badMatchTable[pattern[i]] = patternLenght - i - 1;
            }

            int index = 0;

            while (index <= arrayLenght - patternLenght) {
                int matchIndex = patternLenght - 1;

                while (true) {
                    if (array[index + matchIndex] == pattern[matchIndex]) {
                        matchIndex--;
                    } else {
                        break;
                    }

                    if (matchIndex <= 0) {
                        indexList.Add(index);

                        if (findAll == false) {
                            return indexList;
                        }
                        break;
                    }
                }

                if (badMatchTable.TryGetValue(array[index + patternLenght - 1], out int shiftAmmount)) {
                    index += shiftAmmount;
                } else {
                    index += patternLenght;
                }
            }

            return indexList;
        }
    }
}
