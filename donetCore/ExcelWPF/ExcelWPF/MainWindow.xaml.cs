using FluentFTP;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.IO.Path;

namespace ExcelWPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private MessageWindow msgWindow;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var exePath = Process.GetCurrentProcess().MainModule.FileName;
            exeDir = GetDirectoryName(exePath);
            tmpDir = Combine(exeDir, "output");
            Environment.CurrentDirectory = exeDir;
            try
            {
                APPConfig.Init();
            }
            catch (Exception ex)
            {
                showMsg($@"读取配置文件出错！
{ex}", Close);
            }



            var info = new DirectoryInfo(Config.ProjDir);
            ProjDir = info.FullName;

            setDataGrid(grid_branch);
            grid_branch.ItemsSource = Config.Branchs;
            grid_branch.Items.Refresh();
            //APPConfig.ToJson();
            setDataGrid(grid_ftp);
            grid_ftp.ItemsSource = Config.Ftps;
            grid_ftp.Items.Refresh();
        }

        private void showMsg(string content, Action onClose = null)
        {
            msgWindow = new MessageWindow();
            msgWindow.Open(content, this, onClose);
        }

        private void setDataGrid(DataGrid grid)
        {
            Style styleRight = new Style(typeof(TextBlock));
            Setter setRight = new Setter(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            styleRight.Setters.Add(setRight);
            setRight = new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
            styleRight.Setters.Add(setRight);
            foreach (DataGridColumn c in grid.Columns)
            {
                DataGridTextColumn tc = c as DataGridTextColumn;
                if (tc != null)
                {
                    tc.ElementStyle = styleRight;
                }
            }
        }

        private string exeDir;
        private string tmpDir;
        private APPConfig Config { get { return APPConfig.Config; } }
        public string ProjDir;

        private int exeBat(string path, bool showError, params string[] args)
        {
            var proc = new Process();
            var file = new FileInfo(path);

            proc.StartInfo.WorkingDirectory = ProjDir;
            proc.StartInfo.FileName = file.FullName;
            var argsStr = args.Join(" ");
            proc.StartInfo.Arguments = argsStr;

            proc.Start();
            proc.WaitForExit();

            var code = proc.ExitCode;
            if (showError && code != 0)
            {
                showMsg($@"执行出错，exit code:{code}");
            }
            return code;
        }

        private BranchConfig getSelectBranch()
        {
            var data = grid_branch.SelectedValue;
            if (data == null)
            {
                //MessageBox.Show("未选择分支");
                showMsg("未选择分支");
                return null;
            }
            return (BranchConfig)data;
        }

        private void Btn_update_Click(object sender, RoutedEventArgs e)
        {
            var data = getSelectBranch();
            if (data == null)
                return;

            exeBat("Excel update.cmd", false, Combine(ProjDir, Config.ExcelLocalBaseDir, data.LocalFolder), data.SvnPath);
        }

        private void Btn_commit_Click(object sender, RoutedEventArgs e)
        {
            var data = getSelectBranch();
            if (data == null)
                return;

            exeBat("Excel commit.cmd", false, Combine(ProjDir, Config.ExcelLocalBaseDir, data.LocalFolder), data.SvnPath);
        }

        private void Btn_export_Click(object sender, RoutedEventArgs e)
        {
            var data = getSelectBranch();
            if (data == null)
                return;

            if (Directory.Exists(tmpDir))
                Directory.Delete(tmpDir, true);

            var code = exeBat("Excel export.cmd", true, Combine(ProjDir, Config.ExcelLocalBaseDir, data.LocalFolder), data.SvnPath, Combine(tmpDir, "client"));
            if (code != 0)
                return;

            code = exeBat("Excel export server.cmd", true, Combine(ProjDir, Config.ExcelLocalBaseDir, data.LocalFolder), data.SvnPath, Combine(tmpDir, "server"));
            if (code != 0)
                return;

        }

        private UploadWindow uploadWindow;
        private void Btn_upload_Click(object sender, RoutedEventArgs e)
        {
            var ftpData = (FTPConfig)grid_ftp.SelectedValue;
            if (ftpData == null)
            {
                showMsg("未选择ftp服务器");
                return;
            }
            uploadWindow = new UploadWindow();
            uploadWindow.Owner = this;
            uploadWindow.Show();
            uploadWindow.Upload(ftpData, tmpDir);
        }

        private void Btn_code_Click(object sender, RoutedEventArgs e)
        {
            var data = getSelectBranch();
            if (data == null)
                return;

            var code = exeBat("Excel code generate.cmd", true, Combine(ProjDir, Config.ExcelLocalBaseDir, data.LocalFolder), data.SvnPath, Combine(ProjDir, Config.CodeOutputPath));
            if (code != 0)
                return;
        }

        private void Btn_openfolder_Click(object sender, RoutedEventArgs e)
        {
            var data = getSelectBranch();
            if (data == null)
                return;

            var path = Combine(ProjDir, Config.ExcelLocalBaseDir, data.LocalFolder);
            ExplorePath(path);
        }

        /// <summary>
        /// 浏览文件
        /// </summary>
        /// <param name="filePath"></param>
        public static void ExploreFile(string filePath)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = "explorer";
            //打开资源管理器
            proc.StartInfo.Arguments = @"/select," + filePath;
            //选中"notepad.exe"这个程序,即记事本
            proc.Start();
        }

        /// <summary>
        /// 浏览文件夹
        /// </summary>
        /// <param name="path"></param>
        public static void ExplorePath(string path)
        {
            System.Diagnostics.Process.Start("explorer.exe", path);
        }
    }
}
