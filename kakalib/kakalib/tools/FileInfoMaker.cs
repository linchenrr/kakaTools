using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using KLib;
using System.Data.SQLite;
using System.Data;

namespace KLib
{
    public class FileInfoMaker
    {

        static public void makeCfg(String inputPath, String outputPath)
        {
            inputPath = inputPath + "/";
            outputPath = outputPath + "/";

            DirectoryInfo outputDir = new DirectoryInfo(outputPath);
            if (outputDir.Exists)
            {
                outputDir.Delete(true);
            }

            FileUtil.copyDirectoryStruct(inputPath, outputPath);

            ArrayList list = new ArrayList();

            String buildVersion = null;
            try
            {
                buildVersion = readFromSqlite(inputPath, list);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }

            ResourceInfo[] resInfoList = (ResourceInfo[])list.ToArray(typeof(ResourceInfo));

            StringBuilder sb = new StringBuilder();
            StringBuilder lost = new StringBuilder();
            int i = 0;
            int count = 0;
            int c = resInfoList.Length;
            while (i < c)
            {
                ResourceInfo resInfo = resInfoList[i];
                i++;

                String fileName = resInfo.name;
                int idx = fileName.LastIndexOf(".");
                String newFileName;
                if (idx != -1)
                    newFileName = fileName.Insert(idx, "_" + resInfo.version);
                else
                    newFileName = fileName + "_" + resInfo.version;

                FileInfo file = new FileInfo(inputPath + fileName);
                if (file.Exists)
                {

                    file.CopyTo(outputPath + newFileName);

                    sb.Append(fileName);
                    sb.Append(",");
                    sb.Append(newFileName);
                    sb.Append(",");
                    sb.Append(resInfo.version);
                    sb.Append(",");
                    sb.Append(Convert.ToString(resInfo.bytesTotal));
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

            Byte[] FileInfoBytes = Encoding.UTF8.GetBytes(sb.ToString());

            String fileInfoName = "fileInfo_" + buildVersion + ".txt";
            FileUtil.writeFile(outputPath + fileInfoName, FileInfoBytes);
            FileUtil.writeFile(outputPath + "fileInfoName.txt", Encoding.UTF8.GetBytes(fileInfoName));
            FileUtil.writeFile(outputPath + "buildVersion.txt", Encoding.UTF8.GetBytes(buildVersion));

            if (lost.Length != 0)
            {
                Console.WriteLine("未发现以下文件:");
                Console.WriteLine(lost);
            }

            Console.WriteLine("buildVersion:" + buildVersion);
            Console.WriteLine("已生成" + count + "个文件信息");
            //Console.ReadLine();
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
                    Console.WriteLine("获取文件svn信息失败:\r\n" + resInfo.name);
                }
            }

            conn.Close();

            return buildVersion.Rows[0]["revision"].ToString();

        }
    }

}
