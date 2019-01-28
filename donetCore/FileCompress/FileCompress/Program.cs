using KLib;
using System;

namespace FileCompress
{
    class Program
    {
        static void Main(string[] args)
        {
            CommandMode.exec(CommandParse.parse(args));
        }
    }
}
