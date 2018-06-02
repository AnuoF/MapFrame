/*****************************************************
** 文件名：XmlHelper.cs
** 版 本：1.0
** 内容简述：xml与object相互转化类
** 创建日期：Feb 13,2014
** 创建人：Allen 
** 修改记录：
*****************************************************/

using System;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace MapFrame.Core.Common
{
    /// <summary>
    /// Xml文档操作类
    /// </summary>
    public static class XmlHelper
    {
        private static void XmlSerializeInternal(Stream stream, object o, Encoding encoding)
        {
            try
            {
                if (o == null)
                    throw new ArgumentNullException("o");
                if (encoding == null)
                    throw new ArgumentNullException("encoding");

                XmlSerializer serializer = new XmlSerializer(o.GetType());

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.NewLineChars = "\r\n";
                settings.Encoding = encoding;
                settings.IndentChars = "    ";

                //不生成声明头
                //settings.OmitXmlDeclaration = true;
                //强制指定命名空间，覆盖默认命名空间
                XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                namespaces.Add(string.Empty, string.Empty);

                using (XmlWriter writer = XmlWriter.Create(stream, settings))
                {
                    
                    serializer.Serialize(writer, o, namespaces);
                    writer.Close();
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// 将一个对象序列化为XML字符串
        /// </summary>
        /// <param name="o">要序列化的对象</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>序列化产生的XML字符串</returns>
        public static string XmlSerialize(object o, Encoding encoding)
        {
            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    XmlSerializeInternal(stream, o, encoding);

                    stream.Position = 0;
                    using (StreamReader reader = new StreamReader(stream, encoding))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 将一个对象按XML序列化的方式写入到一个文件
        /// </summary>
        /// <param name="o">要序列化的对象</param>
        /// <param name="path">保存文件路径</param>
        /// <param name="encoding">编码方式</param>
        public static void XmlSerializeToFile(object o, string path, Encoding encoding)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                    throw new ArgumentNullException("path");
                
                using (FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    XmlSerializeInternal(file, o, encoding);
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// 从XML字符串中反序列化对象
        /// </summary>
        /// <typeparam name="T">结果对象类型</typeparam>
        /// <param name="s">包含对象的XML字符串</param>
        /// <param name="encoding">编码方式</param>
        /// <param name="isHaveUnKnow">是否存在未知元素</param>
        /// <returns>反序列化得到的对象</returns>
        public static T XmlDeserialize<T>(string s, Encoding encoding, out bool isHaveUnKnow)
        {
            return new XmlHelperThreadSafety().XmlDeserialize<T>(s, encoding, out isHaveUnKnow);
        }

        /// <summary>
        /// 读入一个文件，并按XML的方式反序列化对象。
        /// </summary>
        /// <typeparam name="T">结果对象类型</typeparam>
        /// <param name="path">文件路径</param>
        /// <param name="encoding">编码方式</param>
        /// <param name="isHaveUnKnow">是否存在未知元素</param>
        /// <returns>反序列化得到的对象</returns>
        public static T XmlDeserializeFromFile<T>(string path, Encoding encoding, out bool isHaveUnKnow)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            if (encoding == null)
                throw new ArgumentNullException("encoding");

            string xml = File.ReadAllText(path, encoding);
            return XmlDeserialize<T>(xml, encoding, out  isHaveUnKnow);
        }

        /// <summary>
        /// 读入一个文件，并按XML的方式反序列化对象。
        /// </summary>
        /// <typeparam name="T">结果对象类型</typeparam>
        /// <param name="path">文件路径</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>反序列化得到的对象</returns>
        public static T XmlDeserializeFromFile<T>(string path, Encoding encoding)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            bool isHaveUnKnow = false;
            string xml = File.ReadAllText(path, encoding);

            return XmlDeserialize<T>(xml, encoding, out  isHaveUnKnow);
        }
    }
}
