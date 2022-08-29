using Dragoon_Modifier.Core;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Dataset {
    public class DragoonSpells : IDragoonSpells {
        public bool Percentage { get; set; }
        public double Damage { get; set; }
        public byte Accuracy { get; set; }
        public byte MP { get; set; }
        public byte Element { get; set; }
        public string Description { get; set; }
        public byte[] Encoded_Description { get; set; } = new byte[] { 0xFF, 0xA0, 0xFF, 0xA0 };
        public long Description_Pointer { get; set; }

        IDictionary<string, bool> perc = new Dictionary<string, bool> {
                { "yes", true},
                { "no", false },
                { "true", true },
                { "false", false },
                { "1", true },
                { "0", false },
                { "", false}
            };

        public DragoonSpells(string[] values, int spell, IDictionary<string, byte> Element2Num) {
            bool key = new bool();
            double dkey = new double();
            byte bkey = new byte();
            string name = values[0];

            if (perc.TryGetValue(values[1].ToLower(), out key)) {
                Percentage = key;
            } else {
                Console.WriteLine("Incorrect percentage swith " + values[1] + " for spell " + values[0]);
            }

            if (Double.TryParse(values[2], out dkey)) {
                Damage = dkey;
            } else {
                Console.WriteLine($"{values[2]} couldn't be parsed as damage for spell {values[0]}");
            }

            if (Byte.TryParse(values[3], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                Accuracy = bkey;
            } else {
                Console.WriteLine($"{values[3]} couldn't be parsed as accuracy for spell {values[0]}");
            }

            if (Byte.TryParse(values[4], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                MP = bkey;
            } else {
                Console.WriteLine($"{values[4]} couldn't be parsed as MP for spell {values[0]}");
            }

            if (Element2Num.TryGetValue(values[5].ToLower(), out bkey)) {
                Element = bkey;
            } else {
                Console.WriteLine($"{values[5]} not found as element for spell {values[0]}");
            }

            Description = values[6];
            if (Description != "") {
                Encoded_Description = Emulator.TextEncoding.GetBytes(Description).Concat(new byte[] { 0xFF, 0xA0 }).ToArray();
            }
        }
    }
}
