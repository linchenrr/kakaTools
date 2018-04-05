using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace KLib
{
    public class INIConfig
    {
        private Dictionary<string, Dictionary<string, string>> dic_section = new Dictionary<string, Dictionary<string, string>>();

        public INIConfig()
        {

        }

        public INIConfig(string text)
        {
            parse(text);
        }

        public void createSection(string sectionName, bool coverOnExist = true)
        {
            if (!coverOnExist && dic_section.ContainsKey(sectionName))
                return;
            dic_section[sectionName] = new Dictionary<string, string>();
        }

        public bool hasSection(string sectionName)
        {
            return dic_section.ContainsKey(sectionName);
        }

        public Dictionary<string, string> getSection(string sectionName)
        {
            return new Dictionary<string, string>(tryGetSection(sectionName));
        }

        public List<string> getSectionNameList()
        {
            var list = new List<string>();
            foreach (var sectionName in dic_section.Keys)
            {
                list.Add(sectionName);
            }
            return list;
        }

        public void setValue(string sectionName, string key, string value)
        {
            var section = tryGetSection(sectionName);
            section[key] = value;
        }

        public string getValue(string sectionName, string key)
        {
            var section = tryGetSection(sectionName);
            return section[key];
        }

        private Dictionary<string, string> tryGetSection(string sectionName)
        {
            Dictionary<string, string> section;
            if (dic_section.TryGetValue(sectionName, out section))
                return section;

            throw new Exception(String.Format("section \"{0}\" does not exist:", sectionName));
        }

        public void parse(string text)
        {

            //先去除注释
            text = reg_comment.Replace(text, "");

            var list_match = reg_section.Matches(text);
            for (int i = 0; i < list_match.Count; i++)
            {
                var match = list_match[i];
                var sectionName = match.Groups[1].Value;
                createSection(sectionName);

                var list_keyValueMatch = reg_keyValue.Matches(match.Groups[2].Value);
                for (int j = 0; j < list_keyValueMatch.Count; j++)
                {
                    var keyValueResult = list_keyValueMatch[j];
                    if (keyValueResult.Groups.Count < 3)
                    {
                        break;
                    }

                    dic_section[sectionName][keyValueResult.Groups[1].Value.Trim()] = keyValueResult.Groups[2].Value.Trim();
                }

            }

        }

        override public string ToString()
        {
            var sb = new StringBuilder();
            foreach (var section in dic_section)
            {
                sb.AppendLine("[" + section.Key + "]");
                foreach (var prop in section.Value)
                {
                    sb.Append(prop.Key);
                    sb.Append("=");
                    sb.AppendLine(prop.Value);
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        /**
        * (?=pattern)  神器
        * 执行正向预测先行搜索的子表达式，该表达式匹配处于匹配 pattern 的字符串的起始点的字符串。它是一个非捕获匹配，即不能捕获供以后使用的匹配。例如“Windows (?=95|98|NT|2000)”匹配“Windows 2000”中的“Windows”，而不匹配“Windows 3.1”中的“Windows”。预测先行不占用字符，即发生匹配后，下一匹配的搜索紧随上一匹配之后，而不是在组成预测先行的字符后。
        */
        static private readonly Regex reg_section = new Regex(@"\[(.*)\]([\s\S]*?)((?=\[.*\])|$)");
        static private readonly Regex reg_keyValue = new Regex(@"(.+?)=(.+)");
        static private readonly Regex reg_comment = new Regex(@"^[ \f\t\v]*(#|\/\/|;).*", RegexOptions.Multiline);

    }
}