using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod.Controller {
    internal static class Battle {
        static readonly uint[] sharanda = new uint[] { 0x2, 0x8 };
        static readonly ushort[] slot1FinalBlow = new ushort[] { 414, 408, 409, 392, 431 };      // Urobolus, Wounded Virage, Complete Virage, Lloyd, Zackwell
        static readonly ushort[] slot2FinalBlow = new ushort[] { 387, 403 };                     // Fruegel II, Gehrich

        public static void Setup(Emulator.IEmulator emulator, UI.IUIControl uiControl) {
            Console.WriteLine("Battle detected. Loading...");

            uint tableBase = emulator.Memory.BattleBasePoint;
            while (tableBase == emulator.Memory.CharacterPoint || tableBase == emulator.Memory.MonsterPoint) { // Wait until both C_Point and M_Point were set
                if (emulator.Memory.GameState != Emulator.GameState.Battle) { // TODO add Constants.RUN check
                    return;
                }
                Thread.Sleep(50);
            }

            emulator.LoadBattle();

            UpdateUI(emulator, uiControl);
        }

        public static void Run(Emulator.IEmulator emulator, UI.IUIControl uiControl) {
            UpdateUI(emulator, uiControl);
        }

        private static void UpdateUI(Emulator.IEmulator emulator, UI.IUIControl uiControl) {
            for (int i = 0; i < emulator.Battle.MonsterTable.Length; i++) {
                Console.WriteLine($"Monster Table {emulator.Battle.MonsterTable.Length}");
                uiControl.UpdateMonster(i, new UI.MonsterUpdate(emulator, i));
            }
            for (int i = 0; i < emulator.Battle.CharacterTable.Length; i++) {
                Console.WriteLine($"Character Table {emulator.Battle.CharacterTable.Length}");
                uiControl.UpdateCharacter(i, new UI.CharacterUpdate(emulator, i));
            }
        }
    }
}
