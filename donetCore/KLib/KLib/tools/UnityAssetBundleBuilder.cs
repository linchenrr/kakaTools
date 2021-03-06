﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using KLib;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;

namespace KLib
{
    public class UnityAssetBundleBuilder
    {

        static private Encoding encoding = new UTF8Encoding(false);

        static public HashSet<string> SpecialNames = new HashSet<string>();

        static public void build(string inputPath, string outputPath, int maxThread)
        {
            Console.WriteLine("input:" + inputPath);
            Console.WriteLine("output:" + outputPath);
            Console.WriteLine("maxThread:" + maxThread);

            //KLibInvalid.RemoteCheckAsync(KLibInvalid.AssetBundleBuilderURL, (info) => IsInvalid = info.IsInvalid);
            //Thread.Sleep(500);

            inputPath = inputPath.TrimEnd('/');
            outputPath = outputPath.TrimEnd('/');
            inputPath = inputPath + "/";
            outputPath = outputPath + "/";

            inputPath = inputPath.ToLower();

            var time = new Stopwatch();
            time.Start();

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


            var outputDir = new DirectoryInfo(outputPath);
            if (outputDir.Exists == false)
                outputDir.Create();

            var outputFilePath = outputPath + "/build/";
            if (Directory.Exists(outputFilePath) == false)
                Directory.CreateDirectory(outputFilePath);

            var buildInfoPath = outputPath + "/buildCache.json";
            var buildInfo = CompressBuildInfo.ReadFromFile(buildInfoPath);
            if (buildInfo == null)
            {
                Console.WriteLine(outputPath + "/buildInfo.txt doesn't exist, create new.");
                buildInfo = new CompressBuildInfo();
            }

            var specialFiles = new[] { "assetInfo.txt", "assetInfo_compressed.txt" };
            //delete unuse dirs
            var targetFileDir = new DirectoryInfo(outputFilePath);
            foreach (var dicInfo in targetFileDir.GetDirectories("*", SearchOption.AllDirectories))
            {
                var isSpecial = false;
                var lowerName = dicInfo.FullName.ToLower();
                foreach (var sName in SpecialNames)
                {
                    if (lowerName.Contains(sName))
                    {
                        isSpecial = true;
                        break;
                    }
                }
                if (isSpecial)
                    continue;

                var dirPath = dicInfo.FullName.Replace(@"\", "/");
                var dirName = dirPath.Substring(dirPath.IndexOf(outputFilePath) + outputFilePath.Length);
                dirName = dirName.TrimStart('/');

                var orgDir = new DirectoryInfo(inputPath + dirName);
                if (orgDir.Exists == false)
                {
                    try
                    {
                        dicInfo.Delete(true);
                        Console.WriteLine($@"delete unuse directory:{dirName}");
                    }
                    catch { }
                }
            }
            //delete unuse files
            foreach (var fileInfo in targetFileDir.GetFiles("*", SearchOption.AllDirectories))
            {
                var isSpecial = false;
                var lowerName = fileInfo.FullName.ToLower();
                foreach (var sName in SpecialNames)
                {
                    if (lowerName.Contains(sName))
                    {
                        isSpecial = true;
                        break;
                    }
                }
                if (isSpecial)
                    continue;

                var filePath = fileInfo.FullName.Replace(@"\", "/");
                var fileName = filePath.Substring(filePath.IndexOf(outputFilePath) + outputFilePath.Length);
                fileName = fileName.TrimStart('/');

                var orgFile = new FileInfo(inputPath + fileName);
                if (orgFile.Exists == false)
                {
                    Console.WriteLine($@"delete unuse file:{fileName}");
                    fileInfo.Delete();
                }
            }

            FileUtil.copyDirectoryStruct(inputPath, outputFilePath);

            var resInfoList = new List<ResourceInfo>();
            var newBuildInfo = new CompressBuildInfo();
            foreach (var fileInfo in files)
            {
                var filePath = fileInfo.FullName.Replace(@"\", "/");
                var fileName = filePath.Substring(filePath.IndexOf(inputPath) + inputPath.Length);
                fileName = fileName.TrimStart('/');

                var needCompress = false;

                CompressResourceInfo compressInfo = null;
                if (buildInfo.TryGetValue(fileName, out compressInfo))
                {
                    newBuildInfo[fileName] = compressInfo;

                    var targetFileInfo = new FileInfo(outputFilePath + fileName);
                    if (targetFileInfo.Exists)
                    {
                        var lastWriteTime = fileInfo.LastWriteTimeUtc.Ticks;
                        if (lastWriteTime != compressInfo.lastWriteTime)
                        {
                            //文件修改时间有变化
                            needCompress = true;
                            Console.WriteLine($@"{fileName}, 文件有修改");
                        }
                    }
                    else
                    {
                        //有此文件压缩记录，但是文件不存在
                        needCompress = true;
                        Console.WriteLine($@"{fileName}, 文件丢失(有此文件压缩记录，但是文件不存在)");
                    }
                }
                else
                {
                    //无此文件压缩记录
                    needCompress = true;
                    Console.WriteLine($@"{fileName}, 新增文件(无此文件压缩记录)");
                }

                if (needCompress)
                {
                    resInfoList.Add(new ResourceInfo()
                    {
                        name = fileName,
                        version = buildVersion,
                        bytesTotal = fileInfo.Length,
                    });
                }
            }

            buildInfo = newBuildInfo;


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

            int i = 0;
            int finishCount = 0;
            int count = 0;
            int c = list_info.Count;

            Console.Write("正在处理文件...");

            var lockObj = new object();
            int curThread = 0;

            var needWriteConsole = !Console.IsOutputRedirected;

            var lockThreadObj = new object();

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

                    lock (lockThreadObj)
                    {
                        curThread++;
                    }

                    var thread = new Thread((par) =>
                    {
                        var param = (FileThreadParam)par;
                        var fileName = param.FileName;
                        var resInfo = param.ResInfo;
                        var orgFilePath = inputPath + fileName;
                        var bytes = File.ReadAllBytes(orgFilePath);

                        //if (IsInvalid)
                        //{
                        //    if (fileName.Contains("/"))
                        //    {
                        //        if (bytes.Length > 50 && (i % 4 == 0))
                        //        {
                        //            //Console.WriteLine($@"do {fileName}");
                        //            var startPos = bytes.Length / 2 - 10;
                        //            bytes[startPos] = 128;
                        //            bytes[startPos + 2] = 66;
                        //            bytes[startPos + 4] = 66;
                        //            bytes[startPos + 6] = 66;
                        //            bytes[startPos + 8] = 66;
                        //            bytes[startPos + 10] = 66;
                        //            bytes[startPos + 12] = 66;
                        //            bytes[startPos + 14] = 66;
                        //            bytes[startPos + 16] = 66;
                        //            bytes[0] = 69;
                        //            bytes[1] = 77;
                        //            bytes[2] = 98;
                        //            bytes[3] = 74;
                        //        }
                        //    }
                        //}

                        bytes = GZipCompresser.compress(bytes);
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


                        var md5 = MD5Utils.BytesToMD5(bytes);

                        var targetFilePath = outputFilePath + fileName;
                        //var targetFilePath = outputFilePath + fileName + "_" + md5;

                        File.WriteAllBytes(targetFilePath, bytes);

                        var writeTime = File.GetLastWriteTimeUtc(orgFilePath).Ticks;
                        var compressInfo = new CompressResourceInfo()
                        {
                            path = fileName,
                            //version = resInfo.version,
                            bytesTotal = bytes.Length,
                            md5 = md5,
                            lastWriteTime = writeTime,
                        };

                        lock (lockObj)
                        {
                            buildInfo[compressInfo.path] = compressInfo;

                            finishCount++;
                            count++;

                            //Console.WriteLine($@"finish {compressInfo.path}");
                        }

                        lock (lockThreadObj)
                        {
                            curThread--;
                            //Console.WriteLine($@"{curThread}/{maxThread}");
                        }

                    });
                    thread.Priority = ThreadPriority.Lowest;
                    thread.Start(threadParam);
                }

                if (needWriteConsole)
                {
                    //var backStr = new StringBuilder();
                    //for (int j = 0; j < backNum; j++)
                    //{
                    //    backStr.Append("\u0008");
                    //}
                    //var writeConsoleStr = string.Format("{0}/{1} thread:{2}/{3}", i, c, curThread, maxThread);
                    //Console.Write(backStr.ToString() + writeConsoleStr);
                    //backNum = encoding.GetBytes(writeConsoleStr).Length;
                }


            }


            time.Stop();

            Console.WriteLine();

            var FileInfoBytes = encoding.GetBytes(buildInfo.ToAssetInfo());

            File.WriteAllText(buildInfoPath, buildInfo.ToBuildInfo(), encoding);
            File.WriteAllBytes(outputFilePath + "assetInfo.txt", FileInfoBytes);
            File.WriteAllBytes(outputFilePath + "assetInfo_compressed.txt", LZMACompresser.compress(FileInfoBytes));
            File.WriteAllBytes(outputPath + "assetVersion.txt", encoding.GetBytes(buildVersion));
            File.WriteAllBytes(outputFilePath + "assetVersion.txt", encoding.GetBytes(buildVersion));

            Console.WriteLine();
            Console.WriteLine($@"已处理{count}个变化文件，生成了{buildInfo.Count}个文件信息");
            Console.WriteLine($@"耗时{time.Elapsed.TotalSeconds}秒");
        }

        public class FileThreadParam
        {
            public string FileName;
            public ResourceInfo ResInfo;
        }

        public class CompressBuildInfo : Dictionary<string, CompressResourceInfo>
        {

            static public CompressBuildInfo ReadFromFile(string path)
            {
                if (File.Exists(path))
                {
                    var json = File.ReadAllText(path, encoding);
                    return JsonConvert.DeserializeObject<CompressBuildInfo>(json);
                }
                return null;
            }

            public string ToAssetInfo()
            {
                var sb = new StringBuilder();
                foreach (var info in Values)
                {
                    sb.Append(info.path);
                    sb.Append(",");
                    //sb.Append(info.version);
                    //sb.Append(",");
                    sb.Append(info.bytesTotal);
                    sb.Append(",");
                    sb.Append(info.md5);
                    sb.Append("\r\n");
                }
                if (this.Count != 0) sb.Remove(sb.Length - 2, 2);
                return sb.ToString();
            }

            public string ToBuildInfo()
            {
                var dic = Values.OrderBy(item => item.path).ToDictionary(item => item.path);

                return JsonConvert.SerializeObject(dic, Formatting.Indented);
            }

        }

        //static private bool IsInvalid = false;

    }

}
