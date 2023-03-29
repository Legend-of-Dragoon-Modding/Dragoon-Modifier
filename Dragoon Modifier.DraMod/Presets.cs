using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod {
    public enum Preset : byte {
        [Description("US_Base")]
        Normal = 1,
        [Description("Hard_Mode")]
        NormalHard = 2,
        [Description("Hard_Mode")]
        Hard = 3,
        [Description("Hell_Mode")]
        HardHell = 4,
        [Description("Hell_Mode")]
        Hell = 5,
        [Description("")]
        Custom = 255
    }

    public static class PresetExtentions {
        public static string PresetToModFolder(this Preset preset) {
            DescriptionAttribute[] attributes = (DescriptionAttribute[]) preset
               .GetType()
               .GetField(preset.ToString())
               .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    }

    
}
