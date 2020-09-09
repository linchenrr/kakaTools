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

namespace UnityPatcher2020
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            txt_info.Text = "";
        }

        private FileVersionInfo info;
        private void txt_path_TextChanged(object sender, TextChangedEventArgs e)
        {
            var path = txt_path.Text.Trim();
            if (string.IsNullOrEmpty(path))
            {
                txt_info.Text = "";
                return;
            }
            var fileInfo = new FileInfo(path);

            if (fileInfo.Exists)
            {
                info = FileVersionInfo.GetVersionInfo(path);

                txt_info.Text = $@"ProductName：{info.ProductName}
ProductVersion：{info.ProductVersion}";
                //string productName = info.ProductName;
                //string productVersion = info.ProductVersion;
                //string companyName = info.CompanyName;
                //string legalCopyright = info.LegalCopyright;
            }
            else
            {
                txt_info.Text = "file does not exist!";
            }
        }

        private void btn_browser_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.DefaultExt = ".exe";
            ofd.Filter = "unity file|*.exe";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == true)
            {
                txt_path.Text = ofd.FileName;


            }
        }

        private void btn_patch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var path = txt_path.Text.Trim();
                var watch = new Stopwatch();

                var signCode = new byte[] { 0x75, 0x11, 0xB8, 0x02, 0x00, 0x00, 0x00, 0xE9 };
                var patchCode = new byte[] { 0xEB };

                //if (info.ProductVersion.StartsWith("2020.2"))
                //{
                //    signCode = new byte[] { 0x55, 0x57, 0x41, 0x56, 0x48, 0x8D, 0xA8, 0x68, 0xFD, 0xFF, 0xFF, 0x48, 0x81 };
                //    patchCode = new byte[] { 0xC3 };
                //}

                var codeLength = signCode.Length;
                var finalFound = false;
                var index = 0l;
                byte[] sourceBytes;

                using (var fs = File.Open(path, FileMode.Open, FileAccess.ReadWrite))
                {
                    sourceBytes = new byte[fs.Length];
                    fs.Read(sourceBytes, 0, sourceBytes.Length);
                    var maxIndex = fs.Length - signCode.Length;

                    watch.Start();

                    while (index < maxIndex)
                    {
                        var codeIndex = 0;
                        var found = true;
                        while (codeIndex < codeLength)
                        {
                            if (signCode[codeIndex] == sourceBytes[index + codeIndex])
                                codeIndex++;
                            else
                            {
                                found = false;
                                break;
                            }
                        }
                        if (found)
                        {
                            finalFound = true;
                            for (int i = 0; i < patchCode.Length; i++)
                            {
                                sourceBytes[index + i] = patchCode[i];
                            }
                            //MessageBox.Show($@"find code at index {index}, use time: {watch.Elapsed.TotalSeconds} seconds.");
                            break;
                        }
                        index++;
                    }
                }
                if (finalFound)
                {
                    var fileInfo = new FileInfo(path);
                    var dirInfo = fileInfo.Directory;
                    var licensingDir = Path.Combine(dirInfo.FullName, @"Data\Resources\Licensing");
                    var bakLicensingDir = Path.Combine(dirInfo.FullName, @"Data\Resources\Licensing_bak");
                    if (Directory.Exists(licensingDir))
                    {
                        if (Directory.Exists(bakLicensingDir))
                        {
                            Directory.Delete(bakLicensingDir, true);
                        }
                        Directory.Move(licensingDir, bakLicensingDir);
                    }

                    File.Move(path, Path.Combine(dirInfo.FullName, fileInfo.Name + ".bak"));
                    File.WriteAllBytes(fileInfo.FullName, sourceBytes);

                    MessageBox.Show($@"patch success for {path}!");
                }
                else
                {
                    MessageBox.Show($@"can't find pattern in {path}!");
                }

                watch.Stop();
            }
            catch (Exception ex)
            {
                MessageBox.Show($@"patch error!!
{ex}");
            }

        }

    }
}
