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
            }
        }

        static public object GetValue(string key)
        {
            return regedit.GetValue(key);
        }

        static public void SetValue(string key, object value)
        {
            regedit.SetValue(key, value);
        }

    }
}
