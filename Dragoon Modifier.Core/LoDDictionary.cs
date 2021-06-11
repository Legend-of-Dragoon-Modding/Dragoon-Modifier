using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Core {
    public static class LoDDictionary {

        private static LoDDict.Item[] _item = new LoDDict.Item[256];

        public static readonly Dictionary<byte, string> Num2Element = new Dictionary<byte, string>() {
                {0, "None" },
                {1, "Water" },
                {2, "Earth" },
                {4, "Dark" },
                {8, "Non-Elemental" },
                {16, "Thunder" },
                {32, "Light" },
                {64, "Wind" },
                {128, "Fire" }
            };
        public static readonly Dictionary<string, byte> Element2Num = new Dictionary<string, byte>() {
                {"", 0 },
                {"none", 0 },
                {"null", 0 },
                {"water", 1 },
                {"earth", 2 },
                {"dark", 4 },
                {"non-elemental", 8 },
                {"thunder", 16 },
                {"light", 32 },
                {"wind", 64 },
                {"fire", 128 }
            };
        public static readonly Dictionary<string, byte> Status2Num = new Dictionary<string, byte>() {
                {"none", 0 },
                {"", 0 },
                {"petrification", 1 },
                {"pe", 1 },
                {"bewitchment", 2 },
                {"be", 2 },
                {"confusion", 4 },
                {"cn", 4 },
                {"fear", 8 },
                {"fe", 8 },
                {"stun", 16 },
                {"st", 16 },
                {"armblocking", 32 },
                {"ab", 32 },
                {"dispirit", 64 },
                {"ds", 64 },
                {"poison", 128 },
                {"po", 128 },
                {"all", 255 }
            };
        public static readonly Dictionary<string, byte> DamageBase2Num = new Dictionary<string, byte>() {
            {"", 0x0 },
            {"none", 0x0 },
            {"0", 0x0},
            {"100", 0x0 },
            {"800", 0x1 },
            {"600", 0x2 },
            {"500", 0x4 },
            {"400", 0x8 },
            {"300", 0x10 },
            {"200", 0x20 },
            {"150", 0x40 },
            {"50", 0x80 }
        };

        public static LoDDict.Item[] Items { get { return _item; } private set { _item = value; } }
        public static string EncodedNames { get; private set; }
        public static string EncodedDescriptions { get; private set; }
        public static string EncodedBattleNames { get; private set; }
        public static string EncodedBattleDescriptions { get; private set; }
    }
}
