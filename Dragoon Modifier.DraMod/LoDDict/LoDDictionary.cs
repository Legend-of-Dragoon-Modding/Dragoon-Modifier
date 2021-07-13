using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.LoDDict {
    internal class LoDDictionary : ILoDDictionary {
        public IItem[] Item { get; private set; } = new IItem[256];

        internal LoDDictionary() {

        }
    }
}
