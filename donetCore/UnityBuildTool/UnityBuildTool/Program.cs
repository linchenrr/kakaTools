using KLib;
using System;
using System.Text;

namespace UnityBuildTool
{
    class Program
    {
        static int Main(string[] args)
        {
            CommandLineTool.DisbleQuickEditMode();

            if (Console.IsOutputRedirected)
            {
                Console.OutputEncoding = Encoding.UTF8;
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            }

            Console.WriteLine("CurrentDirectory:" + Environment.CurrentDirectory);
            try
            {
                var dic = CommandParse.parse(args);

                var command = dic["command"];

                if (command == "processBuildInfo")
                {
                    var buildInfoPath = dic["buildInfoPath"];
                    var tamplateDir = dic["tamplateDir"];

                    BuildVersionProcesser.Process(buildInfoPath, tamplateDir);
                }

#if DEBUG
                Console.ReadLine();
#endif
            }
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
            return 0;
        }
    }
}
