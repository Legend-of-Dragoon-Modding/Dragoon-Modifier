using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace Dragoon_Modifier.UI {
    public class DebugOutput : TraceListener {
        private TextBoxBase output;

        public DebugOutput(TextBoxBase output) {
            this.Name = "Trace";
            this.output = output;
        }


        public override void Write(string message) {

            Action append = delegate () {
                output.AppendText(message);
            };
            append();

        }

        public override void WriteLine(string message) {
            Write(message + Environment.NewLine);
        }
    }
}
