using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace KLib
{
    public class CodeTemplate
    {

        private const string mark_member = "$(member)";
        private const string mark_className = "$(className)";
        private const string mark_comment = "$(comment)";

        private string classExtension;

        public string ClassExtension
        {
            get { return classExtension; }
        }

        private string protocolEnumName;

        public string ProtocolEnumName
        {
            get { return protocolEnumName; }
        }

        private string template_singleFile;
        private string template_class;
        private string template_listDecode;
        private string template_listEncode;
        private string template_listAccess;
        private string template_definitionMember;
        private string template_definitionList;

        private string template_mapDecode;
        private string template_mapEncode;
        private string template_mapKeyAccess;
        private string template_mapValueAccess;
        private string template_definitionMap;

        private string template_enumClass;
        private string template_enumDefinition;

        private string template_messageRegisterClass;
        private string template_messageRegister;
        private string template_messageCreater;

        private string template_messageDispatcherItem;

        public XElement element_SingleProtocolFile;
        public XElement element_ProtocolEnumClass;
        public XElement element_MessageRegisterClass;

        private XElement xml_template;
        private Dictionary<string, ParamVO> dic_param = new Dictionary<string, ParamVO>();
        public CodeTemplate()
        {

        }

        public void load(XElement xml_template)
        {
            this.xml_template = xml_template;

            classExtension = getConfig("classExtension");
            if (classExtension.IndexOf(".") < 0)
                classExtension = "." + classExtension;

            protocolEnumName = getConfig("protocolEnumName");

            element_SingleProtocolFile = xml_template.Element("SingleProtocolFile");
            if (element_SingleProtocolFile != null)
                template_singleFile = element_SingleProtocolFile.Value;

            template_class = xml_template.Element("ProtocolVOClass").Value;
            template_class = Properties.Resources.logo_protocol + template_class;
            template_definitionMember = xml_template.Element("definitionMember").Value;
            template_definitionList = xml_template.Element("definitionList").Value;
            template_listDecode = xml_template.Element("decodeList").Value;
            template_listEncode = xml_template.Element("encodeList").Value;
            template_listAccess = xml_template.Element("accessList").Value.Trim();

            template_definitionMap = TryGetElementString("definitionMap");
            template_mapKeyAccess = TryGetElementString("accessMapKey", true);
            template_mapValueAccess = TryGetElementString("accessMapValue", true);
            template_mapDecode = TryGetElementString("decodeMap");
            template_mapEncode = TryGetElementString("encodeMap");


            element_ProtocolEnumClass = xml_template.Element("ProtocolEnumClass");
            template_enumClass = element_ProtocolEnumClass.Value;
            template_enumClass = Properties.Resources.logo_protocol + template_enumClass;
            template_enumDefinition = xml_template.Element("definitionEnum").Value;

            element_MessageRegisterClass = xml_template.Element("MessageCenterClass");
            template_messageRegisterClass = element_MessageRegisterClass.Value;
            template_messageRegisterClass = Properties.Resources.logo_protocol + template_messageRegisterClass;

            var element_MessageRegisterCreater = xml_template.Element("MessageRegisterCreater");
            if (element_MessageRegisterCreater == null)
                throw new Exception("缺少MessageRegisterCreater节点！");
            template_messageRegister = element_MessageRegisterCreater.Value;

            var element_MessageCreater = xml_template.Element("MessageCreateFun");
            if (element_MessageCreater != null)
                template_messageCreater = element_MessageCreater.Value;

            var element_messageDispatcher = xml_template.Element("MessageDispatcherItem");
            if (element_messageDispatcher != null)
                template_messageDispatcherItem = element_messageDispatcher.Value;

            var list_type = xml_template.Element("params").Elements("param");
            foreach (var item in list_type)
            {
                var paramVO = new ParamVO()
                {
                    paramType = item.Attribute("type").Value.Trim().ToLower(),
                    className = item.Attribute("class").Value.Trim(),
                    template_decode = item.Element("decode").Value,
                    template_encode = item.Element("encode").Value,
                };
                dic_param[paramVO.paramType] = paramVO;
            }

        }

        public void load(string templatePath)
        {
            load(XElement.Load(templatePath));
        }

        public string getConfig(string name)
        {
            var config = xml_template.Element("config");
            var item = config.Element(name);
            if (item != null)
                return item.Value;
            return null;
        }

        public string TryGetElementString(string name, bool trim = false)
        {
            var el = xml_template.Element(name);
            if (el == null)
                return null;

            if (trim)
                return el.Value.Trim();
            return el.Value;
        }

        public string getClassName(string paramType)
        {
            var paramVO = getParamVO(paramType);
            if (paramVO == null)
                return null;
            return paramVO.className;
        }

        public string getDecode(ProtocolMemberType memberType, string paramType, string member, string structClassName = "structClassName")
        {
            var paramVO = getParamVO(paramType, memberType);
            var result = paramVO.template_decode.Replace(mark_member, member);
            result = result.Replace(mark_className, structClassName);
            return result;
        }

        public string getListDecode(ProtocolMemberType memberType, string paramType, string member, string structClassName = "structClassName")
        {
            var arrayMember = template_listAccess.Replace(mark_member, member);
            var decode = getDecode(memberType, paramType, arrayMember, structClassName);
            var result = template_listDecode.Replace("$(decode)", decode.Trim());
            result = result.Replace(mark_member, member);
            result = result.Replace(mark_className, structClassName);
            return result;
        }

        public string getEncode(ProtocolMemberType memberType, string paramType, string member, string structClassName = "structClassName")
        {
            var paramVO = getParamVO(paramType, memberType);
            var result = paramVO.template_encode.Replace(mark_member, member);
            result = result.Replace(mark_className, structClassName);
            return result;
        }

        public string getListEncode(ProtocolMemberType memberType, string paramType, string member, string structClassName = "structClassName")
        {
            var arrayMember = template_listAccess.Replace(mark_member, member);
            var encode = getEncode(memberType, paramType, arrayMember, structClassName);
            var result = template_listEncode.Replace("$(encode)", encode.Trim());
            result = result.Replace(mark_member, member);
            result = result.Replace(mark_className, structClassName);
            return result;
        }

        public string getDefinition(string className, string member, string comment)
        {
            var result = template_definitionMember.Replace(mark_className, className);
            result = result.Replace(mark_member, member);
            result = result.Replace(mark_comment, comment);
            return result;
        }

        public string getArrayDefinition(string className, string member, string comment)
        {
            var result = template_definitionList.Replace(mark_className, className);
            result = result.Replace(mark_member, member);
            result = result.Replace(mark_comment, comment);
            return result;
        }

        public string getMapDefinition(string keyClassName, string valueClassName, string member, string comment)
        {
            var result = template_definitionMap.Replace("$(keyClassName)", keyClassName);
            result = result.Replace("$(valueClassName)", valueClassName);
            result = result.Replace(mark_member, member);
            result = result.Replace(mark_comment, comment);
            return result;
        }

        public string getMapDecode(string member, ProtocolMemberType memberType_key, string paramType_key, string structClassName_key, ProtocolMemberType memberType_value, string paramType_value, string structClassName_value)
        {
            var keyAccess = template_mapKeyAccess.Replace(mark_member, member);
            var keyDecode = getDecode(memberType_key, paramType_key, keyAccess, structClassName_key);

            var valueAccess = template_mapValueAccess.Replace(mark_member, member);
            //实际可以不需要，后面replace了
            //valueAccess = valueAccess.Replace("$(keyName)", keyAccess);
            var valueDecode = getDecode(memberType_value, paramType_value, valueAccess, structClassName_value);

            var result = template_mapDecode.Replace("$(decodeKey)", keyDecode.Trim());
            result = result.Replace("$(decodeValue)", valueDecode.Trim());

            result = result.Replace(mark_member, member);
            result = result.Replace("$(keyName)", keyAccess);
            result = result.Replace("$(keyClassName)", structClassName_key);
            result = result.Replace("$(valueClassName)", structClassName_value);

            return result;
        }

        public string getMapEncode(string member, ProtocolMemberType memberType_key, string paramType_key, string structClassName_key, ProtocolMemberType memberType_value, string paramType_value, string structClassName_value)
        {
            var keyAccess = template_mapKeyAccess.Replace(mark_member, member);
            var keyEncode = getEncode(memberType_key, paramType_key, keyAccess, structClassName_key);

            var valueAccess = template_mapValueAccess.Replace(mark_member, member);
            var valueEncode = getEncode(memberType_value, paramType_value, valueAccess, structClassName_value);


            var result = template_mapEncode.Replace("$(encodeKey)", keyEncode.Trim());
            result = result.Replace("$(encodeValue)", valueEncode.Trim());

            result = result.Replace(mark_member, member);
            result = result.Replace("$(keyName)", keyAccess);
            result = result.Replace("$(keyClassName)", structClassName_key);
            result = result.Replace("$(valueClassName)", structClassName_value);
            return result;
        }

        public string getProtocolVOText(string className, string definition, string messageName, string decode, string encode, string comment)
        {
            var text = template_class;
            text = text.Replace(mark_className, className);
            text = text.Replace("$(definition)", definition);
            text = text.Replace("$(messageName)", messageName);
            text = text.Replace("$(decode)", decode);
            text = text.Replace("$(encode)", encode);
            text = text.Replace(mark_comment, comment);
            text = text.Replace("\n", "\r\n");
            text = text.Replace("\r\r\n", "\r\n");
            return text;
        }

        public string getSingleFileText(string content)
        {
            var text = template_singleFile;
            text = text.Replace("$(content)", content);
            return text;
        }

        public string getFinalClassName(string className)
        {
            return className + getConfig("classNameTail");
        }

        private string firstCharToUp(string str)
        {
            var firstChar = str.Substring(0, 1);
            str = firstChar.ToUpper() + str.Substring(1);
            return str;
        }

        private ParamVO getParamVO(string paramType, ProtocolMemberType memberType = ProtocolMemberType.Normal)
        {
            paramType = paramType.ToLower();
            switch (memberType)
            {
                case ProtocolMemberType.Normal:
                    if (dic_param.ContainsKey(paramType) == false)
                        throw new Exception($"代码模板未定义的paramType:{paramType}");
                    return dic_param[paramType];
                case ProtocolMemberType.Struct:
                    return dic_param["struct"];
                case ProtocolMemberType.Enum:
                    return dic_param["enum"];
            }
            //throw new Exception("不支持的paramType:" + paramType);
            return null;
        }

        private class ParamVO
        {
            public string paramType;
            public string className;
            public string template_encode;
            public string template_decode;
        }


        public string getEnumDefinition(string member, string value, string comment)
        {
            var result = template_enumDefinition.Replace(mark_member, member);
            result = result.Replace("$(value)", value);
            result = result.Replace(mark_comment, comment);
            return result;
        }

        public string getProtocolEnumClass(string enumName, string content, string comment)
        {
            var text = template_enumClass;
            text = text.Replace("$(className)", enumName);
            text = text.Replace("$(content)", content);
            text = text.Replace("$(comment)", comment);
            text = text.Replace("\n", "\r\n");
            text = text.Replace("\r\r\n", "\r\n");
            return text;
        }

        public string getMessageRegister(string member, string className)
        {
            var result = template_messageRegister.Replace(mark_member, member);
            result = result.Replace(mark_className, className);
            return result;
        }

        public string getMessageCreater(string className)
        {
            if (template_messageCreater == null)
                return "";
            var result = template_messageCreater.Replace(mark_className, className);
            return result;
        }

        public string getMessageRegisterClass(string content, string creater, string dispatch)
        {
            var text = template_messageRegisterClass;
            text = text.Replace("$(content)", content);
            text = text.Replace("$(creater)", creater);
            text = text.Replace("$(dispatch)", dispatch);
            text = text.Replace("\n", "\r\n");
            text = text.Replace("\r\r\n", "\r\n");
            return text;
        }

        public string getMessageDispatcher(string messageName)
        {
            if (template_messageDispatcherItem == null)
                return "";
            var result = template_messageDispatcherItem.Replace("$(protocolEnumName)", protocolEnumName);
            result = result.Replace("$(messageName)", messageName);
            return result;
        }

    }
}
