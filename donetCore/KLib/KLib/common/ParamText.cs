using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace KLib
{
    public class ParamText
    {

        /**
          * 用于搜索并替换文本参数的正则表达式
          */
        static private readonly Regex reg = new Regex("<%(.*?)%>");

        protected string text;

        public string Text
        {
            get { return text; }
            set { text = value == null ? "" : value; }
        }

        public Dictionary<string, object> paramDic;

        public ParamText(string text = null, Dictionary<string, object> paramDic = null)
        {
            this.Text = text;
            setParam(paramDic);
        }

        public void setParam(string key, object value)
        {
            paramDic[key] = value;
        }

        public void setParam(Dictionary<string, object> paramDic = null)
        {
            this.paramDic = paramDic == null ? new Dictionary<string, object>() : paramDic;
        }

        override public string ToString()
        {
            return reg.Replace(text, matchHandler);
        }

        private string matchHandler(Match match)
        {

            var key = match.Groups[1].Value;

            object value;
            if (paramDic.TryGetValue(key, out value))
                return value.ToString();
            return match.Groups[0].Value;

        }

    }
}