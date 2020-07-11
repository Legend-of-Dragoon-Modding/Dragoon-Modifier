using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace Dragoon_Modifier {
    public partial class InputWindow {
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public InputWindow() {
            InitializeComponent();
        }
        public InputWindow(bool enter) {
            InitializeComponent();
            btnOk.IsDefault = enter;
        }

        public InputWindow(string title) {
            InitializeComponent();
            this.Title = title;
        }

        public InputWindow(string title, bool enter) {
            InitializeComponent();
            this.Title = title;
            btnOk.IsDefault = enter;
        }

        protected override void OnSourceInitialized(EventArgs e) {
            /*base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);*/
        }

        public void AddObject(UIElement add) {
            pnlMain.Children.Insert(0, add);
        }

        public void AddTextBlock(string text) {
            TextBlock txtBlock = new TextBlock();
            txtBlock.Text = text;
            txtBlock.HorizontalAlignment = HorizontalAlignment.Center;
            pnlMain.Children.Insert(0, txtBlock);
        }

        public void AddObjectF(UIElement add) {
            pnlMain.Children.Insert(pnlMain.Children.Count - 1, add);
        }

        public void AddTextBlockF(string text) {
            TextBlock txtBlock = new TextBlock();
            txtBlock.Text = text;
            txtBlock.HorizontalAlignment = HorizontalAlignment.Center;
            pnlMain.Children.Insert(pnlMain.Children.Count - 1, txtBlock);
        }

        public UIElement GetElement(int index) {
            return pnlMain.Children[index];
        }

        private void btnOk_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }
    }
}
