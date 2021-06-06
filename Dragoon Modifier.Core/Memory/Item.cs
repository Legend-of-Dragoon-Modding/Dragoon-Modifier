using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Core.Memory {
    public class Item {
        protected int _baseAddress;
        protected int _namePointerAddress;
        protected int _descriptionPointerAddress;
        protected int _sellPriceAddress;

        public byte Icon { get { return Emulator.ReadByte(_baseAddress + 0x7); } set { Emulator.WriteByte(_baseAddress + 0x7, value); } }
        public uint NamePointer { get { return Emulator.ReadUInt24(_namePointerAddress); } set { Emulator.WriteUInt24(_namePointerAddress, value); } }
        public uint DescriptionPointer { get { return Emulator.ReadUInt24(_descriptionPointerAddress); } set { Emulator.WriteUInt24(_descriptionPointerAddress, value); } }
        public ushort SellPrice { get { return Emulator.ReadUShort(_sellPriceAddress); } set { Emulator.WriteUShort(_sellPriceAddress, value); } }
    }
}
