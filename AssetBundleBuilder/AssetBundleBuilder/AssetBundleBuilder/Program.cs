using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KLib;

namespace AssetBundleBuilder
{
    class Program
    {
        static int Main(string[] args)
        {
            //Console.InputEncoding = Encoding.UTF8;
            //Console.OutputEncoding = Encoding.UTF8;

            var dic = CommandParse.parse(args);

            var thread = Environment.ProcessorCount;
            if (dic.ContainsKey("thread"))
                thread = Convert.ToInt32(dic["thread"]);

            if (dic.ContainsKey("input") && dic.ContainsKey("output"))
            {
#if !DEBUG
                try
#endif
                {
                    UnityAssetBundleBuilder.build(dic["input"], dic["output"], thread);
#if DEBUG
                    Console.ReadLine();
#endif
                }
#if !DEBUG
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.Source);
                    Console.WriteLine(e.StackTrace);
                    Console.ReadLine();
                    return 5;
                }
#endif
            }
            else
            {
                Console.WriteLine("-input 源assetbundle文件夹，必须是包含svn信息的根目录");
                Console.WriteLine("-output 导出目录");
                Console.ReadLine();
            }

            return 0;
        }
    }
}
