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

        public static DraMod.UI.IUIControl UIControl;

        public MainWindow() {
            InitializeComponent();
            InitUI();
            this.Title += DraMod.Constants.Version;
            Console.SetOut(new TextBoxOutput(txtOutput));
            Debug.Listeners.Add(new DebugOutput(txtOutput));

            DraMod.Setup.Run(UIControl);
        }

        private void InitUI() {
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

            UIControl = Factory.UIControl(monsterLables, characterLables, stsGame, stsProgram, fieldLables);

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

            cboSwitch1.Items.Add("Dart");
            cboSwitch1.Items.Add("Lavitz");
            cboSwitch1.Items.Add("Shana");
            cboSwitch1.Items.Add("Rose");
            cboSwitch1.Items.Add("Haschel");
            cboSwitch1.Items.Add("Albert");
            cboSwitch1.Items.Add("Meru");
            cboSwitch1.Items.Add("Kongol");
            cboSwitch1.Items.Add("Miranda");

            cboSwitch2.Items.Add("Dart");
            cboSwitch2.Items.Add("Lavitz");
            cboSwitch2.Items.Add("Shana");
            cboSwitch2.Items.Add("Rose");
            cboSwitch2.Items.Add("Haschel");
            cboSwitch2.Items.Add("Albert");
            cboSwitch2.Items.Add("Meru");
            cboSwitch2.Items.Add("Kongol");
            cboSwitch2.Items.Add("Miranda");

            cboElement.Items.Add("Fire");
            cboElement.Items.Add("Water");
            cboElement.Items.Add("Wind");
            cboElement.Items.Add("Earth");
            cboElement.Items.Add("Dark");
            cboElement.Items.Add("Light");
            cboElement.Items.Add("Thunder");

            cboQTB.Items.Add("Dart");
            cboQTB.Items.Add("Lavitz");
            cboQTB.Items.Add("Shana");
            cboQTB.Items.Add("Rose");
            cboQTB.Items.Add("Haschel");
            cboQTB.Items.Add("Albert");
            cboQTB.Items.Add("Meru");
            cboQTB.Items.Add("Kongol");
            cboQTB.Items.Add("Miranda");

            cboFlowerStorm.Items.Add("2 Turns (40 MP)");
            cboFlowerStorm.Items.Add("3 Turns (60 MP)");
            cboFlowerStorm.Items.Add("4 Turns (80 MP)");
            cboFlowerStorm.Items.Add("5 Turns (100 MP)");

            cboReaderUIRemoval.Items.Add("None");
            cboReaderUIRemoval.Items.Add("Remove Pictures and Stats");
            cboReaderUIRemoval.Items.Add("Remove Entire UI");

            cboReaderOnHotkey.Items.Add("None");
            cboReaderOffHotkey.Items.Add("None");
            cboReaderFieldHotkey.Items.Add("None");

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

            cboHelpTopic.Items.Add("General");
            cboHelpTopic.Items.Add("Battle Stats Tab");
            cboHelpTopic.Items.Add("Difficulty Tab");
            cboHelpTopic.Items.Add("Enhancements Tab I");
            cboHelpTopic.Items.Add("Enhancements Tab II");
            cboHelpTopic.Items.Add("Enhancements Tab III");
            cboHelpTopic.Items.Add("Ultimate Boss");
            cboHelpTopic.Items.Add("Shop Tab");
            cboHelpTopic.Items.Add("Reader Tab");
            cboHelpTopic.Items.Add("Settings Tab");
            cboHelpTopic.Items.Add("Hotkeys");
            cboHelpTopic.Items.Add("How To");

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

            cboSoloLeader.SelectedIndex = 0;
            cboAspectRatio.SelectedIndex = 0;
            cboCamera.SelectedIndex = 0;
            cboKillBGM.SelectedIndex = 1;
            cboSwitchChar.SelectedIndex = 0;
            cboElement.SelectedIndex = 0;
            cboUltimateBoss.SelectedIndex = 0;
            cboSwitch1.SelectedIndex = 0;
            cboSwitch2.SelectedIndex = 1;
            cboQTB.SelectedIndex = 0;
            cboFlowerStorm.SelectedIndex = 0;
            cboReaderUIRemoval.SelectedIndex = 0;
            cboReaderOnHotkey.SelectedIndex = 0;
            cboReaderOffHotkey.SelectedIndex = 0;
            cboReaderFieldHotkey.SelectedIndex = 0;
            cboHelpTopic.SelectedIndex = 0;
        }

        private void miAttach_Click(object sender, RoutedEventArgs e) {
          
        }

        private void miEmulator_Click(object sender, RoutedEventArgs e) {
  
        }

        public void SetupEmulator(bool onOpen) {
       
        }

        public void SetupScripts() {

        }

        private void miRegion_Click(object sender, RoutedEventArgs e) {
  
        }

        private void miSaveSlot_Click(object sender, RoutedEventArgs e) {
   
        }

        private void miLog_Click(object sender, RoutedEventArgs e) {

        }

        private void btnField_Click(object sender, RoutedEventArgs e) {

        }

        private void btnBattle_Click(object sender, RoutedEventArgs e) {
  
        }

        private void btnHotkeys_Click(object sender, RoutedEventArgs e) {

        }

        private void btnOther_Click(object sender, RoutedEventArgs e) {
       
        }

        private void lstField_ScrollChanged(object sender, ScrollChangedEventArgs e) {

        }

        private void lstBattle_ScrollChanged(object sender, ScrollChangedEventArgs e) {
  
        }

        private void lstHotkey_ScrollChanged(object sender, ScrollChangedEventArgs e) {
 
        }

        private void lstOther_ScrollChanged(object sender, ScrollChangedEventArgs e) {
 
        }

        private void lstField_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {

        }

        private void lstBattle_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {

        }

        private void lstHotkey_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
  
        }

        private void lstOther_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
      
        }

        private void miNew_Click(object sender, RoutedEventArgs e) {

        }

        private void miOpen_Click(object sender, RoutedEventArgs e) {

        }

        private void miSave_Click(object sender, RoutedEventArgs e) {
            
        }

        private void miOpenPreset_Click(object sender, RoutedEventArgs e) {
            
        }

        private void miPresetHotkeys_Click(object sender, RoutedEventArgs e) {
            
        }

        private void miDeleteSave_Click(object sender, RoutedEventArgs e) {
 
        }

        private void miEatbSound_Click(object sender, RoutedEventArgs e) {
            
        }

        private void miManualOffset_Click(object sender, RoutedEventArgs e) {
            
        }


        private void miModOptions_Click(object sender, RoutedEventArgs e) {
            
        }

        private void miAuthor_Click(object sender, RoutedEventArgs e) {
         
        }

        private void miCredits_Click(object sender, RoutedEventArgs e) {
            
        }

        private void miCredits1_Click(object sender, RoutedEventArgs e) {
            
        }

        private void miCredits2_Click(object sender, RoutedEventArgs e) {
        }

        private void miCredits3_Click(object sender, RoutedEventArgs e) {
            
        }

        private void miCredits4_Click(object sender, RoutedEventArgs e) {
            
        }

        private void miVersion_Click(object sender, RoutedEventArgs e) {
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
        }

        public void SwitchButton(object sender, EventArgs e) {
            Button btn = (Button) sender;
            switch (btn.Name) {
                case "btnKillBGM":
                    ToggleButton(ref btn, ref DraMod.Settings.KillBGM);
                    break;
                case "btnAutoTransform":
                    ToggleButton(ref btn, ref DraMod.Settings.AutoTransform);
                    break;
                case "btnSaveAnywhere":
                    ToggleButton(ref btn, ref DraMod.Settings.SaveAnywhere);
                    break;
                case "btnCharmPotion":
                    ToggleButton(ref btn, ref DraMod.Settings.AutoCharmPotion);
                    break;
                case "btnRemoveCaps":
                    ToggleButton(ref btn, ref DraMod.Settings.RemoveDamageCaps);
                    break;
                case "btnHPNames":
                    ToggleButton(ref btn, ref DraMod.Settings.MonsterHPAsNames);
                    break;
                case "btnNeverGuard":
                    ToggleButton(ref btn, ref DraMod.Settings.NeverGuard);
                    break;
                case "btnSoulEater":
                    ToggleButton(ref btn, ref DraMod.Settings.NoDecaySoulEater);
                    break;
            }
        }

        private void ToggleButton(ref Button btn, ref bool value) {
            if (!value) {
                btn.Background = _onColor;
            } else {
                btn.Background = _offColor;
            }
            value = !value;
        }

        public void GreenButton(object sender, EventArgs e) {

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
                case "cboKillBGM":
                    ChangeComboBox(cbo, ref DraMod.Settings.KillBGMMode);
                    break;
            }
        }

        private void ChangeComboBox(ComboBox cbo, ref byte value) {
            value = (byte) cbo.SelectedIndex;
        }

        public void DifficultyButton(object sender, EventArgs e) {

        }

        private void Slider_ValueChanged(object sender, EventArgs e) {

        }

        private void Window_Closing(object sender, CancelEventArgs e) {

        }

        private void Window_Closed(object sender, EventArgs e) {
        
        }

        public void CloseEmulator() {

        }
    }
}
