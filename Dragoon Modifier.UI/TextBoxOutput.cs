﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Dragoon_Modifier.UI {
    public class TextBoxOutput : TextWriter {
        TextBox textBox = null;

        public TextBoxOutput(TextBox output) {
            textBox = output;
        }

        public override void Write(char value) {
            base.Write(value);
            textBox.Dispatcher.BeginInvoke(new Action(() => { textBox.AppendText(value.ToString()); }));
        }

        public override Encoding Encoding {
            get { return Encoding.UTF8; }
        }
    }
}
