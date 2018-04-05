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

namespace excelExport
{
    public partial class Form_ExcelExport : Form
    {
        private static Mutex myMutex;
        private static bool requestInitialOwnership = true;
        private static bool mutexWasCreated;

        public Form_ExcelExport()
        {

            InitializeComponent();

        }

        OpenFileDialog fd;
        FolderBrowserDialog fbd;

        ArrayList selectedPath;

        public void addFile(String[] newPaths)
        {
            int i = 0;
            while (i < newPaths.Length)
            {
                String path = newPaths[i];
                if (selectedPath.IndexOf(path) < 0)
                {
                    if (File.Exists(path))
                    {

                        String ext = Path.GetExtension(path);

                        if (ext == ".xls" || ext == ".xlsx")
                        {
                            selectedPath.Add(path);
                        }

                    }
                    else if (Directory.Exists(path))
                    {
                        selectedPath.Add(path);
                    }

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

        private void init()
        {

            //this.AllowDrop = true;
            tb_outPutPath.AllowDrop = true;
            tb_fileNames.AllowDrop = true;

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            tray.Text = this.Text;

            CheckForIllegalCrossThreadCalls = false;

            selectedPath = new ArrayList();

            fd = new OpenFileDialog();
            fd.Multiselect = true;
            fd.Filter = "Excel文件|*.xls;*.xlsx";
            fd.Title = "选择要处理的文件";


            fbd = new FolderBrowserDialog();
            fbd.Description = "选择文件导出目录";

            //cb_compressOP.Items.AddRange(new Object[]{
            //    CompressOption.none,
            //    CompressOption.lzma,
            //    CompressOption.zlib
            //});
            cb_compressOP.DataSource = Enum.GetNames(typeof(CompressOption));
           
            //恢复配置
            String outPutPath = getSettings("outPutPath");
            if (!Directory.Exists(outPutPath)) outPutPath = "";
            fbd.SelectedPath = outPutPath;
            tb_outPutPath.Text = fbd.SelectedPath;

            cb_compressOP.SelectedItem = getSettings("compress");
            checkBox_ignore.Checked = getSettings("ignore").ToLower() == "true";
            checkBox_merge.Checked = getSettings("merge").ToLower() == "true";
            tb_ignoreColumn.Text = getSettings("ignoreColumn");
            tb_ignoreLine.Text = getSettings("ignoreLine");
            tb_ignoreSheet.Text = getSettings("ignoreSheet");
            tb_primaryKey.Text = getSettings("primaryKey");

            String path = getSettings("selectedPath");
            if (path != "")
            {
                addFile(path.Split('|'));
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {

            fd.ShowDialog();

            addFile(fd.FileNames);

        }

        private void btn_export_Click(object sender, EventArgs e)
        {

            if (selectedPath.Count == 0)
            {
                MessageBox.Show("请选择待处理文件");
                return;
            }

            //if (tb_outPutPath.Text == null || tb_outPutPath.Text == "" || !Directory.Exists(tb_outPutPath.Text))
            //{
            //    MessageBox.Show("请选择有效的导出目录");
            //    return;
            //}

            lb_status.Text = "正在处理...";

            btn_export.Enabled = false;

            Thread t = new Thread(new ThreadStart(startExport));
            t.Start();

        }

        private void startExport()
        {

            try
            {

                String[] pathList = new String[selectedPath.Count];
                this.selectedPath.CopyTo(pathList);

                String outPutPath = tb_outPutPath.Text;
                Hashtable sheets = new Hashtable();

                ExcelUtil.export(pathList, outPutPath, (CompressOption)Enum.Parse(typeof(CompressOption), (String)cb_compressOP.SelectedItem), tb_primaryKey.Text, tb_ignoreSheet.Text, tb_ignoreLine.Text, tb_ignoreColumn.Text, checkBox_ignore.Checked, checkBox_merge.Checked);

                lb_status.Text = "处理完成！";

            }
            catch (Exception e)
            {

                MessageBox.Show("出现异常:\r\n" + e.Message);

                lb_status.Text = "处理失败！";

            }
            finally
            {
                btn_export.Enabled = true;
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {

            myMutex = new Mutex(requestInitialOwnership, Application.ProductName, out mutexWasCreated);
            if (!(requestInitialOwnership && mutexWasCreated))
            {
                MessageBox.Show("不能同时运行多个程序实例", "提示", MessageBoxButtons.OK);
                doExit();
            }
            /*
            这里就是进程互斥的实现。我看过一些人写的启功互斥，他们采用的方式是先看当前进程表里有没有要启动的进程；有，看看这个进程是否和要运行的进程来之相同的目录。
            实际上看来，这样不能彻底解决问题，例如，如果我把程序改名，然后换个目录这样就可以在此运行了，而且时间复杂度偏大。
            而以上的代码：
            myMutex = new Mutex(requestInitialOwnership,"Test",out mutexWasCreated);

            这里是申请一个命名互斥，并且返回是否已经有同名的申请了。
            if(!(requestInitialOwnership && mutexWasCreated))
 

            myMutex.WaitOne();

            如果互斥已经申请过了，阻塞要运行的程序。
            */

            init();
        }

        private void btn_selectOutPutFolder_Click(object sender, EventArgs e)
        {

            if (fbd.ShowDialog() == DialogResult.OK)
            {

                tb_outPutPath.Text = fbd.SelectedPath;

            }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (cancelExit)
            {

                this.Hide();

                e.Cancel = true;
            }

        }

        private String getSettings(String key)
        {

            return RegistryControl.getSettings(key);

        }

        private void setSettings(String key, Object value)
        {

            RegistryControl.setSettings(key, value);

        }

        Boolean cancelExit = true;
        private void exit()
        {
            //保存配置

            StringBuilder sb = new StringBuilder();

            int i = 0;
            int c = selectedPath.Count;
            while (i < c)
            {
                sb.Append(selectedPath[i]);
                if (i != selectedPath.Count - 1)
                {
                    sb.Append("|");
                }
                i++;
            }

            setSettings("selectedPath", sb.ToString());
            setSettings("outPutPath", tb_outPutPath.Text);
            setSettings("compress", cb_compressOP.SelectedItem.ToString());
            setSettings("ignore", checkBox_ignore.Checked.ToString());
            setSettings("merge", checkBox_merge.Checked.ToString());
            setSettings("ignoreColumn", tb_ignoreColumn.Text);
            setSettings("ignoreLine", tb_ignoreLine.Text);
            setSettings("ignoreSheet", tb_ignoreSheet.Text);
            setSettings("primaryKey", tb_primaryKey.Text);

            tray.Visible = false;
            tray.Dispose();

            doExit();

        }

        private void doExit()
        {
            cancelExit = false;

            Application.Exit();
        }

        private void item_close_Click(object sender, EventArgs e)
        {
            exit();
        }

        private void item_show_Click(object sender, EventArgs e)
        {

            this.Show();
            this.TopMost = true;
            this.TopMost = false;

        }

        private void item_hide_Click(object sender, EventArgs e)
        {

            this.Hide();

        }

        private void tray_MouseClick(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {
                if (this.Visible)
                {
                    this.Hide();
                }
                else
                {
                    this.Show();
                    this.TopMost = true;
                    this.TopMost = false;
                }

            }
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            clearFile();
        }

        private void tb_outPutPath_DragEnter(object sender, DragEventArgs e)
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

        private void tb_outPutPath_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);
                String path = paths[0];
                tb_outPutPath.Text = path;
            }
        }

        private void tb_fileNames_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);
                int i = 0;
                while (i < paths.Length)
                {
                    String path = paths[i];

                    String ext = Path.GetExtension(path);

                    if (ext == ".xls" || ext == ".xlsx" || Directory.Exists(path))
                    {
                        e.Effect = DragDropEffects.Copy;
                        return;
                    }
                    i++;
                }

                //e.Effect = DragDropEffects.None;
                //return;

            }
            e.Effect = DragDropEffects.None;
        }

        private void tb_fileNames_DragDrop(object sender, DragEventArgs e)
        {
            string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);
            addFile(paths);
        }
    }
}
