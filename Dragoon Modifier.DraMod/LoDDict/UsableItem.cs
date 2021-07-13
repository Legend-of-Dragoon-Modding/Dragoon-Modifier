using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.LoDDict {
    internal class UsableItem : Item, IUsableItem {
        public string BattleDescription { get; private set; } = "<END>";
        public string EncodedBattleDescription { get; private set; } = "FF A0 FF A0";
        public long BattleDescriptionPointer { get; set; } = 0;
        public long BattleNamePointer { get; set; } = 0;
        public byte Target { get; private set; } = 0;
        public byte Element { get; private set; } = 0;
        public byte Damage { get; private set; } = 0;
        public byte Special1 { get; private set; } = 0;
        public byte Special2 { get; private set; } = 0;
        public byte Unknown1 { get; private set; } = 0;
        public byte SpecialAmmount { get; private set; } = 0;
        public byte Status { get; private set; } = 0;
        public byte Percentage { get; private set; } = 0;
        public byte Unknown2 { get; private set; } = 0;
        public byte BaseSwitch { get; private set; } = 0;
    }
}
