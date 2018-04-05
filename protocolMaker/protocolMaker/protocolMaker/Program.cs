using org.superkaka.kakalib.tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace protocolMaker
{
    class Program
    {
        static void Main(string[] args)
        {

            var dic_args = CommandParse.parse(args);

            if (dic_args.ContainsKey("input") &&
                dic_args.ContainsKey("output"))
            {
                String package = "enum";
                if (dic_args.ContainsKey("package"))
                    package = dic_args["package"];
                String className = "Protocol";
                if (dic_args.ContainsKey("className"))
                    className = dic_args["className"];

                try
                {

                    String outputFileName = dic_args["output"] + @"\" + className + ".as";
                    FileUtil.writeFile(outputFileName, makeProtocol(dic_args["input"], package, className));
                    Console.WriteLine("成功生成文件  " + outputFileName);
                    //String protocol = Encoding.UTF8.GetString(FileUtil.readFile(dic_args["input"]));
                    //FileUtil.writeFile(dic_args["output"] + "/" + dic_args["className"] + ".as", Encoding.UTF8.GetBytes(protocol));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    //Console.ReadLine();
                }

            }
            else
            {
                Console.WriteLine(protocolMaker.Properties.Resources.usage);
                //Console.ReadLine();
            }

        }

        static private Byte[] makeProtocol(String path, String package, String className)
        {

            String template = protocolMaker.Properties.Resources.template;
            template = template.Replace("$(className)", className);
            template = template.Replace("$(package)", package);

            DataSet ds = new DataSet();
            ds.ReadXml(path);
            DataRowCollection rows = ds.Tables["message"].Rows;

            StringBuilder sb = new StringBuilder();
            var ed = "\r\n";

            int i = 0;
            int c = rows.Count;

            while (i < c)
            {
                sb.Append(ed + "		static public const " + rows[i]["name"] + ":int = " + rows[i]["id"] + ";" + ed + "		");
                i++;
            }

            template = template.Replace("$(body)", sb.ToString());

            return Encoding.UTF8.GetBytes(template);
        }

    }
}
