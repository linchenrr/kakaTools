using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// UpdateWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UpdateWindow : Window
    {
        public UpdateWindow()
        {
            InitializeComponent();
        }

        private VersionInfo info;
        public void SetInfo(VersionInfo info)
        {
            this.info = info;
            txt_version.Content = info.Version;
            txt_url.Content = info.DownLoadURL;
            txt_code.Content = info.Code;
            txt_date.Content = info.BuildDate.ToString("yyyy-MM-dd HH:mm");
            txt_updateInfo.Content = info.UpdateInfo;
        }

        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_copyCode_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(info.Code);
        }

        private void txt_url_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Process.Start("explorer.exe", info.DownLoadURL);
        }
    }
}
