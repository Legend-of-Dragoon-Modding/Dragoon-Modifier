using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Emulator.Memory {
    internal class Item : IItem {

        protected int _baseAddress;
        protected int _namePointerAddress;
        protected int _descriptionPointerAddress;
        protected int _sellPriceAddress;

        protected IEmulator _emulator;

        public virtual byte Icon { get; set; }
        public uint NamePointer { get { return _emulator.ReadUInt24(_namePointerAddress); } set { _emulator.WriteUInt24(_namePointerAddress, value); } }
        public string Name { get { return _emulator.ReadText(NamePointer); } }
        public uint DescriptionPointer { get { return _emulator.ReadUInt24(_descriptionPointerAddress); } set { _emulator.WriteUInt24(_descriptionPointerAddress, value); } }
        public string Description { get { return _emulator.ReadText(DescriptionPointer); } }
        public ushort SellPrice { get { return _emulator.ReadUShort(_sellPriceAddress); } set { _emulator.WriteUShort(_sellPriceAddress, value); } }

    }
}
