using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.IO;

namespace KLib
{
    public class FileUtil
    {

        static public void writeFile(String path, Byte[] bytes)
        {

            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
            fs.Write(bytes, 0, bytes.Length);
            fs.Flush();
            fs.Close();
            fs.Dispose();

        }

        static public Byte[] readFile(String path)
        {

            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);

            Byte[] bytes = new Byte[(int)fs.Length];

            fs.Read(bytes, 0, bytes.Length);

            fs.Close();
            fs.Dispose();

            return bytes;

        }

        static public void copyDirectoryStruct(String inputPath, String outputPath)
        {

            DirectoryInfo input = new DirectoryInfo(inputPath);
            if (!input.Exists)
                throw new Exception("源目录不存在:" + inputPath);

            DirectoryInfo output = new DirectoryInfo(outputPath);
            if (!output.Exists)
            {
                output.Create();
            }

            doCopyDirectoryStruct(input, output);

        }

        static private void doCopyDirectoryStruct(DirectoryInfo input, DirectoryInfo output)
        {
            DirectoryInfo[] list_subDir = input.GetDirectories("*", SearchOption.TopDirectoryOnly);

            int i = 0;
            int c = list_subDir.Length;
            while (i < c)
            {
                if ((list_subDir[i].Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                {
                    var newDir = output.CreateSubdirectory(list_subDir[i].Name);
                    //var newDir = new DirectoryInfo(output.FullName + "/" + list_subDir[i].Name);
                    //if (newDir.Exists == false)
                    //    newDir.Create();
                    doCopyDirectoryStruct(list_subDir[i], newDir);
                }
                i++;
            }
        }

        static public void copyFolder(String inputPath, String outputPath, bool reCreate = false)
        {

            DirectoryInfo input = new DirectoryInfo(inputPath);
            if (!input.Exists)
                throw new Exception("源目录不存在:" + inputPath);

            DirectoryInfo output = new DirectoryInfo(outputPath);
            if (output.Exists && reCreate)
            {
                output.Delete(true);
            }
            if (!output.Exists)
                output.Create();

            var files = input.GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                files[i].CopyTo(output.FullName + "/" + files[i].Name, true);
            }

        }

    }

}
