using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KLib
{
    public delegate void DoLogHandler(string message);
    public class Logger
    {
        static public DoLogHandler LogHandler = logToConsole;
        static public void Log(params object[] messages)
        {
            doLog(joinString(messages));
        }

        static public void FormatLog(string format, params object[] args)
        {
            doLog(String.Format(format, args));
        }

        static private string joinString(object[] list_message)
        {
            if (list_message.Length == 0)
                return list_message.ToString();

            StringBuilder sb = new StringBuilder();
            int i = 0;
            int c = list_message.Length - 1;
            for (; i < c; i++)
            {
                if (null == list_message[i])
                    sb.Append("null" + ",");
                else
                    sb.Append(list_message[i].ToString() + ",");
            }
            if (null == list_message[i])
                sb.Append("null");
            else
                sb.Append(list_message[i].ToString());
            return sb.ToString();
        }

        static private void logToConsole(string message)
        {
            Console.WriteLine(message);
        }

        static private void doLog(string message)
        {
            if (LogHandler != null)
                LogHandler(message);
        }

    }
}
