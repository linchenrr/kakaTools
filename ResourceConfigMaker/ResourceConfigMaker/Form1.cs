using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using KLib;
using System.IO;

namespace ResourceConfigMaker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            tb_path.AllowDrop = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            //DateTime now = DateTime.Now;
            //if (now.Year != 2012 || now.Month > 2)
            //{
            //    MessageBox.Show("测试版已过期，有问题找卡卡");
            //    Close();
            //}
        }

        private void btn_start_Click(object sender, EventArgs e)
        {

            if (tb_path.Text == "")
            {
                MessageBox.Show("请先填写目录");
                return;
            }

            if (!Directory.Exists(tb_path.Text))
            {
                MessageBox.Show("不存在的目录");
                return;
            }
            MessageBox.Show(FileInfoConfigMaker.makeCfg(tb_path.Text));

        }

        private void tb_path_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (paths.Length > 1)
                {
                    return;
                }
                String path = paths[0];

                if (Directory.Exists(path))
                {
                    e.Effect = DragDropEffects.Copy;
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                }
            }
        }

        private void tb_path_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);
                String path = paths[0];
                tb_path.Text = path;
            }
        }
    }
}
