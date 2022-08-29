using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Dataset {
    public class DragoonAddition : IDragoonAddition {
        public ushort HIT1 { get; private set; } = 0;
        public ushort HIT2 { get; private set; } = 0;
        public ushort HIT3 { get; private set; } = 0;
        public ushort HIT4 { get; private set; } = 0;
        public ushort HIT5 { get; private set; } = 0;

        internal DragoonAddition(string hit1, string hit2, string hit3, string hit4, string hit5) {
            ushort key = 0;
            if (ushort.TryParse(hit1, NumberStyles.AllowLeadingSign, null as IFormatProvider, out key)) {
                HIT1 = key;
            } else if (hit1 != "") {
                Console.WriteLine(hit1 + " not found as HIT1");
            }

            if (ushort.TryParse(hit2, NumberStyles.AllowLeadingSign, null as IFormatProvider, out key)) {
                HIT2 = key;
            } else if (hit2 != "") {
                Console.WriteLine(hit2 + " not found as HIT2");
            }

            if (ushort.TryParse(hit3, NumberStyles.AllowLeadingSign, null as IFormatProvider, out key)) {
                HIT3 = key;
            } else if (hit3 != "") {
                Console.WriteLine(hit3 + " not found as HIT3");
            }

            if (ushort.TryParse(hit4, NumberStyles.AllowLeadingSign, null as IFormatProvider, out key)) {
                HIT4 = key;
            } else if (hit4 != "") {
                Console.WriteLine(hit4 + " not found as HIT4");
            }

            if (ushort.TryParse(hit5, NumberStyles.AllowLeadingSign, null as IFormatProvider, out key)) {
                HIT5 = key;
            } else if (hit5 != "") {
                Console.WriteLine(hit5 + " not found as HIT5");
            }
        }

    }
}
