using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace XReminder
{
    /// <summary>
    /// RemindViewItem.xaml 的交互逻辑
    /// </summary>
    public partial class RemindViewItem : UserControl
    {
        public RemindViewItem()
        {
            InitializeComponent();
        }

        private RemindRunner runner;
        public void SetInfo(RemindRunner runner)
        {
            this.runner = runner;
            runner.OnRemindTimeUpdate = OnRemindTimeUpdate;
        }

        public void OnRemindTimeUpdate(RemindRunner runner)
        {
            this.Dispatcher.Invoke(() =>
            {
                var time = runner.remindTime;
                txt_time.Content = $"{time.ToString("MM-dd HH:mm")}";
                txt_message.Content = $"{runner.Data.Text}";
            });

        }

        private void btn_setTimeToNow_Click(object sender, RoutedEventArgs e)
        {
            runner.SetStartTimeToNow();
        }
    }
}
