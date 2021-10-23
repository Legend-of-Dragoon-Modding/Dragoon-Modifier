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

        public MainWindow() {
            InitializeComponent();
            UIControl = InitUI();
            DragoonModifier = DraMod.Factory.DraMod(UIControl, Directory.GetCurrentDirectory());
            Console.SetOut(new TextBoxOutput(txtOutput));
            this.Title += DraMod.Constants.Version;
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

            for (int i = 0; i < characters.Length; i++) {
                cboSwitchChar.Items.Add(characters[i]);
                cboSwitch1.Items.Add(characters[i]);
                cboSwitch2.Items.Add(characters[i]);
            }

            cboAspectRatio.SelectedIndex = 0;
            cboCamera.SelectedIndex = 0;
            cboSoloLeader.SelectedIndex = 0;
            cboSwitchChar.SelectedIndex = 0;
            cboSwitch1.SelectedIndex = 0;
            cboSwitch2.SelectedIndex = 0;
            cboKillBGM.SelectedIndex = 1;

            TextBlock[,] monsterLables = new TextBlock[5, 6] {
                {lblEnemy1Name,  lblEnemy1HP, lblEnemy1ATK, lblEnemy1DEF, lblEnemy1SPD, lblEnemy1TRN},
                {lblEnemy2Name,  lblEnemy2HP, lblEnemy2ATK, lblEnemy2DEF, lblEnemy2SPD, lblEnemy2TRN},
                {lblEnemy3Name,  lblEnemy3HP, lblEnemy3ATK, lblEnemy3DEF, lblEnemy3SPD, lblEnemy3TRN},
                {lblEnemy4Name,  lblEnemy4HP, lblEnemy4ATK, lblEnemy4DEF, lblEnemy4SPD, lblEnemy4TRN},
                {lblEnemy5Name,  lblEnemy5HP, lblEnemy5ATK, lblEnemy5DEF, lblEnemy5SPD, lblEnemy5TRN}
            };
            TextBlock[,] characterLables = new TextBlock[3, 9] {
                { lblCharacter1Name, lblCharacter1HMP, lblCharacter1ATK, lblCharacter1DEF, lblCharacter1VHIT, lblCharacter1DATK, lblCharacter1DDEF, lblCharacter1SPD, lblCharacter1TRN },
                { lblCharacter2Name, lblCharacter2HMP, lblCharacter2ATK, lblCharacter2DEF, lblCharacter2VHIT, lblCharacter2DATK, lblCharacter2DDEF, lblCharacter2SPD, lblCharacter2TRN },
                { lblCharacter3Name, lblCharacter3HMP, lblCharacter3ATK, lblCharacter3DEF, lblCharacter3VHIT, lblCharacter3DATK, lblCharacter3DDEF, lblCharacter3SPD, lblCharacter3TRN }
            };
            TextBlock[] fieldLables = new TextBlock[3] {
                lblEncounter, lblEnemyID, lblMapID
            };

            return Factory.UIControl(monsterLables, characterLables, stsGame, stsProgram, fieldLables);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
        }

        private void Window_Closing(object sender, CancelEventArgs e) {
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

        private void miAttach_Click(object sender, RoutedEventArgs e) {
            if (DraMod.Constants.Run) {
                DraMod.Constants.Run = false;
                miAttach.Header = "Attach";
            } else {
                if (DragoonModifier.Attach(DraMod.Constants.EmulatorName, DraMod.Constants.PreviousOffset)) {
                    miAttach.Header = "Detach";
                    UIControl.WriteGLog("Game Log");
                    UIControl.WritePLog("Program Log");
                }
            }
        }

        public void SwitchButton(object sender, EventArgs e) {
            Button btn = (Button) sender;
            switch (btn.Name) {
                //Field
                case "btnSaveAnywhere":
                    ToggleButton(ref btn, ref DraMod.Settings.SaveAnywhere);
                    break;
                case "btnCharmPotion":
                    ToggleButton(ref btn, ref DraMod.Settings.AutoCharmPotion);
                    break;
                case "btnTextSpeed":
                    ToggleButton(ref btn, ref DraMod.Settings.IncreaseTextSpeed);
                    break;
                case "btnAutoText":
                    ToggleButton(ref btn, ref DraMod.Settings.AutoAdvanceText);
                    break;
                //Battle
                case "btnRemoveCaps":
                    ToggleButton(ref btn, ref DraMod.Settings.RemoveDamageCaps);
                    break;
                case "btnElementalBomb":
                    ToggleButton(ref btn, ref DraMod.Settings.ElementalBomb);
                    break;
                case "btnHPNames":
                    ToggleButton(ref btn, ref DraMod.Settings.MonsterHPAsNames);
                    break;
                case "btnEnrage":
                    ToggleButton(ref btn, ref DraMod.Settings.EnrageMode);

                    if (DraMod.Settings.EnrageBossOnly) {
                        btnEnrageBoss.Background = _grayOffColor;
                        DraMod.Settings.EnrageBossOnly = false;
                        UIControl.WritePLog("Enrage Boss Only can't be turned on when Enrage Mode is on.");
                    }
                    break;
                case "btnDamageTracker":
                    ToggleButton(ref btn, ref DraMod.Settings.DamageTracker);
                    break;
                case "btnNoDragoon":
                    ToggleButton(ref btn, ref DraMod.Settings.NoDragoon);
                    break;
                case "btnEarlyAdditions":
                    ToggleButton(ref btn, ref DraMod.Settings.EarlyAdditions);
                    break;
                case "btnAdditionLevel":
                    ToggleButton(ref btn, ref DraMod.Settings.AdditionLevel);
                    break;
                case "btnSaveHP":
                    ToggleButton(ref btn, ref DraMod.Settings.SaveHP);
                    break;
                case "btnNeverGuard":
                    ToggleButton(ref btn, ref DraMod.Settings.NeverGuard);
                    break;
                case "btnSoulEater":
                    ToggleButton(ref btn, ref DraMod.Settings.NoDecaySoulEater);
                    break;
                case "btnAspectRatio":
                    ToggleButton(ref btn, ref DraMod.Settings.AspectRatio);
                    break;
                //Field & Battle
                case "btnSoloMode":
                    ToggleButton(ref btn, ref DraMod.Settings.SoloMode);
                    break;
                case "btnDuoMode":
                    ToggleButton(ref btn, ref DraMod.Settings.DuoMode);
                    break;
                case "btnAddPartyMembersOn":
                    ToggleButton(ref btn, ref DraMod.Settings.AlwaysAddSoloPartyMembers);
                    DraMod.Settings.AddPartyMembers = DraMod.Settings.AlwaysAddSoloPartyMembers;
                    DraMod.Settings.BtnAddPartyMembers = DraMod.Settings.AlwaysAddSoloPartyMembers;
                    DraMod.Settings.AddSoloPartyMembers = DraMod.Settings.AlwaysAddSoloPartyMembers;
                    break;
                case "btnKillBGM":
                    ToggleButton(ref btn, ref DraMod.Settings.KillBGM);
                    break;
                case "btnReduceSDEXP":
                    ToggleButton(ref btn, ref DraMod.Settings.ReduceSoloDuoEXP);
                    break;
                    //Hard & Hell Mode

                    //Battle Rows

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
                    DraMod.Settings.BtnAddPartyMembers = true;
                    break;
                case "btnSwitchSoloChar":
                    DraMod.Settings.SwitchSlot1 = true;
                    break;
                case "btnSwitchExp":
                    DraMod.Settings.BtnSwitchExp = true;
                    break;
            }
        }

        public void ComboBox(object sender, EventArgs e) {
            ComboBox cbo = (ComboBox) sender;
            switch (cbo.Name) {
                case "cboFlowerStorm":
                    ChangeComboBox(cbo, ref DraMod.Settings.FlowerStorm);
                    break;
                case "cboAspectRatio":
                    ChangeComboBox(cbo, ref DraMod.Settings.AspectRatioMode);
                    break;
                case "cboCamera":
                    ChangeComboBox(cbo, ref DraMod.Settings.AdvancedCameraMode);
                    break;
                case "cboKillBGM":
                    ChangeComboBox(cbo, ref DraMod.Settings.KillBGMMode);
                    break;
                case "cboSoloLeader":
                    ChangeComboBox(cbo, ref DraMod.Settings.SoloLeader);
                    break;
                case "cboSwitchChar":
                    ChangeComboBox(cbo, ref DraMod.Settings.Slot1Select);
                    break;
                case "cboSwitch1":
                    ChangeComboBox(cbo, ref DraMod.Settings.SwitchEXPSlot1);
                    break;
                case "cboSwitch2":
                    ChangeComboBox(cbo, ref DraMod.Settings.SwitchEXPSlot2);
                    break;
            }
        }

        private void ChangeComboBox(ComboBox cbo, ref byte value) {
            value = (byte) cbo.SelectedIndex;
        }

        public void DifficultyButton(object sender, EventArgs e) {
            Button btn = (Button) sender;
            DraMod.Settings.DualDifficulty = false;

            switch (btn.Name) {
                case "btnNormal":
                    btn.Background = _onColor;
                    btnNormalHard.Background = _grayOffColor;
                    btnHard.Background = _grayOffColor;
                    btnHardHell.Background = _grayOffColor;
                    btnHell.Background = _grayOffColor;
                    DragoonModifier.ChangeLoDDirectory("US_Base");
                    DraMod.Settings.Difficulty = "Normal";
                    break;
                case "btnNormalHard":
                    btnNormal.Background = _grayOffColor;
                    btn.Background = _onColor;
                    btnHard.Background = _grayOffColor;
                    btnHardHell.Background = _grayOffColor;
                    btnHell.Background = _grayOffColor;
                    DragoonModifier.ChangeLoDDirectory("Hard_Mode");
                    DraMod.Settings.DualDifficulty = true;
                    DraMod.Settings.Difficulty = "Hard";
                    break;
                case "btnHard":
                    btnNormal.Background = _grayOffColor;
                    btnNormalHard.Background = _grayOffColor;
                    btn.Background = _onColor;
                    btnHardHell.Background = _grayOffColor;
                    btnHell.Background = _grayOffColor;
                    DragoonModifier.ChangeLoDDirectory("Hard_Mode");
                    DraMod.Settings.Difficulty = "Hard";
                    break;
                case "btnHardHell":
                    btnNormal.Background = _grayOffColor;
                    btnNormalHard.Background = _grayOffColor;
                    btnHard.Background = _grayOffColor;
                    btn.Background = _onColor;
                    btnHell.Background = _grayOffColor;
                    DragoonModifier.ChangeLoDDirectory("Hell_Mode");
                    DraMod.Settings.DualDifficulty = true;
                    DraMod.Settings.Difficulty = "Hell";
                    break;
                case "btnHell":
                    btnNormal.Background = _grayOffColor;
                    btnNormalHard.Background = _grayOffColor;
                    btnHard.Background = _grayOffColor;
                    btnHardHell.Background = _grayOffColor;
                    btn.Background = _onColor;
                    DragoonModifier.ChangeLoDDirectory("Hell_Mode");
                    DraMod.Settings.Difficulty = "Hell";
                    break;
                case "btnEnrageBoss":
                    if (DraMod.Settings.EnrageBossOnly) {
                        btn.Background = _grayOffColor;
                    } else {
                        btn.Background = _onColor;
                    }
                    DraMod.Settings.EnrageBossOnly = !DraMod.Settings.EnrageBossOnly;
                    if (DraMod.Settings.EnrageMode) {
                        ToggleButton(ref btnEnrage, ref DraMod.Settings.EnrageMode);
                        UIControl.WritePLog("Enrage Mode can't be turned on when Enrage Boss Only Mode is on.");
                    }
                    break;

            }

            //DragoonModifier.ChangeLoDDirectory(DraMod.Settings.Mod);
        }

        private void Slider_ValueChanged(object sender, EventArgs e) {
            var slider = sender as Slider;
            switch (slider.Name) {
                case "sldHP":
                    DraMod.Settings.HPMulti = slider.Value;
                    break;
                case "sldATK":
                    DraMod.Settings.ATMulti = slider.Value;
                    break;
                case "sldDEF":
                    DraMod.Settings.DFMulti = slider.Value;
                    break;
                case "sldMAT":
                    DraMod.Settings.MATMulti = slider.Value;
                    break;
                case "sldMDF":
                    DraMod.Settings.MDFMulti = slider.Value;
                    break;
                case "sldSPD":
                    DraMod.Settings.SPDMulti = slider.Value;
                    break;
            }
        }
    }
}
