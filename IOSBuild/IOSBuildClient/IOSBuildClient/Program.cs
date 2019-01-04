using System;

namespace IOSBuildClient
{
    class Program
    {
        static void Main(string[] args)
        {

            var commands = CommandParser.Parse(args);
            var buildRunner = new BuildRunner();
            buildRunner.Start(commands);

#if DEBUG
            Console.ReadLine();
#endif

        }
    }

}
