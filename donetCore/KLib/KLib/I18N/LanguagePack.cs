using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KLib
{
    public class LanguagePack : I18NLanguagePack
    {

        private string langId;

        public string LangId
        {
            get { return langId; }
            private set { langId = value; }
        }

        private Dictionary<string, string> dic_text = new Dictionary<string, string>();

        public LanguagePack(string langId)
        {
            this.LangId = langId;
        }

        /**
         * 获取文本内容
         * @param	textId			文本id
         * @return
         */
        public string GetText(string textId)
        {
            if (dic_text.ContainsKey(textId))
                return dic_text[textId];
            return "null";
        }

        /**
         * 添加文本
         * @param	textId			文本id
         * @param	text				文本内容
         */
        public void AddText(string textId, string text)
        {
            dic_text[textId] = text;
        }

    }
}
