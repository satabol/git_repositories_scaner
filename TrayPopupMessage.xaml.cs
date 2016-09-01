using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace git_repositories_scanner
{
    /// <summary>
    /// Interaction logic for TrayPopupMessage.xaml
    /// </summary>
    public partial class TrayPopupMessage : UserControl
    {
        public string path = null;
        public TaskbarIcon tb = null;
        private System.Timers.Timer temp = null;

        public enum ControlButtons
        {
            None = 0, Close = 4
        };

        private void init(string _path, TaskbarIcon _tb, ControlButtons _buttons)
        {
            tb = _tb;

            this.path = _path;
            this.text_message.Text = _path;

            temp = new System.Timers.Timer();
            temp.Interval = 4000;
            temp.Elapsed += new System.Timers.ElapsedEventHandler(customballoon_close);

            this.MouseEnter += (sender, args) =>
            {
                tb.ResetBalloonCloseTimer();
                //this.Visibility = Visibility.Hidden;
                if (temp.Enabled)
                {
                    temp.Stop();
                }
            };
            this.MouseLeave += (sender, args) =>
            {
                temp.Start();
            };
            /*
            this.MouseDown += (sender, args) =>
            {
                tb.CustomBalloon.IsOpen = false;
                //this.Visibility = Visibility.Hidden;
                App.gotoPathByWindowsExplorer(path, wType);
            };
            //*/
            this.ToolTipClosing += (sender, args) =>
            {
                temp.Stop();
            };
        }

        public TrayPopupMessage(string _path, TaskbarIcon _tb, ControlButtons _buttons)
        {
            InitializeComponent();
            init(_path, _tb, _buttons);
        }
        public TrayPopupMessage(string _path, string _title, TaskbarIcon _tb, ControlButtons _buttons)
        {
            InitializeComponent();
            this.title.Text = _title;
            init(_path, _tb, _buttons);
        }


        public void customballoon_close(object sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate  // http://stackoverflow.com/questions/2329978/the-calling-thread-must-be-sta-because-many-ui-components-require-this#2329978
            {
                // popup_test.Visibility = Visibility.Hidden;
                //this.Visibility = Visibility.Hidden;
                tb.CustomBalloon.IsOpen = false;
            });
            System.Timers.Timer temp = ((System.Timers.Timer)sender);
            temp.Stop();
        }

        private void btn_close_window_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }
    }
}
