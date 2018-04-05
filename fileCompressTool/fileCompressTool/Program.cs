using KLib;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace fileCompressTool
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
            else
            {
                CommandMode.exec(CommandParse.parse(args));
            }

        }
    }
}
