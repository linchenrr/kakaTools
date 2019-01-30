using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace KLib
{
    public class WindowUtils
    {

        static private string exePath;

        static WindowUtils()
        {
            var dllPath = Assembly.GetEntryAssembly().Location;
            var fileInfo = new FileInfo(dllPath);
            exePath = Path.Combine(fileInfo.DirectoryName, "windowHelp.exe");
            Console.WriteLine($@"windowHelp path:{exePath}");
        }

        static private void startProcess(string args)
        {
            var pInfo = new ProcessStartInfo()
            {
                FileName = exePath,
                Arguments = args,
            };

            var process = new Process();
            process.StartInfo = pInfo;
            process.Start();
            process.WaitForExit();
        }

        static public void Alert(string message)
        {
            //Console.WriteLine(message);

            var bytes = Encoding.Default.GetBytes(message);
            //转成 Base64 形式的 String  
            var str = Convert.ToBase64String(bytes);
            startProcess("-content " + str);
        }

    }
}
