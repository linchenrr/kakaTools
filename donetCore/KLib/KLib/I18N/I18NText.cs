using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KLib
{
    public class I18NText : ParamText
    {

        public string textId;

        /**
         * 设置或获取此文本使用的指定语言id
         * 取文本时此值的优先级高于I18NTextManager的当前语言，如果设置了此值，则优先使用customLangId获取文本，如果没有获取到相应文本则使用I18NTextManager当前语言的默认值
         */
        public string customLangId;

        public I18NText(string textId = null, Dictionary<string, object> param = null, string customLangId = null)
            : base(null, param)
        {

            this.textId = textId;
            this.customLangId = customLangId;

        }

        /**
         * 获取此文本的求解字符串
         * 此方法会先从I18NTextManager获取textId对应的文本，再调用上级的toString方法完成参数替换再返回
         * @return
         */
        override public string ToString()
        {

            base.text = I18NTextManager.GetText(textId, customLangId, true);

            return base.ToString();

        }

    }
}
