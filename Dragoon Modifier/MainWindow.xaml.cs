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
using System.Text.RegularExpressions;
using System.Globalization;

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
        public Button[] difficultyButton = new Button[3];
        public ProgressBar[] progressMATB = new ProgressBar[5];
        public ProgressBar[] progressCATB = new ProgressBar[3];
        #endregion

        #region Script Variables
        //General
        public bool keepStats = false;
        public double[,] originalCharacterStats = new double[3, 10];
        public double[,] originalMonsterStats = new double[5, 6];
        //Shop Changes
        public bool shopChange = false;
        //Icon Changes
        public bool wroteIcons = false;
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
        public ushort recoveryRateSave = 0;
        public int dartBurnStack = 0;
        public bool burnActive = false;
        public bool dragoonChangesOnBattleEntry = false;
        public bool checkFlowerStorm = false;
        public ushort checkRoseDamageSave = 0;
        public bool checkRoseDamage = false;
        public bool roseEnhanceDragoon = false;
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
        //Ultimate Boss
        public int[] ultimateHP = new int[5];
        public int[] ultimateHPSave = new int[5];
        public int[] ultimateMaxHP = new int[5];
        public int ultimateBossCompleted = 0;
        public int inventorySize = 32;
        public bool ultimateBossOnBattleEntry = false;
        public bool ubHPChanged = false;
        public bool ubCheckedDamage = false;
        public bool ubUltimateHPSet = false;
        public int ubCheckDamageCycle = 0;
        public bool ubGuardBreak = false;
        public bool ubHealingPotion = false;
        public bool ubZeroSP = false;
        public bool ubMPAttack = false;
        public int[] ubMPAttackTrk = new int[3];
        public bool ubWoundDamage = false;
        public ushort[] ubWHP = new ushort[3];
        public ushort[] ubWMHP = new ushort[3];
        public bool ubHealthSteal = false;
        public bool ubHealthStealSave = false;
        public ushort ubHealthStealDamage = 0;
        public bool ubSPAttack = false;
        public bool ubMoveChange = false;
        public ushort[] ubMoveChgTrn = new ushort[5];
        public bool ubMoveChgSet = false;
        public bool ubMagicChange = false;
        public double magicChangeTurns = 0;
        public bool ubElementalShift = false;
        public bool ubArmorShell = false;
        public ushort ubHeartHPSave = 0;
        public byte ubArmorShellTurns = 0;
        public ushort ubArmorShellTP = 0;
        public bool ubSharedHP = false;
        public bool ubRemoveResistances = false;
        public bool ubTPDamage = false;
        public bool ubTrackHPChange = false;
        public bool ubBodyDamage = false;
        public bool ubVirageKilledPart = false;
        public int ubDragoonBondMode = 0;
        public bool ubDragoonBond = false;
        public bool ubDragoonExtras = false;
        public bool ubTrackDragoon = false;
        public bool ubCountdown = false;
        public bool ubUltimateEnrage = false;
        public bool ubInventoryRefresh = false;
        public bool ubEnhancedShield = false;
        public bool ubReverseDBS = false;
        public long ultimateShopLimited = 0;
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
        public bool ubSoasWargod = false;
        public bool ubSoasDragoonBoost = false;
        //Damage Tracker
        public bool damageTrackerOnBattleEntry = false;
        public int[] dmgTrkHP = new int[5];
        public int[] dmgTrkChr = new int[3];
        public int dmgTrkSlot = 0;
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
                Globals.DICTIONARY = new LoDDict();

                if (Constants.EMULATOR != 255) {
                    SetupEmulator();
                } else {
                    Constants.WriteOutput("Please pick an emulator to use in the settings menu.");
                }
            } catch (Exception ex) {
                MessageBox.Show("Error loading Scripts folder.");
                MessageBox.Show(ex.ToString());
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

            difficultyButton[0] = btnNormal;
            difficultyButton[1] = btnHard;
            difficultyButton[2] = btnHell;

            progressCATB[0] = pgrEATBC1;
            progressCATB[1] = pgrEATBC2;
            progressCATB[2] = pgrEATBC3;

            progressMATB[0] = pgrEATBM1;
            progressMATB[1] = pgrEATBM2;
            progressMATB[2] = pgrEATBM3;
            progressMATB[3] = pgrEATBM4;
            progressMATB[4] = pgrEATBM5;

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

            cboUltimateBoss.Items.Add("Zone 1 - Commander II");
            cboUltimateBoss.Items.Add("Zone 1 - Fruegel");
            cboUltimateBoss.Items.Add("Zone 1 - Urobolus");
            cboUltimateBoss.Items.Add("Zone 2 - Sandora Elite");
            cboUltimateBoss.Items.Add("Zone 2 - Drake the Bandit");
            cboUltimateBoss.Items.Add("Zone 2 - Jiango");
            cboUltimateBoss.Items.Add("Zone 2 - Fruegel II");
            cboUltimateBoss.Items.Add("Zone 2 - Fire Bird");
            cboUltimateBoss.Items.Add("Zone 3 - Feyrbrand (Spirit)");
            cboUltimateBoss.Items.Add("Zone 3 - Mappi");
            cboUltimateBoss.Items.Add("Zone 3 - Mappi + Gehrich");
            cboUltimateBoss.Items.Add("Zone 3 - Ghost Commander");
            cboUltimateBoss.Items.Add("Zone 3 - Kamuy");
            cboUltimateBoss.Items.Add("Zone 3 - Regole (Spirit)");
            cboUltimateBoss.Items.Add("Zone 3 - Grand Jewel");
            cboUltimateBoss.Items.Add("Zone 3 - Windigo");
            cboUltimateBoss.Items.Add("Zone 3 - Polter Sword, Helm, and Armor");
            cboUltimateBoss.Items.Add("Zone 3 - Last Kraken");
            cboUltimateBoss.Items.Add("Zone 3 - Kubila, Selebus, and Vector");
            cboUltimateBoss.Items.Add("Zone 3 - Caterpiller");
            cboUltimateBoss.Items.Add("Zone 3 - Zackwell");
            cboUltimateBoss.Items.Add("Zone 3 - Divine Dragon (Spirit)");
            cboUltimateBoss.Items.Add("Zone 4 - Virage I");
            cboUltimateBoss.Items.Add("Zone 4 - Kongol II");
            cboUltimateBoss.Items.Add("Zone 4 - Lenus");
            cboUltimateBoss.Items.Add("Zone 4 - Syuviel");
            cboUltimateBoss.Items.Add("Zone 4 - Virage II");
            cboUltimateBoss.Items.Add("Zone 4 - Greham + Feybrand");
            cboUltimateBoss.Items.Add("Zone 4 - Damia");
            cboUltimateBoss.Items.Add("Zone 4 - Lenus + Regole");
            cboUltimateBoss.Items.Add("Zone 4 - Belzac");
            cboUltimateBoss.Items.Add("Zone 4 - S Virage I");
            cboUltimateBoss.Items.Add("Zone 4 - Kanzas");
            cboUltimateBoss.Items.Add("Zone 4 - Emperor Doel");
            cboUltimateBoss.Items.Add("Zone 4 - S Virage II");
            cboUltimateBoss.Items.Add("Zone 4 - Divine Dragon");
            cboUltimateBoss.Items.Add("Zone 4 - Lloyd");
            cboUltimateBoss.Items.Add("Zone 4 - Magician Faust");
            cboUltimateBoss.Items.Add("Zone 4 - Zieg");
            cboUltimateBoss.Items.Add("Zone 4 - Melbu Frahma");

            cboSoloLeader.SelectedIndex = 0;
            cboAspectRatio.SelectedIndex = 0;
            cboCamera.SelectedIndex = 0;
            cboKillBGM.SelectedIndex = 1;
            cboSwitchChar.SelectedIndex = 0;
            cboElement.SelectedIndex = 0;
            cboUltimateBoss.SelectedIndex = 0;
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

            if (Constants.SUBKEY.GetValue("Inventory Size") == null) {
                Constants.SUBKEY.SetValue("Inventory Size", 32);
                inventorySize = 32;
            } else {
                inventorySize = (int) Constants.SUBKEY.GetValue("Inventory Size");
            }

            if (Constants.SUBKEY.GetValue("Ultimate Shop") == null) {
                Constants.SUBKEY.SetValue("Ultimate Shop", 0);
                ultimateShopLimited = 0;
            } else {
                ultimateShopLimited = Convert.ToInt64(Constants.SUBKEY.GetValue("Ultimate Shop"));
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
            Constants.SUBKEY.SetValue("Inventory Size", inventorySize);
            Constants.SUBKEY.SetValue("Ultimate Shop", ultimateShopLimited);
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
            Constants.SetSubKey(Constants.SAVE_SLOT);
            LoadSubKey();
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

        #region LoDDict
        public class LoDDict {
            List<dynamic> itemList = new List<dynamic>();
            List<string> descriptionList = new List<string>();
            List<string> nameList = new List<string>();
            IDictionary<int, dynamic> statList = new Dictionary<int, dynamic>();
            IDictionary<int, dynamic> ultimateStatList = new Dictionary<int, dynamic>();
            List<int[]>[] shopList = new List<int[]>[39];
            dynamic[][] characterStats = new dynamic[9][];
            dynamic[,,] additionData = new dynamic[9, 8, 8];
            List<int> monsterScript = new List<int>();
            dynamic[][] dragoonStats = new dynamic[9][];
            IDictionary<int, string> num2item = new Dictionary<int, string>();
            IDictionary<string, int> item2num = new Dictionary<string, int>();
            IDictionary<int, string> num2element = new Dictionary<int, string>() {
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
            IDictionary<string, int> element2num = new Dictionary<string, int>() {
                {"", 0 },
                {"none", 0 },
                {"water", 1 },
                {"earth", 2 },
                {"dark", 4 },
                {"non-elemental", 8 },
                {"thunder", 16 },
                {"light", 32 },
                {"wind", 64 },
                {"fire", 128 }
            };
            IDictionary<string, int> status2num = new Dictionary<string, int>() {
                {"none", 0 },
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
                {"po", 128 }
            };

            public List<dynamic> ItemList { get { return itemList; } }
            public List<string> DescriptionList { get { return descriptionList; } }
            public List<string> NameList { get { return nameList; } }
            public IDictionary<int, dynamic> StatList { get { return statList; } }
            public IDictionary<int, dynamic> UltimateStatList { get { return ultimateStatList; } }
            public List<int[]>[] ShopList { get { return shopList; } }
            public dynamic[][] CharacterStats { get { return characterStats; } }
            public dynamic[, ,] AdditionData { get { return additionData; } }
            public List<int> MonsterScript { get { return monsterScript; } }
            public IDictionary<int, string> Num2Item { get { return num2item; } }
            public IDictionary<string, int> Item2Num { get { return item2num; } }
            public IDictionary<int, string> Num2Element { get { return num2element; } }
            public IDictionary<string, int> Element2Num { get { return element2num; } }
            public IDictionary<string, int> Status2Num { get { return status2num; } }
            public dynamic[][] DragoonStats { get { return dragoonStats; } }

            public LoDDict() {
                string cwd = AppDomain.CurrentDomain.BaseDirectory;       
                try {
                    using (var itemData = new StreamReader(cwd + "Mods/" + Globals.MOD + "/Items.tsv")) {
                        bool firstline = true;
                        int i = 0;
                        while (!itemData.EndOfStream) {
                            var line = itemData.ReadLine();
                            if (firstline == false) {
                                var values = line.Split('\t').ToArray();
                                itemList.Add(new ItemList(i, values));
                                if (values[0] != "") {
                                    item2num.Add(values[0].ToLower(), i);
                                    num2item.Add(i, values[0]);
                                }
                                i++;
                            } else {
                                firstline = false;
                            }
                        }
                    }
                    long offset = 0x0;
                    long start = 0x80000000 | Constants.GetAddress("ITEM_DESC");
                    List<dynamic> sortedList = itemList.OrderByDescending(o => o.Description.Length).ToList();
                    foreach (dynamic item in sortedList) {
                        if (descriptionList.Any(l => l.Contains(item.EncodedDescription)) == true) {
                            int index = sortedList.IndexOf(sortedList.Find(x => x.EncodedDescription.Contains(item.EncodedDescription)));
                            item.DescriptionPointer = sortedList[index].DescriptionPointer + (sortedList[index].Description.Length - item.Description.Length) * 2;
                        } else {
                            descriptionList.Add(item.EncodedDescription);
                            item.DescriptionPointer = start + offset;
                            offset += (item.EncodedDescription.Replace(" ", "").Length / 2);
                        }
                    }
                    offset = 0;
                    start = 0x80000000 | Constants.GetAddress("ITEM_NAME");
                    sortedList = itemList.OrderByDescending(o => o.Name.Length).ToList();
                    foreach (dynamic item in sortedList) {
                        if (nameList.Any(l => l.Contains(item.EncodedName)) == true) {
                            int index = sortedList.IndexOf(sortedList.Find(x => x.EncodedName.Contains(item.EncodedName)));
                            item.NamePointer = sortedList[index].NamePointer + (sortedList[index].Name.Length - item.Name.Length) * 2;
                        } else {
                            nameList.Add(item.EncodedName);
                            item.NamePointer = start + (int)offset;
                            offset += (item.EncodedName.Replace(" ", "").Length / 2);
                        }
                    }
                    for (int i = 0; i < characterStats.Length; i++) {
                        characterStats[i] = new dynamic[61];
                    }
                    try { 
                        using (var characterData = new StreamReader(cwd + "Mods/" + Globals.MOD + "/Character_Stats.tsv")) {
                            var i = 0;
                            while (!characterData.EndOfStream) {
                                var line = characterData.ReadLine();
                                if (i > 1) {
                                    var values = line.Split('\t').ToArray();
                                    int level = int.Parse(values[0]);
                                    characterStats[0][level] = new CharacterStats(values[1], values[2], values[3], values[4], values[5], values[6]);
                                    characterStats[1][level] = new CharacterStats(values[7], values[8], values[9], values[10], values[11], values[12]);
                                    characterStats[2][level] = new CharacterStats(values[13], values[14], values[15], values[16], values[17], values[18]);
                                    characterStats[3][level] = new CharacterStats(values[19], values[20], values[21], values[22], values[23], values[24]);
                                    characterStats[4][level] = new CharacterStats(values[25], values[26], values[27], values[28], values[29], values[30]);
                                    characterStats[5][level] = new CharacterStats(values[7], values[8], values[9], values[10], values[11], values[12]);
                                    characterStats[6][level] = new CharacterStats(values[31], values[32], values[33], values[34], values[35], values[36]);
                                    characterStats[7][level] = new CharacterStats(values[37], values[38], values[39], values[40], values[41], values[42]);
                                    characterStats[8][level] = new CharacterStats(values[13], values[14], values[15], values[16], values[17], values[18]);
                                }
                                i++;
                            }
                        }
                    } catch (FileNotFoundException) {
                        string file = cwd + @"Mods\" + Globals.MOD + @"\Character_Stats.tsv";
                        Constants.WriteDebug(file + " not found. Turning off Stat and Equip Changes.");
                    }
                    try {
                        using (var monsterData = new StreamReader(cwd + "Mods/" + Globals.MOD + "/Monster_Data.tsv")) {
                            bool firstline = true;
                            while (!monsterData.EndOfStream) {
                                var line = monsterData.ReadLine();
                                if (firstline == false) {
                                    var values = line.Split('\t').ToArray();
                                    statList.Add(Int32.Parse(values[0]), new StatList(values, element2num, item2num));
                                } else {
                                    firstline = false;
                                }
                            }
                        }
                    } catch (FileNotFoundException) {
                        string file = cwd + @"Mods\" + Globals.MOD + @"\Monster_Data.tsv";
                        Constants.WriteDebug(file + " not found. Turning off Monster and Drop Changes.");
                        Globals.MONSTER_CHANGE = false;
                        Globals.DROP_CHANGE = false;
                    }
                    try {
                        using (var monsterData = new StreamReader(cwd + "Mods/" + Globals.MOD + "/Ultimate_Data.tsv")) {
                            bool firstline = true;
                            while (!monsterData.EndOfStream) {
                                var line = monsterData.ReadLine();
                                if (firstline == false) {
                                    var values = line.Split('\t').ToArray();
                                    ultimateStatList.Add(Int32.Parse(values[0]), new StatList(values, element2num, item2num));
                                } else {
                                    firstline = false;
                                }
                            }
                        }
                    } catch (FileNotFoundException) {
                        string file = cwd + @"Mods\" + Globals.MOD + @"\Ultimate_Data.tsv";
                        Constants.WriteDebug(file + " not found.");
                    }
                } catch (FileNotFoundException) {
                    string file = cwd + @"Mods\" + Globals.MOD + @"\Items.tsv";
                    Constants.WriteDebug(file + " not found. Turning off Monster and Drop Changes.");
                    Globals.MONSTER_CHANGE = false;
                    Globals.DROP_CHANGE = false;
                }
                try {
                    string[] lines = File.ReadAllLines(cwd + @"Mods\" + Globals.MOD + @"\Monster_Script.txt");
                    foreach (string row in lines) {
                        if (row != "") {
                            monsterScript.Add(Int32.Parse(row));
                        }
                    }
                } catch (FileNotFoundException) {
                    string file = cwd + @"Mods\" + Globals.MOD + @"\Monster_Script.txt";
                    Constants.WriteDebug(file + " not found.");
                }
                try {
                    using (var dragoon = new StreamReader(cwd + "Mods/" + Globals.MOD + "/Dragoon_Stats.tsv")) {
                        bool firstline = true;
                        var i = 0;
                        while (!dragoon.EndOfStream) {
                            var line = dragoon.ReadLine();
                            if (firstline == false) {
                                var values = line.Split('\t').ToArray();
                                dragoonStats[i] = new dynamic[] {
                                    new DragoonStats("0", "0", "0", "0", "0"),
                                    new DragoonStats(values[1], values[2], values[3], values[4], values[5]),
                                    new DragoonStats(values[6], values[7], values[8], values[9], values[10]),
                                    new DragoonStats(values[11], values[12], values[13], values[14], values[15]),
                                    new DragoonStats(values[16], values[17], values[18], values[19], values[20]),
                                    new DragoonStats(values[21], values[22], values[23], values[24], values[25])
                                };
                                i++;
                            } else {
                                firstline = false;
                            }
                        }
                    }
                } catch (FileNotFoundException) {
                    string file = cwd + @"Mods\" + Globals.MOD + @"\Dragoon_Stats.tsv";
                    Constants.WriteDebug(file + " not found. Turning off Dragoon Changes.");
                    Globals.DRAGOON_CHANGE = false;
                }
                for (int i = 0; i < shopList.Length; i++) {
                    shopList[i] = new List<int[]>();
                }
                try {
                    int key = 0;
                    using (var shop = new StreamReader(cwd + "Mods/" + Globals.MOD + "/Shops.tsv")) {
                        var row = 0;
                        while (!shop.EndOfStream) {
                            var line = shop.ReadLine();
                            if (row > 1) {
                                var values = line.Split('\t').ToArray();
                                int column = 0;
                                foreach (string number in values) {
                                    if (column % 2 == 0 && number != "") {
                                        if (item2num.TryGetValue(number.ToLower(), out key)) {
                                            var array = new int[] {
                                    key, Int32.Parse(values[column + 1])
                                    };
                                            shopList[column / 2].Add(array);
                                        } else {
                                            Constants.WriteDebug("Incorrect item " + number + " in ShopList at Row: " + row + " Column: " + column);
                                        }
                                    }
                                    column++;
                                }
                            }
                            row++;
                        }
                    }
                } catch (FileNotFoundException) {
                    string file = cwd + @"Mods\" + Globals.MOD + @"\Shops.tsv";
                    Constants.WriteDebug(file + " not found. Turning off Shop Changes.");
                    Globals.SHOP_CHANGE = false;
                }
                try {
                    using (var shop = new StreamReader(cwd + "Mods/" + Globals.MOD + "/Dragoon_Spells.tsv")) {
                        var i = 0;
                        while (!shop.EndOfStream) {
                            var line = shop.ReadLine();
                            if (i > 0) {
                                var values = line.Split('\t').ToArray();
                                Globals.DRAGOON_SPELLS.Add(new DragoonSpells(values, element2num));
                            }
                            i++;
                        }
                    }
                } catch (FileNotFoundException) {
                    string file = cwd + @"Mods\" + Globals.MOD + @"\Dragoon_Spells.tsv";
                    Constants.WriteDebug(file + " not found. Turning off Dragoon Changes.");
                    Globals.DRAGOON_CHANGE = false;
                }
                using (var addition = new StreamReader(cwd + "Mods/" + Globals.MOD + "/Additions.tsv")) {
                    var i = 0;
                    bool firstline = true;
                    while (!addition.EndOfStream) {
                        var line = addition.ReadLine();
                        if (firstline == false) {
                            var values = line.Split('\t').ToArray();
                            additionData[0, i / 8, i % 8] = new AdditionData(values.Skip(1).Take(26).ToArray());
                            additionData[1, i / 8, i % 8] = new AdditionData(values.Skip(28).Take(53).ToArray());
                            additionData[2, i / 8, i % 8] = new AdditionData(values.Skip(55).Take(80).ToArray());
                            additionData[3, i / 8, i % 8] = new AdditionData(values.Skip(82).Take(107).ToArray());
                            additionData[4, i / 8, i % 8] = new AdditionData(values.Skip(109).Take(134).ToArray());
                            additionData[5, i / 8, i % 8] = new AdditionData(values.Skip(136).Take(161).ToArray());
                            additionData[6, i / 8, i % 8] = new AdditionData(values.Skip(163).Take(188).ToArray());
                            additionData[7, i / 8, i % 8] = new AdditionData(values.Skip(190).Take(215).ToArray());
                            additionData[8, i / 8, i % 8] = new AdditionData(values.Skip(217).Take(242).ToArray());
                            i++;
                        } else {
                            firstline = false;
                        }
                    }
                }
            }
        }

        public class ItemList {
            int id = 0;
            string name = "";
            string description = "";
            string encodedName = "00 00 FF A0";
            string encodedDescription = "00 00 FF A0";
            long descriptionPointer = 0;
            long namePointer = 0;
            byte icon = 0;
            byte equips = 0;
            byte type = 0;
            byte element = 0;
            byte status = 0;
            byte status_chance = 0;
            byte at = 0;
            byte mat = 0;
            byte df = 0;
            byte mdf = 0;
            byte spd = 0;
            byte a_hit = 0;
            byte m_hit = 0;
            byte a_av = 0;
            byte m_av = 0;
            byte e_half = 0;
            byte e_immune = 0;
            byte stat_res = 0;
            byte special1 = 0;
            byte special2 = 0;
            short special_ammount = 0;
            byte death_res = 0;

            Dictionary<string, byte> iconDict = new Dictionary<string, byte>() {
                { "sword", 0 },
                { "axe", 1 },
                { "hammer", 2 },
                { "spear", 3 },
                { "bow", 4 },
                { "mace", 5 },
                { "knuckle", 6 },
                { "boxing glove", 7 },
                { "clothes", 8 },
                { "robe", 9 },
                { "armor", 10 },
                { "breastplate", 11 },
                { "red dress", 12 },
                { "loincloth", 13 },
                { "warrior dress", 14 },
                { "crown", 15 },
                { "hairband", 16 },
                { "bandana", 16 },
                { "hat", 17 },
                { "helm", 18 },
                { "shoes", 19 },
                { "kneepiece", 20 },
                { "boots", 21 },
                { "bracelet", 22 },
                { "ring", 23 },
                { "amulet", 24 },
                { "stone", 25 },
                { "jewelery", 26 },
                { "pin", 27 },
                { "bell", 28 },
                { "bag", 29 },
                { "cloak", 30 },
                { "scarf", 30 },
                { "horn", 32 },
                {"none", 64 },
                {"", 64 }
            };
            Dictionary<string, byte> charDict = new Dictionary<string, byte>() {
                {"meru", 1 },
                {"shana", 2 },
                {"miranda", 2 },
                {"???", 2 },
                {"rose", 4 },
                {"haschel", 16 },
                {"kongol", 32 },
                {"lavitz", 64 },
                {"albert", 64 },
                {"dart", 128 },
                {"female", 7 },
                {"male", 240 },
                {"all", 247 },
                {"none", 0 },
                {"", 0 }
            };
            Dictionary<string, byte> typeDict = new Dictionary<string, byte>() {
                {"weapon", 128 },
                {"armor", 32 },
                {"helm", 64 },
                {"boots", 16 },
                {"accessory", 8 },
                {"none", 0},
                {"", 0 }
            };
            Dictionary<string, byte> element2num = new Dictionary<string, byte>() {
                {"", 0 },
                {"none", 0 },
                {"water", 1 },
                {"earth", 2 },
                {"dark", 4 },
                {"non-elemental", 8 },
                {"thunder", 16 },
                {"light", 32 },
                {"wind", 64 },
                {"fire", 128 }
            };
            Dictionary<string, byte> status2num = new Dictionary<string, byte>() {
                {"", 0 },
                {"none", 0 },
                {"death", 0 },
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
            Dictionary<string, byte> special12num = new Dictionary<string, byte>() {
                {"", 0 },
                {"none", 0 },
                {"mp_m_hit", 1 },
                {"sp_m_hit", 2 },
                {"mp_p_hit", 4 },
                {"sp_p_hit", 8 },
                {"sp_multi", 16 },
                {"p_half", 32 }
            };
            Dictionary<string, byte> special22num = new Dictionary<string, byte>() {
                {"", 0 },
                {"none", 0 },
                {"mp_multi", 1 },
                {"hp_multi", 2 },
                {"m_half", 4 },
                {"revive", 8 },
                {"sp_regen", 16 },
                {"mp_regen", 32 },
                {"hp_regen", 64 }
            };
            Dictionary<string, byte> death2num = new Dictionary<string, byte>() {
                {"", 0 },
                {"none", 0 },
                {"dragon_buster", 4 },
                {"attack_all", 8 },
                {"death_chance", 64 },
                {"death_res", 128 }
            };

            public int ID { get { return id; } }
            public string Name { get { return name; } }
            public string Description { get { return description; } }
            public string EncodedName { get { return encodedName; } }
            public string EncodedDescription { get { return encodedDescription; } }
            public long DescriptionPointer { get; set; }
            public long NamePointer { get; set; }
            public byte Icon { get { return icon; } }
            public byte Equips { get { return equips; } }
            public byte Type { get { return type; } }
            public byte Element { get { return element; } }
            public byte Status { get { return status; } }
            public byte Status_Chance { get { return status_chance; } }
            public byte AT { get { return at; } }
            public byte MAT { get { return mat; } }
            public byte DF { get { return df; } }
            public byte MDF { get { return mdf; } }
            public byte SPD { get { return spd; } }
            public byte A_Hit { get { return a_hit; } }
            public byte M_Hit { get { return m_hit; } }
            public byte A_AV { get { return a_av; } }
            public byte M_AV { get { return m_av; } }
            public byte E_Half { get { return e_half; } }
            public byte E_Immune { get { return e_immune; } }
            public byte Stat_Res { get { return stat_res; } }
            public byte Special1 { get { return special1; } }
            public byte Special2 { get { return special2; } }
            public short Special_Ammount { get { return special_ammount; } }
            public byte Death_Res { get { return death_res; } }

            public ItemList(int index, string[] values) {
                byte key = 0;
                short key2 = 0;
                id = index;
                name = values[0];
                if (name != "") {
                    encodedName = StringEncode(name);
                }
                if (typeDict.TryGetValue(values[1].ToLower(), out key)) {
                    type = key;
                } else {
                    Constants.WriteDebug(values[1] + " not found as equipment type for item: " + name);
                }
                foreach (string substring in values[2].Replace(" ", "").Split(',')) {
                    if (charDict.TryGetValue(substring.ToLower(), out key)) {
                        equips |= key;
                    } else {
                        Constants.WriteDebug(substring + " not found as character for item: " + name);
                    }
                }
                if (iconDict.TryGetValue(values[3].ToLower(), out key)) {
                    icon = key;
                } else {
                    Constants.WriteDebug(values[3] + " not found as icon for item: " + name);
                }
                if (element2num.TryGetValue(values[4].ToLower(), out key)) {
                    element = key;
                } else {
                    Constants.WriteDebug(values[4] + " not found as element for item: " + name);
                }
                if (status2num.TryGetValue(values[5].ToLower(), out key)) {
                    status = key;
                } else {
                    Constants.WriteDebug(values[5] + " not found as Status for item: " + name);
                }
                if (Byte.TryParse(values[6], NumberStyles.AllowLeadingSign, null as IFormatProvider, out key)) {
                    status_chance = key;
                } else if (values[6] != "") {
                    Constants.WriteDebug(values[6] + " not found as Status Chance for item: " + name);
                }
                if (Byte.TryParse(values[7], NumberStyles.AllowLeadingSign, null as IFormatProvider, out key)) {
                    at = key;
                } else if (values[7] != "") {
                    Constants.WriteDebug(values[7] + " not found as AT for item: " + name);
                }
                if (Byte.TryParse(values[8], NumberStyles.AllowLeadingSign, null as IFormatProvider, out key)) {
                    mat = key;
                } else if (values[8] != "") {
                    Constants.WriteDebug(values[8] + " not found as MAT for item: " + name);
                }
                if (Byte.TryParse(values[9], NumberStyles.AllowLeadingSign, null as IFormatProvider, out key)) {
                    df = key;
                } else if (values[9] != "") {
                    Constants.WriteDebug(values[9] + " not found as DF for item: " + name);
                }
                if (Byte.TryParse(values[10], NumberStyles.AllowLeadingSign, null as IFormatProvider, out key)) {
                    mdf = key;
                } else if (values[10] != "") {
                    Constants.WriteDebug(values[10] + " not found as MDF for item: " + name);
                }
                if (Byte.TryParse(values[11], NumberStyles.AllowLeadingSign, null as IFormatProvider, out key)) {
                    spd = key;
                } else if (values[11] != "") {
                    Constants.WriteDebug(values[11] + " not found as SPD for item: " + name);
                }
                if (Byte.TryParse(values[12], NumberStyles.AllowLeadingSign, null as IFormatProvider, out key)) {
                    a_hit = key;
                } else if (values[12] != "") {
                    Constants.WriteDebug(values[12] + " not found as A_Hit for item: " + name);
                }
                if (Byte.TryParse(values[13], NumberStyles.AllowLeadingSign, null as IFormatProvider, out key)) {
                    m_hit = key;
                } else if (values[13] != "") {
                    Constants.WriteDebug(values[13] + " not found as M_Hit for item: " + name);
                }
                if (Byte.TryParse(values[14], NumberStyles.AllowLeadingSign, null as IFormatProvider, out key)) {
                    a_av = key;
                } else if (values[14] != "") {
                    Constants.WriteDebug(values[14] + " not found as A_AV for item: " + name);
                }
                if (Byte.TryParse(values[15], NumberStyles.AllowLeadingSign, null as IFormatProvider, out key)) {
                    m_av = key;
                } else if (values[15] != "") {
                    Constants.WriteDebug(values[15] + " not found as M_AV for item: " + name);
                }
                foreach (string substring in values[16].Replace(" ", "").Split(',')) {
                    if (element2num.TryGetValue(substring.ToLower(), out key)) {
                        e_half |= key;
                    } else {
                        Constants.WriteDebug(substring + " not found as E_Half for item:" + name);
                    }
                }
                foreach (string substring in values[17].Replace(" ", "").Split(',')) {
                    if (element2num.TryGetValue(substring.ToLower(), out key)) {
                        e_immune |= key;
                    } else {
                        Constants.WriteDebug(substring + " not found as E_Immune for item:" + name);
                    }
                }
                foreach (string substring in values[18].Replace(" ", "").Split(',')) {
                    if (status2num.TryGetValue(substring.ToLower(), out key)) {
                        stat_res |= key;
                    } else {
                        Constants.WriteDebug(substring + " not found as Status_Resist for item:" + name);
                    }
                }
                foreach (string substring in values[19].Replace(" ", "").Split(',')) {
                    if (special12num.TryGetValue(substring.ToLower(), out key)) {
                        special1 |= key;
                    } else {
                        Constants.WriteDebug(substring + " not found as Special1 for item:" + name);
                    }
                }
                foreach (string substring in values[20].Replace(" ", "").Split(',')) {
                    if (special22num.TryGetValue(substring.ToLower(), out key)) {
                        special2 |= key;
                    } else {
                        Constants.WriteDebug(substring + " not found as Special2 for item:" + name);
                    }
                }
                if (Int16.TryParse(values[21], NumberStyles.AllowLeadingSign, null as IFormatProvider, out key2)) {
                    special_ammount = key2;
                } else if (values[21] != "") {
                    Constants.WriteDebug(values[21] + " not found as Special_Ammount for item: " + name);
                }
                foreach (string substring in values[22].Replace(" ", "").Split(',')) {
                    if (death2num.TryGetValue(substring.ToLower(), out key)) {
                        death_res |= key;
                    } else {
                        Constants.WriteDebug(substring + " not found as Death_Res for item:" + name);
                    }
                }
                description = values[23];
                if (description != "") {
                    encodedDescription = StringEncode(description);
                }
            }
        }

        public class StatList {
            string name = "Monster";
            int element = 128;
            int hp = 1;
            int at = 1;
            int mat = 1;
            int df = 1;
            int mdf = 1;
            int spd = 1;
            int a_av = 0;
            int m_av = 0;
            int p_immune = 0;
            int m_immune = 0;
            int p_half = 0;
            int m_half = 0;
            int e_immune = 0;
            int e_half = 0;
            int stat_res = 0;
            int death_res = 0;
            int exp = 0;
            int gold = 0;
            int drop_item = 255;
            int drop_chance = 0;

            public string Name { get { return name; } }
            public int Element { get { return element; } }
            public int HP { get { return hp; } }
            public int AT { get { return at; } }
            public int MAT { get { return mat; } }
            public int DF { get { return df; } }
            public int MDF { get { return mdf; } }
            public int SPD { get { return spd; } }
            public int A_AV { get { return a_av; } }
            public int M_AV { get { return m_av; } }
            public int P_Immune { get { return p_immune; } }
            public int M_Immune { get { return m_immune; } }
            public int P_Half { get { return p_half; } }
            public int M_Half { get { return m_half; } }
            public int E_Immune { get { return e_immune; } }
            public int E_Half { get { return e_half; } }
            public int Stat_Res { get { return stat_res; } }
            public int Death_Res { get { return death_res; } }
            public int EXP { get { return exp; } }
            public int Gold { get { return gold; } }
            public int Drop_Item { get { return drop_item; } }
            public int Drop_Chance { get { return drop_chance; } }

            public StatList(string[] monster, IDictionary<string, int> element2num, IDictionary<string, int> item2num) {
                name = monster[1];
                int key = 0;
                if (element2num.TryGetValue(monster[2].ToLower(), out key)) {
                    element = key;
                } else {
                    Constants.WriteDebug(monster[2] + " not found as element for " + monster[1] + " (ID " + monster[0] + ")");
                }
                hp = Int32.Parse(monster[3]);
                at = Int32.Parse(monster[4]);
                mat = Int32.Parse(monster[5]);
                df = Int32.Parse(monster[6]);
                mdf = Int32.Parse(monster[7]);
                spd = Int32.Parse(monster[8]);
                a_av = Int32.Parse(monster[9]);
                m_av = Int32.Parse(monster[10]);
                p_immune = Int32.Parse(monster[11]);
                m_immune = Int32.Parse(monster[12]);
                p_half = Int32.Parse(monster[13]);
                m_half = Int32.Parse(monster[14]);
                foreach (string substring in monster[15].Replace(" ", "").Split(',')) {
                    if (element2num.TryGetValue(substring.ToLower(), out key)) {
                        e_immune |= key;
                    } else {
                        Constants.WriteDebug(substring + " not found as e_immune for " + monster[1] + " (ID " + monster[0] + ")");
                    }
                }
                foreach (string substring in monster[16].Replace(" ", "").Split(',')) {
                    if (element2num.TryGetValue(substring.ToLower(), out key)) {
                        e_half |= key;
                    } else {
                        Constants.WriteDebug(substring + " not found as e_half for " + monster[1] + " (ID " + monster[0] + ")");
                    }
                }
                stat_res = Int32.Parse(monster[17]);
                death_res = Int32.Parse(monster[18]);
                exp = Int32.Parse(monster[19]);
                gold = Int32.Parse(monster[20]);
                if (item2num.TryGetValue(monster[21].ToLower(), out key)) {
                    drop_item = key;
                } else {
                    Constants.WriteDebug(monster[21] + " not found in Item List as drop for " + monster[1] + " (ID " + monster[0] + ")");
                }
                drop_chance = Int32.Parse(monster[22]);
            }
        }

        public class DragoonStats {
            byte dat = 0;
            byte dmat = 0;
            byte ddf = 0;
            byte dmdf = 0;
            ushort mp = 0;

            public byte DAT { get { return dat; } }
            public byte DMAT { get { return dmat; } }
            public byte DDF { get { return ddf; } }
            public byte DMDF { get { return dmdf; } }
            public ushort MP { get { return mp; } }

            public DragoonStats(string ndat, string ndmat, string nddf, string ndmdf, string nmp) {
                byte key = 0;
                if (Byte.TryParse(ndat, NumberStyles.AllowLeadingSign, null as IFormatProvider, out key)) {
                    dat = key;
                } else if (ndat != "") {
                    Constants.WriteDebug(ndat + " not found as D-AT");
                }
                if (Byte.TryParse(ndmat, NumberStyles.AllowLeadingSign, null as IFormatProvider, out key)) {
                    dmat = key;
                } else if (ndmat != "") {
                    Constants.WriteDebug(ndmat + " not found as D-MAT");
                }
                if (Byte.TryParse(nddf, NumberStyles.AllowLeadingSign, null as IFormatProvider, out key)) {
                    ddf = key;
                } else if (nddf != "") {
                    Constants.WriteDebug(nddf + " not found as D-DF");
                }
                if (Byte.TryParse(ndmdf, NumberStyles.AllowLeadingSign, null as IFormatProvider, out key)) {
                    dmdf = key;
                } else if (ndmdf != "") {
                    Constants.WriteDebug(ndmdf + " not found as D-MDF");
                }
                if (UInt16.TryParse(nmp, NumberStyles.AllowLeadingSign, null as IFormatProvider, out ushort shortkey)) {
                    mp = shortkey;
                } else if (nmp != "") {
                    Constants.WriteDebug(ndmdf + " not found as MP");
                }
            }
        }

        public class DragoonSpells {
            bool percentage = false;
            byte dmg_base = 0;
            byte multi = 0;
            byte accuracy = 100;
            byte mp = 10;
            byte element = 128;
            IDictionary<string, bool> perc = new Dictionary<string, bool> {
                { "yes", true},
                { "no", false },
                { "true", true },
                { "false", false },
                { "1", true },
                { "0", false },
                { "", false}
            };

            public bool Percentage { get { return percentage; } }
            public byte DMG_Base { get { return dmg_base; } }
            public byte Multi { get { return multi; } }
            public byte Accuracy { get { return accuracy; } }
            public byte MP { get { return mp; } }
            public byte Element { get { return element; } }

            public DragoonSpells(string[] values, IDictionary<string, int> Element2Num) {
                bool key = new bool();
                if (perc.TryGetValue(values[1].ToLower(), out key)) {
                    percentage = key;
                } else {
                    Constants.WriteDebug("Incorrect percentage swith " + values[1] + " for spell " + values[0]);
                }
                double damage = Convert.ToDouble(values[2]);
                if (percentage == true) {
                    dmg_base = 0;
                    multi = (byte)Math.Round(damage);
                } else {
                    double[] bases = new double[] { 800, 600, 500, 400, 300, 200, 100, 75, 50 };
                    byte[] base_table = new byte[] { 1, 2, 4, 8, 16, 32, 0, 64, 128 };
                    double[] nearest_list = new double[9];
                    byte[] multi_list = new byte[9];
                    for (int i = 0; i < 9; i++) {
                        if (damage < bases[i]) {
                            nearest_list[i] = bases[i] - damage;
                            multi_list[i] = 0;
                        } else if (damage > (bases[i] * 2.275)) {
                            nearest_list[i] = damage - bases[i] * 2.275;
                            multi_list[i] = 255;
                        } else {
                            double mod = (damage - bases[i]) % (bases[i] / 200);
                            if (mod < (bases[i] / 400)) {
                                nearest_list[i] = mod;
                                multi_list[i] = (byte)Math.Round((damage - bases[i]) / (bases[i] / 200));
                            } else {
                                nearest_list[i] = (bases[i] / 200) - mod;
                                multi_list[i] = (byte)Math.Round((damage - bases[i]) / (bases[i] / 200) + 1);
                            }
                        }
                    }
                    int index = Array.IndexOf(nearest_list, nearest_list.Min());
                    dmg_base = base_table[index];
                    multi = multi_list[index];
                }
                accuracy = (byte)Convert.ToInt32(values[3]);
                mp = (byte)Convert.ToInt32(values[4]);
                element = (byte)Element2Num[values[5].ToLower()];
            }
        }

        public class CharacterStats {
            short max_hp = 1;
            byte spd = 1;
            byte at = 1;
            byte mat = 1;
            byte df = 1;
            byte mdf = 1;

            public short Max_HP { get { return max_hp; } }
            public byte SPD { get { return spd; } }
            public byte AT { get { return at; } }
            public byte MAT { get { return mat; } }
            public byte DF { get { return df; } }
            public byte MDF { get { return mdf; } }
            
            public CharacterStats(string nmax_hp, string nspd, string nat, string nmat, string ndf, string nmdf) {
                max_hp = Int16.Parse(nmax_hp);
                spd = Convert.ToByte(nspd, 10);
                at = Convert.ToByte(nat, 10);
                mat = Convert.ToByte(nmat, 10);
                df = Convert.ToByte(ndf, 10);
                mdf = Convert.ToByte(nmdf, 10);
            }
        }

        public class AdditionData {
            short uu1 = 0;
            short next_hit = 0;
            short blue_time = 0;
            short gray_time = 0;
            short dmg = 0;
            short sp = 0;
            short id = 0;
            byte final_hit = 0;
            byte uu2 = 0;
            byte uu3 = 0;
            byte uu4 = 0;
            byte uu5 = 0;
            byte uu6 = 0;
            byte uu7 = 0;
            byte uu8 = 0;
            byte uu9 = 0;
            byte uu10 = 0;
            short vertical_distance = 0;
            byte uu11 = 0;
            byte uu12 = 0;
            byte uu13 = 0;
            byte uu14 = 0;
            byte start_time = 0;
            byte uu15 = 0;
            short add_dmg_multi = 0;
            short add_sp_multi = 0;

            public short UU1 { get { return uu1; } }
            public short Next_Hit { get { return next_hit; } }
            public short Blue_Time { get { return blue_time; } }
            public short Gray_Time { get { return gray_time; } }
            public short DMG { get { return dmg; } }
            public short SP { get { return sp; } }
            public short ID { get { return id; } }
            public byte Final_Hit { get { return final_hit; } }
            public byte UU2 { get { return uu2; } }
            public byte UU3 { get { return uu3; } }
            public byte UU4 { get { return uu4; } }
            public byte UU5 { get { return uu5; } }
            public byte UU6 { get { return uu6; } }
            public byte UU7 { get { return uu7; } }
            public byte UU8 { get { return uu8; } }
            public byte UU9 { get { return uu9; } }
            public byte UU10 { get { return uu10; } }
            public short Vertical_Distance { get { return vertical_distance; } }
            public byte UU11 { get { return uu11; } }
            public byte UU12 { get { return uu12; } }
            public byte UU13 { get { return uu13; } }
            public byte UU14 { get { return uu14; } }
            public byte Start_Time { get { return start_time; } }
            public byte UU15 { get { return uu15; } }
            public short ADD_DMG_Multi { get { return add_dmg_multi; } }
            public short ADD_SP_Multi { get { return add_sp_multi; } }

            public AdditionData(string[] values) {
                short skey = 0;
                byte bkey = 0;
                if (Int16.TryParse(values[0], NumberStyles.AllowLeadingSign, null as IFormatProvider, out skey)) {
                    uu1 = skey;
                } else {
                    Constants.WriteDebug(values[0] + " not found as UU1");
                }
                if (Int16.TryParse(values[1], NumberStyles.AllowLeadingSign, null as IFormatProvider, out skey)) {
                    next_hit = skey;
                } else {
                    Constants.WriteDebug(values[1] + " not found as Next Hit");
                }
                if (Int16.TryParse(values[2], NumberStyles.AllowLeadingSign, null as IFormatProvider, out skey)) {
                    blue_time = skey;
                } else {
                    Constants.WriteDebug(values[2] + " not found as Blue Time");
                }
                if (Int16.TryParse(values[3], NumberStyles.AllowLeadingSign, null as IFormatProvider, out skey)) {
                    gray_time = skey;
                } else {
                    Constants.WriteDebug(values[3] + " not found as Gray Time");
                }
                if (Int16.TryParse(values[4], NumberStyles.AllowLeadingSign, null as IFormatProvider, out skey)) {
                    dmg = skey;
                } else {
                    Constants.WriteDebug(values[4] + " not found as DMG");
                }
                if (Int16.TryParse(values[5], NumberStyles.AllowLeadingSign, null as IFormatProvider, out skey)) {
                    sp = skey;
                } else {
                    Constants.WriteDebug(values[5] + " not found as DMG");
                }
                if (Int16.TryParse(values[6], NumberStyles.AllowLeadingSign, null as IFormatProvider, out skey)) {
                    id = skey;
                } else {
                    Constants.WriteDebug(values[6] + " not found as ID");
                }
                if (Byte.TryParse(values[7], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                    final_hit = bkey;
                } else {
                    Constants.WriteDebug(values[7] + " not found as Final Hit");
                }
                if (Byte.TryParse(values[8], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                    uu2 = bkey;
                } else {
                    Constants.WriteDebug(values[8] + " not found as UU2");
                }
                if (Byte.TryParse(values[9], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                    uu3 = bkey;
                } else {
                    Constants.WriteDebug(values[9] + " not found as UU3");
                }
                if (Byte.TryParse(values[10], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                    uu4 = bkey;
                } else {
                    Constants.WriteDebug(values[10] + " not found as UU4");
                }
                if (Byte.TryParse(values[11], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                    uu5 = bkey;
                } else {
                    Constants.WriteDebug(values[11] + " not found as UU5");
                }
                if (Byte.TryParse(values[12], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                    uu6 = bkey;
                } else {
                    Constants.WriteDebug(values[12] + " not found as UU6");
                }
                if (Byte.TryParse(values[13], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                    uu7 = bkey;
                } else {
                    Constants.WriteDebug(values[13] + " not found as UU7");
                }
                if (Byte.TryParse(values[14], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                    uu8 = bkey;
                } else {
                    Constants.WriteDebug(values[14] + " not found as UU8");
                }
                if (Byte.TryParse(values[15], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                    uu9 = bkey;
                } else {
                    Constants.WriteDebug(values[15] + " not found as UU9");
                }
                if (Byte.TryParse(values[16], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                    uu10 = bkey;
                } else {
                    Constants.WriteDebug(values[16] + " not found as UU10");
                }
                if (Int16.TryParse(values[17], NumberStyles.AllowLeadingSign, null as IFormatProvider, out skey)) {
                    vertical_distance = skey;
                } else {
                    Constants.WriteDebug(values[17] + " not found as Vertical Distance");
                }
                if (Byte.TryParse(values[18], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                    uu11 = bkey;
                } else {
                    Constants.WriteDebug(values[18] + " not found as UU11");
                }
                if (Byte.TryParse(values[19], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                    uu12 = bkey;
                } else {
                    Constants.WriteDebug(values[19] + " not found as UU12");
                }
                if (Byte.TryParse(values[20], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                    uu13 = bkey;
                } else {
                    Constants.WriteDebug(values[20] + " not found as UU13");
                }
                if (Byte.TryParse(values[21], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                    uu14 = bkey;
                } else {
                    Constants.WriteDebug(values[21] + " not found as UU14");
                }
                if (Byte.TryParse(values[22], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                    start_time = bkey;
                } else {
                    Constants.WriteDebug(values[22] + " not found as Start Time");
                }
                if (Byte.TryParse(values[23], NumberStyles.AllowLeadingSign, null as IFormatProvider, out bkey)) {
                    uu15 = bkey;
                } else {
                    Constants.WriteDebug(values[23] + " not found as UU15");
                }
                if (Int16.TryParse(values[24], NumberStyles.AllowLeadingSign, null as IFormatProvider, out skey)) {
                    add_dmg_multi = skey;
                } else {
                    Constants.WriteDebug(values[24] + " not found as ADD_DMG_Multi");
                }
                if (Int16.TryParse(values[25], NumberStyles.AllowLeadingSign, null as IFormatProvider, out skey)) {
                    add_sp_multi = skey;
                } else {
                    Constants.WriteDebug(values[25] + " not found as ADD_SP_Multi");
                }
            }
        }

        public static string StringEncode(string text) {
            IDictionary<string, string> codeDict = new Dictionary<string, string>() {
                {"<END>", "FF A0" },
                {"<LINE>", "FF A1" },
                {"<GOLD>", "00 A8" },
                {"<WHITE>", "00 A7" },
                {"<RED>", "05 A7" },
                {"<YELLOW>", "08 A7" }
            };
            IDictionary<char, string> symbolDict = new Dictionary<char, string>() {
                {' ', "00 00" },
                {',', "01 00" },
                {'.', "02 00" },
                {'·', "03 00" },
                {':', "04 00" },
                {'?', "05 00" },
                {'!', "06 00" },
                {'_', "07 00" },
                {'/', "08 00" },
                {'\'', "09 00" },
                {'"', "0A 00" },
                {'(', "0B 00" },
                {')', "0C 00" },
                {'-', "0D 00" },
                {'`', "0E 00" },
                {'%', "0F 00" },
                {'&', "10 00" },
                {'*', "11 00" },
                {'@', "12 00" },
                {'+', "13 00" },
                {'~', "14 00" },
                {'0', "15 00" },
                {'1', "16 00" },
                {'2', "17 00" },
                {'3', "18 00" },
                {'4', "19 00" },
                {'5', "1A 00" },
                {'6', "1B 00" },
                {'7', "1C 00" },
                {'8', "1D 00" },
                {'9', "1E 00" },
                {'A', "1F 00" },
                {'B', "20 00" },
                {'C', "21 00" },
                {'D', "22 00" },
                {'E', "23 00" },
                {'F', "24 00" },
                {'G', "25 00" },
                {'H', "26 00" },
                {'I', "27 00" },
                {'J', "28 00" },
                {'K', "29 00" },
                {'L', "2A 00" },
                {'M', "2B 00" },
                {'N', "2C 00" },
                {'O', "2D 00" },
                {'P', "2E 00" },
                {'Q', "2F 00" },
                {'R', "30 00" },
                {'S', "31 00" },
                {'T', "32 00" },
                {'U', "33 00" },
                {'V', "34 00" },
                {'W', "35 00" },
                {'X', "36 00" },
                {'Y', "37 00" },
                {'Z', "38 00" },
                {'a', "39 00" },
                {'b', "3A 00" },
                {'c', "3B 00" },
                {'d', "3C 00" },
                {'e', "3D 00" },
                {'f', "3E 00" },
                {'g', "3F 00" },
                {'h', "40 00" },
                {'i', "41 00" },
                {'j', "42 00" },
                {'k', "43 00" },
                {'l', "44 00" },
                {'m', "45 00" },
                {'n', "46 00" },
                {'o', "47 00" },
                {'p', "48 00" },
                {'q', "49 00" },
                {'r', "4A 00" },
                {'s', "4B 00" },
                {'t', "4C 00" },
                {'u', "4D 00" },
                {'v', "4E 00" },
                {'w', "4F 00" },
                {'x', "50 00" },
                {'y', "51 00" },
                {'z', "52 00" },
                {'[', "53 00" },
                {']', "54 00" }
            };
            List<string> encoded = new List<string>();
            string[] parts = Regex.Split(text, @"(<[\s\S]+?>)").Where(l => l != string.Empty).ToArray();
            foreach (string segment in parts) {
                if (segment.StartsWith("<")) {
                    encoded.Add(codeDict[segment]);
                } else {
                    List<string> temp = new List<string>();
                    foreach (char letter in segment.ToCharArray()) {
                        temp.Add(symbolDict[letter]);
                    }
                    encoded.Add(String.Join(" ", temp.ToArray()));
                }
            }
            return String.Join(" ", encoded.ToArray()) + " FF A0";
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

                if (inventorySize != 32) {
                    ExtendInventory();
                }

                try {
                    if (dmScripts.ContainsKey("btnSaveAnywhere") && dmScripts["btnSaveAnywhere"])
                        SaveAnywhere();
                    if (dmScripts.ContainsKey("btnIconChanges") && dmScripts["btnIconChanges"])
                        IconChanges();
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
                    if (dmScripts.ContainsKey("btnUltimateBoss") && dmScripts["btnUltimateBoss"])
                        UltimateBossField();
                } catch (Exception ex) {
                    Constants.RUN = false;
                    Constants.WriteGLog("Program stopped.");
                    Constants.WritePLogOutput("INTERNAL FIELD SCRIPT ERROR");
                    Constants.WriteOutput("Fatal Error. Closing all threads.");
                    Constants.WriteDebug(ex.ToString());
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

                try {
                    if (dmScripts.ContainsKey("btnRemoveCaps") && dmScripts["btnRemoveCaps"])
                        RemoveDamageCap();
                    if (dmScripts.ContainsKey("btnSoloMode") && dmScripts["btnSoloMode"])
                        SoloModeBattle();
                    if (dmScripts.ContainsKey("btnDragoonChanges") && dmScripts["btnDragoonChanges"]) {
                        DragoonChanges();
                        if (burnActive) {
                            for (int i = 0; i < 3; i++) {
                                if (Globals.PARTY_SLOT[i] == 0) {
                                    byte action = Globals.CHARACTER_TABLE[i].Read("Action");
                                    if (action == 0 || action == 2) {
                                        Globals.CHARACTER_TABLE[i].Write("AT", originalCharacterStats[i, 1]);
                                        Globals.CHARACTER_TABLE[i].Write("MAT", originalCharacterStats[i, 2]);
                                        burnActive = false;
                                    }
                                }
                            }
                        }
                    }
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
                    if (dmScripts.ContainsKey("btnSoulEater") && dmScripts["btnSoulEater"])
                        NoHPDecaySoulEater();
                    if (dmScripts.ContainsKey("btnHPNames") && dmScripts["btnHPNames"])
                        MonsterHPNames();
                    if (dmScripts.ContainsKey("btnUltimateBoss") && dmScripts["btnUltimateBoss"])
                        UltimateBossBattle();
                    if (dmScripts.ContainsKey("btnDamageTracker") && dmScripts["btnDamageTracker"])
                        DamageTracker();
                    if (dmScripts.ContainsKey("btnEquipChanges") && dmScripts["btnEquipChanges"])
                        EquipChangesBattle();
                } catch (Exception ex) {
                    Constants.RUN = false;
                    Constants.WriteGLog("Program stopped.");
                    Constants.WritePLogOutput("INTERNAL BATTLE SCRIPT ERROR");
                    Constants.WriteOutput("Fatal Error. Closing all threads.");
                    Constants.WriteDebug(ex.ToString());
                }

                if (!keepStats && Globals.IN_BATTLE && Globals.STATS_CHANGED) {
                    for (int i = 0; i < 3; i++) { //Should execute after equip changes
                        if (Globals.PARTY_SLOT[i] < 9) {
                            originalCharacterStats[i, 0] = Globals.CHARACTER_TABLE[i].Read("Max_HP"); //MAX HP
                            originalCharacterStats[i, 1] = Globals.CHARACTER_TABLE[i].Read("AT"); //AT
                            originalCharacterStats[i, 2] = Globals.CHARACTER_TABLE[i].Read("MAT"); //MAT
                            originalCharacterStats[i, 3] = Globals.CHARACTER_TABLE[i].Read("DF"); //DF
                            originalCharacterStats[i, 4] = Globals.CHARACTER_TABLE[i].Read("MDF"); //MDF
                            originalCharacterStats[i, 5] = Globals.CHARACTER_TABLE[i].Read("SPD"); //SPD
                            originalCharacterStats[i, 6] = Globals.CHARACTER_TABLE[i].Read("SP_P_Hit"); //SP HEAL PHYSICAL
                            originalCharacterStats[i, 7] = Globals.CHARACTER_TABLE[i].Read("MP_P_Hit"); //MP HEAL PHYSICAL
                            originalCharacterStats[i, 8] = Globals.CHARACTER_TABLE[i].Read("SP_M_Hit"); //SP HEAL MAGIC
                            originalCharacterStats[i, 9] = Globals.CHARACTER_TABLE[i].Read("MP_M_Hit"); //MP HEAL MAGI
                        }
                    }

                    for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                        originalMonsterStats[i, 0] = Globals.MONSTER_TABLE[i].Read("Max_HP"); //MAX HP
                        originalMonsterStats[i, 1] = Globals.MONSTER_TABLE[i].Read("AT"); //AT
                        originalMonsterStats[i, 2] = Globals.MONSTER_TABLE[i].Read("MAT"); //MAT
                        originalMonsterStats[i, 3] = Globals.MONSTER_TABLE[i].Read("DF"); //DF
                        originalMonsterStats[i, 4] = Globals.MONSTER_TABLE[i].Read("MDF"); //MDF
                        originalMonsterStats[i, 5] = Globals.MONSTER_TABLE[i].Read("SPD"); //SPD
                    }
                    keepStats = true;
                } else {
                    if (!Globals.IN_BATTLE) {
                        keepStats = false;
                    }
                }

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
                
                if (Globals.CURRENT_TIME >= (Globals.LAST_HOTKEY + 2)) {
                    if (!Globals.IN_BATTLE) { //Field
                        if (Globals.HOTKEY == (Hotkey.KEY_SQUARE + Hotkey.KEY_CIRCLE)) { //Change Lohan Shop
                            ChangeShop();
                            Globals.LAST_HOTKEY = Constants.GetTime();
                        }
                    } else { //Battle
                        if (Globals.HOTKEY == (Hotkey.KEY_L1 + Hotkey.KEY_UP)) { //Exit Dragoon Slot 1
                            if (emulator.ReadByte(Constants.GetAddress("DRAGOON_TURNS")) > 0) {
                                emulator.WriteByte(Constants.GetAddress("DRAGOON_TURNS"), 1);
                                Constants.WriteGLogOutput("Slot 1 will exit Dragoon after next action.");
                                Globals.LAST_HOTKEY = Constants.GetTime();
                            }
                        }
                        if (Globals.HOTKEY == (Hotkey.KEY_L1 + Hotkey.KEY_RIGHT)) { //Exit Dragoon Slot 2
                            if (emulator.ReadByte(Constants.GetAddress("DRAGOON_TURNS") + 0x4) > 0) {
                                emulator.WriteByte(Constants.GetAddress("DRAGOON_TURNS") + 0x4, 1);
                                Constants.WriteGLogOutput("Slot 2 will exit Dragoon after next action.");
                                Globals.LAST_HOTKEY = Constants.GetTime();
                            }
                        }
                        if (Globals.HOTKEY == (Hotkey.KEY_L1 + Hotkey.KEY_LEFT)) { //Exit Dragoon Slot 3
                            if (emulator.ReadByte(Constants.GetAddress("DRAGOON_TURNS") + 0x8) > 0) {
                                emulator.WriteByte(Constants.GetAddress("DRAGOON_TURNS") + 0x8, 1);
                                Constants.WriteGLogOutput("Slot 3 will exit Dragoon after next action.");
                                Globals.LAST_HOTKEY = Constants.GetTime();
                            }
                        }
                        if (Globals.HOTKEY == (Hotkey.KEY_CIRCLE + Hotkey.KEY_LEFT)) { //Burn Stack
                            if (!burnActive) {
                                for (int i = 0; i < 3; i++) {
                                    if (Globals.PARTY_SLOT[i] == 0) {
                                        byte action = Globals.CHARACTER_TABLE[i].Read("Action");
                                        if (action == 8 || action == 10) {
                                            Globals.CHARACTER_TABLE[i].Write("AT", Math.Round(originalCharacterStats[i, 1] * (1 + (dartBurnStack * 0.2))));
                                            Globals.CHARACTER_TABLE[i].Write("MAT", Math.Round(originalCharacterStats[i, 2] * (1 + (dartBurnStack * 0.2))));
                                            burnActive = true;
                                            Constants.WriteGLogOutput("Burn stack activated.");
                                        }
                                    }
                                }
                            } else {
                                Constants.WriteGLogOutput("Burn stack is already active.");
                            }
                            Globals.LAST_HOTKEY = Constants.GetTime();
                        }
                        if (Globals.HOTKEY == (Hotkey.KEY_CIRCLE + Hotkey.KEY_RIGHT)) { //Dragon Buster II
                            bool skip = true;
                            for (int i = 0; i < 3; i++) {
                                if (Globals.PARTY_SLOT[i] == 3 && Globals.CHARACTER_TABLE[i].Read("Weapon") == 162) {
                                    skip = false;
                                }
                            }
                            if (!skip) {
                                if (dmScripts.ContainsKey("btnDragoonChanges") && dmScripts["btnDragoonChanges"] && !checkRoseDamage) {
                                    if (roseEnhanceDragoon) {
                                        if (Constants.REGION == Region.USA) {
                                            emulator.WriteAOB(0x51ADC, "22 00 39 00 4A 00 43 00 00 00 31 00 32 00 30 00 00 00 1A 00 1E 00 15 00 0F 00 00 00 10 00 00 00 26 00 2E 00 00 00 30 00 3D 00 3B 00 4E 00 FF A0");
                                            emulator.WriteAOB(0x51B14, "22 00 39 00 4A 00 43 00 00 00 31 00 32 00 30 00 00 00 18 00 1E 00 1A 00 0F 00 00 00 10 00 00 00 24 00 3D 00 39 00 4A 00 FF A0");
                                            emulator.WriteAOB(0x51BA8, "22 00 39 00 4A 00 43 00 00 00 31 00 32 00 30 00 00 00 16 00 1B 00 1D 00 15 00 0F 00 00 00 10 00 00 00 26 00 2E 00 00 00 FF A0");
                                        }
                                        emulator.WriteByte(Constants.GetAddress("SPELL_TABLE") + 0x8 + (15 * 0xC), 10); //Astral Drain MP
                                        emulator.WriteByte(Constants.GetAddress("SPELL_TABLE") + 0x8 + (16 * 0xC), 20); //Death Dimension MP
                                        emulator.WriteByte(Constants.GetAddress("SPELL_TABLE") + 0x8 + (19 * 0xC), 80); //Dark Dragon MP
                                        roseEnhanceDragoon = false;
                                        Constants.WriteGLogOutput("Rose's dragoon magic has returned to normal.");
                                    } else {
                                        if (Constants.REGION == Region.USA) {
                                            emulator.WriteAOB(0x51ADC, "22 00 39 00 4A 00 43 00 00 00 31 00 32 00 30 00 00 00 1D 00 17 00 1A 00 0F 00 00 00 10 00 00 00 26 00 2E 00 00 00 30 00 3D 00 3B 00 4E 00 FF A0");
                                            emulator.WriteAOB(0x51B14, "22 00 39 00 4A 00 43 00 00 00 31 00 32 00 30 00 00 00 1C 00 1E 00 15 00 0F 00 00 00 10 00 00 00 24 00 3D 00 39 00 4A 00 FF A0");
                                            emulator.WriteAOB(0x51BA8, "22 00 39 00 4A 00 43 00 00 00 31 00 32 00 30 00 00 00 16 00 16 00 1A 00 15 00 0F 00 00 00 10 00 00 00 26 00 2E 00 FF A0");
                                        }
                                        emulator.WriteByte(Constants.GetAddress("SPELL_TABLE") + 0x8 + (15 * 0xC), 20); //Astral Drain MP
                                        emulator.WriteByte(Constants.GetAddress("SPELL_TABLE") + 0x8 + (16 * 0xC), 50); //Death Dimension MP
                                        emulator.WriteByte(Constants.GetAddress("SPELL_TABLE") + 0x8 + (19 * 0xC), 100); //Dark Dragon MP
                                        roseEnhanceDragoon = true;
                                        Constants.WriteGLogOutput("Rose will now consume more MP for bonus effects.");
                                    }
                                } else {
                                    Constants.WriteGLogOutput("You can't swap MP modes right now.");
                                }
                            } else {
                                Constants.WriteGLogOutput("Dragon Buster II not equipped.");
                            }
                            Globals.LAST_HOTKEY = Constants.GetTime();
                        }
                        if (Globals.HOTKEY == (Hotkey.KEY_CIRCLE + Hotkey.KEY_DOWN)) { //Jeweled Hammer
                            bool skip = true;
                            for (int i = 0; i < 3; i++) {
                                if (Globals.PARTY_SLOT[i] == 6 && Globals.CHARACTER_TABLE[i].Read("Weapon") == 164) {
                                    skip = false;
                                }
                            }
                            if (!skip) {
                                if (dmScripts.ContainsKey("btnDragoonChanges") && dmScripts["btnDragoonChanges"] && !checkRoseDamage) {
                                    if (jeweledHammer) {
                                        if (Constants.REGION == Region.USA) {
                                            emulator.WriteAOB(0x51CA8, "35 00 39 00 4C 00 3D 00 4A 00 00 00 31 00 32 00 30 00 00 00 1A 00 16 00 15 00 0F 00 FF A0");
                                            emulator.WriteAOB(0x51D3C, "35 00 39 00 4C 00 3D 00 4A 00 00 00 31 00 32 00 30 00 00 00 1A 00 18 00 15 00 0F 00 FF A0");
                                            emulator.WriteAOB(0x51D64, "35 00 39 00 4C 00 3D 00 4A 00 00 00 31 00 32 00 30 00 00 00 16 00 19 00 15 00 15 00 0F 00 FF A0");
                                        }
                                        emulator.WriteByte(Constants.GetAddress("SPELL_TABLE") + 0x8 + (24 * 0xC), 10); //Freezing Ring MP
                                        emulator.WriteByte(Constants.GetAddress("SPELL_TABLE") + 0x8 + (25 * 0xC), 20); //Rainbow Breath MP
                                        emulator.WriteByte(Constants.GetAddress("SPELL_TABLE") + 0x8 + (27 * 0xC), 80); //Diamond Dust MP
                                        emulator.WriteByte(Constants.GetAddress("SPELL_TABLE") + 0x8 + (28 * 0xC), 80); //Blue Sea Dragon MP
                                        jeweledHammer = false;
                                        Constants.WriteGLogOutput("Meru's dragoon magic has returned to normal.");
                                    } else {
                                        if (Constants.REGION == Region.USA) {
                                            emulator.WriteAOB(0x51CA8, "35 00 39 00 4C 00 3D 00 4A 00 00 00 31 00 32 00 30 00 00 00 1D 00 15 00 15 00 0F 00 FF A0");
                                            emulator.WriteAOB(0x51D3C, "35 00 39 00 4C 00 3D 00 4A 00 00 00 31 00 32 00 30 00 00 00 1D 00 1D 00 15 00 0F 00 FF A0");
                                            emulator.WriteAOB(0x51D64, "35 00 39 00 4C 00 3D 00 4A 00 00 00 31 00 32 00 30 00 00 00 17 00 16 00 15 00 15 00 0F 00 FF A0");
                                        }
                                        emulator.WriteByte(Constants.GetAddress("SPELL_TABLE") + 0x8 + (24 * 0xC), 50); //Freezing Ring MP
                                        emulator.WriteByte(Constants.GetAddress("SPELL_TABLE") + 0x8 + (25 * 0xC), 100); //Rainbow Breath MP
                                        emulator.WriteByte(Constants.GetAddress("SPELL_TABLE") + 0x8 + (27 * 0xC), 100); //Diamond Dust MP
                                        emulator.WriteByte(Constants.GetAddress("SPELL_TABLE") + 0x8 + (28 * 0xC), 150); //Blue Sea Dragon MP
                                        jeweledHammer = true;
                                        Constants.WriteGLogOutput("Meru will now consume more MP for bonus effects.");
                                    }
                                } else {
                                    Constants.WriteGLogOutput("You can't swap MP modes right now.");
                                }
                            } else {
                                Constants.WriteGLogOutput("Jeweled Hammer not equipped.");
                            }
                            Globals.LAST_HOTKEY = Constants.GetTime();
                        }
                        if (Globals.HOTKEY == (Hotkey.KEY_L2 + Hotkey.KEY_LEFT)) { //Soa's Wargod
                            if ((134217728 & ultimateShopLimited) != 0) {
                                ubSoasWargod = ubSoasWargod ? false : true;
                                Constants.WriteGLogOutput("Soa's Wargod has been " + (ubSoasWargod ? "activated" : "deactivated") + ".");
                            } else {
                                Constants.WriteGLogOutput("You do not have Soa's Wargod.");
                            }
                            Globals.LAST_HOTKEY = Constants.GetTime();
                        }
                        if (Globals.HOTKEY == (Hotkey.KEY_L2 + Hotkey.KEY_RIGHT)) { //Soa's Dragoon Boost
                            if ((268435456 & ultimateShopLimited) != 0) {
                                ubSoasDragoonBoost = ubSoasDragoonBoost ? false : true;
                                Constants.WriteGLogOutput("Soa's Dragoon Boost has been " + (ubSoasDragoonBoost ? "activated" : "deactivated") + ".");
                            } else {
                                Constants.WriteGLogOutput("You do not have Soa's Dragoon Boost.");
                            }
                            Globals.LAST_HOTKEY = Constants.GetTime();
                        }
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
                    if (ReadShop(0x11E0FA) == 30) {
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
            if (!Globals.IN_BATTLE && menu == 4 && !wroteIcons) {
                WriteIcons();
                wroteIcons = true;
            } else {
                if (menu == 125) {
                    wroteIcons = false;
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
                for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                    if (!Constants.BATTLE_UI)
                        monsterDisplay[i, 0].Text = emulator.ReadName(0xC69D0 + (0x2C * i));

                    if (dmScripts.ContainsKey("btnUltimateBoss") && dmScripts["btnUltimateBoss"]) {
                        if (ultimateMaxHP[i] > 65535) {
                            monsterDisplay[i, 1].Text = " " + ultimateHP[i] + "/" + ultimateMaxHP[i];
                        } else {
                            monsterDisplay[i, 1].Text = " " + Convert.ToString(Globals.MONSTER_TABLE[i].Read("HP"), 10) + "/" + Globals.MONSTER_TABLE[i].Read("Max_HP");
                        }
                    } else {
                        monsterDisplay[i, 1].Text = " " + Convert.ToString(Globals.MONSTER_TABLE[i].Read("HP"), 10) + "/" + Globals.MONSTER_TABLE[i].Read("Max_HP");
                    }

                    monsterDisplay[i, 2].Text = " " + Globals.MONSTER_TABLE[i].Read("AT") + "/" + Globals.MONSTER_TABLE[i].Read("MAT");
                    monsterDisplay[i, 3].Text = " " + Globals.MONSTER_TABLE[i].Read("DF") + "/" + Globals.MONSTER_TABLE[i].Read("MDF");
                    monsterDisplay[i, 4].Text = " " + Globals.MONSTER_TABLE[i].Read("SPD");
                    monsterDisplay[i, 5].Text = " " + Globals.MONSTER_TABLE[i].Read("Turn");
                }
                for (int i = 0; i < 3; i++) {
                    if (Globals.PARTY_SLOT[i] < 9) {
                        characterDisplay[i, 0].Text = Constants.GetCharName(Globals.PARTY_SLOT[i]);
                        characterDisplay[i, 1].Text = " " + Globals.CHARACTER_TABLE[i].Read("HP") + "/" + Globals.CHARACTER_TABLE[i].Read("Max_HP") + "\r\n\r\n " + Globals.CHARACTER_TABLE[i].Read("MP") + "/" + Globals.CHARACTER_TABLE[i].Read("Max_MP");
                        characterDisplay[i, 2].Text = " " + Globals.CHARACTER_TABLE[i].Read("AT") + "\r\n\r\n " + Globals.CHARACTER_TABLE[i].Read("MAT");
                        characterDisplay[i, 3].Text = " " + Globals.CHARACTER_TABLE[i].Read("DF") + "\r\n\r\n " + Globals.CHARACTER_TABLE[i].Read("MDF");
                        characterDisplay[i, 4].Text = " " + Globals.CHARACTER_TABLE[i].Read("A_HIT") + "/" + Globals.CHARACTER_TABLE[i].Read("M_HIT") + "\r\n\r\n " + Globals.CHARACTER_TABLE[i].Read("A_AV") + "/" + Globals.CHARACTER_TABLE[i].Read("M_AV");
                        characterDisplay[i, 5].Text = " " + Globals.CHARACTER_TABLE[i].Read("DAT") + "\r\n\r\n " + Globals.CHARACTER_TABLE[i].Read("DMAT");
                        characterDisplay[i, 6].Text = " " + Globals.CHARACTER_TABLE[i].Read("DDF") + "\r\n\r\n " + Globals.CHARACTER_TABLE[i].Read("DMDF");
                        characterDisplay[i, 7].Text = " " + Globals.CHARACTER_TABLE[i].Read("SPD") + "\r\n\r\n " + Globals.CHARACTER_TABLE[i].Read("SP");
                        characterDisplay[i, 8].Text = " " + Globals.CHARACTER_TABLE[i].Read("Turn");
                    }
                }

                Constants.BATTLE_UI = true;
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
            if (Globals.IN_BATTLE && Globals.STATS_CHANGED && !hpCapBreakOnBattleEntry && maxHPTableLoaded) {
                for (int i = 0; i < 3; i++) {
                    if (Globals.PARTY_SLOT[i] < 9) {
                        if (hpChangeCheck[i] != 65535) {
                            Globals.CHARACTER_TABLE[i].Write("Max_HP", hpChangeCheck[i]);
                            if (hpChangeSave[i] != 65535 && Globals.CHARACTER_TABLE[i].Read("HP") < hpChangeSave[i]) {
                                Globals.CHARACTER_TABLE[i].Write("HP", hpChangeSave[i]);
                            }
                        }
                    }
                }
                hpCapBreakOnBattleEntry = true;
            } else {
                if (!Globals.IN_BATTLE && hpCapBreakOnBattleEntry) {
                    hpCapBreakOnBattleEntry = false;
                } else {
                    if (emulator.ReadShort(Constants.GetAddress("BATTLE_VALUE")) == 41215 && Globals.STATS_CHANGED && hpCapBreakOnBattleEntry && maxHPTableLoaded) {
                        for (int i = 0; i < 3; i++) {
                            hpChangeSave[i] = Globals.CHARACTER_TABLE[i].Read("HP");
                            //Constants.WriteDebug("xBreak HP: " + hpChangeSave[0] + "/" + hpChangeCheck[0] + " | " + hpChangeSave[1] + "/" + hpChangeCheck[1] + " | " + hpChangeSave[2] + "/" + hpChangeCheck[2]);
                        }
                    }
                }
            }
        }

        public void LoadMaxHPTable() {
            if (!maxHPTableLoaded && !Globals.IN_BATTLE && emulator.ReadShort(Constants.GetAddress("BATTLE_VALUE")) < 5130) {
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
                            Globals.CHARACTER_TABLE[i].Write("HP", 0);
                            Globals.CHARACTER_TABLE[i].Write("Max_HP", 0);
                            Globals.CHARACTER_TABLE[i].Write("HP_Regen", 200);
                            Globals.CHARACTER_TABLE[i].Write("Turn", 10000);
                            //yeet
                            Globals.CHARACTER_TABLE[i].Write("POS_X", 255);
                            Globals.CHARACTER_TABLE[i].Write("POS_Y", 255);
                            Globals.CHARACTER_TABLE[i].Write("POS_Z", 255);
                        } else {
                            Globals.CHARACTER_TABLE[i].Write("POS_X", 9);
                            Globals.CHARACTER_TABLE[i].Write("POS_Y", 0);
                            Globals.CHARACTER_TABLE[i].Write("POS_Z", 0);
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
                            if (Globals.CHARACTER_TABLE[i].Read("Weapon") == 28 && emulator.ReadByte(Constants.GetAddress("CHAPTER")) >= 3) { //Sparkle Arrow
                                Globals.CHARACTER_TABLE[i].Write("AT", (Globals.CHARACTER_TABLE[i].Read("AT") + 8));
                            }

                            bool boost = false;
                            double boostAmount = 0.0;
                            byte level = 0;
                            if (Globals.PARTY_SLOT[i] == 2) {
                                if (emulator.ReadByte(Constants.GetAddress("SHANA_LEVEL")) >= 10) {
                                    boost = true;
                                    level = emulator.ReadByte(Constants.GetAddress("SHANA_LEVEL"));
                                }
                            } else {
                                if (emulator.ReadByte(Constants.GetAddress("MIRANDA_LEVEL")) >= 10) {
                                    boost = true;
                                    level = emulator.ReadByte(Constants.GetAddress("MIRANDA_LEVEL"));
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
                                if (Globals.CHARACTER_TABLE[i].Read("Weapon") == 32) {
                                    Globals.CHARACTER_TABLE[i].Write("AT", Math.Round(Globals.CHARACTER_TABLE[i].Read("AT") * 1.4));
                                } else {
                                    Globals.CHARACTER_TABLE[i].Write("AT", Math.Round(Globals.CHARACTER_TABLE[i].Read("AT") * boostAmount));
                                }
                            }
                        }

                        if (Globals.PARTY_SLOT[i] == 2 && Constants.GetAddress("SHANA_LEVEL") >= 30) {
                            Globals.CHARACTER_TABLE[i].Write("DF", Math.Round(Globals.CHARACTER_TABLE[i].Read("DF") * 1.12));
                        }

                        if (Globals.PARTY_SLOT[i] == 8 && Constants.GetAddress("MIRANDA_LEVEL") >= 30) {
                            Globals.CHARACTER_TABLE[i].Write("DF", Math.Round(Globals.CHARACTER_TABLE[i].Read("DF") * 1.12));
                        }

                        if (Globals.PARTY_SLOT[i] == 3 && Constants.GetAddress("ROSE_LEVEL") >= 30) {
                            Globals.CHARACTER_TABLE[i].Write("DF", Math.Round(Globals.CHARACTER_TABLE[i].Read("DF") * 1.1));
                        }

                        if (Globals.PARTY_SLOT[i] == 6 && Constants.GetAddress("MERU_LEVEL") >= 30) {
                            Globals.CHARACTER_TABLE[i].Write("DF", Math.Round(Globals.CHARACTER_TABLE[i].Read("DF") * 1.26));
                        }

                        if (Globals.CHARACTER_TABLE[i].Read("Accessory") == 149) { //Phantom Shield
                            s = Globals.CHARACTER_TABLE[i].Read("DF");
                            emulator.WriteByte(p + 0x30, 0);
                            emulator.WriteByte(p + 0x31, 0);
                            if (Globals.CHARACTER_TABLE[i].Read("Armor") == 74) {
                                Globals.CHARACTER_TABLE[i].Write("DF", (Math.Ceiling(s * 1.1)));
                            } else {
                                Globals.CHARACTER_TABLE[i].Write("DF", (Math.Ceiling(s * 0.7)));
                            }
                            s = Globals.CHARACTER_TABLE[i].Read("MDF");
                            emulator.WriteByte(p + 0x32, 0);
                            emulator.WriteByte(p + 0x33, 0);
                            if (Globals.CHARACTER_TABLE[i].Read("Helmet") == 89) {
                                Globals.CHARACTER_TABLE[i].Write("MDF", (Math.Ceiling(s * 1.1)));
                            } else {
                                Globals.CHARACTER_TABLE[i].Write("MDF", (Math.Ceiling(s * 0.7)));
                            }
                        }

                        if (Globals.CHARACTER_TABLE[i].Read("Weapon") == 9) { //Tomahawk
                            Globals.CHARACTER_TABLE[i].Write("Element", 2);
                        }

                        if (Globals.CHARACTER_TABLE[i].Read("Helmet") == 77) { //Sallet
                            s = Globals.CHARACTER_TABLE[i].Read("M_HIT");
                            Globals.CHARACTER_TABLE[i].Write("M_HIT", (s + 10));
                        }

                        if (Globals.CHARACTER_TABLE[i].Read("Helmet") == 84) { //Tiara
                            s = Globals.CHARACTER_TABLE[i].Read("A_HIT");
                            Globals.CHARACTER_TABLE[i].Write("A_HIT", (s + 10));
                        }

                        if (Globals.CHARACTER_TABLE[i].Read("Accessory") == 150) { //Dragon Shield
                            s = Globals.CHARACTER_TABLE[i].Read("DF");
                            emulator.WriteByte(p + 0x30, 0);
                            emulator.WriteByte(p + 0x31, 0);
                            if (Globals.CHARACTER_TABLE[i].Read("Armor") == 74) {
                                Globals.CHARACTER_TABLE[i].Write("DF", (Math.Ceiling(s * 1.2)));
                            } else {
                                Globals.CHARACTER_TABLE[i].Write("DF", (Math.Ceiling(s * 0.7)));
                            }
                        }

                        if (Globals.CHARACTER_TABLE[i].Read("Accessory") == 151) { //Angel Scarf
                            s = Globals.CHARACTER_TABLE[i].Read("MDF");
                            emulator.WriteByte(p + 0x32, 0);
                            emulator.WriteByte(p + 0x33, 0);
                            if (Globals.CHARACTER_TABLE[i].Read("Helmet") == 89) {
                                Globals.CHARACTER_TABLE[i].Write("MDF", (Math.Ceiling(s * 1.2)));
                            } else {
                                Globals.CHARACTER_TABLE[i].Write("MDF", (Math.Ceiling(s * 0.7)));
                            }
                        }

                        if (emulator.ReadByte(Constants.GetAddress("CHAPTER")) >= 3) {
                            if (Globals.CHARACTER_TABLE[i].Read("Weapon") == 2) { //Heat Blade
                                Globals.CHARACTER_TABLE[i].Write("AT", (Globals.CHARACTER_TABLE[i].Read("AT") + 7));
                            }

                            if (Globals.CHARACTER_TABLE[i].Read("Weapon") == 14) { //Shadow Cutter
                                Globals.CHARACTER_TABLE[i].Write("AT", (Globals.CHARACTER_TABLE[i].Read("AT") + 9));
                            }

                            if (Globals.CHARACTER_TABLE[i].Read("Weapon") == 35) { //Morning Star
                                Globals.CHARACTER_TABLE[i].Write("AT", (Globals.CHARACTER_TABLE[i].Read("AT") - 20));
                                Globals.CHARACTER_TABLE[i].Write("Element", 1);
                            }
                        }

                        Globals.CHARACTER_TABLE[i].Write("E_Immune", 0); //Elemental Null

                        if (Globals.CHARACTER_TABLE[i].Read("Weapon") == 158 && Globals.PARTY_SLOT[i] == 3) { //Sabre
                            Globals.CHARACTER_TABLE[i].Write("AT", (Globals.CHARACTER_TABLE[i].Read("AT") + 70));
                        }

                        if (Globals.CHARACTER_TABLE[i].Read("Weapon") == 159 && Globals.PARTY_SLOT[i] == 0) { //Spirit Eater
                            spiritEaterSP = 35;
                            if (dmScripts.ContainsKey("btnHalfSP") && dmScripts["btnHalfSP"]) {
                                spiritEaterSP = 15;
                            }
                            spiritEaterSaveSP = Globals.CHARACTER_TABLE[i].Read("SP_Regen");
                            Globals.CHARACTER_TABLE[i].Write("AT", (Globals.CHARACTER_TABLE[i].Read("AT") + 75));
                            Globals.CHARACTER_TABLE[i].Write("MAT", (Globals.CHARACTER_TABLE[i].Read("MAT") + 50));
                            if (spiritEaterSaveSP < spiritEaterSP) {
                                Globals.CHARACTER_TABLE[i].Write("SP_Regen", (65536 - (spiritEaterSP - spiritEaterSaveSP)));
                            } else {
                                Globals.CHARACTER_TABLE[i].Write("SP_Regen", (spiritEaterSaveSP - spiritEaterSP));
                            }
                        }

                        if (Globals.CHARACTER_TABLE[i].Read("Weapon") == 160 && (Globals.PARTY_SLOT[i] == 1 || Globals.PARTY_SLOT[i] == 5)) { //Harpoon
                            Globals.CHARACTER_TABLE[i].Write("AT", (Globals.CHARACTER_TABLE[i].Read("AT") + 100));
                        }

                        if (Globals.CHARACTER_TABLE[i].Read("Weapon") == 161 && (Globals.PARTY_SLOT[i] == 2 || Globals.PARTY_SLOT[i] == 8)) { //Element Arrow
                            Globals.CHARACTER_TABLE[i].Write("Element", elementArrowElement);
                            Globals.CHARACTER_TABLE[i].Write("AT", (Globals.CHARACTER_TABLE[i].Read("AT") + 50));
                            Globals.CHARACTER_TABLE[i].Write("MAT", (Globals.CHARACTER_TABLE[i].Read("MAT") + 50));
                            elementArrowLastAction = 255;
                            elementArrowTurns = 0;
                        }

                        if (Globals.CHARACTER_TABLE[i].Read("Weapon") == 162 && Globals.PARTY_SLOT[i] == 3) { //Dragon Buster II
                            Globals.CHARACTER_TABLE[i].Write("AT", (Globals.CHARACTER_TABLE[i].Read("AT") + 130));
                        }

                        if (Globals.CHARACTER_TABLE[i].Read("Weapon") == 163 && Globals.PARTY_SLOT[i] == 4) { //Battery Glove
                            Globals.CHARACTER_TABLE[i].Write("AT", (Globals.CHARACTER_TABLE[i].Read("AT") + 80));
                            Globals.CHARACTER_TABLE[i].Write("MAT", (Globals.CHARACTER_TABLE[i].Read("MAT") + 20));
                            gloveLastAction = 0;
                            gloveCharge = 0;
                        }

                        if (Globals.CHARACTER_TABLE[i].Read("Weapon") == 164 && Globals.PARTY_SLOT[i] == 6) { //Jeweled Hammer
                            Globals.CHARACTER_TABLE[i].Write("AT", (Globals.CHARACTER_TABLE[i].Read("AT") + 40));
                            Globals.CHARACTER_TABLE[i].Write("MAT", (Globals.CHARACTER_TABLE[i].Read("MAT") + 40));
                            if (jeweledHammer) {
                                if (Constants.REGION == Region.USA) {
                                    emulator.WriteAOB(0x51CA8, "35 00 39 00 4C 00 3D 00 4A 00 00 00 31 00 32 00 30 00 00 00 1D 00 15 00 15 00 0F 00 FF A0");
                                    emulator.WriteAOB(0x51D3C, "35 00 39 00 4C 00 3D 00 4A 00 00 00 31 00 32 00 30 00 00 00 1D 00 1D 00 15 00 0F 00 FF A0");
                                    emulator.WriteAOB(0x51D64, "35 00 39 00 4C 00 3D 00 4A 00 00 00 31 00 32 00 30 00 00 00 17 00 16 00 15 00 15 00 0F 00 FF A0");
                                }
                                emulator.WriteByte(Constants.GetAddress("SPELL_TABLE") + 0x8 + (24 * 0xC), 50); //Freezing Ring MP
                                emulator.WriteByte(Constants.GetAddress("SPELL_TABLE") + 0x8 + (25 * 0xC), 100); //Rainbow Breath MP
                                emulator.WriteByte(Constants.GetAddress("SPELL_TABLE") + 0x8 + (27 * 0xC), 100); //Diamond Dust MP
                                emulator.WriteByte(Constants.GetAddress("SPELL_TABLE") + 0x8 + (28 * 0xC), 150); //Blue Sea Dragon
                            }
                        }

                        if (Globals.CHARACTER_TABLE[i].Read("Weapon") == 165 && Globals.PARTY_SLOT[i] == 7) { //Giant Axe
                            Globals.CHARACTER_TABLE[i].Write("AT", (Globals.CHARACTER_TABLE[i].Read("AT") + 100));
                            Globals.CHARACTER_TABLE[i].Write("MAT", (Globals.CHARACTER_TABLE[i].Read("MAT") + 10));
                            axeLastAction = 0;
                        }

                        if (Globals.CHARACTER_TABLE[i].Read("Weapon") == 166) { //Soa's Light
                            Globals.CHARACTER_TABLE[i].Write("AT", (Globals.CHARACTER_TABLE[i].Read("AT") + 200));
                            Globals.CHARACTER_TABLE[i].Write("MAT", (Globals.CHARACTER_TABLE[i].Read("MAT") + 140));
                            Globals.CHARACTER_TABLE[i].Write("SP_Multi", 65436);
                            Globals.CHARACTER_TABLE[i].Write("SP_Regen", 100);
                            for (int x = 0; x < 3; x++) {
                                if (x != i && Globals.PARTY_SLOT[x] < 9) {
                                    Globals.CHARACTER_TABLE[x].Write("DF", Math.Round(Globals.CHARACTER_TABLE[i].Read("DF") * 0.7));
                                    Globals.CHARACTER_TABLE[x].Write("MDF", Math.Round(Globals.CHARACTER_TABLE[i].Read("MDF") * 0.7));
                                }
                            }
                        }

                        if (Globals.CHARACTER_TABLE[i].Read("Helmet") == 167) { //Fake Legend Casque
                            Globals.CHARACTER_TABLE[i].Write("MDF", (Globals.CHARACTER_TABLE[i].Read("MDF") + 30));
                            Globals.CHARACTER_TABLE[i].Write("A_HIT", (Globals.CHARACTER_TABLE[i].Read("A_HIT") + 100));
                            Globals.CHARACTER_TABLE[i].Write("M_HIT", (Globals.CHARACTER_TABLE[i].Read("M_HIT") + 100));
                        }

                        if (Globals.CHARACTER_TABLE[i].Read("Helmet") == 168) { //Soa's Helm
                            Globals.CHARACTER_TABLE[i].Write("MDF", (Globals.CHARACTER_TABLE[i].Read("MDF") + 200));
                            Globals.CHARACTER_TABLE[i].Write("MP_Regen", (Globals.CHARACTER_TABLE[i].Read("MP_Regen") + 20));
                            Globals.CHARACTER_TABLE[i].Write("A_HIT", (Globals.CHARACTER_TABLE[i].Read("A_HIT") + 100));
                            Globals.CHARACTER_TABLE[i].Write("M_HIT", (Globals.CHARACTER_TABLE[i].Read("M_HIT") + 100));

                            for (int x = 0; x < 3; x++) {
                                if (x != i && Globals.PARTY_SLOT[x] < 9) {
                                    Globals.CHARACTER_TABLE[x].Write("AT", Math.Round(Globals.CHARACTER_TABLE[x].Read("AT") * 0.7));
                                }
                            }
                        }

                        if (Globals.CHARACTER_TABLE[i].Read("Armor") == 169) { //Fake Legend Armor
                            Globals.CHARACTER_TABLE[i].Write("DF", (Globals.CHARACTER_TABLE[i].Read("DF") + 30));
                        }

                        if (Globals.CHARACTER_TABLE[i].Read("Armor") == 170) { //Divine DG Armor
                            Globals.CHARACTER_TABLE[i].Write("DF", (Globals.CHARACTER_TABLE[i].Read("DF") + 50));
                            Globals.CHARACTER_TABLE[i].Write("MDF", (Globals.CHARACTER_TABLE[i].Read("MDF") + 50));
                            Globals.CHARACTER_TABLE[i].Write("SP_P_Hit", (Globals.CHARACTER_TABLE[i].Read("SP_P_Hit") + 20));
                            Globals.CHARACTER_TABLE[i].Write("MP_P_Hit", (Globals.CHARACTER_TABLE[i].Read("MP_P_Hit") + 10));
                            Globals.CHARACTER_TABLE[i].Write("SP_M_Hit", (Globals.CHARACTER_TABLE[i].Read("SP_M_Hit") + 20));
                            Globals.CHARACTER_TABLE[i].Write("MP_M_Hit", (Globals.CHARACTER_TABLE[i].Read("MP_M_Hit") + 10));
                        }

                        if (Globals.CHARACTER_TABLE[i].Read("Armor") == 171) { //Soa's Armor
                            Globals.CHARACTER_TABLE[i].Write("DF", (Globals.CHARACTER_TABLE[i].Read("DF") + 200));
                            Globals.CHARACTER_TABLE[i].Write("HP_Regen", (Globals.CHARACTER_TABLE[i].Read("HP_Regen") + 20));
                            for (int x = 0; x < 3; x++) {
                                if (x != i && Globals.PARTY_SLOT[x] < 9) {
                                    Globals.CHARACTER_TABLE[x].Write("MATK", Math.Round(Globals.CHARACTER_TABLE[x].Read("MATK") * 0.7));
                                }
                            }
                        }

                        if (Globals.CHARACTER_TABLE[i].Read("Shoes") == 172) { //Lloyd's Boots
                            Globals.CHARACTER_TABLE[i].Write("SPD", (Globals.CHARACTER_TABLE[i].Read("SPD") + 15));
                            Globals.CHARACTER_TABLE[i].Write("A_AV", (Globals.CHARACTER_TABLE[i].Read("A_AV") + 15));
                            Globals.CHARACTER_TABLE[i].Write("M_AV", (Globals.CHARACTER_TABLE[i].Read("M_AV") + 15));
                        }

                        if (Globals.CHARACTER_TABLE[i].Read("Shoes") == 173) { //Winged Shoes
                            Globals.CHARACTER_TABLE[i].Write("SPD", (Globals.CHARACTER_TABLE[i].Read("SPD") + 25));
                        }

                        if (Globals.CHARACTER_TABLE[i].Read("Shoes") == 174) { //Soa's Greaves
                            Globals.CHARACTER_TABLE[i].Write("SPD", (Globals.CHARACTER_TABLE[i].Read("SPD") + 40));
                            Globals.CHARACTER_TABLE[i].Write("A_AV", (Globals.CHARACTER_TABLE[i].Read("A_AV") + 40));
                            Globals.CHARACTER_TABLE[i].Write("M_AV", (Globals.CHARACTER_TABLE[i].Read("M_AV") + 40));
                            for (int x = 0; x < 3; x++) {
                                if (x != i && Globals.PARTY_SLOT[x] < 9) {
                                    Globals.CHARACTER_TABLE[x].Write("SPD", (Globals.CHARACTER_TABLE[x].Read("SPD") - 25));
                                }
                            }
                        }

                        if (Globals.CHARACTER_TABLE[i].Read("Accessory") == 175) { //Heal Ring
                            Globals.CHARACTER_TABLE[i].Write("HP_Regen", (Globals.CHARACTER_TABLE[i].Read("HP_Regen") + 7));
                            Globals.CHARACTER_TABLE[i].Write("MP_Regen", (Globals.CHARACTER_TABLE[i].Read("MP_Regen") + 7));
                            Globals.CHARACTER_TABLE[i].Write("SP_Regen", (Globals.CHARACTER_TABLE[i].Read("SP_Regen") + 7));
                            if (Globals.PARTY_SLOT[i] == 2 || Globals.PARTY_SLOT[i] == 8) {
                                recoveryRateSave = Globals.CHARACTER_TABLE[i].Read("HP_Regen");
                            }
                        }

                        if (Globals.CHARACTER_TABLE[i].Read("Accessory") == 176) { //Soa's Sash
                            Globals.CHARACTER_TABLE[i].Write("SP_Multi", (ushort) (Globals.CHARACTER_TABLE[i].Read("SP_Multi") + 100));
                            for (int x = 0; x < 3; x++) {
                                if (x != i && Globals.PARTY_SLOT[x] < 9) {
                                    ushort spMulti = Globals.CHARACTER_TABLE[x].Read("SP_Multi");
                                    if (spMulti == 0) {
                                        Globals.CHARACTER_TABLE[x].Write("SP_Multi", 65486);
                                    } else {
                                        Globals.CHARACTER_TABLE[x].Write("SP_Multi", (Globals.CHARACTER_TABLE[x].Read("SP_Multi") - 50));
                                    }
                                }
                            }
                        }

                        if (Globals.CHARACTER_TABLE[i].Read("Accessory") == 177) { //Soa's Ahnk
                            if (dmScripts.ContainsKey("btnSoloMode") && dmScripts["btnSoloMode"]) {
                                Globals.CHARACTER_TABLE[i].Write("Revive", (Globals.CHARACTER_TABLE[i].Read("Revive") + 50));
                            } else {
                                Globals.CHARACTER_TABLE[i].Write("Revive", 100);
                            }
                        }

                        if (Globals.CHARACTER_TABLE[i].Read("Accessory") == 178) { //Soa's Health Ring
                            for (int x = 0; x < 3; x++) {
                                if (x != i && Globals.PARTY_SLOT[x] < 9) {
                                    Globals.CHARACTER_TABLE[x].Write("Max_HP", Math.Round(Globals.CHARACTER_TABLE[x].Read("Max_HP") * 0.75));

                                    if (Globals.CHARACTER_TABLE[x].Read("HP") > Globals.CHARACTER_TABLE[x].Read("Max_HP")) {
                                        Globals.CHARACTER_TABLE[x].Write("HP", Globals.CHARACTER_TABLE[x].Read("Max_HP"));
                                    }
                                }
                            }
                        }

                        if (Globals.CHARACTER_TABLE[i].Read("Accessory") == 179) { //Soa's Mage Ring
                            Globals.CHARACTER_TABLE[i].Write("MP", (Globals.CHARACTER_TABLE[i].Read("MP") * 3));
                            Globals.CHARACTER_TABLE[i].Write("Max_MP", (Globals.CHARACTER_TABLE[i].Read("Max_MP") * 3));
                            for (int x = 0; x < 3; x++) {
                                if (x != i && Globals.PARTY_SLOT[x] < 9) {
                                    Globals.CHARACTER_TABLE[x].Write("HP" + 0xA, (ushort) Math.Round(Globals.CHARACTER_TABLE[x].Read("Max_MP") * 0.5));

                                    if (Globals.CHARACTER_TABLE[x].Read("MP") > Globals.CHARACTER_TABLE[x].Read("Max_MP")) {
                                        Globals.CHARACTER_TABLE[x].Write("HP", Globals.CHARACTER_TABLE[x].Read("Max_MP"));
                                    }
                                }
                            }
                        }

                        if (Globals.CHARACTER_TABLE[i].Read("Accessory") == 180) { //Soa's Shield Ring
                            Globals.CHARACTER_TABLE[i].Write("DF", 1);
                            Globals.CHARACTER_TABLE[i].Write("MDF", 1);
                            Globals.CHARACTER_TABLE[i].Write("A_AV", 90);
                            Globals.CHARACTER_TABLE[i].Write("M_AV", 90);
                            for (int x = 0; x < 3; x++) {
                                if (x != i && Globals.PARTY_SLOT[x] < 9) {
                                    Globals.CHARACTER_TABLE[x].Write("HP" + 0x34, Math.Round(Globals.CHARACTER_TABLE[x].Read("A_HIT") * 0.8));
                                    Globals.CHARACTER_TABLE[x].Write("HP" + 0x36, Math.Round(Globals.CHARACTER_TABLE[x].Read("M_HIT") * 0.8));
                                }
                            }
                        }

                        if (Globals.CHARACTER_TABLE[i].Read("Accessory") == 181) { //Soa's Siphon Ring
                            soasSiphonSlot = i;
                            Globals.CHARACTER_TABLE[i].Write("MAT", (Globals.CHARACTER_TABLE[i].Read("MAT") * 2));
                            Globals.CHARACTER_TABLE[i].Write("DMAT", Math.Round(Globals.CHARACTER_TABLE[i].Read("DMAT") * 0.3));
                            for (int x = 0; x < 3; x++) {
                                if (x != i && Globals.PARTY_SLOT[x] < 9) {
                                    Globals.CHARACTER_TABLE[x].Write("MATK", Math.Round(Globals.CHARACTER_TABLE[x].Read("MATK") * 0.8));
                                }
                            }
                        }

                        if (Globals.CHARACTER_TABLE[i].Read("Armor") == 74) { //Armor of Legend
                            emulator.WriteByte(p + 0x10C, 0);
                            Globals.CHARACTER_TABLE[i].Write("A_AV", (Globals.CHARACTER_TABLE[i].Read("A_AV") - 50));
                            if (Globals.PARTY_SLOT[i] == 0) {
                                Globals.CHARACTER_TABLE[i].Write("DF", (Globals.CHARACTER_TABLE[i].Read("DF") + 41 - 127));
                                Globals.CHARACTER_TABLE[i].Write("MDF", (Globals.CHARACTER_TABLE[i].Read("MDF") + 40));
                            } else if (Globals.PARTY_SLOT[i] == 1 || Globals.PARTY_SLOT[i] == 5) {
                                Globals.CHARACTER_TABLE[i].Write("DF", (Globals.CHARACTER_TABLE[i].Read("DF") + 54 - 127));
                                Globals.CHARACTER_TABLE[i].Write("MDF", (Globals.CHARACTER_TABLE[i].Read("MDF") + 27));
                            } else if (Globals.PARTY_SLOT[i] == 2 || Globals.PARTY_SLOT[i] == 8) {
                                Globals.CHARACTER_TABLE[i].Write("DF", (Globals.CHARACTER_TABLE[i].Read("DF") + 27 - 127));
                                Globals.CHARACTER_TABLE[i].Write("MDF", (Globals.CHARACTER_TABLE[i].Read("MDF") + 80));
                            } else if (Globals.PARTY_SLOT[i] == 3) {
                                Globals.CHARACTER_TABLE[i].Write("DF", (Globals.CHARACTER_TABLE[i].Read("DF") + 41 - 127));
                                Globals.CHARACTER_TABLE[i].Write("MDF", (Globals.CHARACTER_TABLE[i].Read("MDF") + 42));
                            } else if (Globals.PARTY_SLOT[i] == 4) {
                                Globals.CHARACTER_TABLE[i].Write("DF", (Globals.CHARACTER_TABLE[i].Read("DF") + 45 - 127));
                                Globals.CHARACTER_TABLE[i].Write("MDF", (Globals.CHARACTER_TABLE[i].Read("MDF") + 40));
                            } else if (Globals.PARTY_SLOT[i] == 6) {
                                Globals.CHARACTER_TABLE[i].Write("DF", (Globals.CHARACTER_TABLE[i].Read("DF") + 30 - 127));
                                Globals.CHARACTER_TABLE[i].Write("MDF", (Globals.CHARACTER_TABLE[i].Read("MDF") + 54));
                            } else if (Globals.PARTY_SLOT[i] == 7) {
                                Globals.CHARACTER_TABLE[i].Write("DF", (Globals.CHARACTER_TABLE[i].Read("DF") + 88 - 127));
                                Globals.CHARACTER_TABLE[i].Write("MDF", (Globals.CHARACTER_TABLE[i].Read("MDF") + 23));
                            }
                        }

                        if (Globals.CHARACTER_TABLE[i].Read("Helmet") == 89) { //Legend Casque
                            emulator.WriteByte(p + 0x10E, 0);
                            Globals.CHARACTER_TABLE[i].Write("M_AV", (Globals.CHARACTER_TABLE[i].Read("M_AV") - 50));
                            Globals.CHARACTER_TABLE[i].Write("MDF", (Globals.CHARACTER_TABLE[i].Read("MDF") + 70 - 127));
                        }


                        guardStatusDF[i] = 0;
                        guardStatusMDF[i] = 0;
                        lGuardStatusDF[i] = 0;
                        lGuardStatusMDF[i] = 0;
                        lGuardStateDF[i] = false;
                        lGuardStateMDF[i] = false;
                        sGuardStatusDF[i] = false;
                        sGuardStatusMDF[i] = false;
                    }
                }
                equipChangesOnBattleEntry = true;
            } else {
                if (!Globals.IN_BATTLE && equipChangesOnBattleEntry) {
                    equipChangesOnBattleEntry = false;
                    checkHarpoon = false;
                } else {
                    if (emulator.ReadShort(Constants.GetAddress("BATTLE_VALUE")) == 41215 && Globals.STATS_CHANGED /*&& equipChangesLoop*/) { //Battle Loop
                        for (int i = 0; i < 3; i++) {
                            long p = Globals.CHAR_ADDRESS[i];
                            if (Globals.CHARACTER_TABLE[i].Read("Weapon") == 159 && Globals.PARTY_SLOT[i] == 0) { //Spirit Eater
                                if (Globals.CHARACTER_TABLE[i].Read("SP") == (emulator.ReadByte(Constants.GetAddress("DART_DRAGOON_LEVEL")) * 100)) {
                                    Globals.CHARACTER_TABLE[i].Write("SP_Regen", spiritEaterSaveSP);
                                } else {
                                    if (spiritEaterSaveSP < spiritEaterSP) {
                                        Globals.CHARACTER_TABLE[i].Write("SP_Regen", (65536 - (spiritEaterSP - spiritEaterSaveSP)));
                                    } else {
                                        Globals.CHARACTER_TABLE[i].Write("SP_Regen", (spiritEaterSaveSP - spiritEaterSP));
                                    }
                                }
                            }

                            if (Globals.CHARACTER_TABLE[i].Read("Weapon") == 160 && (Globals.PARTY_SLOT[i] == 1 || Globals.PARTY_SLOT[i] == 5)) { //Harpoon
                                if (Globals.CHARACTER_TABLE[i].Read("Action") == 10 && Globals.CHARACTER_TABLE[i].Read("SP") >= 400) {
                                    checkHarpoon = true;
                                    if (Globals.CHARACTER_TABLE[i].Read("SP") == 500) {
                                        emulator.WriteAOB(p + 0xC0, "00 00 00 04");
                                        Globals.CHARACTER_TABLE[i].Write("SP", 200);
                                        emulator.WriteByte(Constants.GetAddress("DRAGOON_TURNS") + i * 4, 2);
                                    } else {
                                        emulator.WriteAOB(p + 0xC0, "00 00 00 03");
                                        Globals.CHARACTER_TABLE[i].Write("SP", 100);
                                        emulator.WriteByte(Constants.GetAddress("DRAGOON_TURNS") + i * 4, 1);
                                    }
                                }

                                if (emulator.ReadByte(Constants.GetAddress("DRAGOON_TURNS") + i * 4) == 0) {
                                    checkHarpoon = false;
                                }
                            }

                            if (Globals.CHARACTER_TABLE[i].Read("Weapon") == 161 && (Globals.PARTY_SLOT[i] == 2 || Globals.PARTY_SLOT[i] == 8)) { //Element Arrow
                                if (elementArrowLastAction != Globals.CHARACTER_TABLE[i].Read("Action")) {
                                    if (elementArrowLastAction == 8 && Globals.CHARACTER_TABLE[i].Read("Action") == 136) {
                                        //old method ...
                                    } else {
                                        elementArrowLastAction = Globals.CHARACTER_TABLE[i].Read("Action");
                                        if (elementArrowLastAction == 8) {
                                            Globals.CHARACTER_TABLE[i].Write("Element", elementArrowElement);
                                            elementArrowTurns += 1;
                                        } else {
                                            if (elementArrowLastAction == 10) {
                                                Globals.CHARACTER_TABLE[i].Write("Element", 0);
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

                            if (Globals.CHARACTER_TABLE[i].Read("Weapon") == 162 && Globals.PARTY_SLOT[i] == 3) { //Dragon Buster II
                                if (Globals.CHARACTER_TABLE[i].Read("Action") == 136) {
                                    if (emulator.ReadShort(Constants.GetAddress("DAMAGE_SLOT1")) != 0) {
                                        emulator.WriteShort(p, (ushort) Math.Min(Globals.CHARACTER_TABLE[i].Read("HP") + Math.Round(emulator.ReadShort(Constants.GetAddress("DAMAGE_SLOT1")) * 0.02) + 2, Globals.CHARACTER_TABLE[i].Read("Max_HP")));
                                        emulator.WriteShort(Constants.GetAddress("DAMAGE_SLOT1"), 0);
                                        /*} else {
                                            emulator.WriteShort(Constants.GetAddress("DAMAGE_SLOT1"), 0);*/
                                    }
                                }
                            }

                            if (Globals.CHARACTER_TABLE[i].Read("Weapon") == 163 && Globals.PARTY_SLOT[i] == 4) { //Battery Glove
                                if ((Globals.CHARACTER_TABLE[i].Read("Action") == 136 || Globals.CHARACTER_TABLE[i].Read("Action") == 26) &&
                                (gloveLastAction != 136 && gloveLastAction != 26)) {
                                    gloveCharge += 1;
                                    if (gloveCharge == 7) {
                                        Globals.CHARACTER_TABLE[i].Write("AT", Math.Round(originalCharacterStats[i, 1] * 2.5));
                                    } else {
                                        if (gloveCharge > 7) {
                                            gloveCharge = 1;
                                            Globals.CHARACTER_TABLE[i].Write("AT", originalCharacterStats[i, 1]);
                                        }
                                    }

                                    gloveLastAction = Globals.CHARACTER_TABLE[i].Read("Action");
                                }

                                //stsProgram.Text = "Glove Charge: " & gloveCharge & " / " & gloveLastAction & " / " & Globals.CHARACTER_TABLE[i].Read("Action") & " / " &
                                //    (Globals.CHARACTER_TABLE[i].Read("Action") = 136 Or Globals.CHARACTER_TABLE[i].Read("Action") = 138) & " / " &
                                //    (gloveLastAction <> 136 Or gloveLastAction <> 138) & " / " &
                                //    ((Globals.CHARACTER_TABLE[i].Read("Action") = 136 Or Globals.CHARACTER_TABLE[i].Read("Action") = 138) And
                                //    (gloveLastAction <> 136 And gloveLastAction <> 138))
                            }

                            if (Globals.CHARACTER_TABLE[i].Read("Weapon") == 165 && Globals.PARTY_SLOT[i] == 7) { //Giant Axe
                                if (Globals.CHARACTER_TABLE[i].Read("Action") == 136 && axeLastAction != Globals.CHARACTER_TABLE[i].Read("Action")) {
                                    axeLastAction = Globals.CHARACTER_TABLE[i].Read("Action");
                                    if (new Random().Next(0, 9) < 2) {
                                        Globals.CHARACTER_TABLE[i].Write("Guard", 1);
                                    }
                                } else {
                                    axeLastAction = Globals.CHARACTER_TABLE[i].Read("Action");
                                }
                            }

                            if (Globals.CHARACTER_TABLE[i].Read("Helmet") == 167) { //Fake Legend Casque
                                if (Globals.CHARACTER_TABLE[i].Read("Guard") == 1 && guardStatusMDF[i] == 0) {
                                    if (new Random().Next(0, 9) < 3) {
                                        Globals.CHARACTER_TABLE[i].Write("MDF", (Globals.CHARACTER_TABLE[i].Read("MDF") + 40));
                                        guardStatusMDF[i] = 1;
                                    }
                                }
                                if ((Globals.CHARACTER_TABLE[i].Read("Action") == 8 || Globals.CHARACTER_TABLE[i].Read("Action") == 10) && guardStatusMDF[i] == 1) {
                                    Globals.CHARACTER_TABLE[i].Write("MDF", originalCharacterStats[i, 4]);
                                    guardStatusMDF[i] = 0;
                                }
                            }

                            if (Globals.CHARACTER_TABLE[i].Read("Armor") == 169) { //Fake Legend Armor
                                if (Globals.CHARACTER_TABLE[i].Read("Guard") == 1 && guardStatusDF[i] == 0) {
                                    if (new Random().Next(0, 9) < 3) {
                                        Globals.CHARACTER_TABLE[i].Write("DF", (Globals.CHARACTER_TABLE[i].Read("DF") + 40));
                                        guardStatusDF[i] = 1;
                                    }
                                }
                                if ((Globals.CHARACTER_TABLE[i].Read("Action") == 8 || Globals.CHARACTER_TABLE[i].Read("Action") == 10) && guardStatusDF[i] == 1) {
                                    Globals.CHARACTER_TABLE[i].Write("DF", originalCharacterStats[i, 3]);
                                    guardStatusDF[i] = 0;
                                }
                            }

                            if (Globals.CHARACTER_TABLE[i].Read("Armor") == 170) { //Divine DG Armor
                                Globals.CHARACTER_TABLE[i].Write("SP_P_Hit", originalCharacterStats[i, 6]);
                                Globals.CHARACTER_TABLE[i].Write("MP_P_Hit", originalCharacterStats[i, 7]);
                                Globals.CHARACTER_TABLE[i].Write("SP_M_Hit", originalCharacterStats[i, 8]);
                                Globals.CHARACTER_TABLE[i].Write("MP_M_Hit", originalCharacterStats[i, 9]);
                            }

                            if (Globals.CHARACTER_TABLE[i].Read("Shoes") == 172 || Globals.CHARACTER_TABLE[i].Read("Shoes") == 173 || Globals.CHARACTER_TABLE[i].Read("Shoes") == 174) { //Lloyd's Boots/ Winged Shoes / Soa's Greaves
                                if (emulator.ReadByte(p + 0xC1) > 0) {
                                    Globals.CHARACTER_TABLE[i].Write("SPD", (originalCharacterStats[i, 5] * 2));
                                } else {
                                    if (emulator.ReadByte(p + 0xC3) > 0) {
                                        Globals.CHARACTER_TABLE[i].Write("SPD", (originalCharacterStats[i, 5] / 2));
                                    } else {
                                        Globals.CHARACTER_TABLE[i].Write("SPD", (originalCharacterStats[i, 5]));
                                    }
                                }

                                if (Globals.CHARACTER_TABLE[i].Read("Shoes") == 174) {
                                    for (int x = 0; x < 3; x++) {
                                        if (x != i && Globals.PARTY_SLOT[i] < 9 && Globals.CHARACTER_TABLE[x].Read("HP") > 0) {
                                            if (Globals.CHARACTER_TABLE[x].Read("HP") > 0) {
                                                if (emulator.ReadByte(Globals.CHAR_ADDRESS[x] + 0xC1) > 0) {
                                                    Globals.CHARACTER_TABLE[x].Write("HP" + 0x2A, (ushort) (originalCharacterStats[x, 5] * 2));
                                                } else {
                                                    if (emulator.ReadByte(Globals.CHAR_ADDRESS[x] + 0xC3) > 0) {
                                                        Globals.CHARACTER_TABLE[x].Write("HP" + 0x2A, (ushort) (originalCharacterStats[x, 5] / 2));
                                                    } else {
                                                        Globals.CHARACTER_TABLE[x].Write("HP" + 0x2A, (ushort) (originalCharacterStats[x, 5]));
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if (Globals.CHARACTER_TABLE[i].Read("Accessory") == 177) { //Soa's Ahnk
                                bool alive = false;
                                int kill = -1;
                                int lastPartyID = -1;
                                if (Globals.CHARACTER_TABLE[i].Read("HP") == 0) {
                                    for (int x = 0; x < 3; x++) {
                                        if (x != i && Globals.PARTY_SLOT[i] < 9 && Globals.CHARACTER_TABLE[x].Read("HP") > 0) {
                                            alive = true;
                                        }
                                    }

                                    if (alive) {
                                        for (int x = 0; x < 3; x++) {
                                            if (kill == -1 && new Random().Next(0, 9) < 5 && Globals.CHARACTER_TABLE[x].Read("HP") > 0) {
                                                kill = x;
                                            } else {
                                                lastPartyID = x;
                                            }
                                        }
                                    }

                                    if (kill != -1) {
                                        Globals.CHARACTER_TABLE[kill].Write("HP", 0);
                                        Globals.CHARACTER_TABLE[kill].Write("Action", 192);
                                    } else {
                                        Globals.CHARACTER_TABLE[lastPartyID].Write("HP", 0);
                                        Globals.CHARACTER_TABLE[lastPartyID].Write("Action", 192);
                                    }
                                    Globals.CHARACTER_TABLE[i].Write("HP", 1);
                                } else {
                                    Globals.CHARACTER_TABLE[i].Write("MAX_HP", 0);
                                    Globals.CHARACTER_TABLE[i].Write("Revive", 0);
                                    Globals.CHARACTER_TABLE[i].Write("Action", 192);
                                }
                            }

                            if (Globals.CHARACTER_TABLE[i].Read("Armor") == 74) { //Armor of Legend
                                if (lGuardStateDF[i] == false) {
                                    if (Globals.CHARACTER_TABLE[i].Read("Guard") == 1 && guardStatusDF[i] == 0) {
                                        guardStatusDF[i] = 1;
                                        lGuardStatusDF[i] += 1;
                                    }
                                    if ((Globals.CHARACTER_TABLE[i].Read("Action") == 8 || Globals.CHARACTER_TABLE[i].Read("Action") == 10) && guardStatusDF[i] == 1 && Globals.CHARACTER_TABLE[i].Read("Guard") == 0) {
                                        guardStatusDF[i] = 0;
                                    }
                                    if (lGuardStatusDF[i] >= 3) {
                                        lGuardStatusDF[i] = 0;
                                        guardStatusDF[i] = 1;
                                        lGuardStateDF[i] = true;
                                        emulator.WriteByte(p + 0xB5, 4);
                                        Globals.CHARACTER_TABLE[i].Write("DF", Math.Round(originalCharacterStats[i, 3] * 1.2));
                                    }
                                } else {
                                    if (emulator.ReadByte(p + 0xB5) == 0) {
                                        Globals.CHARACTER_TABLE[i].Write("DF", originalCharacterStats[i, 3]);
                                        lGuardStateDF[i] = false;
                                    }
                                }
                                if (!sGuardStatusDF[i]) {
                                    if (Globals.CHARACTER_TABLE[i].Read("Action") == 8 || Globals.CHARACTER_TABLE[i].Read("Action") == 10) {
                                        if (new Random().Next(0, 100) <= 10) {
                                            emulator.WriteByte(Constants.GetAddress("SPECIAL_EFFECT") + ((Globals.MONSTER_SIZE + i) * 0x20) + EquipChangesBattleGlobalOffset(), emulator.ReadByte(Constants.GetAddress("SPECIAL_EFFECT") + ((Globals.MONSTER_SIZE + i) * 0x20) + EquipChangesBattleGlobalOffset()) + 1);
                                        }
                                        sGuardStatusDF[i] = true;
                                    }
                                } else {
                                    if (Globals.CHARACTER_TABLE[i].Read("Action") != 8 && Globals.CHARACTER_TABLE[i].Read("Action") != 10) {
                                        sGuardStatusDF[i] = false;
                                    }
                                }
                            }

                            if (Globals.CHARACTER_TABLE[i].Read("Helmet") == 89) { //Legend Casque
                                if (!lGuardStateMDF[i]) {
                                    if (Globals.CHARACTER_TABLE[i].Read("Guard") == 1 && guardStatusMDF[i] == 0) {
                                        guardStatusMDF[i] = 1;
                                        lGuardStatusMDF[i] += 1;
                                    }
                                    if ((Globals.CHARACTER_TABLE[i].Read("Action") == 8 || Globals.CHARACTER_TABLE[i].Read("Action") == 10) && guardStatusMDF[i] == 1 && Globals.CHARACTER_TABLE[i].Read("Guard") == 0) {
                                        guardStatusMDF[i] = 0;
                                    }
                                    if (lGuardStatusMDF[i] >= 3) {
                                        lGuardStatusMDF[i] = 0;
                                        guardStatusMDF[i] = 1;
                                        lGuardStateMDF[i] = true;
                                        emulator.WriteByte(p + 0xB7, 4);
                                        Globals.CHARACTER_TABLE[i].Write("MDF", Math.Round(originalCharacterStats[i, 4] * 1.2));
                                    }
                                } else {
                                    if (emulator.ReadByte(p + 0xB7) == 0) {
                                        Globals.CHARACTER_TABLE[i].Write("MDF", originalCharacterStats[i, 4]);
                                        lGuardStateMDF[i] = false;
                                    }
                                }
                                if (!sGuardStatusMDF[i]) {
                                    if (Globals.CHARACTER_TABLE[i].Read("Action") == 8 || Globals.CHARACTER_TABLE[i].Read("Action") == 10) {
                                        if (new Random().Next(0, 100) <= 10) {
                                            emulator.WriteByte(Constants.GetAddress("SPECIAL_EFFECT") + ((Globals.MONSTER_SIZE + i) * 0x20) + EquipChangesBattleGlobalOffset(), emulator.ReadByte(Constants.GetAddress("SPECIAL_EFFECT") + ((Globals.MONSTER_SIZE + i) * 0x20) + EquipChangesBattleGlobalOffset()) + 4);
                                        }
                                        sGuardStatusMDF[i] = true;
                                    }
                                } else {
                                    if (Globals.CHARACTER_TABLE[i].Read("Action") != 8 && Globals.CHARACTER_TABLE[i].Read("Action") != 10) {
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

        #region Ultimate Boss
        #region Field
        public void UltimateBossField() {
            if ((Globals.MAP >= 393 && Globals.MAP <= 405) && Globals.CHAPTER == 4 && !ultimateBossOnBattleEntry) {
                if (uiCombo["cboUltimateBoss"] == 0 && (Globals.MAP >= 393 && Globals.MAP <= 394)) {
                    emulator.WriteShort(Constants.GetAddress("ENCOUNTER_ID"), 487);
                    emulator.WriteByte(Constants.GetAddress("BATTLE_FIELD"), 10);
                } else if (uiCombo["cboUltimateBoss"] == 1 && (Globals.MAP >= 393 && Globals.MAP <= 394)) {
                    emulator.WriteShort(Constants.GetAddress("ENCOUNTER_ID"), 386);
                    emulator.WriteByte(Constants.GetAddress("BATTLE_FIELD"), 3);
                } else if (uiCombo["cboUltimateBoss"] == 2 && (Globals.MAP >= 393 && Globals.MAP <= 394)) {
                    emulator.WriteShort(Constants.GetAddress("ENCOUNTER_ID"), 414);
                    emulator.WriteByte(Constants.GetAddress("BATTLE_FIELD"), 8);
                } else if (uiCombo["cboUltimateBoss"] == 3 && (Globals.MAP >= 395 && Globals.MAP <= 397)) {
                    emulator.WriteShort(Constants.GetAddress("ENCOUNTER_ID"), 461);
                    emulator.WriteByte(Constants.GetAddress("BATTLE_FIELD"), 21);
                } else if (uiCombo["cboUltimateBoss"] == 4 && (Globals.MAP >= 395 && Globals.MAP <= 397)) {
                    emulator.WriteShort(Constants.GetAddress("ENCOUNTER_ID"), 412);
                    emulator.WriteByte(Constants.GetAddress("BATTLE_FIELD"), 16);
                } else if (uiCombo["cboUltimateBoss"] == 5 && (Globals.MAP >= 395 && Globals.MAP <= 397)) {
                    emulator.WriteShort(Constants.GetAddress("ENCOUNTER_ID"), 413);
                    emulator.WriteByte(Constants.GetAddress("BATTLE_FIELD"), 70);
                } else if (uiCombo["cboUltimateBoss"] == 6 && (Globals.MAP >= 395 && Globals.MAP <= 397)) {
                    emulator.WriteShort(Constants.GetAddress("ENCOUNTER_ID"), 387);
                    emulator.WriteByte(Constants.GetAddress("BATTLE_FIELD"), 5);
                } else if (uiCombo["cboUltimateBoss"] == 7 && (Globals.MAP >= 395 && Globals.MAP <= 397)) {
                    emulator.WriteShort(Constants.GetAddress("ENCOUNTER_ID"), 415);
                    emulator.WriteByte(Constants.GetAddress("BATTLE_FIELD"), 12);
                } else if (uiCombo["cboUltimateBoss"] == 8 && (Globals.MAP >= 398 && Globals.MAP <= 400)) {
                    emulator.WriteShort(Constants.GetAddress("ENCOUNTER_ID"), 449);
                    emulator.WriteByte(Constants.GetAddress("BATTLE_FIELD"), 68);
                } else if (uiCombo["cboUltimateBoss"] == 9 && (Globals.MAP >= 398 && Globals.MAP <= 400)) {
                    emulator.WriteShort(Constants.GetAddress("ENCOUNTER_ID"), 402);
                    emulator.WriteByte(Constants.GetAddress("BATTLE_FIELD"), 23);
                } else if (uiCombo["cboUltimateBoss"] == 10 && (Globals.MAP >= 398 && Globals.MAP <= 400)) {
                    emulator.WriteShort(Constants.GetAddress("ENCOUNTER_ID"), 403);
                    emulator.WriteByte(Constants.GetAddress("BATTLE_FIELD"), 29);
                } else if (uiCombo["cboUltimateBoss"] == 11 && (Globals.MAP >= 398 && Globals.MAP <= 400)) {
                    emulator.WriteShort(Constants.GetAddress("ENCOUNTER_ID"), 417);
                    emulator.WriteByte(Constants.GetAddress("BATTLE_FIELD"), 31);
                } else if (uiCombo["cboUltimateBoss"] == 12 && (Globals.MAP >= 398 && Globals.MAP <= 400)) {
                    emulator.WriteShort(Constants.GetAddress("ENCOUNTER_ID"), 418);
                    emulator.WriteByte(Constants.GetAddress("BATTLE_FIELD"), 41);
                } else if (uiCombo["cboUltimateBoss"] == 13 && (Globals.MAP >= 398 && Globals.MAP <= 400)) {
                    emulator.WriteShort(Constants.GetAddress("ENCOUNTER_ID"), 448);
                    emulator.WriteByte(Constants.GetAddress("BATTLE_FIELD"), 68);
                } else if (uiCombo["cboUltimateBoss"] == 14 && (Globals.MAP >= 398 && Globals.MAP <= 400)) {
                    emulator.WriteShort(Constants.GetAddress("ENCOUNTER_ID"), 416);
                    emulator.WriteByte(Constants.GetAddress("BATTLE_FIELD"), 38);
                } else if (uiCombo["cboUltimateBoss"] == 15 && (Globals.MAP >= 398 && Globals.MAP <= 400)) {
                    emulator.WriteShort(Constants.GetAddress("ENCOUNTER_ID"), 422);
                    emulator.WriteByte(Constants.GetAddress("BATTLE_FIELD"), 42);
                } else if (uiCombo["cboUltimateBoss"] == 16 && (Globals.MAP >= 398 && Globals.MAP <= 400)) {
                    emulator.WriteShort(Constants.GetAddress("ENCOUNTER_ID"), 423);
                    emulator.WriteByte(Constants.GetAddress("BATTLE_FIELD"), 47);
                } else if (uiCombo["cboUltimateBoss"] == 17 && (Globals.MAP >= 398 && Globals.MAP <= 400)) {
                    emulator.WriteShort(Constants.GetAddress("ENCOUNTER_ID"), 432);
                    emulator.WriteByte(Constants.GetAddress("BATTLE_FIELD"), 69);
                } else if (uiCombo["cboUltimateBoss"] == 18 && (Globals.MAP >= 398 && Globals.MAP <= 400)) {
                    emulator.WriteShort(Constants.GetAddress("ENCOUNTER_ID"), 430);
                    emulator.WriteByte(Constants.GetAddress("BATTLE_FIELD"), 67);
                } else if (uiCombo["cboUltimateBoss"] == 19 && (Globals.MAP >= 398 && Globals.MAP <= 400)) {
                    emulator.WriteShort(Constants.GetAddress("ENCOUNTER_ID"), 433);
                    emulator.WriteByte(Constants.GetAddress("BATTLE_FIELD"), 56);
                } else if (uiCombo["cboUltimateBoss"] == 20 && (Globals.MAP >= 398 && Globals.MAP <= 400)) {
                    emulator.WriteShort(Constants.GetAddress("ENCOUNTER_ID"), 431);
                    emulator.WriteByte(Constants.GetAddress("BATTLE_FIELD"), 54);
                } else if (uiCombo["cboUltimateBoss"] == 21 && (Globals.MAP >= 398 && Globals.MAP <= 400)) {
                    emulator.WriteShort(Constants.GetAddress("ENCOUNTER_ID"), 447);
                    emulator.WriteByte(Constants.GetAddress("BATTLE_FIELD"), 68);
                } else if (uiCombo["cboUltimateBoss"] == 22 && (Globals.MAP >= 401 && Globals.MAP <= 405)) {
                    emulator.WriteShort(Constants.GetAddress("ENCOUNTER_ID"), 408);
                    emulator.WriteByte(Constants.GetAddress("BATTLE_FIELD"), 12);
                } else if (uiCombo["cboUltimateBoss"] == 23 && (Globals.MAP >= 401 && Globals.MAP <= 405)) {
                    emulator.WriteShort(Constants.GetAddress("ENCOUNTER_ID"), 389);
                    emulator.WriteByte(Constants.GetAddress("BATTLE_FIELD"), 21);
                } else if (uiCombo["cboUltimateBoss"] == 24 && (Globals.MAP >= 401 && Globals.MAP <= 405)) {
                    emulator.WriteShort(Constants.GetAddress("ENCOUNTER_ID"), 396);
                    emulator.WriteByte(Constants.GetAddress("BATTLE_FIELD"), 30);
                } else if (uiCombo["cboUltimateBoss"] == 25 && (Globals.MAP >= 401 && Globals.MAP <= 405)) {
                    emulator.WriteShort(Constants.GetAddress("ENCOUNTER_ID"), 399);
                    emulator.WriteByte(Constants.GetAddress("BATTLE_FIELD"), 72);
                } else if (uiCombo["cboUltimateBoss"] == 26 && (Globals.MAP >= 401 && Globals.MAP <= 405)) {
                    emulator.WriteShort(Constants.GetAddress("ENCOUNTER_ID"), 409);
                    emulator.WriteByte(Constants.GetAddress("BATTLE_FIELD"), 27);
                } else if (uiCombo["cboUltimateBoss"] == 27 && (Globals.MAP >= 401 && Globals.MAP <= 405)) {
                    emulator.WriteShort(Constants.GetAddress("ENCOUNTER_ID"), 393);
                    emulator.WriteByte(Constants.GetAddress("BATTLE_FIELD"), 14);
                } else if (uiCombo["cboUltimateBoss"] == 28 && (Globals.MAP >= 401 && Globals.MAP <= 405)) {
                    emulator.WriteShort(Constants.GetAddress("ENCOUNTER_ID"), 398);
                    emulator.WriteByte(Constants.GetAddress("BATTLE_FIELD"), 73);
                } else if (uiCombo["cboUltimateBoss"] == 29 && (Globals.MAP >= 401 && Globals.MAP <= 405)) {
                    emulator.WriteShort(Constants.GetAddress("ENCOUNTER_ID"), 397);
                    emulator.WriteByte(Constants.GetAddress("BATTLE_FIELD"), 35);
                } else if (uiCombo["cboUltimateBoss"] == 30 && (Globals.MAP >= 401 && Globals.MAP <= 405)) {
                    emulator.WriteShort(Constants.GetAddress("ENCOUNTER_ID"), 400);
                    emulator.WriteByte(Constants.GetAddress("BATTLE_FIELD"), 76);
                } else if (uiCombo["cboUltimateBoss"] == 31 && (Globals.MAP >= 401 && Globals.MAP <= 405)) {
                    emulator.WriteShort(Constants.GetAddress("ENCOUNTER_ID"), 410);
                } else if (uiCombo["cboUltimateBoss"] == 32 && (Globals.MAP >= 401 && Globals.MAP <= 405)) {
                    emulator.WriteShort(Constants.GetAddress("ENCOUNTER_ID"), 401);
                    emulator.WriteByte(Constants.GetAddress("BATTLE_FIELD"), 77);
                } else if (uiCombo["cboUltimateBoss"] == 33 && (Globals.MAP >= 401 && Globals.MAP <= 405)) {
                    emulator.WriteShort(Constants.GetAddress("ENCOUNTER_ID"), 390);
                    emulator.WriteByte(Constants.GetAddress("BATTLE_FIELD"), 22);
                } else if (uiCombo["cboUltimateBoss"] == 34 && (Globals.MAP >= 401 && Globals.MAP <= 405)) {
                    emulator.WriteShort(Constants.GetAddress("ENCOUNTER_ID"), 411);
                    emulator.WriteByte(Constants.GetAddress("BATTLE_FIELD"), 63);
                } else if (uiCombo["cboUltimateBoss"] == 35 && (Globals.MAP >= 401 && Globals.MAP <= 405)) {
                    emulator.WriteShort(Constants.GetAddress("ENCOUNTER_ID"), 394);
                    emulator.WriteByte(Constants.GetAddress("BATTLE_FIELD"), 40);
                } else {
                    this.Dispatcher.BeginInvoke(new Action(() => {
                        dmScripts["btnUltimateBoss"] = false;
                        TurnOnOffButton(ref btnUltimateBoss);
                    }), DispatcherPriority.ContextIdle);
                }
            }
        }

        public void UltimateBossFieldSet() {
            if (dmScripts.ContainsKey("btnUltimateBoss") && dmScripts["btnUltimateBoss"]) {
                if (Globals.MAP >= 393 && Globals.MAP <= 405) {
                    if (ultimateBossCompleted >= uiCombo["cboUltimateBoss"]) {
                        if ((uiCombo["cboUltimateBoss"] >= 0 && uiCombo["cboUltimateBoss"] <= 2) && Globals.MAP > 394) {
                            dmScripts["btnUltimateBoss"] = false;
                            Constants.WriteGLogOutput("You must be in Zone 1 - Maps 393-394.");
                        }

                        if ((uiCombo["cboUltimateBoss"] >= 3 && uiCombo["cboUltimateBoss"] <= 7) && (Globals.MAP < 395 || Globals.MAP > 397)) {
                            dmScripts["btnUltimateBoss"] = false;
                            Constants.WriteGLogOutput("You must be in Zone 2 - Maps 395-397.");
                        }

                        if ((uiCombo["cboUltimateBoss"] >= 8 && uiCombo["cboUltimateBoss"] <= 21) && (Globals.MAP < 398 || Globals.MAP > 400)) {
                            dmScripts["btnUltimateBoss"] = false;
                            Constants.WriteGLogOutput("You must be in Zone 3 - Maps 398-400.");
                        }

                        if (uiCombo["cboUltimateBoss"] >= 22 && Globals.MAP < 401) {
                            dmScripts["btnUltimateBoss"] = false;
                            Constants.WriteGLogOutput("You must be in Zone 4 - Maps 401-405.");
                        }
                    } else {
                        dmScripts["btnUltimateBoss"] = false;
                        Constants.WriteGLogOutput("You must defeat a previous Ultimate Boss.");
                    }
                } else {
                    dmScripts["btnUltimateBoss"] = false;
                    Constants.WriteGLogOutput("You are not in the Forbidden Land.");
                }

                this.Dispatcher.BeginInvoke(new Action(() => {
                    TurnOnOffButton(ref btnUltimateBoss);
                }), DispatcherPriority.ContextIdle);
            }
        }
        #endregion

        #region Battle
        public void UltimateBossBattle() {
            if (Globals.IN_BATTLE && Globals.STATS_CHANGED && !ultimateBossOnBattleEntry) {
                ubHPChanged = ubCheckedDamage = ubUltimateHPSet = false;
                ubCheckDamageCycle = 0;
                ubDragoonBondMode = -1;
                ubGuardBreak = ubHealingPotion = ubZeroSP = ubMPAttack = ubWoundDamage = ubHealthSteal = ubSPAttack = ubMoveChange = ubMagicChange = ubElementalShift = ubArmorShell = ubSharedHP = ubRemoveResistances = ubTPDamage = ubTrackHPChange = ubBodyDamage = ubVirageKilledPart = ubDragoonBond = ubDragoonExtras = ubCountdown = ubUltimateEnrage = ubInventoryRefresh = ubEnhancedShield = ubReverseDBS = false;
                SetUltimateStats();

                if (dmScripts.ContainsKey("btnRemoveCaps") && !dmScripts["btnRemoveCaps"]) {
                    this.Dispatcher.BeginInvoke(new Action(() => {
                        dmScripts["btnRemoveCaps"] = true;
                        TurnOnOffButton(ref btnRemoveCaps);
                    }), DispatcherPriority.ContextIdle);
                } else {
                    if (!dmScripts.ContainsKey("btnRemoveCaps")) {
                        this.Dispatcher.BeginInvoke(new Action(() => {
                            dmScripts.Add("btnRemoveCaps", true);
                            TurnOnOffButton(ref btnRemoveCaps);
                        }), DispatcherPriority.ContextIdle);
                    }
                }

                if (ubZeroSP) {
                    ZeroSPStart();
                }

                if (ubMPAttack) {
                    for (int i = 0; i < 3; i++) {
                        if (Globals.PARTY_SLOT[i] < 9) {
                            ubMPAttackTrk[i] = Globals.CHARACTER_TABLE[i].Read("HP");
                        }
                    }
                }

                if (ubWoundDamage) {
                    for (int i = 0; i < 3; i++) {
                        if (Globals.PARTY_SLOT[i] < 9) {
                            ubWHP[i] = Globals.CHARACTER_TABLE[i].Read("HP");
                            ubWMHP[i] = Globals.CHARACTER_TABLE[i].Read("Max_HP");
                        }
                    }
                }

                if (ubMoveChange) {
                    for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                        ubMoveChgTrn[i] = Globals.MONSTER_TABLE[i].Read("Turn");
                    }
                    ubMoveChgSet = false;
                }

                if (ubArmorShell) {
                    if (Globals.ENCOUNTER_ID == 422) {
                        ubHeartHPSave = Globals.MONSTER_TABLE[3].Read("HP");
                        ubArmorShellTurns = 0;
                    }
                }

                if (ubRemoveResistances) {
                    for (int i = 0; i < 3; i++) {
                        Globals.CHARACTER_TABLE[i].Write("Stat_Res", 0);
                    }
                }

                ultimateBossOnBattleEntry = true;
            } else {
                if (!Globals.IN_BATTLE && ultimateBossOnBattleEntry) {
                    ultimateBossOnBattleEntry = false;
                    this.Dispatcher.BeginInvoke(new Action(() => {
                        dmScripts["btnUltimateBoss"] = false;
                        TurnOnOffButton(ref btnUltimateBoss);
                    }), DispatcherPriority.ContextIdle);
                } else {
                    if (emulator.ReadShort(Constants.GetAddress("BATTLE_VALUE")) == 41215 && Globals.STATS_CHANGED) {
                        UltimateBossHP();

                        if (ubGuardBreak) {
                            if (Globals.ENCOUNTER_ID == 414) { //Urobolus
                                GuardBreak(0, 2); //Poison Spit 
                            }
                        }

                        if (ubHealingPotion) {
                            if (Globals.ENCOUNTER_ID == 412) { //Drake the Bandit
                                UltimateHealthPotion(0, 3, 3.33333); //Healing Potion
                            } else if (Globals.ENCOUNTER_ID == 416) { //Grand Jewel
                                UltimateHealthPotion(0, 1, 3.33333); //Heal Fountain
                            }
                        }

                        if (ubMPAttack) {
                            if (Globals.ENCOUNTER_ID == 415) { //Fire Bird
                                MPAttack(0, 3, 0, 231, 2); //Summon Volcano Balls
                            }
                        }

                        if (ubWoundDamage) {
                            if (Globals.ENCOUNTER_ID == 417) { //Ghost Commander
                                WoundDamage(0, 0); //Slash
                            }
                        }

                        if (ubHealthSteal) {
                            if (Globals.ENCOUNTER_ID == 417) { //Ghost Commander
                                HealthSteal(0, 2); //Life Drain
                            }
                        }

                        if (ubSPAttack) {
                            if (Globals.ENCOUNTER_ID == 418) { //Kamuy
                                if (ultimateHP[0] > 150000) {
                                    SPAttack(0, 1, 25, 231, 2); //Wind Blade
                                } else if (ultimateHP[0] > 75000) {
                                    SPAttack(0, 1, 50, 206, 2); //Wind Blade
                                } else {
                                    SPAttack(0, 1, 125, 156, 2); //Wind Blade
                                }
                            }
                        }

                        if (ubMoveChange) {
                            if (Globals.ENCOUNTER_ID == 418) { //Kamuy
                                MoveChange(0, 2, 25); //Purple Spear
                            }
                        }

                        if (ubMagicChange) {
                            if (Globals.ENCOUNTER_ID == 416) { //Grand Jewel
                                double hpDamage = ultimateMaxHP[0] - ultimateHP[0];
                                if ((hpDamage / ultimateMaxHP[0]) * 10 >= magicChangeTurns) {
                                    MagicChange();
                                }
                            } else if (Globals.ENCOUNTER_ID == 396) { //Lenus
                                double hpDamage = ultimateMaxHP[0] - ultimateHP[0];
                                if ((hpDamage / ultimateMaxHP[0]) * 20 >= magicChangeTurns) {
                                    MagicChange();
                                }
                            } else if (Globals.ENCOUNTER_ID == 390) { //Doel
                                if (ultimateHP[1] <= 75000) {
                                    emulator.WriteAOB(Globals.MONS_ADDRESS[1] - 0x6F8C, "C2");
                                    emulator.WriteAOB(Globals.MONS_ADDRESS[1] - 0x6F4C, "F8");
                                } else {
                                    if (magicChangeTurns > emulator.ReadShort(Globals.MONS_ADDRESS[1] + 0x44)) {
                                        if (emulator.ReadByte(Constants.GetAddress("DRAGOON_SPECIAL_ATTACK")) >= 0 && emulator.ReadByte(Constants.GetAddress("DRAGOON_SPECIAL_ATTACK") + 0x1) == 0) {
                                            if (emulator.ReadByte(Constants.GetAddress("DRAGOON_SPECIAL_ATTACK")) == 0) {
                                                emulator.WriteAOB(Globals.MONS_ADDRESS[1] - 0x6F8C, "C3");
                                                emulator.WriteAOB(Globals.MONS_ADDRESS[1] - 0x6F4C, "D1");
                                            } else if (emulator.ReadByte(Constants.GetAddress("DRAGOON_SPECIAL_ATTACK")) == 1 || emulator.ReadByte(Constants.GetAddress("DRAGOON_SPECIAL_ATTACK")) == 5) {
                                                emulator.WriteAOB(Globals.MONS_ADDRESS[1] - 0x6F8C, "C7");
                                                emulator.WriteAOB(Globals.MONS_ADDRESS[1] - 0x6F4C, "DC");
                                            } else if (emulator.ReadByte(Constants.GetAddress("DRAGOON_SPECIAL_ATTACK")) == 2 || emulator.ReadByte(Constants.GetAddress("DRAGOON_SPECIAL_ATTACK")) == 8) {
                                                emulator.WriteAOB(Globals.MONS_ADDRESS[1] - 0x6F8C, "C9");
                                                emulator.WriteAOB(Globals.MONS_ADDRESS[1] - 0x6F4C, "D2");
                                            } else if (emulator.ReadByte(Constants.GetAddress("DRAGOON_SPECIAL_ATTACK")) == 3) {
                                                emulator.WriteAOB(Globals.MONS_ADDRESS[1] - 0x6F8C, "CA");
                                                emulator.WriteAOB(Globals.MONS_ADDRESS[1] - 0x6F4C, "D8");
                                            } else if (emulator.ReadByte(Constants.GetAddress("DRAGOON_SPECIAL_ATTACK")) == 4) {
                                                emulator.WriteAOB(Globals.MONS_ADDRESS[1] - 0x6F8C, "C2");
                                                emulator.WriteAOB(Globals.MONS_ADDRESS[1] - 0x6F4C, "CF");
                                            } else if (emulator.ReadByte(Constants.GetAddress("DRAGOON_SPECIAL_ATTACK")) == 6) {
                                                emulator.WriteAOB(Globals.MONS_ADDRESS[1] - 0x6F8C, "C6");
                                                emulator.WriteAOB(Globals.MONS_ADDRESS[1] - 0x6F4C, "D6");
                                            } else if (emulator.ReadByte(Constants.GetAddress("DRAGOON_SPECIAL_ATTACK")) == 7) {
                                                emulator.WriteAOB(Globals.MONS_ADDRESS[1] - 0x6F8C, "C5");
                                                emulator.WriteAOB(Globals.MONS_ADDRESS[1] - 0x6F4C, "D0");
                                            }
                                        } else {
                                            emulator.WriteAOB(Globals.MONS_ADDRESS[1] - 0x6F8C, "C2");
                                            emulator.WriteAOB(Globals.MONS_ADDRESS[1] - 0x6F4C, "CF");
                                        }
                                    }

                                    magicChangeTurns = emulator.ReadShort(Globals.MONS_ADDRESS[1] + 0x44);
                                }
                            }
                        }

                        if (ubElementalShift) {
                            ElementalShift();
                        }

                        if (ubArmorShell) {
                            BreakArmor();
                        }

                        UltimateBossDefeatCheck();
                    }
                }
            }
        }

        public void SetUltimateStats() {
            for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                ultimateHP[i] = 0;
                ultimateMaxHP[i] = 0;

                double[] multiplyMode = { 1, 1, 1, 1, 1, 1 };

                if (Globals.MONSTER_IDS[i] == 131) { //Sandora Solider
                    multiplyMode[0] = 300;
                    multiplyMode[1] = 16;
                    multiplyMode[2] = 16;
                } else if (Globals.MONSTER_IDS[i] == 135) { //Commander II
                    multiplyMode[0] = 500;
                    multiplyMode[1] = 16;
                    multiplyMode[2] = 16;
                } else if (Globals.MONSTER_IDS[i] == 261) { //Fruegel
                    multiplyMode[0] = 700;
                    multiplyMode[1] = 40;
                    multiplyMode[2] = 40;
                } else if (Globals.MONSTER_IDS[i] == 259) { //Hellena Warden
                    multiplyMode[0] = 500;
                    multiplyMode[1] = 35;
                    multiplyMode[2] = 35;
                } else if (Globals.MONSTER_IDS[i] == 260) { //Senior Warden
                    multiplyMode[0] = 500;
                    multiplyMode[1] = 30;
                    multiplyMode[2] = 25;
                } else if (Globals.MONSTER_IDS[i] == 332) { //Urobolus
                    multiplyMode[0] = 220;
                    multiplyMode[1] = 22;
                    multiplyMode[2] = 22;
                    multiplyMode[3] = 2;
                    multiplyMode[4] = 2;
                    ubGuardBreak = true;
                } else if (Globals.MONSTER_IDS[i] == 102) { //Sandora Elite
                    ultimateHP[i] = ultimateMaxHP[i] = 159600;
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 7;
                    multiplyMode[2] = 7;
                } else if (Globals.MONSTER_IDS[i] == 325) { //Drake the Bandit
                    ultimateHP[i] = ultimateMaxHP[i] = 148000;
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 10;
                    multiplyMode[2] = 10;
                    ubHealingPotion = true;
                } else if (Globals.MONSTER_IDS[i] == 326) { //Wire
                    multiplyMode[0] = 80;
                    multiplyMode[1] = 7;
                    multiplyMode[2] = 7;
                } else if (Globals.MONSTER_IDS[i] == 327) { //Bursting Ball
                    multiplyMode[0] = 160;
                    multiplyMode[1] = 15;
                    multiplyMode[2] = 15;
                } else if (Globals.MONSTER_IDS[i] == 329) { //Jiango
                    ultimateHP[i] = ultimateMaxHP[i] = 204800;
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 8.5;
                    multiplyMode[2] = 8.5;
                    ubZeroSP = true;
                } else if (Globals.MONSTER_IDS[i] == 262) { //Fruegel
                    ultimateHP[i] = ultimateMaxHP[i] = 220000;
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 10;
                    multiplyMode[2] = 10;
                } else if (Globals.MONSTER_IDS[i] == 263) { //Rodriguez
                    multiplyMode[0] = 120;
                    multiplyMode[1] = 7.5;
                    multiplyMode[2] = 7.5;
                } else if (Globals.MONSTER_IDS[i] == 264) { //Gustaf
                    multiplyMode[0] = 120;
                    multiplyMode[1] = 7.5;
                    multiplyMode[2] = 7.5;
                } else if (Globals.MONSTER_IDS[i] == 333) { //Fire Bird
                    ultimateHP[i] = ultimateMaxHP[i] = 281600;
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 18.5;
                    multiplyMode[2] = 18.5;
                    ubGuardBreak = true;
                    ubZeroSP = true;
                    ubMPAttack = true;
                } else if (Globals.MONSTER_IDS[i] == 334) { //Volcano Ball
                    multiplyMode[0] = 2500;
                    multiplyMode[1] = 25;
                    multiplyMode[2] = 25;
                } else if (Globals.MONSTER_IDS[i] == 354 || Globals.MONSTER_IDS[i] == 385) { //GhostFB + Dragon Spirit (Feyrbrand)
                    ultimateHP[i] = ultimateMaxHP[i] = 320000;
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 2.5;
                    multiplyMode[2] = 2.5;
                } else if (Globals.MONSTER_IDS[i] == 299) { //Mappi
                    ultimateHP[i] = ultimateMaxHP[i] = 128000;
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 7;
                    multiplyMode[2] = 7;
                } else if (Globals.MONSTER_IDS[i] == 274) { //Crafty Thief
                    ultimateHP[i] = ultimateMaxHP[i] = 96000;
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 6;
                    multiplyMode[2] = 6;
                } else if (Globals.MONSTER_IDS[i] == 300) { //Mappi
                    ultimateHP[i] = ultimateMaxHP[i] = 128000;
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 6;
                    multiplyMode[2] = 6;
                } else if (Globals.MONSTER_IDS[i] == 301) { //Gehrich
                    ultimateHP[i] = ultimateMaxHP[i] = 200000;
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 6.5;
                    multiplyMode[2] = 6.5;
                    ubZeroSP = true;
                } else if (Globals.MONSTER_IDS[i] == 340) { //Ghost Commander
                    ultimateHP[i] = ultimateMaxHP[i] = 221000;
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 5;
                    multiplyMode[2] = 7;
                    ubWoundDamage = true;
                    ubHealthSteal = true;
                } else if (Globals.MONSTER_IDS[i] == 341) { //Ghost Knight
                    multiplyMode[0] = 0.065;
                    multiplyMode[1] = 3.5;
                    multiplyMode[2] = 3.5;
                    multiplyMode[3] = 0;
                    multiplyMode[4] = 0;
                } else if (Globals.MONSTER_IDS[i] == 343) { //Kamuy
                    ultimateHP[i] = ultimateMaxHP[i] = 300000;
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 4.5;
                    multiplyMode[2] = 4.5;
                    ubSPAttack = true;
                    ubMoveChange = true;
                } else if (Globals.MONSTER_IDS[i] == 353 || Globals.MONSTER_IDS[i] == 384) { //Ghost Regole + Dragon Spirit (Regole)
                    ultimateHP[i] = ultimateMaxHP[i] = 336000;
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 2.8;
                    multiplyMode[2] = 2.8;
                } else if (Globals.MONSTER_IDS[i] == 335) { //Grand Jewel
                    ultimateHP[i] = ultimateMaxHP[i] = 260000;
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 3.9;
                    multiplyMode[2] = 3.9;
                    ubMagicChange = true;
                    ubElementalShift = true;
                    ubHealingPotion = true;
                } else if (Globals.MONSTER_IDS[i] == 346) { //Windigo
                    ultimateHP[i] = ultimateMaxHP[i] = 700000;
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 4;
                    multiplyMode[2] = 4;
                    multiplyMode[3] = 0;
                    multiplyMode[4] = 0;
                    ubArmorShell = true;
                    ubZeroSP = true;
                } else if (Globals.MONSTER_IDS[i] == 347) { //Snow Cannon
                    multiplyMode[0] = 22;
                    multiplyMode[1] = 3;
                    multiplyMode[2] = 3;
                } else if (Globals.MONSTER_IDS[i] == 348) { //Heart
                    multiplyMode[0] = 333;
                } else if (Globals.MONSTER_IDS[i] == 349) { //Polter Helm
                    ultimateHP[i] = ultimateMaxHP[i] = 666666;
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 2.5;
                    multiplyMode[2] = 2.5;
                    ubSharedHP = true;
                } else if (Globals.MONSTER_IDS[i] == 350) { //Polter Armor
                    ultimateHP[i] = ultimateMaxHP[i] = 666666;
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 2.5;
                    multiplyMode[2] = 2.25;
                } else if (Globals.MONSTER_IDS[i] == 351) { //Polter Sword
                    ultimateHP[i] = ultimateMaxHP[i] = 666666;
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 3.5;
                    multiplyMode[2] = 2.5;
                } else if (Globals.MONSTER_IDS[i] == 365) { //The Last Kraken
                    ultimateHP[i] = ultimateMaxHP[i] = 360000;
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 2.7;
                    multiplyMode[2] = 2.7;
                    emulator.WriteByte(Globals.MONS_ADDRESS[0] + 0x175, 0);
                } else if (Globals.MONSTER_IDS[i] == 366) { //Cleone
                    ultimateHP[i] = ultimateMaxHP[i] = 360000;
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 2.8;
                    multiplyMode[2] = 2.8;
                } else if (Globals.MONSTER_IDS[i] == 360) { //Vector
                    ultimateHP[i] = ultimateMaxHP[i] = 180000;
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 2.7;
                    multiplyMode[2] = 2.7;
                } else if (Globals.MONSTER_IDS[i] == 361) { //Selebus
                    ultimateHP[i] = ultimateMaxHP[i] = 135000;
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 2.7;
                    multiplyMode[2] = 2.7;
                } else if (Globals.MONSTER_IDS[i] == 362) { //Kubila
                    ultimateHP[i] = ultimateMaxHP[i] = 157500;
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 2.7;
                    multiplyMode[2] = 2.7;
                } else if (Globals.MONSTER_IDS[i] == 369) { //Caterpillar
                    ultimateHP[i] = ultimateMaxHP[i] = 120000;
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 2.85;
                    multiplyMode[2] = 2.85;
                } else if (Globals.MONSTER_IDS[i] == 370) { //Pupa
                    ultimateHP[i] = ultimateMaxHP[i] = 180000;
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 0;
                    multiplyMode[2] = 0;
                    multiplyMode[3] = 0.2;
                    multiplyMode[4] = 0.2;
                } else if (Globals.MONSTER_IDS[i] == 371) { //Imago
                    ultimateHP[i] = ultimateMaxHP[i] = 240000;
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 2.85;
                    multiplyMode[2] = 2.85;
                } else if (Globals.MONSTER_IDS[i] == 363) { //Zackwell
                    ultimateHP[i] = ultimateMaxHP[i] = 360000;
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 2.5;
                    multiplyMode[2] = 2.5;
                } else if (Globals.MONSTER_IDS[i] == 352 || Globals.MONSTER_IDS[i] == 383) { //Divine Dragoon Ghost + Dragon Spirit (Divine Dragon)
                    ultimateHP[i] = ultimateMaxHP[i] = 400000;
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 2.35;
                    multiplyMode[2] = 2.35;
                } else if (Globals.MONSTER_IDS[i] == 363) { //Zackwell
                    ultimateHP[i] = ultimateMaxHP[i] = 360000;
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 2.5;
                    multiplyMode[2] = 2.5;
                } else if (Globals.MONSTER_IDS[i] == 308) { //Virage (head)
                    ultimateHP[i] = ultimateMaxHP[i] = 360000;
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 24;
                    multiplyMode[2] = 24;
                    ubRemoveResistances = true;
                } else if (Globals.MONSTER_IDS[i] == 309) { //Virage (body)
                    ultimateHP[i] = ultimateMaxHP[i] = 360000;
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 20;
                    multiplyMode[2] = 20;
                } else if (Globals.MONSTER_IDS[i] == 310) { //Virage (arm)
                    multiplyMode[0] = 1250;
                    multiplyMode[1] = 20;
                    multiplyMode[2] = 20;
                    multiplyMode[3] = 0.7;
                    multiplyMode[4] = 0.7;
                } else if (Globals.MONSTER_IDS[i] == 266) { //Kongol II
                    ultimateHP[i] = ultimateMaxHP[i] = 420000;
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 13;
                    multiplyMode[2] = 13;
                    multiplyMode[3] = 1.5;
                    multiplyMode[4] = 0.5;
                } else if (Globals.MONSTER_IDS[i] == 0) { //Lenus
                    ultimateHP[i] = ultimateMaxHP[i] = 525000;
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 6.6;
                    multiplyMode[2] = 6.6;
                    multiplyMode[3] = 0.8;
                    multiplyMode[4] = 0.8;
                    ubMagicChange = true;
                } else if (Globals.MONSTER_IDS[i] == 0) { //Syuveil
                    ultimateHP[i] = ultimateMaxHP[i] = 500000;
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 3.2;
                    multiplyMode[2] = 3.9;
                    ubTPDamage = true;
                    ubTrackHPChange = true;
                } else if (Globals.MONSTER_IDS[i] == 0) { //Syuveil
                    ultimateHP[i] = ultimateMaxHP[i] = 500000;
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 3.2;
                    multiplyMode[2] = 3.9;
                    ubTPDamage = true;
                    ubTrackHPChange = true;
                } else if (Globals.MONSTER_IDS[i] == 0) { //Virage(head)
                    ultimateHP[i] = ultimateMaxHP[i] = 1280000;
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 14;
                    multiplyMode[2] = 14;
                    ubBodyDamage = true;
                    ubTrackHPChange = true;
                    ubVirageKilledPart = true;
                } else if (Globals.MONSTER_IDS[i] == 0) { //Virage(body)
                    ultimateHP[i] = ultimateMaxHP[i] = 500000;
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 14;
                    multiplyMode[2] = 14;
                    multiplyMode[3] = 0.33;
                    multiplyMode[4] = 0.33;
                } else if (Globals.MONSTER_IDS[i] == 0) { //Virage(arm)
                    ultimateHP[i] = ultimateMaxHP[i] = 288000;
                    multiplyMode[0] = 180;
                    multiplyMode[1] = 14;
                    multiplyMode[2] = 14;
                    multiplyMode[3] = 0.25;
                    multiplyMode[4] = 0.25;
                } else if (Globals.MONSTER_IDS[i] == 0) { //Greham
                    ultimateHP[i] = ultimateMaxHP[i] = 210000;
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 20;
                    multiplyMode[2] = 20;
                    ubDragoonBond = true;
                    ubRemoveResistances = true;
                    ubDragoonBondMode = -1;
                } else if (Globals.MONSTER_IDS[i] == 0) { //Damia
                    ultimateHP[i] = ultimateMaxHP[i] = 360000;
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 3.1;
                    multiplyMode[2] = 3.1;
                    ubDragoonExtras = true;
                    ubTrackHPChange = true;
                } else if (Globals.MONSTER_IDS[i] == 0) { //Regole
                    ultimateHP[i] = ultimateMaxHP[i] = 300000;
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 6;
                    multiplyMode[2] = 6;
                } else if (Globals.MONSTER_IDS[i] == 0) { //Dragoon Lenus
                    ultimateHP[i] = ultimateMaxHP[i] = 300000;
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 6;
                    multiplyMode[2] = 6;
                    ubDragoonBond = true;
                    ubDragoonBondMode = -1;
                } else if (Globals.MONSTER_IDS[i] == 0) { //Belzac
                    ultimateHP[i] = ultimateMaxHP[i] = 608000;
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 4;
                    multiplyMode[2] = 4;
                    multiplyMode[3] = 0.25;
                    multiplyMode[4] = 0.25;
                    ubTrackDragoon = true;
                    ubTrackHPChange = true;
                } else if (Globals.MONSTER_IDS[i] == 0) { //S Virage(head)
                    ultimateHP[i] = ultimateMaxHP[i] = 320000;
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 6;
                    multiplyMode[2] = 15;
                    ubCountdown = true;
                } else if (Globals.MONSTER_IDS[i] == 0) { //S Virage(body)
                    ultimateHP[i] = ultimateMaxHP[i] = 320000;
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 6;
                    multiplyMode[2] = 6;
                } else if (Globals.MONSTER_IDS[i] == 0) { //S Virage(arm)
                    ultimateHP[i] = ultimateMaxHP[i] = 160000;
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 6;
                    multiplyMode[2] = 6;
                } else if (Globals.MONSTER_IDS[i] == 0) { //Kanzas
                    ultimateHP[i] = ultimateMaxHP[i] = 396000;
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 2.4;
                    multiplyMode[2] = 2.4;
                    ubTrackDragoon = true;
                    ubTrackHPChange = true;
                } else if (Globals.MONSTER_IDS[i] == 0) { //Emperor Doel
                    ultimateHP[i] = ultimateMaxHP[i] = 250000;
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 7;
                    multiplyMode[2] = 7;
                    multiplyMode[3] = 0.3;
                    multiplyMode[4] = 0.3;
                    ubZeroSP = true;
                    ubUltimateEnrage = true;
                } else if (Globals.MONSTER_IDS[i] == 0) { //Dragoon Doel
                    ultimateHP[i] = ultimateMaxHP[i] = 750000;
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 7;
                    multiplyMode[2] = 7;
                    multiplyMode[3] = 0.3;
                    multiplyMode[4] = 0.3;
                    ubInventoryRefresh = true;
                    ubMagicChange = true;
                    ubEnhancedShield = true;
                } else if (Globals.MONSTER_IDS[i] == 0) { //Divine Dragon
                    multiplyMode[0] = 0.25;
                    multiplyMode[1] = 6;
                    multiplyMode[2] = 6;
                    multiplyMode[3] = 100;
                    multiplyMode[4] = 200;
                    ubReverseDBS = true;
                } else if (Globals.MONSTER_IDS[i] == 0) { //Divine Cannon
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 0;
                    multiplyMode[2] = 0;
                    multiplyMode[3] = 0;
                    multiplyMode[4] = 0;
                } else if (Globals.MONSTER_IDS[i] == 0) { //Divine Ball
                    multiplyMode[0] = 65535;
                    multiplyMode[1] = 0;
                    multiplyMode[2] = 0;
                    multiplyMode[3] = 0;
                    multiplyMode[4] = 0;
                }

                Globals.MONSTER_TABLE[i].Write("HP", Math.Min(65535, Math.Round(Globals.MONSTER_TABLE[i].Read("HP") * multiplyMode[0])));
                Globals.MONSTER_TABLE[i].Write("Max_HP", Math.Min(65535, Math.Round(Globals.MONSTER_TABLE[i].Read("Max_HP") * multiplyMode[0])));
                Globals.MONSTER_TABLE[i].Write("AT", Math.Min(65535, Math.Round(Globals.MONSTER_TABLE[i].Read("AT") * multiplyMode[1])));
                Globals.MONSTER_TABLE[i].Write("MAT", Math.Min(65535, Math.Round(Globals.MONSTER_TABLE[i].Read("MAT") * multiplyMode[2])));
                Globals.MONSTER_TABLE[i].Write("DF", Math.Min(65535, Math.Round(Globals.MONSTER_TABLE[i].Read("DF") * multiplyMode[3])));
                Globals.MONSTER_TABLE[i].Write("MDF", Math.Min(65535, Math.Round(Globals.MONSTER_TABLE[i].Read("MDF") * multiplyMode[4])));
                Globals.MONSTER_TABLE[i].Write("SPD", Math.Min(65535, Math.Round(Globals.MONSTER_TABLE[i].Read("SPD") * multiplyMode[5])));
            }

            if (Globals.ENCOUNTER_ID == 487) { //Commander II
                emulator.WriteByte(Constants.GetAddress("MONSTER_REWARDS_CHANCE") + 0x1A8, 100);
                emulator.WriteByte(Constants.GetAddress("MONSTER_REWARDS_DROP") + 0x1A8, 158); //Sabre
            }
        }

        public void UltimateBossDefeatCheck() {
            if (emulator.ReadShort(Constants.GetAddress("BATTLE_VALUE")) == 41215) {
                switch (Globals.ENCOUNTER_ID) {
                    case 461: //Sandora Elite
                    case 415: //Fire Bird
                    case 386: //Fruegel I
                    case 412: //Drake the Bandit
                    case 387: //Fruegel II
                    case 402: //Mappi
                    case 418: //Kamuy
                    case 422: //Windigo
                    case 432: //Last Kraken
                    case 431: //Zackwell
                    case 409: //Virage II
                    case 394: //Divine Dragon
                        if (Globals.MONSTER_TABLE[0].Read("HP") == 0 || ultimateHP[0] == 0) {
                            UltimateBossDefeated();
                        }
                        break;
                    case 487: //Commander II
                    case 449: //Feyrbrand (Spirit)
                    case 448: //Regole (Spirit)
                    case 447: //Divine Dragon (Spirit)
                        if (Globals.MONSTER_TABLE[1].Read("HP") == 0 || ultimateHP[1] == 0) {
                            UltimateBossDefeated();
                        }
                        break;
                    default:
                        byte defeatedEnemies = 0;
                        for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                            if (Globals.MONSTER_TABLE[i].Read("HP") == 0 || ultimateHP[i] == 0) {
                                defeatedEnemies += 1;
                            }
                        }

                        if (defeatedEnemies == Globals.MONSTER_SIZE) {
                            UltimateBossDefeated();
                        }
                        break;
                }
            }
        }

        public void UltimateBossDefeated() {
            this.Dispatcher.BeginInvoke(new Action(() => {
                dmScripts["btnUltimateBoss"] = false;
                TurnOnOffButton(ref btnUltimateBoss);
                ultimateBossOnBattleEntry = false;
                ubHPChanged = ubCheckedDamage = ubUltimateHPSet = false;
                ubCheckDamageCycle = 0;
                ubDragoonBondMode = -1;
                ubGuardBreak = ubHealingPotion = ubZeroSP = ubMPAttack = ubWoundDamage = ubHealthSteal = ubSPAttack = ubMoveChange = ubMagicChange = ubElementalShift = ubArmorShell = ubSharedHP = ubRemoveResistances = ubTPDamage = ubTrackHPChange = ubBodyDamage = ubVirageKilledPart = ubDragoonBond = ubDragoonExtras = ubCountdown = ubUltimateEnrage = ubInventoryRefresh = ubEnhancedShield = ubReverseDBS = false;

                if (ultimateBossCompleted == uiCombo["cboUltimateBoss"]) {
                    ultimateBossCompleted += 1;
                }

                if (ultimateBossCompleted == 3) {
                    inventorySize = 36;
                    ExtendInventory();
                    Constants.WritePLog("On how to extend inventory please see the output log in the settings tab.");
                    Constants.WriteGLogOutput("Ultimate Boss Zone 1 completed! Inventory expanded to 36 slots.");
                    Constants.WriteOutput("Please note Extended Inventory must be applied at the Save Slot screen once per emulator session to avoid loss of items. To do this simply open up Dragoon Modifier right before you load your save.");
                } else if (ultimateBossCompleted == 8) {
                    inventorySize = 40;
                    ExtendInventory();
                    Constants.WritePLog("On how to extend inventory please see the output log in the settings tab.");
                    Constants.WriteGLogOutput("Ultimate Boss Zone 2 completed! Inventory expanded to 40 slots.");
                    Constants.WriteOutput("Please note Extended Inventory must be applied at the Save Slot screen once per emulator session to avoid loss of items. To do this simply open up Dragoon Modifier right before you load your save.");
                } else {
                    Constants.WriteGLogOutput("Ultimate Boss defeated.");
                }

                SaveSubKey();
            }), DispatcherPriority.ContextIdle);
        }
        #endregion

        #region Extras
        public void UltimateBossHP() {
            bool partyAttacking = false;
            bool guardCheck = false;
            int guardSlot = 0;
            bool dragoonSpecialCheck = false;
            int totalDamage = 0;
            for (int i = 0; i < 3; i++) {
                if (Globals.PARTY_SLOT[i] < 9) {
                    byte action = Globals.CHARACTER_TABLE[i].Read("Action");
                    if (action == 24 || action == 26 || action == 136 || action == 138) {
                        partyAttacking = true;
                    }
                    if (action == 136) {
                        guardCheck = true;
                        guardSlot = i;
                    }
                    if (action == 24) {
                        dragoonSpecialCheck = true;
                    }
                }
            }

            if (guardCheck) {
                if (Globals.PARTY_SLOT[guardSlot] != 2 && Globals.PARTY_SLOT[guardSlot] != 8) {
                    for (int i = 0; i < 3; i++) {
                        if (Globals.PARTY_SLOT[i] < 9) {
                            byte action = Globals.CHARACTER_TABLE[i].Read("Action");
                            if (action == 0 || action == 2) {
                                partyAttacking = false;
                            }
                        }
                    }
                }
            }

            if (dragoonSpecialCheck) {
                for (int i = 0; i < 3; i++) {
                    if (Globals.PARTY_SLOT[i] < 9) {
                        byte action = Globals.CHARACTER_TABLE[i].Read("Action");
                        if (action == 18 || action == 19) {
                            partyAttacking = false;
                        }
                    }
                }
            }

            if (partyAttacking && !ubUltimateHPSet) {
                for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                    if (ultimateHP[i] > 0) {
                        Globals.MONSTER_TABLE[i].Write("HP", 65535);
                    }
                }
                ubUltimateHPSet = true;
                ubCheckedDamage = false;
            }

            for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                if (ultimateHP[i] > 0) {
                    if (partyAttacking) {
                        if (!ubCheckedDamage) {
                            ushort currentHP = Globals.MONSTER_TABLE[i].Read("HP");
                            if (ubSharedHP) {
                                if (currentHP < 65535) {
                                    totalDamage += 65535 - currentHP;
                                    ubHPChanged = true;
                                }
                                if ((i + 1) == Globals.MONSTER_SIZE) {
                                    for (int x = 0; x < Globals.MONSTER_SIZE; x++) {
                                        ultimateHP[x] -= totalDamage;
                                        if (ultimateHP[x] < 0) {
                                            ultimateHP[x] = 0;
                                            Globals.MONSTER_TABLE[x].Write("HP", 0);
                                        }
                                    }
                                }
                            } else {
                                if (currentHP < 65535) {
                                    ultimateHP[i] -= 65535 - currentHP;
                                    ubHPChanged = true;
                                }
                                if (ultimateHP[i] < 0) {
                                    ultimateHP[i] = 0;
                                    Globals.MONSTER_TABLE[i].Write("HP", 0);
                                }
                            }
                        }
                        ubCheckDamageCycle = ubCheckedDamage ? 0 : 2;
                    } else {
                        if (ubCheckDamageCycle > 0) {
                            ushort currentHP = Globals.MONSTER_TABLE[i].Read("HP");
                            if (ubSharedHP) {
                                if (currentHP < 65535) {
                                    totalDamage += 65535 - currentHP;
                                    ubHPChanged = true;
                                    ubCheckDamageCycle = 0;
                                }
                                if ((i + 1) == Globals.MONSTER_SIZE) {
                                    for (int x = 0; x < Globals.MONSTER_SIZE; x++) {
                                        ultimateHP[x] -= totalDamage;
                                        Globals.MONSTER_TABLE[i].Write("HP", (ushort) Math.Round(((double) ultimateHP[i] / ultimateMaxHP[i]) * 65535));
                                        if (ultimateHP[x] < 0) {
                                            ultimateHP[x] = 0;
                                            Globals.MONSTER_TABLE[x].Write("HP", 0);
                                        }
                                    }
                                }
                            } else {
                                if (currentHP < 65535) {
                                    ultimateHP[i] -= 65535 - currentHP;
                                    Globals.MONSTER_TABLE[i].Write("HP", (ushort) Math.Round(((double) ultimateHP[i] / ultimateMaxHP[i]) * 65535));
                                    ubHPChanged = true;
                                    ubCheckDamageCycle = 0;
                                }
                                if (ultimateHP[i] < 0) {
                                    ultimateHP[i] = 0;
                                    Globals.MONSTER_TABLE[i].Write("HP", 0);
                                }
                            }

                            if ((i + 1) == Globals.MONSTER_SIZE) 
                                ubCheckDamageCycle--;
                        } else {
                            ubCheckedDamage = false;
                            ubUltimateHPSet = false;
                            ubHPChanged = false;
                            ushort hpAmt = (ushort) Math.Round(((double) ultimateHP[i] / ultimateMaxHP[i]) * 65535);
                            ushort currentHP = Globals.MONSTER_TABLE[i].Read("HP");
                            //Constants.WriteDebug("HP%: " + hpAmt + " / " + currentHP);
                            if (hpAmt != currentHP) {
                                Globals.MONSTER_TABLE[i].Write("HP", hpAmt);
                            }
                        }
                    }
                }
            }

            if (ubHPChanged) {
                ubCheckedDamage = true;
            }

            Constants.WriteDebug("HP[0]: " + ultimateHP[0] + "/" + Globals.MONSTER_TABLE[0].Read("HP") + " | P ATK: " + partyAttacking + "/" + ubCheckDamageCycle + " | CHK DMG: " + ubCheckedDamage + " | HP CHG: " + ubHPChanged +  " | SET: " + ubUltimateHPSet + " | ACT: " + Globals.CHARACTER_TABLE[0].Read("Action") + "/" + Globals.CHARACTER_TABLE[1].Read("Action") + "/" + Globals.CHARACTER_TABLE[2].Read("Action"));

            /*if (!partyAttacking && ubCheckDamageCycle > 0 && ubHPChanged) {
                if (dmScripts.ContainsKey("btnDamageTracker") && dmScripts["btnDamageTracker"]) {
                    DamageTracker();
                }
                ubCheckedDamage = false;
                ubHPChanged = false;
                ubCheckDamageCycle = 0;
                //Constants.WriteDebug("Failed party attack check.");
            }*/

            //Constants.WritePLog("Attack Move: " + Globals.MONSTER_TABLE[0].Read("Attack_Move"));
        }

        public void GuardBreak(int monsterSlot, byte attack) {
            //Constants.WritePLog("Attack Move: " + Globals.MONSTER_TABLE[monsterSlot].Read("Attack_Move") + "/" + attack);
            if (Globals.MONSTER_TABLE[monsterSlot].Read("Attack_Move") == attack) {
                //Constants.WriteDebug("Attack Move: " + Globals.MONSTER_TABLE[monsterSlot].Read("Attack_Move") + "/" + attack);
                for (int i = 0; i < 3; i++) {
                    if (Globals.PARTY_SLOT[i] < 9) {
                        Globals.CHARACTER_TABLE[i].Write("Guard", 0);
                    }
                }
                Thread.Sleep(1500); //it'll soft lock the game otherwise, game needs time to load the move
                Globals.MONSTER_TABLE[monsterSlot].Write("Attack_Move", 255);
            }
        }

        public void UltimateHealthPotion(int monsterSlot, byte attack, double heal) {
            if (Globals.MONSTER_TABLE[monsterSlot].Read("Attack_Move") == attack) {
                ultimateHP[0] += (int) Math.Round(ultimateMaxHP[0] / heal);
                Thread.Sleep(1500);
                Globals.MONSTER_TABLE[monsterSlot].Write("Attack_Move", 255);
            }
        }

        public void ZeroSPStart() {
            for (int i = 0; i < 3; i++) {
                if (Globals.PARTY_SLOT[i] < 9) {
                    Globals.CHARACTER_TABLE[i].Write("SP", 0);
                }
            }
        }

        public void MPAttack(int monsterSlot, byte attack, byte mpAmount, byte mpOnHit, byte turns) {
            //Constants.WritePLog("Attack Move: " + Globals.MONSTER_TABLE[monsterSlot].Read("Attack_Move") + "/" + attack);
            if (Globals.MONSTER_TABLE[monsterSlot].Read("Attack_Move") == attack) {
                for (int i = 0; i < 3; i++) {
                    if (Globals.PARTY_SLOT[i] < 9) {
                        if (mpOnHit > 0) {
                            Globals.CHARACTER_TABLE[i].Write("MP_ONHIT_PHYSICAL", mpOnHit);
                            Globals.CHARACTER_TABLE[i].Write("MP_ONHIT_MAGIC", mpOnHit);
                            Globals.CHARACTER_TABLE[i].Write("MP_ONHIT_PHYSICAL_TRN", turns);
                            Globals.CHARACTER_TABLE[i].Write("MP_ONHIT_MAGIC_TRN", turns);
                        }
                        int mp = Globals.CHARACTER_TABLE[i].Read("MP") - mpAmount;
                        Globals.CHARACTER_TABLE[i].Write("MP", mpAmount > 0 ? mpAmount : 0);
                    }
                }
                Thread.Sleep(2000); 
                Globals.MONSTER_TABLE[monsterSlot].Write("Attack_Move", 255);
            }
        }

        public void WoundDamage(int monsterSlot, byte attack) {
            for (int i = 0; i < 3; i++) {
                if (Globals.PARTY_SLOT[i] < 9) {
                    ushort hp = Globals.CHARACTER_TABLE[i].Read("HP");
                    if (hp < ubWHP[i] && Globals.MONSTER_TABLE[i].Read("Attack_Move") == attack) {
                        ushort woundDamage = (ushort) (ubWHP[i] - hp);
                        Globals.CHARACTER_TABLE[i].Write("Max_HP", Math.Max(0, Globals.CHARACTER_TABLE[i].Read("Max_HP") - woundDamage));
                    }
                    ubWHP[i] = hp;
                    if (Globals.CHARACTER_TABLE[i].Read("Action") == 192) {
                        Globals.CHARACTER_TABLE[i].Write("Max_HP", ubWMHP[i]);
                    }
                }
            }
        }

        public void HealthSteal(int monsterSlot, byte attack) {
            if (Globals.MONSTER_TABLE[monsterSlot].Read("Attack_Move") == attack) {
                if (ubHealthStealSave) {
                    ushort dmg  = emulator.ReadShort(Constants.GetAddress("DAMAGE_SLOT1"));
                    if (dmg != 65534 && dmg != ubHealthStealDamage) {
                        ubHealthStealDamage = dmg;
                        ultimateHP[monsterSlot] += dmg;
                        ubHealthStealSave = false;
                    }
                } else {
                    ubHealthStealDamage = emulator.ReadShort(Constants.GetAddress("DAMAGE_SLOT1"));
                    ubHealthStealSave = true;
                }
            } else {
                ubHealthStealSave = false;
            }
        }

        public void SPAttack(int monsterSlot, byte attack, byte spAmount, byte spOnHit, byte turns) {
            if (Globals.MONSTER_TABLE[monsterSlot].Read("Attack_Move") == attack) {
                for (int i = 0; i < 3; i++) {
                    if (Globals.PARTY_SLOT[i] < 9) {
                        if (spOnHit > 0) {
                            Globals.CHARACTER_TABLE[i].Write("SP_ONHIT_PHYSICAL", spOnHit);
                            Globals.CHARACTER_TABLE[i].Write("SP_ONHIT_MAGIC", spOnHit);
                            Globals.CHARACTER_TABLE[i].Write("SP_ONHIT_PHYSICAL_TRN", turns);
                            Globals.CHARACTER_TABLE[i].Write("SP_ONHIT_MAGIC_TRN", turns);
                        }
                        if (emulator.ReadByte(Constants.GetAddress("DRAGOON_TURNS") + (i * 0x4)) == 0) {
                            int sp = Globals.CHARACTER_TABLE[i].Read("SP") - spAmount;
                            Globals.CHARACTER_TABLE[i].Write("SP", sp > 0 ? sp : 0);
                        }
                    }
                }
                Thread.Sleep(2000);
                Globals.MONSTER_TABLE[monsterSlot].Write("Attack_Move", 255);
            }
        }

        public void MoveChange(int monsterSlot, byte attack, int chance) { //Some moves it won't overwrite but that's fine.
            ushort trn = Globals.MONSTER_TABLE[monsterSlot].Read("Turn");
            if (ubMoveChgSet) {
                bool partyAttacking = false;
                for (int i = 0; i < 3; i++) {
                    if (Globals.PARTY_SLOT[i] < 9) {
                        byte action = Globals.CHARACTER_TABLE[i].Read("Action");
                        if (action == 8 || action == 10 || action == 24 || action == 26 || action == 136 || action == 138) {
                            partyAttacking = true;
                        }
                    } 
                }

                if (!partyAttacking) {
                    Globals.MONSTER_TABLE[monsterSlot].Write("Attack_Move", attack);
                    //Constants.WriteDebug("[MOVE] " + Globals.MONSTER_TABLE[monsterSlot].Read("Attack_Move"));
                } else {
                    ubMoveChgSet = false;
                    if (Globals.ENCOUNTER_ID == 418) {
                        Globals.MONSTER_TABLE[monsterSlot].Write("AT", originalMonsterStats[monsterSlot, 1]);
                    }
                }
            } else {
                if (trn < ubMoveChgTrn[monsterSlot]) {
                    if (new Random().Next(0, 100) < chance) {
                        ubMoveChgSet = true;
                        if (Globals.ENCOUNTER_ID == 418) {
                            Globals.MONSTER_TABLE[monsterSlot].Write("AT", Math.Round(originalMonsterStats[monsterSlot, 1] * 1.75));
                        }
                        //Constants.WriteDebug("[MOVE CHANGE] Roll success.");
                    } else {
                        //Constants.WriteDebug("Roll failed.");
                    }
                }
            }
            ubMoveChgTrn[monsterSlot] = trn;
        }

        public void MagicChange() {
            ArrayList singleMagic = new ArrayList();
            ArrayList wideMagic = new ArrayList();
            ArrayList powerMagic = new ArrayList();
            int index = 0;

            singleMagic.Add(0xC3);
            singleMagic.Add(0xC6);
            singleMagic.Add(0xC7);
            singleMagic.Add(0xC5);
            singleMagic.Add(0xCA);
            singleMagic.Add(0xC9);
            singleMagic.Add(0xC2);
            wideMagic.Add(0xD1);
            wideMagic.Add(0xD6);
            wideMagic.Add(0xDC);
            wideMagic.Add(0xD0);
            wideMagic.Add(0xD8);
            wideMagic.Add(0xD2);
            wideMagic.Add(0xCF);
            powerMagic.Add(0xF2);
            powerMagic.Add(0xF3);
            powerMagic.Add(0xF4);
            powerMagic.Add(0xF5);
            powerMagic.Add(0xF7);
            powerMagic.Add(0xF6);
            powerMagic.Add(0xF8);

            if (Globals.ENCOUNTER_ID == 416) {
                index = new Random().Next(0, singleMagic.Count);
                emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0x102C, Convert.ToByte(singleMagic[index]));
                singleMagic.RemoveAt(index);
                index = new Random().Next(0, singleMagic.Count);
                emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0xFEC, Convert.ToByte(singleMagic[index]));
                singleMagic.RemoveAt(index);
                index = new Random().Next(0, singleMagic.Count);
                emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0xFAC, Convert.ToByte(singleMagic[index]));
                singleMagic.RemoveAt(index);


                index = new Random().Next(0, wideMagic.Count);
                emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0xF6C, Convert.ToByte(wideMagic[index]));
                wideMagic.RemoveAt(index);
                index = new Random().Next(0, wideMagic.Count);
                emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0xF2C, Convert.ToByte(wideMagic[index]));
                wideMagic.RemoveAt(index);


                index = new Random().Next(0, powerMagic.Count);
                emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0xEEC, Convert.ToByte(powerMagic[index]));
                powerMagic.RemoveAt(index);
            } else if (Globals.ENCOUNTER_ID == 396) {
                index = new Random().Next(0, singleMagic.Count);
                emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0xC1C, Convert.ToByte(singleMagic[index]));
                singleMagic.RemoveAt(index);
                index = new Random().Next(0, singleMagic.Count);
                emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0xAF0, Convert.ToByte(singleMagic[index]));
                singleMagic.RemoveAt(index);

                index = new Random().Next(0, wideMagic.Count);
                emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0xBB8, Convert.ToByte(wideMagic[index]));
                wideMagic.RemoveAt(index);
                index = new Random().Next(0, wideMagic.Count);
                emulator.WriteByte(Globals.MONS_ADDRESS[0] - 0xB54, Convert.ToByte(wideMagic[index]));
                wideMagic.RemoveAt(index);
            }

            magicChangeTurns += 1;
        }

        public void ElementalShift() {
            int lastItem = emulator.ReadByte(Globals.MONS_ADDRESS[0] + 0xABC);
            if (lastItem == 0xC0 || lastItem == 0xC3 || lastItem == 0xD1 || lastItem == 0xF2) {
                Globals.MONSTER_TABLE[0].Write("Element", 128);
                Globals.MONSTER_TABLE[0].Write("Display_Element", 128);
            } else if (lastItem == 0xC6 || lastItem == 0xD6 || lastItem == 0xF3) {
                Globals.MONSTER_TABLE[0].Write("Element", 1);
                Globals.MONSTER_TABLE[0].Write("Display_Element", 1);
            } else if (lastItem == 0xC7 || lastItem == 0xDC || lastItem == 0xF4) {
                Globals.MONSTER_TABLE[0].Write("Element", 64);
                Globals.MONSTER_TABLE[0].Write("Display_Element", 64);
            } else if (lastItem == 0xC5 || lastItem == 0xD0 || lastItem == 0xF5) {
                Globals.MONSTER_TABLE[0].Write("Element", 2);
                Globals.MONSTER_TABLE[0].Write("Display_Element", 2);
            } else if (lastItem == 0xCA || lastItem == 0xD8 || lastItem == 0xF7) {
                Globals.MONSTER_TABLE[0].Write("Element", 4);
                Globals.MONSTER_TABLE[0].Write("Display_Element", 4);
            } else if (lastItem == 0xC9 || lastItem == 0xD2 || lastItem == 0xF6) {
                Globals.MONSTER_TABLE[0].Write("Element", 32);
                Globals.MONSTER_TABLE[0].Write("Display_Element", 32);
            } else if (lastItem == 0xC2 || lastItem == 0xCF || lastItem == 0xF8) {
                Globals.MONSTER_TABLE[0].Write("Element", 16);
                Globals.MONSTER_TABLE[0].Write("Display_Element", 16);
            } else if (lastItem == 0xC1 || lastItem == 0xF1) {
                Globals.MONSTER_TABLE[0].Write("Element", 0);
                Globals.MONSTER_TABLE[0].Write("Display_Element", 0);
            }
        }

        public void BreakArmor() {
            if (Globals.MONSTER_TABLE[3].Read("HP") != ubHeartHPSave) {
                ubHeartHPSave = Globals.MONSTER_TABLE[3].Read("HP");
                ubArmorShellTurns = 1;
                ubArmorShellTP = Globals.MONSTER_TABLE[0].Read("Turn");
            }

            if (ubArmorShellTurns >= 1) {
                Globals.MONSTER_TABLE[0].Write("DF", 30);
                Globals.MONSTER_TABLE[0].Write("MDF", 36);
                if (Globals.MONSTER_TABLE[0].Read("Turn") >= ubArmorShellTP) {
                    ubArmorShellTP = Globals.MONSTER_TABLE[0].Read("Turn");
                } else {
                    ubArmorShellTurns += 1;
                    ubArmorShellTP = Globals.MONSTER_TABLE[0].Read("Turn");
                }
            }

            if (ubArmorShellTurns > 3) {
                Globals.MONSTER_TABLE[0].Write("DF", 0);
                Globals.MONSTER_TABLE[0].Write("MDF", 0);
                ubArmorShellTurns = 0;
            }
        }
        #endregion

        #region Extend Inventory
        public void ExtendInventory() {
            if (Constants.REGION == Region.USA) { //Temp
                if (Globals.IN_BATTLE) {
                    emulator.WriteShort(0x231F8, 64);
                    emulator.WriteShort(0x23308, 64);
                    emulator.WriteShort(0x233B8, 64);
                    emulator.WriteShort(0x23500, 64);
                    emulator.WriteShort(0x23324, 63);
                    emulator.WriteShort(0x2334C, 63);
                    emulator.WriteShort(0x23234, 65);
                    emulator.WriteShort(0x23250, 65);
                    if (emulator.ReadByte(0x10B0C8) == 32) {
                        emulator.WriteShort(0x10B0C8, 64);
                    }
                    if (emulator.ReadByte(0x10C3D4) == 32) {
                        emulator.WriteShort(0x10C3D4, 64);
                    }
                    if (emulator.ReadByte(0x102970) == 32) {
                        emulator.WriteShort(0x102970, 64);
                    }
                    emulator.WriteShort(0x2336C, 808);
                    //writeByte(0xBADAE, 64);
                } else {
                    if (emulator.ReadByte(0xBDC30) == 19) {
                        emulator.WriteShort(0x231F8, 64);
                        emulator.WriteShort(0x23308, 64);
                        emulator.WriteShort(0x233B8, 64);
                        emulator.WriteShort(0x23500, 64);
                        emulator.WriteShort(0x23324, 63);
                        emulator.WriteShort(0x2334C, 63);
                        emulator.WriteShort(0x23234, 65);
                        emulator.WriteShort(0x23250, 65);
                        emulator.WriteShort(0x10B0C8, 64);
                        emulator.WriteShort(0x10C3D4, (ushort) inventorySize);
                        emulator.WriteShort(0x102970, (ushort) inventorySize);
                        emulator.WriteShort(0x2336C, 808);
                    } else {
                        emulator.WriteShort(0x231F8, (ushort) inventorySize);
                        emulator.WriteShort(0x23308, (ushort) inventorySize);
                        emulator.WriteShort(0x233B8, (ushort) inventorySize);
                        emulator.WriteShort(0x23500, (ushort) inventorySize);

                        emulator.WriteShort(0x23324, (ushort) (inventorySize - 1));
                        emulator.WriteShort(0x2334C, (ushort) (inventorySize - 1));

                        emulator.WriteShort(0x23234, (ushort) (inventorySize + 1));
                        emulator.WriteShort(0x23250, (ushort) (inventorySize + 1));
                        //if (emulator.ReadByte(0x10B0C8) == 32) {
                        emulator.WriteShort(0x10B0C8, (ushort) inventorySize);
                        //}
                        //if (emulator.ReadByte(0x10C3D4) == 32) {
                        emulator.WriteShort(0x10C3D4, (ushort) inventorySize);
                        //}
                        //if (emulator.ReadByte(0x102970) == 32) {
                        emulator.WriteShort(0x102970, (ushort) inventorySize);
                        //}
                        emulator.WriteShort(0x2336C, 808);
                    }
                }
            }
        }
        #endregion
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
            if (emulator.ReadShort(Constants.GetAddress("BATTLE_VALUE")) == 41215 && Globals.STATS_CHANGED) {
                for (int i = 0; i < 3; i++) {
                    if (Globals.PARTY_SLOT[i] < 9) {
                        Globals.CHARACTER_TABLE[i].Write("Guard", 0);
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
                    recoveryRateSave = Globals.CHARACTER_TABLE[i].Read("HP_Regen");
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
                    checkRoseDamage = checkFlowerStorm = burnActive = false;
                    dartBurnStack = 0;
                } else {
                    if (emulator.ReadShort(Constants.GetAddress("BATTLE_VALUE")) == 41215) {
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

                            if (Globals.PARTY_SLOT[i] == 3 && Globals.CHARACTER_TABLE[i].Read("Weapon") == 162) {
                                multi *= 1.1;
                            }

                            currentMP[i] = Globals.CHARACTER_TABLE[i].Read("MP");

                            if (Globals.PARTY_SLOT[i] == 0) { //Dart
                                if (dragoonSpecialAttack == 0 || dragoonSpecialAttack == 9) {
                                    if (Globals.DRAGOON_SPIRITS >= 254) {
                                        Globals.CHARACTER_TABLE[i].Write("DAT", (306 * multi));
                                    } else {
                                        if (dmScripts.ContainsKey("btnDivineRed") && dmScripts["btnDivineRed"]) {
                                            Globals.CHARACTER_TABLE[i].Write("DAT", (612 * multi));
                                        } else {
                                            Globals.CHARACTER_TABLE[i].Write("DAT", (422 * multi));
                                        }
                                    }
                                } else {
                                    if (Globals.DRAGOON_SPIRITS >= 254) {
                                        emulator.WriteByte(Globals.CHAR_ADDRESS[i] + 0xA5, 0);
                                        Globals.CHARACTER_TABLE[i].Write("DAT", (204 * multi));
                                    } else {
                                        if (dmScripts.ContainsKey("btnDivineRed") && dmScripts["btnDivineRed"]) {
                                            Globals.CHARACTER_TABLE[i].Write("DAT", (408 * multi));
                                        } else {
                                            Globals.CHARACTER_TABLE[i].Write("DAT", (281 * multi));
                                        }
                                    }
                                }

                                if (multi == 1) {
                                    emulator.WriteByte(Globals.CHAR_ADDRESS[i] + 0xA9, 0);
                                    emulator.WriteByte(Globals.CHAR_ADDRESS[i] + 0xAB, 0);
                                    Globals.CHARACTER_TABLE[i].Write("DDF", (180 * multi));
                                    Globals.CHARACTER_TABLE[i].Write("DMDF", (180 * multi));
                                } else {
                                    Globals.CHARACTER_TABLE[i].Write("DDF", (180 * multi));
                                    Globals.CHARACTER_TABLE[i].Write("DMDF", (180 * multi));
                                }

                                if (soasSiphonSlot == i) { //Soa's Siphon Ring
                                    multi *= 0.3;
                                }

                                if (currentMP[i] != previousMP[i] && currentMP[i] < previousMP[i]) {
                                    mp = previousMP[i] - currentMP[i];
                                    if (dmScripts.ContainsKey("btnDivineRed") && dmScripts["btnDivineRed"]) {
                                        if (Globals.CHARACTER_TABLE[i].Read("Spell_Cast") == 1) {
                                            Globals.CHARACTER_TABLE[i].Write("DMAT", (1020 * multi));
                                        } else {
                                            Globals.CHARACTER_TABLE[i].Write("DMAT", (510 * multi));
                                        }
                                    } else {
                                        if (mp == 10) {
                                            if (multi == 1) {
                                                emulator.WriteByte(Globals.CHAR_ADDRESS[i] + 0xA7, 0);
                                                Globals.CHARACTER_TABLE[i].Write("DMAT", (255 * multi));
                                            } else {
                                                Globals.CHARACTER_TABLE[i].Write("DMAT", (255 * multi));
                                            }
                                            AddBurnStack(1);
                                        } else if (mp == 20) {
                                            Globals.CHARACTER_TABLE[i].Write("DMAT", (340 * multi));
                                            AddBurnStack(1);
                                        } else if (mp == 30) {
                                            if (multi == 1) {
                                                emulator.WriteByte(Globals.CHAR_ADDRESS[i] + 0xA7, 0);
                                                Globals.CHARACTER_TABLE[i].Write("DMAT", (255 * multi));
                                            } else {
                                                Globals.CHARACTER_TABLE[i].Write("DMAT", (255 * multi));
                                            }
                                            AddBurnStack(2);
                                        } else if (mp == 50) {
                                            if (multi == 1) {
                                                emulator.WriteByte(Globals.CHAR_ADDRESS[i] + 0xA7, 0);
                                                Globals.CHARACTER_TABLE[i].Write("DMAT", (255 * multi));
                                            } else {
                                                Globals.CHARACTER_TABLE[i].Write("DMAT", (255 * multi));
                                            }
                                        } else if (mp == 80) {
                                            Globals.CHARACTER_TABLE[i].Write("DMAT", (340 * multi));
                                            AddBurnStack(3);
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
                                    Globals.CHARACTER_TABLE[i].Write("DDF", (180 * multi));
                                    Globals.CHARACTER_TABLE[i].Write("DMDF", (180 * multi));
                                } else {
                                    Globals.CHARACTER_TABLE[i].Write("DDF", (180 * multi));
                                    Globals.CHARACTER_TABLE[i].Write("DMDF", (180 * multi));
                                }

                                if (dragoonSpecialAttack == 1 || dragoonSpecialAttack == 5) {
                                    Globals.CHARACTER_TABLE[i].Write("DAT", (495 * multi));
                                } else {
                                    Globals.CHARACTER_TABLE[i].Write("DAT", (330 * multi));
                                }

                                if (soasSiphonSlot == i) { //Soa's Siphon Ring
                                    multi *= 0.3;
                                }

                                if (currentMP[i] != previousMP[i] && currentMP[i] < previousMP[i]) {
                                    mp = previousMP[i] - currentMP[i];
                                    if (mp == 20 || mp == 80) {
                                        Globals.CHARACTER_TABLE[i].Write("DMAT", (440 * multi));
                                    } else if (mp == 30) {
                                        Globals.CHARACTER_TABLE[i].Write("DMAT", (330 * multi));
                                    }
                                    if ((Globals.CHARACTER_TABLE[i].Read("Spell_Cast") == 7 || Globals.CHARACTER_TABLE[i].Read("Spell_Cast") == 26) && Globals.DIFFICULTY_MODE.Equals("Hell")) {
                                        checkFlowerStorm = true;
                                    }
                                } else {
                                    if (currentMP[i] > previousMP[i]) {
                                        previousMP[i] = currentMP[i];
                                    } else {
                                        if (checkFlowerStorm) {
                                            byte changed = 0;
                                            byte partySize = 0;
                                            for (int x = 0; x < 3; x++) {
                                                if (Globals.PARTY_SLOT[x] < 9) {
                                                    partySize++;
                                                    if (Globals.CHARACTER_TABLE[i].Read("PWR_DF_TRN") != 0) {
                                                        changed++;
                                                    }
                                                }
                                            }
                                            if (changed == partySize) {
                                                checkFlowerStorm = false;
                                                for (int x = 0; x < 3; x++) {
                                                    Globals.CHARACTER_TABLE[i].Write("PWR_DF_TRN", uiCombo["cboFlowerStorm"]);
                                                    Globals.CHARACTER_TABLE[i].Write("PWR_MDF_TRN", uiCombo["cboFlowerStorm"]);
                                                }
                                            }
                                        }
                                    }
                                }
                            } else if (Globals.PARTY_SLOT[i] == 2 || Globals.PARTY_SLOT[i] == 8) { //Shana
                                if (dragoonSpecialAttack == 2 || dragoonSpecialAttack == 8) {
                                    Globals.CHARACTER_TABLE[i].Write("DAT", (510 * multi));
                                } else {
                                    Globals.CHARACTER_TABLE[i].Write("DAT", (365 * multi));
                                }

                                if (multi == 1) {
                                    emulator.WriteByte(Globals.CHAR_ADDRESS[i] + 0xA9, 0);
                                    emulator.WriteByte(Globals.CHAR_ADDRESS[i] + 0xAB, 0);
                                    Globals.CHARACTER_TABLE[i].Write("DDF", (180 * multi));
                                    Globals.CHARACTER_TABLE[i].Write("DMDF", (180 * multi));
                                } else {
                                    Globals.CHARACTER_TABLE[i].Write("DDF", (180 * multi));
                                    Globals.CHARACTER_TABLE[i].Write("DMDF", (180 * multi));
                                }

                                if (soasSiphonSlot == i) { //Soa's Siphon Ring
                                    multi *= 0.3;
                                }

                                if (currentMP[i] != previousMP[i] && currentMP[i] < previousMP[i]) {
                                    mp = previousMP[i] - currentMP[i];
                                    if (mp == 20) {
                                        Globals.CHARACTER_TABLE[i].Write("DMAT", (332 * multi));
                                        Globals.CHARACTER_TABLE[i].Write("HP_Regen", (recoveryRateSave + 20));
                                    } else if (mp == 80) {
                                        Globals.CHARACTER_TABLE[i].Write("DMAT", (289 * multi));
                                    }
                                    previousMP[i] = currentMP[i];
                                } else {
                                    if (currentMP[i] > previousMP[i]) {
                                        previousMP[i] = currentMP[i];
                                    }
                                }
                            } else if (Globals.PARTY_SLOT[i] == 3) { //Rose
                                if (dragoonSpecialAttack == 3) {
                                    Globals.CHARACTER_TABLE[i].Write("DAT", (495 * multi));
                                } else {
                                    Globals.CHARACTER_TABLE[i].Write("DAT", (330 * multi));
                                }

                                if (multi == 1) {
                                    emulator.WriteByte(Globals.CHAR_ADDRESS[i] + 0xA9, 0);
                                    emulator.WriteByte(Globals.CHAR_ADDRESS[i] + 0xAB, 0);
                                    Globals.CHARACTER_TABLE[i].Write("DDF", (180 * multi));
                                    Globals.CHARACTER_TABLE[i].Write("DMDF", (180 * multi));
                                } else {
                                    Globals.CHARACTER_TABLE[i].Write("DDF", (180 * multi));
                                    Globals.CHARACTER_TABLE[i].Write("DMDF", (180 * multi));
                                }

                                if (soasSiphonSlot == i) { //Soa's Siphon Ring
                                    multi *= 0.3;
                                }

                                if (currentMP[i] != previousMP[i] && currentMP[i] < previousMP[i]) {
                                    mp = previousMP[i] - currentMP[i];
                                    if (mp == 10) {
                                        Globals.CHARACTER_TABLE[i].Write("DMAT", (295 * multi));
                                        for (int x = 0; x < 3; x++) {
                                            if (Globals.PARTY_SLOT[x] < 9 && Globals.CHARACTER_TABLE[i].Read("HP") > 0) {
                                                Globals.CHARACTER_TABLE[i].Write("HP", (ushort) Math.Min(Globals.CHARACTER_TABLE[i].Read("Max_HP"), Globals.CHARACTER_TABLE[x].Read("HP") + Math.Round(Globals.CHARACTER_TABLE[i].Read("HP") * (emulator.ReadByte(Constants.GetAddress("ROSE_DRAGOON_LEVEL")) * 0.05))));
                                            }
                                        }
                                    } else if (mp == 20) {
                                        Globals.CHARACTER_TABLE[i].Write("DMAT", (395 * multi));
                                    } else if (mp == 25) {
                                        Globals.CHARACTER_TABLE[i].Write("DMAT", (410 * multi));
                                        for (int x = 0; x < 3; x++) {
                                            if (Globals.PARTY_SLOT[x] < 9 && Globals.CHARACTER_TABLE[i].Read("HP") > 0) {
                                                Globals.CHARACTER_TABLE[i].Write("HP", (ushort) Math.Min(Globals.CHARACTER_TABLE[i].Read("Max_HP"), Globals.CHARACTER_TABLE[x].Read("HP") + Math.Round(Globals.CHARACTER_TABLE[i].Read("HP") * (emulator.ReadByte(Constants.GetAddress("ROSE_DRAGOON_LEVEL")) * 0.04))));
                                            }
                                        }
                                    } else if (mp == 50) {
                                        Globals.CHARACTER_TABLE[i].Write("DMAT", (790 * multi));
                                    } else if (mp == 80) {
                                        Globals.CHARACTER_TABLE[i].Write("DMAT", (420 * multi));
                                        checkRoseDamage = true;
                                        checkRoseDamageSave = emulator.ReadShort(Constants.GetAddress("DAMAGE_SLOT1"));
                                    } else if (mp == 100) {
                                        Globals.CHARACTER_TABLE[i].Write("DMAT", (290 * multi));
                                        checkRoseDamage = true;
                                        checkRoseDamageSave = emulator.ReadShort(Constants.GetAddress("DAMAGE_SLOT1"));
                                    }
                                    previousMP[i] = currentMP[i];
                                } else {
                                    if (currentMP[i] > previousMP[i]) {
                                        previousMP[i] = currentMP[i];
                                    } else {
                                        if (checkRoseDamage && emulator.ReadShort(Constants.GetAddress("DAMAGE_SLOT1")) != checkRoseDamageSave) {
                                            checkRoseDamage = false;
                                            if (roseEnhanceDragoon) {
                                                Globals.CHARACTER_TABLE[i].Write("HP", (ushort) Math.Min(Globals.CHARACTER_TABLE[i].Read("HP") + (emulator.ReadShort(Constants.GetAddress("DAMAGE_SLOT1")) * 0.4), Globals.CHARACTER_TABLE[i].Read("Max_HP")));
                                            } else {
                                                Globals.CHARACTER_TABLE[i].Write("HP", (ushort) Math.Min(Globals.CHARACTER_TABLE[i].Read("HP") + (emulator.ReadShort(Constants.GetAddress("DAMAGE_SLOT1")) * 0.1), Globals.CHARACTER_TABLE[i].Read("Max_HP")));
                                            }
                                        }
                                    }
                                }
                            } else if (Globals.PARTY_SLOT[i] == 4) { //Haschel
                                if (dragoonSpecialAttack == 4) {
                                    Globals.CHARACTER_TABLE[i].Write("DAT", (422 * multi));
                                } else {
                                    Globals.CHARACTER_TABLE[i].Write("DAT", (281 * multi));
                                }

                                if (multi == 1) {
                                    emulator.WriteByte(Globals.CHAR_ADDRESS[i] + 0xA9, 0);
                                    emulator.WriteByte(Globals.CHAR_ADDRESS[i] + 0xAB, 0);
                                    Globals.CHARACTER_TABLE[i].Write("DDF", (180 * multi));
                                    Globals.CHARACTER_TABLE[i].Write("DMDF", (180 * multi));
                                } else {
                                    Globals.CHARACTER_TABLE[i].Write("DDF", (180 * multi));
                                    Globals.CHARACTER_TABLE[i].Write("DMDF", (180 * multi));
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
                                        Globals.CHARACTER_TABLE[i].Write("DMAT", (330 * multi));
                                    } else if (mp == 80) {
                                        Globals.CHARACTER_TABLE[i].Write("DMAT", (374 * multi));
                                    }
                                    previousMP[i] = currentMP[i];
                                } else {
                                    if (currentMP[i] > previousMP[i]) {
                                        previousMP[i] = currentMP[i];
                                    }
                                }
                            } else if (Globals.PARTY_SLOT[i] == 6) { //Meru
                                if (dragoonSpecialAttack == 6) {
                                    Globals.CHARACTER_TABLE[i].Write("DAT", (495 * multi));
                                } else {
                                    Globals.CHARACTER_TABLE[i].Write("DAT", (330 * multi));
                                }

                                if (multi == 1) {
                                    emulator.WriteByte(Globals.CHAR_ADDRESS[i] + 0xA9, 0);
                                    emulator.WriteByte(Globals.CHAR_ADDRESS[i] + 0xAB, 0);
                                    Globals.CHARACTER_TABLE[i].Write("DDF", (180 * multi));
                                    Globals.CHARACTER_TABLE[i].Write("DMDF", (180 * multi));
                                } else {
                                    Globals.CHARACTER_TABLE[i].Write("DDF", (180 * multi));
                                    Globals.CHARACTER_TABLE[i].Write("DMDF", (180 * multi));
                                }

                                if (soasSiphonSlot == i) { //Soa's Siphon Ring
                                    multi *= 0.3;
                                }

                                if (currentMP[i] != previousMP[i] && currentMP[i] < previousMP[i]) {
                                    mp = previousMP[i] - currentMP[i];
                                    if (mp == 10) {
                                        if (multi == 1) {
                                            emulator.WriteByte(Globals.CHAR_ADDRESS[i] + 0xA7, 0);
                                            Globals.CHARACTER_TABLE[i].Write("DMAT", (255 * multi));
                                        } else {
                                            Globals.CHARACTER_TABLE[i].Write("DMAT", (255 * multi));
                                        }
                                    } else if (mp == 30) {
                                        Globals.CHARACTER_TABLE[i].Write("DMAT", (264 * multi));
                                    } else if (mp == 80) {
                                        Globals.CHARACTER_TABLE[i].Write("DMAT", (350 * multi));
                                    }

                                    //Jeweled Hammer
                                    if (Globals.CHARACTER_TABLE[i].Read("Weapon") == 164) {
                                        if (mp == 50) {
                                            if (multi == 1) {
                                                emulator.WriteByte(Globals.CHAR_ADDRESS[i] + 0xA7, 0);
                                                Globals.CHARACTER_TABLE[i].Write("DMAT", (255 * multi));
                                            } else {
                                                Globals.CHARACTER_TABLE[i].Write("DMAT", (255 * multi));
                                            }
                                        } else if (mp == 100) {
                                            if (Globals.CHARACTER_TABLE[i].Read("Spell_Cast") == 25) {
                                                //trackchp
                                                //rainbow breath
                                            } else {
                                                Globals.CHARACTER_TABLE[i].Write("DMAT", (350 * multi));
                                            }
                                        } else if (mp == 150) {
                                            Globals.CHARACTER_TABLE[i].Write("DMAT", (525 * multi));
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
                                    Globals.CHARACTER_TABLE[i].Write("DAT", (600 * multi));
                                } else {
                                    Globals.CHARACTER_TABLE[i].Write("DAT", (500 * multi));
                                }

                                if (multi == 1) {
                                    emulator.WriteByte(Globals.CHAR_ADDRESS[i] + 0xA9, 0);
                                    emulator.WriteByte(Globals.CHAR_ADDRESS[i] + 0xAB, 0);
                                    Globals.CHARACTER_TABLE[i].Write("DDF", (130 * multi));
                                    Globals.CHARACTER_TABLE[i].Write("DMDF", (130 * multi));
                                } else {
                                    Globals.CHARACTER_TABLE[i].Write("DDF", (130 * multi));
                                    Globals.CHARACTER_TABLE[i].Write("DMDF", (130 * multi));
                                }

                                if (soasSiphonSlot == i) { //Soa's Siphon Ring
                                    multi *= 0.3;
                                }

                                if (currentMP[i] != previousMP[i] && currentMP[i] < previousMP[i]) {
                                    mp = previousMP[i] - currentMP[i];
                                    if (mp == 20) {
                                        Globals.CHARACTER_TABLE[i].Write("DMAT", (450 * multi));
                                    } else if (mp == 30) {
                                        Globals.CHARACTER_TABLE[i].Write("DMAT", (560 * multi));
                                    } else if (mp == 80) {
                                        Globals.CHARACTER_TABLE[i].Write("DMAT", (740 * multi));
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

        public void AddBurnStack(int amount) {
            dartBurnStack = (dartBurnStack + amount) > 6 ? 6 : (dartBurnStack + amount);
            Constants.WriteGLogOutput("Dart's Burn Stack Count: " + dartBurnStack);
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
            if (Globals.IN_BATTLE && Globals.STATS_CHANGED && !ubElementalShift && eleBombTurns == 0) {
                eleBombItemUsed = emulator.ReadByte(Globals.MONS_ADDRESS[0] + 0xABC);
                if ((eleBombItemUsed >= 241 && eleBombItemUsed <= 248) || eleBombItemUsed == 250) {
                    byte player1Action = Globals.CHARACTER_TABLE[0].Read("Action");
                    byte player2Action = Globals.CHARACTER_TABLE[1].Read("Action");
                    byte player3Action = Globals.CHARACTER_TABLE[2].Read("Action");
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
                //Constants.WriteDebug("Item: " + eleBombItemUsed + " | Slot: " + eleBombSlot + " | Turns: " + eleBombTurns + " | Change: " + eleBombChange + " | Element: " + eleBombElement + " | Action: " + Globals.CHARACTER_TABLE[eleBombSlot].Read("Action"));
                if (emulator.ReadShort(Constants.GetAddress("BATTLE_VALUE")) == 41215 && Globals.STATS_CHANGED && eleBombSlot >= 0) {
                    if ((Globals.CHARACTER_TABLE[eleBombSlot].Read("Action") == 8 || Globals.CHARACTER_TABLE[eleBombSlot].Read("Action") == 10) && !eleBombChange) {
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
                                eleBombOldElement[i] = Globals.MONSTER_TABLE[i].Read("Element");
                                Globals.MONSTER_TABLE[i].Write("Element", element);
                                Globals.MONSTER_TABLE[i].Write("Display_Element", element);
                            }

                            eleBombTurns -= 1;
                        }
                    }

                    if (eleBombChange && (Globals.CHARACTER_TABLE[eleBombSlot].Read("Action") == 0 || Globals.CHARACTER_TABLE[eleBombSlot].Read("Action") == 2)) {
                        eleBombChange = false;
                        eleBombTurns -= 1;
                        if (eleBombTurns <= 0) {
                            for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                                Globals.MONSTER_TABLE[i].Write("Element", eleBombOldElement[i]);
                                Globals.MONSTER_TABLE[i].Write("Display_Element", eleBombOldElement[i]);
                            }
                        }
                    }

                    if (Globals.CHARACTER_TABLE[eleBombSlot].Read("Action") == 192) {
                        for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                            Globals.MONSTER_TABLE[i].Write("Element", eleBombOldElement[i]);
                            Globals.MONSTER_TABLE[i].Write("Display_Element", eleBombOldElement[i]);
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
            if (Globals.IN_BATTLE && Globals.STATS_CHANGED && !noDragoonModeOnBattleEntry) {
                for (int i = 0; i < 3; i++) {
                    if (Globals.PARTY_SLOT[i] < 9) {
                        Globals.CHARACTER_TABLE[i].Write("Dragoon", 0);
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
                            Globals.CHARACTER_TABLE[i].Write("ADD_SP_Multi", spMulti);
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
                            Globals.CHARACTER_TABLE[i].Write("ADD_DMG_Multi", (byte) Math.Ceiling(Globals.CHARACTER_TABLE[i].Read("ADD_DMG_Multi") * 2.2285));
                        }
                    } else if (Globals.PARTY_SLOT[i] == 3) {
                        if (emulator.ReadByte(Constants.GetAddress("EQUIPPED_ADDITION") + (0x2C * Globals.PARTY_SLOT[i])) == 15) {
                            Globals.CHARACTER_TABLE[i].Write("ADD_DMG_Multi", (byte) (Globals.CHARACTER_TABLE[i].Read("ADD_DMG_Multi") + 60));
                        } else if (emulator.ReadByte(Constants.GetAddress("EQUIPPED_ADDITION") + (0x2C * Globals.PARTY_SLOT[i])) == 16) {
                            Globals.CHARACTER_TABLE[i].Write("ADD_DMG_Multi", (ushort) Math.Ceiling(Globals.CHARACTER_TABLE[i].Read("ADD_DMG_Multi") * 1.4));
                        } else if (emulator.ReadByte(Constants.GetAddress("EQUIPPED_ADDITION") + (0x2C * Globals.PARTY_SLOT[i])) == 17) {
                            Globals.CHARACTER_TABLE[i].Write("ADD_DMG_Multi", (byte) Math.Ceiling(Globals.CHARACTER_TABLE[i].Read("ADD_DMG_Multi") * 1.1666));
                        }
                    } else if (Globals.PARTY_SLOT[i] == 6) {
                        if (emulator.ReadByte(Constants.GetAddress("EQUIPPED_ADDITION") + (0x2C * Globals.PARTY_SLOT[i])) == 27) {
                            Globals.CHARACTER_TABLE[i].Write("ADD_DMG_Multi", (byte) Math.Ceiling(Globals.CHARACTER_TABLE[i].Read("ADD_DMG_Multi") * 0.75));
                        }
                    } else if (Globals.PARTY_SLOT[i] == 7) {
                        if (emulator.ReadByte(Constants.GetAddress("EQUIPPED_ADDITION") + (0x2C * Globals.PARTY_SLOT[i])) == 19) {
                            Globals.CHARACTER_TABLE[i].Write("ADD_DMG_Multi", (byte) (Globals.CHARACTER_TABLE[i].Read("ADD_DMG_Multi") * 2));
                        } else if (emulator.ReadByte(Constants.GetAddress("EQUIPPED_ADDITION") + (0x2C * Globals.PARTY_SLOT[i])) == 20) {
                            Globals.CHARACTER_TABLE[i].Write("ADD_SP_Multi", 80);
                            Globals.CHARACTER_TABLE[i].Write("ADD_DMG_Multi", (byte) (Globals.CHARACTER_TABLE[i].Read("ADD_DMG_Multi") * 2));
                        } else if (emulator.ReadByte(Constants.GetAddress("EQUIPPED_ADDITION") + (0x2C * Globals.PARTY_SLOT[i])) == 21) {
                            Globals.CHARACTER_TABLE[i].Write("ADD_SP_Multi", 50);
                            Globals.CHARACTER_TABLE[i].Write("ADD_DMG_Multi", (ushort) (Globals.CHARACTER_TABLE[i].Read("ADD_DMG_Multi") * 4));
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
                        if (Globals.CHARACTER_TABLE[i].Read("HP_Regen") == 246) {
                            Globals.CHARACTER_TABLE[i].Write("HP_Regen", 0);
                        } else if (Globals.CHARACTER_TABLE[i].Read("HP_Regen") == 256) {
                            Globals.CHARACTER_TABLE[i].Write("HP_Regen", 10);
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
            if (emulator.ReadShort(Constants.GetAddress("BATTLE_VALUE")) == 41215 && Globals.STATS_CHANGED && Constants.BATTLE_UI) {
                for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                    int lastX = 0;
                    long hpName = Constants.GetAddress("MONSTERS_NAMES") + (i * 0x2C);
                    char[] hpArray = Globals.MONSTER_TABLE[i].Read("HP").ToString().ToCharArray();
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

        #region Damage Tracker
        public void DamageTracker() {
            if (Globals.IN_BATTLE && Globals.STATS_CHANGED && !damageTrackerOnBattleEntry) {
                for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                    if (ultimateHP[i] > 0) {
                        dmgTrkHP[i] = ultimateHP[i];
                    } else {
                        dmgTrkHP[i] = Globals.MONSTER_TABLE[i].Read("HP");
                    }
                }
                for (int i = 0; i < 3; i++) {
                    dmgTrkChr[i] = 0;
                }
                damageTrackerOnBattleEntry = true;
            } else {
                if (!Globals.IN_BATTLE && damageTrackerOnBattleEntry) {
                    damageTrackerOnBattleEntry = false;
                } else {
                    if (Globals.IN_BATTLE && Globals.STATS_CHANGED) {
                        bool partyAttacking = false;
                        for (int i = 0; i < 3; i++) {
                            byte action = Globals.CHARACTER_TABLE[i].Read("Action");
                            if (action == 24 || action == 26 || action == 136 || action == 138) {
                                partyAttacking = true;
                                dmgTrkSlot = i;
                            }
                        }

                        //if (partyAttacking || ubCheckDamageCycle > 0) {
                            for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                                if (ultimateHP[i] > 0) {
                                    if (ultimateHP[i] < dmgTrkHP[i]) {
                                        dmgTrkChr[dmgTrkSlot] += dmgTrkHP[i] - ultimateHP[i];
                                        dmgTrkHP[i] = ultimateHP[i];
                                    } else if (ultimateHP[i] > dmgTrkHP[i]) {
                                        dmgTrkHP[i] = ultimateHP[i];
                                    }
                                } else {
                                    if (Globals.MONSTER_TABLE[i].Read("HP") < dmgTrkHP[i]) {
                                        dmgTrkChr[dmgTrkSlot] += dmgTrkHP[i] - Globals.MONSTER_TABLE[i].Read("HP");
                                        dmgTrkHP[i] = Globals.MONSTER_TABLE[i].Read("HP");
                                    } else if (Globals.MONSTER_TABLE[i].Read("HP") > dmgTrkHP[i]) {
                                        dmgTrkHP[i] = Globals.MONSTER_TABLE[i].Read("HP");
                                    }
                                }
                            }
                        //}

                        Constants.WritePLog("Damage Track: " + dmgTrkChr[0] + " / " + dmgTrkChr[1] + " / " + dmgTrkChr[2]);
                    }
                }
            }
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

            if (!this.IsLoaded) {
                Constants.RUN = false;
                Thread.Sleep(3000);
            } else {
                CloseEmulator();
            }

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
                SaveSubKey();

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

            if (btn.Name.Equals("btnUltimateBoss")) {
                UltimateBossFieldSet();
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

            if (cbo.Name.Equals("cboUltimateBoss")) {
                UltimateBossFieldSet();
            }
        }

        public void DifficultyButton(object sender, EventArgs e) {

        }

        private void Slider_ValueChanged(object sender,
            RoutedPropertyChangedEventArgs<double> e) {
            var slider = sender as Slider;
            if (sender == sldHP) {
                Globals.HP_MULTI = slider.Value;
            } else if (sender == sldATK) {
                Globals.AT_MULTI = slider.Value;
            } else if (sender == sldDEF) {
                Globals.DF_MULTI = slider.Value;
            } else if (sender == sldMAT) {
                Globals.MAT_MULTI = slider.Value;
            } else if (sender == sldMDF) {
                Globals.MDF_MULTI = slider.Value;
            } else if (sender == sldSPD) {
                Globals.SPD_MULTI = slider.Value;
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