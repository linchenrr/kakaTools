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
using System.Windows.Shapes;

namespace XReminder
{
    /// <summary>
    /// ReadMe.xaml 的交互逻辑
    /// </summary>
    public partial class ReadMeWindow : Window
    {
        public ReadMeWindow()
        {
            InitializeComponent();

            var txt = File.ReadAllText("使用说明.txt");
            txt_readme.Text = txt;
        }

        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void btn_donate_Click(object sender, RoutedEventArgs e)
        {
            var aboutWindow = new AboutWindow();
            aboutWindow.Owner = this.Owner;
            this.Close();
            aboutWindow.ShowDialog();
        }
    }
}
