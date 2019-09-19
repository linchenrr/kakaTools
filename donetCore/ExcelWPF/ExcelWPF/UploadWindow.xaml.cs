using FluentFTP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// UploadWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UploadWindow : Window
    {
        public UploadWindow()
        {
            InitializeComponent();
        }

        private class UploadItem
        {
            public FtpClient Client;
            public bool IsUploading;
        }


        private FTPConfig ftpData;
        private UploadItem[] items;
        private Thread thread;
        private string localDir;
        private bool isCancel;
        public void Upload(FTPConfig ftpData, string localDir, int maxThreads = 8)
        {
            try
            {
                this.maxThreads = maxThreads;
                items = new UploadItem[maxThreads];
                this.localDir = localDir;
                for (int i = 0; i < maxThreads; i++)
                {
                    var ftp = new FtpClient(
                    ftpData.host,
                    ftpData.port,
                    ftpData.userName,
                    ftpData.pwd
                    );

                    ftp.UploadDataType = FtpDataType.Binary;
                    items[i] = new UploadItem()
                    {
                        Client = ftp,
                    };
                }

                txt_content.Content = $@"正在上传至 {ftpData.name}  {ftpData.url}";
                updateThreadNum();

                exceptions = new List<Exception>();
                this.ftpData = ftpData;
                thread = new Thread(UploadSync);
                thread.Start();
            }
            catch (Exception e)
            {
                var msgWindow = new MessageWindow();
                msgWindow.Open(e.ToString(), this);
            }
        }

        private UploadItem getFreeItem()
        {
            foreach (var item in items)
            {
                if (item.IsUploading == false)
                    return item;
            }
            return null;
        }

        private DirectoryInfo localInfo;
        private void UploadSync()
        {
            localInfo = new DirectoryInfo(localDir);
            var files = localInfo.GetFiles("*", SearchOption.AllDirectories);
            maxCount = files.Length;
            updateProgress();

            foreach (var file in files)
            {
                var remoteName = file.FullName.Replace(localInfo.FullName, "");

                while (true)
                {
                    var item = getFreeItem();
                    if (item == null)
                    {
                        Thread.Sleep(1);
                        continue;
                    }
                    if (isCancel)
                        return;
                    item.IsUploading = true;
                    updateThreadNum();
                    UploadTaskSync(item, file.FullName, remoteName);
                    break;
                }

            }

            if (exceptions.Count > 0)
            {
                var str = $@"
";
                foreach (var ex in exceptions)
                {
                    str += ex;
                    str += $@"
";
                }
                var msgWindow = new MessageWindow();
                msgWindow.Open($@"上传文件异常
{str}
", this);
            }
            else
            {
                Close();
            }
        }

        private List<Exception> exceptions;
        private async Task UploadTaskSync(UploadItem item, string localPath, string remotePath)
        {
            try
            {
                var ftp = item.Client;
                if (ftp.IsConnected == false)
                    await ftp.ConnectAsync();

                var result = await ftp.UploadFileAsync(localPath, remotePath, FtpExists.Overwrite, true);
                if (result)
                {
                    curCount++;
                    updateProgress();
                }
            }
            catch (Exception e)
            {
                exceptions.Add(e);
            }
            item.IsUploading = false;
            updateThreadNum();
        }

        private int maxCount;
        private int curCount;
        private void updateProgress()
        {
            this.Dispatcher.Invoke(() =>
            {
                txt_progress.Content = $@"{curCount}/{maxCount}";
                var v = (double)curCount / maxCount;
                bar_progress.Value = v * 100;
            });
        }

        private int maxThreads;
        private void updateThreadNum()
        {
            this.Dispatcher.Invoke(() =>
            {
                var num = 0;
                foreach (var item in items)
                {
                    if (item.IsUploading)
                        num++;
                }
                txt_thread.Content = $@"上传线程数:{num}/{maxThreads}";
            });
        }

        private void Btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            isCancel = true;
        }
    }
}
