using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KLib;

namespace UnityBuildTool
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("CurrentDirectory:" + Environment.CurrentDirectory);
#if !DEBUG
            try
#endif
            {
                var dic = CommandParse.parse(args);

                var command = dic["command"];

                if (command == "processBuildInfo")
                {
                    var buildInfoPath = dic["buildInfoPath"];
                    var tamplateDir = dic["tamplateDir"];

                    BuildVersionProcesser.Process(buildInfoPath, tamplateDir);
                }
            }
#if !DEBUG
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
#endif
        }
    }
}
