using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

public class CommandParser
{

    static public CommandDictionary Parse(string[] args)
    {

        var dic = new CommandDictionary();
        int i = 0;
        while (i < args.Length - 1)
        {
            if (args[i].Substring(0, 1) == "-")
            {
                dic[args[i].Substring(1).ToLower()] = args[i + 1];
            }
            else
            {
                dic[args[i].ToLower()] = args[i + 1];
            }

            i += 2;
        }

        return dic;

    }

}

public class CommandDictionary : Dictionary<string, string>
{

    public string GetValue(string key, string defaultValue = null)
    {
        string value = null;
        if (TryGetValue(key.ToLower(), out value) == false)
            value = defaultValue;
        return value;
    }

}

