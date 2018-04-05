using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace org.superkaka.kakalib.tools
{
    public class CommandMode
    {

        static public void exec(String[] args)
        {

            Hashtable param = parse(args);

            switch (param["command"].ToString().ToLower())
            {

                case "excel":

                    break;

            }

        }

        static public Hashtable parse(String[] args)
        {

            Hashtable tb = new Hashtable();
            int i = 0;
            while (i < args.Length)
            {
                String[] param = args[i].Split(new String[] { ":" }, StringSplitOptions.None);

                tb[param[0]] = param[1];

                i++;
            }
            return tb;

        }

    }
}
