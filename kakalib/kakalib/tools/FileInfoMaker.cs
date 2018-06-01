using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections;
using KLib;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Windows.Forms;

namespace KLib
{
    public class FileInfoMaker
    {

        static private StringBuilder sb = new StringBuilder();

        static private string inputPath;
        static private string outputPath;

        static private Dictionary<string, string> dic_ver = new Dictionary<string, string>();

        static public string[] compressFiles = new string[0];
        static public string[] compressFileName = new string[0];
        static public bool WithOriginalFiles = false;
        static public bool CompressPNG = false;
        static public string SpecifiedFolder;

        static public void makeCfg(String input, String output)
        {
            inputPath = input + "/";
            outputPath = output + "/";
            var originalOutputPath = outputPath;

            for (int i = 0; i < compressFiles.Length; i++)
            {
                compressFiles[i] = compressFiles[i].Trim().ToLower();
            }

            var outputDir = new DirectoryInfo(outputPath);
            if (outputDir.Exists)
            {
                try
                {
                    outputDir.Delete(true);
                }
                catch (Exception)
                {
                    outputDir.Delete(true);
                }
            }

            FileUtil.copyDirectoryStruct(inputPath, outputPath);

            if (SpecifiedFolder != null)
            {
                inputPath += SpecifiedFolder + "/";
                outputPath += SpecifiedFolder + "/";
            }
            var count = 0;
            readFiles(inputPath, "", ref count);

            if (count > 0) sb.Remove(sb.Length - 2, 2);

            var FileInfoBytes = Encoding.UTF8.GetBytes(sb.ToString());

            var buildVersion = DateTime.Now.ToString("yyyyMMddHHmm");

            //var fileInfoName = "fileInfo_" + buildVersion + ".txt";
            //File.WriteAllBytes(outputPath + fileInfoName, FileInfoBytes);
            //File.WriteAllBytes(outputPath + "fileInfoName.txt", Encoding.UTF8.GetBytes(fileInfoName));

            var json = JsonConvert.SerializeObject(dic_ver);
            var jsonName = "fileInfo_" + buildVersion + ".txt";
            var jsonName_compress = "fileInfo_" + buildVersion + "_compress.txt";


            //var bytes = Encoding.UTF8.GetBytes(json);
            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            File.WriteAllBytes(originalOutputPath + jsonName, bytes);

            var bytes_compress = ZlibCompresser.compress(bytes);
            File.WriteAllBytes(originalOutputPath + jsonName_compress, bytes_compress);

            File.WriteAllBytes(originalOutputPath + "fileInfoName.txt", Encoding.UTF8.GetBytes(jsonName_compress));
            File.WriteAllBytes(originalOutputPath + "buildVersion.txt", Encoding.UTF8.GetBytes(buildVersion));

            Console.WriteLine("已生成" + count + "个文件信息");
            //Console.ReadLine();
        }

        static private void readFiles(string basePath, string dirPath, ref int count)
        {
            Console.WriteLine();
            Console.WriteLine($"dir:{dirPath}");

            var curPath = basePath + "/" + dirPath;
            foreach (var filePath in Directory.GetFiles(curPath))
            {
                //Console.WriteLine(filePath);

                var fileInfo = new FileInfo(filePath);
                var fileName = fileInfo.Name;
                var bytes = File.ReadAllBytes(filePath);

                var crc32 = new Crc32();
                var crc = "";
                foreach (byte b in crc32.ComputeHash(bytes)) crc += b.ToString("x2").ToLower();

                int idx = fileName.LastIndexOf(".");
                String newFileName;
                if (idx != -1)
                    newFileName = fileName.Insert(idx, "_" + crc);
                else
                    newFileName = fileName + "_" + crc;

                if (WithOriginalFiles)
                    File.WriteAllBytes(outputPath + "/" + dirPath + fileName, bytes);

                var isCompress = false;
                var fileNameLower = fileInfo.Name.ToLower();
                if (compressFiles.FirstOrDefault(item => fileNameLower.EndsWith(item)) != null)
                {
                    isCompress = true;
                    bytes = ZlibCompresser.compress(bytes);
                }

                var newPah = outputPath + "/" + dirPath + newFileName;
                if (CompressPNG && fileInfo.Extension.ToLower() == ".png")
                {
                    compressPNGFile(fileInfo.FullName, bytes, newPah);
                }
                else
                {
                    File.WriteAllBytes(newPah, bytes);
                }

                dic_ver.Add(dirPath + fileName, dirPath + newFileName);

                sb.Append(dirPath + fileName);
                sb.Append(",");
                sb.Append(dirPath + newFileName);
                sb.Append(",");
                sb.Append(isCompress.ToString().ToLower());
                sb.Append(",");
                sb.Append(Convert.ToString(bytes.Length));
                sb.Append("\r\n");

                count++;
            }

            foreach (var dir in Directory.GetDirectories(curPath))
            {
                var dirInfo = new DirectoryInfo(dir);
                readFiles(basePath, dirPath + dirInfo.Name + "/", ref count);
            }
        }

        static private void compressPNGFile(string path, byte[] bytes, string output)
        {
            try
            {
                Console.WriteLine($@"compress {Path.GetFileName(path)}");
                var tmpFileName = Path.GetTempPath() + "resourceInfoExporter.png";
                var tmpWriteName = Path.GetTempPath() + "resourceInfoExporter_compress.png";
                File.WriteAllBytes(tmpFileName, bytes);
                var p = new Process();
                p.StartInfo.FileName = Application.StartupPath + "/pngquant.exe";
                p.StartInfo.Arguments = $@"{tmpFileName} -o {tmpWriteName} --force";
                //p.StartInfo.Arguments = $@"{tmpFileName} --force --ext .png";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.Start();
                p.WaitForExit();

                File.Copy(tmpWriteName, output);
            }
            catch (Exception e)
            {
                var msg = $@"压缩图片错误
{path}";
                Console.WriteLine(msg);
                MessageBox.Show(msg);
                throw e;
            }

        }

    }

}
