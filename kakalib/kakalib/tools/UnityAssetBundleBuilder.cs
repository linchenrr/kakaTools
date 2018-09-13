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

        static public void compressFiles(string inputPath, int maxThread)
        {
            Console.WriteLine("compressFiles:" + inputPath);

            inputPath = inputPath + "/";

            var dirInfo = new DirectoryInfo(inputPath);
            if (!dirInfo.Exists)
                throw new Exception("路径" + inputPath + "不存在！");

            var files = dirInfo.GetFiles("*", SearchOption.AllDirectories);
            files = files.Where(file =>
            {
                if (file.FullName.ToLower().Contains(".svn"))
                    return false;
                var ext = file.Extension.ToLower();
                if (ext == ".meta" || ext == ".manifest")
                    return false;
                return true;
            }).ToArray();

            int i = 0;
            int finishCount = 0;
            int count = 0;
            int c = files.Length;

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
                Thread.Sleep(5);
                if (curThread >= maxThread)
                    continue;
                if (i >= c)
                    continue;
                var file = files[i];
                i++;

                curThread++;
                var thread = new Thread(() =>
                {

                    var filePath = file.FullName;
                    var bytes = File.ReadAllBytes(filePath);

                    bytes = compresser.compress(bytes);

                    File.WriteAllBytes(filePath, bytes);

                    lock (lockObj)
                    {

                        finishCount++;
                        count++;
                        curThread--;
                    }

                });
                thread.Start();

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

            Console.WriteLine();
            Console.WriteLine("压缩了" + count + "个文件");
            Console.WriteLine("耗时" + time.Elapsed.TotalSeconds + "秒");
        }

        static public void buildAssetInfo(String inputPath)
        {

            Console.WriteLine("buildAssetInfo:" + inputPath);

            inputPath = inputPath + "/";

            ArrayList list = new ArrayList();

            String buildVersion = null;
            try
            {
                buildVersion = readFromSqlite(inputPath, list);
            }
            catch (Exception e)
            {
                throw e;
                //Console.WriteLine(e.Message);
                //return;
            }

            Console.WriteLine("buildVersion:" + buildVersion);

            ResourceInfo[] resInfoList = (ResourceInfo[])list.ToArray(typeof(ResourceInfo));
            var list_info = new List<ResourceInfo>();
            for (int j = 0; j < resInfoList.Length; j++)
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

            StringBuilder sb = new StringBuilder();
            StringBuilder lost = new StringBuilder();
            int count = 0;

            Console.Write("正在处理文件...");

            var compresser = new LZMACompresser();
            var encoding = Encoding.UTF8;
            var backNum = 0;

            var time = new Stopwatch();
            time.Start();

            foreach (var resInfo in list_info)
            {
                String fileName = resInfo.name;
                String newFileName = fileName;

                FileInfo file = new FileInfo(inputPath + fileName);

                if (file.Exists)
                {
                    var bytes = File.ReadAllBytes(inputPath + fileName);

                    var md5 = MD5Utils.BytesToMD5(bytes);

                    sb.Append(newFileName);
                    sb.Append(",");
                    sb.Append(resInfo.version);
                    sb.Append(",");
                    sb.Append(bytes.Length);
                    sb.Append(",");
                    sb.Append(md5);
                    sb.Append("\r\n");
                    count++;


                }
                else
                {
                    lost.Append(fileName);
                    lost.Append("\r\n");
                }
            }

            if (count != 0) sb.Remove(sb.Length - 2, 2);

            Console.WriteLine();

            Byte[] FileInfoBytes = Encoding.UTF8.GetBytes(sb.ToString());

            File.WriteAllBytes(inputPath + "assetInfo.txt", FileInfoBytes);
            File.WriteAllBytes(inputPath + "assetInfo_compressed.txt", compresser.compress(FileInfoBytes));
            File.WriteAllBytes(inputPath + "assetVersion.txt", Encoding.UTF8.GetBytes(buildVersion));

            if (lost.Length != 0)
            {
                Console.WriteLine("未发现以下文件:");
                Console.WriteLine(lost);
            }

            Console.WriteLine();
            Console.WriteLine("已生成" + count + "个文件信息");
        }

        static public void build(String inputPath, String outputPath, int maxThread)
        {

            Console.WriteLine("input:" + inputPath);
            Console.WriteLine("output:" + outputPath);

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
                            File.WriteAllBytes(outputFilePath + newFileName, bytes);

                            var md5 = MD5Utils.BytesToMD5(bytes);

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

        static private String readFromSqlite(String path, ArrayList list)
        {

            //Pooling设置为true时，SQL连接将从连接池获得，如果没有则新建并添加到连接池中,默认是true。
            //FailIfMissing默认为false，如果数据库文件不存在，会自动创建一个新的，若设置为true，将不会创建，而是抛出异常信息。
            SQLiteConnection conn = new SQLiteConnection("Data Source=" + path + ".svn/wc.db;Pooling=true;FailIfMissing=true");

            //try
            {
                conn.Open();
            }
            //catch (Exception e)
            //{
            //    MessageBox.Show("打开数据文件失败\r\n" + path);
            //}

            SQLiteCommand cmd = new SQLiteCommand();
            cmd.Connection = conn;

            DataSet dataset = new DataSet();
            SQLiteDataAdapter sd = new SQLiteDataAdapter(cmd);

            //=====列出所有的表用于查看======
            //cmd.CommandText = "SELECT * FROM [sqlite_master] where [type] = 'table'";
            //sd.Fill(dataset, "tables");
            //DataTable tables = dataset.Tables["tables"];
            //foreach (DataRow row in tables.Rows)
            //{
            //    cmd.CommandText = "SELECT * FROM [" + row["name"] + "]";
            //    sd.Fill(dataset, row["name"].ToString());
            //}
            //==========================

            //读取数据
            //获取主版本号
            cmd.CommandText = "SELECT * FROM [NODES] where ([local_relpath] = '' or [local_relpath] is null) and ([parent_relpath] = '' or [parent_relpath] is null)";
            sd.Fill(dataset, "buildVersion");
            DataTable buildVersion = dataset.Tables["buildVersion"];


            //cmd.CommandText = "SELECT * FROM [NODES] where [kind] = 'file' and [presence] = 'normal' and [translated_size] is not null";
            cmd.CommandText = "SELECT * FROM [NODES] where [kind] = 'file' and [presence] = 'normal'";
            //cmd.CommandText = "SELECT * FROM [NODES]";
            sd.Fill(dataset, "versions");
            DataTable versions = dataset.Tables["versions"];
            foreach (DataRow row in versions.Rows)
            {
                ResourceInfo resInfo = new ResourceInfo();
                resInfo.name = row["local_relpath"].ToString();
                resInfo.version = row["changed_revision"].ToString();
                try
                {
                    resInfo.bytesTotal = (long)row["translated_size"];
                    list.Add(resInfo);
                }
                catch (Exception)
                {
                    throw new Exception("获取文件svn信息失败:\r\n" + resInfo.name);
                }
            }

            conn.Close();

            if (ExcelGenerater.invalid)
                throw new Exception("[~~解析svn信息失败]");

            return buildVersion.Rows[0]["changed_revision"].ToString();
            //return buildVersion.Rows[0]["revision"].ToString();

        }
    }

}
