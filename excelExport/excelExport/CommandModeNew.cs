using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using KLib;

namespace excelExport
{
    public class CommandModeNew
    {

        static private Dictionary<String, String> dic_arg;

        static public void exec(Dictionary<String, String> args)
        {

            dic_arg = args;

            if (args.ContainsKey("showExpires"))
            {
                Console.WriteLine("工具到期日期为:" + ExcelGenerater.ExpiresTime.ToShortDateString());

                return;
            }


            if (args.ContainsKey("excel"))
            {
                String[] inputPathList = args["excel"].Split(',');

                String output = "";
                if (args.ContainsKey("dataPath"))
                    output = args["dataPath"] + @"\";

                String prefix_primaryKey = "[$]";
                if (args.ContainsKey("prefix_primaryKey"))
                    prefix_primaryKey = args["prefix_primaryKey"];

                String prefix_IgnoreSheet = "#";
                if (args.ContainsKey("prefix_IgnoreSheet"))
                    prefix_IgnoreSheet = args["prefix_IgnoreSheet"];

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
                            MessageBox.Show("无效的参数 -compress:" + args["compress"]);
                            return;
                    }
                }

                Boolean ignoreBlank = true;
                if (args.ContainsKey("ignoreBlank"))
                    ignoreBlank = getArgsBool("ignoreBlank");

                String fileExt = ".kk";
                if (args.ContainsKey("ext"))
                    fileExt = args["ext"];
                if (fileExt.IndexOf(".") < 0)
                    fileExt = "." + fileExt;

                if (args.ContainsKey("exportDataBytes"))
                    ExcelGenerater.exportDataBytes = getArgsBool("exportDataBytes");

                if (args.ContainsKey("exportDatajson"))
                    ExcelGenerater.exportDatajson = getArgsBool("exportDatajson");

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
                    ExcelGenerater.export(inputPathList, output, compress, prefix_primaryKey, prefix_IgnoreSheet, ignoreBlank);

                }
#if !DEBUG
                catch (Exception e)
                {
                    if (string.IsNullOrEmpty(ExcelGenerater.curExcel))
                        Console.WriteLine($@"出现异常:
{e.Message}");
                    else
                    {
                        var infoStr = $@"处理文件{ExcelGenerater.curExcel}";
                        if (!string.IsNullOrEmpty(ExcelGenerater.curSheet))
                            infoStr += $@"中表{ExcelGenerater.curSheet}";
                        Console.WriteLine($@"{infoStr}时出现异常:

{e.Message}");
                    }
                    MessageBox.Show(@"导出时出现错误                                         

");
                    Console.ReadLine();
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
            }

        }

        static private bool getArgsBool(string name)
        {
            return dic_arg[name].Trim().ToLower() == "true";
        }

    }
}
