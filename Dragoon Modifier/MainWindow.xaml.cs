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

        public TextBlock[,] monsterDisplay = new TextBlock[5,6];
        public TextBlock[,] characterDisplay = new TextBlock[3,9];
#endregion

        #region Script Variables
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
        public bool soloModeOnBattleEntry = false;
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
            characterDisplay[0,0] = lblCharacter1Name;
            characterDisplay[0,1] = lblCharacter1HMP;
            characterDisplay[0,2] = lblCharacter1ATK;
            characterDisplay[0,3] = lblCharacter1DEF;
            characterDisplay[0,4] = lblCharacter1VHIT;
            characterDisplay[0,5] = lblCharacter1DATK;
            characterDisplay[0,6] = lblCharacter1DDEF;
            characterDisplay[0,7] = lblCharacter1SPD;
            characterDisplay[0,8] = lblCharacter1TRN;
            characterDisplay[1,0] = lblCharacter2Name;
            characterDisplay[1,1] = lblCharacter2HMP;
            characterDisplay[1,2] = lblCharacter2ATK;
            characterDisplay[1,3] = lblCharacter2DEF;
            characterDisplay[1,4] = lblCharacter2VHIT;
            characterDisplay[1,5] = lblCharacter2DATK;
            characterDisplay[1,6] = lblCharacter2DDEF;
            characterDisplay[1,7] = lblCharacter2SPD;
            characterDisplay[1,8] = lblCharacter2TRN;
            characterDisplay[2,0] = lblCharacter3Name;
            characterDisplay[2,1] = lblCharacter3HMP;
            characterDisplay[2,2] = lblCharacter3ATK;
            characterDisplay[2,3] = lblCharacter3DEF;
            characterDisplay[2,4] = lblCharacter3VHIT;
            characterDisplay[2,5] = lblCharacter3DATK;
            characterDisplay[2,6] = lblCharacter3DDEF;
            characterDisplay[2,7] = lblCharacter3SPD;
            characterDisplay[2,8] = lblCharacter3TRN;


            cboSoloLeader.Items.Add("Slot 1");
            cboSoloLeader.Items.Add("Slot 2");
            cboSoloLeader.Items.Add("Slot 3");

            cboSoloLeader.SelectedIndex = 0;
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

                if (dmScripts.ContainsKey("btnSaveAnywhere") && dmScripts["btnSaveAnywhere"])
                    SaveAnywhere();
                if (dmScripts.ContainsKey("btnShopChanges") && dmScripts["btnShopChanges"])
                    ShopChanges();
                if (dmScripts.ContainsKey("btnSoloMode") && dmScripts["btnSoloMode"])
                    SoloModeField();

                foreach (SubScript script in lstField.Items) {
                    if (script.state == ScriptState.DISABLED)
                        continue;
                    currentScript = script.ToString();
                    this.Dispatcher.BeginInvoke(new Action(() => {
                        run = script.Run(emulator);
                    }), DispatcherPriority.ContextIdle);
                }

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

                if (dmScripts.ContainsKey("btnRemoveCaps") && dmScripts["btnRemoveCaps"])
                    RemoveDamageCap();
                //if (dmScripts.ContainsKey("btnSoloMode") && dmScripts["btnSoloMode"])
                //    SoloModeBattle();

                foreach (SubScript script in lstBattle.Items) {
                    if (script.state == ScriptState.DISABLED)
                        continue;
                    currentScript = script.ToString();
                    this.Dispatcher.BeginInvoke(new Action(() => {
                        run = script.Run(emulator);
                    }), DispatcherPriority.ContextIdle);
                }

                Thread.Sleep(250);
                this.Dispatcher.BeginInvoke(new Action(() => {
                    BattleUI();
                }), DispatcherPriority.ContextIdle);
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
                    monsterDisplay[i, 0].Text = emulator.ReadName(0xC69D0 + (0x2C * i));
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
                        characterDisplay[i, 5].Text = " " + emulator.ReadShort(Globals.CHAR_ADDRESS[i] + 0xA4) + "\r\n\r\n " + emulator.ReadShort(Globals.CHAR_ADDRESS[i] + 0xA8);
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
                Constants.WriteGLog("Changed Lohan shop to state: " + shopChange);
                Constants.WriteOutput("Changed Lohan shop to state: " + shopChange);
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
                    if (i != cboSoloLeader.SelectedIndex) {
                        emulator.WriteInteger(Globals.CHAR_ADDRESS[i], 0);
                        emulator.WriteInteger(Globals.CHAR_ADDRESS[i] + 0x8, 0);
                        emulator.WriteShort(Globals.CHAR_ADDRESS[i] + 0x12C, 200);
                        emulator.WriteByte(Globals.CHAR_ADDRESS[i] + 0x45, 25);
                    } else {
                        //battle rows
                    }
                }
                soloModeOnBattleEntry = true;
            } else {
                soloModeOnBattleEntry = false;
            }
        }
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
            Button test = (Button) sender;
            if (!dmScripts.ContainsKey(test.Name)) {
                dmScripts.Add(test.Name, true);
            } else {
                dmScripts[test.Name] = dmScripts[test.Name] ? false : true;
            }

            TurnOnOffButton(ref test);
        }

        public void TurnOnOffButton(ref Button sender) {
            if (!dmScripts[sender.Name]) {
                sender.Background = new SolidColorBrush(Color.FromArgb(255, 255, 168, 168));
            } else {
                sender.Background = new SolidColorBrush(Color.FromArgb(255, 168, 211, 255));
            }
        }

        public void GreenButton(object sender, EventArgs e) {

        }
        #endregion

        #region On Close
        private void Window_Closed(object sender, EventArgs e) {
            CloseEmulator();
        }

        public void CloseEmulator() {
            Constants.RUN = false;
            Thread.Sleep(3000);

            Constants.KEY.SetValue("Save Slot", Constants.SAVE_SLOT);
            if (Constants.EMULATOR != 255)
                Constants.KEY.SetValue("EmulatorType", (int) Constants.EMULATOR);
            Constants.KEY.SetValue("Region", (int) Constants.REGION);
            Constants.KEY.SetValue("LoadPreset", miOpenPreset.IsChecked);
            if (miOpenPreset.IsChecked)
                Constants.KEY.SetValue("Preset", preset);
        }
        #endregion
    }
}