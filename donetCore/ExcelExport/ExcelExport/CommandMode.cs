using System;
using System.Collections.Generic;
using System.Text;
using KLib;

namespace ExcelExport
{
    public class CommandMode
    {

        static private Dictionary<String, String> dic_arg;

        static public void exec(Dictionary<String, String> args)
        {

            dic_arg = args;

            if (args.ContainsKey("showExpires"))
            {
                Console.WriteLine($@"是否过期:{KLibInvalid.IsInvalid}，工具到期日期为:{KLibInvalid.ExpiresTime.ToShortDateString()}");
                return;
            }

            if (args.ContainsKey("excel"))
            {
                ExcelGenerater.excelPath = args["excel"];


                if (args.ContainsKey("dataPath"))
                    ExcelGenerater.outputDataPath = args["dataPath"] + @"\";

                if (args.ContainsKey("prefix_primaryKey"))
                    ExcelGenerater.prefix_primaryKey = args["prefix_primaryKey"];

                if (args.ContainsKey("prefix_IgnoreSheet"))
                    ExcelGenerater.prefix_IgnoreSheet = args["prefix_IgnoreSheet"];

                string customerEncoder = null;
                if (args.ContainsKey("customerEncoder"))
                    customerEncoder = args["customerEncoder"];

                Endian endian = Endian.LittleEndian;
                if (args.ContainsKey("endian"))
                {
                    if (args["endian"].ToLower() == Endian.BigEndian.ToString().ToLower())
                        endian = Endian.BigEndian;
                }

                CompressOption compress = CompressOption.none;
                if (args.ContainsKey("compress"))
                {
                    switch (args["compress"].ToLower())
                    {
                        case "lzma":
                            compress = CompressOption.lzma;
                            break;

                        case "zlib":
                            compress = CompressOption.zlib;
                            break;

                        case "gzip":
                            compress = CompressOption.gzip;
                            break;

                        default:
                            WindowUtils.Alert("无效的参数 -compress:" + args["compress"]);
                            return;
                    }
                    ExcelGenerater.compressOP = compress;
                }

                if (args.ContainsKey("ignoreBlank"))
                    ExcelGenerater.ignoreBlank = getArgsBool("ignoreBlank");

                String fileExt = ".kk";
                if (args.ContainsKey("ext"))
                    fileExt = args["ext"];
                if (fileExt.IndexOf(".") < 0)
                    fileExt = "." + fileExt;

                if (args.ContainsKey("shareMode"))
                    ExcelGenerater.IsShareClassMode = getArgsBool("shareMode");

                if (args.ContainsKey("exportDataBytes"))
                    ExcelGenerater.exportDataBytes = getArgsBool("exportDataBytes");

                if (args.ContainsKey("exportDatajson"))
                    ExcelGenerater.exportDatajson = getArgsBool("exportDatajson");

                if (args.ContainsKey("mergeSheets"))
                    ExcelGenerater.mergeSheets = getArgsBool("mergeSheets");

                if (args.ContainsKey("writeCellLen"))
                    ExcelGenerater.writeCellLen = getArgsBool("writeCellLen");

                if (args.ContainsKey("writeCellLenExclude"))
                    ExcelGenerater.writeCellLenExclude = args["writeCellLenExclude"].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                if (args.ContainsKey("exclude"))
                    ExcelGenerater.excludes = args["exclude"].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                //if (args.ContainsKey("addLogo"))
                //    ExcelGenerater.addLogo = getArgsBool("addLogo");

                if (args.ContainsKey("defaultArraySpliter"))
                    ExcelGenerater.defaultArraySpliter = args["defaultArraySpliter"].Trim();

                if (args.ContainsKey("commentRowNum"))
                    ExcelGenerater.commentRowNum = Convert.ToInt32(args["commentRowNum"]);

                if (args.ContainsKey("fieldNameRowNum"))
                    ExcelGenerater.fieldNameRowNum = Convert.ToInt32(args["fieldNameRowNum"]);

                if (args.ContainsKey("typeRowNum"))
                    ExcelGenerater.typeRowNum = Convert.ToInt32(args["typeRowNum"]);

                if (args.ContainsKey("dataRowStartNum"))
                    ExcelGenerater.dataRowStartNum = Convert.ToInt32(args["dataRowStartNum"]);
#if !DEBUG
                try
#endif
                {
                    if (args.ContainsKey("template") && args.ContainsKey("codeGeneratePath"))
                    {
                        ExcelGenerater.templatePath = args["template"];
                        ExcelGenerater.codeFolderPath = args["codeGeneratePath"] + @"\";
                    }
                    ExcelGenerater.endian = endian;
                    ExcelGenerater.fileExt = fileExt;
                    ExcelGenerater.customerEncoder = customerEncoder;
                    ExcelGenerater.startExport();

                }
#if !DEBUG
                catch (Exception e)
                {
                    string errMsg = null;
                    if (string.IsNullOrEmpty(ExcelGenerater.curExcel))
                        errMsg = $@"出现异常:
{e}";
                    else
                    {
                        errMsg = $@"处理文件{ExcelGenerater.curExcel}";
                        if (!string.IsNullOrEmpty(ExcelGenerater.curSheet))
                            errMsg += $@"中表{ExcelGenerater.curSheet}";
                        errMsg = $@"{errMsg}时出现异常:
{e}";
                    }
                    Console.WriteLine(errMsg);
                    WindowUtils.Alert($@"导出时出现错误:
{errMsg}");
                    Console.ReadLine();
                    Environment.Exit(5);
                }
#endif
#if DEBUG
                Console.WriteLine();
                Console.WriteLine("已完成");
                Console.ReadLine();
#endif
            }
            else
            {
                Console.WriteLine(Properties.Resources.usage);
                Console.ReadLine();
                Environment.Exit(10);
            }

        }

        static private bool getArgsBool(string name)
        {
            return dic_arg[name].Trim().ToLower() == "true";
        }

    }
}
