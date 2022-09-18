using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Dataset {
    internal sealed class Character : ICharacter {
        private static readonly string[] _characterDir = new string[] { "Dart", "Lavitz Albert", "Shana Miranda", "Rose", "Haschel", "Lavitz Albert", "Meru", "Kongol", "Shana Miranda" };
        private static readonly string[][] _additions = new string[][] {
            new string[] { "Double Slash", "Volcano", "Burning Rush", "Crush Dance", "Madness Hero", "Moon Strike", "Blazing Dynamo"},
            new string[] { "[Lavitz] Harpoon", "[Lavitz] Spinning Cane", "[Lavitz] Rod Typhoon", "[Lavitz] Gust of Wind Dance", "[Lavitz] Flower Storm" },
            Array.Empty<string>(),
            new string[] { "Whip Smack", "More & More", "Hard Blade", "Demon's Dance"},
            new string[] { "Double Punch", "Flurry of Styx", "Summon 4 Gods", "5 Ring Shattering", "Hex Hammer", "Omni-Sweep"},
            new string[] { "[Albert] Harpoon", "[Albert] Spinning Cane", "[Albert] Rod Typhoon", "[Albert] Gust of Wind Dance", "[Albert] Flower Storm" },
            new string[] { "Double Smack", "Hammer Spin", "Cool Boogie", "Cat's Cradle", "Perky Step"},
            new string[] { "Pursuit", "Inferno", "Bone Crush"},
            Array.Empty<string>()
        };

        public IBaseStats BaseStats { get; private set; }
        public Addition[] Additions { get; private set; }

        internal Character(byte character, string modPath) {
            string dir = $"{modPath}\\{_characterDir[character]}";

            try {
                BaseStats = new BaseStats(dir, 0);

                Additions = GetAdditions(dir, character);
            } catch (DirectoryNotFoundException) {
                Console.WriteLine($"[ERROR] Directory {dir} not found.");
            }
        }

        private static Addition[] GetAdditions(string path, byte character) {
            var result = new Addition[_additions[character].Length];

            byte i = 0;
            foreach (var addition in _additions[character]) {
                try {
                    result[i] = new Addition($"{path}\\{addition}.tsv");
                } catch (FileNotFoundException) {
                    Console.WriteLine($"[ERROR] File {path}\\{addition}.tsv not found.");
                }

                i++;
            }

            return result;
        }
    }
}
