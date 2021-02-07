using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reminder
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private List<RemindRunner> runners = new List<RemindRunner>();

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                //var testJson = RemindConfig.MakeDefaultConfig();
                //var config = JsonConvert.DeserializeObject<RemindConfig>(testJson);

                var json = File.ReadAllText("config.json", new UTF8Encoding(false));
                var config = JsonConvert.DeserializeObject<RemindConfig>(json);

                RemindRunner.CheckInterval = config.CheckInterval * 1000;

                for (int i = 0; i < config.Items.Count; i++)
                {
                    var item = config.Items[i];
                    item.ShowBalloon = ShowNotify;
                    item.OnRemindTimeUpdate = OnRemindTimeUpdate;

                    var runner = new RemindRunner();
                    if (runner.Run(item))
                    {
                        var txt = new Label();
                        txt.Location = new Point(20, i * 30 + 20);
                        txt.AutoSize = true;
                        Controls.Add(txt);
                        item.Txt = txt;

                        runners.Add(runner);
                    }
                }

                UpdateStartUp();

                if (config.HideOnStartUp)
                {
                    HideAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($@"初始化错误,
{ex.Message}");

                RunExit(1);
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
            this.Invoke(new AsynUpdateUI(delegate ()
            {
                runner.Data.Txt.Text = $"{time.ToShortDateString()} {time.ToShortTimeString()} {runner.Data.Text}";
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

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            //if (this.WindowState == System.Windows.Forms.FormWindowState.Minimized)
            //            this.WindowState = System.Windows.Forms.FormWindowState.Normal;
            if (e.Button == MouseButtons.Left)
            {
                if (this.Visible)
                {
                    this.Hide();
                }
                else
                {
                    this.Show(Owner);
                    this.Activate();

                }
            }
            else if (e.Button == MouseButtons.Right)
            {

            }
        }

        private void iconMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var item = e.ClickedItem;
            var index = iconMenu.Items.IndexOf(item);
            switch (index)
            {
                case 0:
                    this.Show();
                    break;

                case 1:
                    RunExit();
                    break;
            }
        }

        private void RunExit(int exitCode = 0)
        {
            isRealClose = true;
            this.Close();
            Environment.Exit(exitCode);
        }

        private bool isRealClose;
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isRealClose == false)
            {
                this.Hide();
                e.Cancel = true;
            }
        }


        private string Regpath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        private string keyName = "XiaoKaReminder";
        private string ExEPath = Process.GetCurrentProcess().MainModule.FileName;
        private void SetStartUp(bool runOnStartUp)
        {
            var path = ExEPath;
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
            var path = ExEPath;
            var Rkey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(Regpath, true);


            cb_autoRun.Checked = false;
            if (Rkey != null)
            {
                var keyValue = Rkey.GetValue(keyName) as string;
                cb_autoRun.Checked = keyValue != null;
                if (keyValue != null && keyValue != path)
                    Rkey.SetValue(keyName, path);
            }
            cb_autoRun.CheckedChanged += cb_autoRun_CheckedChanged;
        }

        private void cb_autoRun_CheckedChanged(object sender, EventArgs e)
        {
            SetStartUp(cb_autoRun.Checked);
        }
    }
}
