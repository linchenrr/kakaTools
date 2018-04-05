using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace ping
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ping = new Ping();
            ping.PingCompleted += PingCompleted;
            timer = new DispatcherTimer();
            timer.Tick += onTimer;
            updateInterval();
        }

        private void updateInterval()
        {
            if (txt_interval == null)
                return;
            var interval = (int)(1000 * sl_interval.Value);
            txt_interval.Content = "interval: " + interval + "ms";
            timer.Interval = new TimeSpan(0, 0, 0, 0, interval);
        }

        private bool isStart;
        private DispatcherTimer timer;
        private Ping ping;

        private void changeStatus()
        {
            if (isStart)
            {
                button.Content = "开始";
                timer.Stop();
                ping.SendAsyncCancel();
            }
            else
            {
                button.Content = "停止";
                timer.Start();
            }
            isStart = !isStart;
            txt_url.IsEnabled = !isStart;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            changeStatus();
        }

        private void onTimer(object sender, EventArgs e)
        {
            timer.Stop();
            ping.SendAsync(txt_url.Text, this);
        }

        private void PingCompleted(object sender, PingCompletedEventArgs e)
        {
            if (isStart)
                timer.Start();

            if (e.Cancelled)
                return;

            var pingReply = e.Reply;
            if (pingReply.Status == IPStatus.Success)
            {
                txt_ping.Content = "ping: " + pingReply.RoundtripTime + "ms";
            }
            else
            {
                txt_ping.Content = "已断开！";
            }

        }

        private void txt_url_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void sl_interval_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            updateInterval();
        }
    }
}
