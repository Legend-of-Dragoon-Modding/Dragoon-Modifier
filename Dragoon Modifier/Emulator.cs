using Memory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dragoon_Modifier {
    public class Emulator : Mem {

        public void WriteMem(long address, string type, string write, string file = "") {
            WriteMemory("0x" + (address + Constants.OFFSET).ToString("x8"), type, write, file = "");
        }

        public void WriteByte(long address, int write, string file = "") {
            WriteMemory("0x" + (address + Constants.OFFSET).ToString("x8"), "byte", write.ToString("X"), file = "");
        }

        public void WriteShort(long address, ushort write, string file = "") {
            WriteMemory("0x" + (address + Constants.OFFSET).ToString("x8"), "2bytes", write.ToString(), file = "");
        }

        public void WriteInteger(long address, int write, string file = "") {
            WriteMemory("0x" + (address + Constants.OFFSET).ToString("x8"), "int", write.ToString(), file = "");
        }

        public void WriteAOB(long address, string aob) {
            String[] aobArray = aob.Split(' ');
            for (int i = 0; i < aobArray.Length; i++) {
                WriteByte(address + i, Convert.ToByte(aobArray[i], 16));
            }
        }

        public byte ReadByte(long address, string file = "") {
            return (byte) ReadByte("0x" + (address + Constants.OFFSET).ToString("x8"), file);
        }

        public ushort ReadShort(long address, string file = "") {
            return (ushort) Read2Byte("0x" + (address + Constants.OFFSET).ToString("x8"), file);
        }

        public int ReadInteger(long address, string file = "") {
            return ReadInt("0x" + (address + Constants.OFFSET).ToString("x8"), file);
        }

        public long ScanAOB(string search) {
            long value = 0;
            var scan = AoBScan(Constants.OFFSET, Constants.OFFSET + 0x2A865F, search, true, true);
            scan.Wait();
            var results = scan.Result;
            foreach (var x in results)
                value = x;
            return value;
        }

        public long ScanAOB(string search, long startOffset, long endOffset) {
            long value = 0;
            var scan = AoBScan(Constants.OFFSET + startOffset, Constants.OFFSET + endOffset, search, true, true);
            scan.Wait();
            var results = scan.Result;
            foreach (var x in results)
                value = x;
            return value;
        }

        public ArrayList ScanAllAOB(string search) {
            ArrayList values = new ArrayList();
            var scan = AoBScan(Constants.OFFSET, Constants.OFFSET + 0x2A865F, search, true, true);
            scan.Wait();
            var results = scan.Result;
            foreach (var x in results)
                values.Add(x);
            return values;
        }

        public ArrayList ScanAllAOB(string search, long startOffset, long endOffset) {
            ArrayList values = new ArrayList();
            var scan = AoBScan(Constants.OFFSET + startOffset, Constants.OFFSET + endOffset, search, true, true);
            scan.Wait();
            var results = scan.Result;
            foreach (var x in results)
                values.Add(x);
            return values;
        }

        public bool WriteMemU(long address, string type, string write, string file = "") {
            return WriteMemory("0x" + address.ToString("x8"), type, write, file = "");
        }

        public bool WriteByteU(long address, byte write, string file = "") {
            return WriteMemory("0x" + address.ToString("x8"), "byte", write.ToString("X"), file = "");
        }

        public bool WriteShortU(long address, ushort write, string file = "") {
            return WriteMemory("0x" + address.ToString("x8"), "2bytes", write.ToString(), file = "");
        }

        public bool WriteIntegerU(long address, int write, string file = "") {
            return WriteMemory("0x" + address.ToString("x8"), "int", write.ToString(), file = "");
        }

        public byte ReadByteU(long address, string file = "") {
            return (byte) ReadByte("0x" + address.ToString("x8"), file);
        }

        public ushort ReadShortU(long address, string file = "") {
            return (ushort) Read2Byte("0x" + address.ToString("x8"), file);
        }

        public int ReadIntegerU(long address, string file = "") {
            return ReadInt("0x" + address.ToString("x8"), file);
        }

        public string ReadName(int nameAddress) {
            string name = "";
            for (int i = 0; i <= 20; i++) {
                if (GetCharacterByNumber(nameAddress + (i * 2)) == "")
                    break;
                name += GetCharacterByNumber(nameAddress + (i * 2));
            }
            return name;
        }

        public string GetCharacterByNumber(int charAddress) {
            byte character = ReadByte(charAddress);
            if (character == 0) {
                return " ";
            } else if (character == 1) {
                return ",";
            } else if (character == 2) {
                return ".";
            } else if (character == 3) {
                return "-";
            } else if (character == 4) {
                return ":";
            } else if (character == 5) {
                return "?";
            } else if (character == 6) {
                return "!";
            } else if (character == 7) {
                return "_";
            } else if (character == 8) {
                return "/";
            } else if (character == 9) {
                return "'";
            } else if (character == 10) {
                return "\"";
            } else if (character == 11) {
                return "(";
            } else if (character == 12) {
                return ")";
            } else if (character == 13) {
                return "—";
            } else if (character == 14) {
                return "@";
            } else if (character == 15) {
                return "%";
            } else if (character == 16) {
                return "&";
            } else if (character == 17) {
                return "*";
            } else if (character == 18) {
                return "#";
            } else if (character == 19) {
                return ">";
            } else if (character == 20) {
                return ",";
            } else if (character == 21) {
                return "0";
            } else if (character == 22) {
                return "1";
            } else if (character == 23) {
                return "2";
            } else if (character == 24) {
                return "3";
            } else if (character == 25) {
                return "4";
            } else if (character == 26) {
                return "5";
            } else if (character == 27) {
                return "6";
            } else if (character == 28) {
                return "7";
            } else if (character == 29) {
                return "8";
            } else if (character == 30) {
                return "9";
            } else if (character == 31) {
                return "A";
            } else if (character == 32) {
                return "B";
            } else if (character == 33) {
                return "C";
            } else if (character == 34) {
                return "D";
            } else if (character == 35) {
                return "E";
            } else if (character == 36) {
                return "F";
            } else if (character == 37) {
                return "G";
            } else if (character == 38) {
                return "H";
            } else if (character == 39) {
                return "I";
            } else if (character == 40) {
                return "J";
            } else if (character == 41) {
                return "K";
            } else if (character == 42) {
                return "L";
            } else if (character == 43) {
                return "M";
            } else if (character == 44) {
                return "N";
            } else if (character == 45) {
                return "O";
            } else if (character == 46) {
                return "P";
            } else if (character == 47) {
                return "Q";
            } else if (character == 48) {
                return "R";
            } else if (character == 49) {
                return "S";
            } else if (character == 50) {
                return "T";
            } else if (character == 51) {
                return "U";
            } else if (character == 52) {
                return "V";
            } else if (character == 53) {
                return "W";
            } else if (character == 54) {
                return "X";
            } else if (character == 55) {
                return "Y";
            } else if (character == 56) {
                return "Z";
            } else if (character == 57) {
                return "a";
            } else if (character == 58) {
                return "b";
            } else if (character == 59) {
                return "c";
            } else if (character == 60) {
                return "d";
            } else if (character == 61) {
                return "e";
            } else if (character == 62) {
                return "f";
            } else if (character == 63) {
                return "g";
            } else if (character == 64) {
                return "h";
            } else if (character == 65) {
                return "i";
            } else if (character == 66) {
                return "j";
            } else if (character == 67) {
                return "k";
            } else if (character == 68) {
                return "l";
            } else if (character == 69) {
                return "m";
            } else if (character == 70) {
                return "n";
            } else if (character == 71) {
                return "o";
            } else if (character == 72) {
                return "p";
            } else if (character == 73) {
                return "q";
            } else if (character == 74) {
                return "r";
            } else if (character == 75) {
                return "s";
            } else if (character == 76) {
                return "t";
            } else if (character == 77) {
                return "u";
            } else if (character == 78) {
                return "v";
            } else if (character == 79) {
                return "w";
            } else if (character == 80) {
                return "x";
            } else if (character == 81) {
                return "y";
            } else if (character == 82) {
                return "z";
            } else if (character == 83) {
                return "[";
            } else if (character == 84) {
                return "]";
            } else {
                return "";
            }
        }

        public byte GetCharacterByChar(char character) {
            if (character == ' ') {
                return 0;
            } else if (character == ',') {
                return 1;
            } else if (character == '.') {
                return 2;
            } else if (character == '-') {
                return 3;
            } else if (character == ':') {
                return 4;
            } else if (character == '?') {
                return 5;
            } else if (character == '!') {
                return 6;
            } else if (character == '_') {
                return 7;
            } else if (character == '/') {
                return 8;
            } else if (character == '\'') {
                return 9;
            } else if (character == '"') {
                return 10;
            } else if (character == '(') {
                return 11;
            } else if (character == ')') {
                return 12;
            } else if (character == '—') {
                return 13;
            } else if (character == '@') {
                return 14;
            } else if (character == '%') {
                return 15;
            } else if (character == '&') {
                return 16;
            } else if (character == '*') {
                return 17;
            } else if (character == '#') {
                return 18;
            } else if (character == '>') {
                return 19;
            } else if (character == '<') {
                return 20;
            } else if (character == '0') {
                return 21;
            } else if (character == '1') {
                return 22;
            } else if (character == '2') {
                return 23;
            } else if (character == '3') {
                return 24;
            } else if (character == '4') {
                return 25;
            } else if (character == '5') {
                return 26;
            } else if (character == '6') {
                return 27;
            } else if (character == '7') {
                return 28;
            } else if (character == '8') {
                return 29;
            } else if (character == '9') {
                return 30;
            } else if (character == 'A') {
                return 31;
            } else if (character == 'B') {
                return 32;
            } else if (character == 'C') {
                return 33;
            } else if (character == 'D') {
                return 34;
            } else if (character == 'E') {
                return 35;
            } else if (character == 'F') {
                return 36;
            } else if (character == 'G') {
                return 37;
            } else if (character == 'H') {
                return 38;
            } else if (character == 'I') {
                return 39;
            } else if (character == 'J') {
                return 40;
            } else if (character == 'K') {
                return 41;
            } else if (character == 'L') {
                return 42;
            } else if (character == 'M') {
                return 43;
            } else if (character == 'N') {
                return 44;
            } else if (character == 'O') {
                return 45;
            } else if (character == 'P') {
                return 46;
            } else if (character == 'Q') {
                return 47;
            } else if (character == 'R') {
                return 48;
            } else if (character == 'S') {
                return 49;
            } else if (character == 'T') {
                return 50;
            } else if (character == 'U') {
                return 51;
            } else if (character == 'V') {
                return 52;
            } else if (character == 'W') {
                return 53;
            } else if (character == 'X') {
                return 54;
            } else if (character == 'Y') {
                return 55;
            } else if (character == 'Z') {
                return 56;
            } else if (character == 'a') {
                return 57;
            } else if (character == 'b') {
                return 58;
            } else if (character == 'c') {
                return 59;
            } else if (character == 'd') {
                return 60;
            } else if (character == 'e') {
                return 61;
            } else if (character == 'f') {
                return 62;
            } else if (character == 'g') {
                return 63;
            } else if (character == 'h') {
                return 64;
            } else if (character == 'i') {
                return 65;
            } else if (character == 'j') {
                return 66;
            } else if (character == 'k') {
                return 67;
            } else if (character == 'l') {
                return 68;
            } else if (character == 'm') {
                return 69;
            } else if (character == 'n') {
                return 70;
            } else if (character == 'o') {
                return 71;
            } else if (character == 'p') {
                return 72;
            } else if (character == 'q') {
                return 73;
            } else if (character == 'r') {
                return 74;
            } else if (character == 's') {
                return 75;
            } else if (character == 't') {
                return 76;
            } else if (character == 'u') {
                return 77;
            } else if (character == 'v') {
                return 78;
            } else if (character == 'w') {
                return 79;
            } else if (character == 'x') {
                return 80;
            } else if (character == 'y') {
                return 81;
            } else if (character == 'z') {
                return 82;
            } else if (character == '[') {
                return 83;
            } else if (character == ']') {
                return 84;
            } else {
                return 0;
            }
        }
    }
}
