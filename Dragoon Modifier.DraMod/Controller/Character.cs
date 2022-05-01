using Dragoon_Modifier.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Controller {
    internal static class Character {

        /// <summary>
        /// Changes the character <paramref name="characterID"/> according to <paramref name="LoDDictionary"/>.
        /// </summary>
        /// <param name="LoDDictionary"></param>
        /// <param name="characterID"></param>
        internal static void ChangeStats(LoDDict.ILoDDictionary LoDDictionary, int characterID) {
            var character = Emulator.Memory.SecondaryCharacterTable[characterID];
            var baseStats = LoDDictionary.Character[characterID].BaseStats;

            byte level = character.Level;

            character.BodyAT = baseStats.AT[level];
            character.BodyMAT = baseStats.MAT[level];
            character.BodyDF = baseStats.DF[level];
            character.BodyMDF = baseStats.MDF[level];
            character.BodySPD = baseStats.SPD[level];
        }
    }
}
