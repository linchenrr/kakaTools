using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

                foreach (var item in config.Items)
                {
                    var runner = new RemindRunner();
                    runner.Run(item);
                    runners.Add(runner);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($@"初始化错误,
{ex.Message}");

                Environment.Exit(1);
            }
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
                    Environment.Exit(0);
                    break;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }
    }
}
