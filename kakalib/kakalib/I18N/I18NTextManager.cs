using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KLib
{
    public class I18NTextManager
    {

        static private readonly Dictionary<string, I18NLanguagePack> dic_langPack = new Dictionary<string, I18NLanguagePack>();

        static private string currentLangId;
        public static string CurrentLangId
        {
            get { return currentLangId; }
            set { currentLangId = value; }
        }

        /**
         * 注册语言包
         * @param	languagePack
         * @param	langId
         */
        static public void AddLang(I18NLanguagePack languagePack)
        {
            if (languagePack.LangId == null) throw new Exception("语言包没有指定有效的语言id");
            dic_langPack[languagePack.LangId] = languagePack;

            ///如果还没设置当前语言包则使用此语言包
            if (null == currentLangId)
                currentLangId = languagePack.LangId;
        }

        /**
         * 获取原始文本内容
         * @param	textId					文本id
         * @param	langId					语言id，如果省略此参数，则使用当前语言id
         * @param	compatibleMode		是否启用兼容模式，在兼容模式下此方法会先尝试根据传入的语言id获取文本，如果未取到则会使用当前语言id再次进行获取。默认值为false
         * @return
         */
        static public string GetText(string textId, string langId = null, bool compatibleMode = false)
        {

            var str = doGetText(textId, langId, compatibleMode);

            if (str == null)
            {
                //str = "缺少文本:{" + textId + "}";
                str = "{" + textId + "}";
            }

            return str;

        }

        /**
         * 查询语言包是否包含指定的文本
         * @param	textId					文本id
         * @param	langId					语言id，如果省略此参数，则使用当前语言id
         * @param	compatibleMode		是否启用兼容模式，在兼容模式下此方法会先尝试根据传入的语言id获取文本，如果未取到则会使用当前语言id再次进行获取。默认值为false
         * @return
         */
        static public bool HasText(string textId, string langId = null, bool compatibleMode = false)
        {

            return doGetText(textId, langId, compatibleMode) != null;

        }

        /**
         * 获取文本内容
         * @param	textId					文本id
         * @param	param					文本参数 (参数名/参数值对的Object对象)
         * @param	langId					语言id，如果省略此参数，则使用当前语言id
         * @return
         */
        static public string GetText(string textId = null, Dictionary<string, object> param = null, string langId = null)
        {

            return (new I18NText(textId, param, langId)).ToString();

        }

        static private string doGetText(string textId, string langId = null, bool compatibleMode = false)
        {

            if (langId == null)
            {
                langId = currentLangId;
            }

            var languagePack = dic_langPack[langId];

            //if (null == languagePack) throw new Error("语言包未设置！");
            if (null == languagePack) return null;

            var str = languagePack.GetText(textId);

            if (str == null)
            {
                if (compatibleMode)
                {
                    languagePack = dic_langPack[langId];

                    str = languagePack.GetText(textId);
                }

            }

            return str;

        }

    }
}
