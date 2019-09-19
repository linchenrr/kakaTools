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
using System.Windows.Shapes;

namespace ExcelWPF
{
    /// <summary>
    /// MessageWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MessageWindow : Window
    {
        public MessageWindow()
        {
            InitializeComponent();
        }

        public void Open(string content, Window owner, Action onClose = null)
        {
            this.Owner = owner;
            txt_content.Text = content;
            closeHandler = onClose;
            Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txt_content.IsReadOnly = true;
        }

        private Action closeHandler;
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if (closeHandler != null)
            {
                try
                {
                    closeHandler();
                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}
