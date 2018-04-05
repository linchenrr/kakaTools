using System;
using System.Collections.Generic;
using System.Windows.Forms;
using KLib;

namespace excelExport
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {

            RegistryControl.init();

            if (args.Length == 0)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form_ExcelExport());
            }
            else
            {
                CommandMode.exec(CommandParse.parse(args));
            }
        }
    }
}
