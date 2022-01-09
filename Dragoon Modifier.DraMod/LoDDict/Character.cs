using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Dragoon_Modifier.DraMod.LoDDict {
    public class Character {
        static readonly string[] characterDir = new string[] { "Dart", "Lavitz Albert", "Shana Miranda", "Rose", "Haschel", "Lavitz Albert", "Meru", "Kongol", "Shana Miranda" };
        static readonly string[][] additions = new string[][] {
            new string[] { "Double Slash", "Volcano", "Burning Rush", "Crush Dance", "Madness Hero", "Moon Strike", "Blazing Dynamo"},
            new string[] { "[Lavitz] Harpoon", "[Lavitz] Spinning Cane", "[Lavitz] Rod Typhoon", "[Lavitz] Gust of Wind Dance", "[Lavitz] Flower Storm" },
            new string[0],
            new string[] { "Whip Smack", "More & More", "Hard Blade", "Demon's Dance"},
            new string[] { "Double Punch", "Flurry of Styx", "Summon 4 Gods", "5 Ring Shattering", "Hex Hammer", "Omni-Sweep"},
            new string[] { "[Albert] Harpoon", "[Albert] Spinning Cane", "[Albert] Rod Typhoon", "[Albert] Gust of Wind Dance", "[Albert] Flower Storm" },
            new string[] { "Double Smack", "Hammer Spin", "Cool Boogie", "Cat's Cradle", "Perky Step"},
            new string[] { "Pursuit", "Inferno", "Bone Crush"},
            new string[0],
        };

        public readonly BaseStats BaseStats;
        public readonly Addition[] Additions;

        internal Character(byte character, string modPath) {
            string dir = $"{modPath}\\{characterDir[character]}";

            try {
                BaseStats = new BaseStats(dir);

                Additions = GetAdditions(dir, character);
            } catch (DirectoryNotFoundException) {
                Console.WriteLine($"[ERROR] Directory {dir} not found.") ;
            }
        }

        private static Addition[] GetAdditions(string path, byte character) {
            var result = new Addition[additions[character].Length];

            byte i = 0;
            foreach(var addition in additions[character]) {
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
