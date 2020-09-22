
/// <summary>
/// 序列化工具集
/// </summary>
namespace UtilityToolsCollect.UtilityToolsCollect.SerializeToolUtility
{
    /// <summary>
    ///  Xml序列化工具集
    /// </summary>
    public static class XmlSerialize
    {
        /// <summary>
        /// 直接从网络或本地获取xml文件执行反序列化并返回对象，或者把一个对象序列化并保存一个xml文件到本地
        /// </summary>
        public static class FileAndPathTool
        {
            /// <summary>
            /// 通过路径（包括网络地址）获取xml数据，并反序列化到对象
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="Path"></param>
            /// <returns></returns>
            public static T PathToXmlDeserialize<T>(string Path)
            {
                using (System.Xml.XmlReader xr = System.Xml.XmlReader.Create(Path))
                {
                    System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                    return (T)serializer.Deserialize(xr);
                }
            }
            /// <summary>
            /// 序列化到Xml文件
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="Path"></param>
            /// <param name="t"></param>
            public static void XmlSerializerToXmlFile<T>(string Path, T t, bool EnableIndent = false)
            {
                using (System.Xml.XmlWriter xw = System.Xml.XmlWriter.Create(Path, 
                    new System.Xml.XmlWriterSettings { Encoding = System.Text.Encoding.UTF8, CloseOutput = true, Indent = EnableIndent }))
                {
                    System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(T));
                    xs.Serialize(xw, t);
                }
            }
        }
        /// <summary>
        /// 从byte[]反序列化到类对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="b">Xmlbyte[]</param>
        /// <param name="usedXmlReader">是否使用XmlReader进行更准确的反序列化</param>
        /// <returns><T></returns>
        public static T XmlDeserialize<T>(byte[] b, bool usedXmlReader = false)
        {
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(b))
            {
                return XmlDeserialize<T>(ms, usedXmlReader);
            }
        }
        /// <summary>
        /// 从字符串反序列化到类对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str">Xml字符串</param>
        /// <param name="usedXmlReader">是否使用XmlReader进行更准确的反序列化</param>
        /// <returns><T></returns>
        public static T XmlDeserialize<T>(string str, bool usedXmlReader = false)
        {
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(str)))
            {
                return XmlDeserialize<T>(ms, usedXmlReader);
            }
        }
        /// <summary>
        /// 从xml内存流反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ms"></param>
        /// <param name="usedXmlReader"></param>
        /// <returns></returns>
        public static T XmlDeserialize<T>(System.IO.MemoryStream ms, bool usedXmlReader = false)
        {
            if (usedXmlReader)
                using (System.Xml.XmlReader xr = System.Xml.XmlReader.Create(ms))
                {
                    System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                    return (T)serializer.Deserialize(xr);
                }
            else
            {
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(ms);
            }
        }
        /// <summary>
        /// xml序列化到MemoryStream
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"><T></param>
        /// <returns>MemoryStream</returns>
        public static System.IO.MemoryStream XmlSerializerToMemoryStream<T>(T t, bool SimpleConfiguration = true, bool EnableIndent = false)
        {
            if (EnableIndent) SimpleConfiguration = true;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                if (SimpleConfiguration)
                    serializer.Serialize(ms, t);
                else
                    using (System.Xml.XmlWriter xw = System.Xml.XmlWriter.Create(ms,
                        new System.Xml.XmlWriterSettings { Encoding = System.Text.Encoding.UTF8, CloseOutput = true, Indent = EnableIndent }))
                        serializer.Serialize(xw, t);
                return ms;
            }
        }
        /// <summary>
        /// 序列化到Byte数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"><T></param>
        /// <returns>Byte[]</returns>
        public static byte[] XmlSerializerToBytes<T>(T t, bool SimpleConfiguration = true, bool EnableIndent = false)
        {
            return XmlSerializerToMemoryStream(t, SimpleConfiguration,EnableIndent).ToArray();
        }
        /// <summary>
        /// 序列化到字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"><T></param>
        /// <returns>String</returns>
        public static string XmlSerializerToString<T>(T t, bool SimpleConfiguration = true, bool EnableIndent = false, bool EnableOmitXmlDeclaration = false, bool EnableRemoveNamespaces = false)
        {
            if (EnableIndent || EnableOmitXmlDeclaration || EnableRemoveNamespaces) SimpleConfiguration = false;
            if (SimpleConfiguration)
                return System.Text.Encoding.UTF8.GetString(XmlSerializerToMemoryStream(t).ToArray());

            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            System.Text.StringBuilder Result = new System.Text.StringBuilder();
            using (System.Xml.XmlWriter xw = System.Xml.XmlWriter.Create(Result,
                        new System.Xml.XmlWriterSettings { Encoding = System.Text.Encoding.UTF8, CloseOutput = true, Indent = EnableIndent , OmitXmlDeclaration= EnableOmitXmlDeclaration}))
                if (EnableRemoveNamespaces)
                {
                    System.Xml.Serialization.XmlSerializerNamespaces namespaces = new System.Xml.Serialization.XmlSerializerNamespaces();
                    namespaces.Add(string.Empty, string.Empty);
                    serializer.Serialize(xw, t, namespaces);
                }
                else
                    serializer.Serialize(xw, t);
            return Result.ToString();
        }
    }
}
