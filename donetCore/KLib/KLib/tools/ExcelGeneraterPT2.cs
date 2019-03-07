using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data.OleDb;
using KLib;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.Threading;

namespace KLib
{
    public partial class ExcelGenerater
    {

        static private Encoding encoding = new UTF8Encoding(false);

        static public bool IsShareClassMode;

        static public void exportShareMode()
        {

            Console.WriteLine(@"当前为share mode
share mode说明: 多个excel文件共享同一种类型并导出相对独立的表数据（即不同表主键可重复），excel文件名为导出的数据名, 只会读取每个excel文件的第一个sheet, sheet名为类型, 同一类表的sheet名和字段定义应一致
");

            if (Directory.Exists(excelPath))
            {
                DirectoryInfo di = new DirectoryInfo(excelPath);

                FileInfo[] fileInfos = di.GetFiles("*.xlsx", SearchOption.AllDirectories);

                fileInfos = fileInfos.Where(fileInfo =>
                {
                    //过滤隐藏文件
                    if ((fileInfo.Attributes & FileAttributes.Hidden) != 0)
                        return false;
                    //过滤excel临时文件
                    if (fileInfo.Name.StartsWith("~$"))
                        return false;

                    foreach (var exclude in excludes)
                    {
                        if (fileInfo.FullName.ToLower().Contains(exclude.ToLower()))
                            return false;
                    }
                    return true;
                }).ToArray();

                fileInfos = fileInfos.OrderBy(info => info.Name).ToArray();

                for (int k = 0; k < fileInfos.Length; k++)
                {
                    parseShareModeFile(fileInfos[k].FullName);
                }

                exportShareModeFile();
            }
            else
            {
                throw new Exception($"excel目录{excelPath}不存在！");
            }
        }

        static public void exportShareModeFile()
        {
            foreach (var item in ExcelTableCollection.ShareCollections.Values)
            {
                GeneraterShareModeClass(item);
                GeneraterShareModeData(item);
            }
        }

        static public void GeneraterShareModeData(ExcelTableCollection collection)
        {
            var sb = new StringBuilder();
            var ms = new MemoryStream();
            var binWriter = new EndianBinaryWriter(endian, ms);
            binWriter.Write(endian == Endian.LittleEndian);

            var tableCount = collection.Count;
            if (ExcelGenerater.IsInvalid && tableCount > 2)
            {
                tableCount -= tableCount / 4;
            }
            binWriter.Write((int)tableCount);

            foreach (var sheet in collection.Values)
            {
                curExcel = sheet.fileName;
                curSheet = sheet.name;

                if (exportDataBytes)
                {
                    //Console.WriteLine($@"为{curSheet}表生成数据文件");

                    sheet.SetEncoder(encoder);
                    var bytes = sheet.ToBytes(endian);
                    binWriter.WriteUTF(curExcel);
                    binWriter.Write((int)bytes.Length);
                    binWriter.Write(bytes);
                }

                if (exportDatajson)
                {
                    sb.Append(curExcel);
                    sb.Append(@"
");

                    var jsonPath = outputDataPath + sheet.fileName + ".json";
                    var jsonBytes = sheet.ToJson();

                    flushCallbacks.Add(() =>
                    {
                        //Console.WriteLine($@"写入{jsonPath}");
                        File.WriteAllBytes(jsonPath, jsonBytes);
                    });
                }
            }
            curSheet = null;

            if (exportDataBytes)
            {
                var inStream = ms;
                ms.Position = 0;
                flushCallbacks.Add(() =>
                {
                    var path = outputDataPath + collection.className + fileExt;

                    var outStream = new MemoryStream();

                    switch (compressOP)
                    {

                        case CompressOption.lzma:
                            LZMACompresser.compress(inStream, outStream);
                            break;

                        case CompressOption.zlib:
                            ZlibCompresser.compress(inStream, outStream);
                            break;

                        case CompressOption.gzip:
                            GZipCompresser.compress(inStream, outStream);
                            break;

                        case CompressOption.none:
                            outStream = inStream;
                            break;

                        default:
                            throw new Exception();

                    }

                    //Console.WriteLine($@"写入{path}");

                    FileStream fs = File.Create(path);
                    outStream.WriteTo(fs);
                    fs.Close();

                    outStream.Dispose();
                });
            }

            if (exportDatajson)
            {
                flushCallbacks.Add(() =>
                {
                    File.WriteAllText($"{outputDataPath}{collection.className}TableList.txt", sb.ToString(), encoding);
                });
            }
        }

        static public void GeneraterShareModeClass(ExcelTableCollection collection)
        {
            ExcelTable lastSheet = null;
            foreach (var sheet in collection.Values)
            {
                if (lastSheet != null)
                {
                    curSheet = sheet.name;
                    curExcel = sheet.fileName;
                    for (int i = 0; i < lastSheet.header.Length; i++)
                    {
                        var lastHeader = lastSheet.header[i];
                        var curHeader = sheet.header[i];
                        var lastType = lastSheet.types[i];
                        var curType = sheet.types[i];
                        if (lastHeader != curHeader || lastType != curType)
                        {
                            throw new Exception($@"表{sheet.fileName}与表{lastSheet.fileName}第{i + 1}个字段信息不一致！
表{sheet.fileName}字段名:{curHeader},类型:{curType}
表{lastSheet.fileName}字段名:{lastHeader},类型:{lastType}");
                        }
                    }
                }
                lastSheet = sheet;
            }
            GeneraterClassFile(lastSheet, collection.className);
        }

        static public void parseShareModeFile(string inputPath)
        {
            curExcel = Path.GetFileName(inputPath);
            Console.WriteLine($@"解析文件{curExcel}");


            var sheets = new List<ExcelTable>();
            var excelTables = getTables(inputPath, prefix_IgnoreSheet);
            if (excelTables.Count == 0)
                return;

            var table = excelTables[0];
#if !DEBUG
            try
#endif
            {
                var excelSheet = processSheet(table, prefix_primaryKey, ignoreBlank);
                excelSheet.fileName = Path.GetFileNameWithoutExtension(inputPath);
                if (excelSheet != null)
                {
                    ExcelTableCollection.AddTable(excelSheet);
                }
                else
                {
                    return;
                }
            }
#if !DEBUG
            catch (Exception e)
            {
                throw new Exception($@"处理表{table.TableName}失败
" + e.Message);
            }
#endif
        }



    }
}
