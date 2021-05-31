using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dragoon_Modifier {
    public static class BattleController2 {
        static readonly uint[] sharanda = new uint[] { 0x2, 0x8 };
        static readonly ushort[] slot1FinalBlow = new ushort[] { 414, 408, 409, 392, 431 };      // Urobolus, Wounded Virage, Complete Virage, Lloyd, Zackwell
        static readonly ushort[] slot2FinalBlow = new ushort[] { 387, 403 };                     // Fruegel II, Gehrich

        static Difficulty _difficulty = Difficulty.Normal;
        static int _aspectRatioOption = 0;
        static int _cameraOption = 0;
        static int _killBGM = 0;

        static Hotkey[] hotkeys = new Hotkey[] {
            new ExitDragoonSlot(Hotkey.KEY_L1 + Hotkey.KEY_UP, 0),
            new ExitDragoonSlot(Hotkey.KEY_L1 + Hotkey.KEY_RIGHT, 1),
            new ExitDragoonSlot(Hotkey.KEY_L1 + Hotkey.KEY_LEFT, 2),
            new AdditionSwap(Hotkey.KEY_L1 + Hotkey.KEY_R1)
        };

        public static void Setup() {
            Constants.WriteOutput("Battle detected. Loading...");

            // _difficulty = 
            // _aspectRatioOption =
            // _killBGM =


            /*
            if (Globals.CheckDMScript("btnAspectRatio")) {
                ChangeAspectRatio();
            }
            if (Globals.CheckDMScript("btnKillBGM") && killBGM != 0) {
                KillBGM();
            }
            if (difficulty == "NormalHard" || difficulty == "HardHell") {
                SwitchDualDifficulty();
            }
            if (Globals.DIFFICULTY_MODE.Contains("Hell")) {
                SetSPLossAmmount();
            } else {
                bossSPLoss = 0;
            }
            */

            uint tableBase = Emulator.MemoryController.BattlePointBase; // Base address in the Battle Pointer Table
            while (tableBase == Emulator.MemoryController.CharacterPoint || tableBase == Emulator.MemoryController.MonsterPoint) { // Wait until both C_Point and M_Point were set
                if (Emulator.MemoryController.GameState != GameState.Battle) {
                    return;
                }
                Thread.Sleep(50);
            }

            Emulator.BattleInit();

            Constants.WriteDebug($"Monster Size:        {Emulator.BattleController.MonsterTable.Length}");



            GameController.StatsChanged = true;
        }

        public static void Run() {
            if (Emulator.MemoryController.PartySlot[0] == 4 && Emulator.ReadByte("HASCHEL_FIX" + Globals.DISC) != 0x80) {
                MemoryController.Battle.NoDart.HaschelFix(Globals.DISC);
            }

            if (slot1FinalBlow.Contains(Emulator.BattleController.EncounterID) && sharanda.Contains(Emulator.MemoryController.PartySlot[0])) {
                //ShanaFix(0);
            }
            if (slot2FinalBlow.Contains(Emulator.BattleController.EncounterID) && sharanda.Contains(Emulator.MemoryController.PartySlot[1])) {
                //ShanaFix(1);
            }

            /*
            if (Globals.CheckDMScript("btnAdditionLevel")) {
                AdditionLevelUp();
            }
            if (Globals.CheckDMScript("btnNeverGuard")) {
                NeverGuard();
            }
            if (Globals.CheckDMScript("btnRemoveCaps")) {
                RemoveDamageCap();
            }
            */

            if (_difficulty != Difficulty.Normal) {
                HardMode.EquipChangesRun(GameController.InventorySize);
                /*
                if (!Globals.CheckDMScript("btnDivineRed")) {
                    HardMode.DartBurnStackHandler();
                }
                //HardMode.MagicInventoryHandler();
                HardMode.BattleDragoonRun((ushort) Globals.ENCOUNTER_ID, reverseDBS, eleBombTurns, eleBombElement);
                */
            }

            foreach (var hotkey in hotkeys) {
                if (hotkey.ButtonPress == Globals.HOTKEY) {
                    hotkey.Init();
                }
            }
            if (_difficulty != Difficulty.Normal) {
                foreach (var hotkey in HardMode.Hotkeys) {
                    if (hotkey.ButtonPress == Globals.HOTKEY) {
                        hotkey.Init();
                    }
                }
            }
        }



        class ExitDragoonSlot : Hotkey {
            byte _slot;

            public ExitDragoonSlot(int buttonPress, byte slot) : base(buttonPress) {
                _slot = slot;
            }

            public override void Init() {
                if (Emulator.BattleController.CharacterTable[_slot].DragoonTurns > 1) {
                    Emulator.BattleController.CharacterTable[_slot].DragoonTurns = 1;
                    Constants.WriteGLogOutput($"Slot {_slot + 1} will exit Dragoon after next action.");
                }
                Globals.LAST_HOTKEY = Constants.GetTime();
                return;
            }
        }

        class AdditionSwap : Hotkey {
            public AdditionSwap(int buttonPress) : base(buttonPress) {

            }

            public override void Init() {
                MemoryController.Battle.AdditionSwap.Init();
                Globals.LAST_HOTKEY = Constants.GetTime();
                return;
            }
        }
    }   
}
