using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Core.Memory {
    internal class Item : IItem {

        protected int _baseAddress;
        protected int _namePointerAddress;
        protected int _descriptionPointerAddress;
        protected int _sellPriceAddress;

        public virtual byte Icon { get; set; }
        public uint NamePointer { get { return Emulator.DirectAccess.ReadUInt24(_namePointerAddress); } set { Emulator.DirectAccess.WriteUInt24(_namePointerAddress, value); } }
        public string Name { get { return Emulator.DirectAccess.ReadText(NamePointer); } }
        public uint DescriptionPointer { get { return Emulator.DirectAccess.ReadUInt24(_descriptionPointerAddress); } set { Emulator.DirectAccess.WriteUInt24(_descriptionPointerAddress, value); } }
        public string Description { get { return Emulator.DirectAccess.ReadText(DescriptionPointer); } }
        public ushort SellPrice { get { return Emulator.DirectAccess.ReadUShort(_sellPriceAddress); } set { Emulator.DirectAccess.WriteUShort(_sellPriceAddress, value); } }

    }
}
