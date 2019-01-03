using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data.OleDb;
using KLib;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.Threading;

namespace KLib
{
    public class ExcelGenerater
    {

        static public string templatePath;
        static public Endian endian;
        static public string fileExt = ".kk";
        static public string defaultArraySpliter = "-";
        static public int commentRowNum = 1;
        static public int fieldNameRowNum = 2;
        static public int typeRowNum = 3;
        static public int dataRowStartNum = 4;

        static public bool exportDataBytes = true;
        static public bool exportDatajson = true;
        static public bool mergeSheets = false;
        static public bool writeCellLen = true;
        static public string[] writeCellLenExclude = new string[0];
        static public string exclude;

        static public string customerEncoder;
        static private ExcelTable.ExcelEncoder encoder;

        static public string curExcel;
        static public string curSheet;

        static private List<Action> flushCallbacks = new List<Action>();

        static public bool IsInvalid
        {
            get
            {
                return KLibInvalid.IsInvalid;
            }
        }

        static public void export(String inputPath, String outputPath, CompressOption op, String prefix_primaryKey, String prefix_IgnoreSheet, Boolean ignoreBlank)
        {

            if (Directory.Exists(inputPath))
            {
                DirectoryInfo di = new DirectoryInfo(inputPath);

                FileInfo[] fileInfos = di.GetFiles("*.xlsx", SearchOption.AllDirectories);

                fileInfos = fileInfos.Where(fileInfo =>
                {
                    //过滤隐藏文件
                    if ((fileInfo.Attributes & FileAttributes.Hidden) != 0)
                        return false;
                    //过滤excel临时文件
                    if (fileInfo.Name.StartsWith("~$"))
                        return false;
                    if (string.IsNullOrEmpty(exclude) == false && fileInfo.FullName.ToLower().Contains(exclude))
                        return false;
                    return true;
                }).ToArray();

                fileInfos = fileInfos.OrderBy(info => info.Name).ToArray();

                for (int k = 0; k < fileInfos.Length; k++)
                {
                    export(fileInfos[k].FullName, outputPath, op, prefix_primaryKey, prefix_IgnoreSheet, ignoreBlank);
                }
            }
            else
            {
                curExcel = Path.GetFileName(inputPath);
                ExcelTable[] sheets = doExport(inputPath, prefix_primaryKey, prefix_IgnoreSheet, ignoreBlank);

                if (sheets == null || sheets.Length == 0)
                    return;

                foreach (ExcelTable sheet in sheets)
                {
                    curSheet = sheet.name;

                    if (exportDataBytes)
                    {
                        Console.WriteLine($@"为{curSheet}表生成数据文件");

                        var path = outputPath + sheet.name + fileExt;
                        sheet.SetEncoder(encoder);
                        var bytes = sheet.ToBytes(endian);

                        flushCallbacks.Add(() =>
                        {
                            var inStream = new MemoryStream(bytes);
                            var outStream = new MemoryStream();

                            ICompresser compresser;

                            switch (op)
                            {

                                case CompressOption.lzma:
                                    compresser = new LZMACompresser();
                                    compresser.compress(inStream, outStream);
                                    break;

                                case CompressOption.zlib:
                                    compresser = new ZlibCompresser();
                                    compresser.compress(inStream, outStream);
                                    break;

                                case CompressOption.gzip:
                                    compresser = new GZipCompresser();
                                    compresser.compress(inStream, outStream);
                                    break;

                                case CompressOption.none:
                                    outStream = inStream;
                                    break;

                                default:
                                    throw new Exception();

                            }

                            Console.WriteLine($@"写入{path}");

                            FileStream fs = File.Create(path);
                            outStream.WriteTo(fs);
                            fs.Close();

                            outStream.Dispose();
                        });
                    }

                    if (exportDatajson)
                    {
                        var jsonPath = outputPath + sheet.name + ".json";
                        var jsonBytes = sheet.ToJson();

                        flushCallbacks.Add(() =>
                        {
                            Console.WriteLine($@"写入{jsonPath}");
                            File.WriteAllBytes(jsonPath, jsonBytes);
                        });
                    }

                }

                curSheet = null;

            }
        }

        static public void startExport(String inputPath, String outputPath, CompressOption op, String prefix_primaryKey, String prefix_IgnoreSheet, Boolean ignoreBlank)
        {
            //customerEncoder = @"C:\work\unity\project\demo\demo\demo\codes\client\tools\excelEncoder\excelEncoder\bin\Release\excelEncoder.dll";

            flushCallbacks.Clear();

            Assembly ass = null;
            if (!string.IsNullOrEmpty(customerEncoder))
            {
                Console.WriteLine($@"加载自定义编码器
{customerEncoder}
");
                try
                {
                    ass = Assembly.LoadFile(Path.Combine(Environment.CurrentDirectory, customerEncoder));
                }
                catch (Exception e)
                {
                    throw new Exception(@"加载自定义编码器失败
" + e.Message);
                }
            }

            if (ass != null)
            {
                try
                {
                    var t = ass.GetType("ExcelEncoder");
                    var methodInfo = t.GetMethod("Encode", BindingFlags.Static | BindingFlags.Public);
                    encoder = (ExcelTable.ExcelEncoder)
                        Delegate.CreateDelegate(typeof(ExcelTable.ExcelEncoder), null, methodInfo, true);
                }
                catch (Exception e)
                {
                    throw new Exception(@"查找符合的自定义编码器失败
" + e.Message);
                }
                /*
                Console.WriteLine(@"加载自定义编码器成功
");
*/
            }

            if (null == outputPath || "" == outputPath)
            {
                FileInfo fi = new FileInfo(inputPath);
                outputPath = fi.DirectoryName;
            }

            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            if (exportDataBytes)
            {
                var deleteFiles = Directory.GetFiles(outputPath, "*" + fileExt, SearchOption.TopDirectoryOnly);
                flushCallbacks.Add(() =>
                {
                    foreach (var deleteFilePath in deleteFiles)
                    {
                        File.Delete(deleteFilePath);
                    }
                });
            }

            if (templatePath != null)
            {
                codeTemplate = new ExcelCodeTemplate();
                codeTemplate.load(templatePath);

                if (codeFolderPath != null && !Directory.Exists(codeFolderPath))
                    Directory.CreateDirectory(codeFolderPath);

                var codeFiles = Directory.GetFiles(codeFolderPath, "*" + codeTemplate.ClassExtension, SearchOption.TopDirectoryOnly);
                flushCallbacks.Add(() =>
                {
                    foreach (var codeFilePath in codeFiles)
                    {
                        File.Delete(codeFilePath);
                    }
                });
            }


            commentRowNum--;
            fieldNameRowNum--;
            typeRowNum--;
            dataRowStartNum--;


            export(inputPath, outputPath, op, prefix_primaryKey, prefix_IgnoreSheet, ignoreBlank);

            Console.WriteLine();

            foreach (var action in flushCallbacks)
            {
                action();
            }

            if (codeTemplate != null)
            {
                Console.WriteLine();
                Console.WriteLine("已生成代码至");
                Console.WriteLine(codeFolderPath);
            }
        }

        static private ExcelCodeTemplate codeTemplate;

        static ExcelTable[] doExport(String path, String prefix_primaryKey, String prefix_IgnoreSheet, Boolean ignoreBlank)
        {
            var sheets = new List<ExcelTable>();
            var excelTables = getTables(path, prefix_IgnoreSheet);
            if (excelTables.Count == 0)
                return null;

            if (mergeSheets)
            {
                var mainTable = excelTables[0];
                for (int i = 1; i < excelTables.Count; i++)
                {
                    var subTable = excelTables[i];
                    ///去掉表头
                    for (int j = 0; j < dataRowStartNum; j++)
                    {
                        subTable.Rows.RemoveAt(0);
                    }
                    mainTable.Merge(subTable);
                }

                var excelSheet = processSheet(mainTable, prefix_primaryKey, ignoreBlank);
                excelSheet.name = Path.GetFileNameWithoutExtension(path).ToLower();
                if (excelSheet != null)
                {
                    sheets.Add(excelSheet);
                }
            }
            else
            {
                foreach (var table in excelTables)
                {
#if !DEBUG
                    try
#endif
                    {
                        var excelSheet = processSheet(table, prefix_primaryKey, ignoreBlank);
                        if (excelSheet != null)
                        {
                            sheets.Add(excelSheet);
                        }
                    }
#if !DEBUG
                    catch (Exception e)
                    {
                        throw new Exception($@"处理表{table.TableName}失败
" + e.Message);
                    }
#endif
                }
            }


            ExcelTable[] tables = sheets.ToArray();


            if (codeTemplate != null)
            {
                var reg_enter = new Regex(@"[\r\n]");
                foreach (var sheet in tables)
                {
                    string str_definition = "";
                    string str_decode = "";
                    var fileClassName = codeTemplate.getFinalClassName(sheet.name);
                    for (int i = 0; i < sheet.types.Length; i++)
                    {
                        var memberName = /*sheet.header[i] =*/ codeTemplate.getFinalMemberName(sheet.header[i]);
                        var relationName = sheet.relations[i];
                        var type = sheet.types[i];
                        var enumName = sheet.enumNames[i];
                        var isArray = sheet.isArray[i];
                        var comment = reg_enter.Replace(sheet.comments[i], " ");
                        var className = codeTemplate.getTypeClassName(type);
                        if (enumName != null)
                            className = enumName;

                        if (!string.IsNullOrEmpty(isArray))
                        {
                            str_decode += codeTemplate.getDecode(type, memberName, true, enumName);
                            str_definition += codeTemplate.getArrayDefinition(className, memberName, comment);
                        }
                        else
                        {
                            str_decode += codeTemplate.getDecode(type, memberName, false, enumName);
                            str_definition += codeTemplate.getDefinition(className, memberName, comment);
                            if (!string.IsNullOrEmpty(relationName))
                            {
                                str_definition += codeTemplate.getRelationMember(codeTemplate.getFinalClassName(relationName), memberName);
                            }
                        }
                    }

                    var classText = codeTemplate.getClassText(sheet.name, fileClassName, str_definition, str_decode, sheet.types[sheet.primaryKeyIndex], codeTemplate.getFinalMemberName(sheet.header[sheet.primaryKeyIndex]));

                    flushCallbacks.Add(() =>
                    {
                        File.WriteAllBytes(codeFolderPath + fileClassName + codeTemplate.ClassExtension, Encoding.UTF8.GetBytes(classText));
                    });

                    ExcelCodeTemplate.AddClassName(fileClassName);

                }

                if (codeTemplate.HasInitClass)
                {
                    flushCallbacks.Add(() =>
                    {
                        File.WriteAllBytes(codeFolderPath + codeTemplate.GetInitClassFileName(), Encoding.UTF8.GetBytes(codeTemplate.GetInitClassText()));
                    });
                }

            }

            return tables;

        }

        static public string codeFolderPath;


        static private ExcelTable processSheet(DataTable dt, String prefix_primaryKey, Boolean ignoreBlank)
        {
            //字段名+类型  至少2行
            if (dt.Rows.Count < 3 && dt.Columns.Count == 1)
            {
                return null;
            }

            var sheetName = dt.TableName;

            Object[] header = new Object[dt.Rows[0].ItemArray.Length];
            dt.Rows[0].ItemArray.CopyTo(header, 0);

            if (ignoreBlank)
            {
                ///消除空白行
                int maxColumn = header.Length;
                int row = dt.Rows.Count - 1;
                while (row >= dataRowStartNum)
                {
                    Boolean hasData = false;
                    DataRow dataRow = dt.Rows[row];
                    int column = 0;
                    while (column < maxColumn)
                    {
                        if (dataRow[column].ToString().Trim() != "")
                        {
                            hasData = true;
                            break;
                        }
                        column++;
                    }

                    if (!hasData)
                    {
                        dt.Rows.RemoveAt(row);
                    }

                    row--;
                }
            }

            int i;

            header = new Object[dt.Rows[fieldNameRowNum].ItemArray.Length];
            dt.Rows[fieldNameRowNum].ItemArray.CopyTo(header, 0);

            var row_types = new Object[dt.Rows[typeRowNum].ItemArray.Length];
            dt.Rows[typeRowNum].ItemArray.CopyTo(row_types, 0);

            //if (ignoreBlank)
            {
                ///消除未定义表头列(字段名、字段类型)
                int column = header.Length - 1;
                while (column >= 0)
                {
                    if (header[column].ToString().Trim() == "" || row_types[column].ToString().Trim() == "")
                    {
                        dt.Columns.RemoveAt(column);
                    }
                    column--;
                }

                if (dt.Columns.Count <= 0)
                    return null;

                header = new String[dt.Rows[fieldNameRowNum].ItemArray.Length];
                dt.Rows[fieldNameRowNum].ItemArray.CopyTo(header, 0);

            }

            //关联字段解析
            var reg_relation = new Regex(@"\[(\w+)\]$");
            var relations = new string[header.Length];

            for (int i_header = 0; i_header < header.Length; i_header++)
            {
                var str_header = header[i_header].ToString().Trim();
                //字段名先不换成大写开头
                //str_header = firstCharToUp(str_header);

                Match m_relation = reg_relation.Match(str_header);
                if (m_relation.Groups.Count > 1)
                {
                    str_header = reg_relation.Replace(str_header, "");
                    relations[i_header] = firstCharToUp(m_relation.Groups[1].Value);
                }
                header[i_header] = str_header;
            }

            var comments = new String[dt.Rows[commentRowNum].ItemArray.Length];
            for (int i_comments = 0; i_comments < comments.Length; i_comments++)
            {
                comments[i_comments] = dt.Rows[commentRowNum].ItemArray[i_comments].ToString();
            }

            //数组相关解析
            var reg_array = new Regex(@"\[(.*)\]$");
            var types = new String[dt.Rows[typeRowNum].ItemArray.Length];
            var isArray = new String[types.Length];
            var enumNames = new String[types.Length];

            dt.Rows[typeRowNum].ItemArray.CopyTo(types, 0);
            for (int i_types = 0; i_types < types.Length; i_types++)
            {
                var str_type = types[i_types];

                Match m_isArray = reg_array.Match(str_type);
                if (m_isArray.Groups.Count > 1)
                {
                    str_type = reg_array.Replace(str_type, "");
                    isArray[i_types] = m_isArray.Groups[1].Value;
                    if (isArray[i_types] == "")
                        isArray[i_types] = defaultArraySpliter;
                }


                if (codeTemplate != null && !codeTemplate.HasParamVO(str_type.ToLower()))
                    enumNames[i_types] = firstCharToUp(str_type);

                str_type = str_type.ToLower();
                types[i_types] = str_type;
            }

            ///去掉表头
            for (int j = 0; j < dataRowStartNum; j++)
            {
                dt.Rows.RemoveAt(0);
            }


            ExcelTable sheet = new ExcelTable();

            ///查找主键并赋值
            i = 0;
            while (i < header.Length)
            {
                String head = header[i].ToString();

                dt.Columns[i].ColumnName = head;

                if (isPrefix(head, prefix_primaryKey))
                {
                    head = head.Substring(prefix_primaryKey.Length);
                    sheet.primaryKeyIndex = i;
                    header[i] = head;
                    break;
                }
                i++;
            }

            sheet.name = sheetName;
            sheet.header = (String[])header;
            sheet.relations = relations;
            sheet.comments = comments;
            sheet.types = types;
            sheet.enumNames = enumNames;
            sheet.isArray = isArray;

            ///清除主键未赋值的行，以及主键查重
            var hs_primaryValue = new HashSet<string>();
            i = dt.Rows.Count - 1;
            while (i >= 0)
            {
                var primaryValue = dt.Rows[i][sheet.primaryKeyIndex].ToString();

                if (string.IsNullOrEmpty(primaryValue.Trim()))
                {
                    dt.Rows.RemoveAt(i);
                    i--;
                    continue;
                }
                else if (hs_primaryValue.Contains(primaryValue))
                {
                    throw new Exception(string.Format("{0}表有重复主键{1}:{2}", sheet.name, sheet.header[sheet.primaryKeyIndex], primaryValue));
                }
                hs_primaryValue.Add(primaryValue);
                i--;
            }

            sheet.dataTable = dt;

            return sheet;

        }

        static private string firstCharToUp(string str)
        {
            var firstChar = str.Substring(0, 1);
            str = firstChar.ToUpper() + str.Substring(1);
            return str;
        }

        static private HashSet<string> hs_sheetNames = new HashSet<string>();

        static private List<DataTable> getTables(string path, string prefix_IgnoreSheet)
        {
            var sheets = new List<DataTable>();

            using (ExcelPackage package = new ExcelPackage(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                for (int i = 1; i <= package.Workbook.Worksheets.Count; ++i)
                {
                    ExcelWorksheet sheet = package.Workbook.Worksheets[i];
                    //有些带筛选区域的表会导致出现多余的表 类似   "表名$_xlnm#_FilterDatabase"
                    if (sheet.Name.IndexOf("FilterDatabase") >= 0)
                        continue;

                    //忽略表
                    if (isPrefix(sheet.Name, prefix_IgnoreSheet))
                        continue;

                    if (!mergeSheets)
                    {
                        if (hs_sheetNames.Contains(sheet.Name))
                            throw new Exception(string.Format("有相同的表名:{0}", sheet.Name));
                        hs_sheetNames.Add(sheet.Name);
                    }


                    var table = new DataTable(sheet.Name);
                    for (int c = sheet.Dimension.Start.Column, k = sheet.Dimension.End.Column; c <= k; c++)
                    {
                        table.Columns.Add();
                    }


                    for (int r = 1, n = sheet.Dimension.End.Row; r <= n; r++)
                    {

                        var row = table.NewRow();
                        for (int c = 1, k = sheet.Dimension.End.Column; c <= k; c++)
                        {
                            var cell = sheet.Cells[r, c];
                            try
                            {
                                row[c - 1] = cell.Text;
                            }
                            catch (Exception e)
                            {
                                //Console.WriteLine(e);
                                //row[c - 1] = cell.Value.ToString();
                                var msg = $@"读取单元格数据异常
表:{sheet.Name},行:{r},列:{c}

{e}";
                                throw new Exception(msg, e);
                            }
                        }
                        table.Rows.Add(row);
                    }

                    sheets.Add(table);

                }
                return sheets;
            }
        }

        /// <summary>
        /// 是否包含前缀
        /// </summary>
        /// <param name="str"></param>
        /// <param name="refix"></param>
        /// <returns></returns>
        static private Boolean isPrefix(String str, String prefix)
        {
            if (null == prefix) return false;
            prefix = prefix.Trim();
            if (prefix == "") return false;
            return str.IndexOf(prefix) == 0;
        }

    }

    public class ExcelCodeTemplate
    {

        private string logo = "";

        private XElement config;
        private const string mark_member = "$(member)";
        private const string mark_className = "$(className)";
        private const string mark_comment = "$(comment)";

        private string classExtension;

        public string ClassExtension
        {
            get { return classExtension; }
        }

        private string template_class;
        private string template_definitionMember;
        private string template_memberDecode;
        private string template_relationMember;
        private string template_definitionArray;
        private string template_arrayDecode;

        public XElement element_SingleProtocolFile;
        public XElement element_ProtocolEnumClass;
        public XElement element_MessageRegisterClass;

        private XElement xml_template;

        private Dictionary<string, ParamVO> dic_param = new Dictionary<string, ParamVO>();

        public void load(XElement xml_template)
        {
            this.xml_template = xml_template;

            config = xml_template.Element("config");
            classExtension = getConfig("classExtension");
            if (classExtension.IndexOf(".") < 0)
                classExtension = "." + classExtension;

            template_class = xml_template.Element("Class").Value;

            var language = config.Attribute("language");
            if (language == null)
                throw new Exception("代码模板config缺少language属性");
            switch (language.Value.ToLower())
            {
                case "lua":
                    logo = Properties.Resources.logo_excel_lua;
                    break;

                default:
                    logo = Properties.Resources.logo_excel_CSharp;
                    break;
            }
            template_class = logo + template_class;
            template_definitionMember = xml_template.Element("definitionMember").Value;
            template_memberDecode = xml_template.Element("decodeMember").Value;
            template_relationMember = xml_template.Element("relationMember").Value;
            template_definitionArray = xml_template.Element("definitionArray").Value;
            template_arrayDecode = xml_template.Element("decodeArray").Value;

            var list_type = xml_template.Element("params").Elements("param");
            foreach (var item in list_type)
            {
                var paramVO = new ParamVO();
                paramVO.paramType = item.Attribute("type").Value.Trim().ToLower();
                paramVO.template_decode = item.Element("decode").Value;
                paramVO.isEnum = paramVO.paramType == "enum";
                if (paramVO.isEnum == false)
                    paramVO.className = item.Attribute("class").Value.Trim();

                dic_param[paramVO.paramType] = paramVO;
            }

        }

        public void load(string templatePath)
        {
            Console.WriteLine($@"加载代码模板
{templatePath}
");
            load(XElement.Load(templatePath));
        }

        public string getConfig(string name)
        {
            var item = config.Element(name);
            if (item != null)
                return item.Value;
            return "";
        }

        public string getTypeClassName(string paramType)
        {
            var paramVO = getParamVO(paramType);
            return paramVO.className;
        }

        private string getDecode(string paramType)
        {
            var paramVO = getParamVO(paramType);
            var result = paramVO.template_decode;
            return result.Trim();
        }

        public string getDecode(string paramType, string member, bool isArray, string enumName)
        {
            var paramVO = getParamVO(paramType);
            var structClassName = paramVO.className;
            if (paramVO.isEnum)
                structClassName = enumName;

            var decode = getDecode(paramType);
            string result;
            if (isArray)
                result = template_arrayDecode;
            else
                result = template_memberDecode;

            result = result.Replace("$(decode)", decode);
            result = result.Replace(mark_member, member);
            result = result.Replace(mark_className, structClassName);
            result = result.Replace("$(enumName)", enumName);
            return result;
        }

        public string getDefinition(string typeClassName, string member, string comment)
        {
            var result = template_definitionMember.Replace(mark_className, typeClassName);
            result = result.Replace(mark_member, member);
            result = result.Replace(mark_comment, comment);
            return result;
        }

        public string getArrayDefinition(string className, string member, string comment)
        {
            var result = template_definitionArray.Replace(mark_className, className);
            result = result.Replace(mark_member, member);
            result = result.Replace(mark_comment, comment);
            return result;
        }

        public string getRelationMember(string className, string member)
        {
            var result = template_relationMember.Replace(mark_className, className);
            result = result.Replace(mark_member, member);
            return result;
        }

        public string getClassText(string tableName, string className, string definition, string decode, string primaryKeyType, string primaryKeyName, string comment = "")
        {
            var text = template_class;
            text = text.Replace("$(tableName)", tableName);
            text = text.Replace(mark_className, className);
            text = text.Replace("$(definition)", definition);
            text = text.Replace("$(decode)", decode);
            text = text.Replace("$(primaryKeyType)", getTypeClassName(primaryKeyType));
            text = text.Replace("$(primaryKeyName)", primaryKeyName);

            text = text.Replace(mark_comment, comment);
            text = text.Replace("\n", "\r\n");
            text = text.Replace("\r\r\n", "\r\n");
            return text;
        }

        public string getFinalMemberName(string member)
        {
            if (getConfig("memberStartUpperCase") == "true")
                member = firstCharToUp(member);
            return member;
        }

        public string getFinalClassName(string className)
        {
            if (getConfig("classStartUpperCase") == "true")
                className = firstCharToUp(className);
            return getConfig("classNamePrefix") + className + getConfig("classNameTail");
        }

        static public string firstCharToUp(string str)
        {
            var firstChar = str.Substring(0, 1);
            str = firstChar.ToUpper() + str.Substring(1);
            return str;
        }

        static private List<string> list_className = new List<string>();
        static public void AddClassName(string className)
        {
            list_className.Add(className);
        }

        public bool HasInitClass { get { return xml_template.Element("InitClass") != null; } }

        public string GetInitClassText()
        {
            var InitClass = logo + xml_template.Element("InitClass").Value;
            var InitClassItem = xml_template.Element("InitClassItem").Value;
            var str_item = "";
            for (int i = 0; i < list_className.Count; i++)
            {
                var item = InitClassItem.Replace("$(className)", list_className[i]);
                str_item += item;
            }
            return InitClass.Replace("$(initCode)", str_item).Replace("$(initCount)", list_className.Count.ToString()).Trim();
        }

        public string GetInitClassFileName()
        {
            var str = xml_template.Element("InitClass").Attribute("fileName").Value + ClassExtension;
            return str;
        }

        private ParamVO getParamVO(string paramType)
        {
            paramType = paramType.ToLower();
            if (dic_param.ContainsKey(paramType))
                return dic_param[paramType];
            //throw new Exception("不支持的paramType:" + paramType);
            return dic_param["enum"];
        }

        public bool HasParamVO(string paramType)
        {
            return dic_param.ContainsKey(paramType);
        }

        private class ParamVO
        {
            public bool isEnum;
            public string paramType;
            public string className;
            public string template_decode;
        }

    }

    public class ExcelTable
    {
        /// <summary>
        /// 工作薄显示名
        /// </summary>
        public String name;

        /// <summary>
        /// 字段名列表
        /// </summary>
        public String[] header;
        public String[] relations;
        public String[] comments;
        public String[] types;
        public String[] enumNames;
        public String[] isArray;

        public int primaryKeyIndex;


        /// <summary>
        /// 数据表对象
        /// </summary>
        public DataTable dataTable;

        public delegate void ExcelEncoder(EndianBinaryWriter binWriter, string type, string value);
        static private ExcelEncoder encoder;

        public byte[] ToBytes(Endian endian)
        {

            var ms = new MemoryStream();
            var binWriter = new EndianBinaryWriter(endian, ms);
            binWriter.Write(endian == Endian.LittleEndian);


            var jumpPos = binWriter.BaseStream.Position;
            binWriter.Write((int)-1);

            binWriter.Write(header.Length);

            for (var i = 0; i < header.Length; i++)
            {
                binWriter.WriteUTF(ExcelCodeTemplate.firstCharToUp(header[i]));
                var str_type = types[i];
                if (enumNames[i] != null)
                    str_type = $@"enum";
                binWriter.WriteUTF(str_type);
                binWriter.Write(isArray[i] != null);
            }


            var nowPos = binWriter.BaseStream.Position;

            binWriter.BaseStream.Position = jumpPos;
            binWriter.Write((int)nowPos);
            binWriter.BaseStream.Position = nowPos;

            int rowCount = (int)dataTable.Rows.Count;

            if (ExcelGenerater.IsInvalid && rowCount > 1)
            {
                rowCount -= rowCount / 4;
            }

            binWriter.Write(rowCount);

            for (int i = 0; i < rowCount; i++)
            {
                var row = dataTable.Rows[i];
                for (int j = 0; j < header.Length; j++)
                {
                    var type = types[j];
                    var fieldName = header[j];
                    var isEnum = enumNames[j] != null;
                    if (isEnum)
                        type = "enum";

                    var pos = 0l;
                    var needWriteCellLen = false;
                    if (ExcelGenerater.writeCellLen)
                    {
                        needWriteCellLen = !ExcelGenerater.writeCellLenExclude.Contains(type);
                    }

                    if (needWriteCellLen)
                    {
                        binWriter.BaseStream.Seek(2, SeekOrigin.Current);
                        pos = binWriter.BaseStream.Position;
                    }

                    if (isArray[j] != null)
                    {
                        var list_arrayValue = row[j].ToString().Split(new string[] { isArray[j] }, StringSplitOptions.RemoveEmptyEntries);

                        binWriter.Write(list_arrayValue.Length);
                        for (int k = 0; k < list_arrayValue.Length; k++)
                        {
                            EncodeCell(binWriter, fieldName, type, list_arrayValue[k]);
                        }
                    }
                    else
                        EncodeCell(binWriter, fieldName, type, row[j].ToString());

                    if (needWriteCellLen)
                    {
                        var len = (ushort)(binWriter.BaseStream.Position - pos);
                        binWriter.BaseStream.Seek(pos - 2, SeekOrigin.Begin);
                        binWriter.Write(len);
                        binWriter.BaseStream.Seek(len, SeekOrigin.Current);
                    }
                }
            }

            return ms.ToArray();
        }

        public void SetEncoder(ExcelEncoder customerEncoder)
        {
            if (customerEncoder == null)
                encoder = Encode;
            else
                encoder = customerEncoder;
        }

        public void EncodeCell(EndianBinaryWriter binWriter, string fieldName, string type, string value)
        {
            try
            {
                encoder(binWriter, type, value);
            }
            catch (Exception e)
            {
                throw new Exception($@"编码单元格数据异常:{e.Message}

字段名:{fieldName},类型:{type},值:{value}");
            }

        }

        static public void Encode(EndianBinaryWriter binWriter, string type, string value)
        {
            //binWriter.WriteUTF(value);
            //return;

            switch (type)
            {
                case "int":
                case "enum":
                    if (value.Trim() == "")
                        binWriter.Write(0);
                    else
                        binWriter.Write(Convert.ToInt32(value));
                    break;

                case "bool":
                    var str_bool = value.Trim();
                    binWriter.Write(str_bool.ToLower() == "true" || str_bool == "1");
                    break;

                case "float":
                    if (value.Trim() == "")
                        binWriter.Write(0f);
                    else
                        binWriter.Write(Convert.ToSingle(value));
                    break;

                case "date":

                    if (value.Trim() == "")
                        binWriter.WriteDate(new DateTime());
                    else
                    {
                        var str_date = value;

                        /*
                        var reg_date = new Regex(@"(\d{4})/(\d{1,2})/(\d{1,2})");
                        var reg_time = new Regex(@"(\d{1,2}):(\d{1,2}):(\d{1,2})");

                        Match m_date = reg_date.Match(str_date);
                        Match m_time = reg_time.Match(str_date);

                        if (m_date.Groups.Count < 3 && m_time.Groups.Count < 3)
                            throw new Exception("无效的date格式:" + str_date);

                        var date = new DateTime(
                            Convert.ToInt32(m_date.Groups[1].Value),
                            Convert.ToInt32(m_date.Groups[2].Value),
                            Convert.ToInt32(m_date.Groups[3].Value),
                            Convert.ToInt32(m_time.Groups[1].Value),
                            Convert.ToInt32(m_time.Groups[2].Value),
                            Convert.ToInt32(m_time.Groups[3].Value)
                            );
                         binWriter.WriteDate(date);
                         */

                        try
                        {
                            var date = DateTime.Parse(str_date);
                            binWriter.WriteDate(date);
                        }
                        catch
                        {
                            throw new Exception("无效的date格式:" + str_date);
                        }
                    }

                    break;

                default:
                    binWriter.WriteUTF(value);
                    break;
            }
        }

        public byte[] ToJson()
        {
            var jsonList = new List<Dictionary<string, object>>();

            int rowCount = (int)dataTable.Rows.Count;

            if (ExcelGenerater.IsInvalid && rowCount > 1)
            {
                rowCount -= rowCount / 4;
            }

            for (int i = 0; i < rowCount; i++)
            {
                var jsonDic = new Dictionary<string, object>();
                jsonList.Add(jsonDic);
                var row = dataTable.Rows[i];
                for (int j = 0; j < header.Length; j++)
                {
                    var type = types[j];
                    var fieldName = header[j];
                    var isEnum = enumNames[j] != null;
                    if (isEnum)
                        type = "enum";
                    //??是否需要
                    //if (isEnum)
                    //    types[j] = "int";

                    if (isArray[j] != null)
                    {
                        var list_arrayValue = row[j].ToString().Split(new string[] { isArray[j] }, StringSplitOptions.RemoveEmptyEntries);
                        var arr = new List<object>();
                        for (int k = 0; k < list_arrayValue.Length; k++)
                        {
                            arr.Add(EncodeJsonCell(fieldName, type, list_arrayValue[k]));
                        }
                        jsonDic.Add(fieldName, arr);
                    }
                    else
                        jsonDic.Add(fieldName, EncodeJsonCell(fieldName, type, row[j].ToString()));
                }
            }

            var headInfo = new List<object>();
            for (var i = 0; i < header.Length; i++)
            {
                headInfo.Add(new
                {
                    name = header[i],
                    type = types[i],
                    isArray = isArray[i] != null,
                }
            );
            }

            var infoJson = new Dictionary<string, object>();
            infoJson.Add("primaryKey", header[primaryKeyIndex]);
            infoJson.Add("heads", headInfo);
            infoJson.Add("count", rowCount);

            var finalDic = new Dictionary<string, object>();
            finalDic.Add("headerInfo", infoJson);
            finalDic.Add("data", jsonList);

            var json = JsonConvert.SerializeObject(finalDic, Formatting.Indented);

            return Encoding.UTF8.GetBytes(json);
        }

        static public object EncodeJsonCell(string fieldName, string type, string value)
        {
            try
            {
                switch (type)
                {
                    case "int":
                    case "enum":
                        if (value.Trim() == "")
                            return 0;
                        else
                            return Convert.ToInt32(value);

                    case "bool":
                        var str_bool = value.Trim();
                        return (str_bool.ToLower() == "true" || str_bool == "1");

                    case "float":
                    case "fp":
                        if (value.Trim() == "")
                            return (0f);
                        else
                            return (Convert.ToSingle(value));
                        break;

                    case "date":

                        if (value.Trim() == "")
                            return (new DateTime());
                        else
                        {
                            var str_date = value;

                            /*
                            var reg_date = new Regex(@"(\d{4})/(\d{1,2})/(\d{1,2})");
                            var reg_time = new Regex(@"(\d{1,2}):(\d{1,2}):(\d{1,2})");

                            Match m_date = reg_date.Match(str_date);
                            Match m_time = reg_time.Match(str_date);

                            if (m_date.Groups.Count < 3 && m_time.Groups.Count < 3)
                                throw new Exception("无效的date格式:" + str_date);

                            var date = new DateTime(
                                Convert.ToInt32(m_date.Groups[1].Value),
                                Convert.ToInt32(m_date.Groups[2].Value),
                                Convert.ToInt32(m_date.Groups[3].Value),
                                Convert.ToInt32(m_time.Groups[1].Value),
                                Convert.ToInt32(m_time.Groups[2].Value),
                                Convert.ToInt32(m_time.Groups[3].Value)
                                );
                             binWriter.WriteDate(date);
                             */

                            try
                            {
                                var date = DateTime.Parse(str_date);
                                return (date);
                            }
                            catch
                            {
                                throw new Exception("无效的date格式:" + str_date);
                            }
                        }

                        break;

                    default:
                        return (value);
                        break;
                }
            }
            catch (Exception e)
            {
                throw new Exception($@"编码单元格数据异常:{e.Message}

字段名:{fieldName},类型:{type},值:{value}");
            }

        }

    }


}
