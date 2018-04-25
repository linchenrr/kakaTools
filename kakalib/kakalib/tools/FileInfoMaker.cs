using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using KLib;

namespace KLib
{
    public class FileInfoMaker
    {

        static private StringBuilder sb = new StringBuilder();

        static private string inputPath;
        static private string outputPath;

        static public void makeCfg(String input, String output)
        {
            inputPath = input + "/";
            outputPath = output + "/";

            DirectoryInfo outputDir = new DirectoryInfo(outputPath);
            if (outputDir.Exists)
            {
                outputDir.Delete(true);
            }

            FileUtil.copyDirectoryStruct(inputPath, outputPath);

            var count = 0;
            readFiles(inputPath, "", ref count);

            if (count > 0) sb.Remove(sb.Length - 2, 2);

            Byte[] FileInfoBytes = Encoding.UTF8.GetBytes(sb.ToString());

            var buildVersion = DateTime.Now.ToString("yyyyMMddHHmm");

            String fileInfoName = "fileInfo_" + buildVersion + ".txt";
            File.WriteAllBytes(outputPath + fileInfoName, FileInfoBytes);
            File.WriteAllBytes(outputPath + "fileInfoName.txt", Encoding.UTF8.GetBytes(fileInfoName));
            File.WriteAllBytes(outputPath + "buildVersion.txt", Encoding.UTF8.GetBytes(buildVersion));

            Console.WriteLine("已生成" + count + "个文件信息");
            //Console.ReadLine();
        }

        static private void readFiles(string basePath, string dirPath, ref int count)
        {
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

                File.WriteAllBytes(outputPath + "/" + dirPath + "/" + newFileName, bytes);

                sb.Append(fileName);
                sb.Append(",");
                sb.Append(newFileName);
                sb.Append(",");
                sb.Append(Convert.ToString(bytes.Length));
                sb.Append("\r\n");

                count++;
            }

            var addPath = "";
            if (dirPath.Length > 0)
                addPath = dirPath + "/";
            foreach (var dir in Directory.GetDirectories(curPath))
            {
                var dirInfo = new DirectoryInfo(dir);
                readFiles(basePath, addPath + dirInfo.Name, ref count);
            }
        }

    }

}
