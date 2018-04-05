using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace excelExport
{
    public class RegistryControl
    {
        static private RegistryKey regedit;
        static public void init()
        {
            regedit = Registry.CurrentUser.OpenSubKey(@"Software").OpenSubKey(@"kakaTools\excelExport", true);
            if (null == regedit)
            {
                regedit = Registry.CurrentUser.OpenSubKey(@"Software", true).CreateSubKey("kakaTools").CreateSubKey("excelExport");
                regedit.SetValue("outPutPath", "");
                regedit.SetValue("selectedPath", "");
                regedit.SetValue("ignoreColumn", "*");
                regedit.SetValue("ignoreLine", "#");
                regedit.SetValue("ignoreSheet", "#");
                regedit.SetValue("primaryKey", "$");
                regedit.SetValue("compress", "lzma");
                regedit.SetValue("ignore", true);
                regedit.SetValue("merge", false);

            }
        }

        static public String getSettings(String key)
        {

            return (String)regedit.GetValue(key);

        }

        static public void setSettings(String key, Object value)
        {

            regedit.SetValue(key, value);

        }

    }
}
