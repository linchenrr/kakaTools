using KLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetBundleBuilder
{
    class Program
    {
        static int Main(string[] args)
        {
            //Console.InputEncoding = Encoding.UTF8;
            //Console.OutputEncoding = Encoding.UTF8;

            //KLibInvalid.RemoteCheckAsync(KLibInvalid.AssetBundleBuilderURL, (info) =>
            //{
            //    Console.WriteLine(info.IsInvalid);
            //});
            //Console.ReadLine();
            //return 0;

            CommandLineTool.DisbleQuickEditMode();

            if (Console.IsOutputRedirected)
            {
                Console.OutputEncoding = Encoding.UTF8;
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            }

            var dic = CommandParse.parse(args);

            var thread = Environment.ProcessorCount;
            if (dic.ContainsKey("thread"))
                thread = Convert.ToInt32(dic["thread"]);

            var method = "gzip";
            if (dic.ContainsKey("method"))
                method = dic["method"];

            if (dic.ContainsKey("specialNames"))
            {
                var parts = dic["specialNames"].Split(',');
                foreach (var part in parts)
                {
                    UnityAssetBundleBuilder.SpecialNames.Add(part.ToLower());
                }
            }

            if (dic.ContainsKey("input") && dic.ContainsKey("output"))
            {
#if !DEBUG
                try
#endif
                {
                    UnityAssetBundleBuilder.build(dic["input"], dic["output"], thread);
#if DEBUG
                    //Console.WriteLine("完成，按任意键结束");
                    //Console.ReadLine();
#endif
                }
#if !DEBUG
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.Source);
                    Console.WriteLine(e.StackTrace);
                    Console.Error.Write(e);
                    if (Console.IsErrorRedirected == false)
                        Console.ReadLine();
                    return 5;
                }
#endif
            }
            else
            {
                Console.WriteLine("-input 源assetbundle文件夹");
                Console.WriteLine("-output 导出目录");
                Console.Error.Write("-input 源assetbundle文件夹 -output 导出目录");
                if (Console.IsErrorRedirected == false)
                    Console.ReadLine();
                return 7;
            }

            return 0;
        }
    }
}
