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
    public class FileInfoConfigMaker
    {

        static private String path_entries = ".svn/entries";

        static public String makeCfg(String path_folder, IResourceInfoGenerator resInfoGenerator = null)
        {

            if (resInfoGenerator == null)
                resInfoGenerator = new ResourceInfoGenerator();

            ArrayList list = new ArrayList();



            //ArrayList a1 = new ArrayList();
            //ArrayList a2 = new ArrayList();
            //readFromSqlite(@"E:\flash\project\sgt\7k分支", a1);
            //doMakeCfg(@"E:\flash\project\sgt\复件 7k分支", "", a2);

            //Hashtable t1 = new Hashtable();
            //foreach (ResourceInfo rs in a1)
            //{
            //    t1[rs.name] = rs;
            //}

            //Hashtable t2 = new Hashtable();
            //foreach (ResourceInfo rs in a2)
            //{
            //    t2[rs.name] = rs;
            //}

            //ArrayList aa = new ArrayList();
            //foreach (String key in t1.Keys)
            //{
            //    if (t2[key] == null)
            //        aa.Add(key);
            //}

            //ArrayList bb = new ArrayList();
            //foreach (String key in t2.Keys)
            //{
            //    if (t1[key] == null)
            //        bb.Add(key);
            //}


            //String path_svnEntries = path_folder + "/" + path_entries;

            String buildVersion;
            try
            {
                buildVersion = readFromSqlite(path_folder, list);
            }
            catch (Exception)
            {
                buildVersion = getBuildVersion(path_folder);
                doMakeCfg(path_folder, "", list);
            }

            if (null == buildVersion)
            {
                return "无效的svn目录";
            }

            ResourceInfo[] resInfoList = (ResourceInfo[])list.ToArray(typeof(ResourceInfo));

            File.WriteAllBytes(path_folder + "/" + resInfoGenerator.resourceInfoFileName, resInfoGenerator.resourceInfoToBytes(resInfoList));

            File.WriteAllBytes(path_folder + "/" + resInfoGenerator.buildVersionFileName, resInfoGenerator.buildVersionToBytes(buildVersion));

            File.WriteAllBytes(path_folder + "/" + "buildVersion.txt", Encoding.UTF8.GetBytes(buildVersion));

            return "buildVersion:" + buildVersion + "\r\n已生成" + list.Count + "个文件信息";

        }

        static private String getBuildVersion(String path_folder)
        {

            String path_svnEntries = path_folder + "/" + path_entries;

            if (File.Exists(path_svnEntries))
            {

                Byte[] ba = FileUtil.readFile(path_svnEntries);

                String entries = Encoding.UTF8.GetString(ba);

                Regex reg = new Regex(@"\s+dir\s+(\d+)\s+");

                Match mh = reg.Match(entries);

                if (mh.Groups.Count == 1) return null;

                String result = mh.Groups[1].Value;

                return result;

            }
            return null;
        }

        /// <summary>
        /// 1.6及之前版本解析算法
        /// 已知问题：对于某些曾经删除的文件解析不正确导致导出时缺少该文件信息
        /// </summary>
        /// <param name="path_baseFolder"></param>
        /// <param name="path_subFolder"></param>
        /// <param name="list"></param>
        static private void doMakeCfg(String path_baseFolder, String path_subFolder, ArrayList list)
        {

            String path_folder = path_baseFolder + "/" + path_subFolder;

            String path_svnEntries = path_folder + "/" + path_entries;

            if (File.Exists(path_svnEntries))
            {
                DirectoryInfo di = new DirectoryInfo(path_folder);

                Byte[] ba = FileUtil.readFile(path_svnEntries);

                String entries = Encoding.UTF8.GetString(ba);

                Regex reg = new Regex(@"[^\f\n\r\t\v]+\s+file\s+[\s\S]+?\s+\d+\s+[^\f\n\r\t\v]+");
                Regex reg_file = new Regex(@"^[^\f\n\r\t\v]+");
                Regex reg_version = new Regex(@"\s+(\d+)\s+[^\f\n\r\t\v]+$");

                MatchCollection mc = reg.Matches(entries);

                foreach (Match mh in mc)
                {

                    String str = mh.Value;

                    Match mh_ver = reg_version.Match(str);

                    String file = reg_file.Match(str).Value;
                    String ver = mh_ver.Groups[1].Value;

                    FileInfo fi = new FileInfo(path_folder + "/" + file);
                    if (!fi.Exists) continue;

                    ResourceInfo fc = new ResourceInfo();
                    fc.name = path_subFolder + "/" + file;
                    if (fc.name[0] == '/') fc.name = fc.name.Substring(1, fc.name.Length - 1);
                    fc.version = ver;
                    fc.bytesTotal = fi.Length;

                    list.Add(fc);

                }

                //枚举子目录进行递归查找

                DirectoryInfo[] list_di = di.GetDirectories();
                foreach (DirectoryInfo subDi in list_di)
                {
                    doMakeCfg(path_baseFolder, path_subFolder + "/" + subDi.Name, list);
                }

            }
        }

        static private String readFromSqlite(String path, ArrayList list)
        {

            //Pooling设置为true时，SQL连接将从连接池获得，如果没有则新建并添加到连接池中,默认是true。
            //FailIfMissing默认为false，如果数据库文件不存在，会自动创建一个新的，若设置为true，将不会创建，而是抛出异常信息。
            SQLiteConnection conn = new SQLiteConnection("Data Source=" + path + "/.svn/wc.db;Pooling=true;FailIfMissing=true");

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

            cmd.CommandText = "SELECT * FROM [NODES] where [kind] = 'file' and [presence] = 'normal'";
            //cmd.CommandText = "SELECT * FROM [NODES]";
            sd.Fill(dataset, "versions");
            DataTable versions = dataset.Tables["versions"];
            foreach (DataRow row in versions.Rows)
            {
                ResourceInfo resInfo = new ResourceInfo();
                resInfo.name = row["local_relpath"].ToString();
                resInfo.version = row["changed_revision"].ToString();
                resInfo.bytesTotal = (long)row["translated_size"];
                list.Add(resInfo);
            }

            conn.Close();

            return buildVersion.Rows[0]["revision"].ToString();

        }
    }

}
