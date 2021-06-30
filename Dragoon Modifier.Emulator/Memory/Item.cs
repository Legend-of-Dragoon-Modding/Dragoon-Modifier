using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Emulator.Memory {
    public class Item {

        protected int _baseAddress;
        protected int _namePointerAddress;
        protected int _descriptionPointerAddress;
        protected int _sellPriceAddress;

        protected IEmulator _emulator;

        public virtual byte Icon { get; set; }
        public uint NamePointer { get { return _emulator.ReadUInt24(_namePointerAddress); } set { _emulator.WriteUInt24(_namePointerAddress, value); } }
        public uint DescriptionPointer { get { return _emulator.ReadUInt24(_descriptionPointerAddress); } set { _emulator.WriteUInt24(_descriptionPointerAddress, value); } }
        public ushort SellPrice { get { return _emulator.ReadUShort(_sellPriceAddress); } set { _emulator.WriteUShort(_sellPriceAddress, value); } }
    }
}
