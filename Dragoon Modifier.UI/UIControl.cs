using Dragoon_Modifier.DraMod;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Dragoon_Modifier.UI {
    internal class UIControl : DraMod.UI.IUIControl {
        private readonly TextBlock[,] _monsterDisplay;
        private readonly TextBlock[,] _characterDisplay;
        private readonly TextBlock _glog;
        private readonly TextBlock _plog;
        private readonly TextBlock[] _fieldDisplay;
        private readonly ProgressBar _qtbBar;
        private readonly ProgressBar[] _mtbBar;
        private readonly ProgressBar[] _ctbBar;

        internal UIControl(TextBlock[,] monsterDisplay, TextBlock[,] characterDisplay, TextBlock glog, TextBlock plog, TextBlock[] fieldDisplay, ProgressBar[] turnBattleBars) {
            _monsterDisplay = monsterDisplay;
            _characterDisplay = characterDisplay;
            _glog = glog;
            _plog = plog;
            _fieldDisplay = fieldDisplay;
            _qtbBar = turnBattleBars[0];
            _mtbBar = new ProgressBar[6] { turnBattleBars[1], turnBattleBars[2], turnBattleBars[3], turnBattleBars[4], turnBattleBars[5], turnBattleBars[6] };
            _ctbBar = new ProgressBar[3] { turnBattleBars[7], turnBattleBars[8], turnBattleBars[9] };
        }
        public void UpdateMonster(int index, DraMod.UI.MonsterUpdate data) {
            Application.Current.Dispatcher.Invoke(() => {
                if (!data.Name.All(char.IsDigit)) {
                    _monsterDisplay[index, 0].Text = data.Name;
                }
                _monsterDisplay[index, 1].Text = $" {data.HP} / {data.MaxHP}";
                _monsterDisplay[index, 2].Text = $" {data.AT} / {data.MAT}";
                _monsterDisplay[index, 3].Text = $" {data.DF} / {data.MDF}";
                _monsterDisplay[index, 4].Text = $" {data.SPD}";
                _monsterDisplay[index, 5].Text = $" {data.Turn}";
            });
        }

        public void UpdateCharacter(int index, DraMod.UI.CharacterUpdate data) {
            Application.Current.Dispatcher.Invoke(() => {
                _characterDisplay[index, 0].Text = data.Name;
                _characterDisplay[index, 1].Text = $" {data.HP} / {data.MaxHP}\r\n\r\n {data.MP} / {data.MaxMP}";
                
                _characterDisplay[index, 2].Text = $" {data.AT}\r\n\r\n {data.MAT}";
                _characterDisplay[index, 3].Text = $" {data.DF}\r\n\r\n{data.MDF}";
                _characterDisplay[index, 4].Text = $" {data.A_HIT} / {data.M_HIT}\r\n\r\n {data.A_AV} / {data.M_AV}";

                _characterDisplay[index, 5].Text = $" {data.DAT}\r\n\r\n {data.DMAT}";
                _characterDisplay[index, 6].Text = $" {data.DDF}\r\n\r\n {data.DMDF}";
                _characterDisplay[index, 7].Text = $" {data.SPD}\r\n\r\n {data.SP}";
                _characterDisplay[index, 8].Text = $" {data.Turn}";
            });
        }

        public void ResetBattle() {
            Settings.Instance.UltimateBoss = false;
            Application.Current.Dispatcher.Invoke(() => {
                for (int i = 0; i < 6; i++) {
                    _monsterDisplay[i, 0].Text = "";
                    _monsterDisplay[i, 1].Text = "";
                    _monsterDisplay[i, 2].Text = "";
                    _monsterDisplay[i, 3].Text = "";
                    _monsterDisplay[i, 4].Text = "";
                    _monsterDisplay[i, 5].Text = "";
                    _mtbBar[i].Value = 0;
                }
                for (int i = 0; i < 3; i++) {
                    _characterDisplay[i, 0].Text = "";
                    _characterDisplay[i, 1].Text = "";
                    _characterDisplay[i, 2].Text = "";
                    _characterDisplay[i, 3].Text = "";
                    _characterDisplay[i, 4].Text = "";
                    _characterDisplay[i, 5].Text = "";
                    _characterDisplay[i, 6].Text = "";
                    _characterDisplay[i, 7].Text = "";
                    _characterDisplay[i, 8].Text = "";
                    _ctbBar[i].Value = 0;
                }
                _qtbBar.Value = 0;
            });
        }

        public void UpdateField(uint battleValue, uint encounterID, uint map) {
            Application.Current.Dispatcher.Invoke(() => {
                _fieldDisplay[0].Text = $"Encounter Value: {battleValue}";
                _fieldDisplay[1].Text = $"Enemy ID: {encounterID}";
                _fieldDisplay[2].Text = $"Map ID: {map}";
            });
        }

        public void WriteGLog(object text) {
            Application.Current.Dispatcher.Invoke(() => {
                _glog.Text = text.ToString();
            });
        }

        public void WritePLog(object text) {
            Application.Current.Dispatcher.Invoke(() => {
                _plog.Text = text.ToString();
            });
        }

        public void UpdateQTB(int value) {
            Application.Current.Dispatcher.Invoke(() => {
                _qtbBar.Value = value;
            });
        }

        public void UpdateMTB(int value, int slot) {
            Application.Current.Dispatcher.Invoke(() => {
                _mtbBar[slot].Value = value;
            });
        }

        public void UpdateCTB(int value, int slot) {
            Application.Current.Dispatcher.Invoke(() => {
                _ctbBar[slot].Value = value;
            });
        }

        public int GetQTB() {
            int value = 0;
            Application.Current.Dispatcher.Invoke(() => {
                value = (int) _qtbBar.Value;
            });
            return value;
        }

        public int GetMTB(int slot) {
            int value = 0;
            Application.Current.Dispatcher.Invoke(() => {
                value = (int) _mtbBar[slot].Value;
            });
            return value;
        }

        public int GetCTB(int slot) {
            int value = 0;
            Application.Current.Dispatcher.Invoke(() => {
                value = (int) _ctbBar[slot].Value;
            });
            return value;
        }
    }
}
