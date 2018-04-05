using System;
using System.Collections.Generic;
using System.Linq;
using KLib;

namespace excelExport
{
    static class ProgramNew
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {

            CommandModeNew.exec(CommandParse.parse(args));

#if DEBUG
            return;
            SchoolVO.Fill(FileUtil.readFile(@"J:\school.kk"));

            var tb = new Table();
            tb.Fill(FileUtil.readFile(@"J:\lang.kk"));

            var list_row = tb.Rows;
            var list_result = (from row in list_row where row["id"].ToString() == "卡卡" select row).ToList();

            var langPack = new LanguagePack("zh_cn");
            langPack.Fill(tb);
            I18NTextManager.AddLang(langPack);
            var text = I18NTextManager.GetText("卡卡", new Dictionary<string, object>()
            {
                {"name","kaka's name"}
            });
#endif

        }
    }
}
