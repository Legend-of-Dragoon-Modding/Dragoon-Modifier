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
using System.ComponentModel;
using Microsoft.Win32;
using System.Windows.Input;
using System.Reflection;
using ControlzEx.Standard;
using System.Net;
using System.Threading.Tasks;
using MahApps.Metro.Controls;
using Dragoon_Modifier.Core;

namespace Dragoon_Modifier.UI {
    /// <summary>
    /// Interakční logika pro MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow {
        private static readonly SolidColorBrush _offColor = new SolidColorBrush(Color.FromArgb(255, 255, 168, 168));
        private static readonly SolidColorBrush _onColor = new SolidColorBrush(Color.FromArgb(255, 168, 211, 255));
        private static readonly SolidColorBrush _grayOffColor = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        private static readonly string[] characters = { "Dart", "Lavitz", "Shana", "Rose", "Haschel", "Albert", "Meru", "Kongol", "Miranda" };

        public readonly DraMod.UI.IUIControl UIControl;
        public readonly DraMod.IDraMod DragoonModifier;

        public Thread uiThread; //TODO Track maximize event

        public ComboBox[] battleRow = new ComboBox[9];
        public ComboBox[] battleRowBoost = new ComboBox[9];

        public MainWindow() {
            InitializeComponent();
            UIControl = InitUI();
            DragoonModifier = DraMod.Factory.DraMod(UIControl, Directory.GetCurrentDirectory());
            Console.SetOut(new TextBoxOutput(txtOutput));
            this.Title += DraMod.Constants.Version;
            DraMod.Constants.LoadRegistry();
            LoadRegistry();
        }

        private DraMod.UI.IUIControl InitUI() {
            cboAspectRatio.Items.Add("4:3");
            cboAspectRatio.Items.Add("16:9");
            cboAspectRatio.Items.Add("16:10");
            cboAspectRatio.Items.Add("21:9");
            cboAspectRatio.Items.Add("32:9");

            cboCamera.Items.Add("Default");
            cboCamera.Items.Add("Advanced");

            cboSoloLeader.Items.Add("Slot 1");
            cboSoloLeader.Items.Add("Slot 2");
            cboSoloLeader.Items.Add("Slot 3");

            cboKillBGM.Items.Add("Field");
            cboKillBGM.Items.Add("Battle");
            cboKillBGM.Items.Add("Both");

            cboFlowerStorm.Items.Add("2 Turns (40 MP)");
            cboFlowerStorm.Items.Add("3 Turns (60 MP)");
            cboFlowerStorm.Items.Add("4 Turns (80 MP)");
            cboFlowerStorm.Items.Add("5 Turns (100 MP)");

            for (int i = 0; i < characters.Length; i++) {
                cboSwitchChar.Items.Add(characters[i]);
                cboSwitch1.Items.Add(characters[i]);
                cboSwitch2.Items.Add(characters[i]);
            }

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

            battleRow[0] = cboRowDart;
            battleRow[1] = cboRowLavitz;
            battleRow[2] = cboRowShana;
            battleRow[3] = cboRowRose;
            battleRow[4] = cboRowHaschel;
            battleRow[5] = cboRowAlbert;
            battleRow[6] = cboRowMeru;
            battleRow[7] = cboRowKongol;
            battleRow[8] = cboRowMiranda;

            battleRowBoost[0] = cboRowDartBoost;
            battleRowBoost[1] = cboRowLavitzBoost;
            battleRowBoost[2] = cboRowShanaBoost;
            battleRowBoost[3] = cboRowRoseBoost;
            battleRowBoost[4] = cboRowHaschelBoost;
            battleRowBoost[5] = cboRowAlbertBoost;
            battleRowBoost[6] = cboRowMeruBoost;
            battleRowBoost[7] = cboRowKongolBoost;
            battleRowBoost[8] = cboRowMirandaBoost;

            for (int i = 0; i < 9; i++) {
                battleRow[i].Items.Add("Stay");
                battleRow[i].Items.Add("Front");
                battleRow[i].Items.Add("Back");
                battleRow[i].SelectedIndex = 0;
                DraMod.Settings.Instance.BattleRowsFormation[i] = 0;
            }

            for (int i = 0; i < 9; i++) {
                battleRowBoost[i].Items.Add("No Boost");
                battleRowBoost[i].Items.Add("Attack Boost");
                battleRowBoost[i].Items.Add("Magic Boost");
                battleRowBoost[i].SelectedIndex = 0;
                DraMod.Settings.Instance.BattleRowsBoost[i] = 0;
            }

            cboQTB.Items.Add("Dart");
            cboQTB.Items.Add("Lavitz");
            cboQTB.Items.Add("Shana");
            cboQTB.Items.Add("Rose");
            cboQTB.Items.Add("Haschel");
            cboQTB.Items.Add("Albert");
            cboQTB.Items.Add("Meru");
            cboQTB.Items.Add("Kongol");
            cboQTB.Items.Add("Miranda");

            lstTicketShop.Items.Add("1 Ticket / 15 G");
            lstTicketShop.Items.Add("5 Tickets / 60 G");
            lstTicketShop.Items.Add("10 Tickets / 100 G");
            lstHeroShop.Items.Add("Spirit Poition/20 Tickets");
            lstHeroShop.Items.Add("Total Vanishing/40 Tickets");
            lstHeroShop.Items.Add("Healing Rain/60 Tickets");
            lstHeroShop.Items.Add("Moon Serenade/100 Tickets");
            lstUltimateShop.Items.Add("Spirit Eater / 70,000 G");
            lstUltimateShop.Items.Add("Harpoon / 70,000 G");
            lstUltimateShop.Items.Add("Element Arrow / 70,000 G");
            lstUltimateShop.Items.Add("Dragon Beater / 70,000 G");
            lstUltimateShop.Items.Add("Battery Glove / 70,000 G");
            lstUltimateShop.Items.Add("Jeweled Hammer  / 70,000 G");
            lstUltimateShop.Items.Add("Giant Axe / 70,000 G");
            lstUltimateShop.Items.Add("Soa's Light / 280,000 G");
            lstUltimateShop.Items.Add("Fake Legend Casque / 30,000 G");
            lstUltimateShop.Items.Add("Soa's Helm / 120,000 G");
            lstUltimateShop.Items.Add("Fake Legend Armor / 30,000 G");
            lstUltimateShop.Items.Add("Divine DG Armor / 60,000 G");
            lstUltimateShop.Items.Add("Soa's Armor / 120,000 G");
            lstUltimateShop.Items.Add("Lloyd's Boots / 40,000 G");
            lstUltimateShop.Items.Add("Winged Shoes / 40,000 G");
            lstUltimateShop.Items.Add("Soa's Greaves / 120,000 G");
            lstUltimateShop.Items.Add("Heal Ring / 20,000 G");
            lstUltimateShop.Items.Add("Soa's Sash / 40,000 G");
            lstUltimateShop.Items.Add("Soa's Ahnk / 50,000 G");
            lstUltimateShop.Items.Add("Soa's Health Ring / 50,000 G");
            lstUltimateShop.Items.Add("Soa's Mage Ring / 50,000 G");
            lstUltimateShop.Items.Add("Soa's Shield / 50,000 G");
            lstUltimateShop.Items.Add("Soa's Siphon Ring / 50,000 G");
            lstUltimateShop.Items.Add("Super Power Up / 100,000 G");
            lstUltimateShop.Items.Add("Super Power Down / 100,000 G");
            lstUltimateShop.Items.Add("Super Speed Up / 100,000 G");
            lstUltimateShop.Items.Add("Super Speed Down / 100,000 G");
            lstUltimateShop.Items.Add("Super Magic Shield / 100,000 G");
            lstUltimateShop.Items.Add("Super Material Shield / 100,000 G");
            lstUltimateShop.Items.Add("Super Magic Stone of Signet / 75,000 G");
            lstUltimateShop.Items.Add("Super Pandemonium / 25,000 G");
            lstUltimateShop.Items.Add("Psychedelic Bomb X / 1,000,000 G");
            lstUltimateShop.Items.Add("Empty Dragoon Crystal / 250,000 G");
            lstUltimateShop.Items.Add("Soa's Wargod / 500,000 G");
            lstUltimateShop.Items.Add("Soa's Dragoon Boost / 500,000 G");

            cboAspectRatio.SelectedIndex = 0;
            cboCamera.SelectedIndex = 0;
            cboSoloLeader.SelectedIndex = 0;
            cboSwitchChar.SelectedIndex = 0;
            cboSwitch1.SelectedIndex = 0;
            cboSwitch2.SelectedIndex = 0;
            cboKillBGM.SelectedIndex = 1;
            cboUltimateBoss.SelectedIndex = 0;
            cboFlowerStorm.SelectedIndex = 0;
            cboQTB.SelectedIndex = 0;
            lstTicketShop.SelectedIndex = 0;
            lstHeroShop.SelectedIndex = 0;
            lstUltimateShop.SelectedIndex = 0;

            TextBlock[,] monsterLables = new TextBlock[6, 6] {
                {lblEnemy1Name, lblEnemy1HP, lblEnemy1ATK, lblEnemy1DEF, lblEnemy1SPD, lblEnemy1TRN},
                {lblEnemy2Name, lblEnemy2HP, lblEnemy2ATK, lblEnemy2DEF, lblEnemy2SPD, lblEnemy2TRN},
                {lblEnemy3Name, lblEnemy3HP, lblEnemy3ATK, lblEnemy3DEF, lblEnemy3SPD, lblEnemy3TRN},
                {lblEnemy4Name, lblEnemy4HP, lblEnemy4ATK, lblEnemy4DEF, lblEnemy4SPD, lblEnemy4TRN},
                {lblEnemy5Name, lblEnemy5HP, lblEnemy5ATK, lblEnemy5DEF, lblEnemy5SPD, lblEnemy5TRN},
                {lblEnemy6Name, lblEnemy6HP, lblEnemy6ATK, lblEnemy6DEF, lblEnemy6SPD, lblEnemy6TRN}
            };
            TextBlock[,] characterLables = new TextBlock[3, 9] {
                { lblCharacter1Name, lblCharacter1HMP, lblCharacter1ATK, lblCharacter1DEF, lblCharacter1VHIT, lblCharacter1DATK, lblCharacter1DDEF, lblCharacter1SPD, lblCharacter1TRN },
                { lblCharacter2Name, lblCharacter2HMP, lblCharacter2ATK, lblCharacter2DEF, lblCharacter2VHIT, lblCharacter2DATK, lblCharacter2DDEF, lblCharacter2SPD, lblCharacter2TRN },
                { lblCharacter3Name, lblCharacter3HMP, lblCharacter3ATK, lblCharacter3DEF, lblCharacter3VHIT, lblCharacter3DATK, lblCharacter3DDEF, lblCharacter3SPD, lblCharacter3TRN }
            };
            TextBlock[] fieldLables = new TextBlock[3] {
                lblEncounter, lblEnemyID, lblMapID
            };
            ProgressBar[] turnBattleBars = new ProgressBar[10] {
                pgrQTB, pgrEATBM1, pgrEATBM2, pgrEATBM3, pgrEATBM4, pgrEATBM5, pgrEATBM6, pgrEATBC1, pgrEATBC2, pgrEATBC3
            };

            return Factory.UIControl(monsterLables, characterLables, stsGame, stsProgram, fieldLables, turnBattleBars);
        }

        private void LoadRegistry() {
            if (DraMod.Constants.KEY.GetValue("Save Slot") == null) {
                foreach (MenuItem mi in miSaveSlot.Items)
                    mi.IsChecked = miSaveSlot.Items.IndexOf(mi) == DraMod.Constants.SaveSlot ? true : false;
                DraMod.Constants.KEY.SetValue("Save Slot", DraMod.Constants.SaveSlot);
            } else {
                int slot = (int) DraMod.Constants.KEY.GetValue("Save Slot");
                DraMod.Constants.SaveSlot = slot;
                foreach (MenuItem mi in miSaveSlot.Items)
                    mi.IsChecked = miSaveSlot.Items.IndexOf(mi) == slot ? true : false;
            }

            if (DraMod.Constants.KEY.GetValue("EmulatorType") != null) {
                int slot = (int) DraMod.Constants.KEY.GetValue("EmulatorType");
                DraMod.Constants.EmulatorID = (byte) slot;

                foreach (MenuItem mi in miEmulator.Items)
                    mi.IsChecked = miEmulator.Items.IndexOf(mi) == slot ? true : false;
            }

            if (DraMod.Constants.KEY.GetValue("Region") == null) {
                foreach (MenuItem mi in miRegion.Items)
                    mi.IsChecked = miRegion.Items.IndexOf(mi) == (byte) DraMod.Constants.Region ? true : false;
                DraMod.Constants.KEY.SetValue("Region", (int) DraMod.Constants.Region);
            } else {
                Region slot = (Region) ((int) DraMod.Constants.KEY.GetValue("Region"));
                DraMod.Constants.Region = slot;
                foreach (MenuItem mi in miRegion.Items)
                    mi.IsChecked = miRegion.Items.IndexOf(mi) == (byte) DraMod.Constants.Region ? true : false;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            SetupEmulator();
        }

        private void Window_Closing(object sender, CancelEventArgs e) {
            DraMod.Constants.SaveRegistry();
            DraMod.Constants.Run = false;
        }

        private void Window_Closed(object sender, EventArgs e) {
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e) {
            //TODO Track maximize event
            foreach (RowDefinition row in gridField.RowDefinitions) {
                row.Height = new GridLength(Math.Round(50 * (this.Height / 750)), GridUnitType.Pixel);
            }

            foreach (RowDefinition row in gridBattle.RowDefinitions) {
                row.Height = new GridLength(Math.Round(50 * (this.Height / 750)), GridUnitType.Pixel);
            }

            foreach (RowDefinition row in gridFieldBattle.RowDefinitions) {
                row.Height = new GridLength(Math.Round(50 * (this.Height / 750)), GridUnitType.Pixel);
            }

            foreach (RowDefinition row in gridHardHell.RowDefinitions) {
                row.Height = new GridLength(Math.Round(50 * (this.Height / 750)), GridUnitType.Pixel);
            }

            foreach (RowDefinition row in gridBattleRows.RowDefinitions) {
                row.Height = new GridLength(Math.Round(50 * (this.Height / 750)), GridUnitType.Pixel);
            }

            foreach (RowDefinition row in gridTurnBattle.RowDefinitions) {
                row.Height = new GridLength(Math.Round(50 * (this.Height / 750)), GridUnitType.Pixel);
            }
        }

        public void CloseEmulator() {
        }

        public void SetupEmulator() {
            if (DraMod.Constants.Run) {
                DraMod.Constants.Run = false;
                miAttach.Header = "Attach";
                return;
            }

            if (DraMod.Constants.EmulatorID == 0) {
                DraMod.Constants.EmulatorName = "RetroArch";
            } else if (DraMod.Constants.EmulatorID == 1) {
                DraMod.Constants.EmulatorName = "DuckStation";
            } else if (DraMod.Constants.EmulatorID == 2) {
                DraMod.Constants.EmulatorName = "ePSXe";
            } else if (DraMod.Constants.EmulatorID == 3) {
                DraMod.Constants.EmulatorName = "PCSXR-PGXP";
            } else if (DraMod.Constants.EmulatorID == 4) {
                DraMod.Constants.EmulatorName = "PCSX2";
            } else {
                DraMod.Constants.EmulatorName = "Other";
            }

            if (DragoonModifier.Attach(DraMod.Constants.EmulatorName, DraMod.Constants.PreviousOffset)) {
                miAttach.Header = "Detach";
                UIControl.WriteGLog("Game Log");
                UIControl.WritePLog("Program Log");
            }
        }

        private void miAttach_Click(object sender, RoutedEventArgs e) {
            SetupEmulator();
        }

        private void miModOptions_Click(object sender, RoutedEventArgs e) {
            if (!DraMod.Settings.Instance.Difficulty.Equals("Normal")) {
                UIControl.WritePLog("You can't change preset options while using a built-in preset.");
            } else {
                InputWindow openModWindow = new InputWindow("Mod Options");
                ComboBox mod = new ComboBox();
                CheckBox monsterStat = new CheckBox();
                CheckBox monsterDrop = new CheckBox();
                CheckBox monsterExpGold = new CheckBox();
                CheckBox characterStat = new CheckBox();
                CheckBox addition = new CheckBox();
                CheckBox dragoonStats = new CheckBox();
                CheckBox dragoonSpell = new CheckBox();
                CheckBox dragoonAddition = new CheckBox();
                CheckBox dragoonDescription = new CheckBox();
                CheckBox itemStat = new CheckBox();
                CheckBox itemIcon = new CheckBox();
                CheckBox itemNameDescription = new CheckBox();
                CheckBox shop = new CheckBox();

                monsterStat.Content = "Monster Stats";
                if (DraMod.Settings.Instance.MonsterStatChange)
                    monsterStat.IsChecked = true;

                monsterDrop.Content = "Drop";
                if (DraMod.Settings.Instance.MonsterDropChange)
                    monsterDrop.IsChecked = true;

                monsterExpGold.Content = "Exp + Gold";
                if (DraMod.Settings.Instance.MonsterExpGoldChange)
                    monsterExpGold.IsChecked = true;

                characterStat.Content = "Character Stats";
                if (DraMod.Settings.Instance.CharacterStatChange)
                    characterStat.IsChecked = true;

                addition.Content = "Addition";
                if (DraMod.Settings.Instance.AdditionChange)
                    addition.IsChecked = true;

                dragoonStats.Content = "Dragoon Stats";
                if (DraMod.Settings.Instance.DragoonStatChange)
                    dragoonStats.IsChecked = true;

                dragoonSpell.Content = "Dragoon Spells";
                if (DraMod.Settings.Instance.DragoonSpellChange)
                    dragoonSpell.IsChecked = true;

                dragoonAddition.Content = "Dragoon Additions";
                if (DraMod.Settings.Instance.DragoonAdditionChange)
                    dragoonAddition.IsChecked = true;

                dragoonDescription.Content = "Dragoon Descriptions";
                if (DraMod.Settings.Instance.DragoonDescriptionChange)
                    dragoonDescription.IsChecked = true;

                itemStat.Content = "Item Stats";
                if (DraMod.Settings.Instance.ItemStatChange)
                    itemStat.IsChecked = true;

                itemIcon.Content = "Item Icons";
                if (DraMod.Settings.Instance.ItemIconChange)
                    itemIcon.IsChecked = true;

                itemNameDescription.Content = "Item Names + Descriptions";
                if (DraMod.Settings.Instance.ItemNameDescChange)
                    itemNameDescription.IsChecked = true;

                shop.Content = "Shop";
                if (DraMod.Settings.Instance.ShopChange)
                    shop.IsChecked = true;

                string[] dirs = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + "Mods\\");
                foreach (string dir in dirs) {
                    mod.Items.Add(new DirectoryInfo(dir).Name);
                }

                openModWindow.AddObject(mod);
                openModWindow.AddTextBlock("Database");
                openModWindow.AddObject(shop);
                openModWindow.AddObject(itemNameDescription);
                openModWindow.AddObject(itemIcon);
                openModWindow.AddObject(itemStat);
                openModWindow.AddObject(dragoonDescription);
                openModWindow.AddObject(dragoonAddition);
                openModWindow.AddObject(dragoonSpell);
                openModWindow.AddObject(dragoonStats);
                openModWindow.AddObject(addition);
                openModWindow.AddObject(characterStat);
                openModWindow.AddObject(monsterExpGold);
                openModWindow.AddObject(monsterDrop);
                openModWindow.AddObject(monsterStat);
                openModWindow.AddTextBlock("Please select the mods you want to turn on or off.");
                openModWindow.ShowDialog();

                DraMod.Settings.Instance.MonsterStatChange = (bool) monsterStat.IsChecked;
                DraMod.Settings.Instance.MonsterDropChange = (bool) monsterDrop.IsChecked;
                DraMod.Settings.Instance.MonsterExpGoldChange = (bool) monsterExpGold.IsChecked;
                DraMod.Settings.Instance.CharacterStatChange = (bool) characterStat.IsChecked;
                DraMod.Settings.Instance.AdditionChange = (bool) addition.IsChecked;
                DraMod.Settings.Instance.DragoonStatChange = (bool) dragoonStats.IsChecked;
                DraMod.Settings.Instance.DragoonSpellChange = (bool) dragoonSpell.IsChecked;
                DraMod.Settings.Instance.DragoonAdditionChange = (bool) dragoonAddition.IsChecked;
                DraMod.Settings.Instance.DragoonDescriptionChange = (bool) dragoonDescription.IsChecked;
                DraMod.Settings.Instance.ItemStatChange = (bool) itemStat.IsChecked;
                DraMod.Settings.Instance.ItemIconChange = (bool) itemIcon.IsChecked;
                DraMod.Settings.Instance.ItemNameDescChange = (bool) itemNameDescription.IsChecked;
                DraMod.Settings.Instance.ShopChange = (bool) shop.IsChecked;

                DraMod.Settings.Instance.Preset = DraMod.Preset.Custom;
                DragoonModifier.ChangeLoDDirectory((string) mod.SelectedValue);

                UIControl.WritePLog($"Mod directory: {(string) mod.SelectedValue}" );
                
            }
        }

        private void miEmulator_Click(object sender, RoutedEventArgs e) {
            foreach (MenuItem mi in miEmulator.Items) {
                mi.IsChecked = (MenuItem) sender == mi ? true : false;
            }

            DraMod.Constants.EmulatorID = (byte) miEmulator.Items.IndexOf((MenuItem) sender);

            SetupEmulator();
        }

        private void miRegion_Click(object sender, RoutedEventArgs e) {
            foreach (MenuItem mi in miRegion.Items) {
                mi.IsChecked = (MenuItem) sender == mi ? true : false;
            }

            DraMod.Constants.Region = (Region) miRegion.Items.IndexOf((MenuItem) sender);

            //TODO: hotkey swap code for JPN

            SetupEmulator();
        }

        public void SwitchButton(object sender, EventArgs e) {
            Button btn = (Button) sender;
            switch (btn.Name) {
                //Field
                case "btnSaveAnywhere":
                    ToggleButton(ref btn, ref DraMod.Settings.Instance.SaveAnywhere);
                    break;
                case "btnCharmPotion":
                    ToggleButton(ref btn, ref DraMod.Settings.Instance.AutoCharmPotion);
                    break;
                case "btnTextSpeed":
                    ToggleButton(ref btn, ref DraMod.Settings.Instance.IncreaseTextSpeed);
                    break;
                case "btnAutoText":
                    ToggleButton(ref btn, ref DraMod.Settings.Instance.AutoAdvanceText);
                    break;
                //Battle
                case "btnRemoveCaps":
                    ToggleButton(ref btn, ref DraMod.Settings.Instance.RemoveDamageCaps);
                    break;
                case "btnElementalBomb":
                    ToggleButton(ref btn, ref DraMod.Settings.Instance.ElementalBomb);
                    break;
                case "btnHPNames":
                    ToggleButton(ref btn, ref DraMod.Settings.Instance.MonsterHPAsNames);
                    break;
                case "btnEnrage":
                    ToggleButton(ref btn, ref DraMod.Settings.Instance.EnrageMode);

                    if (DraMod.Settings.Instance.EnrageBossOnly) {
                        btnEnrageBoss.Background = _grayOffColor;
                        DraMod.Settings.Instance.EnrageBossOnly = false;
                        UIControl.WritePLog("Enrage Boss Only can't be turned on when Enrage Mode is on.");
                    }
                    break;
                case "btnDamageTracker":
                    ToggleButton(ref btn, ref DraMod.Settings.Instance.DamageTracker);
                    break;
                case "btnNoDragoon":
                    ToggleButton(ref btn, ref DraMod.Settings.Instance.NoDragoon);
                    break;
                case "btnEarlyAdditions":
                    ToggleButton(ref btn, ref DraMod.Settings.Instance.EarlyAdditions);
                    break;
                case "btnAdditionLevel":
                    ToggleButton(ref btn, ref DraMod.Settings.Instance.AdditionLevel);
                    break;
                case "btnSaveHP":
                    ToggleButton(ref btn, ref DraMod.Settings.Instance.SaveHP);
                    break;
                case "btnNeverGuard":
                    ToggleButton(ref btn, ref DraMod.Settings.Instance.NeverGuard);
                    break;
                case "btnSoulEater":
                    ToggleButton(ref btn, ref DraMod.Settings.Instance.NoDecaySoulEater);
                    break;
                case "btnAspectRatio":
                    ToggleButton(ref btn, ref DraMod.Settings.Instance.AspectRatio);
                    break;
                case "btnBattleUI":
                    ToggleButton(ref btn, ref DraMod.Settings.Instance.RGBBattleUI);

                    if (DraMod.Settings.Instance.RGBBattleUI) {
                        InputWindow RGBBattleWindow = new InputWindow("RGB Battle UI Window");
                        RadioButton ColourModeSingle = new RadioButton();
                        RadioButton ColourModeCharacter = new RadioButton();
                        RadioButton ColourModeCycle = new RadioButton();
                        ColorPicker ColourPicker = new ColorPicker();

                        ColourModeSingle.IsChecked = true;
                        ColourModeSingle.Content = "Single";
                        ColourModeCharacter.Content = "Character";
                        ColourModeCycle.Content = "Cycle";

                        ColourModeSingle.Checked += new RoutedEventHandler(delegate (Object o, RoutedEventArgs r) {
                            ColourPicker.IsEnabled = true;
                        });

                        ColourModeCharacter.Checked += new RoutedEventHandler(delegate (Object o, RoutedEventArgs r) {
                            ColourPicker.IsEnabled = false;
                        });

                        ColourModeCycle.Checked += new RoutedEventHandler(delegate (Object o, RoutedEventArgs r) {
                            ColourPicker.IsEnabled = false;
                        });

                        ColourPicker.SelectedColor = Color.FromRgb(255, 0, 0);
                        ColourPicker.IsColorPalettesTabVisible = false;
                        ColourPicker.SelectedColorChanged += new RoutedPropertyChangedEventHandler<Color?>(delegate (Object o, RoutedPropertyChangedEventArgs<Color?> c) {
                            Color newColour = (Color) ColourPicker.SelectedColor;
                            newColour.A = 255;
                            ColourPicker.SelectedColor = (Color?) newColour;
                        });

                        RGBBattleWindow.AddObject(ColourPicker);
                        RGBBattleWindow.AddObject(ColourModeCycle);
                        RGBBattleWindow.AddObject(ColourModeCharacter);
                        RGBBattleWindow.AddObject(ColourModeSingle);
                        RGBBattleWindow.ShowDialog();



                        if ((bool) ColourModeSingle.IsChecked) {
                            Color colour = (Color) ColourPicker.SelectedColor;
                            DraMod.Settings.Instance.RGBBattleUIColour[0] = colour.R;
                            DraMod.Settings.Instance.RGBBattleUIColour[1] = colour.G;
                            DraMod.Settings.Instance.RGBBattleUIColour[2] = colour.B;
                        } else if ((bool) ColourModeCharacter.IsChecked) {
                            DraMod.Settings.Instance.RGBBattleUICharacter = true;
                        } else {
                            DraMod.Settings.Instance.RGBBattleUICycle = true;
                        }
                    } else {
                        DraMod.Settings.Instance.RGBBattleUICharacter = false;
                        DraMod.Settings.Instance.RGBBattleUICycle = false;
                    }
                    break;
                //Field & Battle
                case "btnSoloMode":
                    ToggleButton(ref btn, ref DraMod.Settings.Instance.SoloMode);
                    break;
                case "btnDuoMode":
                    ToggleButton(ref btn, ref DraMod.Settings.Instance.DuoMode);
                    break;
                case "btnAddPartyMembersOn":
                    ToggleButton(ref btn, ref DraMod.Settings.Instance.AlwaysAddSoloPartyMembers);
                    DraMod.Settings.Instance.AddPartyMembers = DraMod.Settings.Instance.AlwaysAddSoloPartyMembers;
                    DraMod.Settings.Instance.BtnAddPartyMembers = DraMod.Settings.Instance.AlwaysAddSoloPartyMembers;
                    DraMod.Settings.Instance.AddSoloPartyMembers = DraMod.Settings.Instance.AlwaysAddSoloPartyMembers;
                    break;
                case "btnKillBGM":
                    ToggleButton(ref btn, ref DraMod.Settings.Instance.KillBGM);
                    break;
                case "btnReduceSDEXP":
                    ToggleButton(ref btn, ref DraMod.Settings.Instance.ReduceSoloDuoEXP);
                    break;
                //Hard & Hell Mode
                //Battle Rows
                case "btnRows":
                    ToggleButton(ref btn, ref DraMod.Settings.Instance.BattleRows);
                    break;
                case "btnEATB":
                    ToggleButton(ref btn, ref DraMod.Settings.Instance.EATB);
                    if (DraMod.Settings.Instance.ATB)
                        ToggleButton(ref btnATB, ref DraMod.Settings.Instance.ATB);
                    if (DraMod.Settings.Instance.QTB)
                        ToggleButton(ref btnQTB, ref DraMod.Settings.Instance.QTB);
                    break;
                case "btnATB":
                    ToggleButton(ref btn, ref DraMod.Settings.Instance.ATB);
                    if (DraMod.Settings.Instance.EATB)
                        ToggleButton(ref btnEATB, ref DraMod.Settings.Instance.EATB);
                    if (DraMod.Settings.Instance.QTB)
                        ToggleButton(ref btnQTB, ref DraMod.Settings.Instance.QTB);
                    break;
                case "btnQTB":
                    ToggleButton(ref btn, ref DraMod.Settings.Instance.QTB);
                    if (DraMod.Settings.Instance.EATB)
                        ToggleButton(ref btnEATB, ref DraMod.Settings.Instance.EATB);
                    if (DraMod.Settings.Instance.ATB)
                        ToggleButton(ref btnATB, ref DraMod.Settings.Instance.ATB);
                    break;
                    //Turn Battle System
            }
        }

        private void ToggleButton(ref Button btn, ref bool value) {
            if (value) {
                btn.Background = _offColor;
            } else {
                btn.Background = _onColor;
            }
            value = !value;
        }

        public void GreenButton(object sender, EventArgs e) {
            Button btn = (Button) sender;
            switch (btn.Name) {
                //Field & Battle
                case "btnAddPartyMembers":
                    DraMod.Settings.Instance.BtnAddPartyMembers = true;
                    break;
                case "btnSwitchSoloChar":
                    DraMod.Settings.Instance.SwitchSlot1 = true;
                    break;
                case "btnSwitchExp":
                    DraMod.Settings.Instance.BtnSwitchExp = true;
                    break;
                case "btnUltimateBoss":
                    DraMod.UI.UltimateBoss.Setup(cboUltimateBoss.SelectedIndex);
                    break; 
                case "btnHeroTicketShop":
                    HeroTicketShop();
                    break;
                case "btnHeroItemShop":
                    HeroItemShop();
                    break;
                case "btnUltimateShop":
                    DraMod.UI.DraModShop.UltimateItemShop(lstUltimateShop.SelectedIndex);
                    break;
            }
        }

        public void ComboBox(object sender, EventArgs e) {
            ComboBox cbo = (ComboBox) sender;
            switch (cbo.Name) {
                case "cboFlowerStorm":
                    ChangeComboBox(cbo, ref DraMod.Settings.Instance.FlowerStorm);
                    break;
                case "cboAspectRatio":
                    ChangeComboBox(cbo, ref DraMod.Settings.Instance.AspectRatioMode);
                    break;
                case "cboCamera":
                    ChangeComboBox(cbo, ref DraMod.Settings.Instance.AdvancedCameraMode);
                    break;
                case "cboKillBGM":
                    ChangeComboBox(cbo, ref DraMod.Settings.Instance.KillBGMMode);
                    break;
                case "cboSoloLeader":
                    ChangeComboBox(cbo, ref DraMod.Settings.Instance.SoloLeader);
                    break;
                case "cboSwitchChar":
                    ChangeComboBox(cbo, ref DraMod.Settings.Instance.Slot1Select);
                    break;
                case "cboSwitch1":
                    ChangeComboBox(cbo, ref DraMod.Settings.Instance.SwitchEXPSlot1);
                    break;
                case "cboSwitch2":
                    ChangeComboBox(cbo, ref DraMod.Settings.Instance.SwitchEXPSlot2);
                    break;
                case "cboQTB":
                    DraMod.Settings.Instance.QTBLeader = cboQTB.SelectedIndex;
                    break;
                case "cboUltimateBoss":
                    ChangeComboBox(cbo, ref DraMod.Settings.Instance.UltimateBossSelected);
                    break;
                default:
                    if (cbo.Name.Contains("cboRow")) {
                        if (cbo.Name.Contains("Boost")) {
                            for (int i = 0; i < characters.Length; i++) {
                                if (cbo.Name.Contains(characters[i])) {
                                    ChangeComboBox(cbo, ref DraMod.Settings.Instance.BattleRowsBoost[i]);
                                }
                            }
                        } else {
                            for (int i = 0; i < characters.Length; i++) {
                                if (cbo.Name.Contains(characters[i])) {
                                    ChangeComboBox(cbo, ref DraMod.Settings.Instance.BattleRowsFormation[i]);
                                }
                            }
                        }
                    }
                    break;
            }
        }

        private void ChangeComboBox(ComboBox cbo, ref byte value) {
            value = (byte) cbo.SelectedIndex;
        }

        public void DifficultyButton(object sender, EventArgs e) {
            Button btn = (Button) sender;

            switch (btn.Name) {
                case "btnNormal":
                    btn.Background = _onColor;
                    btnNormalHard.Background = _grayOffColor;
                    btnHard.Background = _grayOffColor;
                    btnHardHell.Background = _grayOffColor;
                    btnHell.Background = _grayOffColor;
                    DragoonModifier.ChangeLoDDirectory(DraMod.Preset.Normal);
                    break;
                case "btnNormalHard":
                    btnNormal.Background = _grayOffColor;
                    btn.Background = _onColor;
                    btnHard.Background = _grayOffColor;
                    btnHardHell.Background = _grayOffColor;
                    btnHell.Background = _grayOffColor;
                    DragoonModifier.ChangeLoDDirectory(DraMod.Preset.NormalHard);
                    break;
                case "btnHard":
                    btnNormal.Background = _grayOffColor;
                    btnNormalHard.Background = _grayOffColor;
                    btn.Background = _onColor;
                    btnHardHell.Background = _grayOffColor;
                    btnHell.Background = _grayOffColor;
                    DragoonModifier.ChangeLoDDirectory(DraMod.Preset.Hard);
                    break;
                case "btnHardHell":
                    btnNormal.Background = _grayOffColor;
                    btnNormalHard.Background = _grayOffColor;
                    btnHard.Background = _grayOffColor;
                    btn.Background = _onColor;
                    btnHell.Background = _grayOffColor;
                    DragoonModifier.ChangeLoDDirectory(DraMod.Preset.HardHell);
                    break;
                case "btnHell":
                    btnNormal.Background = _grayOffColor;
                    btnNormalHard.Background = _grayOffColor;
                    btnHard.Background = _grayOffColor;
                    btnHardHell.Background = _grayOffColor;
                    btn.Background = _onColor;
                    DragoonModifier.ChangeLoDDirectory(DraMod.Preset.Hell);
                    break;
                case "btnEnrageBoss":
                    if (DraMod.Settings.Instance.EnrageBossOnly) {
                        btn.Background = _grayOffColor;
                    } else {
                        btn.Background = _onColor;
                    }
                    DraMod.Settings.Instance.EnrageBossOnly = !DraMod.Settings.Instance.EnrageBossOnly;
                    if (DraMod.Settings.Instance.EnrageMode) {
                        ToggleButton(ref btnEnrage, ref DraMod.Settings.Instance.EnrageMode);
                        UIControl.WritePLog("Enrage Mode can't be turned on when Enrage Boss Only Mode is on.");
                    }
                    break;

            }
        }

        private void Slider_ValueChanged(object sender, EventArgs e) {
            var slider = sender as Slider;
            switch (slider.Name) {
                case "sldHP":
                    DraMod.Settings.Instance.HPMulti = slider.Value;
                    break;
                case "sldATK":
                    DraMod.Settings.Instance.ATMulti = slider.Value;
                    break;
                case "sldDEF":
                    DraMod.Settings.Instance.DFMulti = slider.Value;
                    break;
                case "sldMAT":
                    DraMod.Settings.Instance.MATMulti = slider.Value;
                    break;
                case "sldMDF":
                    DraMod.Settings.Instance.MDFMulti = slider.Value;
                    break;
                case "sldSPD":
                    DraMod.Settings.Instance.SPDMulti = slider.Value;
                    break;
            }
        }

        public void HeroTicketShop() {
            int[] cost = new int[] { 15, 60, 100 };
            int[] tickets = new int[] { 1, 5, 10 };
            DraMod.UI.DraModShop.BuyTicket((uint) cost[lstTicketShop.SelectedIndex], tickets[lstTicketShop.SelectedIndex]);
        }

        public void HeroItemShop() {
            int[] cost = new int[] { 20, 40, 60, 100 };
            byte[] item = new byte[] { 0xD3, 0xDD, 0xE9, 0xEA };
            DraMod.UI.DraModShop.BuyShopItem(item[lstHeroShop.SelectedIndex], 0, cost[lstHeroShop.SelectedIndex]);
        }

        private void miLog_Click(object sender, RoutedEventArgs e) {
            rdLog.Height = miLog.Header.Equals("Expand Log") ? new GridLength(9999, GridUnitType.Star) : new GridLength(2, GridUnitType.Star);
            miLog.Header = miLog.Header.Equals("Expand Log") ? "Collapse Log" : "Expand Log";
        }
    }
}
