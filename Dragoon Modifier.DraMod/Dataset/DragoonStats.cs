
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Dataset {
    public sealed class DragoonStats : IDragoonStats {

        public ushort DAT { get; set; }
        public ushort DDF { get; set; }
        public ushort DMAT { get; set; }
        public ushort DMDF { get; set; }
        public ushort MP { get; set; }

        public DragoonStats(string ndat, string nddf, string ndmat, string ndmdf, string nmp) {
            ushort key = 0;
            if (ushort.TryParse(ndat, NumberStyles.AllowLeadingSign, null as IFormatProvider, out key)) {
                DAT = key;
            } else if (ndat != "") {
                Console.WriteLine(ndat + " not found as D-AT");
            }
            if (ushort.TryParse(ndmat, NumberStyles.AllowLeadingSign, null as IFormatProvider, out key)) {
                DMAT = key;
            } else if (ndmat != "") {
                Console.WriteLine(ndmat + " not found as D-MAT");
            }
            if (ushort.TryParse(nddf, NumberStyles.AllowLeadingSign, null as IFormatProvider, out key)) {
                DDF = key;
            } else if (nddf != "") {
                Console.WriteLine(nddf + " not found as D-DF");
            }
            if (ushort.TryParse(ndmdf, NumberStyles.AllowLeadingSign, null as IFormatProvider, out key)) {
                DMDF = key;
            } else if (ndmdf != "") {
                Console.WriteLine(ndmdf + " not found as D-MDF");
            }
            if (UInt16.TryParse(nmp, NumberStyles.AllowLeadingSign, null as IFormatProvider, out ushort shortkey)) {
                MP = shortkey;
            } else if (nmp != "") {
                Console.WriteLine(ndmdf + " not found as MP");
            }
        }
    }
}
