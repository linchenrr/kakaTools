using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace windowHelp
{
    public class CommandParse
    {

        static public Dictionary<String, String> parse(String[] args)
        {

            var dic = new Dictionary<String, String>();
            int i = 1;
            while (i < args.Length - 1)
            {
                if (args[i].Substring(0, 1) == "-")
                {
                    dic[args[i].Substring(1)] = args[i + 1];
                }
                else
                {
                    dic[args[i]] = args[i + 1];
                }

                i += 2;
            }

            return dic;

        }

    }
}
