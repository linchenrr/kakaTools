using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
            InitializeComponent();
        }

        private List<RemindRunner> runners = new List<RemindRunner>();
        private string exePath;

        //MediaPlayer player;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            StartUpDir = Path.GetDirectoryName(exePath);


            //player = new MediaPlayer();
            //player.Open(new Uri(Path.Combine(StartUpDir, "test.mp3")));
            //player.Play();


            this.MouseDown += (x, y) =>
            {
                if (y.LeftButton == MouseButtonState.Pressed)
                {
                    this.DragMove();
                }
            };


            //string xaml = System.Windows.Markup.XamlWriter.Save(txt);
            //Label newLabel = System.Windows.Markup.XamlReader.Parse(xaml) as Label;
            //canvas.Children.Add(newLabel);
            //Canvas.SetTop(newLabel, 20);
            //newLabel.Content = "sadsad萨达222";


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
                    item.ShowBalloon = ShowNotify;
                    item.OnRemindTimeUpdate = OnRemindTimeUpdate;

                    var runner = new RemindRunner();
                    if (runner.Run(item))
                    {
                        var xaml = System.Windows.Markup.XamlWriter.Save(orgLabel);
                        var newLabel = System.Windows.Markup.XamlReader.Parse(xaml) as Label;
                        canvas.Children.Add(newLabel);
                        Canvas.SetTop(newLabel, i * 30 + 20);
                        item.Txt = newLabel;
                        newLabel.Visibility = Visibility.Visible;

                        runners.Add(runner);
                    }
                }



                UpdateStartUp();

                if (config.HideOnStartUp)
                {
                    //this.Hide();
                    //HideAsync();
                }
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

            this.Dispatcher.Invoke(new AsynUpdateUI(delegate ()
            {
                runner.Data.Txt.Content = $"{time.ToShortDateString()} {time.ToShortTimeString()} {runner.Data.Text}";
            }));

        }

        delegate void AsynUpdateUI();
        private void ShowNotify(string text)
        {
            MessageBox.Show(text, "定时提醒");
            /*
             this.Invoke(new AsynUpdateUI(delegate ()
             {
                 MessageBox.Show(text, "定时提醒");
                 //notifyIcon.ShowBalloonTip(0, "定时提醒", text, ToolTipIcon.Info);
             }));
            */
        }

        private void RunExit(int exitCode = 0)
        {
            this.Close();
            Environment.Exit(exitCode);
        }

        private string Regpath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        private string keyName = "XiaoKaReminder";
        private void SetStartUp(bool runOnStartUp)
        {
            var path = exePath;
            var Rkey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(Regpath, true);

            if (runOnStartUp)
            {
                if (Rkey == null)
                {
                    Rkey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(Regpath);
                }
                Rkey.SetValue(keyName, path);
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
                if (keyValue != null && keyValue != path)
                    Rkey.SetValue(keyName, path);
            }
            cb_autoRun.Checked += cb_autoRun_CheckedChanged;
        }

        private void cb_autoRun_CheckedChanged(object sender, EventArgs e)
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

        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}
