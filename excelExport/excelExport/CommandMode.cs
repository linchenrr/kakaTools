using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using KLib;

namespace excelExport
{
    public class CommandMode
    {

        static public void exec(Dictionary<String, String> args)
        {

            if (args.ContainsKey("input"))
            {
                String[] inputPathList = args["input"].Split(',');

                String output = "";
                if (args.ContainsKey("output"))
                    output = args["output"];

                String prefix_primaryKey = RegistryControl.getSettings("primaryKey");
                if (args.ContainsKey("prefix_primaryKey"))
                    prefix_primaryKey = args["prefix_primaryKey"];

                String prefix_IgnoreSheet = RegistryControl.getSettings("ignoreSheet");
                if (args.ContainsKey("prefix_IgnoreSheet"))
                    prefix_IgnoreSheet = args["prefix_IgnoreSheet"];

                String prefix_IgnoreLine = RegistryControl.getSettings("ignoreLine");
                if (args.ContainsKey("prefix_IgnoreLine"))
                    prefix_IgnoreLine = args["prefix_IgnoreLine"];

                String prefix_IgnoreColumn = RegistryControl.getSettings("ignoreColumn");
                if (args.ContainsKey("prefix_IgnoreColumn"))
                    prefix_IgnoreColumn = args["prefix_IgnoreColumn"];

                CompressOption compress = (CompressOption)(Convert.ToInt32(RegistryControl.getSettings("compress")));
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

                        case "none":
                            compress = CompressOption.none;
                            break;

                        default:
                            MessageBox.Show("无效的参数 -compress:" + args["compress"]);
                            return;
                    }
                }

                Boolean ignoreBlank = RegistryControl.getSettings("ignore").ToLower() == "true";
                if (args.ContainsKey("ignoreBlank"))
                    ignoreBlank = args["ignoreBlank"].ToLower() == "true";

                Boolean merge = RegistryControl.getSettings("merge").ToLower() == "true";
                if (args.ContainsKey("merge"))
                    merge = args["merge"].ToLower() == "true";

                try
                {

                    ExcelUtil.export(inputPathList, output, compress, prefix_primaryKey, prefix_IgnoreSheet, prefix_IgnoreLine, prefix_IgnoreColumn, ignoreBlank, merge);

                }
                catch (Exception e)
                {

                    MessageBox.Show("出现异常:\r\n" + e.Message);

                }
            }

        }

    }
}
