using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Core {
    internal static class Output {

        internal static void WriteDebug(object text) {
            Console.WriteLine("[DEBUG] " + text.ToString());
        }

        internal static void WriteError(object text) {
            Console.WriteLine("[ERROR] " + text.ToString());
        }
    }
}
