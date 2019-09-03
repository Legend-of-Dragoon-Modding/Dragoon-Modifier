using Memory;
using System;
using System.Collections;
using System.Threading.Tasks;

namespace Dragoon_Modifier {
    public class Emulator : Mem {

        public void WriteMem(long address, string type, string write, string file = "") {
            writeMemory("0x" + (address + Constants.OFFSET).ToString("x8"), type, write, file = "");
        }

        public void WriteByte(long address, byte write, string file = "") {
            writeMemory("0x" + (address + Constants.OFFSET).ToString("x8"), "byte", write.ToString(), file = "");
        }

        public void WriteShort(long address, ushort write, string file = "") {
            writeMemory("0x" + (address + Constants.OFFSET).ToString("x8"), "2bytes", write.ToString(), file = "");
        }

        public void WriteInteger(long address, int write, string file = "") {
            writeMemory("0x" + (address + Constants.OFFSET).ToString("x8"), "int", write.ToString(), file = "");
        }

        public byte ReadByte(long address, string file = "") {
            return (byte) readByte("0x" + (address + Constants.OFFSET).ToString("x8"), file);
        }

        public ushort ReadShort(long address, string file = "") {
            return (ushort)read2Byte("0x" + (address + Constants.OFFSET).ToString("x8"), file);
        }

        public int ReadInteger(long address, string file = "") {
            return readInt("0x" + (address + Constants.OFFSET).ToString("x8"), file);
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
            return writeMemory("0x" + address.ToString("x8"), type, write, file = "");
        }

        public bool WriteByteU(long address, byte write, string file = "") {
            return writeMemory("0x" + address.ToString("x8"), "byte", write.ToString(), file = "");
        }

        public bool WriteShortU(long address, ushort write, string file = "") {
            return writeMemory("0x" + address.ToString("x8"), "2bytes", write.ToString(), file = "");
        }

        public bool WriteIntegerU(long address, int write, string file = "") {
            return writeMemory("0x" + address.ToString("x8"), "int", write.ToString(), file = "");
        }

        public byte ReadByteU(long address, string file = "") {
            return (byte) readByte("0x" + address.ToString("x8"), file);
        }

        public ushort ReadShortU(long address, string file = "") {
            return (ushort) read2Byte("0x" + address.ToString("x8"), file);
        }

        public int ReadIntegerU(long address, string file = "") {
            return readInt("0x" + address.ToString("x8"), file);
        }
    }
}
