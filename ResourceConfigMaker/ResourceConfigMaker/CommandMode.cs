using System;
using System.Collections.Generic;
using System.Text;
using KLib;
using System.Collections;

namespace ResourceConfigMaker
{
    public class CommandMode
    {
        static public void exec(Dictionary<String, String> args)
        {

            if (args.ContainsKey("path"))
                Console.WriteLine(FileInfoConfigMaker.makeCfg((String)args["path"]));
        }
    }
}
