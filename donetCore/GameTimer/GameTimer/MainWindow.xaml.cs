using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameTimer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            btn_stop.Visibility = Visibility.Hidden;

            player_start = new SoundPlayer("start.wav");
            player_start.Load();

            player = new SoundPlayer("ding.wav");
            player.Load();
        }

        private SoundPlayer player_start;
        private SoundPlayer player;
        private bool isEnd;
        private long nextPlayTime;
        private double intervalSeconds;
        private void btn_start_Click(object sender, RoutedEventArgs e)
        {
            intervalSeconds = double.Parse(txt_interval.Text.Trim());
            var startDelay = double.Parse(txt_startDelay.Text.Trim());

            isEnd = false;

            nextPlayTime = DateTime.Now.AddSeconds(startDelay).Ticks / 10000;
            player_start.Play();
            Task.Run(() =>
            {
                while (true)
                {
                    if (isEnd)
                        break;

                    var now = DateTime.Now.Ticks / 10000;
                    if (now >= nextPlayTime)
                    {
                        nextPlayTime += Convert.ToInt64(intervalSeconds * 1000);
                        player.Play();
                    }
                    //Console.WriteLine($"{now}");
                    Thread.Sleep(10);
                }
            });

            btn_start.Visibility = Visibility.Hidden;
            btn_stop.Visibility = Visibility.Visible;
        }

        private void btn_stop_Click(object sender, RoutedEventArgs e)
        {
            isEnd = true;
            btn_stop.Visibility = Visibility.Hidden;
            btn_start.Visibility = Visibility.Visible;
        }
    }
}
