using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using Galateia.ConfigWindow;
using Point = System.Drawing.Point;

namespace Galateia
{
    /// <summary>
    ///     MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private NotifyIcon _notifyIcon;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _notifyIcon = new NotifyIcon
            {
                Text = Title,
                Icon = Properties.Resources.galateia,
                Visible = true
            };
            _notifyIcon.MouseClick += notifyIcon_MouseClick;

            Dispatcher.BeginInvoke((Action) Hide);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _notifyIcon.Dispose();
            App.Current.Shutdown(0);
        }

        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point pos = System.Windows.Forms.Cursor.Position;
                Rectangle wa = Screen.FromPoint(e.Location).WorkingArea;
                // どちら側に表示すべきか判定して，位置を指定．
                bool left = wa.Right - pos.X < ActualWidth;
                bool top = wa.Bottom - pos.Y < ActualHeight;
                Left = left ? pos.X - ActualWidth : pos.X;
                Top = top ? pos.Y - ActualHeight : pos.Y;
                // 最前面をとる
                Topmost = false;
                Topmost = true;
                Show();
                Activate();
            }
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Hide();
        }

        private void GlobalConfigButton_Click(object sender, RoutedEventArgs e)
        {
            var configWnd = new GlobalConfigWindow(App.Current.GlobalConfig) {ShowActivated = true};
            configWnd.BeforeSubstitute += (o, args) => App.Current.Renderer.Continue = false;
            configWnd.AfterSubstitute += (o, args) => App.Current.Renderer.Continue = true;
            configWnd.Show();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}