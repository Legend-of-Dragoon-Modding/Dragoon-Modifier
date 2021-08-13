using Dragoon_Modifier.DraMod;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using static System.Net.Mime.MediaTypeNames;

namespace Dragoon_Modifier.UI {
    public class TextBoxOutput : TextWriter {
        TextBox textBox = null;

        public TextBoxOutput(TextBox output) {
            textBox = output;
        }

        public override void WriteLine(string value) {
            base.WriteLine(value);
            WriteText(value);
        }

        public void WriteText(object value) {
            textBox.Dispatcher.BeginInvoke(new Action(() => {
                if (textBox.LineCount == 1 || textBox.LineCount > 5000) {
                    textBox.Text = "-----LOGCUT-----";
                }

                if (value.ToString().ToUpper().StartsWith("[DE")) {
                    if (Constants.Debug) {
                        textBox.AppendText("\r\n" + value.ToString());
                    }
                } else {
                    textBox.AppendText("\r\n" + value.ToString());
                }

                if (!textBox.IsFocused) {
                    textBox.ScrollToEnd();
                }

            }));
        }

        public override Encoding Encoding {
            get { return Encoding.UTF8; }
        }
    }
}
