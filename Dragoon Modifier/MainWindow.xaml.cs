using System;
using System.Windows;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Windows.Threading;
using System.Windows.Media;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Windows.Controls;

namespace Dragoon_Modifier {
    public partial class MainWindow {
        #region Variables
        #region Program Variables
        public static Emulator emulator = new Emulator();
        public Thread fieldThread, battleThread, hotkeyThread, otherThread;
        public string preset = "";
        public static Dictionary<string, bool> dmScripts = new Dictionary<string, bool>();
        public static Dictionary<string, int> uiCombo = new Dictionary<string, int>();

        public TextBlock[,] monsterDisplay = new TextBlock[5, 6];
        public TextBlock[,] characterDisplay = new TextBlock[3, 9];
        #endregion

        #region Script Variables
        //General
        //Shop Changes
        public bool shopChange = false;
        public bool wroteIcons = false;
        //Icon Changes
        public bool firstWriteIcons = false;
        //Damage Cap
        public bool firstDamageCapRemoval = false;
        public int lastItemUsedDamageCap = 0;
        //Solo Mode
        public bool addSoloPartyMembers = false;
        public bool alwaysAddSoloPartyMembers = false;
        public bool soloModeOnBattleEntry = false;
        //Dragoon Changes
        public ushort[] currentMP = { 0, 0, 0 };
        public ushort[] previousMP = { 0, 0, 0 };
        public byte recoveryRateSave = 0;
        public bool dragoonChangesOnBattleEntry = false;
        //HP Cap Break
        public double[] hpChangeCheck = { 65535, 65535, 65535 };
        public byte[] hpChangeSlot = { 255, 255, 255 };
        public ushort[] hpChangeSave = { 0, 0, 0 };
        public bool hpCapBreakOnBattleEntry = false;
        public bool maxHPTableLoaded = false;
        public ushort[,] maxHPTable = new ushort[9, 60];
        //Aspect Ratio
        public bool aspectRatioOnBattleEntry = false;
        //Kill BGM
        public bool killBGMField = false;
        public bool killBGMBattle = false;
        public bool killedBGMField = false;
        public bool killedBGMBattle = false;
        public bool reKilledBGMField = false;
        //Elemental Bomb
        public byte eleBombTurns = 0;
        public byte eleBombItemUsed = 0;
        public byte eleBombSlot = 0;
        public byte eleBombElement = 0;
        public bool eleBombChange = false;
        public ushort[] eleBombOldElement = { 0, 0, 0, 0, 0 };
        //No Dragoon Mode
        public bool noDragoonModeOnBattleEntry = false;
        //Half SP
        public bool halfSPOnBattleEntry = false;
        //Addition Changes
        public bool damageIncreaseOnBattleEntry = false;
        //No HP Decay Soul Eater
        public bool noHPDecayOnBattleEntry = false;
        //Equip Changes
        public bool equipChangesOnFieldEntry = false;
        public bool equipChangesOnBattleEntry = false;
        public int[] guardStatusDF = new int[3];
        public int[] guardStatusMDF = new int[3];
        public int[] lGuardStatusDF = new int[3];
        public int[] lGuardStatusMDF = new int[3];
        public bool[] lGuardStateDF = new bool[3];
        public bool[] lGuardStateMDF = new bool[3];
        public bool[] sGuardStatusDF = new bool[3];
        public bool[] sGuardStatusMDF = new bool[3];
        public double[,] originalCharacterStats = new double[3, 10];
        //Ultimate Boss
        public int[] ultimateHP = new int[5];
        public int[] ultimateHPSave = new int[5];
        public int[] ultimateMaxHP = new int[5];
        public int ultimateBossCompleted = 0;
        public byte inventorySize = 32;
        //Ultimate Boss Equips
        public int soasSiphonSlot = -1;
        public ushort spiritEaterSP = 0;
        public ushort spiritEaterSaveSP = 0;
        public ushort elementArrowItem = 0;
        public ushort elementArrowElement = 0;
        public ushort elementArrowLastAction = 0;
        public ushort elementArrowTurns = 0;
        public ushort gloveLastAction = 0;
        public ushort gloveCharge = 0;
        public ushort axeLastAction = 0;
        public bool jeweledHammer = false;
        public bool checkHarpoon = false;
        #endregion
        #endregion

        #region Startup
        public MainWindow() {
            try {
                InitializeComponent();
                Constants.CONSOLE = txtOutput;
                Constants.GLOG = stsGame;
                Constants.PLOG = stsProgram;
                fieldThread = new Thread(FieldController);
                battleThread = new Thread(BattleController);
                hotkeyThread = new Thread(HotkeysController);
                otherThread = new Thread(OtherController);

                //Is this laziness visualized?
                lstField.Items.Add(new SubScript(Directory.GetFiles("Scripts")[2], ScriptState.LOCKED));
                lstBattle.Items.Add(new SubScript(Directory.GetFiles("Scripts")[1], ScriptState.LOCKED));
                lstHotkey.Items.Add(new SubScript(Directory.GetFiles("Scripts")[3], ScriptState.LOCKED));

                foreach (string file in Directory.GetFiles("Scripts\\Field", "*.cs", SearchOption.AllDirectories).OrderBy(f => f))
                    lstField.Items.Add(new SubScript(file));
                foreach (string file in Directory.GetFiles("Scripts\\Battle", "*.cs", SearchOption.AllDirectories).OrderBy(f => f))
                    lstBattle.Items.Add(new SubScript(file));
                foreach (string file in Directory.GetFiles("Scripts\\Hotkeys", "*.cs", SearchOption.AllDirectories).OrderBy(f => f))
                    lstHotkey.Items.Add(new SubScript(file));
                foreach (string file in Directory.GetFiles("Scripts\\Other", "*.cs", SearchOption.AllDirectories).OrderBy(f => f))
                    lstOther.Items.Add(new SubScript(file));

                Constants.Init();
                InitUI();
                LoadKey();
                LoadSubKey();

                if (Constants.EMULATOR != 255) {
                    SetupEmulator();
                } else {
                    Constants.WriteOutput("Please pick an emulator to use in the settings menu.");
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                MessageBox.Show("Error loading Scripts folder.");
                Application.Current.Shutdown();
            }
        }

        public void InitUI() {
            monsterDisplay[0, 0] = lblEnemy1Name;
            monsterDisplay[0, 1] = lblEnemy1HP;
            monsterDisplay[0, 2] = lblEnemy1ATK;
            monsterDisplay[0, 3] = lblEnemy1DEF;
            monsterDisplay[0, 4] = lblEnemy1SPD;
            monsterDisplay[0, 5] = lblEnemy1TRN;
            monsterDisplay[1, 0] = lblEnemy2Name;
            monsterDisplay[1, 1] = lblEnemy2HP;
            monsterDisplay[1, 2] = lblEnemy2ATK;
            monsterDisplay[1, 3] = lblEnemy2DEF;
            monsterDisplay[1, 4] = lblEnemy2SPD;
            monsterDisplay[1, 5] = lblEnemy2TRN;
            monsterDisplay[2, 0] = lblEnemy3Name;
            monsterDisplay[2, 1] = lblEnemy3HP;
            monsterDisplay[2, 2] = lblEnemy3ATK;
            monsterDisplay[2, 3] = lblEnemy3DEF;
            monsterDisplay[2, 4] = lblEnemy3SPD;
            monsterDisplay[2, 5] = lblEnemy3TRN;
            monsterDisplay[3, 0] = lblEnemy4Name;
            monsterDisplay[3, 1] = lblEnemy4HP;
            monsterDisplay[3, 2] = lblEnemy4ATK;
            monsterDisplay[3, 3] = lblEnemy4DEF;
            monsterDisplay[3, 4] = lblEnemy4SPD;
            monsterDisplay[3, 5] = lblEnemy4TRN;
            monsterDisplay[4, 0] = lblEnemy5Name;
            monsterDisplay[4, 1] = lblEnemy5HP;
            monsterDisplay[4, 2] = lblEnemy5ATK;
            monsterDisplay[4, 3] = lblEnemy5DEF;
            monsterDisplay[4, 4] = lblEnemy5SPD;
            monsterDisplay[4, 5] = lblEnemy5TRN;
            characterDisplay[0, 0] = lblCharacter1Name;
            characterDisplay[0, 1] = lblCharacter1HMP;
            characterDisplay[0, 2] = lblCharacter1ATK;
            characterDisplay[0, 3] = lblCharacter1DEF;
            characterDisplay[0, 4] = lblCharacter1VHIT;
            characterDisplay[0, 5] = lblCharacter1DATK;
            characterDisplay[0, 6] = lblCharacter1DDEF;
            characterDisplay[0, 7] = lblCharacter1SPD;
            characterDisplay[0, 8] = lblCharacter1TRN;
            characterDisplay[1, 0] = lblCharacter2Name;
            characterDisplay[1, 1] = lblCharacter2HMP;
            characterDisplay[1, 2] = lblCharacter2ATK;
            characterDisplay[1, 3] = lblCharacter2DEF;
            characterDisplay[1, 4] = lblCharacter2VHIT;
            characterDisplay[1, 5] = lblCharacter2DATK;
            characterDisplay[1, 6] = lblCharacter2DDEF;
            characterDisplay[1, 7] = lblCharacter2SPD;
            characterDisplay[1, 8] = lblCharacter2TRN;
            characterDisplay[2, 0] = lblCharacter3Name;
            characterDisplay[2, 1] = lblCharacter3HMP;
            characterDisplay[2, 2] = lblCharacter3ATK;
            characterDisplay[2, 3] = lblCharacter3DEF;
            characterDisplay[2, 4] = lblCharacter3VHIT;
            characterDisplay[2, 5] = lblCharacter3DATK;
            characterDisplay[2, 6] = lblCharacter3DDEF;
            characterDisplay[2, 7] = lblCharacter3SPD;
            characterDisplay[2, 8] = lblCharacter3TRN;

            cboSoloLeader.Items.Add("Slot 1");
            cboSoloLeader.Items.Add("Slot 2");
            cboSoloLeader.Items.Add("Slot 3");

            cboAspectRatio.Items.Add("4:3");
            cboAspectRatio.Items.Add("16:9");
            cboAspectRatio.Items.Add("16:10");
            cboAspectRatio.Items.Add("21:9");
            cboAspectRatio.Items.Add("32:9");

            cboCamera.Items.Add("Default");
            cboCamera.Items.Add("Advanced");

            cboKillBGM.Items.Add("Field");
            cboKillBGM.Items.Add("Battle");
            cboKillBGM.Items.Add("Both");

            cboSwitchChar.Items.Add("Dart");
            cboSwitchChar.Items.Add("Lavitz");
            cboSwitchChar.Items.Add("Shana");
            cboSwitchChar.Items.Add("Rose");
            cboSwitchChar.Items.Add("Haschel");
            cboSwitchChar.Items.Add("Albert");
            cboSwitchChar.Items.Add("Meru");
            cboSwitchChar.Items.Add("Kongol");
            cboSwitchChar.Items.Add("Miranda");

            cboElement.Items.Add("Fire");
            cboElement.Items.Add("Water");
            cboElement.Items.Add("Wind");
            cboElement.Items.Add("Earth");
            cboElement.Items.Add("Dark");
            cboElement.Items.Add("Light");
            cboElement.Items.Add("Thunder");

            cboSoloLeader.SelectedIndex = 0;
            cboAspectRatio.SelectedIndex = 0;
            cboCamera.SelectedIndex = 0;
            cboKillBGM.SelectedIndex = 1;
            cboSwitchChar.SelectedIndex = 0;
            cboElement.SelectedIndex = 0;
        }

        public void LoadKey() {
            if (Constants.KEY.GetValue("Save Slot") == null) {
                foreach (MenuItem mi in miSaveSlot.Items)
                    mi.IsChecked = miSaveSlot.Items.IndexOf(mi) == Constants.SAVE_SLOT ? true : false;
                Constants.KEY.SetValue("Save Slot", Constants.SAVE_SLOT);
            } else {
                int slot = (int) Constants.KEY.GetValue("Save Slot");
                Constants.SAVE_SLOT = slot;
                foreach (MenuItem mi in miSaveSlot.Items)
                    mi.IsChecked = miSaveSlot.Items.IndexOf(mi) == slot ? true : false;
            }

            if (Constants.KEY.GetValue("EmulatorType") != null) {
                int slot = (int) Constants.KEY.GetValue("EmulatorType");
                Constants.EMULATOR = (byte) slot;

                if (Constants.EMULATOR == 8) {
                    Constants.EMULATOR_NAME = "RetroArch";
                } else if (Constants.EMULATOR == 9) {
                    Constants.EMULATOR_NAME = "pcsx2";
                } else {
                    Constants.EMULATOR_NAME = "ePSXe";
                }

                foreach (MenuItem mi in miEmulator.Items)
                    mi.IsChecked = miEmulator.Items.IndexOf(mi) == slot ? true : false;
            }

            if (Constants.KEY.GetValue("Region") == null) {
                foreach (MenuItem mi in miRegion.Items)
                    mi.IsChecked = miRegion.Items.IndexOf(mi) == (byte) Constants.REGION ? true : false;
                Constants.KEY.SetValue("Region", (int) Constants.REGION);
            } else {
                Region slot = (Region) ((int) Constants.KEY.GetValue("Region"));
                Constants.REGION = slot;
                foreach (MenuItem mi in miRegion.Items)
                    mi.IsChecked = miRegion.Items.IndexOf(mi) == (byte) Constants.REGION ? true : false;
            }

            if (Constants.KEY.GetValue("LoadPreset") != null) {
                bool load = Constants.KEY.GetValue("LoadPreset").Equals("True");
                if (Constants.KEY.GetValue("Preset") != null && load) {
                    preset = (string) Constants.KEY.GetValue("Preset");
                    Constants.LoadPreset(preset);
                    LoadPreset();
                }
                miOpenPreset.IsChecked = load;
            }
        }

        public void LoadSubKey() {
            if (Constants.SUBKEY.GetValue("Ultimate Boss") == null) {
                Constants.SUBKEY.SetValue("Ultimate Boss", 0);
                ultimateBossCompleted = 0;
            } else {
                ultimateBossCompleted = (int) Constants.SUBKEY.GetValue("Ultimate Boss");
            }
        }

        public void SaveKey() {
            Constants.KEY.SetValue("Save Slot", Constants.SAVE_SLOT);
            if (Constants.EMULATOR != 255)
                Constants.KEY.SetValue("EmulatorType", (int) Constants.EMULATOR);
            Constants.KEY.SetValue("Region", (int) Constants.REGION);
            Constants.KEY.SetValue("LoadPreset", miOpenPreset.IsChecked);
            if (miOpenPreset.IsChecked)
                Constants.KEY.SetValue("Preset", preset);
        }

        public void SaveSubKey() {
            Constants.SUBKEY.SetValue("Ultimate Boss", ultimateBossCompleted);
        }

        private void LoadPreset() {
            string current = "";
            try {
                foreach (KeyValuePair<string, byte> entry in Constants.PRESET_SCRIPTS) {
                    bool found = false;
                    current = entry.Key;

                    found = FindScript(lstField, entry);
                    if (found)
                        continue;

                    found = FindScript(lstBattle, entry);
                    if (found)
                        continue;

                    found = FindScript(lstHotkey, entry);
                    if (found)
                        continue;

                    found = FindScript(lstOther, entry);

                    if (!found) {
                        throw new Exception();
                    }
                }
                ScriptDisplay(lstField);
                ScriptDisplay(lstBattle);
                ScriptDisplay(lstHotkey);
                ScriptDisplay(lstOther);
                ChangeTitle(preset);
                Constants.WriteOutput("Preset '" + preset + "' loaded.");
            } catch (Exception e) {
                Constants.WriteOutput("Failed to load preset. Script not found: '" + current + "'.");
                DisableScripts();
            }
        }

        public bool FindScript(ListView lst, KeyValuePair<string, byte> entry) {
            bool found = false;
            foreach (SubScript s in lst.Items) {
                if (s.GetPath().Contains(entry.Key)) {
                    s.state = (ScriptState) entry.Value;
                    found = true;
                }
            }
            return found;
        }

        public void DisableScripts() {
            DisableScripts(lstField, true);
            DisableScripts(lstBattle, true);
            DisableScripts(lstHotkey, true);
            DisableScripts(lstOther, false);
        }

        public void DisableScripts(ListView lst, bool skip) {
            int index = 0;
            foreach (SubScript s in lst.Items) {
                index++;
                if (index == 1 && skip)
                    continue;
                s.state = ScriptState.DISABLED;
            }
        }

        public void OpenScript(ListView lst) {
            SubScript script = (SubScript) lst.SelectedItem;
            if (script.state != ScriptState.DISABLED) {
                Constants.WriteOutput("Opening script '" + script.ToString() + "'...");
                if (script.Click(emulator) == 0) {
                    Constants.WriteOutput("Script failed.");
                }
            } else {
                Constants.WriteOutput("Script is disabled.");
            }
        }

        public void ChangeTitle(string preset) {
            this.preset = preset;
            this.Title = " Dragoon Modifier " + Constants.VERSION + " (" + preset + ")";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            ScriptDisplay(lstField);
            ScriptDisplay(lstBattle);
            ScriptDisplay(lstHotkey);
            ScriptDisplay(lstOther);
        }

        public void ScriptDisplay(ListView list) {
            for (int i = 0; i < list.Items.Count; i++) {
                try {
                    ListViewItem lbl = (ListViewItem) list.ItemContainerGenerator.ContainerFromIndex(i);
                    SubScript script = (SubScript) list.Items[i];
                    if (script.state == ScriptState.DISABLED) {
                        lbl.Foreground = Brushes.Red;
                    } else {
                        lbl.Foreground = Brushes.Black;
                    }
                } catch (Exception e) { }
            }
        }
        #endregion

        #region Threads
        public void FieldController() {
            string currentScript = "";
            int run = 1;
            while (run == 1 && Constants.RUN) {
                foreach (SubScript script in lstField.Items) {
                    if (script.state == ScriptState.DISABLED)
                        continue;
                    currentScript = script.ToString();
                    this.Dispatcher.BeginInvoke(new Action(() => {
                        run = script.Run(emulator);
                    }), DispatcherPriority.ContextIdle);
                }

                LoadMaxHPTable();

                if (dmScripts.ContainsKey("btnSaveAnywhere") && dmScripts["btnSaveAnywhere"])
                    SaveAnywhere();
                if (dmScripts.ContainsKey("btnShopChanges") && dmScripts["btnShopChanges"])
                    ShopChanges();
                if (dmScripts.ContainsKey("btnSoloMode") && dmScripts["btnSoloMode"])
                    SoloModeField();
                if (dmScripts.ContainsKey("btnHPCapBreak") && dmScripts["btnHPCapBreak"])
                    HPCapBreakField();
                if (dmScripts.ContainsKey("btnKillBGM") && dmScripts["btnKillBGM"] && killBGMField)
                    KillBGMField();
                if (dmScripts.ContainsKey("btnCharmPotion") && dmScripts["btnCharmPotion"])
                    AutoCharmPotion();
                if (dmScripts.ContainsKey("btnEquipChanges") && dmScripts["btnEquipChanges"])
                    EquipChangesField();

                Thread.Sleep(500);
                this.Dispatcher.BeginInvoke(new Action(() => {
                    FieldUI();
                }), DispatcherPriority.ContextIdle);
            }
        }

        public void BattleController() {
            string currentScript = "";
            int run = 1;
            while (run == 1 && Constants.RUN) {
                foreach (SubScript script in lstBattle.Items) {
                    if (script.state == ScriptState.DISABLED)
                        continue;
                    currentScript = script.ToString();
                    this.Dispatcher.BeginInvoke(new Action(() => {
                        run = script.Run(emulator);
                    }), DispatcherPriority.ContextIdle);
                }

                this.Dispatcher.BeginInvoke(new Action(() => {
                    if (Globals.IN_BATTLE && !Constants.BATTLE_UI) {
                        for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                            monsterDisplay[i, 0].Text = emulator.ReadName(0xC69D0 + (0x2C * i));
                        }
                    }
                }), DispatcherPriority.ContextIdle);

                if (dmScripts.ContainsKey("btnRemoveCaps") && dmScripts["btnRemoveCaps"])
                    RemoveDamageCap();
                if (dmScripts.ContainsKey("btnSoloMode") && dmScripts["btnSoloMode"])
                    SoloModeBattle();
                if (dmScripts.ContainsKey("btnDragoonChanges") && dmScripts["btnDragoonChanges"])
                    DragoonChanges();
                if (dmScripts.ContainsKey("btnHPCapBreak") && dmScripts["btnHPCapBreak"])
                    HPCapBreakBattle();
                if (dmScripts.ContainsKey("btnAspectRatio") && dmScripts["btnAspectRatio"])
                    ChangeAspectRatio();
                if (dmScripts.ContainsKey("btnKillBGM") && dmScripts["btnKillBGM"] && killBGMBattle)
                    KillBGMBattle();
                if (dmScripts.ContainsKey("btnElementalBomb") && dmScripts["btnElementalBomb"])
                    ElementalBomb();
                if (dmScripts.ContainsKey("btnNoDragoon") && dmScripts["btnNoDragoon"])
                    NoDragoonMode();
                if (dmScripts.ContainsKey("btnHalfSP") && dmScripts["btnHalfSP"])
                    AdditionHalfSPChanges();
                if (dmScripts.ContainsKey("btnAdditionChanges") && dmScripts["btnAdditionChanges"])
                    AdditionDamageChanges();
                if (dmScripts.ContainsKey("btnEquipChanges") && dmScripts["btnEquipChanges"])
                    EquipChangesBattle();
                if (dmScripts.ContainsKey("btnSoulEater") && dmScripts["btnSoulEater"])
                    NoHPDecaySoulEater();
                if (dmScripts.ContainsKey("btnHPNames") && dmScripts["btnHPNames"])
                    MonsterHPNames();

                Thread.Sleep(250);
                this.Dispatcher.BeginInvoke(new Action(() => {
                    BattleUI();
                }), DispatcherPriority.ContextIdle);

                if (Globals.EXITING_BATTLE > 0)
                    Globals.EXITING_BATTLE -= 1;
            }
        }

        public void HotkeysController() {
            string currentScript = "";
            int run = 1;
            while (run == 1 && Constants.RUN) {
                Globals.CURRENT_TIME = Constants.GetTime();
                foreach (SubScript script in lstHotkey.Items) {
                    if (script.state == ScriptState.DISABLED || Globals.CURRENT_TIME < (Globals.LAST_HOTKEY + 3))
                        continue;
                    currentScript = script.ToString();
                    this.Dispatcher.BeginInvoke(new Action(() => {
                        run = script.Run(emulator);
                    }), DispatcherPriority.ContextIdle);
                }

                if (Globals.CURRENT_TIME >= (Globals.LAST_HOTKEY + 3)) {
                    if (Globals.HOTKEY == (Hotkey.KEY_SQUARE + Hotkey.KEY_CIRCLE)) {
                        ChangeShop();
                        Globals.LAST_HOTKEY = Constants.GetTime();
                    }
                }

                Thread.Sleep(1000);
            }
        }

        public void OtherController() {
            string currentScript = "";
            int run = 1;
            while (run == 1 && Constants.RUN) {
                foreach (SubScript script in lstOther.Items) {
                    if (script.state == ScriptState.DISABLED)
                        continue;
                    currentScript = script.ToString();
                    this.Dispatcher.BeginInvoke(new Action(() => {
                        run = script.Run(emulator);
                    }), DispatcherPriority.ContextIdle);
                }
                Thread.Sleep(1000);
            }
        }
        #endregion

        #region Scripts
        #region Field
        #region Save Anywhere
        public void SaveAnywhere() {
            if (!Globals.IN_BATTLE) {
                emulator.WriteShort(Constants.GetAddress("SAVE_ANYWHERE"), 1);
            }
        }
        #endregion

        #region Shop Changes
        public void ShopChanges() {
            switch (Globals.MAP) {
                case 145: // Lohan
                    if (ReadShop(0x11E0FC) == 229) {
                        WriteShop(0x11E100, 222);
                        WriteShop(0x11E102, 30);
                    } else {
                        WriteShop(0x11E100, 90);
                        WriteShop(0x11E102, 7500);
                    }
                    WriteShop(0x11E118, 101);
                    WriteShop(0x11E11A, 3750);
                    if (shopChange) {
                        WriteShop(0x11E11C, 133);
                        WriteShop(0x11E120, 131);
                    } else {
                        WriteShop(0x11E11C, 150);
                        WriteShop(0x11E120, 151);
                    }
                    WriteShop(0x11E0FE, 100);
                    break;
                case 180: //Kanzas
                    WriteShop(0x11E0FE, 80);
                    break;
                case 267: //I'm not sure what shop this was supposed to be
                case 214: //Feltz
                case 287: //Phantom Ship
                case 309: //Lideria
                case 479: //Vellweb
                    WriteShop(0x11E0FE, 80);
                    WriteShop(0x11E102, 100);
                    break;
                case 247: //Donau
                case 332: //Furni
                    WriteShop(0x11E0FE, 100);
                    break;
                case 349: //Dennigrad
                case 357: //Dennigrad
                    WriteShop(0x11E124, 241);
                    WriteShop(0x11E0FE, 80);
                    WriteShop(0x11E102, 100);
                    break;
                case 384: //Wingly Forest
                    if (emulator.ReadShort(0x11E0FA) == 30) {
                        WriteShop(0x11E0FE, 100);
                        WriteShop(0x11E102, 80);
                    } else {
                        WriteShop(0x11E0FE, 600);
                        WriteShop(0x11E102, 600);
                    }
                    break;
                case 515: //Ulra
                case 525: //Ulra
                case 435: //Kashua
                    WriteShop(0x11E0FE, 100);
                    WriteShop(0x11E102, 80);
                    break;
                case 564: //Rouge
                    if (ReadShop(0x11E0FA) == 30) {
                        WriteShop(0x11E0FE, 80);
                        WriteShop(0x11E102, 100);
                    } else {
                        WriteShop(0x11E0FE, 1000);
                        WriteShop(0x11E102, 1000);
                    }
                    break;
                case 619: //Moon
                    if (ReadShop(0x11E0FA) == 30) {
                        WriteShop(0x11E0FE, 80);
                        WriteShop(0x11E102, 100);
                    } else {
                        WriteShop(0x11E0FE, 500);
                        WriteShop(0x11E102, 500);
                    }
                    break;
                case 530: //Law City
                    if (ReadShop(0x11E0FC) == 229) {
                        WriteShop(0x11E0FE, 80);
                        WriteShop(0x11E102, 100);
                    } else {
                        WriteShop(0x11E0FE, 400);
                        WriteShop(0x11E102, 800);
                    }
                    break;
            }
        }

        public int ReadShop(int address) {
            return emulator.ReadShort(address + ShopOffset());
        }

        public void WriteShop(int address, ushort value) {
            emulator.WriteShort(address + ShopOffset(), value);
        }

        public int ShopOffset() {
            int offset = 0x0;
            if (Constants.REGION == Region.JPN) {
                offset -= 0x4D90;
            } else if (Constants.REGION == Region.EUR_GER) {
                offset += 0x120;
            }
            return offset;
        }

        /*public int GetShopItem(int slot) {
            return emulator.ReadShort(Constants.GetAddress("SHOP") + ((slot - 1) * 0x2));
        }

        public int GetShopPrice(int slot) {
            return emulator.ReadShort(Constants.GetAddress("SHOP") + ((slot - 1) * 0x2)) + 0x2;
        }
        
        public void ChangeShopItemPrice(int slot, byte item, ushort price) {
            emulator.WriteShort(Constants.GetAddress("SHOP") + ((slot - 1) * 0x2), item);
            emulator.WriteShort(Constants.GetAddress("SHOP") + ((slot - 1) * 0x2) + 0x2, price);
        }
        
        public void ChangeShopItem(int slot, byte item) {
            emulator.WriteShort(Constants.GetAddress("SHOP") + ((slot - 1) * 0x2), item);
        }
        
        public void ChangeShopPrice(int slot, ushort price) {
            emulator.WriteShort(Constants.GetAddress("SHOP") + ((slot - 1) * 0x2) + 0x2, price);
        }*/

        public void ChangeShop() { //SQUARE + CIRCLE
            if (Globals.BATTLE_VALUE == 0 && Globals.MAP == 145) {
                shopChange = shopChange ? false : true;
                Constants.WriteGLogOutput("Changed Lohan shop to state: " + shopChange);
            }
        }
        #endregion

        #region Icon Changes
        public void IconChanges() {
            byte menu = emulator.ReadByte(Constants.GetAddress("MENU"));
            if (menu == 12 || menu == 16 || menu == 19) {
                WriteIcons();
                wroteIcons = true;
            } else {
                if (menu == 125) {
                    wroteIcons = false;
                }
            }

            if (!firstWriteIcons && !Globals.IN_BATTLE && Globals.BATTLE_VALUE > 0) {
                WriteIcons();
                firstWriteIcons = true;
            } else {
                if (Globals.IN_BATTLE) {
                    firstWriteIcons = false;
                }
            }
        }

        public void WriteIcons() {
            emulator.WriteByte(0x1120DE + IconOffset(), 1);
            emulator.WriteByte(0x1120FA + IconOffset(), 1);
            emulator.WriteByte(0x112116 + IconOffset(), 1);
            emulator.WriteByte(0x112132 + IconOffset(), 1);
            emulator.WriteByte(0x11214E + IconOffset(), 1);
            emulator.WriteByte(0x1121F6 + IconOffset(), 56);
            emulator.WriteByte(0x11222E + IconOffset(), 3);
            emulator.WriteByte(0x11224A + IconOffset(), 3);
            emulator.WriteByte(0x112266 + IconOffset(), 3);
            emulator.WriteByte(0x112282 + IconOffset(), 3);
            emulator.WriteByte(0x11229E + IconOffset(), 3);
            emulator.WriteByte(0x1122BA + IconOffset(), 3);
            emulator.WriteByte(0x1122D6 + IconOffset(), 3);
            emulator.WriteByte(0x1122F2 + IconOffset(), 4);
            emulator.WriteByte(0x11230E + IconOffset(), 4);
            emulator.WriteByte(0x11232A + IconOffset(), 4);
            emulator.WriteByte(0x112346 + IconOffset(), 4);
            emulator.WriteByte(0x112362 + IconOffset(), 4);
            emulator.WriteByte(0x11237E + IconOffset(), 4);
            emulator.WriteByte(0x11239A + IconOffset(), 4);
            emulator.WriteByte(0x1123B6 + IconOffset(), 2);
            emulator.WriteByte(0x1123D2 + IconOffset(), 2);
            emulator.WriteByte(0x1123EE + IconOffset(), 2);
            emulator.WriteByte(0x11240A + IconOffset(), 2);
            emulator.WriteByte(0x112426 + IconOffset(), 2);
            emulator.WriteByte(0x112442 + IconOffset(), 2);
            emulator.WriteByte(0x11245E + IconOffset(), 7);
            emulator.WriteByte(0x11247A + IconOffset(), 7);
            emulator.WriteByte(0x112496 + IconOffset(), 7);
            emulator.WriteByte(0x1124B2 + IconOffset(), 7);
            emulator.WriteByte(0x1124CE + IconOffset(), 7);
            emulator.WriteByte(0x1124EA + IconOffset(), 7);
            emulator.WriteByte(0x112506 + IconOffset(), 8);
            emulator.WriteByte(0x1125E6 + IconOffset(), 11);
            emulator.WriteByte(0x11263A + IconOffset(), 14);
            emulator.WriteByte(0x112656 + IconOffset(), 14);
            emulator.WriteByte(0x112672 + IconOffset(), 14);
            emulator.WriteByte(0x11268E + IconOffset(), 14);
            emulator.WriteByte(0x1126C6 + IconOffset(), 9);
            emulator.WriteByte(0x1126E2 + IconOffset(), 9);
            emulator.WriteByte(0x11271A + IconOffset(), 12);
            emulator.WriteByte(0x1127C2 + IconOffset(), 14);
            emulator.WriteByte(0x1127DE + IconOffset(), 12);
            emulator.WriteByte(0x11292E + IconOffset(), 15);
            emulator.WriteByte(0x11294A + IconOffset(), 15);
            emulator.WriteByte(0x11299E + IconOffset(), 17);
            emulator.WriteByte(0x112A46 + IconOffset(), 20);
            emulator.WriteByte(0x112A62 + IconOffset(), 19);
            emulator.WriteByte(0x112A7E + IconOffset(), 19);
            emulator.WriteByte(0x112AEE + IconOffset(), 19);
            emulator.WriteByte(0x112B0A + IconOffset(), 19);
            emulator.WriteByte(0x112B42 + IconOffset(), 26);
            emulator.WriteByte(0x112B5E + IconOffset(), 26);
            emulator.WriteByte(0x112B7A + IconOffset(), 26);
            emulator.WriteByte(0x112B96 + IconOffset(), 26);
            emulator.WriteByte(0x112BB2 + IconOffset(), 26);
            emulator.WriteByte(0x112BCE + IconOffset(), 26);
            emulator.WriteByte(0x112BEA + IconOffset(), 28);
            emulator.WriteByte(0x112C06 + IconOffset(), 26);
            emulator.WriteByte(0x112C22 + IconOffset(), 22);
            emulator.WriteByte(0x112C3E + IconOffset(), 22);
            emulator.WriteByte(0x112C5A + IconOffset(), 22);
            emulator.WriteByte(0x112C76 + IconOffset(), 22);
            emulator.WriteByte(0x112C92 + IconOffset(), 22);
            emulator.WriteByte(0x112CAE + IconOffset(), 22);
            emulator.WriteByte(0x112CCA + IconOffset(), 22);
            emulator.WriteByte(0x112CE6 + IconOffset(), 29);
            emulator.WriteByte(0x112D02 + IconOffset(), 29);
            emulator.WriteByte(0x112D1E + IconOffset(), 29);
            emulator.WriteByte(0x112D56 + IconOffset(), 24);
            emulator.WriteByte(0x112D72 + IconOffset(), 30);
            emulator.WriteByte(0x112DE2 + IconOffset(), 24);
            emulator.WriteByte(0x112DFE + IconOffset(), 24);
            emulator.WriteByte(0x112E36 + IconOffset(), 27);
            emulator.WriteByte(0x112EA6 + IconOffset(), 25);
            emulator.WriteByte(0x112EC2 + IconOffset(), 25);
            emulator.WriteByte(0x112EDE + IconOffset(), 25);
            emulator.WriteByte(0x112EFA + IconOffset(), 25);
            emulator.WriteByte(0x112F16 + IconOffset(), 25);
            emulator.WriteByte(0x112F32 + IconOffset(), 25);
            emulator.WriteByte(0x112F4E + IconOffset(), 25);
            emulator.WriteByte(0x11304A + IconOffset(), 32);
            emulator.WriteByte(0x113066 + IconOffset(), 32);
            emulator.WriteByte(0x113082 + IconOffset(), 32);
            emulator.WriteByte(0x1130BA + IconOffset(), 22);
            emulator.WriteByte(0x1130D6 + IconOffset(), 22);
        }

        int IconOffset() {
            int offset = 0x0;
            if (Constants.REGION == Region.JPN) {
                offset = -0x186C;
            } else if (Constants.REGION == Region.EUR_GER) {
                offset = 0x23C;
            }
            return offset;
        }
        #endregion

        #region Auto Charm Potion
        public void AutoCharmPotion() {
            if ((emulator.ReadShort(Constants.GetAddress("BATTLE_VALUE")) > 3850 && emulator.ReadShort(Constants.GetAddress("BATTLE_VALUE")) < 9999) && emulator.ReadInteger(Constants.GetAddress("GOLD")) >= 8) {
                emulator.WriteInteger(Constants.GetAddress("GOLD"), emulator.ReadInteger(Constants.GetAddress("GOLD")) - 8);
                emulator.WriteShort(Constants.GetAddress("BATTLE_VALUE"), 0);
            }
        }
        #endregion
        #endregion

        #region Both
        #region Battle Stats Display
        public void FieldUI() {
            lblEncounter.Text = "Encounter Value: " + Globals.BATTLE_VALUE;
            lblEnemyID.Text = "Enemy ID: " + Globals.ENCOUNTER_ID;
            lblMapID.Text = "Map ID: " + Globals.MAP;
        }

        public void BattleUI() {
            if (Globals.IN_BATTLE && Globals.STATS_CHANGED) {
                Constants.BATTLE_UI = true;
                for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                    monsterDisplay[i, 1].Text = " " + emulator.ReadShort(Globals.MONS_ADDRESS[i]) + "/" + emulator.ReadShort(Globals.MONS_ADDRESS[i] + 0x8);
                    monsterDisplay[i, 2].Text = " " + emulator.ReadShort(Globals.MONS_ADDRESS[i] + 0x2C) + "/" + emulator.ReadShort(Globals.MONS_ADDRESS[i] + 0x2E);
                    monsterDisplay[i, 3].Text = " " + emulator.ReadShort(Globals.MONS_ADDRESS[i] + 0x30) + "/" + emulator.ReadShort(Globals.MONS_ADDRESS[i] + 0x32);
                    monsterDisplay[i, 4].Text = " " + emulator.ReadShort(Globals.MONS_ADDRESS[i] + 0x2A);
                    monsterDisplay[i, 5].Text = " " + emulator.ReadShort(Globals.MONS_ADDRESS[i] + 0x44);
                }
                for (int i = 0; i < 3; i++) {
                    if (Globals.PARTY_SLOT[i] < 9) {
                        characterDisplay[i, 0].Text = Constants.GetCharName(Globals.PARTY_SLOT[i]);
                        characterDisplay[i, 1].Text = " " + emulator.ReadShort(Globals.CHAR_ADDRESS[i]) + "/" + emulator.ReadShort(Globals.CHAR_ADDRESS[i] + 0x8) + "\r\n\r\n " + emulator.ReadShort(Globals.CHAR_ADDRESS[i] + 0x4) + "/" + emulator.ReadShort(Globals.CHAR_ADDRESS[i] + 0xA);
                        characterDisplay[i, 2].Text = " " + emulator.ReadShort(Globals.CHAR_ADDRESS[i] + 0x2C) + "\r\n\r\n " + emulator.ReadShort(Globals.CHAR_ADDRESS[i] + 0x2E);
                        characterDisplay[i, 3].Text = " " + emulator.ReadShort(Globals.CHAR_ADDRESS[i] + 0x30) + "\r\n\r\n " + emulator.ReadShort(Globals.CHAR_ADDRESS[i] + 0x32);
                        characterDisplay[i, 4].Text = " " + emulator.ReadShort(Globals.CHAR_ADDRESS[i] + 0x34) + "/" + emulator.ReadShort(Globals.CHAR_ADDRESS[i] + 0x36) + "\r\n\r\n " + emulator.ReadShort(Globals.CHAR_ADDRESS[i] + 0x38) + "/" + emulator.ReadShort(Globals.CHAR_ADDRESS[i] + 0x3A);
                        characterDisplay[i, 5].Text = " " + emulator.ReadShort(Globals.CHAR_ADDRESS[i] + 0xA4) + "\r\n\r\n " + emulator.ReadShort(Globals.CHAR_ADDRESS[i] + 0xA6);
                        characterDisplay[i, 6].Text = " " + emulator.ReadShort(Globals.CHAR_ADDRESS[i] + 0xA8) + "\r\n\r\n " + emulator.ReadShort(Globals.CHAR_ADDRESS[i] + 0xAA);
                        characterDisplay[i, 7].Text = " " + emulator.ReadShort(Globals.CHAR_ADDRESS[i] + 0x2A) + "\r\n\r\n " + emulator.ReadShort(Globals.CHAR_ADDRESS[i] + 0x2);
                        characterDisplay[i, 8].Text = " " + emulator.ReadShort(Globals.CHAR_ADDRESS[i] + 0x44);
                    }
                }
                TurnOrder();
            } else {
                if (!Globals.IN_BATTLE && Constants.BATTLE_UI) {
                    Constants.BATTLE_UI = false;
                    for (int i = 0; i < 5; i++) {
                        for (int x = 0; x < 6; x++) {
                            monsterDisplay[i, x].Text = "";
                        }
                    }
                    for (int i = 0; i < 3; i++) {
                        for (int x = 0; x < 9; x++) {
                            characterDisplay[i, x].Text = "";
                        }
                    }
                    lblTurnOrder.Text = "Turn Order: ";
                }
            }
        }

        public void TurnOrder() {
            try {
                object[,] battleTurns = new object[9, 3];
                int lastNumber = 0;
                object temp1;
                object temp2;
                string turnLabel = "";
                for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                    if (emulator.ReadShort(Globals.M_POINT - (i * 0x388)) > 0) {
                        battleTurns[lastNumber, 0] = monsterDisplay[i, 0].Text;
                        battleTurns[lastNumber, 1] = emulator.ReadShort(Globals.M_POINT - (i * 0x388) + 0x44);
                        lastNumber += 1;
                    }
                }
                for (int i = 0; i < 3; i++) {
                    if (Globals.PARTY_SLOT[i] < 9) {
                        if (emulator.ReadShort(Globals.C_POINT - (i * 0x388)) > 0) {
                            battleTurns[lastNumber, 0] = characterDisplay[i, 0].Text;
                            battleTurns[lastNumber, 1] = emulator.ReadShort(Globals.C_POINT - (i * 0x388) + 0x44);
                            lastNumber += 1;
                        }
                    }
                }
                for (int i = lastNumber - 1; i >= 0; i--) {
                    for (int x = 0; x < i; x++) {
                        if (Convert.ToInt16(battleTurns[x, 1]) < Convert.ToInt16((battleTurns[x + 1, 1]))) {
                            temp1 = battleTurns[x, 0];
                            temp2 = battleTurns[x, 1];
                            battleTurns[x, 0] = battleTurns[x + 1, 0];
                            battleTurns[x, 1] = battleTurns[x + 1, 1];
                            battleTurns[x + 1, 0] = temp1;
                            battleTurns[x + 1, 1] = temp2;
                        }
                    }
                }
                for (int i = 0; i < lastNumber; i++) {
                    turnLabel += battleTurns[i, 0] + "»";
                }
                if (lastNumber >= 0 && turnLabel.Length > 1) {
                    lblTurnOrder.Text = "Turn Order: " + turnLabel.Substring(0, turnLabel.Length - 1);
                } else {
                    lblTurnOrder.Text = "Turn Order: ";
                }
            } catch (Exception e) { }
        }
        #endregion

        #region HP Cap Break
        public void HPCapBreakField() {
            if (!Globals.IN_BATTLE && (Globals.BATTLE_VALUE > 4000 && Globals.BATTLE_VALUE < 9999) && maxHPTableLoaded) {
                int hp = Constants.GetAddress("CHAR_MAX_HP_START");
                int level = Constants.GetAddress("CHAR_LEVEL_START");
                int helmet = Constants.GetAddress("CHAR_HELMET_START");
                int acc = Constants.GetAddress("CHAR_ACC_START");
                byte dragon = 0x5A;
                byte ring = 0x79;
                byte healthRing = 0xB2;

                for (int i = 0; i < 3; i++) {
                    if (Globals.PARTY_SLOT[i] != hpChangeSlot[i]) {
                        hpChangeCheck[i] = 65535;
                        hpChangeSlot[i] = Globals.PARTY_SLOT[i];
                        hpChangeSave[i] = 65535;
                    }
                    if (Globals.PARTY_SLOT[i] < 9) {
                        hpChangeCheck[i] = 1.0;
                        if (emulator.ReadByte(helmet + (0x2C * hpChangeSlot[i])) == dragon) {
                            hpChangeCheck[i] += 0.5;
                        }
                        if (emulator.ReadByte(acc + (0x2C * hpChangeSlot[i])) == ring) {
                            hpChangeCheck[i] += 0.5;
                        }
                        if (emulator.ReadByte(acc + (0x2C * hpChangeSlot[i])) == healthRing) {
                            hpChangeCheck[i] += 0.5;
                        }
                        if (hpChangeCheck[i] > 1) {
                            hpChangeCheck[i] = hpChangeCheck[i] * maxHPTable[hpChangeSlot[i], emulator.ReadByte(level + (0x2C * hpChangeSlot[i])) - 1];
                        } else {
                            hpChangeCheck[i] = 65535;
                        }
                    }
                }
                //Constants.WriteDebug("Break HP: " + hpChangeSave[0] + "/" + hpChangeCheck[0] + " | " + hpChangeSave[1] + "/" + hpChangeCheck[1] + " | " + hpChangeSave[2] + "/" + hpChangeCheck[2]);
            }
        }

        public void HPCapBreakBattle() {
            if (Globals.IN_BATTLE && Globals.STATS_CHANGED && !hpCapBreakOnBattleEntry) {
                for (int i = 0; i < 3; i++) {
                    if (Globals.PARTY_SLOT[i] < 9) {
                        if (hpChangeCheck[i] != 65535) {
                            emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0x8, (ushort) hpChangeCheck[i]);
                            if (hpChangeSave[i] != 65535 && emulator.ReadShort(Globals.CHAR_ADDRESS[i]) < hpChangeSave[i]) {
                                emulator.WriteShort(Globals.CHAR_ADDRESS[i], hpChangeSave[i]);
                            }
                        }
                    }
                }
                hpCapBreakOnBattleEntry = true;
            } else {
                if (!Globals.IN_BATTLE && hpCapBreakOnBattleEntry) {
                    hpCapBreakOnBattleEntry = false;
                } else {
                    if (Globals.IN_BATTLE && Globals.STATS_CHANGED && hpCapBreakOnBattleEntry) {
                        for (int i = 0; i < 3; i++) {
                            hpChangeSave[i] = emulator.ReadShort(Globals.CHAR_ADDRESS[i]);
                            //Constants.WriteDebug("xBreak HP: " + hpChangeSave[0] + "/" + hpChangeCheck[0] + " | " + hpChangeSave[1] + "/" + hpChangeCheck[1] + " | " + hpChangeSave[2] + "/" + hpChangeCheck[2]);
                        }
                    }
                }
            }
        }

        public void LoadMaxHPTable() {
            if (!maxHPTableLoaded && !Globals.IN_BATTLE && Globals.BATTLE_VALUE < 5130) {
                byte tableSlot = 0;
                for (int i = 0; i < 9; i++) {
                    switch (i) {
                        case 0:
                            tableSlot = 1;
                            break;
                        case 1:
                        case 5:
                            tableSlot = 4;
                            break;
                        case 2:
                        case 8:
                            tableSlot = 6;
                            break;
                        case 3:
                            tableSlot = 5;
                            break;
                        case 4:
                            tableSlot = 2;
                            break;
                        case 6:
                            tableSlot = 3;
                            break;
                        case 7:
                            tableSlot = 0;
                            break;
                    }

                    for (int x = 1; x < 61; x++) {
                        maxHPTable[i, x - 1] = emulator.ReadShort(Constants.GetAddress("STAT_TABLE_HP_START") + (tableSlot * 0x1E8) + (x * 0x8));
                    }
                }
                maxHPTableLoaded = true;
            }
        }
        #endregion

        #region Kill BGM
        public void SetKillBGMState() {
            if (uiCombo["cboKillBGM"] == 0) {
                killBGMField = true;
                killedBGMField = false;
                killBGMBattle = false;
                killedBGMBattle = false;
            } else if (uiCombo["cboKillBGM"] == 1) {
                killBGMField = false;
                killedBGMField = false;
                killBGMBattle = true;
                killedBGMBattle = false;
            } else if (uiCombo["cboKillBGM"] == 2) {
                killBGMField = true;
                killedBGMField = false;
                killBGMBattle = true;
                killedBGMBattle = false;
            }
        }

        public void KillBGM() {
            ArrayList bgmScan = emulator.ScanAllAOB("53 53 73 71", 0xA8660, 0x2A865F);
            foreach (var address in bgmScan) {
                for (int i = 0; i <= 255; i++) {
                    emulator.WriteByteU((long) address + i, 0);
                }
            }
            Constants.WriteGLogOutput("Killed BGM.");
        }

        public void KillBGMField() {
            if (!Globals.IN_BATTLE && !killedBGMField && Globals.BATTLE_VALUE < 9999) {
                KillBGM();
                killedBGMField = true;
                reKilledBGMField = false;
            } else {
                if (killedBGMField && Globals.IN_BATTLE) {
                    killedBGMField = false;
                    reKilledBGMField = false;
                } else {
                    if (!reKilledBGMField && !Globals.IN_BATTLE && Globals.BATTLE_VALUE > 0) {
                        KillBGM();
                        reKilledBGMField = true;
                    }
                }
            }

        }
        public void KillBGMBattle() {
            if (Globals.IN_BATTLE && !killedBGMBattle && Globals.BATTLE_VALUE > 0) {
                emulator.WriteShort(Constants.GetAddress("MUSIC_SPEED_BATTLE"), 0);
                KillBGM();
                killedBGMBattle = true;
            } else {
                if (killedBGMBattle && !Globals.IN_BATTLE) {
                    killedBGMBattle = false;
                }
            }
        }
        #endregion

        #region Solo Mode
        public void SoloModeField() {
            if (!Globals.IN_BATTLE && !addSoloPartyMembers) {
                if (emulator.ReadByte(Constants.GetAddress("PARTY_SLOT") + 0x4) != 255 || emulator.ReadByte(Constants.GetAddress("PARTY_SLOT") + 0x8) != 255) {
                    for (int i = 0; i < 8; i++) {
                        emulator.WriteByte(Constants.GetAddress("PARTY_SLOT") + i + 0x4, 255);
                    }
                }
            }
        }

        public void SoloModeBattle() {
            if (Globals.IN_BATTLE && Globals.STATS_CHANGED && !soloModeOnBattleEntry) {
                for (int i = 0; i < 3; i++) {
                    if (Globals.PARTY_SLOT[i] < 9) {
                        if (i != uiCombo["cboSoloLeader"]) {
                            emulator.WriteShort(Globals.CHAR_ADDRESS[i], 0);
                            emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0x8, 0);
                            emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0x12C, 200);
                            emulator.WriteByte(Globals.CHAR_ADDRESS[i] + 0x45, 25);
                            //yeet
                            emulator.WriteInteger(Globals.CHAR_ADDRESS[i] + 0x16D, 255);
                            emulator.WriteInteger(Globals.CHAR_ADDRESS[i] + 0x171, 255);
                            emulator.WriteInteger(Globals.CHAR_ADDRESS[i] + 0x175, 255);
                        } else {
                            emulator.WriteInteger(Globals.CHAR_ADDRESS[i] + 0x16D, 9);
                            emulator.WriteInteger(Globals.CHAR_ADDRESS[i] + 0x171, 0);
                            emulator.WriteInteger(Globals.CHAR_ADDRESS[i] + 0x175, 0);
                        }
                    }
                }

                if (dmScripts.ContainsKey("btnSoloModeEXP") && dmScripts["btnSoloModeEXP"]) {
                    for (int i = 0; i < 5; i++) {
                        emulator.WriteShort(Constants.GetAddress("MONSTER_REWARDS_EXP") + (i * 0x1A8), (ushort) Math.Round((double) (emulator.ReadShort(Constants.GetAddress("MONSTER_REWARDS_EXP") + (i * 0x1A8)) / 3)));
                    }
                }
                soloModeOnBattleEntry = true;
            } else {
                if (!Globals.IN_BATTLE && soloModeOnBattleEntry) {
                    soloModeOnBattleEntry = false;
                    if (!alwaysAddSoloPartyMembers)
                        addSoloPartyMembers = false;
                }
            }
        }

        public void AddSoloPartyMembers() {
            if (dmScripts.ContainsKey("btnSoloMode") && dmScripts["btnSoloMode"]) {
                addSoloPartyMembers = true;
                emulator.WriteByte(Constants.GetAddress("PARTY_SLOT") + 0x4, Globals.PARTY_SLOT[0]);
                emulator.WriteByte(Constants.GetAddress("PARTY_SLOT") + 0x5, 0);
                emulator.WriteByte(Constants.GetAddress("PARTY_SLOT") + 0x6, 0);
                emulator.WriteByte(Constants.GetAddress("PARTY_SLOT") + 0x7, 0);
                emulator.WriteByte(Constants.GetAddress("PARTY_SLOT") + 0x8, Globals.PARTY_SLOT[0]);
                emulator.WriteByte(Constants.GetAddress("PARTY_SLOT") + 0x9, 0);
                emulator.WriteByte(Constants.GetAddress("PARTY_SLOT") + 0xA, 0);
                emulator.WriteByte(Constants.GetAddress("PARTY_SLOT") + 0xB, 0);
            } else {
                Constants.WritePLogOutput("Solo Mode must be turned on to add party members.");
            }
        }

        public void SwitchSoloCharacter() {
            if (dmScripts.ContainsKey("btnSoloMode") && dmScripts["btnSoloMode"]) {
                emulator.WriteByte(Constants.GetAddress("PARTY_SLOT"), uiCombo["cboSwitchChar"]);
            } else {
                Constants.WritePLogOutput("Solo Mode must be turned on to switch party members.");
            }
        }
        #endregion

        #region Element Arrow
        public void ChangeElementArrow() {
            if (uiCombo["cboElement"] == 0) {
                elementArrowElement = 128;
                elementArrowItem = 0xC3;
                Constants.WriteGLogOutput("Element Arrow changed to Fire.");
            } else if (uiCombo["cboElement"] == 1) {
                elementArrowElement = 1;
                elementArrowItem = 0xC6;
                Constants.WriteGLogOutput("Element Arrow changed to Water.");
            } else if (uiCombo["cboElement"] == 2) {
                elementArrowElement = 64;
                elementArrowItem = 0xC7;
                Constants.WriteGLogOutput("Element Arrow changed to Wind.");
            } else if (uiCombo["cboElement"] == 3) {
                elementArrowElement = 2;
                elementArrowItem = 0xC5;
                Constants.WriteGLogOutput("Element Arrow changed to Earth.");
            } else if (uiCombo["cboElement"] == 4) {
                elementArrowElement = 4;
                elementArrowItem = 0xCA;
                Constants.WriteGLogOutput("Element Arrow changed to Dark.");
            } else if (uiCombo["cboElement"] == 5) {
                elementArrowElement = 32;
                elementArrowItem = 0xC9;
                Constants.WriteGLogOutput("Element Arrow changed to Light.");
            } else if (uiCombo["cboElement"] == 6) {
                elementArrowElement = 16;
                elementArrowItem = 0xC2;
                Constants.WriteGLogOutput("Element Arrow changed to Thunder.");
            } 
        }
        #endregion

        #region Equip Changes
        public void EquipChangesField() {
            if (!Globals.IN_BATTLE && !equipChangesOnFieldEntry && (Globals.BATTLE_VALUE > 0 && Globals.BATTLE_VALUE < 9999)) {
                //Angel Robe
                WriteEquipChanges(0x1127ED, 32);
                WriteEquipChanges(0x1127EE, 64);
                WriteEquipChanges(0x1127EF, 7);
                if (emulator.ReadByte(Constants.GetAddress("CHAPTER")) >= 3) {
                    //Heat Blade
                    WriteEquipChanges(0x112032, 25);
                    //Sparkle Arrow
                    WriteEquipChanges(0x11230A, 17);
                    //Shadow Cutter
                    WriteEquipChanges(0x112182, 33);
                    //Morning Star
                    WriteEquipChanges(0x1123CE, 0);
                }
                //+1C stats
                //+8  name
                //Sabre
                WriteEquipChanges(0x113139, 128);
                WriteEquipChanges(0x11313A, 128);
                WriteEquipChanges(0x11313B, 4);
                WriteEquipChanges(0x113148, 70);
                WriteEquipChangesAOB(0x118F84, "31 00 1F 00 20 00 FF A0");
                //Spirit Eater
                WriteEquipChanges(0x113155, 128);
                WriteEquipChanges(0x113156, 128);
                WriteEquipChanges(0x113157, 128);
                WriteEquipChanges(0x113164, 75);
                WriteEquipChanges(0x113165, 50);
                WriteEquipChangesAOB(0x118F8C, "31 00 2E 00 23 00 FF A0");
                //Harpoon
                WriteEquipChanges(0x113171, 128);
                WriteEquipChanges(0x113172, 128);
                WriteEquipChanges(0x113173, 64);
                WriteEquipChanges(0x11317A, 100);
                WriteEquipChangesAOB(0x118F94, "26 00 30 00 2E 00 FF A0");
                //Element Arrow
                WriteEquipChanges(0x11318D, 128);
                WriteEquipChanges(0x11318E, 128);
                WriteEquipChanges(0x11318F, 2);
                WriteEquipChanges(0x11319C, 50);
                WriteEquipChanges(0x11319D, 50);
                WriteEquipChangesAOB(0x118F9C, "23 00 2A 00 1F 00 FF A0");
                //Dragon Buster II
                WriteEquipChanges(0x1131A9, 128);
                WriteEquipChanges(0x1131AA, 128);
                WriteEquipChanges(0x1131AB, 4);
                WriteEquipChanges(0x1131B2, 3);
                WriteEquipChanges(0x1131B8, 127);
                WriteEquipChangesAOB(0x118FA4, "22 00 20 00 17 00 FF A0");
                //Battery Glove
                WriteEquipChanges(0x1131C5, 128);
                WriteEquipChanges(0x1131C6, 128);
                WriteEquipChanges(0x1131C7, 16);
                WriteEquipChanges(0x1131CE, 80);
                WriteEquipChanges(0x1131D5, 20);
                WriteEquipChangesAOB(0x118FAC, "20 00 25 00 2A 00 FF A0");
                //Jeweled Hammer
                WriteEquipChanges(0x1131E1, 128);
                WriteEquipChanges(0x1131E2, 128);
                WriteEquipChanges(0x1131E3, 1);
                WriteEquipChanges(0x1131EA, 40);
                WriteEquipChanges(0x1131F1, 40);
                WriteEquipChangesAOB(0x118FB4, "28 00 26 00 1F 00 FF A0");
                //Giant Axe
                WriteEquipChanges(0x1131FD, 128);
                WriteEquipChanges(0x1131FE, 128);
                WriteEquipChanges(0x1131FF, 32);
                WriteEquipChanges(0x113206, 100);
                WriteEquipChanges(0x11320D, 10);
                WriteEquipChangesAOB(0x118FBC, "25 00 1F 00 36 00 FF A0");
                //Soa's Light
                WriteEquipChanges(0x113219, 255);
                WriteEquipChanges(0x11321A, 255);
                WriteEquipChanges(0x11321B, 255);
                WriteEquipChanges(0x113222, 200);
                WriteEquipChanges(0x113229, 127);
                WriteEquipChangesAOB(0x118FC4, "31 00 2A 00 27 00 FF A0");
                //Fake Legend Casque
                WriteEquipChanges(0x113235, 64);
                WriteEquipChanges(0x113236, 64);
                WriteEquipChanges(0x113237, 247);
                WriteEquipChanges(0x113242, 18);
                WriteEquipChanges(0x113247, 30);
                WriteEquipChanges(0x113248, 100);
                WriteEquipChanges(0x113249, 100);
                WriteEquipChangesAOB(0x118FCC, "24 00 2A 00 21 00 FF A0");
                //Soa's Helm
                WriteEquipChanges(0x113251, 64);
                WriteEquipChanges(0x113252, 64);
                WriteEquipChanges(0x113253, 247);
                WriteEquipChanges(0x11325E, 18);
                WriteEquipChanges(0x113263, 127);
                WriteEquipChanges(0x113264, 100);
                WriteEquipChanges(0x113265, 100);
                WriteEquipChangesAOB(0x118FD4, "31 00 26 00 2A 00 FF A0");
                //Fake Legend Armor
                WriteEquipChanges(0x11326D, 32);
                WriteEquipChanges(0x11326E, 64);
                WriteEquipChanges(0x11326F, 247);
                WriteEquipChanges(0x11327A, 10);
                WriteEquipChanges(0x11327E, 30);
                WriteEquipChangesAOB(0x118FDC, "24 00 2A 00 1F 00 FF A0");
                //Divine DG Armor
                WriteEquipChanges(0x113289, 32);
                WriteEquipChanges(0x11328A, 64);
                WriteEquipChanges(0x11328B, 247);
                WriteEquipChanges(0x113296, 10);
                WriteEquipChanges(0x11329A, 40);
                WriteEquipChanges(0x11329B, 40);
                WriteEquipChangesAOB(0x118FE4, "22 00 22 00 1F 00 FF A0");
                //Soa's Armor
                WriteEquipChanges(0x1132A5, 32);
                WriteEquipChanges(0x1132A6, 64);
                WriteEquipChanges(0x1132A7, 247);
                WriteEquipChanges(0x1132B2, 10);
                WriteEquipChanges(0x1132B6, 127);
                WriteEquipChangesAOB(0x118FEC, "31 00 1F 00 30 00 FF A0");
                //Lloyd's Boots
                WriteEquipChanges(0x1132C1, 16);
                WriteEquipChanges(0x1132C2, 64);
                WriteEquipChanges(0x1132C3, 255);
                WriteEquipChanges(0x1132CE, 21);
                WriteEquipChanges(0x1132CF, 15);
                WriteEquipChanges(0x1132D6, 15);
                WriteEquipChanges(0x1132D7, 15);
                WriteEquipChangesAOB(0x118FF4, "2A 00 20 00 31 00 FF A0");
                //Winged Shoes
                WriteEquipChanges(0x1132DD, 16);
                WriteEquipChanges(0x1132DE, 64);
                WriteEquipChanges(0x1132DF, 255);
                WriteEquipChanges(0x1132EA, 21);
                WriteEquipChanges(0x1132EB, 25);
                WriteEquipChangesAOB(0x118FFC, "35 00 25 00 31 00 FF A0");
                //Soa's Greaves
                WriteEquipChanges(0x1132F9, 16);
                WriteEquipChanges(0x1132FA, 64);
                WriteEquipChanges(0x1132FB, 255);
                WriteEquipChanges(0x113306, 21);
                WriteEquipChanges(0x113307, 40);
                WriteEquipChanges(0x11330E, 20);
                WriteEquipChanges(0x11330F, 20);
                WriteEquipChangesAOB(0x119004, "31 00 25 00 31 00 FF A0");
                //Heal Ring
                WriteEquipChanges(0x113315, 8);
                WriteEquipChanges(0x113316, 32);
                WriteEquipChanges(0x113317, 255);
                WriteEquipChanges(0x113322, 23);
                WriteEquipChangesAOB(0x11900C, "26 00 2A 00 30 00 FF A0");
                //Soa's Sash
                WriteEquipChanges(0x113331, 8);
                WriteEquipChanges(0x113332, 32);
                WriteEquipChanges(0x113333, 255);
                WriteEquipChanges(0x11333E, 23);
                WriteEquipChangesAOB(0x119014, "31 00 31 00 26 00 FF A0");
                //Soa's Ahnk
                WriteEquipChanges(0x11334D, 8);
                WriteEquipChanges(0x11334E, 32);
                WriteEquipChanges(0x11334F, 255);
                WriteEquipChanges(0x11335A, 23);
                WriteEquipChangesAOB(0x11901C, "31 00 1F 00 29 00 FF A0");
                //Soa's Health Ring
                WriteEquipChanges(0x113369, 8);
                WriteEquipChanges(0x11336A, 32);
                WriteEquipChanges(0x11336B, 255);
                WriteEquipChanges(0x113376, 23);
                WriteEquipChangesAOB(0x119024, "31 00 26 00 30 00 FF A0");
                //Soa's Mage Ring
                WriteEquipChanges(0x113385, 8);
                WriteEquipChanges(0x113386, 32);
                WriteEquipChanges(0x113387, 255);
                WriteEquipChanges(0x113392, 23);
                WriteEquipChangesAOB(0x11902C, "31 00 2B 00 30 00 FF A0");
                //Soa's Shield Ring
                WriteEquipChanges(0x1133A1, 8);
                WriteEquipChanges(0x1133A2, 32);
                WriteEquipChanges(0x1133A3, 255);
                WriteEquipChanges(0x1133AE, 23);
                WriteEquipChangesAOB(0x119034, "31 00 31 00 22 00 FF A0");
                //Soa's Siphon Ring
                WriteEquipChanges(0x1133BD, 8);
                WriteEquipChanges(0x1133BE, 32);
                WriteEquipChanges(0x1133BF, 255);
                WriteEquipChanges(0x1133CA, 23);
                WriteEquipChangesAOB(0x11903C, "31 00 31 00 30 00 FF A0");

                //legend item check?

                equipChangesOnFieldEntry = true;
            } else {
                if (Globals.IN_BATTLE && equipChangesOnFieldEntry) 
                    equipChangesOnFieldEntry = false;
            }
        }

        public void EquipChangesBattle() {
            if (Globals.IN_BATTLE && Globals.STATS_CHANGED && !equipChangesOnBattleEntry) {
                soasSiphonSlot = -1;
                for (int i = 0; i < 3; i++) {
                        if (Globals.PARTY_SLOT[i] < 9) {
                            long p = Globals.CHAR_ADDRESS[i];
                            int s = 0;

                            if (Globals.PARTY_SLOT[i] == 2 || Globals.PARTY_SLOT[i] == 8) {
                                if (emulator.ReadShort(p + 0x116) == 28 && emulator.ReadByte(Constants.GetAddress("CHAPTER")) >= 3) { //Sparkle Arrow
                                    emulator.WriteShort(p + 0x2C, (ushort) (emulator.ReadShort(p + 0x2C) + 8));
                                }

                                bool boost = false;
                                double boostAmount = 0.0;
                                byte level = 0;
                                if (Globals.PARTY_SLOT[i] == 2) {
                                    if (emulator.ReadByte(0xBAF5E) >= 10) {
                                        boost = true;
                                        level = emulator.ReadByte(0xBAF5E);
                                    }
                                } else {
                                    if (emulator.ReadByte(0xBB066) >= 10) {
                                        boost = true;
                                        level = emulator.ReadByte(0xBB066);
                                    }
                                }
                                if (boost) {
                                    if (level >= 28) {
                                        boostAmount = 2.15;
                                    } else if (level >= 20) {
                                        boostAmount = 1.9;
                                    } else {
                                        boostAmount = 1.6;
                                    }
                                    if (emulator.ReadShort(p + 0x116) == 32) {
                                        emulator.WriteShort(p + 0x2C, (ushort) Math.Round(emulator.ReadShort(p + 0x2C) * 1.4));
                                    } else {
                                        emulator.WriteShort(p + 0x2C, (ushort) Math.Round(emulator.ReadShort(p + 0x2C) * boostAmount));
                                    }
                                }
                            }

                            if (Globals.PARTY_SLOT[i] == 2 && emulator.ReadByte(0xBAF5E) >= 30) {
                                emulator.WriteShort(p + 0x30, (ushort) Math.Round(emulator.ReadShort(p + 0x30) * 1.12));
                            }

                            if (Globals.PARTY_SLOT[i] == 8 && emulator.ReadByte(0xBB066) >= 30) {
                                emulator.WriteShort(p + 0x30, (ushort) Math.Round(emulator.ReadShort(p + 0x30) * 1.12));
                            }

                            if (Globals.PARTY_SLOT[i] == 3 && emulator.ReadByte(0xBAF8A) >= 30) {
                                emulator.WriteShort(p + 0x30, (ushort) Math.Round(emulator.ReadShort(p + 0x30) * 1.1));
                            }

                            if (Globals.PARTY_SLOT[i] == 6 && emulator.ReadByte(0xBB00E) >= 30) {
                                emulator.WriteShort(p + 0x30, (ushort) Math.Round(emulator.ReadShort(p + 0x30) * 1.26));
                            }

                            if (emulator.ReadShort(p + 0x11E) == 149) { //Phantom Shield
                                s = emulator.ReadShort(p + 0x30);
                                emulator.WriteByte(p + 0x30, 0);
                                emulator.WriteByte(p + 0x31, 0);
                                if (emulator.ReadShort(p + 0x11A) == 74) {
                                    emulator.WriteShort(p + 0x30, (ushort) (Math.Ceiling(s * 1.1)));
                                } else {
                                    emulator.WriteShort(p + 0x30, (ushort) (Math.Ceiling(s * 0.7)));
                                }
                                s = emulator.ReadShort(p + 0x32);
                                emulator.WriteByte(p + 0x32, 0);
                                emulator.WriteByte(p + 0x33, 0);
                                if (emulator.ReadShort(p + 0x118) == 89) {
                                    emulator.WriteShort(p + 0x32, (ushort) (Math.Ceiling(s * 1.1)));
                                } else {
                                    emulator.WriteShort(p + 0x32, (ushort) (Math.Ceiling(s * 0.7)));
                                }
                            }

                            if (emulator.ReadShort(p + 0x116) == 9) { //Tomahawk
                                emulator.WriteByte(p + 0x14, 2);
                            }
                            if (emulator.ReadShort(p + 0x118) == 77) { //Sallet
                                s = emulator.ReadShort(p + 0x36);
                                emulator.WriteShort(p + 0x36, (ushort) (s + 10));
                            }

                            if (emulator.ReadShort(p + 0x118) == 84) { //Tiara
                                s = emulator.ReadShort(p + 0x34);
                                emulator.WriteShort(p + 0x34, (ushort) (s + 10));
                            }

                            if (emulator.ReadShort(p + 0x11E) == 150) { //Dragon Shield
                                s = emulator.ReadShort(p + 0x30);
                                emulator.WriteByte(p + 0x30, 0);
                                emulator.WriteByte(p + 0x31, 0);
                                if (emulator.ReadShort(p + 0x11A) == 74) {
                                    emulator.WriteShort(p + 0x30, (ushort) (Math.Ceiling(s * 1.2)));
                                } else {
                                    emulator.WriteShort(p + 0x30, (ushort) (Math.Ceiling(s * 0.7)));
                                }
                            }

                            if (emulator.ReadShort(p + 0x11E) == 151) { //Angel Scarf
                                s = emulator.ReadShort(p + 0x32);
                                emulator.WriteByte(p + 0x32, 0);
                                emulator.WriteByte(p + 0x33, 0);
                                if (emulator.ReadShort(p + 0x118) == 89) {
                                    emulator.WriteShort(p + 0x32, (ushort) (Math.Ceiling(s * 1.2)));
                                } else {
                                    emulator.WriteShort(p + 0x32, (ushort) (Math.Ceiling(s * 0.7)));
                                }
                            }

                            if (emulator.ReadByte(Constants.GetAddress("CHAPTER")) >= 3) {
                                if (emulator.ReadShort(p + 0x116) == 2) { //Heat Blade
                                    emulator.WriteShort(p + 0x2C, (ushort) (emulator.ReadShort(p + 0x2C) + 7));
                                }

                                if (emulator.ReadShort(p + 0x116) == 14) { //Shadow Cutter
                                    emulator.WriteShort(p + 0x2C, (ushort) (emulator.ReadShort(p + 0x2C) + 9));
                                }

                                if (emulator.ReadShort(p + 0x116) == 35) { //Morning Star
                                    emulator.WriteShort(p + 0x2C, (ushort) (emulator.ReadShort(p + 0x2C) - 20));
                                    emulator.WriteByte(p + 0x14, 1);
                                }
                            }

                            emulator.WriteByte(p + 0x1A, 0); //Elemental Null

                            if (emulator.ReadShort(p + 0x116) == 158 && Globals.PARTY_SLOT[i] == 3) { //Sabre
                                emulator.WriteShort(p + 0x2C, (ushort) (emulator.ReadShort(p + 0x2C) + 70));
                            }

                            if (emulator.ReadShort(p + 0x116) == 159 && Globals.PARTY_SLOT[i] == 0) { //Spirit Eater
                                spiritEaterSP = 35;
                                if (dmScripts.ContainsKey("btnHalfSP") && dmScripts["btnHalfSP"]) {
                                    spiritEaterSP = 15;
                                }
                                spiritEaterSaveSP = emulator.ReadShort(p + 0x130);
                                emulator.WriteShort(p + 0x2C, (ushort) (emulator.ReadShort(p + 0x2C) + 75));
                                emulator.WriteShort(p + 0x2E, (ushort) (emulator.ReadShort(p + 0x2E) + 50));
                                if (spiritEaterSaveSP < spiritEaterSP) {
                                    emulator.WriteShortU(p + 0x130, (ushort) (65536 - (spiritEaterSP - spiritEaterSaveSP)));
                                } else {
                                    emulator.WriteShortU(p + 0x130, (ushort) (spiritEaterSaveSP - spiritEaterSP));
                                }
                            }

                            if (emulator.ReadShort(p + 0x116) == 160 && (Globals.PARTY_SLOT[i] == 1 || Globals.PARTY_SLOT[i] == 5)) { //Harpoon
                                emulator.WriteShort(p + 0x2C, (ushort) (emulator.ReadShort(p + 0x2C) + 100));
                            }

                            if (emulator.ReadShort(p + 0x116) == 161 && (Globals.PARTY_SLOT[i] == 2 || Globals.PARTY_SLOT[i] == 8)) { //Element Arrow
                                emulator.WriteShort(p + 0x14, elementArrowElement);
                                emulator.WriteShort(p + 0x2C, (ushort) (emulator.ReadShort(p + 0x2C) + 50));
                                emulator.WriteShort(p + 0x2E, (ushort) (emulator.ReadShort(p + 0x2E) + 50));
                                elementArrowLastAction = 255;
                                elementArrowTurns = 0;
                            }

                            if (emulator.ReadShort(p + 0x116) == 162 && Globals.PARTY_SLOT[i] == 3) { //Dragon Buster II
                                emulator.WriteShort(p + 0x2C, (ushort) (emulator.ReadShort(p + 0x2C) + 130));
                            }

                            if (emulator.ReadShort(p + 0x116) == 163 && Globals.PARTY_SLOT[i] == 4) { //Battery Glove
                                emulator.WriteShort(p + 0x2C, (ushort) (emulator.ReadShort(p + 0x2C) + 80));
                                emulator.WriteShort(p + 0x2E, (ushort) (emulator.ReadShort(p + 0x2E) + 20));
                                gloveLastAction = 0;
                                gloveCharge = 0;
                            }

                            if (emulator.ReadShort(p + 0x116) == 164 && Globals.PARTY_SLOT[i] == 6) { //Jeweled Hammer
                                emulator.WriteShort(p + 0x2C, (ushort) (emulator.ReadShort(p + 0x2C) + 40));
                                emulator.WriteShort(p + 0x2E, (ushort) (emulator.ReadShort(p + 0x2E) + 40));
                                if (jeweledHammer) {
                                    if (Constants.REGION == Region.USA) {
                                        emulator.WriteAOB(0x51CA8, "35 00 39 00 4C 00 3D 00 4A 00 00 00 31 00 32 00 30 00 00 00 1D 00 15 00 15 00 0F 00 FF A0");
                                        emulator.WriteAOB(0x51D3C, "35 00 39 00 4C 00 3D 00 4A 00 00 00 31 00 32 00 30 00 00 00 1D 00 1D 00 15 00 0F 00 FF A0");
                                        emulator.WriteAOB(0x51D64, "35 00 39 00 4C 00 3D 00 4A 00 00 00 31 00 32 00 30 00 00 00 17 00 16 00 15 00 15 00 0F 00 FF A0");
                                    }
                                    emulator.WriteByte(0xFA1DE, 50);
                                    emulator.WriteByte(0xFA1EA, 100);
                                    emulator.WriteByte(0xFA202, 99);
                                    emulator.WriteByte(0xFA20E, 150);
                                }
                            }

                            if (emulator.ReadShort(p + 0x116) == 165 && Globals.PARTY_SLOT[i] == 7) { //Giant Axe
                                emulator.WriteShort(p + 0x2C, (ushort) (emulator.ReadShort(p + 0x2C) + 100));
                                emulator.WriteShort(p + 0x2E, (ushort) (emulator.ReadShort(p + 0x2E) + 10));
                                axeLastAction = 0;
                            }

                            if (emulator.ReadShort(p + 0x116) == 166) { //Soa's Light
                                emulator.WriteShort(p + 0x2C, (ushort) (emulator.ReadShort(p + 0x2C) + 200));
                                emulator.WriteShort(p + 0x2E, (ushort) (emulator.ReadShort(p + 0x2E) + 140));
                                emulator.WriteShort(p + 0x120, 65436);
                                emulator.WriteShort(p + 0x130, 100);
                                for (int x = 0; x < 3; x++) {
                                    if (x != i && Globals.PARTY_SLOT[x] < 9) {
                                        emulator.WriteShort(Globals.CHAR_ADDRESS[x] + 0x30, (ushort) Math.Round(emulator.ReadShort(Globals.CHAR_ADDRESS[x] + 0x30) * 0.7));
                                        emulator.WriteShort(Globals.CHAR_ADDRESS[x] + 0x32, (ushort) Math.Round(emulator.ReadShort(Globals.CHAR_ADDRESS[x] + 0x32) * 0.7));
                                    }
                                }
                            }

                            if (emulator.ReadShort(p + 0x118) == 167) { //Fake Legend Casque
                                emulator.WriteShort(p + 0x32, (ushort) (emulator.ReadShort(p + 0x32) + 30));
                                emulator.WriteShort(p + 0x34, (ushort) (emulator.ReadShort(p + 0x34) + 100));
                                emulator.WriteShort(p + 0x36, (ushort) (emulator.ReadShort(p + 0x36) + 100));
                            }

                            if (emulator.ReadShort(p + 0x118) == 168) { //Soa's Helm
                                emulator.WriteShort(p + 0x32, (ushort) (emulator.ReadShort(p + 0x32) + 200));
                                emulator.WriteShort(p + 0x12E, (ushort) (emulator.ReadShort(p + 0x12E) + 20));
                                emulator.WriteShort(p + 0x34, (ushort) (emulator.ReadShort(p + 0x34) + 100));
                                emulator.WriteShort(p + 0x36, (ushort) (emulator.ReadShort(p + 0x36) + 100));

                                for (int x = 0; x < 3; x++) {
                                    if (x != i && Globals.PARTY_SLOT[x] < 9) {
                                        emulator.WriteShort(Globals.CHAR_ADDRESS[x] + 0x2C, (ushort) Math.Round(emulator.ReadShort(Globals.CHAR_ADDRESS[x] + 0x2C) * 0.7));
                                    }
                                }
                            }

                            if (emulator.ReadShort(p + 0x11A) == 169) { //Fake Legend Armor
                                emulator.WriteShort(p + 0x30, (ushort) (emulator.ReadShort(p + 0x30) + 30));
                            }

                            if (emulator.ReadShort(p + 0x11A) == 170) { //Divine DG Armor
                                emulator.WriteShort(p + 0x30, (ushort) (emulator.ReadShort(p + 0x30) + 50));
                                emulator.WriteShort(p + 0x32, (ushort) (emulator.ReadShort(p + 0x32) + 50));
                                emulator.WriteShort(p + 0x122, (ushort) (emulator.ReadShort(p + 0x122) + 20));
                                emulator.WriteShort(p + 0x124, (ushort) (emulator.ReadShort(p + 0x124) + 10));
                                emulator.WriteShort(p + 0x126, (ushort) (emulator.ReadShort(p + 0x126) + 20));
                                emulator.WriteShort(p + 0x128, (ushort) (emulator.ReadShort(p + 0x128) + 10));
                            }

                            if (emulator.ReadShort(p + 0x11A) == 171) { //Soa's Armor
                                emulator.WriteShort(p + 0x30, (ushort) (emulator.ReadShort(p + 0x30) + 200));
                                emulator.WriteShort(p + 0x12C, (ushort) (emulator.ReadShort(p + 0x12C) + 20));
                                for (int x = 0; x < 3; x++) {
                                    if (x != i && Globals.PARTY_SLOT[x] < 9) {
                                        emulator.WriteShort(Globals.CHAR_ADDRESS[x] + 0x2E, (ushort) Math.Round(emulator.ReadShort(Globals.CHAR_ADDRESS[x] + 0x2E) * 0.7));
                                    }
                                }
                            }

                            if (emulator.ReadShort(p + 0x11C) == 172) { //Lloyd's Boots
                                emulator.WriteShort(p + 0x2A, (ushort) (emulator.ReadShort(p + 0x2A) + 15));
                                emulator.WriteShort(p + 0x38, (ushort) (emulator.ReadShort(p + 0x38) + 15));
                                emulator.WriteShort(p + 0x3A, (ushort) (emulator.ReadShort(p + 0x3A) + 15));
                            }

                            if (emulator.ReadShort(p + 0x11C) == 173) { //Winged Shoes
                                emulator.WriteShort(p + 0x2A, (ushort) (emulator.ReadShort(p + 0x2A) + 25));
                            }

                            if (emulator.ReadShort(p + 0x11C) == 174) { //Soa's Greaves
                                emulator.WriteShort(p + 0x2A, (ushort) (emulator.ReadShort(p + 0x2A) + 40));
                                emulator.WriteShort(p + 0x38, (ushort) (emulator.ReadShort(p + 0x38) + 40));
                                emulator.WriteShort(p + 0x3A, (ushort) (emulator.ReadShort(p + 0x3A) + 40));
                                for (int x = 0; x < 3; x++) {
                                    if (x != i && Globals.PARTY_SLOT[x] < 9) {
                                        emulator.WriteShort(Globals.CHAR_ADDRESS[x] + 0x2A, (ushort) (emulator.ReadShort(Globals.CHAR_ADDRESS[x] + 0x2A) - 25));
                                    }
                                }
                            }

                            if (emulator.ReadShort(p + 0x11E) == 175) { //Heal Ring
                                emulator.WriteShort(p + 0x12C, (ushort) (emulator.ReadShort(p + 0x12C) + 7));
                                emulator.WriteShort(p + 0x12E, (ushort) (emulator.ReadShort(p + 0x12E) + 7));
                                emulator.WriteShort(p + 0x130, (ushort) (emulator.ReadShort(p + 0x130) + 7));
                                if (Globals.PARTY_SLOT[i] == 2 || Globals.PARTY_SLOT[i] == 8) {
                                    recoveryRateSave = emulator.ReadByte(p + 0x12C);
                                }
                            }

                            if (emulator.ReadShort(p + 0x11E) == 176) { //Soa's Sash
                                emulator.WriteShort(p + 0x120, (ushort) (emulator.ReadShort(p + 0x120) + 100));
                                for (int x = 0; x < 3; x++) {
                                    if (x != i && Globals.PARTY_SLOT[x] < 9) {
                                        ushort spMulti = emulator.ReadShort(Globals.CHAR_ADDRESS[x] + 0x120);
                                        if (spMulti == 0) {
                                            emulator.WriteShort(Globals.CHAR_ADDRESS[x] + 0x120, 65486);
                                        } else {
                                            emulator.WriteShort(Globals.CHAR_ADDRESS[x] + 0x120, (ushort) (emulator.ReadShort(Globals.CHAR_ADDRESS[x] + 0x120) - 50));
                                        }
                                    }
                                }
                            }

                            if (emulator.ReadShort(p + 0x11E) == 177) { //Soa's Ahnk
                                if (dmScripts.ContainsKey("btnSoloMode") && dmScripts["btnSoloMode"]) {
                                    emulator.WriteShort(p + 0x132, (ushort) (emulator.ReadShort(p + 0x132) + 50));
                                } else {
                                    emulator.WriteShort(p + 0x132, 100);
                                }
                            }

                            if (emulator.ReadShort(p + 0x11E) == 178) { //Soa's Health Ring
                                for (int x = 0; x < 3; x++) {
                                    if (x != i && Globals.PARTY_SLOT[x] < 9) {
                                        emulator.WriteShort(Globals.CHAR_ADDRESS[x] + 0x8, (ushort) Math.Round(emulator.ReadShort(Globals.CHAR_ADDRESS[x] + 0x8) * 0.75));

                                        if (emulator.ReadShort(Globals.CHAR_ADDRESS[x]) > emulator.ReadShort(Globals.CHAR_ADDRESS[x] + 0x8)) {
                                            emulator.WriteShort(Globals.CHAR_ADDRESS[x], emulator.ReadShort(Globals.CHAR_ADDRESS[x] + 0x8));
                                        }
                                    }
                                }
                            }

                            if (emulator.ReadShort(p + 0x11E) == 179) { //Soa's Mage Ring
                                emulator.WriteShort(p + 0x4, (ushort) (emulator.ReadShort(p + 0x4) * 3));
                                emulator.WriteShort(p + 0xA, (ushort) (emulator.ReadShort(p + 0xA) * 3));
                                for (int x = 0; x < 3; x++) {
                                    if (x != i && Globals.PARTY_SLOT[x] < 9) {
                                        emulator.WriteShort(Globals.CHAR_ADDRESS[x] + 0xA, (ushort) Math.Round(emulator.ReadShort(Globals.CHAR_ADDRESS[x] + 0xA) * 0.5));

                                        if (emulator.ReadShort(Globals.CHAR_ADDRESS[x] + 0x4) > emulator.ReadShort(Globals.CHAR_ADDRESS[x] + 0xA)) {
                                            emulator.WriteShort(Globals.CHAR_ADDRESS[x] + 0x4, emulator.ReadShort(Globals.CHAR_ADDRESS[x] + 0xA));
                                        }
                                    }
                                }
                            }

                            if (emulator.ReadShort(p + 0x11E) == 180) { //Soa's Shield Ring
                                emulator.WriteShort(p + 0x30, 1);
                                emulator.WriteShort(p + 0x32, 1);
                                emulator.WriteShort(p + 0x38, 90);
                                emulator.WriteShort(p + 0x3A, 90);
                                for (int x = 0; x < 3; x++) {
                                    if (x != i && Globals.PARTY_SLOT[x] < 9) {
                                        emulator.WriteShort(Globals.CHAR_ADDRESS[x] + 0x34, (ushort) Math.Round(emulator.ReadShort(Globals.CHAR_ADDRESS[x] + 0x34) * 0.8));
                                        emulator.WriteShort(Globals.CHAR_ADDRESS[x] + 0x36, (ushort) Math.Round(emulator.ReadShort(Globals.CHAR_ADDRESS[x] + 0x36) * 0.8));
                                    }
                                }
                            }

                            if (emulator.ReadShort(p + 0x11E) == 181) { //Soa's Siphon Ring
                                soasSiphonSlot = i;
                                emulator.WriteShort(p + 0x2E, (ushort) (emulator.ReadShort(p + 0x2E) * 2));
                                emulator.WriteShort(p + 0xA6, (ushort) Math.Round(emulator.ReadShort(p + 0xA6) * 0.3));
                                for (int x = 0; x < 3; x++) {
                                    if (x != i && Globals.PARTY_SLOT[x] < 9) {
                                        emulator.WriteShort(Globals.CHAR_ADDRESS[x] + 0x2E, (ushort) Math.Round(emulator.ReadShort(Globals.CHAR_ADDRESS[x] + 0x2E) * 0.8));
                                    }
                                }
                            }

                            if (emulator.ReadShort(p + 0x11A) == 74) { //Armor of Legend
                                emulator.WriteByte(p + 0x10C, 0);
                                emulator.WriteShort(p + 0x38, (ushort) (emulator.ReadShort(p + 0x38) - 50));
                                if (Globals.PARTY_SLOT[i] == 0) {
                                    emulator.WriteShort(p + 0x30, (ushort) (emulator.ReadShort(p + 0x30) + 41 - 127));
                                    emulator.WriteShort(p + 0x32, (ushort) (emulator.ReadShort(p + 0x32) + 40));
                                } else if (Globals.PARTY_SLOT[i] == 1 || Globals.PARTY_SLOT[i] == 5) {
                                    emulator.WriteShort(p + 0x30, (ushort) (emulator.ReadShort(p + 0x30) + 54 - 127));
                                    emulator.WriteShort(p + 0x32, (ushort) (emulator.ReadShort(p + 0x32) + 27));
                                } else if (Globals.PARTY_SLOT[i] == 2 || Globals.PARTY_SLOT[i] == 8) {
                                    emulator.WriteShort(p + 0x30, (ushort) (emulator.ReadShort(p + 0x30) + 27 - 127));
                                    emulator.WriteShort(p + 0x32, (ushort) (emulator.ReadShort(p + 0x32) + 80));
                                } else if (Globals.PARTY_SLOT[i] == 3) {
                                    emulator.WriteShort(p + 0x30, (ushort) (emulator.ReadShort(p + 0x30) + 41 - 127));
                                    emulator.WriteShort(p + 0x32, (ushort) (emulator.ReadShort(p + 0x32) + 42));
                                } else if (Globals.PARTY_SLOT[i] == 4) {
                                    emulator.WriteShort(p + 0x30, (ushort) (emulator.ReadShort(p + 0x30) + 45 - 127));
                                    emulator.WriteShort(p + 0x32, (ushort) (emulator.ReadShort(p + 0x32) + 40));
                                } else if (Globals.PARTY_SLOT[i] == 6) {
                                    emulator.WriteShort(p + 0x30, (ushort) (emulator.ReadShort(p + 0x30) + 30 - 127));
                                    emulator.WriteShort(p + 0x32, (ushort) (emulator.ReadShort(p + 0x32) + 54));
                                } else if (Globals.PARTY_SLOT[i] == 7) {
                                    emulator.WriteShort(p + 0x30, (ushort) (emulator.ReadShort(p + 0x30) + 88 - 127));
                                    emulator.WriteShort(p + 0x32, (ushort) (emulator.ReadShort(p + 0x32) + 23));
                                }
                            }

                            if (emulator.ReadShort(p + 0x118) == 89) { //Legend Casque
                                emulator.WriteByte(p + 0x10E, 0);
                                emulator.WriteShort(p + 0x3A, (ushort) (emulator.ReadShort(p + 0x3A) - 50));
                                emulator.WriteShort(p + 0x32, (ushort) (emulator.ReadShort(p + 0x32) + 70 - 127));
                            }


                            guardStatusDF[i] = 0;
                            guardStatusMDF[i] = 0;
                            lGuardStatusDF[i] = 0;
                            lGuardStatusMDF[i] = 0;
                            lGuardStateDF[i] = false;
                            lGuardStateMDF[i] = false;
                            sGuardStatusDF[i] = false;
                            sGuardStatusMDF[i] = false;
                            originalCharacterStats[i, 0] = emulator.ReadShort(p + 0x8); //MAX HP
                            originalCharacterStats[i, 1] = emulator.ReadShort(p + 0x2C); //AT
                            originalCharacterStats[i, 2] = emulator.ReadShort(p + 0x2E); //MAT
                            originalCharacterStats[i, 3] = emulator.ReadShort(p + 0x30); //DF
                            originalCharacterStats[i, 4] = emulator.ReadShort(p + 0x32); //MDF
                            originalCharacterStats[i, 5] = emulator.ReadShort(p + 0x2A); //SPD
                            originalCharacterStats[i, 6] = emulator.ReadShort(p + 0x122); //SP HEAL PHYSICAL
                            originalCharacterStats[i, 7] = emulator.ReadShort(p + 0x124); //MP HEAL PHYSICAL
                            originalCharacterStats[i, 8] = emulator.ReadShort(p + 0x126); //SP HEAL MAGIC
                            originalCharacterStats[i, 9] = emulator.ReadShort(p + 0x128); //MP HEAL MAGIC
                        }
                }
                equipChangesOnBattleEntry = true;
            } else {
                if (!Globals.IN_BATTLE && equipChangesOnBattleEntry) {
                    equipChangesOnBattleEntry = false;
                    checkHarpoon = false;
                } else {
                    if (Globals.IN_BATTLE && Globals.STATS_CHANGED /*&& equipChangesLoop*/) { //Battle Loop
                        for (int i = 0; i < 3; i++) {
                            long p = Globals.CHAR_ADDRESS[i];
                            if (emulator.ReadShort(p + 0x116) == 159 && Globals.PARTY_SLOT[i] == 0) { //Spirit Eater
                                if (emulator.ReadShort(p + 0x2) == (emulator.ReadByte(Constants.GetAddress("DART_DRAGOON_LEVEL")) * 100)) {
                                    emulator.WriteShort(p + 0x130, spiritEaterSaveSP);
                                } else {
                                    if (spiritEaterSaveSP < spiritEaterSP) {
                                        emulator.WriteShort(p + 0x130, (ushort) (65536 - (spiritEaterSP - spiritEaterSaveSP)));
                                    } else {
                                        emulator.WriteShort(p + 0x130, (ushort) (spiritEaterSaveSP - spiritEaterSP));
                                    }
                                }
                            }

                            if (emulator.ReadShort(p + 0x116) == 160 && (Globals.PARTY_SLOT[i] == 1 || Globals.PARTY_SLOT[i] == 5)) { //Harpoon
                                if (emulator.ReadByte(p - 0xA8) == 10 && emulator.ReadShort(p + 0x2) >= 400) {
                                    checkHarpoon = true;
                                    if (emulator.ReadShort(p + 0x2) == 500) {
                                        emulator.WriteAOB(p + 0xC0, "00 00 00 04");
                                        emulator.WriteShort(p + 0x2, 200);
                                        emulator.WriteByte(Constants.GetAddress("DRAGOON_TURNS") + i * 4, 2);
                                    } else {
                                        emulator.WriteAOB(p + 0xC0, "00 00 00 03");
                                        emulator.WriteShort(p + 0x2, 100);
                                        emulator.WriteByte(Constants.GetAddress("DRAGOON_TURNS") + i * 4, 1);
                                    }
                                }

                                if (emulator.ReadByte(Constants.GetAddress("DRAGOON_TURNS") + i * 4) == 0) {
                                    checkHarpoon = false;
                                }
                            }

                            if (emulator.ReadShort(p + 0x116) == 161 && (Globals.PARTY_SLOT[i] == 2 || Globals.PARTY_SLOT[i] == 8)) { //Element Arrow
                                if (elementArrowLastAction != emulator.ReadByte(p - 0xA8)) {
                                    if (elementArrowLastAction == 8 && emulator.ReadByte(p - 0xA8) == 136) {
                                        //old method ...
                                    } else {
                                        elementArrowLastAction = emulator.ReadByte(p - 0xA8);
                                        if (elementArrowLastAction == 8) {
                                            emulator.WriteShort(p + 0x14, elementArrowElement);
                                            elementArrowTurns += 1;
                                        } else {
                                            if (elementArrowLastAction == 10) {
                                                emulator.WriteShort(p + 0x14, 0);
                                            }
                                            if (elementArrowTurns == 4) {
                                                elementArrowTurns = 0;
                                                if (emulator.ReadInteger(Constants.GetAddress("GOLD")) >= 100) {
                                                    for (int x = 0; x < inventorySize; x++) {
                                                        if (emulator.ReadByte(0xBAEB1 + x + EquipChangesInventoryOffset()) == 255) {
                                                            emulator.WriteByte(0xBAEB1 + x + EquipChangesInventoryOffset(), elementArrowItem);
                                                            emulator.WriteByte(0xBADAE + EquipChangesInventoryOffset(), emulator.ReadByte(0xBADAE + EquipChangesInventoryOffset()) + 1);
                                                            emulator.WriteInteger(Constants.GetAddress("GOLD"), emulator.ReadInteger(Constants.GetAddress("GOLD")) - 100);
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if (emulator.ReadShort(p + 0x116) == 162 && Globals.PARTY_SLOT[i] == 3) { //Dragon Buster II
                                if (emulator.ReadByte(p - 0xA8) == 136) {
                                    if (emulator.ReadShort(Constants.GetAddress("DAMAGE_SLOT1")) != 0) {
                                        emulator.WriteShort(p, (ushort) Math.Min(emulator.ReadShort(p) + Math.Round(emulator.ReadShort(Constants.GetAddress("DAMAGE_SLOT1")) * 0.02) + 2, emulator.ReadShort(p + 0x8)));
                                        emulator.WriteShort(Constants.GetAddress("DAMAGE_SLOT1"), 0);
                                    /*} else {
                                        emulator.WriteShort(Constants.GetAddress("DAMAGE_SLOT1"), 0);*/
                                    }
                                }
                            }

                            if (emulator.ReadShort(p + 0x116) == 163 && Globals.PARTY_SLOT[i] == 4) { //Battery Glove
                                if ((emulator.ReadByte(p - 0xA8) == 136 || emulator.ReadByte(p - 0xA8) == 26) &&
                                (gloveLastAction != 136 && gloveLastAction != 26)) {
                                    gloveCharge += 1;
                                    if (gloveCharge == 7) {
                                        emulator.WriteShort(p + 0x2C, (ushort) Math.Round(originalCharacterStats[i, 1] * 2.5));
                                    } else {
                                        if (gloveCharge > 7) {
                                            gloveCharge = 1;
                                            emulator.WriteShort(p + 0x2C, (ushort) originalCharacterStats[i, 1]);
                                        }
                                    }

                                    gloveLastAction = emulator.ReadByte(p - 0xA8);
                                }

                                //stsProgram.Text = "Glove Charge: " & gloveCharge & " / " & gloveLastAction & " / " & emulator.ReadByte(p - 0xA8) & " / " &
                                //    (emulator.ReadByte(p - 0xA8) = 136 Or emulator.ReadByte(p - 0xA8) = 138) & " / " &
                                //    (gloveLastAction <> 136 Or gloveLastAction <> 138) & " / " &
                                //    ((emulator.ReadByte(p - 0xA8) = 136 Or emulator.ReadByte(p - 0xA8) = 138) And
                                //    (gloveLastAction <> 136 And gloveLastAction <> 138))
                            }

                            if (emulator.ReadShort(p + 0x116) == 165 && Globals.PARTY_SLOT[i] == 7) { //Giant Axe
                                if (emulator.ReadByte(p - 0xA8) == 136 && axeLastAction != emulator.ReadByte(p - 0xA8)) {
                                    axeLastAction = emulator.ReadByte(p - 0xA8);
                                    if (new Random().Next(0, 9) < 2) {
                                        emulator.WriteByte(p + 0x4C, 1);
                                    }
                                } else {
                                    axeLastAction = emulator.ReadByte(p - 0xA8);
                                }
                            }

                            if (emulator.ReadShort(p + 0x118) == 167) { //Fake Legend Casque
                                if (emulator.ReadByte(p + 0x4C) == 1 && guardStatusMDF[i] == 0) {
                                    if (new Random().Next(0, 9) < 3) {
                                        emulator.WriteShort(p + 0x32, (ushort) (emulator.ReadShort(p + 0x32) + 40));
                                        guardStatusMDF[i] = 1;
                                    }
                                }
                                if ((emulator.ReadByte(p - 0xA8) == 8 || emulator.ReadByte(p - 0xA8) == 10) && guardStatusMDF[i] == 1) {
                                    emulator.WriteShort(p + 0x32, (ushort) originalCharacterStats[i, 4]);
                                    guardStatusMDF[i] = 0;
                                }
                            }

                            if (emulator.ReadShort(p + 0x11A) == 169) { //Fake Legend Armor
                                if (emulator.ReadByte(p + 0x4C) == 1 && guardStatusDF[i] == 0) {
                                    if (new Random().Next(0, 9) < 3) {
                                        emulator.WriteShort(p + 0x30, (ushort) (emulator.ReadShort(p + 0x30) + 40));
                                        guardStatusDF[i] = 1;
                                    }
                                }
                                if ((emulator.ReadByte(p - 0xA8) == 8 || emulator.ReadByte(p - 0xA8) == 10) && guardStatusDF[i] == 1) {
                                    emulator.WriteShort(p + 0x30, (ushort) originalCharacterStats[i, 3]);
                                    guardStatusDF[i] = 0;
                                }
                            }

                            if (emulator.ReadShort(p + 0x11A) == 170) { //Divine DG Armor
                                emulator.WriteShort(p + 0x122, (ushort) originalCharacterStats[i, 6]);
                                emulator.WriteShort(p + 0x124, (ushort) originalCharacterStats[i, 7]);
                                emulator.WriteShort(p + 0x126, (ushort) originalCharacterStats[i, 8]);
                                emulator.WriteShort(p + 0x128, (ushort) originalCharacterStats[i, 9]);
                            }

                            if (emulator.ReadShort(p + 0x11C) == 172 || emulator.ReadShort(p + 0x11C) == 173 || emulator.ReadShort(p + 0x11C) == 174) { //Lloyd's Boots/ Winged Shoes / Soa's Greaves
                                if (emulator.ReadByte(p + 0xC1) > 0) {
                                    emulator.WriteShort(p + 0x2A, (ushort) (originalCharacterStats[i, 5] * 2));
                                } else {
                                    if (emulator.ReadByte(p + 0xC3) > 0) {
                                        emulator.WriteShort(p + 0x2A, (ushort) (originalCharacterStats[i, 5] / 2));
                                    } else {
                                        emulator.WriteShort(p + 0x2A, (ushort) (originalCharacterStats[i, 5]));
                                    }
                                }

                                if (emulator.ReadShort(p + 0x11C) == 174) {
                                    for (int x = 0; x < 3; x++) {
                                        if (x != i && Globals.PARTY_SLOT[i] < 9 && emulator.ReadShort(Globals.CHAR_ADDRESS[x]) > 0) {
                                            if (emulator.ReadShort(Globals.CHAR_ADDRESS[x]) > 0) {
                                                if (emulator.ReadByte(Globals.CHAR_ADDRESS[x] + 0xC1) > 0) {
                                                    emulator.WriteShort(Globals.CHAR_ADDRESS[x] + 0x2A, (ushort) (originalCharacterStats[x, 5] * 2));
                                                } else {
                                                    if (emulator.ReadByte(Globals.CHAR_ADDRESS[x] + 0xC3) > 0) {
                                                        emulator.WriteShort(Globals.CHAR_ADDRESS[x] + 0x2A, (ushort) (originalCharacterStats[x, 5] / 2));
                                                    } else {
                                                        emulator.WriteShort(Globals.CHAR_ADDRESS[x] + 0x2A, (ushort) (originalCharacterStats[x, 5]));
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if (emulator.ReadShort(p + 0x11E) == 177) { //Soa's Ahnk
                                bool alive = false;
                                int kill = -1;
                                int lastPartyID = -1;
                                if (emulator.ReadShort(p) == 0) {
                                    for (int x = 0; x < 3; x++) {
                                        if (x != i && Globals.PARTY_SLOT[i] < 9 && emulator.ReadShort(Globals.CHAR_ADDRESS[x]) > 0) {
                                            alive = true;
                                        }
                                    }

                                    if (alive) {
                                        for (int x = 0; x < 3; x++) {
                                            if (kill == -1 && new Random().Next(0, 9) < 5 && emulator.ReadShort(Globals.CHAR_ADDRESS[x]) > 0) {
                                                kill = x;
                                            } else {
                                                lastPartyID = x;
                                            }
                                        }
                                    }

                                    if (kill != -1) {
                                        emulator.WriteShort(Globals.CHAR_ADDRESS[kill], 0);
                                        emulator.WriteShort(Globals.CHAR_ADDRESS[kill] - 0xA8, 192);
                                    } else {
                                        emulator.WriteShort(Globals.CHAR_ADDRESS[lastPartyID], 0);
                                        emulator.WriteShort(Globals.CHAR_ADDRESS[lastPartyID] - 0xA8, 192);
                                    }
                                    emulator.WriteShort(p, 1);
                                } else {
                                    emulator.WriteShort(p + 0x8, 0);
                                    emulator.WriteShort(p + 0x132, 0);
                                    emulator.WriteShort(p - 0xA8, 192);
                                }
                            }

                            if (emulator.ReadShort(p + 0x11A) == 74) { //Armor of Legend
                                if (lGuardStateDF[i] == false) {
                                    if (emulator.ReadByte(p + 0x4C) == 1 && guardStatusDF[i] == 0) {
                                        guardStatusDF[i] = 1;
                                        lGuardStatusDF[i] += 1;
                                    }
                                    if ((emulator.ReadByte(p - 0xA8) == 8 || emulator.ReadByte(p - 0xA8) == 10) && guardStatusDF[i] == 1 && emulator.ReadByte(p + 0x4C) == 0) {
                                        guardStatusDF[i] = 0;
                                    }
                                    if (lGuardStatusDF[i] >= 3) {
                                        lGuardStatusDF[i] = 0;
                                        guardStatusDF[i] = 1;
                                        lGuardStateDF[i] = true;
                                        emulator.WriteByte(p + 0xB5, 4);
                                        emulator.WriteShort(p + 0x30, (ushort) Math.Round(originalCharacterStats[i, 3] * 1.2));
                                    }
                                } else {
                                    if (emulator.ReadByte(p + 0xB5) == 0) {
                                        emulator.WriteShort(p + 0x30, (ushort) originalCharacterStats[i, 3]);
                                        lGuardStateDF[i] = false;
                                    }
                                }
                                if (!sGuardStatusDF[i]) {
                                    if (emulator.ReadByte(p - 0xA8) == 8 || emulator.ReadByte(p - 0xA8) == 10) {
                                        if (new Random().Next(0, 100) <= 10) {
                                            emulator.WriteByte(0x6E3B4 + ((Globals.MONSTER_SIZE + i) * 0x20) + EquipChangesBattleGlobalOffset(), emulator.ReadByte(0x6E3B4 + ((Globals.MONSTER_SIZE + i) * 0x20) + EquipChangesBattleGlobalOffset()) + 1);
                                        }
                                        sGuardStatusDF[i] = true;
                                    }
                                } else {
                                    if (emulator.ReadByte(p - 0xA8) != 8 && emulator.ReadByte(p - 0xA8) != 10) {
                                        sGuardStatusDF[i] = false;
                                    }
                                }
                            }

                            if (emulator.ReadShort(p + 0x118) == 89) { //Legend Casque
                                if (!lGuardStateMDF[i]) {
                                    if (emulator.ReadByte(p + 0x4C) == 1 && guardStatusMDF[i] == 0) {
                                        guardStatusMDF[i] = 1;
                                        lGuardStatusMDF[i] += 1;
                                    }
                                    if ((emulator.ReadByte(p - 0xA8) == 8 || emulator.ReadByte(p - 0xA8) == 10) && guardStatusMDF[i] == 1 && emulator.ReadByte(p + 0x4C) == 0) {
                                        guardStatusMDF[i] = 0;
                                    }
                                    if (lGuardStatusMDF[i] >= 3) {
                                        lGuardStatusMDF[i] = 0;
                                        guardStatusMDF[i] = 1;
                                        lGuardStateMDF[i] = true;
                                        emulator.WriteByte(p + 0xB7, 4);
                                        emulator.WriteShort(p + 0x32, (ushort) Math.Round(originalCharacterStats[i, 4] * 1.2));
                                    }
                                } else {
                                    if (emulator.ReadByte(p + 0xB7) == 0) {
                                        emulator.WriteShort(p + 0x32, (ushort) originalCharacterStats[i, 4]);
                                        lGuardStateMDF[i] = false;
                                    }
                                }
                                if (!sGuardStatusMDF[i]) {
                                    if (emulator.ReadByte(p - 0xA8) == 8 || emulator.ReadByte(p - 0xA8) == 10) {
                                        if (new Random().Next(0, 100) <= 10) {
                                            emulator.WriteByte(0x6E3B4 + ((Globals.MONSTER_SIZE + i) * 0x20) + EquipChangesBattleGlobalOffset(), emulator.ReadByte(0x6E3B4 + ((Globals.MONSTER_SIZE + i) * 0x20) + EquipChangesBattleGlobalOffset()) + 4);
                                        }
                                        sGuardStatusMDF[i] = true;
                                    }
                                } else {
                                    if (emulator.ReadByte(p - 0xA8) != 8 && emulator.ReadByte(p - 0xA8) != 10) {
                                        sGuardStatusMDF[i] = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void WriteEquipChanges(long address, byte value) {
            emulator.WriteByte(address + EquipChangesOffset(), value);
        }

        public void WriteEquipChangesAOB(long address, string value) {
            if (Constants.REGION != Region.JPN) {
                emulator.WriteAOB(address + EquipChangesOffset(), value);
            }
        }

        public int EquipChangesOffset() {
            int offset = 0x0;
            if (Constants.REGION == Region.JPN) {
                offset -= 0x186C;
            }
            return offset;
        }

        public int EquipChangesBattleGlobalOffset() {
            int offset = 0x0;
            if (Constants.REGION == Region.JPN) {
                offset -= 0x1300;
            }
            return offset;
        }

        public int EquipChangesInventoryOffset() {
            int offset = 0x0;
            if (Constants.REGION == Region.JPN) {
                offset -= 0x1300;
            }
            return offset;
        }
        #endregion
        #endregion

        #region Battle
        #region Damage Cap Removal
        public void RemoveDamageCap() {
            if (Globals.IN_BATTLE && Globals.STATS_CHANGED) {
                if (!firstDamageCapRemoval) {
                    emulator.WriteInteger(Constants.GetAddress("DAMAGE_CAP"), 50000);
                    emulator.WriteInteger(Constants.GetAddress("DAMAGE_CAP") + 0x8, 50000);
                    emulator.WriteInteger(Constants.GetAddress("DAMAGE_CAP") + 0x14, 50000);
                    DamageCapScan();
                    firstDamageCapRemoval = true;
                } else {
                    ushort currentItem = emulator.ReadShort(Globals.M_POINT + 0xABC);
                    if (lastItemUsedDamageCap != currentItem) {
                        lastItemUsedDamageCap = currentItem;
                        if ((lastItemUsedDamageCap >= 0xC1 && lastItemUsedDamageCap <= 0xCA) || (lastItemUsedDamageCap >= 0xCF && lastItemUsedDamageCap <= 0xD2) || lastItemUsedDamageCap == 0xD6 || lastItemUsedDamageCap == 0xD8 || lastItemUsedDamageCap == 0xDC || (lastItemUsedDamageCap >= 0xF1 && lastItemUsedDamageCap <= 0xF8) || lastItemUsedDamageCap == 0xFA) {
                            DamageCapScan();
                        }
                    }
                    for (int i = 0; i < 3; i++) {
                        if (Globals.PARTY_SLOT[i] < 9) {
                            if (emulator.ReadByte(Globals.C_POINT - 0xA8 - (0x388 * i)) == 24) {
                                DamageCapScan();
                            }
                        }
                    }
                }
            } else {
                firstDamageCapRemoval = false;
                lastItemUsedDamageCap = 0;
            }
        }

        public void DamageCapScan() {
            ArrayList damageCapScan = emulator.ScanAllAOB("0F 27", 0xA8660, 0x2A865F);
            long lastAddress = 0;
            foreach (var address in damageCapScan) {
                long capAddress = (long) address;
                if (emulator.ReadShortU(capAddress) == 9999 && (lastAddress + 0x10) == capAddress) {
                    emulator.WriteIntegerU(capAddress, 50000);
                    emulator.WriteIntegerU(lastAddress, 50000);
                }
                lastAddress = capAddress;
            }
        }
        #endregion

        #region Never Guard
        public void ApplyNeverGuard() {
            if (Globals.IN_BATTLE && Globals.STATS_CHANGED) {
                for (int i = 0; i < 3; i++) {
                    if (Globals.PARTY_SLOT[i] < 9) {
                        emulator.WriteByte(Globals.CHAR_ADDRESS[i] + 0x4C, 0);
                    }
                }
            }
        }
        #endregion

        #region Dragoon Changes
        public void ChangeDragoonDescription() {
            if (Constants.REGION == Region.USA) {
                //Red-Eyed
                emulator.WriteAOB(0x51858, "24 00 41 00 4A 00 3D 00 00 00 31 00 32 00 30 00 00 00 1A 00 16 00 15 00 0F 00 FF A0");
                if ((dmScripts.ContainsKey("btnDivineRed") && dmScripts["btnDivineRed"]) && Globals.PARTY_SLOT[0] == 0 && (Globals.DIFFICULTY_MODE.Equals("Normal") || Globals.DIFFICULTY_MODE.Equals("Hard"))) {
                    emulator.WriteAOB(0x51884, "24 00 41 00 4A 00 3D 00 00 00 31 00 32 00 30 00 00 00 16 00 15 00 17 00 15 00 0F 00 FF A0");
                    emulator.WriteAOB(0x518AC, "24 00 41 00 4A 00 3D 00 00 00 31 00 32 00 30 00 00 00 16 00 1A 00 18 00 15 00 0F 00 FF A0");
                } else {
                    emulator.WriteAOB(0x51884, "24 00 41 00 4A 00 3D 00 00 00 31 00 32 00 30 00 00 00 18 00 19 00 15 00 0F 00 FF A0");
                    emulator.WriteAOB(0x518AC, "24 00 41 00 4A 00 3D 00 00 00 31 00 32 00 30 00 00 00 1C 00 1B 00 1A 00 0F 00 FF A0");
                }
                emulator.WriteAOB(0x518D8, "24 00 41 00 4A 00 3D 00 00 00 31 00 32 00 30 00 00 00 16 00 15 00 17 00 15 00 0F 00 FF A0");
                //Divine
                emulator.WriteAOB(0x519D4, "31 00 32 00 30 00 00 00 16 00 15 00 17 00 15 00 0F 00 FF A0");
                emulator.WriteAOB(0x51900, "31 00 32 00 30 00 00 00 16 00 1A 00 18 00 15 00 0F 00 FF A0");
                //Jade
                emulator.WriteAOB(0x51924, "35 00 41 00 46 00 3C 00 00 00 31 00 32 00 30 00 00 00 19 00 19 00 15 00 0F 00 FF A0");
                emulator.WriteAOB(0x5194C, "35 00 41 00 46 00 3C 00 00 00 31 00 32 00 30 00 00 00 1E 00 1E 00 15 00 0F 00 FF A0");
                emulator.WriteAOB(0x519AC, "35 00 41 00 46 00 3C 00 00 00 31 00 32 00 30 00 00 00 16 00 18 00 17 00 15 00 0F 00 FF A0");
                emulator.WriteAOB(0x51AB4, "35 00 41 00 46 00 3C 00 00 00 31 00 32 00 30 00 00 00 19 00 19 00 15 00 0F 00 FF A0");
                emulator.WriteAOB(0x51B48, "35 00 41 00 46 00 3C 00 00 00 31 00 32 00 30 00 00 00 19 00 19 00 15 00 0F 00 FF A0");
                //White-Silver
                emulator.WriteAOB(0x519F0, "2A 00 41 00 3F 00 40 00 4C 00 00 00 31 00 32 00 30 00 00 00 18 00 18 00 15 00 0F 00 FF A0");
                emulator.WriteAOB(0x51A84, "2A 00 41 00 3F 00 40 00 4C 00 00 00 31 00 32 00 30 00 00 00 1D 00 1B 00 1A 00 0F 00 FF A0");
                //Dark
                emulator.WriteAOB(0x51ADC, "22 00 39 00 4A 00 43 00 00 00 31 00 32 00 30 00 00 00 1A 00 1E 00 15 00 0F 00 00 00 10 00 00 00 26 00 2E 00 00 00 30 00 3D 00 3B 00 4E 00 FF A0");
                emulator.WriteAOB(0x51B14, "22 00 39 00 4A 00 43 00 00 00 31 00 32 00 30 00 00 00 18 00 1E 00 1A 00 0F 00 00 00 10 00 00 00 24 00 3D 00 39 00 4A 00 FF A0");
                emulator.WriteAOB(0x51BA8, "22 00 39 00 4A 00 43 00 00 00 31 00 32 00 30 00 00 00 16 00 1B 00 1D 00 15 00 0F 00 00 00 10 00 00 00 26 00 2E 00 00 00 FF A0");
                //Violet
                emulator.WriteAOB(0x51BD8, "32 00 40 00 4D 00 46 00 3C 00 3D 00 4A 00 00 00 31 00 32 00 30 00 00 00 18 00 18 00 15 00 0F 00 FF A0");
                emulator.WriteAOB(0x51C0C, "32 00 40 00 4D 00 46 00 3C 00 3D 00 4A 00 00 00 31 00 32 00 30 00 00 00 1B 00 1B 00 15 00 0F 00 FF A0");
                emulator.WriteAOB(0x51C40, "32 00 40 00 4D 00 46 00 3C 00 3D 00 4A 00 00 00 31 00 32 00 30 00 00 00 1E 00 1E 00 15 00 0F 00 FF A0");
                emulator.WriteAOB(0x51C74, "32 00 40 00 4D 00 46 00 3C 00 3D 00 4A 00 00 00 31 00 32 00 30 00 00 00 16 00 19 00 1E 00 1A 00 0F 00 FF A0");
                //Blue-Sea
                emulator.WriteAOB(0x51CA8, "35 00 39 00 4C 00 3D 00 4A 00 00 00 31 00 32 00 30 00 00 00 1A 00 16 00 15 00 0F 00 FF A0");
                emulator.WriteAOB(0x51D3C, "35 00 39 00 4C 00 3D 00 4A 00 00 00 31 00 32 00 30 00 00 00 1A 00 18 00 15 00 0F 00 FF A0");
                emulator.WriteAOB(0x51D64, "35 00 39 00 4C 00 3D 00 4A 00 00 00 31 00 32 00 30 00 00 00 16 00 19 00 15 00 15 00 0F 00 FF A0");
                //Golden
                emulator.WriteAOB(0x51D94, "23 00 39 00 4A 00 4C 00 40 00 00 00 31 00 32 00 30 00 00 00 1B 00 1C 00 1A 00 0F 00 FF A0");
                emulator.WriteAOB(0x51DBC, "23 00 39 00 4A 00 4C 00 40 00 00 00 31 00 32 00 30 00 00 00 16 00 16 00 17 00 15 00 0F 00 FF A0");
                emulator.WriteAOB(0x51DE4, "23 00 39 00 4A 00 4C 00 40 00 00 00 31 00 32 00 30 00 00 00 17 00 17 00 17 00 15 00 0F 00 FF A0");
            }

            if ((dmScripts.ContainsKey("btnDivineRed") && dmScripts["btnDivineRed"]) && Globals.PARTY_SLOT[0] == 0 && (Globals.DIFFICULTY_MODE.Equals("Normal") || Globals.DIFFICULTY_MODE.Equals("Hard")) && Globals.PARTY_SLOT[0] == 0) {
                emulator.WriteAOB(Constants.GetAddress("SLOT1_SPELLS"), "01 02 FF FF FF FF FF FF");
                emulator.WriteByte(Constants.GetAddress("SPELL_TABLE") + 0x8 + (1 * 0xC), 50); //Explosion MP
                emulator.WriteByte(Constants.GetAddress("SPELL_TABLE") + 0x8 + (2 * 0xC), 50); //Final Burst MP
            }

            for (int i = 0; i < 3; i++) {
                if (Globals.PARTY_SLOT[i] == 2 || Globals.PARTY_SLOT[i] == 8) {
                    recoveryRateSave = emulator.ReadByte(Globals.CHAR_ADDRESS[i] + 0x12C);
                }
            }

            if (Globals.DIFFICULTY_MODE.Equals("Hell")) {
                emulator.WriteByte(Constants.GetAddress("SPELL_TABLE") + 0x8 + (7 * 0xC), (uiCombo["cboFlowerStorm"] + 1) * 20); //Lavitz's Blossom Storm MP
                emulator.WriteByte(Constants.GetAddress("SPELL_TABLE") + 0x8 + (26 * 0xC), (uiCombo["cboFlowerStorm"] + 1) * 20); //Albert's Rose storm MP
                emulator.WriteByte(Constants.GetAddress("SPELL_TABLE") + 0x8 + (11 * 0xC), 20); //Shana's Moon Light MP
                emulator.WriteByte(Constants.GetAddress("SPELL_TABLE") + 0x8 + (66 * 0xC), 20); //???'s Moon Light MP
                emulator.WriteByte(Constants.GetAddress("SPELL_TABLE") + 0x8 + (25 * 0xC), 30); //Rainbow Breath MP
                emulator.WriteByte(Constants.GetAddress("SPELL_TABLE") + 0x8 + (12 * 0xC), 40); //Shana's Gates of Heaven MP
                emulator.WriteByte(Constants.GetAddress("SPELL_TABLE") + 0x8 + (67 * 0xC), 40); //???'s Gates of Heaven MP
            }
        }

        public void DragoonChanges() {
            if (Globals.IN_BATTLE && Globals.STATS_CHANGED) {
                if (!dragoonChangesOnBattleEntry) {
                    ChangeDragoonDescription();
                    dragoonChangesOnBattleEntry = true;
                } else {
                    for (int i = 0; i < 3; i++) {
                        int mp = 0;
                        double multi = 1;
                        byte dragoonSpecialAttack = emulator.ReadByte(Constants.GetAddress("DRAGOON_SPECIAL_ATTACK"));
                        byte dragoonSpecialMagic = emulator.ReadByte(Constants.GetAddress("DRAGOON_SPECIAL_MAGIC"));

                        if (Globals.ENCOUNTER_ID == 416 || Globals.ENCOUNTER_ID == 394 || Globals.ENCOUNTER_ID == 443) {
                            if (emulator.ReadByte(Constants.GetAddress("DRAGON_BLOCK_STAFF")) == 1) {
                                multi = 8;
                            } else {
                                multi = 1;
                            }
                        }

                        /*if (ubReverseDBS) {
                            multi = 10;
                        }*/

                        /*If(partySlots(i) = 3 And readShort(Globals.CHAR_ADDRESS(i) + &H116) = 162) Then 'Dragon Buster II
                            multi *= 1.1
                        End If*/

                        currentMP[i] = emulator.ReadShort(Globals.CHAR_ADDRESS[i] + 0x4);

                        if (Globals.PARTY_SLOT[i] == 0) { //Dart
                            if (dragoonSpecialAttack == 0 || dragoonSpecialAttack == 9) {
                                if (Globals.DRAGOON_SPIRITS >= 254) {
                                    emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA4, (ushort) (306 * multi));
                                } else {
                                    if (dmScripts.ContainsKey("btnDivineRed") && dmScripts["btnDivineRed"]) {
                                        emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA4, (ushort) (612 * multi));
                                    } else {
                                        emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA4, (ushort) (422 * multi));
                                    }
                                }
                            } else {
                                if (Globals.DRAGOON_SPIRITS >= 254) {
                                    emulator.WriteByte(Globals.CHAR_ADDRESS[i] + 0xA5, 0);
                                    emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA4, (ushort) (204 * multi));
                                } else {
                                    if (dmScripts.ContainsKey("btnDivineRed") && dmScripts["btnDivineRed"]) {
                                        emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA4, (ushort) (408 * multi));
                                    } else {
                                        emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA4, (ushort) (281 * multi));
                                    }
                                }
                            }

                            if (multi == 1) {
                                emulator.WriteByte(Globals.CHAR_ADDRESS[i] + 0xA9, 0);
                                emulator.WriteByte(Globals.CHAR_ADDRESS[i] + 0xAB, 0);
                                emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA8, (ushort) (180 * multi));
                                emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xAA, (ushort) (180 * multi));
                            } else {
                                emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA8, (ushort) (180 * multi));
                                emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xAA, (ushort) (180 * multi));
                            }

                            if (soasSiphonSlot == i) { //Soa's Siphon Ring
                               multi *= 0.3;
                            }

                            if (currentMP[i] != previousMP[i] && currentMP[i] < previousMP[i]) {
                                mp = previousMP[i] - currentMP[i];
                                if (dmScripts.ContainsKey("btnDivineRed") && dmScripts["btnDivineRed"]) {
                                    if (emulator.ReadByte(Globals.CHAR_ADDRESS[i] + 0x46) == 1) {
                                        emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA6, (ushort) (1020 * multi));
                                    } else {
                                        emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA6, (ushort) (510 * multi));
                                    }
                                } else {
                                    if (mp == 10) {
                                        if (multi == 1) {
                                            emulator.WriteByte(Globals.CHAR_ADDRESS[i] + 0xA7, 0);
                                            emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA6, (ushort) (255 * multi));
                                        } else {
                                            emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA6, (ushort) (255 * multi));
                                        }
                                        //addBurnStack(1);
                                    } else if (mp == 20) {
                                        emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA6, (ushort) (340 * multi));
                                        //addBurnStack(1);
                                    } else if (mp == 30) {
                                        if (multi == 1) {
                                            emulator.WriteByte(Globals.CHAR_ADDRESS[i] + 0xA7, 0);
                                            emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA6, (ushort) (255 * multi));
                                        } else {
                                            emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA6, (ushort) (255 * multi));
                                        }
                                        //addBurnStack(2);
                                    } else if (mp == 50) {
                                        if (multi == 1) {
                                            emulator.WriteByte(Globals.CHAR_ADDRESS[i] + 0xA7, 0);
                                            emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA6, (ushort) (255 * multi));
                                        } else {
                                            emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA6, (ushort) (255 * multi));
                                        }
                                    } else if (mp == 80) {
                                        emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA6, (ushort) (340 * multi));
                                        //addBurnStack(3);
                                    }
                                }
                                previousMP[i] = currentMP[i];
                            } else {
                                if (currentMP[i] > previousMP[i]) {
                                    previousMP[i] = currentMP[i];
                                }
                            }
                        } else if (Globals.PARTY_SLOT[i] == 1 || Globals.PARTY_SLOT[i] == 5) { //Lavitz/Albert
                            if (checkHarpoon) {
                                if (Globals.ENCOUNTER_ID == 416 || Globals.ENCOUNTER_ID == 394 || Globals.ENCOUNTER_ID == 443) {
                                    if (emulator.ReadByte(Constants.GetAddress("DRAGON_BLOCK_STAFF")) == 1) {
                                        multi = 24;
                                    } else {
                                        multi = 3;
                                    }
                                } else {
                                    multi = 3;
                                }
                            } else {
                                if (Globals.ENCOUNTER_ID == 416 || Globals.ENCOUNTER_ID == 394 || Globals.ENCOUNTER_ID == 443) {
                                    if (emulator.ReadByte(Constants.GetAddress("DRAGON_BLOCK_STAFF")) == 1) {
                                        multi = 8;
                                    } else {
                                        multi = 1;
                                    }
                                } else {
                                    multi = 1;
                                }
                            }

                            if (multi == 1) {
                                emulator.WriteByte(Globals.CHAR_ADDRESS[i] + 0xA9, 0);
                                emulator.WriteByte(Globals.CHAR_ADDRESS[i] + 0xAB, 0);
                                emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA8, (ushort) (180 * multi));
                                emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xAA, (ushort) (180 * multi));
                            } else {
                                emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA8, (ushort) (180 * multi));
                                emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xAA, (ushort) (180 * multi));
                            }

                            if (dragoonSpecialAttack == 1 || dragoonSpecialAttack == 5) {
                                emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA4, (ushort) (495 * multi));
                            } else {
                                emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA4, (ushort) (330 * multi));
                            }

                            if (soasSiphonSlot == i) { //Soa's Siphon Ring
                                multi *= 0.3;
                            }

                            if (currentMP[i] != previousMP[i] && currentMP[i] < previousMP[i]) {
                                mp = previousMP[i] - currentMP[i];
                                if (mp == 20 || mp == 80) {
                                    emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA6, (ushort) (440 * multi));
                                } else if (mp == 30) {
                                    emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA6, (ushort) (330 * multi));
                                }
                                if ((emulator.ReadByte(Globals.CHAR_ADDRESS[i] + 0x46) == 7 || emulator.ReadByte(Globals.CHAR_ADDRESS[i] + 0x46) == 26) && Globals.DIFFICULTY_MODE.Equals("Hell")) {
                                    //flowerstorm
                                }
                            } else {
                                if (currentMP[i] > previousMP[i]) {
                                    previousMP[i] = currentMP[i];
                                }
                            }
                        } else if (Globals.PARTY_SLOT[i] == 2 || Globals.PARTY_SLOT[i] == 8) { //Shana
                            if (dragoonSpecialAttack == 2 || dragoonSpecialAttack == 8) {
                                emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA4, (ushort) (510 * multi));
                            } else {
                                emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA4, (ushort) (365 * multi));
                            }

                            if (multi == 1) {
                                emulator.WriteByte(Globals.CHAR_ADDRESS[i] + 0xA9, 0);
                                emulator.WriteByte(Globals.CHAR_ADDRESS[i] + 0xAB, 0);
                                emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA8, (ushort) (180 * multi));
                                emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xAA, (ushort) (180 * multi));
                            } else {
                                emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA8, (ushort) (180 * multi));
                                emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xAA, (ushort) (180 * multi));
                            }

                            if (soasSiphonSlot == i) { //Soa's Siphon Ring
                                multi *= 0.3;
                            }

                            if (currentMP[i] != previousMP[i] && currentMP[i] < previousMP[i]) {
                                mp = previousMP[i] - currentMP[i];
                                if (mp == 20) {
                                    emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA6, (ushort) (332 * multi));
                                    emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0x12C, (ushort) (recoveryRateSave + 20));
                                } else if (mp == 80) {
                                    emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA6, (ushort) (289 * multi));
                                }
                                previousMP[i] = currentMP[i];
                            } else {
                                if (currentMP[i] > previousMP[i]) {
                                    previousMP[i] = currentMP[i];
                                }
                            }
                        } else if (Globals.PARTY_SLOT[i] == 3) { //Rose
                            if (dragoonSpecialAttack == 3) {
                                emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA4, (ushort) (495 * multi));
                            } else {
                                emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA4, (ushort) (330 * multi));
                            }

                            if (multi == 1) {
                                emulator.WriteByte(Globals.CHAR_ADDRESS[i] + 0xA9, 0);
                                emulator.WriteByte(Globals.CHAR_ADDRESS[i] + 0xAB, 0);
                                emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA8, (ushort) (180 * multi));
                                emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xAA, (ushort) (180 * multi));
                            } else {
                                emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA8, (ushort) (180 * multi));
                                emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xAA, (ushort) (180 * multi));
                            }

                            if (soasSiphonSlot == i) { //Soa's Siphon Ring
                                multi *= 0.3;
                            }

                            if (currentMP[i] != previousMP[i] && currentMP[i] < previousMP[i]) {
                                mp = previousMP[i] - currentMP[i];
                                if (mp == 10) {
                                    emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA6, (ushort) (295 * multi));
                                    for (int x = 0; x < 2; x++) {
                                        if (Globals.PARTY_SLOT[x] < 9 && emulator.ReadShort(Globals.CHAR_ADDRESS[i]) > 0) {
                                            emulator.WriteShort(Globals.CHAR_ADDRESS[i], (ushort) Math.Min(emulator.ReadShort(Globals.CHAR_ADDRESS[i] + 0x8), emulator.ReadShort(Globals.CHAR_ADDRESS[x]) + Math.Round(emulator.ReadShort(Globals.CHAR_ADDRESS[i]) * (emulator.ReadByte(Constants.GetAddress("ROSE_DRAGOON_LEVEL")) * 0.05))));
                                        }
                                    }
                                } else if (mp == 20) {
                                    emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA6, (ushort) (395 * multi));
                                } else if (mp == 25) {
                                    emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA6, (ushort) (410 * multi));
                                    for (int x = 0; x < 2; x++) {
                                        if (Globals.PARTY_SLOT[x] < 9 && emulator.ReadShort(Globals.CHAR_ADDRESS[i]) > 0) {
                                            emulator.WriteShort(Globals.CHAR_ADDRESS[i], (ushort) Math.Min(emulator.ReadShort(Globals.CHAR_ADDRESS[i] + 0x8), emulator.ReadShort(Globals.CHAR_ADDRESS[x]) + Math.Round(emulator.ReadShort(Globals.CHAR_ADDRESS[i]) * (emulator.ReadByte(Constants.GetAddress("ROSE_DRAGOON_LEVEL")) * 0.04))));
                                        }
                                    }
                                } else if (mp == 50) {
                                    emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA6, (ushort) (790 * multi));
                                } else if (mp == 80) {
                                    emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA6, (ushort) (420 * multi));
                                    //lastDamageSlot1 = readShort(&HAC6238)
                                    //checkdamage
                                } else if (mp == 100) {
                                    emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA6, (ushort) (290 * multi));
                                    //lastDamageSlot1 = readShort(&HAC6238)
                                    //checkdamage
                                }
                                previousMP[i] = currentMP[i];
                            } else {
                                if (currentMP[i] > previousMP[i]) {
                                    previousMP[i] = currentMP[i];
                                }
                            }
                        } else if (Globals.PARTY_SLOT[i] == 4) { //Haschel
                            if (dragoonSpecialAttack == 4) {
                                emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA4, (ushort) (422 * multi));
                            } else {
                                emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA4, (ushort) (281 * multi));
                            }

                            if (multi == 1) {
                                emulator.WriteByte(Globals.CHAR_ADDRESS[i] + 0xA9, 0);
                                emulator.WriteByte(Globals.CHAR_ADDRESS[i] + 0xAB, 0);
                                emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA8, (ushort) (180 * multi));
                                emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xAA, (ushort) (180 * multi));
                            } else {
                                emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA8, (ushort) (180 * multi));
                                emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xAA, (ushort) (180 * multi));
                            }

                            if (soasSiphonSlot == i) { //Soa's Siphon Ring
                                multi *= 0.3;
                            }

                            if (eleBombTurns > 0 && eleBombElement == 16) {
                                multi *= 3;
                            }

                            if (currentMP[i] != previousMP[i] && currentMP[i] < previousMP[i]) {
                                mp = previousMP[i] - currentMP[i];
                                if (mp == 10 || mp == 20 || mp == 30) {
                                    emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA6, (ushort) (330 * multi));
                                } else if (mp == 80) {
                                    emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA6, (ushort) (374 * multi));
                                }
                                previousMP[i] = currentMP[i];
                            } else {
                                if (currentMP[i] > previousMP[i]) {
                                    previousMP[i] = currentMP[i];
                                }
                            }
                        } else if (Globals.PARTY_SLOT[i] == 6) { //Meru
                            if (dragoonSpecialAttack == 6) {
                                emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA4, (ushort) (495 * multi));
                            } else {
                                emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA4, (ushort) (330 * multi));
                            }

                            if (multi == 1) {
                                emulator.WriteByte(Globals.CHAR_ADDRESS[i] + 0xA9, 0);
                                emulator.WriteByte(Globals.CHAR_ADDRESS[i] + 0xAB, 0);
                                emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA8, (ushort) (180 * multi));
                                emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xAA, (ushort) (180 * multi));
                            } else {
                                emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA8, (ushort) (180 * multi));
                                emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xAA, (ushort) (180 * multi));
                            }

                            if (soasSiphonSlot == i) { //Soa's Siphon Ring
                                multi *= 0.3;
                            }

                            if (currentMP[i] != previousMP[i] && currentMP[i] < previousMP[i]) {
                                mp = previousMP[i] - currentMP[i];
                                if (mp == 10) {
                                    if (multi == 1) {
                                        emulator.WriteByte(Globals.CHAR_ADDRESS[i] + 0xA7, 0);
                                        emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA6, (ushort) (255 * multi));
                                    } else {
                                        emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA6, (ushort) (255 * multi));
                                    }
                                } else if (mp == 30) {
                                    emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA6, (ushort) (264 * multi));
                                } else if (mp == 80) {
                                    emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA6, (ushort) (350 * multi));
                                }

                                //Jeweled Hammer
                                if (emulator.ReadShort(Globals.CHAR_ADDRESS[i] + 0x116) == 164) {
                                    if (mp == 50) {
                                        if (multi == 1) {
                                            emulator.WriteByte(Globals.CHAR_ADDRESS[i] + 0xA7, 0);
                                            emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA6, (ushort) (255 * multi));
                                        } else {
                                            emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA6, (ushort) (255 * multi));
                                        }
                                    } else if (mp == 100) {
                                        if (emulator.ReadByte(Globals.CHAR_ADDRESS[i] + 0x46) == 25) {
                                            //trackchp
                                            //rainbow breath
                                        } else {
                                            emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA6, (ushort) (350 * multi));
                                        }
                                    } else if (mp == 150) {
                                        emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA6, (ushort) (525 * multi));
                                    }
                                }
                                previousMP[i] = currentMP[i];
                            } else {
                                if (currentMP[i] > previousMP[i]) {
                                    previousMP[i] = currentMP[i];
                                }
                            }
                        } else if (Globals.PARTY_SLOT[i] == 7) {
                            if (dragoonSpecialAttack == 7) {
                                emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA4, (ushort) (600 * multi));
                            } else {
                                emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA4, (ushort) (500 * multi));
                            }

                            if (multi == 1) {
                                emulator.WriteByte(Globals.CHAR_ADDRESS[i] + 0xA9, 0);
                                emulator.WriteByte(Globals.CHAR_ADDRESS[i] + 0xAB, 0);
                                emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA8, (ushort) (130 * multi));
                                emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xAA, (ushort) (130 * multi));
                            } else {
                                emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA8, (ushort) (130 * multi));
                                emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xAA, (ushort) (130 * multi));
                            }

                            if (soasSiphonSlot == i) { //Soa's Siphon Ring
                                multi *= 0.3;
                            }

                            if (currentMP[i] != previousMP[i] && currentMP[i] < previousMP[i]) {
                                mp = previousMP[i] - currentMP[i];
                                if (mp == 20) {
                                    emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA6, (ushort) (450 * multi));
                                } else if (mp == 30) {
                                    emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA6, (ushort) (560 * multi));
                                } else if (mp == 80) {
                                    emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0xA6, (ushort) (740 * multi));
                                }
                                previousMP[i] = currentMP[i];
                            } else {
                                if (currentMP[i] > previousMP[i]) {
                                    previousMP[i] = currentMP[i];
                                }
                            }
                        }
                    }
                }
            } else {
                if (dragoonChangesOnBattleEntry) {
                    recoveryRateSave = 0;
                    for (int i = 0; i < 3; i++) {
                        currentMP[i] = 0;
                        previousMP[i] = 0;
                    }
                    dragoonChangesOnBattleEntry = false;
                }
            }
        }
        #endregion

        #region Aspect Ratio
        public void ChangeAspectRatio() {
            if (Globals.IN_BATTLE && Globals.STATS_CHANGED && !aspectRatioOnBattleEntry) {
                ushort aspectRatio = 4096;

                if (uiCombo["cboAspectRatio"] == 0)
                    aspectRatio = 4096;
                else if (uiCombo["cboAspectRatio"] == 1)
                    aspectRatio = 3072;
                else if (uiCombo["cboAspectRatio"] == 2)
                    aspectRatio = 3413;
                else if (uiCombo["cboAspectRatio"] == 3)
                    aspectRatio = 2340;
                else if (uiCombo["cboAspectRatio"] == 4)
                    aspectRatio = 2048;

                emulator.WriteShort(Constants.GetAddress("ASPECT_RATIO"), aspectRatio);

                if (uiCombo["cboCamera"] == 1)
                    emulator.WriteShort(Constants.GetAddress("ADVANCED_CAMERA"), aspectRatio);

                aspectRatioOnBattleEntry = true;
            } else {
                if (!Globals.IN_BATTLE && aspectRatioOnBattleEntry) {
                    aspectRatioOnBattleEntry = false;
                }
            }
        }
        #endregion

        #region Elemental Bomb
        public void ElementalBomb() {
            if (Globals.IN_BATTLE && Globals.STATS_CHANGED && eleBombTurns == 0) {
                eleBombItemUsed = emulator.ReadByte(Globals.MONS_ADDRESS[0] + 0xABC);
                if ((eleBombItemUsed >= 241 && eleBombItemUsed <= 248) || eleBombItemUsed == 250) {
                    ushort player1Action = emulator.ReadShort(Globals.CHAR_ADDRESS[0] - 0xA8);
                    ushort player2Action = emulator.ReadShort(Globals.CHAR_ADDRESS[1] - 0xA8);
                    ushort player3Action = emulator.ReadShort(Globals.CHAR_ADDRESS[2] - 0xA8);
                    if (Globals.PARTY_SLOT[2] < 9) {
                        if (player1Action == 24 && (player2Action == 16 || player2Action == 18) && (player3Action == 16 || player3Action == 18)) {
                            eleBombSlot = 0;
                            eleBombTurns = 5;
                            eleBombChange = false;
                        }
                        if (player2Action == 24 && (player1Action == 16 || player1Action == 18) && (player3Action == 16 || player3Action == 18)) {
                            eleBombSlot = 1;
                            eleBombTurns = 5;
                            eleBombChange = false;
                        }
                        if (player3Action == 24 && (player1Action == 16 || player1Action == 18) && (player2Action == 16 || player2Action == 18)) {
                            eleBombSlot = 2;
                            eleBombTurns = 5;
                            eleBombChange = false;
                        }
                    } else if (Globals.PARTY_SLOT[1] < 9) {
                        if (player1Action == 24 && (player2Action == 16 || player2Action == 18)) {
                            eleBombSlot = 0;
                            eleBombTurns = 5;
                            eleBombChange = false;
                        }
                        if (player2Action == 24 && (player1Action == 16 || player1Action == 18)) {
                            eleBombSlot = 1;
                            eleBombTurns = 5;
                            eleBombChange = false;
                        }
                    } else {
                        if (player1Action == 24) {
                            eleBombSlot = 0;
                            eleBombTurns = 5;
                            eleBombChange = false;
                        }
                    }
                }

                //Constants.WriteDebug("Item: " + eleBombItemUsed + " | Slot: " + eleBombSlot + " | Turns: " + eleBombTurns + " | Change: " + eleBombChange);
            } else {
                //Constants.WriteDebug("Item: " + eleBombItemUsed + " | Slot: " + eleBombSlot + " | Turns: " + eleBombTurns + " | Change: " + eleBombChange + " | Element: " + eleBombElement + " | Action: " + emulator.ReadShort(Globals.CHAR_ADDRESS[eleBombSlot] - 0xA8));
                if (Globals.IN_BATTLE && Globals.STATS_CHANGED && eleBombSlot >= 0) {
                    if ((emulator.ReadShort(Globals.CHAR_ADDRESS[eleBombSlot] - 0xA8) == 8 || emulator.ReadShort(Globals.CHAR_ADDRESS[eleBombSlot] - 0xA8) == 10) && !eleBombChange) {
                        eleBombChange = true;
                        if (eleBombTurns == 5) {
                            ushort element = 0;

                            if (eleBombItemUsed == 241)
                                element = 0;
                            else if (eleBombItemUsed == 242)
                                element = 128;
                            else if (eleBombItemUsed == 243)
                                element = 1;
                            else if (eleBombItemUsed == 244)
                                element = 64;
                            else if (eleBombItemUsed == 245)
                                element = 2;
                            else if (eleBombItemUsed == 246)
                                element = 32;
                            else if (eleBombItemUsed == 247)
                                element = 4;
                            else if (eleBombItemUsed == 248)
                                element = 16;
                            else if (eleBombItemUsed == 250)
                                element = 8;

                            eleBombElement = (byte) element;

                            for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                                eleBombOldElement[i] = emulator.ReadShort(Globals.MONS_ADDRESS[i] + 0x14);
                                emulator.WriteShort(Globals.MONS_ADDRESS[i] + 0x14, element);
                                emulator.WriteShort(Globals.MONS_ADDRESS[i] + 0x6A, element);
                            }

                            eleBombTurns -= 1;
                        }
                    }

                    if (eleBombChange && (emulator.ReadShort(Globals.CHAR_ADDRESS[eleBombSlot] - 0xA8) == 0 || emulator.ReadShort(Globals.CHAR_ADDRESS[eleBombSlot] - 0xA8) == 2)) {
                        eleBombChange = false;
                        eleBombTurns -= 1;
                        if (eleBombTurns <= 0) {
                            for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                                emulator.WriteShort(Globals.MONS_ADDRESS[i] + 0x14, eleBombOldElement[i]);
                                emulator.WriteShort(Globals.MONS_ADDRESS[i] + 0x6A, eleBombOldElement[i]);
                            }
                        }
                    }

                    if (emulator.ReadShort(Globals.CHAR_ADDRESS[eleBombSlot] - 0xA8) == 192) {
                        for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                            emulator.WriteShort(Globals.MONS_ADDRESS[i] + 0x14, eleBombOldElement[i]);
                            emulator.WriteShort(Globals.MONS_ADDRESS[i] + 0x6A, eleBombOldElement[i]);
                        }
                        eleBombTurns = 0;
                        eleBombElement = 255;
                        eleBombSlot = 255;
                    }
                } else {
                    if (Globals.EXITING_BATTLE == 1) {
                        eleBombTurns = 0;
                        eleBombElement = 255;
                        eleBombSlot = 255;
                        eleBombItemUsed = 255;
                    }
                }
            }
        }
        #endregion

        #region No Dragoon
        public void NoDragoonMode() {
            if (Globals.IN_BATTLE && Globals.STATS_CHANGED && !soloModeOnBattleEntry) {
                for (int i = 0; i < 3; i++) {
                    if (Globals.PARTY_SLOT[i] < 9) {
                        emulator.WriteByte(Globals.CHAR_ADDRESS[i] + 0x7, 0);
                    }
                }
                noDragoonModeOnBattleEntry = true;
            } else {
                if (!Globals.IN_BATTLE && noDragoonModeOnBattleEntry) {
                    noDragoonModeOnBattleEntry = false;
                }
            }
        }
        #endregion

        #region Half SP
        public void AdditionHalfSPChanges() {
            if (Globals.IN_BATTLE && Globals.STATS_CHANGED && !halfSPOnBattleEntry) {
                for (int i = 0; i < 3; i++) {
                    if (Globals.PARTY_SLOT[i] < 9) {
                        byte equippedAddition = emulator.ReadByte(Constants.GetAddress("EQUIPPED_ADDITION") + (Globals.PARTY_SLOT[i] * 0x2C));
                        byte levelOffset = 0;

                        if (Globals.PARTY_SLOT[i] == 0)
                            levelOffset = equippedAddition;
                        else if (Globals.PARTY_SLOT[i] == 1 || Globals.PARTY_SLOT[i] == 5)
                            levelOffset = (byte) (equippedAddition - 8);
                        else if (Globals.PARTY_SLOT[i] == 3)
                            levelOffset = (byte) (equippedAddition - 14);
                        else if (Globals.PARTY_SLOT[i] == 4)
                            levelOffset = (byte) (equippedAddition - 29);
                        else if (Globals.PARTY_SLOT[i] == 6)
                            levelOffset = (byte) (equippedAddition - 23);
                        else if (Globals.PARTY_SLOT[i] == 7)
                            levelOffset = (byte) (equippedAddition - 19);

                        byte additionLevel = emulator.ReadByte(Constants.GetAddress("EQUIPPED_ADDITION_LEVEL") + (0x2C * (Globals.PARTY_SLOT[i] + levelOffset)));
                        byte totalSP = emulator.ReadByte(Constants.GetAddress("EQUIPPED_ADDITION_SP") + (0x2C * Globals.PARTY_SLOT[i]));
                        ushort spMulti = 65535;

                        if (Globals.PARTY_SLOT[i] == 0) {
                            if (additionLevel == 5) {
                                if (equippedAddition == 0) {
                                    spMulti = 65535 - 50 + 1;
                                } else if (equippedAddition == 1) {
                                    spMulti = 0;
                                } else if (equippedAddition == 2) {
                                    spMulti = 75;
                                } else if (equippedAddition == 3) {
                                    spMulti = 0;
                                } else if (equippedAddition == 4) {
                                    spMulti = 75;
                                } else if (equippedAddition == 5) {
                                    spMulti = 65535 - 50 + 1;
                                } else if (equippedAddition == 6) {
                                    spMulti = 65535 - 20 + 1;
                                }
                            } else {
                                spMulti = 65535 - 50 + 1;
                            }
                        } else if (Globals.PARTY_SLOT[i] == 1 || Globals.PARTY_SLOT[i] == 5) {
                            if (additionLevel == 5) {
                                if (equippedAddition == 8) {
                                    spMulti = 65535 - 25 + 1;
                                } else if (equippedAddition == 9) {
                                    spMulti = 65535 - 50 + 1;
                                } else if (equippedAddition == 10) {
                                    spMulti = 75;
                                } else if (equippedAddition == 11) {
                                    spMulti = 65535 - 40 + 1;
                                } else if (equippedAddition == 12) {
                                    spMulti = 75;
                                }
                            } else {
                                spMulti = 65535 - 50 + 1;
                                if (equippedAddition == 11) {
                                    spMulti = 65535 - 40 + 1;
                                }
                            }
                        } else if (Globals.PARTY_SLOT[i] == 2 || Globals.PARTY_SLOT[i] == 8) {
                            long spScan = emulator.ScanAOB("23 00 00 00 32 00 00 00 46 00 00 00 64", 0xA8660, 0x2A865F);
                            emulator.WriteByteU(spScan, 17);
                            emulator.WriteByteU(spScan + 0x4, 25);
                            emulator.WriteByteU(spScan + 0x8, 35);
                            emulator.WriteByteU(spScan + 0xC, 50);
                            emulator.WriteByteU(spScan + 0x10, 75);
                        } else if (Globals.PARTY_SLOT[i] == 3) {
                            if (additionLevel == 5) {
                                if (equippedAddition == 15) {
                                    spMulti = 75;
                                } else {
                                    spMulti = 65535 - 50 + 1;
                                }
                            } else {
                                spMulti = 65535 - 50 + 1;
                            }
                        } else if (Globals.PARTY_SLOT[i] == 4) {
                            if (additionLevel == 5) {
                                if (equippedAddition == 29) {
                                    spMulti = 65535 - 25 + 1;
                                } else if (equippedAddition == 30) {
                                    spMulti = 65535 - 40 + 1;
                                } else if (equippedAddition == 31) {
                                    spMulti = 0;
                                } else if (equippedAddition == 32) {
                                    spMulti = 65535 - 20 + 1;
                                } else if (equippedAddition == 33) {
                                    spMulti = 65535 - 30 + 1;
                                } else if (equippedAddition == 34) {
                                    spMulti = 50;
                                }
                            } else {
                                spMulti = 65535 - 50 + 1;
                                if (equippedAddition == 30) {
                                    spMulti = 65535 - 40 + 1;
                                } else if (equippedAddition == 32) {
                                    spMulti = 65535 - 40 + 1;
                                } else if (equippedAddition == 33) {
                                    spMulti = 65535 - 30 + 1;
                                }
                            }
                        } else if (Globals.PARTY_SLOT[i] == 6) {
                            if (additionLevel == 5) {
                                if (equippedAddition == 23) {
                                    spMulti = 65535 - 10 + 1;
                                } else if (equippedAddition == 24) {
                                    spMulti = 0;
                                } else if (equippedAddition == 25) {
                                    spMulti = 70;
                                } else if (equippedAddition == 26) {
                                    spMulti = 65535 - 10 + 1;
                                } else if (equippedAddition == 27) {
                                    spMulti = 65535 - 50 + 1;
                                }
                            } else {
                                spMulti = 65535 - 50 + 1;
                                if (equippedAddition == 24) {
                                    spMulti = 65535 - 45 + 1;
                                } else if (equippedAddition == 25) {
                                    spMulti = 65535 - 20 + 1;
                                } else if (equippedAddition == 26) {
                                    spMulti = 65535 - 10 + 1;
                                }
                            }
                        } else if (Globals.PARTY_SLOT[i] == 7) {
                            if (additionLevel == 5) {
                                if (equippedAddition == 19) {
                                    spMulti = 65535 - 25 + 1;
                                } else if (equippedAddition == 20) {
                                    if (dmScripts.ContainsKey("btnAdditionChanges") && dmScripts["btnAdditionChanges"]) {
                                        spMulti = 0;
                                    } else {
                                        spMulti = 65535 - 40 + 1;
                                    }
                                } else if (equippedAddition == 21) {
                                    if (dmScripts.ContainsKey("btnAdditionChanges") && dmScripts["btnAdditionChanges"]) {
                                        spMulti = 65535 - 25 + 1;
                                    } else {
                                        spMulti = 65535 - 50 + 1;
                                    }
                                }
                            } else {
                                if (equippedAddition == 19) {
                                    spMulti = 65535 - 50 + 1;
                                } else if (equippedAddition == 20) {
                                    spMulti = 65535 - 40 + 1;
                                } else if (equippedAddition == 21) {
                                    if (dmScripts.ContainsKey("btnAdditionChanges") && dmScripts["btnAdditionChanges"]) {
                                        spMulti = 65535 - 20 + 1;
                                    } else {
                                        spMulti = 65535 - 50 + 1;
                                    }
                                }
                            }
                        }

                        if (spMulti != 65535) {
                            emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0x112, spMulti);
                        }
                    }
                }
                halfSPOnBattleEntry = true;
            } else {
                if (!Globals.IN_BATTLE && halfSPOnBattleEntry) {
                    halfSPOnBattleEntry = false;
                }
            }
        }
        #endregion

        #region Addition Changes
        public void AdditionDamageChanges() {
            if (Globals.IN_BATTLE && Globals.STATS_CHANGED && !damageIncreaseOnBattleEntry) {
                for (int i = 0; i < 3; i++) {
                    if (Globals.PARTY_SLOT[i] == 1 || Globals.PARTY_SLOT[i] == 5) {
                        if (emulator.ReadByte(Constants.GetAddress("EQUIPPED_ADDITION") + (0x2C * Globals.PARTY_SLOT[i])) == 12) {
                            emulator.WriteByte(Globals.CHAR_ADDRESS[i] + 0x114, (byte) Math.Ceiling(emulator.ReadByte(Globals.CHAR_ADDRESS[i] + 0x114) * 2.2285));
                        }
                    } else if (Globals.PARTY_SLOT[i] == 3) {
                        if (emulator.ReadByte(Constants.GetAddress("EQUIPPED_ADDITION") + (0x2C * Globals.PARTY_SLOT[i])) == 15) {
                            emulator.WriteByte(Globals.CHAR_ADDRESS[i] + 0x114, (byte) (emulator.ReadByte(Globals.CHAR_ADDRESS[i] + 0x114) + 60));
                        } else if (emulator.ReadByte(Constants.GetAddress("EQUIPPED_ADDITION") + (0x2C * Globals.PARTY_SLOT[i])) == 16) {
                            emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0x114, (ushort) Math.Ceiling(emulator.ReadByte(Globals.CHAR_ADDRESS[i] + 0x114) * 1.4));
                        } else if (emulator.ReadByte(Constants.GetAddress("EQUIPPED_ADDITION") + (0x2C * Globals.PARTY_SLOT[i])) == 17) {
                            emulator.WriteByte(Globals.CHAR_ADDRESS[i] + 0x114, (byte) Math.Ceiling(emulator.ReadByte(Globals.CHAR_ADDRESS[i] + 0x114) * 1.1666));
                        }
                    } else if (Globals.PARTY_SLOT[i] == 6) {
                        if (emulator.ReadByte(Constants.GetAddress("EQUIPPED_ADDITION") + (0x2C * Globals.PARTY_SLOT[i])) == 27) {
                            emulator.WriteByte(Globals.CHAR_ADDRESS[i] + 0x114, (byte) Math.Ceiling(emulator.ReadByte(Globals.CHAR_ADDRESS[i] + 0x114) * 0.75));
                        }
                    } else if (Globals.PARTY_SLOT[i] == 7) {
                        if (emulator.ReadByte(Constants.GetAddress("EQUIPPED_ADDITION") + (0x2C * Globals.PARTY_SLOT[i])) == 19) {
                            emulator.WriteByte(Globals.CHAR_ADDRESS[i] + 0x114, (byte) (emulator.ReadByte(Globals.CHAR_ADDRESS[i] + 0x114) * 2));
                        } else if (emulator.ReadByte(Constants.GetAddress("EQUIPPED_ADDITION") + (0x2C * Globals.PARTY_SLOT[i])) == 20) {
                            emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0x112, 80);
                            emulator.WriteByte(Globals.CHAR_ADDRESS[i] + 0x114, (byte) (emulator.ReadByte(Globals.CHAR_ADDRESS[i] + 0x114) * 2));
                        } else if (emulator.ReadByte(Constants.GetAddress("EQUIPPED_ADDITION") + (0x2C * Globals.PARTY_SLOT[i])) == 21) {
                            emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0x112, 50);
                            emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0x114, (ushort) (emulator.ReadByte(Globals.CHAR_ADDRESS[i] + 0x114) * 4));
                        }
                    }
                }
                damageIncreaseOnBattleEntry = true;
            } else {
                if (!Globals.IN_BATTLE && damageIncreaseOnBattleEntry) {
                    damageIncreaseOnBattleEntry = false;
                }
            }
        }
        #endregion

        #region No HP Decay Soul Eater
        public void NoHPDecaySoulEater() {
            if (Globals.IN_BATTLE && Globals.STATS_CHANGED && !noHPDecayOnBattleEntry) {
                for (int i = 0; i < 3; i++) {
                    if (Globals.PARTY_SLOT[i] == 0) {
                        if (emulator.ReadShort(Globals.CHAR_ADDRESS[i] + 0x12C) == 246) {
                            emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0x12C, 0);
                        } else if (emulator.ReadShort(Globals.CHAR_ADDRESS[i] + 0x12C) == 256) {
                            emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0x12C, 10);
                        }
                    }
                }
                noHPDecayOnBattleEntry = true;
            } else {
                if (!Globals.IN_BATTLE && noHPDecayOnBattleEntry) {
                    noHPDecayOnBattleEntry = false;
                }
            }
        }
        #endregion

        #region Monster Names As HP
        public void MonsterHPNames() {
            if (Globals.IN_BATTLE && Globals.STATS_CHANGED) {
                for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                    int lastX = 0;
                    long hpName = Constants.GetAddress("MONSTERS_NAMES") + (i * 0x2C);
                    char[] hpArray = emulator.ReadShort(Globals.MONS_ADDRESS[i]).ToString().ToCharArray();
                    if (ultimateHP[i] > 0) {
                        hpArray = ultimateHP[i].ToString().ToCharArray();
                    }
                    for (int x = 0; x < hpArray.Length; x++) {
                        emulator.WriteShort(hpName + (x * 2), GetNameHP(hpArray[x]));
                        lastX = x;
                    }
                    emulator.WriteInteger(hpName + ((lastX + 1) * 2), 41215);
                }
            }
        }

        public ushort GetNameHP(char single) {
            return (ushort) (emulator.GetCharacterByChar(single) + GetNameHPOffset());
        }

        public ushort GetNameHPOffset() {
            ushort offset = 0x0;
            if (Constants.REGION == Region.JPN) {
                offset += 7;
            }
            return offset;
        }
        #endregion
        #endregion
        #endregion

        #region Settings
        private void ChangeScriptState(string type, SubScript script) {
            try {
                if (script.state != ScriptState.LOCKED) {
                    script.state = script.state == ScriptState.DISABLED ? ScriptState.ENABLED : ScriptState.DISABLED;
                    Constants.WriteOutput("Script '" + script.ToString() + "' is now " + (script.state == ScriptState.DISABLED ? "disabled." : "enabled."));
                    if (script.state == ScriptState.ENABLED) {
                        script.Open(emulator);
                    } else {
                        script.Close(emulator);
                    }
                } else {
                    Constants.WriteOutput("The script '" + script.ToString() + "' is locked and can't be disabled.");
                }
            } catch (Exception ex) {
                Constants.WriteOutput("Please select a " + type + " script.");
            }
        }

        private void miAttach_Click(object sender, RoutedEventArgs e) {
            if (!Constants.RUN) {
                int processID = emulator.getProcIDFromName(Constants.EMULATOR_NAME);

                if (processID > 0) {
                    Constants.RUN = true;
                    emulator.OpenProcess(processID);
                    fieldThread.Start();
                    battleThread.Start();
                    hotkeyThread.Start();
                    otherThread.Start();
                    Constants.WriteOutput("Program opened.");
                } else {
                    Constants.WriteOutput("Program failed to open. Please open " + Constants.EMULATOR_NAME + " then press attach.");
                }
            } else {
                Constants.WriteOutput("Program is already attached to " + Constants.EMULATOR_NAME + ".");
            }
        }

        private void miEmulator_Click(object sender, RoutedEventArgs e) {
            if (!Globals.IN_BATTLE) {
                foreach (MenuItem mi in miEmulator.Items) {
                    mi.IsChecked = (MenuItem) sender == mi ? true : false;
                }

                Constants.EMULATOR = (byte) miEmulator.Items.IndexOf((MenuItem) sender);
                SetupEmulator();
            } else {
                Constants.WriteOutput("You can only change emulators outside of battle.");
            }
        }

        public void SetupEmulator() {
            if (Constants.EMULATOR == 8) {
                Constants.EMULATOR_NAME = "RetroArch";
            } else if (Constants.EMULATOR == 9) {
                Constants.EMULATOR_NAME = "pcsx2";
            } else {
                Constants.EMULATOR_NAME = "ePSXe";
            }

            CloseEmulator();

            switch (Constants.EMULATOR) {
                case 0: //ePSXe 1.6.0
                    Constants.OFFSET = 0x5B6E40;
                    break;
                case 1: //ePSXe 1.7.0
                    Constants.OFFSET = 0x94C020;
                    break;
                case 2: //ePSXe 1.8.0
                    Constants.OFFSET = 0xA52EA0;
                    break;
                case 3: //ePSXe 1.9.0
                    Constants.OFFSET = 0xA579A0;
                    break;
                case 4: //ePSXe 1.9.25
                    Constants.OFFSET = 0xA8B6A0;
                    break;
                case 5: //ePSXe 2.0
                case 6: //ePSXe 2.0.2
                case 7: //ePSXe 2.0.5
                    try {
                        Process emulator = null;
                        foreach (Process p in Process.GetProcessesByName("ePSXe")) {
                            emulator = p;
                        }
                        ProcessModule emMod = emulator.MainModule;

                        if (Constants.EMULATOR == 5) {
                            Constants.OFFSET = (int) (emMod.BaseAddress + 0x81A020);
                        } else if (Constants.EMULATOR == 6) {
                            Constants.OFFSET = (int) (emMod.BaseAddress + 0x825140);
                        } else if (Constants.EMULATOR == 7) {
                            Constants.OFFSET = (int) (emMod.BaseAddress + 0xA82020);
                        }

                    } catch (Exception ex) {
                        Constants.WriteOutput("Address calculation failed. Please open ePSXe.");
                        Constants.RUN = false;
                    }
                    break;
                case 8: //RetroArch Beetle PSX HW
                    try {
                        miAttach_Click(null, null);
                        Process em = null;
                        ProcessModule dll = null;
                        foreach (Process p in Process.GetProcessesByName("retroarch")) {
                            em = p;
                        }
                        foreach (ProcessModule pm in em.Modules) {
                            if (pm.ModuleName == "mednafen_psx_hw_libretro.dll") {
                                dll = pm;
                            }
                        }
                        var scan = emulator.AoBScan((long) (dll.BaseAddress), (long) (dll.BaseAddress + 0xCFFFFF), "82 E3 04 00 00 31 05 00 36 81 05 00 30 D4 05 00", true, true);
                        scan.Wait();
                        var results = scan.Result;
                        long offset = 0;
                        foreach (var x in results)
                            offset = x;
                        Constants.OFFSET = offset - 0x1136C8;
                        if (Constants.OFFSET <= 0) {
                            throw new Exception();
                        }
                    } catch (Exception ex) {
                        Constants.WriteOutput("Address calculation failed. Please open retroarch with Beetle PSX HW at the Load Game screen before loading a save.");
                        Constants.RUN = false;
                    }
                    break;
                case 9: //PCSX2
                    miAttach_Click(null, null);
                    Constants.OFFSET = 0x24000000;
                    break;
            }

            if (Constants.EMULATOR <= 7) {
                miAttach_Click(null, null);
            }
            Constants.ProgramInfo();
        }

        private void miRegion_Click(object sender, RoutedEventArgs e) {
            if (!Globals.IN_BATTLE) {
                foreach (MenuItem mi in miRegion.Items) {
                    mi.IsChecked = (MenuItem) sender == mi ? true : false;
                }
                Constants.REGION = (Region) miRegion.Items.IndexOf((MenuItem) sender);
                Constants.ProgramInfo();

                if (Constants.REGION == Region.JPN) {
                    Hotkey.KEY_CIRCLE = 32;
                    Hotkey.KEY_CROSS = 64;
                } else {
                    Hotkey.KEY_CIRCLE = 64;
                    Hotkey.KEY_CROSS = 32;
                }
            } else {
                Constants.WriteOutput("You can change regions outside of battle only.");
            }
        }

        private void miSaveSlot_Click(object sender, RoutedEventArgs e) {
            if (!Globals.IN_BATTLE) {
                foreach (MenuItem mi in miSaveSlot.Items) {
                    mi.IsChecked = (MenuItem) sender == mi ? true : false;
                }
                Constants.SetSubKey(miSaveSlot.Items.IndexOf((MenuItem) sender));

                foreach (SubScript s in lstField.Items) {
                    if (s.state != ScriptState.DISABLED)
                        s.Open(emulator);
                }
                foreach (SubScript s in lstBattle.Items) {
                    if (s.state != ScriptState.DISABLED)
                        s.Open(emulator);
                }
                foreach (SubScript s in lstHotkey.Items) {
                    if (s.state != ScriptState.DISABLED)
                        s.Open(emulator);
                }
                foreach (SubScript s in lstOther.Items) {
                    if (s.state != ScriptState.DISABLED)
                        s.Open(emulator);
                }

                LoadSubKey();

                Constants.ProgramInfo();
            } else {
                Constants.WriteOutput("You can change save slots outside of battle only.");
            }
        }

        private void miLog_Click(object sender, RoutedEventArgs e) {
            rdLog.Height = miLog.Header.Equals("Expand Log") ? new GridLength(9999, GridUnitType.Star) : new GridLength(2, GridUnitType.Star);
            miLog.Header = miLog.Header.Equals("Expand Log") ? "Collapse Log" : "Expand Log";
        }

        private void btnField_Click(object sender, RoutedEventArgs e) {
            ChangeScriptState("Field", (SubScript) lstField.SelectedItem);
            ScriptDisplay(lstField);
        }

        private void btnBattle_Click(object sender, RoutedEventArgs e) {
            ChangeScriptState("Battle", (SubScript) lstBattle.SelectedItem);
            ScriptDisplay(lstBattle);
        }

        private void btnHotkeys_Click(object sender, RoutedEventArgs e) {
            ChangeScriptState("Hotkeys", (SubScript) lstHotkey.SelectedItem);
            ScriptDisplay(lstHotkey);
        }

        private void btnOther_Click(object sender, RoutedEventArgs e) {
            ChangeScriptState("Other", (SubScript) lstOther.SelectedItem);
            ScriptDisplay(lstOther);
        }

        private void lstField_ScrollChanged(object sender, ScrollChangedEventArgs e) {
            ScriptDisplay(lstField);
        }

        private void lstBattle_ScrollChanged(object sender, ScrollChangedEventArgs e) {
            ScriptDisplay(lstBattle);
        }

        private void lstHotkey_ScrollChanged(object sender, ScrollChangedEventArgs e) {
            ScriptDisplay(lstHotkey);
        }

        private void lstOther_ScrollChanged(object sender, ScrollChangedEventArgs e) {
            ScriptDisplay(lstOther);
        }

        private void lstField_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            OpenScript(lstField);
        }

        private void lstBattle_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            OpenScript(lstBattle);
        }

        private void lstHotkey_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            OpenScript(lstHotkey);
        }

        private void lstOther_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            OpenScript(lstOther);
        }

        private void miNew_Click(object sender, RoutedEventArgs e) {
            DisableScripts();
            Constants.WriteOutput("All scripts have been disabled. Enable the scripts you want to save for the preset then click the Save option.");
        }

        private void miOpen_Click(object sender, RoutedEventArgs e) {
            InputWindow openPresetWindow = new InputWindow("Open Preset");
            TextBox txt = new TextBox();
            openPresetWindow.AddObject(txt);
            openPresetWindow.AddTextBlock("What preset do you want to open?");
            openPresetWindow.ShowDialog();

            if (!txt.Text.Equals("")) {
                if (Constants.LoadPreset(txt.Text)) {
                    ChangeTitle(txt.Text);
                    DisableScripts();
                    Constants.LoadPreset(preset);
                    LoadPreset();
                }
            } else {
                Constants.WriteOutput("Nothing was input.");
            }
        }

        private void miSave_Click(object sender, RoutedEventArgs e) {
            InputWindow openPresetWindow = new InputWindow("Save Preset");
            TextBox txt = new TextBox();
            openPresetWindow.AddObject(txt);
            openPresetWindow.AddTextBlock("What do you want to call your preset?");
            openPresetWindow.ShowDialog();

            if (!txt.Text.Equals("")) {
                ArrayList text = new ArrayList();
                int index = 0;
                foreach (SubScript s in lstField.Items) {
                    index++;
                    if ((s.state == ScriptState.ENABLED || s.state == ScriptState.LOCKED) && index > 1) {
                        text.Add(s.GetPath().Substring(System.IO.Directory.GetCurrentDirectory().Length + 1));
                    }
                }
                index = 0;
                foreach (SubScript s in lstBattle.Items) {
                    index++;
                    if ((s.state == ScriptState.ENABLED || s.state == ScriptState.LOCKED) && index > 1) {
                        text.Add(s.GetPath().Substring(System.IO.Directory.GetCurrentDirectory().Length + 1));
                    }
                }
                index = 0;
                foreach (SubScript s in lstHotkey.Items) {
                    index++;
                    if ((s.state == ScriptState.ENABLED || s.state == ScriptState.LOCKED) && index > 1) {
                        text.Add(s.GetPath().Substring(System.IO.Directory.GetCurrentDirectory().Length + 1));
                    }
                }
                foreach (SubScript s in lstOther.Items) {
                    if (s.state == ScriptState.ENABLED || s.state == ScriptState.LOCKED) {
                        text.Add(s.GetPath().Substring(System.IO.Directory.GetCurrentDirectory().Length + 1));
                    }
                }
                if (text.Count >= 1) {
                    try {
                        using (StreamWriter presetFile = new StreamWriter("Presets\\" + txt.Text + ".csv")) {
                            foreach (string line in text)
                                presetFile.WriteLine(line.Substring(0, line.Length - 3) + ",1");
                        }
                        Constants.WriteOutput("Saved preset '" + txt.Text + "'.");
                    } catch (Exception ex) {
                        Constants.WriteOutput("Error writing file.");
                    }
                } else {
                    Constants.WriteOutput("Nothing was enabled.");
                }
            } else {
                Constants.WriteOutput("Nothing was input.");
            }
        }

        private void miOpenPreset_Click(object sender, RoutedEventArgs e) {
            miOpenPreset.IsChecked = miOpenPreset.IsChecked ? false : true;
        }

        private void miChangeMonster_Click(object sender, RoutedEventArgs e) {
            miChangeMonster.IsChecked = miChangeMonster.IsChecked ? false : true;
            Globals.MONSTER_CHANGE = miChangeMonster.IsChecked ? true : false;
        }

        private void miAuthor_Click(object sender, RoutedEventArgs e) {
            Constants.WriteOutput("-------------");
            Constants.WriteOutput("Author: Zychronix");
            Constants.WriteOutput("https://legendofdragoonhardmode.wordpress.com/");
        }

        private void miCredits_Click(object sender, RoutedEventArgs e) {
            Constants.WriteOutput("-------------");
            Constants.WriteOutput("Program Base: Zychronix");
            Constants.WriteOutput("Monster Base Address: Illeprih");
            Constants.WriteOutput("Memory Functions: erfg12 - memory.dll");
            Constants.WriteOutput("Scripting Engine: CS-Script - https://github.com/oleg-shilo/cs-script/graphs/contributors");
        }

        private void miCredits1_Click(object sender, RoutedEventArgs e) {
            System.Diagnostics.Process.Start("https://legendofdragoonhardmode.wordpress.com/");
        }

        private void miCredits2_Click(object sender, RoutedEventArgs e) {
        }

        private void miCredits3_Click(object sender, RoutedEventArgs e) {
            System.Diagnostics.Process.Start("https://github.com/erfg12/memory.dll/wiki");
        }

        private void miCredits4_Click(object sender, RoutedEventArgs e) {
            System.Diagnostics.Process.Start("https://www.cs-script.net/");
        }

        private void miVersion_Click(object sender, RoutedEventArgs e) {
            Constants.WriteOutput("-------------");
            Constants.WriteOutput("Version 3.0 - Revision 1");
        }
        #endregion

        #region UI
        public void SwitchButton(object sender, EventArgs e) {
            Button btn = (Button) sender;
            if (!dmScripts.ContainsKey(btn.Name)) {
                dmScripts.Add(btn.Name, true);
            } else {
                dmScripts[btn.Name] = dmScripts[btn.Name] ? false : true;
            }

            if (btn.Name.Equals("btnAddPartyMembersOn")) {
                alwaysAddSoloPartyMembers = dmScripts[btn.Name] ? true : false;
            }

            if (btn.Name.Equals("btnKillBGM")) {
                SetKillBGMState();
            }

            TurnOnOffButton(ref btn);
        }

        public void TurnOnOffButton(ref Button sender) {
            if (!dmScripts[sender.Name]) {
                sender.Background = new SolidColorBrush(Color.FromArgb(255, 255, 168, 168));
            } else {
                sender.Background = new SolidColorBrush(Color.FromArgb(255, 168, 211, 255));
            }
        }

        public void GreenButton(object sender, EventArgs e) {
            Button btn = (Button) sender;
            if (btn.Name.Equals("btnAddPartyMembers")) {
                AddSoloPartyMembers();
            } else if (btn.Name.Equals("btnSwitchSoloChar")) {
                SwitchSoloCharacter();
            } else if (btn.Name.Equals("btnElementArrow")) {
                ChangeElementArrow();
            }
        }

        public void ComboBox(object sender, EventArgs e) {
            ComboBox cbo = (ComboBox) sender;
            if (!uiCombo.ContainsKey(cbo.Name)) {
                uiCombo.Add(cbo.Name, cbo.SelectedIndex);
            } else {
                uiCombo[cbo.Name] = cbo.SelectedIndex;
            }

            if (cbo.Name.Equals("cboKillBGM")) {
                SetKillBGMState();
            }
        }
        #endregion

        #region On Close
        private void Window_Closed(object sender, EventArgs e) {
            CloseEmulator();
        }

        public void CloseEmulator() {
            Constants.RUN = false;
            Thread.Sleep(3000);
            SaveKey();
            SaveSubKey();
        }
        #endregion
    }
}