using KLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace KLib
{
    static public class LanguagePackExtension
    {

        static public void Fill(this I18NLanguagePack langPack, XElement source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            var list_text = source.Elements("text");
            foreach (var item in list_text)
            {
                langPack.AddText(item.Attribute("id").Value, item.Value);
            }
        }

        static public void Fill(this I18NLanguagePack langPack, Table source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            var table = source;

            foreach (var textItem in table)
            {
                langPack.AddText(textItem["id"].ToString(), textItem["text"].ToString());
            }

        }

    }
}
