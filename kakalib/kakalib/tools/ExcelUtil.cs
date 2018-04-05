using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data.OleDb;
using KLib;
using System.Data;
using System.IO;


namespace KLib
{
    public class ExcelUtil
    {

        static private KDataPackager packager = new KDataPackager();

        static public void export(String inputPath, String outputPath, CompressOption op, String prefix_primaryKey, String prefix_IgnoreSheet, String prefix_IgnoreLine, String prefix_IgnoreColumn, Boolean ignoreBlank, Boolean merge)
        {

            if (Directory.Exists(inputPath))
            {
                DirectoryInfo di = new DirectoryInfo(inputPath);

                FileInfo[] fileInfos = di.GetFiles("*.xls", SearchOption.AllDirectories);

                for (int k = 0; k < fileInfos.Length; k++)
                {
                    //如果不是隐藏文件  则解析
                    if ((fileInfos[k].Attributes & FileAttributes.Hidden) == 0)
                    {
                        export(fileInfos[k].FullName, outputPath, op, prefix_primaryKey, prefix_IgnoreSheet, prefix_IgnoreLine, prefix_IgnoreColumn, ignoreBlank, merge);
                    }

                }
            }
            else
            {
                KTable[] sheets = doExport(inputPath, prefix_primaryKey, prefix_IgnoreSheet, prefix_IgnoreLine, prefix_IgnoreColumn, ignoreBlank, merge);

                if (null == outputPath || "" == outputPath)
                {
                    FileInfo fi = new FileInfo(inputPath);
                    outputPath = fi.DirectoryName;
                }

                if (!Directory.Exists(outputPath))
                    throw new Exception("导出路径\"" + outputPath + "\"不存在");

                foreach (KTable sheet in sheets)
                {

                    packager.reset();
                    packager.writeTable(sheet);

                    String path = outputPath + "/" + sheet.name + ".kk";

                    Byte[] bytes = packager.data;

                    MemoryStream inStream = new MemoryStream(bytes);
                    MemoryStream outStream = new MemoryStream();

                    ICompresser compresser;

                    switch (op)
                    {

                        case CompressOption.lzma:
                            compresser = new LZMACompresser();
                            compresser.compress(inStream, outStream);
                            break;

                        case CompressOption.zlib:
                            compresser = new ZlibCompresser();
                            compresser.compress(inStream, outStream);
                            break;

                        case CompressOption.none:
                            outStream = inStream;
                            break;

                        default:
                            throw new Exception();

                    }

                    FileStream fs = File.Create(path);
                    outStream.WriteTo(fs);
                    fs.Close();

                    outStream.Dispose();

                }

            }

        }

        static public void export(String[] inputPathList, String[] outputPathList, CompressOption op, String prefix_primaryKey, String prefix_IgnoreSheet, String prefix_IgnoreLine, String prefix_IgnoreColumn, Boolean ignoreBlank, Boolean merge)
        {

            int i = 0;
            int len = inputPathList.Length;

            while (i < len)
            {

                export(inputPathList[i], outputPathList[i], op, prefix_primaryKey, prefix_IgnoreSheet, prefix_IgnoreLine, prefix_IgnoreColumn, ignoreBlank, merge);

                i++;

            }

        }

        static public void export(String[] inputPathList, String outputPath, CompressOption op, String prefix_primaryKey, String prefix_IgnoreSheet, String prefix_IgnoreLine, String prefix_IgnoreColumn, Boolean ignoreBlank, Boolean merge)
        {

            int i = 0;
            int len = inputPathList.Length;

            while (i < len)
            {

                export(inputPathList[i], outputPath, op, prefix_primaryKey, prefix_IgnoreSheet, prefix_IgnoreLine, prefix_IgnoreColumn, ignoreBlank, merge);

                i++;

            }

        }

        static KTable[] doExport(String source, String prefix_primaryKey, String prefix_IgnoreSheet, String prefix_IgnoreLine, String prefix_IgnoreColumn, Boolean ignoreBlank, Boolean merge)
        {

            //HDR参数：YES忽视第一行；NO包括第一行
            String ConnStr = "Provider=Microsoft.Ace.OleDB.12.0;Data Source=" + source + ";Extended Properties='Excel 12.0;HDR=NO;IMEX=1;'";

            OleDbConnection conn = new OleDbConnection(ConnStr);
            conn.Open();

            ArrayList sheets = new ArrayList();

            String[] sheetNames = getSheetName(conn);

            //KTable[] list = new KTable[sheetNames.Length];

            foreach (String sheetName in sheetNames)
            {

                //忽略表
                if (isPrefix(sheetName, prefix_IgnoreSheet))
                    continue;

                KTable excelSheet = processSheet(conn, sheetName, prefix_primaryKey, prefix_IgnoreLine, prefix_IgnoreColumn, ignoreBlank);
                if (excelSheet != null)
                {
                    sheets.Add(excelSheet);
                }

            }

            KTable[] tables = (KTable[])sheets.ToArray(Type.GetType("KLib.KTable"));

            if (merge)
            {

                KTable mainSheet = tables[0];
                mainSheet.name = Path.GetFileNameWithoutExtension(source);


                for (int i = 1; i < tables.Length; i++)
                {
                    try
                    {
                        mainSheet.dataTable.Merge(tables[i].dataTable);
                    }
                    catch
                    {
                        throw new Exception("文件" + Path.GetFileName(source) + "执行合并子表操作时出错\r\n原因：子表的主键名不一致");
                    }
                }

                tables = new KTable[] { mainSheet };

            }

            conn.Close();
            conn.Dispose();

            return tables;

        }


        static private KTable processSheet(OleDbConnection conn, String sheetName, String prefix_primaryKey, String prefix_IgnoreLine, String prefix_IgnoreColumn, Boolean ignoreBlank)
        {

            string query = "SELECT   *   FROM  [" + sheetName + "]";


            OleDbCommand oleCommand = new OleDbCommand(query, conn);
            OleDbDataAdapter oleAdapter = new OleDbDataAdapter(oleCommand);
            DataSet myDataSet = new DataSet();

            //   将   Excel   的[Sheet1]表内容填充到   DataSet   对象 
            oleAdapter.Fill(myDataSet, sheetName);


            DataTable dt = myDataSet.Tables[0];


            if (dt.Rows.Count == 1 && dt.Columns.Count == 1)
            {
                return null;
            }


            //while (true)
            //{
            //    //忽略行
            //    if (isPrefix(dt.Rows[0][0].ToString(), prefix_IgnoreLine))
            //    {

            //        dt.Rows.RemoveAt(0);
            //    }
            //    else
            //    {
            //        break;
            //    }
            //}

            Object[] header = new Object[dt.Rows[0].ItemArray.Length];
            dt.Rows[0].ItemArray.CopyTo(header, 0);

            if (ignoreBlank)
            {
                ///消除空白行
                int maxColumn = header.Length;
                int row = dt.Rows.Count - 1;
                while (row >= 0)
                {
                    Boolean hasData = false;
                    DataRow dataRow = dt.Rows[row];
                    int column = 0;
                    while (column < maxColumn)
                    {
                        if (dataRow[column].ToString() != "")
                        {
                            hasData = true;
                            break;
                        }
                        column++;
                    }

                    if (!hasData)
                    {
                        dt.Rows.RemoveAt(row);
                    }

                    row--;
                }
            }

            int i = dt.Rows.Count - 1;
            while (i >= 0)
            {
                //忽略行
                if (isPrefix(dt.Rows[i][0].ToString(), prefix_IgnoreLine))
                {
                    dt.Rows.RemoveAt(i);
                }
                i--;
            }

            header = new Object[dt.Rows[0].ItemArray.Length];
            dt.Rows[0].ItemArray.CopyTo(header, 0);


            i = header.Length - 1;
            while (i >= 0)
            {
                //忽略列
                if (isPrefix(header[i].ToString(), prefix_IgnoreColumn))
                {
                    dt.Columns.RemoveAt(i);
                }
                i--;
            }

            header = new Object[dt.Rows[0].ItemArray.Length];
            dt.Rows[0].ItemArray.CopyTo(header, 0);

            if (ignoreBlank)
            {
                ///消除空白列
                int column = header.Length - 1;
                while (column >= 0)
                {
                    if (header[column].ToString() == "")
                    {
                        dt.Columns.RemoveAt(column);
                    }
                    column--;
                }

                header = new String[dt.Rows[0].ItemArray.Length];
                dt.Rows[0].ItemArray.CopyTo(header, 0);

            }


            ///去掉表头
            dt.Rows.RemoveAt(0);

            KTable sheet = new KTable();

            ///查找主键并赋值
            i = 0;
            while (i < header.Length)
            {
                String head = header[i].ToString();

                dt.Columns[i].ColumnName = head;

                if (isPrefix(head, prefix_primaryKey))
                {
                    head = head.Substring(prefix_primaryKey.Length);
                    sheet.primaryKeyIndex = i;
                    header[i] = head;
                    break;
                }
                i++;
            }

            sheet.name = sheetName.Substring(0, sheetName.Length - 1);
            sheet.header = (String[])header;
            sheet.dataTable = dt;

            //给DataTable设置主键，用于后面的合并表
            try
            {
                dt.PrimaryKey = new DataColumn[] { dt.Columns[sheet.primaryKeyIndex] };
            }
            catch
            {
                throw new Exception("表" + sheet.name + "的主键值有重复");
            }

            return sheet;

        }

        /// <summary>
        /// 获取所有工作表名
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        static private String[] getSheetName(OleDbConnection conn)
        {

            DataTable tb = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

            String[] excelSheets = new String[tb.Rows.Count];
            int i = 0;

            // Add the sheet name to the string array.
            foreach (DataRow row in tb.Rows)
            {
                var sheetName = row["TABLE_NAME"].ToString().Trim('\'');
                //有些带筛选区域的表会导致出现多余的表 类似   "表名$_xlnm#_FilterDatabase"
                if (sheetName.IndexOf("FilterDatabase") >= 0)
                    continue;

                excelSheets[i] = sheetName;
                i++;
            }

            return excelSheets;

        }

        /// <summary>
        /// 是否包含前缀
        /// </summary>
        /// <param name="str"></param>
        /// <param name="refix"></param>
        /// <returns></returns>
        static private Boolean isPrefix(String str, String prefix)
        {
            if (null == prefix) return false;
            prefix = prefix.Trim();
            if (prefix == "") return false;
            return str.IndexOf(prefix) == 0;
        }

    }
}
