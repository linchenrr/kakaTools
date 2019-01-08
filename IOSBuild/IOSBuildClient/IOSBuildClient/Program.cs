using System;

namespace IOSBuildClient
{
    class Program
    {
        static int Main(string[] args)
        {

            var commands = CommandParser.Parse(args);
            var buildRunner = new BuildRunner();
            var success = buildRunner.Start(commands);

#if DEBUG
            Console.ReadLine();
#endif

            if (success)
                return 0;

            return 5;
        }
    }

}
