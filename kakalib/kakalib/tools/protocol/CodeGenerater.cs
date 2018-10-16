using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using KLib;

namespace KLib
{
    public class CodeGenerater
    {
        private CodeTemplate codeTemplate;
        private Dictionary<string, bool> dic_name = new Dictionary<string, bool>();
        private Dictionary<int, ProtocolStruct> dic_message = new Dictionary<int, ProtocolStruct>();
        private Dictionary<string, ProtocolStruct> dic_struct = new Dictionary<string, ProtocolStruct>();
        private Dictionary<string, ProtocolEnum> dic_enum = new Dictionary<string, ProtocolEnum>();

        public string readComment(XElement item)
        {
            var attribute_Comment = item.Attribute("comment");
            if (attribute_Comment != null)
                return attribute_Comment.Value;
            return "";
        }

        private string codeFolderPath;
        private string voFolderPath;
        private bool singleMode;
        private string voFolder;

        private Regex reg_message = new Regex(@"\s*([^}]*)message\s+(\S+)\s+(\d+)\s*{([^}]*)}", RegexOptions.IgnoreCase);
        private Regex reg_struct = new Regex(@"\s*([^}]*)struct\s+(\S+)\s*{([^}]*)}", RegexOptions.IgnoreCase);
        private Regex reg_enum = new Regex(@"\s*([^}]*)enum\s+(\S+)\s*{([^}]*)}", RegexOptions.IgnoreCase);
        private Regex reg_member = new Regex(@"(\S+)\s+(\S+)([^$]*?)$", RegexOptions.Multiline);
        private Regex reg_memberListCheck = new Regex(@"List<\s*(\S+)\s*>", RegexOptions.IgnoreCase);
        private Regex reg_memberMapCheck = new Regex(@"map<\s*(\S+)\s*,\s*(\S+)\s*>", RegexOptions.IgnoreCase);
        private Regex reg_enumMember = new Regex(@"(\S+)\s*=\s*(\d+)([^$]*?)$", RegexOptions.Multiline);


        public void generate(string protocolFolderPath, string templateXmlPath, string codeFolderPath)
        {

            var template = XElement.Load(templateXmlPath);
            codeTemplate = new CodeTemplate();
            codeTemplate.load(template);

            singleMode = codeTemplate.getConfig("singleMode") == "true";

            voFolder = codeTemplate.getConfig("voFolder");

            codeFolderPath += "/";
            this.codeFolderPath = codeFolderPath;

            if (Directory.Exists(codeFolderPath))
            {
                var files = Directory.GetFiles(codeFolderPath, "*" + codeTemplate.ClassExtension);
                foreach (var file in files)
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("删除文件失败:" + file);
                        throw e;
                    }
                }
                //try
                //{
                //    Directory.Delete(codeFolderPath, true);
                //}
                //catch (Exception e)
                //{
                //    Console.WriteLine("删除目录失败，重试..");
                //    Thread.Sleep(50);
                //    Directory.Delete(codeFolderPath, true);
                //}
            }
            else
            {
                Directory.CreateDirectory(codeFolderPath);
            }

            Thread.Sleep(5);

            if (!singleMode)
            {
                if (String.IsNullOrEmpty(voFolder))
                    this.voFolderPath = this.codeFolderPath;
                else
                {
                    this.voFolderPath = codeFolderPath + voFolder + "/";
                    if (Directory.Exists(voFolderPath))
                        Directory.Delete(voFolderPath, true);

                    Directory.CreateDirectory(voFolderPath);
                }
            }


            var text = "";

            var list_ptlFile = Directory.GetFiles(protocolFolderPath, "*.ptl");
            foreach (var filePath in list_ptlFile)
            {
                text += File.ReadAllText(filePath, Encoding.UTF8);
                text += @"
";
            }
            text = clearComment(text);

            var match_structs = reg_struct.Matches(text);
            for (int m = 0; m < match_structs.Count; m++)
            {
                var match_struct = match_structs[m];
                var protoStruct = new ProtocolStruct()
                {
                    isMessage = false,
                    comment = cleanComment(match_struct.Groups[1].Value),
                    type = match_struct.Groups[2].Value,
                };
                protoStruct.memberList = parseMembers(match_struct.Groups[3].Value);

                if (dic_struct.ContainsKey(protoStruct.type))
                    throw new Exception("重复的struct定义:" + protoStruct.type);
                dic_struct.Add(protoStruct.type, protoStruct);

                // text += readParams(item);
            }

            match_structs = reg_message.Matches(text);
            for (int m = 0; m < match_structs.Count; m++)
            {
                var match_struct = match_structs[m];
                var protoStruct = new ProtocolStruct()
                {
                    isMessage = true,
                    comment = cleanComment(match_struct.Groups[1].Value),
                    type = match_struct.Groups[2].Value,
                };
                protoStruct.memberList = parseMembers(match_struct.Groups[4].Value);

                var idStr = match_struct.Groups[3].Value;
                try
                {
                    protoStruct.id = Convert.ToInt32(idStr);
                }
                catch
                {
                    throw new Exception(String.Format("转换消息id:{0} 失败", idStr));
                }

                if (dic_message.ContainsKey(protoStruct.id))
                    throw new Exception("重复的消息号定义:" + protoStruct.id);
                dic_message.Add(protoStruct.id, protoStruct);

                if (dic_struct.ContainsKey(protoStruct.type))
                    throw new Exception("重复的struct定义:" + protoStruct.type);
                dic_struct.Add(protoStruct.type, protoStruct);

                // text += readParams(item);
            }

            match_structs = reg_enum.Matches(text);
            for (int m = 0; m < match_structs.Count; m++)
            {
                var match_struct = match_structs[m];
                var protoStruct = new ProtocolEnum()
                {
                    comment = cleanComment(match_struct.Groups[1].Value),
                    type = match_struct.Groups[2].Value,
                };
                protoStruct.memberList = parseEnumMembers(match_struct.Groups[3].Value);

                if (dic_enum.ContainsKey(protoStruct.type))
                    throw new Exception("重复的enum定义:" + protoStruct.type);
                dic_enum.Add(protoStruct.type, protoStruct);

                // text += readParams(item);
            }

            var singleText = "";
            foreach (var item in dic_struct.Values)
            {
                singleText += generaterStruct(item);
            }

            foreach (var item in dic_enum.Values)
            {
                singleText += generaterEnum(item);
            }

            //Console.ReadLine();

            if (singleMode)
                File.WriteAllBytes(codeFolderPath + codeTemplate.element_SingleProtocolFile.Attribute("fileName").Value, Encoding.UTF8.GetBytes(codeTemplate.getSingleFileText(singleText)));


            var list_message = dic_message.Values.ToList();
            list_message = (from item in list_message orderby item.id select item).ToList();
            var str_enum = "";
            var str_registerMessage = "";

            str_enum += codeTemplate.getEnumDefinition("None", "0", "");

            for (int i = 0; i < list_message.Count; i++)
            {
                var msg = list_message[i];
                str_enum += codeTemplate.getEnumDefinition(msg.type, msg.id.ToString(), msg.comment);

                str_registerMessage += codeTemplate.getMessageRegister(msg.type, msg.type);
            }
            str_enum = codeTemplate.getProtocolEnumClass(codeTemplate.ProtocolEnumName, str_enum, "");
            str_registerMessage = codeTemplate.getMessageRegisterClass(str_registerMessage);

            File.WriteAllBytes(codeFolderPath + codeTemplate.ProtocolEnumName + codeTemplate.ClassExtension, Encoding.UTF8.GetBytes(str_enum));
            File.WriteAllBytes(codeFolderPath + codeTemplate.element_MessageRegisterClass.Attribute("fileName").Value, Encoding.UTF8.GetBytes(str_registerMessage));


            var copyFiles = template.Element("CopyFiles");
            if (copyFiles != null)
            {
                var list_copy = copyFiles.Elements("CopyFile");
                foreach (var item in list_copy)
                {
                    var fileName = item.Attribute("fileName").Value.Trim();
                    var content = item.Value;
                    File.WriteAllBytes(codeFolderPath + fileName, Encoding.UTF8.GetBytes(content));
                }
            }

            var templateFileInfo = new FileInfo(templateXmlPath);
            var copyDirInfo = new DirectoryInfo(templateFileInfo.Directory + "/CopyFiles");
            if (copyDirInfo.Exists)
                FileUtil.copyFolder(copyDirInfo.FullName, codeFolderPath);


            Console.WriteLine("已生成代码至{0}", codeFolderPath);

        }

        private List<ProtocolMember> parseMembers(string content)
        {
            var list = new List<ProtocolMember>();
            var match_members = reg_member.Matches(content);
            for (int i = 0; i < match_members.Count; i++)
            {
                var match_member = match_members[i];
                var type = match_member.Groups[1].Value;
                var type2 = "";
                var dataStructureType = ProtocolMemberDataStructureType.Normal;
                var match_list = reg_memberListCheck.Match(type);

                if (match_list.Groups.Count > 1)
                {
                    type = match_list.Groups[1].Value;
                    dataStructureType = ProtocolMemberDataStructureType.List;
                }
                else
                {
                    match_list = reg_memberMapCheck.Match(type);
                    if (match_list.Groups.Count > 1)
                    {
                        type = match_list.Groups[1].Value;
                        type2 = match_list.Groups[2].Value;
                        dataStructureType = ProtocolMemberDataStructureType.Map;
                    }
                }

                var protoMember = new ProtocolMember()
                {
                    DataStructureType = dataStructureType,
                    type = type,
                    type2 = type2,
                    name = match_member.Groups[2].Value.TrimEnd(';'),
                    comment = cleanComment(match_member.Groups[3].Value),
                };
                list.Add(protoMember);
            }
            return list;
        }

        private List<ProtocolEnumMember> parseEnumMembers(string content)
        {
            var list = new List<ProtocolEnumMember>();
            var match_members = reg_enumMember.Matches(content);
            for (int i = 0; i < match_members.Count; i++)
            {
                var match_member = match_members[i];
                var protoMember = new ProtocolEnumMember()
                {
                    name = match_member.Groups[1].Value,
                    //value = Convert.ToInt32(match_member.Groups[2].Value),
                    comment = cleanComment(match_member.Groups[3].Value),
                };
                var valueStr = match_member.Groups[2].Value.TrimEnd(';');
                try
                {
                    protoMember.value = Convert.ToInt32(valueStr);
                }
                catch
                {
                    throw new Exception(String.Format("转换枚举 {0} 的值 {1} 失败", protoMember.name, valueStr));
                }
                list.Add(protoMember);
            }
            return list;
        }

        private string cleanComment(string comment)
        {
            comment = comment.Trim();
            return comment;
        }

        private string clearComment(string content)
        {
            var reg = new Regex(@"\/{2,}.*\n");
            content = reg.Replace(content, @"
");
            reg = new Regex(@"(\/\*+[\s\S]*?\*\/)");
            content = reg.Replace(content, replaceCommentHandler);
            return content;
        }

        private string replaceCommentHandler(Match match)
        {

            var content = match.Groups[1].Value;

            if (content.IndexOf(@"
") >= 0)
            {
                return @"
";
            }
            return " ";

        }

        private ProtocolMemberType GetMenberType(string type)
        {
            var menberType = ProtocolMemberType.Normal;
            if (dic_struct.ContainsKey(type))
                menberType = ProtocolMemberType.Struct;
            if (dic_enum.ContainsKey(type))
                menberType = ProtocolMemberType.Enum;
            return menberType;
        }

        private string getClassName(string type, ProtocolMemberType memberType)
        {
            var className = type;
            if (memberType == ProtocolMemberType.Normal)
                className = codeTemplate.getClassName(type);
            return className;
        }

        public string generaterStruct(ProtocolStruct protoStruct)
        {
            if (dic_name.ContainsKey(protoStruct.type))
            {
                throw new Exception(String.Format("重复的类名:{0}", protoStruct.type));
            }

            dic_name.Add(protoStruct.type, true);

            string text = "";
            string singleVOText = "";

            var isMessage = protoStruct.isMessage;

            var pathName = protoStruct.type;

            string str_definition = "";
            string str_encode = "";
            string str_decode = "";

            var list_param = protoStruct.memberList;
            for (int i = 0; i < list_param.Count; i++)
            {
                var param = list_param[i];
                var memberName = param.name;
                var type = param.type;

                var menberType = GetMenberType(type);
                var className = getClassName(type, menberType);

                switch (param.DataStructureType)
                {
                    case ProtocolMemberDataStructureType.Normal:
                        str_decode += codeTemplate.getDecode(menberType, type, memberName, className);
                        str_encode += codeTemplate.getEncode(menberType, type, memberName, className);
                        str_definition += codeTemplate.getDefinition(className, memberName, param.comment);
                        break;

                    case ProtocolMemberDataStructureType.List:
                        str_decode += codeTemplate.getListDecode(menberType, type, memberName, className);
                        str_encode += codeTemplate.getListEncode(menberType, type, memberName, className);
                        str_definition += codeTemplate.getArrayDefinition(className, memberName, param.comment);
                        break;

                    case ProtocolMemberDataStructureType.Map:
                        var type2 = param.type2;
                        var menberType2 = GetMenberType(type2);
                        var className2 = getClassName(type2, menberType2);
                        str_decode += codeTemplate.getMapDecode(memberName, menberType, type, className, menberType2,
                            type2, className2);
                        str_encode += codeTemplate.getMapEncode(memberName, menberType, type, className, menberType2,
                         type2, className2);
                        str_definition += codeTemplate.getMapDefinition(className, className2, memberName, param.comment);
                        break;
                }

            }

            var fileClassName = codeTemplate.getFinalClassName(pathName);

            var typeName = protoStruct.type;
            if (protoStruct.isMessage == false)
                typeName = "None";
            singleVOText = codeTemplate.getProtocolVOText(fileClassName, str_definition, typeName, str_decode, str_encode, protoStruct.comment);

            //if (isMessage)
            //    singleVOText = codeTemplate.getProtocolVOText(fileClassName, str_definition, protoStruct.type, str_decode, str_encode, protoStruct.comment);
            //else
            //    singleVOText = codeTemplate.getStructText(fileClassName, str_definition, str_decode, str_encode, protoStruct.comment);

            if (singleMode)
                text += singleVOText;
            else
                File.WriteAllBytes(voFolderPath + fileClassName + codeTemplate.ClassExtension, Encoding.UTF8.GetBytes(singleVOText));

            return text;
        }

        public string generaterEnum(ProtocolEnum protoEnum)
        {
            if (dic_name.ContainsKey(protoEnum.type))
            {
                throw new Exception(String.Format("重复的类名:{0}", protoEnum.type));
            }

            dic_name.Add(protoEnum.type, true);

            string text = "";

            string singleVOText = "";

            var pathName = protoEnum.type;

            string str = "";

            var list_param = protoEnum.memberList;
            for (int i = 0; i < list_param.Count; i++)
            {
                var param = list_param[i];
                var memberName = param.name;

                str += codeTemplate.getEnumDefinition(param.name, param.value.ToString(), param.comment);

            }

            var fileClassName = codeTemplate.getFinalClassName(pathName);

            var typeName = protoEnum.type;
            singleVOText = codeTemplate.getProtocolEnumClass(typeName, str, protoEnum.comment);

            if (singleMode)
                text += singleVOText;
            else
                File.WriteAllBytes(voFolderPath + fileClassName + codeTemplate.ClassExtension, Encoding.UTF8.GetBytes(singleVOText));


            return text;
        }

    }

    public class ProtocolStruct
    {
        public int id;
        public bool isMessage;
        public string type;
        public string comment;
        public List<ProtocolMember> memberList = new List<ProtocolMember>();
    }

    public class ProtocolMember
    {
        public string type;
        //for map
        public string type2;
        public ProtocolMemberDataStructureType DataStructureType;
        public string name;
        public string comment;
    }

    public enum ProtocolMemberDataStructureType
    {
        Normal = 0, List, Map,
    }

    public class ProtocolEnum
    {
        public string type;
        public string comment;
        public List<ProtocolEnumMember> memberList = new List<ProtocolEnumMember>();
    }

    public class ProtocolEnumMember
    {
        public string name;
        public int value;
        public string comment;
    }

    public enum ProtocolMemberType
    {
        Normal,
        Struct,
        Enum,
    }

}
