using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace XReminder
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        Mutex instance;
        protected override void OnStartup(StartupEventArgs e)
        {
            bool createdNew;
            instance = new Mutex(true, "XiaokaReminderfhjdks456", out createdNew);

            if (createdNew == false)
            {
                MessageBox.Show("程序已在运行中...");
                Environment.Exit(0);
            }
        }

    }
}
