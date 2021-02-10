using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XReminder
{

    public class RegKey
    {

        public const string Key_HideOnStartUp = "HideOnStartUp";
        public const string Key_LastCheckUpdateTime = "LastCheckUpdateTime";

        static public RegKey Instance { get; private set; }
        static public void Init(string path)
        {
            Instance = new RegKey(path);
        }

        private RegistryKey reg;

        public RegKey(string path)
        {
            reg = Registry.CurrentUser.OpenSubKey(path, true);
            if (reg == null)
            {
                reg = Registry.CurrentUser.CreateSubKey(path);
            }
        }

        public void SetString(string key, string value)
        {
            reg.SetValue(key, value, RegistryValueKind.String);
        }

        public string GetString(string key)
        {
            return reg.GetValue(key) as string;
        }

        public void SetBool(string key, bool value)
        {
            SetString(key, value ? "true" : "false");
        }

        public bool GetBool(string key, bool defaultValue = false)
        {
            var value = GetString(key);
            if (value == null)
                return defaultValue;
            return string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
        }

        public void SetInt(string key, int value)
        {
            SetString(key, value.ToString());
        }

        public int GetInt(string key, int defaultValue = 0)
        {
            var value = GetString(key);
            if (value == null)
                return defaultValue;
            return int.Parse(value);
        }

        public void SetDateTime(string key, DateTime value)
        {
            SetString(key, value.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        public DateTime? GetDateTime(string key)
        {
            var value = GetString(key);
            if (value == null)
                return null;
            return DateTime.Parse(value);
        }

    }
}
