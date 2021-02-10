using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using static XReminder.GlobalValues;

namespace XReminder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            //WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
        }

        private List<RemindRunner> runners = new List<RemindRunner>();
        private string exePath;
        private const string AutoRunParam = "-autorun";
        //MediaPlayer player;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            StartUpDir = Path.GetDirectoryName(exePath);
            Environment.CurrentDirectory = StartUpDir;

            RegKey.Init(@"SOFTWARE\XReminder");
            //test
            //RegKey.Instance.SetBool("bb", true);
            //RegKey.Instance.SetString("str", "sadsadas阿斯顿撒sad");
            //RegKey.Instance.SetInt("int", -85);

            //var bb = RegKey.Instance.GetBool("bb");
            //var str = RegKey.Instance.GetString("str");
            //var ii = RegKey.Instance.GetInt("int");


            this.MouseDown += (x, y) =>
            {
                if (y.LeftButton == MouseButtonState.Pressed)
                {
                    this.DragMove();
                }
            };


#if DEBUG

#else
            btn_test.Visibility = Visibility.Hidden;
#endif


            foreach (var item in notifyIcon.ContextMenu.Items)
            {
                var menuItem = item as MenuItem;
                if (menuItem == null)
                    continue;
                menuItem.Click += OnMenuItemClick;
            }
            //var menuItem = (MenuItem)notifyIcon.ContextMenu.Items[0];
            //menuItem.Click += OnMenuItemClick;

            try
            {
                //var testJson = RemindConfig.MakeDefaultConfig();
                //var config = JsonConvert.DeserializeObject<RemindConfig>(testJson);

                var json = File.ReadAllText("config.json", new UTF8Encoding(false));
                var config = JsonConvert.DeserializeObject<RemindConfig>(json);

                RemindRunner.CheckInterval = config.CheckInterval * 1000;

                orgLabel.Visibility = Visibility.Hidden;
                for (int i = 0; i < config.Items.Count; i++)
                {
                    var item = config.Items[i];
                    if (item.IsTestItem)
                    {
#if DEBUG
                        item.IsActive = true;
                        item.StartTime = DateTime.Now.AddSeconds(10d);
#endif
                    }

                    item.ShowBalloon = ShowNotify;
                    item.OnRemindTimeUpdate = OnRemindTimeUpdate;

                    var runner = new RemindRunner();
                    var startY = Canvas.GetTop(orgLabel);
                    if (runner.Run(item))
                    {
                        var xaml = System.Windows.Markup.XamlWriter.Save(orgLabel);
                        var newLabel = System.Windows.Markup.XamlReader.Parse(xaml) as Label;
                        canvas.Children.Add(newLabel);
                        Canvas.SetTop(newLabel, i * 30 + startY);
                        item.Txt = newLabel;
                        newLabel.Visibility = Visibility.Visible;

                        runners.Add(runner);
                    }
                }

                UpdateStartUp();

                var hideOnStartUp = RegKey.Instance.GetBool(RegKey.Key_HideOnStartUp);
                cb_hideOnStartUp.IsChecked = hideOnStartUp;
                if (hideOnStartUp)
                {
                    //var args = Environment.GetCommandLineArgs();
                    //foreach (var item in args)
                    //{
                    //    if (item == AutoRunParam)
                    //        this.Hide();
                    //}
                    this.Hide();
                    //HideAsync();
                }

                CheckUpdate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($@"初始化错误,
{ex.Message}");

                RunExit(1);
            }
        }

        private void OnMenuItemClick(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var par = (string)menuItem.CommandParameter;

            switch (int.Parse(par))
            {
                case 0:
                    RunExit();
                    break;

                case 1:
                    this.Show();
                    this.Activate();
                    break;
            }
        }

        private async void HideAsync()
        {
            await Task.Delay(1);
            this.Hide();
        }

        public void OnRemindTimeUpdate(RemindRunner runner)
        {
            var time = runner.remindTime;

            this.Dispatcher.Invoke(delegate ()
            {
                runner.Data.Txt.Content = $"{time.ToShortDateString()} {time.ToShortTimeString()} {runner.Data.Text}";
            });

        }

        private void ShowNotify(string text)
        {
            //MessageBox.Show(text, "定时提醒");
            this.Dispatcher.Invoke(delegate ()
            {
                var balloon = new FancyBalloon();
                balloon.SetInfo(text);

                //notifyIcon.ShowCustomBalloon(balloon, PopupAnimation.Slide, 4000);
                notifyIcon.ShowCustomBalloon(balloon, PopupAnimation.Slide, null);
            });
        }

        private void RunExit(int exitCode = 0)
        {
            //notifyIcon.ContextMenu.Visibility = Visibility.Hidden;
            notifyIcon.Dispose();
            this.Close();
            Environment.Exit(exitCode);
        }

        private string Regpath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        private string keyName = "XiaoKaReminder";
        private string commandLine => $"{exePath} {AutoRunParam}";
        private void SetStartUp(bool runOnStartUp)
        {
            var Rkey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(Regpath, true);

            if (runOnStartUp)
            {
                if (Rkey == null)
                {
                    Rkey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(Regpath);
                }
                Rkey.SetValue(keyName, commandLine);
            }
            else
            {
                if (Rkey != null)
                {
                    Rkey.DeleteValue(keyName, false);
                }
            }
        }

        private void UpdateStartUp()
        {
            var path = exePath;
            var Rkey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(Regpath, true);

            cb_autoRun.IsChecked = false;
            if (Rkey != null)
            {
                var keyValue = Rkey.GetValue(keyName) as string;
                cb_autoRun.IsChecked = keyValue != null;
                if (keyValue != null && keyValue != commandLine)
                    Rkey.SetValue(keyName, commandLine);
            }
        }

        private void cb_autoRun_Click(object sender, RoutedEventArgs e)
        {
            SetStartUp(cb_autoRun.IsChecked.Value);
        }

        private void notifyIcon_TrayLeftMouseUp(object sender, RoutedEventArgs e)
        {

        }

        private void notifyIcon_TrayLeftMouseDown(object sender, RoutedEventArgs e)
        {
            if (this.IsVisible)
            {
                this.Hide();
            }
            else
            {
                this.Show();
                this.Activate();
            }
        }

        private void btn_test_Click(object sender, RoutedEventArgs e)
        {
            var balloon = new FancyBalloon();
            balloon.SetInfo("Custom Balloon");

            //notifyIcon.ShowCustomBalloon(balloon, PopupAnimation.Slide, 4000);
            notifyIcon.ShowCustomBalloon(balloon, PopupAnimation.Slide, null);
            //notifyIcon.ShowBalloonTip("111", "222奥术大师", Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Info);
        }

        private void btn_min_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            RunExit();
        }

        private void btn_about_Click(object sender, RoutedEventArgs e)
        {
            btn_about.ContextMenu.IsOpen = true;
        }

        private void CheckUpdate()
        {
            var lastCheckTime = RegKey.Instance.GetDateTime(RegKey.Key_LastCheckUpdateTime);
            if (lastCheckTime != null)
            {
                if ((DateTime.Now - lastCheckTime.Value).TotalDays < 1)
                    return;
            }

            RegKey.Instance.SetDateTime(RegKey.Key_LastCheckUpdateTime, DateTime.Now);

            //var jj = Application.Current.Resources["XReminderUpdate"];
            var json = Properties.Resources.XReminderUpdate;
            //var json = File.ReadAllText(UpdateChecker.UpdateInfoFileName, new UTF8Encoding(false));
            var localInfo = JsonConvert.DeserializeObject<VersionInfo>(json);

            UpdateChecker.RemoteCheckAsync(info =>
            {
                if (localInfo.Version != info.Version)
                {
                    this.Show();
                    var aboutWindow = new UpdateWindow();
                    aboutWindow.SetInfo(info);
                    aboutWindow.Owner = this;
                    aboutWindow.ShowDialog();
                }
            });
        }

        private void cb_hideOnStartUp_Click(object sender, RoutedEventArgs e)
        {
            RegKey.Instance.SetBool(RegKey.Key_HideOnStartUp, cb_hideOnStartUp.IsChecked.Value);
        }

        private void menuItem_about_Click(object sender, RoutedEventArgs e)
        {
            var aboutWindow = new AboutWindow();
            aboutWindow.Owner = this;
            aboutWindow.ShowDialog();
            //aboutWindow.Owner = this;
        }

        private void menuItem_help_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", Path.Combine(StartUpDir, "使用说明.txt"));
        }
    }
}
