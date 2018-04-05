using KLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ProtocolGenerater
{
    class Program
    {
        static void Main(string[] args)
        {

            var dic_params = CommandParse.parse(args);

#if DEBUG
            dic_params["protocol"] = @"C:\work\VS\project\kakaTools\ProtocolGenerater\ProtocolGenerater\protocol";
            dic_params["template"] = @"C:\work\VS\project\kakaTools\ProtocolGenerater\ProtocolGenerater\templates\csharp\template_csharp.xml";
            dic_params["output"] = @"J:\codes";

            dic_params["protocol"] = @"C:\work\unity\FrameSync\Protocol";
            dic_params["template"] = @"C:\work\unity\FrameSync\Protocol\templates\template_csharp.xml";
#endif

            if (dic_params.ContainsKey("protocol") && dic_params.ContainsKey("template") && dic_params.ContainsKey("output"))
            {
                //            var codeGenerater = new CodeGenerater();

                //            codeGenerater.generate(
                //dic_params["protocol"],
                //dic_params["template"],
                //dic_params["output"]
                //);
#if !DEBUG
                try
#endif
                {
                    var codeGenerater = new CodeGenerater();

                    codeGenerater.generate(
                    dic_params["protocol"],
                    dic_params["template"],
                    dic_params["output"]
                    );
                }
#if !DEBUG
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.ReadLine();
                }
#endif
            }
            else
            {
                Console.WriteLine(Properties.Resources.usage);
                Console.ReadLine();
            }
#if DEBUG
            Console.WriteLine("按任意键退出");
            Console.ReadLine();
#endif
        }
    }
}
