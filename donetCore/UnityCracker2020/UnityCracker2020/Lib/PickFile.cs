using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Win32;

namespace Unity3dPacth.Lib
{
    public class PickFile
    {
        public static bool ShowPickFile(string FileName, string Extension, out FileInfo Result, string InitialDirectory = null)
        {
            var openFileDialog = new OpenFileDialog();

            openFileDialog.CheckFileExists = true;
            openFileDialog.FileName = $"{FileName}.{Extension}";
            openFileDialog.Filter = $"|{FileName}.{Extension}";
            openFileDialog.DefaultExt = Extension;
            openFileDialog.Multiselect = false;
            openFileDialog.ReadOnlyChecked = false;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.ShowReadOnly = false;
            openFileDialog.Title = $"拾取[{FileName}.{Extension}]文件";
            if (!string.IsNullOrWhiteSpace(InitialDirectory) && Directory.Exists(InitialDirectory))
                openFileDialog.InitialDirectory = InitialDirectory;
            else
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            if (openFileDialog.ShowDialog() == true)
            {
                Result = new FileInfo(openFileDialog.FileName);
                return true;
            }
            Result = null;
            return false;
        }
    }
}
