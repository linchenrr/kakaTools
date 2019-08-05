using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using System.Threading;
using KLib;

namespace fileCompressTool
{
    public partial class Form1 : Form
    {

        OpenFileDialog fd;

        ArrayList selectedPath;

        FileCompresser fc;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            init();
        }

        private void init()
        {

            this.AllowDrop = true;

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            CheckForIllegalCrossThreadCalls = false;

            selectedPath = new ArrayList();

            fd = new OpenFileDialog();
            fd.Multiselect = true;
            //fd.Filter = "Excel文件|*.xls;*.xlsx";
            fd.Title = "选择要处理的文件";

            //cb_algorithm.Items.Add("sss");

            cb_algorithm.Items.AddRange(new Object[] {
                CompressOption.lzma,
                CompressOption.gzip,
                CompressOption.zlib
            });

            fc = new FileCompresser();
            cb_algorithm.SelectedIndex = 0;

        }

        private void updateCompressAlgorithm()
        {
            fc.setCompressAlgorithm((CompressOption)Enum.Parse(typeof(CompressOption), cb_algorithm.SelectedItem.ToString()));
        }

        public void addFile(String[] newPaths)
        {
            int i = 0;
            while (i < newPaths.Length)
            {
                String path = newPaths[i];
                if (selectedPath.IndexOf(path) < 0)
                {
                    selectedPath.Add(path);
                }
                i++;
            }
            updatePathList();
        }

        private void updatePathList()
        {
            StringBuilder sb = new StringBuilder();

            int i = 0;
            int c = selectedPath.Count;
            while (i < c)
            {
                sb.Append(selectedPath[i]);
                sb.Append("\r\n");
                i++;
            }

            tb_fileNames.Text = sb.ToString();

            lb_status.Text = "状态";
        }

        public void clearFile()
        {
            selectedPath.Clear();
            updatePathList();
        }

        private void btn_addFile_Click(object sender, EventArgs e)
        {
            fd.ShowDialog();

            addFile(fd.FileNames);
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);
            addFile(paths);
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            clearFile();
        }

        private void btn_compress_Click(object sender, EventArgs e)
        {
            startCompress(true, checkBox_cover.Checked);
        }

        private void btn_uncompress_Click(object sender, EventArgs e)
        {
            startCompress(false, checkBox_cover.Checked);
        }

        private void startCompress(Boolean compress, Boolean overWrite)
        {

            if (selectedPath.Count == 0)
            {
                MessageBox.Show("请选择待处理文件");
                return;
            }

            lb_status.Text = "正在处理...";

            btn_compress.Enabled = false;
            btn_uncompress.Enabled = false;

            DoWorkInfo dwInfo = new DoWorkInfo();
            dwInfo.compress = compress;
            dwInfo.overWrite = overWrite;

            Thread t = new Thread(new ParameterizedThreadStart(doWork));
            t.Start(dwInfo);

        }

        private void doWork(Object param)
        {

            DoWorkInfo dwInfo = (DoWorkInfo)param;

            String[] pathList = new String[selectedPath.Count];
            this.selectedPath.CopyTo(pathList);

            String addName = "";

            if (!dwInfo.overWrite)
            {

                if (dwInfo.compress)
                {
                    addName = "_compress";
                }
                else
                {
                    addName = "_uncompress";
                }
            }

            try
            {
                foreach (var path in pathList)
                {
                    if (dwInfo.compress)
                    {
                        fc.compress(path, path + addName);
                    }
                    else
                    {
                        fc.uncompress(path, path + addName);
                    }
                }

                lb_status.Text = "处理完成！";

            }
            catch (Exception e)
            {

                MessageBox.Show("出现异常:\r\n" + e.Message);

                lb_status.Text = "处理失败！";

            }
            finally
            {

                btn_compress.Enabled = true;
                btn_uncompress.Enabled = true;

            }

        }

        private void cb_algorithm_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateCompressAlgorithm();
        }

    }

    class DoWorkInfo
    {
        public Boolean compress;
        public Boolean overWrite;
    }

    class AlgorithmItem
    {
        public CompressAlgorithm algorithm;

        public AlgorithmItem(CompressAlgorithm algorithm, String name)
        {
            this.algorithm = algorithm;
            this.name = name;
        }

        private String _name;
        public String name
        {
            set { _name = value; }
            get { return _name; }
        }

    }

}
